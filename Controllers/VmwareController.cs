using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Moetech.Zhuangzhou.Data;
using Moetech.Zhuangzhou.Models;
using Moetech.Zhuangzhou.Common;

namespace Moetech.Zhuangzhou.Controllers
{
    /// <summary>
    /// 申请虚拟机
    /// 控制器
    /// </summary>
    public class VmwareController : FilterController
    {
        /// <summary>
        /// 数据量上下文
        /// </summary>
        private readonly VirtualMachineDB _db;

        /// <summary>
        /// 角色 
        /// </summary>
        public override int[] Role { get; set; } = { 0, 1 };

        public VmwareController(VirtualMachineDB context)
        {
            _db = context;
        }

        /// <summary>
        /// GET
        /// Vmware/Index
        /// 查看空闲虚拟机
        /// </summary>
        /// <returns>IActionResult</returns>
        public async Task<IActionResult> Index()
        {
            var list = from m in _db.MachineInfo
                       where m.MachineState == 0    // 空闲状态
                       orderby m.MachineSystem ascending, m.MachineDiskCount ascending, m.MachineMemory ascending
                       group m by new { m.MachineSystem, m.MachineDiskCount, m.MachineMemory } into b
                       select new MachineInfo
                       {
                           MachineSystem = b.Key.MachineSystem,
                           MachineDiskCount = b.Key.MachineDiskCount,
                           MachineMemory = b.Key.MachineMemory,
                           MachineState = b.Count() // 临时当做剩余数量显示
                       };

            return View(await list.ToListAsync());
        }

