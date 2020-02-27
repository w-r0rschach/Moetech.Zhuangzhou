using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vSphereRestWrapper.CIS;
using vSphereRestWrapper.VCenter;

namespace Moetech.Zhuangzhou.Interface
{
    /// <summary>
    /// vSphere接口
    /// </summary>
    public interface IVsphere
    {
        /// <summary>
        /// 连接
        /// </summary>
        Task<bool> Connect(string sessionURl, string userName, string password);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="sessionId"></param>
        Task<List<VMItem>> ListAsync();
    }
}
