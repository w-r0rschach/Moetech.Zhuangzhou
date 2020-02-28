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


    }
}
