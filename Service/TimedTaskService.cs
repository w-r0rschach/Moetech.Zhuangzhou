using Microsoft.Extensions.Logging;
using Moetech.Zhuangzhou.Data;
using Moetech.Zhuangzhou.Interface;
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
            var list = _context.CommonPersonnelInfo.ToList();

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(list);

            _logger.LogInformation(json);
        }
    }
}
