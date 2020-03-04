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
        IQueryable<MachineInfo> SelectVmware(CommonPersonnelInfo personnelInfo);

        /// <summary>
        /// 提交申请
        /// </summary>
        /// <param name="machineSystem">操作系统 0：Windows 1：Linux</param>
        /// <param name="machineDiskCount">硬盘大小/G</param>
        /// <param name="machineMemory">内存大小/G</param>
        /// <param name="applyNumber">申请数量</param>
        /// <param name="remark">备注</param>
        /// <param name="userInfo">当前用户信息</param>
        /// <returns>
        /// -1:申请失败
        /// 0:待审批
        /// 2:同意
        /// </returns>
        Task<int> SubmitApplication(string machineSystem, int machineDiskCount, 
            int machineMemory, int applyNumber, string remark, CommonPersonnelInfo userInfo);

        /// <summary>
        /// 我的虚拟机
        /// </summary>
        IQueryable<ReturnMachineInfoApplyData> MyVmware(CommonPersonnelInfo userInfo);

        /// <summary>
        /// 提前归还
        /// </summary>
        /// <param name="userInfo"></param>
        Task<bool> EarlyReturn(int id,CommonPersonnelInfo userInfo);

        /// <summary>
        /// 续租
        /// </summary>
        Task<bool> Renew(int id, CommonPersonnelInfo userInfo);

        /// <summary>
        /// 保存提醒消息记录
        /// </summary>
        /// <param name="messageWarn"></param>
        Task SaveMesageWarn(MessageWarn messageWarn);
    }
}
