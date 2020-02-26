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
using Moetech.Zhuangzhou.Interface;

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
        //private readonly VirtualMachineDB _db;

        /// <summary>
        /// 虚拟机信息接口
        /// </summary>
        private IVmware _vmware;

        public VmwareController(IVmware vmware)
        {
            _vmware = vmware;
        }

        /// <summary>
        /// 角色 
        /// </summary>
        public override int[] Role { get; set; } = { 0, 1 };

        //public VmwareController(VirtualMachineDB context)
        //{
        //    _db = context;
        //}

        /// <summary>
        /// GET
        /// Vmware/Index
        /// 查看空闲虚拟机
        /// </summary>
        /// <returns>IActionResult</returns>
        public async Task<IActionResult> Index()
        {
            return View(await _vmware.SelectVmware().ToListAsync());
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
            IQueryable<MachineInfo> list = await _vmware.SubmitApplication(machineSystem, machineDiskCount, machineMemory, applyNumber, remark, userInfo);
            // 空闲数量小于申请数量 申请失败
            if (list.Count() < applyNumber)
            {
                ViewData["Title"] = "申请失败";
                ViewData["Message"] = "虚拟机空闲数量不足，请重新申请！";
                return View("Views/Vmware/Error.cshtml");
            }
            else
            {
                ViewData["Title"] = "申请成功";
                ViewData["Message"] = "申请成功！";
                return View("Views/Vmware/Succeed.cshtml");
            }
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
            return View(_vmware.MyVmware(userInfo));
        }

        /// <summary>
        /// 提前归还
        /// </summary>
        /// <returns></returns>
        public IActionResult EarlyReturn(int id)
        {
            // 当前用户信息
            CommonPersonnelInfo userInfo = JsonConvert.DeserializeObject<CommonPersonnelInfo>(HttpContext.Session.GetString("User"));
            _vmware.EarlyReturn(id, userInfo);
            if (id == 0)
            {
                ViewData["Title"] = "操作失败";
                ViewData["Message"] = "数据非法，操作终止！";
                return View("Views/Vmware/Error.cshtml");
            }
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
            else
            {
                var list = await _vmware.Renew(id, userInfo);
                if (list.Count() == 0)
                {
                    ViewData["Title"] = "操作失败";
                    ViewData["Message"] = "数据非法，操作终止！";
                    return View("Views/Vmware/Error.cshtml");
                }
                else
                {
                    ViewData["Title"] = "续租成功";
                    ViewData["Message"] = "续租成功！";
                    return View("Views/Vmware/Succeed.cshtml");
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public string Test()
        {
            return "Vmware/MyVmware";
        }
    }
}
