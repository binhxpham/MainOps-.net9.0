using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MainOps.Data;
using MainOps.Models;
using System.IO;
using System.Drawing;
using QRCoder;
using Microsoft.AspNetCore.Hosting;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MainOps.ExtensionMethods;
using System.Net.Http.Headers;
using System.IO.Compression;
using System.Text;
using Rotativa.AspNetCore.Options;
using MainOps.Resources;

namespace MainOps.Controllers
{
    
    public class HJItemsController : BaseController
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LocService _sharedLocalizer;

        public HJItemsController(LocService SharedLocalizer,DataContext context, IWebHostEnvironment hostingenvironment,UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            _context = context;
            _env = hostingenvironment;
            _userManager = userManager;
            _sharedLocalizer = SharedLocalizer;
                    
        }
        [HttpGet]
        [Route("/HJItems/DataOutPut/data.csv")]
        [Produces("text/csv")]
        public async Task<IActionResult> DataOutPut()
        {
            StringBuilder sb = new StringBuilder();
            List<string> headerrow = new List<string>(new string[] { "ID","Master Klasse","Under Klasse", "weight", "length", "width", "height"});
            List<string> datarow = new List<string>(new string[] { "", "", "", "", "", "", "" });
            sb.AppendLine(string.Join(";", headerrow.ToArray()));
            var items = await _context.HJItems.Include(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass).ToListAsync();
            foreach(var item in items)
            {
                datarow[0] = item.HJId;
                datarow[1] = item.HJItemClass.HJItemMasterClass.ClassName;
                datarow[2] = item.HJItemClass.ClassName;
                datarow[3] = String.Format("{0:N2}",item.Weight);
                datarow[4] = String.Format("{0:N2}", item.ItemLength);
                datarow[5] = String.Format("{0:N2}", item.ItemWidth);
                datarow[6] = String.Format("{0:N2}", item.ItemHeight);
                sb.AppendLine(string.Join(";",datarow.ToArray()));
            }
            return File(System.Text.Encoding.ASCII.GetBytes(sb.ToString()), "text/csv", "data.csv");
        }
        public async Task<IActionResult> CreatePipeBridgeElements()
        {
            var MasterClass = await _context.HJItemMasterClasses.FindAsync(11);
            var Class1 = await _context.HJItemClasses.FindAsync(41);
            var Class2 = await _context.HJItemClasses.FindAsync(42);
            var Class3 = await _context.HJItemClasses.FindAsync(43);
            for(int i = 0; i < 233; i++)
            {
                HJItem newitem = new HJItem();
                newitem.Name = Class1.ClassName;
                newitem.HJItemClassId = Class1.Id;
                newitem.DivisionId = 1;
                newitem.SetHJNumber(i, MasterClass.ClassNumber, Class1.ClassNumber);
                newitem.Ownership = "HJ";
                _context.Add(newitem);
            }            
            for(int i = 0; i < 150; i++)
            {
                HJItem newitem = new HJItem();
                newitem.Name = Class2.ClassName;
                newitem.HJItemClassId = Class2.Id;
                newitem.DivisionId = 1;
                newitem.SetHJNumber(i, MasterClass.ClassNumber, Class2.ClassNumber);
                newitem.Ownership = "HJ";
                _context.Add(newitem);
            }
            for(int i = 0; i < 150; i++)
            {
                HJItem newitem = new HJItem();
                newitem.Name = Class3.ClassName;
                newitem.HJItemClassId = Class3.Id;
                newitem.DivisionId = 1;
                newitem.SetHJNumber(i, MasterClass.ClassNumber, Class3.ClassNumber);
                newitem.Ownership = "HJ";
                _context.Add(newitem);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // GET: HJItems
        [Authorize(Roles = "DivisionAdmin,Admin,Member,StorageManager")]
        public async Task<IActionResult> Index(int? ProjectId = null,int? SubProjectId = null, bool AllCategories = false)
        {
            var theuser = await _userManager.GetUserAsync(User);
            //IEnumerable<SelectListItem> selList = await createFilterlist();
            //ViewData["filterchoices"] = new SelectList(selList, "Value", "Text");
            if (theuser != null)
            {
                ViewData["AllCategories"] = AllCategories;
                ViewData["masters"] = await _context.HJItemMasterClasses.Where(x => x.DivisionId.Equals(theuser.DivisionId)).OrderBy(x =>Convert.ToInt32(x.ClassNumber)).ToListAsync();
                ViewData["subclasses"] = await _context.HJItemClasses.Include(x => x.HJItemMasterClass).Where(x => x.HJItemMasterClass.DivisionId.Equals(theuser.DivisionId)).OrderBy(x => Convert.ToInt32(x.ClassNumber)).ToListAsync();
                ViewData["ProjectId"] = await GetProjectList();
                if (User.IsInRole("Admin"))
                {
                    var items = await _context.HJItems.Include(x => x.Locations).ThenInclude(x => x.Project)
                        .Include(x => x.Locations).ThenInclude(x => x.SubProject)
                        .Include(x => x.HJItemClass).ThenInclude(x=>x.HJItemMasterClass).OrderBy(x => x.HJItemClass.HJItemMasterClass.ClassNumber).ThenBy(x => x.HJItemClass.ClassNumber).ThenBy(x => x.HJId).ToListAsync();
                    if(SubProjectId != null)
                    {
                        items = items.Where(x => x.Locations != null).ToList();
                        items = items.Where(x => x.Locations.LastOrDefault() != null).ToList();
                        items = items.Where(x => x.Locations.Last().SubProjectId == SubProjectId).ToList();
                    }
                    else if(ProjectId != null && SubProjectId == null)
                    {
                        items = items.Where(x => x.Locations != null).ToList();
                        items = items.Where(x => x.Locations.LastOrDefault() != null).ToList();
                        items = items.Where(x => x.Locations.Last().ProjectId == ProjectId).ToList();
                    }
                    return View(items);
                }
                else
                {
                    var items = await _context.HJItems.Include(x => x.Locations).ThenInclude(x => x.Project)
                        .Include(x => x.Locations).ThenInclude(x => x.SubProject)
                        .Include(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass).Where(x => x.DivisionId.Equals(theuser.DivisionId))
                        .OrderBy(x => x.HJItemClass.HJItemMasterClass.ClassNumber).ThenBy(x => x.HJItemClass.ClassNumber).ThenBy(x => x.HJId)
                        .ToListAsync();
                    if (SubProjectId != null)
                    {
                        items = items.Where(x => x.Locations != null).ToList();
                        items = items.Where(x => x.Locations.LastOrDefault() != null).ToList();
                        items = items.Where(x => x.Locations.Last().SubProjectId == SubProjectId).ToList();
                    }
                    else if (ProjectId != null && SubProjectId == null)
                    {
                        items = items.Where(x => x.Locations != null).ToList();
                        items = items.Where(x => x.Locations.LastOrDefault() != null).ToList();
                        items = items.Where(x => x.Locations.Last().ProjectId == ProjectId).ToList();
                    }
                    return View(items);
                }
                
            }
            else
            {
                List<HJItem> theitems = new List<HJItem>();
                return View(theitems);
            }
            
        }
        public async Task<IActionResult> Classes()
        {
            var theuser = await _userManager.GetUserAsync(User);
            ViewData["masters"] = await _context.HJItemMasterClasses.Include(x => x.Division).Where(x => x.DivisionId.Equals(theuser.DivisionId)).OrderBy(x => Convert.ToInt32(x.ClassNumber)).ToListAsync();
            ViewData["subclasses"] = await _context.HJItemClasses.Include(x => x.HJItemMasterClass).Where(x => x.HJItemMasterClass.DivisionId.Equals(theuser.DivisionId)).OrderBy(x => Convert.ToInt32(x.ClassNumber)).ToListAsync();

            return View();
            
        }
        [AllowAnonymous]
        public ActionResult Footer()
        {
            return View();
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ExportClassesPDF()
        {
            var theuser = await _userManager.GetUserAsync(User);
            HJItemClasses model = new HJItemClasses();
            model.Masters = await _context.HJItemMasterClasses.Include(x => x.Division).Where(x => x.DivisionId.Equals(theuser.DivisionId)).OrderBy(x => Convert.ToInt32(x.ClassNumber)).ToListAsync();
            model.Subclasses = await _context.HJItemClasses.Include(x => x.HJItemMasterClass).Where(x => x.HJItemMasterClass.DivisionId.Equals(theuser.DivisionId)).OrderBy(x => Convert.ToInt32(x.ClassNumber)).ToListAsync();
            //string customSwitches = string.Format("--print-media-type --allow {0} --footer-html {0} --footer-spacing -10 --footer-center [page]",
            //    Url.Action("Footer", "Document", new { area = "" }, "https"));
            string footer = "--footer-center \"" + _sharedLocalizer.GetLocalizedHtmlString("Printed on:").Value + DateTime.Now.Date.ToString("yyyy-MM-dd") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
            return new ViewAsPdf("_ClassesPDF", model) {
                FileName = String.Concat(_sharedLocalizer.GetLocalizedHtmlString("Machine Account Plan"), ".pdf"),
                PageOrientation = Orientation.Portrait,
                MinimumFontSize = 10,
                //PageMargins  = new Margins(5,5,5,5),
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                CustomSwitches = footer
            };
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ExportClassesPDFWithCosts()
        {
            var theuser = await _userManager.GetUserAsync(User);
            HJItemClasses model = new HJItemClasses();
            model.Masters = await _context.HJItemMasterClasses.Include(x => x.Division).Where(x => x.DivisionId.Equals(theuser.DivisionId)).OrderBy(x => Convert.ToInt32(x.ClassNumber)).ToListAsync();
            model.Subclasses = await _context.HJItemClasses.Include(x => x.HJItemMasterClass).Where(x => x.HJItemMasterClass.DivisionId.Equals(theuser.DivisionId)).OrderBy(x => Convert.ToInt32(x.ClassNumber)).ToListAsync();
            //string customSwitches = string.Format("--print-media-type --allow {0} --footer-html {0} --footer-spacing -10 --footer-center [page]",
            //    Url.Action("Footer", "HJItems", new { area = "" }, "https"));
            string footer = "--footer-center \"" + _sharedLocalizer.GetLocalizedHtmlString("Printed on:").Value + DateTime.Now.Date.ToString("yyyy-MM-dd") + "  " + _sharedLocalizer.GetLocalizedHtmlString("Page:").Value + " [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("_ClassesPDFWithCosts", model)
            {
                FileName = String.Concat(_sharedLocalizer.GetLocalizedHtmlString("Machine Account Plan"), ".pdf"),
                PageOrientation = Orientation.Portrait,
                MinimumFontSize = 10,
                //PageMargins  = new Margins(5,5,5,5),
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                CustomSwitches = footer
            };
        }
        [Route("/HJItems/ExportClassesCSV/classes.csv")]
        [Produces("text/csv")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ExportClassesCSV()
        {
            var theuser = await _userManager.GetUserAsync(User);
            var masters = await _context.HJItemMasterClasses.Where(x => x.DivisionId.Equals(theuser.DivisionId)).OrderBy(x => Convert.ToInt32(x.ClassNumber)).ToListAsync();
            StringBuilder sb = new StringBuilder();
            List<string> row = new List<string>(new string[] { "masternumber", "mastername", "subclassnumber", "subclassname" });
            foreach (var master in masters)
            {
                string masterclassnumber = String.Concat("'31-", Convert.ToInt32(master.ClassNumber).ToString("D2"));
                row = new List<string>(new string[] { String.Concat(masterclassnumber," : ",master.ClassName), "","" });
                sb.AppendLine(string.Join(";", row.ToArray()));
                var subclasses = await _context.HJItemClasses.Include(x => x.HJItemMasterClass).Where(x => x.HJItemMasterClass.DivisionId.Equals(theuser.DivisionId) && x.HJItemMasterClassId.Equals(master.Id)).OrderBy(x => Convert.ToInt32(x.ClassNumber)).ToListAsync();
                foreach(var subclass in subclasses)
                {
                    string subclassnumber = String.Concat(masterclassnumber, "-", Convert.ToInt32(subclass.ClassNumber).ToString("D2")) ;
                    row = new List<string>(new string[] {subclassnumber, "",Convert.ToInt32(subclass.ClassNumber).ToString("D2"), subclass.ClassName });
                    sb.AppendLine(string.Join(";", row.ToArray()));
                }
            }
            return File(System.Text.Encoding.ASCII.GetBytes(sb.ToString()), "text/csv", "classes.csv");
        }
        [Route("/HJItems/ExportClassesCSVWithCosts/classes.csv")]
        [Produces("text/csv")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ExportClassesCSVWithCosts()
        {
            var theuser = await _userManager.GetUserAsync(User);
            var masters = await _context.HJItemMasterClasses.Where(x => x.DivisionId.Equals(theuser.DivisionId)).OrderBy(x => Convert.ToInt32(x.ClassNumber)).ToListAsync();
            StringBuilder sb = new StringBuilder();
            List<string> row = new List<string>(new string[] { "masternumber", "mastername", "subclassnumber", "subclassname","rent" });
            foreach (var master in masters)
            {
                string masterclassnumber = String.Concat("'31-", Convert.ToInt32(master.ClassNumber).ToString("D2"));
                row = new List<string>(new string[] { String.Concat(masterclassnumber, " : ",master.ClassName), "", "","" });
                sb.AppendLine(string.Join(";", row.ToArray()));
                var subclasses = await _context.HJItemClasses.Include(x => x.HJItemMasterClass).Where(x => x.HJItemMasterClass.DivisionId.Equals(theuser.DivisionId) && x.HJItemMasterClassId.Equals(master.Id)).OrderBy(x => Convert.ToInt32(x.ClassNumber)).ToListAsync();
                foreach (var subclass in subclasses)
                {
                    string subclassnumber = String.Concat(masterclassnumber, "-", Convert.ToInt32(subclass.ClassNumber).ToString("D2"));
                    double internal_rent = subclass.Internal_Rent;
                    row = new List<string>(new string[] { subclassnumber, "", Convert.ToInt32(subclass.ClassNumber).ToString("D2"), subclass.ClassName,internal_rent.ToString() });
                    sb.AppendLine(string.Join(";", row.ToArray()));
                }
            }
            return File(System.Text.Encoding.ASCII.GetBytes(sb.ToString()), "text/csv", "classes.csv");
        }


        // GET: HJItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var hJItem = await _context.HJItems.Include(x=>x.Division).Include(x=>x.HJItemClass).ThenInclude(x=>x.HJItemMasterClass)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hJItem == null)
            {
                return NotFound();
            }
            else if(user == null)
            {

            }
            else if (!hJItem.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
            }
            List<string> pathstofotos = new List<string>();
            if(hJItem.PathToPicture != "" && hJItem.PathToPicture != null) {
                try { 
                    foreach (string fileName in Directory.GetFiles(hJItem.PathToPicture))
                    {
                        pathstofotos.Add(Path.Combine(_env.WebRootPath.ReplaceFirst("/", ""),"images","Items","Photos", hJItem.Id.ToString()) + $@"\{fileName.Split("\\").Last()}");
                    }
                }
                catch
                {

                }
            }
            ViewBag.Photos = pathstofotos;
            List<string> pathsgeneralphotos = new List<string>();
            try
            {
                var directory = Path.Combine(_env.WebRootPath.ReplaceFirst("/", ""), "Images", "ItemClasses", "Photos", hJItem.HJItemClassId.ToString());
                foreach (string fileName in Directory.GetFiles(directory))
                {
                    if (fileName.Contains("edit"))
                    {
                        pathsgeneralphotos.Add(Path.Combine(_env.WebRootPath.ReplaceFirst("/", ""), "images", "ItemClasses", "Photos", hJItem.HJItemClassId.ToString()) + $@"\{fileName.Split("\\").Last()}");
                    }
                }
            }
            catch
            {

            }
            ViewBag.GeneralPhotos = pathsgeneralphotos;
            List<string> pathstodrawings = new List<string>();
            if(hJItem.PathTo3DDrawing != "" && hJItem.PathTo3DDrawing != null) {
                try { 
                foreach (string fileName in Directory.GetFiles(hJItem.PathTo3DDrawing))
                {
                    pathstodrawings.Add(Path.Combine(_env.WebRootPath.ReplaceFirst("/", ""), "images", "Items", "Drawings", hJItem.Id.ToString()) + $@"\{fileName.Split("\\").Last()}");
                }
                }
                catch
                {

                }
            }
            ViewBag.Drawings = pathstodrawings;
            return View(hJItem);
        }
        [HttpGet]
        public async Task<IActionResult> ShowItems(int? ProjectId = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if(ProjectId == null) { 
                var items = await _context.HJItems.Include(x=>x.Division).Include(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass).Where(x=>x.Latitude != null && x.Longitude != null && x.DivisionId.Equals(user.DivisionId)).ToListAsync();
                ViewData["ProjectId"] = await GetProjectList();
                ViewData["HJItemClassId"] = new SelectList(_context.HJItemClasses.Include(x=>x.HJItemMasterClass).Where(x=>x.HJItemMasterClass.DivisionId.Equals(user.DivisionId)).OrderBy(x=>x.ClassName).ToList(), "Id", "ClassName");
                ViewData["HJItemMasterClassId"] = new SelectList(_context.HJItemMasterClasses.Where(x=>x.DivisionId.Equals(user.DivisionId)).OrderBy(x=>x.ClassName).ToList(), "Id", "ClassName");
                return View(items);
            }
            else
            {
                var proj = await _context.Projects.FindAsync(ProjectId);
                var items = await _context.HJItems.Include(x => x.Locations).Include(x => x.Division).Include(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass)
                    .Where(x => x.Latitude != null && x.Longitude != null 
                    && x.DivisionId.Equals(user.DivisionId)
                    ).ToListAsync();
                List<HJItem> filtered_items = new List<HJItem>();
                foreach(var item in items)
                {
                    if(item.Locations.Count >= 1)
                    {
                        if(item.Locations.OrderByDescending(x => x.StartTime).First().ProjectId.Equals(ProjectId))
                        {
                            filtered_items.Add(item);
                        }
                        else if(DistanceAlgorithm.DistanceBetweenPlaces(Convert.ToDouble(item.Longitude),Convert.ToDouble(item.Latitude),Convert.ToDouble(proj.Longitude),Convert.ToDouble(proj.Latitude)) <= 0.5)
                        {
                            filtered_items.Add(item);
                        }
                    }
                }
                ViewData["ProjectId"] = await GetProjectList();
                ViewData["HJItemClassId"] = new SelectList(_context.HJItemClasses.Include(x => x.HJItemMasterClass).Where(x => x.HJItemMasterClass.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.ClassName).ToList(), "Id", "ClassName");
                ViewData["HJItemMasterClassId"] = new SelectList(_context.HJItemMasterClasses.Where(x => x.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.ClassName).ToList(), "Id", "ClassName");
                return View(filtered_items);
            }
        }
        [HttpGet]
        public async Task<JsonResult> getItems(string theId)
        {
            int id = Convert.ToInt32(theId);
            var items = await _context.HJItemClasses.Where(x => x.HJItemMasterClassId.Equals(id)).ToListAsync();
            return Json(items);
        }
        [HttpPost]
        public async Task<IActionResult> AddItemToProject([Bind("Id,HJItemId,StartTime,EndTime,ProjectId,SubProjectId")] Item_Location model)
        {
            if (ModelState.IsValid)
            {
                var previous_location = await _context.Item_Locations.Where(x => x.HJItemId.Equals(model.HJItemId)).OrderByDescending(x => x.StartTime).FirstOrDefaultAsync();
                if(previous_location != null)
                {
                    if (previous_location.EndTime == null)
                    {
                        previous_location.EndTime = model.StartTime.AddDays(-1);
                        _context.Update(previous_location);
                    }
                }
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = model.HJItemId });
            }
            else
            {
                return View(model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> AddItemToProject(int? id)
        {
            if(id != null)
            {
                var user = await _userManager.GetUserAsync(User);
                var theitem = await _context.HJItems.FindAsync(id);
                int nearestProjectId = 0;
                double smallest_dist = 1000000.0;
                int? nearestSubProjectId = null;
                double smallestsub_dist = 1000000.0;
                foreach (Project p in _context.Projects.Where(x=>x.DivisionId.Equals(user.DivisionId) && x.Active.Equals(true)))
                {
                    double distance = DistanceAlgorithm.DistanceBetweenPlaces(Convert.ToDouble(theitem.Latitude), Convert.ToDouble(theitem.Longitude), Convert.ToDouble(p.Latitude), Convert.ToDouble(p.Longitude));
                    if(distance < smallest_dist)
                    {
                        nearestProjectId = p.Id;
                        smallest_dist = distance;
                    }
                }
                var subprojects = await _context.SubProjects.Where(x => x.ProjectId.Equals(nearestProjectId)).ToListAsync();
                if(subprojects.Count > 0)
                {
                    foreach(SubProject p in subprojects)
                    {
                        double distance = DistanceAlgorithm.DistanceBetweenPlaces(Convert.ToDouble(theitem.Latitude), Convert.ToDouble(theitem.Longitude), Convert.ToDouble(p.Latitude), Convert.ToDouble(p.Longitude));
                        if (distance < smallest_dist)
                        {
                            nearestSubProjectId = p.Id;
                            smallestsub_dist = distance;
                        }
                    }
                }
                Item_Location loc = new Item_Location();
                loc.HJItemId = theitem.Id;
                loc.HJItem = theitem;
                loc.ProjectId = nearestProjectId;
                loc.SubProjectId = nearestSubProjectId;
                ViewData["ProjectId"] = await GetProjectList();
                loc.Id = 0;
                return View(loc);
            }
            else { return NotFound(); }
        }
        [HttpGet]
        public async Task<string> getItem(int id)
        {
            var item = await _context.HJItemClasses.FindAsync(id);
            return item.ClassName;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<string> UpdateCoords(int theid,double lati, double longi)
        {
            var theitem = await _context.HJItems.Include(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass).Where(x=>x.Id.Equals(theid)).FirstAsync();
            theitem.Latitude = lati;
            theitem.Longitude = longi;
            _context.Update(theitem);
            CoordTrack3 coordtrack = new CoordTrack3();
            coordtrack.Latitude = lati;
            coordtrack.Longitude = longi;
            coordtrack.HJItemId = theitem.Id;
            coordtrack.TimeStamp = DateTime.Now;
            coordtrack.Accuracy = 0;
            _context.CoordTrack3s.Add(coordtrack);
            await _context.SaveChangesAsync();
            var user = await _userManager.GetUserAsync(User);
            if(user != null)
            {
                return "isuser";
            }
            else { return "isnotuser"; }
            //return View();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCoordsOfAllItems()
        {
            var items = await _context.HJItems.ToListAsync();
            foreach (HJItem hji in items)
            {
                CoordTrack3 coord = new CoordTrack3
                {
                    Latitude = hji.Latitude,
                    Longitude = hji.Longitude,
                    HJItemId = hji.Id,
                    TimeStamp = DateTime.Now,
                    Accuracy = 0
                };
                _context.CoordTrack3s.Add(coord);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [AllowAnonymous]
        public async Task<IActionResult> DetailsAndUpdate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var hJItem = await _context.HJItems.Include(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass)
                .FirstOrDefaultAsync(m => m.Id == id);
            var user = await _userManager.GetUserAsync(User);
            if(user != null)
            {
                if (hJItem == null)
                {
                    return NotFound();
                }
                else if (!hJItem.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
                }
            }
            else
            {
                if (hJItem == null)
                {
                    return NotFound();
                }
            }
            

            return View(hJItem);
        }

        // GET: HJItems/Create
        [Authorize(Roles = "DivisionAdmin,Admin,Member,StorageManager")]
        public async Task<IActionResult> Create()
        {
            IEnumerable<SelectListItem> selList = await createFilterlist();
            ViewData["HJItemClassId"] = new SelectList(selList, "Value", "Text");
            return View();
        }
        public async Task<IActionResult> DetailsAll()
        {
            var theuser = await _userManager.GetUserAsync(User);
            var items = await _context.HJItems.Where(x=>x.DivisionId.Equals(theuser.DivisionId)).Include(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass).ToListAsync();
            return View(items);
        }
        [HttpGet]
        public async Task<IActionResult> DetailsSmallPdf(int? id)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (id != null)
            {
                var model = await _context.HJItems.Include(x=>x.Division).Include(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass).Where(x => x.Id.Equals(id) && x.DivisionId.Equals(theuser.DivisionId)).FirstAsync();
                return new ViewAsPdf("_DetailsSmall", model);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpGet]
        public async Task<IActionResult> DetailsSpecificPdf(int? id)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (id != null)
            {
                var model = await _context.HJItems.Include(x => x.Division).Include(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass).Where(x => x.Id.Equals(id) && x.DivisionId.Equals(theuser.DivisionId)).FirstAsync();
                return new ViewAsPdf("_DetailsSpecific", model);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpGet]
        public async Task<IActionResult> CreateNewItem(int? id)
        {
            if(id != null)
            {
                var user = await _userManager.GetUserAsync(User);
                var division = await _context.Divisions.FindAsync(user.DivisionId);
                var lastitem = await _context.HJItems.Where(x => x.HJItemClassId.Equals(id)).OrderBy(x => x.HJId).LastOrDefaultAsync();
                HJItem new_item = new HJItem();
                new_item.HJItemClassId = id;
                new_item.DivisionId = user.DivisionId;
                new_item.Ownership = division.Name;
                if(lastitem != null) { 
                    new_item.Weight = lastitem.Weight;
                    new_item.ItemHeight = lastitem.ItemHeight;
                    new_item.ItemLength = lastitem.ItemLength;
                    new_item.ItemWidth = lastitem.ItemWidth;
                    new_item.Name = lastitem.Name;
                }
                IEnumerable<SelectListItem> selList = await createFilterlist();
                ViewData["HJItemClassId"] = new SelectList(selList, "Value", "Text");
                return View("Create",new_item);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet]
        public async Task<IActionResult> DetailsSmallClassPdf(int? id)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (id != null)
            {
                var model = await _context.HJItems.Include(x => x.Division).Include(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass).Where(x => x.HJItemClassId.Equals(id) && x.DivisionId.Equals(theuser.DivisionId)).OrderBy(x=>x.HJId).ToListAsync();
                //return View("_DetailsClassSmall", model);
                return new ViewAsPdf("_DetailsClassSmall", model);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpGet]
        public async Task<IActionResult> DetailsSpecificClassPdf(int? id)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (id != null)
            {
                var model = await _context.HJItems.Include(x => x.Division).Include(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass).Where(x => x.HJItemClassId.Equals(id) && x.DivisionId.Equals(theuser.DivisionId)).OrderBy(x => x.HJId).ToListAsync();
                //return View("_DetailsClassSmall", model);
                return new ViewAsPdf("_DetailsClassSpecific", model);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
        public async Task<IActionResult> DetailsPdf(int? id)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if(id != null)
            {
                var model = await _context.HJItems.Include(x  => x.Division).Include(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass).Where(x=>x.Id.Equals(id) && x.DivisionId.Equals(theuser.DivisionId)).FirstAsync();
                ViewAsPdf pdf = new ViewAsPdf("_Details", model)
                {
                    FileName = model.HJId + ".pdf",
                    PageSize = Rotativa.AspNetCore.Options.Size.A5,
                    PageOrientation = Orientation.Landscape,
                    PageMargins = new Margins(0, 0, 0, 0),
                    CustomSwitches = "--disable-smart-shrinking"
                };
                return pdf;
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }

        }
        public async Task<IActionResult> DetailsPdf2(int? id)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (id != null)
            {
                var model = await _context.HJItems.Include(x => x.Division).Include(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass).Where(x => x.Id.Equals(id) && x.DivisionId.Equals(theuser.DivisionId)).FirstAsync();
                ViewAsPdf pdf = new ViewAsPdf("_Details", model)
                {
                    FileName = model.HJId + ".pdf",
                    PageHeight = 385,
                    PageSize = Rotativa.AspNetCore.Options.Size.A5,
                    PageOrientation = Orientation.Landscape,
                    PageMargins = new Margins(0, 0, 0, 0),
                    CustomSwitches = "--disable-smart-shrinking"


                };
                return pdf;
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }

        }
        [Authorize]
        public async Task<IActionResult> DetailsAllPdf()
        {
            var theuser = await _userManager.GetUserAsync(User);
            var model = await _context.HJItems.Include(x => x.Division).Include(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass).Where(x=>x.DivisionId.Equals(theuser.DivisionId)).Include(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass).ToListAsync();
            return new ViewAsPdf("_DetailsAll", model);
        }
        [Authorize]
        public async Task<IActionResult> DetailsAllPdf2()
        {
            var theuser = await _userManager.GetUserAsync(User);
            var model = await _context.HJItems.Include(x => x.Division).Include(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass).Where(x=>x.DivisionId.Equals(theuser.DivisionId)).Include(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass).ToListAsync();
            return new ViewAsPdf("_DetailsAll2", model);
        }

        // POST: HJItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "DivisionAdmin,Admin,Member,StorageManager")]
        public async Task<IActionResult> Create([Bind("Id,Name,weight,ItemLength,ItemWidth,ItemHeight,HJItemClassId,PathToPicture,PathTo3DDrawing,Comments,Ownership,GPS_Tracker")] HJItem hJItem)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                try
                {
                    var lastitem = await _context.HJItems.Where(x=>x.HJItemClassId.Equals(hJItem.HJItemClassId)).OrderByDescending(x=>x.HJId).FirstAsync();
                    var itemclass = await _context.HJItemClasses.Include(x => x.HJItemMasterClass).Where(x => x.Id.Equals(hJItem.HJItemClassId)).FirstAsync();
                    var masterclass = itemclass.HJItemMasterClass;
                    if (lastitem != null)
                    {
                        hJItem.SetHJNumber(Convert.ToInt32(lastitem.HJId.Split("-")[2]),masterclass.ClassNumber,itemclass.ClassNumber);
                    }
                    else
                    {
                        hJItem.SetHJNumber(0, masterclass.ClassNumber, itemclass.ClassNumber);
                    }
                }
                catch
                {
                    var itemclass = await _context.HJItemClasses.Include(x => x.HJItemMasterClass).Where(x => x.Id.Equals(hJItem.HJItemClassId)).FirstAsync();
                    var masterclass = itemclass.HJItemMasterClass;
                    hJItem.SetHJNumber(0, masterclass.ClassNumber, itemclass.ClassNumber);
                }
                hJItem.DivisionId = theuser.DivisionId;
                _context.Add(hJItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            IEnumerable<SelectListItem> selList = await createFilterlist();
            ViewData["HJItemClassId"] = new SelectList(selList, "Value", "Text");
            ViewData["DivisionId"] = new SelectList(_context.Divisions, "Id", "Name");
            return View(hJItem);
        }

        // GET: HJItems/Edit/5
        [Authorize(Roles = "DivisionAdmin,Admin,Member,StorageManager")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var hJItem = await _context.HJItems.Include(x=>x.Division).Include(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass).Where(x=>x.Id.Equals(id)).FirstOrDefaultAsync();
            if (hJItem == null)
            {
                return NotFound();
            }
            else if(!hJItem.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
            }
            IEnumerable<SelectListItem> selList = await createFilterlist();
            ViewData["HJItemClassId"] = new SelectList(selList, "Value", "Text");
            ViewData["DivisionId"] = new SelectList(_context.Divisions, "Id", "Name");
            //photos part
            List<string> pathstofotos = new List<string>();
            if (hJItem.PathToPicture != "" && hJItem.PathToPicture != null)
            {
                try
                {
                    foreach (string fileName in Directory.GetFiles(hJItem.PathToPicture))
                    {
                        //pathstofotos.Add(Path.Combine(_env.WebRootPath.ReplaceFirst("/", ""), "images", "Items", "Photos", hJItem.Id.ToString()) + $@"\{fileName.Split("\\").Last()}");
                        pathstofotos.Add(fileName.Split("\\").Last());
                    }
                }
                catch
                {

                }
            }
            ViewBag.Photos = pathstofotos;
            List<string> pathsgeneralphotos = new List<string>();
            try
            {
                var directory = Path.Combine(_env.WebRootPath.ReplaceFirst("/", ""), "Images", "ItemClasses", "Photos", hJItem.HJItemClassId.ToString());
                foreach (string fileName in Directory.GetFiles(directory))
                {
                    if (fileName.Contains("edit"))
                    {
                        pathsgeneralphotos.Add(Path.Combine(_env.WebRootPath.ReplaceFirst("/", ""), "images", "ItemClasses", "Photos", hJItem.HJItemClassId.ToString()) + $@"\{fileName.Split("\\").Last()}");
                    }
                }
            }
            catch
            {

            }
            ViewBag.GeneralPhotos = pathsgeneralphotos;
            List<string> pathstodrawings = new List<string>();
            if (hJItem.PathTo3DDrawing != "" && hJItem.PathTo3DDrawing != null)
            {
                try
                {
                    foreach (string fileName in Directory.GetFiles(hJItem.PathTo3DDrawing))
                    {
                        pathstodrawings.Add(Path.Combine(_env.WebRootPath.ReplaceFirst("/", ""), "images", "Items", "Drawings", hJItem.Id.ToString()) + $@"\{fileName.Split("\\").Last()}");
                    }
                }
                catch
                {

                }
            }
            ViewBag.Drawings = pathstodrawings;
            //
            return View(hJItem);
        }
        [HttpPost]
        public IActionResult RemovePhoto(string theid, string thephoto)
        {
            var path = Path.Combine(_env.WebRootPath.ReplaceFirst("/", ""), "Images", "Items", "Photos", theid);
            System.IO.File.Delete(path + "\\" + thephoto);
            if (thephoto.Contains("_edit"))
            {
                string fileextension = thephoto.Split(".")[1];
                string potentialoriginal = path + thephoto.Split("_edit")[0] + "." + fileextension;
                if (System.IO.File.Exists(potentialoriginal))
                {
                    System.IO.File.Delete(potentialoriginal);
                }
            }
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> DownloadPhoto(string theid, string thephoto)
        {
            string fileextension = thephoto.Split(".")[1];
            string potentialoriginal = thephoto.Split("_edit")[0] + "." + fileextension;
            var botsFolderPath = Path.Combine(_env.WebRootPath.ReplaceFirst("/", ""), "Images", "Items", "Photos", theid);
            var botFilePaths = Directory.GetFiles(botsFolderPath);
            var zipFileMemoryStream = new MemoryStream();
            using (ZipArchive archive = new ZipArchive(zipFileMemoryStream, ZipArchiveMode.Update, leaveOpen: true))
            {
                foreach (var botFilePath in botFilePaths)
                {
                    if (botFilePath.ToLower().Contains(thephoto.ToLower()) || botFilePath.ToLower().Contains(potentialoriginal.ToLower()))
                    {
                        var botFileName = Path.GetFileName(botFilePath);
                        var entry = archive.CreateEntry(botFileName);
                        using (var entryStream = entry.Open())
                        using (var fileStream = System.IO.File.OpenRead(botFilePath))
                        {
                            await fileStream.CopyToAsync(entryStream);
                        }
                    }
                }
            }
            zipFileMemoryStream.Seek(0, SeekOrigin.Begin);
            return File(zipFileMemoryStream, "application/octet-stream", "Photos.zip");

        }
        public async Task<IEnumerable<SelectListItem>> createFilterlist()
        {
            var user = await _userManager.GetUserAsync(User);
            var filternames = await _context.HJItemClasses.Include(x=>x.HJItemMasterClass).Where(x=>x.HJItemMasterClass.DivisionId.Equals(user.DivisionId)).OrderBy(b => b.HJItemMasterClass.ClassNumber).ThenBy(x=>x.ClassNumber).ToListAsync();


            IEnumerable<SelectListItem> selList = from s in filternames
                                                  select new SelectListItem
                                                  {
                                                      Value = s.Id.ToString(),
                                                      Text = s.HJItemMasterClass.ClassName + "-" + s.ClassNumber + " : " + s.ClassName
                                                  };
            return selList;
        }
        [HttpPost]
        [Authorize(Roles = "Admin,DivisionAdmin,StorageManager")]
        public void PutOnInternationalList(int? id)
        {
            if(id != null)
            {
                var hjitem = _context.HJItems.Find(id);
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,StorageManager")]
        public async Task<IActionResult> Maintenances(int? id)
        {
            
            if (id != null) { 
            var user = await _userManager.GetUserAsync(User);
                List<MaintenanceWithEntry> maintenanceviews = new List<MaintenanceWithEntry>();
            var maintenances = await _context.Maintenances
                .Include(x => x.Project)
                .Include(x => x.MeasPoint)
                .Include(x => x.SubProject)
                .Include(x => x.HJItem)
                .Include(x => x.Install)
                .Include(x => x.MaintenanceEntries).ThenInclude(x => x.MaintenanceType)
                .Include(x => x.MaintenanceEntries).ThenInclude(x => x.MaintenanceSubType)
                .Where(x => x.Project.DivisionId.Equals(user.DivisionId) && x.HJItemId.Equals(id))
                .OrderByDescending(x => x.TimeStamp)
                .ToListAsync();
                foreach (var m in maintenances)
                {
                    foreach (var m_entry in m.MaintenanceEntries)
                    {
                        MaintenanceWithEntry entry = new MaintenanceWithEntry(m, m_entry);
                        if (m.MaintenancePhotos.Count() > 0)
                        {
                            entry.HasPhotos = true;
                        }
                        else
                        {
                            entry.HasPhotos = false;
                        }
                        maintenanceviews.Add(entry);
                    }
                }
                var installations = await _context.Installations
                .Include(x => x.ItemType)
                .Include(x => x.Project)
                .Where(x => x.isInstalled.Equals(true) && x.Project.DivisionId.Equals(user.DivisionId))
                .ToListAsync();
            IEnumerable<SelectListItem> selList = from s in installations
                                                  select new SelectListItem
                                                  {
                                                      Value = s.Id.ToString(),
                                                      Text = s.Project.Name + " ID: " + s.Id + " : " + s.ItemType.Item_Type
                                                  };
            ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
            ViewData["TitleId"] = new SelectList(_context.Titles.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "TheTitle");
            ViewData["InstallId"] = new SelectList(selList, "Value", "Text");
            ViewData["MaintenanceTypeId"] = new SelectList(_context.MaintenanceTypes, "Id", "Type");
            ViewData["MaintenanceSubTypeId"] = new SelectList(_context.MaintenanceSubTypes, "Id", "Type");
            var hjitems = await createHJItemlist();
            ViewData["HJItemId"] = new SelectList(hjitems, "Value", "Text");
            return View(maintenanceviews);
            }
            else
            {
                return NotFound();
            }
        }
        public async Task<IEnumerable<SelectListItem>> createHJItemlist()
        {
            var user = await _userManager.GetUserAsync(User);
            var filternames = await _context.HJItems.Where(x=>x.DivisionId.Equals(user.DivisionId)).Include(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass).OrderBy(b => b.HJItemClass.HJItemMasterClass.ClassNumber).ThenBy(x => x.HJItemClass.ClassNumber).ThenBy(x => x.HJId).ToListAsync();


            IEnumerable<SelectListItem> selList = from s in filternames
                                                  select new SelectListItem
                                                  {
                                                      Value = s.Id.ToString(),
                                                      Text = s.HJId + " : " + s.Name
                                                  };
            return selList;
        }

        // POST: HJItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "DivisionAdmin,Admin,Member,StorageManager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,HJId,weight,ItemLength,ItemWidth,ItemHeight,HJItemClassId,latitude,longitude,PathToPicture,PathTo3DDrawing,Comments,DivisionId,Ownership,GPS_Tracker")] HJItem hJItem)
        {
            if (id != hJItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hJItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HJItemExists(hJItem.Id))
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
            IEnumerable<SelectListItem> selList = await createFilterlist();
            ViewData["HJItemClassId"] = new SelectList(selList, "Value", "Text");
            return View(hJItem);
        }

        // GET: HJItems/Delete/5
        [Authorize(Roles = "DivisionAdmin,Admin,Member,StorageManager")]
        public async Task<IActionResult> Delete(int? id)
        {
            var theuser = await _userManager.GetUserAsync(User);

            if (id == null)
            {
                return NotFound();
            }

            var hJItem = await _context.HJItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hJItem == null)
            {
                return NotFound();
            }

            return View(hJItem);
        }

        // POST: HJItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hJItem = await _context.HJItems.FindAsync(id);
            _context.HJItems.Remove(hJItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HJItemExists(int id)
        {
            return _context.HJItems.Any(e => e.Id == id);
        }
        private static byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
        public async Task<ActionResult> GetQR(int? Id)
        {
            if(Id != null)
            {
                string webRootPath = _env.WebRootPath;
                var item = await _context.HJItems.FindAsync(Id);
                var routeUrl = Url.Action("DetailsAndUpdate","HJItems", new { id = Id });
                var absUrl = string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, routeUrl);
                //var uri = new Uri(absUrl, UriKind.Absolute);

                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(absUrl, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20);
                var bitmapBytes = BitmapToBytes(qrCodeImage); //Convert bitmap into a byte array
                return File(bitmapBytes, "image/jpeg"); //Return as file result
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<ActionResult> GetQR2(int? Id)
        {
            if (Id != null)
            {
                string webRootPath = _env.WebRootPath;
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
        public async Task<ActionResult> GetQR3(int? Id)
        {
            //var user = await _userManager.GetUserAsync(User);
            if (Id != null)
            {
                string webRootPath = _env.WebRootPath;
                var item = await _context.MeasPoints.FindAsync(Id);
                var routeUrl = Url.Action("CreateMeas", "Meas", new { id = Id });
                var absUrl = string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, routeUrl);
                //var uri = new Uri(absUrl, UriKind.Absolute);
                //if (user.DivisionId.Equals(4))
                //{
                //    absUrl = absUrl.ToLower().Replace("mainops.azurewebsites.net", "tjaden-maps.nl");
                //    absUrl = absUrl.ToLower().Replace("hj-mainops.com", "tjaden-maps.nl");
                //}
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(absUrl, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20);
                var bitmapBytes = BitmapToBytes(qrCodeImage);
                return File(bitmapBytes, "image/jpeg");
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<ActionResult> GetQR4(int? Id)
        {
            if (Id != null)
            {
                string webRootPath = _env.WebRootPath;
                var item = await _context.TrackItems.FindAsync(Id);
                var routeUrl = Url.Action("Main", "TrackItems", new { id = Id });
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
        [HttpPost]
        [Authorize(Roles = "DivisionAdmin,Admin,Member,StorageManager")]
        public async Task<IActionResult> UploadPhotoHJItem(int? id)
        {
            if(id != null)
            {

                var item = await _context.HJItems.FindAsync(id);
                //var user = await _userManager.GetUserAsync(User);
                var folderpath = Path.Combine(_env.WebRootPath.ReplaceFirst("/", ""), "Images", "Items","Photos",id.ToString());
                if (!Directory.Exists(folderpath))
                {
                    Directory.CreateDirectory(folderpath);
                }
                if (HttpContext.Request.Form.Files != null)
                {
                    var files = HttpContext.Request.Form.Files;

                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            //Getting FileName
                            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            //fileName = user.FirstName + user.LastName + fileName;
                            // Combines two strings into a path.
                            fileName = folderpath + $@"\{fileName}";
                            item.PathToPicture = folderpath;
                            using (FileStream fs = System.IO.File.Create(fileName))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }
                            _context.Update(item);
                            await _context.SaveChangesAsync();

                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "DivisionAdmin,Admin,Member,StorageManager")]
        [HttpPost]
        public async Task<IActionResult> UploadDrawingHJItem(int? id)
        {
            if (id != null)
            {

                var item = await _context.HJItems.FindAsync(id);
                //var user = await _userManager.GetUserAsync(User);
                var folderpath = Path.Combine(_env.WebRootPath.ReplaceFirst("/", ""), "Images", "Items","Drawings", id.ToString());
                if (!Directory.Exists(folderpath))
                {
                    Directory.CreateDirectory(folderpath);
                }
                if (HttpContext.Request.Form.Files != null)
                {
                    var files = HttpContext.Request.Form.Files;

                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            //Getting FileName
                            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            //fileName = user.FirstName + user.LastName + fileName;
                            // Combines two strings into a path.
                            fileName = folderpath + $@"\{fileName}";
                            item.PathTo3DDrawing = folderpath;
                            using (FileStream fs = System.IO.File.Create(fileName))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }
                            _context.Update(item);
                            await _context.SaveChangesAsync();

                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Combsearch(string searchstring, string filterchoice)
        {
            int f_c_converted;
            f_c_converted = Convert.ToInt32(filterchoice);
            var theuser = await _userManager.GetUserAsync(User);
            IEnumerable<SelectListItem> selList = await createFilterlist();
            ViewData["Filterchoices"] = new SelectList(selList, "Value", "Text");
            List<HJItem> items = new List<HJItem>();

            if (string.IsNullOrEmpty(searchstring) && (string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
            {
                return RedirectToAction(nameof(Index));
            }
            else if (string.IsNullOrEmpty(searchstring) && (!string.IsNullOrEmpty(filterchoice) || !filterchoice.Equals("All")))
            {
                items = await _context.HJItems.Where(x => x.HJItemClassId.Equals(f_c_converted)).ToListAsync();
            }
            else if (!string.IsNullOrEmpty(searchstring) && (string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
            {
                items = await _context.HJItems.Where(x => x.HJItemClass.ClassName.ToLower().Contains(searchstring.ToLower()) || x.HJId.Contains(searchstring.ToLower())).ToListAsync();
            }
            else
            {
                items = await _context.HJItems.Where(x => (x.HJItemClass.ClassName.ToLower().Contains(searchstring.ToLower()) || x.HJId.Contains(searchstring.ToLower())) || x.HJId.Contains(searchstring.ToLower()) && x.HJItemClassId.Equals(f_c_converted)).ToListAsync();
            }
            return View(nameof(Index),items);
        }
        [HttpGet]
        public async Task<IActionResult> MaintenanceToDo()
        {
            var user = await _userManager.GetUserAsync(User);
            var hjitems = await _context.HJItems.Include(x => x.HJItemClass).Include(x=>x.MaintenanceList).Where(x => x.DivisionId.Equals(user.DivisionId)).ToListAsync();
            List<HJItemMaintenaceVM> MainToDoList = new List<HJItemMaintenaceVM>();
            foreach(var item in hjitems)
            {
                HJItemMaintenaceVM vm_item = new HJItemMaintenaceVM(item);
                MainToDoList.Add(vm_item);
            }
            return View(MainToDoList.OrderBy(x=>x.Next_Check));
        }
    }
}
