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
using MainOps.Services;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;
using System.IO;
using MainOps.ExtensionMethods;
using QRCoder;
using System.Drawing;
using Rotativa.AspNetCore;
using MainOps.Models.ViewModels;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Renci.SshNet;
using Renci.SshNet.Common;
using Microsoft.Extensions.Options;

namespace MainOps.Controllers
{
    [Authorize]
    public class MeasPointsController : BaseController
    {
        private readonly DataContext _context;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStringLocalizer<MeasPointsController> _localizer;
        private readonly IWebHostEnvironment _environment;
        private AuthSFTPOptions _Options { get; }

        public MeasPointsController(DataContext context, IEmailSender emailSender, UserManager<ApplicationUser> userManager, IStringLocalizer<MeasPointsController> localizer, IWebHostEnvironment environment,IOptions<AuthSFTPOptions> optionsAccessor) : base(context, userManager)
        {
            _context = context;
            _emailSender = emailSender;
            _userManager = userManager;
            _localizer = localizer;
            _environment = environment;
            _Options = optionsAccessor.Value;
        }

        // GET: MeasPoints
        public async Task<IActionResult> Index()
        {
            var theuser = await _userManager.GetUserAsync(HttpContext.User);
            IEnumerable<SelectListItem> selList = await createFilterlist();
            ViewData["Filterchoices"] = new SelectList(selList, "Value", "Text");
            if (User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
            {
                var data = await (from mp in _context.MeasPoints.Include(m => m.MeasType).Include(x => x.SubProject).Include(m => m.Project).Include(x => x.logger).OrderBy(x => x.Project.Name).ThenBy(x => x.SubProject.Name).ThenBy(x => x.Name)
                                  join pu in _context.ProjectUsers on mp.ProjectId
                                  equals pu.projectId
                                  where pu.userId == theuser.Id && mp.Project.Active.Equals(true) && mp.ToBeHidden.Equals(false)
                                  select mp).ToListAsync();
                ViewData["Title"] = _localizer["Index"];
                return View(data);

            }
            if (User.IsInRole("Admin"))
            {
                var dataContextAdmin = _context.MeasPoints.Include(m => m.MeasType).Include(x => x.SubProject).Include(m => m.Project).ThenInclude(x => x.Division).Include(x => x.logger)
                    .Where(x => x.Project.Active.Equals(true)).Take(100)
                .OrderBy(x => x.Project.Name).ThenBy(x => x.Name);
                ViewData["Title"] = _localizer["Index"];
                return View(await dataContextAdmin.ToListAsync());
            }
            var dataContext = _context.MeasPoints.Include(m => m.MeasType).Include(m => m.Project).ThenInclude(x => x.Division).Include(x => x.logger)
                .Where(x => x.Project.Division.Id.Equals(theuser.DivisionId) && x.Project.Active.Equals(true)).Take(100).OrderBy(x => x.Project.Name).ThenBy(x => x.Name);
            ViewData["Title"] = _localizer["Index"];
            return View(await dataContext.ToListAsync());
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AlarmBoxes()
        {
            List<SelectListItem> selList = await createMeasPointList2();
            ViewData["MeasPointId"] = selList;
            var data = await _context.MeasPoints.Include(x => x.MeasType).Include(x => x.MonitorType).Include(x => x.Project).Where(x => x.MonitorType.MonitorTypeName.ToLower().Equals("alarm box") && x.LoggerActive.Equals(true) && x.ToBeHidden.Equals(false)).ToListAsync();
            return View(data);
        }
        [HttpGet]
        public async Task<IActionResult> AllAlarmBoxes()
        {
            var data = await _context.MeasPoints.Include(x => x.MeasType).Include(x => x.MonitorType).Include(x => x.Project).Where(x => x.MonitorType.MonitorTypeName.ToLower().Equals("alarm box")).ToListAsync();
            return View(data);
        }
        [HttpGet]
        public async Task<IActionResult> EditAlarmBox(int id)
        {
            var mp = await _context.MeasPoints.FindAsync(id);
            ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.Active.Equals(true)).Include(x => x.Division).Where(x => x.Division.Id.Equals(1) && x.Active.Equals(true)), "Id", "Name");
            return View(mp);
        }
        [HttpPost]
        public async Task<IActionResult> EditAlarmBox(MeasPoint mp)
        {
            if (ModelState.IsValid)
            {
                _context.Update(mp);
                await _context.SaveChangesAsync();
            }

            ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.Active.Equals(true)).Include(x => x.Division).Where(x => x.Division.Id.Equals(1) && x.Active.Equals(true)), "Id", "Name");
            return RedirectToAction("AllAlarmBoxes");
        }
        [HttpGet]
        public IActionResult AddAlarmBox()
        {
            ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.Active.Equals(true)).Include(x => x.Division).Where(x => x.Division.Id.Equals(1) && x.Active.Equals(true)), "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddALarmBox(AddAlarmBoxVM model)
        {
            if (ModelState.IsValid)
            {
                MeasPoint mp = new MeasPoint();
                mp.Name = model.Name;
                mp.ProjectId = model.ProjectId;
                mp.Lati = model.Latitude;
                mp.Longi = model.Longitude;
                mp.MeasTypeId = 36;
                mp.MonitorTypeId = 19;
                mp.LoggerActive = true;
                mp.Coordx = Convert.ToDouble(model.Latitude);
                mp.Coordy = Convert.ToDouble(model.Longitude);
                mp.Coordz = 0;
                mp.LaborId = await FindRightLabor(model.ProjectId, 1);
                mp.Description = model.Description;
                _context.Add(mp);
                await _context.SaveChangesAsync();
                return RedirectToAction("AlarmBoxes");
            }
            return RedirectToAction("AlarmBoxes");

        }
        public async Task<IActionResult> UpdateCoords()
        {
            //LatLngUTMConverter ltUTMconv = new LatLngUTMConverter("WGS 84");
            var data = await _context.MeasPoints.Where(x => x.Name.ToLower().Contains("flow") || x.Name.ToLower().Contains("watermeter")).ToListAsync();
            foreach (MeasPoint mp in data)
            {
                if(mp.Coordx == 0 || mp.Coordy == 0) { 
                    StringBuilder sb = new StringBuilder();
                    string[] basenames = mp.Name.Trim().Split(' ');

                    for (int i = 0; i < (basenames.Count() - 1); i++)
                    {
                        if (i != 0)
                        {
                            sb.Append(" ");
                            sb.Append(basenames[i]);
                        }
                        else
                        {
                            sb.Append(basenames[i]);
                        }

                    }
                    var basepoints = await _context.MeasPoints.Where(x => x.Name.ToLower().Equals(sb.ToString().ToLower()) && x.ProjectId.Equals(mp.ProjectId)).ToListAsync();
                    if (basepoints.Count() > 0)
                    {
                        var basepoint = basepoints[0];
                        try
                        {
                            mp.Coordx = basepoint.Coordx;
                            mp.Coordy = basepoint.Coordy;
                            mp.Coordz = basepoint.Coordz;
                            
                            _context.Update(mp);
                            
                        }
                        catch
                        {

                        }
                    }
                }

            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> AllAlertSettings()
        {
            var theuser = await _userManager.GetUserAsync(HttpContext.User);
            IEnumerable<SelectListItem> selList = await createFilterlist();
            ViewData["Filterchoices"] = new SelectList(selList, "Value", "Text");
            if (User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
            {
                var data = await (from mp in _context.MeasPoints.Include(m => m.MeasType).Include(m => m.Project).Include(x => x.logger).OrderBy(x => x.Project.Name).ThenBy(x => x.Name)
                                  join pu in _context.ProjectUsers on mp.ProjectId
                                  equals pu.projectId
                                  where pu.userId == theuser.Id && mp.Project.Active.Equals(true)
                                  select new AlertEditVM(mp)).ToListAsync();

                ViewData["Title"] = _localizer["Index"];
                return View(data);

            }
            if (User.IsInRole("Admin"))
            {
                var dataContextAdmin = await (from mp in _context.MeasPoints.Include(m => m.MeasType).Include(m => m.Project).Include(x => x.logger)
                                              .OrderBy(x => x.Project.Name).ThenBy(x => x.Name)
                                              where mp.Project.Active.Equals(true)
                                              select new AlertEditVM(mp)).ToListAsync();
                ViewData["Title"] = _localizer["Index"];
                return View(dataContextAdmin);
            }
            var dataContext = await (from mp in _context.MeasPoints.Include(m => m.MeasType).Include(m => m.Project).Include(x => x.logger)
                                             .OrderBy(x => x.Project.Name).ThenBy(x => x.Name)
                                     where mp.Project.Active.Equals(true) && mp.Project.Division.Id.Equals(theuser.DivisionId) && mp.Project.Active.Equals(true)
                                     select new AlertEditVM(mp)).ToListAsync();
            ViewData["Title"] = _localizer["Index"];
            return View(dataContext);
        }
        [HttpPost]
        public async Task<IActionResult> GetAlerts(int? id)
        {
            if (id != null)
            {
                var measPoint = await _context.MeasPoints.FindAsync(id);
                AlertEditVM AEVM = new AlertEditVM(measPoint);
                return PartialView("_AlertEdit", AEVM);
            }
            else
            {
                return null;
            }
        }
        [HttpPost]
        public IActionResult EditAlerts(AlertEditVM model)
        {
            if (ModelState.IsValid)
            {
                var measPoint = _context.MeasPoints.Find(model.MeasPointId);
                measPoint.Lower_Lower_Limit = model.Lower_Lower_Limit;
                measPoint.Middle_Lower_Limit = model.Middle_Lower_Limit;
                measPoint.Middle_Upper_Limit = model.Middle_Upper_Limit;
                measPoint.Upper_Upper_Limit = model.Upper_Upper_Limit;
                measPoint.Flow_Lower_Lower_Limit = model.Flow_Lower_Lower_Limit;
                measPoint.Flow_Middle_Lower_Limit = model.Flow_Middle_Lower_Limit;
                measPoint.Flow_Middle_Upper_Limit = model.Flow_Middle_Upper_Limit;
                measPoint.Flow_Upper_Upper_Limit = model.Flow_Upper_Upper_Limit;
                _context.Update(measPoint);
                _context.SaveChanges();
                //measPoint = null;
                //MeasPoint mp = _context.MeasPoints.Find(model.MeasPointId);
                //AlertEditVM AEVM = new AlertEditVM(mp);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
        public async Task<IActionResult> ShowNonFunctional()
        {
            var theuser = await _userManager.GetUserAsync(HttpContext.User);
            IEnumerable<SelectListItem> selList = await createFilterlist();
            ViewData["Filterchoices"] = new SelectList(selList, "Value", "Text");
            if (User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
            {
                var data = await (from mp in _context.MeasPoints.Include(m => m.MeasType).Include(m => m.Project).Include(x => x.logger).OrderBy(x => x.Project.Name).ThenBy(x => x.Name)
                                  join pu in _context.ProjectUsers on mp.ProjectId
                                  equals pu.projectId
                                  where pu.userId == theuser.Id && mp.LoggerActive.Equals(false)
                                  select mp).ToListAsync();
                ViewData["Title"] = _localizer["Index"];
                return View("Index", data);

            }
            var dataContext = _context.MeasPoints
                .Include(m => m.MeasType).Include(m => m.Project).ThenInclude(x => x.Division).Include(x => x.logger)
                .Where(x => x.Project.Division.Id.Equals(theuser.DivisionId) && x.LoggerActive.Equals(false)).OrderBy(x => x.Project.Name).ThenBy(x => x.Name);
            ViewData["Title"] = _localizer["Index"];
            return View("Index", dataContext);
        }
        [HttpGet]
        public async Task<IActionResult> ShowHidden()
        {
            var theuser = await _userManager.GetUserAsync(HttpContext.User);
            IEnumerable<SelectListItem> selList = await createFilterlist();
            ViewData["Filterchoices"] = new SelectList(selList, "Value", "Text");
            if (User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
            {
                var data = await (from mp in _context.MeasPoints.Include(m => m.MeasType).Include(m => m.Project).Include(x => x.logger).OrderBy(x => x.Project.Name).ThenBy(x => x.Name)
                                  join pu in _context.ProjectUsers on mp.ProjectId
                                  equals pu.projectId
                                  where pu.userId == theuser.Id && mp.ToBeHidden.Equals(true)
                                  select mp).ToListAsync();
                ViewData["Title"] = _localizer["Index"];
                return View("Index", data);

            }
            var dataContext = _context.MeasPoints
                .Include(m => m.MeasType).Include(m => m.Project).ThenInclude(x => x.Division).Include(x => x.logger)
                .Where(x => x.Project.Division.Id.Equals(theuser.DivisionId) && x.ToBeHidden.Equals(true)).OrderBy(x => x.Project.Name).ThenBy(x => x.Name);
            ViewData["Title"] = _localizer["Index"];
            return View("Index", dataContext);
        }
        public async Task<IActionResult> ShowData_2(int theid, string theName)
        {
            var measpoints = await createWMMeasPointList();
            ViewData["graphname"] = theName;
            ViewData["MeasPointId"] = theid;
            ViewData["MeasPoints"] = measpoints;
            return View();
        }
        public async Task<IActionResult> ShowData(int theid, string theName)
        {
            var measpoints = await createMeasPointList(theid);
            ViewData["graphname"] = theName;
            ViewData["MeasPointId"] = theid;
            ViewData["MeasPoints"] = measpoints;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> JsonData(int id)
        {
            var thedata = await _context.Measures.Include(x => x.MeasPoint).ThenInclude(x => x.MeasType).Where(x => x.MeasPointId.Equals(id) && x.TheMeasurement != null).OrderBy(x => x.When).ToListAsync();
            List<double> therealmeases = await findRealMeases(thedata.First().When, thedata.Last().When, thedata.First().MeasPoint.Id);
            List<PlotData> plotdatas = new List<PlotData>();
            int counter = 0;
            if (thedata.First().MeasPoint.MeasTypeId.Equals(113))
            {
                foreach (Meas x in thedata)
                {
                    PlotData pd = new PlotData();
                    pd.DataValue = therealmeases[counter] * 10.0;
                    pd.TimeStamp = x.When;
                    plotdatas.Add(pd);
                    counter += 1;
                }
            }
            else
            {
                foreach (Meas x in thedata)
                {
                    PlotData pd = new PlotData();
                    pd.DataValue = therealmeases[counter];
                    pd.TimeStamp = x.When;
                    plotdatas.Add(pd);
                    counter += 1;
                }
            }
            var data = plotdatas.ToArray();
            return Json(data);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> JsonDataFlowRateMinutes(int id)
        {
            var thedata = await _context.Measures.Include(x => x.MeasPoint).ThenInclude(x => x.MeasType).Where(x => x.MeasPointId.Equals(id) && x.TheMeasurement != null).OrderBy(x => x.When).ToListAsync();
            List<double> therealmeases = await findRealMeases(thedata.First().When, thedata.Last().When, thedata.First().MeasPoint.Id);
            List<PlotData> plotdatas = new List<PlotData>();
            int counter = 0;
            if (thedata.First().MeasPoint.MeasTypeId.Equals(113))
            {
                foreach (Meas x in thedata)
                {
                    if (counter != 0)
                    {
                        if ((thedata[counter].When - thedata[counter - 1].When).TotalMinutes != 0)
                        {
                            PlotData pd = new PlotData();
                            pd.DataValue = (therealmeases[counter] * 10.0 - therealmeases[counter - 1] * 10.0) / (thedata[counter].When - thedata[counter - 1].When).TotalMinutes;
                            //pd.DataValue = await measdiff(thedata[counter - 1], thedata[counter]) / (thedata[counter].When - thedata[counter - 1].When).TotalMinutes;
                            pd.TimeStamp = x.When;
                            plotdatas.Add(pd);
                        }
                    }
                    counter += 1;
                }
            }
            else
            {
                foreach (Meas x in thedata)
                {
                    if (counter != 0)
                    {
                        if ((thedata[counter].When - thedata[counter - 1].When).TotalMinutes != 0)
                        {
                            PlotData pd = new PlotData();
                            pd.DataValue = (therealmeases[counter] - therealmeases[counter - 1]) / (thedata[counter].When - thedata[counter - 1].When).TotalMinutes;
                            //pd.DataValue = await measdiff(thedata[counter - 1], thedata[counter]) / (thedata[counter].When - thedata[counter - 1].When).TotalMinutes;
                            pd.TimeStamp = x.When;
                            plotdatas.Add(pd);
                        }
                    }
                    counter += 1;
                }
            }
            var data = plotdatas.ToArray();
            return Json(data);
        }
        public async Task<double> measdiff(Meas m1, Meas m2)
        {
            double truemeas1 = await findRealMeas(m1);
            var offsets = await _context.Offsets.Where(x => x.MeasPointId.Equals(m2.MeasPointId) && x.starttime > m1.When && x.starttime <= m2.When).OrderByDescending(x => x.starttime).ToListAsync();
            if (offsets != null)
            {
                if (offsets.Count > 0)
                {
                    return (Convert.ToDouble(m2.TheMeasurement) - offsets.First().offset) - truemeas1;
                }
                else
                {
                    return Convert.ToDouble(m2.TheMeasurement) - truemeas1;
                }
            }
            else
            {
                return Convert.ToDouble(m2.TheMeasurement) - truemeas1;
            }
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> JsonDataTotalLitersPerDay(int id)
        {
            var thedata = await _context.Measures.Include(x => x.MeasPoint).ThenInclude(x => x.MeasType).Where(x => x.MeasPointId.Equals(id) && x.TheMeasurement != null).OrderBy(x => x.When).ToListAsync();
            List<double> therealmeases = await findRealMeases(thedata.First().When, thedata.Last().When, thedata.First().MeasPoint.Id);
            List<PlotData> plotdatas = new List<PlotData>();
            int dayspan = 0;
            double flowrate = 0;
            for (DateTime dt = thedata[1].When.Date; dt <= thedata.Last().When.Date; dt = dt.AddDays(1).Date)
            {
                var measures = thedata.Where(x => x.When.Date.Equals(dt)).OrderBy(x => x.When);
                if (measures == null)
                {
                    dayspan += 1;
                }
                else if (measures.Count().Equals(0) || thedata.IndexOf(measures.Last()).Equals(1))
                {
                    dayspan += 1;
                }
                else if (measures.Count().Equals(1))
                {
                    var themeas = measures.First();
                    int indexOfthemeas = thedata.IndexOf(themeas);
                    var lastmeas = thedata.Where(x => x.When < dt).OrderByDescending(x => x.When).First();
                    int indexOfthemeasBefore = thedata.IndexOf(lastmeas);
                    flowrate = (therealmeases[indexOfthemeas] - therealmeases[indexOfthemeasBefore]) / (themeas.When - lastmeas.When).TotalMinutes;
                    //double flowrate = await measdiff(lastmeas,themeas) / (themeas.When - lastmeas.When).TotalMinutes;
                    for (int i = dayspan; i >= 0; i--)
                    {
                        PlotData pd = new PlotData();
                        pd.DataValue = flowrate * 60.0 * 24.0;
                        pd.TimeStamp = dt.AddDays(-i);
                        plotdatas.Add(pd);
                    }
                    dayspan = 1;
                }
                else
                {
                    var themeas = measures.First();
                    int indexOfthemeas = thedata.IndexOf(themeas);
                    var lastmeas = thedata.Where(x => x.When < dt).OrderByDescending(x => x.When).First();
                    int indexOfthemeasBefore = thedata.IndexOf(lastmeas);

                    //double flowrate = await measdiff(lastmeas, themeas) / (themeas.When - lastmeas.When).TotalMinutes;
                    if ((themeas.When - lastmeas.When).TotalMinutes != 0)
                    {
                        flowrate = (therealmeases[indexOfthemeas] - therealmeases[indexOfthemeasBefore]) / (themeas.When - lastmeas.When).TotalMinutes;
                    }
                    for (int i = dayspan; i >= 1; i--)
                    {
                        PlotData pd = new PlotData();
                        pd.DataValue = flowrate * 60.0 * 24.0;
                        pd.TimeStamp = dt.AddDays(-i);
                        plotdatas.Add(pd);
                    }

                    themeas = measures.Last();
                    indexOfthemeas = thedata.IndexOf(themeas);
                    lastmeas = measures.First();
                    indexOfthemeasBefore = thedata.IndexOf(lastmeas);
                    //flowrate = await measdiff(lastmeas, themeas) / (themeas.When - lastmeas.When).TotalMinutes;
                    if ((themeas.When - lastmeas.When).TotalMinutes != 0)
                    {
                        flowrate = (therealmeases[indexOfthemeas] - therealmeases[indexOfthemeasBefore]) / (themeas.When - lastmeas.When).TotalMinutes;
                    }
                    PlotData pd2 = new PlotData();
                    pd2.DataValue = flowrate * 60.0 * 24.0;
                    pd2.TimeStamp = dt;
                    plotdatas.Add(pd2);
                    dayspan = 1;
                }
            }
            var data = plotdatas.ToArray();
            return Json(data);
        }
        public async Task<ActionResult> SomeAction(int id)
        {
            LatLngUTMConverter ltUTMconv = new LatLngUTMConverter("WGS 84");
            var mp = _context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.CoordSystem)
                .Where(x => x.Id.Equals(id)).SingleOrDefault();
            LatLngUTMConverter.LatLng latlng;
            if (mp.Project.CoordSystem.system == "UTM32N")
            {
                latlng = ltUTMconv.convertUtmToLatLng(mp.Coordx, mp.Coordy, 32, "N");
                if (mp.Lati == null || mp.Longi == null)
                {
                    mp.Lati = latlng.Lat;
                    mp.Longi = latlng.Lng;
                    _context.Update(mp);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    mp.Lati = latlng.Lat;
                    mp.Longi = latlng.Lng;
                }
            }
            else if (mp.Project.CoordSystem.system == "UTM17N")
            {
                latlng = ltUTMconv.convertUtmToLatLng(mp.Coordx, mp.Coordy, 17, "N");

                if (mp.Lati == null || mp.Longi == null)
                {
                    mp.Lati = latlng.Lat;
                    mp.Longi = latlng.Lng;
                    _context.Update(mp);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    mp.Lati = latlng.Lat;
                    mp.Longi = latlng.Lng;
                }
            }
            else
            {
                latlng = await findUTMcoords(mp);
                if (mp.Lati == null || mp.Longi == null)
                {
                    mp.Lati = latlng.Lat;
                    mp.Longi = latlng.Lng;
                    _context.Update(mp);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    mp.Lati = latlng.Lat;
                    mp.Longi = latlng.Lng;
                }
            }

            return Json(new { foo = latlng.Lat, baz = latlng.Lng, name = mp.Project.Name + " : " + mp.Name });
        }
        [HttpPost]
        public async Task<ActionResult> GetMarker(int id)
        {
            LatLngUTMConverter ltUTMconv = new LatLngUTMConverter("WGS 84");
            var mp = await _context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.CoordSystem)
                .Where(x => x.Id.Equals(id)).SingleOrDefaultAsync();
            if (mp.Project.CoordSystem.system == "UTM32N")
            {
                if ((mp.Lati == null || mp.Longi == null) && !(mp.Coordx == 0 || mp.Coordy == 0))
                {
                    LatLngUTMConverter.LatLng latlng = ltUTMconv.convertUtmToLatLng(mp.Coordx, mp.Coordy, 32, "N");
                    mp.Lati = latlng.Lat;
                    mp.Longi = latlng.Lng;
                    _context.Update(mp);
                    await _context.SaveChangesAsync();
                    return Json(new { foo = latlng.Lat.ToString(), baz = latlng.Lng.ToString() });
                }
            }
            else if (mp.Project.CoordSystem.system == "UTM17N")
            {
                if ((mp.Lati == null || mp.Longi == null) && !(mp.Coordx == 0 || mp.Coordy == 0))
                {
                    LatLngUTMConverter.LatLng latlng = ltUTMconv.convertUtmToLatLng(mp.Coordx, mp.Coordy, 17, "N");
                    mp.Lati = latlng.Lat;
                    mp.Longi = latlng.Lng;
                    _context.Update(mp);
                    await _context.SaveChangesAsync();
                    return Json(new { foo = latlng.Lat.ToString(), baz = latlng.Lng.ToString() });
                }
            }
            else
            {
                LatLngUTMConverter.LatLng latlng = await findUTMcoords(mp);
                mp.Lati = latlng.Lat;
                mp.Longi = latlng.Lng;
                if (mp.Lati != null && mp.Longi != null)
                {
                    _context.Update(mp);
                    await _context.SaveChangesAsync();
                }
                return Json(new { foo = latlng.Lat.ToString(), baz = latlng.Lng.ToString() });
            }
            return Json(new { foo = "lol", baz = "lol2" });
        }
        [HttpPost]
        public async Task<JsonResult> JsonData2(int id)
        {

            var thedata = await _context.DataPoints.Include(x => x.MeasPoint).Where(x => x.MeasPointId.Equals(id)).OrderBy(x => x.Datum).ToListAsync();
            List<PlotData> plotdatas = new List<PlotData>();
            foreach (DataPoint x in thedata)
            {
                PlotData pd = new PlotData();
                pd.DataValue = x.MessWert;
                pd.TimeStamp = x.Datum;
                plotdatas.Add(pd);
            }
            var data = plotdatas.ToArray();
            return Json(data);
        }

        // GET: MeasPoints/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var measPoint = await _context.MeasPoints
                .Include(m => m.MeasType)
                .Include(m => m.Project)
                .Include(m => m.logger)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (measPoint == null)
            {
                return NotFound();
            }
            ViewData["Title"] = _localizer["Details"];
            return View(measPoint);
        }
        public async Task<int> FindRightLabor(int projectnr, int labnr)
        {
            var theuser = await _userManager.GetUserAsync(User);
            var project = await _context.Projects.FindAsync(projectnr);
            var division = await _context.Divisions.Where(x => x.Id.Equals(project.DivisionId)).FirstAsync();
            List<int> laborids = new List<int>();
            var all_measpoints = await _context.MeasPoints.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(division.Id)).ToListAsync();
            foreach (MeasPoint mp in all_measpoints)
            {
                laborids.Add(mp.LaborId);
            }
            int thelabor = labnr;
            while (true)
            {
                if (laborids.IndexOf(thelabor) < 0)
                {
                    return thelabor;
                }
                else
                {
                    thelabor += 1;
                }
            }
        }
        // GET: MeasPoints/Create
        [Authorize(Roles = "Admin,DivisionAdmin,Member")]
        public async Task<IActionResult> Create()
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["LoggerId"] = new SelectList(_context.Loggers.Include(x => x.Division), "Id", "LoggerNo");
                ViewData["MeasTypeId"] = new SelectList(_context.MeasTypes.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Type");
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.Active.Equals(true)).Include(x => x.Division), "Id", "Name");
                ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Project.DivisionId.Equals(theuser.DivisionId)), "Id", "Name");
                ViewData["MonitorTypeId"] = new SelectList(_context.MonitorType.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "MonitorTypeName");
                ViewData["Title"] = _localizer["Create"];
                return View();
            }
            ViewData["LoggerId"] = new SelectList(_context.Loggers.Include(x => x.Division).Where(x => x.Division.Id.Equals(theuser.DivisionId)), "Id", "LoggerNo");
            ViewData["MeasTypeId"] = new SelectList(_context.MeasTypes.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Type");
            ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.Active.Equals(true)).Include(x => x.Division).Where(x => x.Division.Id.Equals(theuser.DivisionId)), "Id", "Name");
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Project.DivisionId.Equals(theuser.DivisionId)), "Id", "Name");
            ViewData["MonitorTypeId"] = new SelectList(_context.MonitorType.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "MonitorTypeName");
            ViewData["Title"] = _localizer["Create"];
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> UpdateUTMandLatLng(int? ProjectId)
        {
            if(ProjectId != null) {
                var measpoints = await _context.MeasPoints.Where(x => x.ProjectId.Equals(ProjectId) && !x.MeasType.Type.Equals("Miscellaneous")).ToListAsync();
                LatLngUTMConverter ltUTMconv = new LatLngUTMConverter("WGS 84");
                foreach (var mp in measpoints)
                {
                    LatLngUTMConverter.LatLng res = await findUTMcoords(mp);
                    if(res.Lat > 0)
                    {
                        mp.Lati = res.Lat;
                        mp.Longi = res.Lng;
                        if (mp.Lati != null && mp.Longi != null && mp.Lati != 0 && mp.Longi != 0)
                        {
                            _context.Update(mp);
                        }
                   
                    }
                

                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Home");
        }
        // POST: MeasPoints/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DivisionAdmin,Member")]
        public async Task<IActionResult> Create([Bind("Id,Name,Offset,Coordx,Coordy,Coordz,MonitorTypeId,ProjectId,MeasTypeId,LaborId,LoggerId,ScadaAddress,LoggerActive,SubProjectId,DGUnr,Bottom,Coordinates_Are_AsBuilt")] MeasPoint measPoint, bool addFlow = false, bool addWM = false)
        {
            if (ModelState.IsValid)
            {
                measPoint.ExternalCoordx = measPoint.Coordx;
                measPoint.ExternalCoordy = measPoint.Coordy;
                measPoint.ExternalCoordz = measPoint.Coordz;
                var user = await _userManager.GetUserAsync(User);
                int rightlabor = 100000;
                if (measPoint.ProjectId != null)
                {
                    var projid = Convert.ToInt32(measPoint.ProjectId);
                    rightlabor = await FindRightLabor(projid, measPoint.LaborId);
                }
                else
                {
                    rightlabor = await FindRightLabor(100000, measPoint.LaborId);
                }
                var proj = await _context.Projects.Include(x => x.CoordSystem).Where(x => x.Id.Equals(measPoint.ProjectId)).SingleOrDefaultAsync();
                if (measPoint.Coordx >= 0.1)
                {
                    //
                    LatLngUTMConverter ltUTMconv = new LatLngUTMConverter("WGS 84");
                    if (proj.CoordSystem.system != "LatLong")
                    {
                        if ((measPoint.Lati == null && measPoint.Longi == null) || (measPoint.Lati == 0 && measPoint.Longi == 0))
                        {
                            if (proj.CoordSystem.system == "UTM32N")
                            {
                                if ((measPoint.Lati == null || measPoint.Longi == null) && !(measPoint.Coordx == 0 || measPoint.Coordy == 0))
                                {
                                    LatLngUTMConverter.LatLng latlng = ltUTMconv.convertUtmToLatLng(measPoint.Coordx, measPoint.Coordy, 32, "N");
                                    measPoint.Lati = latlng.Lat;
                                    measPoint.Longi = latlng.Lng;
                                }
                            }
                            else if (proj.CoordSystem.system == "UTM17N")
                            {
                                if ((measPoint.Lati == null || measPoint.Longi == null) && !(measPoint.Coordx == 0 || measPoint.Coordy == 0))
                                {
                                    LatLngUTMConverter.LatLng latlng = ltUTMconv.convertUtmToLatLng(measPoint.Coordx, measPoint.Coordy, 17, "N");
                                    measPoint.Lati = latlng.Lat;
                                    measPoint.Longi = latlng.Lng;
                                }
                            }
                            else
                            {
                                LatLngUTMConverter.LatLng latlng = await findUTMcoords(measPoint);
                                measPoint.Lati = latlng.Lat;
                                measPoint.Longi = latlng.Lng;
                            }
                        }

                    }
                    else
                    {
                        if ((measPoint.Lati == null || measPoint.Longi == null) || (measPoint.Lati == 0 || measPoint.Longi == 0))
                        {
                            if (measPoint.Coordx != 0 && measPoint.Coordy != 0)
                            {
                                measPoint.Lati = measPoint.Coordx;
                                measPoint.Longi = measPoint.Coordy;
                            }
                        }
                    }
                    //

                }
                measPoint.LaborId = rightlabor;
                _context.Add(measPoint);
                await _context.SaveChangesAsync();
                var thenewmeaspoint = await _context.MeasPoints.Where(x => x.Name.ToLower().Trim().Equals(measPoint.Name.ToLower().Trim()) && x.ProjectId.Equals(measPoint.ProjectId)).OrderByDescending(x => x.Id).FirstAsync();
                if (measPoint.LoggerId != null)
                {
                    LoggerChange LC = new LoggerChange(Convert.ToInt32(measPoint.LoggerId), thenewmeaspoint.Id, true, false);
                    _context.LoggerChanges.Add(LC);
                    await _context.SaveChangesAsync();
                    var previousInstall = await _context.LoggerChanges.Where(x => x.LoggerId.Equals(measPoint.LoggerId) && x.MeasPointId != thenewmeaspoint.Id && x.LoggerRemoved.Equals(false)).OrderByDescending(x => x.When).ToListAsync();
                    if (previousInstall != null)
                    {
                        if (previousInstall.Count() > 0)
                        {
                            var lastInstall = previousInstall.First();
                            LoggerChange LC2 = new LoggerChange(lastInstall.LoggerId, lastInstall.MeasPointId, false, true);
                            _context.LoggerChanges.Add(LC2);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                Offset newOffset = new Offset();
                newOffset.MeasPointId = thenewmeaspoint.Id;
                newOffset.offset = thenewmeaspoint.Offset;
                newOffset.starttime = DateTime.Now;
                _context.Add(newOffset);
                await _context.SaveChangesAsync();
                try
                {
                    await _emailSender.SendEmailAsync("OFW@hj-as.dk", "New MeasPoint", string.Format("New MeasPoint created with name: {0} and LaborId {1}", measPoint.Name, measPoint.LaborId));
                }
                catch
                {

                }
                if (addFlow)
                {
                    MeasPoint flowMeas = new MeasPoint();
                    flowMeas.Coordx = measPoint.Coordx;
                    flowMeas.Coordy = measPoint.Coordy;
                    flowMeas.Coordz = measPoint.Coordz;
                    flowMeas.ExternalCoordx = measPoint.Coordx;
                    flowMeas.ExternalCoordy = measPoint.Coordy;
                    flowMeas.ExternalCoordz = measPoint.Coordz;
                    flowMeas.Coordinates_Are_AsBuilt = measPoint.Coordinates_Are_AsBuilt;
                    flowMeas.Lati = measPoint.Lati;
                    flowMeas.Longi = measPoint.Longi;
                   
                    var meastype = await _context.MeasTypes.FirstOrDefaultAsync(x => x.DivisionId.Equals(user.DivisionId) && x.Type.ToLower().Equals("flow rate"));
                    var monitorttype = await _context.MonitorType.FirstOrDefaultAsync(x => x.DivisionId.Equals(user.DivisionId) && x.MonitorTypeName.ToLower().Equals("flow meter"));
                    flowMeas.MeasTypeId = meastype.Id;
                    flowMeas.MonitorTypeId = monitorttype.Id;
                    flowMeas.ProjectId = measPoint.ProjectId;
                    flowMeas.Offset = 0;
                    flowMeas.LoggerActive = true;
                    flowMeas.Name = measPoint.Name + " Flow";
                    if (measPoint.ScadaAddress != null)
                    {
                        var monityp = await _context.MonitorType.FindAsync(measPoint.MonitorTypeId);
                        if (monityp.MonitorTypeName == "Reinfiltration Well")
                        {
                            flowMeas.ScadaAddress = measPoint.ScadaAddress - 1;
                        }
                        else if (monityp.MonitorTypeName == "Pumping Well")
                        {
                            flowMeas.ScadaAddress = measPoint.ScadaAddress - 3;
                        }

                    }

                    var projid = Convert.ToInt32(flowMeas.ProjectId);
                    rightlabor = await FindRightLabor(projid, measPoint.LaborId);
                    flowMeas.LaborId = rightlabor;
                    _context.Add(flowMeas);
                    await _context.SaveChangesAsync();
                    var last_added1 = await _context.MeasPoints.LastAsync();
                    Offset newOffsetflow = new Offset();
                    newOffsetflow.MeasPointId = last_added1.Id;
                    newOffsetflow.offset = last_added1.Offset;
                    newOffsetflow.starttime = DateTime.Now;
                    _context.Add(newOffsetflow);
                    await _context.SaveChangesAsync();
                }
                if (addWM)
                {
                    MeasPoint VMMeas = new MeasPoint();
                    VMMeas.Coordx = measPoint.Coordx;
                    VMMeas.Coordy = measPoint.Coordy;
                    VMMeas.Coordz = measPoint.Coordz;
                    VMMeas.ExternalCoordx = measPoint.Coordx;
                    VMMeas.ExternalCoordy = measPoint.Coordy;
                    VMMeas.ExternalCoordz = measPoint.Coordz;
                    VMMeas.Coordinates_Are_AsBuilt = measPoint.Coordinates_Are_AsBuilt;
                    VMMeas.Lati = measPoint.Lati;
                    VMMeas.Longi = measPoint.Longi;
                    var meastype = await _context.MeasTypes.FirstOrDefaultAsync(x => x.DivisionId.Equals(user.DivisionId) && x.Type.ToLower().Equals("water meter"));
                    var monitorttype = await _context.MonitorType.FirstOrDefaultAsync(x => x.DivisionId.Equals(user.DivisionId) && x.MonitorTypeName.ToLower().Equals("flow meter"));
                    VMMeas.MeasTypeId = meastype.Id;
                    VMMeas.MonitorTypeId = monitorttype.Id;
                    VMMeas.ProjectId = measPoint.ProjectId;
                    VMMeas.Offset = 0;
                    VMMeas.LoggerActive = true;
                    VMMeas.Name = measPoint.Name + " Watermeter";
                    var projid = Convert.ToInt32(VMMeas.ProjectId);
                    rightlabor = await FindRightLabor(projid, measPoint.LaborId);
                    VMMeas.LaborId = rightlabor;
                    _context.Add(VMMeas);
                    await _context.SaveChangesAsync();
                    var last_added2 = await _context.MeasPoints.LastAsync();
                    Offset newOffsetWM = new Offset();
                    newOffsetWM.MeasPointId = last_added2.Id;
                    newOffsetWM.offset = last_added2.Offset;
                    newOffsetWM.starttime = DateTime.Now;
                    _context.Add(newOffsetWM);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["LoggerId"] = new SelectList(_context.Loggers.Include(x => x.Division).Where(x => x.Division.Id.Equals(theuser.DivisionId)), "Id", "LoggerNo", measPoint.LoggerId);
                ViewData["MeasTypeId"] = new SelectList(_context.MeasTypes, "Id", "Type", measPoint.MeasTypeId);
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.Active.Equals(true)).Include(x => x.Division), "Id", "Name", measPoint.ProjectId);
                ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => x.Project.Active.Equals(true)).Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Project.DivisionId.Equals(theuser.DivisionId)), "Id", "Name");
                ViewData["MonitorTypeId"] = new SelectList(_context.MonitorType, "Id", "MonitorTypeName");

            }
            else
            {
                ViewData["LoggerId"] = new SelectList(_context.Loggers.Include(x => x.Division).Where(x => x.Division.Id.Equals(theuser.DivisionId)), "Id", "LoggerNo", measPoint.LoggerId);
                ViewData["MeasTypeId"] = new SelectList(_context.MeasTypes.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Type", measPoint.MeasTypeId);
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.Active.Equals(true)).Include(x => x.Division).Where(x => x.Division.Id.Equals(theuser.DivisionId)), "Id", "Name", measPoint.ProjectId);
                ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => x.Project.Active.Equals(true)).Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Project.DivisionId.Equals(theuser.DivisionId)), "Id", "Name");
                ViewData["MonitorTypeId"] = new SelectList(_context.MonitorType.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "MonitorTypeName");

            }
            return View(measPoint);
        }
        public async Task<IActionResult> addFlow(int? id)
        {
            MeasPoint flowMeas = new MeasPoint();
            var measPoint = await _context.MeasPoints.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);
            var meastype = await _context.MeasTypes.FirstOrDefaultAsync(x => x.DivisionId.Equals(user.DivisionId) && x.Type.ToLower().Equals("flow rate"));
            var monitorttype = await _context.MonitorType.FirstOrDefaultAsync(x => x.DivisionId.Equals(user.DivisionId) && x.MonitorTypeName.ToLower().Equals("flow meter"));
            flowMeas.Coordx = measPoint.Coordx;
            flowMeas.Coordy = measPoint.Coordy;
            flowMeas.Coordz = measPoint.Coordz;
            flowMeas.Coordinates_Are_AsBuilt = measPoint.Coordinates_Are_AsBuilt;
            flowMeas.MeasTypeId = meastype.Id;
            flowMeas.MonitorTypeId = monitorttype.Id;
            flowMeas.ProjectId = measPoint.ProjectId;
            flowMeas.Offset = 0;
            flowMeas.Name = measPoint.Name + " Flow";
            if (measPoint.ScadaAddress != null)
            {
                var monityp = await _context.MonitorType.FindAsync(measPoint.MonitorTypeId);
                if (monityp.MonitorTypeName == "Reinfiltration Well")
                {
                    flowMeas.ScadaAddress = measPoint.ScadaAddress - 1;
                }
                else if (monityp.MonitorTypeName == "Pumping Well")
                {
                    flowMeas.ScadaAddress = measPoint.ScadaAddress - 3;
                }

            }
            var projid = Convert.ToInt32(flowMeas.ProjectId);
            int rightlabor = await FindRightLabor(projid, measPoint.LaborId);
            flowMeas.LaborId = rightlabor;
            _context.Add(flowMeas);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Member,DivisionAdmin")]
        public async Task<bool> ToggleActivity(int theid, DateTime thetime)
        {
            var measpoint = await _context.MeasPoints.Include(x => x.MonitorType).Where(x => x.Id.Equals(theid)).FirstAsync();
            measpoint.LoggerActive = !measpoint.LoggerActive;
            if (measpoint.LoggerActive && measpoint.MonitorType.MonitorTypeName.ToLower() == "pumping well")
            {
                PumpActivity PA = new PumpActivity(measpoint, thetime);
                _context.Add(PA);
                await _context.SaveChangesAsync();
            }
            else if (!measpoint.LoggerActive && measpoint.MonitorType.MonitorTypeName.ToLower() == "pumping well")
            {
                try
                {
                    var pumpactivity = await _context.PumpActivities.Where(x => x.MeasPointId.Equals(measpoint.Id)).LastAsync();
                    pumpactivity.End_activity = thetime;
                    _context.Update(pumpactivity);
                    await _context.SaveChangesAsync();
                }
                catch
                {

                }

            }
            _context.MeasPoints.Update(measpoint);
            await _context.SaveChangesAsync();
            return measpoint.LoggerActive;
        }
        [Authorize(Roles = "Admin,DivisionAdmin")]
        [HttpGet]
        public IActionResult UploadSCADAAddresses()
        {
            return View();
        }
        [Authorize(Roles = "Admin,DivisionAdmin")]
        [HttpPost]
        public async Task<IActionResult> UploadSCADAAddresses(IFormFile postedFile)
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
                   

                    while (!sreader.EndOfStream)
                    {

                        string[] rows = sreader.ReadLine().Split(';');
                        try { 
                        string basenamefile = rows[1].Split(" ")[1].ToLower();
                        if (rows[1].ToLower().Contains("flow"))
                        {
                            var mp = await _context.MeasPoints.Where(x => x.MeasTypeId.Equals(4) && x.Name.ToLower().Contains(basenamefile)).SingleOrDefaultAsync();
                            if(mp != null)
                            {
                                mp.ScadaAddress = Convert.ToInt32(rows[0]);
                                _context.Update(mp);
                            }
                        }
                        else
                        {
                            var mp = await _context.MeasPoints.Where(x => x.MeasTypeId.Equals(1) && x.Name.ToLower().Contains(basenamefile)).SingleOrDefaultAsync();
                            if (mp != null)
                            {
                                mp.ScadaAddress = Convert.ToInt32(rows[0]);
                                _context.Update(mp);
                            }
                        }
                        }
                        catch
                        {
                            double much = 1.0;
                        }



                    }
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Member,DivisionAdmin")]
        public async Task<bool> ToggleHidden(int theid)
        {
            var measpoint = await _context.MeasPoints.Where(x => x.Id.Equals(theid)).FirstAsync();
            measpoint.ToBeHidden = !measpoint.ToBeHidden;
            _context.MeasPoints.Update(measpoint);
            await _context.SaveChangesAsync();
            return measpoint.ToBeHidden;
        }
        [Authorize(Roles = "Admin,Member,DivisionAdmin")]
        public async Task<IActionResult> addWM(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            var meastype = await _context.MeasTypes.FirstOrDefaultAsync(x => x.DivisionId.Equals(user.DivisionId) && x.Type.ToLower().Equals("water meter"));
            var monitorttype = await _context.MonitorType.FirstOrDefaultAsync(x => x.DivisionId.Equals(user.DivisionId) && x.MonitorTypeName.ToLower().Equals("flow meter"));
            MeasPoint VMMeas = new MeasPoint();
            var measPoint = await _context.MeasPoints.FindAsync(id);
            VMMeas.Coordx = measPoint.Coordx;
            VMMeas.Coordy = measPoint.Coordy;
            VMMeas.Coordz = measPoint.Coordz;
            VMMeas.ExternalCoordx = measPoint.Coordx;
            VMMeas.ExternalCoordy = measPoint.Coordy;
            VMMeas.ExternalCoordz = measPoint.Coordz;
            VMMeas.Coordinates_Are_AsBuilt = measPoint.Coordinates_Are_AsBuilt;
            VMMeas.MonitorTypeId = monitorttype.Id;
            VMMeas.MeasTypeId = meastype.Id;
            VMMeas.ProjectId = measPoint.ProjectId;
            VMMeas.Offset = 0;
            VMMeas.Name = measPoint.Name + " Watermeter";
            var projid = Convert.ToInt32(VMMeas.ProjectId);
            int rightlabor = await FindRightLabor(projid, measPoint.LaborId);
            VMMeas.LaborId = rightlabor;
            _context.Add(VMMeas);
            await _context.SaveChangesAsync();
            var last_added2 = await _context.MeasPoints.LastAsync();
            Offset newOffsetWM = new Offset();
            newOffsetWM.MeasPointId = last_added2.Id;
            newOffsetWM.offset = last_added2.Offset;
            newOffsetWM.starttime = DateTime.Now;
            _context.Add(newOffsetWM);
            return RedirectToAction(nameof(Index));
        }
        // GET: MeasPoints/Edit/5
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (id == null)
            {
                return NotFound();
            }

            var measPoint = await _context.MeasPoints.FindAsync(id);
            if (measPoint == null)
            {
                return NotFound();
            }
            if (User.IsInRole("Admin"))
            {
                ViewData["LoggerId"] = new SelectList(_context.Loggers.Where(x => x.DivisionId.Equals(theuser.DivisionId) || x.Id.Equals(measPoint.LoggerId)).Include(x => x.Division), "Id", "LoggerNo");
                ViewData["MeasTypeId"] = new SelectList(_context.MeasTypes.Where(x => x.DivisionId.Equals(theuser.DivisionId) || x.Id.Equals(measPoint.MeasTypeId)), "Id", "Type");
                ViewData["ProjectId"] = await GetProjectList();
                ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => x.Project.Active.Equals(true)).Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Project.DivisionId.Equals(theuser.DivisionId)), "Id", "Name");
                ViewData["MonitorTypeId"] = new SelectList(_context.MonitorType.Where(x => x.DivisionId.Equals(theuser.DivisionId) || x.Id.Equals(measPoint.MonitorTypeId)), "Id", "MonitorTypeName");
                ViewData["Title"] = _localizer["Edit"];
                return View(measPoint);
            }
            ViewData["LoggerId"] = new SelectList(_context.Loggers.Include(x => x.Division).Where(x => x.Division.Id.Equals(theuser.DivisionId)), "Id", "LoggerNo");
            ViewData["MeasTypeId"] = new SelectList(_context.MeasTypes.Where(x => x.DivisionId.Equals(theuser.DivisionId) || x.Id.Equals(measPoint.MeasTypeId)), "Id", "Type");
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => x.Project.Active.Equals(true)).Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Project.DivisionId.Equals(theuser.DivisionId)), "Id", "Name");
            ViewData["MonitorTypeId"] = new SelectList(_context.MonitorType.Where(x => x.DivisionId.Equals(theuser.DivisionId) || x.Id.Equals(measPoint.MonitorTypeId)), "Id", "MonitorTypeName");
            ViewData["Title"] = _localizer["Edit"];
            return View(measPoint);
        }

        // POST: MeasPoints/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> Edit(int id, int? theloggerid, [Bind("Id,Name,Offset,Coordx,Coordy,Coordz,MonitorTypeId,ProjectId,MeasTypeId,LaborId,LoggerId,ScadaAddress,LoggerActive,SubProjectId,DGUnr,Bottom,Coordinates_Are_AsBuilt,ExternalCoordx,ExternalCoordy,ExternalCoordz")] MeasPoint measPoint)
        {
            if (id != measPoint.Id)
            {
                return NotFound();
            }
            var beforeedit = await _context.MeasPoints.AsNoTracking().SingleOrDefaultAsync(x => x.Id.Equals(id));
            if (ModelState.IsValid)
            {
                try
                {
                    if (measPoint.Coordx != beforeedit.Coordx || measPoint.Coordy != beforeedit.Coordy)
                    {
                        //
                        var proj = await _context.Projects.Include(x => x.CoordSystem).Where(x => x.Id.Equals(measPoint.ProjectId)).SingleOrDefaultAsync();
                        LatLngUTMConverter ltUTMconv = new LatLngUTMConverter("WGS 84");
                        if (proj.CoordSystem.system != "LatLong")
                        {
                            if ((measPoint.Lati == null && measPoint.Longi == null) || (measPoint.Lati == 0 && measPoint.Longi == 0))
                            {
                                if (proj.CoordSystem.system == "UTM32N")
                                {
                                    if ((measPoint.Lati == null || measPoint.Longi == null) && !(measPoint.Coordx == 0 || measPoint.Coordy == 0))
                                    {
                                        LatLngUTMConverter.LatLng latlng = ltUTMconv.convertUtmToLatLng(measPoint.Coordx, measPoint.Coordy, 32, "N");
                                        measPoint.Lati = latlng.Lat;
                                        measPoint.Longi = latlng.Lng;
                                    }
                                }
                                else if (proj.CoordSystem.system == "UTM17N")
                                {
                                    if ((measPoint.Lati == null || measPoint.Longi == null) && !(measPoint.Coordx == 0 || measPoint.Coordy == 0))
                                    {
                                        LatLngUTMConverter.LatLng latlng = ltUTMconv.convertUtmToLatLng(measPoint.Coordx, measPoint.Coordy, 17, "N");
                                        measPoint.Lati = latlng.Lat;
                                        measPoint.Longi = latlng.Lng;
                                    }
                                }
                                else
                                {
                                    LatLngUTMConverter.LatLng latlng = await findUTMcoords(measPoint);
                                    measPoint.Lati = latlng.Lat;
                                    measPoint.Longi = latlng.Lng;
                                }
                            }

                        }
                        else
                        {
                            if ((measPoint.Lati == null || measPoint.Longi == null) || (measPoint.Lati == 0 || measPoint.Longi == 0))
                            {
                                if (measPoint.Coordx != 0 && measPoint.Coordy != 0)
                                {
                                    measPoint.Lati = measPoint.Coordx;
                                    measPoint.Longi = measPoint.Coordy;
                                }
                            }
                        }
                        //
                        var othermeasp = await _context.MeasPoints.Where(x => x.Name.ToLower().StartsWith(measPoint.Name.ToLower()) && x.Id != measPoint.Id && x.ProjectId.Equals(measPoint.ProjectId)).ToListAsync();
                        foreach (MeasPoint mp in othermeasp)
                        {
                            mp.Coordx = measPoint.Coordx;
                            mp.Coordy = measPoint.Coordy;
                            mp.Coordz = measPoint.Coordz;
                            mp.Lati = measPoint.Lati;
                            mp.Longi = measPoint.Longi;
                            _context.Update(mp);
                            await _context.SaveChangesAsync();
                        }
                    }
                    if (measPoint.ExternalCoordx != beforeedit.ExternalCoordx || measPoint.ExternalCoordy != beforeedit.ExternalCoordy)
                    {
                       
                        var othermeasp = await _context.MeasPoints.Where(x => x.Name.ToLower().StartsWith(measPoint.Name.ToLower()) && x.Id != measPoint.Id && x.ProjectId.Equals(measPoint.ProjectId)).ToListAsync();
                        foreach (MeasPoint mp in othermeasp)
                        {
                            mp.ExternalCoordx = measPoint.ExternalCoordx;
                            mp.ExternalCoordy = measPoint.ExternalCoordy;
                            mp.ExternalCoordz = measPoint.ExternalCoordz;
                            
                            _context.Update(mp);
                            await _context.SaveChangesAsync();
                        }
                    }
                    if (theloggerid != measPoint.LoggerId && theloggerid != null)
                    {
                        var prevloggerchange = await _context.LoggerChanges.Where(x => x.LoggerId.Equals(theloggerid)).ToListAsync();
                        if (prevloggerchange.Count() > 0)
                        {
                            var measp = prevloggerchange.First().MeasPointId;
                            LoggerChange LC1 = new LoggerChange(Convert.ToInt32(theloggerid), measp, false, true);
                            _context.LoggerChanges.Add(LC1);
                            await _context.SaveChangesAsync();

                        }
                    }
                    if (measPoint.DGUnr != "" && (measPoint.Name.Contains("_1") || measPoint.Name.Contains("_2")))
                    {
                        if (measPoint.Name.Contains("_1"))
                        {
                            if (measPoint.DGUnr != null)
                            {
                                string othername = measPoint.Name.Replace("_1", "_2");
                                var othermp = await _context.MeasPoints.Where(x => x.Name.Equals(othername) && x.ProjectId.Equals(measPoint.ProjectId)).SingleOrDefaultAsync();
                                if(othermp != null) {
                                    othermp.DGUnr = measPoint.DGUnr;
                                    _context.Update(othermp);
                                }
                                othername = measPoint.Name.Replace("_1", "_3");
                                var otherothermp = await _context.MeasPoints.Where(x => x.Name.Equals(othername) && x.ProjectId.Equals(measPoint.ProjectId)).SingleOrDefaultAsync();
                                if (otherothermp != null)
                                {
                                    otherothermp.DGUnr = measPoint.DGUnr;
                                    _context.Update(otherothermp);
                                }
                            }
                        }
                        else
                        {
                            if(measPoint.DGUnr != null)
                            {
                                string othername = measPoint.Name.Replace("_2", "_1");
                                var othermp = await _context.MeasPoints.Where(x => x.Name.Equals(othername) && x.ProjectId.Equals(measPoint.ProjectId)).SingleOrDefaultAsync();
                                if (othermp != null)
                                {
                                    othermp.DGUnr = measPoint.DGUnr;
                                    _context.Update(othermp);
                                }
                                othername = measPoint.Name.Replace("_2", "_3");
                                var otherothermp = await _context.MeasPoints.Where(x => x.Name.Equals(othername) && x.ProjectId.Equals(measPoint.ProjectId)).SingleOrDefaultAsync();
                                if (otherothermp != null)
                                {
                                    otherothermp.DGUnr = measPoint.DGUnr;
                                    
                                    _context.Update(otherothermp);
                                }
                            }
                        }
                    }
                    _context.Update(measPoint);
                    await _context.SaveChangesAsync();
                    if (measPoint.LoggerId != null)
                    {
                        var changes = await _context.LoggerChanges.Where(x => x.LoggerId.Equals(measPoint.LoggerId) && x.MeasPointId.Equals(id) && x.LoggerAdded.Equals(true)).ToListAsync();
                        if (changes != null)
                        {
                            if (changes.Count() < 1)
                            {
                                LoggerChange LC = new LoggerChange(Convert.ToInt32(measPoint.LoggerId), id, true, false);
                                _context.LoggerChanges.Add(LC);
                                await _context.SaveChangesAsync();
                                var previousInstall = await _context.LoggerChanges.Where(x => x.LoggerId.Equals(measPoint.LoggerId) && x.MeasPointId != id && x.LoggerRemoved.Equals(false)).OrderByDescending(x => x.When).ToListAsync();
                                if (previousInstall != null)
                                {
                                    if (previousInstall.Count > 0)
                                    {
                                        var lastInstall = previousInstall.First();
                                        LoggerChange LC2 = new LoggerChange(lastInstall.LoggerId, lastInstall.MeasPointId, false, true);
                                        _context.LoggerChanges.Add(LC2);
                                        await _context.SaveChangesAsync();
                                    }
                                }

                            }

                        }
                    }
                    if (beforeedit.Offset != measPoint.Offset)
                    {
                        var oldOffsets = await _context.Offsets.Where(x => x.MeasPointId.Equals(id)).OrderByDescending(x => x.starttime).ToListAsync();
                        if (oldOffsets.Count() == 0)
                        {
                            // no old offsets , create new
                            Offset newOffset = new Offset();
                            newOffset.MeasPointId = measPoint.Id;
                            newOffset.offset = measPoint.Offset;
                            newOffset.starttime = DateTime.Now;
                            _context.Add(newOffset);
                            await _context.SaveChangesAsync();
                        }
                        else if (oldOffsets.Count() == 1)
                        {
                            if (beforeedit.Offset < 0.01 && (beforeedit.Offset > -0.01))
                            {
                                //update existing 0-offset.
                                oldOffsets.First().offset = measPoint.Offset;
                                _context.Update(oldOffsets.First());
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                Offset newOffset = new Offset();
                                newOffset.MeasPointId = measPoint.Id;
                                newOffset.offset = measPoint.Offset;
                                newOffset.starttime = DateTime.Now;
                                _context.Add(newOffset);
                                await _context.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            Offset newOffset = new Offset();
                            newOffset.MeasPointId = measPoint.Id;
                            newOffset.offset = measPoint.Offset;
                            newOffset.starttime = DateTime.Now;
                            _context.Add(newOffset);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MeasPointExists(measPoint.Id))
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
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["LoggerId"] = new SelectList(_context.Loggers.Include(x => x.Division), "Id", "LoggerNo");
                ViewData["MeasTypeId"] = new SelectList(_context.MeasTypes.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Type");
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.Active.Equals(true)).Include(x => x.Division), "Id", "Name");
                ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => x.Project.Active.Equals(true)).Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Project.DivisionId.Equals(theuser.DivisionId)), "Id", "Name");
                ViewData["MonitorTypeId"] = new SelectList(_context.MonitorType.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "MonitorTypeName");
                ViewData["Title"] = _localizer["Create"];
                return View(measPoint);
            }
            ViewData["LoggerId"] = new SelectList(_context.Loggers.Include(x => x.Division).Where(x => x.Division.Id.Equals(theuser.DivisionId)), "Id", "LoggerNo");
            ViewData["MeasTypeId"] = new SelectList(_context.MeasTypes.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Type");
            ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.Active.Equals(true)).Include(x => x.Division).Where(x => x.Division.Id.Equals(theuser.DivisionId)), "Id", "Name");
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => x.Project.Active.Equals(true)).Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Project.DivisionId.Equals(theuser.DivisionId)), "Id", "Name");
            ViewData["MonitorTypeId"] = new SelectList(_context.MonitorType.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "MonitorTypeName");
            ViewData["Title"] = _localizer["Create"];
            return View(measPoint);
        }
        [HttpGet]
        public async Task<IActionResult> exportData()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.Active.Equals(true)).Include(x => x.Division).OrderBy(x => x.ProjectNr), "Id", "Name");
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.Active.Equals(true)).Include(x => x.Division).Where(x => x.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.ProjectNr), "Id", "Name");
            }
            List<SelectListItem> delimiters = new List<SelectListItem>();
            delimiters.Add(new SelectListItem() { Text = ";", Value = ";" });
            delimiters.Add(new SelectListItem() { Text = ",", Value = "," });
            ViewData["delimiters"] = new SelectList(delimiters, "Value", "Text");
            return View("Meas/exportData");
        }

        // GET: MeasPoints/Delete/5
        [Authorize(Roles = "Admin,DivisionAdmin,StorageManager,Member")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var measPoint = await _context.MeasPoints
                .Include(m => m.MeasType)
                .Include(m => m.Project)
                .Include(m => m.logger)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (measPoint == null)
            {
                return NotFound();
            }
            ViewData["Title"] = _localizer["Delete"];
            return View(measPoint);
        }

        // POST: MeasPoints/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DivisionAdmin,StorageManager,Member")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var measPoint = await _context.MeasPoints.FindAsync(id);
            _context.MeasPoints.Remove(measPoint);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MeasPointExists(int id)
        {
            return _context.MeasPoints.Any(e => e.Id == id);
        }
        [HttpGet]
        [Authorize(Roles = "Admin,DivisionAdmin,Guest")]
        public IActionResult ExpData()
        {
            return RedirectToAction("exportData", "Meas");

        }

        [HttpGet]
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> exportLabor()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.Active.Equals(true)).Include(x => x.Division).OrderBy(x => x.ProjectNr), "Id", "Name");
                ViewData["MonitorTypeId"] = new SelectList(_context.MonitorType.Include(x => x.Division), "Id", "MonitorTypeName");

            }
            else
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.Active.Equals(true)).Include(x => x.Division).Where(x => x.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.ProjectNr), "Id", "Name");
                ViewData["MonitorTypeId"] = new SelectList(_context.MonitorType.Include(x => x.Division).Where(x => x.DivisionId.Equals(user.DivisionId)), "Id", "MonitorTypeName");

            }
            List<SelectListItem> delimiters = new List<SelectListItem>();
            delimiters.Add(new SelectListItem() { Text = ";", Value = ";" });
            delimiters.Add(new SelectListItem() { Text = ",", Value = "," });
            ViewData["delimiters"] = new SelectList(delimiters, "Value", "Text");
            return View();
        }
        [HttpGet]
        [Authorize(Roles = "Admin,DivisionAdmin,Member")]
        public async Task<IActionResult> ExportDipList(string returnUrl = null)
        {
            ExportDipListsViewModel model = new ExportDipListsViewModel();
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.Active.Equals(true) && x.DivisionId.Equals(user.DivisionId)).Include(x => x.Division).Where(x => x.Active.Equals(true)).OrderBy(x => x.ProjectNr), "Id", "Name");
                ViewData["MonitorTypeId"] = new SelectList(_context.MonitorType.Include(x => x.Division), "Id", "MonitorTypeName");


            }
            else
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.Active.Equals(true)).Include(x => x.Division).Where(x => x.DivisionId.Equals(user.DivisionId) && x.Active.Equals(true)).OrderBy(x => x.ProjectNr), "Id", "Name");
                ViewData["MonitorTypeId"] = new SelectList(_context.MonitorType.Include(x => x.Division).Where(x => x.DivisionId.Equals(user.DivisionId)), "Id", "MonitorTypeName");
            }
            List<SelectListItem> delimiters = new List<SelectListItem>();
            delimiters.Add(new SelectListItem() { Text = ";", Value = ";" });
            delimiters.Add(new SelectListItem() { Text = ",", Value = "," });
            ViewData["delimiters"] = new SelectList(delimiters, "Value", "Text");
            ViewData["ReturnUrl"] = returnUrl;
            //var data = new List<MeasPoint>();
            //var OldMeas = new List<Meas>();
            //ViewBag.Measures = OldMeas;
            //ViewBag.Data = data;
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Admin,DivisionAdmin,Member")]
        public async Task<IActionResult> ExportDipList(ExportDipListsViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = "MeasPoints/ExportDipList";
            var user = await _userManager.GetUserAsync(User);
            List<MeasPoint> data = new List<MeasPoint>();
            if (User.IsInRole("Admin"))
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.Active.Equals(true) && x.DivisionId.Equals(user.DivisionId)).Include(x => x.Division).Where(x => x.Active.Equals(true)).OrderBy(x => x.ProjectNr), "Id", "Name");
                ViewData["MonitorTypeId"] = new SelectList(_context.MonitorType.Include(x => x.Division), "Id", "MonitorTypeName");


            }
            else
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.Active.Equals(true)).Include(x => x.Division).Where(x => x.DivisionId.Equals(user.DivisionId) && x.Active.Equals(true)).OrderBy(x => x.ProjectNr), "Id", "Name");
                ViewData["MonitorTypeId"] = new SelectList(_context.MonitorType.Include(x => x.Division).Where(x => x.DivisionId.Equals(user.DivisionId)), "Id", "MonitorTypeName");
            }
            if (model.SubProjectId != null)
            {
                if (model.AddFlowMeters.Equals(true))
                {
                    data = await _context.MeasPoints.Include(x => x.Offsets).Where(x => x.ToBeHidden.Equals(false) && x.SubProjectId.Equals(model.SubProjectId) && (x.MeasTypeId.Equals(1) || x.MeasTypeId.Equals(2)))
                    .Include(x => x.MonitorType).Include(x => x.MeasType).Include(x => x.Project).ThenInclude(x => x.Division)
                    .ToListAsync();
                }
                else
                {
                    data = await _context.MeasPoints.Include(x => x.Offsets).Where(x => x.ToBeHidden.Equals(false) && x.SubProjectId.Equals(model.SubProjectId) && x.MeasTypeId.Equals(1))
                    .Include(x => x.MonitorType).Include(x => x.MeasType).Include(x => x.Project).ThenInclude(x => x.Division)
                    .ToListAsync();
                }

            }
            else
            {
                if (model.AddFlowMeters.Equals(true))
                {
                    data = await _context.MeasPoints.Include(x => x.Offsets).Where(x => x.ToBeHidden.Equals(false) && x.ProjectId.Equals(model.ProjectId) && (x.MeasTypeId.Equals(1) || x.MeasTypeId.Equals(2)))
                        .Include(x => x.MonitorType).Include(x => x.MeasType).Include(x => x.Project).ThenInclude(x => x.Division)
                        .ToListAsync();
                }
                else
                {
                    data = await _context.MeasPoints.Include(x => x.Offsets).Where(x => x.ToBeHidden.Equals(false) && x.ProjectId.Equals(model.ProjectId) && x.MeasTypeId.Equals(1))
                        .Include(x => x.MonitorType).Include(x => x.MeasType).Include(x => x.Project).ThenInclude(x => x.Division)
                        .ToListAsync();
                }
            }
            if (!model.wholeSite)
            {
                data = data.Where(x => (x.MonitorTypeId.Equals(2)
                    || x.MonitorTypeId.Equals(1))).ToList();
            }
            if (model.searchfield != "All")
            {
                data = data.Where(x => x.Name.ToLower().Contains(model.searchfield.ToLower())).ToList();
            }
            if (!User.IsInRole("Admin"))
            {
                data = data.Where(x => x.Project.DivisionId.Equals(user.DivisionId)).ToList();
            }
            data = data.Where(x => x.ToBeHidden.Equals(false)).OrderBy(x => x.ProjectId).ThenBy(x => x.SubProjectId).ThenBy(x => x.Name).ToList();
            List<DipListVM> alldata = new List<DipListVM>();
            foreach (MeasPoint mp in data)
            {
                DipListVM vm = new DipListVM();
                vm.MeasPoint = mp;
                var lastDip = await _context.Measures.Where(x => x.MeasPointId.Equals(mp.Id)).OrderByDescending(x => x.When).FirstOrDefaultAsync();
                vm.LastMeas = lastDip;
                vm.Offset = mp.Offsets.Where(x => x.MeasPointId.Equals(mp.Id)).OrderByDescending(x => x.starttime).FirstOrDefault();
                if (vm.Offset == null)
                {
                    vm.Offset = new Offset { MeasPointId = mp.Id, offset = 0.0, starttime = DateTime.Now, };
                }

                alldata.Add(vm);
            }
            ExportDipListsViewModel modelout = new ExportDipListsViewModel();
            modelout.DipList = alldata;
            modelout.ProjectId = model.ProjectId;
            modelout.SubProjectId = model.SubProjectId;
            modelout.searchfield = model.searchfield;
            modelout.wholeSite = model.wholeSite;
            return View(modelout);

        }
        [HttpPost]
        [Authorize(Roles = "Admin,DivisionAdmin,Member")]
        public async Task<IActionResult> ExportDipList2(ExportDipListsViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            List<MeasPoint> data = new List<MeasPoint>();
            if (model.wholeSite)
            {
                if (model.searchfield != "All")
                {
                    if (User.IsInRole("Admin"))
                    {
                        data = await _context.MeasPoints.Include(x => x.MonitorType).Include(x => x.MeasType).Include(x => x.Project).ThenInclude(x => x.Division)
                    .Where(x => x.MeasTypeId.Equals(1) && x.ProjectId.Equals(model.ProjectId) && x.Name.ToLower().Contains(model.searchfield.ToLower())).OrderBy(x => x.MonitorTypeId).ThenBy(x => x.Name).ToListAsync();
                    }
                    else
                    {
                        data = await _context.MeasPoints.Include(x => x.MonitorType).Include(x => x.MeasType).Include(x => x.Project).ThenInclude(x => x.Division)
                    .Where(x => x.Project.DivisionId.Equals(user.DivisionId)
                    && x.MeasTypeId.Equals(1)
                    && x.ProjectId.Equals(model.ProjectId)
                    && x.Name.ToLower().Contains(model.searchfield.ToLower())).OrderBy(x => x.MonitorTypeId).ThenBy(x => x.Name).ToListAsync();
                    }

                }
                else
                {
                    if (User.IsInRole("Admin"))
                    {
                        data = await _context.MeasPoints.Include(x => x.MonitorType).Include(x => x.MeasType).Include(x => x.Project)
                                            .Where(x => ((x.MeasTypeId.Equals(1)) && x.ProjectId.Equals(model.ProjectId))).OrderBy(x => x.MonitorTypeId).ThenBy(x => x.Name).ToListAsync();
                    }
                    else
                    {
                        data = await _context.MeasPoints.Include(x => x.MonitorType).Include(x => x.MeasType).Include(x => x.Project)
                                            .Where(x => ((x.Project.DivisionId.Equals(user.DivisionId)
                                            && x.MeasTypeId.Equals(1))
                                            && x.ProjectId.Equals(model.ProjectId))).OrderBy(x => x.MonitorTypeId).ThenBy(x => x.Name).ToListAsync();
                    }

                }
            }
            else
            {
                if (model.searchfield != "All")
                {
                    if (User.IsInRole("Admin"))
                    {
                        data = await _context.MeasPoints.Include(x => x.MonitorType).Include(x => x.MeasType).Include(x => x.Project)
                    .Where(x => (x.MonitorTypeId.Equals(2)
                    || x.MonitorTypeId.Equals(1))
                    && x.MeasTypeId.Equals(1)
                    && x.ProjectId.Equals(model.ProjectId)
                    && x.Name.ToLower().Contains(model.searchfield.ToLower())).OrderBy(x => x.Name).ToListAsync();
                    }
                    else
                    {
                        data = await _context.MeasPoints.Include(x => x.MonitorType).Include(x => x.MeasType).Include(x => x.Project)
                        .Where(x => x.Project.DivisionId.Equals(user.DivisionId)
                        && (x.MonitorTypeId.Equals(2)
                        || x.MonitorTypeId.Equals(1))
                        && x.MeasTypeId.Equals(1)
                        && x.ProjectId.Equals(model.ProjectId)
                        && x.Name.ToLower().Contains(model.searchfield.ToLower())).OrderBy(x => x.Name).ToListAsync();
                    }
                }
                else
                {
                    if (User.IsInRole("Admin"))
                    {
                        data = await _context.MeasPoints.Include(x => x.MonitorType).Include(x => x.MeasType).Include(x => x.Project)
                                            .Where(x => (x.MonitorTypeId.Equals(1) || x.MonitorTypeId.Equals(2)) && x.MeasTypeId.Equals(1) && x.ProjectId.Equals(7)).OrderBy(x => x.Name).ToListAsync();
                    }
                    data = await _context.MeasPoints.Include(x => x.MonitorType).Include(x => x.MeasType).Include(x => x.Project)
                                            .Where(x => x.Project.DivisionId.Equals(user.DivisionId) && (x.MonitorTypeId.Equals(1) || x.MonitorTypeId.Equals(2)) && x.MeasTypeId.Equals(1) && x.ProjectId.Equals(model.ProjectId)).OrderBy(x => x.Name).ToListAsync();
                }
            }
            if (model.SubProjectId != null)
            {
                data = data.Where(x => x.SubProjectId.Equals(model.SubProjectId)).OrderBy(x => x.Name).ToList();
            }

            List<Meas> OldMeas = new List<Meas>();
            foreach (MeasPoint mp in data)
            {
                var lastDips = await _context.Measures.Where(x => x.MeasPointId.Equals(mp.Id)).OrderByDescending(x => x.When).ToListAsync();
                if (lastDips.Count() > 0)
                {
                    OldMeas.Add(lastDips.First());
                }
                else
                {
                    OldMeas.Add(null);
                }
            }
            if (User.IsInRole("Admin"))
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.Active.Equals(true)).Include(x => x.Division).OrderBy(x => x.ProjectNr), "Id", "Name");
                ViewData["MonitorTypeId"] = new SelectList(_context.MonitorType.Include(x => x.Division), "Id", "MonitorTypeName");

            }
            else
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.Active.Equals(true)).Include(x => x.Division).Where(x => x.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.ProjectNr), "Id", "Name");
                ViewData["MonitorTypeId"] = new SelectList(_context.MonitorType.Include(x => x.Division).Where(x => x.DivisionId.Equals(user.DivisionId)), "Id", "MonitorTypeName");

            }
            List<SelectListItem> delimiters = new List<SelectListItem>();
            delimiters.Add(new SelectListItem() { Text = ";", Value = ";" });
            delimiters.Add(new SelectListItem() { Text = ",", Value = "," });
            ViewData["delimiters"] = new SelectList(delimiters, "Value", "Text");
            ViewBag.Measures = OldMeas;
            ViewBag.Data = data;
            return View();
        }
        [HttpGet]
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> ExportMonitorPoints()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.Active.Equals(true))
                .Include(x => x.Division)
                .OrderBy(x => x.ProjectNr), "Id", "Name");
                ViewData["MonitorTypeId"] = new SelectList(_context.MonitorType
                    .Include(x => x.Division), "Id", "MonitorTypeName");
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.Active.Equals(true))
                .Include(x => x.Division)
                .Where(x => x.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.ProjectNr), "Id", "Name");
                ViewData["MonitorTypeId"] = new SelectList(_context.MonitorType
                    .Include(x => x.Division).Where(x => x.DivisionId.Equals(user.DivisionId)), "Id", "MonitorTypeName");
            }

            List<SelectListItem> delimiters = new List<SelectListItem>();
            delimiters.Add(new SelectListItem() { Text = ";", Value = ";" });
            delimiters.Add(new SelectListItem() { Text = ",", Value = "," });
            ViewData["delimiters"] = new SelectList(delimiters, "Value", "Text");
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin,DivisionAdmin")]
        [Route("/MeasPoints/ExportMonitorPoints/MonitorPoints.csv")]
        [Produces("text/csv")]
        public async Task<IActionResult> ExportMonitorPoints(ExportMonitorPointsViewModel model)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                List<MeasPoint> data = new List<MeasPoint>();
                if (model.projectNumber != null)
                {
                    if (model.searchfield != "" && model.searchfield != null)
                    {
                        data = await _context.MeasPoints.Include(x => x.Offsets).Include(x => x.MonitorType).Include(x => x.MeasType).Include(x => x.Project)
                        .Where(x => !x.MeasType.Type.ToLower().Equals("miscellaneous") && x.ProjectId.Equals(model.projectNumber) && x.Name.ToLower().Contains(model.searchfield.ToLower())).OrderBy(x => x.Name).ToListAsync();
                    }
                    else
                    {
                        data = await _context.MeasPoints.Include(x => x.Offsets).Include(x => x.MonitorType).Include(x => x.MeasType).Include(x => x.Project)
                                                .Where(x => !x.MeasType.Type.ToLower().Equals("miscellaneous") && x.ProjectId.Equals(model.projectNumber)).OrderBy(x => x.Name).ToListAsync();
                    }

                }
                else
                {
                    if (model.searchfield != "" && model.searchfield != null)
                    {
                        data = await _context.MeasPoints.Include(x => x.Offsets).Include(x => x.MonitorType).Include(x => x.MeasType).Include(x => x.Project)
                            .Where(x => !x.MeasType.Type.ToLower().Equals("miscellaneous") && x.Name.ToLower().Contains(model.searchfield.ToLower()))
                        .OrderBy(x => x.Name)
                    .ToListAsync();
                    }
                    else
                    {
                        data = await _context.MeasPoints.Where(x => !x.MeasType.Type.ToLower().Equals("miscellaneous")).Include(x => x.Offsets).Include(x => x.MonitorType).Include(x => x.MeasType).Include(x => x.Project)
                        .OrderBy(x => x.Name)
                    .ToListAsync();
                    }

                }
                List<string> headerrow;
                if (model.Kronos)
                {
                    if (model.projectNumber.Equals(14))
                    {
                        headerrow = new List<string>(new string[] { "Nr", "Typ", "Grupp", "Kategori", "Utförare", "SysPlan", "MapX-(east)", "MapY-(north)", "SysHöjd", "RÖK", "Dag", "Meta Spetsnivå", "Meta MPD" });
                    }
                    else
                    {
                        headerrow = new List<string>(new string[] { "MonitorPoint", "ExtMonitorPoint", "Type", "Easting", "Northing", "Elevation" });
                    }                
                }
                else
                {
                    headerrow = new List<string>(new string[] { "ExtMonitorPoint", "Type", "Easting", "Northing", "Elevation", "MonitorPoint" });
                }
                sb.AppendLine(string.Join(model.Delimiter, headerrow.ToArray()));
                List<string> fillerrow;
                if (model.Kronos && model.projectNumber.Equals(14))
                {
                    fillerrow = new List<string>(new string[] { "", "", "", "", "", "", "", "", "", "", "" });
                }
                else
                {
                    fillerrow = new List<string>(new string[] { "", "", "", "", "", "" });
                }
                if(model.LaborWerte == true)
                {
                    foreach (MeasPoint mp in data)
                    {
                        if (model.Kronos && !model.projectNumber.Equals(14))
                        {
                            fillerrow[0] = mp.Name + " M";
                            fillerrow[1] = "L" + mp.LaborId.ToString();
                            if (mp.MeasTypeId.Equals(1))
                            {
                                fillerrow[2] = "WL";
                            }
                            else if (mp.MeasTypeId.Equals(2))
                            {
                                fillerrow[2] = "WM";
                            }
                            else if (mp.MeasTypeId.Equals(3))
                            {
                                fillerrow[2] = "Cond";
                            }
                            else if (mp.MeasTypeId.Equals(4))
                            {
                                fillerrow[2] = "WF";
                            }
                            else if (mp.MeasTypeId.Equals(5))
                            {
                                fillerrow[2] = "TURB";
                            }
                            else if (mp.MeasTypeId.Equals(6))
                            {
                                fillerrow[2] = "PH";
                            }
                            else if (mp.MeasTypeId.Equals(7))
                            {
                                fillerrow[2] = "BAR";
                            }
                            else if (mp.MeasTypeId.Equals(8))
                            {
                                fillerrow[2] = "%";
                            }
                            else if (mp.MeasTypeId.Equals(9))
                            {
                                fillerrow[2] = "kW";
                            }
                            if(model.UseExternalCoords == true) {
                                fillerrow[3] = mp.ExternalCoordx.ToString();
                                fillerrow[4] = mp.ExternalCoordy.ToString();
                                
                            }
                            else
                            {
                                fillerrow[3] = mp.Coordx.ToString();
                                fillerrow[4] = mp.Coordy.ToString();
                            }
                            if (mp.Coordz != 0)
                            {
                                if(model.UseExternalCoords == true)
                                {
                                    fillerrow[5] = mp.ExternalCoordz.ToString();
                                }
                                else
                                {
                                    fillerrow[5] = mp.Coordz.ToString();
                                }
                               
                            }
                            else
                            {
                                fillerrow[5] = mp.Offset.ToString();
                            }

                        }
                        else if (model.Kronos && model.projectNumber.Equals(14))
                        {
                            //{ "Nr",	"Typ",	"Grupp", "Kategori", "Utförare", "SysPlan", "MapX-(east)", "MapY-(north)", "SysHöjd", "RÖK", "Dag", "Meta Spetsnivå", "Meta MPD" })
                            var theoffsets = mp.Offsets.OrderByDescending(x => x.starttime).FirstOrDefault();
                            fillerrow[0] = mp.Name;
                            fillerrow[1] = "G";
                            fillerrow[2] = "WaterLevel";
                            fillerrow[3] = "Kategori";
                            fillerrow[4] = "COWI";
                            fillerrow[5] = "SWEREF991630";
                            if(model.UseExternalCoords == true)
                            {
                                fillerrow[6] = mp.ExternalCoordx.ToString();
                                fillerrow[7] = mp.ExternalCoordy.ToString();
                                fillerrow[8] = mp.ExternalCoordz.ToString();
                            }
                            else
                            {
                                fillerrow[6] = mp.Coordx.ToString();
                                fillerrow[7] = mp.Coordy.ToString();
                                fillerrow[8] = mp.Coordz.ToString();
                            }
                           
                            fillerrow[9] = theoffsets.offset.ToString();
                            fillerrow[10] = "0";
                            fillerrow[11] = "12.123";
                            fillerrow[12] = "10.123";
                        }
                        else
                        {
                            fillerrow[0] = "L" + mp.LaborId.ToString();
                            if (mp.MeasTypeId.Equals(1))
                            {
                                fillerrow[1] = "WL";
                            }
                            else if (mp.MeasTypeId.Equals(2))
                            {
                                fillerrow[1] = "WM";
                            }
                            else if (mp.MeasTypeId.Equals(3))
                            {
                                fillerrow[1] = "Cond";
                            }
                            else if (mp.MeasTypeId.Equals(4))
                            {
                                fillerrow[1] = "WF";
                            }
                            else if (mp.MeasTypeId.Equals(5))
                            {
                                fillerrow[1] = "TURB";
                            }
                            else if (mp.MeasTypeId.Equals(6))
                            {
                                fillerrow[1] = "PH";
                            }
                            else if (mp.MeasTypeId.Equals(7))
                            {
                                fillerrow[1] = "BAR";
                            }
                            else if (mp.MeasTypeId.Equals(8))
                            {
                                fillerrow[1] = "%";
                            }
                            else if (mp.MeasTypeId.Equals(9))
                            {
                                fillerrow[1] = "kW";
                            }
                            if(model.UseExternalCoords == true)
                            {
                                fillerrow[2] = mp.ExternalCoordx.ToString();
                                fillerrow[3] = mp.ExternalCoordy.ToString();
                                if (mp.Coordz != 0)
                                {
                                    fillerrow[4] = mp.ExternalCoordz.ToString();
                                }
                                else
                                {
                                    fillerrow[4] = mp.Offset.ToString();
                                }
                            }
                            else
                            {
                                fillerrow[2] = mp.Coordx.ToString();
                                fillerrow[3] = mp.Coordy.ToString();
                                if (mp.Coordz != 0)
                                {
                                    fillerrow[4] = mp.Coordz.ToString();
                                }
                                else
                                {
                                    fillerrow[4] = mp.Offset.ToString();
                                }
                            }
                           
                            fillerrow[5] = mp.Name;
                        }
                        sb.AppendLine(string.Join(model.Delimiter, fillerrow.ToArray()));
                    }
                }
                else
                {

                
                    foreach (MeasPoint mp in data.Where(x => x.ScadaAddress != null))
                    {
                        if (model.Kronos && !model.projectNumber.Equals(14))
                        {
                            fillerrow[0] = mp.Name;
                            fillerrow[1] = "A" + mp.ScadaAddress.ToString();
                            if (mp.MeasTypeId.Equals(1))
                            {
                                fillerrow[2] = "WL";
                            }
                            else if (mp.MeasTypeId.Equals(2))
                            {
                                fillerrow[2] = "WM";
                            }
                            else if (mp.MeasTypeId.Equals(3))
                            {
                                fillerrow[2] = "Cond";
                            }
                            else if (mp.MeasTypeId.Equals(4))
                            {
                                fillerrow[2] = "WF";
                            }
                            else if (mp.MeasTypeId.Equals(5))
                            {
                                fillerrow[2] = "TURB";
                            }
                            else if (mp.MeasTypeId.Equals(6))
                            {
                                fillerrow[2] = "PH";
                            }
                            else if (mp.MeasTypeId.Equals(7))
                            {
                                fillerrow[2] = "BAR";
                            }
                            else if (mp.MeasTypeId.Equals(8))
                            {
                                fillerrow[2] = "%";
                            }
                            else if (mp.MeasTypeId.Equals(9))
                            {
                                fillerrow[2] = "kW";
                            }
                            if(model.UseExternalCoords == true)
                            {
                                fillerrow[3] = mp.ExternalCoordx.ToString();
                                fillerrow[4] = mp.ExternalCoordy.ToString();
                                if (mp.Coordz != 0)
                                {
                                    fillerrow[5] = mp.ExternalCoordz.ToString();
                                }
                                else
                                {
                                    fillerrow[5] = mp.Offset.ToString();
                                }
                            }
                            else
                            {
                                fillerrow[3] = mp.Coordx.ToString();
                                fillerrow[4] = mp.Coordy.ToString();
                                if (mp.Coordz != 0)
                                {
                                    fillerrow[5] = mp.Coordz.ToString();
                                }
                                else
                                {
                                    fillerrow[5] = mp.Offset.ToString();
                                }
                            }
                            

                        }
                        else if (model.Kronos && model.projectNumber.Equals(14))
                        {
                            //{ "Nr",	"Typ",	"Grupp", "Kategori", "Utförare", "SysPlan", "MapX-(east)", "MapY-(north)", "SysHöjd", "RÖK", "Dag", "Meta Spetsnivå", "Meta MPD" })
                            var theoffsets = mp.Offsets.OrderByDescending(x => x.starttime).FirstOrDefault();
                            fillerrow[0] = mp.Name;
                            fillerrow[1] = "G";
                            fillerrow[2] = "WaterLevel";
                            fillerrow[3] = "Kategori";
                            fillerrow[4] = "COWI";
                            fillerrow[5] = "SWEREF991630";
                            if(model.UseExternalCoords == true)
                            {
                                fillerrow[6] = mp.ExternalCoordx.ToString();
                                fillerrow[7] = mp.ExternalCoordy.ToString();
                                fillerrow[8] = mp.ExternalCoordz.ToString();
                            }
                            else
                            {
                                fillerrow[6] = mp.Coordx.ToString();
                                fillerrow[7] = mp.Coordy.ToString();
                                fillerrow[8] = mp.Coordz.ToString();
                            }
                           
                            fillerrow[9] = theoffsets.offset.ToString();
                            fillerrow[10] = "0";
                            fillerrow[11] = "12.123";
                            fillerrow[12] = "10.123";
                        }
                        else
                        {
                            fillerrow[0] = "A" + mp.ScadaAddress.ToString();
                            if (mp.MeasTypeId.Equals(1))
                            {
                                fillerrow[1] = "WL";
                            }
                            else if (mp.MeasTypeId.Equals(2))
                            {
                                fillerrow[1] = "WM";
                            }
                            else if (mp.MeasTypeId.Equals(3))
                            {
                                fillerrow[1] = "Cond";
                            }
                            else if (mp.MeasTypeId.Equals(4))
                            {
                                fillerrow[1] = "WF";
                            }
                            else if (mp.MeasTypeId.Equals(5))
                            {
                                fillerrow[1] = "TURB";
                            }
                            else if (mp.MeasTypeId.Equals(6))
                            {
                                fillerrow[1] = "PH";
                            }
                            else if (mp.MeasTypeId.Equals(7))
                            {
                                fillerrow[1] = "BAR";
                            }
                            else if (mp.MeasTypeId.Equals(8))
                            {
                                fillerrow[1] = "%";
                            }
                            else if (mp.MeasTypeId.Equals(9))
                            {
                                fillerrow[1] = "kW";
                            }
                            if(model.UseExternalCoords == true)
                            {
                                fillerrow[2] = mp.ExternalCoordx.ToString();
                                fillerrow[3] = mp.ExternalCoordy.ToString();
                                if (mp.Coordz != 0)
                                {
                                    fillerrow[4] = mp.ExternalCoordz.ToString();
                                }
                                else
                                {
                                    fillerrow[4] = mp.Offset.ToString();
                                }
                            }
                            else
                            {
                                fillerrow[2] = mp.Coordx.ToString();
                                fillerrow[3] = mp.Coordy.ToString();
                                if (mp.Coordz != 0)
                                {
                                    fillerrow[4] = mp.Coordz.ToString();
                                }
                                else
                                {
                                    fillerrow[4] = mp.Offset.ToString();
                                }
                            }
                           
                            fillerrow[5] = mp.Name;
                        }
                        sb.AppendLine(string.Join(model.Delimiter, fillerrow.ToArray()));
                    }
                }

                return File(System.Text.Encoding.ASCII.GetBytes(sb.ToString()), "text/csv", "MonitorPoints.csv");

            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpGet]
        public async Task<IActionResult> UploadLaborIDs()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.DivisionId.Equals(user.DivisionId)), "Id", "Name");
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadLaborIDs(int ProjectId, int? SubProjectId, IFormFile files)
        {
            var user = await _userManager.GetUserAsync(User);

            if (files != null)
            {
                string fileExtension = Path.GetExtension(files.FileName);

                //Validate uploaded file and return error.
                if (fileExtension != ".csv")
                {
                    return NotFound();
                }
                using (var sreader = new StreamReader(files.OpenReadStream()))
                {
                    string headerLine = sreader.ReadLine();
                    while (!sreader.EndOfStream)
                    {
                        string[] rows = sreader.ReadLine().Split(';');
                        var existingmp = await _context.MeasPoints.Where(x => x.LaborId.Equals(rows[0])).SingleOrDefaultAsync();
                        if (existingmp == null)
                        {
                            MeasPoint mp = new MeasPoint();
                            mp.Name = rows[1];
                            mp.ProjectId = ProjectId;
                            mp.SubProjectId = SubProjectId;
                            mp.LoggerActive = true;
                            mp.MonitorTypeId = 2;
                            mp.MeasTypeId = 1;
                            mp.Coordx = 0;
                            mp.Coordy = 0;
                            mp.Coordz = 0;
                            mp.Offset = 0.0;
                            mp.ToBeHidden = false;
                            mp.LaborId = Convert.ToInt32(rows[0]);
                            if (mp.Name.ToLower().Contains("watermeter") || mp.Name.ToLower().Contains("uhr") || mp.Name.ToLower().Contains("vandur") || mp.Name.ToLower().Contains("zaehler") || mp.Name.ToLower().Contains("zähler"))
                            {
                                mp.MeasTypeId = 2;
                                mp.MonitorTypeId = (from moni in _context.MonitorType
                                                    where moni.DivisionId.Equals(user.DivisionId)
                                                    && moni.MonitorTypeName.ToLower().Equals("´flow meter")
                                                    select moni.Id).FirstOrDefault();
                            }
                            else if (mp.Name.ToLower().Contains(" ph "))
                            {
                                mp.MeasTypeId = 6;
                                mp.MonitorTypeId = (from moni in _context.MonitorType
                                                    where moni.DivisionId.Equals(user.DivisionId)
                                                    && moni.MonitorTypeName.ToLower().Equals("manometer")
                                                    select moni.Id).FirstOrDefault();
                            }
                            else if (mp.Name.ToLower().Contains("turbid"))
                            {
                                mp.MeasTypeId = 5;
                                mp.MonitorTypeId = (from moni in _context.MonitorType
                                                    where moni.DivisionId.Equals(user.DivisionId)
                                                    && moni.MonitorTypeName.ToLower().Equals("manometer")
                                                    select moni.Id).FirstOrDefault();
                            }
                            else if (mp.Name.ToLower().Contains("salini") || mp.Name.ToLower().Contains("conductiv"))
                            {
                                mp.MeasTypeId = 3;
                                mp.MonitorTypeId = (from moni in _context.MonitorType
                                                    where moni.DivisionId.Equals(user.DivisionId)
                                                    && moni.MonitorTypeName.ToLower().Equals("manometer")
                                                    select moni.Id).FirstOrDefault();
                            }
                            else
                            {
                                mp.MeasTypeId = 1;
                                mp.MonitorTypeId = (from moni in _context.MonitorType
                                                    where moni.DivisionId.Equals(user.DivisionId)
                                                    && moni.MonitorTypeName.ToLower().Equals("monitoring well")
                                                    select moni.Id).FirstOrDefault();
                            }
                            _context.Add(mp);
                        }
                    }
                    await _context.SaveChangesAsync();
                }
            }
            return View(nameof(Index));
        }
        [HttpGet]
        public IActionResult UploadMeasPointsLetbanen()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadMeasPointsLetbanen(IFormFile files)
        {
            if (files != null)
            {
                string fileExtension = Path.GetExtension(files.FileName);

                //Validate uploaded file and return error.
                if (fileExtension != ".csv")
                {
                    return NotFound();
                }
                using (var sreader = new StreamReader(files.OpenReadStream()))
                {
                    while (!sreader.EndOfStream)
                    {


                        string[] rows = sreader.ReadLine().Split(';');
                        var existingmp = await _context.MeasPoints.Where(x => x.Name.Equals(rows[0]) && x.ProjectId.Equals(57)).SingleOrDefaultAsync();
                        if (existingmp == null)
                        {
                            MeasPoint mp = new MeasPoint();
                            mp.Name = rows[0];
                            mp.DGUnr = rows[2];
                            mp.ProjectId = 57;
                            if (rows[4] == "Herlev")
                            {
                                mp.SubProjectId = 215;
                            }
                            else
                            {
                                mp.SubProjectId = 214;
                            }
                            mp.LoggerActive = true;
                            mp.MonitorTypeId = 2;
                            mp.MeasTypeId = 1;
                            mp.Coordx = Convert.ToDouble(rows[10]);
                            mp.Coordy = Convert.ToDouble(rows[11]);
                            mp.Coordz = Convert.ToDouble(rows[9]);
                            mp.Offset = 0.0;
                            mp.ToBeHidden = false;
                            _context.Add(mp);
                        }
                    }
                    await _context.SaveChangesAsync();
                }
            }
            return View(nameof(Index));
        }
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,DivisionAdmin")]
        public async Task<IActionResult> UploadAsBuilt()
        {
            ViewData["ProjectId"] = await GetProjectList();
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,DivisionAdmin")]
        public async Task<IActionResult> UploadAsBuilt(int ProjectId, IFormFile files)
        {
            if (files != null)
            {
                string fileExtension = Path.GetExtension(files.FileName);

                //Validate uploaded file and return error.
                if (fileExtension != ".csv")
                {
                    return NotFound();
                }
                MeasPoint mp = new MeasPoint();
                LatLngUTMConverter ltUTMconv = new LatLngUTMConverter("WGS 84");
                using (var sreader = new StreamReader(files.OpenReadStream()))
                {
                    while (!sreader.EndOfStream)
                    {
                        string[] rows = sreader.ReadLine().Split(';');
                        
                        mp = await _context.MeasPoints.Where(x => x.Name.Equals(rows[0]) && x.ProjectId.Equals(ProjectId) && x.MeasTypeId.Equals(1)).SingleOrDefaultAsync();
                        if(mp != null)
                            {
                                if(rows[4] != "") 
                                {
                                    var prev_offs = await _context.Offsets.Where(x => x.MeasPointId.Equals(mp.Id)).ToListAsync();
                                    if(prev_offs.Count < 1) {
                                        Offset offset = new Offset();
                                        offset.MeasPointId = mp.Id;
                                        offset.offset = Convert.ToDouble(rows[4]);
                                        offset.starttime = DateTime.Now.AddDays(-300);
                                        _context.Add(offset);

                                    }
                                    mp.Coordinates_Are_AsBuilt = true;
                                }

                            }
                            if (mp != null)
                            {
                            try { 
                                if(rows[1] != "") { 
                                    mp.Coordx = Convert.ToDouble(rows[1]);
                                }
                                if(rows[2] != "") { 
                                    mp.Coordy = Convert.ToDouble(rows[2]);
                                }
                                if (rows[3] != "") { 
                                    mp.Coordz = Convert.ToDouble(rows[3]);
                                }
                                else
                                {
                                    mp.Coordz = 0;
                                }
                            if (rows[4] != "")
                            {
                                mp.Offset = Convert.ToDouble(rows[4]);
                            }
                            


                            if (rows[1] != "" && rows[2] != "")
                            {
                                LatLngUTMConverter.LatLng latlng = ltUTMconv.convertUtmToLatLng(mp.Coordx, mp.Coordy, 32, "N");
                           
                                mp.Lati = latlng.Lat;
                                mp.Longi = latlng.Lng;
                            }
                            }
                            catch
                            {
                                double nemlig = 2.0;
                            }
                            mp.DGUnr = rows[5];
                                
                                
                                _context.Update(mp);
                            }
                    }
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> UploadMeasPointsMisc()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.DivisionId.Equals(user.DivisionId)), "Id", "Name");
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadMeasPointsMisc(int ProjectId, int? SubProjectId, IFormFile files)
        {
            if (files != null)
            {
                string fileExtension = Path.GetExtension(files.FileName);

                //Validate uploaded file and return error.
                if (fileExtension != ".csv")
                {
                    return NotFound();
                }
                using (var sreader = new StreamReader(files.OpenReadStream()))
                {
                    sreader.ReadLine(); //Skip header.
                    while (!sreader.EndOfStream)
                    {
                        string[] rows = sreader.ReadLine().Split(';');
                        if (rows[14] == "")
                        {
                            var existingmp = await _context.MeasPoints.Where(x => x.Name.Equals(rows[0]) && x.ProjectId.Equals(ProjectId) && x.SubProjectId.Equals(SubProjectId)).SingleOrDefaultAsync();
                            if (existingmp == null)
                            {
                                MeasPoint mp = new MeasPoint();
                                mp.Name = rows[0];
                                mp.DGUnr = rows[1];
                                mp.ProjectId = ProjectId;
                                mp.SubProjectId = SubProjectId;
                                mp.LoggerActive = false;
                                mp.MonitorTypeId = 20;
                                mp.MeasTypeId = 1;
                                try
                                {
                                    mp.Coordx = Convert.ToDouble(rows[4]);
                                    mp.Coordy = Convert.ToDouble(rows[5]);
                                }
                                catch
                                {
                                    mp.Coordx = Convert.ToDouble(rows[4].Replace(",", "."));
                                    mp.Coordy = Convert.ToDouble(rows[5].Replace(",", "."));
                                }

                                mp.Coordz = 0.0;
                                mp.Offset = 0.0;
                                mp.ToBeHidden = false;
                                _context.Add(mp);
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("/MeasPoints/exportLabor/MeasPLabor.csv")]
        [Produces("text/csv")]
        public async Task<IActionResult> exportLabor(ExportLaborViewModel model)
        {
            var theuser = await _userManager.GetUserAsync(User);
            //try
            //{
                StringBuilder sb = new StringBuilder();
                List<MeasPoint> data = new List<MeasPoint>();
                if (model.projectNumber != null)
                {
                    data = await _context.MeasPoints.Include(x => x.MeasType).Include(x => x.Project)
                        .Where(x => x.ProjectId.Equals(model.projectNumber) && !x.MeasType.Type.ToLower().Equals("miscellaneous")).ToListAsync();
                }
                else
                {
                    data = await _context.MeasPoints.Where(x => x.Project.DivisionId.Equals(theuser.DivisionId) && !x.MeasType.Type.ToLower().Equals("miscellaneous")).Include(x => x.MeasType).Include(x => x.Project)
                    .ToListAsync();
                }

                if (model.AQ9.Equals(true))
                {

                
                List<string> headerrow = new List<string>(new string[] { "Nr.", "Bezeichnung", "Akt-Dim.", "2Std-Dim.", "Tag-Dim.", "Monat-Dim.", "Jahr-Dim.", "Nachkommastellen", "Wert-Skalierung", "Std-Tag Umrechnung", "untere Grenze", "obere Grenze", "Ersatzwert", "Verarbeitungskennung", "Kenn-Nr.", "EKS-Nr.", "PV-Gruppe" });
                sb.AppendLine(string.Join(model.Delimiter, headerrow.ToArray()));

                List<string> fillerrow = new List<string>(new string[] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" });
                foreach (MeasPoint m in data)
                {
                    fillerrow[0] = m.LaborId.ToString();
                    fillerrow[1] = m.Name;
                    if (m.MeasTypeId.Equals(1))
                    {
                        fillerrow[2] = "mDVR90";
                        fillerrow[3] = "mDVR90";
                        fillerrow[4] = "mDVR90";
                        fillerrow[5] = "mDVR90";
                        fillerrow[6] = "mDVR90";
                        fillerrow[10] = "-10000";
                        fillerrow[11] = "10000";

                    }
                    else if (m.MeasTypeId.Equals(2))
                    {
                        fillerrow[2] = "m3";
                        fillerrow[3] = "m3";
                        fillerrow[4] = "m3";
                        fillerrow[5] = "m3";
                        fillerrow[6] = "m3";
                        fillerrow[10] = "0";
                        fillerrow[11] = "1000000";
                    }
                    else if (m.MeasTypeId.Equals(3))
                    {
                        fillerrow[2] = "mS/cm";
                        fillerrow[3] = "mS/cm";
                        fillerrow[4] = "mS/cm";
                        fillerrow[5] = "mS/cm";
                        fillerrow[6] = "mS/cm";
                        fillerrow[10] = "0";
                        fillerrow[11] = "10000";
                    }
                    else if (m.MeasTypeId.Equals(4))
                    {
                        fillerrow[2] = "m3/h";
                        fillerrow[3] = "m3/h";
                        fillerrow[4] = "m3/h";
                        fillerrow[5] = "m3/h";
                        fillerrow[6] = "m3/h";
                        fillerrow[10] = "0";
                        fillerrow[11] = "10000";
                    }
                    else if (m.MeasTypeId.Equals(5))
                    {
                        fillerrow[2] = "mg/ml";
                        fillerrow[3] = "mg/ml";
                        fillerrow[4] = "mg/ml";
                        fillerrow[5] = "mg/ml";
                        fillerrow[6] = "mg/ml";
                        fillerrow[10] = "0";
                        fillerrow[11] = "1000000";
                    }
                    else if (m.MeasTypeId.Equals(6))
                    {
                        fillerrow[2] = "[H+]";
                        fillerrow[3] = "[H+]";
                        fillerrow[4] = "[H+]";
                        fillerrow[5] = "[H+]";
                        fillerrow[6] = "[H+]";
                        fillerrow[10] = "0";
                        fillerrow[11] = "14";
                    }
                    else if (m.MeasTypeId.Equals(7))
                    {
                        fillerrow[2] = "bar";
                        fillerrow[3] = "bar";
                        fillerrow[4] = "bar";
                        fillerrow[5] = "bar";
                        fillerrow[6] = "bar";
                        fillerrow[10] = "0";
                        fillerrow[11] = "1000000";
                    }
                    else if (m.MeasTypeId.Equals(8))
                    {
                        fillerrow[2] = "%";
                        fillerrow[3] = "%";
                        fillerrow[4] = "%";
                        fillerrow[5] = "%";
                        fillerrow[6] = "%";
                        fillerrow[10] = "0";
                        fillerrow[11] = "100";
                    }
                    else if (m.MeasTypeId.Equals(8))
                    {
                        fillerrow[2] = "°C";
                        fillerrow[3] = "°C";
                        fillerrow[4] = "°C";
                        fillerrow[5] = "°C";
                        fillerrow[6] = "°C";
                        fillerrow[10] = "-273";
                        fillerrow[11] = "1000000";
                    }
                    else if (m.MeasTypeId.Equals(9))
                    {
                        fillerrow[2] = "kW";
                        fillerrow[3] = "kW";
                        fillerrow[4] = "kW";
                        fillerrow[5] = "kW";
                        fillerrow[6] = "kW";
                        fillerrow[10] = "0";
                        fillerrow[11] = "1000000000";
                    }
                    else
                    {
                        fillerrow[2] = "";
                        fillerrow[3] = "";
                        fillerrow[4] = "";
                        fillerrow[5] = "";
                        fillerrow[6] = "";
                        fillerrow[10] = "-1000000";
                        fillerrow[11] = "1000000";
                    }
                    fillerrow[7] = "2";
                    fillerrow[8] = "";
                    fillerrow[9] = "";
                    fillerrow[12] = "0";
                    fillerrow[13] = "M";
                    fillerrow[14] = "";
                    fillerrow[15] = m.Project.Abbreviation;
                    fillerrow[16] = "0";
                    sb.AppendLine(string.Join(model.Delimiter, fillerrow.ToArray()));
                }
                }
                else
                {
                    List<string> headerrow = new List<string>(new string[] { "Platzhalter (1=nicht importieren)","Nr.", "PV-Bereich", "Vater-Knoten", "Name", "EKS", "Akt-Dim.", "Std-Dim.", "Tag-Dim.", "Monat-Dim.", "Jahr-Dim.", "Berichtsverdichtung (K=Keine Verdichtung, M=Mittelwert, S=Summe)", "Nachkommastellen", "Ersatzwert", "Untergrenze (Bericht)", "Obergrenze (Bericht)", "Kennnummer", "Farbe" });
                    sb.AppendLine(string.Join(model.Delimiter, headerrow.ToArray()));

                    List<string> fillerrow = new List<string>(new string[] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" });
                    foreach (MeasPoint m in data)
                    {
                        fillerrow[0] = "";
                        fillerrow[1] = m.LaborId.ToString();
                        fillerrow[2] = "0";
                        fillerrow[3] = "0";
                        fillerrow[4] = m.Name;
                        fillerrow[5] = m.Project.Abbreviation;
                        fillerrow[11] = "M";
                        fillerrow[12] = "2";
                        fillerrow[13] = "0";
                        fillerrow[16] = "";
                        fillerrow[17] = "#00000000";
                        if (m.MeasTypeId.Equals(1))
                        {
                            fillerrow[6] = "mDVR90";
                            fillerrow[7] = "mDVR90";
                            fillerrow[8] = "mDVR90";
                            fillerrow[9] = "mDVR90";
                            fillerrow[10] = "mDVR90";
                            fillerrow[14] = "-100";
                            fillerrow[15] = "100";
                            
                            ;

                        }
                        else if (m.MeasTypeId.Equals(2))
                        {
                            fillerrow[6] = "m3";
                            fillerrow[7] = "m3";
                            fillerrow[8] = "m3";
                            fillerrow[9] = "m3";
                            fillerrow[10] = "m3";
                            fillerrow[14] = "0";
                            fillerrow[15] = "1000000";
                        }
                        else if (m.MeasTypeId.Equals(3))
                        {
                            fillerrow[6] = "mS/cm";
                            fillerrow[7] = "mS/cm";
                            fillerrow[8] = "mS/cm";
                            fillerrow[9] = "mS/cm";
                            fillerrow[10] = "mS/cm";
                            fillerrow[14] = "0";
                            fillerrow[15] = "10000";
                        }
                        else if (m.MeasTypeId.Equals(4))
                        {
                            fillerrow[6] = "m3/h";
                            fillerrow[7] = "m3/h";
                            fillerrow[8] = "m3/h";
                            fillerrow[9] = "m3/h";
                            fillerrow[10] = "m3/h";
                            fillerrow[14] = "0";
                            fillerrow[15] = "10000";
                        }
                        else if (m.MeasTypeId.Equals(5))
                        {
                            fillerrow[6] = "mg/ml";
                            fillerrow[7] = "mg/ml";
                            fillerrow[8] = "mg/ml";
                            fillerrow[9] = "mg/ml";
                            fillerrow[10] = "mg/ml";
                            fillerrow[14] = "0";
                            fillerrow[15] = "1000000";
                        }
                        else if (m.MeasTypeId.Equals(6))
                        {
                            fillerrow[6] = "[H+]";
                            fillerrow[7] = "[H+]";
                            fillerrow[8] = "[H+]";
                            fillerrow[9] = "[H+]";
                            fillerrow[10] = "[H+]";
                            fillerrow[14] = "0";
                            fillerrow[15] = "14";
                        }
                        else if (m.MeasTypeId.Equals(7))
                        {
                            fillerrow[6] = "bar";
                            fillerrow[7] = "bar";
                            fillerrow[8] = "bar";
                            fillerrow[9] = "bar";
                            fillerrow[10] = "bar";
                            fillerrow[14] = "0";
                            fillerrow[15] = "1000000";
                        }
                        else if (m.MeasTypeId.Equals(8))
                        {
                            fillerrow[6] = "%";
                            fillerrow[7] = "%";
                            fillerrow[8] = "%";
                            fillerrow[9] = "%";
                            fillerrow[10] = "%";
                            fillerrow[14] = "0";
                            fillerrow[15] = "100";
                        }
                        else if (m.MeasTypeId.Equals(8))
                        {
                            fillerrow[6] = "°C";
                            fillerrow[7] = "°C";
                            fillerrow[8] = "°C";
                            fillerrow[9] = "°C";
                            fillerrow[10] = "°C";
                            fillerrow[14] = "-273";
                            fillerrow[15] = "1000000";
                        }
                        else if (m.MeasTypeId.Equals(9))
                        {
                            fillerrow[6] = "kW";
                            fillerrow[7] = "kW";
                            fillerrow[8] = "kW";
                            fillerrow[9] = "kW";
                            fillerrow[10] = "kW";
                            fillerrow[14] = "0";
                            fillerrow[15] = "1000000000";
                        }
                        else
                        {
                            fillerrow[6] = "";
                            fillerrow[7] = "";
                            fillerrow[8] = "";
                            fillerrow[9] = "";
                            fillerrow[10] = "";
                            fillerrow[14] = "-1000000";
                            fillerrow[15] = "1000000";
                        }
                        //fillerrow[7] = "2";
                        //fillerrow[8] = "";
                        //fillerrow[9] = "";
                        //fillerrow[12] = "0";
                        //fillerrow[13] = "M";
                        //fillerrow[14] = "";
                        //fillerrow[15] = m.Project.Abbreviation;
                        //fillerrow[16] = "0";
                        sb.AppendLine(string.Join(model.Delimiter, fillerrow.ToArray()));
                    }
                }
                return File(System.Text.Encoding.ASCII.GetBytes(sb.ToString()), "text/csv", "MeasPLabor.csv");
            //}
            //catch
            //{
            //    return BadRequest();
            //}



        }
        [HttpGet]
        public async Task<IActionResult> ShowMap(int? filterchoice = null)
        {
            var user = await _userManager.GetUserAsync(User);
            List<MeasPoint> mps = new List<MeasPoint>();
            ViewData["ProjectId"] = await GetProjectList();
            if (User.IsInRole("Admin"))
            {
                if(filterchoice != null)
                {
                    mps = await _context.MeasPoints.Include(x => x.MonitorType).Include(x => x.MeasType).Include(x => x.Project).ThenInclude(x => x.CoordSystem)
                    .Where(x => x.MeasType.Type.ToLower().Equals("water level") && x.Project.DivisionId.Equals(user.DivisionId) && x.ProjectId.Equals(filterchoice))
                    .ToListAsync();
                }
                else
                {
                    mps = await _context.MeasPoints.Include(x => x.MonitorType).Include(x => x.MeasType).Include(x => x.Project).ThenInclude(x => x.CoordSystem)
                    .Where(x => x.MeasType.Type.ToLower().Equals("water level") && x.Project.DivisionId.Equals(user.DivisionId))
                    .ToListAsync();
                }
                
            }
            else if (User.IsInRole("Member") || User.IsInRole("DivisionAdmin"))
            {
                if(filterchoice != null)
                {
                    mps = await _context.MeasPoints.Include(x => x.MonitorType).Include(x => x.MeasType).Include(x => x.Project).ThenInclude(x => x.CoordSystem)
                  .Where(x => x.Project.DivisionId.Equals(user.DivisionId) && x.ToBeHidden.Equals(false) && x.MeasType.Type.ToLower().Equals("water level") && x.ProjectId.Equals(filterchoice)).ToListAsync();
                }
                else
                {
                    mps = await _context.MeasPoints.Include(x => x.MonitorType).Include(x => x.MeasType).Include(x => x.Project).ThenInclude(x => x.CoordSystem)
                  .Where(x => x.Project.DivisionId.Equals(user.DivisionId) && x.ToBeHidden.Equals(false) && x.MeasType.Type.ToLower().Equals("water level")).ToListAsync();
                }
               

            }
            else
            {
                if(filterchoice != null)
                {
                    mps = await (from mp in _context.MeasPoints.Include(x => x.MonitorType).Include(x => x.MeasType).Include(x => x.Project).ThenInclude(x => x.CoordSystem)
                                 join pu in _context.ProjectUsers on mp.ProjectId
                                 equals pu.projectId
                                 where pu.userId == user.Id && mp.Project.Active.Equals(true) && mp.ToBeHidden.Equals(false)
                                 && mp.Project.DivisionId.Equals(user.DivisionId) && mp.MeasType.Type.ToLower().Equals("water level")
                                 && mp.ProjectId.Equals(filterchoice)
                                 select mp).ToListAsync();
                }
                else
                {
                    mps = await (from mp in _context.MeasPoints.Include(x => x.MonitorType).Include(x => x.MeasType).Include(x => x.Project).ThenInclude(x => x.CoordSystem)
                                 join pu in _context.ProjectUsers on mp.ProjectId
                                 equals pu.projectId
                                 where pu.userId == user.Id && mp.Project.Active.Equals(true) && mp.ToBeHidden.Equals(false)
                                 && mp.Project.DivisionId.Equals(user.DivisionId) && mp.MeasType.Type.ToLower().Equals("water level")
                                 select mp).ToListAsync();
                }
                
            }
            LatLngUTMConverter ltUTMconv = new LatLngUTMConverter("WGS 84");
            foreach (MeasPoint mp in mps)
            {
                if (mp.Project.CoordSystem.system != "LatLong")
                {
                    if ((mp.Lati == null && mp.Longi == null) || (mp.Lati == 0 && mp.Longi == 0))
                    {
                        if (mp.Project.CoordSystem.system == "UTM32N")
                        {
                            if ((mp.Lati == null || mp.Longi == null) && !(mp.Coordx == 0 || mp.Coordy == 0))
                            {
                                LatLngUTMConverter.LatLng latlng = ltUTMconv.convertUtmToLatLng(mp.Coordx, mp.Coordy, 32, "N");
                                mp.Lati = latlng.Lat;
                                mp.Longi = latlng.Lng;
                                _context.Update(mp);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                try
                                {
                                    LatLngUTMConverter.LatLng latlng = await findUTMcoords(mp);
                                    mp.Lati = latlng.Lat;
                                    mp.Longi = latlng.Lng;
                                    if (mp.Lati != null && mp.Longi != null)
                                    {
                                        _context.Update(mp);
                                        await _context.SaveChangesAsync();
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        else if (mp.Project.CoordSystem.system == "UTM17N")
                        {
                            if ((mp.Lati == null || mp.Longi == null) && !(mp.Coordx == 0 || mp.Coordy == 0))
                            {
                                LatLngUTMConverter.LatLng latlng = ltUTMconv.convertUtmToLatLng(mp.Coordx, mp.Coordy, 17, "N");
                                mp.Lati = latlng.Lat;
                                mp.Longi = latlng.Lng;
                                _context.Update(mp);
                                await _context.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            try
                            {
                                LatLngUTMConverter.LatLng latlng = await findUTMcoords(mp);
                                mp.Lati = latlng.Lat;
                                mp.Longi = latlng.Lng;
                                if (mp.Lati != null && mp.Longi != null)
                                {
                                    _context.Update(mp);
                                    await _context.SaveChangesAsync();
                                }
                            }
                            catch
                            {

                            }
                        }
                    }

                }
                else
                {
                    if ((mp.Lati == null || mp.Longi == null) || (mp.Lati == 0 || mp.Longi == 0))
                    {
                        if (mp.Coordx != 0 && mp.Coordy != 0)
                        {
                            mp.Lati = mp.Coordx;
                            mp.Longi = mp.Coordy;
                            _context.Update(mp);
                            await _context.SaveChangesAsync();

                        }
                    }
                }
            }
            if(filterchoice != null) { 
                var mps_latinotnull = mps.Where(x => x.Lati != null).ToList();
                double thecount = mps_latinotnull.Count();
                double avg_lati = Convert.ToDouble(mps_latinotnull.Sum(x => x.Lati)) / thecount;
                double avg_longi = Convert.ToDouble(mps_latinotnull.Sum(x => x.Longi)) / thecount;
                ViewData["avg_lati"] = avg_lati;
                ViewData["avg_longi"] = avg_longi;
            }
            return View(mps.Where(x => x.Lati != null));
        }
        [AllowAnonymous]
        public async Task<IActionResult> DetailsTotal(int? id)
        {
            if (id != null)
            {
                LatLngUTMConverter ltUTMconv = new LatLngUTMConverter("WGS 84");
                var mp = await _context.MeasPoints.Include(x => x.Offsets).Include(x => x.Project).ThenInclude(x => x.CoordSystem).Include(x => x.MonitorType).Include(x => x.MeasType).Include(x => x.logger).Where(x => x.Id.Equals(id)).SingleOrDefaultAsync();
                if ((mp.Lati == null || mp.Longi == null) && mp.Project.CoordSystem.system == "UTM32N")
                {
                    LatLngUTMConverter.LatLng latlng = ltUTMconv.convertUtmToLatLng(mp.Coordx, mp.Coordy, 32, "N");
                    mp.Lati = latlng.Lat;
                    mp.Longi = latlng.Lng;
                    _context.Update(mp);
                    await _context.SaveChangesAsync();
                }
                else if ((mp.Lati == null || mp.Longi == null) && mp.Project.CoordSystem.system != "UTM32N")
                {
                    LatLngUTMConverter.LatLng latlng = await findUTMcoords(mp);
                    mp.Lati = latlng.Lat;
                    mp.Longi = latlng.Lng;
                    if (mp.Lati != null && mp.Longi != null)
                    {
                        _context.Update(mp);
                        await _context.SaveChangesAsync();
                    }

                }

                Offset ofs = mp.Offsets.OrderByDescending(x => x.starttime).FirstOrDefault();
                MeasPointViewModel MVM = new MeasPointViewModel(mp, ofs);
                var measpoints = await createMeasPointList(null);
                ViewData["MeasPoints"] = measpoints;
                return View(MVM);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
        public async Task<LatLngUTMConverter.LatLng> findUTMcoords(MeasPoint mp)
        {
            LatLngUTMConverter ltUTMconv = new LatLngUTMConverter("WGS 84");
            string line;
            bool NotFound = true;
            while (NotFound)
            {
                foreach (string filename in Directory.GetFiles(_environment.WebRootPath + "/UTMCoordinates/"))
                {
                    StreamReader file = new StreamReader(filename);

                    while ((line = file.ReadLine()) != null)
                    {
                        string[] linessplitted = line.Split(" ");
                        if (mp.Name.Split(" ")[0] == linessplitted[0] || mp.getBaseName == linessplitted[0])
                        {
                            LatLngUTMConverter.LatLng latlng;
                            if(linessplitted.Length >= 4) { 
                                if (linessplitted[3] != "32" && linessplitted[3] != "33")
                                {
                                    latlng = ltUTMconv.convertUtmToLatLng(Convert.ToDouble(linessplitted[1]), Convert.ToDouble(linessplitted[2]), 32, "N");
                                    mp.Lati = latlng.Lat;
                                    mp.Longi = latlng.Lng;
                                    mp.Coordx = Convert.ToDouble(linessplitted[1]);
                                    mp.Coordy = Convert.ToDouble(linessplitted[2]);
                                    _context.Update(mp);
                                    await _context.SaveChangesAsync();
                                }
                                else
                                {
                                    latlng = ltUTMconv.convertUtmToLatLng(Convert.ToDouble(linessplitted[1]), Convert.ToDouble(linessplitted[2]), Convert.ToInt32(linessplitted[3]), "N");
                                    mp.Lati = latlng.Lat;
                                    mp.Longi = latlng.Lng;
                                    mp.Coordx = Convert.ToDouble(linessplitted[1]);
                                    mp.Coordy = Convert.ToDouble(linessplitted[2]);
                                    _context.Update(mp);
                                    await _context.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                latlng = ltUTMconv.convertUtmToLatLng(Convert.ToDouble(linessplitted[1]), Convert.ToDouble(linessplitted[2]), 32, "N");
                                mp.Lati = latlng.Lat;
                                mp.Longi = latlng.Lng;
                                mp.Coordx = Convert.ToDouble(linessplitted[1]);
                                mp.Coordy = Convert.ToDouble(linessplitted[2]);
                                _context.Update(mp);
                                await _context.SaveChangesAsync();
                            }
                            return latlng;
                        }
                    }
                    file.Close();
                    if (!NotFound)
                    {
                        break;
                    }
                }
                NotFound = false;
            }
            return new LatLngUTMConverter.LatLng();
        }
        [HttpGet]
        public async Task<JsonResult> AutoComplete(string search)
        {
            var user = await _userManager.GetUserAsync(User);
            List<MeasPoint> results = new List<MeasPoint>();
            if (HttpContext.User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
            {
                var theuser = await _userManager.GetUserAsync(HttpContext.User);
                results = await (from mp in _context.MeasPoints.Include(m => m.MeasType).Include(m => m.Project).ThenInclude(x => x.Division).Include(x => x.logger).OrderBy(x => x.Project.Name).ThenBy(x => x.Name)
                                 join pu in _context.ProjectUsers on mp.ProjectId
                                 equals pu.projectId
                                 where pu.userId == theuser.Id && mp.Project.Division.Id.Equals(theuser.DivisionId) && (mp.Name.ToLower().Contains(search.ToLower()) || mp.Project.Name.ToLower().Contains(search.ToLower()))
                                 select mp).ToListAsync();
            }
            else
            {
                if (User.IsInRole("Admin"))
                {
                    results = _context.MeasPoints.Include(x => x.MeasType)
                                        .Include(x => x.logger).ThenInclude(x => x.Division)
                                        .Include(x => x.Project).ThenInclude(x => x.Division)
                                        .Where(x => x.Name.ToLower().Contains(search.ToLower()) || x.Project.Name.ToLower().Contains(search.ToLower())).ToList();
                }
                else
                {
                    results = _context.MeasPoints.Include(x => x.MeasType)
                    .Include(x => x.logger).ThenInclude(x => x.Division)
                    .Include(x => x.Project).ThenInclude(x => x.Division)
                    .Where(x => x.Project.Division.Id.Equals(user.DivisionId) && x.Name.ToLower().Contains(search.ToLower()) || x.Project.Name.ToLower().Contains(search.ToLower())).ToList();
                }

            }

            return Json(results.Select(m => new
            {
                id = m.Id,
                value = m.Name,
                label = m.Project.Name + '_' + m.Name
            }).OrderBy(x => x.label));
        }
        public async Task<IEnumerable<SelectListItem>> createFilterlist()
        {

            List<Project> filternames = new List<Project>();
            var theuser = await _userManager.GetUserAsync(HttpContext.User);
            if (HttpContext.User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
            {

                filternames = await (from pr in _context.Projects
                                     join pu in _context.ProjectUsers on pr.Id
                                     equals pu.projectId
                                     where pu.userId == theuser.Id && theuser.DivisionId.Equals(pr.DivisionId)
                                     select pr).OrderBy(x => x.Name).ToListAsync();
            }
            else
            {
                if (User.IsInRole("Admin"))
                {
                    filternames = await _context.Projects.Include(x => x.Division).OrderBy(x => x.Division.Name)
                        .Where(x => x.Active.Equals(true)).OrderBy(x => x.DivisionId).ThenBy(b => b.Name).ToListAsync();
                }
                else
                {
                    filternames = await _context.Projects.Where(x => x.DivisionId.Equals(theuser.DivisionId) && x.Active.Equals(true))
                .OrderBy(b => b.Name).ToListAsync();
                }

            }

            IEnumerable<SelectListItem> selList = from s in filternames
                                                  select new SelectListItem
                                                  {
                                                      Value = s.Id.ToString(),
                                                      Text = s.Name
                                                  };
            return selList;
        }
        [HttpPost]
        public async Task<IActionResult> Combsearch(string? searchstring, string filterchoice)
        {
            int f_c_converted;
            f_c_converted = Convert.ToInt32(filterchoice);
            var theuser = await _userManager.GetUserAsync(HttpContext.User);
            IEnumerable<SelectListItem> selList = await createFilterlist();
            ViewData["Filterchoices"] = new SelectList(selList, "Value", "Text");
            List<MeasPoint> mps = new List<MeasPoint>();
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(searchstring) && (string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
                {
                    return RedirectToAction(nameof(Index));
                }
                else if (string.IsNullOrEmpty(searchstring) && (!string.IsNullOrEmpty(filterchoice) || !filterchoice.Equals("All")))
                {

                    if (HttpContext.User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
                    {

                        mps = await (from mp in _context.MeasPoints
                                     .Include(m => m.MeasType).Include(m => m.Project).ThenInclude(x => x.Division).Include(x => x.logger).Include(x => x.MeasType).Include(x => x.SubProject)
                                     join pu in _context.ProjectUsers on mp.ProjectId
                                     equals pu.projectId
                                     where pu.userId == theuser.Id && mp.Project.DivisionId.Equals(theuser.DivisionId) && (mp.ProjectId.Equals(f_c_converted))
                                     select mp)
                                         .OrderBy(b => b.Project.Name).ThenBy(x => x.Name).ToListAsync();
                    }
                    else
                    {
                        if (User.IsInRole("Admin"))
                        {
                            mps = await _context.MeasPoints
                                   .Include(m => m.MeasType).Include(m => m.Project).ThenInclude(x => x.Division)
                                   .Include(x => x.logger)
                                   .Include(x => x.MeasType)
                                   .Include(x => x.SubProject)
                                   .Where(b => b.ProjectId == f_c_converted)
                                   .OrderBy(b => b.Project.Name).ThenBy(x => x.Name).ToListAsync();
                        }
                        else
                        {
                            mps = await _context.MeasPoints
                        .Include(m => m.MeasType).Include(m => m.Project).ThenInclude(x => x.Division).Include(x => x.logger).Include(x => x.MeasType).Include(x => x.SubProject)
                        .Where(b => b.ProjectId.Equals(f_c_converted) && b.Project.DivisionId.Equals(theuser.DivisionId))
                        .OrderBy(b => b.Project.Name).ThenBy(x => x.Name).ToListAsync();
                        }

                    }
                    ViewData["Title"] = _localizer["Index"];
                }
                else if (!string.IsNullOrEmpty(searchstring) && (string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
                {

                    if (HttpContext.User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
                    {

                        mps = await (from mp in _context.MeasPoints
                                     .Include(m => m.MeasType).Include(m => m.Project).Include(x => x.logger).Include(x => x.MeasType).Include(x => x.SubProject)
                                     join pu in _context.ProjectUsers on mp.ProjectId
                                     equals pu.projectId
                                     where pu.userId == theuser.Id && mp.Project.DivisionId.Equals(theuser.DivisionId) && (mp.Name.ToLower().Contains(searchstring.ToLower()))
                                     select mp)
                                         .OrderBy(b => b.Project.Name).ThenBy(x => x.Name).ToListAsync();
                    }
                    else
                    {
                        if (User.IsInRole("Admin"))
                        {
                            mps = await _context.MeasPoints
                        .Include(x => x.logger).Include(x => x.Project).ThenInclude(x => x.Division).Include(m => m.MeasType).Include(x => x.SubProject)
                        .Where(b => b.Name.ToLower().Contains(searchstring.ToLower())).OrderBy(b => b.Project.Name).ThenBy(x => x.Name).ToListAsync();
                        }
                        else
                        {
                            mps = await _context.MeasPoints
                        .Include(x => x.logger).Include(x => x.Project).ThenInclude(x => x.Division).Include(m => m.MeasType).Include(x => x.SubProject)
                        .Where(b => b.Project.DivisionId.Equals(theuser.DivisionId) && b.Name.ToLower().Contains(searchstring.ToLower())).OrderBy(b => b.Project.Name).ThenBy(x => x.Name).ToListAsync();
                        }

                    }
                    ViewData["Title"] = _localizer["Index"];

                }
                else
                {
                    if (HttpContext.User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
                    {
                        mps = await (from mp in _context.MeasPoints
                                     .Include(m => m.MeasType).Include(m => m.Project).Include(x => x.logger).Include(x => x.MeasType).Include(x => x.SubProject)
                                     join pu in _context.ProjectUsers on mp.ProjectId
                                     equals pu.projectId
                                     where pu.userId == theuser.Id && (mp.Name.ToLower().Contains(searchstring.ToLower()) && mp.ProjectId.Equals(f_c_converted))
                                     select mp)
                                         .OrderBy(b => b.Project.Name).ThenBy(x => x.Name).ToListAsync();
                    }
                    else
                    {
                        if (User.IsInRole("Admin"))
                        {
                            mps = await _context.MeasPoints
                        .Include(x => x.logger).Include(x => x.Project).Include(m => m.MeasType).Include(x => x.SubProject)
                        .Where(b => b.ProjectId.Equals(f_c_converted) && b.Name.ToLower().Contains(searchstring.ToLower())).OrderBy(b => b.Project.Name).ThenBy(x => x.Name).ToListAsync();
                        }
                        else
                        {
                            mps = await _context.MeasPoints
                        .Include(x => x.logger).Include(x => x.Project).Include(m => m.MeasType).Include(x => x.SubProject)
                        .Where(b => b.ProjectId.Equals(f_c_converted) && b.Project.DivisionId.Equals(theuser.DivisionId) && b.Name.ToLower().Contains(searchstring.ToLower())).OrderBy(b => b.Project.Name).ThenBy(x => x.Name).ToListAsync();
                        }

                    }
                    ViewData["Title"] = _localizer["Index"];

                }
                return View(nameof(Index), mps.Where(x => x.ToBeHidden.Equals(false)));
            }
            ViewData["Title"] = _localizer["Index"];
            return View(nameof(Index));
        }
        public async Task<double> getWatermeterTrueMeas(Meas m_1)
        {
            double thesum = 0;
            double lastmeas = 0;
            double lastoffset = 0;
            int lastoffsetId = 0;
            var mp = await _context.MeasPoints.Include(x => x.Offsets).Where(x => x.Id.Equals(m_1.MeasPointId)).SingleOrDefaultAsync();
            var measures = await _context.Measures.Where(x => x.MeasPointId.Equals(m_1.MeasPointId) && x.When <= m_1.When && x.TheMeasurement != null).OrderBy(x => x.When).ToListAsync();
            foreach (Meas m in measures)
            {
                Offset theoffset = new Offset();
                try
                {
                    theoffset = mp.Offsets.Where(x => x.starttime <= m.When).OrderByDescending(x => x.starttime).First();
                }
                catch
                {
                    try
                    {
                        theoffset = mp.Offsets.OrderBy(x => x.starttime).First();
                    }
                    catch
                    {
                        theoffset.offset = m.MeasPoint.Offset;
                        theoffset.starttime = DateTime.Now;
                    }

                }
                //if (lastoffset + 0.01 >= theoffset.offset && lastoffset - 0.01 < theoffset.offset)
                if (lastoffsetId.Equals(theoffset.Id))
                {
                    thesum += (Convert.ToDouble(m.TheMeasurement) - Convert.ToDouble(theoffset.offset) - lastmeas);
                    lastmeas = Convert.ToDouble(m.TheMeasurement) - Convert.ToDouble(theoffset.offset);
                }
                else
                {
                    thesum += (Convert.ToDouble(m.TheMeasurement) - Convert.ToDouble(theoffset.offset));
                    lastmeas = Convert.ToDouble(m.TheMeasurement) - Convert.ToDouble(theoffset.offset);
                }
                lastoffset = theoffset.offset;
                lastoffsetId = theoffset.Id;

            }
            return thesum;
        }
        public async Task<double> findRealMeas(Meas m)
        {
            Offset theoffset = new Offset();
            var theoffsets = await _context.Offsets.Where(x => x.MeasPointId.Equals(m.MeasPointId)).OrderByDescending(x => x.starttime).ToListAsync();
            if (theoffsets.Count == 0)
            {
                theoffset.offset = 0;
            }
            else if (theoffsets.Count == 1)
            {
                theoffset = theoffsets.First();
            }
            else
            {
                try
                {
                    theoffset = theoffsets.Where(x => x.starttime <= m.When).OrderByDescending(x => x.starttime).First();
                }
                catch
                {
                    try
                    {
                        theoffset = theoffsets.OrderBy(x => x.starttime).First();
                    }
                    catch
                    {
                        theoffset.offset = m.MeasPoint.Offset;
                        theoffset.starttime = DateTime.Now;
                    }

                }
            }
            double themeasurement;
            if (m.MeasPoint.MeasType.Type.ToLower().Equals("water level"))
            {
                themeasurement = theoffset.offset - Convert.ToDouble(m.TheMeasurement);

            }
            else if (m.MeasPoint.MeasType.Type.ToLower().Equals("water meter"))
            {
                themeasurement = await getWatermeterTrueMeas(m);
            }
            else
            {
                themeasurement = Convert.ToDouble(m.TheMeasurement);
            }
            return themeasurement;
        }
        public async Task<List<SelectListItem>> createMeasPointList(int? id)
        {
            var theuser = await _userManager.GetUserAsync(HttpContext.User);
            if (User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
            {
                var groups2 = await (from p in _context.Projects
                                     join pu in _context.ProjectUsers on p.Id equals pu.projectId
                                     where pu.userId == theuser.Id && theuser.DivisionId.Equals(p.DivisionId)
                                     select p).OrderBy(b => b.Name).ToListAsync();
                List<SelectListGroup> thegroups2 = new List<SelectListGroup>();
                List<SelectListItem> theList2 = new List<SelectListItem>();
                foreach (Project p in groups2)
                {
                    if (!thegroups2.Any(x => x.Name == p.Name))
                    {
                        thegroups2.Add(new SelectListGroup() { Name = p.Name });
                    }
                }
                var monpoints2 = await (from mp in _context.MeasPoints.Include(m => m.MeasType)
                                        .Include(m => m.Project).Include(x => x.logger)
                                        .OrderBy(x => x.Project.Name).ThenBy(x => x.Name)
                                        join pu in _context.ProjectUsers on mp.ProjectId
                                        equals pu.projectId
                                        where pu.userId == theuser.Id && theuser.DivisionId.Equals(mp.Project.DivisionId) && mp.ToBeHidden.Equals(false)
                                        select mp).OrderBy(b => b.Project.Name).ThenBy(x => x.Name).ToListAsync();

                foreach (MeasPoint m in monpoints2)
                {
                    theList2.Add(new SelectListItem()
                    {
                        Value = m.Id.ToString(),
                        Text = m.Name,
                        Group = thegroups2.Where(x => x.Name.Equals(m.Project.Name)).First()
                    });
                }
                return theList2;
            }
            try
            {
                List<Project> groups = new List<Project>();
                if (User.IsInRole("Admin"))
                {
                    groups = await _context.Projects.OrderBy(x => x.Name).ToListAsync();
                }
                else
                {
                    groups = await _context.Projects.Where(x => x.DivisionId.Equals(theuser.DivisionId)).OrderBy(x => x.Name).ToListAsync();
                }

                List<SelectListGroup> thegroups = new List<SelectListGroup>();
                List<SelectListItem> theList = new List<SelectListItem>();
                foreach (Project p in groups)
                {
                    if (!thegroups.Any(x => x.Name == p.Name))
                    {
                        thegroups.Add(new SelectListGroup() { Name = p.Name });
                    }
                }
                List<MeasPoint> monpoints = new List<MeasPoint>();
                if (User.IsInRole("Admin"))
                {
                    monpoints = await _context.MeasPoints.Include(x => x.Project)
                    .OrderBy(b => b.Project.Name).ThenBy(x => x.Name).ToListAsync();
                }
                else
                {
                    monpoints = await _context.MeasPoints.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(theuser.DivisionId))
                    .OrderBy(b => b.Project.Name).ThenBy(x => x.Name).ToListAsync();
                }
                if (id == null)
                {
                    foreach (MeasPoint m in monpoints)
                    {
                        theList.Add(new SelectListItem()
                        {
                            Value = m.Id.ToString(),
                            Text = m.Name,
                            Group = thegroups.Where(x => x.Name.Equals(m.Project.Name)).First()
                        });
                    }
                    return theList;
                }
                else
                {
                    var ppp = await _context.MeasPoints.FindAsync(id);
                    foreach (MeasPoint m in monpoints.Where(x => x.ProjectId.Equals(ppp.ProjectId)))
                    {
                        theList.Add(new SelectListItem()
                        {
                            Value = m.Id.ToString(),
                            Text = m.Name,
                            Group = thegroups.Where(x => x.Name.Equals(m.Project.Name)).First()
                        });
                    }
                    return theList;
                }
            }
            catch
            {
                List<SelectListItem> theList = new List<SelectListItem>();
                return theList;
            }


        }
        public async Task<List<SelectListItem>> createMeasPointList2()
        {
            var groups2 = await (from p in _context.Projects
                                 where p.Active.Equals(true)
                                 select p).OrderBy(b => b.Name).ToListAsync();
            List<SelectListGroup> thegroups2 = new List<SelectListGroup>();
            List<SelectListItem> theList2 = new List<SelectListItem>();
            foreach (Project p in groups2)
            {
                if (!thegroups2.Any(x => x.Name == p.Name))
                {
                    thegroups2.Add(new SelectListGroup() { Name = p.Name });
                }
            }
            var monpoints2 = await (from mp in _context.MeasPoints.Include(m => m.MeasType)
                                     .Include(m => m.Project).Include(x => x.logger)
                                     .Include(m => m.SubProject)
                                     .OrderBy(x => x.Project.Name).ThenBy(x => x.Name)
                                    where mp.Project.Active.Equals(true) && mp.MonitorTypeId.Equals(19) && mp.ToBeHidden.Equals(false)
                                    select mp).OrderBy(b => b.Project.Name).ThenBy(x => x.Name).ToListAsync();

            foreach (MeasPoint m in monpoints2)
            {
                SelectListItem newitem = new SelectListItem();
                newitem.Value = m.Id.ToString();
                newitem.Text = m.Name;
                newitem.Group = thegroups2.Where(x => x.Name.Equals(m.Project.Name)).First();
                theList2.Add(newitem);
            }
            return theList2;

        }
        public async Task<List<SelectListItem>> createWMMeasPointList()
        {
            var theuser = await _userManager.GetUserAsync(HttpContext.User);
            if (User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
            {
                var groups2 = await (from p in _context.Projects
                                     join pu in _context.ProjectUsers on p.Id equals pu.projectId
                                     where pu.userId == theuser.Id && theuser.DivisionId.Equals(p.DivisionId)
                                     select p).OrderBy(b => b.Name).ToListAsync();
                List<SelectListGroup> thegroups2 = new List<SelectListGroup>();
                List<SelectListItem> theList2 = new List<SelectListItem>();
                foreach (Project p in groups2)
                {
                    if (!thegroups2.Any(x => x.Name == p.Name))
                    {
                        thegroups2.Add(new SelectListGroup() { Name = p.Name });
                    }
                }
                var monpoints2 = await (from mp in _context.MeasPoints.Include(m => m.MeasType)
                                        .Include(m => m.Project).Include(x => x.logger)
                                        .OrderBy(x => x.Project.Name).ThenBy(x => x.Name)
                                        join pu in _context.ProjectUsers on mp.ProjectId
                                        equals pu.projectId
                                        where mp.ToBeHidden.Equals(false) && pu.userId == theuser.Id && theuser.DivisionId.Equals(mp.Project.DivisionId) && mp.MeasType.Type.ToLower().Equals("water meter")
                                        select mp).OrderBy(b => b.Project.Name).ThenBy(x => x.Name).ToListAsync();

                foreach (MeasPoint m in monpoints2)
                {
                    theList2.Add(new SelectListItem()
                    {
                        Value = m.Id.ToString(),
                        Text = m.Name,
                        Group = thegroups2.Where(x => x.Name.Equals(m.Project.Name)).First()
                    });
                }
                return theList2;
            }
            try
            {
                List<Project> groups = new List<Project>();
                if (User.IsInRole("Admin"))
                {
                    groups = await _context.Projects.OrderBy(x => x.Name).ToListAsync();
                }
                else
                {
                    groups = await _context.Projects.Where(x => x.DivisionId.Equals(theuser.DivisionId)).OrderBy(x => x.Name).ToListAsync();
                }

                List<SelectListGroup> thegroups = new List<SelectListGroup>();
                List<SelectListItem> theList = new List<SelectListItem>();
                foreach (Project p in groups)
                {
                    if (!thegroups.Any(x => x.Name == p.Name))
                    {
                        thegroups.Add(new SelectListGroup() { Name = p.Name });
                    }
                }
                List<MeasPoint> monpoints = new List<MeasPoint>();
                if (User.IsInRole("Admin"))
                {
                    monpoints = await _context.MeasPoints.Include(x => x.MeasType).Include(x => x.Project).Where(x => x.MeasType.Type.ToLower().Equals("water meter"))
                    .OrderBy(b => b.Project.Name).ThenBy(x => x.Name).ToListAsync();
                }
                else
                {
                    monpoints = await _context.MeasPoints.Include(x => x.MeasType).Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(theuser.DivisionId)
                      && x.MeasType.Type.ToLower().Equals("water meter"))
                    .OrderBy(b => b.Project.Name).ThenBy(x => x.Name).ToListAsync();
                }

                foreach (MeasPoint m in monpoints)
                {
                    theList.Add(new SelectListItem()
                    {
                        Value = m.Id.ToString(),
                        Text = m.Name,
                        Group = thegroups.Where(x => x.Name.Equals(m.Project.Name)).First()
                    });
                }
                return theList;
            }
            catch
            {
                List<SelectListItem> theList = new List<SelectListItem>();
                return theList;
            }


        }
        /*
        [Authorize(Roles = "Admin,Member,DivisionAdmin")]
        public async Task<ActionResult> GetQRDetails(int? Id)
        {
            if (Id != null)
            {
                string webRootPath = _environment.WebRootPath;
                var item = await _context.MeasPoints.FindAsync(Id);
                var routeUrl = Url.Action("DetailsTotal", "MeasPoints", new { id = Id });
                var absUrl = string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, routeUrl);
                //var uri = new Uri(absUrl, UriKind.Absolute);

                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(absUrl, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20);
                var bitmapBytes = BitmapToBytes(qrCodeImage); 
                return File(bitmapBytes, "image/jpeg"); 
            }
            return RedirectToAction(nameof(Index));
        }
        */
        private static byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
        [Authorize]
        public async Task<IActionResult> DetailsAll(string filterchoice, string searchstring)
        {
            List<MeasPoint> uniqueMeasPoints = new List<MeasPoint>();
            List<MeasPoint> allMeasPoints = new List<MeasPoint>();
            int f_c_converted = Convert.ToInt32(filterchoice);
            var user = await _userManager.GetUserAsync(User);
            if (string.IsNullOrEmpty(searchstring) && (string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
            {
                allMeasPoints = await _context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType)
                    .Where(x => (x.MonitorTypeId.Equals(1) || x.MonitorTypeId.Equals(2) || x.MonitorTypeId.Equals(5)) && x.MeasType.Type.Equals("Water Level") && user.DivisionId.Equals(x.Project.DivisionId))
                .OrderBy(x => x.Project.Name).ThenBy(x => x.getBaseName)

                .ToListAsync();
            }
            else if (string.IsNullOrEmpty(searchstring) && !(string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
            {
                allMeasPoints = await _context.MeasPoints
                    .Where(x => x.ProjectId.Equals(f_c_converted) &&
                    (x.MonitorTypeId.Equals(1) || x.MonitorTypeId.Equals(2) || x.MonitorTypeId.Equals(5)) && x.MeasType.Type.Equals("Water Level") && user.DivisionId.Equals(x.Project.DivisionId))
                   .Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType)
                .OrderBy(x => x.Project.Name).ThenBy(x => x.getBaseName)
                .ToListAsync();
            }
            else if (!string.IsNullOrEmpty(searchstring) && (string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
            {
                allMeasPoints = await _context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType)
                    .Where(x => x.Name.ToLower().Trim().Contains(searchstring.ToLower()) &&
                    (x.MonitorTypeId.Equals(1) || x.MonitorTypeId.Equals(2) || x.MonitorTypeId.Equals(5)) && x.MeasType.Type.Equals("Water Level") && user.DivisionId.Equals(x.Project.DivisionId))
                    .OrderBy(x => x.Project.Name).ThenBy(x => x.getBaseName)
                    .ToListAsync();
            }
            else
            {
                allMeasPoints = await _context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType)
                    .Where(x => x.Name.Trim().ToLower().Contains(searchstring.ToLower()) && x.ProjectId.Equals(f_c_converted) &&
                    (x.MonitorTypeId.Equals(1) || x.MonitorTypeId.Equals(2) || x.MonitorTypeId.Equals(5)) && x.MeasType.Type.Equals("Water Level") && user.DivisionId.Equals(x.Project.DivisionId))
                   .OrderBy(x => x.Project.Name).ThenBy(x => x.getBaseName)
                   .ToListAsync();
            }

            foreach (MeasPoint mp in allMeasPoints)
            {
                if (uniqueMeasPoints.FindIndex(x => x.getBaseName.Equals(mp.getBaseName)) < 0)
                {
                    uniqueMeasPoints.Add(mp);
                }
            }
            return View(uniqueMeasPoints);
        }
        [Authorize]
        public async Task<IActionResult> DipTagsAll(string filterchoice, string searchstring)
        {
            int f_c_converted;
            f_c_converted = Convert.ToInt32(filterchoice);
            List<MeasPoint> allMeasPoints = new List<MeasPoint>();
            var user = await _userManager.GetUserAsync(User);
            if (string.IsNullOrEmpty(searchstring) && (string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
            {
                allMeasPoints = await _context.MeasPoints.Where(x => user.DivisionId.Equals(x.Project.DivisionId)).Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType)
                .OrderBy(x => x.Project.Name).ThenBy(x => x.Name)
                .ToListAsync();
            }
            else if (string.IsNullOrEmpty(searchstring) && !(string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
            {
                allMeasPoints = await _context.MeasPoints
                    .Where(x => x.ProjectId.Equals(f_c_converted) && user.DivisionId.Equals(x.Project.DivisionId))
                   .Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType)
                .OrderBy(x => x.Project.Name).ThenBy(x => x.Name)
                .ToListAsync();
            }
            else if (!string.IsNullOrEmpty(searchstring) && (string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
            {
                allMeasPoints = await _context.MeasPoints.Where(x => user.DivisionId.Equals(x.Project.DivisionId)).Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType)
                    .Where(x => x.Name.ToLower().Trim().Contains(searchstring.ToLower()))
                    .OrderBy(x => x.Project.Name).ThenBy(x => x.Name)
                    .ToListAsync();
            }
            else
            {
                allMeasPoints = await _context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType)
                    .Where(x => x.Name.Trim().ToLower().Contains(searchstring.ToLower()) && x.ProjectId.Equals(f_c_converted) && user.DivisionId.Equals(x.Project.DivisionId))
                   .OrderBy(x => x.Project.Name).ThenBy(x => x.Name)
                   .ToListAsync();
            }
            allMeasPoints = allMeasPoints.Where(x => !x.MeasType.Type.ToLower().Equals("miscellaneous")).ToList();
            return View("DipTagsAll", allMeasPoints);
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,StorageManager")]
        [HttpGet]
        public async Task<IActionResult> ExternalWellTag()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["ProjectId"] = await GetProjectList3();
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
            return View();
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,StorageManager")]
        [HttpPost]
        public async Task<IActionResult> ExternalWellTag(ExternalWellTagVM model)
        {
            if (ModelState.IsValid)
            {
                

                if (model.logo != null)
                {
                    var directory = _environment.WebRootPath + "\\images\\ExternalWellTagPhotos\\";
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    model.photopath = Path.Combine(directory, model.logo.FileName);
                    if (!System.IO.File.Exists(model.photopath))
                    {
                        using (var stream = new FileStream(model.photopath, FileMode.Create))
                        {
                            await model.logo.CopyToAsync(stream);
                        }
                    }
                }
                if (model.PrintSecondaries == false)
                {
                    if (model.OnlyAsBuilt == true)
                    {
                        if (!string.IsNullOrEmpty(model.searchtext))
                        {
                            if (model.SubProjectId != null)
                            {
                                model.measpoints = await _context.MeasPoints.Include(x => x.MeasType)
                                .Include(x => x.Project)
                                .Include(x => x.SubProject)
                                .Where(x => x.MeasType.Type.ToLower().Equals("water level") && x.SubProjectId.Equals(model.SubProjectId) && x.ProjectId.Equals(model.ProjectId) && (x.Name.ToLower().Contains(model.searchtext.ToLower()) || x.DGUnr.ToLower().Contains(model.searchtext.ToLower())) && x.Coordinates_Are_AsBuilt.Equals(true)).OrderBy(x => x.getBaseName)
                                .GroupBy(x => x.getBaseName).Select(g => g.FirstOrDefault()).ToListAsync();
                            }
                            else
                            {
                                model.measpoints = await _context.MeasPoints
                                    .Include(x => x.MeasType)
                                    .Include(x => x.Project)
                                    .Where(x => x.MeasType.Type.ToLower().Equals("water level") && x.ProjectId.Equals(model.ProjectId) && (x.Name.ToLower().Contains(model.searchtext.ToLower()) || x.DGUnr.ToLower().Contains(model.searchtext.ToLower())) && x.Coordinates_Are_AsBuilt.Equals(true)).OrderBy(x => x.getBaseName)
                                    .GroupBy(x => x.getBaseName).Select(g => g.FirstOrDefault()).ToListAsync();
                            }
                        }
                        else
                        {
                            if (model.SubProjectId != null)
                            {
                                model.measpoints = await _context.MeasPoints
                                .Include(x => x.MeasType)
                                .Include(x => x.Project)
                                .Include(x => x.SubProject)
                                .Where(x => x.MeasType.Type.ToLower().Equals("water level") && x.SubProjectId.Equals(model.SubProjectId) && x.ProjectId.Equals(model.ProjectId) && x.Coordinates_Are_AsBuilt.Equals(true)).OrderBy(x => x.getBaseName)
                                .GroupBy(x => x.getBaseName).Select(g => g.FirstOrDefault()).ToListAsync();
                            }
                            else
                            {
                                model.measpoints = await _context.MeasPoints
                                .Include(x => x.MeasType)
                                .Include(x => x.Project)
                                .Where(x => x.MeasType.Type.ToLower().Equals("water level") && x.ProjectId.Equals(model.ProjectId) && x.Coordinates_Are_AsBuilt.Equals(true)).OrderBy(x => x.getBaseName).OrderBy(x => x.getBaseName)
                                .GroupBy(x => x.getBaseName).Select(g => g.FirstOrDefault()).ToListAsync();
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(model.searchtext))
                        {
                            if (model.SubProjectId != null)
                            {
                                model.measpoints = await _context.MeasPoints.Include(x => x.MeasType)
                                    .Include(x => x.Project)
                                    .Include(x => x.SubProject)
                                    .Where(x => x.MeasType.Type.ToLower().Equals("water level") && x.SubProjectId.Equals(model.SubProjectId) && x.ProjectId.Equals(model.ProjectId) && (x.Name.ToLower().Contains(model.searchtext.ToLower()) || x.DGUnr.ToLower().Contains(model.searchtext.ToLower()))).OrderBy(x => x.getBaseName)
                                    .GroupBy(x => x.getBaseName).Select(g => g.FirstOrDefault()).ToListAsync();
                            }
                            else
                            {
                                model.measpoints = await _context.MeasPoints
                                    .Include(x => x.MeasType)
                                    .Include(x => x.Project)
                                    .Where(x => x.MeasType.Type.ToLower().Equals("water level") && x.ProjectId.Equals(model.ProjectId) && (x.Name.ToLower().Contains(model.searchtext.ToLower()) || x.DGUnr.ToLower().Contains(model.searchtext.ToLower()))).OrderBy(x => x.getBaseName)
                                    .GroupBy(x => x.getBaseName).Select(g => g.FirstOrDefault()).ToListAsync();
                            }
                        }
                        else
                        {
                            if (model.SubProjectId != null)
                            {
                                model.measpoints = await _context.MeasPoints
                                    .Include(x => x.MeasType)
                                    .Include(x => x.Project)
                                    .Include(x => x.SubProject)
                                    .Where(x => x.MeasType.Type.ToLower().Equals("water level") && x.SubProjectId.Equals(model.SubProjectId) && x.ProjectId.Equals(model.ProjectId)).OrderBy(x => x.getBaseName)
                                    .GroupBy(x => x.getBaseName).Select(g => g.FirstOrDefault()).ToListAsync();
                            }
                            else
                            {
                                model.measpoints = await _context.MeasPoints
                                    .Include(x => x.MeasType)
                                    .Include(x => x.Project)
                                    .Where(x => x.MeasType.Type.ToLower().Equals("water level") && x.ProjectId.Equals(model.ProjectId)).OrderBy(x => x.getBaseName)
                                    .GroupBy(x => x.getBaseName).Select(g => g.FirstOrDefault()).ToListAsync();
                            }
                        }
                    }
                }
                else
                {
                    if (model.OnlyAsBuilt == true)
                    {
                        if (!string.IsNullOrEmpty(model.searchtext))
                        {
                            if (model.SubProjectId != null)
                            {
                                model.measpoints = await _context.MeasPoints.Include(x => x.MeasType)
                                .Include(x => x.Project)
                                .Include(x => x.SubProject)
                                .Where(x => x.MeasType.Type.ToLower().Equals("water level") && x.SubProjectId.Equals(model.SubProjectId) && x.ProjectId.Equals(model.ProjectId) && (x.Name.ToLower().Contains(model.searchtext.ToLower()) || x.DGUnr.ToLower().Contains(model.searchtext.ToLower())) && x.Coordinates_Are_AsBuilt.Equals(true)).OrderBy(x => x.Name)
                                .ToListAsync();
                            }
                            else
                            {
                                model.measpoints = await _context.MeasPoints
                                    .Include(x => x.MeasType)
                                    .Include(x => x.Project)
                                    .Where(x => x.MeasType.Type.ToLower().Equals("water level") && x.ProjectId.Equals(model.ProjectId) && (x.Name.ToLower().Contains(model.searchtext.ToLower()) || x.DGUnr.ToLower().Contains(model.searchtext.ToLower())) && x.Coordinates_Are_AsBuilt.Equals(true)).OrderBy(x => x.Name)
                                    .ToListAsync();
                            }
                        }
                        else
                        {
                            if (model.SubProjectId != null)
                            {
                                model.measpoints = await _context.MeasPoints
                                .Include(x => x.MeasType)
                                .Include(x => x.Project)
                                .Include(x => x.SubProject)
                                .Where(x => x.MeasType.Type.ToLower().Equals("water level") && x.SubProjectId.Equals(model.SubProjectId) && x.ProjectId.Equals(model.ProjectId) && x.Coordinates_Are_AsBuilt.Equals(true)).OrderBy(x => x.Name)
                                .ToListAsync();
                            }
                            else
                            {
                                model.measpoints = await _context.MeasPoints
                                .Include(x => x.MeasType)
                                .Include(x => x.Project)
                                .Where(x => x.MeasType.Type.ToLower().Equals("water level") && x.ProjectId.Equals(model.ProjectId) && x.Coordinates_Are_AsBuilt.Equals(true)).OrderBy(x => x.getBaseName).OrderBy(x => x.Name)
                                .ToListAsync();
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(model.searchtext))
                        {
                            if (model.SubProjectId != null)
                            {
                                model.measpoints = await _context.MeasPoints.Include(x => x.MeasType)
                                    .Include(x => x.Project)
                                    .Include(x => x.SubProject)
                                    .Where(x => x.MeasType.Type.ToLower().Equals("water level") && x.SubProjectId.Equals(model.SubProjectId) && x.ProjectId.Equals(model.ProjectId) && (x.Name.ToLower().Contains(model.searchtext.ToLower()) || x.DGUnr.ToLower().Contains(model.searchtext.ToLower()))).OrderBy(x => x.Name)
                                    .ToListAsync();
                            }
                            else
                            {
                                model.measpoints = await _context.MeasPoints
                                    .Include(x => x.MeasType)
                                    .Include(x => x.Project)
                                    .Where(x => x.MeasType.Type.ToLower().Equals("water level") && x.ProjectId.Equals(model.ProjectId) && (x.Name.ToLower().Contains(model.searchtext.ToLower()) || x.DGUnr.ToLower().Contains(model.searchtext.ToLower()))).OrderBy(x => x.Name)
                                    .ToListAsync();
                            }
                        }
                        else
                        {
                            if (model.SubProjectId != null)
                            {
                                model.measpoints = await _context.MeasPoints
                                    .Include(x => x.MeasType)
                                    .Include(x => x.Project)
                                    .Include(x => x.SubProject)
                                    .Where(x => x.MeasType.Type.ToLower().Equals("water level") && x.SubProjectId.Equals(model.SubProjectId) && x.ProjectId.Equals(model.ProjectId)).OrderBy(x => x.Name)
                                    .ToListAsync();
                            }
                            else
                            {
                                model.measpoints = await _context.MeasPoints
                                    .Include(x => x.MeasType)
                                    .Include(x => x.Project)
                                    .Where(x => x.MeasType.Type.ToLower().Equals("water level") && x.ProjectId.Equals(model.ProjectId)).OrderBy(x => x.Name)
                                    .ToListAsync();
                            }
                        }
                    }
                }
                return new ViewAsPdf("_ExternalWellTags", model);
            }
            var user = await _userManager.GetUserAsync(User);
            ViewData["ProjectId"] = await GetProjectList3();
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
            return View(model);
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,StorageManager")]
        public async Task<IActionResult> DipTagsAllPdf(string filterchoice, string searchstring)
        {
            int f_c_converted;
            f_c_converted = Convert.ToInt32(filterchoice);
            List<MeasPoint> allMeasPoints = new List<MeasPoint>();
            var user = await _userManager.GetUserAsync(User);
            if (string.IsNullOrEmpty(searchstring) && (string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
            {
                allMeasPoints = await _context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType)
                .OrderBy(x => x.Project.Name).ThenBy(x => x.Name)
                .ToListAsync();
            }
            else if (string.IsNullOrEmpty(searchstring) && !(string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
            {
                allMeasPoints = await _context.MeasPoints
                    .Where(x => x.ProjectId.Equals(f_c_converted))
                   .Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType)
                .OrderBy(x => x.Project.Name).ThenBy(x => x.Name)
                .ToListAsync();
            }
            else if (!string.IsNullOrEmpty(searchstring) && (string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
            {
                allMeasPoints = await _context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType)
                    .Where(x =>  x.Name.ToLower().Trim().Contains(searchstring.ToLower()))
                    .OrderBy(x => x.Project.Name).ThenBy(x => x.Name)
                    .ToListAsync();
            }
            else
            {
                allMeasPoints = await _context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType)
                    .Where(x => x.Name.Trim().ToLower().Contains(searchstring.ToLower()) && x.ProjectId.Equals(f_c_converted))
                   .OrderBy(x => x.Project.Name).ThenBy(x => x.Name)
                   .ToListAsync();
            }
            if (!User.IsInRole("Admin"))
            {
                allMeasPoints = allMeasPoints.Where(x => x.Project.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.Project.Name).ThenBy(x => x.Name).ToList();
            }
            allMeasPoints = allMeasPoints.Where(x => x.ToBeHidden.Equals(false) && !x.MeasType.Type.ToLower().Equals("miscellaneous")).ToList();
            return new ViewAsPdf("_DipTagsAll", allMeasPoints);
        }
        [AllowAnonymous]
        public async Task<IActionResult> DetailsTag(int? id)
        {
            if (id != null)
            {
                var user = await _userManager.GetUserAsync(User);
                var model = await _context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType).Where(x => x.Id.Equals(id) && user.DivisionId.Equals(x.Project.DivisionId)).SingleOrDefaultAsync();
                if (model == null)
                {
                    return NotFound();
                }
                return View("DetailsTag", model);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }

        }
        [Authorize]
        public async Task<IActionResult> DetailsPdf(int? id)
        {
            if (id != null)
            {
                var user = await _userManager.GetUserAsync(User);
                var model = await _context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType).Where(x => x.Id.Equals(id) && user.DivisionId.Equals(x.Project.DivisionId)).SingleOrDefaultAsync();
                if (model == null)
                {
                    return NotFound();
                }
                return new ViewAsPdf("_Details", model);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }

        }
        [Authorize]
        public async Task<IActionResult> DipTag(int? id)
        {
            if (id != null)
            {
                var user = await _userManager.GetUserAsync(User);
                if (!User.IsInRole("Admin"))
                {
                    var model = await _context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType).Where(x => x.Id.Equals(id) && user.DivisionId.Equals(x.Project.DivisionId)).SingleOrDefaultAsync();
                    if (model == null)
                    {
                        return NotFound();
                    }
                    return View("_DipTag", model);
                }
                else
                {
                    var model = await _context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType).SingleOrDefaultAsync(x => x.Id.Equals(id));
                    if (model == null)
                    {
                        return NotFound();
                    }
                    return View("_DipTag", model);
                }
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
        [Authorize]
        public async Task<IActionResult> DipTagPdf(int? id)
        {
            if (id != null)
            {
                var user = await _userManager.GetUserAsync(User);
                if (!User.IsInRole("Admin"))
                {
                    var model = await _context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType).Where(x => x.Id.Equals(id) && user.DivisionId.Equals(x.Project.DivisionId)).SingleOrDefaultAsync();
                    if (model == null)
                    {
                        return NotFound();
                    }
                    return new ViewAsPdf("_DipTag", model);
                }
                else
                {
                    var model = await _context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType).SingleOrDefaultAsync(x => x.Id.Equals(id));
                    if (model == null)
                    {
                        return NotFound();
                    }
                    return new ViewAsPdf("_DipTag", model);
                }

            }
            else
            {
                return RedirectToAction(nameof(Index));
            }

        }
        [Authorize]
        public async Task<IActionResult> DetailsAllPdf(string filterchoice, string searchstring, bool AllTypes = false)
        {
            List<MeasPoint> uniqueMeasPoints = new List<MeasPoint>();
            List<MeasPoint> allMeasPoints = new List<MeasPoint>();
            int f_c_converted = Convert.ToInt32(filterchoice);
            var user = await _userManager.GetUserAsync(User);
            if (string.IsNullOrEmpty(searchstring) && (string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
            {
                if (AllTypes)
                {
                    allMeasPoints = await _context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType)
                    .Where(x => x.MeasType.Type.Equals("Water Level") && user.DivisionId.Equals(x.Project.DivisionId))
                .OrderBy(x => x.Project.Name).ThenBy(x => x.getBaseName)
                .ToListAsync();
                }
                else
                {
                    allMeasPoints = await _context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType)
                    .Where(x => (x.MonitorTypeId.Equals(1) || x.MonitorTypeId.Equals(2)) && x.MeasType.Type.Equals("Water Level") && user.DivisionId.Equals(x.Project.DivisionId))
                .OrderBy(x => x.Project.Name).ThenBy(x => x.getBaseName)
                .ToListAsync();
                }

            }
            else if (string.IsNullOrEmpty(searchstring) && !(string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
            {
                if (AllTypes)
                {
                    allMeasPoints = await _context.MeasPoints
                    .Where(x => x.ProjectId.Equals(f_c_converted) && x.MeasType.Type.Equals("Water Level") && user.DivisionId.Equals(x.Project.DivisionId))
                   .Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType)
                .OrderBy(x => x.Project.Name).ThenBy(x => x.getBaseName)
                .ToListAsync();
                }
                else
                {
                    allMeasPoints = await _context.MeasPoints
                    .Where(x => x.ProjectId.Equals(f_c_converted) &&
                    (x.MonitorTypeId.Equals(1) || x.MonitorTypeId.Equals(2)) && x.MeasType.Type.Equals("Water Level") && user.DivisionId.Equals(x.Project.DivisionId))
                   .Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType)
                .OrderBy(x => x.Project.Name).ThenBy(x => x.getBaseName)
                .ToListAsync();
                }
            }
            else if (!string.IsNullOrEmpty(searchstring) && (string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
            {
                if (AllTypes)
                {
                    allMeasPoints = await _context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType)
                    .Where(x => x.Name.ToLower().Trim().Contains(searchstring.ToLower())
                    && x.MeasType.Type.Equals("Water Level") && user.DivisionId.Equals(x.Project.DivisionId))
                    .OrderBy(x => x.Project.Name).ThenBy(x => x.getBaseName)
                    .ToListAsync();
                }
                else
                {
                    allMeasPoints = await _context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType)
                    .Where(x => x.Name.ToLower().Trim().Contains(searchstring.ToLower()) &&
                    (x.MonitorTypeId.Equals(1) || x.MonitorTypeId.Equals(2)) && x.MeasType.Type.Equals("Water Level") && user.DivisionId.Equals(x.Project.DivisionId))
                    .OrderBy(x => x.Project.Name).ThenBy(x => x.getBaseName)
                    .ToListAsync();
                }

            }
            else
            {
                if (AllTypes)
                {
                    allMeasPoints = await _context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType)
                   .Where(x => x.Name.Trim().ToLower().Contains(searchstring.ToLower()) && x.ProjectId.Equals(f_c_converted) &&
                   x.MeasType.Type.Equals("Water Level") && user.DivisionId.Equals(x.Project.DivisionId))
                  .OrderBy(x => x.Project.Name).ThenBy(x => x.getBaseName)
                  .ToListAsync();
                }
                else
                {
                    allMeasPoints = await _context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType)
                   .Where(x => x.Name.Trim().ToLower().Contains(searchstring.ToLower()) && x.ProjectId.Equals(f_c_converted) &&
                   (x.MonitorTypeId.Equals(1) || x.MonitorTypeId.Equals(2)) && x.MeasType.Type.Equals("Water Level") && user.DivisionId.Equals(x.Project.DivisionId))
                  .OrderBy(x => x.Project.Name).ThenBy(x => x.getBaseName)
                  .ToListAsync();
                }

            }
            allMeasPoints = allMeasPoints.Where(x => !x.MeasType.Type.ToLower().Equals("miscellaneous")).ToList();
            foreach (MeasPoint mp in allMeasPoints)
            {
                if (uniqueMeasPoints.FindIndex(x => x.getBaseName.Equals(mp.getBaseName)) < 0)
                {
                    uniqueMeasPoints.Add(mp);
                }
            }
            return new ViewAsPdf("_DetailsAll", uniqueMeasPoints);
        }
        [Authorize]
        public async Task<IActionResult> DipTagAllPdf()
        {
            List<MeasPoint> uniqueMeasPoints = new List<MeasPoint>();
            var allMeasPoints = await _context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.MeasType)
                .Where(x => !x.MeasType.Type.ToLower().Equals("miscellaneous") && (x.MonitorTypeId.Equals(1) || x.MonitorTypeId.Equals(2) || x.MonitorTypeId.Equals(5)) && x.ProjectId.Equals(7) && x.MeasType.Type.Equals("Water Level"))
                .OrderBy(x => x.Project.Name).ThenBy(x => x.getBaseName)
                .ToListAsync();
            foreach (MeasPoint mp in allMeasPoints)
            {
                if (uniqueMeasPoints.FindIndex(x => x.getBaseName.Equals(mp.getBaseName)) < 0)
                {
                    uniqueMeasPoints.Add(mp);
                }
            }
            return new ViewAsPdf("_DetailsAll", uniqueMeasPoints);
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Member,DivisionAdmin,Guest")]
        public async Task<IActionResult> GetDocuments(int? id)
        {
            if (id != null)
            {
                var documents = await _context.Documents.Include(x => x.DocumentType).Where(x => x.MeasPointId.Equals(id)).ToListAsync();
                return PartialView("_DocumentInfo", documents);
            }
            else
            {
                List<Document> nodocu = new List<Document>();
                return PartialView("_DocumentInfo", nodocu);
            }
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GetDocuments2(int? id)
        {
            if (id != null)
            {
                var documents = await _context.Documents.Include(x => x.DocumentType).Where(x => x.MeasPointId.Equals(id) && x.DocumentType.Type.ToLower().Equals("alarmboxinfo")).ToListAsync();
                return PartialView("_AlarmPictures", documents);
            }
            else
            {
                List<Document> nodocu = new List<Document>();
                return PartialView("_DocumentInfo", nodocu);
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Member,DivisionAdmin,Guest")]
        public async Task<IActionResult> GetMeasPInRadius(double radi, double lati, double longi)
        {
            var user = await _userManager.GetUserAsync(User);
            List<MeasPoint> measpoints = new List<MeasPoint>();
            LatLngUTMConverter ltUTMconv = new LatLngUTMConverter("WGS 84");
            List<MeasPoint> measP = new List<MeasPoint>();
            if (User.IsInRole("Guest"))
            {
                measP = await (from mp in _context.MeasPoints.Include(m => m.MeasType).Include(x => x.Documents).ThenInclude(x => x.DocumentType)
                               .Include(x => x.Project).ThenInclude(x => x.CoordSystem)
                                .Include(x => x.MonitorType)
                               join pu in _context.ProjectUsers on mp.ProjectId
                               equals pu.projectId
                               join doc in _context.Documents on mp.Id equals doc.MeasPointId
                               where pu.userId == user.Id && mp.Project.Active.Equals(true) && mp.Project.DivisionId.Equals(user.DivisionId)
                               select mp).GroupBy(x => x.Id).Select(x => x.First()).OrderBy(x => x.Project.Name).ThenBy(x => x.Name)
                                  .ToListAsync();

            }
            else if (User.IsInRole("Admin"))
            {
                measP = await (from mp in _context.MeasPoints
                               .Include(x => x.Documents).ThenInclude(x => x.DocumentType)
                               .Include(x => x.Project).ThenInclude(x => x.CoordSystem)
                               .Include(x => x.MonitorType)
                               join doc in _context.Documents
                                on mp.Id equals doc.MeasPointId
                               select mp).GroupBy(x => x.Id).Select(x => x.First()).OrderBy(x => x.Project.Name).ThenBy(x => x.Name)
                                            .ToListAsync();
            }
            else
            {
                measP = await (from mp in _context.MeasPoints
                               .Include(x => x.Documents).ThenInclude(x => x.DocumentType)
                               .Include(x => x.Project).ThenInclude(x => x.CoordSystem)
                               join doc in _context.Documents
                                on mp.Id equals doc.MeasPointId
                               where mp.Project.DivisionId.Equals(user.DivisionId)
                               select mp).GroupBy(x => x.Id).Select(x => x.First()).OrderBy(x => x.Project.Name).ThenBy(x => x.Name)
                                            .ToListAsync();
            }

            foreach (MeasPoint mp in measP)
            {
                if (mp.Project.CoordSystem.system == "UTM32N")
                {
                    if ((mp.Lati == null || mp.Longi == null) && !(mp.Coordx == 0 || mp.Coordy == 0))
                    {
                        LatLngUTMConverter.LatLng latlng = ltUTMconv.convertUtmToLatLng(mp.Coordx, mp.Coordy, 32, "N");
                        mp.Lati = latlng.Lat;
                        mp.Longi = latlng.Lng;
                        _context.Update(mp);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        try {
                            LatLngUTMConverter.LatLng latlng = await findUTMcoords(mp);
                            mp.Lati = latlng.Lat;
                            mp.Longi = latlng.Lng;
                            if (mp.Lati != null && mp.Longi != null)
                            {
                                _context.Update(mp);
                                await _context.SaveChangesAsync();
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                else if (mp.Project.CoordSystem.system == "UTM17N")
                {
                    if ((mp.Lati == null || mp.Longi == null) && !(mp.Coordx == 0 || mp.Coordy == 0))
                    {
                        LatLngUTMConverter.LatLng latlng = ltUTMconv.convertUtmToLatLng(mp.Coordx, mp.Coordy, 17, "N");
                        mp.Lati = latlng.Lat;
                        mp.Longi = latlng.Lng;
                        _context.Update(mp);
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    LatLngUTMConverter.LatLng latlng = await findUTMcoords(mp);
                    mp.Lati = latlng.Lat;
                    mp.Longi = latlng.Lng;
                    if (mp.Lati != null && mp.Longi != null)
                    {
                        _context.Update(mp);
                        await _context.SaveChangesAsync();
                    }
                }
                if (mp.Lati != null && mp.Longi != null)
                {
                    var distance = DistanceAlgorithm.DistanceBetweenPlaces(longi, lati, Convert.ToDouble(mp.Longi), Convert.ToDouble(mp.Lati));
                    if (distance < radi)
                    {
                        measpoints.Add(mp);
                    }
                }

            }

            return PartialView("_MeasPointDocuments", measpoints);
        }
        [Authorize(Roles = "Admin,Member,Guest,DivisionAdmin,MemberGuest,ProjectMember")]
        public JsonResult GetMeasPointsInRadius2(double lati, double longi)
        {
            List<MeasPoint> mps = new List<MeasPoint>();
            var thedata = _context.MeasPoints.Include(x => x.Project).Where(x => x.Project.Active.Equals(true) && DistanceAlgorithm.DistanceBetweenPlaces(longi, lati, Convert.ToDouble(x.Longi), Convert.ToDouble(x.Lati)) <= 2 && (x.MeasType.Type.ToLower().Equals("water level") || x.MeasType.Type.ToLower().Equals("miscellaneous"))).ToList();
            return Json(thedata);
        }
        [HttpPost]
        public JsonResult UpdateOffset(double theoffset, int theId)
        {
            Offset offset = new Offset();
            offset.MeasPointId = theId;
            offset.starttime = DateTime.Now;
            offset.offset = theoffset;
            _context.Add(offset);
            _context.SaveChanges();
            return Json(offset);
        }
        public JsonResult GetCurrentOffset(string theId)
        {
            int Id = Convert.ToInt32(theId);
            var offset = _context.Offsets.Where(x => x.MeasPointId.Equals(Id)).OrderByDescending(x => x.starttime).First();
            return Json(offset);
        }
        [HttpGet]
        public JsonResult GetCurrentTop(string theId,string WellName)
        {
            int Id = Convert.ToInt32(theId);
            var offset = _context.Offsets.Include(x => x.measpoint).Where(x => x.measpoint.ProjectId.Equals(Id) && x.measpoint.Name.Equals(WellName)).OrderByDescending(x => x.starttime).FirstOrDefault();
            if(offset == null)
            {
                var measpoint = _context.MeasPoints.SingleOrDefault(x => x.ProjectId.Equals(Id) && x.Name.Equals(WellName));
                Offset o = new Offset();
                o.offset = measpoint.Offset;
                o.MeasPointId = measpoint.Id;
                o.starttime = DateTime.Now;
                return Json(o);
            }
            return Json(offset);
        }
        [HttpGet]
        public JsonResult GetCoord(string theId, string WellName)
        {
            int Id = Convert.ToInt32(theId);
            var measpoint = _context.MeasPoints.SingleOrDefault(x => x.ProjectId.Equals(Id) && x.Name.Equals(WellName));
            return Json(measpoint);
        }
        
        public JsonResult GetMeasPointsProject(string theId)
        {

            int Id = Convert.ToInt32(theId);
            //var thedata =  _context.MeasPoints.Where(x => x.ProjectId.Equals(Id)).OrderBy(x=>x.Name).ToList();
            var thedata = _context.MeasPoints.Include(x => x.Project).Include(x => x.MeasType).Where(x => x.ProjectId.Equals(Id) && x.MeasType.Type.ToLower().Contains("level") && x.ToBeHidden.Equals(false)).OrderBy(x => x.Name).ToList();

            return Json(thedata);
        }
        public JsonResult GetMeasPointsWLProject(string theId)
        {
            int Id = Convert.ToInt32(theId);
            var thedata = _context.MeasPoints.Include(x => x.MeasType).Where(x => x.ProjectId.Equals(Id) && x.MeasType.Type.ToLower().Equals("water level") && x.ToBeHidden.Equals(false)).OrderBy(x => x.Name).ToList();

            return Json(thedata);
        }
        public async Task<List<double>> findRealMeases(DateTime starttime, DateTime endtime, int id)
        {
            List<double> themeasures = new List<double>();
            var measures = await _context.Measures.Include(x => x.MeasPoint).ThenInclude(x => x.MeasType).Where(x => x.MeasPointId.Equals(id) && x.When >= starttime && x.When <= endtime && x.TheMeasurement != null).OrderBy(x => x.When).ToListAsync();
            var offsets = await _context.Offsets.Where(x => x.MeasPointId.Equals(id) && x.starttime <= endtime).OrderBy(x => x.starttime).ToListAsync();
            if (offsets.Count() < 1)
            {
                var theoffset = await _context.Offsets.Where(x => x.MeasPointId.Equals(id)).OrderBy(x => x.starttime).FirstAsync();
                foreach (Meas m in measures)
                {
                    if (m.MeasPoint.MeasType.Type.ToLower().Equals("water level"))
                    {
                        themeasures.Add(theoffset.offset - Convert.ToDouble(m.TheMeasurement));
                    }
                    else if (m.MeasPoint.MeasType.Type.ToLower().Equals("water meter"))
                    {
                        themeasures.Add(Convert.ToDouble(m.TheMeasurement) - theoffset.offset);
                    }
                    else
                    {
                        themeasures.Add(Convert.ToDouble(m.TheMeasurement));
                    }

                }
            }
            else
            {
                Offset thecurrentoffset = offsets.First();
                DateTime measdate = measures.First().When;
                double total = 0.0;
                int lastoffsetid = 0;
                foreach (Meas m in measures)
                {
                    try
                    {

                        thecurrentoffset = offsets.Where(x => x.starttime <= m.When).OrderByDescending(x => x.starttime).First();
                        if (lastoffsetid != thecurrentoffset.Id && thecurrentoffset.Id != offsets.First().Id)
                        {
                            total += themeasures.Last();
                        }
                        lastoffsetid = thecurrentoffset.Id;
                    }
                    catch
                    {

                    }

                    if (m.MeasPoint.MeasType.Type.ToLower().Equals("water level"))
                    {
                        themeasures.Add(thecurrentoffset.offset - Convert.ToDouble(m.TheMeasurement));
                    }
                    else if (m.MeasPoint.MeasType.Type.ToLower().Equals("water meter"))
                    {
                        themeasures.Add(Convert.ToDouble(m.TheMeasurement) - thecurrentoffset.offset + total);
                    }
                    else
                    {
                        themeasures.Add(Convert.ToDouble(m.TheMeasurement));
                    }
                }
            }
            return themeasures;

        }
        [HttpGet]
        public async Task<IActionResult> CreateMeasPointSeries()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["MonitorTypeId"] = new SelectList(_context.MonitorType.Where(x => x.DivisionId.Equals(user.DivisionId)), "Id", "MonitorTypeName");
            ViewData["MeasTypeId"] = new SelectList(_context.MeasTypes.Where(x => x.DivisionId.Equals(user.DivisionId)), "Id", "Type");
            ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.Active.Equals(true)).Include(x => x.Division).Where(x => x.Division.Id.Equals(user.DivisionId)), "Id", "Name");
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateMeasPointSeries(MeasPointSeriesVM model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                bool notfound = true;
                int rightlabor = 1;
                string wellname = "";
                List<int> laborids = new List<int>();
                var all_measpoints = await _context.MeasPoints.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)).ToListAsync();
                foreach (MeasPoint mp in all_measpoints)
                {
                    laborids.Add(mp.LaborId);
                }
                
                for (int i = model.startnum; i <= model.endnum; i++)
                {
                    if (model.numDigits == 1)
                    {
                        wellname = model.PrelimName + i.ToString("D1");
                    }
                    else if (model.numDigits == 2)
                    {
                        wellname = model.PrelimName + i.ToString("D2");
                    }
                    else if (model.numDigits == 3)
                    {
                        wellname = model.PrelimName + i.ToString("D3");

                    }
                    else if (model.numDigits == 4)
                    {
                        wellname = model.PrelimName + i.ToString("D4");
                    }
                    MeasPoint mp = new MeasPoint();
                    mp.Name = wellname;
                    mp.ProjectId = model.ProjectId;
                    mp.SubProjectId = model.SubProjectId;
                    
                    mp.MonitorTypeId = model.MonitorTypeId;
                    mp.MeasTypeId = model.MeasTypeId;
                    mp.Coordx = 0;
                    mp.Coordy = 0;
                    mp.Coordz = 0;
                    mp.LoggerActive = true;
                    mp.ToBeHidden = false;
                    mp.Offset = 0;
                    notfound = true;
                    while (notfound)
                    {
                        if (laborids.IndexOf(rightlabor) < 0)
                        {
                            mp.LaborId = rightlabor;
                            notfound = false;
                            rightlabor += 1;
                        }
                        else
                        {
                            rightlabor += 1;
                        }
                    }
                    _context.Add(mp);


                }

                if (model.AddWM == true)
                {
                    for (int i = model.startnum; i <= model.endnum; i++)
                    {
                        if (model.numDigits == 1)
                        {
                            wellname = model.PrelimName + i.ToString("D1") + " Watermeter";
                        }
                        else if (model.numDigits == 2)
                        {
                            wellname = model.PrelimName + i.ToString("D2") + " Watermeter";
                        }
                        else if (model.numDigits == 3)
                        {
                            wellname = model.PrelimName + i.ToString("D3") + " Watermeter";

                        }
                        else if (model.numDigits == 4)
                        {
                            wellname = model.PrelimName + i.ToString("D4") + " Watermeter";
                        }
                        MeasPoint mp = new MeasPoint();
                        mp.ProjectId = model.ProjectId;
                        mp.SubProjectId = model.SubProjectId;
                        mp.Name = wellname;
                        var meastype = await _context.MeasTypes.FirstOrDefaultAsync(x => x.DivisionId.Equals(user.DivisionId) && x.Type.ToLower().Equals("water meter"));
                        var monitorttype = await _context.MonitorType.FirstOrDefaultAsync(x => x.DivisionId.Equals(user.DivisionId) && x.MonitorTypeName.ToLower().Equals("flow meter"));
                        mp.MeasTypeId = meastype.Id;
                        mp.MonitorTypeId = monitorttype.Id;
                        mp.Coordx = 0;
                        mp.Coordy = 0;
                        mp.Coordz = 0;
                        mp.LoggerActive = true;
                        mp.ToBeHidden = false;
                        mp.Offset = 0;
                        notfound = true;
                        while (notfound)
                        {
                            if (laborids.IndexOf(rightlabor) < 0)
                            {
                                mp.LaborId = rightlabor;
                                notfound = false;
                                rightlabor += 1;
                            }
                            else
                            {
                                rightlabor += 1;
                            }
                        }
                        _context.Add(mp);
                    }
                }
                if (model.AddFlow == true)
                {
                    for (int i = model.startnum; i <= model.endnum; i++)
                    {
                        if (model.numDigits == 1)
                        {
                            wellname = model.PrelimName + i.ToString("D1") + " Flow";
                        }
                        else if (model.numDigits == 2)
                        {
                            wellname = model.PrelimName + i.ToString("D2") + " Flow";
                        }
                        else if (model.numDigits == 3)
                        {
                            wellname = model.PrelimName + i.ToString("D3") + " Flow";

                        }
                        else if (model.numDigits == 4)
                        {
                            wellname = model.PrelimName + i.ToString("D4") + " Flow";
                        }
                        MeasPoint mp = new MeasPoint();
                        mp.ProjectId = model.ProjectId;
                        mp.SubProjectId = model.SubProjectId;
                        mp.Name = wellname;
                        var meastype = await _context.MeasTypes.FirstOrDefaultAsync(x => x.DivisionId.Equals(user.DivisionId) && x.Type.ToLower().Equals("flow rate"));
                        var monitorttype = await _context.MonitorType.FirstOrDefaultAsync(x => x.DivisionId.Equals(user.DivisionId) && x.MonitorTypeName.ToLower().Equals("flow meter"));
                        mp.MonitorTypeId = monitorttype.Id;
                        mp.MeasTypeId = meastype.Id;
                        mp.Coordx = 0;
                        mp.Coordy = 0;
                        mp.Coordz = 0;
                        mp.LoggerActive = true;
                        mp.ToBeHidden = false;
                        mp.Offset = 0;
                        notfound = true;
                        while (notfound)
                        {
                            if (laborids.IndexOf(rightlabor) < 0)
                            {
                                mp.LaborId = rightlabor;
                                notfound = false;
                                rightlabor += 1;
                            }
                            else
                            {
                                rightlabor += 1;
                            }
                        }
                        _context.Add(mp);
                    }
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewData["MonitorTypeId"] = new SelectList(_context.MonitorType.Where(x => x.DivisionId.Equals(user.DivisionId)), "Id", "MonitorTypeName");
                ViewData["MeasTypeId"] = new SelectList(_context.MeasTypes.Where(x => x.DivisionId.Equals(user.DivisionId)), "Id", "Type");
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.Active.Equals(true)).Include(x => x.Division).Where(x => x.Division.Id.Equals(user.DivisionId)), "Id", "Name");
                ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
                return View(model);
            }

        }
        [HttpGet]
        public async Task<IActionResult> ReUploadToFTP()
        {
            List<SelectListItem> selList = await createMeasPointList();
            ViewData["MeasPointId"] = selList;
            ViewData["ProjectId"] = await GetProjectList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ReUploadToFTP(UploadDataMeasPointVM model)
        {
            var user = await _userManager.GetUserAsync(User);
            if(user.DivisionId != 9 && !User.IsInRole("Admin"))
            {
                return RedirectToAction(nameof(Index));
            }
            var measures = await _context.Measures
                    .Include(x => x.MeasPoint).ThenInclude(x => x.Offsets)
                    .Include(x => x.MeasPoint).ThenInclude(x => x.Project)
                    .Include(x => x.MeasPoint).ThenInclude(x => x.MeasType)
                    .Include(x => x.TheComment)
                    .Where(x => x.MeasPointId.Equals(model.MeasPointId) && x.When >= model.StartTime && x.When <= model.EndTime).ToListAsync();
            var mp = await _context.MeasPoints.SingleOrDefaultAsync(x => x.Id.Equals(model.MeasPointId));
            var directory = _environment.WebRootPath + "\\TempFile\\";
            var path = Path.Combine(directory, String.Concat(mp.Name,"_",DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss"),".txt"));
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            StringBuilder sb = new StringBuilder();


            List<string> headerrow = new List<string>(new string[8]);
            headerrow[0] = "Measurement Point Name";
            headerrow[1] = "Labor ID";
            headerrow[2] = "TimeStamp";
            headerrow[3] = "The Raw Measurement";
            headerrow[4] = "The Measurement Reference Point";
            headerrow[5] = "The Measurement in Reference";
            headerrow[6] = "NewComment";
            headerrow[7] = "ChosenComment";
            sb.AppendLine(string.Join(";", headerrow.ToArray()));
            List<string> datarow = new List<string>(new string[8]);
            foreach (Meas meas in measures)
            {

                //get offset
                Offset theoffset = new Offset();
                double themeasurement = 100000;
                if (meas.TheMeasurement != null)
                {

                    var theoffsets = meas.MeasPoint.Offsets.OrderByDescending(x => x.starttime).ToList();
                    if (theoffsets.Count == 0)
                    {
                        theoffset.offset = 0;
                    }
                    else if (theoffsets.Count == 1)
                    {
                        theoffset = theoffsets.First();
                    }
                    else
                    {
                        try
                        {
                            theoffset = theoffsets.Where(x => x.starttime <= meas.When).OrderByDescending(x => x.starttime).First();
                        }
                        catch
                        {
                            try
                            {
                                theoffset = theoffsets.OrderBy(x => x.starttime).First();
                            }
                            catch
                            {
                                theoffset.offset = meas.MeasPoint.Offset;
                                theoffset.starttime = DateTime.Now;
                            }

                        }
                    }
                    if (meas.MeasPoint.MeasType.Type.ToLower().Equals("water level"))
                    {
                        themeasurement = theoffset.offset - Convert.ToDouble(meas.TheMeasurement);

                    }
                    else if (meas.MeasPoint.MeasTypeId.Equals(113))
                    {
                        themeasurement = await getWatermeterTrueMeas(meas) * 10.0;
                    }
                    else if (meas.MeasPoint.MeasTypeId.Equals(114))
                    {
                        themeasurement = await getWatermeterTrueMeas(meas) * 100.0;
                    }
                    else if (meas.MeasPoint.MeasType.Type.ToLower().Equals("water meter"))
                    {
                        themeasurement = await getWatermeterTrueMeas(meas);
                    }

                    else
                    {
                        themeasurement = Convert.ToDouble(meas.TheMeasurement);
                    }
                }
                //
                datarow[0] = meas.MeasPoint.Name;
                datarow[1] = meas.MeasPoint.LaborId.ToString();
                datarow[2] = meas.When.ToString("yyyy-MM-dd HH:mm:ss");
                if (meas.TheMeasurement != null)
                {
                    datarow[3] = String.Format("{0:#,0.000}", meas.TheMeasurement);
                }
                else
                {
                    datarow[3] = "";
                }
                datarow[4] = theoffset.offset.ToString();
                if (meas.TheMeasurement != null)
                {
                    datarow[5] = String.Format("{0:#,0.000}", themeasurement);
                }
                else
                {
                    datarow[5] = "";
                }
                if(model.CommentToAdd != "")
                {
                    if(meas.NewComment != "") 
                    {
                        datarow[6] = meas.NewComment + " : " + model.CommentToAdd;
                    }
                    else
                    {
                        datarow[6] = model.CommentToAdd;
                    }
                    
                }
                else
                {
                    datarow[6] = meas.NewComment;
                }
               
                if (meas.CommentId != null)
                {
                    var comment = await _context.Comments.SingleOrDefaultAsync(x => x.Id.Equals(meas.CommentId));
                    datarow[7] = comment.comment;
                }
                else
                {
                    datarow[7] = "";
                }


                sb.AppendLine(string.Join(";", datarow.ToArray()));
            }


            System.IO.File.WriteAllText(path, sb.ToString());
            PasswordAuthenticationMethod pMethod = new PasswordAuthenticationMethod(_Options.UserName, _Options.Key);
            Renci.SshNet.ConnectionInfo conInfo = new Renci.SshNet.ConnectionInfo(_Options.Domain, 22, _Options.UserName, pMethod);
            using (SftpClient client = new SftpClient(conInfo))
            {

                client.Connect();
                try
                {
                    using (var fileStream = new FileStream(path, FileMode.Open))
                    {
                        client.BufferSize = 10 * 1024;
                        client.ChangeDirectory("/public/MainOps");
                        client.UploadFile(fileStream, Path.GetFileName(path));
                    }
                }
                catch
                {
                    client.Disconnect();
                }
                try
                {
                    client.Disconnect();
                }
                catch
                {

                }

            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<List<SelectListItem>> createMeasPointList()
        {
            var user = await _userManager.GetUserAsync(User);
            List<Project> groups = new List<Project>();
            if (User.IsInRole("Admin"))
            {
                groups = await _context.Projects.Include(x => x.Division).ToListAsync();
            }
            else
            {
                groups = await _context.Projects.Include(x => x.Division)
                .Where(x => x.Division.Id.Equals(user.DivisionId)).ToListAsync();
            }

            List<SelectListGroup> thegroups = new List<SelectListGroup>();
            List<SelectListItem> theList = new List<SelectListItem>();
            foreach (Project p in groups)
            {
                if (!thegroups.Any(x => x.Name == p.Name))
                {
                    thegroups.Add(new SelectListGroup() { Name = p.Name });
                }
            }
            List<MeasPoint> monpoints = new List<MeasPoint>();
            if (User.IsInRole("MemberGuest"))
            {
                monpoints = await (from m in _context.MeasPoints.Include(m => m.Project).ThenInclude(x => x.Division)
                                   join pu in _context.ProjectUsers on m.ProjectId
                                                     equals pu.projectId
                                   where pu.userId == user.Id && m.Project.Active.Equals(true) && m.ToBeHidden.Equals(false)
                                   select m).OrderBy(b => b.Project.Name).ThenBy(x => x.Name).ToListAsync();
            }
            else
            {
                if (User.IsInRole("Admin"))
                {
                    monpoints = await _context.MeasPoints.Where(x => x.ToBeHidden.Equals(false)).Include(x => x.Project).ThenInclude(x => x.Division)
                .OrderBy(b => b.Project.Name).ThenBy(x => x.Name).ToListAsync();
                }
                else
                {
                    monpoints = await _context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division)
                .Where(x => x.Project.Division.Id.Equals(user.DivisionId) && x.ToBeHidden.Equals(false))
                .OrderBy(b => b.Project.Name).ThenBy(x => x.Name).ToListAsync();
                }

            }
            foreach (MeasPoint m in monpoints)
            {
                theList.Add(new SelectListItem()
                {
                    Value = m.Id.ToString(),
                    Text = m.Name,
                    Group = thegroups.Where(x => x.Name.Equals(m.Project.Name)).First()
                });
            }


            return theList;
        }

    }
   
}