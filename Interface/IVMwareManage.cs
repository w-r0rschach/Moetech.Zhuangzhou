﻿using Moetech.Zhuangzhou.Common;
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
        /// <param name="personnelInfo">登录用户信息</param>
        /// <returns></returns>
        Task<PaginatedList<ReturnMachineInfoApplyData>> SelectAll(CommonPersonnelInfo personnelInfo, string name = "", string type = "", int? status = -1, int? pageIndex = 1);

        /// <summary>
        /// 查询审批虚拟机
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        Task<PaginatedList<ReturnData>> SelectApprove(CommonPersonnelInfo personnelInfo, int? pageIndex = 1);

        /// <summary>
        /// 提交审批虚拟机
        /// </summary>
        /// <param name="mid">虚拟机信息ID</param>
        /// <param name="aid">申请归还信息ID</param>
        /// <param name="state">状态1：拒绝  2：同意</param>
        /// <param name="userid">用户ID</param>
        /// <returns></returns>
        Task<int> SubmitApprove(CommonPersonnelInfo personnelInfo, int mid, int aid, int state, int userid);

        /// <summary>
        /// 根据参数返回对需要Id
        /// </summary> 
        /// <param name="ApplyUserID">申请人</param>
        /// <param name="ApplyTime">申请时间</param>
        /// <param name="ResultTime">归还时间</param>
        /// <param name="Remark">申请原因</param>
        /// <param name="state">审批状态</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        Task<bool> ResultSubmitApprove(CommonPersonnelInfo personnelInfo, int ApplyUserID, DateTime ApplyTime, DateTime ResultTime, string Remark, int state, int userId);

        /// <summary>
        /// 回收虚拟机
        /// </summary>
        /// <param name="mid">虚拟机信息ID</param>
        /// <param name="rid">申请虚拟机记录ID</param>
        /// <returns></returns>
        int Recycle(CommonPersonnelInfo personnelInfo, int mid, int rid);

        /// <summary>
        /// 查询单台虚拟机数据
        /// </summary>
        /// <param name="id">虚拟机ID</param>
        /// <returns></returns>
        Task<MachineInfo> Details(CommonPersonnelInfo personnelInfo, int? id);

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="machineInfo">虚拟机对象</param>
        /// <returns></returns>
        Task<int> Save(MachineInfo machineInfo, CommonPersonnelInfo personnelInfo);

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="machineInfo">虚拟机对象</param>
        /// <returns></returns>
        Task<int> Update(CommonPersonnelInfo personnelInfo, MachineInfo machineInfo);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="machineInfo">虚拟机对象</param>
        /// <returns></returns>
        Task<int> Delete(CommonPersonnelInfo personnelInfo, MachineInfo machineInfo);
        /// <summary>
        /// 验证ip地址是否存在
        /// </summary>
        /// <param name="host">IP地址</param>
        /// <returns> true 存在  否则不存在</returns>
        bool CheckHost(string host, int machineId = 0);

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="machineId">虚拟机Id</param>
        /// <param name="applyPersonId">使用者id</param>
        /// <returns></returns>
        Task SendMail(int machineId,int applyPersonId);
    }
}
