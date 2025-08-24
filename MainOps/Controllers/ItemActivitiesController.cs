using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MainOps.Data;
using MainOps.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace MainOps.Controllers
{
    public class ItemActivitiesController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ItemActivitiesController(DataContext context, UserManager<ApplicationUser> userManager) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ItemActivities
        public IActionResult Index()
        {
            //var dataContext = _context.ItemActivities.Include(i => i.Arrival).Include(i => i.HJItem).Include(i => i.Install);
            //return View(await dataContext.ToListAsync());
            return RedirectToAction("UploadGPS");
        }
        public async Task<IActionResult> IndexBackup()
        {
            var dataContext = _context.ItemActivities.Include(i => i.Arrival).Include(i => i.HJItem).Include(i => i.Install);
            return View(await dataContext.ToListAsync());
        }
        [HttpGet]
        
        public async Task<IActionResult> UploadGPS()
        {
            var theuser = await _userManager.GetUserAsync(User);
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(theuser.DivisionId)), "Id", "Name");
            var oilchanges = await _context.NeedsOilChanges.OrderByDescending(x => x.HoursOperated).ToListAsync();
            return View(oilchanges);
        }
        [HttpPost]
        public async Task<IActionResult> UploadGPS(IFormFile file,DateTime StartTime,DateTime EndTime,int ProjectId,int? SubProjectId)
        {
            string fileExtension = Path.GetExtension(file.FileName);
            char lineseperator = ';';
            //Validate uploaded file and return error.
            if (fileExtension != ".csv")
            {
                ViewBag.Message = "Please select the csv file with .csv extension";
                return View("Index");
            }
            var user = await _userManager.GetUserAsync(User);
            using (var sreader = new StreamReader(file.OpenReadStream()))
            {

                var project = await _context.Projects.SingleOrDefaultAsync(x => x.Id.Equals(ProjectId));
                if (project.DivisionId != user.DivisionId && !User.IsInRole("Admin"))
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to that project" });
                }
                List<ItemActivity> IAs = new List<ItemActivity>();
                if (SubProjectId != null)
                {
                    IAs = await _context.ItemActivities.Where(x => x.ProjectId.Equals(ProjectId) && x.SubProjectId.Equals(SubProjectId)).ToListAsync();
                }
                else
                {
                    IAs = await _context.ItemActivities.Where(x => x.ProjectId.Equals(ProjectId)).ToListAsync();
                }
                string[] headers = sreader.ReadLine().Split(lineseperator);
                while (!sreader.EndOfStream)
                {

                    string[] rows = sreader.ReadLine().Split(lineseperator);
                    string ItemName = rows[3];
                    DateTime theDate = Convert.ToDateTime(rows[1]);
                    DateTime theDate2 = Convert.ToDateTime(rows[2]);
                    //string tobeparsed = rows[4];


                    int amountdays = 0;
                    for(DateTime dt = theDate.Date; dt <= theDate2.Date;dt = dt.AddDays(1))
                    {
                        amountdays += 1;
                    }
                    
                    TimeSpan OperationTime = new TimeSpan(); /*= TimeSpan.Parse(tobeparsed);*/
                        for(int i = 0; i < amountdays; i++)
                        {
                            if (i == 0)// first day, easy peasy
                            {
                                if(theDate2.Date == theDate.Date)
                                {
                                    OperationTime = (theDate2 - theDate);
                                }
                                else
                                {
                                    OperationTime = (new DateTime(theDate.Year, theDate.Month, theDate.Day, 23, 59, 59) - theDate);
                                }
                               
                            }
                            else if(i == (amountdays-1))
                            {
                                OperationTime = TimeSpan.Parse(String.Concat(theDate2.Hour.ToString("00"), ":", theDate2.Minute.ToString("00"), ":", theDate2.Second.ToString("00")));
                            }

                            else
                            {
                                OperationTime = TimeSpan.Parse("1.00:00:00");
                            }
                        
                        var existingIA = IAs.Where(x => x.ItemName.Equals(ItemName) && x.TheDate.Equals(theDate.AddDays(i).Date) && x.StartTime.Equals(theDate)).SingleOrDefault();
                        if (existingIA == null) //Not an existing IA - create new
                        {
                            ItemActivity IA = new ItemActivity();
                            IA.StartTime = theDate;
                            IA.ItemName = ItemName;
                            IA.TheDate = theDate.AddDays(i).Date;
                            IA.ProjectId = ProjectId;
                            IA.SubProjectId = SubProjectId;
                            IA.UniqueID = "#" + new String(ItemName.Where(Char.IsDigit).ToArray());
                            //IA.Install = await _context.Installations.LastOrDefaultAsync(x => x.UniqueID.Equals(IA.UniqueID) && x.TimeStamp >= StartTime && x.TimeStamp <= EndTime);
                            IA.OperationTime = OperationTime;
                            //if (IA.Install != null)
                            //{
                            //    IA.InstallId = IA.Install.Id;
                            //}
                            //else
                            //{
                            //    IA.Arrival = await _context.Arrivals.LastOrDefaultAsync(x => x.UniqueID.Contains(IA.UniqueID));
                            //    if (IA.Arrival != null)
                            //    {
                            //        IA.ArrivalId = IA.Arrival.Id;
                            //    }
                            //    else
                            //    {
                            //        IA.HJItem = await _context.HJItems.SingleOrDefaultAsync(x => x.HJId.Equals(ItemName));
                            //        if (IA.HJItemId != null)
                            //        {
                            //            IA.HJItemId = IA.HJItem.Id;
                            //        }
                            //    }
                            //}
                            if (OperationTime != TimeSpan.Zero)
                            {
                                IA.WasActive = true;
                            }
                            else
                            {
                                IA.WasActive = false;
                            }
                            _context.Add(IA);
                        }
                        else
                        { 
                            existingIA.ItemName = ItemName;
                            existingIA.TheDate = theDate.AddDays(i).Date;
                            existingIA.StartTime = theDate;
                            existingIA.ProjectId = ProjectId;
                            existingIA.SubProjectId = SubProjectId;
                            existingIA.UniqueID = "#" + new String(ItemName.Where(Char.IsDigit).ToArray());
                            existingIA.OperationTime = OperationTime;
                            if (OperationTime != TimeSpan.Zero)
                            {
                                existingIA.WasActive = true;
                            }
                            else
                            {
                                existingIA.WasActive = false;
                            }
                            _context.Update(existingIA);
                        }
                    }
                }
                await _context.SaveChangesAsync();
                ViewBag.Message = "Upload Success!";
                var pumps = await _context.ItemActivities.Select(x => x.UniqueID).Distinct().ToListAsync();
                var NeedsOilChanges = await _context.NeedsOilChanges.ToListAsync();
                IAs = await _context.ItemActivities.Where(x => x.ProjectId.Equals(ProjectId)).ToListAsync();
                foreach (var pump in pumps)
                {
                    var lastmaintenance = await (from m in _context.Maintenances join me in _context.MaintenanceEntries
                                              on m.Id equals me.MaintenanceId
                                              where ((m.MaintenancePoint.Replace("#", "").Contains(pump.Replace("#", "")) && !m.MaintenancePoint.Replace("#","").Contains("Horizontal")) || m.LogText.Replace("#", "").Contains(pump.Replace("#", ""))) && (me.MaintenanceSubTypeId.Equals(68) || me.MaintenanceSubTypeId.Equals(58) || me.MaintenanceSubTypeId.Equals(77)) orderby m.TimeStamp descending select m).FirstOrDefaultAsync();
                    //var lastmaintenance = await _context.Maintenances.Include(x => x.MaintenanceEntries).Where(x => (x.MaintenancePoint.ToLower().Replace("#", "").Contains(pump.Replace("#", "")) || x.LogText.ToLower().Contains(pump.Replace("#",""))) && x.MaintenanceEntries.Select(y => y.Id).Contains(68)).LastOrDefaultAsync();
                    double totalhourssincelastoilchange = 0.0;
                    if(lastmaintenance != null) {
                        totalhourssincelastoilchange = IAs.Where(x => x.UniqueID.Replace("#", "").Equals(pump.Replace("#","")) && x.TheDate >= lastmaintenance.TimeStamp).Sum(x => x.OperationTime.TotalSeconds / 3600.0);
                    }
                    else
                    {
                        totalhourssincelastoilchange = IAs.Where(x => x.UniqueID.Replace("#", "").Equals(pump.Replace("#", ""))).Sum(x => x.OperationTime.TotalSeconds / 3600.0);
                    }
                    var NeedsOilChange = NeedsOilChanges.SingleOrDefault(x => x.UniqueID.Equals(pump));
                    if (NeedsOilChange != null)
                    {
                        if (lastmaintenance != null)
                        {
                            NeedsOilChange.LastOilChange = lastmaintenance.TimeStamp;
                        }
                        else
                        {
                            NeedsOilChange.LastOilChange = DateTime.Now.AddDays(-1000);
                        }
                        NeedsOilChange.UniqueID = pump;
                        NeedsOilChange.HoursOperated = totalhourssincelastoilchange;
                        _context.Update(NeedsOilChange);
                    }
                    else
                    {
                        NeedsOilChange NOChange = new NeedsOilChange();
                        NOChange.UniqueID = pump;
                        NOChange.HoursOperated = totalhourssincelastoilchange;
                        if (lastmaintenance != null)
                        {
                            NOChange.LastOilChange = lastmaintenance.TimeStamp;
                        }
                        else
                        {
                            NOChange.LastOilChange = DateTime.Now.AddDays(-1000);
                        }
                        _context.Add(NOChange);
                    }

                    
                }
                await _context.SaveChangesAsync();
            }
            var oilchanges = await _context.NeedsOilChanges.OrderByDescending(x => x.HoursOperated).ToListAsync();
            ViewData["ProjectId"] = await GetProjectList();
            return View(oilchanges);

        }
        [HttpGet]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
        public async Task<IActionResult> AddActivity(int? ProjectId)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["ProjectId"] = await GetProjectList();
            if(ProjectId != null)
            {
                ViewData["UniqueIDs"] = new SelectList(_context.Installations.Where(x => x.UniqueID.Contains("#") && x.Project.DivisionId.Equals(user.DivisionId) && x.ProjectId.Equals(ProjectId) && x.TimeStamp >= DateTime.Now.AddDays(-150)), "UniqueID", "UniqueID");
            }
            else
            {
                ViewData["UniqueIDs"] = new SelectList(_context.Installations.Where(x => x.UniqueID.Contains("#") && x.Project.DivisionId.Equals(user.DivisionId) && x.TimeStamp >= DateTime.Now.AddDays(-150)), "UniqueID", "UniqueID");
            }
            
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager" )]
        public async Task<IActionResult> AddActivity(int ProjectId,string uniqueid,DateTime start, DateTime end)
        {
            var user = await _userManager.GetUserAsync(User);
            for(DateTime dt = start.Date; dt <= end.Date; dt = dt.AddDays(1))
            {
                ItemActivity IA = new ItemActivity();
                IA.ItemName = uniqueid;
                IA.OperationTime = TimeSpan.Parse("06:00:00");
                IA.ProjectId = ProjectId;
                IA.TheDate = dt;
                IA.UniqueID = uniqueid;
                IA.WasActive = true;
                IA.StartTime = dt;
                _context.Add(IA);
            }
            await _context.SaveChangesAsync();
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["UniqueIDs"] = new SelectList(_context.Installations.Where(x => x.UniqueID.Contains("#") && x.Project.DivisionId.Equals(user.DivisionId) && x.ProjectId.Equals(ProjectId) && x.TimeStamp >= DateTime.Now.AddDays(-150)), "UniqueID", "UniqueID");
            return View();
        }
        // GET: ItemActivities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemActivity = await _context.ItemActivities
                .Include(i => i.Arrival)
                .Include(i => i.HJItem)
                .Include(i => i.Install)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (itemActivity == null)
            {
                return NotFound();
            }

            return View(itemActivity);
        }

        // GET: ItemActivities/Create
        public IActionResult Create()
        {
            ViewData["ArrivalId"] = new SelectList(_context.Arrivals, "Id", "Id");
            ViewData["HJItemId"] = new SelectList(_context.HJItems, "Id", "Id");
            ViewData["InstallId"] = new SelectList(_context.Installations, "Id", "Id");
            return View();
        }

        // POST: ItemActivities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TheDate,WasActive,HJItemId,UniqueID,ProjectId,SubProjectId,Latitude,Longitude,ItemName,InstallId,ArrivalId")] ItemActivity itemActivity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(itemActivity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArrivalId"] = new SelectList(_context.Arrivals, "Id", "Id", itemActivity.ArrivalId);
            ViewData["HJItemId"] = new SelectList(_context.HJItems, "Id", "Id", itemActivity.HJItemId);
            ViewData["InstallId"] = new SelectList(_context.Installations, "Id", "Id", itemActivity.InstallId);
            return View(itemActivity);
        }

        // GET: ItemActivities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemActivity = await _context.ItemActivities.FindAsync(id);
            if (itemActivity == null)
            {
                return NotFound();
            }
            ViewData["ArrivalId"] = new SelectList(_context.Arrivals, "Id", "Id", itemActivity.ArrivalId);
            ViewData["HJItemId"] = new SelectList(_context.HJItems, "Id", "Id", itemActivity.HJItemId);
            ViewData["InstallId"] = new SelectList(_context.Installations, "Id", "Id", itemActivity.InstallId);
            return View(itemActivity);
        }

        // POST: ItemActivities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TheDate,WasActive,HJItemId,UniqueID,ProjectId,SubProjectId,Latitude,Longitude,ItemName,InstallId,ArrivalId")] ItemActivity itemActivity)
        {
            if (id != itemActivity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(itemActivity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemActivityExists(itemActivity.Id))
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
            ViewData["ArrivalId"] = new SelectList(_context.Arrivals, "Id", "Id", itemActivity.ArrivalId);
            ViewData["HJItemId"] = new SelectList(_context.HJItems, "Id", "Id", itemActivity.HJItemId);
            ViewData["InstallId"] = new SelectList(_context.Installations, "Id", "Id", itemActivity.InstallId);
            return View(itemActivity);
        }
        public async Task<IActionResult> GetUniqueIDData(string UniqueID)
        {
            var data = await _context.ItemActivities.Where(x => x.UniqueID.Equals(UniqueID)).OrderBy(x => x.TheDate).Select(x => x.TheDate).Distinct().ToListAsync();
            return PartialView("_dates", data);
        }
        // GET: ItemActivities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemActivity = await _context.ItemActivities
                .Include(i => i.Arrival)
                .Include(i => i.HJItem)
                .Include(i => i.Install)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (itemActivity == null)
            {
                return NotFound();
            }

            return View(itemActivity);
        }

        // POST: ItemActivities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var itemActivity = await _context.ItemActivities.FindAsync(id);
            _context.ItemActivities.Remove(itemActivity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemActivityExists(int id)
        {
            return _context.ItemActivities.Any(e => e.Id == id);
        }
    }
}
