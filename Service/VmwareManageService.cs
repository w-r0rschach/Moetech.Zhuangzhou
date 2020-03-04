using Abp.Domain.Uow;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeKit;
using Moetech.Zhuangzhou.Common;
using Moetech.Zhuangzhou.Common.EnumDefine;
using Moetech.Zhuangzhou.Data;
using Moetech.Zhuangzhou.Email;
using Moetech.Zhuangzhou.Interface;
using Moetech.Zhuangzhou.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Service
{
    /// <summary>
    /// 虚拟机管理
    /// </summary>
    public class VmwareManageService : IVmwareManage
    {
        /// <summary>
        /// 每页条数
        /// </summary>
        private readonly int pageSize = 10;
        /// <summary>
        /// 数据量上下文
        /// </summary>
        private readonly VirtualMachineDB _context;
        /// <summary>
        /// 日志接口
        /// </summary>
        private readonly ILogs _logs;

        public VmwareManageService(VirtualMachineDB context,ILogs logs)
        {
            _context = context;
            _logs = logs;
        }

        /// <summary>
        /// 查询全部虚拟机
        /// </summary>
        /// <param name="name">申请人员</param>
        /// <param name="type">操作系统</param>
        /// <param name="status">虚拟机状态</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="personnelInfo">登录用户信息</param>
        /// <returns></returns>
        public async Task<PaginatedList<ReturnMachineInfoApplyData>> SelectAll(CommonPersonnelInfo personnelInfo,string name = "",
            string type = "", int? status = -1, int? pageIndex = 1)
        {
            var list = from m1 in _context.MachineInfo
                       join m2 in (from m3 in _context.MachApplyAndReturn
                                   join cp in _context.CommonPersonnelInfo on m3.ApplyUserID equals cp.PersonnelId
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
            if (!string.IsNullOrWhiteSpace(type))
            {
                list = list.Where(o => o.MachineInfo.MachineSystem.Contains(type));
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

            await _logs.LoggerInfo("虚拟机管理-列表", $"{personnelInfo.PersonnelName} 查询了 申请人为：{name};" +
              $"操作系统类型为：{type}；虚拟机状态为：{status} 的虚拟机信息", personnelInfo.PersonnelId, LogLevel.Information,
              OperationLogType.SELECT);
            return await PaginatedList<ReturnMachineInfoApplyData>.CreateAsync(list.AsNoTracking(), pageIndex ?? 1, pageSize);
        }


        /// <summary>
        /// 查询审批虚拟机
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        public async Task<PaginatedList<ReturnData>> SelectApprove(CommonPersonnelInfo personnelInfo, int? pageIndex = 1)
        {
            var machApplyAndReturn = from m in _context.MachApplyAndReturn
                                     join m1 in _context.MachineInfo on m.MachineInfoID equals m1.MachineId
                                     join m2 in _context.CommonPersonnelInfo on m.ApplyUserID equals m2.PersonnelId
                                     where m.ExamineResult == 0 && m.OprationType == 0
                                     group m by new
                                     {
                                         m1.MachineMemory,
                                         m1.MachineSystem,
                                         m1.MachineDiskCount,
                                         m.ApplyTime,
                                         m.ApplyUserID,
                                         m.OprationType,
                                         m.ExamineResult,
                                         m2.PersonnelName,
                                         m.ResultTime,
                                         m.Remark
                                     } into b
                                     select new ReturnData
                                     {
                                         MachineMemory = b.Key.MachineMemory,
                                         MachineDiskCount = b.Key.MachineDiskCount,
                                         MachineSystem = b.Key.MachineSystem,
                                         ApplyUserID = b.Key.ApplyUserID,
                                         ApplyTime = b.Key.ApplyTime,
                                         OprationType = b.Key.OprationType,
                                         ExamineResult = b.Key.ExamineResult,
                                         AppUserName = b.Key.PersonnelName,
                                         ResultTime = b.Key.ResultTime,
                                         Remark = b.Key.Remark,
                                         NumberCount = b.Count()
                                     };

            await _logs.LoggerInfo("虚拟机管理-列表", $"{personnelInfo.PersonnelName} 查询了所有待审批状态的虚拟机",
                personnelInfo.PersonnelId, LogLevel.Information,OperationLogType.SELECT);

            return await PaginatedList<ReturnData>.CreateAsync(machApplyAndReturn.AsNoTracking(), pageIndex ?? 1, pageSize);
        }


        /// <summary>
        /// 提交审批虚拟机
        /// </summary>；
        /// <param name="mid">虚拟机信息ID</param>
        /// <param name="aid">申请归还信息ID</param>
        /// <param name="state">状态1：拒绝  2：同意</param>
        /// <param name="userid">用户ID</param>
        /// <returns></returns>
        public async Task<int> SubmitApprove(CommonPersonnelInfo personnelInfo, int mid, int aid, int state, int userid)
        {
            var list = from m1 in _context.MachApplyAndReturn
                       join m2 in _context.MachineInfo on m1.MachineInfoID equals m2.MachineId
                       join m3 in _context.CommonPersonnelInfo on m1.ApplyUserID equals m3.PersonnelId
                       where m1.MachineInfoID == mid && m1.ApplyAndReturnId == aid
                       select new ReturnMachineInfoApplyData
                       {
                           MachApplyAndReturn = m1,
                           MachineInfo = m2,
                           CommonPersonnelInfo=m3
                       };

            if (list.Count() == 0)
            {
                return -1;
            }
            else
            {
                foreach (var item in list)
                {
                    item.MachApplyAndReturn.ExamineUserID = userid;                      // 修改审批人员ID
                    item.MachApplyAndReturn.ExamineResult = state;                       // 修改审批结果
                    item.MachineInfo.MachineState = state == 2 ? 2 : 0;                  // 修改虚拟机状态

                    _context.MachApplyAndReturn.Update(item.MachApplyAndReturn);
                    _context.MachineInfo.Update(item.MachineInfo);

                    //await _logs.LoggerInfo("虚拟机管理-审批", $"{personnelInfo.PersonnelName} {(state == 1 ? "拒绝" : "同意")} 了" +
                    //  $" {item.CommonPersonnelInfo.PersonnelName} 的虚拟机申请；虚拟机IP地址:{item.MachineInfo.MachineIP}、" +
                    //  $" 虚拟机类型：{item.MachineInfo.MachineMemory}、虚拟机内存:{item.MachineInfo.MachineMemory}、" +
                    //  $"硬盘:{item.MachineInfo.MachineDiskCount}", personnelInfo.PersonnelId,
                    //  LogLevel.Trace, OperationLogType.MODIFY);


                }
              
 return await _context.SaveChangesAsync();
            }   
        }

        /// <summary>
        /// 回收虚拟机
        /// </summary>
        /// <param name="mid">虚拟机信息ID</param>
        /// <param name="rid">申请虚拟机记录ID</param>
        /// <returns></returns>
        public int Recycle(CommonPersonnelInfo personnelInfo, int mid, int rid)
        {
            var list = from m1 in _context.MachineInfo
                       join m2 in _context.MachApplyAndReturn on m1.MachineId equals m2.MachineInfoID
                       where m1.MachineId == mid && m2.ApplyAndReturnId == rid
                       select new
                       {
                           MachineInfo = m1,
                           MachApplyAndReturn = m2
                       };

            if (list.Count() == 0)
            {
                return -1;
            }
            else
            {
                foreach (var item in list)
                {
                    if (item.MachineInfo.MachineState == 2)
                    {
                        item.MachineInfo.MachineState = 0;
                        item.MachApplyAndReturn.OprationType = 1;
                        item.MachApplyAndReturn.ResultTime = DateTime.Now;

                        _context.MachineInfo.Update(item.MachineInfo);
                        _context.MachApplyAndReturn.Update(item.MachApplyAndReturn);

                        _logs.LoggerInfo("虚拟机管理-回收", $"{personnelInfo.PersonnelName} 强制回收了IP为:{item.MachineInfo.MachineIP}的虚拟机", 
                            personnelInfo.PersonnelId, LogLevel.Trace, OperationLogType.MODIFY);

                    }
                    else
                    {
                        return -2;
                    }
                }

                _context.SaveChangesAsync().Wait();
                return 1;
            }
        }

        /// <summary>
        /// 查询单台虚拟机数据
        /// </summary>
        /// <param name="id">虚拟机ID</param>
        /// <returns></returns>
        public async Task<MachineInfo> Details(CommonPersonnelInfo personnelInfo, int? id)
        {
       
          var machineInfo = _context.MachineInfo.FirstOrDefaultAsync(m => m.MachineId == id);

            await _logs.LoggerInfo("虚拟机管理-查询", $"{personnelInfo.PersonnelName} 查询了IP为:{machineInfo.Result.MachineIP}的虚拟机",
                     personnelInfo.PersonnelId, LogLevel.Trace, OperationLogType.SELECT);

            return await machineInfo;
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="machineInfo">虚拟机对象</param>
        /// <returns></returns>
        public async Task<int> Save(MachineInfo machineInfo, CommonPersonnelInfo personnelInfo)
        {
            await _logs.LoggerInfo("虚拟机管理-添加", $"{personnelInfo.PersonnelName} 添加了IP为:{machineInfo.MachineIP}，" +
                $"系统类型：{machineInfo.MachineSystem},硬盘：{machineInfo.MachineDiskCount},内存：{machineInfo.MachineMemory} 的虚拟机",
                   personnelInfo.PersonnelId, LogLevel.Trace, OperationLogType.ADD);

            _context.MachineInfo.Add(machineInfo);
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="machineInfo">虚拟机对象</param>
        /// <returns></returns>
        public async Task<int> Update( CommonPersonnelInfo personnelInfo,MachineInfo machineInfo)
        {
            await _logs.LoggerInfo("虚拟机管理-修改", $"{personnelInfo.PersonnelName} 修改了IP为:{machineInfo.MachineIP}，" +
               $"系统类型：{machineInfo.MachineSystem},硬盘：{machineInfo.MachineDiskCount},内存：{machineInfo.MachineMemory} 的虚拟机",
                  personnelInfo.PersonnelId, LogLevel.Trace, OperationLogType.MODIFY);

            _context.Update(machineInfo);
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="machineInfo">虚拟机对象</param>
        /// <returns></returns>
        public async Task<int> Delete(CommonPersonnelInfo personnelInfo, MachineInfo machineInfo)
        {
            await _logs.LoggerInfo("虚拟机管理-删除", $"{personnelInfo.PersonnelName} 删除了IP为:{machineInfo.MachineIP}，" +
                 $"系统类型：{machineInfo.MachineSystem},硬盘：{machineInfo.MachineDiskCount},内存：{machineInfo.MachineMemory} 的虚拟机",
                    personnelInfo.PersonnelId, LogLevel.Trace, OperationLogType.DELETE);

            _context.MachineInfo.Remove(machineInfo);
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 虚拟机审批
        /// </summary>
        /// <param name="ApplyUserID">申请人</param>
        /// <param name="ApplyTime">申请时间</param>
        /// <param name="ResultTime">归还时间</param>
        /// <param name="Remark">申请原因</param>
        /// <param name="state">审批状态</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public async Task<bool> ResultSubmitApprove(CommonPersonnelInfo personnelInfo, int ApplyUserID, DateTime ApplyTime,
            DateTime ResultTime, string Remark, int state, int userId)
        {

            int rsultInt = 0;
            var MachApplyAndReturnInfo = from m in _context.MachApplyAndReturn
                                         join m1 in _context.CommonPersonnelInfo on m.ApplyUserID equals m1.PersonnelId 
                                         where m.ApplyUserID == ApplyUserID && m.ApplyTime == ApplyTime &&
                                               m.ResultTime == ResultTime && m.Remark == Remark && m1.PersonnelId == ApplyUserID
                                         select new ReturnMachineInfoApplyData { 
                                               MachApplyAndReturn = m,
                                               CommonPersonnelInfo = m1
                                         };

            foreach (var item in MachApplyAndReturnInfo.ToList())
            {
                int result = await SubmitApprove(personnelInfo,item.MachApplyAndReturn.MachineInfoID, item.MachApplyAndReturn.ApplyAndReturnId, state, userId);

                if (result > 0)
                {
                    rsultInt += 1;
                }
                else
                {
                    rsultInt = 0;
                }
            }
            if (MachApplyAndReturnInfo.ToList().Count>0)
            {
                await _logs.LoggerInfo("虚拟机管理-通知", $"系统已发送邮件通知 {MachApplyAndReturnInfo.ToList()[0].CommonPersonnelInfo.PersonnelName} 审批信息" ,
                    personnelInfo.PersonnelId, LogLevel.Trace, OperationLogType.DELETE);

                //发送审批结果邮件
                MessageWarn messageWarn = await SendMailFctory.ApprovalSendMailAsync(MachApplyAndReturnInfo.ToList()[0], state);
                _context.MessageWarns.Add(messageWarn);
                await _context.SaveChangesAsync();
            }

            return rsultInt > 0 ? true : false;
        }
        /// <summary>
        /// 判断是否存在重复ip地址
        /// 实现：
        /// 1、machineId为0时为新增操作，执行新增验证；
        /// 反之为修改排除当前Id并验证是否存在相同Ip
        /// </summary>
        /// <param name="host">ip地址</param>
        /// <param name="machineId">虚拟机Id</param>
        /// <returns></returns>
        public bool CheckHost(string host, int machineId = 0)
        {
            List<MachineInfo> machineInfo;
            if (machineId > 0)
            {
                var info = from m in _context.MachineInfo.Where(s => s.MachineIP == host
                               && s.MachineId != machineId)
                           select m;
                machineInfo = info.ToList();
            }
            else
            {
                var info = from m in _context.MachineInfo.Where(s => s.MachineIP == host)
                           select m;
                machineInfo = info.ToList();
            }

            if (machineInfo.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="machineId">虚拟机Id</param>
        /// <param name="applyPersonId">使用者id</param>
        /// <returns></returns>
        public async Task SendMail( int machineId, int applyPersonId)
        {
            var personInfo = from m in _context.CommonPersonnelInfo
                             from n in _context.MachineInfo
                             where m.PersonnelId == applyPersonId && n.MachineId == machineId
                             select new ReturnMachineInfoApplyData
                             {
                                 MachineInfo = n,
                                 CommonPersonnelInfo = m
                             };
            if (personInfo != null)
            {
                ReturnMachineInfoApplyData info = personInfo.ToList()[0];
                //发送邮件
                MessageWarn messageWarn = await SendMailFctory.AdminiSendMailAsync(info);
                //添加提醒记录信息
                _context.MessageWarns.Add(messageWarn);
                _context.SaveChanges();
            }
        }
    }
}
