using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Moetech.Zhuangzhou.Data;
using Moetech.Zhuangzhou.Models;
using Moetech.Zhuangzhou.Return;

namespace Moetech.Zhuangzhou.Controllers
{
    public class PersonnelInfoController : FilterController
    {
        /// <summary>
        /// 数据量上下文
        /// </summary>
        private readonly VirtualMachineDB _db;

        /// <summary>
        /// 每页条数
        /// </summary>
        private readonly int pageSize = 10;

        /// <summary>
        /// 角色 
        /// </summary>
        public override int Role { get; set; } = 1;

        public PersonnelInfoController(VirtualMachineDB context)
        {
            _db = context;
        }

        // GET: PersonnelInfo
        public async Task<IActionResult> Index(string name = "", int? pageIndex = 1)
        {
            var list = from c in _db.CommonPersonnelInfo select c;

            if (!string.IsNullOrWhiteSpace(name))
            {
                list = list.Where(o => o.PersonnelName.Contains(name));
            }

            var PagingList = await PaginatedList<CommonPersonnelInfo>.CreateAsync(list, pageIndex ?? 1, pageSize);
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

            var commonPersonnelInfo = await _db.CommonPersonnelInfo
                .FirstOrDefaultAsync(m => m.PersonnelId == id);
            if (commonPersonnelInfo == null)
            {
                return NotFound();
            }

            return View(commonPersonnelInfo);
        }

        // GET: PersonnelInfo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PersonnelInfo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PersonnelId,PersonnelNo,PersonnelName,DepId,Avatar,PersonnelSex,BirthDate,IdentityCard,IsWork,Nation,MaritalStatus,LiveAddress,Phone,WeChatAccount,Mailbox,Degree,Address,OnboardingTime,DepartureTime,TrialTime,IsStruggle,IsSecrecy,UserName,Password,AppMaxCount")] CommonPersonnelInfo commonPersonnelInfo)
        {
            if (ModelState.IsValid)
            {
                _db.Add(commonPersonnelInfo);
                await _db.SaveChangesAsync();
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

            var commonPersonnelInfo = await _db.CommonPersonnelInfo.FindAsync(id);
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
        public async Task<IActionResult> Edit(int id, [Bind("PersonnelId,PersonnelNo,PersonnelName,DepId,Avatar,PersonnelSex,BirthDate,IdentityCard,IsWork,Nation,MaritalStatus,LiveAddress,Phone,WeChatAccount,Mailbox,Degree,Address,OnboardingTime,DepartureTime,TrialTime,IsStruggle,IsSecrecy,UserName,Password,AppMaxCount")] CommonPersonnelInfo commonPersonnelInfo)
        {
            if (id != commonPersonnelInfo.PersonnelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(commonPersonnelInfo);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommonPersonnelInfoExists(commonPersonnelInfo.PersonnelId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
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

            var commonPersonnelInfo = await _db.CommonPersonnelInfo
                .FirstOrDefaultAsync(m => m.PersonnelId == id);
            if (commonPersonnelInfo == null)
            {
                return NotFound();
            }

            return View(commonPersonnelInfo);
        }

        // POST: PersonnelInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var commonPersonnelInfo = await _db.CommonPersonnelInfo.FindAsync(id);
            _db.CommonPersonnelInfo.Remove(commonPersonnelInfo);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommonPersonnelInfoExists(int id)
        {
            return _db.CommonPersonnelInfo.Any(e => e.PersonnelId == id);
        }
    }
}
