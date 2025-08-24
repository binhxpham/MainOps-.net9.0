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
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Identity;
using MainOps.ExtensionMethods;
using MainOps.Models.ViewModels;
using MainOps.Resources;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,Member,ExternalDriller,AlarmTeam")]
    public class ProjectsController : BaseController
    {
        private readonly DataContext _context;
        private readonly IStringLocalizer _localizer;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LocService _sharedLocalizer;
        private readonly IConfiguration _config;
        public ProjectsController(DataContext context,IStringLocalizer<ProjectsController> localizer, UserManager<ApplicationUser> userManager,LocService locService, IConfiguration configuration):base(context,userManager)
        {
            _context = context;
            _localizer = localizer;
            _userManager = userManager;
            _sharedLocalizer = locService;
            _config = configuration;
        }
        [HttpGet]
        [Authorize(Roles ="Admin,DivisionAdmin,Manager,AlarmTeam")]
        public async Task<IActionResult> AlertCallProjects()
        {
            var user = await _userManager.GetUserAsync(User);
            var projects =  _context.Projects.Where(x => x.DivisionId.Equals(user.DivisionId) && x.AlertCalls.Equals(true)).OrderBy(x => x.Name);
            return View(await projects.ToListAsync());
        }
        [HttpGet]
        [Authorize(Roles ="Admin,DivisionAdmin,Manager,Supervisor,ProjectMember")]
        public async Task<IActionResult> ProjectUserOverView(int? ProjectId = null,string UserId = null,DateTime? StartDate = null,DateTime? EndDate = null)
        {
            var theuser = await _userManager.GetUserAsync(User);
            var division = await _context.Divisions.FindAsync(theuser.DivisionId);
            /*var users = await (from user in _context.Users
                               join userRole in _context.UserRoles
                               on user.Id equals userRole.UserId
                               join role in _context.Roles on userRole.RoleId
                               equals role.Id
                               where userRole.UserId == user.Id && !role.Name.Equals("Guest") && !role.Name.Equals("MemberGuest")
                               && user.DivisionId.Equals(theuser.DivisionId)
                               select new
                               {
                                   Id = user.Id,
                                   NameEmail = user.full_name() + " : " + user.Email,
                                   TheUser = user
                               }).OrderBy(x => x.NameEmail).GroupBy(x => x.NameEmail).Select(grp => grp.First()).ToListAsync();*/

            var users = await (from user in _context.Users
                               join userRole in _context.UserRoles on user.Id equals userRole.UserId
                               join role in _context.Roles on userRole.RoleId equals role.Id
                               where userRole.UserId == user.Id
                                     && role.Name != "Guest"
                                     && role.Name != "MemberGuest"
                                     && user.DivisionId == theuser.DivisionId
                               select new
                               {
                                   user.Id,
                                   user.Email,
                                   NameEmail = user.full_name() + " : " + user.Email,
                                   TheUser = user
                               }).ToListAsync();  // <-- fetch to memory

            // Now handle the parts EF cannot translate:
            var result = users
                .Select(x => new
                {
                    x.Id,
                    NameEmail = x.TheUser.full_name() + " : " + x.Email,
                    x.TheUser
                })
                .OrderBy(x => x.NameEmail)
                .GroupBy(x => x.NameEmail)
                .Select(grp => grp.First())
                .ToList();


            if (!User.IsInRole("Admin") && !User.IsInRole("DivisionAdmin") && !User.IsInRole("Manager") && !User.IsInRole("Supervisor"))
            {
                //users = users.Where(x => x.NameEmail.Equals(theuser.full_name() + " : " + theuser.Email)).ToList();
                result = result.Where(x => x.NameEmail.Equals(theuser.full_name() + " : " + theuser.Email)).ToList();
            }
            ViewData["UserId"] = new SelectList(result, "Id", "NameEmail");
            ViewData["ProjectId"] = await GetProjectList();
            ProjectUserOverview PUO = new ProjectUserOverview();
            PUO.Division = division;
            PUO.EmployeeFilterId = UserId;
            PUO.ProjectFilterId = ProjectId;
            if (ProjectId == null && UserId == null)
            {
                PUO.Projects = await _context.Projects.Where(x => x.DivisionId.Equals(theuser.DivisionId) && x.Active.Equals(true)).ToListAsync();
                //var users = await _context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)).ToListAsync();
                foreach (var user in result)
                {
                    UserWithDailyReports UWDR = new UserWithDailyReports();
                    UWDR.User = user.TheUser;
                    if(StartDate == null && EndDate == null)
                    {
                        UWDR.DailyReports = await _context.Daily_Report_2s.Where(x => x.DoneBy.Equals(user.TheUser.full_name()) || x.OtherPeopleIDs.Contains(user.Id)).ToListAsync();
                    }
                    else if(StartDate != null && EndDate == null)
                    {
                        UWDR.DailyReports = await _context.Daily_Report_2s.Where(x => (x.DoneBy.Equals(user.TheUser.full_name()) || x.OtherPeopleIDs.Contains(user.Id)) && x.Report_Date.Date >= Convert.ToDateTime(StartDate).Date).ToListAsync();
                    }
                    else if(EndDate != null  && StartDate == null)
                    {
                        UWDR.DailyReports = await _context.Daily_Report_2s.Where(x => (x.DoneBy.Equals(user.TheUser.full_name()) || x.OtherPeopleIDs.Contains(user.Id)) && x.Report_Date.Date <= Convert.ToDateTime(EndDate).Date).ToListAsync();
                    }
                    else
                    {
                        UWDR.DailyReports = await _context.Daily_Report_2s.Where(x => (x.DoneBy.Equals(user.TheUser.full_name()) || x.OtherPeopleIDs.Contains(user.Id)) && x.Report_Date.Date >= Convert.ToDateTime(StartDate).Date && x.Report_Date.Date <= Convert.ToDateTime(EndDate).Date).ToListAsync();
                    }
                    PUO.UsersWithDailyReports.Add(UWDR);
                }

                return View(PUO);
            }
            else if(ProjectId != null && UserId == null)
            {
                
                PUO.Projects = await _context.Projects.Where(x => x.DivisionId.Equals(theuser.DivisionId) && x.Id.Equals(ProjectId) && x.Active.Equals(true)).ToListAsync();
                foreach (var user in result)
                {
                    UserWithDailyReports UWDR = new UserWithDailyReports();
                    UWDR.User = user.TheUser;
                    if (StartDate == null && EndDate == null)
                    {
                        UWDR.DailyReports = await _context.Daily_Report_2s.Where(x => x.ProjectId.Equals(ProjectId) && x.DoneBy.Equals(user.TheUser.full_name()) || x.OtherPeopleIDs.Contains(user.Id)).ToListAsync();
                    }
                    else if (StartDate != null && EndDate == null)
                    {
                        UWDR.DailyReports = await _context.Daily_Report_2s.Where(x => x.ProjectId.Equals(ProjectId) && (x.DoneBy.Equals(user.TheUser.full_name()) || x.OtherPeopleIDs.Contains(user.Id)) && x.Report_Date.Date >= Convert.ToDateTime(StartDate).Date).ToListAsync();
                    }
                    else if (EndDate != null && StartDate == null)
                    {
                        UWDR.DailyReports = await _context.Daily_Report_2s.Where(x => x.ProjectId.Equals(ProjectId) && (x.DoneBy.Equals(user.TheUser.full_name()) || x.OtherPeopleIDs.Contains(user.Id)) && x.Report_Date.Date <= Convert.ToDateTime(EndDate).Date).ToListAsync();
                    }
                    else
                    {
                        UWDR.DailyReports = await _context.Daily_Report_2s.Where(x => x.ProjectId.Equals(ProjectId) && (x.DoneBy.Equals(user.TheUser.full_name()) || x.OtherPeopleIDs.Contains(user.Id)) && x.Report_Date.Date >= Convert.ToDateTime(StartDate).Date && x.Report_Date.Date <= Convert.ToDateTime(EndDate).Date).ToListAsync();
                    }
                    PUO.UsersWithDailyReports.Add(UWDR);
                }

                return View(PUO);
            }
            else if(UserId != null && ProjectId == null)
            {
                PUO.Projects = await _context.Projects.Where(x => x.DivisionId.Equals(theuser.DivisionId) && x.Active.Equals(true)).ToListAsync();
                foreach (var user in result)
                {
                    if (user.Id.Equals(UserId)) {
                        UserWithDailyReports UWDR = new UserWithDailyReports();
                        UWDR.User = user.TheUser;
                        if (StartDate == null && EndDate == null)
                        {
                            UWDR.DailyReports = await _context.Daily_Report_2s.Where(x => x.DoneBy.Equals(user.TheUser.full_name()) || x.OtherPeopleIDs.Contains(user.Id)).ToListAsync();
                        }
                        else if (StartDate != null && EndDate == null)
                        {
                            UWDR.DailyReports = await _context.Daily_Report_2s.Where(x => (x.DoneBy.Equals(user.TheUser.full_name()) || x.OtherPeopleIDs.Contains(user.Id)) && x.Report_Date.Date >= Convert.ToDateTime(StartDate).Date).ToListAsync();
                        }
                        else if (EndDate != null && StartDate == null)
                        {
                            UWDR.DailyReports = await _context.Daily_Report_2s.Where(x => (x.DoneBy.Equals(user.TheUser.full_name()) || x.OtherPeopleIDs.Contains(user.Id)) && x.Report_Date.Date <= Convert.ToDateTime(EndDate).Date).ToListAsync();
                        }
                        else
                        {
                            UWDR.DailyReports = await _context.Daily_Report_2s.Where(x => (x.DoneBy.Equals(user.TheUser.full_name()) || x.OtherPeopleIDs.Contains(user.Id)) && x.Report_Date.Date >= Convert.ToDateTime(StartDate).Date && x.Report_Date.Date <= Convert.ToDateTime(EndDate).Date).ToListAsync();
                        }
                        PUO.UsersWithDailyReports.Add(UWDR);
                    }
                }

                return View(PUO);
            }
            else
            {
                PUO.Projects = await _context.Projects.Where(x => x.DivisionId.Equals(theuser.DivisionId) && x.Id.Equals(ProjectId) && x.Active.Equals(true)).ToListAsync();
                foreach (var user in result)
                {
                    if (user.Id.Equals(UserId))
                    {
                        UserWithDailyReports UWDR = new UserWithDailyReports();
                        UWDR.User = user.TheUser;
                        if (StartDate == null && EndDate == null)
                        {
                            UWDR.DailyReports = await _context.Daily_Report_2s.Where(x => x.ProjectId.Equals(ProjectId) && x.DoneBy.Equals(user.TheUser.full_name()) || x.OtherPeopleIDs.Contains(user.Id)).ToListAsync();
                        }
                        else if (StartDate != null && EndDate == null)
                        {
                            UWDR.DailyReports = await _context.Daily_Report_2s.Where(x => x.ProjectId.Equals(ProjectId) && (x.DoneBy.Equals(user.TheUser.full_name()) || x.OtherPeopleIDs.Contains(user.Id)) && x.Report_Date.Date >= Convert.ToDateTime(StartDate).Date).ToListAsync();
                        }
                        else if (EndDate != null && StartDate == null)
                        {
                            UWDR.DailyReports = await _context.Daily_Report_2s.Where(x => x.ProjectId.Equals(ProjectId) && (x.DoneBy.Equals(user.TheUser.full_name()) || x.OtherPeopleIDs.Contains(user.Id)) && x.Report_Date.Date <= Convert.ToDateTime(EndDate).Date).ToListAsync();
                        }
                        else
                        {
                            UWDR.DailyReports = await _context.Daily_Report_2s.Where(x => x.ProjectId.Equals(ProjectId) && (x.DoneBy.Equals(user.TheUser.full_name()) || x.OtherPeopleIDs.Contains(user.Id)) && x.Report_Date.Date >= Convert.ToDateTime(StartDate).Date && x.Report_Date.Date <= Convert.ToDateTime(EndDate).Date).ToListAsync();
                        }
                        PUO.UsersWithDailyReports.Add(UWDR);
                    }
                }
                return View(PUO);
            }

        }
        // GET: Projects
        [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
        public async Task<IActionResult> Index()
        {
            var theuser = await _userManager.GetUserAsync(User);
            if(theuser.Active == true) { 
            ViewData["Title"] = _localizer["Index"];
            ViewData["TemplateId"] = new SelectList(_context.Projects.Where(x => x.DivisionId.Equals(theuser.DivisionId) && x.IsTemplate.Equals(true)), "Id", "Name");
            if (User.IsInRole("Admin"))
            {
                return View(await _context.Projects.Where(x=>x.Active.Equals(true)).Include(x=>x.Department).Include(x => x.SubProjects).Include(x => x.CoordSystem).ToListAsync());
            }
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You are inactive" });
            }
            return View(await _context.Projects.Include(x=>x.SubProjects).Include(x => x.Department).Include(x=>x.CoordSystem).Where(x=>x.DivisionId.Equals(theuser.DivisionId) && x.Active.Equals(true)).ToListAsync());
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MoveLagerarbejde()
        {
            var dailyreportslagerarbejde = await _context.Daily_Report_2s.Where(x => x.ProjectId.Equals(58)).ToListAsync();
            var hjitems = await _context.Item_Locations.Where(x => x.ProjectId.Equals(58)).ToListAsync();
            foreach(var item in dailyreportslagerarbejde)
            {
                if (item.SubProjectId.Equals(228)) //bådehavnsgade
                {
                    item.SubProjectId = null;
                    item.ProjectId = 396; // Bådehavnsgade project
                }
                else if(item.SubProjectId.Equals(229)) // Ejbyvej
                {
                    item.SubProjectId = null;
                    item.ProjectId = 395; // Ejbyvej project

                }
                else if (item.SubProjectId.Equals(230)) //Glostrup
                {
                    item.SubProjectId = null;
                    item.ProjectId = 333; // Bådehavnsgade project
                }
                else //put on Glostrup if date < 22 Nov else Baldersbuen
                {
                    item.SubProjectId = null;
                    item.ProjectId = 333; // Bådehavnsgade project
                }
                _context.Update(item);
            }
            foreach (var item in hjitems)
            {
                if (item.SubProjectId.Equals(228)) //bådehavnsgade
                {
                    item.SubProjectId = null;
                    item.ProjectId = 396; // Bådehavnsgade project
                }
                else if (item.SubProjectId.Equals(229)) // Ejbyvej
                {
                    item.SubProjectId = null;
                    item.ProjectId = 395; // Ejbyvej project

                }
                else if (item.SubProjectId.Equals(230)) //Glostrup
                {
                    item.SubProjectId = null;
                    item.ProjectId = 333; // Bådehavnsgade project
                }
                else //put on Glostrup if date < 22 Nov else Baldersbuen
                {
                    item.SubProjectId = null;
                    item.ProjectId = 333; // Bådehavnsgade project
                }
                _context.Update(item);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
        public async Task<IActionResult> OldProjects()
        {
            var theuser = await _userManager.GetUserAsync(User);
            ViewData["Title"] = _localizer["Index"];
            if (User.IsInRole("Admin"))
            {
                return View("Index",await _context.Projects.Where(x => x.Active.Equals(false)).Include(x => x.Department).Include(x => x.SubProjects).Include(x => x.CoordSystem).ToListAsync());
            }
            else
            {
                return View("Index",await _context.Projects.Where(x => x.Active.Equals(false) && x.DivisionId.Equals(theuser.DivisionId)).Include(x => x.SubProjects).Include(x => x.CoordSystem).ToListAsync());
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
        public IActionResult ActiveProjects()
        {
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Member,DivisionAdmin,Guest,ProjectMember")]
        public async Task<IActionResult> GetDocuments(int? id)
        {
            if (id != null)
            {
                var documents = await _context.Documents
                    .Include(x=>x.MeasPoint).Include(x => x.DocumentType).Include(x=>x.SubProject)
                    .Where(x => x.ProjectId.Equals(id) || x.MeasPoint.ProjectId.Equals(id) || x.SubProject.ProjectId.Equals(id)).ToListAsync();
                return PartialView("_DocumentInfo", documents);
            }
            else
            {
                List<Document> nodocu = new List<Document>();
                return PartialView("_DocumentInfo", nodocu);
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Member,DivisionAdmin,Guest,ProjectMember")]
        public async Task<IActionResult> GetAlertDocuments(int? id)
        {
            if (id != null)
            {
                var documents = await _context.Documents
                    .Include(x => x.MeasPoint).Include(x => x.DocumentType).Include(x => x.SubProject)
                    .Where(x => (x.ProjectId.Equals(id) || x.MeasPoint.ProjectId.Equals(id) || x.SubProject.ProjectId.Equals(id)) && (x.DocumentTypeId.Equals(16) || x.DocumentTypeId.Equals(31))).ToListAsync();
                return PartialView("_DocumentInfo", documents);
            }
            else
            {
                List<Document> nodocu = new List<Document>();
                return PartialView("_DocumentInfo", nodocu);
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PopulateGroutProject()
        {
            int oldprojectid = 70;
            int newprojectid = 81;
            var boqheadlines = await _context.BoQHeadLines.Where(x => x.ProjectId.Equals(oldprojectid)).ToListAsync();
            var itemtypes = await _context.ItemTypes.Where(x => x.ProjectId.Equals(oldprojectid)).ToListAsync();
            var titles = await _context.Titles.Where(x => x.ProjectId.Equals(oldprojectid)).ToListAsync();
            var boqheadlinesold = await _context.BoQHeadLines.Where(x => x.ProjectId.Equals(newprojectid)).ToListAsync();
            var itemtypesold = await _context.ItemTypes.Where(x => x.ProjectId.Equals(newprojectid)).ToListAsync();
            var titlesold = await _context.Titles.Where(x => x.ProjectId.Equals(newprojectid)).ToListAsync();
            foreach (var item in boqheadlinesold)
            {
                _context.Remove(item);
            }
            await _context.SaveChangesAsync();
            foreach (var item in titlesold)
            {
                _context.Remove(item);
            }
            await _context.SaveChangesAsync();
            foreach (var item in itemtypesold)
            {
                _context.Remove(item);
            }

            await _context.SaveChangesAsync();
            foreach (var item in boqheadlines)
            {
                BoQHeadLine boq = new BoQHeadLine(item, newprojectid);
                _context.Add(boq);
            }
            await _context.SaveChangesAsync();
            foreach (var item in itemtypes)
            {
                ItemType it = new ItemType(item, newprojectid);
                _context.Add(it);
            }
            await _context.SaveChangesAsync();
            foreach (var item in titles)
            {
                var itemtyp = await _context.ItemTypes.Where(x => x.Item_Type.Equals(item.TheTitle) && x.ProjectId.Equals(newprojectid)).SingleOrDefaultAsync();
                Title tit = new Title(item, newprojectid, itemtyp.Id);
                _context.Add(tit);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> UploadProjects(IFormFile postedFile)
        {
            //this ONLY WORKS FOR TJADEN DIVISION - no automatic division choice!
            if (postedFile != null)
            {
                string fileExtension = Path.GetExtension(postedFile.FileName);
                
                if (fileExtension != ".csv")
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "please provide a CSV file!" });
                }
                using (var sreader = new StreamReader(postedFile.OpenReadStream()))
                {
                    var user = await _userManager.GetUserAsync(User);
                    //WO-number,Description,Project team,Project leader,Performer,Debtor,Starting date,Ready before,Postcode,Working address
                    List<string> headers = sreader.ReadLine().Split(';').ToList();
                    int AddressIndex = headers.IndexOf("Working address");
                    int PostalIndex = headers.IndexOf("Postcode");
                    int WONumberIndex = headers.IndexOf("WO-number");
                    int DescriptionIndex = headers.IndexOf("Description");
                    int ProjectLeaderIndex = headers.IndexOf("Performer");
                    int DebtorIndex = headers.IndexOf("Debtor");
                    int StartingDayIndex = headers.IndexOf("Starting date");
                    int EndingDayIndex = headers.IndexOf("Ready before");
                    int DivisionId = user.DivisionId;
                    int DepartmentIndex = headers.IndexOf("Project team");
                    int CityIndex = headers.IndexOf("City");
                    int ClientContact = headers.IndexOf("Contactpersoon Opdracht");
                    int ClientPhone = headers.IndexOf("Client contact phone number");
                    var departments = await _context.Departments.ToListAsync();
                    if (User.IsInRole("Admin"))
                    {
                        DivisionId = 4; // so that Rho uploads to Tjaden, else it uses DivisionId of the user! (which will most likely always be 4!
                    }
                    while (!sreader.EndOfStream)
                    {
                        string[] rows = sreader.ReadLine().Split(';');
                        var prev_proj = await _context.Projects.SingleOrDefaultAsync(x => x.ProjectNr.Equals(rows[WONumberIndex].Replace("\"", "").Replace("'", "")));
                        if(prev_proj == null) { 
                            if (rows[PostalIndex].Replace("\"", "").Replace("'", "") != "" && rows[AddressIndex].Replace("\"", "").Replace("'", "") != "")
                            {
                                Project new_Project = new Project();
                                new_Project.CoordSystemId = 3; // latitude longitude
                                new_Project.Address = rows[AddressIndex].Replace("\"", "").Replace("'","");
                                new_Project.AddressLine1 = rows[AddressIndex].Replace("\"", "").Replace("'", "");
                                new_Project.PostalCode = rows[PostalIndex].Replace("\"", "").Replace("'", "");
                                new_Project.Name = rows[DescriptionIndex].Replace("\"", "").Replace("'", "");
                                new_Project.Description = rows[DescriptionIndex].Replace("\"", "").Replace("'", "");
                                new_Project.ProjectNr = rows[WONumberIndex].Replace("\"", "").Replace("'", "");
                                new_Project.Responsible_Person = rows[ProjectLeaderIndex].Replace("\"", "").Replace("'", "");
                                new_Project.Client = rows[DebtorIndex].Replace("\"", "").Replace("'", "");
                                new_Project.DivisionId = DivisionId;
                                new_Project.DepartmentId = departments.FirstOrDefault(x => x.DepartmentName.ToLower().Equals(rows[DepartmentIndex].Replace("\"", "").ToLower())).Id;
                                new_Project.Abbreviation = new_Project.Name.Split(" ").FirstOrDefault();
                                new_Project.ClientContact = rows[ClientContact].Replace("\"", "").Replace("'", "");
                                new_Project.ClientPhone = rows[ClientPhone].Replace("\"","").Replace("'", "");
                                new_Project.City = rows[CityIndex].Replace("\"", "").Replace("'", "");
                                new_Project.Active = true;
                                try
                                {
                                    if(rows[StartingDayIndex].Replace("\"","").Replace("'", "") != "") { 
                                        new_Project.StartDate = Convert.ToDateTime(rows[StartingDayIndex].Replace("\"", "").Replace("'", ""));
                                    }
                                    if(rows[EndingDayIndex].Replace("\"","").Replace("'", "") != "") { 
                                        new_Project.EndDate = Convert.ToDateTime(rows[EndingDayIndex].Replace("\"", "").Replace("'", ""));
                                        
                                    }
                                }
                                catch
                                {
                                    //
                                }
                                try
                                {
                                    string requestUrl = "https://maps.googleapis.com/maps/api/geocode/xml?address={0}&key={2}"; //&region={1}
                                    requestUrl = requestUrl.Replace("{0}", new_Project.GoogleAddress);
                                    requestUrl = requestUrl.Replace("{2}", _config["GoogleApiKey"]);
                                    var result = new System.Net.WebClient().DownloadString(requestUrl);
                                    var xmlElm = XElement.Parse(result);
                                    var status = (from elm in xmlElm.Descendants()
                                                  where
                                                      elm.Name == "status"
                                                  select elm).FirstOrDefault();

                                    if (status.Value.ToLower() == "ok")
                                    {
                                        var locationResult = (from elm in xmlElm.Descendants()
                                                              where
                                                                  elm.Name == "location"
                                                              select elm);

                                        foreach (XElement item in locationResult)
                                        {
                                            new_Project.Latitude = float.Parse(item.Elements().Where(e => e.Name.LocalName == "lat").FirstOrDefault().Value);
                                            new_Project.Longitude = float.Parse(item.Elements().Where(e => e.Name.LocalName == "lng").FirstOrDefault().Value);
                                        }
                                        _context.Add(new_Project);
                                    }
                                }
                                catch
                                {

                                }

                            }
                        }
                        else // This needs to be an update procedure instead!
                        {
                            
                            if (rows[PostalIndex].Replace("\"", "").Replace("'", "") != "" && rows[AddressIndex].Replace("\"", "").Replace("'", "") != "")
                            {

                                    prev_proj.Address = rows[AddressIndex].Replace("\"", "").Replace("'", "");
                                    prev_proj.AddressLine1 = rows[AddressIndex].Replace("\"", "").Replace("'", "");
                                    prev_proj.PostalCode = rows[PostalIndex].Replace("\"", "").Replace("'", "");
                                    prev_proj.Name = rows[DescriptionIndex].Replace("\"", "").Replace("'", "");
                                    prev_proj.Description = rows[DescriptionIndex].Replace("\"", "").Replace("'", "");
                                    prev_proj.ProjectNr = rows[WONumberIndex].Replace("\"", "").Replace("'", "");
                                    prev_proj.Responsible_Person = rows[ProjectLeaderIndex].Replace("\"", "").Replace("'", "");
                                    prev_proj.Client = rows[DebtorIndex].Replace("\"", "").Replace("'", "");
                                    prev_proj.DivisionId = DivisionId;
                                    prev_proj.ClientContact = rows[ClientContact].Replace("\"", "").Replace("'", "");
                                    prev_proj.ClientPhone = rows[ClientPhone].Replace("\"", "").Replace("'", "");
                                    prev_proj.City = rows[CityIndex].Replace("\"", "").Replace("'", "");
                                    prev_proj.DepartmentId = departments.FirstOrDefault(x => x.DepartmentName.ToLower().Equals(rows[DepartmentIndex].Replace("\"", "").ToLower())).Id;
                                    prev_proj.Abbreviation = prev_proj.Name.Split(" ").FirstOrDefault();
                                    prev_proj.Active = true;
                                try
                                {
                                    if (rows[StartingDayIndex].Replace("\"", "").Replace("'", "") != "")
                                    {
                                            prev_proj.StartDate = Convert.ToDateTime(rows[StartingDayIndex].Replace("\"", "").Replace("'", ""));
                                    }
                                    if (rows[EndingDayIndex].Replace("\"", "").Replace("'", "") != "")
                                    {
                                            prev_proj.EndDate = Convert.ToDateTime(rows[EndingDayIndex].Replace("\"", "").Replace("'", ""));

                                    }
                                }
                                catch
                                {
                                    //
                                }
                                if (prev_proj.Latitude == null || prev_proj.Latitude == 0 || prev_proj.Longitude == null) 
                                { 
                                    try
                                    {
                                        string requestUrl = "https://maps.googleapis.com/maps/api/geocode/xml?address={0}&key={2}"; //&region={1}
                                        requestUrl = requestUrl.Replace("{0}", prev_proj.GoogleAddress);
                                        requestUrl = requestUrl.Replace("{2}", _config["GoogleApiKey"]);
                                        var result = new System.Net.WebClient().DownloadString(requestUrl);
                                        var xmlElm = XElement.Parse(result);
                                        var status = (from elm in xmlElm.Descendants()
                                                      where
                                                          elm.Name == "status"
                                                      select elm).FirstOrDefault();

                                        if (status.Value.ToLower() == "ok")
                                        {
                                            var locationResult = (from elm in xmlElm.Descendants()
                                                                  where
                                                                      elm.Name == "location"
                                                                  select elm);

                                            foreach (XElement item in locationResult)
                                            {
                                                prev_proj.Latitude = float.Parse(item.Elements().Where(e => e.Name.LocalName == "lat").FirstOrDefault().Value);
                                                prev_proj.Longitude = float.Parse(item.Elements().Where(e => e.Name.LocalName == "lng").FirstOrDefault().Value);
                                            }
                                            
                                        }
                                    }
                                    catch
                                    {

                                    }
                                }
                                _context.Update(prev_proj);
                            }
                        }
                    }
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        [Authorize(Roles = "Admin,ProjectMember,DivisionAdmin,Manager,Member")]
        public async Task<IActionResult> ProjectOverView()
        {
            var user = await _userManager.GetUserAsync(User);
            List<Project> projects;
            List<Project> projectsFinal = new List<Project>();
            if (User.IsInRole("Admin")) { 
                projects = await _context.Projects.Include(x => x.Department)
                .Where(x => x.Active.Equals(true)).ToListAsync(); // 
            }
            else
            {
                projects = await _context.Projects.Include(x => x.Department)
                .Where(x => x.DivisionId.Equals(user.DivisionId) && x.Active.Equals(true)).ToListAsync();
            }
            //&& x.Latitude != null
            bool needsUpdate = false;
            foreach(var proj in projects)
            {
                if(proj.Latitude == null || proj.Latitude == 0 || proj.Longitude == null)
                {
                    try { 
                        string requestUrl = "https://maps.googleapis.com/maps/api/geocode/xml?address={0}&key={2}"; //&region={1}
                        requestUrl = requestUrl.Replace("{0}", proj.GoogleAddress);
                        requestUrl = requestUrl.Replace("{2}", _config["GoogleApiKey"]);
                        var result = new System.Net.WebClient().DownloadString(requestUrl);
                        var xmlElm = XElement.Parse(result);
                        var status = (from elm in xmlElm.Descendants()
                                  where
                                      elm.Name == "status"
                                  select elm).FirstOrDefault();
 
                        if (status.Value.ToLower() == "ok") 
                        { 
                            var locationResult = (from elm in xmlElm.Descendants()
                                                  where
                                                      elm.Name == "location"
                                                  select elm);

                            foreach (XElement item in locationResult)
                            {
                                proj.Latitude = float.Parse(item.Elements().Where(e => e.Name.LocalName == "lat").FirstOrDefault().Value);
                                proj.Longitude = float.Parse(item.Elements().Where(e => e.Name.LocalName == "lng").FirstOrDefault().Value);
                                _context.Update(proj);
                            }
                            needsUpdate = true;
                        }
                    }
                    catch
                    {

                    }
                }
            }
            foreach(var proj in projects)
            {
                if(proj.Latitude != null && proj.Longitude != null && proj.Latitude != 0)
                {
                    projectsFinal.Add(proj);
                }
            }
            foreach(var proj in projectsFinal)
            {
                int i = 1;
                int thecount = projectsFinal.Where(x => x.Id != proj.Id && x.Latitude == proj.Latitude && x.Longitude == proj.Longitude).ToList().Count();
                foreach (var otherproj in projectsFinal.Where(x => x.Id != proj.Id && x.Latitude == proj.Latitude && x.Longitude == proj.Longitude).ToList())
                {
                    var a = 360.0 / thecount;
                    otherproj.Latitude = proj.Latitude + -.00015 * Math.Cos((+a * i) / 180 * Math.PI);  //x
                    otherproj.Longitude = proj.Longitude + -.00015 * Math.Sin((+a * i) / 180 * Math.PI);  //Y
                    i += 1;
                }
            }
            if(needsUpdate == true)
            {
                await _context.SaveChangesAsync();
            }
            var subprojects = await _context.SubProjects.Include(x => x.Project).ThenInclude(x=>x.Division)
                .Where(x => x.Project.DivisionId.Equals(user.DivisionId) && x.Project.Active.Equals(true) && x.Latitude != null).ToListAsync();
            ProjectsOverViewVM model = new ProjectsOverViewVM(projectsFinal,subprojects,_sharedLocalizer);
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["SubProjectId"] = new SelectList(subprojects, "Id", "Name");
            return View(model);
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,Member,ExternalDriller")]
        public JsonResult GetProjectId(string theId)
        {
            int Id = Convert.ToInt32(theId);
            var mp = _context.MeasPoints.Find(Id);
            var project = _context.Projects.Find(mp.ProjectId);
            return Json(project);
        }
        // GET: Projects/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user.Active == false)
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You are inactive" });
            }
            var project = await _context.Projects.Include(x => x.CoordSystem).Include(x => x.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }
            ViewData["Title"] = _localizer["Details"];
            return View(project);
        }
        [AllowAnonymous]
        public async Task<IActionResult> Details_Partial(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.Include(x => x.CoordSystem).Include(x => x.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }
            ViewData["Title"] = _localizer["Details"];
            return PartialView("_Details",project);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user.Active == false)
            {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "You are inactive" });
            }
            ViewData["Title"] = _localizer["Create"];
            ViewData["CoordSystemId"] = new SelectList(_context.CoordSystems.OrderBy(x => x.system), "Id", "system");
            if (User.IsInRole("Admin"))
            {
                ViewData["DivisionId"] = new SelectList(_context.Divisions.OrderBy(x => x.Name), "Id", "Name");
                ViewData["DepartmentId"] = new SelectList(_context.Departments.OrderBy(x => x.DepartmentName), "Id", "DepartmentName");
            }
            else
            {
                ViewData["DivisionId"] = new SelectList(_context.Divisions.Where(x => x.Id.Equals(user.DivisionId)), "Id", "Name");
                ViewData["DepartmentId"] = new SelectList(_context.Departments.Where(x=>x.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.DepartmentName), "Id", "DepartmentName");
            }
            
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> Create([Bind("Id,ProjectNr,Name,Abbreviation,Client,DepartmentId,DivisionId,CoordSystemId,Active,Latitude,Longitude,ClientEmail,Responsible_Person,ImplementBreakTime,IsTemplate,Description,StartDate,EndDate,AddressAddressLine1,City,PostalCode,Country,AlertCalls,ClientContactAlert,ClientAlertPhone")] Project project)
        {
            if (ModelState.IsValid)
            {
                var theuser = await _userManager.GetUserAsync(User);
                if (!User.IsInRole("Admin")) { 
                    project.DivisionId = theuser.DivisionId;
                }
                _context.Add(project);
                await _context.SaveChangesAsync();
                var lastadded = await _context.Projects.LastAsync();
                var result = await PopulateProjectAtStart(lastadded.Id);
                return RedirectToAction(nameof(Index));
            }
            var user = await _userManager.GetUserAsync(User);
            ViewData["CoordSystemId"] = new SelectList(_context.CoordSystems.OrderBy(x => x.system), "Id", "system");
            if (User.IsInRole("Admin"))
            {
                ViewData["DivisionId"] = new SelectList(_context.Divisions.OrderBy(x => x.Name), "Id", "Name");
                ViewData["DepartmentId"] = new SelectList(_context.Departments.OrderBy(x => x.DepartmentName), "Id", "DepartmentName");
            }
            else
            {
                ViewData["DivisionId"] = new SelectList(_context.Divisions.Where(x => x.Id.Equals(user.DivisionId)), "Id", "Name");
                ViewData["DepartmentId"] = new SelectList(_context.Departments.Where(x => x.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.DepartmentName), "Id", "DepartmentName");
            }
            ViewData["Title"] = _localizer["Create"];
            return View(project);
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,Member")]
        public async Task<bool> PopulateProjectAtStart(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.DivisionId.Equals(1))
            {
                var itemtypes = await _context.ItemTypes.Where(x => x.ProjectId.Equals(45)).Include(x => x.Rental_Unit).ToListAsync();
                var existingitemtypes = await _context.ItemTypes.Where(x => x.ProjectId.Equals(id)).ToListAsync();
                var existingtitles = await _context.Titles.Where(x => x.ProjectId.Equals(id)).ToListAsync();
                var titles = await _context.Titles.Where(x => x.ProjectId.Equals(45)).ToListAsync();
                var existingBoQheadlines = await _context.BoQHeadLines.Where(x => x.ProjectId.Equals(id)).ToListAsync();
                var BoQheadlines = await _context.BoQHeadLines.Where(x => x.ProjectId.Equals(45)).ToListAsync();
                foreach (ItemType IT in itemtypes)
                {
                    if (existingitemtypes.Find(x => x.Item_Type.Equals(IT.Item_Type)) != null)
                    {

                    }
                    else
                    {
                        ItemType ITnew = new ItemType
                        {
                            Item_Type = IT.Item_Type,
                            BoQnr = IT.BoQnr,
                            BoQnr_Rental = IT.BoQnr_Rental,
                            Install_UnitId = IT.Install_UnitId,
                            Rental_UnitId = IT.Rental_UnitId,
                            price = IT.price,
                            rental_price = IT.rental_price,
                            Valuta = IT.Valuta,
                            ProjectId = id,
                            daily_cost = IT.daily_cost,
                            initial_cost = IT.initial_cost
                        };
                        _context.Add(ITnew);
                    }
                }
                await _context.SaveChangesAsync();
                foreach (Title t in titles)
                {
                    if (existingtitles.Find(x => x.TheTitle.Equals(t.TheTitle)) != null)
                    {

                    }
                    else
                    {
                        var theitemtype = await _context.ItemTypes.Where(x => x.ProjectId.Equals(id) && x.Item_Type.Equals(t.ItemType.Item_Type)).SingleOrDefaultAsync();
                        Title t_new = new Title
                        {
                            ItemTypeId = theitemtype.Id,
                            TheTitle = t.TheTitle,
                            ProjectId = id
                        };
                        _context.Add(t_new);
                    }
                }
                await _context.SaveChangesAsync();
                foreach (BoQHeadLine boq in BoQheadlines)
                {
                    if (existingBoQheadlines.Find(x => x.HeadLine.Equals(boq.HeadLine)) != null)
                    {

                    }
                    else
                    {
                        BoQHeadLine newBoQ = new BoQHeadLine
                        {
                            HeadLine = boq.HeadLine,
                            BoQnum = boq.BoQnum,
                            ProjectId = id,
                            Type = boq.Type
                        };
                        _context.Add(newBoQ);
                    }

                }
            }
            else
            {
                var itemtypes = await _context.ItemTypes.Where(x => x.ProjectId.Equals(79)).Include(x => x.Rental_Unit).ToListAsync();
                var titles = await _context.Titles.Where(x => x.ProjectId.Equals(79)).ToListAsync();
                var BoQheadlines = await _context.BoQHeadLines.Where(x => x.ProjectId.Equals(79)).ToListAsync();
                var existingitemtypes = await _context.ItemTypes.Where(x => x.ProjectId.Equals(id)).ToListAsync();
                var existingtitles = await _context.Titles.Where(x => x.ProjectId.Equals(id)).ToListAsync();
                var existingBoQheadlines = await _context.BoQHeadLines.Where(x => x.ProjectId.Equals(id)).ToListAsync();
                foreach (ItemType IT in itemtypes)
                {
                    if (existingitemtypes.Find(x => x.Item_Type.Equals(IT.Item_Type)) != null)
                    {

                    }
                    else
                    {
                        ItemType ITnew = new ItemType
                        {
                            Item_Type = IT.Item_Type,
                            BoQnr = IT.BoQnr,
                            BoQnr_Rental = IT.BoQnr_Rental,
                            Install_UnitId = IT.Install_UnitId,
                            Rental_UnitId = IT.Rental_UnitId,
                            price = IT.price,
                            rental_price = IT.rental_price,
                            Valuta = IT.Valuta,
                            ProjectId = id,
                            daily_cost = IT.daily_cost,
                            initial_cost = IT.initial_cost
                        };
                        _context.Add(ITnew);
                    }
                }
                await _context.SaveChangesAsync();
                foreach (Title t in titles)
                {
                    if (existingtitles.Find(x => x.TheTitle.Equals(t.TheTitle)) != null)
                    {

                    }
                    else
                    {
                        var theitemtype = await _context.ItemTypes.Where(x => x.ProjectId.Equals(id) && x.Item_Type.Equals(t.ItemType.Item_Type)).SingleOrDefaultAsync();
                        Title t_new = new Title
                        {
                            ItemTypeId = theitemtype.Id,
                            TheTitle = t.TheTitle,
                            ProjectId = id
                        };
                        _context.Add(t_new);
                    }
                }
                await _context.SaveChangesAsync();
                foreach (BoQHeadLine boq in BoQheadlines)
                {
                    if (existingBoQheadlines.Find(x => x.HeadLine.Equals(boq.HeadLine)) != null)
                    {

                    }
                    else
                    {
                        BoQHeadLine newBoQ = new BoQHeadLine
                        {
                            HeadLine = boq.HeadLine,
                            BoQnum = boq.BoQnum,
                            ProjectId = id,
                            Type = boq.Type
                        };
                        _context.Add(newBoQ);
                    }

                }
            }
            
            await _context.SaveChangesAsync();
            return true;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PopulateProject(int id)
        {
            var itemtypes = await _context.ItemTypes.Where(x => x.ProjectId.Equals(45)).Include(x=>x.Rental_Unit).ToListAsync();
            var existingitemtypes = await _context.ItemTypes.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var existingtitles = await _context.Titles.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var titles = await _context.Titles.Where(x => x.ProjectId.Equals(45)).ToListAsync();
            var existingBoQheadlines = await _context.BoQHeadLines.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var BoQheadlines = await _context.BoQHeadLines.Where(x => x.ProjectId.Equals(45)).ToListAsync();
            foreach (ItemType IT in itemtypes)
            {
                if (existingitemtypes.Find(x => x.Item_Type.Equals(IT.Item_Type)) != null)
                {

                }
                else
                {
                    ItemType ITnew = new ItemType
                    {
                        Item_Type = IT.Item_Type,
                        BoQnr = IT.BoQnr,
                        BoQnr_Rental = IT.BoQnr_Rental,
                        Install_UnitId = IT.Install_UnitId,
                        Rental_UnitId = IT.Rental_UnitId,
                        price = IT.price,
                        rental_price = IT.rental_price,
                        Valuta = IT.Valuta,
                        ProjectId = id,
                        daily_cost = IT.daily_cost,
                        initial_cost = IT.initial_cost
                    };
                    _context.Add(ITnew);
                }
            }
            await _context.SaveChangesAsync();
            foreach (Title t in titles)
            {
                if (existingtitles.Find(x => x.TheTitle.Equals(t.TheTitle)) != null)
                {

                }
                else
                {
                    var theitemtype = await _context.ItemTypes.Where(x => x.ProjectId.Equals(id) && x.Item_Type.Equals(t.ItemType.Item_Type)).SingleOrDefaultAsync();
                    Title t_new = new Title
                    {
                        ItemTypeId = theitemtype.Id,
                        TheTitle = t.TheTitle,
                        ProjectId = id
                    };
                    _context.Add(t_new);
                }
            }
            await _context.SaveChangesAsync();
            foreach (BoQHeadLine boq in BoQheadlines)
            {
                if (existingBoQheadlines.Find(x => x.HeadLine.Equals(boq.HeadLine)) != null)
                {

                }
                else
                {
                    BoQHeadLine newBoQ = new BoQHeadLine
                    {
                        HeadLine = boq.HeadLine,
                        BoQnum = boq.BoQnum,
                        ProjectId = id,
                        Type = boq.Type
                    };
                    _context.Add(newBoQ);
                }

            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PopulateProject2(int id)
        {
            var itemtypes = await _context.ItemTypes.Where(x => x.ProjectId.Equals(40)).Include(x => x.Rental_Unit).ToListAsync();
            var existingitemtypes = await _context.ItemTypes.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var existingtitles = await _context.Titles.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var titles = await _context.Titles.Where(x => x.ProjectId.Equals(40)).ToListAsync();
            var boqheadlines = await _context.BoQHeadLines.Where(x => x.ProjectId.Equals(40)).ToListAsync();
            var existingsboqheadlines = await _context.BoQHeadLines.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            foreach (ItemType IT in itemtypes)
            {
                if (existingitemtypes.Find(x => x.Item_Type.Equals(IT.Item_Type)) != null)
                {

                }
                else
                {
                    ItemType ITnew = new ItemType
                    {
                        Item_Type = IT.Item_Type,
                        BoQnr = IT.BoQnr,
                        BoQnr_Rental = IT.BoQnr_Rental,
                        Install_UnitId = IT.Install_UnitId,
                        Rental_UnitId = IT.Rental_UnitId,
                        price = IT.price,
                        rental_price = IT.rental_price,
                        Valuta = IT.Valuta,
                        ProjectId = id,
                        daily_cost = IT.daily_cost,
                        initial_cost = IT.initial_cost
                    };
                    _context.Add(ITnew);
                }
            }
            await _context.SaveChangesAsync();
            foreach (Title t in titles)
            {
                if (existingtitles.Find(x => x.TheTitle.Equals(t.TheTitle)) != null)
                {

                }
                else
                {
                    var theitemtype = await _context.ItemTypes.Where(x => x.ProjectId.Equals(id) && x.Item_Type.Equals(t.ItemType.Item_Type)).SingleOrDefaultAsync();
                    Title t_new = new Title
                    {
                        ItemTypeId = theitemtype.Id,
                        TheTitle = t.TheTitle,
                        ProjectId = id
                    };
                    _context.Add(t_new);
                }
            }
            foreach(BoQHeadLine bqh in boqheadlines)
            {
                if(existingsboqheadlines.Find(x=>x.BoQnum.Equals(bqh.BoQnum)) != null)
                {

                }
                else
                {
                    BoQHeadLine boqh = new BoQHeadLine
                    {
                        BoQnum = bqh.BoQnum,
                        ProjectId = id,
                        HeadLine = bqh.HeadLine,
                        Type = bqh.Type
                    };
                    _context.Add(boqh);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> ClearProject(int? id)
        {
            var project = await _context.Projects.FindAsync(id);
            
            var mobs = await _context.Mobilisations.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var arrs = await _context.Arrivals.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var ins = await _context.Installations.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var deins = await _context.Deinstallations.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var titles = await _context.Titles.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var dailyreports = await _context.Daily_Report_2s.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var boqheadlines = await _context.BoQHeadLines.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var itemtypes = await _context.ItemTypes.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var extraworks = await _context.ExtraWorks.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var safety = await _context.SafetyProblems.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var summaryreports = await _context.SummaryReports.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var toolboxes = await _context.ToolBoxes.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            foreach (var tb in toolboxes)
            {
                var toolboxusers = await _context.ToolBoxUsers.Where(x => x.ToolBoxId.Equals(tb.Id)).ToListAsync();
                foreach (var tbu in toolboxusers)
                {
                    _context.ToolBoxUsers.Remove(tbu);
                }
                await _context.SaveChangesAsync();
                _context.ToolBoxes.Remove(tb);
                await _context.SaveChangesAsync();
            }
            foreach (var boqh in boqheadlines)
            {
                _context.BoQHeadLines.Remove(boqh);
            }
            await _context.SaveChangesAsync();
            foreach (var sumrep in summaryreports)
            {
                _context.SummaryReports.Remove(sumrep);
            }
            await _context.SaveChangesAsync();
            foreach (var saft in safety)
            {
                _context.SafetyProblems.Remove(saft);
            }
            await _context.SaveChangesAsync();
            foreach (var title in titles)
            {
                _context.Titles.Remove(title);
            }
            await _context.SaveChangesAsync();
            foreach (var dr in dailyreports)
            {
                _context.Daily_Report_2s.Remove(dr);
            }
            await _context.SaveChangesAsync();
            foreach (var ext in extraworks)
            {
                _context.ExtraWorks.Remove(ext);
            }
            await _context.SaveChangesAsync();
            foreach (var mob in mobs)
            {
                var coords = await _context.CoordTrack2s.Where(x => x.MobilizeId.Equals(mob.Id)).ToListAsync();
                foreach (var coord in coords)
                {
                    _context.CoordTrack2s.Remove(coord);
                }
                await _context.SaveChangesAsync();
                _context.Mobilisations.Remove(mob);
            }
            await _context.SaveChangesAsync();
            foreach (var inst in ins)
            {
                var coords = await _context.CoordTrack2s.Where(x => x.InstallId.Equals(inst.Id)).ToListAsync();
                foreach (var coord in coords)
                {
                    _context.CoordTrack2s.Remove(coord);
                }
                var installoperations = await _context.InstallOperations.Where(x => x.InstallId.Equals(inst.Id)).ToListAsync();
                foreach (var insop in installoperations)
                {
                    _context.InstallOperations.Remove(insop);
                }
                await _context.SaveChangesAsync();
                _context.Installations.Remove(inst);
            }
            await _context.SaveChangesAsync();
            foreach (var arr in arrs)
            {
                var coords = await _context.CoordTrack2s.Where(x => x.ArrivalId.Equals(arr.Id)).ToListAsync();
                foreach (var coord in coords)
                {
                    _context.CoordTrack2s.Remove(coord);
                }
                await _context.SaveChangesAsync();
                _context.Arrivals.Remove(arr);
            }
            await _context.SaveChangesAsync();
            foreach (var deinst in deins)
            {
                _context.Deinstallations.Remove(deinst);
            }
            await _context.SaveChangesAsync();
            foreach (var item in itemtypes)
            {
                var logs = await _context.Log2s.Where(x => x.ItemTypeId.Equals(item.Id)).ToListAsync();
                foreach (var logreport in logs)
                {
                    logreport.Description = logreport.Description += " ItemTypeId = " + item.Item_Type;
                    logreport.ItemTypeId = null;
                    _context.Log2s.Update(logreport);
                    await _context.SaveChangesAsync();
                }
                _context.ItemTypes.Remove(item);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user.Active == false)
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You are inactive" });
            }
            var project = await _context.Projects.Include(x=>x.Division).Include(x=>x.Department).Include(x => x.CoordSystem).Where(x=>x.Id.Equals(id)).FirstAsync();
            if (project == null)
            {
                return NotFound();
            }
            ViewData["Title"] = _localizer["Edit"];
            ViewData["CoordSystemId"] = new SelectList(_context.CoordSystems.OrderBy(x => x.system), "Id", "system");
            if (User.IsInRole("Admin"))
            {
                ViewData["DivisionId"] = new SelectList(_context.Divisions.OrderBy(x => x.Name), "Id", "Name");
                ViewData["DepartmentId"] = new SelectList(_context.Departments.OrderBy(x => x.DepartmentName), "Id", "DepartmentName");
            }
            else
            {
                ViewData["DivisionId"] = new SelectList(_context.Divisions.Where(x => x.Id.Equals(user.DivisionId)), "Id", "Name");
                ViewData["DepartmentId"] = new SelectList(_context.Departments.Where(x => x.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.DepartmentName), "Id", "DepartmentName");
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectNr,Name,Abbreviation,DepartmentId,Client,DivisionId,CoordSystemId,Active,Latitude,Longitude,ClientEmail,Responsible_Person,ImplementBreakTime,Address,IsTemplate,Description,StartDate,EndDate,AlertCalls,ClientContactAlert,ClientAlertPhone")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
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
            var user = await _userManager.GetUserAsync(User);
            ViewData["Title"] = _localizer["Edit"];
            ViewData["CoordSystemId"] = new SelectList(_context.CoordSystems.OrderBy(x => x.system), "Id", "system");
            if (User.IsInRole("Admin"))
            {
                ViewData["DivisionId"] = new SelectList(_context.Divisions.OrderBy(x => x.Name), "Id", "Name");
                ViewData["DepartmentId"] = new SelectList(_context.Departments.OrderBy(x => x.DepartmentName), "Id", "DepartmentName");
            }
            else
            {
                ViewData["DivisionId"] = new SelectList(_context.Divisions.Where(x => x.Id.Equals(user.DivisionId)), "Id", "Name");
                ViewData["DepartmentId"] = new SelectList(_context.Departments.Where(x => x.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.DepartmentName), "Id", "DepartmentName");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.Include(x => x.CoordSystem).Include(x=>x.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }
            ViewData["Title"] = _localizer["Delete"];
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            var monitorpoints = await _context.MeasPoints.Where(x => x.ProjectId.Equals(project.Id)).ToListAsync();
            var mobs = await _context.Mobilisations.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var arrs = await _context.Arrivals.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var ins = await _context.Installations.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var deins = await _context.Deinstallations.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var titles = await _context.Titles.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var dailyreports = await _context.Daily_Report_2s.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var boqheadlines = await _context.BoQHeadLines.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var itemtypes = await _context.ItemTypes.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var extraworks = await _context.ExtraWorks.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var safety = await _context.SafetyProblems.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var summaryreports = await _context.SummaryReports.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            var toolboxes = await _context.ToolBoxes.Where(x => x.ProjectId.Equals(id)).ToListAsync();
            
            foreach (MeasPoint mp in monitorpoints)
            {
                _context.MeasPoints.Remove(mp);
                await _context.SaveChangesAsync();
            }    
            foreach(var tb in toolboxes)
            {
                var toolboxusers = await _context.ToolBoxUsers.Where(x => x.ToolBoxId.Equals(tb.Id)).ToListAsync();
                foreach(var tbu in toolboxusers)
                {
                    _context.ToolBoxUsers.Remove(tbu);                    
                }
                await _context.SaveChangesAsync();
                _context.ToolBoxes.Remove(tb);
                await _context.SaveChangesAsync();
            }
            foreach(var boqh in boqheadlines)
            {
                _context.BoQHeadLines.Remove(boqh);
            }
            await _context.SaveChangesAsync();
            foreach(var sumrep in summaryreports)
            {
                _context.SummaryReports.Remove(sumrep);
            }
            await _context.SaveChangesAsync();
            foreach(var saft in safety)
            {
                _context.SafetyProblems.Remove(saft);
            }
            await _context.SaveChangesAsync();
            foreach(var title in titles)
            {
                _context.Titles.Remove(title);                
            }
            await _context.SaveChangesAsync();
            foreach(var dr in dailyreports)
            {
                _context.Daily_Report_2s.Remove(dr);
            }
            await _context.SaveChangesAsync();
            foreach(var ext in extraworks)
            {
                _context.ExtraWorks.Remove(ext);
            }
            await _context.SaveChangesAsync();
            foreach(var mob in mobs)
            {
                var coords = await _context.CoordTrack2s.Where(x => x.MobilizeId.Equals(mob.Id)).ToListAsync();
                foreach(var coord in coords)
                {
                    _context.CoordTrack2s.Remove(coord);
                }
                await _context.SaveChangesAsync();
                _context.Mobilisations.Remove(mob);
            }
            await _context.SaveChangesAsync();
            foreach (var inst in ins)
            {
                var coords = await _context.CoordTrack2s.Where(x => x.InstallId.Equals(inst.Id)).ToListAsync();
                foreach (var coord in coords)
                {
                    _context.CoordTrack2s.Remove(coord);
                }
                var installoperations = await _context.InstallOperations.Where(x => x.InstallId.Equals(inst.Id)).ToListAsync();
                foreach(var insop in installoperations)
                {
                    _context.InstallOperations.Remove(insop);
                }
                await _context.SaveChangesAsync();
                _context.Installations.Remove(inst);
            }
            await _context.SaveChangesAsync();
            foreach (var arr in arrs)
            {
                var coords = await _context.CoordTrack2s.Where(x => x.ArrivalId.Equals(arr.Id)).ToListAsync();
                foreach (var coord in coords)
                {
                    _context.CoordTrack2s.Remove(coord);
                }
                await _context.SaveChangesAsync();
                _context.Arrivals.Remove(arr);
            }
            await _context.SaveChangesAsync();
            foreach (var deinst in deins)
            {
                _context.Deinstallations.Remove(deinst);
            }
            foreach(var item in itemtypes)
            {
                var logs = await _context.Log2s.Where(x => x.ItemTypeId.Equals(item.Id)).ToListAsync();
                foreach(var logreport in logs)
                {
                    logreport.Description = logreport.Description += " ItemTypeId = " + item.Item_Type;
                    logreport.ItemTypeId = null;
                    _context.Log2s.Update(logreport);
                    await _context.SaveChangesAsync();
                }
                _context.ItemTypes.Remove(item);
            }
            await _context.SaveChangesAsync();
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,DivisionAdmin")]
        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
        [AllowAnonymous]
        public JsonResult GetMeasPointsProject(string theId)
        {
            int Id = Convert.ToInt32(theId);
            List<MeasPoint> thedata = new List<MeasPoint>();               
            thedata = _context.MeasPoints.Include(x => x.MeasType).Where(x => x.ProjectId.Equals(Id) && x.ToBeHidden.Equals(false) && !x.MeasType.Type.ToLower().Equals("miscellaneous")).OrderBy(x => x.Name).ToList();
            return Json(thedata);
        }
        [AllowAnonymous]
        public JsonResult GetWellsProject(string theId)
        {
            int Id = Convert.ToInt32(theId);
            List<MeasPoint> thedata = new List<MeasPoint>();
            thedata = _context.MeasPoints.Include(x => x.MeasType).Where(x => x.ProjectId.Equals(Id) && x.ToBeHidden.Equals(false) && x.MeasType.Type.ToLower().Equals("water level")).OrderBy(x => x.Name).ToList();
            return Json(thedata);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GetProjectInRadius(double radi, double lati, double longi)
        {
            var user = await _userManager.GetUserAsync(User);
            List<Project> projects = new List<Project>();
            LatLngUTMConverter ltUTMconv = new LatLngUTMConverter("WGS 84");
            List<Project> projectP = new List<Project>();
            //join dok1 in _context.Generator_Test on p.Id equals dok1.ProjectId
            //join dok2 in _context.WTP_Tests on p.Id equals dok2.ProjectId
            //join dok3 in _context.Well_Installations on p.Id equals dok3.ProjectId
            //join dok4 in _context.Well_Developments on p.Id equals dok4.ProjectId
            if (User.IsInRole("Guest"))
            {
                projectP = await (from p in _context.Projects.Include(x => x.Documents).ThenInclude(x => x.DocumentType)
                                  .Include(x=>x.Generator_TestsDocuments)
                                  .Include(x=>x.WTP_TestsDocuments)
                                  .Include(x=>x.WellInstallationsDocuments)
                                  .Include(x=>x.WellDevelopmentsDocuments)
                                  .Include(x => x.CoordSystem)
                                  .Include(x => x.Department)
                                  join pu in _context.ProjectUsers on p.Id
                               equals pu.projectId
                               join doc in _context.Documents on p.Id equals doc.ProjectId
                               where pu.userId == user.Id && p.Active.Equals(true) && p.DivisionId.Equals(user.DivisionId)
                                  select p).GroupBy(x => x.Id).Select(x => x.First()).OrderBy(x => x.Name)
                                            .ToListAsync();
            }
            else if (User.IsInRole("Admin"))
            {
                projectP = await (from p in _context.Projects
                               .Include(x => x.Documents).ThenInclude(x => x.DocumentType)
                               .Include(x => x.Generator_TestsDocuments)
                                  .Include(x => x.WTP_TestsDocuments)
                                  .Include(x => x.WellInstallationsDocuments)
                                  .Include(x => x.WellDevelopmentsDocuments)
                                  .Include(x => x.Department)
                               .Include(x => x.CoordSystem)
                                  join doc in _context.Documents on p.Id equals doc.ProjectId
                                  select p).GroupBy(x => x.Id).Select(x => x.First()).OrderBy(x => x.Name)
                                            .ToListAsync();
            }
            else
            {
                projectP = await (from p in _context.Projects
                               .Include(x => x.Documents).ThenInclude(x => x.DocumentType)
                               .Include(x => x.Generator_TestsDocuments)
                                  .Include(x => x.WTP_TestsDocuments)
                                  .Include(x => x.WellInstallationsDocuments)
                                  .Include(x => x.WellDevelopmentsDocuments)
                                  .Include(x => x.Department)
                               .Include(x => x.CoordSystem)
                                  join doc in _context.Documents on p.Id equals doc.ProjectId
                                  where p.DivisionId.Equals(user.DivisionId)
                                  select p).GroupBy(x => x.Id).Select(x => x.First()).OrderBy(x => x.Name)
                                            .ToListAsync();
            }
            
            foreach (Project p in projectP)
            {
                
                if (p.Latitude != null && p.Longitude != null)
                {
                    var distance = DistanceAlgorithm.DistanceBetweenPlaces(longi, lati, Convert.ToDouble(p.Longitude), Convert.ToDouble(p.Latitude));
                    if (distance < radi)
                    {
                        projects.Add(p);
                    }
                }

            }

            return PartialView("_ProjectDocuments", projects);
        }
        
    }
   
}
