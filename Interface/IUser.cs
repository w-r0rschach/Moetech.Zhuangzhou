using Moetech.Zhuangzhou.Common;
using Moetech.Zhuangzhou.Common.EnumDefine;
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

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <returns></returns>
        CommonPersonnelInfo GetPersonnelInfo(int id);

        /// <summary>
        /// 验证数据是否存在
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="operation">操作类型</param>
        /// <param name="personnelId">用户Id</param>
        /// <returns> true 存在；反之不存在/returns>
        bool CheckUserName(object userName, OperationUserType operation, int personnelId = 0);

        /// <summary>
        /// 查询用户信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        Task<PaginatedList<CommonPersonnelInfo>> GetUserInfo(string name, int? pageIndex = 1);

        /// <summary>
        /// 根据主键查询实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        Task<CommonPersonnelInfo> Details(int id);

        /// <summary>
        /// 创建用户信息
        /// </summary>
        /// <param name="info">用户实体对象</param>
        /// <returns></returns>
        Task CreateAsync(CommonPersonnelInfo info);

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="info"></param>
        /// <returns></returns>
        void Edit(CommonPersonnelInfo info);

        /// <summary>
        /// 根据用户主键删除用户实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        void DeleteConfirmed(int id);

        /// <summary>
        /// 获取最大员工工号
        /// </summary>
        /// <returns></returns>
        int GetMaxPersonnelNo();
    }
}
