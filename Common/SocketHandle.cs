using Microsoft.AspNetCore.Http;
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
    public class SocketHandler
    {
        public async static Task SocketConnect(HttpContext context, WebSocket webSocket)
        {
            await RecvAsync(webSocket, CancellationToken.None);
        }

        public static async Task RecvAsync(WebSocket webSocket, CancellationToken cancellationToken)
        {
            WebSocketReceiveResult result;
            do
            {
                var ms = new MemoryStream();
                var buffer = new ArraySegment<byte>(new byte[1024 * 8]);
                result = await webSocket.ReceiveAsync(buffer, cancellationToken);
                if (result.MessageType.ToString() == "Close")
                {
                    break;
                }
                ms.Write(buffer.Array, buffer.Offset, result.Count - buffer.Offset);
                ms.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(ms);
                var s = reader.ReadToEnd();
                reader.Dispose();
                ms.Dispose();
                if (!string.IsNullOrEmpty(s))
                {
                    await SendAsync(s, webSocket);
                }
            } while (result.EndOfMessage);
        }


        /// <summary>
        /// 向客户端发送数据 
        /// </summary>
        /// <param name="msg">数据</param>
        /// <param name="webSocket">socket对象  sleep 心跳周期</param>
        /// <returns></returns>
        public static async Task SendAsync(string msg, WebSocket webSocket)
        {
            try
            {
                //业务逻辑
                CancellationToken cancellation = default(CancellationToken);
                var buf = Encoding.UTF8.GetBytes("213645");
                var segment = new ArraySegment<byte>(buf);
                await webSocket.SendAsync(segment, WebSocketMessageType.Text, true, cancellation);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
