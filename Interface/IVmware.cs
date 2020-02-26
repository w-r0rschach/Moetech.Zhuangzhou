using Microsoft.AspNetCore.Mvc;
using Moetech.Zhuangzhou.Common;
using Moetech.Zhuangzhou.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Interface
{
    /// <summary>
    /// 虚拟机接口
    /// </summary>
    public interface IVmware
    {
        /// <summary>
        /// 查询虚拟机信息
        /// </summary>
        /// <returns></returns>
        IQueryable<MachineInfo> SelectVmware();

        /// <summary>
        /// 提交申请 
        /// </summary>
        IQueryable<MachineInfo> SubmitApplication(int machineSystem, int machineDiskCount, int machineMemory, int applyNumber, string remark, CommonPersonnelInfo userInfo);

        /// <summary>
        /// 我的虚拟机
        /// </summary>
        IQueryable<ReturnMachineInfoApplyData> MyVmware(CommonPersonnelInfo userInfo);

        /// <summary>
        ///  提前归还
        /// </summary>
        /// <param name="userInfo"></param>
        void EarlyReturn(int id,CommonPersonnelInfo userInfo);

        /// <summary>
        /// 续租
        /// </summary>
        IQueryable<MachApplyAndReturn> Renew(int id, CommonPersonnelInfo userInfo);
    }
}
