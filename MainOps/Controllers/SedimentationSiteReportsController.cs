using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MainOps.Data;
using MainOps.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using MainOps.ExtensionMethods;
using System.IO;
using Rotativa.AspNetCore;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember")]
    public class SedimentationSiteReportsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public SedimentationSiteReportsController(DataContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
            _env = env;

        }

        // GET: SedimentationSiteReports
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            await CheckUser(user);
            var dataContext = _context.SedimentationSiteReports.Include(s => s.Project).Include(s => s.SubProject).Where(x => x.Project.DivisionId.Equals(user.DivisionId));
            return View(await dataContext.ToListAsync());
        }

        // GET: SedimentationSiteReports/Details/5
        [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            await CheckUser(user);
            var sedimentationSiteReport = await _context.SedimentationSiteReports
                .Include(s => s.Project).ThenInclude(x => x.Division)
                .Include(s => s.SubProject)
                .Include(x => x.Photos)
                .FirstOrDefaultAsync(m => m.Id == id && m.Project.DivisionId.Equals(user.DivisionId));
            if (sedimentationSiteReport == null)
            {
                return NotFound();
            }

            return new ViewAsPdf("_SediCheck",sedimentationSiteReport);
        }

        // GET: SedimentationSiteReports/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ProjectId"] = await GetProjectList();
            return View();
        }

        // POST: SedimentationSiteReports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(90000000)]
        public async Task<IActionResult> Create([Bind("Id,SedimenationCleanedAndEmptied,SedimentationShouldBeEmptiedAndCleaned,GeoTubeExchanged,GeoTubeShouldBeExchanged,SiteCheckPerformed,Leakages,AccessWays,Safety,AcidExchanged,AcidShouldBeExchanged,AlarmFunction,ProjectId,SubProjectId,TimeStamp,Comments,DoneBy,PlantId,SedimentationMinimumWater,SedimenationFlowRate,OilSeperatorClogged")] SedimentationSiteReport sedimentationSiteReport, IFormFile[] photos)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                sedimentationSiteReport.DoneBy = user.full_name();
                _context.Add(sedimentationSiteReport);
                await _context.SaveChangesAsync();
                var lastadded = await _context.SedimentationSiteReports.OrderBy(x => x.Id).LastAsync();
                if (photos != null)
                {

                    var directory = _env.WebRootPath + "\\SediChecks\\" + lastadded.Id.ToString() + "\\";
                    if (!Directory.Exists(directory) && photos.Count() != 0)
                    {
                        Directory.CreateDirectory(directory);
                    }
                    foreach (var photo1 in photos)
                    {
                        var path = Path.Combine(directory, photo1.FileName);
                        SedimentationSiteReportPhoto pic = new SedimentationSiteReportPhoto { SedimentationSiteReportId = lastadded.Id, Path = path };
                        _context.Add(pic);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await photo1.CopyToAsync(stream);
                        };
                        if (path.ToLower().Contains(".jpg") || path.ToLower().Contains(".jpeg"))
                        {
                            PhotoExtensions.SaveAndCompressJpeg(path, 95);
                        }
                    }
                }
                var itemtype = await _context.ItemTypes.Include(x => x.ReportType).Where(x => x.ProjectId.Equals(lastadded.ProjectId) && x.ReportType.Type.Equals("SediCheck")).SingleOrDefaultAsync();

                if (itemtype != null)
                {
                    Install inst = new Install
                    {
                        ToBePaid = true,
                        ItemTypeId = itemtype.Id,
                        Latitude = 0,
                        Longitude = 0,
                        TimeStamp = lastadded.TimeStamp,
                        InvoiceDate = DateTime.Now,
                        RentalStartDate = lastadded.TimeStamp,
                        Install_Text = lastadded.Comments,
                        UniqueID = "Sedimentation Site Check : " + lastadded.PlantId,
                        isInstalled = true,
                        Amount = 1,
                        ProjectId = lastadded.ProjectId,
                        SubProjectId = lastadded.SubProjectId,
                        EnteredIntoDataBase = DateTime.Now,
                        LastEditedInDataBase = DateTime.Now,
                        DoneBy = user.full_name()
                    };
                    _context.Installations.Add(inst);
                    await _context.SaveChangesAsync();
                }
            return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => x.ProjectId.Equals(sedimentationSiteReport.ProjectId)), "Id", "Name", sedimentationSiteReport.SubProjectId);
            return View(sedimentationSiteReport);
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember")]
        // GET: SedimentationSiteReports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            await CheckUser(user);
            var sedimentationSiteReport = await _context.SedimentationSiteReports.Include(x => x.Project).FirstOrDefaultAsync(x => x.Id.Equals(id) && x.Project.DivisionId.Equals(user.DivisionId));
            if (sedimentationSiteReport == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x =>x.ProjectId.Equals(sedimentationSiteReport.ProjectId)), "Id", "Name", sedimentationSiteReport.SubProjectId);
            return View(sedimentationSiteReport);
        }

        // POST: SedimentationSiteReports/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SedimenationCleanedAndEmptied,SedimentationShouldBeEmptiedAndCleaned,GeoTubeExchanged,GeoTubeShouldBeExchanged,SiteCheckPerformed,Leakages,AccessWays,Safety,AcidExchanged,AcidShouldBeExchanged,AlarmFunction,ProjectId,SubProjectId,TimeStamp,Comments,DoneBy,PlantId,SedimentationMinimumWater,SedimenationFlowRate,OilSeperatorClogged")] SedimentationSiteReport sedimentationSiteReport, IFormFile[] photos)
        {
            if (id != sedimentationSiteReport.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    _context.Update(sedimentationSiteReport);
                    if (photos != null)
                    {

                        var directory = _env.WebRootPath + "\\SediChecks\\" + sedimentationSiteReport.Id.ToString() + "\\";
                        if (!Directory.Exists(directory) && photos.Count() != 0)
                        {
                            Directory.CreateDirectory(directory);
                        }
                        foreach (var photo1 in photos)
                        {
                            var path = Path.Combine(directory, photo1.FileName);
                            SedimentationSiteReportPhoto pic = new SedimentationSiteReportPhoto { SedimentationSiteReportId = sedimentationSiteReport.Id, Path = path };
                            _context.Add(pic);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await photo1.CopyToAsync(stream);
                            };
                            if (path.ToLower().Contains(".jpg") || path.ToLower().Contains(".jpeg"))
                            {
                                PhotoExtensions.SaveAndCompressJpeg(path, 95);
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                    var itemtype = await _context.ItemTypes.Include(x => x.ReportType).Where(x => x.ProjectId.Equals(sedimentationSiteReport.ProjectId) && x.ReportType.Type.Equals("SediCheck")).SingleOrDefaultAsync();

                    if (itemtype != null)
                    {
                        Install inst = new Install
                        {
                            ToBePaid = true,
                            ItemTypeId = itemtype.Id,
                            Latitude = 0,
                            Longitude = 0,
                            TimeStamp = sedimentationSiteReport.TimeStamp,
                            InvoiceDate = DateTime.Now,
                            RentalStartDate = sedimentationSiteReport.TimeStamp,
                            Install_Text = sedimentationSiteReport.Comments,
                            UniqueID = "Sedimentation Site Check : " + sedimentationSiteReport.PlantId.ToString(),
                            isInstalled = true,
                            Amount = 1,
                            ProjectId = sedimentationSiteReport.ProjectId,
                            SubProjectId = sedimentationSiteReport.SubProjectId,
                            EnteredIntoDataBase = DateTime.Now,
                            LastEditedInDataBase = DateTime.Now,
                            DoneBy = sedimentationSiteReport.DoneBy
                        };
                        var prev_install = await _context.Installations.SingleOrDefaultAsync(x => x.ItemTypeId.Equals(itemtype.Id) && x.UniqueID.Equals("Sedimentation Site Check : " + sedimentationSiteReport.PlantId.ToString()) && x.TimeStamp.Equals(sedimentationSiteReport.TimeStamp) && x.ProjectId.Equals(sedimentationSiteReport.ProjectId));
                        if(prev_install == null)
                        {
                            _context.Installations.Add(inst);
                            await _context.SaveChangesAsync();
                        }
                       
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SedimentationSiteReportExists(sedimentationSiteReport.Id))
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
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => x.ProjectId.Equals(sedimentationSiteReport.ProjectId)), "Id", "Name", sedimentationSiteReport.SubProjectId);
            return View(sedimentationSiteReport);
        }

        // GET: SedimentationSiteReports/Delete/5
        [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            await CheckUser(user);
            var sedimentationSiteReport = await _context.SedimentationSiteReports
                .Include(s => s.Project)
                .Include(s => s.SubProject)
                .FirstOrDefaultAsync(m => m.Id == id && m.Project.DivisionId.Equals(user.DivisionId));
            if (sedimentationSiteReport == null)
            {
                return NotFound();
            }

            return View(sedimentationSiteReport);
        }

        // POST: SedimentationSiteReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sedimentationSiteReport = await _context.SedimentationSiteReports.FindAsync(id);
            _context.SedimentationSiteReports.Remove(sedimentationSiteReport);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SedimentationSiteReportExists(int id)
        {
            return _context.SedimentationSiteReports.Any(e => e.Id == id);
        }
        [Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<IActionResult> UpdateSediChecks(int? ProjectId)
        {
            var itemtype = await _context.ItemTypes.Include(x => x.ReportType).Where(x => x.ProjectId.Equals(ProjectId) && x.ReportType.Type.Equals("SediCheck")).SingleOrDefaultAsync();
            var sedis = await _context.SedimentationSiteReports.Where(x => x.ProjectId.Equals(ProjectId)).ToListAsync();
           
            if (itemtype != null)
            {
                foreach (var lastadded in sedis) 
                {
                    var prev_install = await _context.Installations.FirstOrDefaultAsync(x => x.UniqueID.Equals("Sedimentation Site Check : " + lastadded.PlantId) && x.ProjectId.Equals(ProjectId));
                    if(prev_install == null) 
                    { 
                        Install inst = new Install
                        {
                            ToBePaid = true,
                            ItemTypeId = itemtype.Id,
                            Latitude = 0,
                            Longitude = 0,
                            TimeStamp = lastadded.TimeStamp,
                            InvoiceDate = DateTime.Now,
                            RentalStartDate = lastadded.TimeStamp,
                            Install_Text = lastadded.Comments,
                            UniqueID = "Sedimentation Site Check : " + lastadded.PlantId,
                            isInstalled = true,
                            Amount = 1,
                            ProjectId = lastadded.ProjectId,
                            SubProjectId = lastadded.SubProjectId,
                            EnteredIntoDataBase = DateTime.Now,
                            LastEditedInDataBase = DateTime.Now,
                            DoneBy = lastadded.DoneBy
                        };
                        _context.Installations.Add(inst);
                    }
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
