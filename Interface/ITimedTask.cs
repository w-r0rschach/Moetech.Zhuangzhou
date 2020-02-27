using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Interface
{
    /// <summary>
    /// 定时任务
    /// </summary>
    public interface ITimedTask
    {
        /// <summary>
        /// 邮件任务
        /// </summary>
        void EmailTask();
    }
}
