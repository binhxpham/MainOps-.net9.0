using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MainOps.Data;
using MainOps.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace MainOps.Controllers
{
    [Authorize]
    public class SummaryReportsController : BaseController
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;

        public SummaryReportsController(DataContext context, IWebHostEnvironment env, UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        // GET: SummaryReports
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                var dataContext = _context.SummaryReports.Include(s => s.Project);
                return View(await dataContext.ToListAsync());
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                var dataContext = _context.SummaryReports.Include(s => s.Project).Where(x=>x.Project.DivisionId.Equals(user.DivisionId));
                return View(await dataContext.ToListAsync());
            }
            
        }

        // GET: SummaryReports/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var summaryReport = await _context.SummaryReports
                .Include(s => s.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (summaryReport == null)
            {
                return NotFound();
            }
            else if(!summaryReport.Project.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
            }
            string FileName = @"" + _env.WebRootPath + "\\AHAK\\DailySummaries\\DailySummary_" + summaryReport.Project.Abbreviation + "_" + summaryReport.Report_Date.ToString("yyyy-MM-dd") + ".pdf";
            string actualFileName = "DailySummary_" + summaryReport.Project.Abbreviation + "_" + summaryReport.Report_Date.ToString("yyyy-MM-dd") + ".pdf";
            var stream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            return File(stream, "application/pdf", actualFileName);
            //return File(FileName, "application/pdf",FileMode.Open,FileMode.Read);
        }
        [HttpGet]
        public async Task<IActionResult> SeeClientSignedReport(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var summaryReport = await _context.SummaryReports
                .Include(s => s.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (summaryReport == null)
            {
                return NotFound();
            }
            else if (!summaryReport.Project.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
            }
            var FileName = _env.WebRootPath + "\\AHAK\\DailySummaries\\DailySummary_" + summaryReport.Project.Abbreviation + "_" + summaryReport.Report_Date.ToString("yyyy-MM-dd") + "_signed" + ".pdf";
            string actualFileName = "DailySummary_" + summaryReport.Project.Abbreviation + "_" + summaryReport.Report_Date.ToString("yyyy-MM-dd") + "_signed" + ".pdf";
            var stream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            return File(stream, "application/pdf", actualFileName);
        }
        [HttpPost]
        public async Task<IActionResult> UploadSignedReport(int? id,IFormFile file)
        {
            if(id == null)
            {
                return NotFound();
            }
            if(file == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var summaryReport = await _context.SummaryReports
                .Include(s => s.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if(summaryReport == null)
            {
                return NotFound();
            }
            else if (!summaryReport.Project.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
            }
            var FileName = _env.WebRootPath + "\\AHAK\\DailySummaries\\DailySummary_" + summaryReport.Project.Abbreviation + "_" + summaryReport.Report_Date.ToString("yyyy-MM-dd") + "_signed" + ".pdf";
            using (var stream = new FileStream(FileName, FileMode.Create)) {
                await file.CopyToAsync(stream);
            };
            summaryReport.IsSignedByClient = true;
            _context.Update(summaryReport);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            
        }

        // GET: SummaryReports/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x=>x.DivisionId.Equals(user.DivisionId) && x.Active.Equals(true)), "Id", "Name");
            return View();
        }

        // POST: SummaryReports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Report_Date,ProjectId,People,IsSignedByHJ,IsSignedByClient,signatureHJ,signatureClient,SignatureHJDateTime,SignatureClientDateTime,TheDailyText,SentToClient,ClientEmail")] SummaryReport summaryReport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(summaryReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var user = await _userManager.GetUserAsync(User);
            ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.DivisionId.Equals(user.DivisionId)), "Id", "Name", summaryReport.ProjectId);
            return View(summaryReport);
        }

        // GET: SummaryReports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var summaryReport = await _context.SummaryReports.FindAsync(id);
            if (summaryReport == null)
            {
                return NotFound();
            }
            else if (!summaryReport.Project.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.DivisionId.Equals(user.DivisionId) && x.Active.Equals(true)), "Id", "Name", summaryReport.ProjectId);
            return View(summaryReport);
        }

        // POST: SummaryReports/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Report_Date,ProjectId,People,IsSignedByHJ,IsSignedByClient,signatureHJ,signatureClient,SignatureHJDateTime,SignatureClientDateTime,TheDailyText,SentToClient,ClientEmail")] SummaryReport summaryReport)
        {
            if (id != summaryReport.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(summaryReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SummaryReportExists(summaryReport.Id))
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
            var user = await _userManager.GetUserAsync(User);
            ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.DivisionId.Equals(user.DivisionId)), "Id", "Name", summaryReport.ProjectId);
            return View(summaryReport);
        }

        // GET: SummaryReports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var summaryReport = await _context.SummaryReports
                .Include(s => s.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (summaryReport == null)
            {
                return NotFound();
            }
            else if (!summaryReport.Project.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
            }
            return View(summaryReport);
        }

        // POST: SummaryReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var summaryReport = await _context.SummaryReports.FindAsync(id);
            _context.SummaryReports.Remove(summaryReport);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SummaryReportExists(int id)
        {
            return _context.SummaryReports.Any(e => e.Id == id);
        }
    }
}
