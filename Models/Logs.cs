using Microsoft.Extensions.Logging;
using Moetech.Zhuangzhou.Common.EnumDefine;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Models
{
    /// <summary>
    /// 日志
    /// </summary>
    public class Logs
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        [Display(Name = "编号")]
        public int LogId { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        [Required, StringLength(255)]
        [Display(Name = "模块名称")]
        public string ModuleName { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        [Required]
        [Display(Name = "操作类型")]
        public OperationLogType OpenationType { get; set; }

        /// <summary>
        /// 日志等级
        /// </summary>
        [Required]
        [Display(Name = "日志等级")]
        public LogLevel Level { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [Required]
        [Display(Name = "用户编号")]
        public int UserId { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Required, StringLength(2048)]
        [Display(Name = "内容")]
        public string Content { get; set; }

        /// <summary>
        /// 发生时间
        /// </summary>
        [Required]
        [Display(Name = "发生时间")]
        public DateTime OccurredTime { get; set; }
    }
}
