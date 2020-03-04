using Microsoft.Extensions.Logging;
using Moetech.Zhuangzhou.Common.EnumDefine;
using Moetech.Zhuangzhou.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Interface
{
   public interface ILogs
    {
        /// <summary>
        /// 日志信息记录
        /// </summary>
        /// <param name="logs">日志信息操作类</param>
        /// <remarks>对象参数是说明:Content,ModuleName,不能为空，UserId不能小于0</remarks>
        Task LoggerInfoAsync(Logs logs);
        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="moduleName">模块名称</param>
        /// <param name="level">日志等级</param>
        /// <param name="openationType">操作类型</param>
        /// <param name="Content">日志内容</param>
        Task LoggerInfo(string moduleName, string Content,int userId=0, LogLevel level=LogLevel.Information, OperationLogType openationType=OperationLogType.NONE);
    }
}
