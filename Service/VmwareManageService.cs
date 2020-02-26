﻿
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moetech.Zhuangzhou.Common;
using Moetech.Zhuangzhou.Data;
using Moetech.Zhuangzhou.Interface;
using Moetech.Zhuangzhou.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Service
{
    /// <summary>
    /// 虚拟机管理
    /// </summary>
    public class VmwareManageService : IVmwareManage
    {
        /// <summary>
        /// 每页条数
        /// </summary>
        private readonly int pageSize = 10;

        /// <summary>
        /// 数据量上下文
        /// </summary>
        private readonly VirtualMachineDB _context;

        public VmwareManageService(VirtualMachineDB context)
        {
            _context = context;
        }

        /// <summary>
        /// 查询全部虚拟机
        /// </summary>
        /// <param name="name">申请人员</param>
        /// <param name="type">操作系统</param>
        /// <param name="status">虚拟机状态</param>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        public async Task<PaginatedList<ReturnMachineInfoApplyData>> SelectAll(string name = "", int? type = -1, int? status = -1, int? pageIndex = 1)
        {
            var list = from m1 in _context.MachineInfo
                       join m2 in (from m3 in _context.MachApplyAndReturn
                                   join cp in _context.CommonPersonnelInfo on m3.ApplyUserID equals cp.PersonnelId
                                   where m3.OprationType == 0 && m3.ExamineResult != 1
                                   select new ReturnMachineInfoApplyData
                                   {
                                       MachApplyAndReturn = m3,
                                       CommonPersonnelInfo = cp
                                   }) on m1.MachineId equals m2.MachApplyAndReturn.MachineInfoID
                       into re
                       from r in re.DefaultIfEmpty()
                       select new ReturnMachineInfoApplyData
                       {
                           MachineInfo = m1,
                           MachApplyAndReturn = r.MachApplyAndReturn,
                           CommonPersonnelInfo = r.CommonPersonnelInfo
                       };

            // 操作系统类型
            if (type != -1)
            {
                list = list.Where(o => o.MachineInfo.MachineSystem == type);
            }

            // 虚拟机状态
            if (status != -1)
            {
                list = list.Where(o => o.MachineInfo.MachineState == status);
            }

            // 申请人员
            if (!string.IsNullOrWhiteSpace(name))
            {
                list = list.Where(o => o.CommonPersonnelInfo.PersonnelName.Contains(name));
            }

            return await PaginatedList<ReturnMachineInfoApplyData>.CreateAsync(list.AsNoTracking(), pageIndex ?? 1, pageSize);
        }


        /// <summary>
        /// 查询审批虚拟机
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <returns></returns>
        public async Task<PaginatedList<ReturnMachineInfoApplyData>> SelectApprove(int? pageIndex = 1)
        {
            var list = from m1 in _context.MachineInfo
                       join m2 in _context.MachApplyAndReturn on m1.MachineId equals m2.MachineInfoID
                       join p3 in _context.CommonPersonnelInfo on m2.ApplyUserID equals p3.PersonnelId
                       where m2.OprationType == 0 && m2.ExamineUserID == -1 && m2.ExamineResult == 0
                       select new ReturnMachineInfoApplyData
                       {
                           MachineInfo = m1,
                           MachApplyAndReturn = m2,
                           CommonPersonnelInfo = p3
                       };

            return await PaginatedList<ReturnMachineInfoApplyData>.CreateAsync(list.AsNoTracking(), pageIndex ?? 1, pageSize);
        }

        /// <summary>
        /// 提交审批虚拟机
        /// </summary>
        /// <param name="mid">虚拟机信息ID</param>
        /// <param name="aid">申请归还信息ID</param>
        /// <param name="state">状态1：拒绝  2：同意</param>
        /// <param name="userid">用户ID</param>
        /// <returns></returns>
        public async Task<int> SubmitApprove(int mid, int aid, int state, int userid)
        {
            var list = from m1 in _context.MachApplyAndReturn
                       join m2 in _context.MachineInfo on m1.MachineInfoID equals m2.MachineId
                       where m1.MachineInfoID == mid && m1.ApplyAndReturnId == aid
                       select new ReturnMachineInfoApplyData
                       {
                           MachApplyAndReturn = m1,
                           MachineInfo = m2
                       };

            if (list.Count() == 0)
            {
                return -1;
            }
            else
            {
                foreach (var item in list)
                {
                    item.MachApplyAndReturn.ExamineUserID = userid;                      // 修改审批人员ID
                    item.MachApplyAndReturn.ExamineResult = state;                       // 修改审批结果
                    item.MachineInfo.MachineState = state == 2 ? 2 : 0;                  // 修改虚拟机状态

                    _context.MachApplyAndReturn.Update(item.MachApplyAndReturn);
                    _context.MachineInfo.Update(item.MachineInfo);
                }

                await _context.SaveChangesAsync();

                return 1;
            }
        }

        /// <summary>
        /// 回收虚拟机
        /// </summary>
        /// <param name="mid">虚拟机信息ID</param>
        /// <param name="rid">申请虚拟机记录ID</param>
        /// <returns></returns>
        public async Task<int> Recycle(int mid, int rid)
        {
            var list = from m1 in _context.MachineInfo
                       join m2 in _context.MachApplyAndReturn on m1.MachineId equals m2.MachineInfoID
                       where m1.MachineId == mid && m2.ApplyAndReturnId == rid
                       select new
                       {
                           MachineInfo = m1,
                           MachApplyAndReturn = m2
                       };

            if (list.Count() == 0)
            {
                return -1;
            }
            else
            {
                foreach (var item in list)
                {
                    if (item.MachineInfo.MachineState == 2)
                    {
                        item.MachineInfo.MachineState = 0;
                        item.MachApplyAndReturn.OprationType = 1;
                        item.MachApplyAndReturn.ResultTime = DateTime.Now;

                        _context.MachineInfo.Update(item.MachineInfo);
                        _context.MachApplyAndReturn.Update(item.MachApplyAndReturn);

                    }
                    else
                    {
                        return -2;
                    }
                }

                await _context.SaveChangesAsync();
                return 1;
            }
        }

        /// <summary>
        /// 查询单台虚拟机数据
        /// </summary>
        /// <param name="id">虚拟机ID</param>
        /// <returns></returns>
        public async Task<MachineInfo> Details(int? id)
        {
            return await _context.MachineInfo.FirstOrDefaultAsync(m => m.MachineId == id);
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="machineInfo">虚拟机对象</param>
        /// <returns></returns>
        public async Task<int> Save(MachineInfo machineInfo)
        {
            _context.MachineInfo.Add(machineInfo);
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="machineInfo">虚拟机对象</param>
        /// <returns></returns>
        public async Task<int> Update(MachineInfo machineInfo)
        {
            _context.Update(machineInfo);
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="machineInfo">虚拟机对象</param>
        /// <returns></returns>
        public async Task<int> Delete(MachineInfo machineInfo)
        {
            _context.MachineInfo.Remove(machineInfo);
            return await _context.SaveChangesAsync();
        }
    }
}
