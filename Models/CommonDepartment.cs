using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Models
{
    /// <summary>
    /// 部门
    /// </summary>
    public class CommonDepartment
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        public int DepId { get; set; }

        /// <summary>
        /// 上级部门ID
        /// 无上级部门为：0
        /// </summary>
        public int ParentNumber { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [Required, StringLength(20, MinimumLength = 1)]
        public string DepName { get; set; }
    }
}
