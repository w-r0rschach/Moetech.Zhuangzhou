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
        /// 数据量上下文
        /// </summary>
        private readonly VirtualMachineDB _context;

        public TimedTaskService(ILogger<TimedTaskService> logger, VirtualMachineDB context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// 邮件任务
        /// </summary>
        public void EmailTask()
        {
            var info = from n in _context.MachApplyAndReturn
                       where n.OprationType == 0 && n.ExamineResult == 2
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
                                         where m.PersonnelId == machApplyAndReturns[i].ApplyUserID && n.MachineId == machApplyAndReturns[i].MachineInfoID
                                         select new ReturnMachineInfoApplyData
                                         {
                                             MachineInfo = n,
                                             CommonPersonnelInfo = m
                                         };

                        var ss = personInfo.ToList();
                        //获取到人员信息
                        var data = personInfo.ToList().ElementAt(0);
                        //发送邮件
                        SendMail(data.CommonPersonnelInfo, data.MachineInfo, machApplyAndReturns[i]).Wait();
                        //添加日志记录 TODO
                    }
                }
                else
                {
                    return;
                }
            }
            else {
                return;
            }
        }


        /// <summary>
        /// 发送邮件
        /// </summary>
        public async Task SendMail(CommonPersonnelInfo personnelInfo, MachineInfo machineInfo,MachApplyAndReturn mach)
        {
            EmailHelper helper = new EmailHelper();
            var subject = "虚拟机到期提醒";
            var content = $"你申请的虚拟机（{machineInfo.MachineIP}）即将在{mach.ResultTime}到期，请及时归还或者续期，否则在到期之后系统将强制回收该虚拟机";
            var address = new MailboxAddress[] { new MailboxAddress(personnelInfo.Mailbox) };
            await helper.SendEMailAsync(subject, content, address);
        }
    }
}
