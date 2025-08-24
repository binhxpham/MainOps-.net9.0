using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MainOps.Data;
using MainOps.Models;
using MainOps.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,Manager,ProjectMember,DivisionAdmin,StorageManager,Supervisor")]
    public class UserActionsController : BaseController
    {
        private readonly DataContext _context;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public UserActionsController(DataContext context, IEmailSender emailSender, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment):base(context,userManager)
        {
            _context = context;
            _emailSender = emailSender;
            _userManager = userManager;
            _env = environment;
        }
        public async Task<IActionResult> UserActionsMain()
        {
            var theuser = await _userManager.GetUserAsync(User);
            var users = await (from user in _context.Users
                               join userRole in _context.UserRoles
                               on user.Id equals userRole.UserId
                               join role in _context.Roles on userRole.RoleId
                               equals role.Id
                               where userRole.UserId == user.Id && !role.Name.Equals("Guest") && !role.Name.Equals("MemberGuest")
                               && user.DivisionId.Equals(theuser.DivisionId)
                               select new
                               {
                                   Id = user.Id,
                                   NameEmail = user.full_name() + " : " + user.Email
                               }).OrderBy(x => x.NameEmail).GroupBy(x => x.NameEmail).Select(grp => grp.First()).ToListAsync();
            ViewData["UserId"] = new SelectList(users, "Id", "NameEmail");
            return View();
        }
        [HttpGet]
        public async Task<JsonResult> GetDailyReportDays(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var theuser = await _userManager.GetUserAsync(User);
            var dailyreports = await (from dr in _context.Daily_Report_2s.Include(x=>x.Project) where dr.Report_Date >= DateTime.Now.AddDays(-100) && dr.Project.DivisionId.Equals(theuser.DivisionId) && (dr.DoneBy.Equals(user.full_name()) || dr.OtherPeopleIDs.ToLower().Contains(user.Id.ToLower())
                                      || dr.OtherPeople.Contains(user.full_name()) || dr.OtherPeople.Contains(user.FirstName) )
                                      select new { Id = dr.Id, Report_Date = dr.Report_Date }).ToListAsync();
            return Json(dailyreports);
        }
        [HttpPost]
        public async Task<IActionResult> GetDailyReports(string Daton,string userId)
        {
            DateTime Dato = Convert.ToDateTime(Daton).AddHours(3);
            var user = await _userManager.FindByIdAsync(userId);
            //var daily = await _context.Daily_Report_2s.FindAsync(id);
            var dailyreports = await  _context.Daily_Report_2s.Include(x=>x.Project).Include(x=>x.SubProject).Where(x => x.Report_Date.Date.Equals(Dato.Date) && x.Project.DivisionId.Equals(user.DivisionId) && (x.DoneBy.Equals(user.full_name())
                                            || x.OtherPeople.Contains(user.full_name()) || x.OtherPeople.Contains(user.FirstName) || x.OtherPeopleIDs.ToLower().Contains(user.Id.ToLower()))
                                          ).ToListAsync();

            return PartialView("_userdailyreports",dailyreports);
        }
        [HttpGet]
        public async Task<JsonResult> GetMaintenanceReportDays(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var maintenancereports = await (from dr in _context.Maintenances.Include(x=>x.Project)
                                      where dr.TimeStamp >= DateTime.Now.AddDays(-100) && dr.Project.DivisionId.Equals(user.DivisionId) && dr.DoneBy.Equals(user.full_name())  
                                      select new { Id = dr.Id, Report_Date = dr.TimeStamp.Date }).ToListAsync();
            return Json(maintenancereports);
        }
        [HttpPost]
        public async Task<IActionResult> GetMaintenanceReports(string Daton, string userId)
        {
            DateTime Dato = Convert.ToDateTime(Daton).AddHours(3);
            var user = await _userManager.FindByIdAsync(userId);
            var theuser = await _userManager.GetUserAsync(User);
            var maintenancereports = await _context.Maintenances.Include(x => x.Project).Include(x => x.SubProject)
                .Where(x => x.TimeStamp.Date.Equals(Dato.Date) && x.Project.DivisionId.Equals(theuser.DivisionId) && x.DoneBy.Equals(user.full_name())).ToListAsync();
            
            return PartialView("_usermaintenancereports", maintenancereports);
        }
        [HttpGet]
        public async Task<JsonResult> GetAlarmReportDays(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var theuser = await _userManager.GetUserAsync(User);
            var alarmreports = await (from dr in _context.AlarmCalls
                                            where dr.TimeStamp >= DateTime.Now.AddDays(-100) && dr.Project.DivisionId.Equals(theuser.DivisionId) && dr.DoneBy.Equals(user.full_name())
                                            select new { Id = dr.Id, Report_Date = dr.TimeStamp.Date }).ToListAsync();
            return Json(alarmreports);
        }
        [HttpPost]
        public async Task<IActionResult> GetAlarmReports(string Daton, string userId)
        {
            DateTime Dato = Convert.ToDateTime(Daton).AddHours(3);
            var user = await _userManager.FindByIdAsync(userId);
            var theuser = await _userManager.GetUserAsync(User);
            var alarmreports = await _context.AlarmCalls.Include(x => x.Project).Include(x => x.SubProject)
                .Where(x => x.TimeStamp.Date.Equals(Dato.Date) && x.Project.DivisionId.Equals(theuser.DivisionId) && x.DoneBy.Equals(user.full_name())).ToListAsync();

            return PartialView("_useralarmreports", alarmreports);
        }
    }
}
