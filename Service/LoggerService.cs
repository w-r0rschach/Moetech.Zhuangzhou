using log4net;
using log4net.Config;
using Microsoft.Extensions.Logging;
using Moetech.Zhuangzhou.Common.EnumDefine;
using Moetech.Zhuangzhou.Data;
using Moetech.Zhuangzhou.Interface;
using Moetech.Zhuangzhou.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Service
{
    /// <summary>
    /// 日志信息实现
    /// </summary>
    public class LoggerService : ILogs
    {  /// <summary>
       /// 数据量上下文
       /// </summary>
        private readonly VirtualMachineDB _context;
        Logs Logs = new Logs();

        public LoggerService(VirtualMachineDB context)
        {
            _context = context;
        }
        /// <summary>
        /// 日志操作对象
        /// </summary>
        private  ILog logger=LogManager.GetLogger(Startup.repository.Name, "LoggerInfo");
        /// <summary>
        /// 日志信息记录
        /// </summary>
        /// <param name="logs">日志信息操作类</param>
        /// <remarks>对象参数是说明:Content,ModuleName,不能为空，UserId不能小于0</remarks>
        public void LoggerInfo(Logs logs)
        {
            if (string.IsNullOrWhiteSpace(logs.Content) ||
                string.IsNullOrWhiteSpace(logs.ModuleName)||
                logs.UserId<0)
            {
                throw new NotImplementedException("日志对象参数值不合法!");
            }
            else
            {
                logs.OccurredTime = DateTime.Now;
                string message = $"模块:{logs.ModuleName}  类型:{logs.OpenationType}  内容:{logs.Content}";
                if (logs.UserId>0)
                {
                    ///将对象值存储到数据库中
                    _context.Add(logs);
                    _context.SaveChanges();
                }
                else
                {
                   // LogInfo(logs.Level,message);
                }
            }
        } 
        /// <summary>
        /// 日志写入到文件中
        /// </summary>
        /// <param name="level">日志等级</param>
        /// <param name="message">日志内容</param>
        private void LogInfo(LogLevel level,string message)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    logger.Info(message);
                    break;
                case LogLevel.Debug:
                    logger.Debug(message);
                    break;
                case LogLevel.Information:
                    logger.Info(message);
                    break;
                case LogLevel.Warning:
                    logger.Warn(message);
                    break;
                case LogLevel.Error:
                    logger.Error(message);
                    break;
                case LogLevel.Critical:
                    logger.Fatal(message);
                    break;
                default:
                    break;
            } 
        }
        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="moduleName">模块名称</param>
        /// <param name="level">日志等级</param>
        /// <param name="openationType">操作类型</param>
        /// <param name="Content">日志内容</param>
        public void LoggerInfo(string moduleName, string Content,int userId=0, LogLevel level=LogLevel.Information, 
            OperationLogType openationType=OperationLogType.NONE )
        {
            Logs.UserId = userId;
            Logs.ModuleName = moduleName;
            Logs.Level =level;
            Logs.OpenationType =openationType;
            Logs.Content = Content;
            LoggerInfo(Logs);
        }
    }
}
