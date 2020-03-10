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
using Microsoft.Extensions.Logging;
using Moetech.Zhuangzhou.Common.EnumDefine;

namespace Moetech.Zhuangzhou.Service
{
    public class VmwareService : IVmware
    {
        /// <summary>
        /// 数据量上下文
        /// </summary>
        private VirtualMachineDB _context;
        /// <summary>
        /// 日志接口
        /// </summary>
        private ILogs _logs;
        /// <summary>
        /// 续租时间
        /// </summary>
        private readonly int Renewal = 15;
        public VmwareService(VirtualMachineDB context, ILogs logs)
        {
            _context = context;
            _logs = logs;
        }

        /// <summary>
        /// 查询虚拟机
        /// </summary>
        /// <returns></returns>
        public IQueryable<MachineInfo> SelectVmware(CommonPersonnelInfo personnelInfo)
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

            _logs.LoggerInfo("虚拟机申请-列表", $"{personnelInfo.PersonnelName} 查询了所有可申请的虚拟机",
                personnelInfo.PersonnelId, LogLevel.Trace, OperationLogType.SELECT);

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
        /// <param name="userInfo">当前用户信息</param>
        /// <returns>
        /// -1:申请失败
        /// 0:待审批
        /// 2:同意
        /// </returns>
        public async Task<int> SubmitApplication(string machineSystem, int machineDiskCount, int machineMemory,
            int applyNumber, string remark, CommonPersonnelInfo userInfo)
        {
            ///获取当前时间
            DateTime _dateTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            // 当前用户ID
            int userId = userInfo.PersonnelId;
            // 最大数量
            int appMaxCount = userInfo.AppMaxCount;

            IEnumerable<MachineInfo> list = from m in _context.MachineInfo
                                            where m.MachineState == 0 &&
                                                 m.MachineSystem == machineSystem &&
                                                 m.MachineDiskCount == machineDiskCount &&
                                                 m.MachineMemory == machineMemory
                                            select m;


            // 空闲数量小于申请数量 申请失败
            if (list.Count() < applyNumber)
            {
                return -1;
            }


            // 查询当前用户已申请的数量
            var machApplyAndReturnList = from m in _context.MachApplyAndReturn
                                         where m.OprationType == 0 && m.ExamineResult == 2 && m.ApplyUserID == userId
                                         select m;

            // 自动批准
            bool autoApprove = false;

            if ((machApplyAndReturnList.Count() + applyNumber) <= appMaxCount)
            {
                autoApprove = true;
            }

            // 未超过数量系统自动审批
            // 超过数量由管理员审批
            // 修改虚拟机状态：申请中
            for (int i = 0; i < applyNumber; i++)
            {
                var model = list.ElementAt(i);
                model.MachineState = (autoApprove == true ? 2 : 1);

                _context.MachineInfo.Update(model);

                // 添加到申请记录表
                _context.MachApplyAndReturn.Add(
                    new MachApplyAndReturn()
                    {
                        OprationType = 0,
                        ApplyUserID = userId,
                        ExamineUserID = -1,
                        MachineInfoID = model.MachineId,
                        ExamineResult = (autoApprove == true ? 2 : 0),
                        ApplyTime = _dateTime,
                        ResultTime = _dateTime.AddDays(15), // 默认申请15天
                        Remark = remark
                    }); 
            }
            await _context.SaveChangesAsync();

            await _logs.LoggerInfo("虚拟机申请-提交申请", $"{userInfo.PersonnelName} 申请了  内存：{list.FirstOrDefault().MachineMemory} " +
                $"硬盘：{list.FirstOrDefault().MachineDiskCount} 系统：{list.FirstOrDefault().MachineSystem} 的虚拟机 {applyNumber}台",
                userInfo.PersonnelId, LogLevel.Trace, OperationLogType.MODIFY);

            return autoApprove == true ? 2 : 0;
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

            _logs.LoggerInfo("我的虚拟机-列表", $"{userInfo.PersonnelName} 查询了我的虚拟机数据",
                userInfo.PersonnelId, LogLevel.Trace, OperationLogType.SELECT);

            return list;
        }

