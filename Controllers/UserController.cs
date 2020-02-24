using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moetech.Zhuangzhou.Data;
using Moetech.Zhuangzhou.Models;

namespace Moetech.Zhuangzhou.Controllers
{
    public class UserController : Controller
    {
        /// <summary>
        /// 数据量上下文
        /// </summary>
        private VirtualMachineDB _db;

        public UserController(VirtualMachineDB context)
        {
            _db = context;
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
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(userName))
            {
                ViewData["Message"] = "账号或密码错误。";
                return View("Views/User/Login.cshtml");
            }

            IEnumerable<CommonPersonnelInfo> users = from us in _db.CommonPersonnelInfo
                                                     where us.UserName == userName && us.Password == userPwd
                                                     select us;

            if (users.Count() == 0)
            {
                ViewData["Message"] = "账号或密码错误。";
                return View("Views/User/Login.cshtml");
            }

            CommonPersonnelInfo info = users.ElementAt(0);
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(info);
            // 存入Session
            HttpContext.Session.Set("User", System.Text.Encoding.UTF8.GetBytes(json));

            if (info.DepId == 1)
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