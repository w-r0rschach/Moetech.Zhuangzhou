using MimeKit;
using Moetech.Zhuangzhou.Common;
using Moetech.Zhuangzhou.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Email
{
    public static class SendMailFctory
    {
        /// <summary>
        /// 系统自动回收到期虚拟机发送邮件
        /// </summary>
        /// <param name="info"></param>
        public static async Task SysSendMailAsync(ReturnMachineInfoApplyData info) 
        {
            if (!string.IsNullOrWhiteSpace(info.CommonPersonnelInfo.Mailbox))
            {
                var subject = "虚拟机到期回收通知";
                var content = $"你申请的虚拟机（{info.MachineInfo.MachineIP}）在（{info.MachApplyAndReturn.ResultTime}）已到期，系统已自动回收该虚拟机。";
                EmailHelper helper = new EmailHelper();
                var address = new MailboxAddress[] { new MailboxAddress(info.CommonPersonnelInfo.Mailbox) };
                await helper.SendEMailAsync(subject, content, address);
            }
        }

        /// <summary>
        /// 系统提醒虚拟机即将到期发送邮件（3天）
        /// </summary>
        /// <param name="info"></param>
        public static async Task RemindSendMailAsync(ReturnMachineInfoApplyData info)
        {
            if (!string.IsNullOrWhiteSpace(info.CommonPersonnelInfo.Mailbox))
            {
                string subject = "虚拟机到期提醒通知";
                string content = $"你申请的虚拟机（{info.MachineInfo.MachineIP}）即将在（{info.MachApplyAndReturn.ResultTime}）到期，请及时归还或者续期，否则在到期之后系统将强制回收该虚拟机。";
                EmailHelper helper = new EmailHelper();
                var address = new MailboxAddress[] { new MailboxAddress(info.CommonPersonnelInfo.Mailbox) };
                await helper.SendEMailAsync(subject, content, address);
            }
        }    

        /// <summary>
        /// 管理员强制回收虚拟机发送邮件
        /// </summary>

        public static async Task AdminiSendMailAsync(ReturnMachineInfoApplyData info) 
        {
            if (!string.IsNullOrWhiteSpace(info.CommonPersonnelInfo.Mailbox))
            {
                string subject = "虚拟机到期回收通知";
                string content = $"由于检测到你申请的虚拟机（{info.MachineInfo.MachineIP}）许久未使用，管理员已强制回收该虚拟机。";
                EmailHelper helper = new EmailHelper();
                var address = new MailboxAddress[] { new MailboxAddress(info.CommonPersonnelInfo.Mailbox) };
                await helper.SendEMailAsync(subject, content, address);
            }
        }

        /// <summary>
        /// 新增员工时发送邮件
        /// </summary>
        /// <param name="info"></param>
        public static async Task PersonalSendMailAsync(CommonPersonnelInfo info)
        {
            if (!string.IsNullOrWhiteSpace(info.Mailbox))
            {
                string subject = "系统通知";
                string content = $"欢迎新用户{info.PersonnelName}\r\n" +
                    $"虚拟机的地址为https://localhost:44339/User/Login,你的登录账户名为{info.UserName}，密码为{info.Password}";
                EmailHelper helper = new EmailHelper();
                var address = new MailboxAddress[] { new MailboxAddress(info.Mailbox) };
                await helper.SendEMailAsync(subject, content, address);
            }
        }

        /// <summary>
        /// 审批发送邮件
        /// </summary>
        /// <param name="info"></param>
        public static async Task ApprovalSendMailAsync(ReturnMachineInfoApplyData info,int resultInt)
        {
            string resultStr = resultInt == 2 ? "同意" : "拒绝";
            if (!string.IsNullOrWhiteSpace(info.CommonPersonnelInfo.Mailbox))
            {
                string subject = "审批结果通知";
                string content =   $"管理员已{resultStr}你对虚拟机的申请。";
                EmailHelper helper = new EmailHelper();
                var address = new MailboxAddress[] { new MailboxAddress(info.CommonPersonnelInfo.Mailbox) };
                await helper.SendEMailAsync(subject, content, address);
            }
        }

        /// <summary>
        /// 审批提醒管理员
        /// </summary>
        /// <param name="info"></param>
        public static async Task ApprovalSendMailManageAsync(CommonPersonnelInfo info)
        {
            if (!string.IsNullOrWhiteSpace(info.Mailbox))
            {
                string subject = "申请通知";
                string content = $"{info.PersonnelName}正在申请虚拟机，请管理员尽快对申请做出审批。";
                EmailHelper helper = new EmailHelper();
                var address = new MailboxAddress[] { new MailboxAddress(info.Mailbox) };
                await helper.SendEMailAsync(subject, content, address);
            }
        }
    }
}
