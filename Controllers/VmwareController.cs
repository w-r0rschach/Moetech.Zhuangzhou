using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Moetech.Zhuangzhou.Models;
using Moetech.Zhuangzhou.Interface;
using Moetech.Zhuangzhou.Email;
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
        /// 虚拟机信息接口
        /// </summary>
        private IVmware _vmware;
        /// <summary>
        /// 角色 
        /// </summary>
        public override int[] Role { get; set; } = { 0, 1 };

        public VmwareController(IVmware vmware)
        {
            _vmware = vmware;

        }

        /// <summary>
        /// GET
        /// Vmware/Index
        /// 查看空闲虚拟机
        /// </summary>
        /// <returns>IActionResult</returns>
        public async Task<IActionResult> Index()
        {
            CommonPersonnelInfo userInfo; userInfo = JsonConvert.DeserializeObject<CommonPersonnelInfo>(HttpContext.Session.GetString("User"));
            return View(await _vmware.SelectVmware(userInfo).ToListAsync());
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Apply(string machineSystem, double machineDiskCount, double machineMemory, int freeNumber)
        {
            if (string.IsNullOrWhiteSpace(machineSystem) || machineDiskCount == 0 || machineMemory == 0 || freeNumber == 0)
            {
                ViewData["Title"] = "操作失败";
                ViewData["Message"] = "数据非法，操作终止！";
                return View("Views/Shared/Tip.cshtml");
            }
            else
            {
                ViewData["MachineSystem"] = machineSystem;
                ViewData["MachineDiskCount"] = machineDiskCount;
                ViewData["MachineMemory"] = machineMemory;
                ViewData["FreeNumber"] = freeNumber;

                return View();
            }
        }

        /// <summary>
        /// POST
        /// Vmware/SubmitApply
        /// 提交申请
        /// </summary>
        /// <param name="machineSystem">操作系统</param>
        /// <param name="machineDiskCount">硬盘大小/G</param>
        /// <param name="machineMemory">内存大小/G</param>
        /// <param name="applyNumber">申请数量</param>
        /// <param name="remark">备注</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitApply(string machineSystem, int machineDiskCount, int machineMemory, int applyNumber, string remark)
        {
            CommonPersonnelInfo userInfo; userInfo = JsonConvert.DeserializeObject<CommonPersonnelInfo>(HttpContext.Session.GetString("User"));

            if (string.IsNullOrWhiteSpace(machineSystem) || machineDiskCount == 0 ||
                machineMemory == 0 || applyNumber == 0 || string.IsNullOrWhiteSpace(remark) || remark.Length > 255)
            {
                ViewData["Title"] = "操作失败";
                ViewData["Message"] = "数据非法，操作终止！";
                return View("Views/Shared/Tip.cshtml");
            }
            else
            {
                int result = await _vmware.SubmitApplication(machineSystem, machineDiskCount, machineMemory, applyNumber, remark, userInfo);

                if (result == -1)
                {
                    ViewData["Title"] = "申请失败";
                    ViewData["Message"] = "虚拟机空闲数量不足，请重新申请！返回<a href='/Vmware'>申请虚拟机</a>";
                    return View("Views/Shared/Tip.cshtml");
                }
                else if (result == 0)
                {
                    ViewData["Title"] = "申请成功";
                    ViewData["Message"] = "申请虚拟机数量超过设定值，需要等待管理员审批！ 查看<a href='/Vmware/MyVmware'>我的虚拟机</a>";
                    List<CommonPersonnelInfo> infos = await _vmware.GetAdminInfo(); //获取所有管理员的信息
                    List<MessageWarn> messageWarn = await SendMailFctory.ApprovalSendMailManageAsync(infos); //发送邮件提醒管理员
                    await _vmware.SaveMesageWarn(messageWarn); //保存消息记录
                    return View("Views/Shared/Tip.cshtml");
                }
                else
                {
                    ViewData["Title"] = "申请成功";
                    ViewData["Message"] = "申请成功！查看<a href='/Vmware/MyVmware'>我的虚拟机</a>";
                    return View("Views/Shared/Tip.cshtml");
                }
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
            CommonPersonnelInfo userInfo; userInfo = JsonConvert.DeserializeObject<CommonPersonnelInfo>(HttpContext.Session.GetString("User"));
            return View(_vmware.MyVmware(userInfo));
        }

        /// <summary>
        /// 提前归还
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> EarlyReturn(int id)
        {
            CommonPersonnelInfo userInfo; userInfo = JsonConvert.DeserializeObject<CommonPersonnelInfo>(HttpContext.Session.GetString("User"));
            if (id == 0)
            {
                ViewData["Title"] = "操作失败";
                ViewData["Message"] = "数据非法，操作终止！";
                return View("Views/Shared/Tip.cshtml");
            }

            bool result = await _vmware.EarlyReturn(id, userInfo);

            if (!result)
            {
                ViewData["Title"] = "归还失败";
                ViewData["Message"] = "归还失败，未找到指定归还的虚拟机！查看<a href='/Vmware/MyVmware'>我的虚拟机</a>";
                return View("Views/Shared/Tip.cshtml");
            }
            else
            {
                return RedirectToAction(nameof(MyVmware));
            }
        }


        /// <summary>
        /// 续租
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Relet(int id)
        {
            CommonPersonnelInfo userInfo; userInfo = JsonConvert.DeserializeObject<CommonPersonnelInfo>(HttpContext.Session.GetString("User"));
            if (id == 0)
            {
                ViewData["Title"] = "操作失败";
                ViewData["Message"] = "数据非法，操作终止！";
                return View("Views/Shared/Tip.cshtml");
            }
            else
            {
                var result = await _vmware.Renew(id, userInfo);

                if (result == false)
                {
                    ViewData["Title"] = "续租失败";
                    ViewData["Message"] = "虚拟机归还时间必须小于三天！查看<a href='/Vmware/MyVmware'>我的虚拟机</a>";
                    return View("Views/Shared/Tip.cshtml");
                }
                else
                {
                    ViewData["Title"] = "续租成功";
                    ViewData["Message"] = "续租成功！查看<a href='/Vmware/MyVmware'>我的虚拟机</a>";
                    return View("Views/Shared/Tip.cshtml");
                }
            }
        }

        /// <summary>
        /// 修改提醒记录
        /// </summary>
        /// <returns></returns>
        public IActionResult RedateRemain(int id)
        {
            List<MessageWarn> messageWarns = _vmware.UpdateRemain(id);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// 获取对应的提醒消息
        /// </summary>
        public IActionResult SelectRemain() 
        {
            List<MessageWarn> messageWarns = new List<MessageWarn>();
            if (CommonUserInfo.MessageWarnList.Count() > 0)
            {
                //获取对应的提醒消息并删除缓存中对应的提醒消息
                for (int i = 0; i < CommonUserInfo.MessageWarnList.Count(); i++)
                {
                    if (CommonUserInfo.MessageWarnList[i].PonsonalId == CommonUserInfo.UserInfo.PersonnelId)
                    {
                        messageWarns.Add(CommonUserInfo.MessageWarnList[i]);
                        CommonUserInfo.MessageWarnList.Remove(CommonUserInfo.MessageWarnList[i]);
                    }
                }
            }
            else 
            {
                messageWarns = _vmware.SelevtMessageWarn(CommonUserInfo.UserInfo);
            }
            return Json(messageWarns);
        }
    }
}
