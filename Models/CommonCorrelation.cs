using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Models
{
    /// <summary>
    /// 角色关联
    /// </summary>
    public class CommonCorrelation
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        public int CorrelationId { get; set; }

        /// <summary>
        /// 员工ID
        /// </summary>
        public int PersonnelId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(200)]
        public string Remark { get; set; }
    }
}
