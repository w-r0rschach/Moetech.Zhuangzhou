using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moetech.Zhuangzhou.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Common
{
    /// <summary>
    /// 启动定时任务
    /// </summary>
    public class StartTimedTask : IHostedService, IDisposable
    {
        /// <summary>
        /// 服务
        /// </summary>
        private readonly IServiceProvider _services;

        /// <summary>
        /// 定时器
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger _logger;

        public StartTimedTask(ILogger<StartTimedTask> logger, IServiceProvider services)
        {
            _services = services;
            _logger = logger;
        }

        /// <summary>
        /// 工作任务
        /// </summary>
        /// <param name="state"></param>
        public void WorkTask(object state)
        {
            // 注入数据库服务
            using var scope = _services.CreateScope();
            var task = scope.ServiceProvider.GetRequiredService<ITimedTask>();
            task.EmailTask();
        }

        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("启动定时任务");

            // 1天执行一次
            _timer = new Timer(WorkTask, null, TimeSpan.Zero, TimeSpan.FromDays(1));   
            // 1分钟执行一次
            //_timer = new Timer(WorkTask, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        /// <summary>
        /// 结束
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("停止定时任务");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
