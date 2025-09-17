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
using System.Text;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Http;
using System.IO;
using MainOps.Resources;
using Microsoft.AspNetCore.Hosting;
using MainOps.ExtensionMethods;
using Renci.SshNet;
using MainOps.Services;
using Microsoft.Extensions.Options;
using Renci.SshNet.Common;
using System.Net;
using System.Diagnostics;
using Rotativa.AspNetCore;

namespace MainOps.Controllers
{
    [Authorize]
    public class MeasController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LocService _localizer;
        private readonly IWebHostEnvironment _env;
        private AuthSFTPOptions _Options { get; }
        public MeasController(DataContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env, LocService localizer, IOptions<AuthSFTPOptions> optionsAccessor) :base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
            _localizer = localizer;
            _env = env;
            _Options = optionsAccessor.Value;
        }

        
        public async Task<IEnumerable<SelectListItem>> createFilterlist()
        {
            var theuser = await _userManager.GetUserAsync(HttpContext.User);
            if (User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
            {
                var filternames = await (from p in _context.Projects
                                         join pu in _context.ProjectUsers on p.Id equals pu.projectId
                                         where pu.userId == theuser.Id
                                         select p).OrderBy(b => b.Name).ToListAsync();
                IEnumerable<SelectListItem> selList2 = from s in filternames
                                                      select new SelectListItem
                                                      {
                                                          Value = s.Id.ToString(),
                                                          Text = s.Name
                                                      };
                return selList2;
            }
            if (User.IsInRole("Admin"))
            {
                var filternames2 = await _context.Projects.Include(x => x.Division).OrderBy(x => x.DivisionId).ThenBy(b => b.Name).ToListAsync();

                IEnumerable<SelectListItem> selList = from s in filternames2
                                                      select new SelectListItem
                                                      {
                                                          Value = s.Id.ToString(),
                                                          Text = s.Name
                                                      };
                return selList;
            }
            else
            {
                var filternames2 = await _context.Projects.Include(x => x.Division).Where(x => x.Division.Id.Equals(theuser.DivisionId)).OrderBy(b => b.Name).ToListAsync();

                IEnumerable<SelectListItem> selList = from s in filternames2
                                                      select new SelectListItem
                                                      {
                                                          Value = s.Id.ToString(),
                                                          Text = s.Name
                                                      };
                return selList;
            }
            
        }
        // GET: Meas
        [Authorize(Roles = ("Admin,DivisionAdmin,Member,Manager,Guest,MemberGuest"))]
        public async Task<IActionResult> Index()
        {
            IEnumerable<SelectListItem> selList = await createFilterlist();
            ViewData["Filterchoices"] = new SelectList(selList, "Value", "Text");
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
            {
                var theuser = await _userManager.GetUserAsync(User);
                var measList = _context.Measures.Include(m => m.MeasPoint).ThenInclude(x => x.Project)
                                  .Include(x => x.TheComment)
                                  .Include(x => x.MeasPoint).ThenInclude(x => x.MeasType)
                                  .OrderByDescending(x => x.When);
                var data = await (from m in measList
                                  join pu in _context.ProjectUsers on m.MeasPoint.ProjectId
                                  equals pu.projectId
                                  where pu.userId == theuser.Id && m.MeasPoint.Project.Active.Equals(true)
                                  select m).OrderByDescending(x=>x.When).Take(100).ToListAsync();
                return View(data);

            }
            if (User.IsInRole("Admin"))
            {
                var dataContextAdmin = _context.Measures
                        .Include(m => m.MeasPoint).ThenInclude(x => x.Project).ThenInclude(x => x.Division)
                        .Include(x => x.MeasPoint).ThenInclude(x => x.MeasType)
                        .Include(m => m.TheComment).Where(x=>x.MeasPoint.Project.Active.Equals(true))
                        .OrderByDescending(x => x.When).Take(100);

                return View(await dataContextAdmin.ToListAsync());
            }
            var dataContext = _context.Measures
                .Include(m => m.MeasPoint).ThenInclude(x => x.Project).ThenInclude(x=>x.Division)
                .Include(m => m.TheComment)
                .Include(x => x.MeasPoint).ThenInclude(x => x.MeasType)
                .Where(x=>x.MeasPoint.Project.Division.Id.Equals(user.DivisionId) && x.MeasPoint.Project.Active.Equals(true))
                .OrderByDescending(x => x.When).Take(100);
            return View(await dataContext.ToListAsync());
        }
        [HttpPost]
        [Authorize(Roles = ("Admin,DivisionAdmin,Member,Manager,Guest,MemberGuest"))]
        public async Task<IActionResult> Combsearch_origin(string searchstring, string filterchoice, DateTime? startdate, DateTime? enddate)
        {
            int f_c_converted;
            f_c_converted = Convert.ToInt32(filterchoice);

            IEnumerable<SelectListItem> selList = await createFilterlist();
            ViewData["Filterchoices"] = new SelectList(selList, "Value", "Text");
            if (ModelState.IsValid)
            {
                if (startdate == null)
                {
                    startdate = new DateTime(1990, 1, 1);
                }
                if (enddate == null)
                {
                    enddate = new DateTime(2100, 1, 1);
                }
                var theuser = await _userManager.GetUserAsync(User);
                if (HttpContext.User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
                {
                    if (string.IsNullOrEmpty(searchstring) && (string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
                    {
                        var data = await (from m in _context.Measures.Include(m => m.MeasPoint).ThenInclude(x => x.Project)
                                          .Include(x=>x.MeasPoint).ThenInclude(x=>x.MeasType)
                                  .Include(x => x.TheComment).OrderByDescending(x => x.When).Take(100)
                                          join pu in _context.ProjectUsers on m.MeasPoint.ProjectId
                                                            equals pu.projectId
                                          where pu.userId == theuser.Id && m.MeasPoint.Project.Active.Equals(true)
                                          && (m.When >= startdate && m.When <= Convert.ToDateTime(enddate).AddDays(1))
                                          select m).OrderByDescending(x => x.When).ThenBy(x=>x.MeasPoint.Name).ToListAsync();
                        return View(nameof(Index),data);
                    }
                    else if (string.IsNullOrEmpty(searchstring) && (!string.IsNullOrEmpty(filterchoice) || !filterchoice.Equals("All")))
                    {
                        var data = await (from m in _context.Measures.Include(m => m.MeasPoint).ThenInclude(x => x.Project).ThenInclude(x=>x.Division)
                                 .Include(x => x.TheComment).OrderByDescending(x => x.When)
                                 .Include(x => x.MeasPoint).ThenInclude(x => x.MeasType)
                                          join pu in _context.ProjectUsers on m.MeasPoint.ProjectId
                                          equals pu.projectId
                                          where pu.userId == theuser.Id && m.MeasPoint.ProjectId.Equals(f_c_converted) && theuser.DivisionId.Equals(m.MeasPoint.Project.Division.Id)
                                          && (m.When >= startdate && m.When <= Convert.ToDateTime(enddate).AddDays(1))
                                          select m).OrderByDescending(x => x.When).ThenBy(x => x.MeasPoint.Name).ToListAsync();
                        return View(nameof(Index), data);
                    }
                    else if (!string.IsNullOrEmpty(searchstring) && (string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
                    {
                        var data = await (from m in _context.Measures.Include(m => m.MeasPoint).ThenInclude(x => x.Project).ThenInclude(x=>x.Division)
                                 .Include(x => x.TheComment)
                                 .Include(x => x.MeasPoint).ThenInclude(x => x.MeasType).OrderByDescending(x => x.When)
                                          join pu in _context.ProjectUsers on m.MeasPoint.ProjectId
                                          equals pu.projectId
                                          where pu.userId == theuser.Id && theuser.DivisionId.Equals(m.MeasPoint.Project.Division.Id) && m.MeasPoint.Name.ToLower().Contains(searchstring.ToLower())
                                          && (m.When >= startdate && m.When <= Convert.ToDateTime(enddate).AddDays(1))
                                          select m).OrderByDescending(x => x.When).ThenBy(x => x.MeasPoint.Name).ToListAsync();
                        return View(nameof(Index), data);
                    }
                    else
                    {
                        var data = await (from m in _context.Measures.Include(m => m.MeasPoint).ThenInclude(x => x.Project).ThenInclude(x=>x.Division)
                                 .Include(x => x.TheComment)
                                 .Include(x => x.MeasPoint).ThenInclude(x => x.MeasType).OrderByDescending(x => x.When)
                                          join pu in _context.ProjectUsers on m.MeasPoint.ProjectId
                                          equals pu.projectId
                                          where pu.userId == theuser.Id && theuser.DivisionId.Equals(m.MeasPoint.Project.Division.Id) && m.MeasPoint.Name.ToLower().Contains(searchstring.ToLower()) && m.MeasPoint.ProjectId.Equals(f_c_converted)
                                          && (m.When >= startdate && m.When <= Convert.ToDateTime(enddate).AddDays(1))
                                          select m).OrderByDescending(x => x.When).ThenBy(x => x.MeasPoint.Name).ToListAsync();
                        return View(nameof(Index), data);
                    }


                }
                if (string.IsNullOrEmpty(searchstring) && (string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
                {
                    if (User.IsInRole("Admin"))
                    {
                        var dataC = await _context.Measures
                        .Include(m => m.MeasPoint).ThenInclude(x => x.Project).ThenInclude(x => x.Division)
                        .Include(x => x.MeasPoint).ThenInclude(x => x.MeasType)
                        .Include(m => m.TheComment).Where(x => x.MeasPoint.Project.Active.Equals(true) && x.When >= startdate && x.When <= Convert.ToDateTime(enddate).AddDays(1))
                        .OrderByDescending(x => x.When).ThenBy(x => x.MeasPoint.Name).Take(100).ToListAsync();
                        return View(nameof(Index), dataC);
                    }
                    var data = await _context.Measures
                        .Include(m => m.MeasPoint).ThenInclude(x => x.Project).ThenInclude(x => x.Division)
                        .Include(m => m.TheComment).Include(x => x.MeasPoint).ThenInclude(x => x.MeasType)
                        .Where(x => x.MeasPoint.Project.Division.Id.Equals(theuser.DivisionId) && x.MeasPoint.Project.Active.Equals(true) && x.When >= startdate && x.When <= Convert.ToDateTime(enddate).AddDays(1))
                        .OrderByDescending(x => x.When).ThenBy(x => x.MeasPoint.Name).Take(100).ToListAsync();
                    return View(nameof(Index), data);
                }
                else if (string.IsNullOrEmpty(searchstring) && (!string.IsNullOrEmpty(filterchoice) || !filterchoice.Equals("All")))
                {
                    if (User.IsInRole("Admin"))
                    {
                        var data = await _context.Measures
                        .Include(x => x.MeasPoint).ThenInclude(x => x.Project).ThenInclude(x => x.Division).Include(m => m.TheComment)
                        .Include(x => x.MeasPoint).ThenInclude(x => x.MeasType)
                        .Where(b => b.MeasPoint.ProjectId.Equals(f_c_converted) && (b.When >= startdate && b.When <= Convert.ToDateTime(enddate).AddDays(1)))
                        .OrderByDescending(x => x.When).ThenBy(x => x.MeasPoint.Name).ToListAsync();
                        return View(nameof(Index), data);
                    }
                    else
                    {
                        var data = await _context.Measures
                        .Include(x => x.MeasPoint).ThenInclude(x => x.Project).ThenInclude(x => x.Division).Include(m => m.TheComment)
                        .Include(x => x.MeasPoint).ThenInclude(x => x.MeasType)
                        .Where(b => b.MeasPoint.ProjectId.Equals(f_c_converted) && b.MeasPoint.Project.Division.Id.Equals(theuser.DivisionId) && (b.When >= startdate && b.When <= Convert.ToDateTime(enddate).AddDays(1)))
                        .OrderByDescending(x => x.When).ThenBy(x => x.MeasPoint.Name).ToListAsync();
                        return View(nameof(Index), data);
                    }
                    
                }
                else if (!string.IsNullOrEmpty(searchstring) && (string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
                {
                    if (User.IsInRole("Admin"))
                    {
                        var wtp_blocks = await _context.Measures
                        .Include(x => x.MeasPoint).ThenInclude(x => x.Project).ThenInclude(x => x.Division).Include(m => m.TheComment)
                        .Include(x => x.MeasPoint).ThenInclude(x => x.MeasType)
                        .Where(b => b.MeasPoint.Name.ToLower().Contains(searchstring.ToLower()) && (b.When >= startdate && b.When <= Convert.ToDateTime(enddate).AddDays(1)))
                        .OrderByDescending(x => x.When).ThenBy(x => x.MeasPoint.Name).ToListAsync();
                        return View(nameof(Index), wtp_blocks);
                    }
                    else
                    {
                        var wtp_blocks = await _context.Measures
                        .Include(x => x.MeasPoint).ThenInclude(x => x.Project).ThenInclude(x => x.Division).Include(m => m.TheComment)
                        .Include(x => x.MeasPoint).ThenInclude(x => x.MeasType)
                        .Where(b => b.MeasPoint.Name.ToLower().Contains(searchstring.ToLower()) && b.MeasPoint.Project.Division.Id.Equals(theuser.DivisionId) && (b.When >= startdate && b.When <= Convert.ToDateTime(enddate).AddDays(1)))
                        .OrderByDescending(x => x.When).ThenBy(x => x.MeasPoint.Name).ToListAsync();
                        return View(nameof(Index), wtp_blocks);
                    }
                    
                }
                else
                {
                    if (User.IsInRole("Admin"))
                    {
                        var wtp_blocks = await _context.Measures
                        .Include(x => x.MeasPoint).ThenInclude(x => x.Project).ThenInclude(x => x.Division).Include(m => m.TheComment)
                        .Include(x => x.MeasPoint).ThenInclude(x => x.MeasType)
                        .Where(b => b.MeasPoint.ProjectId.Equals(f_c_converted) && b.MeasPoint.Name.ToLower().Contains(searchstring.ToLower()) && (b.When >= startdate && b.When <= Convert.ToDateTime(enddate).AddDays(1)))
                        .OrderByDescending(x => x.When).ThenBy(x => x.MeasPoint.Name).ToListAsync();
                        return View(nameof(Index), wtp_blocks);
                    }
                    else
                    {
                        var wtp_blocks = await _context.Measures
                        .Include(x => x.MeasPoint).ThenInclude(x => x.Project).ThenInclude(x => x.Division).Include(m => m.TheComment)
                        .Include(x => x.MeasPoint).ThenInclude(x => x.MeasType)
                        .Where(b => b.MeasPoint.ProjectId.Equals(f_c_converted) && b.MeasPoint.Project.Division.Id.Equals(theuser.DivisionId) && b.MeasPoint.Name.ToLower().Contains(searchstring.ToLower()) && (b.When >= startdate && b.When <= Convert.ToDateTime(enddate).AddDays(1)))
                        .OrderByDescending(x => x.When).ThenBy(x => x.MeasPoint.Name).ToListAsync();
                        return View(nameof(Index), wtp_blocks);
                    }
                    
                }

            }

            return View(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,DivisionAdmin,Member,Manager,Guest,MemberGuest")]
        public async Task<IActionResult> Combsearch(string searchstring,
                                                    string filterchoice,
                                                    DateTime? startdate,
                                                    DateTime? enddate,
                                                    int page = 1,
                                                    int pageSize = 100)
        {
            int f_c_converted;
            int.TryParse(filterchoice, out f_c_converted);

            IEnumerable<SelectListItem> selList = await createFilterlist();
            ViewData["Filterchoices"] = new SelectList(selList, "Value", "Text");

            // Default date range
            startdate = startdate ?? new DateTime(1990, 1, 1);
            enddate = (enddate ?? new DateTime(2100, 1, 1)).AddDays(1);

            var theuser = await _userManager.GetUserAsync(User);

            // Base query
            var query = _context.Measures
                .Include(m => m.MeasPoint).ThenInclude(mp => mp.Project).ThenInclude(p => p.Division)
                .Include(m => m.MeasPoint).ThenInclude(mp => mp.MeasType)
                .Include(m => m.TheComment)
                .Where(m => m.When >= startdate && m.When <= enddate);

            // Role-based restrictions
            if (User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
            {
                query = from m in query
                        join pu in _context.ProjectUsers on m.MeasPoint.ProjectId equals pu.projectId
                        where pu.userId == theuser.Id && m.MeasPoint.Project.Active
                        select m;
            }
            else if (!User.IsInRole("Admin"))
            {
                query = query.Where(m => m.MeasPoint.Project.Division.Id == theuser.DivisionId && m.MeasPoint.Project.Active);
            }

            // Filter by project (if chosen)
            if (!string.IsNullOrEmpty(filterchoice) && filterchoice != "All")
            {
                query = query.Where(m => m.MeasPoint.ProjectId == f_c_converted);
            }

            // Search by name (case-insensitive)
            if (!string.IsNullOrEmpty(searchstring))
            {
                query = query.Where(m => EF.Functions.Like(m.MeasPoint.Name, $"%{searchstring}%"));
            }

            // Order + pagination
            var data = await query
                .OrderByDescending(m => m.When)
                .ThenBy(m => m.MeasPoint.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Repopulate filter dropdown
            //IEnumerable<SelectListItem> selList = await createFilterlist();
            //ViewData["Filterchoices"] = new SelectList(selList, "Value", "Text");

            ViewData["CurrentPage"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["HasMore"] = data.Count == pageSize; // true if there might be more results

            ViewData["SearchString"] = searchstring;
            ViewData["FilterChoice"] = filterchoice;
            ViewData["StartDate"] = startdate;
            ViewData["EndDate"] = enddate;

            Debug.WriteLine($"FilterChoice: {filterchoice}");

            return View(nameof(Index), data);
        }
        [HttpGet]
        [Authorize(Roles = ("Admin,DivisionAdmin,Member,Manager,Guest,MemberGuest"))]
        public async Task<JsonResult> AutoComplete(string search)
        {
            //WTP_block[] matching = string.IsNullOrWhiteSpace(search) ?
            //             await _context.WTP_blocks.ToArrayAsync() :
            //          await _context.WTP_blocks.Where(p => p.name.ToUpper().Contains(search.ToUpper())).ToArrayAsync();
            var theuser = await _userManager.GetUserAsync(HttpContext.User);
            if (HttpContext.User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
            {
                
                var results = await (from m in _context.Measures
                                     .Include(x => x.MeasPoint).ThenInclude(x => x.Project).ThenInclude(x=>x.Division)
                                join pu in _context.ProjectUsers on m.MeasPoint.ProjectId
                                equals pu.projectId
                                where pu.userId == theuser.Id && m.MeasPoint.Project.Division.Id.Equals(theuser.DivisionId) && (m.MeasPoint.Name.ToLower().Contains(search.ToLower()) || m.MeasPoint.Project.Name.ToLower().Contains(search.ToLower()))
                                select m).ToListAsync();
                return Json(results.Select(m => new
                {
                    id = m.Id,
                    value = m.MeasPoint.Name,
                    label = m.MeasPoint.Project.Name + '_' + m.MeasPoint.Name
                }).OrderBy(x => x.label));
            }
            else
            {
                var results = _context.Measures
                    .Include(x => x.MeasPoint).ThenInclude(x => x.Project).ThenInclude(x=>x.Division)
                    .Where(x => x.MeasPoint.Name.ToLower().Contains(search.ToLower()) && x.MeasPoint.Project.Division.Id.Equals(theuser.DivisionId)).ToList();
                return Json(results.Select(m => new
                {
                    id = m.Id,
                    value = m.MeasPoint.Name,
                    label = m.MeasPoint.Project.Name + '_' + m.MeasPoint.Name
                }).OrderBy(x => x.label));
            }
            
        }

        [HttpGet]
        [Authorize(Roles = ("Admin,DivisionAdmin,Member,MemberGuest"))]
        public async Task<IActionResult> Meas_PDF(int? id)
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
            
            var meas = await _context.Measures
                .Include(m => m.MeasPoint).ThenInclude(m => m.Project).ThenInclude(m => m.Division)
                .Include(m => m.MeasPhotos)
                .Include(m => m.TheComment)
                .FirstOrDefaultAsync(m => m.Id == id);
           

            if (meas == null)
            {
                return NotFound(); 
            }

            int? mpId = meas.MeasPointId;
            var mp = meas.MeasPoint;

            var measguest = await _context.Measures
                            .Include(x => x.MeasPoint).ThenInclude(x => x.MeasType)
                            .Where(x => x.When.Equals(meas.When) && (x.MeasPoint.Name.Contains(mp.Name)))
                            .ToListAsync();

            MeasMoreVM model = new MeasMoreVM();
            model.ProjectId = Convert.ToInt32(mp.ProjectId);
            model.BaseName = mp.getBaseName;
            model.Project = mp.Project;
            model.TheComment = meas.TheComment;
            model.When = meas.When;
            model.DoneBy = meas.DoneBy;
            
            foreach (Meas mg in measguest)
            {
                var measp = mg.MeasPoint;
                if (measp.MeasType.Type.ToLower().Equals("water level"))
                {                    
                    model.MeasPointNameLevelIds.Add(mg.MeasPoint.Name);                 
                    model.MeasPointLevelComment.Add(mg.NewComment);                    
                    model.TheMeasurement = mg.TheMeasurement;
                    model.MeasPoint = mg.MeasPoint;
                }
                else if (measp.MeasType.Type.ToLower().Equals("flow rate"))
                {
                    model.MeasPointNameFlowIds.Add(mg.MeasPoint.Name);
                    model.MeasPointFlowComment.Add(mg.NewComment);
                    model.TheFlowMeasurement = mg.TheMeasurement;
                }
                else if (measp.MeasType.Type.ToLower().Equals("water meter"))// || mp.MeasType.Type.ToLower().Equals("water meter *10") || mp.MeasType.Type.ToLower().Equals("water meter *100"))
                {
                    //model.MeasPointWMIds.Add(mp.Id);
                    //model.MeasPointWMMeasures.Add(null);
                    model.MeasPointNameWMIds.Add(mg.MeasPoint.Name);
                    model.MeasPointWMComment.Add(mg.NewComment);
                    model.TheWMMeasurement = mg.TheMeasurement;
                }
            }

            return new ViewAsPdf("_AllDetails", model);
        }

        // GET: Meas/Details/5

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meas = await _context.Measures
                .Include(m => m.MeasPoint).Include(m => m.TheComment)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (meas == null)
            {
                return NotFound();
            }
            return View(meas);
        }

        // GET: Meas/Create
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
                    monpoints = await _context.MeasPoints.Where(x=>x.ToBeHidden.Equals(false)).Include(x => x.Project).ThenInclude(x => x.Division)
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
        [HttpPost]
        public async Task<IActionResult> CreateDip(int theid,double? themeasurement,DateTime thetime,string thecom,double thelat,double thelong)
        {
            if(themeasurement != null || !(thecom == "" || thecom == null)) { 
                var theuser = await _userManager.GetUserAsync(User);
                Meas theMeas = new Meas();
                theMeas.DoneBy = theuser.UserName;
                theMeas.TheMeasurement = themeasurement;
                theMeas.When = thetime;
                 if (!thetime.IsDaylightSavingTime()) { 
                    theMeas.When = thetime.AddHours(1);
                 }
                theMeas.MeasPointId = theid;
                theMeas.NewComment = thecom;
                theMeas.Latitude = thelat;
                theMeas.Longitude = thelong;            
                _context.Measures.Add(theMeas);
                var mp = await _context.MeasPoints.FindAsync(theid);
                var boqitem = await _context.ItemTypes.SingleOrDefaultAsync(x => x.ProjectId.Equals(mp.ProjectId) && x.ReportTypeId.Equals(9));
                if(boqitem != null)
                {
                    Install inst = new Install
                    {
                        ItemTypeId = boqitem.Id,
                        Amount = 1,
                        TimeStamp = thetime,
                        RentalStartDate = thetime,
                        DoneBy = theuser.full_name(),
                        EnteredIntoDataBase = DateTime.Now,
                        Latitude = thelat,
                        Longitude = thelong,
                        Install_Text = String.Concat("Dip on: ",mp.Name," ",thecom),
                        ProjectId = mp.ProjectId,
                        SubProjectId = mp.SubProjectId,
                        isInstalled = false,
                        Location = FindNearestKM(Convert.ToInt32(mp.ProjectId), thelat, thelong),
                        IsInOperation = false,

                    };
                    _context.Installations.Add(inst);
                    await _context.SaveChangesAsync();
                    var lastinstall = await _context.Installations.LastAsync();
                    DeInstall deinst = new DeInstall
                    {
                        InstallId = lastinstall.Id,
                        TimeStamp = thetime.AddSeconds(1),
                        ItemTypeId = boqitem.Id,
                        DeInstall_Text = "handdip",
                        Amount = 1,
                        Latitude = thelat,
                        Longitude = thelong,
                        ProjectId = mp.ProjectId,
                        SubProjectId = mp.SubProjectId,
                        EnteredIntoDataBase = DateTime.Now
                    };
                   _context.Deinstallations.Add(deinst);
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ExportDipList", "MeasPoints");
        }
 
        [HttpPost]
        public async Task<IActionResult> CreateDip2(int theid, double themeasurement, DateTime thetime,string thecom)
        {
            var theuser = await _userManager.GetUserAsync(User);
            Meas theMeas = new Meas();
            theMeas.DoneBy = theuser.UserName;
            theMeas.TheMeasurement = themeasurement;
            try
            {
                //TimeZoneInfo cetZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                //thetime = TimeZoneInfo.ConvertTimeFromUtc(thetime, cetZone);
                if (!thetime.IsDaylightSavingTime())
                {
                    theMeas.When = thetime.AddHours(1);
                }
                else
                {
                    theMeas.When = thetime;
                }
                
            }
            catch
            {
                theMeas.When = thetime;
            }

            theMeas.MeasPointId = theid;
            theMeas.Latitude = 0.0;
            theMeas.Longitude = 0.0;
            theMeas.NewComment = thecom;
            _context.Measures.Add(theMeas);
            var mp = await _context.MeasPoints.FindAsync(theid);
            var boqitem = await _context.ItemTypes.SingleOrDefaultAsync(x =>x.ProjectId.Equals(mp.ProjectId) && x.ReportTypeId.Equals(9));
            if (boqitem != null)
            {               
                Install inst = new Install
                {
                    ItemTypeId = boqitem.Id,
                    Amount = 1,
                    TimeStamp = thetime,
                    RentalStartDate = thetime,
                    DoneBy = theuser.full_name(),
                    EnteredIntoDataBase = DateTime.Now,
                    Latitude = 0,
                    Longitude = 0,
                    Install_Text = String.Concat("Dip on: ", mp.Name, " ", thecom),
                    ProjectId = mp.ProjectId,
                    SubProjectId = mp.SubProjectId,
                    isInstalled = false,
                    Location = FindNearestKM(Convert.ToInt32(mp.ProjectId), 0, 0),
                    IsInOperation = false,

                };
                _context.Installations.Add(inst);
                await _context.SaveChangesAsync();
                var lastinstall = await _context.Installations.LastAsync();
                DeInstall deinst = new DeInstall
                {
                    InstallId = lastinstall.Id,
                    TimeStamp = thetime.AddSeconds(1),
                    ItemTypeId = boqitem.Id,
                    DeInstall_Text = "handdip",
                    Amount = 1,
                    Latitude = 0,
                    Longitude = 0,
                    ProjectId = mp.ProjectId,
                    SubProjectId = mp.SubProjectId,
                    EnteredIntoDataBase = DateTime.Now
                };
                _context.Deinstallations.Add(deinst);
            }
            return RedirectToAction("ExportDipList", "MeasPoints");
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
        [Authorize(Roles = "Admin,DivisionAdmin")]
        [HttpGet]
        public async Task<IActionResult> UploadDips()
        {
            List<SelectListItem> selList = await createMeasPointList();
            ViewData["MeasPointId"] = selList;
            return View("UploadView");
        }
        [Authorize(Roles = "Admin,DivisionAdmin")]
        [HttpPost]
        public async Task<ActionResult> UploadDips(UploadDips model,IFormFile postedFile)
        {
            if (postedFile != null)
            {
                try
                {
                    string fileExtension = Path.GetExtension(postedFile.FileName);

                    //Validate uploaded file and return error.
                    if (fileExtension.ToLower() != ".csv")
                    {
                        return View("Index");
                    }

                    int mpId = model.MeasPointId;
                    using (var sreader = new StreamReader(postedFile.OpenReadStream()))
                    {
                        while (!sreader.EndOfStream)
                        {
                            try
                            {
                                string[] rows = sreader.ReadLine().Split(';');
                                if(rows.Length < 2)
                                {
                                    rows = sreader.ReadLine().Split(',');
                                }
                                if (rows[1] != "")
                                {

                                    Meas Measure = new Meas
                                    {
                                        MeasPointId = mpId,
                                        TheMeasurement = Convert.ToDouble(rows[1]),
                                        When = Convert.ToDateTime(rows[0]),
                                        Latitude = 0,
                                        Longitude = 0
                                    };
                                    var prevMeas = await _context.Measures.Where(x => x.MeasPointId.Equals(Measure.MeasPointId)
                                    && x.When >= Measure.When.AddMinutes(-1)
                                    && x.When <= Measure.When.AddMinutes(1)).ToListAsync();
                                    if ( prevMeas != null)
                                    {
                                        if(prevMeas.Count > 0)
                                        {

                                        }
                                        else
                                        {
                                            _context.Measures.Add(Measure);
                                        }
                                        //dont add measurement again.
                                    }
                                    else
                                    {
                                        _context.Measures.Add(Measure);
                                    }
                                   
                                }

                            }
                            catch
                            {

                            }
                        }
                        await _context.SaveChangesAsync();

                    }
                    List<SelectListItem> selList = await createMeasPointList();
                    ViewData["MeasPointId"] = selList;
                    return View("UploadView");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
            }
            else
            {
                ViewBag.Message = "Please select the file first to upload.";
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> exportData()
        {
            ViewData["ProjectId"] = await GetProjectList();
            List<SelectListItem> delimiters = new List<SelectListItem>();
            delimiters.Add(new SelectListItem() { Text = ";", Value = ";" });
            delimiters.Add(new SelectListItem() { Text = ",", Value = "," });
            ViewData["delimiters"] = new SelectList(delimiters, "Value", "Text");
            ViewData["Title"] = _localizer.GetLocalizedHtmlString("Export Data");
            return View();
        }
        [HttpPost]
        [Route("/Meas/exportData/data.csv")]
        [Produces("text/csv")]
        public async Task<IActionResult> exportData(ExportDataViewModel model)
        {

            StringBuilder sb = new StringBuilder();
            List<Meas> data = new List<Meas>();
            if (model.projectNumber != null)
            {
                if (User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
                {
                    var theuser = await _userManager.GetUserAsync(HttpContext.User);
                    data = await (from m in _context.Measures.Include(x => x.MeasPoint).ThenInclude(x=>x.MeasType).Include(x => x.MeasPoint).ThenInclude(x => x.Offsets)
                                  join pu in _context.ProjectUsers on m.MeasPoint.ProjectId
                                  equals pu.projectId
                                  where pu.userId == theuser.Id 
                                  && m.MeasPoint.ProjectId.Equals(model.projectNumber) 
                                  && m.TheMeasurement != null 
                                  && m.When >= model.startDate 
                                  && m.When < model.endDate.AddDays(1)
                                  select m).OrderBy(x => x.MeasPoint.MeasType.Type).ThenBy(x=>x.MeasPoint.Name).ThenBy(x => x.When).ToListAsync();
                }
                else
                {
                    data = await _context.Measures
                    .Include(x => x.MeasPoint).ThenInclude(x=>x.MeasType).Include(x => x.MeasPoint).ThenInclude(x => x.Offsets)
                    .Where(x => x.TheMeasurement != null
                    && x.MeasPoint.ProjectId.Equals(model.projectNumber)
                    && x.When >= model.startDate
                    && x.When < model.endDate.AddDays(1))
                    .OrderBy(x => x.MeasPoint.MeasType.Type).ThenBy(x => x.MeasPoint.Name).ThenBy(x => x.When).ToListAsync();
                }
                
            }
            else
            {
                if (User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
                {
                    var theuser = await _userManager.GetUserAsync(HttpContext.User);
                    data = await (from m in _context.Measures.Include(x=>x.MeasPoint).ThenInclude(x=>x.MeasType).Include(x => x.MeasPoint).ThenInclude(x => x.Offsets)
                                  join pu in _context.ProjectUsers on m.MeasPoint.ProjectId
                     equals pu.projectId
                     where pu.userId == theuser.Id 
                     && m.TheMeasurement != null 
                     && m.When >= model.startDate 
                     && m.When < model.endDate.AddDays(1)
                                  select m).OrderBy(x => x.MeasPoint.MeasType.Type).ThenBy(x => x.MeasPoint.Name).ThenBy(x => x.When).ToListAsync();
                }
                else
                {
                    data = await _context.Measures
                    .Include(x => x.MeasPoint).ThenInclude(x=>x.MeasType)
                    .Include(x => x.MeasPoint).ThenInclude(x=>x.Offsets)
                    .Where(x => x.TheMeasurement != null 
                    && x.When >= model.startDate 
                    && x.When < model.endDate.AddDays(1))
                    .OrderBy(x => x.MeasPoint.MeasType.Type).ThenBy(x => x.MeasPoint.Name).ThenBy(x => x.When).ToListAsync();
                }
                
            }
            if (model.ForScada)
            {
                List<string> headerrow = new List<string>(new string[] { "Datum", "Nr", "Kennung", "Tageswert", "0-1", "1-2", "2-3", "3-4", "4-5", "5-6", "6-7", "7-8", "8-9", "9-10", "10-11", "11-12", "12-13", "13-14", "14-15", "15-16", "16-17", "17-18", "18-19", "19-20", "20-21", "21-22", "22-23", "23-24" });
                sb.AppendLine(string.Join(model.Delimiter, headerrow.ToArray()));
                string lastnr = "10000";
                DateTime lastDate = DateTime.Now;
                DateTime d_o;
                int indexs;
                int counter = 0;
                double themeasurement = 0.00;
                List<string> fillerrow = new List<string>(new string[] { "", "", "X", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" });
                foreach (Meas m in data)
                {
                    Offset theoffset = new Offset();
                    if (m.TheMeasurement != null)
                    {

                        var theoffsets = m.MeasPoint.Offsets.OrderByDescending(x => x.starttime).ToList();
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
                        if (m.MeasPoint.MeasType.Type.ToLower().Equals("water level"))
                        {
                            themeasurement = theoffset.offset - Convert.ToDouble(m.TheMeasurement);

                        }
                        else if (m.MeasPoint.MeasTypeId.Equals(113))
                        {
                            themeasurement = await getWatermeterTrueMeas(m) * 10.0;
                        }
                        else if (m.MeasPoint.MeasTypeId.Equals(114))
                        {
                            themeasurement = await getWatermeterTrueMeas(m) * 100.0;
                        }
                        else if (m.MeasPoint.MeasType.Type.ToLower().Equals("water meter"))
                        {
                            themeasurement = await getWatermeterTrueMeas(m);
                        }
                       
                        else
                        {
                            themeasurement = Convert.ToDouble(m.TheMeasurement);
                        }
                        if (m.When.Year.Equals(lastDate.Year) && m.When.Month.Equals(lastDate.Month) && m.When.Date.Equals(lastDate.Date) && m.MeasPoint.LaborId.Equals(Convert.ToInt32(lastnr)))
                        {

                            d_o = m.When;
                            indexs = d_o.Hour + 4; //changed from 3
                            fillerrow[indexs] = themeasurement.ToString("0.##").Replace(".", ",");

                        }
                        else
                        {
                            if (counter != 0)
                            {
                                sb.AppendLine(string.Join(model.Delimiter, fillerrow.ToArray()));
                            }
                            fillerrow = new List<string>(new string[] { "", "", "X", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" });
                            d_o = m.When;
                            fillerrow[0] = string.Format("{0:dd.MM.yy}", d_o);
                            fillerrow[1] = m.MeasPoint.LaborId.ToString();
                            fillerrow[d_o.Hour + 4] = themeasurement.ToString("0.##").Replace(".",","); // changed from +3 to +4
                        }

                    }
                    lastnr = m.MeasPoint.LaborId.ToString();
                    lastDate = m.When;
                    counter += 1;

                }
                sb.AppendLine(string.Join(model.Delimiter, fillerrow.ToArray()));
                return File(System.Text.Encoding.ASCII.GetBytes(sb.ToString()), "text/csv", "data.csv");
            }
            else if (model.AddRaw)
            {
                List<List<string>> headerrows = new List<List<string>>();
                
                headerrows.Add(new List<string>(new string[] { _localizer.GetLocalizedHtmlString("MonitorPoint"), _localizer.GetLocalizedHtmlString("Date Measured"), _localizer.GetLocalizedHtmlString("Water Level in refernce"), _localizer.GetLocalizedHtmlString("Dip (m)"), _localizer.GetLocalizedHtmlString("ExtMonitorPoint"), _localizer.GetLocalizedHtmlString("Reference level"), _localizer.GetLocalizedHtmlString("Comments") }));
                headerrows.Add(new List<string>(new string[] { _localizer.GetLocalizedHtmlString("MonitorPoint"), _localizer.GetLocalizedHtmlString("Date Measured"), _localizer.GetLocalizedHtmlString("Reading Converted"), _localizer.GetLocalizedHtmlString("Reading Watermeter"), _localizer.GetLocalizedHtmlString("ExtMonitorPoint"), _localizer.GetLocalizedHtmlString("Last WM reference"), _localizer.GetLocalizedHtmlString("Comments") }));
                headerrows.Add(new List<string>(new string[] { _localizer.GetLocalizedHtmlString("MonitorPoint"), _localizer.GetLocalizedHtmlString("Date Measured"), _localizer.GetLocalizedHtmlString("Measurement Converted"), _localizer.GetLocalizedHtmlString("Raw Measurement"), _localizer.GetLocalizedHtmlString("ExtMonitorPoint"), _localizer.GetLocalizedHtmlString("Reference value for Measurement"), _localizer.GetLocalizedHtmlString("Comments") }));                
                double themeasurement = 0.00;
                double rawmeasurement = 0.00;
                List<string> fillerrow = new List<string>();
                sb.AppendLine(string.Join(model.Delimiter, headerrows[0].ToArray()));
                foreach (var m in data.Where(x=>x.MeasPoint.MeasType.Type.ToLower().Equals("water level")))
                {
                   
                    string thecomment = "";
                    if(m.NewComment != null && m.NewComment != "")
                    {
                        thecomment = m.NewComment;
                    }
                    else if(m.CommentId != null)
                    {
                        var thecommentdata = await _context.Comments.Where(x => x.Id.Equals(m.CommentId)).SingleOrDefaultAsync();
                        thecomment = thecommentdata.comment;
                    }
                    rawmeasurement = Convert.ToDouble(m.TheMeasurement);
                    
                    Offset theoffset = new Offset();
                    if (m.TheMeasurement != null)
                    {

                        var theoffsets = m.MeasPoint.Offsets.OrderByDescending(x => x.starttime).ToList();
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
                        fillerrow = new List<string>(new string[] { "", "", "", "","","","" });
                        fillerrow[0] = m.MeasPoint.Name;
                        fillerrow[1] = string.Format("{0:yyyy-MM-dd HH:mm:ss}", m.When);
                        fillerrow[2] = themeasurement.ToString("0.##");
                        fillerrow[3] = rawmeasurement.ToString("0.##");
                        if (m.MeasPoint.ScadaAddress != null)
                        {
                            fillerrow[4] = "A" + m.MeasPoint.ScadaAddress.ToString();
                        }
                        else
                        {
                            fillerrow[4] = "";
                        }
                        fillerrow[5] = theoffset.offset.ToString();
                        fillerrow[6] = thecomment;
                        sb.AppendLine(string.Join(model.Delimiter, fillerrow.ToArray()));

                    }
                    else
                    {
                        fillerrow[0] = m.MeasPoint.Name;
                        fillerrow[1] = string.Format("{0:yyyy-MM-dd HH:mm:ss}", m.When);
                        fillerrow[2] = "";
                        fillerrow[3] = "";
                        fillerrow[4] = "";
                        fillerrow[5] = theoffset.offset.ToString();
                        fillerrow[6] = thecomment;
                    }
                }
                sb.AppendLine(string.Join(model.Delimiter, headerrows[1].ToArray()));
                foreach (var m in data.Where(x => x.MeasPoint.MeasType.Type.ToLower().Equals("water meter") || x.MeasPoint.MeasTypeId.Equals(113) || x.MeasPoint.MeasTypeId.Equals(114)))
                {
                    
                    string thecomment = "";
                    if (m.NewComment != null && m.NewComment != "")
                    {
                        thecomment = m.NewComment;
                    }
                    else if (m.CommentId != null)
                    {
                        var thecommentdata = await _context.Comments.Where(x => x.Id.Equals(m.CommentId)).SingleOrDefaultAsync();
                        thecomment = thecommentdata.comment;
                    }
                    rawmeasurement = Convert.ToDouble(m.TheMeasurement);
                    Offset theoffset = new Offset();
                    if (m.TheMeasurement != null)
                    {

                        var theoffsets = m.MeasPoint.Offsets.OrderByDescending(x => x.starttime).ToList();
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
                        if (m.MeasPoint.MeasType.Type.ToLower().Equals("water level"))
                        {
                            themeasurement = theoffset.offset - Convert.ToDouble(m.TheMeasurement);

                        }
                        else if (m.MeasPoint.MeasTypeId.Equals(113))
                        {
                            themeasurement = await getWatermeterTrueMeas(m) * 10.0;
                        }
                        else if (m.MeasPoint.MeasTypeId.Equals(114))
                        {
                            themeasurement = await getWatermeterTrueMeas(m) * 100.0;
                        }
                        else if (m.MeasPoint.MeasType.Type.ToLower().Equals("water meter"))
                        {
                            themeasurement = await getWatermeterTrueMeas(m);
                        }
                        else
                        {
                            themeasurement = Convert.ToDouble(m.TheMeasurement);
                        }
                        fillerrow = new List<string>(new string[] { "", "", "", "", "", "","" });
                        fillerrow[0] = m.MeasPoint.Name;
                        fillerrow[1] = string.Format("{0:yyyy-MM-dd HH:mm:ss}", m.When);
                        fillerrow[2] = themeasurement.ToString("0.##");
                        fillerrow[3] = rawmeasurement.ToString("0.##");
                        if (m.MeasPoint.ScadaAddress != null)
                        {
                            fillerrow[4] = "A" + m.MeasPoint.ScadaAddress.ToString();
                        }
                        else
                        {
                            fillerrow[4] = "";
                        }
                        fillerrow[5] = theoffset.offset.ToString();
                        sb.AppendLine(string.Join(model.Delimiter, fillerrow.ToArray()));

                    }
                    else
                    {
                        fillerrow[0] = m.MeasPoint.Name;
                        fillerrow[1] = string.Format("{0:yyyy-MM-dd HH:mm:ss}", m.When);
                        fillerrow[2] = "";
                        fillerrow[3] = "";
                        fillerrow[4] = "";
                        fillerrow[5] = theoffset.offset.ToString();
                        fillerrow[6] = thecomment;
                    }
                }
                sb.AppendLine(string.Join(model.Delimiter, headerrows[2].ToArray()));
                foreach (Meas m in data.Where(x => !x.MeasPoint.MeasType.Type.ToLower().Equals("water meter") && !x.MeasPoint.MeasType.Type.ToLower().Equals("water level") && !x.MeasPoint.MeasTypeId.Equals(113) && !x.MeasPoint.MeasTypeId.Equals(114)))
                {
                    
                    string thecomment = "";
                    if (m.NewComment != null && m.NewComment != "")
                    {
                        thecomment = m.NewComment;
                    }
                    else if (m.CommentId != null)
                    {
                        var thecommentdata = await _context.Comments.Where(x => x.Id.Equals(m.CommentId)).SingleOrDefaultAsync();
                        thecomment = thecommentdata.comment;
                    }
                    
                    if (m.MeasPoint.MeasTypeId.Equals(113))
                    {
                        rawmeasurement = Convert.ToDouble(m.TheMeasurement) * 10.0;
                    }
                    else if (m.MeasPoint.MeasTypeId.Equals(114))
                    {
                        rawmeasurement = Convert.ToDouble(m.TheMeasurement) * 100.0;
                    }
                    else
                    {
                        rawmeasurement = Convert.ToDouble(m.TheMeasurement);
                    }
                    Offset theoffset = new Offset();
                    if (m.TheMeasurement != null)
                    {

                        var theoffsets = m.MeasPoint.Offsets.OrderByDescending(x => x.starttime).ToList();
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
                        if (m.MeasPoint.MeasType.Type.ToLower().Equals("water level"))
                        {
                            themeasurement = theoffset.offset - Convert.ToDouble(m.TheMeasurement);

                        }
                        else if (m.MeasPoint.MeasTypeId.Equals(113))
                        {
                            themeasurement = await getWatermeterTrueMeas(m) * 10.0;
                        }
                        else if (m.MeasPoint.MeasTypeId.Equals(114))
                        {
                            themeasurement = await getWatermeterTrueMeas(m) * 100.0;
                        }
                        else if (m.MeasPoint.MeasType.Type.ToLower().Equals("water meter"))
                        {
                            themeasurement = await getWatermeterTrueMeas(m);
                        }
                        else
                        {
                            themeasurement = Convert.ToDouble(m.TheMeasurement);
                        }
                        fillerrow = new List<string>(new string[] { "", "", "", "", "", "","" });
                        fillerrow[0] = m.MeasPoint.Name;
                        fillerrow[1] = string.Format("{0:yyyy-MM-dd HH:mm:ss}", m.When);
                        fillerrow[2] = themeasurement.ToString("0.##");
                        fillerrow[3] = rawmeasurement.ToString("0.##");
                        if (m.MeasPoint.ScadaAddress != null)
                        {
                            fillerrow[4] = "A" + m.MeasPoint.ScadaAddress.ToString();
                        }
                        else
                        {
                            fillerrow[4] = "";
                        }
                        fillerrow[5] = theoffset.offset.ToString();
                        fillerrow[6] = thecomment;
                        sb.AppendLine(string.Join(model.Delimiter, fillerrow.ToArray()));

                    }
                    else
                    {
                        fillerrow[0] = m.MeasPoint.Name;
                        fillerrow[1] = string.Format("{0:yyyy-MM-dd HH:mm:ss}", m.When);
                        fillerrow[2] = "";
                        fillerrow[3] = "";
                        fillerrow[4] = "";
                        fillerrow[5] = theoffset.offset.ToString();
                        fillerrow[6] = thecomment;
                    }
                }
                return File(System.Text.Encoding.ASCII.GetBytes(sb.ToString()), "text/csv", "ManualData.csv");
            }
            else
            {
                List<string> headerrow = new List<string>(new string[] { "MonitorPoint", "DateMeasured", "Type", "Value", "Active","ExtMonitorPoint" });
                sb.AppendLine(string.Join(model.Delimiter, headerrow.ToArray()));
                double themeasurement = 0.00;
                List<string> fillerrow = new List<string>();
                foreach (Meas m in data)
                {
                    
                    Offset theoffset = new Offset();
                    if (m.TheMeasurement != null)
                    {

                        var theoffsets = m.MeasPoint.Offsets.OrderByDescending(x => x.starttime).ToList();
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
                        if (m.MeasPoint.MeasType.Type.ToLower().Equals("water level"))
                        {
                            themeasurement = theoffset.offset - Convert.ToDouble(m.TheMeasurement);

                        }
                        else if (m.MeasPoint.MeasTypeId.Equals(113))
                        {
                            themeasurement = await getWatermeterTrueMeas(m) * 10.0;
                        }
                        else if (m.MeasPoint.MeasTypeId.Equals(114))
                        {
                            themeasurement = await getWatermeterTrueMeas(m) * 100.0;
                        }
                        else if (m.MeasPoint.MeasType.Type.ToLower().Equals("water meter"))
                        {
                            themeasurement = await getWatermeterTrueMeas(m);
                        }
                        else
                        {
                            themeasurement = Convert.ToDouble(m.TheMeasurement);
                        }
                        fillerrow = new List<string>(new string[] { "", "", "", "", "", "" });
                        fillerrow[0] = m.MeasPoint.Name;
                        fillerrow[1] = string.Format("{0:yyyy-MM-dd hh:mm:ss}", m.When);
                        fillerrow[2] = "";
                        fillerrow[3] = themeasurement.ToString("0.##");
                        fillerrow[4] = "";
                        if (m.MeasPoint.ScadaAddress != null)
                        {
                            fillerrow[5] = "A" + m.MeasPoint.ScadaAddress.ToString();
                        }
                        else
                        {
                            fillerrow[5] = "";
                        } 
                        sb.AppendLine(string.Join(model.Delimiter, fillerrow.ToArray()));

                    }
                }
                return File(System.Text.Encoding.ASCII.GetBytes(sb.ToString()), "text/csv", "ManualData.csv");
            }




        }
        public async Task<double> getWatermeterTrueMeas(Meas m_1)
        {
            double thesum = 0;
            double lastmeas = 0;
            double lastoffset = 0;
            int lastoffsetId = 0;
            var mp = await _context.MeasPoints.Where(x => x.Id.Equals(m_1.MeasPointId)).SingleOrDefaultAsync();
            var measures = await _context.Measures.Where(x => x.MeasPointId.Equals(m_1.MeasPointId) && x.When <= m_1.When && x.TheMeasurement != null).OrderBy(x=>x.When).ToListAsync();
            foreach (Meas m in measures)
            {
                Offset theoffset = new Offset();
                try
                {
                    theoffset = mp.Offsets.Where(x => x.starttime <= m.When && x.MeasPointId.Equals(m.MeasPointId)).OrderByDescending(x => x.starttime).First();
                }
                catch
                {
                    try
                    {
                        theoffset = mp.Offsets.Where(x => x.MeasPointId.Equals(m.MeasPointId)).OrderBy(x => x.starttime).First();
                    }
                    catch
                    {
                        try
                        {
                            theoffset.offset = lastoffset;
                            theoffset.Id = lastoffsetId;
                            theoffset.starttime = DateTime.Now;
                        }
                        catch
                        {
                            theoffset.offset = m.MeasPoint.Offset;
                            theoffset.starttime = DateTime.Now;
                        }
                       
                    }

                }
                //if (lastoffset + 0.01 >= theoffset.offset && lastoffset - 0.01 < theoffset.offset)
                if(lastoffsetId.Equals(theoffset.Id))
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
        [HttpPost]
        [Authorize(Roles = ("Admin,DivisionAdmin,Member,MemberGuest,ProjectMember"))]
        public async Task<IActionResult> CreateMore(MeasMoreVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if(model.MeasPointLevelIds.Count > 0)
                {
                    for(int i = 0; i < model.MeasPointLevelIds.Count; i++) { 
                        Meas LevelMeas = new Meas();
                        LevelMeas.MeasPointId = model.MeasPointLevelIds.ElementAt(i);//(await _context.MeasPoints.Include(x => x.MeasType).SingleOrDefaultAsync(x => x.getBaseName.Equals(model.BaseName) && x.ProjectId.Equals(model.ProjectId) && x.MeasType.Type.ToLower().Equals("water level"))).Id;
                        if(LevelMeas.MeasPointId != null) {
                            LevelMeas.TheMeasurement = model.MeasPointLevelMeasures.ElementAt(i);
                            LevelMeas.Longitude = model.Longitude;
                            LevelMeas.Latitude = model.Latitude;
                            LevelMeas.NewComment = model.MeasPointLevelComment.ElementAtOrDefault(i);
                            LevelMeas.CommentId = model.CommentId;
                            LevelMeas.When = model.When;
                            LevelMeas.DoneBy = user.full_name();
                            //if (LevelMeas.CommentId == null && (LevelMeas.NewComment == null || LevelMeas.NewComment == "") && LevelMeas.TheMeasurement == null)
                            if (LevelMeas.CommentId != null || !string.IsNullOrEmpty(LevelMeas.NewComment) || LevelMeas.TheMeasurement != null)
                            {
                                _context.Add(LevelMeas);
                                await UploadMeasToFTP(LevelMeas);
                            }
                            //else
                            //{
                            //    _context.Add(LevelMeas);
                            //    await UploadMeasToFTP(LevelMeas);
                            //}
                            
                        }
                    }
                }
                if(model.MeasPointFlowIds.Count > 0)
                {
                    for(int i = 0;i < model.MeasPointFlowIds.Count;i++)
                    {
                        Meas FlowMeas = new Meas();
                        FlowMeas.MeasPointId =  model.MeasPointFlowIds.ElementAt(i);//(await _context.MeasPoints.Include(x => x.MeasType).SingleOrDefaultAsync(x => x.getBaseName.Equals(model.BaseName) && x.ProjectId.Equals(model.ProjectId) && x.MeasType.Type.ToLower().Equals("flow rate"))).Id;
                        if (FlowMeas.MeasPointId != null)
                        {
                            FlowMeas.TheMeasurement = model.MeasPointFlowMeasures.ElementAt(i);
                            FlowMeas.Longitude = model.Longitude;
                            FlowMeas.Latitude = model.Latitude;
                            FlowMeas.NewComment = model.MeasPointFlowComment.ElementAtOrDefault(i);
                            FlowMeas.CommentId = model.CommentId;
                            FlowMeas.When = model.When;
                            FlowMeas.DoneBy = user.full_name();
                            if (FlowMeas.CommentId == null && (FlowMeas.NewComment == null || FlowMeas.NewComment == "") && FlowMeas.TheMeasurement == null)
                            {

                            }
                            else
                            {
                                _context.Add(FlowMeas);
                                await UploadMeasToFTP(FlowMeas);
                            }

                        }
                    }
                }
                if(model.MeasPointWMIds.Count > 0)
                {
                    for(int i = 0; i < model.MeasPointWMIds.Count; i++)
                    {
                        Meas WMMeas = new Meas();
                        WMMeas.MeasPointId = model.MeasPointWMIds.ElementAt(i);//(await _context.MeasPoints.Include(x => x.MeasType).SingleOrDefaultAsync(x => x.getBaseName.Equals(model.BaseName) && x.ProjectId.Equals(model.ProjectId) && (x.MeasType.Type.ToLower().Equals("water meter") || x.MeasType.Type.ToLower().Equals("water meter *10") || x.MeasType.Type.ToLower().Equals("water meter *100")))).Id;
                        if (WMMeas.MeasPointId != null)
                        {
                            WMMeas.TheMeasurement = model.MeasPointWMMeasures.ElementAt(i);
                            WMMeas.Longitude = model.Longitude;
                            WMMeas.Latitude = model.Latitude;
                            WMMeas.NewComment = model.MeasPointWMComment.ElementAtOrDefault(i);
                            WMMeas.CommentId = model.CommentId;
                            WMMeas.When = model.When;
                            WMMeas.DoneBy = user.full_name();
                            if (WMMeas.CommentId == null && (WMMeas.NewComment == null || WMMeas.NewComment == "") && WMMeas.TheMeasurement == null)
                            {

                            }
                            else
                            {
                                _context.Add(WMMeas);
                                await UploadMeasToFTP(WMMeas);
                            }
                        }
                    }
                }
                
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadAllPreviousMeasToFTP()
        {

            var measures = await _context.Measures
                .Include(x => x.MeasPoint).ThenInclude(x => x.Offsets)
                .Include(x => x.MeasPoint).ThenInclude(x => x.Project)
                .Include(x => x.MeasPoint).ThenInclude(x => x.MeasType)
                .Include(x => x.TheComment)
                .Where(x => x.MeasPoint.Project.DivisionId.Equals(9)).ToListAsync();
            
            var directory = _env.WebRootPath + "\\TempFile\\";
            var path = Path.Combine(directory, "All.txt");
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
                if(meas.TheMeasurement != null)
                {
                    datarow[3] = String.Format("{0:#,0.000}", meas.TheMeasurement);
                }
                else
                {
                    datarow[3] = "";
                }
                datarow[4] = theoffset.offset.ToString();
                if(meas.TheMeasurement != null) {
                    datarow[5] = String.Format("{0:#,0.000}", themeasurement);
                }
                else
                {
                    datarow[5] = "";
                }

                datarow[6] = meas.NewComment;
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
                catch{

                }
               
            }
            return RedirectToAction(nameof(Index));
        }



        public async Task UploadMeasToFTP(Meas meas)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.DivisionId.Equals(9) || User.IsInRole("Admin"))
            {
                var directory = _env.WebRootPath + "\\TempFile\\";

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var mp = await _context.MeasPoints.Include(x => x.Offsets).Include(x => x.Project).Include(x => x.MeasType).SingleOrDefaultAsync(x => x.Id.Equals(meas.MeasPointId));

                try
                {
                    if (mp.Project.DivisionId.Equals(9))
                    {
                        Offset theoffset = new Offset();
                        double themeasurement = 100000;
                        if (meas.TheMeasurement != null)
                        {

                            var theoffsets =  mp.Offsets.OrderByDescending(x => x.starttime).ToList();
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
                                        theoffset.offset = mp.Offset;
                                        theoffset.starttime = DateTime.Now;
                                    }

                                }
                            }
                            if (mp.MeasType.Type.ToLower().Equals("water level"))
                            {
                                themeasurement = theoffset.offset - Convert.ToDouble(meas.TheMeasurement);

                            }
                            else if (mp.MeasTypeId.Equals(113))
                            {
                                themeasurement = await getWatermeterTrueMeas(meas) * 10.0;
                            }
                            else if (mp.MeasTypeId.Equals(114))
                            {
                                themeasurement = await getWatermeterTrueMeas(meas) * 100.0;
                            }
                            else if (mp.MeasType.Type.ToLower().Equals("water meter"))
                            {
                                themeasurement = await getWatermeterTrueMeas(meas);
                            }

                            else
                            {
                                themeasurement = Convert.ToDouble(meas.TheMeasurement);
                            }
                        }
                        var path = Path.Combine(directory, mp.LaborId.ToString() + "_" + meas.When.ToString("yyyyMMdd_HHmmss") + ".txt");
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
                        List<string> datarow = new List<string>(new string[8]);
                        datarow[0] = mp.Name;
                        datarow[1] = mp.LaborId.ToString();
                        datarow[2] = meas.When.ToString("yyyy-MM-dd HH:mm:ss");
                        if(meas.TheMeasurement != null)
                        {
                            datarow[3] = String.Format("{0:#,0.000}", meas.TheMeasurement);
                        }
                        else
                        {
                            datarow[3] = "";
                        }
                       
                        datarow[4] = theoffset.offset.ToString();
                        if(meas.TheMeasurement != null)
                        {
                            datarow[5] = String.Format("{0:#,0.000}", themeasurement);
                        }
                        else
                        {
                            datarow[5] = "";
                        }
                        datarow[6] = meas.NewComment;
                        if(meas.CommentId != null)
                        {
                            var comment = await _context.Comments.SingleOrDefaultAsync(x => x.Id.Equals(meas.CommentId));
                            datarow[7] = comment.comment;
                        }
                        else
                        {
                            datarow[7] = "";
                        }
                        sb.AppendLine(string.Join(";", headerrow.ToArray()));
                        sb.AppendLine(string.Join(";", datarow.ToArray()));
                        System.IO.File.WriteAllText(path, sb.ToString());
                        //SFTP
                        PasswordAuthenticationMethod pMethod = new PasswordAuthenticationMethod(_Options.UserName, _Options.Key);
                        Renci.SshNet.ConnectionInfo conInfo = new Renci.SshNet.ConnectionInfo(_Options.Domain, 22, _Options.UserName, pMethod);
                        using (SftpClient client = new SftpClient(conInfo))
                        {

                            client.Connect();
                            try
                            {
                                using (var fileStream = new FileStream(path, FileMode.Open))
                                {
                                    client.BufferSize = 1024;
                                    client.ChangeDirectory("/public/MainOps");
                                    client.UploadFile(fileStream, Path.GetFileName(path));
                                }
                            }
                            catch
                            {
                                client.Disconnect();
                            }
                            client.Disconnect();
                        }
                    }
                }
                catch
                {
                    //send an email to the user.
                }
            }
        }
        [HttpGet]
        [Authorize(Roles =("Admin,DivisionAdmin,Member,MemberGuest"))]
        public async Task<IActionResult> CreateMeas(int? id)
        {
            if (id != null)
            {
                var measp = await _context.MeasPoints.Include(x => x.MeasType).Include(x => x.Project).Where(x => x.Id.Equals(id)).SingleOrDefaultAsync();
                if (measp != null)
                {
                    ViewData["IsPosted"] = 1;
                    if (User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
                    {
                        var user = await _userManager.GetUserAsync(User);

                        var allowedproject = await _context.ProjectUsers.Where(x => x.userId.Equals(user.Id) && x.projectId.Equals(measp.ProjectId)).FirstOrDefaultAsync();
                        if (allowedproject == null)
                        {
                            return RedirectToAction("ErrorMessage", "Home", new { text = "You are not allowed to enter data for this measurement point" });
                        }

                        var measpointsguest = await _context.MeasPoints
                            .Include(x => x.MeasType)
                            .Where(x => x.ProjectId.Equals(measp.ProjectId)
                                            && (
                                                x.Name.Equals(measp.Name)
                                                || (x.getBaseName.Equals(measp.getBaseName) && x.Name.Contains("_1"))
                                                || (x.getBaseName.Equals(measp.getBaseName) && x.Name.Contains("_2"))
                                                || (x.getBaseName.Equals(measp.getBaseName) && x.Name.Contains("_3"))
                                                || (x.getBaseName.Equals(measp.getBaseName) && x.Name.ToLower().Contains("watermeter"))
                                                || (x.getBaseName.Equals(measp.getBaseName) && x.Name.ToLower().Contains("flow"))
                                            )
                            ).ToListAsync();

                        if (measpointsguest.Count > 1)
                        {
                            MeasMoreVM model = new MeasMoreVM();
                            foreach (MeasPoint mp in measpointsguest)
                            {
                                if (mp.MeasType.Type.ToLower().Equals("water level"))
                                {
                                    model.MeasPointLevelIds.Add(mp.Id);
                                    model.MeasPointNameLevelIds.Add(mp.Name);
                                    model.MeasPointLevelComment.Add("");
                                    model.MeasPointLevelMeasures.Add(null);
                                }
                                else if (mp.MeasType.Type.ToLower().Equals("flow rate"))
                                {
                                    model.MeasPointFlowIds.Add(mp.Id);
                                    model.MeasPointNameFlowIds.Add(mp.Name);
                                    model.MeasPointFlowComment.Add("");
                                    model.MeasPointFlowMeasures.Add(null);
                                }
                                else if (mp.MeasType.Type.ToLower().Equals("water meter") || mp.MeasType.Type.ToLower().Equals("water meter *10") || mp.MeasType.Type.ToLower().Equals("water meter *100"))
                                {
                                    model.MeasPointWMIds.Add(mp.Id);
                                    model.MeasPointNameWMIds.Add(mp.Name);
                                    model.MeasPointWMComment.Add("");
                                    model.MeasPointWMMeasures.Add(null);
                                }
                                else
                                {
                                    model.MeasPointLevelIds.Add(mp.Id);
                                    model.MeasPointNameLevelIds.Add(mp.Name);
                                    model.MeasPointLevelComment.Add("");
                                    model.MeasPointLevelMeasures.Add(null);
                                }

                            }
                            model.ProjectId = Convert.ToInt32(measp.ProjectId);
                            model.BaseName = measp.getBaseName;

                            ViewData["CommentId"] = new SelectList(_context.Comments.OrderBy(x => x.comment), "Id", "comment");
                            model.When = DateTime.Now;
                            return View("CreateMore", model);
                        }


                    }
                    
                    var measpoints = await _context.MeasPoints
                        .Include(x => x.MeasType)
                        .Where(x => x.ProjectId.Equals(measp.ProjectId) && 
                                    (x.Name.Equals(measp.Name) || (x.getBaseName.Equals(measp.getBaseName) && x.Name.Contains("_1")) || (x.getBaseName.Equals(measp.getBaseName) && x.Name.Contains("_2")) || (x.getBaseName.Equals(measp.getBaseName) && x.Name.Contains("_3")) || (x.getBaseName.Equals(measp.getBaseName) && x.Name.ToLower().Contains("watermeter")) || (x.getBaseName.Equals(measp.getBaseName) && x.Name.ToLower().Contains("flow")))).ToListAsync();
                    if (measpoints.Count > 1)
                    {
                        MeasMoreVM model = new MeasMoreVM();
                        foreach (MeasPoint mp in measpoints)
                        {
                            if (mp.MeasType.Type.ToLower().Equals("water level"))
                            {
                                model.MeasPointLevelIds.Add(mp.Id);
                                model.MeasPointNameLevelIds.Add(mp.Name);
                                model.MeasPointLevelComment.Add("");
                                model.MeasPointLevelMeasures.Add(null);
                            }
                            else if (mp.MeasType.Type.ToLower().Equals("flow rate"))
                            {
                                model.MeasPointFlowIds.Add(mp.Id);
                                model.MeasPointNameFlowIds.Add(mp.Name);
                                model.MeasPointFlowComment.Add("");
                                model.MeasPointFlowMeasures.Add(null);
                            }
                            else if (mp.MeasType.Type.ToLower().Equals("water meter") || mp.MeasType.Type.ToLower().Equals("water meter *10") || mp.MeasType.Type.ToLower().Equals("water meter *100"))
                            {
                                model.MeasPointWMIds.Add(mp.Id);
                                model.MeasPointNameWMIds.Add(mp.Name);
                                model.MeasPointWMComment.Add("");
                                model.MeasPointWMMeasures.Add(null);
                            }
                            else
                            {
                                model.MeasPointLevelIds.Add(mp.Id);
                                model.MeasPointNameLevelIds.Add(mp.Name);
                                model.MeasPointLevelComment.Add("");
                                model.MeasPointLevelMeasures.Add(null);
                            }

                        }
                        model.ProjectId = Convert.ToInt32(measp.ProjectId);
                        model.BaseName = measp.getBaseName;

                        ViewData["CommentId"] = new SelectList(_context.Comments.OrderBy(x => x.comment), "Id", "comment");
                        model.When = DateTime.Now;
                        return View("CreateMore", model);
                    }



                    ViewData["ProjectId"] = await GetProjectList();
                    ViewData["CommentId"] = new SelectList(_context.Comments.OrderBy(x => x.comment), "Id", "comment");
                    List<SelectListItem> selList = await createMeasPointList();
                    ViewData["MeasPointId"] = selList;
                    var lastMeas = await _context.Measures.LastAsync();
                    Meas measvar = new Meas();
                    measvar.When = DateTime.Now;
                    measvar.MeasPointId = id;
                    
                    return View("Create", measvar);


                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }
            
        }
        [HttpGet]
        [Authorize(Roles = ("Admin,DivisionAdmin,Member,MemberGuest"))]
        public async Task<IActionResult> Create()
        {
            var theuser = await _userManager.GetUserAsync(User);
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["CommentId"] = new SelectList(_context.Comments.OrderBy(x => x.comment), "Id", "comment");
            List<SelectListItem> selList = await createMeasPointList();
            ViewData["MeasPointId"] = selList;
            var lastMeas = await _context.Measures.LastAsync();

            Meas measvar = new Meas();
            measvar.When = lastMeas.When;
            return View(measvar);
        }
        


        // POST: Meas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Admin,DivisionAdmin,Member,MemberGuest"))]
        public async Task<IActionResult> Create([Bind("Id,TheMeasurement,CommentId,When,MeasPointId,Latitude,Longitude,DoneBy,NewComment")] Meas meas,string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                /*if (meas.NewComment != null && meas.NewComment != "")
                {
                    bool exists = await DoesCommentExist(meas.NewComment);
                    if (!exists)
                    {
                        Comment newCom = new Comment();
                        newCom.comment = meas.NewComment;
                        _context.Add(newCom);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        var theid = await _context.Comments.FirstAsync(x => x.comment.Trim().ToLower().Equals(meas.NewComment.Trim().ToLower()));
                        meas.CommentId = Convert.ToInt32(theid);
                    }
                }
                */
                if(meas.TheMeasurement != null || meas.TheComment != null || meas.NewComment != null)
                {
                    _context.Add(meas);
                    await _context.SaveChangesAsync();
                    var theuser = await _userManager.GetUserAsync(User);
                    var mp = await _context.MeasPoints.FindAsync(meas.MeasPointId);
                    var boqitem = await _context.ItemTypes.SingleOrDefaultAsync(x => x.ProjectId.Equals(mp.ProjectId) && x.ReportTypeId.Equals(9));
                    if (boqitem != null)
                    {
                        string location = "";
                        double lat = 0.0;
                        double lng = 0.0;

                        try
                        {
                            location = FindNearestKM(Convert.ToInt32(mp.ProjectId), Convert.ToDouble(meas.Latitude), Convert.ToDouble(meas.Longitude));
                        }
                        catch
                        {

                        }
                        if(meas.Latitude != null)
                        {
                            lat = Convert.ToDouble(meas.Latitude);
                        }
                        if(meas.Longitude != null)
                        {
                            lng = Convert.ToDouble(meas.Longitude);
                        }
                        Install inst = new Install
                        {
                            ItemTypeId = boqitem.Id,
                            Amount = 1,
                            TimeStamp = meas.When,
                            RentalStartDate = meas.When,
                            DoneBy = theuser.full_name(),
                            EnteredIntoDataBase = DateTime.Now,
                            Latitude = lat,
                            Longitude = lng,
                            Install_Text = String.Concat("Dip on: ", mp.Name, " ", meas.NewComment),
                            ProjectId = mp.ProjectId,
                            SubProjectId = mp.SubProjectId,
                            isInstalled = false,
                            Location = location,
                            IsInOperation = false,
                            UniqueID = "N/A",
                            LastEditedInDataBase = null,
                            DeinstallDate = DateTime.Now.Date,
                            ToBePaid = true,

                        };
                        _context.Installations.Add(inst);
                        await _context.SaveChangesAsync();
                        var lastinstall = await _context.Installations.LastAsync();
                        DeInstall deinst = new DeInstall
                        {
                            InstallId = lastinstall.Id,
                            TimeStamp = meas.When.AddSeconds(1),
                            ItemTypeId = boqitem.Id,
                            DeInstall_Text = "handdip",
                            Amount = 1,
                            Latitude = lat,
                            Longitude = lng,
                            ProjectId = mp.ProjectId,
                            SubProjectId = mp.SubProjectId,
                            EnteredIntoDataBase = DateTime.Now
                        };
                        _context.Deinstallations.Add(deinst);
                        await _context.SaveChangesAsync();
                    }
                }
                await UploadMeasToFTP(meas);
                ViewData["ReturnUrl"] = returnUrl;
                if (returnUrl != null)
                {                   
                    return RedirectToAction("ExportDipList","MeasPoints");
                }
                else { 
                    return RedirectToAction(nameof(Create));
                }

            }
            
            ViewData["CommentId"] = new SelectList(_context.Comments.OrderBy(x => x.comment), "Id", "comment", meas.CommentId);
            List<SelectListItem> selList = await createMeasPointList();
            ViewData["MeasPointId"] = selList;
            return View(meas);
        }
        public async Task<bool> DoesCommentExist(string com)
        {
            var comments = await _context.Comments.Where(x => x.comment.Trim().ToLower().Equals(com.Trim().ToLower())).ToListAsync();
            if(comments != null)
            {
                if(comments.Count() >= 1)
                {
                    return true;
                }
            }
            return false;
        }
        
        // GET: Meas/Edit/5
        [Authorize(Roles = ("Admin,DivisionAdmin,Member,MemberGuest"))]
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (id == null)
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "Wrong call to controller" });
            }

            var meas = await _context.Measures.Include(m => m.TheComment).Include(m => m.MeasPoint).Where(x => x.Id.Equals(id)).FirstAsync();            
            if (meas == null)
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "Cannot find Measurement" });
            }
            if (!User.IsInRole("Admin") && !User.IsInRole("DivisionAdmin") && !User.IsInRole("Manager") && !meas.DoneBy.Equals(user.full_name()))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this measurement" });
            }
            ViewData["CommentId"] = new SelectList(_context.Comments.OrderBy(x => x.comment), "Id", "comment", meas.CommentId);
            List<SelectListItem> selList = await createMeasPointList();
            ViewData["MeasPointId"] = selList;
            return View(meas);
        }

        // POST: Meas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Admin,DivisionAdmin,Member,MemberGuest"))]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TheMeasurement,CommentId,When,MeasPointId,Latitude,Longitude,DoneBy,NewComment")] Meas meas)
        {

            if (id != meas.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(meas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MeasExists(meas.Id))
                    {
                        return RedirectToAction("ErrorMessage", "Home", new { text = "Something went wrong" });
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CommentId"] = new SelectList(_context.Comments, "Id", "comment", meas.CommentId);
            List<SelectListItem> selList = await createMeasPointList();
            ViewData["MeasPointId"] = selList;
            return View(meas);
        }

        // GET: Meas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var meas = await _context.Measures
                .Include(m => m.MeasPoint).Include(m => m.TheComment)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (meas == null)
            {
                return NotFound();
            }
            if (User.IsInRole("Admin") || User.IsInRole("DivisionAdmin") || User.IsInRole("Manager") || user.full_name().Equals(meas.DoneBy)) 
            { 
                return View(meas);
            }
            else
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have permission to delete this measurement" });
            }
        }

        // POST: Meas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var meas = await _context.Measures.FindAsync(id);
            _context.Measures.Remove(meas);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MeasExists(int id)
        {
            return _context.Measures.Any(e => e.Id == id);
        }
        [HttpPost]
        public async Task<string> CheckAlertLimit(string measurementIn, string thetimeIn, string IdIn)
        {
            string alertMessage = "No Limit Breached";
            if (measurementIn != null && measurementIn != "")
            {
                double measurement = Convert.ToDouble(measurementIn);
                
                int Id = Convert.ToInt32(IdIn);
                var thetime = Convert.ToDateTime(thetimeIn);
                var measpoint = await _context.MeasPoints.Include(x=>x.Offsets).Include(x=>x.Project).Include(x=>x.MeasType).Where(x=>x.Id.Equals(Id)).FirstAsync();
                var offset = measpoint.Offsets.OrderByDescending(x => x.starttime).FirstOrDefault();
                if(measpoint.MeasType.Type.ToLower().Equals("water level"))
                {
                    measurement = offset.offset - measurement;
                }
                if (measpoint.MeasType.Type.ToLower().Equals("water meter"))
                {
                    if (measpoint.Lower_Lower_Limit != null && measpoint.Middle_Lower_Limit != null)
                    {
                        if (measurement < measpoint.Lower_Lower_Limit)
                        {
                            alertMessage =  "Lower Limit";
                        }
                        else if (measurement < measpoint.Middle_Lower_Limit)
                        {
                            alertMessage = "Middle Lower Limit";
                        }
                    }
                    else if (measpoint.Lower_Lower_Limit != null && measpoint.Middle_Lower_Limit == null)
                    {
                        if (measurement < measpoint.Lower_Lower_Limit)
                        {
                            alertMessage = "Lower Limit";
                        }
                    }
                    if (measpoint.Upper_Upper_Limit != null && measpoint.Middle_Upper_Limit != null)
                    {
                        if (measurement > measpoint.Upper_Upper_Limit)
                        {
                            alertMessage = "Upper Limit";
                        }
                        else if (measurement > measpoint.Middle_Upper_Limit)
                        {
                            alertMessage = "Middle Upper Limit";
                        }
                    }
                    else if (measpoint.Upper_Upper_Limit != null && measpoint.Middle_Upper_Limit == null)
                    {
                        if (measurement > measpoint.Upper_Upper_Limit)
                        {
                            alertMessage = "Upper Limit";
                        }
                    }
                    var measures = await _context.Measures.Where(x => x.MeasPointId.Equals(Id) && x.TheMeasurement != null && x.When < thetime).OrderByDescending(x => x.When).ToListAsync();
                    if(measures != null)
                    {
                        var lastmeas = measures.First();
                        double timediff = 0.0;
                        double flowrate = 0.0;
                        if (measpoint.Project.DivisionId.Equals(2))
                        {                      
                            timediff = thetime.Subtract(lastmeas.When).TotalMinutes;
                        }
                        else
                        {
                            timediff = thetime.Subtract(lastmeas.When).TotalHours;
                        }
                        
                        var offsetchanges = measpoint.Offsets.Where(x => x.starttime > lastmeas.When && x.starttime < thetime).OrderByDescending(x=>x.starttime).ToList();
                        if(offsetchanges != null && offsetchanges.Count > 0)
                        {
                            var trueWM = await getWatermeterTrueMeas(lastmeas);
                            Meas newMeas = new Meas{ When = thetime, TheMeasurement = measurement, MeasPointId = Id};
                            var newTrueWM = measurement - offsetchanges.First().offset;
                            flowrate = (newTrueWM - trueWM) / timediff;
                        }
                        else
                        {
                            flowrate = (measurement - Convert.ToDouble(lastmeas.TheMeasurement)) / timediff;
                        
                        }
                        if(flowrate != 0.0)
                        {
                            if (measpoint.Flow_Lower_Lower_Limit != null && measpoint.Flow_Middle_Lower_Limit != null)
                            {
                                if (flowrate < measpoint.Flow_Lower_Lower_Limit)
                                {
                                    if (alertMessage == "No Limit Breached")
                                    {
                                        alertMessage = "Lower Limit Flowrate";
                                    }
                                    else
                                    {
                                        alertMessage = alertMessage +  " And Lower Limit Flowrate";
                                    }
                                
                                }
                                else if (flowrate < measpoint.Flow_Middle_Lower_Limit)
                                {
                                    if(alertMessage == "No Limit  Breached")
                                    {
                                        alertMessage = "Middle Lower Limit Flowrate";
                                    }
                                    else
                                    {
                                        alertMessage = alertMessage + " And Middle Lower Limit Flowrate";
                                    }
                                
                                }
                            }
                            else if (measpoint.Flow_Lower_Lower_Limit != null && measpoint.Flow_Middle_Lower_Limit == null)
                            {
                                if (flowrate < measpoint.Flow_Lower_Lower_Limit)
                                {
                                    if (alertMessage == "No Limit  Breached")
                                    {
                                        alertMessage = "Lower Limit Flowrate";
                                    }
                                    else
                                    {
                                        alertMessage = alertMessage + " And Lower Limit Flowrate";
                                    }
                                }
                            }
                            if (measpoint.Flow_Upper_Upper_Limit != null && measpoint.Flow_Middle_Upper_Limit != null)
                            {
                                if (flowrate > measpoint.Flow_Upper_Upper_Limit)
                                {
                                    if (alertMessage == "No Limit  Breached")
                                    {
                                        alertMessage = "Upper Limit Flowrate";
                                    }
                                    else
                                    {
                                        alertMessage = alertMessage + " And Upper Limit Flowrate";
                                    }
                                }
                                else if (flowrate > measpoint.Flow_Middle_Upper_Limit)
                                {
                                    if (alertMessage == "No Limit  Breached")
                                    {
                                        alertMessage = "Middle Upper Limit Flowrate";
                                    }
                                    else
                                    {
                                        alertMessage = alertMessage + " And Middle Upper Limit Flowrate";
                                    }
                                }
                            }
                            else if (measpoint.Flow_Upper_Upper_Limit != null && measpoint.Flow_Middle_Upper_Limit == null)
                            {
                                if (flowrate > measpoint.Flow_Upper_Upper_Limit)
                                {
                                    if (alertMessage == "No Limit  Breached")
                                    {
                                        alertMessage = "Upper Limit Flowrate";
                                    }
                                    else
                                    {
                                        alertMessage = alertMessage + " And Upper Limit Flowrate";
                                    }
                                }
                            }
                        }                  
                    }
                }
                //not water meter dont check flow rates
                else
                {
                    if(measpoint.Lower_Lower_Limit != null && measpoint.Middle_Lower_Limit != null)
                    {
                        if(measurement < measpoint.Lower_Lower_Limit)
                        {
                            alertMessage = "Lower Limit";
                        }
                        else if(measurement < measpoint.Middle_Lower_Limit)
                        {
                            alertMessage = "Middle Lower Limit";
                        }
                    }
                    else if(measpoint.Lower_Lower_Limit != null && measpoint.Middle_Lower_Limit == null)
                    {
                        if (measurement < measpoint.Lower_Lower_Limit)
                        {
                            alertMessage = "Lower Limit";
                        }
                    }
                    if (measpoint.Upper_Upper_Limit != null && measpoint.Middle_Upper_Limit != null)
                    {
                        if (measurement > measpoint.Upper_Upper_Limit)
                        {
                            alertMessage = "Upper Limit";
                        }
                        else if (measurement > measpoint.Middle_Upper_Limit)
                        {
                            alertMessage = "Middle Upper Limit";
                        }
                    }
                    else if (measpoint.Upper_Upper_Limit != null && measpoint.Middle_Upper_Limit == null)
                    {
                        if (measurement > measpoint.Upper_Upper_Limit)
                        {
                            alertMessage = "Upper Limit";
                        }
                    }
                }
            }
            return alertMessage;
        }
        [HttpGet]
        public async Task<IActionResult> ExportReport()
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
            return View("ExportReport");
        }
        [HttpPost]
        public async Task<IActionResult> ExportReport(ExportDataReportViewModel model)
        {
            StringBuilder sb = new StringBuilder();
            var datapoints = await _context.MeasPoints.Include(x=>x.Offsets).Include(x=>x.MeasType).Include(x=>x.MonitorType).Include(x => x.Measures).Where(x => x.ProjectId.Equals(model.projectNumber) && x.ToBeHidden.Equals(false)).OrderBy(x => x.MeasTypeId).ThenBy(x => x.Name).ToListAsync();
            
            var days = await (from m in _context.Measures
                              where m.MeasPoint.ProjectId.Equals(model.projectNumber)
                                    && m.When.Date >= model.startDate.Date && m.When.Date <= model.endDate.Date
                              select m.When.Date).GroupBy(x => x.Date).ToListAsync();
            var daycount = (model.endDate.Date - model.startDate.Date).TotalDays;
            int colcount = 0;
            if (model.AllDays.Equals(false))
            {
                colcount = days.Count() + 1;
            }
            else
            {
                colcount = Convert.ToInt32(daycount) + 2;
            }
            List<string> dayrow = new List<string>(new string[colcount]);
            dayrow[0] = "Name";
            if (model.AllDays.Equals(false))
            {
                for (int i = 1; i < colcount; i++)
                {
                    dayrow[i] = days[i-1].First().ToString();
                }
            }
            else
            {
                for (int i = 1; i < colcount; i++)
                {
                    dayrow[i] = model.startDate.Date.AddDays(i - 1).ToString();
                }
            }
            List<double> pumpsumrow = new List<double>(new double[colcount]);
            List<double> reinfsumrow = new List<double>(new double[colcount]);
            List<double> pumpflowrow = new List<double>(new double[colcount]);
            List<double> reinfflowrow = new List<double>(new double[colcount]);
            sb.AppendLine(string.Join(model.Delimiter, dayrow.ToArray()));
            foreach (MeasPoint mp in datapoints)
            {
                List<string> fillerrow = new List<string>(new string[colcount]);
                List<string> fillerrow_flow = new List<string>(new string[colcount]);
                fillerrow[0] = mp.Name;
                fillerrow_flow[0] = mp.Name + " flowrate";
                int count = 0;
                var meastotal = mp.Measures.Where(x => x.TheMeasurement != null && x.When.Date >= days.First().First()).ToList();
                MeasPoint potentialpumpMP = null;
                MeasPoint potentialreinfMP = null;
                if(mp.MonitorType.MonitorTypeName.ToLower().Equals("flow meter"))
                {
                    potentialpumpMP = datapoints.Where(x => x.Name.Contains(mp.Name.Split(" ")[0]) && x.MonitorType.MonitorTypeName.ToLower().Equals("pumping well")).FirstOrDefault();
                    potentialreinfMP = datapoints.Where(x => x.Name.Contains(mp.Name.Split(" ")[0]) && x.MonitorType.MonitorTypeName.ToLower().Equals("reinfiltration well")).FirstOrDefault();
                }
                if (meastotal.Count > 0) {
                    if (model.AllDays.Equals(false)) { 
                        foreach (var d in days)
                        {
                            var meas = mp.Measures.Where(x => x.TheMeasurement != null && x.When.Date.Equals(d.First().Date)).ToList();
                            var offsets = mp.Offsets.Where(x => x.starttime.Date <= d.First().Date).OrderByDescending(x => x.starttime).ToList();
                            var offset = offsets.FirstOrDefault();
                            if (meas.Count() > 0)
                            {
                                if (mp.MeasType.Type.ToLower().Equals("water meter"))
                                {
                                    if(offsets.Count() < 1)
                                    {
                                        double average = meas.Average(x => Convert.ToDouble(x.TheMeasurement));
                                        fillerrow[count + 1] = String.Format("{0:n2}", average);
                                        if(potentialpumpMP != null)
                                        {
                                            pumpsumrow[count + 1] += average;
                                        }
                                        else if(potentialreinfMP != null)
                                        {
                                            reinfsumrow[count + 1] += average;
                                        }
                                        var lastmeas = meastotal.Where(x => x.When < d.First().Date).OrderByDescending(x => x.When).FirstOrDefault();
                                        if(lastmeas != null)
                                        {
                                            var hourdiff = (d.First().Date - lastmeas.When.Date).TotalHours;
                                            var measdiff = average - Convert.ToDouble(lastmeas.TheMeasurement);
                                            fillerrow_flow[count + 1] = String.Format("{0:n2}", measdiff / hourdiff);
                                            if (potentialpumpMP != null)
                                            {
                                                pumpflowrow[count + 1] += measdiff / hourdiff;
                                            }
                                            else if (potentialreinfMP != null)
                                            {
                                                reinfflowrow[count + 1] += measdiff / hourdiff;
                                            }
                                        }
                                    }
                                    else if(offsets.Count < 2)
                                    {
                                        double average = meas.Average(x => Convert.ToDouble(x.TheMeasurement));
                                        fillerrow[count + 1] = String.Format("{0:n2}", average - offset.offset);
                                        if (potentialpumpMP != null)
                                        {
                                            pumpsumrow[count + 1] += average - offset.offset;
                                        }
                                        else if (potentialreinfMP != null)
                                        {
                                            reinfsumrow[count + 1] += average - offset.offset;
                                        }
                                        var lastmeas = meastotal.Where(x => x.When < d.First().Date).OrderByDescending(x => x.When).FirstOrDefault();
                                        if (lastmeas != null)
                                        {
                                            var hourdiff = (d.First().Date - lastmeas.When.Date).TotalHours;
                                            var measdiff = (average - offset.offset) - (Convert.ToDouble(lastmeas.TheMeasurement) - offset.offset);
                                            fillerrow_flow[count + 1] = String.Format("{0:n2}", measdiff / hourdiff);
                                            if (potentialpumpMP != null)
                                            {
                                                pumpflowrow[count + 1] += measdiff / hourdiff;
                                            }
                                            else if (potentialreinfMP != null)
                                            {
                                                reinfflowrow[count + 1] += measdiff / hourdiff;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        List<double> wmmeas = new List<double>();
                                        foreach (Meas m_wm in meas)
                                        {
                                            double themeas = await getWatermeterTrueMeas(m_wm);
                                            wmmeas.Add(themeas);

                                        }
                                        double averagevm = wmmeas.Average();
                                        fillerrow[count + 1] = String.Format("{0:n2}", averagevm);
                                        if (potentialpumpMP != null)
                                        {
                                            pumpsumrow[count + 1] += averagevm;
                                        }
                                        else if (potentialreinfMP != null)
                                        {
                                            reinfsumrow[count + 1] += averagevm;
                                        }
                                        var lastmeas = meastotal.Where(x => x.When < d.First().Date).OrderByDescending(x => x.When).FirstOrDefault();
                                        if (lastmeas != null)
                                        {
                                            var hourdiff = (d.First().Date - lastmeas.When.Date).TotalHours;
                                            var measdiff = (averagevm - await getWatermeterTrueMeas(lastmeas));
                                            fillerrow_flow[count + 1] = String.Format("{0:n2}", measdiff / hourdiff);
                                            if (potentialpumpMP != null)
                                            {
                                                pumpflowrow[count + 1] += measdiff / hourdiff;
                                            }
                                            else if (potentialreinfMP != null)
                                            {
                                                reinfflowrow[count + 1] += measdiff / hourdiff;
                                            }
                                        }
                                    }
                            
                                }
                                else if (mp.MeasTypeId.Equals(113))
                                {
                                        if (offsets.Count() < 1)
                                        {
                                            double average = meas.Average(x => Convert.ToDouble(x.TheMeasurement));
                                            fillerrow[count + 1] = String.Format("{0:n2}", average*10.0);
                                            if (potentialpumpMP != null)
                                            {
                                                pumpsumrow[count + 1] += average*10;
                                            }
                                            else if (potentialreinfMP != null)
                                            {
                                                reinfsumrow[count + 1] += average*10;
                                            }
                                            var lastmeas = meastotal.Where(x => x.When < d.First().Date).OrderByDescending(x => x.When).FirstOrDefault();
                                            if (lastmeas != null)
                                            {
                                                var hourdiff = (d.First().Date - lastmeas.When.Date).TotalHours;
                                                var measdiff = average*10.0 - Convert.ToDouble(lastmeas.TheMeasurement)*10.0;
                                                fillerrow_flow[count + 1] = String.Format("{0:n2}", measdiff / hourdiff);
                                                if (potentialpumpMP != null)
                                                {
                                                    pumpflowrow[count + 1] += measdiff / hourdiff;
                                                }
                                                else if (potentialreinfMP != null)
                                                {
                                                    reinfflowrow[count + 1] += measdiff / hourdiff;
                                                }
                                            }
                                        }
                                        else if (offsets.Count < 2)
                                        {
                                            double average = meas.Average(x => Convert.ToDouble(x.TheMeasurement));
                                            fillerrow[count + 1] = String.Format("{0:n2}", average * 10 - offset.offset);
                                            if (potentialpumpMP != null)
                                            {
                                                pumpsumrow[count + 1] += (average * 10 - offset.offset);
                                            }
                                            else if (potentialreinfMP != null)
                                            {
                                                reinfsumrow[count + 1] += (average * 10 - offset.offset)*10;
                                            }
                                            var lastmeas = meastotal.Where(x => x.When < d.First().Date).OrderByDescending(x => x.When).FirstOrDefault();
                                            if (lastmeas != null)
                                            {
                                                var hourdiff = (d.First().Date - lastmeas.When.Date).TotalHours;
                                                var measdiff = (average * 10 - offset.offset) - (Convert.ToDouble(lastmeas.TheMeasurement) * 10 - offset.offset);
                                                fillerrow_flow[count + 1] = String.Format("{0:n2}", measdiff / hourdiff);
                                                if (potentialpumpMP != null)
                                                {
                                                    pumpflowrow[count + 1] += measdiff / hourdiff;
                                                }
                                                else if (potentialreinfMP != null)
                                                {
                                                    reinfflowrow[count + 1] += measdiff / hourdiff;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            List<double> wmmeas = new List<double>();
                                            foreach (Meas m_wm in meas)
                                            {
                                                double themeas = await getWatermeterTrueMeas(m_wm) * 10;
                                                wmmeas.Add(themeas);

                                            }
                                            double averagevm = wmmeas.Average();
                                            fillerrow[count + 1] = String.Format("{0:n2}", averagevm * 10);
                                            if (potentialpumpMP != null)
                                            {
                                                pumpsumrow[count + 1] += averagevm * 10;
                                            }
                                            else if (potentialreinfMP != null)
                                            {
                                                reinfsumrow[count + 1] += averagevm * 10;
                                            }
                                            var lastmeas = meastotal.Where(x => x.When < d.First().Date).OrderByDescending(x => x.When).FirstOrDefault();
                                            if (lastmeas != null)
                                            {
                                                var hourdiff = (d.First().Date - lastmeas.When.Date).TotalHours;
                                                var measdiff = (averagevm * 10 - await getWatermeterTrueMeas(lastmeas) * 10);
                                                fillerrow_flow[count + 1] = String.Format("{0:n2}", measdiff / hourdiff);
                                                if (potentialpumpMP != null)
                                                {
                                                    pumpflowrow[count + 1] += measdiff / hourdiff;
                                                }
                                                else if (potentialreinfMP != null)
                                                {
                                                    reinfflowrow[count + 1] += measdiff / hourdiff;
                                                }
                                            }
                                        }
                                    }
                                else if (mp.MeasTypeId.Equals(114))
                                {
                                    if (offsets.Count() < 1)
                                    {
                                        double average = meas.Average(x => Convert.ToDouble(x.TheMeasurement));
                                        fillerrow[count + 1] = String.Format("{0:n2}", average * 100.0);
                                        if (potentialpumpMP != null)
                                        {
                                            pumpsumrow[count + 1] += average * 100.0;
                                        }
                                        else if (potentialreinfMP != null)
                                        {
                                            reinfsumrow[count + 1] += average * 100.0;
                                        }
                                        var lastmeas = meastotal.Where(x => x.When < d.First().Date).OrderByDescending(x => x.When).FirstOrDefault();
                                        if (lastmeas != null)
                                        {
                                            var hourdiff = (d.First().Date - lastmeas.When.Date).TotalHours;
                                            var measdiff = average * 100.0 - Convert.ToDouble(lastmeas.TheMeasurement) * 100.0;
                                            fillerrow_flow[count + 1] = String.Format("{0:n2}", measdiff / hourdiff);
                                            if (potentialpumpMP != null)
                                            {
                                                pumpflowrow[count + 1] += measdiff / hourdiff;
                                            }
                                            else if (potentialreinfMP != null)
                                            {
                                                reinfflowrow[count + 1] += measdiff / hourdiff;
                                            }
                                        }
                                    }
                                    else if (offsets.Count < 2)
                                    {
                                        double average = meas.Average(x => Convert.ToDouble(x.TheMeasurement));
                                        fillerrow[count + 1] = String.Format("{0:n2}", average * 100.0 - offset.offset);
                                        if (potentialpumpMP != null)
                                        {
                                            pumpsumrow[count + 1] += (average * 100.0 - offset.offset);
                                        }
                                        else if (potentialreinfMP != null)
                                        {
                                            reinfsumrow[count + 1] += (average * 100.0 - offset.offset) * 100.0;
                                        }
                                        var lastmeas = meastotal.Where(x => x.When < d.First().Date).OrderByDescending(x => x.When).FirstOrDefault();
                                        if (lastmeas != null)
                                        {
                                            var hourdiff = (d.First().Date - lastmeas.When.Date).TotalHours;
                                            var measdiff = (average * 100.0 - offset.offset) - (Convert.ToDouble(lastmeas.TheMeasurement) * 100.0 - offset.offset);
                                            fillerrow_flow[count + 1] = String.Format("{0:n2}", measdiff / hourdiff);
                                            if (potentialpumpMP != null)
                                            {
                                                pumpflowrow[count + 1] += measdiff / hourdiff;
                                            }
                                            else if (potentialreinfMP != null)
                                            {
                                                reinfflowrow[count + 1] += measdiff / hourdiff;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        List<double> wmmeas = new List<double>();
                                        foreach (Meas m_wm in meas)
                                        {
                                            double themeas = await getWatermeterTrueMeas(m_wm) * 100.0;
                                            wmmeas.Add(themeas);

                                        }
                                        double averagevm = wmmeas.Average();
                                        fillerrow[count + 1] = String.Format("{0:n2}", averagevm * 100.0);
                                        if (potentialpumpMP != null)
                                        {
                                            pumpsumrow[count + 1] += averagevm * 100.0;
                                        }
                                        else if (potentialreinfMP != null)
                                        {
                                            reinfsumrow[count + 1] += averagevm * 100.0;
                                        }
                                        var lastmeas = meastotal.Where(x => x.When < d.First().Date).OrderByDescending(x => x.When).FirstOrDefault();
                                        if (lastmeas != null)
                                        {
                                            var hourdiff = (d.First().Date - lastmeas.When.Date).TotalHours;
                                            var measdiff = (averagevm * 100.0 - await getWatermeterTrueMeas(lastmeas) * 100.0);
                                            fillerrow_flow[count + 1] = String.Format("{0:n2}", measdiff / hourdiff);
                                            if (potentialpumpMP != null)
                                            {
                                                pumpflowrow[count + 1] += measdiff / hourdiff;
                                            }
                                            else if (potentialreinfMP != null)
                                            {
                                                reinfflowrow[count + 1] += measdiff / hourdiff;
                                            }
                                        }
                                    }
                                }
                                else if(mp.MeasType.Type.ToLower().Equals("water level"))
                                {
                                    double average = meas.Average(x => Convert.ToDouble(x.TheMeasurement));
                                    if (offset != null)
                                    {
                                        fillerrow[count + 1] = String.Format("{0:n2}", offset.offset - average);
                                    }
                                    else
                                    {
                                        fillerrow[count + 1] = String.Format("{0:n2}", average);
                                    }
                                }
                                else
                                {
                                    double average = meas.Average(x => Convert.ToDouble(x.TheMeasurement));
                                    fillerrow[count + 1] = String.Format("{0:n2}", average);  
                                }
                        
                            }
                            count += 1;
                        }
                        sb.AppendLine(string.Join(model.Delimiter, fillerrow.ToArray()));
                        if(mp.MeasType.Type.ToLower().Equals("water meter"))
                        {
                            sb.AppendLine(string.Join(model.Delimiter, fillerrow_flow.ToArray()));
                        }
                    }
                    else
                    {
                        for (DateTime d = model.startDate.Date; d <= model.endDate.Date; d = d.AddDays(1).Date)
                        {
                            var meas = mp.Measures.Where(x => x.TheMeasurement != null && x.When.Date.Equals(d.Date)).ToList();
                            var offsets = mp.Offsets.Where(x => x.starttime.Date <= d.Date).OrderByDescending(x => x.starttime).ToList();
                            var offset = offsets.FirstOrDefault();
                            System.Console.WriteLine(d.ToString());
                            if (meas.Count() > 0)
                            {
                                if (mp.MeasType.Type.ToLower().Equals("water meter"))
                                {
                                    if (offsets.Count() < 1)
                                    {
                                        double average = meas.Average(x => Convert.ToDouble(x.TheMeasurement));
                                        fillerrow[count + 1] = String.Format("{0:n2}", average);
                                        if (potentialpumpMP != null)
                                        {
                                            pumpsumrow[count + 1] += average;
                                        }
                                        else if (potentialreinfMP != null)
                                        {
                                            reinfsumrow[count + 1] += average;
                                        }
                                        var lastmeas = meastotal.Where(x => x.When < d.Date).OrderByDescending(x => x.When).FirstOrDefault();
                                        if (lastmeas != null)
                                        {
                                            var hourdiff = (d.Date - lastmeas.When.Date).TotalHours;
                                            var measdiff = average - Convert.ToDouble(lastmeas.TheMeasurement);
                                            fillerrow_flow[count + 1] = String.Format("{0:n2}", measdiff / hourdiff);
                                            if (potentialpumpMP != null)
                                            {
                                                pumpflowrow[count + 1] += measdiff / hourdiff;
                                            }
                                            else if (potentialreinfMP != null)
                                            {
                                                reinfflowrow[count + 1] += measdiff / hourdiff;
                                            }
                                        }
                                    }
                                    else if (offsets.Count < 2)
                                    {
                                        double average = meas.Average(x => Convert.ToDouble(x.TheMeasurement));
                                        fillerrow[count + 1] = String.Format("{0:n2}", average - offset.offset);
                                        if (potentialpumpMP != null)
                                        {
                                            pumpsumrow[count + 1] += average - offset.offset;
                                        }
                                        else if (potentialreinfMP != null)
                                        {
                                            reinfsumrow[count + 1] += average - offset.offset;
                                        }
                                        var lastmeas = meastotal.Where(x => x.When < d.Date).OrderByDescending(x => x.When).FirstOrDefault();
                                        if (lastmeas != null)
                                        {
                                            var hourdiff = (d.Date - lastmeas.When.Date).TotalHours;
                                            var measdiff = (average - offset.offset) - (Convert.ToDouble(lastmeas.TheMeasurement) - offset.offset);
                                            fillerrow_flow[count + 1] = String.Format("{0:n2}", measdiff / hourdiff);
                                            if (potentialpumpMP != null)
                                            {
                                                pumpflowrow[count + 1] += measdiff / hourdiff;
                                            }
                                            else if (potentialreinfMP != null)
                                            {
                                                reinfflowrow[count + 1] += measdiff / hourdiff;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        List<double> wmmeas = new List<double>();
                                        foreach (Meas m_wm in meas)
                                        {
                                            double themeas = await getWatermeterTrueMeas(m_wm);
                                            wmmeas.Add(themeas);

                                        }
                                        double averagevm = wmmeas.Average();
                                        fillerrow[count + 1] = String.Format("{0:n2}", averagevm);
                                        if (potentialpumpMP != null)
                                        {
                                            pumpsumrow[count + 1] += averagevm;
                                        }
                                        else if (potentialreinfMP != null)
                                        {
                                            reinfsumrow[count + 1] += averagevm;
                                        }
                                        var lastmeas = meastotal.Where(x => x.When < d.Date).OrderByDescending(x => x.When).FirstOrDefault();
                                        if (lastmeas != null)
                                        {
                                            var hourdiff = (d.Date - lastmeas.When.Date).TotalHours;
                                            var measdiff = (averagevm - await getWatermeterTrueMeas(lastmeas));
                                            fillerrow_flow[count + 1] = String.Format("{0:n2}", measdiff / hourdiff);
                                            if (potentialpumpMP != null)
                                            {
                                                pumpflowrow[count + 1] += measdiff / hourdiff;
                                            }
                                            else if (potentialreinfMP != null)
                                            {
                                                reinfflowrow[count + 1] += measdiff / hourdiff;
                                            }
                                        }
                                    }

                                }
                                else if (mp.MeasType.Type.ToLower().Equals("water level"))
                                {
                                    double average = meas.Average(x => Convert.ToDouble(x.TheMeasurement));
                                    if (offset != null)
                                    {
                                        System.Console.WriteLine(count.ToString());
                                        fillerrow[count + 1] = String.Format("{0:n2}", offset.offset - average);
                                    }
                                    else
                                    {
                                        fillerrow[count + 1] = String.Format("{0:n2}", average);
                                    }
                                }
                                else
                                {
                                    double average = meas.Average(x => Convert.ToDouble(x.TheMeasurement));
                                    fillerrow[count + 1] = String.Format("{0:n2}", average);
                                }

                            }
                            count += 1;
                        }
                        sb.AppendLine(string.Join(model.Delimiter, fillerrow.ToArray()));
                        if (mp.MeasType.Type.ToLower().Equals("water meter"))
                        {
                            sb.AppendLine(string.Join(model.Delimiter, fillerrow_flow.ToArray()));
                        }
                    }
                }
            }
            //add calculations to report:
            List<string> pumpsumrowstring = new List<string>(new string[colcount]);
            List<string> reinfsumrowstring = new List<string>(new string[colcount]);
            List<string> pumpflowrowstring = new List<string>(new string[colcount]);
            List<string> reinfflowrowstring = new List<string>(new string[colcount]);
            if (pumpsumrow.Sum() > 0.01)
            {
                for(int i = 1; i<pumpsumrow.Count();i++)
                {
                    if(pumpsumrow[i] < 0.01)
                    {
                        pumpsumrowstring[i] = "";
                    }
                    else
                    {
                        pumpsumrowstring[i] = String.Format("{0:n2}", pumpsumrow[i]);
                    }
                }
                pumpsumrowstring[0] = "Total Pumped";
                sb.AppendLine(string.Join(model.Delimiter, pumpsumrowstring.ToArray()));
            }
            if (pumpflowrow.Sum() > 0.01)
            {
                for (int i = 1; i < pumpflowrow.Count(); i++)
                {
                    if(pumpflowrow[i] < 0.01)
                    {
                        pumpflowrowstring[i] = "";
                    }
                    else
                    {
                        pumpflowrowstring[i] = String.Format("{0:n2}", pumpflowrow[i]);
                    }
                    
                }
                pumpflowrowstring[0] = "Total Pumped Flowrate";
                sb.AppendLine(string.Join(model.Delimiter, pumpflowrowstring.ToArray()));
            }
            if (reinfsumrow.Sum() > 0.01)
            {
                for (int i = 1; i < reinfsumrow.Count(); i++)
                {
                    if (reinfsumrow[i] < 0.01)
                    {
                        reinfsumrowstring[i] = "";
                    }
                    else
                    {
                        reinfsumrowstring[i] = String.Format("{0:n2}", reinfsumrow[i]);
                    }
                }
                reinfsumrowstring[0] = "Total Reinfiltrated";
                sb.AppendLine(string.Join(model.Delimiter, reinfsumrowstring.ToArray()));
            }
            if (reinfflowrow.Sum() > 0.01)
            {
                for (int i = 1; i < reinfflowrow.Count(); i++)
                {
                    if(reinfflowrow[i] < 0.01)
                    {
                        reinfflowrowstring[i] = "";
                    }
                    else
                    {
                        reinfflowrowstring[i] = String.Format("{0:n2}", reinfflowrow[i]);
                    }
                    
                }
                reinfflowrowstring[0] = "Total Reinfiltrated Flowrate";
                sb.AppendLine(string.Join(model.Delimiter, reinfflowrowstring.ToArray()));
            }
            return File(System.Text.Encoding.ASCII.GetBytes(sb.ToString()), "text/csv", "Report.csv");
        }

    }
}
