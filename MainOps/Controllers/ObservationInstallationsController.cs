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

namespace MainOps.Controllers
{
    public class ObservationInstallationsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public ObservationInstallationsController(DataContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // GET: ObservationInstallations
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.ObservationInstallation.Include(o => o.MeasPoint).Include(o => o.ObservationType).Include(o => o.Project).Include(o => o.SubProject).Include(o => o.VariationOrder);
            return View(await dataContext.ToListAsync());
        }

        // GET: ObservationInstallations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var observationInstallation = await _context.ObservationInstallation
                .Include(o => o.MeasPoint)
                .Include(o => o.ObservationType)
                .Include(o => o.Project)
                .Include(o => o.SubProject)
                .Include(o => o.VariationOrder)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (observationInstallation == null)
            {
                return NotFound();
            }

            return View(observationInstallation);
        }

        // GET: ObservationInstallations/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ProjectId"] = await GetProjectList();
            return View();
        }

        // POST: ObservationInstallations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MeasPointId,WellName,SensorRange,WellDepth,SensorDepth,WaterLevel,VariationOrderId,Latitude,Longitude,Accuracy,ObservationTypeId,ProjectId,SubProjectId,TimeStamp,Comments,DoneBy")] ObservationInstallation observationInstallation)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                _context.Add(observationInstallation);
                await _context.SaveChangesAsync();
                if (observationInstallation.ObservationTypeId != null)
                {
                    var itemtype = await _context.ItemTypes.SingleOrDefaultAsync(x => x.Id.Equals(observationInstallation.ObservationTypeId));

                    Install inst = new Install
                    {
                        ToBePaid = true,
                        ItemTypeId = itemtype.Id,
                        Latitude = Convert.ToDouble(observationInstallation.Latitude),
                        Longitude = Convert.ToDouble(observationInstallation.Longitude),
                        TimeStamp = observationInstallation.TimeStamp,
                        InvoiceDate = DateTime.Now,
                        RentalStartDate = observationInstallation.TimeStamp,
                        Install_Text = observationInstallation.WellName + " : " + observationInstallation.Comments,
                        isInstalled = true,
                        Amount = 1,
                        UniqueID = observationInstallation.WellName,
                        ProjectId = observationInstallation.ProjectId,
                        SubProjectId = observationInstallation.SubProjectId,
                        EnteredIntoDataBase = DateTime.Now,
                        LastEditedInDataBase = DateTime.Now,
                        DoneBy = user.full_name(),
                        VariationOrderId = observationInstallation.VariationOrderId

                    };
                    _context.Add(inst);
                    await _context.SaveChangesAsync();
                    var lasdadded = _context.Installations.OrderBy(x => x.Id).Last();
                    CoordTrack2 coords = new CoordTrack2();
                    coords.InstallId = lasdadded.Id;
                    coords.Latitude = lasdadded.Latitude;
                    coords.Longitude = lasdadded.Longitude;
                    coords.MeasPointId = observationInstallation.MeasPointId;
                    coords.TimeStamp = observationInstallation.TimeStamp;
                    coords.TypeCoord = "Installed";
                    _context.Add(coords);
                    await _context.SaveChangesAsync();
                }
                if (observationInstallation.PipeCut != null)
                {
                    if (observationInstallation.PipeCut != 0)
                    {
                        var itemtype = await _context.ItemTypes.Where(x => x.ProjectId.Equals(observationInstallation.ProjectId) && x.ReportTypeId.Equals(17)).SingleOrDefaultAsync();
                        if (itemtype != null)
                        {
                            Install inst = new Install();
                            inst.TimeStamp = observationInstallation.TimeStamp;
                            inst.ProjectId = observationInstallation.ProjectId;
                            inst.SubProjectId = observationInstallation.SubProjectId;
                            inst.RentalStartDate = observationInstallation.TimeStamp;
                            inst.InvoiceDate = DateTime.Now;
                            inst.Install_Text = "Automatic Installation Report of Pipe Cut/Extension of" + Convert.ToDouble(observationInstallation.PipeCut).ToString("F") + " meters";
                            inst.isInstalled = false;
                            inst.DeinstallDate = observationInstallation.TimeStamp;
                            inst.EnteredIntoDataBase = DateTime.Now;
                            inst.LastEditedInDataBase = DateTime.Now;
                            inst.ItemTypeId = itemtype.Id;
                            inst.Latitude = Convert.ToDouble(observationInstallation.Latitude);
                            inst.Longitude = Convert.ToDouble(observationInstallation.Longitude);
                            inst.UniqueID = observationInstallation.WellName;
                            inst.PayedAmount = 0;
                            inst.DoneBy = user.full_name();
                            inst.Amount = 1;
                            inst.ToBePaid = true;
                            _context.Add(inst);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = await GetProjectList();
            return View(observationInstallation);
        }

        // GET: ObservationInstallations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var observationInstallation = await _context.ObservationInstallation.FindAsync(id);
            if (observationInstallation == null)
            {
                return NotFound();
            }
            ViewData["MeasPointId"] = new SelectList(_context.MeasPoints, "Id", "Name", observationInstallation.MeasPointId);
            ViewData["ObservationTypeId"] = new SelectList(_context.ItemTypes, "Id", "Id", observationInstallation.ObservationTypeId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Abbreviation", observationInstallation.ProjectId);
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Id", observationInstallation.SubProjectId);
            ViewData["VariationOrderId"] = new SelectList(_context.BoQHeadLines, "Id", "Id", observationInstallation.VariationOrderId);
            return View(observationInstallation);
        }

        // POST: ObservationInstallations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MeasPointId,WellName,SensorRange,WellDepth,SensorDepth,WaterLevel,VariationOrderId,Latitude,Longitude,Accuracy,ObservationTypeId,ProjectId,SubProjectId,TimeStamp,Comments,DoneBy")] ObservationInstallation observationInstallation)
        {
            if (id != observationInstallation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(observationInstallation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ObservationInstallationExists(observationInstallation.Id))
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
            ViewData["MeasPointId"] = new SelectList(_context.MeasPoints, "Id", "Name", observationInstallation.MeasPointId);
            ViewData["ObservationTypeId"] = new SelectList(_context.ItemTypes, "Id", "Id", observationInstallation.ObservationTypeId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Abbreviation", observationInstallation.ProjectId);
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Id", observationInstallation.SubProjectId);
            ViewData["VariationOrderId"] = new SelectList(_context.BoQHeadLines, "Id", "Id", observationInstallation.VariationOrderId);
            return View(observationInstallation);
        }

        // GET: ObservationInstallations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var observationInstallation = await _context.ObservationInstallation
                .Include(o => o.MeasPoint)
                .Include(o => o.ObservationType)
                .Include(o => o.Project)
                .Include(o => o.SubProject)
                .Include(o => o.VariationOrder)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (observationInstallation == null)
            {
                return NotFound();
            }

            return View(observationInstallation);
        }

        // POST: ObservationInstallations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var observationInstallation = await _context.ObservationInstallation.FindAsync(id);
            _context.ObservationInstallation.Remove(observationInstallation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ObservationInstallationExists(int id)
        {
            return _context.ObservationInstallation.Any(e => e.Id == id);
        }
    }
}
