using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Common.EnumDefine
{
    /// <summary>
    /// 日志操作类型
    /// </summary>
    public enum OperationLogType
    {
        /// <summary>
        /// 新增
        /// </summary>
        ADD = 0,
        /// <summary>
        /// 删除
        /// </summary>
        DELETE = 1,
        /// <summary>
        /// 查询
        /// </summary>
        SELECT = 2,
        /// <summary>
        /// 修改
        /// </summary>
        MODIFY = 3,
        /// <summary>
        /// 除增删改查其他类型
        /// </summary>
        NONE = 4
    }
}
