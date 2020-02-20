using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Moetech.Zhuangzhou.Data;
using Moetech.Zhuangzhou.Models;
using Moetech.Zhuangzhou.Return;

namespace Moetech.Zhuangzhou.Controllers
{
    /// <summary>
    /// 管理虚拟机
    /// 控制器
    /// </summary>
    public class ManageController : FilterController
    {
        /// <summary>
        /// 数据量上下文
        /// </summary>
        private readonly VirtualMachineDB _db;

        /// <summary>
        /// 每页条数
        /// </summary>
        private readonly int pageSize = 10;

        /// <summary>
        /// 角色 
        /// </summary>
        public override int Role { get; set; } = 1;

        public ManageController(VirtualMachineDB context)
        {
            _db = context;
        }

        /// <summary>
        /// GET
        /// Manage/Index
        /// 管理虚拟机主页
        /// </summary>
        /// <param name="name">申请人员</param>
        /// <param name="type">操作系统</param>
        /// <param name="status">虚拟机状态</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string name = "", int? type = -1, int? status = -1, int? pageIndex = 1)
        {
            var list = from m1 in _db.MachineInfo
                       join m2 in (from m3 in _db.MachApplyAndReturn
                                   join cp in _db.CommonPersonnelInfo on m3.ApplyUserID equals cp.PersonnelId
                                   where m3.OprationType == 0 && m3.ExamineResult != 1
                                   select new ReturnMachineInfoApplyData
                                   {
                                       MachApplyAndReturn = m3,
                                       CommonPersonnelInfo = cp
                                   }) on m1.MachineId equals m2.MachApplyAndReturn.MachineInfoID
                       into re
                       from r in re.DefaultIfEmpty()
                       select new ReturnMachineInfoApplyData
                       {
                           MachineInfo = m1,
                           MachApplyAndReturn = r.MachApplyAndReturn,
                           CommonPersonnelInfo = r.CommonPersonnelInfo
                       };

            // 操作系统类型
            if (type != -1)
            {
                list = list.Where(o => o.MachineInfo.MachineSystem == type);
            }

            // 虚拟机状态
            if (status != -1)
            {
                list = list.Where(o => o.MachineInfo.MachineState == status);
            }

            // 申请人员
            if (!string.IsNullOrWhiteSpace(name))
            {
                list = list.Where(o => o.CommonPersonnelInfo.PersonnelName.Contains(name));
            }

            var PagingList = await PaginatedList<ReturnMachineInfoApplyData>.CreateAsync(list.AsNoTracking(), pageIndex ?? 1, pageSize);
            // 查询条件参数
            ViewBag.name = name;
            ViewBag.type = type;
            ViewBag.status = status;
            return View(PagingList);
        }

        /// <summary>
        /// GET
        /// Manage/Approve
        /// 审批虚拟机
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Approve(int? pageIndex = 1)
        {
            var list = from m1 in _db.MachineInfo
                       join m2 in _db.MachApplyAndReturn on m1.MachineId equals m2.MachineInfoID
                       join p3 in _db.CommonPersonnelInfo on m2.ApplyUserID equals p3.PersonnelId
                       where m2.OprationType == 0 && m2.ExamineUserID == -1 && m2.ExamineResult == 0
                       select new ReturnMachineInfoApplyData
                       {
                           MachineInfo = m1,
                           MachApplyAndReturn = m2,
                           CommonPersonnelInfo = p3
                       };

            var PagingList = await PaginatedList<ReturnMachineInfoApplyData>.CreateAsync(list.AsNoTracking(), pageIndex ?? 1, pageSize);

            return View(PagingList);
        }

