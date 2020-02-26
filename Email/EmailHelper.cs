using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Moetech.Zhuangzhou.Common;

namespace Moetech.Zhuangzhou.Email
{
    /// <summary>
    /// 电子邮件辅助类
    /// </summary>
    public class EmailHelper
    {
        /// <summary>
        /// 邮件服务器Host
        /// </summary>
        public static string Host { get; private set; }

        /// <summary>
        /// 邮件服务器Port
        /// </summary>
        public static int Port { get; private set; }

        /// <summary>
        /// 邮件服务器是否是ssl
        /// </summary>
        public static bool UseSsl { get; private set; }

        /// <summary>
        /// 发送邮件的账号友善名称
        /// </summary>
        public static string UserName { get; private set; }

        /// <summary>
        /// 发送邮件的账号地址
        /// </summary>
        public static string UserAddress { get; private set; }

        /// <summary>
        /// 发现邮件所需的账号授权码
        /// </summary>
        public static string Password { get; private set; }

        public EmailHelper()
        {

        }

        public EmailHelper(EmailConfig emailConfig)
        {
            Host = emailConfig.Host;
            Port = emailConfig.Port;
            UseSsl = emailConfig.UseSsl;
            UserName = emailConfig.UserName;
            UserAddress = emailConfig.UserAddress;
            Password = emailConfig.AuthorizationCode;
        }

        /// <summary>
        /// 发送电子邮件，默认发送方为<see cref="UserAddress"/>
        /// </summary>
        /// <param name="subject">邮件主题</param>
        /// <param name="content">邮件内容主题</param>
        /// <param name="toAddress">接收方信息</param>
        /// <param name="textFormat">内容主题模式，默认TextFormat.Text</param>
        /// <param name="attachments">附件</param>
        /// <param name="dispose">是否自动释放附件所用Stream</param>
        /// <returns></returns>
        public async Task SendEMailAsync(string subject, string content, IEnumerable<MailboxAddress> toAddress, TextFormat textFormat = TextFormat.Text, IEnumerable<AttachmentInfo> attachments = null, bool dispose = true)
        {
            await SendEMailAsync(subject, content, new MailboxAddress[] { new MailboxAddress(UserName, UserAddress) }, toAddress, textFormat, attachments, dispose).ConfigureAwait(false);
        }

        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="subject">邮件主题</param>
        /// <param name="content">邮件内容主题</param>
        /// <param name="fromAddress">发送方信息</param>
        /// <param name="toAddress">接收方信息</param>
        /// <param name="textFormat">内容主题模式，默认TextFormat.Text</param>
        /// <param name="attachments">附件</param>
        /// <param name="dispose">是否自动释放附件所用Stream</param>
        /// <returns></returns>
        public async Task SendEMailAsync(string subject, string content, MailboxAddress fromAddress, IEnumerable<MailboxAddress> toAddress, TextFormat textFormat = TextFormat.Text, IEnumerable<AttachmentInfo> attachments = null, bool dispose = true)
        {
            await SendEMailAsync(subject, content, new MailboxAddress[] { fromAddress }, toAddress, textFormat, attachments, dispose).ConfigureAwait(false);
        }

        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="subject">邮件主题</param>
        /// <param name="content">邮件内容主题</param>
        /// <param name="fromAddress">发送方信息</param>
        /// <param name="toAddress">接收方信息</param>
        /// <param name="textFormat">内容主题模式，默认TextFormat.Text</param>
        /// <param name="attachments">附件</param>
        /// <param name="dispose">是否自动释放附件所用Stream</param>
        /// <returns></returns>
        public async Task SendEMailAsync(string subject, string content, IEnumerable<MailboxAddress> fromAddress, IEnumerable<MailboxAddress> toAddress, TextFormat textFormat = TextFormat.Text, IEnumerable<AttachmentInfo> attachments = null, bool dispose = true)
        {
            var message = new MimeMessage();
            message.From.AddRange(fromAddress);
            message.To.AddRange(toAddress);
            message.Subject = subject;
            var body = new TextPart(textFormat)
            {
                Text = content
            };
            MimeEntity entity = body;
            if (attachments != null)
            {
                var mult = new Multipart("mixed")
                {
                    body
                };
                foreach (var att in attachments)
                {
                    if (att.Stream != null)
                    {
                        var attachment = string.IsNullOrWhiteSpace(att.ContentType) ? new MimePart() : new MimePart(att.ContentType);
                        attachment.Content = new MimeContent(att.Stream);
                        attachment.ContentDisposition = new ContentDisposition(ContentDisposition.Attachment);
                        attachment.ContentTransferEncoding = att.ContentTransferEncoding;
                        attachment.FileName = ConvertHeaderToBase64(att.FileName, Encoding.UTF8);//解决附件中文名问题
                        mult.Add(attachment);
                    }
                }
                entity = mult;
            }
            message.Body = entity;
            message.Date = DateTime.Now;
            using (var client = new SmtpClient())
            {
                //创建连接
                await client.ConnectAsync(Host, Port, UseSsl).ConfigureAwait(false);
                await client.AuthenticateAsync(UserAddress, Password).ConfigureAwait(false);
                await client.SendAsync(message).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
                if (dispose && attachments != null)
                {
                    foreach (var att in attachments)
                    {
                        att.Dispose();
                    }
                }
            }
        }

        private string ConvertToBase64(string inputStr, Encoding encoding)
        {
            return Convert.ToBase64String(encoding.GetBytes(inputStr));
        }

        private string ConvertHeaderToBase64(string inputStr, Encoding encoding)
        {
            var encode = !string.IsNullOrEmpty(inputStr) && inputStr.Any(c => c > 127);
            if (encode)
            {
                return "=?" + encoding.WebName + "?B?" + ConvertToBase64(inputStr, encoding) + "?=";
            }
            return inputStr;
        }


        public static async Task Main(string[] args)
        {
            var subject = "测试多个邮件";
            var content = "Just a test!";
            var address = new MailboxAddress[] { new MailboxAddress("8009002214@qq.com") };

            await new EmailHelper().SendEMailAsync(subject, content, address);
        }
    }
}
