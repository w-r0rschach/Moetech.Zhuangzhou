using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moetech.Zhuangzhou.Interface;
using Moetech.Zhuangzhou.Models;
using Newtonsoft.Json;

namespace Moetech.Zhuangzhou.Controllers
{
    public class ResetController : FilterController
    {
        /// <summary>
        /// 用户接口
        /// </summary>
        private IUser _user;
        public ResetController(IUser user)
        {
            _user = user;
        }
        /// <summary>
        /// 角色 
        /// </summary>
        public override int[] Role { get; set; } = { 0,1 };
        public IActionResult Reset()
        {
            // 当前用户信息
            CommonPersonnelInfo userInfo = JsonConvert.DeserializeObject<CommonPersonnelInfo>(HttpContext.Session.GetString("User"));
            var personnelInfo = _user.GetPersonnelInfo(userInfo,userInfo.PersonnelId);
            return View(personnelInfo); 
        } 
        /// <summary>    
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public IActionResult ModifyPassWord(string pwd)
        {  // 当前用户信息
            CommonPersonnelInfo userInfo = JsonConvert.DeserializeObject<CommonPersonnelInfo>(HttpContext.Session.GetString("User"));
            if (_user.ModifyPassWord(userInfo,userInfo.PersonnelId, pwd) > 0)
            {
                return RedirectToAction("LoginOut", "User");
            }
            else
            {
                ViewData["Title"] = "密码修改失败";
                ViewData["Message"] = $"密码修改失败，请稍后重试！返回<a href='/PersonnelInfo/Reset'></a>";
                return View("Views/Shared/Tip.cshtml");
            }
        }
    }
}