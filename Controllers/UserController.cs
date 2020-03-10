using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MimeKit;
using Moetech.Zhuangzhou.Common;
using Moetech.Zhuangzhou.Common.EnumDefine;
using Moetech.Zhuangzhou.Data;
using Moetech.Zhuangzhou.Email;
using Moetech.Zhuangzhou.Interface;
using Moetech.Zhuangzhou.Models;
using Newtonsoft.Json;

namespace Moetech.Zhuangzhou.Controllers
{
    public class UserController : Controller
    {
        /// <summary>
        /// 用户接口
        /// </summary>
        private IUser _user;
        /// <summary>
        /// 日志接口
        /// </summary>
        private ILogs _log;
        public UserController(IUser user,ILogs logs)
        {
            _log = logs;
            _user = user;  
            _log.LoggerInfo("用户登录-初始化","用户登录初始化参数");
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public IActionResult Login()
        { 
            _log.LoggerInfo("用户登录-初始化","初始化登录视图");
            return View();
        }

        /// <summary>
        ///处理登录
        /// </summary>
        /// <param name="userName">账号</param>
        /// <param name="userPwd">密码</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DoLogin(string userName, string userPwd)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(userPwd))
            {
                ViewData["Message"] = "账号或密码错误。";
                return View("Views/User/Login.cshtml");
            }

            var user = _user.Login(userName, userPwd);
            //根据用户信息找到消息提醒记录
            var messageWarns = _user.SelevtMessageWarn(user);
            if (messageWarns != null)
            {
                CommonUserInfo.MessageWarns = JsonConvert.SerializeObject(messageWarns);
            }
            else 
            {
                CommonUserInfo.MessageWarns = JsonConvert.SerializeObject(new List<MessageWarn>());
            }
            TempData["MessageWarns"] = CommonUserInfo.MessageWarns;
            if (user == null)
            {
                ViewData["Message"] = "账号或密码错误。";
                return View("Views/User/Login.cshtml");
            }
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(user);
            // 存入Session
            HttpContext.Session.Set("User", System.Text.Encoding.UTF8.GetBytes(json));
            if (user.DepId == 1)
            {
                _log.LoggerInfo( "用户登录-登录处理", $"管理员：{user.PersonnelName} 登录成功", 
                    user.PersonnelId, LogLevel.Information, OperationLogType.SELECT );

                return RedirectToAction("Index", "Manage");
            }
            else
            {
                _log.LoggerInfo( "用户登录-登录处理",$"普通用户：{user.PersonnelName} 登录成功",
                    user.PersonnelId, LogLevel.Information,OperationLogType.SELECT );

                return RedirectToAction("Index", "Vmware");
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        public IActionResult LoginOut()
        {
            // 当前用户信息
            CommonPersonnelInfo userInfo = JsonConvert.DeserializeObject<CommonPersonnelInfo>(HttpContext.Session.GetString("User"));

            string userType = userInfo.DepId == 1 ? "管理员" : "普通用户";
            _log.LoggerInfo( "用户登录-退出",$"{userType}：{userInfo.PersonnelName} 退出成功",
                userInfo.PersonnelId, LogLevel.Information,OperationLogType.NONE );

            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        } 
    }
}