using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Common
{
    public class ReturnData
    {
        /// <summary>
        /// 操作系统
        /// 0：Windows
        /// 1：Linux
        /// </summary>
        [Display(Name = "操作系统")]
        public string MachineSystem { get; set; }

        /// <summary>
        /// 硬盘大小/G
        /// </summary>
        [Display(Name = "硬盘大小/G")]
        public double MachineDiskCount { get; set; }

        /// <summary>
        /// 内存大小/G
        /// </summary>
        [Display(Name = "内存大小/G")]
        public double MachineMemory { get; set; }
        /// <summary>
        /// 申请人员ID
        /// </summary>
        public int ApplyUserID { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime ApplyTime { get; set; }
        /// <summary>
        /// 操作类型
        /// 0：申请
        /// 1：归还
        /// </summary>
        public int OprationType { get; set; } 
        /// <summary>
        /// 申请人姓名
        /// </summary>
        public string AppUserName { get; set; }
        /// <summary>
        /// 审批结果
        /// 0:待审批
        /// 1：拒绝
        /// 2：同意
        /// </summary>
        public int ExamineResult { get; set; }
        /// <summary>
        /// 归还时间
        /// </summary>
        public DateTime ResultTime { get; set; }
        /// <summary>
        /// 申请原因
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 申请数量
        /// </summary>
        public int NumberCount { get; set; }
    }
}
