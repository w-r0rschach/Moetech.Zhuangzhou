using Moetech.Zhuangzhou.Common;
using Moetech.Zhuangzhou.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Interface
{
    /// <summary>
    /// 虚拟机管理接口
    /// </summary>
    public interface IVmwareManage
    {
        /// <summary>
        /// 查询全部虚拟机
        /// </summary>
        /// <param name="name">申请人员</param>
        /// <param name="type">操作系统</param>
        /// <param name="status">虚拟机状态</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        Task<PaginatedList<ReturnMachineInfoApplyData>> SelectAll(string name = "", int? type = -1, int? status = -1, int? pageIndex = 1);

        /// <summary>
        /// 查询审批虚拟机
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        Task<PaginatedList<ReturnMachineInfoApplyData>> SelectApprove(int? pageIndex = 1);

        /// <summary>
        /// 提交审批虚拟机
        /// </summary>
        /// <param name="mid">虚拟机信息ID</param>
        /// <param name="aid">申请归还信息ID</param>
        /// <param name="state">状态1：拒绝  2：同意</param>
        /// <param name="userid">用户ID</param>
        /// <returns></returns>
        Task<int> SubmitApprove(int mid, int aid, int state, int userid);

        /// <summary>
        /// 回收虚拟机
        /// </summary>
        /// <param name="mid">虚拟机信息ID</param>
        /// <param name="rid">申请虚拟机记录ID</param>
        /// <returns></returns>
        Task<int> Recycle(int mid, int rid);

        /// <summary>
        /// 查询单台虚拟机数据
        /// </summary>
        /// <param name="id">虚拟机ID</param>
        /// <returns></returns>
        Task<MachineInfo> Details(int? id);

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="machineInfo">虚拟机对象</param>
        /// <returns></returns>
        Task<int> Save(MachineInfo machineInfo);

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="machineInfo">虚拟机对象</param>
        /// <returns></returns>
        Task<int> Update(MachineInfo machineInfo);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="machineInfo">虚拟机对象</param>
        /// <returns></returns>
        Task<int> Delete(MachineInfo machineInfo);
    }
}
