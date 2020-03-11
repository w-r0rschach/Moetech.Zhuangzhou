using Moetech.Zhuangzhou.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Common
{
    public static class CommonUserInfo
    {
        /// <summary>
        /// 个人信息
        /// </summary>
        public static CommonPersonnelInfo UserInfo { set; get; }

        /// <summary>
        /// 消息提醒缓存信息
        /// </summary>
        internal static List<MessageWarn> MessageWarnList = new List<MessageWarn>();
    }
}
