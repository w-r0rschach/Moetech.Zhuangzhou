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

namespace Moetech.Zhuangzhou.Service
{
    public class VmwareService : IVmware
    {
        /// <summary>
        /// 数据量上下文
        /// </summary>
        private VirtualMachineDB _context;

        public VmwareService(VirtualMachineDB context)
        {
            _context = context;
        }

        /// <summary>
        /// 查询虚拟机
        /// </summary>
        /// <returns></returns>
        public IQueryable<MachineInfo> SelectVmware()
        {
            IQueryable<MachineInfo> list = from m in _context.MachineInfo
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
            return list;
        }

        /// <summary>
        /// 提交申请
        /// </summary>
        /// <param name="machineSystem">操作系统 0：Windows 1：Linux</param>
        /// <param name="machineDiskCount">硬盘大小/G</param>
        /// <param name="machineMemory">内存大小/G</param>
        /// <param name="applyNumber">申请数量</param>
        /// <param name="remark">备注</param>
        public IQueryable<MachineInfo> SubmitApplication(int machineSystem, int machineDiskCount, int machineMemory, int applyNumber, string remark, CommonPersonnelInfo userInfo)
        {
            // 当前用户ID
            int userId = userInfo.PersonnelId;
            // 最大数量
            int appMaxCount = userInfo.AppMaxCount;

            var list = from m in _context.MachineInfo
                                            where m.MachineState == 0
                                               && m.MachineSystem == machineSystem
                                               && m.MachineDiskCount == machineDiskCount
                                               && m.MachineMemory == machineMemory
                                            select m;



            // 查询当前用户已申请的数量
            var machApplyAndReturnList = from m in _context.MachApplyAndReturn
                                                                     where m.OprationType == 0 && m.ExamineResult == 2 && m.ApplyUserID == userId
                                                                     select m;

            // 未超过数量系统自动审批
            // 超过数量由管理员审批
            // 修改虚拟机状态：申请中
            for (int i = 0; i < applyNumber; i++)
            {
                var model = list.ElementAt(i);
                model.MachineState = ((machApplyAndReturnList.Count() + applyNumber) <= appMaxCount ? 2 : 1);

                _context.MachineInfo.Update(model);

                // 添加到申请记录表
                _context.MachApplyAndReturn.Add(
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
            _context.SaveChangesAsync();

            return list;
        }

        /// <summary>
        /// 我的虚拟机
        /// </summary>
        public IQueryable<ReturnMachineInfoApplyData> MyVmware(CommonPersonnelInfo userInfo)
        {
            int userId = userInfo.PersonnelId;
            var list = from m1 in _context.MachineInfo
                       join m2 in _context.MachApplyAndReturn on m1.MachineId equals m2.MachineInfoID
                       where m2.ApplyUserID == userId && m2.OprationType == 0
                       select new ReturnMachineInfoApplyData
                       {
                           MachineInfo = m1,
                           MachApplyAndReturn = m2
                       };
            return list;
        }

        /// <summary>
        /// 提前归还
        /// </summary>
        /// <param name="userInfo"></param>
        public void EarlyReturn(int id,CommonPersonnelInfo userInfo)
        {
            int userId = userInfo.PersonnelId;
            var list = from m1 in _context.MachineInfo
                       join m2 in _context.MachApplyAndReturn on m1.MachineId equals m2.MachineInfoID
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
                _context.MachineInfo.Update(item.MachineInfo);
                _context.MachApplyAndReturn.Update(item.MachApplyAndReturn);
            }
            _context.SaveChangesAsync();
        }

        /// <summary>
        /// 续租
        /// </summary>
        public IQueryable<MachApplyAndReturn> Renew(int id, CommonPersonnelInfo userInfo)
        {
            int userId = userInfo.PersonnelId;

            var list = from m in _context.MachApplyAndReturn
                       where m.ApplyAndReturnId == id && m.OprationType == 0 && m.ApplyUserID == userId
                       select m;
            foreach (var item in list)
            {

                TimeSpan time = (item.ResultTime - DateTime.Now);
                // 小于三天才能续租
                //if (time.Days > 3)
                //{
                //    ViewData["Title"] = "续租失败";
                //    ViewData["Message"] = "虚拟机归还时间必须小于三天！";
                //    return View("Views/Vmware/Error.cshtml");
                //}

                // 当前时间+15天
                item.ResultTime = DateTime.Now.AddDays(15);

                _context.MachApplyAndReturn.Update(item);
                _context.SaveChangesAsync();                
            }
            return list;
        }
    }
}
