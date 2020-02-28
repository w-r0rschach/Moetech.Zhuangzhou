using Moetech.Zhuangzhou.Interface;
using Moetech.Zhuangzhou.Models;
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
        public async Task<List<MachineInfo>> ListAsync()
        {
            ///获取虚拟机列表
            //List<VMItem> vmItems = await _Vm.ListAsync(_SessionId);
            //List<MachineInfo> _listMachineInfo = new List<MachineInfo>();
            //long iskCount = 0;

            //foreach (var item in vmItems)
            //{
            //    ///实例化虚拟机接收实体
            //    MachineInfo machineInfo = new MachineInfo();
            //    ///获取虚拟机详细信息对象
            //    var details = await _Vm.DetailAsync(_SessionId,item.ID);

            //   // machineInfo.MachineSystem = details.GuestOS;
            //    machineInfo.MachineIP = item.ID;
            //    machineInfo.MachineMemory = details.Memory.Size / 1024;
            //    //  machineInfo.MachineDiskCount=details.
            //    ///获取虚拟机磁盘大小
            //    foreach (var itemDisks in details.Disks)
            //    {
            //        iskCount += itemDisks.Value.Capacity;
            //    }
            //    machineInfo.MachineDiskCount = iskCount / 1024 / 1024;
            //}
            return null;
        }
    }
}
