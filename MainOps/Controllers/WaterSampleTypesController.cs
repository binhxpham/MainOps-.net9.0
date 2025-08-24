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
using Microsoft.AspNetCore.Authorization;
using System.IO;
using MainOps.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Rotativa.AspNetCore;
using MainOps.ExtensionMethods;
using System.Globalization;
using System.Text;
using Rotativa.AspNetCore.Options;
using MainOps.Resources;
using Microsoft.AspNetCore.Hosting;
using System.IO.Compression;

namespace MainOps.Controllers
{
    public class WaterSampleTypesController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LocService _sharedLocalizer;
        private readonly IWebHostEnvironment _env;

        public WaterSampleTypesController(DataContext context,UserManager<ApplicationUser> userManager,LocService localizer, IWebHostEnvironment env):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
            _sharedLocalizer = localizer;
            _env = env;
        }

        // GET: WaterSampleTypes
        public async Task<IActionResult> Index()
        {
            ViewData["ProjectId"] = await GetProjectList();
            var user = await _userManager.GetUserAsync(User);
            ViewData["WaterSamplePlaceId"] = new SelectList(_context.WaterSamplePlaces.Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
            return View(await _context.WaterSampleTypes.ToListAsync());
        }
        public IActionResult UploadSamples()
        {
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
            return View();
        }
        
        [Authorize(Roles = "Admin,Member")]
        [HttpPost]
        public async Task<ActionResult> UploadSamples(UploadSamples model, IFormFile postedFile)
        {
            if (postedFile != null)
            {
                string fileExtension = Path.GetExtension(postedFile.FileName);
                var user = await _userManager.GetUserAsync(User);
                //Validate uploaded file and return error.
                if (fileExtension != ".csv")
                {
                    return View("Index");
                }
                string testname = "";
                string sampletaker = "";
                DateTime DateReceived = DateTime.Now;
                DateTime DateTaken = DateTime.Now;
                
                int pId = model.ProjectId;
                int sampleId = model.WaterSamplePlaceId;
                int rowcounter = 1;
                
                using (var sreader = new StreamReader(postedFile.OpenReadStream(), System.Text.Encoding.GetEncoding("iso-8859-1")))
                {
                    while (!sreader.EndOfStream)
                    {
                        string[] rows = sreader.ReadLine().Split(';');
                        string[] formats = { "dd-MM-yyyy HH:mm", "yyyy-MM-dd HH:mm","dd-MM-yyyy","yyyy-MM-dd","dd.MM.yyyy","dd.MM.yyyy HH:mm" };
                        if (rows.Length < 2)
                        {
                            rows = sreader.ReadLine().Split(',');
                        }

                        if (rows[1] != "")
                        {
                            if (rows[0].ToLower().Contains("batch"))
                            {
                                testname = rows[1];
                            }
                            if (rows[0].ToLower().Contains("udtagning"))
                            {
                                DateTaken = DateTime.ParseExact(rows[1].Split("/")[0], formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                                try
                                {
                                    sampletaker = rows[1].Split(",")[1];
                                }
                                catch
                                {
                                    try
                                    {
                                        sampletaker = rows[1].Split("/")[1];
                                    }
                                    catch
                                    {
                                        sampletaker = "default";
                                    }                                  
                                }
                            }
                            if (rows[0].ToLower().Contains("modtaget"))
                            {
                                DateReceived = DateTime.ParseExact(rows[1], formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                            }
                            if (!rows[0].ToLower().Contains("prøve") && rowcounter > 11)
                            {
                                var wts = await _context.WaterSampleTypes.Where(x => x.Komponent.ToLower().Equals(rows[0].ToLower())).ToListAsync();
                                if (wts.Count() > 0)
                                {
                                    if (wts.First().Id.Equals(451))
                                    {
                                        //akkrediteret blah blah
                                    }
                                    else {
                                        double thevalue;
                                        try
                                        {
                                            thevalue = Convert.ToDouble(rows[1].Replace(",", ".").Replace("<", "").Trim());
                                        }
                                        catch
                                        {
                                            thevalue = 0.0;
                                        }
                                        var prev_ws = await _context.WaterSampleMeasures.OrderBy(x => x.Id).LastOrDefaultAsync(x => x.ProjectId.Equals(pId)
                                        && x.WaterSamplePlaceId.Equals(sampleId) && x.WaterSampleNumber.Equals(testname) && x.WaterSampleTypeId.Equals(wts.First().Id));
                                        if(prev_ws == null) { 
                                        WaterSampleMeas Measure = new WaterSampleMeas
                                        {
                                            ProjectId = pId,
                                            value = thevalue,
                                            Dato = model.Dato,
                                            WaterSampleTypeId = wts.First().Id,
                                            method = rows[4],
                                            WaterSamplePlaceId = sampleId,
                                            WaterSampleNumber = testname,
                                            SampleTakerName = sampletaker,
                                            DateLabReceived = DateReceived,
                                            DateNextSample = model.NextSample

                                        };
                                        _context.WaterSampleMeasures.Add(Measure);
                                        }
                                        else
                                        {
                                            prev_ws.value = thevalue;
                                            prev_ws.method = rows[4];
                                            prev_ws.DateLabReceived = DateReceived;
                                            prev_ws.DateNextSample = model.NextSample;
                                            prev_ws.SampleTakerName = sampletaker;
                                            _context.Update(prev_ws);
                                        

                                        }
                                    }
                                }

                                else
                                {
                                    WaterSampleType WT = new WaterSampleType();
                                    if (rows[3].Trim() != "")
                                    {
                                        WT.Komponent = rows[0];
                                        WT.Enhed = rows[2];
                                        WT.DL = Convert.ToDouble(rows[3].Replace(",", "."));
                                        _context.WaterSampleTypes.Add(WT);
                                    }
                                    else
                                    {
                                        WT.Komponent = rows[0];
                                        WT.Enhed = rows[2];
                                        WT.DL = 0.0;
                                        _context.WaterSampleTypes.Add(WT);
                                    }

                                    await _context.SaveChangesAsync();
                                    WaterSampleType LastAdded = await _context.WaterSampleTypes.Where(x => x.Komponent.Equals(rows[0])).FirstAsync();
                                    double thevalue;
                                    try
                                    {
                                        thevalue = Convert.ToDouble(rows[1].Replace(",", ".").Replace("<", "").Trim());
                                    }
                                    catch
                                    {
                                        thevalue = 0.0;
                                    }
                                    WaterSampleMeas Measure = new WaterSampleMeas
                                    {
                                        ProjectId = pId,
                                        value = thevalue,
                                        Dato = model.Dato,
                                        WaterSampleTypeId = LastAdded.Id,
                                        method = rows[4],
                                        WaterSamplePlaceId = model.WaterSamplePlaceId,
                                        WaterSampleNumber = testname,
                                        SampleTakerName = sampletaker,
                                        DateLabReceived = DateReceived,
                                        DateNextSample = model.NextSample
                                    };
                                    _context.WaterSampleMeasures.Add(Measure);
                                }
                            }
                        }
                        rowcounter += 1;
                    }
                    await _context.SaveChangesAsync();
                    var watersampleplace = await _context.WaterSamplePlaces.Include(x => x.Project).SingleOrDefaultAsync(x => x.Id.Equals(model.WaterSamplePlaceId));
                    var itemtypes = await _context.ItemTypes.Where(x => x.ProjectId.Equals(watersampleplace.ProjectId) && x.ReportTypeId.Equals(5)).ToListAsync();

                    if(itemtypes.Count != 0)
                    {
                        var measures = await _context.WaterSampleMeasures.Where(x => x.Dato.Equals(model.Dato) && x.WaterSamplePlaceId.Equals(model.WaterSamplePlaceId) && !x.WaterSampleTypeId.Equals(451)).ToListAsync();
                        string sample = await WaterSampleDone(watersampleplace.ProjectId, measures);
                        
                        string thesampleitemtype = "(" + sample + ")";
                        string theuniqueid = String.Concat(watersampleplace.Name, " : ", model.Dato.ToString("yyyy-MM-dd")," (", sample, ")");
                        var prev_install = await _context.Installations.Where(x => x.ItemTypeId.Equals(itemtypes.First()) && x.UniqueID.Equals(theuniqueid)).FirstOrDefaultAsync();
                        
                        if (prev_install == null)
                        {
                            if(itemtypes.Count > 1)
                            {
                                if(sample != "") {
                                    try
                                    {

                                  
                                        Install inst = new Install
                                        {
                                            ToBePaid = true,
                                            ItemTypeId = itemtypes.ElementAt(itemtypes.FindIndex(x => x.Item_Type.Contains(thesampleitemtype))).Id,
                                            Latitude = 0,
                                            Longitude = 0,
                                            TimeStamp = model.Dato.Date,
                                            RentalStartDate = model.Dato.Date,
                                            InvoiceDate = DateTime.Now,
                                            Install_Text = "",
                                            isInstalled = true,
                                            Amount = 1,
                                            UniqueID = theuniqueid,
                                            ProjectId = model.ProjectId,
                                            SubProjectId = null,
                                            EnteredIntoDataBase = DateTime.Now,
                                            LastEditedInDataBase = DateTime.Now,
                                            DoneBy = user.full_name()
                                        };
                                        _context.Installations.Add(inst);
                                        await _context.SaveChangesAsync();
                                    }
                                    catch
                                    {

                                    }
                                }
                                else
                                {
                                    Install inst = new Install
                                    {
                                        ToBePaid = true,
                                        ItemTypeId = itemtypes.First().Id,
                                        Latitude = 0,
                                        Longitude = 0,
                                        TimeStamp = model.Dato.Date,
                                        RentalStartDate = model.Dato.Date,
                                        InvoiceDate = DateTime.Now,
                                        Install_Text = "",
                                        isInstalled = true,
                                        Amount = 1,
                                        UniqueID = theuniqueid.Split("(")[0] + "(X)",
                                        ProjectId = model.ProjectId,
                                        SubProjectId = null,
                                        EnteredIntoDataBase = DateTime.Now,
                                        LastEditedInDataBase = DateTime.Now,
                                        DoneBy = user.full_name()
                                    };
                                    _context.Installations.Add(inst);
                                    await _context.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                Install inst = new Install
                                {
                                    ToBePaid = true,
                                    ItemTypeId = itemtypes.First().Id,
                                    Latitude = 0,
                                    Longitude = 0,
                                    TimeStamp = model.Dato.Date,
                                    RentalStartDate = model.Dato.Date,
                                    InvoiceDate = DateTime.Now,
                                    Install_Text = "",
                                    isInstalled = true,
                                    Amount = 1,
                                    UniqueID = theuniqueid,
                                    ProjectId = model.ProjectId,
                                    SubProjectId = null,
                                    EnteredIntoDataBase = DateTime.Now,
                                    LastEditedInDataBase = DateTime.Now,
                                    DoneBy = user.full_name()
                                };
                                _context.Installations.Add(inst);
                                await _context.SaveChangesAsync();
                            }
                            
                        }
                    }
                    
                }
                ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
                ViewData["WaterSamplePlaceId"] = new SelectList(_context.WaterSamplePlaces.Where(x => x.ProjectId.Equals(pId)), "Id", "Name");
                return View("UploadSamples");
            }
            else
            {
                ViewBag.Message = "Please select the file first to upload.";
            }
            return View();
        }
        [HttpGet]
        public async Task<JsonResult> checklimit(int id)
        {
            var themeas = await _context.WaterSampleMeasures.Where(x => x.Id.Equals(id)).SingleOrDefaultAsync();
            if(themeas != null)
            { 
                var thelimit = await _context.WaterSampleLimits.Where(x => x.WaterSamplePlaceId.Equals(themeas.WaterSamplePlaceId) && x.WaterSampleTypeId.Equals(themeas.WaterSampleTypeId)).ToListAsync();
                if(thelimit.Count() < 1)
                {
                    var thetype = await _context.WaterSampleTypes.Where(x => x.Id.Equals(themeas.WaterSampleTypeId)).SingleOrDefaultAsync();
                    var theplace = await _context.WaterSamplePlaces.Where(x => x.Id.Equals(themeas.WaterSamplePlaceId)).SingleOrDefaultAsync();
                    var thestandardLimit = await _context.StandardLimits.Where(x => x.Komponent.Equals(thetype.Komponent) && x.WaterSamplePlaceTypeId.Equals(theplace.WaterSamplePlaceTypeId)).SingleOrDefaultAsync();
                    if (thestandardLimit != null)
                    {
                        if (thestandardLimit.MaxLimit >= themeas.value)
                        {
                            return Json(new { data = 1 });
                        }
                        else
                        {
                            return Json(new { data = 2 });
                        }
                    }
                    else
                    {
                        return Json(new { data = 0 });
                    }
                    
                }
                else
                {
                    if(thelimit.First().Limit >= themeas.value)
                    {
                        return Json(new { data = 1 });
                    }
                    else
                    {
                        return Json(new { data = 2 });
                    }
                }
            }
            else
            {
                return Json(new { data = 2 });
            }
        }
        [HttpGet]
        public async Task<IActionResult> UploadSamplePackage()
        {
            ViewData["ProjectId"] = await GetProjectList();
            UploadPackageViewModel model = new UploadPackageViewModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UploadSamplePackage(UploadPackageViewModel model, IFormFile postedFile)
        {
            if (postedFile != null)
            {
                string fileExtension = Path.GetExtension(postedFile.FileName);
                string ListOfComponents = "";
                if (fileExtension != ".csv")
                {
                    return View(model);
                }
                WaterSamplePackage WSP;
                WaterSamplePackage package;
                List<WaterSampleType> wts;
                List<WaterSampleTypeWaterSamplePackage> List_WSTWSP = new List<WaterSampleTypeWaterSamplePackage>();
                var maybealready = await _context.WaterSamplePackages.Where(x => x.Annotation.Equals(model.Annotation) && x.ProjectId.Equals(model.ProjectId)).SingleOrDefaultAsync();               
                if(maybealready != null)
                {
                    package = maybealready;
                }
                else
                {
                    WSP = new WaterSamplePackage();
                    WSP.ProjectId = model.ProjectId;
                    WSP.Annotation = model.Annotation;
                    _context.Add(WSP);
                    await _context.SaveChangesAsync();
                    package = await _context.WaterSamplePackages.LastAsync();
                } 
                using (var sreader = new StreamReader(postedFile.OpenReadStream()))
                {
                    while (!sreader.EndOfStream)
                    {
                        try { 
                            string[] rows = sreader.ReadLine().Split(';');
                            if (rows.Length < 2)
                            {
                                rows = sreader.ReadLine().Split(',');
                            }
                            if (rows[1] != "")
                            {
                                wts = await _context.WaterSampleTypes.Where(x => x.Komponent.ToLower().Equals(rows[0].ToLower())).ToListAsync();
                                if (wts.Count() > 0)
                                {
                                    if (wts.First().Id.Equals(451))
                                    {

                                    }
                                    else { 
                                        var maybealreadyWSWSP = await _context.WaterSampleTypeWaterSamplePackages.Where(x => x.WaterSamplePackageId.Equals(package.Id) && x.WaterSampleTypeId.Equals(wts.First().Id)).SingleOrDefaultAsync();
                                        if(maybealreadyWSWSP == null)
                                        {
                                            WaterSampleTypeWaterSamplePackage WSWSP = new WaterSampleTypeWaterSamplePackage(package, wts.First()); 
                                            //_context.Add(WSWSP);
                                            if(ListOfComponents == "")
                                            {
                                                ListOfComponents = wts.First().Komponent;
                                            }
                                            else
                                            {
                                                ListOfComponents += "," + wts.First().Komponent;
                                            }
                                            if(List_WSTWSP.FindIndex(x=>x.WaterSamplePackageId == package.Id && x.WaterSampleTypeId == wts.First().Id) == -1) {
                                                _context.Add(WSWSP);
                                                List_WSTWSP.Add(WSWSP);
                                            }
                                        
                                        }
                                    }
                                }
                                else
                                {
                                    WaterSampleType WT = new WaterSampleType();
                                    if (rows[3].Trim() != "")
                                    {
                                        WT.Komponent = rows[0];
                                        WT.Enhed = rows[2];
                                        WT.DL = Convert.ToDouble(rows[3].Replace(",", "."));
                                        _context.WaterSampleTypes.Add(WT);
                                    }
                                    else
                                    {
                                        WT.Komponent = rows[0];
                                        WT.Enhed = rows[2];
                                        WT.DL = 0.0;
                                        _context.WaterSampleTypes.Add(WT);
                                    }
                                    WaterSampleType WST = await _context.WaterSampleTypes.Where(x => x.Komponent.Equals(rows[0])).FirstAsync();
                                    WaterSampleTypeWaterSamplePackage WSWSP = new WaterSampleTypeWaterSamplePackage(package, WST);
                                    if (ListOfComponents == "")
                                    {
                                        ListOfComponents = WST.Komponent;
                                    }
                                    else
                                    {
                                        ListOfComponents += "," + WST.Komponent;
                                    }
                                    if (List_WSTWSP.FindIndex(x => x.WaterSamplePackageId == package.Id && x.WaterSampleTypeId == wts.First().Id) == -1)
                                    {
                                        _context.Add(WSWSP);
                                        List_WSTWSP.Add(WSWSP);
                                    }
                                    

                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                    package.ListOfComponents = ListOfComponents;
                    _context.Update(package);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            else { return View(model); }
        }
        [HttpGet]
        public async Task<IActionResult> CreatePackage()
        {
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["WaterSampleTypeId"] = new SelectList(_context.WaterSampleTypes.Where(x => !x.Id.Equals(451)), "Id", "Komponent");
            CreatePackageViewModel model = new CreatePackageViewModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> CreatePackage(CreatePackageViewModel model)
        {
            WaterSamplePackage package = new WaterSamplePackage(model.ProjectId,model.Annotation,model.WaterSampleTypesNames);
            _context.Add(package);
            await _context.SaveChangesAsync();
            var lastadded = await _context.WaterSamplePackages.LastAsync();
            foreach(string s in model.WaterSampleTypes.Split(","))
            {
                WaterSampleTypeWaterSamplePackage new_item = new WaterSampleTypeWaterSamplePackage();
                new_item.WaterSamplePackageId = lastadded.Id;
                new_item.WaterSampleTypeId = Convert.ToInt32(s);
                _context.Add(new_item);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Delete_Package(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if(id != null)
            {
                var package = await _context.WaterSamplePackages.Include(x=>x.Project).Include(x=>x.WaterSampleTypeWaterSamplePackages).Where(x => x.Id.Equals(id)).SingleOrDefaultAsync();
                if (user.DivisionId.Equals(package.Project.DivisionId)) { 
                foreach(var item in package.WaterSampleTypeWaterSamplePackages)
                {
                    _context.Remove(item);
                }
                await _context.SaveChangesAsync();
                _context.Remove(package);
                await _context.SaveChangesAsync();
                return RedirectToAction("Packages");
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have the right to delete this item" });
                }
            }
            else { return NotFound(); }
        }
        [HttpGet]
        public async Task<IActionResult> Edit_Package(int? id)
        {
            if(id != null)
            {
                ViewData["ProjectId"] = await GetProjectList();
                ViewData["WaterSampleTypeId"] = new SelectList(_context.WaterSampleTypes, "Id", "Komponent");
                var user = await _userManager.GetUserAsync(User);
                var package = await _context.WaterSamplePackages.Include(x=>x.Project).SingleOrDefaultAsync(x => x.Id.Equals(id));
                if (package.Project.DivisionId.Equals(user.DivisionId) || User.IsInRole("Admin"))
                {
                    var itemlist = await _context.WaterSampleTypeWaterSamplePackages.Include(x=>x.WaterSampleType).Where(x => x.WaterSamplePackageId.Equals(id)).ToListAsync();
                    CreatePackageViewModel model = new CreatePackageViewModel(package,itemlist);
                    return View(model);
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "you do not have access to edit this water sample package" });
                }

            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit_Package(CreatePackageViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                foreach (string s in model.WaterSampleTypes.Split(","))
                {
                    var existing_WSTWSP = await _context.WaterSampleTypeWaterSamplePackages.Where(x => x.WaterSamplePackageId.Equals(model.OldPackageId) && x.WaterSampleTypeId.Equals(Convert.ToInt32(s))).SingleOrDefaultAsync();
                    if(existing_WSTWSP == null)
                    {
                        WaterSampleTypeWaterSamplePackage new_item = new WaterSampleTypeWaterSamplePackage();
                        new_item.WaterSamplePackageId = Convert.ToInt32(model.OldPackageId);
                        new_item.WaterSampleTypeId = Convert.ToInt32(s);
                        _context.Add(new_item);
                    }                   
                }
                await _context.SaveChangesAsync();
                var now_existing = await _context.WaterSampleTypeWaterSamplePackages.Where(x => x.WaterSamplePackageId.Equals(model.OldPackageId)).ToListAsync();
                foreach(var item in now_existing)
                {
                    if (!model.WaterSampleTypes.Contains(item.WaterSampleTypeId.ToString()))
                    {
                        _context.Remove(item);
                    }
                }
                await _context.SaveChangesAsync();
                var thepackage = await _context.WaterSamplePackages.FindAsync(model.OldPackageId);
                thepackage.ListOfComponents = model.WaterSampleTypesNames;
                thepackage.Annotation = model.Annotation;
                thepackage.ProjectId = model.ProjectId;
                _context.Update(thepackage);
                await _context.SaveChangesAsync();
                return RedirectToAction("Packages");
            }
            else
            {
                return View(model);
            }
        } 
        [HttpGet]
        public async Task<IActionResult> PDFSamples(int? PDFProjectId)
        {
            List<WaterSamplePackage> packages;
            if(PDFProjectId != null)
            {
                packages = await _context.WaterSamplePackages
                .Include(x => x.WaterSampleTypeWaterSamplePackages).ThenInclude(x => x.WaterSampleType)
                .Include(x => x.Project)
                .Where(x => x.ProjectId.Equals(PDFProjectId)).ToListAsync();
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                packages = await _context.WaterSamplePackages
                .Include(x => x.WaterSampleTypeWaterSamplePackages).ThenInclude(x => x.WaterSampleType)
                .Include(x => x.Project)
                .Where(x => x.Project.DivisionId.Equals(user.DivisionId)).ToListAsync();
            }
            List<string> fulllist = new List<string>();
            foreach (WaterSamplePackage WSP in packages)
            {
                foreach (WaterSampleTypeWaterSamplePackage WSTWSP in WSP.WaterSampleTypeWaterSamplePackages)
                    if (!fulllist.Contains(WSTWSP.WaterSampleType.Komponent))
                    {
                        fulllist.Add(WSTWSP.WaterSampleType.Komponent);
                    }
            }
            WaterSamplePackagePDFViewModel model = new WaterSamplePackagePDFViewModel(packages, fulllist);
            return new ViewAsPdf("_packages",model);
        }
        public async Task<string> WaterSampleDone(int ProjectId, List<WaterSampleMeas> measOnDate)
        {
            var packages = await _context.WaterSamplePackages.Include(x => x.WaterSampleTypeWaterSamplePackages).Where(x => x.ProjectId.Equals(ProjectId)).OrderByDescending(x => x.WaterSampleTypeWaterSamplePackages.Count()).ToListAsync();
            string annotationtoreturn = "";
            int largestcount = 0;
            int diffcount = 1000000;
            string diffannotation = "";
            foreach ( WaterSamplePackage package in packages)
            {
                List<WaterSampleType> list1 = package.WaterSampleTypeWaterSamplePackages.Where(x => !x.WaterSampleTypeId.Equals(451)).Select(x => x.WaterSampleType).ToList();
                List<WaterSampleType> list2 = measOnDate.Where(x => !x.WaterSampleTypeId.Equals(451)).Select(x => x.WaterSampleType).ToList();
               
                
                if (list2 != null)
                {
                    if (list2.Count() > 0)
                    {
                        var intersectlist = list1.Intersect(list2);
                        var allOfList1IsInList2 = intersectlist.Count() == list1.Count();
                        
                        if (allOfList1IsInList2)
                        {
                            if (intersectlist.Count() > largestcount)
                            {
                                largestcount = intersectlist.Count();
                                annotationtoreturn = package.Annotation;
                            }

                        }
                        else
                        {
                            if (Math.Abs(list1.Count() - intersectlist.Count()) < diffcount)
                            {
                                diffcount = Math.Abs(list1.Count() - intersectlist.Count());
                                diffannotation = package.Annotation;
                            }
                           
                        }
                    }
                    else
                    {
                        return "NotASample";
                    }
                }
                else
                {
                    return "NotASample";
                }
            }
            if (annotationtoreturn != "")
            {
                return annotationtoreturn;
            }
            else if (diffannotation != "" && diffcount < 4)
            {
                return diffannotation;
            }
            else return "NotASample";
           
        }
        [HttpGet]
        public async Task<IActionResult> SamplingLog(DateTime? start, DateTime? end,int? ProjectId)
        {
            DateTime starttime;
            DateTime endtime = DateTime.Now.Date;
            List<SamplingLog> model = new List<SamplingLog>();
            if(start != null)
            {
                starttime = Convert.ToDateTime(start).Date;
            }
            else
            {
                starttime = DateTime.Now.AddYears(-10).Date;
            }
            if(end != null)
            {
                end = Convert.ToDateTime(end).Date;
            }
            var user = await _userManager.GetUserAsync(User);
            List<WaterSampleMeas> measures = await _context.WaterSampleMeasures.Include(x=>x.WaterSampleType).Where(x => x.Dato >= starttime && x.Dato <= endtime && x.ProjectId.Equals(ProjectId)).ToListAsync();
            foreach(DateTime d in measures.Select(x => x.Dato.Date).Distinct())
            {
                List<WaterSampleMeas> measOnDate = measures.Where(x => x.Dato.Equals(d) && !x.WaterSampleTypeId.Equals(451)).ToList();
                List<int> watersampleplacesondate = measOnDate.Select(x => x.WaterSamplePlaceId).Distinct().ToList();
                var packages = await _context.WaterSamplePackages.Include(x => x.WaterSampleTypeWaterSamplePackages).Where(x => x.ProjectId.Equals(ProjectId)).OrderByDescending(x=>x.WaterSampleTypeWaterSamplePackages.Count()).ToListAsync();
                foreach (int i in watersampleplacesondate)                   
                {
                    SamplingLog new_log = new SamplingLog();
                    new_log.Dato = d;
                    new_log.WeekNumber = StringExtensions.GetIso8601WeekOfYear(d);
                    new_log.WaterSamplePlaceId = i;
                    new_log.WaterSamplePlace = await _context.WaterSamplePlaces.FindAsync(i);
                    foreach (WaterSamplePackage package in packages)
                    {
                        List<WaterSampleType> list1 = package.WaterSampleTypeWaterSamplePackages.Where(x => !x.WaterSampleTypeId.Equals(451)).Select(x => x.WaterSampleType).ToList();                        
                        List<WaterSampleType> list2 = measOnDate.Where(x => x.WaterSamplePlaceId.Equals(i) && !x.WaterSampleTypeId.Equals(451)).Select(x => x.WaterSampleType).ToList();
                        
                        if(list2 != null)
                        {
                            if(list2.Count() > 0)
                            {
                                var allOfList1IsInList2 = list1.Intersect(list2).Count() == list1.Count();
                                if (allOfList1IsInList2)
                                {
                                    new_log.WaterSamplePackages.Add(package);
                                    List<string> samplenames = measOnDate.Where(x => x.WaterSamplePlaceId.Equals(i) && x.WaterSampleNumber != "").Select(x => x.WaterSampleNumber).Distinct().ToList();
                                    if (samplenames.Count() < 2)
                                    {
                                        new_log.SampleFileName = samplenames.First();
                                    }
                                    else
                                    {
                                        List<string> samplenames2 = measOnDate.Where(x => x.WaterSamplePlaceId.Equals(i) && x.WaterSampleNumber != "" && !x.WaterSampleTypeId.Equals(451)).Select(x => x.WaterSampleNumber).ToList();
                                        string maxname = "";
                                        int maxcount = 0;
                                        int tempmaxcount = 0;
                                        foreach(string name in samplenames)
                                        {
                                            tempmaxcount = samplenames2.Where(x => x.Equals(name)).Count();
                                            if (tempmaxcount > maxcount)
                                            {
                                                maxcount = tempmaxcount;
                                                maxname = name;
                                            }
                                        }
                                        new_log.SampleFileName = maxname;
                                    }
                                    foreach (var item in list1)
                                    {
                                        var toremove = measOnDate.Where(x => x.ProjectId.Equals(ProjectId) && x.Dato.Equals(d) && x.WaterSamplePlaceId.Equals(i) && x.WaterSampleTypeId.Equals(item.Id)).FirstOrDefault();
                                        if (toremove != null)
                                        {
                                            measOnDate.Remove(toremove);
                                        }
                                    }
                                   
                                    if(measOnDate.Count() == 0)
                                    {
                                        break;
                                    }
                                }
                            }
                        } 
                    }
                    model.Add(new_log);
                    if (measOnDate.Count() == 0)
                    {
                        break;
                    }

                }

            }
            return View(model.OrderBy(x=>x.Dato).ThenBy(x=>x.WaterSamplePlace.Name));
        }
        [HttpGet]
        public async Task<IActionResult> Packages()
        {
            ViewData["ProjectId"] = await GetProjectList();
            var user = await _userManager.GetUserAsync(User);
            var packages = await _context.WaterSamplePackages
                .Include(x=>x.WaterSampleTypeWaterSamplePackages).ThenInclude(x=>x.WaterSampleType)
                .Include(x => x.Project)
                .Where(x => x.Project.DivisionId.Equals(user.DivisionId)).ToListAsync();
            List<string> fulllist = new List<string>();
            foreach(WaterSamplePackage WSP in packages)
            {
                foreach(WaterSampleTypeWaterSamplePackage WSTWSP in WSP.WaterSampleTypeWaterSamplePackages)
                    if (!fulllist.Contains(WSTWSP.WaterSampleType.Komponent))
                    {
                        fulllist.Add(WSTWSP.WaterSampleType.Komponent);
                    }
            }
            ViewData["WaterSampleTypesAll"] = fulllist;
            return View(packages);
        }
        [HttpPost]
        public async Task<IActionResult> EditAlert(AlertEditWSTVM model)
        {
            if (ModelState.IsValid)
            {
                if(model.Limit != null)
                {
                    var prevlimit = await _context.WaterSampleLimits.Where(x => x.WaterSamplePlaceId.Equals(model.WaterSamplePlaceId) && x.WaterSampleTypeId.Equals(model.WaterSampleTypeId)).ToListAsync();
                    if(prevlimit.Count > 0)
                    {
                        prevlimit.First().Limit = Convert.ToDouble(model.Limit);
                        prevlimit.First().MeanLimit = Convert.ToDouble(model.MeanLimit);
                        _context.Update(prevlimit.First());
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        WaterSampleLimit limit = new WaterSampleLimit();
                        limit.WaterSamplePlaceId = model.WaterSamplePlaceId;
                        limit.WaterSampleTypeId = model.WaterSampleTypeId;
                        limit.Limit = Convert.ToDouble(model.Limit);
                        limit.MeanLimit = Convert.ToDouble(model.MeanLimit);
                        _context.Add(limit);
                        await _context.SaveChangesAsync();
                    }
                
                
                }
                var type = await _context.WaterSampleTypes.FindAsync(model.WaterSampleTypeId);
                var place = await _context.WaterSamplePlaces.Include(x => x.Project).Where(x => x.Id.Equals(model.WaterSamplePlaceId)).FirstAsync();
                AlertEditWSTVM ae = new AlertEditWSTVM();
                ae.Limit = Convert.ToDouble(model.Limit);
                ae.MeanLimit = Convert.ToDouble(model.MeanLimit);
                ae.WaterSamplePlaceId = model.WaterSamplePlaceId;
                ae.WaterSampleTypeId = model.WaterSampleTypeId;
                ae.WaterSamplePlace = place;
                ae.WaterSampleType = type;
                return Ok();
            }
            else
            {
                return Ok();
            }
        }
        [HttpGet]
        public async Task<IActionResult> CopyFromMeasPlaceToMeasPlace()
        {
            ViewData["ProjectId"] = await GetProjectListNoChoice();
            ViewData["WaterSamplePlaceId1"] = new SelectList(_context.WaterSamplePlaces, "Id", "Name");
            ViewData["WaterSamplePlaceId2"] = new SelectList(_context.WaterSamplePlaces, "Id", "Name");
           
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CopyFromMeasPlaceToMeasPlace(int p1, int p2)
        {
            var measureLimits = await _context.WaterSampleLimits.Where(x => x.WaterSamplePlaceId.Equals(p1)).ToListAsync();
            foreach(var x in measureLimits)
            {
                var lol = await EditAlert2(x.Limit,x.MeanLimit, x.WaterSampleTypeId, p2);
            }
            return RedirectToAction("CopyFromMeasPlaceToMeasPlace");
        }
        public async Task<IActionResult> CopyFromMeasPlaceToProject(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var wp = await _context.WaterSamplePlaces.FindAsync(id);
            List<WaterSampleLimit> limits = await _context.WaterSampleLimits.Where(x => x.WaterSamplePlaceId.Equals(id)).ToListAsync();
            var watersampleplaces = await _context.WaterSamplePlaces.Where(x => x.ProjectId.Equals(wp.ProjectId) && !x.Id.Equals(id) && x.WaterSamplePlaceTypeId.Equals(wp.WaterSamplePlaceTypeId)).ToListAsync();
            foreach (var wsp in watersampleplaces)
            {
                List<WaterSampleLimit> new_limits = (from li in limits
                                                     select
                                   new WaterSampleLimit
                                   {
                                       Limit = li.Limit,
                                       WaterSamplePlaceId = wsp.Id,
                                       MeanLimit = li.MeanLimit,
                                       WaterSampleTypeId = li.WaterSampleTypeId

                                   }
                                                     )
                                                     .ToList();
                foreach (var li in new_limits)
                {
                    _context.Add(li);
                }

            }
            await _context.SaveChangesAsync();
            return RedirectToAction("CopyFromMeasPlaceToMeasPlace");
        }
        public async Task<int> EditAlert2(double? Limit,double? MeanLimit,int WaterSampleTypeId, int WaterSamplePlaceId)
        {
            if (ModelState.IsValid)
            {
                if (Limit != null)
                {
                    var prevlimit = await _context.WaterSampleLimits.Where(x => x.WaterSamplePlaceId.Equals(WaterSamplePlaceId) && x.WaterSampleTypeId.Equals(WaterSampleTypeId)).ToListAsync();
                    if (prevlimit.Count > 0)
                    {
                        prevlimit.First().Limit = Convert.ToDouble(Limit);
                        prevlimit.First().MeanLimit = Convert.ToDouble(MeanLimit);
                        _context.Update(prevlimit.First());
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        WaterSampleLimit limit = new WaterSampleLimit();
                        limit.WaterSamplePlaceId = WaterSamplePlaceId;
                        limit.WaterSampleTypeId = WaterSampleTypeId;
                        limit.Limit = Convert.ToDouble(Limit);
                        limit.MeanLimit = Convert.ToDouble(MeanLimit);
                        _context.Add(limit);
                        await _context.SaveChangesAsync();
                    }


                }
                return 1;
            }
            else
            {
                return 0;
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditAlert(int? id)
        {
            if (id != null)
            {
                var themeas = await _context.WaterSampleMeasures.Include(x => x.WaterSamplePlace).ThenInclude(x => x.Project).Include(x => x.WaterSampleType).Where(x => x.Id.Equals(id)).FirstAsync();
                var thelimit = await _context.WaterSampleLimits.Where(x => x.WaterSamplePlaceId.Equals(themeas.WaterSamplePlaceId) && x.WaterSampleTypeId.Equals(themeas.WaterSampleTypeId)).Include(x=>x.WaterSamplePlace).ThenInclude(x=>x.Project).Include(x=>x.WaterSampleType).ToListAsync();
                if(thelimit.Count > 0)
                {
                    AlertEditWSTVM aeWSTVM1 = new AlertEditWSTVM(thelimit.First());
                    return PartialView("_AlertEdit", aeWSTVM1);
                }
                else
                {

                }
                AlertEditWSTVM aeWSTVM2 = new AlertEditWSTVM { WaterSampleTypeId = Convert.ToInt32(themeas.WaterSampleTypeId),WaterSampleType = themeas.WaterSampleType,WaterSamplePlaceId=themeas.WaterSamplePlaceId,WaterSamplePlace=themeas.WaterSamplePlace};
                return PartialView("_AlertEdit", aeWSTVM2);
            }
            else
            {
                return null;
            }
        }
        [HttpGet]
        public async Task<IActionResult> WaterSamplePlaces()
        {
            var wsp = await _context.WaterSamplePlaces.Include(x=>x.Project).Include(x=>x.WaterSamplePlaceType).ToListAsync();
            return View(wsp);
        }
        [HttpGet]
        public IActionResult WaterSamplePlace()
        {
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
            ViewData["WaterSamplePlaceTypeId"] = new SelectList(_context.WaterSamplePlaceTypes, "Id", "Type");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> WaterSamplePlace(WaterSamplePlace model)
        {
            if (ModelState.IsValid)
            {
                _context.WaterSamplePlaces.Add(model);
                await _context.SaveChangesAsync();
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
            ViewData["WaterSamplePlaceTypeId"] = new SelectList(_context.WaterSamplePlaceTypes, "Id", "Type");
            return RedirectToAction("WaterSamplePlaces");
        }
        public JsonResult GetWaterSamplePlacesProject(string theId)
        {
            int Id = Convert.ToInt32(theId);
            var thedata = _context.WaterSamplePlaces.Where(x => x.ProjectId.Equals(Id)).ToList();

            return Json(thedata);
        }
        // GET: WaterSampleTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waterSampleType = await _context.WaterSampleTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (waterSampleType == null)
            {
                return NotFound();
            }

            return View(waterSampleType);
        }

        // GET: WaterSampleTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WaterSampleTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Komponent,Enhed,DL")] WaterSampleType waterSampleType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(waterSampleType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(waterSampleType);
        }
        
        [HttpGet]
        public async Task<IActionResult> Edit_WaterSamplePlace(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waterSamplePlace = await _context.WaterSamplePlaces.FindAsync(id);
            if (waterSamplePlace == null)
            {
                return NotFound();
            }
            ViewData["WaterSamplePlaceTypeId"] = new SelectList(_context.WaterSamplePlaceTypes, "Id", "Type");
            ViewData["ProjectId"] = await GetProjectList();
            return View(waterSamplePlace);
        }
        [HttpPost]
        public async Task<IActionResult> Edit_WaterSamplePlace(WaterSamplePlace model)
        {
            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(model);
            }
        }
        // GET: WaterSampleTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waterSampleType = await _context.WaterSampleTypes.FindAsync(id);
            if (waterSampleType == null)
            {
                return NotFound();
            }
            return View(waterSampleType);
        }

        // POST: WaterSampleTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Komponent,Enhed,DL")] WaterSampleType waterSampleType)
        {
            if (id != waterSampleType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(waterSampleType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WaterSampleTypeExists(waterSampleType.Id))
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
            return View(waterSampleType);
        }
        [HttpGet]
        public async Task<IActionResult> GetProjectReportAll(DateTime? StartDate,DateTime? EndDate,int? ProjectId,bool goodbad)
        {
            if(ProjectId == null)
            {
                return RedirectToAction("ShowMeasures");
            }
            
            List<WaterSampleMeasVM> model;
            DateTime start; 
            DateTime end;
            if(EndDate == null)
            {
                end = DateTime.Now.Date;
            }
            else
            {
                end = Convert.ToDateTime(EndDate).Date;
            }
            if(StartDate == null)
            {
                start = DateTime.Now.AddYears(-5).Date;
            }
            else
            {
                start = Convert.ToDateTime(StartDate).Date;
            }
            int ProjId = Convert.ToInt32(ProjectId);
            var measures = await _context.WaterSampleMeasures
                .Include(x => x.WaterSamplePlace)
                .Include(x => x.WaterSampleType)
                .Where(x => x.ProjectId.Equals(ProjId) && x.Dato >= start && x.Dato <= end).ToListAsync();
            model = await CheckLimits(measures);
            if(goodbad == false)
            {
                model = model.Where(x => x.holdslimit.Equals(false)).ToList();
            }
            //foreach(WaterSampleMeas m in measures)
            //{
            //    WaterSampleMeasVM m_vm = new WaterSampleMeasVM(m);
            //    m_vm.holdslimit = await checklimit2(m);
            //    model.Add(m_vm);
            //}
            ViewData["Project"] = await _context.Projects.SingleOrDefaultAsync(x => x.Id.Equals(ProjId));
            ViewData["ProjectId"] = ProjId;
            ViewData["StartDate"] = start;
            ViewData["EndDate"] = end;
            ViewData["GoodBad"] = goodbad;
           return View(model);

        }
        public async Task<IActionResult> GetProjectReportPDF(DateTime? StartDate,DateTime? EndDate,int? ProjectId,bool GoodBad)
        {   
            if(GoodBad == true)
            {
                var measures = await _context.WaterSampleMeasures
                .Include(x => x.WaterSamplePlace)
                .Include(x => x.WaterSampleType)
                .Include(x => x.Project)
                .Where(x => x.ProjectId.Equals(ProjectId) && x.Dato >= StartDate && x.Dato <= EndDate).ToListAsync();
                List<WaterSampleMeasVM> model = await CheckLimits(measures);
                return new ViewAsPdf("_GetProjectReportPDF", model);
            }
            else
            {
                var measures = await _context.WaterSampleMeasures
                .Include(x => x.WaterSamplePlace)
                .Include(x => x.WaterSampleType)
                .Include(x => x.Project)
                .Where(x => x.ProjectId.Equals(ProjectId) && x.Dato >= StartDate && x.Dato <= EndDate).ToListAsync();
                List<WaterSampleMeasVM> model = await CheckLimits(measures);
                model = model.Where(x => x.holdslimit.Equals(2)).ToList();
                return new ViewAsPdf("_GetProjectReportPDF", model);
            }
            
        }
        public async Task<IActionResult> GetProjectReportBad(DateTime? StartDate, DateTime? EndDate, int? ProjectId)
        {
            if (ProjectId == null)
            {
                return RedirectToAction("ShowMeasures");
            }

            List<WaterSampleMeasVM> model = new List<WaterSampleMeasVM>();
            DateTime start;
            DateTime end;
            if (EndDate == null)
            {
                end = DateTime.Now.Date;
            }
            else
            {
                end = Convert.ToDateTime(EndDate).Date;
            }
            if (StartDate == null)
            {
                start = DateTime.Now.AddYears(-5).Date;
            }
            else
            {
                start = Convert.ToDateTime(StartDate).Date;
            }
            int ProjId = Convert.ToInt32(ProjectId);
            var measures = await _context.WaterSampleMeasures
                .Include(x=>x.WaterSamplePlace)
                .Include(x=>x.WaterSampleType)
                .Where(x => x.ProjectId.Equals(ProjId) && x.Dato >= start && x.Dato <= end).ToListAsync();
            model = await CheckLimits(measures);
            //foreach(WaterSampleMeas m in measures)
            //{
            //    WaterSampleMeasVM m_vm = new WaterSampleMeasVM(m);
            //    m_vm.holdslimit = await checklimit2(m);
            //    model.Add(m_vm);
            //}
            ViewData["Project"] = await _context.Projects.SingleOrDefaultAsync(x => x.Id.Equals(ProjId));
            ViewData["ProjectId"] = ProjId;
            ViewData["StartDate"] = start;
            ViewData["EndDate"] = end;
            ViewData["GoodBad"] = false;
            return View("GetProjectReportAll",model);

        }
        public async Task<List<WaterSampleMeasVM>> CheckLimits(List<WaterSampleMeas> input)
        {
            List<WaterSampleMeasVM> output = new List<WaterSampleMeasVM>();
            var thelimits = await _context.WaterSampleLimits.OrderBy(x => x.Id).ToListAsync();
            var thestandardLimits = await _context.StandardLimits.ToListAsync();
            foreach (var item in input)
            {
                WaterSampleMeasVM new_vm = new WaterSampleMeasVM(item);
                var thelimit = thelimits.Where(x => x.WaterSamplePlaceId.Equals(item.WaterSamplePlaceId) && x.WaterSampleTypeId.Equals(item.WaterSampleTypeId)).LastOrDefault();           
                if (thelimit == null)
                {
                    var thestandardLimit = thestandardLimits.Where(x => x.Komponent.Equals(item.WaterSampleType.Komponent) && x.WaterSamplePlaceTypeId.Equals(item.WaterSamplePlace.WaterSamplePlaceTypeId)).SingleOrDefault();
                    if (thestandardLimit != null)
                    {
                        if (thestandardLimit.MaxLimit >= item.value)
                        {
                            new_vm.holdslimit = 1;
                        }
                        else
                        {
                            new_vm.holdslimit = 2;
                        }
                    }
                    else
                    {
                        new_vm.holdslimit = 0;
                    }

                }
                else
                {
                    if (thelimit.Limit >= item.value)
                    {  
                        new_vm.holdslimit = 1;
                        new_vm.theLimit = thelimit.Limit;
                        if(thelimit.MeanLimit != null)
                        {
                            if (item.value > thelimit.MeanLimit)
                            {
                                new_vm.theMeanLimit = thelimit.MeanLimit;
                                new_vm.holdslimit = 3;
                            }
                        }
                        
                    }
                    else
                    {
                        new_vm.holdslimit = 2;
                        new_vm.theLimit = thelimit.Limit;
                        if(thelimit.MeanLimit != null)
                        {
                            if (item.value > thelimit.MeanLimit)
                            {
                                new_vm.theMeanLimit = thelimit.MeanLimit;
                                new_vm.holdslimit = 4;
                            }
                        }
                    }
                }
                output.Add(new_vm);
            }
            return output;
        }
        public async Task<List<WaterSampleMeasVM>> CheckLimitsMean(List<WaterSampleMeas> input,int? FreqWeeks = null,int? FreqSamples = null)
        {
            List<WaterSampleMeasVM> output = new List<WaterSampleMeasVM>();
            var wsp = input.Select(x => x.WaterSamplePlaceId).Distinct().ToList();
            List<WaterSampleMeas> allMeasures = new List<WaterSampleMeas>();
            var measures = await _context.WaterSampleMeasures.Where(x => x.ProjectId.Equals(input.First().ProjectId)).ToListAsync();
            measures = measures.Where(x => wsp.IndexOf(x.WaterSamplePlaceId) >= 0).ToList();
            var thelimits = await _context.WaterSampleLimits.OrderBy(x => x.Id).ToListAsync();
            var thestandardLimits = await _context.StandardLimits.ToListAsync();
            foreach (var item in input)
            {
                WaterSampleMeasVM new_vm = new WaterSampleMeasVM(item);
                var thelimit = thelimits.Where(x => x.WaterSamplePlaceId.Equals(item.WaterSamplePlaceId) && x.WaterSampleTypeId.Equals(item.WaterSampleTypeId)).LastOrDefault();
                if (thelimit == null)
                {
                    var thestandardLimit = thestandardLimits.Where(x => x.Komponent.Equals(item.WaterSampleType.Komponent) && x.WaterSamplePlaceTypeId.Equals(item.WaterSamplePlace.WaterSamplePlaceTypeId)).SingleOrDefault();
                    if (thestandardLimit != null)
                    {
                        if (thestandardLimit.MaxLimit >= item.value)
                        {
                            new_vm.holdslimit = 1;
                        }
                        else
                        {
                            new_vm.holdslimit = 2;
                        }
                    }
                    else
                    {
                        new_vm.holdslimit = 0;
                    }

                }
                else
                {
                    if (thelimit.Limit >= item.value)
                    {
                        new_vm.holdslimit = 1;
                        if ((FreqWeeks != null || FreqSamples != null) && thelimit.MeanLimit != null)
                        {
                            
                            if (FreqSamples != null)
                            {
                                var prev_measures = measures.Where(x => x.WaterSamplePlaceId.Equals(item.WaterSamplePlaceId) && x.Dato < item.Dato && x.WaterSampleTypeId.Equals(item.WaterSampleTypeId)).OrderByDescending(x => x.Dato).ToList();
                                if(prev_measures.Count > 0)
                                {

                               
                                if(prev_measures.Count >= FreqSamples)
                                {
                                    if(prev_measures.Take(Convert.ToInt32(FreqSamples)).Where(x => x.value > thelimit.MeanLimit).Count() == prev_measures.Take(Convert.ToInt32(FreqSamples)).Count())
                                    {
                                        new_vm.holdslimit = 3;
                                        new_vm.theMeanLimit = thelimit.MeanLimit;
                                    }
                                }
                                else
                                {
                                    if (prev_measures.Where(x => x.value > thelimit.MeanLimit).Count() == prev_measures.Count())
                                    {
                                        new_vm.holdslimit = 3;
                                        new_vm.theMeanLimit = thelimit.MeanLimit;
                                    }
                                }
                                }
                            }
                            else if (FreqWeeks != null)
                            {
                                var prev_measures = measures.Where(x => x.WaterSamplePlaceId.Equals(item.WaterSamplePlaceId) && x.Dato < item.Dato && x.Dato >= item.Dato.AddDays(-Convert.ToInt32(FreqWeeks) * 7) && x.WaterSampleTypeId.Equals(item.WaterSampleTypeId)).OrderByDescending(x => x.Dato).ToList();
                                if(prev_measures.Count > 0)
                                {
                                    if (prev_measures.Where(x => x.value > thelimit.MeanLimit).Count() == prev_measures.Count())
                                    {
                                        new_vm.holdslimit = 3;
                                        new_vm.theMeanLimit = thelimit.MeanLimit;
                                    }
                                }
                               
                            }
                        }
                        new_vm.theLimit = thelimit.Limit;
                    }
                    else
                    {
                        new_vm.holdslimit = 2;
                        new_vm.theLimit = thelimit.Limit;
                        if (FreqWeeks != null || FreqSamples != null)
                        {

                            if (FreqSamples != null)
                            {
                                var prev_measures = measures.Where(x => x.WaterSamplePlaceId.Equals(item.WaterSamplePlaceId) && x.Dato < item.Dato && x.WaterSampleTypeId.Equals(item.WaterSampleTypeId)).OrderByDescending(x => x.Dato).ToList();
                                if(prev_measures.Count > 0)
                                {
                                    if (prev_measures.Count >= FreqSamples)
                                    {
                                        if (prev_measures.Take(Convert.ToInt32(FreqSamples)).Where(x => x.value > thelimit.MeanLimit).Count() == prev_measures.Take(Convert.ToInt32(FreqSamples)).Count())
                                        {
                                            new_vm.holdslimit = 4;
                                            new_vm.theMeanLimit = thelimit.MeanLimit;
                                        }
                                    }
                                    else
                                    {
                                        if (prev_measures.Where(x => x.value > thelimit.MeanLimit).Count() == prev_measures.Count())
                                        {
                                            new_vm.holdslimit = 4;
                                            new_vm.theMeanLimit = thelimit.MeanLimit;
                                        }
                                    }
                                }
                            }
                            else if (FreqWeeks != null)
                            {
                                var prev_measures = measures.Where(x => x.WaterSamplePlaceId.Equals(item.WaterSamplePlaceId) && x.Dato < item.Dato && x.Dato >= item.Dato.AddDays(-Convert.ToInt32(FreqWeeks) * 7) && x.WaterSampleTypeId.Equals(item.WaterSampleTypeId)).OrderByDescending(x => x.Dato).ToList();
                                if(prev_measures.Count > 0)
                                {
                                    if (prev_measures.Where(x => x.value > thelimit.MeanLimit).Count() == prev_measures.Count())
                                    {
                                        new_vm.holdslimit = 4;
                                        new_vm.theMeanLimit = thelimit.MeanLimit;
                                    }
                                }
                            }
                        }
                       
                    }
                }
                output.Add(new_vm);
            }
            return output;
        }
        // GET: WaterSampleTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waterSampleType = await _context.WaterSampleTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (waterSampleType == null)
            {
                return NotFound();
            }

            return View(waterSampleType);
        }

        // POST: WaterSampleTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var waterSampleType = await _context.WaterSampleTypes.FindAsync(id);
            _context.WaterSampleTypes.Remove(waterSampleType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WaterSampleTypeExists(int id)
        {
            return _context.WaterSampleTypes.Any(e => e.Id == id);
        }
        [HttpGet]
        public async Task<IActionResult> ShowMeasures()
        {
            var user = await _userManager.GetUserAsync(User);
            var measures = await _context.WaterSampleMeasures.Include(x=>x.WaterSampleType).Include(x=>x.WaterSamplePlace).ThenInclude(x=>x.Project)
                .Where(x=>x.Project.DivisionId.Equals(user.DivisionId)).OrderByDescending(x=>x.Dato).Take(50).ToListAsync();
            List<WaterSampleMeasVM> model = await CheckLimits(measures);// = new List<WaterSampleMeasVM>();

            //foreach(var meas in measures)
            //{
            //    WaterSampleMeasVM vm = new WaterSampleMeasVM(meas);
            //    vm.holdslimit = await checklimit2(meas);
            //    model.Add(vm);
            //}
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["WaterSamplePlaceId"] = await createFilterlist();
            return View(model);
        }
        public async Task<IActionResult> Combsearch(string searchstring, string filterchoice, DateTime? StartDate,DateTime? EndDate)
        {
            DateTime starttime = DateTime.Now.AddDays(-3000);
            DateTime endtime = DateTime.Now;
            if(StartDate != null)
            {
                starttime = Convert.ToDateTime(StartDate);
            }
            if(EndDate != null)
            {
                endtime = Convert.ToDateTime(EndDate);
            }
            int f_c_converted;
            f_c_converted = Convert.ToInt32(filterchoice);
            var user = await _userManager.GetUserAsync(User);
            ViewData["WaterSamplePlaceId"] = await createFilterlist();
            List<WaterSampleMeas> data = new List<WaterSampleMeas>();
            
            if (searchstring != null && filterchoice != null)
            {

                data = await _context.WaterSampleMeasures.Include(x=>x.WaterSampleType)
                    .Include(x=>x.WaterSamplePlace).ThenInclude(x=>x.Project)
                    .Where(x => ((x.Dato >= starttime && x.Dato <= endtime) && x.WaterSampleType.Komponent.ToLower().Contains(searchstring.ToLower()) || x.WaterSamplePlace.Name.ToString().ToLower().Contains(searchstring.ToLower())) || x.WaterSamplePlace.Project.Name.ToString().ToLower().Contains(searchstring.ToLower()) && x.WaterSamplePlaceId.Equals(f_c_converted))
                    .OrderByDescending(x => x.Dato)
                    .ToListAsync();
            }
            else if (searchstring != null && (filterchoice == null || filterchoice == "All"))
            {
                data = await _context.WaterSampleMeasures.Include(x => x.WaterSampleType)
                     .Include(x => x.WaterSamplePlace).ThenInclude(x => x.Project)
                     .Where(x => ((x.Dato >= starttime && x.Dato <= endtime) && x.WaterSampleType.Komponent.ToLower().Contains(searchstring.ToLower()) || x.WaterSamplePlace.Name.ToString().ToLower().Contains(searchstring.ToLower())) || x.WaterSamplePlace.Project.Name.ToString().ToLower().Contains(searchstring.ToLower()))
                     .OrderByDescending(x => x.Dato)
                     .ToListAsync();
            }
            else if (searchstring == null && filterchoice != null)
            {
                data = await _context.WaterSampleMeasures.Include(x => x.WaterSampleType)
                    .Include(x => x.WaterSamplePlace).ThenInclude(x => x.Project)
                    .Where(x => (x.Dato >= starttime && x.Dato <= endtime) && x.WaterSamplePlaceId.Equals(f_c_converted))
                    .OrderByDescending(x => x.Dato)
                    .ToListAsync();

            }
            else
            {

                data = await _context.WaterSampleMeasures.Include(x => x.WaterSampleType)
                    .Include(x => x.WaterSamplePlace).ThenInclude(x => x.Project)
                    .Where(x=> (x.Dato >= starttime && x.Dato <= endtime))
                    .Take(50)
                    .OrderByDescending(x => x.Dato)
                    .ToListAsync();
            }
            List<WaterSampleMeasVM> model = new List<WaterSampleMeasVM>();
            foreach (var meas in data)
            {
                WaterSampleMeasVM vm = new WaterSampleMeasVM(meas);
                vm.holdslimit = await checklimit2(meas);
                model.Add(vm);
            }
            ViewData["ProjectId"] = await GetProjectList();
            return View("ShowMeasures", model);
        }
        public async Task<IEnumerable<SelectListItem>> createFilterlist()
        {
            var filternames = await  _context.WaterSamplePlaces.Include(x=>x.Project)
                                         .ToListAsync();
            IEnumerable<SelectListItem> selList = from s in filternames
            select new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Project.Name + " : " + s.Name
            };
            return selList;
        }
        [HttpGet]
        public async Task<IActionResult> ExportSamples()
        {
            var sampleplaces = await createFilterlist();
            ViewData["WaterSamplePlaceId"] = new SelectList( sampleplaces, "Value", "Text");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ExportSamples(ExportSamplesVM model)
        {
            var measures = await _context.WaterSampleMeasures.Include(x=>x.Project).Include(x=>x.WaterSamplePlace).Include(x=>x.WaterSampleType).Where(x => x.WaterSamplePlaceId.Equals(model.WaterSamplePlaceId) && x.Dato.Date >= model.starttime.Date && x.Dato.Date <= model.endtime.Date)
                .OrderBy(x=>x.WaterSampleType.Komponent).ThenBy(x=>x.Dato)
                .ToListAsync();
            List<WaterSampleMeasVM> measureswm = new List<WaterSampleMeasVM>();
            foreach(var m in measures)
            {
                WaterSampleMeasVM nm = new WaterSampleMeasVM(m);
                nm.holdslimit = await checklimit2(m);
                measureswm.Add(nm);
            }
            return new ViewAsPdf("_Samples", measureswm);
        }
        public async Task<int> checklimit2(WaterSampleMeas themeas)
        {
            
            if (themeas != null)
            {
                var thelimit = await _context.WaterSampleLimits.Where(x => x.WaterSamplePlaceId.Equals(themeas.WaterSamplePlaceId) && x.WaterSampleTypeId.Equals(themeas.WaterSampleTypeId)).ToListAsync();
                if (thelimit.Count() < 1)
                {
                    var thestandardLimit = await _context.StandardLimits.Where(x=>x.Komponent.Equals(themeas.WaterSampleType.Komponent) && x.WaterSamplePlaceTypeId.Equals(themeas.WaterSamplePlace.WaterSamplePlaceTypeId)).SingleOrDefaultAsync();
                    if(thestandardLimit != null)
                    {
                        if (thestandardLimit.MaxLimit >= themeas.value)
                        {
                            return 1;
                        }
                        else
                        {
                            return 2;
                        }
                    }
                    else
                    {
                        return 0;
                    }
                    
                }
                else
                {
                    if (thelimit.First().Limit >= themeas.value)
                    {
                        return 1;
                    }
                    else
                    {
                        return 2;
                    }
                }
            }
            else
            {
                return 2;
            }
        }
        public async Task<IActionResult> WaterSampleReportZip(int? id, bool? updatereportdate)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var measures = await _context.WaterSampleMeasures
                .Include(x => x.WaterSampleType)
                .Include(x => x.WaterSamplePlace)
                .Include(x => x.Project).ThenInclude(x => x.Division)
                .Where(x => x.WaterSamplePlace.ProjectId.Equals(id)).OrderBy(x => x.Dato)
                .ToListAsync();

            if (updatereportdate == true)
            {
                foreach (var m in measures)
                {
                    if (m.DateReporting == null)
                    {
                        m.DateReporting = DateTime.Now.Date;
                        _context.Update(m);
                    }

                }
                await _context.SaveChangesAsync();
            }
            foreach (var watersampleplace in measures.Select(x => x.WaterSamplePlaceId).Distinct()) { 
                List<WaterSampleMeasVM> model = await CheckLimits(measures.Where(x => x.WaterSamplePlaceId.Equals(watersampleplace)).ToList());
                string placename = model.First().WaterSamplePlace.Name;
                string footer = "--footer-center \"" + _sharedLocalizer.GetLocalizedHtmlString("Printed on:").Value + DateTime.Now.Date.ToString("yyyy-MM-dd") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
                ViewAsPdf pdf = new ViewAsPdf("_GetProjectReportTimelLinePDF", model)
                {
                    FileName = "Water Sample Report_" + placename + ".pdf",
                    PageOrientation = Orientation.Landscape,
                    PageSize = Size.A3,
                    CustomSwitches = footer

                };
                string fullPath = _env.WebRootPath + "\\TempFolder\\" + model.First().Project.Name.ToString() + "\\" + placename + "\\";
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }
                Task<byte[]> t = Task.Run(() => pdf.BuildFile(ControllerContext));
                try
                {
                    t.Wait();
                }
                catch
                {

                }
                if (t.Status == TaskStatus.RanToCompletion)
                {
                    using (var fileStream = new FileStream(fullPath + pdf.FileName, FileMode.Create, FileAccess.Write))
                    {
                        await fileStream.WriteAsync(t.Result, 0, t.Result.Length);
                    }
                    PersonalFile pfile = new PersonalFile
                    {
                        ApplicationUserId = user.Id,
                        Downloaded = false,
                        FileName = pdf.FileName,
                        FileExtension = ".pdf",
                        path = fullPath + pdf.FileName
                    };
                    _context.Add(pfile);
                   
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> WaterSampleReportGraphs(int? id, string searchplace = "")
        {
            if (id == null)
            {
                return NotFound();
            }
            var measures = await _context.WaterSampleMeasures
                .Include(x => x.WaterSampleType)
                .Include(x => x.WaterSamplePlace)
                .Include(x => x.Project).ThenInclude(x => x.Division)
                .Where(x => x.WaterSamplePlace.ProjectId.Equals(id)).OrderBy(x => x.Dato)
                .ToListAsync();
            if (searchplace == "")
            {
                ViewBag.SearchPlace = "";
            }
            else
            {
                ViewBag.SearchPlace = searchplace;
            }
            List<WaterSampleMeasVM> model = await CheckLimits(measures);
            //List<WaterSampleMeasVM> model = await CheckLimitsMean(measures,FreqWeeks,FreqSamples);
            //--debug-javascript --no-stop-slow-scripts --javascript-delay 10000 --enable-javascript
            return View("_GetProjectReportTimelLineGraphs", model);
           
        }
        [HttpGet]
        public async Task<IActionResult> WaterSampleReportPDF(int? id, bool? updatereportdate, bool AddGraphs = false, string searchplace = "")
        {
            if (id == null)
            {
                return NotFound();
            }
            var measures = await _context.WaterSampleMeasures
                .Include(x => x.WaterSampleType)
                .Include(x => x.WaterSamplePlace)
                .Include(x => x.Project).ThenInclude(x => x.Division)              
                .Where(x => x.WaterSamplePlace.ProjectId.Equals(id)).OrderBy(x => x.Dato)
                .ToListAsync();
            if(updatereportdate == true)
            {
                foreach(var m in measures)
                {
                    if(m.DateReporting == null)
                    {
                        m.DateReporting = DateTime.Now.Date;
                        _context.Update(m);
                    }
                    
                }
                await _context.SaveChangesAsync();
            }
            if (AddGraphs == true)
            {
                ViewBag.AddTheGraphs = true;
            }
            else
            {
                ViewBag.AddTheGraphs = false;
            }
            if (searchplace == "")
            {
                ViewBag.SearchPlace = "";
            }
            else
            {
                ViewBag.SearchPlace = searchplace;
            }
            List<WaterSampleMeasVM> model = await CheckLimits(measures);
            //List<WaterSampleMeasVM> model = await CheckLimitsMean(measures,FreqWeeks,FreqSamples);
            //--debug-javascript --no-stop-slow-scripts --javascript-delay 10000 --enable-javascript
            string footer = "--debug-javascript --no-stop-slow-scripts --javascript-delay 8000  --footer-center \"" + _sharedLocalizer.GetLocalizedHtmlString("Printed on:").Value + DateTime.Now.Date.ToString("yyyy-MM-dd") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
            return new ViewAsPdf("_GetProjectReportTimelLinePDF", model)
            {
                FileName = "Water Sample Report.pdf",
                PageOrientation = Orientation.Landscape,
                PageSize = Size.A3,
                CustomSwitches = footer
                
            };
        }
        [HttpGet]
        public async Task<IActionResult> WaterSampleReportPDF2(int? id, bool? updatereportdate, bool AddGraphs = false, string searchplace = "")
        {
            if (id == null)
            {
                return NotFound();
            }
            var measures = await _context.WaterSampleMeasures
                .Include(x => x.WaterSampleType)
                .Include(x => x.WaterSamplePlace)
                .Include(x => x.Project).ThenInclude(x => x.Division)
                .Where(x => x.WaterSamplePlace.ProjectId.Equals(id)).OrderBy(x => x.Dato)
                .ToListAsync();
            if (AddGraphs == true)
            {
                ViewBag.AddTheGraphs = true;
            }
            else
            {
                ViewBag.AddTheGraphs = false;
            }
            if (searchplace == "")
            {
                ViewBag.SearchPlace = "";
            }
            else
            {
                ViewBag.SearchPlace = searchplace;
            }
            if (updatereportdate == true)
            {
                foreach (var m in measures)
                {
                    if (m.DateReporting == null)
                    {
                        m.DateReporting = DateTime.Now.Date;
                        _context.Update(m);
                    }

                }
                await _context.SaveChangesAsync();
            }
            //ViewBag.AddGraphs = AddGraphs;
            List<WaterSampleMeasVM> model = await CheckLimits(measures);
            //List<WaterSampleMeasVM> model = await CheckLimitsMean(measures,FreqWeeks,FreqSamples);
            string footer = " --debug-javascript --no-stop-slow-scripts --javascript-delay 10000 --enable-javascript --footer-center \"" + _sharedLocalizer.GetLocalizedHtmlString("Printed on:").Value + DateTime.Now.Date.ToString("yyyy-MM-dd") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
            return View("_GetProjectReportTimelLinePDF", model);
           
        }
        public async Task<IActionResult> WaterSampleReportSamplePlacePDF(int? id, bool? updatereportdate)
        {
            if (id == null)
            {
                return NotFound();
            }
            var measures = await _context.WaterSampleMeasures
                .Include(x => x.WaterSampleType)
                .Include(x => x.WaterSamplePlace)
                .Include(x => x.Project).ThenInclude(x => x.Division)
                .Where(x => x.WaterSamplePlaceId.Equals(id)).OrderBy(x => x.Dato)
                .ToListAsync();
            if (updatereportdate == true)
            {
                foreach (var m in measures)
                {
                    if (m.DateReporting == null)
                    {
                        m.DateReporting = DateTime.Now.Date;
                        _context.Update(m);
                    }

                }
                await _context.SaveChangesAsync();
            }

            List<WaterSampleMeasVM> model = await CheckLimits(measures);
            string footer = "--footer-center \"" + _sharedLocalizer.GetLocalizedHtmlString("Printed on:").Value + DateTime.Now.Date.ToString("yyyy-MM-dd") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
            return new ViewAsPdf("_GetProjectReportTimelLinePDF", model)
            {
                FileName = "Water Sample Report.pdf",
                PageOrientation = Orientation.Landscape,
                PageSize = Size.A3,
                CustomSwitches = footer

            };
        }
        public async Task<IActionResult> WaterSampleReportHTML(int? id, bool? updatereportdate, bool AddGraphs = false, string searchplace = "")
        {
            if (id == null)
            {
                return NotFound();
            }
            var measures = await _context.WaterSampleMeasures
                .Include(x => x.WaterSampleType)
                .Include(x => x.WaterSamplePlace)
                .Include(x => x.Project).ThenInclude(x => x.Division)
                .Where(x => x.WaterSamplePlace.ProjectId.Equals(id)).OrderBy(x => x.Dato)
                .ToListAsync();
            if (updatereportdate == true)
            {
                foreach (var m in measures)
                {
                    if (m.DateReporting == null)
                    {
                        m.DateReporting = DateTime.Now.Date;
                        _context.Update(m);
                    }

                }
                await _context.SaveChangesAsync();
            }
            if (AddGraphs == true)
            {
                ViewBag.AddTheGraphs = true;
            }
            else
            {
                ViewBag.AddTheGraphs = false;
            }
            if (searchplace == "")
            {
                ViewBag.SearchPlace = "";
            }
            else
            {
                ViewBag.SearchPlace = searchplace;
            }
            List<WaterSampleMeasVM> model = await CheckLimits(measures);
            string footer = "--footer-center \"" + _sharedLocalizer.GetLocalizedHtmlString("Printed on:").Value + DateTime.Now.Date.ToString("yyyy-MM-dd") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
            return View("_GetProjectReportTimelLinePDF", model);
        }
        public async Task<IActionResult> WaterSampleReport(int? id, string search)
        {
            if(id == null)
            {
                return NotFound();
            }
            StringBuilder sb = new StringBuilder();
            var measures = await _context.WaterSampleMeasures
                .Include(x => x.WaterSampleType)
                .Include(x => x.WaterSamplePlace)
                .Where(x => x.WaterSamplePlace.ProjectId.Equals(id)).OrderBy(x => x.Dato)
                .ToListAsync();
            if(search != "" && search != null)
            {
                measures = measures.Where(x => x.WaterSamplePlace.Name.ToLower().Contains(search.ToLower())).ToList();
            }
            List<WaterSampleMeasVM> model = await CheckLimits(measures);
            var uniqueplaces = measures.Select(x => x.WaterSamplePlace).Distinct();
            var uniqueWST = measures.Select(x => x.WaterSampleType).Distinct();
            int colcount = uniqueWST.Count();
            List<string> typesrow = new List<string>(new string[colcount+6]);
            List<string> unitsrow = new List<string>(new string[colcount+6]);
            List<string> DLrow = new List<string>(new string[colcount+6]);
            List<string> fillerrow = new List<string>(new string[colcount+6]);
            List<string> limitrow;
            fillerrow[0] = "Sampling Spot";
            fillerrow[1] = "Sampling Day";
            fillerrow[2] = "Date Sample Received at Laboratory";
            fillerrow[3] = "Date Reporting";
            fillerrow[4] = "Date next sample";
            fillerrow[5] = "Initials Sample taker";
            sb.AppendLine(string.Join(";", fillerrow.ToArray()));




            foreach (WaterSamplePlace WSP in uniqueplaces.OrderBy(x => x.Name))
            {
                List<string> namerow = new List<string>(new string[colcount + 6]);
                namerow[0] = WSP.Name;
                typesrow[0] = WSP.Name;
                unitsrow[0] = WSP.Name;
                DLrow[0] = WSP.Name;
                typesrow[5] = "Test Parameter";
                unitsrow[5] = "Unit";
                DLrow[5] = "Detection Limit";
                for (int i = 0; i < colcount; i++)
                {
                    typesrow[i + 6] = uniqueWST.ElementAt(i).Komponent;
                    unitsrow[i + 6] = uniqueWST.ElementAt(i).Enhed;
                    DLrow[i + 6] = uniqueWST.ElementAt(i).DL.ToString();
                }
                sb.AppendLine(string.Join(";", namerow.ToArray()));
                sb.AppendLine(string.Join(";", typesrow.ToArray()));
                sb.AppendLine(string.Join(";", unitsrow.ToArray()));
                sb.AppendLine(string.Join(";", DLrow.ToArray()));
                foreach(DateTime d in measures.Where(x => x.WaterSamplePlaceId.Equals(WSP.Id)).Select(x => x.Dato.Date).Distinct())
                {
                    bool wasData = false;
                    fillerrow = new List<string>(new string[colcount+6]);
                    fillerrow[0] = WSP.Name;
                    fillerrow[1] = d.Date.ToString("yyyy-MM-dd");
                    limitrow = new List<string>(new string[colcount + 6]);
                    limitrow[0] = WSP.Name;
                    limitrow[1] = d.Date.ToString("yyyy-MM-dd");
                    limitrow[5] = "Within Limits";
                    int i = 0;
                    foreach(WaterSampleType wst in uniqueWST)
                    {
                        //There are entries which are the same but with dfferent unit scaling.... WHY O WHY
                        var measure = model.FirstOrDefault(x => x.Dato.Date.Equals(d.Date) && x.WaterSampleTypeId.Equals(wst.Id) && x.WaterSamplePlaceId.Equals(WSP.Id));
                        
                        if (measure != null)
                        {
                            if (measure.DateLabReceived != null)
                            {
                                fillerrow[2] = Convert.ToDateTime(measure.DateLabReceived).ToString("yyyy-MM-dd");
                            }
                            if (measure.DateReporting != null)
                            {
                                fillerrow[3] = Convert.ToDateTime(measure.DateReporting).ToString("yyyy-MM-dd");
                            }
                            if (measure.DateNextSample != null)
                            {
                                fillerrow[4] = Convert.ToDateTime(measure.DateNextSample).ToString("yyyy-MM-dd");
                            }
                            fillerrow[5] = measure.SampleTakerName;
                            
                            if(measure.holdslimit == 0)
                            {
                                limitrow[i + 6] = "N/A";
                            }
                            else if(measure.holdslimit == 1)
                            {
                                limitrow[i + 6] = "YES";
                            }
                            else
                            {
                                limitrow[i + 6] = "NO";
                            }
                            wasData = true;
                            fillerrow[i+6] = measure.value.ToString();
                        }
                        i++;
                    }
                    if(wasData == true) {
                        sb.AppendLine(string.Join(";", fillerrow.ToArray()));
                        sb.AppendLine(string.Join(";", limitrow.ToArray()));
                    }
                }
            }
            return File(System.Text.Encoding.UTF8.GetBytes(sb.ToString()),"text/csv","WaterSampleReport.csv");
            //return File(System.Text.Encoding.ASCII.GetBytes(sb.ToString()), "text/csv", "WaterSampleReport.csv");
            //return File(System.Text.Encoding.GetEncoding(1252).GetBytes(sb.ToString()), "text/csv", "WaterSampleReport.csv");
        }
        [HttpGet]
        public async Task<IActionResult> GetWaterSampleZipFile(int? ProjectId)
        {
            if (ProjectId != null)
            {
                var project = await _context.Projects.Include(x => x.SubProjects).SingleOrDefaultAsync(x => x.Id.Equals(ProjectId));
                var botsFolderPath = Path.Combine(_env.WebRootPath, "TempFolder", project.Name);

                var botFileFolders = Directory.GetDirectories(botsFolderPath);
                var zipFileMemoryStream = new MemoryStream();
                using (ZipArchive archive = new ZipArchive(zipFileMemoryStream, ZipArchiveMode.Update, leaveOpen: true))
                {
                    foreach (var botFileFolder in botFileFolders)
                    {
                        var botFilePaths = Directory.GetFiles(botFileFolder);
                        foreach (var botFilePath in botFilePaths)
                        {
                            var botFileName = Path.GetFileName(botFilePath);
                            var entry = archive.CreateEntry(botFileFolder.Split("\\").Last() + "/" + botFileName);
                            using (var entryStream = entry.Open())
                            using (var fileStream = System.IO.File.OpenRead(botFilePath))
                            {
                                await fileStream.CopyToAsync(entryStream);
                            }
                        }
                    }

                }

                zipFileMemoryStream.Seek(0, SeekOrigin.Begin);
                return File(zipFileMemoryStream, "application/octet-stream", project.ProjectNr.ToString() + ".zip");
            }
            else { return NotFound(); }

        }
    }
}
