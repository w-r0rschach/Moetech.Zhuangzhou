using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Models
{
    /// <summary>
    /// 申请归还信息
    /// </summary>
    public class MachApplyAndReturn
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        public int ApplyAndReturnId { get; set; }

        /// <summary>
        /// 操作类型
        /// 0：申请
        /// 1：归还
        /// </summary>
        public int OprationType { get; set; }

        /// <summary>
        /// 申请人员ID
        /// </summary>
        public int ApplyUserID { get; set; }

        /// <summary>
        /// 审批人员ID
        /// </summary>
        public int ExamineUserID { get; set; }

        /// <summary>
        /// 虚拟机ID
        /// </summary>
        public int MachineInfoID { get; set; }

        /// <summary>
        /// 审批结果
        /// 0:待审批
        /// 1：拒绝
        /// 2：同意
        /// </summary>
        public int ExamineResult { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime ApplyTime { get; set; }

        /// <summary>
        /// 归还时间
        /// </summary>
        public DateTime ResultTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(255)]
        public string Remark { get; set; }
    }
}
