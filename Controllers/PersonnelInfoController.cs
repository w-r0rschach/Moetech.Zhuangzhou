using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Moetech.Zhuangzhou.Data;
using Moetech.Zhuangzhou.Models;
using Moetech.Zhuangzhou.Common;
using Moetech.Zhuangzhou.Interface;
using Moetech.Zhuangzhou.Common.EnumDefine;

namespace Moetech.Zhuangzhou.Controllers
{
    public class PersonnelInfoController : FilterController
    {
        /// <summary>
        /// 用户接口
        /// </summary>
        private IUser _user;

        /// <summary>
        /// 每页条数
        /// </summary>
        private readonly int pageSize = 10;

        /// <summary>
        /// 角色 
        /// </summary>
        public override int[] Role { get; set; } = { 1 };

        public PersonnelInfoController(IUser user)
        {
            _user = user;
        }

        // GET: PersonnelInfo
        public async Task<IActionResult> Index(string name = "", int? pageIndex = 1)
        {
            var PagingList=await _user.GetUserInfo(name, pageIndex ?? 1);
            // 查询条件参数
            ViewBag.name = name;
            return View(PagingList);
        }

        // GET: PersonnelInfo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var commonPersonnelInfo = await _user.Details(id ?? 0);
            if (commonPersonnelInfo == null)
            {
                return NotFound();
            }
            return View(commonPersonnelInfo);
        }

        // GET: PersonnelInfo/Create
        public IActionResult Create()
        {
            CommonPersonnelInfo personnelInfo = new CommonPersonnelInfo();
            personnelInfo.PersonnelNo = _user.GetMaxPersonnelNo()+1;
            return View(personnelInfo);
        }

        // POST: PersonnelInfo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Create([Bind("PersonnelId,PersonnelNo,PersonnelName,DepId,Avatar,PersonnelSex,BirthDate,IdentityCard," +
            "IsWork,Nation,MaritalStatus,LiveAddress,Phone,WeChatAccount,Mailbox,Degree,Address,OnboardingTime,DepartureTime," +
            "TrialTime,IsStruggle,IsSecrecy,UserName,Password,AppMaxCount")] CommonPersonnelInfo commonPersonnelInfo)
        {
            if (_user.CheckUserName(commonPersonnelInfo.PersonnelNo, OperationUserType.WORKNUMBER))
            {
                ViewData["Title"] = "操作失败";
                ViewData["Message"] = $"员工工号:{commonPersonnelInfo.PersonnelNo},已存在！查看<a href='/PersonnelInfo/Index'>员工管理</a>";
                return View("Views/Shared/Tip.cshtml");
            }
            if (_user.CheckUserName(commonPersonnelInfo.UserName,OperationUserType.USERNAME))
            {
                ViewData["Title"] = "操作失败";
                ViewData["Message"] =$"用户名:{commonPersonnelInfo.UserName},已存在！查看<a href='/PersonnelInfo/Index'>员工管理</a>";
                return View("Views/Shared/Tip.cshtml");
            }
                if (ModelState.IsValid)
                {
                    _user.Create(commonPersonnelInfo);
                    return RedirectToAction(nameof(Index));
                }
             
            return View(commonPersonnelInfo);
        }

        // GET: PersonnelInfo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var commonPersonnelInfo =await _user.Details(id ?? 0);
            if (commonPersonnelInfo == null)
            {
                return NotFound();
            }
            return View(commonPersonnelInfo);
        }

        // POST: PersonnelInfo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("PersonnelId,PersonnelNo,PersonnelName,DepId,Avatar,PersonnelSex,BirthDate," +
            "IdentityCard,IsWork,Nation,MaritalStatus,LiveAddress,Phone,WeChatAccount,Mailbox,Degree,Address,OnboardingTime," +
            "DepartureTime,TrialTime,IsStruggle,IsSecrecy,UserName,Password,AppMaxCount")] CommonPersonnelInfo commonPersonnelInfo)
        {
            if (_user.CheckUserName(commonPersonnelInfo.PersonnelNo, OperationUserType.WORKNUMBER,id))
            {
                ViewData["Title"] = "操作失败";
                ViewData["Message"] = $"员工工号:{commonPersonnelInfo.PersonnelNo},已存在！查看<a href='/PersonnelInfo/Index'>员工管理</a>";
                return View("Views/Shared/Tip.cshtml");
            }
            if (_user.CheckUserName(commonPersonnelInfo.UserName,OperationUserType.USERNAME,id))
            {
                ViewData["Title"] = "操作失败";
                ViewData["Message"] = $"用户名:{commonPersonnelInfo.UserName},已存在！查看<a href='/PersonnelInfo/Index'>员工管理</a>";
                return View("Views/Shared/Tip.cshtml");
            }
            if (id != commonPersonnelInfo.PersonnelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _user.Edit(commonPersonnelInfo);
                }
                catch (DbUpdateConcurrencyException)
                {
                        throw;
                    
                }
                return RedirectToAction(nameof(Index));
            }
            return View(commonPersonnelInfo);
        }

        // GET: PersonnelInfo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var commonPersonnelInfo = await _user.Details(id ?? 0);
            if (commonPersonnelInfo == null)
            {
                return NotFound();
            }

            return View(commonPersonnelInfo);
        }

        // POST: PersonnelInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public  IActionResult DeleteConfirmed(int id)
        {
            _user.DeleteConfirmed(id);

            return RedirectToAction(nameof(Index));
        }

    }
}
