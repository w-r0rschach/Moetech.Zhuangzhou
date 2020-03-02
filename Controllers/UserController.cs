using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
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

        public UserController(IUser user)
        {
            _user = user;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public IActionResult Login()
        {
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
                return RedirectToAction("Index", "Manage");
            }

            return RedirectToAction("Index", "Vmware");
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        public IActionResult LoginOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        } 
    }
}