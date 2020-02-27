using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Common
{
    /// <summary>
    /// vSphere配置
    /// </summary>
    public class VSphereConfig
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        public string SessionURl { get; set; }
    }
}
