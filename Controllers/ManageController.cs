using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Moetech.Zhuangzhou.Models;
using Moetech.Zhuangzhou.Interface;

namespace Moetech.Zhuangzhou.Controllers
{
    /// <summary>
    /// 管理虚拟机
    /// 控制器
    /// </summary>
    public class ManageController : FilterController
    {
        /// <summary>
        /// 虚拟机管理接口
        /// </summary>
        private readonly IVmwareManage _vmwareManage;

        /// <summary>
        /// 角色 
        /// </summary>
        public override int[] Role { get; set; } = { 1 };

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="vmwareManage">虚拟机接口</param>
        public ManageController(IVmwareManage vmwareManage)
        {
            _vmwareManage = vmwareManage;
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
            var list = await _vmwareManage.SelectAll(name, type, status, pageIndex);

            // 查询条件参数
            ViewBag.name = name;
            ViewBag.type = type;
            ViewBag.status = status;
            return View(list);
        }

        /// <summary>
        /// GET
        /// Manage/Approve
        /// 审批虚拟机
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        public async Task<IActionResult> Approve(int? pageIndex = 1)
        {
            var list = await _vmwareManage.SelectApprove(pageIndex ?? 1);

            return View(list);
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
        public async Task<IActionResult> SubmitApprove(int ApplyUserID, DateTime ApplyTime,
            DateTime ResultTime, string Remark, int state)
        {
            // 当前用户信息
            CommonPersonnelInfo userInfo = JsonConvert.DeserializeObject<CommonPersonnelInfo>(HttpContext.Session.GetString("User"));
            int _userId = userInfo.PersonnelId;
            var result = await _vmwareManage.ResultSubmitApprove(ApplyUserID, ApplyTime, ResultTime, Remark, state, _userId);
            if (!result)
            {
                ViewData["Title"] = "操作失败";
                ViewData["Message"] = "数据非法，操作终止！";
                return View("Views/Shared/Tip.cshtml");
            }
            else
            {
                return RedirectToAction(nameof(Approve));
            }

        }

        /// <summary>
        /// GET
        /// Manage/Recycle
        /// 回收虚拟机
        /// </summary>
        /// <param name="mid">虚拟机信息ID</param>
        /// <param name="rid">申请虚拟机记录ID</param>
        /// <returns></returns>
        public async Task<IActionResult> Recycle(int mid, int rid)
        {
            int result = await _vmwareManage.Recycle(mid, rid);

            if (result == -1)
            {
                ViewData["Title"] = "操作失败";
                ViewData["Message"] = "数据非法，操作终止！";
                return View("Tip");
            }
            else if (result == -2)
            {
                ViewData["Title"] = "回收失败";
                ViewData["Message"] = $"回收失败，虚拟机未在使用！返回<a href='/Manage/Index'>管理虚拟机</a>";
                return View("Views/Shared/Tip.cshtml");
            }
            else
            {
                ViewData["Title"] = "回收成功";
                ViewData["Message"] = "回收成功！返回<a href='/Manage/Index'>管理虚拟机</a>";
                return View("Views/Shared/Tip.cshtml");
            }
        }

        /// <summary>
        /// GET
        /// Manage/Details/1
        /// 虚拟机详细信息
        /// </summary>
        /// <param name="id">虚拟机ID</param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var machineInfo = await _vmwareManage.Details(id);

                if (machineInfo == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(machineInfo);
                }
            }
        }

        /// <summary>
        /// GET
        /// Manage/Create
        /// 新增虚拟机页面
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// POST
        /// Manage/Create
        /// 新增虚拟机数据
        /// </summary>
        /// <param name="machineInfo">虚拟机对象</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MachineId,MachineIP,MachineSystem,MachineDiskCount,MachineMemory,MachineState,MachineUser,MachinePassword")] MachineInfo machineInfo)
        {
            if (ModelState.IsValid)
            {
                await _vmwareManage.Save(machineInfo);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(machineInfo);
            }
        }

        /// <summary>
        /// GET
        /// Manage/Edit/1
        /// 编辑虚拟机页面
        /// </summary>
        /// <param name="id">虚拟机ID</param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var machineInfo = await _vmwareManage.Details(id);
                if (machineInfo == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(machineInfo);
                }
            }
        }

        /// <summary>
        /// POST
        /// Manage/Edit
        /// 编辑虚拟机数据
        /// </summary>
        /// <param name="id">虚拟机ID</param>
        /// <param name="machineInfo">虚拟机对象</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MachineId,MachineIP,MachineSystem,MachineDiskCount,MachineMemory,MachineState,MachineUser,MachinePassword")] MachineInfo machineInfo)
        {
            if (id != machineInfo.MachineId)
            {
                return NotFound();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    await _vmwareManage.Update(machineInfo);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View(machineInfo);
                }
            }
        }

        /// <summary>
        /// GET
        /// Manage/Delete/1
        /// 删除虚拟机页面
        /// </summary>
        /// <param name="id">虚拟机ID</param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var machineInfo = await _vmwareManage.Details(id);
                if (machineInfo == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(machineInfo);
                }
            }
        }

        /// <summary>
        /// POST
        /// Manage/DeleteConfirmed/1
        /// 删除虚拟机数据
        /// </summary>
        /// <param name="id">虚拟机ID</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var machineInfo = await _vmwareManage.Details(id);
            await _vmwareManage.Delete(machineInfo);
            return RedirectToAction(nameof(Index));
        }
    }
}