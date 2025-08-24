using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MainOps.Data;
using MainOps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MainOps.Resources;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using MainOps.ExtensionMethods;
using System.Globalization;
using MainOps.Models.ViewModels;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,Guest,MemberGuest,International")]
    public class InstallsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LocService _localizer;
        private IWebHostEnvironment _env;
        [Authorize(Roles = "Admin")]
        public IActionResult InstallsMenu()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> InstallsWithoutSubProject(int? ProjectId)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["ProjectId"] = await GetProjectList();
            List<Install> data;
            if (ProjectId != null)
            {
                data = await _context.Installations.Include(x => x.Project).Include(x => x.SubProject).Include(x => x.ItemType).Where(x => x.ProjectId.Equals(ProjectId) && x.SubProjectId == null && (!x.ItemTypeId.Equals(1782) && !x.ItemTypeId.Equals(1824) && !x.ItemTypeId.Equals(1786) && !x.ItemTypeId.Equals(1781) && !x.ItemTypeId.Equals(1824) && !x.ItemTypeId.Equals(1723) && !x.ItemTypeId.Equals(1786))).ToListAsync();

            }
            else
            {
                data = await _context.Installations.Include(x => x.Project).Include(x => x.SubProject).Include(x => x.ItemType).Where(x => x.ProjectId.Equals(418) && x.SubProjectId == null
                && !x.ItemTypeId.Equals(1782) && !x.ItemTypeId.Equals(1824) && !x.ItemTypeId.Equals(1723) && !x.ItemTypeId.Equals(1786)).ToListAsync();
            }
            return View(data);
        }
        public async Task<IActionResult> GetHorizontalDrainInfos()
        {
            var data = await (from i in _context.Installations
                              join a in _context.Arrivals on i.UniqueID equals a.UniqueID
                              where i.ItemTypeId == 1703 && a.ItemTypeId == 1703
                              select new InstallArrival { ins = i, ars = a }).ToListAsync();
            return View(data);

            //select ins.id,ins.uniqueid,ins.rentalstartdate,ins.deinstalldate,ars.timestamp,ars.endstamp,ins.amount,ars.amount from installations ins inner join arrivals ars on ins.uniqueid = ars.uniqueid where ins.itemtypeid = 1703 and ars.itemtypeid = 1703;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> InstallReinfitlrationWellsFromSetup()
        {
            var cabsetups = await _context.Installations.Where(x => x.ItemTypeId.Equals(2011)).ToListAsync();
            
            foreach(var cab in cabsetups)
            {
                int number = 0;
                var pipecon = await _context.Installations.Where(x => x.UniqueID.Equals(cab.UniqueID) && (x.ItemTypeId.Equals(1903) || x.ItemTypeId.Equals(2107))).ToListAsync();
                if (cab.UniqueID.Contains("CRW"))
                {
                    number = 100;
                }
                else
                {
                    number = Convert.ToInt32(cab.UniqueID.Substring(6, 2));
                }
                
                Install inst = new Install();
                inst.Latitude = cab.Latitude;
                inst.Longitude = cab.Longitude;
                inst.UniqueID = cab.UniqueID;
                if (number >= 7 && number <= 31)
                {
                    inst.TimeStamp = new DateTime(2024, 04, 05, 0, 0, 0);
                }
                else if (number >= 61 && number <= 68 || number == 100)
                {
                    inst.TimeStamp = new DateTime(2024, 04, 12, 0, 0, 0);
                }
                else
                {
                    inst.TimeStamp = new DateTime(2024, 04, 16, 0, 0, 0);
                }

                inst.RentalStartDate = inst.TimeStamp;
                inst.InvoiceDate = new DateTime(2024, 04, 20, 0, 0, 0);
                inst.ProjectId = 437;
                inst.SubProjectId = 238;
                if(pipecon.Count() > 0)
                {
                    inst.ItemTypeId = 1932;
                }
                else
                {
                    inst.ItemTypeId = 1931;
                }
                inst.isInstalled = true;
                inst.Amount = 1;
                inst.EnteredIntoDataBase = DateTime.Now;
                inst.ToBePaid = true;
                inst.DoneBy = "MainOps";
                inst.Install_Text = "System Added Installation of Reinfiltration Cabinet, based on previous cabinet set data";
                _context.Add(inst);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("MainMenu", "TrackItems");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddSediChecks()
        {
            var itemtype = await _context.ItemTypes.Include(x => x.ReportType).Where(x => x.ProjectId.Equals(562) && x.ReportType.Type.Equals("SediCheck")).SingleOrDefaultAsync();

            if (itemtype != null)
            {
                var sedis = await _context.SedimentationSiteReports.ToListAsync();
                foreach(var lastadded in sedis) { 
                    Install inst = new Install
                    {
                        ToBePaid = true,
                        ItemTypeId = itemtype.Id,
                        Latitude = 0,
                        Longitude = 0,
                        TimeStamp = lastadded.TimeStamp,
                        InvoiceDate = lastadded.TimeStamp,
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
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("MainMenu", "TrackItems");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ArrivalReinfitlrationWellsFromSetup()
        {
            var cabsetups = await _context.Installations.Where(x => x.ItemTypeId.Equals(2011)).ToListAsync();

            foreach (var cab in cabsetups)
            {
                int number = 0;
                var pipecon = await _context.Installations.Where(x => x.UniqueID.Equals(cab.UniqueID) && (x.ItemTypeId.Equals(1903) || x.ItemTypeId.Equals(2107))).ToListAsync();
                if (cab.UniqueID.Contains("CRW"))
                {
                    number = 100;
                }
                else
                {
                    number = Convert.ToInt32(cab.UniqueID.Substring(6, 2));
                }

                Arrival inst = new Arrival();
                inst.Latitude = cab.Latitude;
                inst.Longitude = cab.Longitude;
                inst.UniqueID = cab.UniqueID;
                if (number >= 7 && number <= 31)
                {
                    inst.TimeStamp = new DateTime(2024, 04, 05, 0, 0, 0);
                }
                else if (number >= 61 && number <= 68 || number == 100)
                {
                    inst.TimeStamp = new DateTime(2024, 04, 12, 0, 0, 0);
                }
                else
                {
                    inst.TimeStamp = new DateTime(2024, 04, 16, 0, 0, 0);
                }
                inst.InvoiceDate = new DateTime(2024, 04, 20, 0, 0, 0);
                inst.ProjectId = 437;
                inst.SubProjectId = 238;
                inst.Amount = 1;
                if (pipecon.Count() > 0)
                {
                    inst.ItemTypeId = 1932;
                }
                else
                {
                    inst.ItemTypeId = 1931;
                }
                inst.EnteredIntoDataBase = DateTime.Now;
                inst.ToBePaid = true;
                inst.Arrival_Text = "System Added Installation of Reinfiltration Cabinet, based on previous cabinet set data";
                _context.Add(inst);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("MainMenu", "TrackItems");
        }
        public class InstallArrival
        {
            public Install ins { get; set; }
            public Arrival ars { get; set; }
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> InstallsWithoutArrival(int? ProjectId, int? SubProjectId)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["ProjectId"] = await GetProjectList();
            List<Install> EndResult = new List<Install>();
            if (ProjectId != null && SubProjectId != null)
            {
                var data = await _context.Installations.Include(x => x.Project).Include(x => x.SubProject).Include(x => x.ItemType).Where(x => (x.ProjectId.Equals(ProjectId) && x.SubProjectId.Equals(SubProjectId) && x.UniqueID.Contains("#") && !x.ItemTypeId.Equals(1782) && !x.ItemTypeId.Equals(1824) && !x.ItemTypeId.Equals(1723) && !x.ItemTypeId.Equals(1786))).ToListAsync();
                var arrivals = await _context.Arrivals.Where(x => x.ProjectId.Equals(ProjectId) && x.SubProjectId.Equals(SubProjectId) && x.UniqueID.Contains("#") && (!x.ItemTypeId.Equals(1782) && !x.ItemTypeId.Equals(1824) && !x.ItemTypeId.Equals(1723) && !x.ItemTypeId.Equals(1786))).ToListAsync();
                foreach (var ins in data)
                {
                    var associated_arrival = arrivals.Where(x => x.UniqueID.Equals(ins.UniqueID) && x.ItemTypeId.Equals(ins.ItemTypeId) && x.TimeStamp <= ins.RentalStartDate).SingleOrDefault();
                    if (associated_arrival != null)
                    {
                        if (associated_arrival.EndStamp != null)
                        {
                            if (associated_arrival.EndStamp < ins.RentalStartDate)
                            {
                                EndResult.Add(ins);
                            }
                        }

                    }
                    else
                    {
                        EndResult.Add(ins);
                    }
                }
            }
            if (ProjectId != null)
            {
                var data = await _context.Installations.Include(x => x.Project).Include(x => x.SubProject).Include(x => x.ItemType).Where(x => (x.ProjectId.Equals(ProjectId) && x.UniqueID.Contains("#") && !x.ItemTypeId.Equals(1782) && !x.ItemTypeId.Equals(1824) && !x.ItemTypeId.Equals(1723) && !x.ItemTypeId.Equals(1786))).ToListAsync();
                var arrivals = await _context.Arrivals.Where(x => x.ProjectId.Equals(ProjectId) && x.UniqueID.Contains("#")).ToListAsync();
                foreach (var ins in data)
                {
                    if (ins.DeinstallDate == null)
                    {
                        var associated_arrival = arrivals.Where(x => x.UniqueID.Equals(ins.UniqueID) && x.ItemTypeId.Equals(ins.ItemTypeId) && x.TimeStamp <= ins.RentalStartDate).OrderBy(x => x.TimeStamp).ToList();
                        if (associated_arrival.Count == 0)
                        {
                            EndResult.Add(ins);
                        }
                    }
                    else
                    {
                        var associatd_arrival = arrivals.Where(x => x.UniqueID.Equals(ins.UniqueID) && x.ItemTypeId.Equals(ins.ItemTypeId)
                        && x.TimeStamp.Date <= ins.RentalStartDate.Date
                        ).OrderByDescending(x => x.TimeStamp).ToList();
                        if (associatd_arrival.Count == 0)
                        {
                            EndResult.Add(ins);
                        }
                        else
                        {
                            bool tobeadded = true;
                            foreach (var arr in associatd_arrival)
                            {
                                if (arr.EndStamp == null)
                                {
                                    tobeadded = false;
                                }
                                else
                                {
                                    if (arr.EndStamp >= ins.DeinstallDate)
                                    {
                                        tobeadded = false;
                                    }
                                    else
                                    {

                                    }
                                }
                            }
                            if (tobeadded == true)
                            {
                                EndResult.Add(ins);
                            }
                        }
                    }


                }
            }
            return View(EndResult);
        }
        public double ExtractKM(Install ins)
        {
            if (ins.Location != "")
            {
                try
                {
                    return Convert.ToDouble(ins.Location.Replace("+", ".").Trim());
                }
                catch
                {
                    return 10000;
                }
            };
            return 10000;
        }
        [AllowAnonymous]
        public async Task<IActionResult> MegaShizzle()
        {
            var installs = await _context.Installations.Include(x => x.ItemType).Where(x => x.UniqueID != null && x.UniqueID.Contains("#") && x.ProjectId.Equals(418) && !x.ItemTypeId.Equals(1824) && !x.ItemTypeId.Equals(1786) && !x.ItemTypeId.Equals(1748) && x.DeinstallDate != null).OrderBy(x => x.TimeStamp).ToListAsync();
            var operationtime = await _context.ItemActivities.Where(x => x.ProjectId.Equals(418)).ToListAsync();
            var IdleItems = await _context.ItemTypes.Where(x => x.ProjectId.Equals(418) && x.Item_Type.Contains("idle")).ToListAsync();
            List<StrechList> StretchLists = new List<StrechList>();
            List<Stretch> Stretches = new List<Stretch>();
            Stretches.Add(new Stretch { start = 0.0, end = 2.4999, rentaldays = 20 });
            Stretches.Add(new Stretch { start = 2.5, end = 4.999, rentaldays = 10 });
            Stretches.Add(new Stretch { start = 5.0, end = 46.0, rentaldays = 100 });
            string idleitemname = "";
            foreach (Stretch st in Stretches)
            {
                List<GantInstall> Stretchinstalls = new List<GantInstall>();
                foreach(var ins in installs)
                {
                    double km = ExtractKM(ins);
                    if (km >= st.start && km <= st.end)
                    {
                        GantInstall GI = new GantInstall();
                        GI.Install = ins;
                        GI.KMmark = ins.Location;
                        GI.KMstart = st.start;
                        GI.KMend = st.end;
                        GI.KM = km;
                        GI.MovedFromDestination = Convert.ToDateTime(ins.DeinstallDate);
                        GI.MovedToDestination = ins.TimeStamp;
                        GI.totaldays = Convert.ToInt32((ins.DeinstallDate.Value.Date - ins.TimeStamp.Date).TotalDays) + 1;
                        var optime = operationtime.Where(x => x.UniqueID.Equals(ins.UniqueID) && x.StartTime >= ins.TimeStamp && x.StartTime <= ins.DeinstallDate.Value).OrderBy(x => x.StartTime).ToList().FirstOrDefault();
                        if(optime != null)
                        {
                            GI.StartOperation = operationtime.Where(x => x.UniqueID.Equals(ins.UniqueID) && x.StartTime >= ins.TimeStamp && x.StartTime <= ins.DeinstallDate.Value).OrderBy(x => x.StartTime).ToList().FirstOrDefault().TheDate;
                            GI.EndOperation = operationtime.Where(x => x.UniqueID.Equals(ins.UniqueID) && x.StartTime >= ins.TimeStamp && x.StartTime <= ins.DeinstallDate.Value).OrderBy(x => x.StartTime).ToList().Last().TheDate;
                        }
                        else
                        {
                            GI.StartOperation = ins.TimeStamp;
                            GI.EndOperation = ins.DeinstallDate.Value;
                        }
                        GI.operationdays = Convert.ToInt32((GI.EndOperation - GI.StartOperation).TotalDays) + 1;
                        GI.idledays = GI.totaldays - GI.operationdays;
                        //check for idle rate
                        idleitemname = String.Concat(ins.ItemType.Item_Type, " - Idle");
                        var idleitem = IdleItems.Where(x => x.Item_Type.Equals(idleitemname)).SingleOrDefault();
                        if(idleitem != null)
                        {
                            GI.RealPrice = (decimal)((GI.operationdays * Convert.ToDouble(ins.ItemType.rental_price) * Convert.ToDouble(ins.Amount)) + GI.idledays * Convert.ToDouble(idleitem.rental_price) * Convert.ToDouble(ins.Amount));
                        }
                        else
                        {
                            GI.RealPrice = (decimal)((GI.operationdays * Convert.ToDouble(ins.ItemType.rental_price) * Convert.ToDouble(ins.Amount)));
                        }
                        
                        GI.PlannedPrice = (decimal)(st.rentaldays * Convert.ToDouble(ins.ItemType.rental_price) * Convert.ToDouble(ins.Amount));
                        Stretchinstalls.Add(GI);
                    }
                     
                }
              
                StretchLists.Add(new StrechList { GantInstalls = Stretchinstalls, Strectch = st });
            }
            
            
            return View(StretchLists);
        }
        public InstallsController(DataContext context, UserManager<ApplicationUser> userManager, LocService Localizer, IWebHostEnvironment env) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
            _localizer = Localizer;
            _env = env;
        }
        public async Task<IActionResult> UpdateAlarmCallInstalls()
        {
            var alarmcalls = await _context.AlarmCalls.ToListAsync();
            foreach(var itemadded in alarmcalls)
            {
                var itemtypealarm = await _context.ItemTypes.Where(x => x.ProjectId.Equals(itemadded.ProjectId) && x.ReportTypeId.Equals(8)).FirstOrDefaultAsync();
                if (itemtypealarm != null)
                {
                    var inst_exists = await _context.Installations.SingleOrDefaultAsync(x => x.ProjectId.Equals(itemadded.ProjectId) && x.Install_Text.Equals(itemadded.LogText));
                    if(inst_exists == null) { 
                    Install inst = new Install { ProjectId = itemadded.ProjectId, SubProjectId = itemadded.SubProjectId, ItemTypeId = itemtypealarm.Id, Install_Text = itemadded.LogText, Amount = 1, TimeStamp = itemadded.TimeStamp, InvoiceDate = DateTime.Now,RentalStartDate = itemadded.TimeStamp,ToBePaid = true, DoneBy = itemadded.DoneBy, EnteredIntoDataBase = itemadded.EnteredIntoDataBase, Latitude = Convert.ToDouble(itemadded.Latitude), Longitude = Convert.ToDouble(itemadded.Longitude), UniqueID = "N/A"};
                    _context.Add(inst);
                    await _context.SaveChangesAsync();
                    }
                }
            }
            return RedirectToAction("MainMenu", "TrackItems");
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Meas()
        {
            ViewData["UniqueIDs"] = new SelectList(_context.Installations.Where(x => x.isInstalled.Equals(true) && x.ItemTypeId.Equals(1812)), "UniqueID", "UniqueID");
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Meas(PipeLineMeas model, IFormFile[] files)
        {
            if (ModelState.IsValid)
            {
                Meas m = new Meas();
                m.TheMeasurement = model.Measurement;
                m.DoneBy = "Pipeline user";
                m.Latitude = model.Latitude;
                m.Longitude = model.Longitude;
                m.When = model.TimeStamp;
                string thename = model.type + " #" + model.uniqueidtext.Replace("#", "");
                var mp = await _context.MeasPoints.SingleOrDefaultAsync(x => x.Name.ToLower().Equals(thename.ToLower()));
                if (mp != null)
                {
                    m.MeasPointId = mp.Id;
                }
                else
                {
                    MeasPoint mp_new = new MeasPoint();
                    mp_new.Name = model.type + " #" + model.uniqueidtext.Replace("#", "");
                    mp_new.Lati = model.Latitude;
                    mp_new.Longi = model.Longitude;
                    mp_new.Coordx = model.Latitude;
                    mp_new.Coordy = model.Longitude;
                    mp_new.MeasTypeId = 2;
                    mp_new.MonitorTypeId = 14;
                    mp_new.ProjectId = 418;
                    mp_new.SubProjectId = null;
                    _context.Add(mp_new);
                    await _context.SaveChangesAsync();
                    var lastmp = await _context.MeasPoints.LastAsync();
                    m.MeasPointId = lastmp.Id;
                }

                _context.Add(m);
                await _context.SaveChangesAsync();
                var lastadded = await _context.Measures.LastAsync();
                var directory = _env.WebRootPath + "\\AHAK\\MeasPhotos\\" + lastadded.Id.ToString() + "\\";
                if (!Directory.Exists(directory) && files != null)
                {
                    Directory.CreateDirectory(directory);
                }
                foreach (IFormFile photo in files)
                {
                    var path = Path.Combine(directory, photo.FileName);
                    var path2 = Path.Combine(directory, photo.FileName.Split(".")[0] + "_edit." + photo.FileName.Split(".")[1]);
                    PhotoFileMeas measphoto = new PhotoFileMeas { Path = path, TimeStamp = model.TimeStamp, MeasId = lastadded.Id, Latitude = Convert.ToDouble(model.Latitude), Longitude = Convert.ToDouble(model.Longitude) };
                    _context.Add(measphoto);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    };
                    if (path.ToLower().Contains(".jpg") || path.ToLower().Contains(".jpeg"))
                    {
                        PhotoExtensions.SaveAndCompressJpeg(path, 80);
                    }

                }
                await _context.SaveChangesAsync();
            }
            else
            {
                ViewData["UniqueIDs"] = new SelectList(_context.Installations.Where(x => x.isInstalled.Equals(true) && x.ItemTypeId.Equals(1812)), "UniqueID", "UniqueID");
                return View(model);
            }
            ViewData["UniqueIDs"] = new SelectList(_context.Installations.Where(x => x.isInstalled.Equals(true) && x.ItemTypeId.Equals(1812)), "UniqueID", "UniqueID");
            PipeLineMeas new_model = new PipeLineMeas();
            return View(new_model);
        }
        // GET: Installs
        public async Task<IActionResult> Index()
        {
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["DateOfTrouble"] = DateTime.Now.Date;
            ViewData["ItemTypeId"] = new SelectList(new List<ItemType>());
            return View(new List<Install>());
        }
        [HttpPost]
        public async Task<IActionResult> Index(DateTime DateOfTrouble, int ProjectId, int ItemTypeId)
        {
            List<Install> datacontext = new List<Install>();
            var result = await (from i in _context.Installations.Include(x => x.Project).Include(x => x.SubProject).Include(x => x.ItemType)
                                where i.ProjectId.Equals(ProjectId)
                                && i.RentalStartDate <= DateOfTrouble
                                && i.ItemTypeId.Equals(ItemTypeId)
                                select i).ToListAsync();
            foreach (Install ins in result)
            {
                var otherinstall = await _context.Installations.Where(x => x.UniqueID.Equals(ins.UniqueID) && !x.Id.Equals(ins.Id)).ToListAsync();
                if (ins.DeinstallDate == null)
                {
                    foreach (var item in otherinstall)
                    {
                        if (item.DeinstallDate == null)
                        {
                            datacontext.Add(ins);
                        }
                    }
                }
                else if (ins.DeinstallDate != null)
                {

                    foreach (var item in otherinstall)
                    {
                        if (item.DeinstallDate != null)
                        {
                            if (item.TimeStamp <= ins.TimeStamp && item.DeinstallDate >= ins.RentalStartDate) // item installed before # but deinstalled after #
                            {
                                datacontext.Add(ins);
                                break;
                            }
                            else if (item.TimeStamp >= ins.TimeStamp && ins.DeinstallDate <= item.DeinstallDate && ins.DeinstallDate > item.TimeStamp) // item installed after # but deinstalled after # but # deinstalled inbetween
                            {
                                datacontext.Add(ins);
                            }
                            else if (item.TimeStamp > ins.TimeStamp && item.TimeStamp < ins.DeinstallDate && item.DeinstallDate >= ins.DeinstallDate)
                            {
                                datacontext.Add(ins);
                                break;
                            }
                        }
                        else
                        {

                        }
                    }
                }
                else
                {
                    foreach (var item in otherinstall)
                    {
                        if (item.DeinstallDate != null)
                        {
                            if (item.DeinstallDate > ins.TimeStamp || item.TimeStamp > ins.TimeStamp)
                            {
                                datacontext.Add(ins);
                                break;
                            }
                        }
                        else
                        {
                            datacontext.Add(ins);
                            break;
                        }

                    }
                }
                if (Convert.ToDateTime(ins.DeinstallDate).Date >= DateOfTrouble.Date)
                {
                    var existingarrival = await _context.Arrivals.FirstOrDefaultAsync(x => x.UniqueID.Equals(ins.UniqueID) && x.EndStamp >= ins.DeinstallDate && x.TimeStamp <= ins.RentalStartDate && x.ItemTypeId.Equals(ins.ItemTypeId));
                    if (existingarrival == null)
                    {
                        datacontext.Add(ins);
                    }
                }
                else
                {
                    //var otherinstall = await _context.Installations.Where(x => x.UniqueID.Equals(ins.UniqueID) && !x.Id.Equals(ins.Id) && ((x.TimeStamp < ins.DeinstallDate && x.DeinstallDate == null) || (x.DeinstallDate >= ins.TimeStamp && x.DeinstallDate <= ins.DeinstallDate))).ToListAsync();
                    var existingarrival = await _context.Arrivals.SingleOrDefaultAsync(x => x.UniqueID.Equals(ins.UniqueID) && x.EndStamp == null && x.TimeStamp <= ins.RentalStartDate && x.ItemTypeId.Equals(ins.ItemTypeId));
                    if (existingarrival == null)
                    {
                        datacontext.Add(ins);
                    }
                }

            }
            ViewData["DateOfTrouble"] = DateOfTrouble.Date;
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["ItemTypeId"] = new SelectList(_context.ItemTypes.Where(x => x.ProjectId.Equals(ProjectId) && x.Install_UnitId.Equals(1)), "Id", "Item_Type", ItemTypeId);
            return View(datacontext);
        }
        [HttpPost]
        [Authorize(Roles = ("Admin,DivisionAdmin,Manager"))]
        public async Task<IActionResult> UpdateInstallLocation(int? id)
        {
            if (id != null)
            {
                var install = await _context.Installations.Where(x => x.Id.Equals(id)).SingleOrDefaultAsync();
                if (install != null)
                {
                    try
                    {
                        install.Location = FindNearestKM(Convert.ToInt32(install.ProjectId), install.Latitude, install.Longitude);
                    }
                    catch
                    {

                    }
                    var coordtrack = await _context.CoordTrack2s.Where(x => x.InstallId.Equals(id) && (x.TypeCoord.Equals("Installed") || x.TypeCoord.Equals("Install") || x.TypeCoord.Equals("DeInstalled"))).ToListAsync();
                    foreach (var coord in coordtrack)
                    {
                        //coord.Latitude_backup = coord.Latitude;
                        //coord.Longitude_backup = coord.Longitude;
                        coord.Latitude = install.Latitude;
                        coord.Longitude = install.Longitude;
                        _context.CoordTrack2s.Update(coord);
                    }
                    _context.Installations.Update(install);

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Installations", "TrackItems");
                }
                return RedirectToAction("Installations", "TrackItems");
            }
            return RedirectToAction("Installations", "TrackItems");
        }
        public string FindNearestKM(int ProjectId, double lat, double lng)
        {
            string kmmark = "0";
            var directory = _env.WebRootPath + "\\AHAK\\Reference Coords\\" + ProjectId.ToString() + "\\";
            double smallest_dist = 10000000000.0;
            int counter = 0;
            int index = 0;
            foreach (var file in Directory.GetFiles(directory))
            {
                using (var reader = new StreamReader(file))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(';');
                        LatLngUTMConverter ltUTMconv = new LatLngUTMConverter("WGS 84");
                        LatLngUTMConverter.LatLng latlng;
                        latlng = ltUTMconv.convertUtmToLatLng(Convert.ToDouble(values[1]), Convert.ToDouble(values[2]), 32, "N");
                        double distance = DistanceAlgorithm.DistanceBetweenPlaces(latlng.Lat, latlng.Lng, lat, lng);
                        if (distance < smallest_dist)
                        {
                            smallest_dist = distance;
                            index = counter;
                            kmmark = Convert.ToInt32(values[0]).ToString();
                        }
                        counter += 1;
                    }
                }
            }
            if (kmmark.Length < 4)
            {
                kmmark = string.Format("{0}+{1}", 0, kmmark);
            }
            else if (kmmark.Length < 5)
            {
                try
                {
                    kmmark = string.Format("{0}+{1}", kmmark.Substring(0, 1), kmmark.Substring(1, 3));
                }
                catch
                {
                    kmmark = "";
                }
            }
            else
            {
                kmmark = string.Format("{0}+{1}", kmmark.Substring(0, 2), kmmark.Substring(2, 3));
            }
            return kmmark;
        }
        [HttpGet]
        [Authorize(Roles = "Guest,Admin,DivisionAdmin,Manager")]
        public async Task<IActionResult> InstallInOperationPDF(DateTime? TheDate)
        {
            DateTime TimeofDate;
            if (TheDate != null)
            {
                TimeofDate = Convert.ToDateTime(TheDate).Date;
            }
            else
            {
                TimeofDate = DateTime.Now.Date;
            }
            List<InstallInOperation> reallist = new List<InstallInOperation>();
            var dataContext = await _context.Installations
                .Include(x => x.ItemType).Include(i => i.ItemType).Include(x => x.Project).Include(i => i.SubProject)
                .Where(x => x.UniqueID.Contains("#") && x.ProjectId.Equals(418) && x.ItemTypeId.Equals(1725)
                ).OrderBy(x => x.TimeStamp).ToListAsync();
            foreach (Install ins in dataContext)
            {
                if ((ins.isInstalled == true && ins.TimeStamp <= TimeofDate) || (ins.isInstalled == false && ins.TimeStamp <= TimeofDate && ins.DeinstallDate >= TimeofDate))
                {
                    var activity = await _context.ItemActivities.Where(x => x.UniqueID.Replace("#", "").Equals(ins.UniqueID.Replace("#", "")) && x.WasActive.Equals(true) && x.TheDate.Date.Equals(TimeofDate)).FirstOrDefaultAsync();
                    if (activity != null)
                    {
                        InstallInOperation IO = new InstallInOperation(ins, TimeofDate, true);
                        reallist.Add(IO);
                    }
                    else
                    {
                        InstallInOperation IO = new InstallInOperation(ins, TimeofDate, false);
                        reallist.Add(IO);
                    }
                }
            }
            return new ViewAsPdf("_InstallInOperation", reallist);
        }
        [HttpGet]
        [Authorize(Roles = "Guest,Admin,DivisionAdmin,Manager")]
        public async Task<IActionResult> InstallInOperation(DateTime? TheDate)
        {
            DateTime TimeofDate;
            if (TheDate != null)
            {
                TimeofDate = Convert.ToDateTime(TheDate).Date;
            }
            else
            {
                TimeofDate = DateTime.Now.Date;
            }
            List<InstallInOperation> reallist = new List<InstallInOperation>();
            var dataContext = await _context.Installations
                .Include(x => x.ItemType).Include(i => i.ItemType).Include(x => x.Project).Include(i => i.SubProject)
                .Where(x => x.UniqueID.Contains("#") && x.ProjectId.Equals(418) && x.ItemTypeId.Equals(1725)
                ).OrderBy(x => x.TimeStamp).ToListAsync();
            foreach (Install ins in dataContext)
            {
                if ((ins.isInstalled == true && ins.TimeStamp <= TimeofDate) || (ins.isInstalled == false && ins.TimeStamp <= TimeofDate && ins.DeinstallDate >= TimeofDate))
                {
                    var activity = await _context.ItemActivities.Where(x => x.UniqueID.Replace("#", "").Equals(ins.UniqueID.Replace("#", "")) && x.WasActive.Equals(true) && x.TheDate.Date.Equals(TimeofDate)).FirstOrDefaultAsync();
                    if (activity != null)
                    {
                        InstallInOperation IO = new InstallInOperation(ins, TimeofDate, true);
                        reallist.Add(IO);
                    }
                    else
                    {
                        InstallInOperation IO = new InstallInOperation(ins, TimeofDate, false);
                        reallist.Add(IO);
                    }
                }
            }

            return View(reallist);
        }
        [HttpGet]
        public async Task<IActionResult> InstallOverView()
        {
            StringBuilder sb = new StringBuilder();
            var dataContext = await _context.Installations
                .Include(x => x.ItemType).Include(i => i.ItemType).Include(x => x.Project).Include(i => i.SubProject)
                .Where(x => x.UniqueID.Contains("#") && x.ProjectId.Equals(418)).OrderBy(x => x.TimeStamp).ToListAsync();
            var startdate = dataContext.First().TimeStamp.Date;
            var enddate = dataContext.Select(x => x.RentalStartDate).Max().Date;
            List<DateTime> days = new List<DateTime>();
            for (DateTime dt = startdate; dt <= enddate; dt = dt.AddDays(1))
            {
                days.Add(dt);
            }
            var daycount = (enddate - startdate).TotalDays;
            int colcount = 0;
            colcount = Convert.ToInt32(daycount) + 2;
            List<string> dayrow = new List<string>(new string[colcount]);

            dayrow[0] = "Unique ID";
            for (int i = 1; i < colcount; i++)
            {
                dayrow[i] = startdate.AddDays(i - 1).ToString();
            }
            sb.AppendLine(string.Join(";", dayrow.ToArray()));
            foreach (var installunique in dataContext.GroupBy(x => x.UniqueID))
            {
                var installs = dataContext.Where(x => x.UniqueID.Equals(installunique.Key)).OrderBy(x => x.TimeStamp).ToList();
                List<string> fillerrow = new List<string>(new string[colcount + 1]);
                List<string> fillerrow_text = new List<string>(new string[colcount + 1]);
                fillerrow[0] = installs.First().UniqueID + " Project";
                fillerrow_text[0] = installs.First().UniqueID + " Type";
                foreach (var install in installs)
                {
                    if (install.SubProjectId != null)
                    {
                        fillerrow[Convert.ToInt32((install.TimeStamp.Date - startdate).TotalDays) + 1] += install.SubProject.Name + " ";
                        if (install.Location != "N/A" && install.Location != "" && install.Location != null)
                        {
                            fillerrow[Convert.ToInt32((install.TimeStamp.Date - startdate).TotalDays) + 1] += " at: " + install.Location + " ";
                        }
                    }
                    else
                    {
                        fillerrow[Convert.ToInt32((install.TimeStamp.Date - startdate).TotalDays) + 1] += " ";
                        if (install.Location != "N/A" && install.Location != "" && install.Location != null)
                        {
                            fillerrow[Convert.ToInt32((install.TimeStamp.Date - startdate).TotalDays) + 1] += " at: " + install.Location + " ";
                        }
                    }
                    if (install.TimeStamp.Date == install.RentalStartDate.Date)
                    {
                        fillerrow_text[Convert.ToInt32((install.TimeStamp.Date - startdate).TotalDays) + 1] += install.ItemType.Item_Type + " Install + Rental Start ";
                    }
                    else
                    {

                        fillerrow_text[Convert.ToInt32((install.TimeStamp.Date - startdate).TotalDays) + 1] += install.ItemType.Item_Type + " Install ";
                        fillerrow_text[Convert.ToInt32((install.RentalStartDate.Date - startdate).TotalDays) + 1] += install.ItemType.Item_Type + " Rental Start ";

                    }
                    if (install.DeinstallDate != null)
                    {
                        fillerrow_text[Convert.ToInt32((Convert.ToDateTime(install.DeinstallDate).Date - startdate).TotalDays) + 1] += " Deinstalled ";
                    }
                }
                sb.AppendLine(string.Join(";", fillerrow.ToArray()));
                sb.AppendLine(string.Join(";", fillerrow_text.ToArray()));
            }

            return File(System.Text.Encoding.ASCII.GetBytes(sb.ToString()), "text/csv", "InstallsReport.csv");
        }
        [HttpGet]
        public async Task<IActionResult> OperationOverView()
        {
            StringBuilder sb = new StringBuilder();
            var dataContext = await _context.Installations
                .Include(x => x.ItemType)
                .Where(x => x.ProjectId.Equals(418) && x.UniqueID.Contains("#") && !x.UniqueID.Contains("-")).OrderBy(x => x.TimeStamp).ToListAsync();
            var startdate = dataContext.First().TimeStamp.Date;
            var enddate = DateTime.Now.Date;
            var itemactivities = await _context.ItemActivities.Where(x => x.ProjectId.Equals(418)).ToListAsync();
            List<DateTime> days = new List<DateTime>();
            for (DateTime dt = startdate; dt <= enddate; dt = dt.AddDays(1))
            {
                days.Add(dt);
            }
            var daycount = days.Count;//(enddate - startdate).TotalDays + 1;
            int colcount = 0;
            colcount = Convert.ToInt32(daycount) + 1;
            List<string> dayrow = new List<string>(new string[colcount]);

            dayrow[0] = "Unique ID";
            for (int i = 1; i < colcount; i++)
            {
                dayrow[i] = days[i - 1].ToString();
            }
            sb.AppendLine(string.Join(";", dayrow.ToArray()));
            foreach (var installunique in dataContext.GroupBy(x => x.UniqueID))
            {
                var installs = dataContext.Where(x => x.UniqueID.Equals(installunique.Key)).OrderBy(x => x.TimeStamp).ToList();
                var activities = itemactivities.Where(x => x.UniqueID.Equals(installunique.Key)).OrderBy(x => x.StartTime).ToList();
                List<string> fillerrow = new List<string>(new string[colcount]);
                List<string> fillerrow_text = new List<string>(new string[colcount]);
                fillerrow[0] = installs.First().UniqueID + " Type";
                fillerrow_text[0] = installs.First().UniqueID;
                foreach (var install in installs)
                {
                    fillerrow[Convert.ToInt32((install.TimeStamp.Date - startdate).TotalDays) + 1] += install.ItemType.Item_Type + " at: " + install.Location;
                }
                foreach (var activity in activities.GroupBy(x => x.TheDate))
                {

                    fillerrow_text[Convert.ToInt32((activity.Key.Date - startdate).TotalDays) + 1] += "X";

                }
                sb.AppendLine(string.Join(";", fillerrow.ToArray()));
                sb.AppendLine(string.Join(";", fillerrow_text.ToArray()));
            }

            return File(System.Text.Encoding.ASCII.GetBytes(sb.ToString()), "text/csv", "OperationsReport.csv");
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var install = await _context.Installations
                .Include(i => i.ItemType)
                .Include(i => i.Project)
                .Include(i => i.SubProject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (install == null)
            {
                return NotFound();
            }

            return View(install);
        }

        // GET: Installs/Create
        public IActionResult Create()
        {
            ViewData["ItemTypeId"] = new SelectList(_context.ItemTypes, "Id", "Id");
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Abbreviation");
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Id");
            return View();
        }

        // POST: Installs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,SubProjectId,ItemTypeId,Install_Text,isInstalled,DoneBy,DeinstallDate,Amount,TimeStamp,Latitude,Longitude,Location,UniqueID,EnteredIntoDataBase,LastEditedInDataBase,RentalStartDate,IsInOperation")] Install install)
        {
            if (ModelState.IsValid)
            {
                _context.Add(install);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemTypeId"] = new SelectList(_context.ItemTypes, "Id", "Id", install.ItemTypeId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Abbreviation", install.ProjectId);
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Id", install.SubProjectId);
            return View(install);
        }

        // GET: Installs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var install = await _context.Installations.FindAsync(id);
            if (install == null)
            {
                return NotFound();
            }
            ViewData["ItemTypeId"] = new SelectList(_context.ItemTypes, "Id", "Id", install.ItemTypeId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Abbreviation", install.ProjectId);
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Id", install.SubProjectId);
            return View(install);
        }

        // POST: Installs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,SubProjectId,ItemTypeId,Install_Text,isInstalled,DoneBy,DeinstallDate,Amount,TimeStamp,Latitude,Longitude,Location,UniqueID,EnteredIntoDataBase,LastEditedInDataBase,RentalStartDate,IsInOperation")] Install install)
        {
            if (id != install.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(install);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstallExists(install.Id))
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
            ViewData["ItemTypeId"] = new SelectList(_context.ItemTypes, "Id", "Id", install.ItemTypeId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Abbreviation", install.ProjectId);
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Id", install.SubProjectId);
            return View(install);
        }

        // GET: Installs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var install = await _context.Installations
                .Include(i => i.ItemType)
                .Include(i => i.Project)
                .Include(i => i.SubProject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (install == null)
            {
                return NotFound();
            }

            return View(install);
        }

        // POST: Installs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var install = await _context.Installations.FindAsync(id);
            _context.Installations.Remove(install);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
        public async Task<IActionResult> AddArrival(int? id)
        {
            if (id != null)
            {
                var install = await _context.Installations.SingleOrDefaultAsync(x => x.Id.Equals(id));
                Arrival arr = new Arrival();
                arr.InvoiceDate = install.InvoiceDate;
                arr.VariationOrderId = install.VariationOrderId;
                arr.UniqueID = install.UniqueID;
                arr.TimeStamp = install.RentalStartDate;
                arr.ProjectId = install.ProjectId;
                arr.SubProjectId = install.SubProjectId;
                arr.ToBePaid = install.ToBePaid;
                arr.MobilisationId = null;
                arr.Latitude = install.Latitude;
                arr.Longitude = install.Longitude;
                arr.ItemTypeId = install.ItemTypeId;
                arr.Arrival_Text = "Auto Added from Install id: " + id.ToString();
                arr.Amount = install.Amount;
                arr.EndStamp = install.DeinstallDate;
                arr.EnteredIntoDataBase = DateTime.Now;

                _context.Add(arr);
                await _context.SaveChangesAsync();
                return RedirectToAction("Installations", "TrackItems");
            }
            else
            {
                return NotFound();
            }
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
        [HttpGet]
        public IActionResult UploadTrencherCoords()
        {
            return View();
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
        [HttpPost]
        public async Task<IActionResult> UploadTrencherCoords(IFormFile postedFile)
        {
            if (postedFile != null)
            {
                string fileExtension = Path.GetExtension(postedFile.FileName);

                if (fileExtension != ".csv")
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "please provide a CSV file!" });
                }
                using (var sreader = new StreamReader(postedFile.OpenReadStream()))
                {
                    LatLngUTMConverter ltUTMconv = new LatLngUTMConverter("WGS 84");
                    LatLngUTMConverter.LatLng latlng;
                    while (!sreader.EndOfStream)
                    {

                        string[] rows = sreader.ReadLine().Split(',');
                        var prev_inst = await _context.Installations.SingleOrDefaultAsync(x => x.ItemTypeId.Equals(1824) && x.UniqueID.Equals(rows[0]));
                        if (prev_inst == null)
                        {
                            CultureInfo provider = CultureInfo.InvariantCulture;
                            // It throws Argument null exception
                            DateTime dateTime10 = DateTime.ParseExact(rows[0].Split(".")[0], "yyyyMMdd-HHmmss", provider);
                            Install inst = new Install();
                            inst.ToBePaid = false;
                            inst.ProjectId = 418;
                            inst.ItemTypeId = 1824;
                            inst.SubProjectId = 232;
                            inst.UniqueID = rows[0];
                            inst.TimeStamp = dateTime10;
                            inst.InvoiceDate = DateTime.Now;
                            latlng = ltUTMconv.convertUtmToLatLng(Convert.ToDouble(rows[2]), Convert.ToDouble(rows[1]), 32, "N");
                            inst.Latitude = latlng.Lat;
                            inst.Longitude = latlng.Lng;
                            _context.Add(inst);
                            await _context.SaveChangesAsync();
                            var lastadded = await _context.Installations.LastAsync();
                            CoordTrack2 coord = new CoordTrack2();
                            coord.InstallId = lastadded.Id;
                            coord.Latitude = lastadded.Latitude;
                            coord.Longitude = lastadded.Longitude;
                            coord.Accuracy = 0;
                            coord.Latitude_backup = 0;
                            coord.Longitude_backup = 0;
                            coord.TypeCoord = "Installed";
                            coord.TimeStamp = dateTime10;
                            _context.Add(coord);

                        }
                    }
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }
        private bool InstallExists(int id)
        {
            return _context.Installations.Any(e => e.Id == id);
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
        public async Task<IActionResult> RemoveSandFromHorizontalDrains()
        {
            var user = await _userManager.GetUserAsync(User);
            List<CoordTrack2> coordstoremove = new List<CoordTrack2>();
            if (user.DivisionId.Equals(1))
            {
                var installs = await _context.Installations.Where(x => x.ItemTypeId.Equals(1786)).ToListAsync();
                foreach (var sand in installs)
                {
                    var coords = await _context.CoordTrack2s.Where(x => x.InstallId.Equals(sand.Id)).ToListAsync();
                    foreach (var coord in coords)
                    {
                        _context.Remove(coord);
                    }
                    await _context.SaveChangesAsync();
                    _context.Remove(sand);
                }

                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Installations", "TrackItems");
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
        public async Task<IActionResult> UpdateRentalStartDateDrainsNew()
        {
            var pumps = await _context.Installations.Where(x => (x.ItemTypeId.Equals(1800) || x.ItemTypeId.Equals(1725) || x.ItemTypeId.Equals(1812) || x.ItemTypeId.Equals(1787) || x.ItemTypeId.Equals(1704) || x.ItemTypeId.Equals(1705) || x.ItemTypeId.Equals(1702))).OrderBy(x => x.UniqueID).ThenBy(x => x.TimeStamp).ToListAsync();
            Install pump;
            Install nextpump;
            for (int i = 0; i < pumps.Count; i++)
            {
                if (pumps[i].ItemTypeId.Equals(1800))
                {
                    pump = pumps[i];
                    if ((i + 1) < pumps.Count)
                    {
                        if (pumps[i + 1].UniqueID.Equals(pump.UniqueID))
                        {
                            nextpump = pumps[i + 1];
                            DateTime StopSearchDate = nextpump.TimeStamp;
                            if (pump.DeinstallDate != null)
                            {
                                if (pump.DeinstallDate < nextpump.TimeStamp)
                                {
                                    StopSearchDate = Convert.ToDateTime(pump.DeinstallDate).AddDays(1);
                                }
                            }
                            var operation = await _context.ItemActivities.Where(X => X.UniqueID.Equals(pump.UniqueID) && X.TheDate >= pump.RentalStartDate && X.TheDate < StopSearchDate).OrderBy(x => x.TheDate).ToListAsync();
                            var matching_drains = await _context.Installations.Where(x => x.UniqueID.Contains(pump.UniqueID) && x.ItemTypeId.Equals(1703)).OrderBy(x => x.TimeStamp).ToListAsync();
                            double dist = 10000000000;
                            Install thedrain = matching_drains.First();
                            foreach (var drain in matching_drains)
                            {
                                double new_dist = DistanceAlgorithm.DistanceBetweenPlaces(pump.Longitude, pump.Latitude, drain.Longitude, drain.Latitude);
                                if (new_dist < dist)
                                {
                                    thedrain = drain;
                                    dist = new_dist;
                                }
                            }
                            Arrival theArrivalDrain = await _context.Arrivals.Where(x => x.UniqueID.Equals(thedrain.UniqueID)).SingleOrDefaultAsync();
                            try
                            {
                                theArrivalDrain.TimeStamp = operation.First().TheDate;
                                thedrain.RentalStartDate = operation.First().TheDate;
                                theArrivalDrain.EndStamp = operation.Last().TheDate;
                                thedrain.DeinstallDate = operation.Last().TheDate;
                                _context.Update(thedrain);
                                _context.Update(theArrivalDrain);
                            }
                            catch
                            {

                            }
                        }
                        else
                        {
                            if (pump.DeinstallDate != null)
                            {
                                DateTime deinstalldate = Convert.ToDateTime(pump.DeinstallDate);
                                var operation = await _context.ItemActivities.Where(X => X.UniqueID.Equals(pump.UniqueID) && X.TheDate >= pump.RentalStartDate && X.TheDate <= deinstalldate).OrderBy(x => x.TheDate).ToListAsync();
                                var matching_drains = await _context.Installations.Where(x => x.UniqueID.Contains(pump.UniqueID) && x.ItemTypeId.Equals(1703)).OrderBy(x => x.TimeStamp).ToListAsync();
                                double dist = 10000000000;
                                try
                                {
                                    Install thedrain = matching_drains.First();
                                    foreach (var drain in matching_drains)
                                    {
                                        double new_dist = DistanceAlgorithm.DistanceBetweenPlaces(pump.Longitude, pump.Latitude, drain.Longitude, drain.Latitude);
                                        if (new_dist < dist)
                                        {
                                            thedrain = drain;
                                            dist = new_dist;
                                        }
                                    }
                                    Arrival theArrivalDrain = await _context.Arrivals.Where(x => x.UniqueID.Equals(thedrain.UniqueID)).SingleOrDefaultAsync();

                                    try
                                    {
                                        theArrivalDrain.TimeStamp = operation.First().TheDate;
                                        thedrain.RentalStartDate = operation.First().TheDate;
                                        theArrivalDrain.EndStamp = operation.Last().TheDate;
                                        thedrain.DeinstallDate = operation.Last().TheDate;
                                        _context.Update(thedrain);
                                        _context.Update(theArrivalDrain);
                                    }
                                    catch
                                    {

                                    }
                                }
                                catch
                                {

                                }
                            }
                            else
                            {
                                //DateTime deinstalldate = Convert.ToDateTime(pump.DeinstallDate);
                                var operation = await _context.ItemActivities.Where(X => X.UniqueID.Equals(pump.UniqueID) && X.TheDate >= pump.RentalStartDate).OrderBy(x => x.TheDate).ToListAsync();
                                var matching_drains = await _context.Installations.Where(x => x.UniqueID.Contains(pump.UniqueID) && x.ItemTypeId.Equals(1703)).OrderBy(x => x.TimeStamp).ToListAsync();
                                double dist = 10000000000;
                                Install thedrain = matching_drains.First();
                                foreach (var drain in matching_drains)
                                {
                                    double new_dist = DistanceAlgorithm.DistanceBetweenPlaces(pump.Longitude, pump.Latitude, drain.Longitude, drain.Latitude);
                                    if (new_dist < dist)
                                    {
                                        thedrain = drain;
                                        dist = new_dist;
                                    }
                                }
                                Arrival theArrivalDrain = await _context.Arrivals.Where(x => x.UniqueID.Equals(thedrain.UniqueID)).SingleOrDefaultAsync();
                                try
                                {
                                    theArrivalDrain.TimeStamp = operation.First().TheDate;
                                    thedrain.RentalStartDate = operation.First().TheDate;
                                    theArrivalDrain.EndStamp = operation.Last().TheDate;
                                    thedrain.DeinstallDate = operation.Last().TheDate;
                                    _context.Update(thedrain);
                                    _context.Update(theArrivalDrain);
                                }
                                catch
                                {

                                }
                            }
                            //handle last position of pump
                        }
                    }
                    else
                    {
                        if (pump.DeinstallDate != null)
                        {
                            DateTime deinstalldate = Convert.ToDateTime(pump.DeinstallDate);
                            var operation = await _context.ItemActivities.Where(X => X.UniqueID.Equals(pump.UniqueID) && X.TheDate >= pump.RentalStartDate && X.TheDate <= deinstalldate).OrderBy(x => x.TheDate).ToListAsync();
                            var matching_drains = await _context.Installations.Where(x => x.UniqueID.Contains(pump.UniqueID) && x.ItemTypeId.Equals(1703)).OrderBy(x => x.TimeStamp).ToListAsync();
                            double dist = 10000000000;
                            Install thedrain = matching_drains.First();
                            foreach (var drain in matching_drains)
                            {
                                double new_dist = DistanceAlgorithm.DistanceBetweenPlaces(pump.Longitude, pump.Latitude, drain.Longitude, drain.Latitude);
                                if (new_dist < dist)
                                {
                                    thedrain = drain;
                                    dist = new_dist;
                                }
                            }
                            Arrival theArrivalDrain = await _context.Arrivals.Where(x => x.UniqueID.Equals(thedrain.UniqueID)).SingleOrDefaultAsync();
                            theArrivalDrain.TimeStamp = operation.First().TheDate;
                            thedrain.RentalStartDate = operation.First().TheDate;
                            theArrivalDrain.EndStamp = operation.Last().TheDate;
                            thedrain.DeinstallDate = operation.Last().TheDate;
                            _context.Update(thedrain);
                            _context.Update(theArrivalDrain);
                        }
                        else
                        {
                            //DateTime deinstalldate = Convert.ToDateTime(pump.DeinstallDate);
                            var operation = await _context.ItemActivities.Where(X => X.UniqueID.Equals(pump.UniqueID) && X.TheDate >= pump.RentalStartDate).OrderBy(x => x.TheDate).ToListAsync();
                            var matching_drains = await _context.Installations.Where(x => x.UniqueID.Contains(pump.UniqueID) && x.ItemTypeId.Equals(1703)).OrderBy(x => x.TimeStamp).ToListAsync();
                            double dist = 10000000000;
                            Install thedrain = matching_drains.First();
                            foreach (var drain in matching_drains)
                            {
                                double new_dist = DistanceAlgorithm.DistanceBetweenPlaces(pump.Longitude, pump.Latitude, drain.Longitude, drain.Latitude);
                                if (new_dist < dist)
                                {
                                    thedrain = drain;
                                    dist = new_dist;
                                }
                            }
                            Arrival theArrivalDrain = await _context.Arrivals.Where(x => x.UniqueID.Equals(thedrain.UniqueID)).SingleOrDefaultAsync();
                            theArrivalDrain.TimeStamp = operation.First().TheDate;
                            thedrain.RentalStartDate = operation.First().TheDate;
                            theArrivalDrain.EndStamp = operation.Last().TheDate;
                            thedrain.DeinstallDate = operation.Last().TheDate;
                            _context.Update(thedrain);
                            _context.Update(theArrivalDrain);
                        }
                    }
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
        public async Task<IActionResult> UpdateRentalStartDateDrains()
        {
            var drains = await _context.Installations.Where(x => x.ItemTypeId.Equals(1703) && x.UniqueID.Contains("#")).ToListAsync();
            foreach (var drain in drains)
            {
                string uniqueidpump = drain.UniqueID.Split("-")[1].Trim();
                var pump = await _context.Installations.OrderBy(x => x.TimeStamp).FirstOrDefaultAsync(x => x.ItemTypeId.Equals(1800) && x.UniqueID.Trim().Equals(uniqueidpump) && x.TimeStamp >= drain.TimeStamp);
                var drainarrival = await _context.Arrivals.SingleOrDefaultAsync(x => x.UniqueID.Equals(drain.UniqueID));
                drain.RentalStartDate = pump.TimeStamp;
                drainarrival.TimeStamp = pump.TimeStamp;
                _context.Update(drain);
                _context.Update(drainarrival);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
        public async Task<IActionResult> AddSandToHorizontalDrains()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.DivisionId.Equals(1))
            {
                var installs = await _context.Installations.Where(x => x.ItemTypeId.Equals(1703)).ToListAsync();
                foreach (var install in installs)
                {
                    Install sand = new Install();
                    sand.ItemTypeId = 1786;
                    sand.Amount = install.Amount;
                    sand.UniqueID = install.UniqueID;
                    sand.Location = install.Location;
                    sand.EnteredIntoDataBase = DateTime.Now;
                    sand.Latitude = install.Latitude;
                    sand.Longitude = install.Longitude;
                    sand.ProjectId = install.ProjectId;
                    sand.SubProjectId = install.SubProjectId;
                    sand.ToBePaid = true;
                    sand.isInstalled = install.isInstalled;
                    sand.DeinstallDate = install.DeinstallDate;
                    sand.TimeStamp = install.TimeStamp;
                    sand.InvoiceDate = install.TimeStamp;
                    sand.RentalStartDate = install.RentalStartDate;
                    sand.Install_Text = "Auto added Sand for horizontal drain";
                    sand.IsInOperation = install.IsInOperation;
                    _context.Add(sand);
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Installations", "TrackItems");
        }
        public async Task<IActionResult> UpdateUniqueIdRWs()
        {
            var installs = await _context.Installations.Where(x => x.ProjectId.Equals(437) && x.UniqueID.ToLower().Contains("rw")).ToListAsync();
            foreach (var ins in installs)
            {
                if (ins.UniqueID.ToLower().Contains("crw"))
                {
                    ins.UniqueID = ins.UniqueID.Replace("Crw", "CRW").Replace("crw", "CRW");
                    ins.UniqueID = ins.UniqueID.Replace("NHT -", "");
                    ins.UniqueID = ins.UniqueID.Replace("NHT-", "");
                    ins.UniqueID = ins.UniqueID.Replace("NHT", "");
                    if (ins.UniqueID.Contains("-"))
                    {

                        string[] parts = ins.UniqueID.Split("-");
                        parts[0] = parts[0].Replace("CRW", "NHT-CRW");
                        parts[0] = parts[0].Replace("NHT-CRW ", "NHT-CRW");
                        parts[1] = parts[1].Replace("CRW", "NHT-CRW");
                        parts[1] = parts[1].Replace("NHT-CRW ", "NHT-CRW");
                        ins.UniqueID = String.Concat(parts[0].Trim(), " - ", parts[1].Trim());
                    }
                    else
                    {
                        if (!ins.UniqueID.Contains("NHT-CRW"))
                        {
                            ins.UniqueID = ins.UniqueID.Replace("CRW", "NHT-CRW");
                            ins.UniqueID = ins.UniqueID.Replace("NHT-CRW ", "NHT-CRW");
                        }

                    }
                }
                else
                {
                    ins.UniqueID = ins.UniqueID.Replace("Rw", "RW").Replace("rw", "RW");
                    ins.UniqueID = ins.UniqueID.Replace("NHT -", "");
                    ins.UniqueID = ins.UniqueID.Replace("NHT-", "");
                    ins.UniqueID = ins.UniqueID.Replace("NHT", "");
                    if (ins.UniqueID.Contains("-"))
                    {

                        string[] parts = ins.UniqueID.Split("-");
                        parts[0] = parts[0].Replace("RW", "NHT-RW");
                        parts[0] = parts[0].Replace("NHT-RW ", "NHT-RW");
                        parts[1] = parts[1].Replace("RW", "NHT-RW");
                        parts[1] = parts[1].Replace("NHT-RW ", "NHT-RW");
                        ins.UniqueID = String.Concat(parts[0].Trim(), " - ", parts[1].Trim());
                    }
                    else
                    {
                        if (!ins.UniqueID.Contains("NHT-RW"))
                        {
                            ins.UniqueID = ins.UniqueID.Replace("RW", "NHT-RW");
                            ins.UniqueID = ins.UniqueID.Replace("NHT-RW ", "NHT-RW");
                        }

                    }
                }
                
                
                _context.Update(ins);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("MainMenu", "TrackItems");
        }
        [HttpGet]
        public async Task<IActionResult> BilledActivities(int? id)
        {
            if(id != null)
            {
                var install = await _context.Installations.Include(x => x.Project).SingleOrDefaultAsync(x => x.Id.Equals(id));
                var user = await _userManager.GetUserAsync(User);
                if(install.Project.DivisionId != user.DivisionId)
                {
                    return NotFound();
                }
                string basename = install.UniqueID;
                MeasPoint mp = null;
                if (basename.Contains(":"))
                {
                    string[] basenameparts = basename.Split(":");
                    foreach(string basenamepart in basenameparts)
                    {
                        mp = await _context.MeasPoints.Where(x => x.Name.ToLower().Replace(" ", "").Equals(basenamepart.Trim().Replace(" ","").ToLower())).SingleOrDefaultAsync();
                        if(mp != null)
                        {
                            break;
                        }
                    }
                    
                }
                if(mp == null)
                {
                    string[] basenameparts = basename.Split(" ");
                    foreach (string basenamepart in basenameparts)
                    {
                        mp = await _context.MeasPoints.Where(x => x.Name.ToLower().Equals(basenamepart.Trim().ToLower())).SingleOrDefaultAsync();
                        if (mp != null)
                        {
                            break;
                        }
                    }
                }
                if(mp == null)
                {
                    mp = await _context.MeasPoints.SingleOrDefaultAsync(x => x.Name.ToLower().Equals(basename.ToLower()));
                }
                ItemBilledActivities IBA = new ItemBilledActivities();
                var allinstalls = await _context.Installations.Include(x => x.ItemType).Where(x => x.UniqueID.ToLower().Contains(mp.Name.ToLower())).OrderByDescending(x => x.TimeStamp).ToListAsync();
                foreach(Install inst in allinstalls)
                {


                    if (inst.ItemTypeId == 1917)
                    {
                        var pre = await _context.PreExcavations.Include(x => x.MeasPoint).FirstOrDefaultAsync(x => x.TimeStamp.Date.Equals(inst.TimeStamp.Date) && (x.MeasPointId.Equals(mp.Id) || x.wellname.ToLower().Contains(mp.Name.ToLower())));
                        IBA.PreExcavations.Add(pre);
                    }
                    else if (inst.ItemTypeId == 1922)
                    {
                        var drill = await _context.Wells.FirstOrDefaultAsync(x => x.Drill_Date_End.Date.Equals(inst.TimeStamp.Date) && x.WellName.Equals(mp.Name));
                        IBA.Drillings.Add(drill);
                    }
                    else if (inst.ItemTypeId == 1949)
                    {
                        var cp = await _context.ClearPumpTests.FirstOrDefaultAsync(x => x.Report_Date.Date.Equals(inst.TimeStamp.Date) && x.MeasPointId.Equals(mp.Id));
                        IBA.ClearPumpTests.Add(cp);
                    }
                    else if (inst.ItemTypeId == 1950)
                    {
                        var airlift = await _context.Airlifts.Include(x => x.MeasPoint).FirstOrDefaultAsync(x => x.TimeStamp.Date.Equals(inst.TimeStamp.Date) && x.MeasPointId.Equals(mp.Id));
                        IBA.Airlifts.Add(airlift);
                    }
                    else if (inst.ItemTypeId == 2011)
                    {
                        IBA.Installations.Add(inst);
                    }
                    else if (inst.ItemTypeId == 1919)
                    {
                        IBA.TrafficDivs.Add(inst);
                    }
                    else if (inst.ItemTypeId == 1954)
                    {
                        IBA.WaterSamples.Add(inst);
                    }
                    else if (inst.ItemTypeId == 1918)
                    {
                        var pre_re = await _context.PreExcavations.Include(x => x.MeasPoint).LastOrDefaultAsync(x => x.TimeStamp.Date.Equals(inst.TimeStamp.Date) && (x.MeasPointId.Equals(mp.Id) || x.wellname.ToLower().Contains(mp.Name.ToLower())));
                        IBA.PreExcavations.Add(pre_re);
                    }
                    else if (inst.ItemTypeId == 1898 || inst.ItemTypeId == 1899 || inst.ItemTypeId == 1903 || inst.ItemTypeId == 1959 || inst.ItemTypeId == 1960 || inst.ItemTypeId == 1961 || inst.ItemTypeId == 1969 || inst.ItemTypeId == 1970 || inst.ItemTypeId == 1971 ||
                        inst.ItemTypeId == 1972)
                    {
                        IBA.PipeWorks.Add(inst);
                    }
                    else if (inst.ItemTypeId == 1962 || inst.ItemTypeId == 1963 || inst.ItemTypeId == 1964 || inst.ItemTypeId == 1965 || inst.ItemTypeId == 1966 || inst.ItemTypeId == 2013)
                    {
                        var decom = await _context.Decommissions.Include(x => x.MeasPoint).SingleOrDefaultAsync(x => x.TimeStamp.Date.Equals(inst.TimeStamp.Date) && x.MeasPointId.Equals(mp.Id));
                        IBA.Decommissions.Add(decom);
                    }

                    else
                    {
                        IBA.Installations.Add(inst);
                    }
                }
                return PartialView("_Billables", IBA);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet]
        public async Task<IActionResult> BilledActivitiesList(int? ProjectId)
        {
            if (ProjectId != null)
            {
                var project = await _context.Projects.SingleOrDefaultAsync(x => x.Id.Equals(ProjectId));
               
                
                var user = await _userManager.GetUserAsync(User);
                if (project.DivisionId != user.DivisionId)
                {
                    return NotFound();
                }
                var mps = await _context.MeasPoints.Include(x => x.Project).Where(x => x.ProjectId.Equals(ProjectId)).ToListAsync();
                List<ItemBilledActivities> IBAs = new List<ItemBilledActivities>();
                foreach (var mp in mps)
                {
                ItemBilledActivities IBA = new ItemBilledActivities();
                var allinstalls = await _context.Installations.Include(x => x.ItemType).Where(x => x.UniqueID.ToLower().Contains(mp.Name.ToLower())).OrderByDescending(x => x.TimeStamp).ToListAsync();
                foreach (Install inst in allinstalls)
                {


                    if (inst.ItemTypeId == 1917)
                    {
                        var pre = await _context.PreExcavations.Include(x => x.MeasPoint).FirstOrDefaultAsync(x => x.TimeStamp.Date.Equals(inst.TimeStamp.Date) && (x.MeasPointId.Equals(mp.Id) || x.wellname.ToLower().Contains(mp.Name.ToLower())));
                        IBA.PreExcavations.Add(pre);
                    }
                    else if (inst.ItemTypeId == 1922)
                    {
                        var drill = await _context.Wells.FirstOrDefaultAsync(x => x.Drill_Date_End.Date.Equals(inst.TimeStamp.Date) && x.WellName.Equals(mp.Name));
                        IBA.Drillings.Add(drill);
                    }
                    else if (inst.ItemTypeId == 1949)
                    {
                        var cp = await _context.ClearPumpTests.FirstOrDefaultAsync(x => x.Report_Date.Date.Equals(inst.TimeStamp.Date) && x.MeasPointId.Equals(mp.Id));
                        IBA.ClearPumpTests.Add(cp);
                    }
                    else if (inst.ItemTypeId == 1950)
                    {
                        var airlift = await _context.Airlifts.Include(x => x.MeasPoint).FirstOrDefaultAsync(x => x.TimeStamp.Date.Equals(inst.TimeStamp.Date) && x.MeasPointId.Equals(mp.Id));
                        IBA.Airlifts.Add(airlift);
                    }
                    else if (inst.ItemTypeId == 2011)
                    {
                        IBA.Installations.Add(inst);
                    }
                    else if (inst.ItemTypeId == 1919)
                    {
                        IBA.TrafficDivs.Add(inst);
                    }
                    else if (inst.ItemTypeId == 1954)
                    {
                        IBA.WaterSamples.Add(inst);
                    }
                    else if (inst.ItemTypeId == 1918)
                    {
                        var pre_re = await _context.PreExcavations.Include(x => x.MeasPoint).LastOrDefaultAsync(x => x.TimeStamp.Date.Equals(inst.TimeStamp.Date) && (x.MeasPointId.Equals(mp.Id) || x.wellname.ToLower().Contains(mp.Name.ToLower())));
                        IBA.PreExcavations.Add(pre_re);
                    }
                    else if (inst.ItemTypeId == 1898 || inst.ItemTypeId == 1899 || inst.ItemTypeId == 1903 || inst.ItemTypeId == 1959 || inst.ItemTypeId == 1960 || inst.ItemTypeId == 1961 || inst.ItemTypeId == 1969 || inst.ItemTypeId == 1970 || inst.ItemTypeId == 1971 ||
                        inst.ItemTypeId == 1972)
                    {
                        IBA.PipeWorks.Add(inst);
                    }
                    else if (inst.ItemTypeId == 1962 || inst.ItemTypeId == 1963 || inst.ItemTypeId == 1964 || inst.ItemTypeId == 1965 || inst.ItemTypeId == 1966 || inst.ItemTypeId == 2013)
                    {
                        var decom = await _context.Decommissions.Include(x => x.MeasPoint).SingleOrDefaultAsync(x => x.TimeStamp.Date.Equals(inst.TimeStamp.Date) && x.MeasPointId.Equals(mp.Id));
                        IBA.Decommissions.Add(decom);
                    }

                    else
                    {
                        IBA.Installations.Add(inst);
                    }
                }
                    IBAs.Add(IBA);
                }
                return PartialView("_BillablesList", IBAs);
            }
            else
            {
                return NotFound();
            }
        }


    }
}
