using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moetech.Zhuangzhou.Common;
using Moetech.Zhuangzhou.Common.EnumDefine;
using Moetech.Zhuangzhou.Data;
using Moetech.Zhuangzhou.Email;
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
        /// 日志接口
        /// </summary>
        private readonly ILogs _logs;
        /// <summary>
        /// 数据量上下文
        /// </summary>
        private VirtualMachineDB _context;

        public UserService(VirtualMachineDB context,ILogs logs)
        {
            _context = context;
            _logs = logs;
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
        public CommonPersonnelInfo GetPersonnelInfo(CommonPersonnelInfo personnelInfo,int id)
        {
            var personeInfo = from m in _context.CommonPersonnelInfo.Where(s => s.PersonnelId == id) select m;
            _logs.LoggerInfo("用户信息-查询",$"{personnelInfo.PersonnelName} 查询了 用户：{personeInfo.FirstOrDefault().UserName} 的详细信息!",
                personnelInfo.PersonnelId,LogLevel.Information,OperationLogType.SELECT);
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
        public Task<CommonPersonnelInfo> Details(CommonPersonnelInfo personnelInfo,int id)
        {
            var personeInfo = _context.CommonPersonnelInfo.FirstOrDefaultAsync(m => m.PersonnelId == id);

            _logs.LoggerInfo("用户信息-查询", $"{personnelInfo.PersonnelName} 查询了 用户：{personeInfo.Result.UserName} 的详细信息!",
                personnelInfo.PersonnelId, LogLevel.Information, OperationLogType.SELECT);

            return personeInfo;
        }

        /// <summary>
        /// 创建用户信息
        /// </summary>
        /// <param name="info">用户实体对象</param>
        /// <returns></returns>
        public async Task CreateAsync(CommonPersonnelInfo personnelInfo,CommonPersonnelInfo info)
        {
          await  _logs.LoggerInfo("用户信息-新增", $"{personnelInfo.PersonnelName} 新增了用户名称：{info.UserName} 的用户!",
           personnelInfo.PersonnelId, LogLevel.Information, OperationLogType.ADD);

            _context.Add(info);
            _context.SaveChanges();
            //新增员工时发送邮件
            MessageWarn messageWarn = await SendMailFctory.PersonalSendMailAsync(info);
            _context.MessageWarns.Add(messageWarn);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="id">用户Id</param>
        /// <param name="info"></param>
        /// <returns></returns>
        public void Edit(CommonPersonnelInfo personnelInfo,CommonPersonnelInfo info)
        {
            _logs.LoggerInfo("用户信息-修改", $"{personnelInfo.PersonnelName} 更新了用户名称：{info.UserName} 的详细信息!",
        personnelInfo.PersonnelId, LogLevel.Information, OperationLogType.MODIFY);

            _context.Update(info);
            _context.SaveChanges();
        }

        /// <summary>
        /// 根据用户主键删除用户实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public void DeleteConfirmed(CommonPersonnelInfo personnelInfo,int id)
        {
            var commonPersonnelInfo = Details(personnelInfo,id).Result;
            _context.CommonPersonnelInfo.Remove(commonPersonnelInfo); 
            _context.SaveChanges();

            _logs.LoggerInfo("用户信息-删除", $"{personnelInfo.PersonnelName} 删除了用户名称：{commonPersonnelInfo.UserName} 的详细信息!",
       personnelInfo.PersonnelId, LogLevel.Information, OperationLogType.DELETE);

        }

        /// <summary>
        /// 返回最大员工编号
        /// </summary>
        /// <returns></returns>
        public int GetMaxPersonnelNo()
        {
            return _context.CommonPersonnelInfo.Max(m => m.PersonnelNo);
        }
        /// <summary>
        /// 根据主键修改用户密码
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="password">新密码</param>
        /// <returns></returns>
      public  int ModifyPassWord(CommonPersonnelInfo personnelInfo,int id, string password)
        {
            CommonPersonnelInfo commonPersonnel = _context.CommonPersonnelInfo.Where(s=>s.PersonnelId==id).FirstOrDefault();
            commonPersonnel.Password = password;

            _logs.LoggerInfo("用户信息-修改密码", $"{personnelInfo.PersonnelName} 修改了 用户名为：{commonPersonnel.UserName} 的密码!",
      personnelInfo.PersonnelId, LogLevel.Information, OperationLogType.DELETE);

            _context.Update(commonPersonnel); 
            return  _context.SaveChanges();
        }
    }
}
