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
    [Authorize(Roles = "Manager,DivisionAdmin,Admin")]
    public class WaterHandlingsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public WaterHandlingsController(DataContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env) : base(context, userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;

        }

        // GET: WaterHandlings
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.WaterHandlings.Include(w => w.ClearPumpTest).Include(w => w.Well).Include(w => w.DrillWater).Include(w => w.MeasPoint);
            return View(await dataContext.ToListAsync());
        }

        // GET: WaterHandlings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waterHandling = await _context.WaterHandlings
                .Include(w => w.ClearPumpTest)
                .Include(w => w.Well)
                .Include(w => w.DrillWater)
                .Include(w => w.MeasPoint)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (waterHandling == null)
            {
                return NotFound();
            }

            return View(waterHandling);
        }

        // GET: WaterHandlings/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ProjectId"] = await GetProjectList();
            return View();
        }

        // POST: WaterHandlings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MeasPointId,ClearPumpTestId,DrillWaterId,WellName,WellId,PumpWaterStart,PumpWaterEnd,DrillWaterStart,DrillWaterEnd,AmountContainerDrill,AmountContainerPump,PumpDateStart,PumpDateEnd,DrillStart,DrillEnd")] WaterHandling waterHandling)
        {
            if (ModelState.IsValid)
            {
                var alreadyexists = await _context.WaterHandlings.SingleOrDefaultAsync(x => x.MeasPointId.Equals(waterHandling.MeasPointId));
                if(alreadyexists == null) {
                _context.Add(waterHandling);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                }
                else
                {
                    alreadyexists.PumpDateEnd = waterHandling.PumpDateEnd;
                    alreadyexists.PumpDateStart = waterHandling.PumpDateStart;
                    alreadyexists.DrillStart = waterHandling.DrillStart;
                    alreadyexists.DrillEnd = waterHandling.DrillEnd;
                    alreadyexists.DrillWaterStart = waterHandling.DrillWaterStart;
                    alreadyexists.DrillWaterEnd = waterHandling.DrillWaterEnd;
                    alreadyexists.DrillWaterId = waterHandling.DrillWaterId;
                    alreadyexists.ClearPumpTestId = waterHandling.ClearPumpTestId;
                    alreadyexists.WellId = waterHandling.WellId;
                    alreadyexists.AmountContainerDrill = waterHandling.AmountContainerDrill;
                    alreadyexists.AmountContainerPump = waterHandling.AmountContainerPump;
                    _context.Update(alreadyexists);
                    await _context.SaveChangesAsync();
                }
            }
            ViewData["ClearPumpTestId"] = new SelectList(_context.ClearPumpTests, "Id", "Id", waterHandling.ClearPumpTestId);
            ViewData["WellId"] = new SelectList(_context.Wells, "Id", "Id", waterHandling.WellId);
            ViewData["DrillWaterId"] = new SelectList(_context.DrillWaters, "Id", "Id", waterHandling.DrillWaterId);
            ViewData["MeasPointId"] = new SelectList(_context.MeasPoints, "Id", "Name", waterHandling.MeasPointId);
            return View(waterHandling);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateWaterHandling()
        {
            var drillwaters = await _context.DrillWaters.ToListAsync();
            var cps = await _context.ClearPumpTests.Where(x => x.ProjectId.Equals(437)).OrderBy(x => x.Report_Date).ToListAsync();
            var wells = await _context.Wells.Where(x => x.ProjectId.Equals(437)).ToListAsync();
            var mps = await _context.MeasPoints.Where(x => x.ProjectId.Equals(437)).ToListAsync();
            foreach(var dw in drillwaters)
            {
                var mp = mps.SingleOrDefault(x => x.Id.Equals(dw.MeasPointId));
                var cp =  cps.FirstOrDefault(x => x.MeasPointId.Equals(dw.MeasPointId));
                var well = wells.FirstOrDefault(x => x.WellName.Equals(mp.Name));
                if(cp != null && well != null && mp != null) { 
                    WaterHandling wh = new WaterHandling();
                    wh.ClearPumpTestId = cp.Id;
                    wh.DrillWaterId = dw.Id;
                    wh.DrillStart = well.Drill_Date_Start;
                    wh.DrillEnd = well.Drill_Date_End;
                    wh.PumpDateStart = cp.Report_Date;
                    wh.PumpDateEnd = cp.Report_Date;
                    wh.PumpWaterStart = cp.Water_Meter_Before;
                    wh.PumpWaterEnd = cp.Water_Meter_After;
                    wh.DrillWaterStart = dw.DrillWaterStart;
                    wh.DrillWaterEnd = dw.DrillWaterEnd;
                    wh.MeasPointId = mp.Id;
                    wh.WellId = well.Id;
                    wh.WellName = mp.Name;
                    _context.Add(wh);
                }

            }
            await _context.SaveChangesAsync();
            var whs = await _context.WaterHandlings.Include(x => x.MeasPoint).ThenInclude(x => x.Project)
                 .Include(x => x.DrillWater).ThenInclude(x => x.Photos)
                 .Include(x => x.Well)
                 .Include(x => x.ClearPumpTest)
                 .ToListAsync();
            var prev_installs = await _context.Installations.Where(x => x.ProjectId.Equals(whs.First().MeasPoint.ProjectId) && x.UniqueID.Contains("WaterHandling")).ToListAsync();
            foreach (var wh in whs)
            {
                var prev_inst = prev_installs.SingleOrDefault(x => x.UniqueID.Equals(String.Concat("WaterHandling (", wh.MeasPoint.Name, ")")));
                if (prev_inst != null)
                {
                    prev_inst.Amount = wh.TotalWater - 5.0;
                    prev_inst.DoneBy = "Calculated MainOps";
                    //prev_inst.EnteredIntoDataBase = DateTime.Now;
                    //prev_inst.InvoiceDate = DateTime.Now;
                    prev_inst.LastEditedInDataBase = DateTime.Now;
                    prev_inst.RentalStartDate = wh.Well.Drill_Date_Start.Date;
                    prev_inst.DeinstallDate = wh.PumpDateEnd.Date;
                    prev_inst.TimeStamp = wh.DrillStart.Date;
                    prev_inst.RentalStartDate = wh.DrillStart.Date;
                    prev_inst.isInstalled = false;
                    prev_inst.UniqueID = String.Concat("WaterHandling (", wh.MeasPoint.Name, ")");
                    prev_inst.ItemTypeId = 2061;
                    prev_inst.ProjectId = wh.MeasPoint.ProjectId;
                    prev_inst.SubProjectId = wh.MeasPoint.SubProjectId;
                    prev_inst.ToBePaid = true;
                    prev_inst.Latitude = Convert.ToDouble(wh.MeasPoint.Lati);
                    prev_inst.Longitude = Convert.ToDouble(wh.MeasPoint.Longi);
                    prev_inst.Install_Text = String.Concat("Changed on ", DateTime.Now.Date.ToString(), ". Water Handling from ", wh.MeasPoint.Name, " with ", wh.AmountContainerDrill.ToString(), " containers for drilling and rental for ", ((wh.DrillEnd - wh.DrillStart).TotalDays + 1).ToString(), " and ", wh.AmountContainerPump.ToString(), " containers for clearpumped water for ", ((wh.PumpDateEnd - wh.PumpDateStart).TotalDays + 1).ToString(), " days of rent");
                    _context.Update(prev_inst);
                }
                else
                {

                    Install inst = new Install();
                    inst.Amount = wh.TotalWater - 5.0;
                    inst.DoneBy = "Calculated MainOps";
                    inst.EnteredIntoDataBase = DateTime.Now;
                    inst.InvoiceDate = DateTime.Now;
                    inst.LastEditedInDataBase = DateTime.Now;
                    inst.RentalStartDate = wh.Well.Drill_Date_Start.Date;
                    inst.DeinstallDate = wh.PumpDateEnd.Date;
                    inst.TimeStamp = wh.DrillStart.Date;
                    inst.RentalStartDate = wh.DrillStart.Date;
                    inst.isInstalled = false;
                    inst.UniqueID = String.Concat("WaterHandling (", wh.MeasPoint.Name, ")");
                    inst.ItemTypeId = 2061;
                    inst.ProjectId = wh.MeasPoint.ProjectId;
                    inst.SubProjectId = wh.MeasPoint.SubProjectId;
                    inst.ToBePaid = true;
                    inst.Latitude = Convert.ToDouble(wh.MeasPoint.Lati);
                    inst.Longitude = Convert.ToDouble(wh.MeasPoint.Longi);
                    inst.Install_Text = String.Concat("Water Handling from ", wh.MeasPoint.Name, " with ", wh.AmountContainerDrill.ToString(), " containers for drilling and rental for ", ((wh.DrillEnd - wh.DrillStart).TotalDays + 1).ToString(), " and ", wh.AmountContainerPump.ToString(), " containers for clearpumped water for ", ((wh.PumpDateEnd - wh.PumpDateStart).TotalDays + 1).ToString(), " days of rent");
                    _context.Add(inst);
                }
                
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateWaterHandling()
        {
            var drillwaters = await _context.DrillWaters.ToListAsync();
            var cps = await _context.ClearPumpTests.Where(x => x.ProjectId.Equals(437)).OrderBy(x => x.Report_Date).ToListAsync();
            var wells = await _context.Wells.Where(x => x.ProjectId.Equals(437)).ToListAsync();
            var mps = await _context.MeasPoints.Where(x => x.ProjectId.Equals(437)).ToListAsync();
            foreach (var dw in drillwaters)
            {
                var wh_existing = await _context.DrillWaters.FirstOrDefaultAsync(x => x.Id.Equals(dw.Id));
                if(wh_existing == null)
                {
                    var mp = mps.SingleOrDefault(x => x.Id.Equals(dw.MeasPointId));
                    var cp = cps.FirstOrDefault(x => x.MeasPointId.Equals(dw.MeasPointId));
                    var well = wells.FirstOrDefault(x => x.WellName.Equals(mp.Name));
                    if (cp != null && well != null && mp != null)
                    {
                        WaterHandling wh = new WaterHandling();
                        wh.ClearPumpTestId = cp.Id;
                        wh.DrillWaterId = dw.Id;
                        wh.DrillStart = well.Drill_Date_Start;
                        wh.DrillEnd = well.Drill_Date_End;
                        wh.PumpDateStart = cp.Report_Date;
                        wh.PumpDateEnd = cp.Report_Date;
                        wh.PumpWaterStart = cp.Water_Meter_Before;
                        wh.PumpWaterEnd = cp.Water_Meter_After;
                        wh.DrillWaterStart = dw.DrillWaterStart;
                        wh.DrillWaterEnd = dw.DrillWaterEnd;
                        wh.MeasPointId = mp.Id;
                        wh.WellId = well.Id;
                        wh.WellName = mp.Name;
                        _context.Add(wh);
                    }
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,DivisionAdmin")]
        public async Task<IActionResult> InvoiceWater(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var wh = await _context.WaterHandlings.Include(x => x.MeasPoint).ThenInclude(x => x.Project)
                .Include(x => x.DrillWater).ThenInclude(x => x.Photos)
                .Include(x => x.Well)
                .Include(x => x.ClearPumpTest)
                .SingleOrDefaultAsync(x => x.Id.Equals(id));
            if(wh == null)
            {
                return NotFound();
            }
            if (!wh.MeasPoint.Project.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
            }
            var prev_inst = await _context.Installations.SingleOrDefaultAsync(x => x.ProjectId.Equals(wh.MeasPoint.ProjectId) && x.UniqueID.Equals(String.Concat("WaterHandling (", wh.MeasPoint.Name, ")")));
            if (prev_inst != null)
            {
                prev_inst.Amount = wh.TotalWater - 5.0;
                prev_inst.DoneBy = "Calculated MainOps";
                //prev_inst.EnteredIntoDataBase = DateTime.Now;
                //prev_inst.InvoiceDate = DateTime.Now;
                prev_inst.LastEditedInDataBase = DateTime.Now;
                prev_inst.RentalStartDate = wh.Well.Drill_Date_Start.Date;
                prev_inst.DeinstallDate = wh.PumpDateEnd.Date;
                prev_inst.TimeStamp = wh.DrillStart.Date;
                prev_inst.RentalStartDate = wh.DrillStart.Date;
                prev_inst.isInstalled = false;
                prev_inst.UniqueID = String.Concat("WaterHandling (", wh.MeasPoint.Name, ")");
                prev_inst.ItemTypeId = 2061;
                prev_inst.ProjectId = wh.MeasPoint.ProjectId;
                prev_inst.SubProjectId = wh.MeasPoint.SubProjectId;
                prev_inst.ToBePaid = true;
                prev_inst.Latitude = Convert.ToDouble(wh.MeasPoint.Lati);
                prev_inst.Longitude = Convert.ToDouble(wh.MeasPoint.Longi);
                prev_inst.Install_Text = String.Concat("Changed on ",DateTime.Now.Date.ToString(),". Water Handling from ", wh.MeasPoint.Name, " with ", wh.AmountContainerDrill.ToString(), " containers for drilling and rental for ", ((wh.DrillEnd - wh.DrillStart).TotalDays + 1).ToString(), " and ", wh.AmountContainerPump.ToString(), " containers for clearpumped water for ", ((wh.PumpDateEnd - wh.PumpDateStart).TotalDays + 1).ToString(), " days of rent");
                _context.Update(prev_inst);
                //if (wh.AmountContainerDrill > 0)
                //{
                //    var arr = await _context.Arrivals.SingleOrDefaultAsync(x => x.UniqueID.Equals(String.Concat("WaterHandling (D) (", wh.MeasPoint.Name, ")")));
                //    arr.Amount = wh.AmountContainerDrill;
                //    arr.ItemTypeId = 2062;
                //    arr.TimeStamp = wh.DrillStart.Date;
                //    arr.EndStamp = wh.DrillEnd.Date;
                //    arr.Arrival_Text = String.Concat("Changed on ",DateTime.Now.Date.ToString(),". Rental of containers for Drilling");
                //    arr.UniqueID = String.Concat("WaterHandling (D) (", wh.MeasPoint.Name, ")");
                //    arr.ProjectId = wh.MeasPoint.ProjectId;
                //    arr.SubProjectId = wh.MeasPoint.SubProjectId;
                //    arr.ToBePaid = true;
                //    //arr.EnteredIntoDataBase = DateTime.Now;
                //    arr.LastEditedInDataBase = DateTime.Now;
                //    arr.Latitude = Convert.ToDouble(wh.MeasPoint.Lati);
                //    arr.Longitude = Convert.ToDouble(wh.MeasPoint.Longi);
                //    _context.Update(arr);
                //}
                //if (wh.AmountContainerPump > 0)
                //{
                //    var arr = await _context.Arrivals.SingleOrDefaultAsync(x => x.UniqueID.Equals(String.Concat("WaterHandling (CP) (", wh.MeasPoint.Name, ")")));
                //    arr.Amount = wh.AmountContainerPump;
                //    arr.ItemTypeId = 2062;
                //    arr.TimeStamp = wh.PumpDateStart.Date;
                //    arr.EndStamp = wh.PumpDateEnd.Date;
                //    arr.Arrival_Text = String.Concat("Changed on ", DateTime.Now.Date.ToString(), ". Rental of containers for Clear Pumping");
                //    arr.UniqueID = String.Concat("WaterHandling (CP) (", wh.MeasPoint.Name, ")");
                //    arr.ProjectId = wh.MeasPoint.ProjectId;
                //    arr.SubProjectId = wh.MeasPoint.SubProjectId;
                //    arr.ToBePaid = true;
                //    //arr.EnteredIntoDataBase = DateTime.Now;
                //    arr.LastEditedInDataBase = DateTime.Now;
                //    arr.Latitude = Convert.ToDouble(wh.MeasPoint.Lati);
                //    arr.Longitude = Convert.ToDouble(wh.MeasPoint.Longi);
                //    _context.Update(arr);
                //}
            }
            else
            {
                
                Install inst = new Install();
                inst.Amount = wh.TotalWater - 5.0;
                inst.DoneBy = "Calculated MainOps";
                inst.EnteredIntoDataBase = DateTime.Now;
                inst.InvoiceDate = DateTime.Now;
                inst.LastEditedInDataBase = DateTime.Now;
                inst.RentalStartDate = wh.Well.Drill_Date_Start.Date;
                inst.DeinstallDate = wh.PumpDateEnd.Date;
                inst.TimeStamp = wh.DrillStart.Date;
                inst.RentalStartDate = wh.DrillStart.Date;
                inst.isInstalled = false;
                inst.UniqueID = String.Concat("WaterHandling (", wh.MeasPoint.Name, ")");
                inst.ItemTypeId = 2061;
                inst.ProjectId = wh.MeasPoint.ProjectId;
                inst.SubProjectId = wh.MeasPoint.SubProjectId;
                inst.ToBePaid = true;
                inst.Latitude = Convert.ToDouble(wh.MeasPoint.Lati);
                inst.Longitude = Convert.ToDouble(wh.MeasPoint.Longi);
                inst.Install_Text = String.Concat("Water Handling from ", wh.MeasPoint.Name, " with ", wh.AmountContainerDrill.ToString(), " containers for drilling and rental for ", ((wh.DrillEnd - wh.DrillStart).TotalDays + 1).ToString()," and ", wh.AmountContainerPump.ToString()," containers for clearpumped water for ", ((wh.PumpDateEnd - wh.PumpDateStart).TotalDays + 1).ToString(), " days of rent");
                _context.Add(inst);
                //if (wh.AmountContainerDrill > 0)
                //{
                //    Arrival arr = new Arrival();
                //    arr.Amount = wh.AmountContainerDrill;
                //    arr.ItemTypeId = 2062;
                //    arr.TimeStamp = wh.DrillStart.Date;
                //    arr.EndStamp = wh.DrillEnd.Date;
                //    arr.Arrival_Text = "Rental of containers for Drilling";
                //    arr.UniqueID = String.Concat("WaterHandling (D) (", wh.MeasPoint.Name, ")");
                //    arr.ProjectId = wh.MeasPoint.ProjectId;
                //    arr.SubProjectId = wh.MeasPoint.SubProjectId;
                //    arr.ToBePaid = true;
                //    arr.EnteredIntoDataBase = DateTime.Now;
                //    arr.LastEditedInDataBase = DateTime.Now;
                //    arr.Latitude = Convert.ToDouble(wh.MeasPoint.Lati);
                //    arr.Longitude = Convert.ToDouble(wh.MeasPoint.Longi);
                //    _context.Add(arr);
                //}
                //if (wh.AmountContainerPump > 0)
                //{
                //    Arrival arr = new Arrival();
                //    arr.Amount = wh.AmountContainerPump;
                //    arr.ItemTypeId = 2062;
                //    arr.TimeStamp = wh.PumpDateStart.Date;
                //    arr.EndStamp = wh.PumpDateEnd.Date;
                //    arr.Arrival_Text = "Rental of containers for Clear Pumping";
                //    arr.UniqueID = String.Concat("WaterHandling (CP) (", wh.MeasPoint.Name, ")");
                //    arr.ProjectId = wh.MeasPoint.ProjectId;
                //    arr.SubProjectId = wh.MeasPoint.SubProjectId;
                //    arr.ToBePaid = true;
                //    arr.EnteredIntoDataBase = DateTime.Now;
                //    arr.LastEditedInDataBase = DateTime.Now;
                //    arr.Latitude = Convert.ToDouble(wh.MeasPoint.Lati);
                //    arr.Longitude = Convert.ToDouble(wh.MeasPoint.Longi);
                //    _context.Add(arr);
                //}

            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
                
        }
        [HttpGet]
        public async Task<JsonResult> GetDrilling(int MeasPointId)
        {
            var mp = await _context.MeasPoints.SingleOrDefaultAsync(x => x.Id.Equals(MeasPointId));
            var drill = await _context.Wells.SingleOrDefaultAsync(x => x.WellName.Equals(mp.Name));
            return Json(drill);
        }
        [HttpGet]
        public async Task<JsonResult> GetClearPump(int MeasPointId)
        {  
            var clearpump = await _context.ClearPumpTests.FirstOrDefaultAsync(x => x.MeasPointId.Equals(MeasPointId) && x.DischargeAvailable.Equals(false));
            return Json(clearpump);
        }
        [HttpGet]
        public async Task<JsonResult> GetDrillWater(int MeasPointId)
        {
            var drillwater = await _context.DrillWaters.SingleOrDefaultAsync(x => x.MeasPointId.Equals(MeasPointId));
            return Json(drillwater);
        }
        // GET: WaterHandlings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waterHandling = await _context.WaterHandlings.FindAsync(id);
            if (waterHandling == null)
            {
                return NotFound();
            }
            ViewData["ClearPumpTestId"] = new SelectList(_context.ClearPumpTests, "Id", "Id", waterHandling.ClearPumpTestId);
            ViewData["DrillId"] = new SelectList(_context.Drillings, "Id", "Id", waterHandling.WellId);
            ViewData["DrillWaterId"] = new SelectList(_context.DrillWaters, "Id", "Id", waterHandling.DrillWaterId);
            ViewData["MeasPointId"] = new SelectList(_context.MeasPoints, "Id", "Name", waterHandling.MeasPointId);
            return View(waterHandling);
        }

        // POST: WaterHandlings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MeasPointId,ClearPumpTestId,DrillWaterId,WellName,WellId,PumpWaterStart,PumpWaterEnd,DrillWaterStart,DrillWaterEnd,AmountContainerDrill,AmountContainerPump,PumpDateStart,PumpDateEnd,DrillStart,DrillEnd")] WaterHandling waterHandling)
        {
            if (id != waterHandling.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(waterHandling);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WaterHandlingExists(waterHandling.Id))
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
            ViewData["ClearPumpTestId"] = new SelectList(_context.ClearPumpTests, "Id", "Id", waterHandling.ClearPumpTestId);
            ViewData["WellId"] = new SelectList(_context.Drillings, "Id", "Id", waterHandling.WellId);
            ViewData["DrillWaterId"] = new SelectList(_context.DrillWaters, "Id", "Id", waterHandling.DrillWaterId);
            ViewData["MeasPointId"] = new SelectList(_context.MeasPoints, "Id", "Name", waterHandling.MeasPointId);
            return View(waterHandling);
        }

        // GET: WaterHandlings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waterHandling = await _context.WaterHandlings
                .Include(w => w.ClearPumpTest)
                .Include(w => w.Well)
                .Include(w => w.DrillWater)
                .Include(w => w.MeasPoint)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (waterHandling == null)
            {
                return NotFound();
            }

            return View(waterHandling);
        }

        // POST: WaterHandlings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var waterHandling = await _context.WaterHandlings.FindAsync(id);
            _context.WaterHandlings.Remove(waterHandling);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WaterHandlingExists(int id)
        {
            return _context.WaterHandlings.Any(e => e.Id == id);
        }
    }
}