        /// <summary>
        /// POST
        /// Manage/SubmitApprove
        /// 提交审批
        /// </summary>
        /// <param name="mid">虚拟机信息ID</param>
        /// <param name="aid">申请归还信息ID</param>
        /// <param name="state">状态1：拒绝  2：同意</param>
        /// <returns></returns>
        //[HttpPost]
        public async Task<IActionResult> SubmitApprove(int mid, int aid, int state)
        {
            // 当前用户信息
            CommonPersonnelInfo userInfo = JsonConvert.DeserializeObject<CommonPersonnelInfo>(HttpContext.Session.GetString("User"));

            int adminID = userInfo.PersonnelId;

            var list = from m1 in _db.MachApplyAndReturn
                       join m2 in _db.MachineInfo on m1.MachineInfoID equals m2.MachineId
                       where m1.MachineInfoID == mid && m1.ApplyAndReturnId == aid
                       select new ReturnMachineInfoApplyData
                       {
                           MachApplyAndReturn = m1,
                           MachineInfo = m2
                       };

            if (list.Count() == 0)
            {
                return NotFound("数据非法，操作终止!");
            }

            foreach (var item in list)
            {
                item.MachApplyAndReturn.ExamineUserID = adminID;        // 修改审批人员ID
                item.MachApplyAndReturn.ExamineResult = state;          // 修改审批结果
                item.MachineInfo.MachineState = state == 2 ? 2 : 0;     // 修改虚拟机状态

                _db.MachApplyAndReturn.Update(item.MachApplyAndReturn);
                _db.MachineInfo.Update(item.MachineInfo);
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Approve));
        }


        /// <summary>
        /// 回收
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Recycle(int mid, int rid)
        {
            var list = from m1 in _db.MachineInfo
                       join m2 in _db.MachApplyAndReturn on m1.MachineId equals m2.MachineInfoID
                       where m1.MachineId == mid && m2.ApplyAndReturnId == rid
                       select new
                       {
                           MachineInfo = m1,
                           MachApplyAndReturn = m2
                       };

            if (list.Count() == 0)
            {
                ViewData["Title"] = "操作失败";
                ViewData["Message"] = "数据非法，操作终止！";
                return View("Views/Vmware/Error.cshtml");
            }

            foreach (var item in list)
            {
                if (item.MachineInfo.MachineState == 2)
                {
                    item.MachineInfo.MachineState = 0;
                    item.MachApplyAndReturn.OprationType = 1;
                    item.MachApplyAndReturn.ResultTime = DateTime.Now;

                    _db.MachineInfo.Update(item.MachineInfo);
                    _db.MachApplyAndReturn.Update(item.MachApplyAndReturn);

                }
                else
                {
                    ViewData["Title"] = "回收失败";
                    ViewData["Message"] = $"回收失败，虚拟机未在使用！";
                    return View("Views/Vmware/Error.cshtml");
                }
            }

            await _db.SaveChangesAsync();

            ViewData["Title"] = "回收成功";
            ViewData["Message"] = "回收成功";
            return View("Views/Manage/Succeed.cshtml");
        }

        // GET: MachineInfoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machineInfo = await _db.MachineInfo
                .FirstOrDefaultAsync(m => m.MachineId == id);
            if (machineInfo == null)
            {
                return NotFound();
            }

            return View(machineInfo);
        }

        // GET: MachineInfoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MachineInfoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MachineId,MachineIP,MachineSystem,MachineDiskCount,MachineMemory,MachineState,MachineUser,MachinePassword")] MachineInfo machineInfo)
        {
            if (ModelState.IsValid)
            {
                _db.Add(machineInfo);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(machineInfo);
        }

        // GET: MachineInfoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machineInfo = await _db.MachineInfo.FindAsync(id);
            if (machineInfo == null)
            {
                return NotFound();
            }
            return View(machineInfo);
        }

        // POST: MachineInfoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MachineId,MachineIP,MachineSystem,MachineDiskCount,MachineMemory,MachineState,MachineUser,MachinePassword")] MachineInfo machineInfo)
        {
            if (id != machineInfo.MachineId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(machineInfo);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MachineInfoExists(machineInfo.MachineId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(machineInfo);
        }

        // GET: MachineInfoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machineInfo = await _db.MachineInfo
                .FirstOrDefaultAsync(m => m.MachineId == id);
            if (machineInfo == null)
            {
                return NotFound();
            }

            return View(machineInfo);
        }

        // POST: MachineInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var machineInfo = await _db.MachineInfo.FindAsync(id);
            _db.MachineInfo.Remove(machineInfo);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MachineInfoExists(int id)
        {
            return _db.MachineInfo.Any(e => e.MachineId == id);
        }
    }
}