using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MainOps.Data;
using MainOps.Models.ReportClasses;
using MainOps.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Rotativa.AspNetCore;
using Microsoft.Extensions.Hosting;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
    public class AccidentReportsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        
        public AccidentReportsController(DataContext context,UserManager<ApplicationUser> userManager, IWebHostEnvironment env):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
            _env = env;

        }

        // GET: AccidentReports
        public async Task<IActionResult> Index(int? ProjectId = null)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["ProjectId"] = await GetProjectList();
            if(ProjectId != null)
            {
                var dataContext = _context.AccidentReports.Where(x => x.ProjectId.Equals(ProjectId)).Include(a => a.Project).Include(a => a.SafetyProblem).Where(x => x.Project.DivisionId.Equals(user.DivisionId));
                return View(await dataContext.OrderByDescending(x => x.SafetyProblem.TimeStamp).ToListAsync());
            }
            else
            {
                var dataContext = _context.AccidentReports.Include(a => a.Project).Include(a => a.SafetyProblem).Where(x => x.Project.DivisionId.Equals(user.DivisionId));
                return View(await dataContext.OrderByDescending(x => x.SafetyProblem.TimeStamp).ToListAsync());
            }
            
        }

        // GET: AccidentReports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accidentReport = await _context.AccidentReports
                .Include(a => a.Project)
                .Include(a => a.SafetyProblem)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (accidentReport == null)
            {
                return NotFound();
            }

            return View(accidentReport);
        }

        // GET: AccidentReports/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ProjectId"] = await GetProjectList();
            var safetyproblems = await _context.SafetyProblems
                .Include(x => x.Project)
                .Select(s => new SelectListItem { 
                    Value = s.Id.ToString(), 
                    Text = s.Project.Name + " : " + s.DoneBy + " : " + s.TimeStamp.ToString() 
                })
                .ToListAsync();
            ViewData["SafetyProblemId"] = new SelectList(safetyproblems, "Value", "Text");
            return View();
        }

        // POST: AccidentReports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,SafetyProblemId,AccidentWithAbsence,AccidentWithoutAbsence,EnvironmentAccident,InjuctionProhibition,NearMiss,HJSubContractor,Other,WhatHappened,NameOfInjured,WhereAndWhen,NameofWitnesses,WhatInjury,WhatWasDoneToFix,WhatCausedAccident,HowCouldItHappen,WasSafetyProcessInOrder,WhatIsDoneToPrevent,CauseForChangeInAPV,WhatCanBeLearned,IsOfferedLightJob,WhatLightJob,StartLightJob,DoneBy")] AccidentReport accidentReport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(accidentReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = await GetProjectList();
            var safetyproblems = await _context.SafetyProblems
                .Include(x => x.Project)
                .Select(s => new SelectListItem { 
                    Value = s.Id.ToString(), 
                    Text = s.Project.Name + " : " + s.DoneBy + " : " + s.TimeStamp.ToString() 
                })
                .ToListAsync();
            ViewData["SafetyProblemId"] = new SelectList(safetyproblems, "Value", "Text");
            return View("AccidentReport",accidentReport);
        }

        // GET: AccidentReports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accidentReport = await _context.AccidentReports.Include(x => x.SafetyProblem).Include(x => x.Project).ThenInclude(x => x.Division).SingleOrDefaultAsync(x => x.Id.Equals(id));
            if (accidentReport == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = await GetProjectList();
            var safetyproblems = await _context.SafetyProblems
                .Include(x => x.Project)
                .Select(s => new SelectListItem { 
                    Value = s.Id.ToString(), 
                    Text = s.Project.Name + " : " + s.DoneBy + " : " + s.TimeStamp.ToString() 
                })
                .ToListAsync();
            ViewData["SafetyProblemId"] = new SelectList(safetyproblems, "Value", "Text");
            return View("AccidentReport",accidentReport);
        }
        [HttpGet]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember")]
        public async Task<IActionResult> GetAccidentReport(int? id)
        {
            if (id != null)
            {
                var AR = await _context.AccidentReports.Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.SafetyProblem).SingleOrDefaultAsync(x => x.Id.Equals(id));
                string path = _env.WebRootPath + "/AHAK/SafetyProblems/" + AR.SafetyProblemId.ToString() + "/";
                List<string> pictures = new List<string>();
                if (Directory.Exists(path))
                {
                    var folder = Directory.EnumerateFiles(path)
                                     .Select(fn => Path.GetFileName(fn));

                    foreach (string file in folder)
                    {
                        if (file.Contains("_edit"))
                        {
                            pictures.Add(file);
                        }
                        else
                        {
                            string[] fileparts = file.Split(".");
                            if (!folder.Contains(fileparts[0] + "_edit." + fileparts[1]))
                            {
                                pictures.Add(file);
                            }
                        }
                    }
                }
                AR.SafetyProblem.pictures = pictures;

                return new ViewAsPdf("_AccidentReport", AR);
            }
            else
            {
                return NotFound();
            }
        }
        // POST: AccidentReports/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,SafetyProblemId,AccidentWithAbsence,AccidentWithoutAbsence,EnvironmentAccident,InjuctionProhibition,NearMiss,HJSubContractor,Other,WhatHappened,NameOfInjured,WhereAndWhen,NameofWitnesses,WhatInjury,WhatWasDoneToFix,WhatCausedAccident,HowCouldItHappen,WasSafetyProcessInOrder,WhatIsDoneToPrevent,CauseForChangeInAPV,WhatCanBeLearned,IsOfferedLightJob,WhatLightJob,StartLightJob,DoneBy")] AccidentReport accidentReport)
        {
            if (id != accidentReport.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(accidentReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccidentReportExists(accidentReport.Id))
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
            ViewData["ProjectId"] = await GetProjectList();
            var safetyproblems = await _context.SafetyProblems
                .Include(x => x.Project)
                .Select(s => new SelectListItem { 
                    Value = s.Id.ToString(), 
                    Text = s.Project.Name + " : " + s.DoneBy + " : " + s.TimeStamp.ToString() 
                })
                .ToListAsync();
            ViewData["SafetyProblemId"] = new SelectList(safetyproblems, "Value", "Text");
            return View(accidentReport);
        }

        // GET: AccidentReports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accidentReport = await _context.AccidentReports
                .Include(a => a.Project)
                .Include(a => a.SafetyProblem)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (accidentReport == null)
            {
                return NotFound();
            }

            return View(accidentReport);
        }

        // POST: AccidentReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var accidentReport = await _context.AccidentReports.FindAsync(id);
            _context.AccidentReports.Remove(accidentReport);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccidentReportExists(int id)
        {
            return _context.AccidentReports.Any(e => e.Id == id);
        }
    }
}
