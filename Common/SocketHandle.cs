using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moetech.Zhuangzhou.Data;
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
    public static class SocketHandler
    {
        /// <summary>
        /// 数据库实体
        /// </summary>
        public static VirtualMachineDB db;

        /// <summary>
        /// 接受socket,并连接
        /// </summary>
        /// <param name="context"></param>
        /// <param name="webSocket"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public async static Task SocketConnect(HttpContext context, WebSocket webSocket, IServiceScope scope)
        {
            db = new VirtualMachineDB(scope.ServiceProvider.GetRequiredService<DbContextOptions<VirtualMachineDB>>());
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            CommonUserInfo.ReceiveResult = result;
        }
    }
}
