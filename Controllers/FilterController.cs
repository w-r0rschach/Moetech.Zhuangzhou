using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moetech.Zhuangzhou.Models;

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
        public abstract  int Role { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string userJson = context.HttpContext.Session.GetString("User");
            if (string.IsNullOrWhiteSpace(userJson))
            {
                context.Result = new RedirectResult("/User/Login");
                return;
            }

            ViewBag.User = JsonConvert.DeserializeObject<CommonPersonnelInfo>(userJson);

            // 验证权限 0：普通 1:管理
            if (ViewBag.User.DepId != Role)
            {
                context.Result = new RedirectResult("/User/LoginOut");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
