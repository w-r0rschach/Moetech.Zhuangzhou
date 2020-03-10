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
        public static CommonPersonnelInfo UserInfo { set; get; }

        public static WebSocketReceiveResult ReceiveResult { set; get; }

        public static WebSocket WebSocket { set; get; }

        public static string MessageWarns { set; get; }
    }
}
