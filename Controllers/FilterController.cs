using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moetech.Zhuangzhou.Models;
using Moetech.Zhuangzhou.Common;
using Moetech.Zhuangzhou.Data;
using Moetech.Zhuangzhou.Email;

namespace Moetech.Zhuangzhou.Controllers
{
    /// <summary>
    /// 拦截器
    /// </summary>
    public abstract class FilterController : Controller
    {
        /// <summary>
        /// 角色
        /// 0：普通
        /// 1：管理
        /// </summary>
        public abstract int[] Role { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string userJson = context.HttpContext.Session.GetString("User");
            if (string.IsNullOrWhiteSpace(userJson))
            {
                context.Result = RedirectToAction("Login", "User");
            }
            else
            {
                CommonPersonnelInfo commonPersonnelInfo = JsonConvert.DeserializeObject<CommonPersonnelInfo>(userJson);

                // 验证权限 0：普通 0,1:管理
                if (!Role.Contains(commonPersonnelInfo.DepId))
                {
                    ViewData["Title"] = "权限不足";
                    ViewData["Message"] = "<p>您的权限不足...</p>"
                                        + "<p>哎呀！您不能操作此模块，</p>"
                                        + "<p>建议您联系管理员开通此模块的操作权限！</p>"
                                        + "<a href='javascript: history.back(-1)'>点击返回</a>" ;

                    context.Result = View("Views/Shared/Tip.cshtml");
                }
                else
                {
                    // _Layout.cshtml页面使用
                    ViewBag.User = commonPersonnelInfo;
                    CommonUserInfo.UserInfo = commonPersonnelInfo;

                    List<MessageWarn> messageWarns = JsonConvert.DeserializeObject<List<MessageWarn>>(CommonUserInfo.MessageWarns);
                    if (messageWarns != null)
                    {
                        ViewData["MessageWarns"] = messageWarns;
                    }
                    else {
                        ViewData["MessageWarns"] = null;
                    }
                    base.OnActionExecuting(context);
                }
            }
        }
    }
}