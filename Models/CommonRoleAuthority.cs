using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Models
{
    /// <summary>
    /// 角色权限关联
    /// </summary>
    public class CommonRoleAuthority
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        public int RoleAuthorityId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 权限ID
        /// </summary>
        public int AuthorityId { get; set; }
    }
}
