using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MainOps.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Authorization;
using MainOps.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.IO;
using System.Text;
using MainOps.Models.ReportClasses;

namespace MainOps.Controllers
{

    public class HomeController : Controller
    {
        private readonly IHttpContextAccessor _http;
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public HomeController(DataContext context,IHttpContextAccessor httpcontext,UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _http = httpcontext;
            _context = context;
            _userManager = userManager;
            _env = env;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult LoadBackupData()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> LoadBackupData(IFormFile postedFile)
        {
            if (postedFile != null)
            {

                using (var sreader = new StreamReader(postedFile.OpenReadStream()))
                {
                    //First line is header. If header is not passed in csv then we can neglect the below line.
                    string[] headers = sreader.ReadLine().Split(';');
                    while (!sreader.EndOfStream)
                    {
                        string[] rows = sreader.ReadLine().Split(';');

                        Daily_Report_2 dr = new Daily_Report_2();

                        dr.Id = Convert.ToInt32(rows[0]);
                        dr.short_Description = rows[1];
                        dr.Report_Date = Convert.ToDateTime(rows[2]);
                        dr.StartHour = TimeSpan.Parse(rows[3]);
                        dr.EndHour = TimeSpan.Parse(rows[4]);
                        dr.Work_Performed = rows[5].Replace("\\r\\n", "\r\n");
                        dr.Extra_Works = rows[6];
                        dr.DoneBy = rows[7];
                        dr.TitleId = Convert.ToInt32(rows[8]);
                        dr.ProjectId = Convert.ToInt32(rows[9]);
                       
                        dr.Signature = rows[11];
                        dr.Amount = Convert.ToInt32(rows[12]);
                        dr.Machinery = rows[13];
                        try
                        {
                            dr.SafetyHours = TimeSpan.Parse(rows[14]);
                        }
                        catch
                        {
                            dr.SafetyHours = TimeSpan.Zero;
                        }
                        try
                        {
                            dr.StandingTime = TimeSpan.Parse(rows[15]);
                        }
                        catch
                        {
                            dr.StandingTime = TimeSpan.Zero;
                        }
                        
                        //dr.EnteredIntoDataBase = Convert.ToDateTime(rows[16]);
                        //dr.LastEditedInDataBase = Convert.ToDateTime(rows[17]);
                        dr.Checked_By = rows[18];
                        dr.Report_Checked = Convert.ToBoolean(rows[19]);
                        //dr.SubProjectId = Convert.ToInt32(rows[20]);
                        dr.OtherPeople = rows[21];
                        dr.HasPhotos = Convert.ToBoolean(rows[22]);
                        dr.OtherPeopleIDs = rows[23];

                        if(rows[10] != "")
                        {
                            dr.tobepaid = Convert.ToInt32(rows[10]);
                        }
                        if (rows[16] != "")
                        {
                            dr.EnteredIntoDataBase = Convert.ToDateTime(rows[16]);
                        }

                        if (rows[17] != "")
                        {
                            dr.LastEditedInDataBase = Convert.ToDateTime(rows[17]);
                        }
                        if (rows[20] != "")
                        {
                            dr.SubProjectId = Convert.ToInt32(rows[20]);
                        }
                        _context.Add(dr);
                    }



                }
                await _context.SaveChangesAsync();

            }
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetBackUpData()
        {
            StringBuilder sb = new StringBuilder();
            var drs = await _context.Daily_Report_2s.Where(x => x.TitleId.Equals(637) || x.TitleId.Equals(638) || x.TitleId.Equals(639)).OrderBy(x => x.Report_Date).ThenBy(x => x.StartHour).ToListAsync();
            List<string> headerrow = new List<string>(new string[] { "Id", "short_Description", "Report_Date", "Starthour", "Endhour", "Work_Performed", "Extra_Works", "DoneBy", "TitleId", "ProjectId", "tobepaid", "Signature", "Amount", "Machinery", "SafetyHours", "StandingTime", "EnteredIntoDataBase", "LastEdited", "Checked_By", "Report_Checked", "SubProjectId", "OtherPeople", "HasPhotos", "OtherPeopleIDs" });
            List<string> fillerrow = new List<string>(new string[] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "","" });
            sb.AppendLine(string.Join(";", headerrow.ToArray()));
            foreach(Daily_Report_2 dr in drs)
            {
                fillerrow[0] = dr.Id.ToString();
                fillerrow[1] = dr.short_Description;
                fillerrow[2] = dr.Report_Date.ToString();
                fillerrow[3] = dr.StartHour.ToString();
                fillerrow[4] = dr.EndHour.ToString();
                fillerrow[5] = dr.Work_Performed.Replace("\r\n","\\r\\n");
                fillerrow[6] = dr.Extra_Works;
                fillerrow[7] = dr.DoneBy;
                if (dr.TitleId.Equals(637))
                {
                    fillerrow[8] = "764";
                }
                else if (dr.TitleId.Equals(638))
                {
                    fillerrow[8] = "762";
                }
                else if (dr.TitleId.Equals(639))
                {
                    fillerrow[8] = "763";
                }
                fillerrow[9] = dr.ProjectId.ToString();
                fillerrow[10] = dr.tobepaid.ToString();
                fillerrow[11] = dr.Signature;
                fillerrow[12] = dr.Amount.ToString();
                fillerrow[13] = dr.Machinery;
                fillerrow[14] = dr.SafetyHours.ToString();
                fillerrow[15] = dr.StandingTime.ToString();
                fillerrow[16] = dr.EnteredIntoDataBase.ToString();
                fillerrow[17] = dr.LastEditedInDataBase.ToString();
                fillerrow[18] = dr.Checked_By;
                fillerrow[19] = dr.Report_Checked.ToString();
                fillerrow[20] = dr.SubProjectId.ToString();
                fillerrow[21] = dr.OtherPeople;
                fillerrow[22] = dr.HasPhotos.ToString();
                fillerrow[23] = dr.OtherPeopleIDs;
                sb.AppendLine(string.Join(";", fillerrow.ToArray()));
                
            }
            return File(System.Text.Encoding.ASCII.GetBytes(sb.ToString()), "text/csv", "dailyreportsbackup.csv");
        }
        [HttpGet]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,StorageManager")]
        public async Task<IActionResult> GetBoQExtraWorkItems(string theId)
        {
            int BoQHeadLineId = Convert.ToInt32(theId);
            var data = await _context.BoQHeadLines.Include(x => x.ExtraWorkBoQs).ThenInclude(x => x.Headers).ThenInclude(x => x.BoQItems).SingleOrDefaultAsync(x => x.Id.Equals(BoQHeadLineId));
            return PartialView("_BoQHeadLine", data);

        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,StorageManager")]
        public async Task<IActionResult> GetUserExtraWorks()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user != null)
            {
                List<BoQHeadLine> enddata = new List<BoQHeadLine>();
                //var data = await (from boq in _context.BoQHeadLines.Include(x => x.Project).Include(x => x.ExtraWorkBoQs).ThenInclude(x => x.Descriptions)
                //                  join ewbq in _context.ExtraWorkBoQs on boq.Id equals ewbq.Id
                //                  where boq.Project.DivisionId.Equals(user.DivisionId) && boq.Type.Equals("ExtraWork") && ewbq.Descriptions.Count > 0
                //                  select boq).OrderBy(x => x.ProjectId).ThenBy(x => x.BoQnum).ToListAsync();
                var data = await _context.BoQHeadLines.Include(x => x.Project).Include(x => x.ExtraWorkBoQs).ThenInclude(x => x.Descriptions)
                    .Where(x => x.Project.DivisionId.Equals(user.DivisionId)
                    && x.Type.Equals("ExtraWork")).OrderBy(x => x.ProjectId).ThenBy(x => x.BoQnum).ToListAsync();
                foreach(var item in data)
                {
                    if (item.ExtraWorkBoQs.Count > 0) 
                    {
                        enddata.Add(item);
                    }
                }
                return PartialView("_extraworkinfo", enddata);
            }
            else
            {
                return NotFound();
            }
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,StorageManager")]
        public async Task<IActionResult> GetUserDownloads()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user != null) {
                //string fullPath = _env.WebRootPath + "\\AHAK\\akonto\\Documentation\\" + user.full_name() + "\\";
                //string fullPath2 = _env.WebRootPath + "\\AHAK\\akonto\\Documentation\\" + user.full_name() + "\\TempFolder2\\";
                //List<string> paths = new List<string>();
                //if (Directory.Exists(fullPath))
                //{
                //     var filePaths = Directory.GetFiles(fullPath);
                //     foreach(string filename in filePaths)
                //     {
                //        paths.Add(filename);
                //     }
                //}
                //if (Directory.Exists(fullPath2))
                //{
                //    var fileFolders = Directory.GetDirectories(fullPath2);
                //    foreach(var folder in fileFolders)
                //    {
                //        var subfileFolders = Directory.GetDirectories(folder);
                //        foreach(var subfolder in subfileFolders) {
                //            var filePaths = Directory.GetFiles(subfolder);
                //            foreach (string filename in filePaths)
                //            {
                //                paths.Add(filename);
                //            }
                //        }
                //    }
                //}
                //return PartialView("_Downloads", paths);
                var data = await _context.PersonalFiles.Where(x => x.ApplicationUserId.Equals(user.Id)).ToListAsync();
                return PartialView("_Downloads2", data);
            }
             else
             {
                 return NotFound();
             }
        }
        public async Task<int> GetAmountDownloads()
        {
            var user = await _userManager.GetUserAsync(User);
            //int numfiles = 0;
            if (user != null)
            {
                //string fullPath = _env.WebRootPath + "\\AHAK\\akonto\\Documentation\\" + user.full_name() + "\\";
                //string fullPath2 = _env.WebRootPath + "\\AHAK\\akonto\\Documentation\\" + user.full_name() + "\\TempFolder2\\";
                //List<string> paths = new List<string>();
                //if (Directory.Exists(fullPath))
                //{
                //    var filePaths = Directory.GetFiles(fullPath);
                //    foreach (string filename in filePaths)
                //    {
                //        numfiles += 1;
                //    }
                //}
                //if (Directory.Exists(fullPath2))
                //{
                //    var fileFolders = Directory.GetDirectories(fullPath2);
                //    foreach (var folder in fileFolders)
                //    {
                //        var subfileFolders = Directory.GetDirectories(folder);
                //        foreach (var subfolder in subfileFolders)
                //        {
                //            var filePaths = Directory.GetFiles(subfolder);
                //            foreach (string filename in filePaths)
                //            {
                //                numfiles += 1;
                //            }
                //        }
                //    }
                //}
                //return numfiles;
                return await _context.PersonalFiles.Where(x => x.Downloaded.Equals(false) && user.Id.Equals(x.ApplicationUserId)).CountAsync();
            }
            else
            {
                return 0;
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,StorageManaager")]
        public IActionResult DownloadFile(string filename)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(filename);

            return File(fileBytes, "application/force-download", "download.pdf");
        }
        [HttpGet]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,StorageManaager")]
        public async Task<IActionResult> DownloadPersonalFile(int? id)
        {
            if(id != null)
            {
                var thefile = await _context.PersonalFiles.SingleOrDefaultAsync(x => x.Id.Equals(id));
                if (thefile.FileExtension.Contains("pdf")) {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(thefile.path);
                    thefile.Downloaded = true;
                    _context.Update(thefile);
                    await _context.SaveChangesAsync();
                    return File(fileBytes, "application/force-download", thefile.FileName);
                }
                else
                {
                    return NotFound();
                }
            }
            else { return NotFound(); }
            

            
        }
        [HttpPost]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,StorageManaager")]
        public IActionResult RemoveFile(string filename)
        {
            System.IO.File.Delete(filename);
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,StorageManaager")]
        public async Task<IActionResult> RemovePersonalFile(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if(id != null)
            {
                var thefile = await _context.PersonalFiles.SingleOrDefaultAsync(x => x.Id.Equals(id) && x.ApplicationUserId.Equals(user.Id));
                if(thefile != null)
                {
                    System.IO.File.Delete(thefile.path);
                    _context.Remove(thefile);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,StorageManaager")]
        public async Task<IActionResult> RemoveAllPersonalFiles()
        {
            var user = await _userManager.GetUserAsync(User);

                var thefiles = await _context.PersonalFiles.Where(x => x.ApplicationUserId.Equals(user.Id)).ToListAsync();
                foreach(var thefile in thefiles)
                {
                    System.IO.File.Delete(thefile.path);
                    _context.Remove(thefile);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Index()
        {
          
            try
            {
                // your logic
                var url = _http.HttpContext.Request.GetDisplayUrl();
                if (url.ToLower().Contains("tjaden-maps") || url.ToLower().Contains("mainops-test"))
                {
                    Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("nl-NL")),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
                );
                }
                return View();
            }
            catch (Exception ex)
            {
                // log error
                return Content($"Error in Index: {ex.Message}");
            }    

       
        }
        [AllowAnonymous]
        public async Task<ActionResult> Footer(int? id)
        {
            if(id != null) {
                
                var well = await _context.Wells
                    .Include(x => x.Project).ThenInclude(x => x.Division)
                    .Include(x => x.CoordSystem)
                    .Include(x => x.SubProject)
                    .SingleOrDefaultAsync(x => x.Id.Equals(id));
               
                    well.BentoniteLayers = await _context.BentoniteWellLayers.Include(x => x.CastingType).Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.meter_start).ToListAsync();
                    well.SoilSamples = await _context.SoilSamples.Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.sample_meter).ToListAsync();
                    well.FilterLayers = await _context.FilterWellLayers.Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.meter_start).ToListAsync();
                    well.SandLayers = await _context.SandWellLayers.Include(x => x.SandType).Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.meter_start).ToListAsync();
                    well.WellLayers = await _context.WellLayers.Include(x => x.Layer).Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.Start_m).ToListAsync();
                    return View(well);
            }
            else
            {
                return NotFound();
            }
        }
        public IActionResult MoreInformation()
        {
            return View();
        }
        public IActionResult Directions()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult ErrorMessage(string text)
        {
            ErrorModel model = new ErrorModel { ErrorText = text };
            return View("Error",model);
        }
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            ViewData["ReturnUrl"] = returnUrl;
            return LocalRedirect(returnUrl);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            //if(HttpContext.Response.StatusCode == 500)
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
