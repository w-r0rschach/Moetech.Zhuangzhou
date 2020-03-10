using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Moetech.Zhuangzhou.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Common
{
    public class WebSocketHandle
    {
        WebSocket socket;
        public WebSocketHandle(WebSocket socket)
        {
            this.socket = socket;
            
        }

        /// <summary>
        /// 创建链接
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name=""></param>
        /// <returns></returns>
        private static async Task Acceptor(HttpContext httpContext, Func<Task> n)
        {

            if (!httpContext.WebSockets.IsWebSocketRequest)
                return;
            var socket = await httpContext.WebSockets.AcceptWebSocketAsync();
            CommonUserInfo.WebSocket = socket;
            var result = await RecvAsync(socket, CancellationToken.None);

        }

        /// <summary>
        /// 接收客户端数据
        /// </summary>
        /// <param name="webSocket">webSocket 对象</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<string> RecvAsync(WebSocket webSocket, CancellationToken cancellationToken)
        {
            WebSocketReceiveResult result;
            do
            {
                var buffer = new ArraySegment<byte>(new byte[1024 * 8]);
                result = await webSocket.ReceiveAsync(buffer, cancellationToken);
                if (result.MessageType.ToString() == "Close")
                {
                    break;
                }                
            } while (result.EndOfMessage);
            return "123";
        }
        /// <summary>
        /// 向客户端发送数据 
        /// </summary>
        /// <param name="msg">数据</param>
        /// <param name="webSocket">socket对象  sleep 心跳周期</param>
        /// <returns></returns>
        public static async Task SendAsync(MessageWarn msg, WebSocket webSocket)
        {
            try
            {
                //业务逻辑
                CancellationToken cancellation = default(CancellationToken);
                byte[] buf = Encoding.Default.GetBytes(JsonConvert.SerializeObject(msg));
                var segment = new ArraySegment<byte>(buf);
                await webSocket.SendAsync(segment, WebSocketMessageType.Text, true, cancellation);
            }
            catch (Exception ex)
            {
            }
        }

        /// 路由绑定处理
        /// </summary>
        /// <param name="app"></param>
        public static void Map(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.Use(WebSocketHandle.Acceptor);
        }
    }
}
