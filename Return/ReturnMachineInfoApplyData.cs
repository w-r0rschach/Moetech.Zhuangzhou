using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moetech.Zhuangzhou.Models;

namespace Moetech.Zhuangzhou.Return
{
    /// <summary>
    /// 虚拟机信息审批数据
    /// </summary>
    public class ReturnMachineInfoApplyData
    {
        /// <summary>
        /// 虚拟机信息
        /// </summary>
        public MachineInfo MachineInfo { get; set; }

        /// <summary>
        /// 申请归还信息
        /// </summary>
        public MachApplyAndReturn MachApplyAndReturn { get; set; }

        /// <summary>
        /// 员工信息
        /// </summary>
        public CommonPersonnelInfo CommonPersonnelInfo { get; set; }
    }
}
