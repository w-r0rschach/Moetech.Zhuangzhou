using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moetech.Zhuangzhou.Models;

namespace Moetech.Zhuangzhou.Data
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    public class VirtualMachineDB : DbContext
    {
        public VirtualMachineDB(DbContextOptions<VirtualMachineDB> options)
            : base(options)
        {
        }


        /// <summary>
        /// 权限
        /// </summary>
        public DbSet<CommonAuthority> CommonAuthority { get; set; }

        /// <summary>
        /// 角色关联
        /// </summary>
        public DbSet<CommonCorrelation> CommonCorrelation { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public DbSet<CommonDepartment> CommonDepartment { get; set; }

        /// <summary>
        /// 员工信息
        /// </summary>
        public DbSet<CommonPersonnelInfo> CommonPersonnelInfo { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public DbSet<CommonRole> CommonRole { get; set; }

        /// <summary>
        /// 角色权限关联
        /// </summary>
        public DbSet<CommonRoleAuthority> CommonRoleAuthority { get; set; }

        /// <summary>
        /// 申请归还信息
        /// </summary>
        public DbSet<MachApplyAndReturn> MachApplyAndReturn { get; set; }

        /// <summary>
        /// 虚拟机信息
        /// </summary>
        public DbSet<MachineInfo> MachineInfo { get; set; }

        /// <summary>
        /// 日志信息
        /// </summary>
        public DbSet<Logs> Logs { get; set; }

        /// <summary>
        /// 消息提醒
        /// </summary>
        public DbSet<MessageWarn> MessageWarns { get; set; }

    }
}
