using Moetech.Zhuangzhou.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Interface
{
    /// <summary>
    /// 用户接口
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userName">登录账号</param>
        /// <param name="userPwd">登录密码</param>
        /// <returns></returns>
        CommonPersonnelInfo Login(string userName, string userPwd);
    }
}
