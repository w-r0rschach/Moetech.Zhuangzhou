using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Models
{
    /// <summary>
    /// 权限
    /// </summary>
    public class CommonAuthority
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        public int AuthorityId { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        [Required, StringLength(20, MinimumLength = 1)]
        public int AuthorityName { get; set; }
    }
}