        // <summary>
        /// GET
        /// Vmware/Apply
        /// 确认申请
        /// </summary>/
        /// <param name="MachineSystem">操作系统 0：Windows 1：Linux</param>
        /// <param name="MachineDiskCount">硬盘大小/G</param>
        /// <param name="MachineMemory">内存大小/G</param>
        /// <param name="FreeNumber">空闲数量</param>
        /// <returns>IActionResult</returns>
        public IActionResult Apply(int machineSystem, double machineDiskCount, double machineMemory, int freeNumber)
        {
            if (machineSystem >= 2 || machineDiskCount == 0 || machineMemory == 0 || freeNumber == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewData["MachineSystem"] = machineSystem;
            ViewData["MachineDiskCount"] = machineDiskCount;
            ViewData["MachineMemory"] = machineMemory;
            ViewData["FreeNumber"] = freeNumber;
            return View();
        }

        /// <summary>
        /// POST
        /// Vmware/SubmitApply
        /// 提交申请
        /// </summary>
        /// <param name="machineSystem">操作系统 0：Windows 1：Linux</param>
        /// <param name="machineDiskCount">硬盘大小/G</param>
        /// <param name="machineMemory">内存大小/G</param>
        /// <param name="applyNumber">申请数量</param>
        /// <param name="remark">备注</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitApply(int machineSystem, int machineDiskCount, int machineMemory, int applyNumber, string remark)
        {
            // 当前用户信息
            CommonPersonnelInfo userInfo = JsonConvert.DeserializeObject<CommonPersonnelInfo>(HttpContext.Session.GetString("User"));
            // 当前用户ID
            int userId = userInfo.PersonnelId;
            // 最大数量
            int appMaxCount = userInfo.AppMaxCount;

            IEnumerable<MachineInfo> list = from m in _db.MachineInfo
                                            where m.MachineState == 0
                                               && m.MachineSystem == machineSystem
                                               && m.MachineDiskCount == machineDiskCount
                                               && m.MachineMemory == machineMemory
                                            select m;

            // 空闲数量小于申请数量 申请失败
            if (list.Count() < applyNumber)
            {
                ViewData["Title"] = "申请失败";
                ViewData["Message"] = "虚拟机空闲数量不足，请重新申请！";
                return View("Views/Vmware/Error.cshtml");
            }

            // 查询当前用户已申请的数量
            IEnumerable<MachApplyAndReturn> machApplyAndReturnList = from m in _db.MachApplyAndReturn
                                                                     where m.OprationType == 0 && m.ExamineResult == 2 && m.ApplyUserID == userId
                                                                     select m;

            // 未超过数量系统自动审批
            // 超过数量由管理员审批
            // 修改虚拟机状态：申请中
            for (int i = 0; i < applyNumber; i++)
            {
                var model = list.ElementAt(i);
                model.MachineState = ((machApplyAndReturnList.Count() + applyNumber) <= appMaxCount ? 2 : 1);

                _db.MachineInfo.Update(model);

                // 添加到申请记录表
                _db.MachApplyAndReturn.Add(
                    new MachApplyAndReturn()
                    {
                        OprationType = 0,
                        ApplyUserID = userId,
                        ExamineUserID = -1,
                        MachineInfoID = model.MachineId,
                        ExamineResult = ((machApplyAndReturnList.Count() + applyNumber) <= appMaxCount ? 2 : 0),
                        ApplyTime = DateTime.Now,
                        ResultTime = DateTime.Now.AddDays(15), // 默认申请15天
                        Remark = remark
                    });
            }
            await _db.SaveChangesAsync();

            ViewData["Title"] = "申请成功";
            ViewData["Message"] = "申请成功！";
            return View("Views/Vmware/Succeed.cshtml");
        }

        /// <summary>
        /// GET
        /// Vmware/MyVmware
        /// 我的虚拟机
        /// </summary>
        /// <returns></returns>
        public IActionResult MyVmware()
        {
            // 当前用户信息
            CommonPersonnelInfo userInfo = JsonConvert.DeserializeObject<CommonPersonnelInfo>(HttpContext.Session.GetString("User"));

            int userId = userInfo.PersonnelId;

            var list = from m1 in _db.MachineInfo
                       join m2 in _db.MachApplyAndReturn on m1.MachineId equals m2.MachineInfoID
                       where m2.ApplyUserID == userId && m2.OprationType == 0 && m2.ExamineResult == 2
                       select new ReturnMachineInfoApplyData
                       {
                           MachineInfo = m1,
                           MachApplyAndReturn = m2
                       };

            return View(list);
        }

        /// <summary>
        /// 提前归还
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> EarlyReturn(int id)
        {
            // 当前用户信息
            CommonPersonnelInfo userInfo = JsonConvert.DeserializeObject<CommonPersonnelInfo>(HttpContext.Session.GetString("User"));

            int userId = userInfo.PersonnelId;

            if (id == 0)
            {
                ViewData["Title"] = "操作失败";
                ViewData["Message"] = "数据非法，操作终止！";
                return View("Views/Vmware/Error.cshtml");
            }

            var list = from m1 in _db.MachineInfo
                       join m2 in _db.MachApplyAndReturn on m1.MachineId equals m2.MachineInfoID
                       where m2.ApplyAndReturnId == id && m2.ApplyUserID == userId
                       select new ReturnMachineInfoApplyData
                       {
                           MachineInfo = m1,
                           MachApplyAndReturn = m2
                       };

            foreach (var item in list)
            {
                item.MachineInfo.MachineState = 0;
                item.MachApplyAndReturn.OprationType = 1;
                item.MachApplyAndReturn.ResultTime = DateTime.Now;
                _db.MachineInfo.Update(item.MachineInfo);
                _db.MachApplyAndReturn.Update(item.MachApplyAndReturn);
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(MyVmware));
        }


        /// <summary>
        /// 续租
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Relet(int id)
        {
            // 当前用户信息
            CommonPersonnelInfo userInfo = JsonConvert.DeserializeObject<CommonPersonnelInfo>(HttpContext.Session.GetString("User"));

            int userId = userInfo.PersonnelId;

            if (id == 0)
            {
                ViewData["Title"] = "操作失败";
                ViewData["Message"] = "数据非法，操作终止！";
                return View("Views/Vmware/Error.cshtml");
            }

            var list = from m in _db.MachApplyAndReturn
                       where m.ApplyAndReturnId == id && m.OprationType == 0 && m.ApplyUserID == userId
                       select m;

            if (list.Count() == 0)
            {
                ViewData["Title"] = "操作失败";
                ViewData["Message"] = "数据非法，操作终止！";
                return View("Views/Vmware/Error.cshtml");
            }

            foreach (var item in list)
            {

                TimeSpan time = (item.ResultTime - DateTime.Now);
                // 小于三天才能续租
                if (time.Days > 3)
                {
                    ViewData["Title"] = "续租失败";
                    ViewData["Message"] = "虚拟机归还时间必须小于三天！";
                    return View("Views/Vmware/Error.cshtml");
                }

                // 当前时间+15天
                item.ResultTime = DateTime.Now.AddDays(15);

                _db.MachApplyAndReturn.Update(item);
            }

            await _db.SaveChangesAsync();

            ViewData["Title"] = "续租成功";
            ViewData["Message"] = "续租成功！";
            return View("Views/Vmware/Succeed.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public string Test()
        {
            return "Vmware/MyVmware";
        }
    }
}
