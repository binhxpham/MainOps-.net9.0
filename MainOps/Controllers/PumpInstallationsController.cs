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

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember")]
    public class PumpInstallationsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public PumpInstallationsController(DataContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env): base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // GET: PumpInstallations
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var dataContext = _context.PumpInstallation.Include(p => p.MeasPoint).Include(p => p.Project).Include(p => p.SubProject).Include(x => x.PumpType)
                .Where(x => x.Project.DivisionId.Equals(user.DivisionId));
            return View(await dataContext.ToListAsync());
        }

        // GET: PumpInstallations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pumpInstallation = await _context.PumpInstallation
                .Include(p => p.MeasPoint)
                .Include(p => p.Project)
                .Include(p => p.SubProject)
                .Include(p => p.PumpType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pumpInstallation == null)
            {
                return NotFound();
            }

            return View(pumpInstallation);
        }

        // GET: PumpInstallations/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ProjectId"] = await GetProjectList();
            return View();
        }

        // POST: PumpInstallations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,SubProjectId,MeasPointId,WellName,TimeStamp,PumpTypeWritten,SensorRange,WellDepth,PumpDepth,SensorDepth,DiameterHose,WaterLevel,PipeCut,Comments,VariationOrderId,Accuracy,Latitude,Longitude,PumpTypeId")] PumpInstallation pumpInstallation)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                await CheckUser(user);
                _context.Add(pumpInstallation);
                await _context.SaveChangesAsync();
                
                if (pumpInstallation.PumpTypeId != null)
                {
                    var itemtype = await _context.ItemTypes.SingleOrDefaultAsync(x => x.Id.Equals(pumpInstallation.PumpTypeId));
                    
                    Install inst = new Install
                    {
                        ToBePaid = true,
                        ItemTypeId = itemtype.Id,
                        Latitude = Convert.ToDouble(pumpInstallation.Latitude),
                        Longitude = Convert.ToDouble(pumpInstallation.Longitude),
                        TimeStamp = pumpInstallation.TimeStamp,
                        InvoiceDate = DateTime.Now,
                        RentalStartDate = pumpInstallation.TimeStamp,
                        Install_Text = pumpInstallation.WellName + " : " + pumpInstallation.Comments,
                        isInstalled = true,
                        Amount = 1,
                        UniqueID = pumpInstallation.WellName,
                        ProjectId = pumpInstallation.ProjectId,
                        SubProjectId = pumpInstallation.SubProjectId,
                        EnteredIntoDataBase = DateTime.Now,
                        LastEditedInDataBase = DateTime.Now,
                        DoneBy = user.full_name(),
                        VariationOrderId = pumpInstallation.VariationOrderId

                    };
                    _context.Add(inst);
                    await _context.SaveChangesAsync();
                    var lasdadded = _context.Installations.OrderBy(x => x.Id).Last();
                    CoordTrack2 coords = new CoordTrack2();
                    coords.InstallId = lasdadded.Id;
                    coords.Latitude = lasdadded.Latitude;
                    coords.Longitude = lasdadded.Longitude;
                    coords.MeasPointId = pumpInstallation.MeasPointId;
                    coords.TimeStamp = pumpInstallation.TimeStamp;
                    coords.TypeCoord = "Installed";
                    _context.Add(coords);
                    await _context.SaveChangesAsync();
                }
                if (pumpInstallation.PipeCut != null)
                {
                    if(pumpInstallation.PipeCut != 0) { 
                    var itemtype = await _context.ItemTypes.Where(x => x.ProjectId.Equals(pumpInstallation.ProjectId) && x.ReportTypeId.Equals(17)).SingleOrDefaultAsync();
                    if (itemtype != null)
                    {
                        Install inst = new Install();
                        inst.TimeStamp = pumpInstallation.TimeStamp;
                        inst.ProjectId = pumpInstallation.ProjectId;
                        inst.SubProjectId = pumpInstallation.SubProjectId;
                        inst.RentalStartDate = pumpInstallation.TimeStamp;
                        inst.InvoiceDate = DateTime.Now;
                        inst.Install_Text = "Automatic Installation Report of Pipe Cut/Extension of" + Convert.ToDouble(pumpInstallation.PipeCut).ToString("F") + " meters";
                        inst.isInstalled = false;
                        inst.DeinstallDate = pumpInstallation.TimeStamp;
                        inst.EnteredIntoDataBase = DateTime.Now;
                        inst.LastEditedInDataBase = DateTime.Now;
                        inst.ItemTypeId = itemtype.Id;
                        inst.Latitude = Convert.ToDouble(pumpInstallation.Latitude);
                        inst.Longitude = Convert.ToDouble(pumpInstallation.Longitude);
                        inst.UniqueID = pumpInstallation.WellName;
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
            return View(pumpInstallation);
        }

        // GET: PumpInstallations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pumpInstallation = await _context.PumpInstallation.FindAsync(id);
            if (pumpInstallation == null)
            {
                return NotFound();
            }
            ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Where(x => x.ProjectId.Equals(pumpInstallation.ProjectId)), "Id", "Name", pumpInstallation.MeasPointId);
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => x.ProjectId.Equals(pumpInstallation.ProjectId)), "Id", "Name", pumpInstallation.SubProjectId);
            return View(pumpInstallation);
        }

        // POST: PumpInstallations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,SubProjectId,MeasPointId,WellName,TimeStamp,PumpTypeWritten,SensorRange,WellDepth,PumpDepth,SensorDepth,DiameterHose,WaterLevel,PipeCut,Comments,VariationOrderId,Accuracy,Latitude,Longitude,PumpTypeId")] PumpInstallation pumpInstallation)
        {
            if (id != pumpInstallation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pumpInstallation);
                    await _context.SaveChangesAsync();
                    //check if installation exists
                    var itemtype = await _context.ItemTypes.SingleOrDefaultAsync(x => x.Id.Equals(pumpInstallation.PumpTypeId));
                    var prev_inst = await _context.Installations.FirstOrDefaultAsync(x => x.UniqueID.Equals(pumpInstallation.WellName) && x.ItemTypeId.Equals(itemtype.Id));
                    if(prev_inst == null)
                    {
                        var user = await _userManager.GetUserAsync(User);
                        Install inst = new Install
                        {
                            ToBePaid = true,
                            ItemTypeId = itemtype.Id,
                            Latitude = Convert.ToDouble(pumpInstallation.Latitude),
                            Longitude = Convert.ToDouble(pumpInstallation.Longitude),
                            TimeStamp = pumpInstallation.TimeStamp,
                            InvoiceDate = DateTime.Now,
                            RentalStartDate = pumpInstallation.TimeStamp,
                            Install_Text = pumpInstallation.WellName + " : " + pumpInstallation.Comments,
                            isInstalled = true,
                            Amount = 1,
                            UniqueID = pumpInstallation.WellName,
                            ProjectId = pumpInstallation.ProjectId,
                            SubProjectId = pumpInstallation.SubProjectId,
                            EnteredIntoDataBase = DateTime.Now,
                            LastEditedInDataBase = DateTime.Now,
                            DoneBy = user.full_name(),
                            VariationOrderId = pumpInstallation.VariationOrderId

                        };
                        _context.Add(inst);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PumpInstallationExists(pumpInstallation.Id))
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
            ViewData["MeasPointId"] = new SelectList(_context.MeasPoints, "Id", "Name", pumpInstallation.MeasPointId);
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Name", pumpInstallation.SubProjectId);
            return View(pumpInstallation);
        }

        // GET: PumpInstallations/Delete/5
        [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pumpInstallation = await _context.PumpInstallation
                .Include(p => p.MeasPoint)
                .Include(p => p.Project)
                .Include(p => p.SubProject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pumpInstallation == null)
            {
                return NotFound();
            }

            return View(pumpInstallation);
        }

        // POST: PumpInstallations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pumpInstallation = await _context.PumpInstallation.FindAsync(id);
            _context.PumpInstallation.Remove(pumpInstallation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PumpInstallationExists(int id)
        {
            return _context.PumpInstallation.Any(e => e.Id == id);
        }
    }
}
