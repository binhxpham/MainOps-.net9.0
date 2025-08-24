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
using Microsoft.AspNetCore.Authorization;
using MainOps.Models.ReportClasses;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Member,ProjectMember,Admin,DivisionAdmin,Manager")]
    public class TruckDailyReportsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TruckDailyReportsController(DataContext context,UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: TruckDailyReports
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            List<TruckDailyReport> drs = new List<TruckDailyReport>();
            try { 
                drs = await _context.TruckDailyReports
                .Include(x=>x.Sites).ThenInclude(x=>x.Project)
                .Include(y=>y.Sites).ThenInclude(y=>y.SubProject)
                .Include(z => z.HJItem)/*.ThenInclude(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass)*/
                .Where(z=>z.Sites.First().Project.DivisionId.Equals(user.DivisionId)).ToListAsync();
            }
            catch
            {
                drs = await _context.TruckDailyReports
                .Include(x => x.Sites).ThenInclude(x => x.Project)
                .Include(y => y.Sites).ThenInclude(y => y.SubProject)
                .Include(z => z.HJItem)
                .Where(z => z.Sites.Count() >= 1).Where(y => y.Sites.First().Project.DivisionId.Equals(user.DivisionId)).ToListAsync();
            }
            List<TruckIndexVM> model = new List<TruckIndexVM>();
            foreach(TruckDailyReport dr in drs)
            {
                foreach(TruckSite site in dr.Sites)
                {
                    //if(site.SubProjectId != null)
                    //{
                    //    var subproj = await _context.SubProjects.SingleOrDefaultAsync(x => x.Id.Equals(site.SubProjectId));
                    //    site.SubProject = subproj;
                    //}
                    TruckIndexVM vm = new TruckIndexVM(dr, site);
                    model.Add(vm);
                }
            }
            return View(model);
           

            
        }
        public async Task<IActionResult> Search(DateTime starttime,DateTime endtime)
        {
            var user = await _userManager.GetUserAsync(User);
            DateTime Start = starttime.Date;
            DateTime End = endtime.Date;
            List<TruckIndexVM> model = new List<TruckIndexVM>();
            var drs = await _context.TruckDailyReports.Include(x => x.Sites).ThenInclude(x => x.Project)
                .Include(x => x.Sites).ThenInclude(x => x.SubProject)
                .Include(x => x.HJItem).Where(x => x.Sites.First().Project.DivisionId.Equals(user.DivisionId) && x.Dato >= Start && x.Dato <= End).ToListAsync();
            foreach (TruckDailyReport dr in drs)
            {
                foreach (TruckSite site in dr.Sites)
                {
                    TruckIndexVM vm = new TruckIndexVM(dr, site);
                    model.Add(vm);
                }
            }
            return View(nameof(Index), model);
        }
        // GET: TruckDailyReports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var truckDailyReport = await _context.TruckDailyReports.Include(x => x.HJItem).Include(x=>x.Sites).ThenInclude(x=>x.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (truckDailyReport == null)
            {
                return NotFound();
            }
            else if (!truckDailyReport.Sites.First().Project.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
            }

            return View(truckDailyReport);
        }

        // GET: TruckDailyReports/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            TruckVM vmModel = new TruckVM();
            ViewData["ProjectId"] = await GetProjectList3();
            ViewData["HJItemId"] = new SelectList(_context.HJItems.Where(x => x.DivisionId.Equals(user.DivisionId) && x.HJItemClass.HJItemMasterClassId.Equals(14)), "Id", "Name");
            return View(vmModel);
        }

        // POST: TruckDailyReports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Manager,DivisionAdmin")]
        public async Task<IActionResult> UpdateTruckDailyReports()
        {
            var user = await _userManager.GetUserAsync(User);
            var truckDailyReports = await _context.TruckDailyReports.Include(x => x.Sites).ThenInclude(x => x.Project).Where(x=>x.Sites.First().Project.DivisionId.Equals(user.DivisionId)).ToListAsync();
            foreach(var tDR in truckDailyReports)
            {
                foreach(var site in tDR.Sites)
                {
                    var potentialExistingDailyReport = await _context.Daily_Report_2s.Where(x => x.Report_Date.Date.Equals(tDR.Dato.Date) && x.short_Description.Equals("Truck Daily Report") && x.ProjectId.Equals(site.ProjectId) && x.Hours.Equals(site.Hours)).SingleOrDefaultAsync();
                    if(potentialExistingDailyReport == null)
                    {
                        await CreateDailyReportFromTruckDailyReportAndSite(tDR, site);
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,Manager,DivisionAdmin")]
        public async Task<IActionResult> MoveTruckDailyReports()
        {
            var user = await _userManager.GetUserAsync(User);
            var truckDailyReports = await _context.Daily_Report_2s.Where(x => x.ProjectId.Equals(60) && x.short_Description.Equals("Truck Daily Report")).ToListAsync();
            foreach(var dr in truckDailyReports)
            {
                var proj = await _context.Projects.FindAsync(dr.ProjectId);
                dr.ProjectId = 70;
                dr.TitleId = 504;
                dr.Work_Performed = dr.Work_Performed + Environment.NewLine + "(Was moved from " + proj.ProjectNr + ")";
                _context.Update(dr);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,Manager,DivisionAdmin")]
        public async Task<IActionResult> DeleteAllTruckDailyReports()
        {
            var user = await _userManager.GetUserAsync(User);
            var truckDailyReports = await _context.Daily_Report_2s.Where(x => x.short_Description.Equals("Truck Daily Report")).ToListAsync();
            foreach (var dr in truckDailyReports)
            {
                _context.Remove(dr);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task CreateDailyReportFromTruckDailyReportAndSite(TruckDailyReport TDR,TruckSite s)
        {
            var itemtype = await _context.ItemTypes.SingleOrDefaultAsync(x => x.Item_Type.Equals("Truck") && x.ProjectId.Equals(s.ProjectId));
            var thetitle = await _context.Titles.SingleOrDefaultAsync(x => x.TheTitle.Equals("Truck") && x.ProjectId.Equals(s.ProjectId));
            var boqheadline = await _context.BoQHeadLines.OrderBy(x=>x.BoQnum).FirstOrDefaultAsync(x => x.ProjectId.Equals(s.ProjectId) && x.Type.Equals("Hours"));
            var boqheadlines = await _context.BoQHeadLines.Where(x => x.ProjectId.Equals(s.ProjectId)).ToListAsync();
            decimal itemtypenr;
            if (boqheadline == null)
            {
                boqheadline = new BoQHeadLine();
                boqheadline.HeadLine = "Hours";
                boqheadline.BoQnum = boqheadlines.Max(x => x.BoQnum) + 1;
                boqheadline.ProjectId = Convert.ToInt32(s.ProjectId);
            }
            var existingItemTypes = await _context.ItemTypes.Where(x => x.BoQnr_Rental >= boqheadline.BoQnum && x.BoQnr_Rental < boqheadline.BoQnum + 1).ToListAsync();
            if (existingItemTypes.Count() > 0)
            {
                // exists 1 item only within the hours - use +0.1
                if (existingItemTypes.Count() > 1)
                {
                    //exists at least two items within the hours - use diff to find next number
                    var lasttwo = existingItemTypes.OrderByDescending(x => x.BoQnr_Rental).Take(2);
                    itemtypenr = Convert.ToDecimal(lasttwo.First().BoQnr_Rental) - Convert.ToDecimal(lasttwo.Last().BoQnr_Rental);
                }
                else
                {
                    if (Convert.ToDecimal(existingItemTypes.First().BoQnr_Rental) % (decimal)0.1 == 0)
                    {
                        itemtypenr = Convert.ToDecimal(existingItemTypes.First().BoQnr_Rental) + (decimal)0.1;
                    }
                    else
                    {
                        itemtypenr = Convert.ToDecimal(existingItemTypes.First().BoQnr_Rental) + (decimal)0.01;
                    }
                }
            }
            else
            {
                itemtypenr = boqheadline.BoQnum + (decimal)0.01;
            }
            if (itemtype == null && thetitle == null)
            {
                ItemType trucktype = new ItemType { BoQnr_Rental = itemtypenr, BoQnr = itemtypenr, Install_UnitId = 1, Rental_UnitId = 5, Item_Type = "Truck", ProjectId = s.ProjectId, price = 0, rental_price = (decimal)39.0, Valuta = "EURO" };
                _context.Add(trucktype);
                await _context.SaveChangesAsync();
                thetitle = new Title { ItemTypeId = (await _context.ItemTypes.LastAsync()).Id, ProjectId = s.ProjectId, TheTitle = "Truck", Worker = false };
                _context.Add(thetitle);
                await _context.SaveChangesAsync();
            }
            else if (itemtype == null && thetitle != null)
            {
                ItemType trucktype = new ItemType { BoQnr_Rental = itemtypenr, BoQnr = itemtypenr, Install_UnitId = 1, Rental_UnitId = 5, Item_Type = "Truck", ProjectId = s.ProjectId, price = 0, rental_price = (decimal)39.0, Valuta = "EURO" };
                _context.Add(trucktype);
                await _context.SaveChangesAsync();
            }
            else if (itemtype != null && thetitle == null)
            {
                thetitle = new Title { ItemTypeId = itemtype.Id, ProjectId = s.ProjectId, TheTitle = "Truck", Worker = false };
                _context.Add(thetitle);
                await _context.SaveChangesAsync();
            }
            itemtype = await _context.ItemTypes.SingleOrDefaultAsync(x => x.Item_Type.Equals("Truck") && x.ProjectId.Equals(s.ProjectId));
            thetitle = await _context.Titles.SingleOrDefaultAsync(x => x.TheTitle.Equals("Truck") && x.ProjectId.Equals(s.ProjectId));
            Daily_Report_2 dr = new Daily_Report_2();
            dr.StartHour = TimeSpan.Zero;
            dr.EndHour = s.Hours;
            dr.Amount = 1;
            dr.DoneBy = TDR.DoneBy;
            dr.EnteredIntoDataBase = DateTime.Now;
            dr.ProjectId = Convert.ToInt32(s.ProjectId);
            dr.Report_Date = TDR.Dato;
            dr.SafetyHours = TimeSpan.Zero;
            dr.StandingTime = TimeSpan.Zero;
            dr.SubProjectId = s.SubProjectId;
            dr.TitleId = thetitle.Id;
            dr.short_Description = "Truck Daily Report";

            var lastsignature = await _context.Daily_Report_2s.Where(x => x.DoneBy.Equals(TDR.DoneBy)).LastAsync();
            dr.Signature = lastsignature.Signature;
            if (s.KM_start != null && s.KM_end != null)
            {
                dr.Work_Performed = s.Description + Environment.NewLine + "Driving: " + String.Format("{0:n2}", Convert.ToDouble(s.KM_end - s.KM_start)) + " kilometers."; // + Environment.NewLine + "This Report was generated automatically and thus, the signature is invalid.";
            }
            else
            {
                dr.Work_Performed = s.Description; //+ Environment.NewLine + "This Report was generated automatically and thus, the signature is invalid.";
            }
            dr.Machinery = "Truck";
            dr.tobepaid = 3;
            _context.Add(dr);
            await _context.SaveChangesAsync();
        }
        public async Task CreateDailyReportFromTruckDailyReport(TruckDailyReport TDR)
        {
            //check first if itemtype / title truck exists:
            foreach(TruckSite s in TDR.Sites) { 
                var itemtype = await _context.ItemTypes.SingleOrDefaultAsync(x => x.Item_Type.Equals("Truck") && x.ProjectId.Equals(s.ProjectId));
                var thetitle = await _context.Titles.SingleOrDefaultAsync(x => x.TheTitle.Equals("Truck") && x.ProjectId.Equals(s.ProjectId));
                var boqheadline = await _context.BoQHeadLines.SingleOrDefaultAsync(x => x.ProjectId.Equals(s.Id) && x.Type.Equals("Hours"));
                var boqheadlines = await _context.BoQHeadLines.Where(x => x.ProjectId.Equals(s.ProjectId)).ToListAsync();
                decimal itemtypenr;
                if(boqheadline == null)
                {
                    boqheadline = new BoQHeadLine();
                    boqheadline.HeadLine = "Hours";
                    boqheadline.BoQnum = boqheadlines.Max(x => x.BoQnum) + 1;
                    boqheadline.ProjectId = Convert.ToInt32(s.ProjectId);
                }
                var existingItemTypes = await _context.ItemTypes.Where(x => x.BoQnr_Rental >= boqheadline.BoQnum && x.BoQnr_Rental < boqheadline.BoQnum + 1).ToListAsync();
                if(existingItemTypes.Count() > 0)
                {
                    // exists 1 item only within the hours - use +0.1
                    if(existingItemTypes.Count() > 1)
                    {
                        //exists at least two items within the hours - use diff to find next number
                        var lasttwo = existingItemTypes.OrderByDescending(x => x.BoQnr_Rental).Take(2);
                        itemtypenr = Convert.ToDecimal(lasttwo.First().BoQnr_Rental) - Convert.ToDecimal(lasttwo.Last().BoQnr_Rental);
                    }
                    else
                    {
                        if(Convert.ToDecimal(existingItemTypes.First().BoQnr_Rental) % (decimal)0.1 == 0) { 
                            itemtypenr = Convert.ToDecimal(existingItemTypes.First().BoQnr_Rental) + (decimal)0.1;
                        }
                        else
                        {
                            itemtypenr = Convert.ToDecimal(existingItemTypes.First().BoQnr_Rental) + (decimal)0.01;
                        }
                    }
                }
                else
                {
                    itemtypenr = boqheadline.BoQnum + (decimal)0.01;
                }
                if(itemtype == null && thetitle == null)
                {
                    ItemType trucktype = new ItemType { BoQnr_Rental = itemtypenr, BoQnr = itemtypenr, Install_UnitId = 1, Rental_UnitId = 5, Item_Type = "Truck", ProjectId = s.ProjectId, price = 0, rental_price = (decimal)39.0, Valuta = "EURO" };
                    _context.Add(trucktype);
                    await _context.SaveChangesAsync();
                    thetitle = new Title { ItemTypeId = (await _context.ItemTypes.LastAsync()).Id, ProjectId = s.ProjectId, TheTitle = "Truck", Worker = false };
                    _context.Add(thetitle);
                    await _context.SaveChangesAsync();
                }
                else if(itemtype == null && thetitle != null)
                {
                    ItemType trucktype = new ItemType { BoQnr_Rental = itemtypenr, BoQnr = itemtypenr, Install_UnitId = 1, Rental_UnitId = 5, Item_Type = "Truck", ProjectId = s.ProjectId, price = 0, rental_price = (decimal)39.0, Valuta = "EURO" };
                    _context.Add(trucktype);
                    await _context.SaveChangesAsync();
                }
                else if(itemtype != null && thetitle == null)
                {
                    thetitle = new Title { ItemTypeId = itemtype.Id, ProjectId = s.ProjectId, TheTitle = "Truck", Worker = false };
                    _context.Add(thetitle);
                    await _context.SaveChangesAsync();
                }
                itemtype = await _context.ItemTypes.SingleOrDefaultAsync(x => x.Item_Type.Equals("Truck") && x.ProjectId.Equals(s.ProjectId));
                thetitle = await _context.Titles.SingleOrDefaultAsync(x => x.TheTitle.Equals("Truck") && x.ProjectId.Equals(s.ProjectId));
                Daily_Report_2 dr = new Daily_Report_2();
                dr.StartHour = TimeSpan.Zero;
                dr.EndHour = s.Hours;
                dr.Amount = 1;
                dr.DoneBy = TDR.DoneBy;
                dr.EnteredIntoDataBase = DateTime.Now;
                dr.ProjectId = Convert.ToInt32(s.ProjectId);
                dr.Report_Date = TDR.Dato;
                dr.SafetyHours = TimeSpan.Zero;
                dr.StandingTime = TimeSpan.Zero;
                dr.SubProjectId = s.SubProjectId;
                dr.TitleId = thetitle.Id;
                dr.short_Description = "Truck Daily Report";
                
                var lastsignature = await _context.Daily_Report_2s.Where(x => x.DoneBy.Equals(TDR.DoneBy)).LastAsync();
                dr.Signature = lastsignature.Signature;
                if (s.KM_start != null && s.KM_end != null)
                {
                    dr.Work_Performed = s.Description + Environment.NewLine + "Driving: " + String.Format("{0:n2}", Convert.ToDouble(s.KM_end - s.KM_start)) + " kilometers."; // + Environment.NewLine + "This Report was generated automatically and thus, the signature is invalid.";
                }
                else
                {
                    dr.Work_Performed = s.Description; //+ Environment.NewLine + "This Report was generated automatically and thus, the signature is invalid.";
                }
                dr.Machinery = "Truck";
                dr.tobepaid = 3;
                _context.Add(dr);
                await _context.SaveChangesAsync();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TruckVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                TruckDailyReport dr = new TruckDailyReport();
                dr.HJItemId = model.HJItemId;
                dr.Dato = model.Dato;
                dr.DoneBy = user.full_name();
                _context.Add(dr);
                 await _context.SaveChangesAsync();
                var lastadded = await _context.TruckDailyReports.LastAsync();
                foreach(TruckSite site in model.Sites.Where(x=>x.Hours.TotalHours > 0 || x.KM_end > 0 || x.KM_start > 0))
                {
                    site.TruckDailyReportId = lastadded.Id;
                    _context.Add(site);
                    lastadded.Sites.Add(site);
                }
                await _context.SaveChangesAsync();
                await CreateDailyReportFromTruckDailyReport(lastadded);
                return RedirectToAction("MainMenu","TrackItems");
            }
            //return View(truckDailyReport);
            ViewData["ProjectId"] = await GetProjectList3();
            return View(model);
        }


        // GET: TruckDailyReports/Delete/5
        public async Task<IActionResult> DeleteSite(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var trucksite = await _context.TruckSites.Include(x=>x.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (trucksite == null)
            {
                return NotFound();
            }
            else if (!trucksite.Project.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
            }
            
            return View(trucksite);
        }
        public async Task<IActionResult> DeleteReport(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var truckdr = await _context.TruckDailyReports.Include(x=>x.Sites).ThenInclude(x=>x.Project)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (truckdr == null)
            {
                return NotFound();
            }
            else if (!truckdr.Sites.First().Project.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
            }

            return View(truckdr);
        }
        [HttpGet]
        public async Task<IActionResult> LoadLast()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user != null)
            {
                var lastreport = await _context.TruckDailyReports.Include(x => x.Sites).Where(x => x.DoneBy.Equals(user.full_name())).LastAsync();
                TruckVM vmModel = new TruckVM(lastreport);
                ViewData["ProjectId"] = await GetProjectList3();
                ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => vmModel.Sites.Select(y => y.ProjectId).Contains(x.ProjectId)), "Id", "Name");
                return View("Edit", vmModel);  
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "Please log in before attempting to retrieve data" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if(id != null)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var lastreport = await _context.TruckDailyReports.Include(x => x.Sites).Where(x => x.Id.Equals(id)).SingleOrDefaultAsync();
                    if(lastreport != null) { 
                        TruckVM vmModel = new TruckVM(lastreport);
                        ViewData["ProjectId"] = await GetProjectList3();
                        ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x=>vmModel.Sites.Select(y=>y.ProjectId).Contains(x.ProjectId)),"Id","Name");
                        ViewData["HJItemId"] = new SelectList(_context.HJItems.Where(x => x.DivisionId.Equals(user.DivisionId) && x.HJItemClass.HJItemMasterClassId.Equals(14)), "Id", "Name");
                        return View("Edit", vmModel);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "Please log in before attempting to retrieve data" });
                }

            }
            else { return NotFound(); }
            
        }
        [HttpPost]
        public async Task<IActionResult> Edit(TruckVM model)
        {
            var original = await _context.TruckDailyReports.Include(x => x.Sites).SingleOrDefaultAsync(x => x.Id.Equals(model.TruckDailyReportId));
            foreach(var site in original.Sites)
            {
                var dailyreport = await _context.Daily_Report_2s.SingleOrDefaultAsync(x => x.Report_Date.Equals(original.Dato) && x.short_Description.Equals("Truck Daily Report") && x.ProjectId.Equals(site.ProjectId) && x.Hours.Equals(site.Hours));
                if (dailyreport != null)
                {
                    _context.Remove(dailyreport);
                }
            }
            
            for(int i = 0; i < 10; i++)
            {

                if(original.Sites.Count() > i)
                {
                    if(model.Sites.ElementAt(i).ProjectId != null) {
                        
                        original.Sites.ElementAt(i).Description =  model.Sites.ElementAt(i).Description;
                        original.Sites.ElementAt(i).Hours = model.Sites.ElementAt(i).Hours;
                        original.Sites.ElementAt(i).KM_end = model.Sites.ElementAt(i).KM_end;
                        original.Sites.ElementAt(i).KM_start = model.Sites.ElementAt(i).KM_start;
                        original.Sites.ElementAt(i).ProjectId = model.Sites.ElementAt(i).ProjectId;
                        original.Sites.ElementAt(i).SubProjectId = model.Sites.ElementAt(i).SubProjectId;
                        _context.Update(original.Sites.ElementAt(i));
                    }
                    else
                    {

                        _context.Remove(original.Sites.ElementAt(i));
                    }
                }
                else
                {
                    if(model.Sites.ElementAt(i).ProjectId != null)
                    {
                        model.Sites.ElementAt(i).TruckDailyReportId = model.TruckDailyReportId;
                        _context.Add(model.Sites.ElementAt(i));
                    }
                }
            }
            await _context.SaveChangesAsync();
            original = await _context.TruckDailyReports.Include(x => x.Sites).SingleOrDefaultAsync(x => x.Id.Equals(model.TruckDailyReportId));
            original.HJItemId = model.HJItemId;
            original.Dato = model.Dato;
            _context.Update(original);
            await _context.SaveChangesAsync();
            await CreateDailyReportFromTruckDailyReport(original);
            return RedirectToAction("MainMenu", "TrackItems");
        }
        // POST: TruckDailyReports/Delete/5
        [HttpPost, ActionName("DeleteReport")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var truckDailyReport = await _context.TruckDailyReports.Where(x=>x.Id.Equals(id)).SingleOrDefaultAsync();
            var sites = await _context.TruckSites.Where(x => x.TruckDailyReportId.Equals(truckDailyReport.Id)).ToListAsync();
            
            foreach(var site in sites)
            {
                var dailyreport = await _context.Daily_Report_2s.SingleOrDefaultAsync(x => x.Report_Date.Equals(truckDailyReport.Dato) && x.short_Description.Equals("Truck Daily Report") && x.ProjectId.Equals(site.ProjectId) && x.Hours.Equals(site.Hours));
                if(dailyreport != null) { 
                    _context.Remove(dailyreport);
                }
                _context.TruckSites.Remove(site);
            }
            await _context.SaveChangesAsync();
            _context.TruckDailyReports.Remove(truckDailyReport);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost, ActionName("DeleteSite")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed2(int id)
        {
            var trucksite = await _context.TruckSites.Where(x => x.Id.Equals(id)).SingleOrDefaultAsync();
            var truckDailyReport = await _context.TruckDailyReports.SingleOrDefaultAsync(x => x.Id.Equals(trucksite.TruckDailyReportId));
            var dailyreport = await _context.Daily_Report_2s.SingleOrDefaultAsync(x => x.Report_Date.Equals(truckDailyReport.Dato) && x.short_Description.Equals("Truck Daily Report") && x.ProjectId.Equals(trucksite.ProjectId) && x.Hours.Equals(trucksite.Hours));
            if (dailyreport != null)
            {
                _context.Remove(dailyreport);
            }
            _context.TruckSites.Remove(trucksite);
            await _context.SaveChangesAsync();
            int remainingsites = await _context.TruckSites.Where(x => x.TruckDailyReportId.Equals(trucksite.TruckDailyReportId)).CountAsync();
            if(remainingsites < 1)
            {
                var truckdailyreport = await _context.TruckDailyReports.SingleOrDefaultAsync(x => x.Id.Equals(trucksite.TruckDailyReportId));
                _context.Remove(truckdailyreport);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        private bool TruckDailyReportExists(int id)
        {
            return _context.TruckDailyReports.Any(e => e.Id == id);
        }
        
    }
}
