using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Models
{
    /// <summary>
    /// 虚拟机信息
    /// </summary>
    public class MachineInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        [Display(Name = "编号")]
        public int MachineId { get; set; }

        /// <summary>
        /// 虚拟机IP
        /// </summary>
        [Display(Name = "虚拟机IP")] 
        [Required(ErrorMessage = "虚拟机IP是必需的。"),StringLength(20)]
        public string MachineIP { get; set; }

        /// <summary>
        /// 操作系统
        /// 0：Windows
        /// 1：Linux
        /// </summary>
        [Display(Name = "操作系统")]
        [Required(ErrorMessage = "操作系统是必需的。"), StringLength(50)]
        public string MachineSystem { get; set; }

        /// <summary>
        /// 硬盘大小/G
        /// </summary>
        [Display(Name = "硬盘大小/G")]
        [Required(ErrorMessage = "硬盘大小是必需的。")]
        public double MachineDiskCount { get; set; }

        /// <summary>
        /// 内存大小/G
        /// </summary>
        [Display(Name = "内存大小/G")]
        [Required(ErrorMessage = "内存大小是必需的。")]
        public double MachineMemory { get; set; }

        /// <summary>
        /// 虚拟机状态
        /// 0：空闲
        /// 1：申请中
        /// 2：使用中
        /// </summary>
        [Display(Name = "虚拟机状态")]
        public int MachineState { get; set; }

        /// <summary>
        /// 登录账号
        /// </summary>
        [Display(Name = "登录账号")]
        [Required(ErrorMessage = "登录账号是必需的。"),StringLength(20)]
        public string MachineUser { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        [Display(Name = "登录密码")]
        [Required(ErrorMessage = "登录密码是必需的。"),StringLength(20)]
        public string MachinePassword { get; set; }
    }
}
