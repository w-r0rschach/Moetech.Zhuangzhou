using Moetech.Zhuangzhou.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vSphereRestWrapper.CIS;
using vSphereRestWrapper.VCenter;

namespace Moetech.Zhuangzhou.vSphere
{
    public class VSphereRestWrapper : IVsphere
    {
        /// <summary>
        /// Session管理对象
        /// </summary>
        public Session _Session { get; private set; }

        /// <summary>
        /// SessionId
        /// </summary>
        public string _SessionId { get; private set; }

        /// <summary>
        /// 虚拟机管理对象
        /// </summary>
        public VM _Vm { get; private set; }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="sessionURl"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> Connect(string sessionURl, string userName, string password)
        {
            _Session = new Session(sessionURl);
            _Vm = new VM(sessionURl);

            try
            {
                _SessionId = await _Session.CreateAsync(userName, password);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<VMItem>> ListAsync()
        {
            return await _Vm.ListAsync(_SessionId);
        }
    }
}
