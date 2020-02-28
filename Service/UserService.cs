using Microsoft.EntityFrameworkCore;
using Moetech.Zhuangzhou.Common;
using Moetech.Zhuangzhou.Common.EnumDefine;
using Moetech.Zhuangzhou.Data;
using Moetech.Zhuangzhou.Interface;
using Moetech.Zhuangzhou.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Service
{
    /// <summary>
    /// 用户
    /// </summary>
    public class UserService : IUser
    {
        /// <summary>
        /// 每页条数
        /// </summary>
        private readonly int pageSize = 10;

        /// <summary>
        /// 数据量上下文
        /// </summary>
        private VirtualMachineDB _context;

        public UserService(VirtualMachineDB context)
        {
            _context = context;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userName">登录账号</param>
        /// <param name="userPwd">登录密码</param>
        /// <returns></returns>
        public CommonPersonnelInfo Login(string userName, string userPwd)
        {
            return _context.CommonPersonnelInfo.FirstOrDefault(o => o.UserName.Equals(userName) && o.Password.Equals(userPwd));
        }

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <returns></returns>
        public CommonPersonnelInfo GetPersonnelInfo(int id)
        {
            var personeInfo = from m in _context.CommonPersonnelInfo.Where(s => s.PersonnelId == id) select m;
            return personeInfo.FirstOrDefault();
        }

        /// <summary>
        /// 验证数据是否存在
        /// 实现思路：
        /// 根据传入的personnelId存人是修改还是添加，如果是添加则根据用户名查询数据库是否存在；
        /// 如果为修改则排除当前员工，在进行查询是否存在这个用户名。
        /// </summary>
        /// <Author>rendelang</Author>
        /// <param name="userName">用户名</param>
        /// <param name="personnelId">用户Id</param>
        /// <returns>true 存在；反之不存在</returns>
        public bool CheckUserName(object userName, OperationUserType operation, int personnelId = 0)
        {
            List<CommonPersonnelInfo> personnelInfos;
            if (operation == OperationUserType.USERNAME)
            {
                if (personnelId > 0)
                {
                    var info = from m in _context.CommonPersonnelInfo.Where(s => s.UserName == userName.ToString() && s.PersonnelId != personnelId) select m;
                    personnelInfos = info.ToList();
                }
                else
                {
                    var info = from m in _context.CommonPersonnelInfo.Where(s => s.UserName == userName.ToString()) select m;
                    personnelInfos = info.ToList();
                }
            }
            else
            {
                if (personnelId > 0)
                {
                    var info = from m in _context.CommonPersonnelInfo.Where(s => s.PersonnelNo == (int)userName && s.PersonnelId != personnelId) select m;
                    personnelInfos = info.ToList();
                }
                else
                {
                    var info = from m in _context.CommonPersonnelInfo.Where(s => s.PersonnelNo == (int)userName) select m;
                    personnelInfos = info.ToList();
                }
            }

            if (personnelInfos.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 查询用户信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public Task<PaginatedList<CommonPersonnelInfo>> GetUserInfo(string name, int? pageIndex = 1)
        {
            var list = from c in _context.CommonPersonnelInfo select c;

            if (!string.IsNullOrWhiteSpace(name))
            {
                list = list.Where(o => o.PersonnelName.Contains(name));
            }
            return PaginatedList<CommonPersonnelInfo>.CreateAsync(list, pageIndex ?? 1, pageSize);
        }

        /// <summary>
        /// 根据主键查询实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public Task<CommonPersonnelInfo> Details(int id)
        {
            return _context.CommonPersonnelInfo.FirstOrDefaultAsync(m => m.PersonnelId == id);
        }

        /// <summary>
        /// 创建用户信息
        /// </summary>
        /// <param name="info">用户实体对象</param>
        /// <returns></returns>
        public void Create(CommonPersonnelInfo info)
        {
            _context.Add(info);
            _context.SaveChanges();
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="info"></param>
        /// <returns></returns>
        public void Edit(CommonPersonnelInfo info)
        {
            _context.Update(info);
            _context.SaveChanges();
        }

        /// <summary>
        /// 根据用户主键删除用户实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public void DeleteConfirmed(int id)
        {
            var commonPersonnelInfo = Details(id).Result;
            _context.CommonPersonnelInfo.Remove(commonPersonnelInfo);
            _context.SaveChanges();
        }

        /// <summary>
        /// 返回最大员工编号
        /// </summary>
        /// <returns></returns>
        public int GetMaxPersonnelNo()
        {
            return _context.CommonPersonnelInfo.Max(m => m.PersonnelNo);
        }
    }
}