        /// <summary>
        /// 提前归还
        /// </summary>
        /// <param name="userInfo"></param>
        public async Task<bool> EarlyReturn(int id, CommonPersonnelInfo userInfo)
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

            if (list.Count() == 0)
            {
                return false;
            }

            foreach (var item in list)
            {
                item.MachineInfo.MachineState = 0;
                _context.MachineInfo.Update(item.MachineInfo);

                item.MachApplyAndReturn.OprationType = 1;
                item.MachApplyAndReturn.ResultTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                _context.MachApplyAndReturn.Update(item.MachApplyAndReturn);


                await _logs.LoggerInfo("我的虚拟机-列表", $"{userInfo.PersonnelName} 归还了 IP：{item.MachineInfo.MachineIP} 硬盘：{item.MachineInfo.MachineDiskCount} " +
                    $"内存：{item.MachineInfo.MachineMemory} 的虚拟机",
                    userInfo.PersonnelId, LogLevel.Trace, OperationLogType.MODIFY);
            }

            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// 续租
        /// </summary>
        public async Task<bool> Renew(int id, CommonPersonnelInfo userInfo)
        {
            int userId = userInfo.PersonnelId; 
            DateTime _dateTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            var machApplyAndReturn = from m in _context.MachApplyAndReturn
                                     join m1 in _context.MachineInfo on  m.MachineInfoID equals m1.MachineId
                                     where (m.ApplyAndReturnId == id&& m.OprationType == 0 && m.ApplyUserID == userId)
                                     select new ReturnMachineInfoApplyData
                                     {
                                         MachineInfo = m1,
                                         MachApplyAndReturn = m
                                     };

            ReturnMachineInfoApplyData ReurnInfo = machApplyAndReturn.FirstOrDefault();
            TimeSpan time = (ReurnInfo.MachApplyAndReturn.ResultTime - _dateTime);

            // 小于三天才能续租
            if (time.Days > 3)
            {
                return false;
            }

            // 当前时间+15天
            ReurnInfo.MachApplyAndReturn.ResultTime = _dateTime.AddDays(Renewal);

            await _logs.LoggerInfo("我的虚拟机-列表", $"{userInfo.PersonnelName} 将虚拟机 IP：{ReurnInfo.MachineInfo.MachineIP} 硬盘：{ReurnInfo.MachineInfo.MachineDiskCount} " +
                   $"内存：{ReurnInfo.MachineInfo.MachineMemory} 续租了 {Renewal}天;虚拟机结束时间为:{_dateTime.AddDays(Renewal)}",
                   userInfo.PersonnelId, LogLevel.Trace, OperationLogType.MODIFY);

            _context.MachApplyAndReturn.Update(ReurnInfo.MachApplyAndReturn);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 保存提醒消息记录
        /// </summary>
        /// <param name="messageWarn"></param>
        public async Task SaveMesageWarn(List<MessageWarn> messageWarn)
        {
            if (messageWarn.Count > 0)
            {
                for (int i = 0; i < messageWarn.Count; i++)
                {
                    _context.MessageWarns.Add(messageWarn[i]);
                    await _context.SaveChangesAsync();
                    //发送给前面显示
                    await WebSocketHandle.SendAsync(messageWarn[i], CommonUserInfo.WebSocket);
                }
            }
        }

        /// <summary>
        /// 得到管理员信息
        /// </summary>
        /// <returns></returns>
        public Task<List<CommonPersonnelInfo>> GetAdminInfo()
        {
            var info = from n in _context.CommonPersonnelInfo
                       where n.DepId == 1
                       select n;
            return info.ToListAsync();
        }

        /// <summary>
        /// 修改记录
        /// </summary>
        /// <returns></returns>
        public List<MessageWarn> UpdateRemain(int id)
        {
            var messageWarn = from m in _context.MessageWarns
                              where m.PonsonalId == CommonUserInfo.UserInfo.PersonnelId
                              select m;
            MessageWarn warn = messageWarn.ToList().Find(o=> o.MessageId == id);
            warn.MessageType = 1;warn.MessageReadDate = DateTime.Now;
            _context.Update(warn);
            _context.SaveChanges();
            List<MessageWarn> MessageWarns = messageWarn.ToList().FindAll(o => o.MessageType == 0);
            return MessageWarns;
        }
    }
}
