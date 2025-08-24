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
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing;
using ImageMagick;

namespace MainOps.Controllers
{
    [Authorize]
    public class OffsetsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public OffsetsController(DataContext context,UserManager<ApplicationUser> userManager, IWebHostEnvironment env):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
            _env = env;

        }

        // GET: Offsets
        [Authorize(Roles = ("Admin,DivisionAdmin,Guest,MemberGuest,Member"))]
        public async Task<IActionResult> Index()
        {
            IEnumerable<SelectListItem> selList = await createFilterlist();
            ViewData["Filterchoices"] = new SelectList(selList, "Value", "Text");
            var theuser = await _userManager.GetUserAsync(HttpContext.User);
            if (User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
            {
                
                var data = await (from o in _context.Offsets.Include(o => o.measpoint).ThenInclude(x => x.Project)
                                  join pu in _context.ProjectUsers on o.measpoint.ProjectId
                                                    equals pu.projectId
                                  where pu.userId == theuser.Id && o.measpoint.Project.Active.Equals(true)
                                  select o).OrderBy(x => x.measpoint.ProjectId).ThenBy(x => x.measpoint.Name).ThenBy(x => x.starttime).Take(100).ToListAsync();
                
                return View(data);

            }
            if (User.IsInRole("Admin"))
            {
                var dataContextAdmin = _context.Offsets.Include(o => o.measpoint).ThenInclude(x => x.Project)
                    .Where(x => x.measpoint.Project.Active.Equals(true))
                .OrderBy(x => x.measpoint.ProjectId).ThenBy(x => x.measpoint.Name).ThenBy(x => x.starttime).Take(100);
                return View(await dataContextAdmin.ToListAsync());
            }
            var dataContext = _context.Offsets.Include(o => o.measpoint).ThenInclude(x=>x.Project)
                .Where(x=>x.measpoint.Project.DivisionId.Equals(theuser.DivisionId) && x.measpoint.Project.Active.Equals(true)).OrderBy(x=>x.measpoint.ProjectId).ThenBy(x=>x.measpoint.Name).ThenBy(x=>x.starttime).Take(100);
            return View(await dataContext.ToListAsync());
        }
        [HttpGet]
        public async Task<IActionResult> PipeCut(int Id)
        {
                var Offset = await _context.Offsets.Include(x => x.measpoint).ThenInclude(x => x.Project).Where(x => x.Id.Equals(Id)).FirstAsync();
                PipeCutViewModel model = new PipeCutViewModel(Offset.Id, Offset);
                return View("PipeCut", model);
        }
        [HttpGet]
        public async Task<IActionResult> PipeCut2()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Include(x=>x.MeasType).Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId) && x.MeasType.Type.ToLower().Equals("water level")), "Id", "Name");
            PipeCutViewModel2 model = new PipeCutViewModel2();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> PipeCut(PipeCutViewModel model)
        {
            //!!!!!!!! LINK THIS TO INSTALLATION!!!!!!!
            var user = await _userManager.GetUserAsync(User);
            Offset new_offset = new Offset();
            var old_offset = await _context.Offsets.FindAsync(model.OffsetId);
            new_offset.starttime = model.start_time;
            new_offset.offset = old_offset.offset - model.meters_cut;
            new_offset.MeasPointId = old_offset.MeasPointId;
            _context.Offsets.Add(new_offset);
            await _context.SaveChangesAsync();
            var mp = await _context.MeasPoints.Include(x => x.Offsets).SingleOrDefaultAsync(x => x.Id.Equals(old_offset.MeasPointId));
            if (mp.Offsets.Count() > 1)
            {
                var newerOffsets = mp.Offsets.Where(x => x.starttime > new_offset.starttime).ToList();
                if (newerOffsets.Count() > 0)
                {
                    // newer offset exists.
                }
                else
                {
                    mp.Offset = new_offset.offset;
                    _context.Update(mp);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                mp.Offset = new_offset.offset;
                _context.Update(mp);
                await _context.SaveChangesAsync();
            }
            var itemtype = await _context.ItemTypes.Where(x => x.ProjectId.Equals(mp.ProjectId) && x.ReportTypeId.Equals(17)).SingleOrDefaultAsync();
            if(itemtype != null)
            {
                Install inst = new Install();
                inst.TimeStamp = model.start_time;
                inst.ProjectId = mp.ProjectId;
                inst.SubProjectId = mp.SubProjectId;
                inst.RentalStartDate = model.start_time;
                inst.InvoiceDate = DateTime.Now;
                inst.Install_Text = "Automatic Installation Report of Pipe Cut/Extension of" + model.meters_cut.ToString("F") + " meters";
                inst.isInstalled = false;
                inst.DeinstallDate = model.start_time;
                inst.EnteredIntoDataBase = DateTime.Now;
                inst.LastEditedInDataBase = DateTime.Now;
                inst.ItemTypeId = itemtype.Id;
                inst.Latitude = 0;
                inst.Longitude = 0;
                inst.UniqueID = mp.Name;
                inst.PayedAmount = 0;
                inst.DoneBy = user.full_name();
                inst.Amount = 1;
                inst.ToBePaid = true;
                _context.Add(inst);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MainMenu","TrackItems");
        }
        [HttpPost]
        public async Task<IActionResult> PipeCut2(PipeCutViewModel2 model,IFormFile[] files)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var offset = await _context.Offsets.Where(x => x.MeasPointId.Equals(model.MeasPointId)).OrderByDescending(x=>x.starttime).FirstAsync();
                Offset new_offset = new Offset();
                new_offset.starttime = model.start_time;
                new_offset.MeasPointId = model.MeasPointId;
                new_offset.offset = offset.offset - model.meters_cut;
                _context.Offsets.Add(new_offset);
                await _context.SaveChangesAsync();
                var mp = await _context.MeasPoints.Include(x => x.Offsets).SingleOrDefaultAsync(x => x.Id.Equals(offset.MeasPointId));
                if (mp.Offsets.Count() > 1)
                {
                    var newerOffsets = mp.Offsets.Where(x => x.starttime > new_offset.starttime).ToList();
                    if (newerOffsets.Count() > 0)
                    {
                        // newer offset exists.
                    }
                    else
                    {
                        mp.Offset = new_offset.offset;
                        _context.Update(mp);
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    mp.Offset = new_offset.offset;
                    _context.Update(mp);
                    await _context.SaveChangesAsync();
                }
                //add report
                PipeCut PipeCut = new PipeCut();
                PipeCut.DoneBy = user.full_name();
                PipeCut.MeasPointId = new_offset.MeasPointId;
                PipeCut.Meters_Cut = model.meters_cut;
                PipeCut.TimeStamp = model.start_time;
                var itemtype = await _context.ItemTypes.FirstOrDefaultAsync(x => x.ProjectId.Equals(mp.ProjectId) && x.ReportTypeId.Equals(17));
                if(itemtype != null)
                {
                    Install inst = new Install();
                    inst.TimeStamp = model.start_time;
                    inst.ProjectId = mp.ProjectId;
                    inst.SubProjectId = mp.SubProjectId;
                    inst.RentalStartDate = model.start_time;
                    inst.InvoiceDate = DateTime.Now;
                    inst.Install_Text = "Automatic Installation Report of Pipe Cut/Extension of" + model.meters_cut.ToString("F") + " meters";
                    inst.isInstalled = false;
                    inst.DeinstallDate = model.start_time;
                    inst.EnteredIntoDataBase = DateTime.Now;
                    inst.LastEditedInDataBase = DateTime.Now;
                    inst.ItemTypeId = itemtype.Id;
                    inst.Latitude = 0;
                    inst.Longitude = 0;
                    inst.UniqueID = mp.Name;
                    inst.PayedAmount = 0;
                    inst.DoneBy = user.full_name();
                    inst.Amount = 1;
                    inst.ToBePaid = true;
                    _context.Add(inst);
                }
                if (model.meters_cut > 0)
                {
                    PipeCut.Cut_Or_Extended = "Cut";
                }
                else
                {
                    PipeCut.Cut_Or_Extended = "Extended";
                }
                _context.PipeCuts.Add(PipeCut);
                await _context.SaveChangesAsync();
                if (files != null)
                {
                    var itemadded = await _context.PipeCuts.LastAsync();
                    var directory = _env.WebRootPath + "\\AHAK\\PipeCutPhotos\\" + itemadded.Id.ToString() + "\\";
                    if (!Directory.Exists(directory) && files != null)
                    {
                        Directory.CreateDirectory(directory);
                    }
                    foreach (IFormFile photo in files)
                    {
                        var path = Path.Combine(directory, photo.FileName);
                        var path2 = Path.Combine(directory, photo.FileName.Split(".")[0] + "_edit." + photo.FileName.Split(".")[1]);
                        PipeCutPhoto cutphoto = new PipeCutPhoto { Path = path, TimeStamp = model.start_time, PipeCutId = itemadded.Id };
                        _context.Add(cutphoto);
                        var stream = new FileStream(path, FileMode.Create);
                        await photo.CopyToAsync(stream);
                        stream.Close();
                        if (path.Contains(".jpg") || path.Contains(".jpeg"))
                        {
                            SaveAndCompressJpeg(path, 85);
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("MainMenu", "TrackItems");
            }
            else { return NotFound(); }
        }
        // GET: Offsets/Details/5
        [Authorize(Roles = ("Admin,DivisionAdmin,Guest,MemberGuest,Member"))]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offset = await _context.Offsets
                .Include(o => o.measpoint)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (offset == null)
            {
                return NotFound();
            }

            return View(offset);
        }

        // GET: Offsets/Create
        [Authorize(Roles = "Admin,Member,DivisionAdmin")]
        public async Task<IActionResult> Create()
        {

            List<SelectListItem> selList = await createMeasPointList();
            ViewData["MeasPointId"] = selList;
            return View();
        }

        // POST: Offsets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin,Member,DivisionAdmin")]
        public async Task<IActionResult> Create([Bind("Id,offset,MeasPointId,starttime")] Offset _off)
        {
            if (ModelState.IsValid)
            {
                _context.Add(_off);
                await _context.SaveChangesAsync();
                var mp = await _context.MeasPoints.Include(x => x.Offsets).SingleOrDefaultAsync(x => x.Id.Equals(_off.MeasPointId)); 
                if(mp.Offsets.Count() > 1)
                {
                    var newerOffsets = mp.Offsets.Where(x => x.starttime > _off.starttime).ToList(); 
                    if(newerOffsets.Count() > 0)
                    {
                        // newer offset exists.
                    }
                    else
                    {
                        mp.Offset = _off.offset;
                        _context.Update(mp);
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    mp.Offset = _off.offset;
                    _context.Update(mp);
                    await _context.SaveChangesAsync();
                }
                                  
                return RedirectToAction(nameof(Index));
            }
            List<SelectListItem> selList = await createMeasPointList();
            ViewData["MeasPointId"] = selList;

            return View(_off);
        }

        // GET: Offsets/Edit/5
        [Authorize(Roles ="Admin,Member,DivisionAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offset = await _context.Offsets.FindAsync(id);
            if (offset == null)
            {
                return NotFound();
            }
            List<SelectListItem> selList = await createMeasPointList();
            ViewData["MeasPointId"] = selList;

            return View(offset);
        }

        // POST: Offsets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Member,DivisionAdmin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,offset,MeasPointId,starttime")] Offset _off)
        {
            if (id != _off.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(_off);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OffsetExists(_off.Id))
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
            List<SelectListItem> selList = await createMeasPointList();
            ViewData["MeasPointId"] = selList;
            return View(_off);
        }
        [Authorize(Roles = ("Admin,DivisionAdmin,Guest,MemberGuest,Member"))]
        public async Task<List<SelectListItem>> createMeasPointList()
        {
            var theuser = await _userManager.GetUserAsync(User);
            List<Project> groups = new List<Project>();
            if (User.IsInRole("Admin"))
            {
                groups = await _context.Projects.OrderBy(x=>x.Name).ToListAsync();
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
                monpoints = await _context.MeasPoints.Include(x => x.Project)
                .Where(x => x.Project.DivisionId.Equals(theuser.DivisionId))
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
                var filternames2 = await _context.Projects.Include(x => x.Division).OrderBy(b => b.Name).ToListAsync();

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
        [HttpPost]
        [Authorize(Roles = ("Admin,DivisionAdmin,Member,Manager,Guest,MemberGuest"))]
        public async Task<IActionResult> Combsearch(string searchstring, string filterchoice)
        {
            int f_c_converted;
            f_c_converted = Convert.ToInt32(filterchoice);

            IEnumerable<SelectListItem> selList = await createFilterlist();
            ViewData["Filterchoices"] = new SelectList(selList, "Value", "Text");
            if (ModelState.IsValid)
            {
                var theuser = await _userManager.GetUserAsync(User);
                if (HttpContext.User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
                {
                    if (string.IsNullOrEmpty(searchstring) && (string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
                    {
                        var data = await (from m in _context.Offsets.Include(m => m.measpoint).ThenInclude(x => x.Project)
                                  .OrderByDescending(x => x.starttime)
                                          join pu in _context.ProjectUsers on m.measpoint.ProjectId
                                                            equals pu.projectId
                                          where pu.userId == theuser.Id && m.measpoint.Project.Active.Equals(true)
                                          select m).OrderBy(x => x.measpoint.Name).ThenBy(x => x.starttime).ToListAsync();
                        return View(nameof(Index), data);
                    }
                    else if (string.IsNullOrEmpty(searchstring) && (!string.IsNullOrEmpty(filterchoice) || !filterchoice.Equals("All")))
                    {
                        var data = await (from m in _context.Offsets.Include(m => m.measpoint).ThenInclude(x => x.Project).ThenInclude(x => x.Division)
                                 .OrderBy(x => x.starttime)
                                          join pu in _context.ProjectUsers on m.measpoint.ProjectId
                                          equals pu.projectId
                                          where pu.userId == theuser.Id && m.measpoint.ProjectId.Equals(f_c_converted) && theuser.DivisionId.Equals(m.measpoint.Project.Division.Id)
                                          select m).OrderBy(x => x.starttime).ThenBy(x => x.measpoint.Name).ToListAsync();
                        return View(nameof(Index), data);
                    }
                    else if (!string.IsNullOrEmpty(searchstring) && (string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
                    {
                        var data = await (from m in _context.Offsets.Include(m => m.measpoint).ThenInclude(x => x.Project).ThenInclude(x => x.Division)
                                 .OrderBy(x => x.starttime)
                                          join pu in _context.ProjectUsers on m.measpoint.ProjectId
                                          equals pu.projectId
                                          where pu.userId == theuser.Id && theuser.DivisionId.Equals(m.measpoint.Project.Division.Id) && m.measpoint.Name.ToLower().Contains(searchstring.ToLower())
                                          select m).OrderBy(x => x.starttime).ThenBy(x => x.measpoint.Name).ToListAsync();
                        return View(nameof(Index), data);
                    }
                    else
                    {
                        var data = await (from m in _context.Offsets.Include(m => m.measpoint).ThenInclude(x => x.Project).ThenInclude(x => x.Division)
                                 .OrderBy(x => x.starttime)
                                          join pu in _context.ProjectUsers on m.measpoint.ProjectId
                                          equals pu.projectId
                                          where pu.userId == theuser.Id && theuser.DivisionId.Equals(m.measpoint.Project.Division.Id) && m.measpoint.Name.ToLower().Contains(searchstring.ToLower()) && m.measpoint.ProjectId.Equals(f_c_converted)
                                          select m).OrderBy(x => x.starttime).ThenBy(x => x.measpoint.Name).ToListAsync();
                        return View(nameof(Index), data);
                    }


                }
                if (string.IsNullOrEmpty(searchstring) && (string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
                {
                    if (User.IsInRole("Admin"))
                    {
                        var dataC = await _context.Offsets
                        .Include(m => m.measpoint).ThenInclude(x => x.Project).ThenInclude(x => x.Division)
                        .Where(x => x.measpoint.Project.Active.Equals(true))
                        .OrderBy(x => x.starttime).ThenBy(x => x.measpoint.Name).ToListAsync();
                        return View(nameof(Index), dataC);
                    }
                    var data = await _context.Offsets
                        .Include(m => m.measpoint).ThenInclude(x => x.Project).ThenInclude(x => x.Division)
                        .Where(x => x.measpoint.Project.Division.Id.Equals(theuser.DivisionId) && x.measpoint.Project.Active.Equals(true))
                        .OrderBy(x => x.starttime).ThenBy(x => x.measpoint.Name).Take(100).ToListAsync();
                    return View(nameof(Index), data);
                }
                else if (string.IsNullOrEmpty(searchstring) && (!string.IsNullOrEmpty(filterchoice) || !filterchoice.Equals("All")))
                {
                    if (User.IsInRole("Admin"))
                    {
                        var data = await _context.Offsets
                        .Include(x => x.measpoint).ThenInclude(x => x.Project).ThenInclude(x => x.Division)
                        .Where(b => b.measpoint.ProjectId.Equals(f_c_converted))
                        .OrderBy(x => x.starttime).ThenBy(x => x.measpoint.Name).ToListAsync();
                        return View(nameof(Index), data);
                    }
                    else
                    {
                        var data = await _context.Offsets
                        .Include(x => x.measpoint).ThenInclude(x => x.Project).ThenInclude(x => x.Division)
                        .Where(b => b.measpoint.ProjectId.Equals(f_c_converted) && b.measpoint.Project.Division.Id.Equals(theuser.DivisionId))
                        .OrderBy(x => x.starttime).ThenBy(x => x.measpoint.Name).ToListAsync();
                        return View(nameof(Index), data);
                    }
                    
                }
                else if (!string.IsNullOrEmpty(searchstring) && (string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
                {
                    if (User.IsInRole("Admin"))
                    {
                        var wtp_blocks = await _context.Offsets
                        .Include(x => x.measpoint).ThenInclude(x => x.Project).ThenInclude(x => x.Division)
                        .Where(b => b.measpoint.Name.ToLower().Contains(searchstring.ToLower()))
                        .OrderBy(x => x.starttime).ThenBy(x => x.measpoint.Name).ToListAsync();
                        return View(nameof(Index), wtp_blocks);
                    }
                    else
                    {
                        var wtp_blocks = await _context.Offsets
                        .Include(x => x.measpoint).ThenInclude(x => x.Project).ThenInclude(x => x.Division)
                        .Where(b => b.measpoint.Name.ToLower().Contains(searchstring.ToLower()) && b.measpoint.Project.Division.Id.Equals(theuser.DivisionId))
                        .OrderBy(x => x.starttime).ThenBy(x => x.measpoint.Name).ToListAsync();
                        return View(nameof(Index), wtp_blocks);
                    }
                    
                }
                else
                {
                    if (User.IsInRole("Admin"))
                    {
                        var wtp_blocks = await _context.Offsets
                        .Include(x => x.measpoint).ThenInclude(x => x.Project).ThenInclude(x => x.Division)
                        .Where(b => b.measpoint.ProjectId.Equals(f_c_converted) && b.measpoint.Name.ToLower().Contains(searchstring.ToLower()))
                        .OrderBy(x => x.starttime).ThenBy(x => x.measpoint.Name).ToListAsync();
                        return View(nameof(Index), wtp_blocks);
                    }
                    else
                    {
                        var wtp_blocks = await _context.Offsets
                        .Include(x => x.measpoint).ThenInclude(x => x.Project).ThenInclude(x => x.Division)
                        .Where(b => b.measpoint.ProjectId.Equals(f_c_converted) && b.measpoint.Project.Division.Id.Equals(theuser.DivisionId) && b.measpoint.Name.ToLower().Contains(searchstring.ToLower()))
                        .OrderBy(x => x.starttime).ThenBy(x => x.measpoint.Name).ToListAsync();
                        return View(nameof(Index), wtp_blocks);
                    }
                    
                }


            }
            return View(nameof(Index));
        }
        [HttpGet]
        [Authorize(Roles = ("Admin,DivisionAdmin,Member,Manager,Guest,MemberGuest"))]
        public async Task<JsonResult> AutoComplete(string search)
        {
           
            var theuser = await _userManager.GetUserAsync(HttpContext.User);
            if (HttpContext.User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
            {

                var results = await (from o in _context.Offsets
                                     .Include(x => x.measpoint).ThenInclude(x => x.Project).ThenInclude(x => x.Division)
                                     join pu in _context.ProjectUsers on o.measpoint.ProjectId
                                     equals pu.projectId
                                     where pu.userId == theuser.Id && o.measpoint.Project.Division.Id.Equals(theuser.DivisionId) && (o.measpoint.Name.ToLower().Contains(search.ToLower()) || o.measpoint.Project.Name.ToLower().Contains(search.ToLower()))
                                     select o).ToListAsync();
                return Json(results.Select(m => new
                {
                    id = m.Id,
                    value = m.measpoint.Name,
                    label = m.measpoint.Project.Name + '_' + m.measpoint.Name
                }).OrderBy(x => x.label));
            }
            else
            {
                if (User.IsInRole("Admin"))
                {
                    var results = _context.Offsets
                    .Include(x => x.measpoint).ThenInclude(x => x.Project).ThenInclude(x => x.Division)
                    .Where(x => x.measpoint.Name.ToLower().Contains(search.ToLower())).ToList();
                    return Json(results.Select(m => new
                    {
                        id = m.Id,
                        value = m.measpoint.Name,
                        label = m.measpoint.Project.Name + '_' + m.measpoint.Name
                    }).OrderBy(x => x.label));
                }
                else
                {
                    var results = _context.Offsets
                    .Include(x => x.measpoint).ThenInclude(x => x.Project).ThenInclude(x => x.Division)
                    .Where(x => x.measpoint.Name.ToLower().Contains(search.ToLower()) && x.measpoint.Project.Division.Id.Equals(theuser.DivisionId)).ToList();
                    return Json(results.Select(m => new
                    {
                        id = m.Id,
                        value = m.measpoint.Name,
                        label = m.measpoint.Project.Name + '_' + m.measpoint.Name
                    }).OrderBy(x => x.label));
                }
                
            }

        }
        // GET: Offsets/Delete/5
        [Authorize(Roles = ("Admin,DivisionAdmin"))]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offset = await _context.Offsets
                .Include(o => o.measpoint)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (offset == null)
            {
                return NotFound();
            }
            return View(offset);
        }

        // POST: Offsets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Admin,DivisionAdmin"))]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var offset = await _context.Offsets.FindAsync(id);
            _context.Offsets.Remove(offset);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OffsetExists(int id)
        {
            return _context.Offsets.Any(e => e.Id == id);
        }
        [RequestSizeLimit(900000000)]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember")]
        public static void SaveAndCompressJpeg(string inputPath, int qualityIn)
        {

            int size = 300;
            int quality = qualityIn;
            string[] fileparts = inputPath.Split(".");
            string path2 = fileparts[0] + "_edit." + fileparts[1];
            using (var image = new Bitmap(System.Drawing.Image.FromFile(inputPath)))
            {
                int width, height;
                if (image.Width > image.Height)
                {
                    width = size;
                    height = Convert.ToInt32(image.Height * size / (double)image.Width);
                }
                else
                {
                    width = Convert.ToInt32(image.Width * size / (double)image.Height);
                    height = size;
                }
                var resized = new Bitmap(width, height);
                using (var graphics = Graphics.FromImage(resized))
                {
                    graphics.CompositingQuality = CompositingQuality.HighSpeed;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.DrawImage(image, 0, 0, width, height);
                    using (var output = System.IO.File.Open(path2, FileMode.Create))
                    {
                        var qualityParamId = System.Drawing.Imaging.Encoder.Quality;
                        var encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0] = new EncoderParameter(qualityParamId, quality);
                        var codec = ImageCodecInfo.GetImageDecoders().FirstOrDefault(codec2 => codec2.FormatID == System.Drawing.Imaging.ImageFormat.Jpeg.Guid);
                        resized.Save(output, codec, encoderParameters);
                    }
                }
            }
        }

        /// Returns the image codec with the given mime type
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        }
        public void ConvertHEICtoJPG(string path)
        {
            using (MagickImage image = new MagickImage(path))
            {
                image.Write(path.Split(".")[0] + ".jpg");
            }
            System.IO.File.Delete(path);
        }
    }
}
