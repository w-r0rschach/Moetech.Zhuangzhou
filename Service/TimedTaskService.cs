using Microsoft.Extensions.Logging;
using MimeKit;
using Moetech.Zhuangzhou.Common;
using Moetech.Zhuangzhou.Data;
using Moetech.Zhuangzhou.Email;
using Moetech.Zhuangzhou.Interface;
using Moetech.Zhuangzhou.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Service
{
    public class TimedTaskService : ITimedTask
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// 虚拟机管理接口
        /// </summary>
        private readonly IVmwareManage _vmwareManage;

        /// <summary>
        /// 数据量上下文
        /// </summary>
        private readonly VirtualMachineDB _context;

        public TimedTaskService(ILogger<TimedTaskService> logger, VirtualMachineDB context,IVmwareManage vmwareManage)
        {
            _logger = logger;
            _context = context;
            _vmwareManage = vmwareManage;
        }

        /// <summary>
        /// 邮件任务
        /// </summary>
        public async Task EmailTaskAsync()
        {
            var info = from n in _context.MachApplyAndReturn
                       where n.OprationType == 0 && n.ExamineResult == 2 && n.ResultTime > DateTime.Now
                       select n;

            if (info.Count() > 0)
            {

                //找到时间即将到期的虚拟机
                List<MachApplyAndReturn> machApplyAndReturns = info.ToList().FindAll(o => (o.ResultTime - DateTime.Now).Days < 3);

                if (machApplyAndReturns.Count > 0)
                {
                    //发送邮件
                    for (int i = 0; i < machApplyAndReturns.Count; i++)
                    {
                        //先找到对应人员的人员信息
                        var personInfo = from m in _context.CommonPersonnelInfo
                                         from n in _context.MachineInfo
                                         from t in _context.MachApplyAndReturn
                                         where m.PersonnelId == machApplyAndReturns[i].ApplyUserID && 
                                         n.MachineId == machApplyAndReturns[i].MachineInfoID &&
                                         t.ApplyAndReturnId == machApplyAndReturns[i].ApplyAndReturnId
                                         select new ReturnMachineInfoApplyData
                                         {
                                             MachineInfo = n,
                                             CommonPersonnelInfo = m,
                                             MachApplyAndReturn = t
                                         };
                        //获取到人员信息
                        var data = personInfo.ToList().ElementAt(0);
                        //发送邮件
                        await SendMailFctory.RemindSendMailAsync(data);
                        //添加日志记录 TODO
                    }
                }
            }
        }

        /// <summary>
        /// 检查虚拟机是否到期，到期则发送邮件
        /// </summary>
        public async Task CheckMaturityAsync()
        {
            //查询正在使用中的,并且归还时间已到期的虚拟机
            var info = from n in _context.MachApplyAndReturn
                       where n.OprationType == 0 && n.ExamineResult == 2 && n.ResultTime <= DateTime.Now
                       select n;

            List<MachApplyAndReturn> maches = info.ToList();
            if (info.Count() > 0)
            {
                //回收虚拟机
                for (int i = 0; i < maches.Count(); i++)
                {
                    //找到个人信息
                    var personInfo = from m in _context.CommonPersonnelInfo
                                     from n in _context.MachineInfo
                                     from t in _context.MachApplyAndReturn
                                     where m.PersonnelId == maches[i].ApplyUserID && n.MachineId == maches[i].MachineInfoID 
                                     && t.ApplyAndReturnId == maches[i].ApplyAndReturnId
                                     select new ReturnMachineInfoApplyData
                                     {
                                         MachineInfo = n,
                                         CommonPersonnelInfo = m,
                                         MachApplyAndReturn = t
                                     };
                    //回收虚拟机
                    int result = _vmwareManage.Recycle(maches[i].MachineInfoID, maches[i].ApplyAndReturnId);
                    //获取到人员信息
                    var data = personInfo.ToList().ElementAt(0);
                    //发送邮件
                    if (result == 1) 
                    {
                        await SendMailFctory.SysSendMailAsync(data);
                        //添加日志信息 TODO
                    }
                }
                
            }
        }
    }
}
