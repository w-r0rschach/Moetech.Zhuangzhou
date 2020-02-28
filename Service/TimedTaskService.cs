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
                                         where m.PersonnelId == machApplyAndReturns[i].ApplyUserID && n.MachineId == machApplyAndReturns[i].MachineInfoID
                                         select new ReturnMachineInfoApplyData
                                         {
                                             MachineInfo = n,
                                             CommonPersonnelInfo = m
                                         };
                        //获取到人员信息
                        var data = personInfo.ToList().ElementAt(0);
                        //发送邮件
                        string subject = "虚拟机到期提醒";
                        string content = $"你申请的虚拟机（{data.MachineInfo.MachineIP}）即将在（{machApplyAndReturns[i].ResultTime}）到期，请及时归还或者续期，否则在到期之后系统将强制回收该虚拟机。";
                        await SendMail(data.CommonPersonnelInfo.Mailbox,subject,content);
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
                                     where m.PersonnelId == maches[i].ApplyUserID && n.MachineId == maches[i].MachineInfoID
                                     select new ReturnMachineInfoApplyData
                                     {
                                         MachineInfo = n,
                                         CommonPersonnelInfo = m
                                     };
                    //回收虚拟机
                    int result = _vmwareManage.Recycle(maches[i].MachineInfoID, maches[i].ApplyAndReturnId);
                    //获取到人员信息
                    var data = personInfo.ToList().ElementAt(0);
                    //发送邮件
                    if (result == 1) 
                    {
                        var subject = "虚拟机到期回收通知";
                        var content = $"你申请的虚拟机（{data.MachineInfo.MachineIP}）在（{maches[i].ResultTime}）已到期，系统已自动回收该虚拟机。";
                        await SendMail(data.CommonPersonnelInfo.Mailbox,subject,content);
                        //添加日志信息 TODO
                    }
                }
                
            }
        }

        /// <summary>
        /// 发送邮件(到期提醒)
        /// </summary>
        /// <param name="Mailbox">邮箱</param>
        /// <param name="subject">邮件标题</param>
        /// <param name="content">邮件内容</param>
        /// <returns></returns>
        public async Task SendMail(string Mailbox, string subject, string content)
        {
            if (!string.IsNullOrWhiteSpace(Mailbox))
            {
                EmailHelper helper = new EmailHelper();
                var address = new MailboxAddress[] { new MailboxAddress(Mailbox) };
                await helper.SendEMailAsync(subject, content, address);
            }
        }
    }
}
