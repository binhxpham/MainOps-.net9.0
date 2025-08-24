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
    public class ReinfiltrationInstallationsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public ReinfiltrationInstallationsController(DataContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // GET: ReinfiltrationInstallations
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.ReinfiltrationInstallation.Include(r => r.MeasPoint).Include(r => r.Project).Include(r => r.ReinfiltrationType).Include(r => r.SubProject).Include(r => r.VariationOrder);
            return View(await dataContext.ToListAsync());
        }

        // GET: ReinfiltrationInstallations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reinfiltrationInstallation = await _context.ReinfiltrationInstallation
                .Include(r => r.MeasPoint)
                .Include(r => r.Project)
                .Include(r => r.ReinfiltrationType)
                .Include(r => r.SubProject)
                .Include(r => r.VariationOrder)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reinfiltrationInstallation == null)
            {
                return NotFound();
            }

            return View(reinfiltrationInstallation);
        }

        // GET: ReinfiltrationInstallations/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ProjectId"] = await GetProjectList();
            return View();
        }

        // POST: ReinfiltrationInstallations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MeasPointId,WellName,SensorRange,WellDepth,SensorDepth,DiameterHose,WaterLevel,VariationOrderId,Latitude,Longitude,Accuracy,ReinfiltrationTypeId,ProjectId,SubProjectId,TimeStamp,Comments,DoneBy")] ReinfiltrationInstallation reinfiltrationInstallation)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                await CheckUser(user);
                _context.Add(reinfiltrationInstallation);
                await _context.SaveChangesAsync();
                if (reinfiltrationInstallation.ReinfiltrationTypeId != null)
                {
                    var itemtype = await _context.ItemTypes.SingleOrDefaultAsync(x => x.Id.Equals(reinfiltrationInstallation.ReinfiltrationTypeId));

                    Install inst = new Install
                    {
                        ToBePaid = true,
                        ItemTypeId = itemtype.Id,
                        Latitude = Convert.ToDouble(reinfiltrationInstallation.Latitude),
                        Longitude = Convert.ToDouble(reinfiltrationInstallation.Longitude),
                        TimeStamp = reinfiltrationInstallation.TimeStamp,
                        InvoiceDate = DateTime.Now,
                        RentalStartDate = reinfiltrationInstallation.TimeStamp,
                        Install_Text = reinfiltrationInstallation.WellName + " : " + reinfiltrationInstallation.Comments,
                        isInstalled = true,
                        Amount = 1,
                        UniqueID = reinfiltrationInstallation.WellName,
                        ProjectId = reinfiltrationInstallation.ProjectId,
                        SubProjectId = reinfiltrationInstallation.SubProjectId,
                        EnteredIntoDataBase = DateTime.Now,
                        LastEditedInDataBase = DateTime.Now,
                        DoneBy = user.full_name(),
                        VariationOrderId = reinfiltrationInstallation.VariationOrderId

                    };
                    _context.Add(inst);
                    await _context.SaveChangesAsync();
                    var lasdadded = _context.Installations.OrderBy(x => x.Id).Last();
                    CoordTrack2 coords = new CoordTrack2();
                    coords.InstallId = lasdadded.Id;
                    coords.Latitude = lasdadded.Latitude;
                    coords.Longitude = lasdadded.Longitude;
                    coords.MeasPointId = reinfiltrationInstallation.MeasPointId;
                    coords.TimeStamp = reinfiltrationInstallation.TimeStamp;
                    coords.TypeCoord = "Installed";
                    _context.Add(coords);
                    await _context.SaveChangesAsync();
                }
                
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = await GetProjectList();
            return View(reinfiltrationInstallation);
        }

        // GET: ReinfiltrationInstallations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reinfiltrationInstallation = await _context.ReinfiltrationInstallation.FindAsync(id);
            if (reinfiltrationInstallation == null)
            {
                return NotFound();
            }
            ViewData["MeasPointId"] = new SelectList(_context.MeasPoints, "Id", "Name", reinfiltrationInstallation.MeasPointId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Abbreviation", reinfiltrationInstallation.ProjectId);
            ViewData["ReinfiltrationTypeId"] = new SelectList(_context.ItemTypes, "Id", "Id", reinfiltrationInstallation.ReinfiltrationTypeId);
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Id", reinfiltrationInstallation.SubProjectId);
            ViewData["VariationOrderId"] = new SelectList(_context.BoQHeadLines, "Id", "Id", reinfiltrationInstallation.VariationOrderId);
            return View(reinfiltrationInstallation);
        }

        // POST: ReinfiltrationInstallations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MeasPointId,WellName,SensorRange,WellDepth,SensorDepth,DiameterHose,WaterLevel,VariationOrderId,Latitude,Longitude,Accuracy,ReinfiltrationTypeId,ProjectId,SubProjectId,TimeStamp,Comments,DoneBy")] ReinfiltrationInstallation reinfiltrationInstallation)
        {
            if (id != reinfiltrationInstallation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reinfiltrationInstallation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReinfiltrationInstallationExists(reinfiltrationInstallation.Id))
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
            ViewData["MeasPointId"] = new SelectList(_context.MeasPoints, "Id", "Name", reinfiltrationInstallation.MeasPointId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Abbreviation", reinfiltrationInstallation.ProjectId);
            ViewData["ReinfiltrationTypeId"] = new SelectList(_context.ItemTypes, "Id", "Id", reinfiltrationInstallation.ReinfiltrationTypeId);
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Id", reinfiltrationInstallation.SubProjectId);
            ViewData["VariationOrderId"] = new SelectList(_context.BoQHeadLines, "Id", "Id", reinfiltrationInstallation.VariationOrderId);
            return View(reinfiltrationInstallation);
        }

        // GET: ReinfiltrationInstallations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reinfiltrationInstallation = await _context.ReinfiltrationInstallation
                .Include(r => r.MeasPoint)
                .Include(r => r.Project)
                .Include(r => r.ReinfiltrationType)
                .Include(r => r.SubProject)
                .Include(r => r.VariationOrder)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reinfiltrationInstallation == null)
            {
                return NotFound();
            }

            return View(reinfiltrationInstallation);
        }

        // POST: ReinfiltrationInstallations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reinfiltrationInstallation = await _context.ReinfiltrationInstallation.FindAsync(id);
            _context.ReinfiltrationInstallation.Remove(reinfiltrationInstallation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReinfiltrationInstallationExists(int id)
        {
            return _context.ReinfiltrationInstallation.Any(e => e.Id == id);
        }
    }
}
