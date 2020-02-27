using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Models
{
    /// <summary>
    /// 返回虚拟机审批所需参数返回类
    /// </summary>
    public class ReturnSubmitApproeId
    {
        /// <summary>
        /// 虚拟机Id
        /// </summary>
        public int MachineId { get; set; }
        /// <summary>
        /// 虚拟机申请单ID
        /// </summary>
        public int ApplyAndReturnId { get; set; }
        /// <summary>
        /// 审批状态
        /// </summary>
        public int ApproveState { get; set; }
        /// <summary>
        /// userId
        /// </summary>
        public int MyProperty { get; set; }
    }
}
