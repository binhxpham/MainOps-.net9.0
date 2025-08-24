using MainOps.Data;
using MainOps.Models;
using MainOps.Models.ReportClasses;
using MainOps.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
    public class DailyReportController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public DailyReportController(DataContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }
        [HttpGet]
        public async Task<IActionResult> SplitDailyReport(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            await CheckUser(user);
            var dailyreport = await _context.Daily_Report_2s.Include(x => x.Project).Include(x => x.SubProject).SingleOrDefaultAsync(x => x.Id.Equals(id));
            if(dailyreport.Project.DivisionId.Equals(user.DivisionId) || User.IsInRole("Admin"))
            {
                ViewData["ProjectId"] = await GetProjectList();
                ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => x.ProjectId.Equals(dailyreport.ProjectId)), "Id", "Name");
                ViewData["VariationOrderId"] = new SelectList(_context.BoQHeadLines.Where(x => x.ProjectId.Equals(dailyreport.ProjectId) && x.Type.Equals("ExtraWork")), "Id", "HeadLine");
                ViewData["TitleId"] = new SelectList(_context.Titles.Where(x => x.ProjectId.Equals(dailyreport.ProjectId)), "Id", "TheTitle");
                SplitModelVM model = new SplitModelVM(dailyreport);
                return View(model);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> SplitDailyReport(SplitModelVM model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid) { 
            
                var drtobackup = await _context.Daily_Report_2s.SingleOrDefaultAsync(x => x.Id.Equals(model.DRId));
                Daily_Report_2Backup drbackup = new Daily_Report_2Backup(drtobackup);
                _context.DailyReportBackups.Add(drbackup);
                await _context.SaveChangesAsync();
                Daily_Report_2 DR1 = new Daily_Report_2();
                Daily_Report_2 DR2 = new Daily_Report_2();
                DR1.ProjectId = model.ProjectId1;
                DR1.SubProjectId = model.SubProjectId1;
                DR1.short_Description = model.ShortDescription1;
                DR1.TitleId = model.TitleId1;
                DR1.VariationOrderId = model.VariationOrderId1;
                DR1.tobepaid = model.MakePaid1;
                DR1.Report_Checked = model.Checked1;
                if(model.Checked1 == true && drtobackup.Report_Checked != true)
                {
                    DR1.Checked_By = user.full_name();
                }
                DR1.StartHour = model.HourStart1;
                DR1.EndHour = model.HourEnd1;
                DR1.StandingTime = model.DownTime1;
                DR1.SafetyHours = model.HoursSafetySetup1;
                DR1.Report_Date = drtobackup.Report_Date;
                DR1.Signature = drtobackup.Signature;
                DR1.short_Description = drtobackup.short_Description;
                DR1.Work_Performed = model.report_text1;
                DR1.Amount = drtobackup.Amount;
                DR1.DoneBy = drtobackup.DoneBy;
                DR1.EnteredIntoDataBase = DateTime.Now;
                DR1.LastEditedInDataBase = DateTime.Now;
                DR1.InvoiceDate = DateTime.Now;
                DR1.Machinery = drtobackup.Machinery;
                DR1.OtherPeople = drtobackup.OtherPeople;
                DR1.OtherPeopleIDs = drtobackup.OtherPeopleIDs;
                DR1.Extra_Works = drtobackup.Extra_Works;
                DR2.ProjectId = model.ProjectId2;
                DR2.SubProjectId = model.SubProjectId2;
                DR2.short_Description = model.ShortDescription2;
                DR2.TitleId = model.TitleId2;
                DR2.VariationOrderId = model.VariationOrderId2;
                DR2.tobepaid = model.MakePaid2;
                DR2.Report_Checked = model.Checked2;
                if (model.Checked2 == true && drtobackup.Report_Checked != true)
                {
                    DR2.Checked_By = user.full_name();
                }
                DR2.StartHour = model.HourStart2;
                DR2.EndHour = model.HourEnd2;
                DR2.StandingTime = model.DownTime2;
                DR2.SafetyHours = model.HoursSafetySetup2;
                DR2.Report_Date = drtobackup.Report_Date;
                DR2.Signature = drtobackup.Signature;
                DR2.short_Description = drtobackup.short_Description;
                DR2.Work_Performed = model.report_text2;
                DR2.Amount = drtobackup.Amount;
                DR2.DoneBy = drtobackup.DoneBy;
                DR2.EnteredIntoDataBase = DateTime.Now;
                DR2.LastEditedInDataBase = DateTime.Now;
                DR2.InvoiceDate = DateTime.Now;
                DR2.Machinery = drtobackup.Machinery;
                DR2.OtherPeople = drtobackup.OtherPeople;
                DR2.OtherPeopleIDs = drtobackup.OtherPeopleIDs;
                DR2.Extra_Works = drtobackup.Extra_Works;
                _context.Daily_Report_2s.Add(DR1);
                await _context.SaveChangesAsync();
                var lastadded1 = await _context.Daily_Report_2s.LastAsync();
                _context.Daily_Report_2s.Add(DR2);
                await _context.SaveChangesAsync();
                var lastadded2 = await _context.Daily_Report_2s.LastAsync();
                var folderpath = _env.WebRootPath + "\\AHAK\\DailyReports\\" + drtobackup.Id.ToString() + "\\";
                var folderpath1 = _env.WebRootPath + "\\AHAK\\DailyReports\\" + lastadded1.Id.ToString() + "\\";
                var folderpath2 = _env.WebRootPath + "\\AHAK\\DailyReports\\" + lastadded2.Id.ToString() + "\\";
            if (Directory.Exists(folderpath))
            {
                Directory.CreateDirectory(folderpath1);
                Directory.CreateDirectory(folderpath2);
                foreach (var file in Directory.GetFiles(folderpath))
                {
                    var fname = file.Split("\\").Last();
                    System.IO.File.Copy(folderpath + fname, folderpath1 + fname);
                    System.IO.File.Copy(folderpath + fname, folderpath2 + fname);
                }
            }
            _context.Daily_Report_2s.Remove(drtobackup);
            await _context.SaveChangesAsync();


            return RedirectToAction("DailyReports", "TrackItems");
            }
            var drtobackup2 = await _context.Daily_Report_2s.SingleOrDefaultAsync(x => x.Id.Equals(model.DRId));
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => x.ProjectId.Equals(drtobackup2.ProjectId)), "Id", "Name");
            ViewData["VariationOrderId"] = new SelectList(_context.BoQHeadLines.Where(x => x.ProjectId.Equals(drtobackup2.ProjectId) && x.Type.Equals("ExtraWork")), "Id", "HeadLine");
            ViewData["TitleId"] = new SelectList(_context.Titles.Where(x => x.ProjectId.Equals(drtobackup2.ProjectId)), "Id", "TheTitle");
            return View(model);
        }
        
    }
}
