using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Models
{
    /// <summary>
    /// 角色
    /// </summary>
    public class CommonRole
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        public int RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [Required, StringLength(20, MinimumLength = 1)]
        public string RoleName { get; set; }
    }
}
