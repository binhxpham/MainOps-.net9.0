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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using MainOps.ExtensionMethods;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Authorization;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,ProjectMember,DivisionAdmin,Supervisor,ExternalDriller")]
    public class PreExcavationsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public PreExcavationsController(DataContext context,UserManager<ApplicationUser> userManager, IWebHostEnvironment env) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // GET: PreExcavations
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("ExternalDriller"))
            {
                var dataContext = _context.PreExcavations.Where(x => x.DoneBy.Equals(user.full_name())).Include(p => p.MeasPoint).Include(p => p.Project).Include(p => p.SubProject);

                return View(await dataContext.ToListAsync());
            }
            else
            {
                var dataContext = _context.PreExcavations.Where(x => x.Project.DivisionId.Equals(user.DivisionId)).Include(p => p.MeasPoint).Include(p => p.Project).Include(p => p.SubProject);
                return View(await dataContext.ToListAsync());
            }
           
        }

        // GET: PreExcavations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var preExcavation = await _context.PreExcavations
                .Include(p => p.MeasPoint)
                .Include(p => p.Project).ThenInclude(x => x.Division)
                .Include(p => p.SubProject)
                .Include(x => x.Before_Photos)
                .Include(x => x.During_Photos)
                .Include(x => x.After_Photos)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (preExcavation == null)
            {
                return NotFound();
            }
            else if (User.IsInRole("ExternalDriller"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (!preExcavation.DoneBy.Equals(user.full_name()))
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "you do not have access to this report" });
                }
            }

            return new ViewAsPdf("_PreExcavation",preExcavation);
        }

        // GET: PreExcavations/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            
            ViewData["ProjectId"] = await GetProjectList();
            if (User.IsInRole("ExternalDriller"))
            {
                var subprojects = await (from pu in _context.ProjectUsers
                                         join p in _context.Projects.Include(x => x.SubProjects).ThenInclude(x => x.Project) on pu.projectId equals p.Id
                                         join sp in _context.SubProjects on p.Id equals sp.ProjectId
                                         where pu.userId.Equals(user.Id)
                                         select sp).ToListAsync();
                var measpoints = await (from pu in _context.ProjectUsers
                                         join p in _context.Projects on pu.projectId equals p.Id
                                         join mp in _context.MeasPoints.Include(x => x.Project) on p.Id equals mp.ProjectId
                                         where pu.userId.Equals(user.Id)
                                         select mp).ToListAsync();
                ViewData["SubProjectId"] = new SelectList(subprojects, "Id", "Name");
                ViewData["MeasPointId"] = new SelectList(measpoints, "Id", "Name");

            }
            else
            {
                ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
                ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
            }
            return View();
        }

        // POST: PreExcavations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<IActionResult> UpdatePreExcavations()
        {
            var preexcavations = await _context.PreExcavations.ToListAsync();
            
            foreach (var preExcavation in preexcavations)
            {
                var itemtype = await _context.ItemTypes.Include(x => x.ReportType).Where(x => x.ProjectId.Equals(preExcavation.ProjectId) && x.ReportType.Type.Equals("PreExcavation")).SingleOrDefaultAsync();
                var prev_install = await _context.Installations.Where(x => x.ItemTypeId.Equals(itemtype.Id) && x.UniqueID.Equals(preExcavation.wellname)).ToListAsync();
                if (prev_install.Count() == 0)
                {

                    Install inst = new Install
                    {
                        ToBePaid = true,
                        ItemTypeId = itemtype.Id,
                        Latitude = 0,
                        Longitude = 0,
                        TimeStamp = preExcavation.TimeStamp,
                        InvoiceDate = DateTime.Now,
                        RentalStartDate = preExcavation.TimeStamp,
                        Install_Text = preExcavation.wellname + " : " + preExcavation.Comments,
                        isInstalled = true,
                        Amount = 1,
                        UniqueID = preExcavation.wellname,
                        ProjectId = preExcavation.ProjectId,
                        SubProjectId = preExcavation.SubProjectId,
                        EnteredIntoDataBase = DateTime.Now,
                        LastEditedInDataBase = DateTime.Now,
                        DoneBy = preExcavation.DoneBy,
                        VariationOrderId = preExcavation.VariationOrderId

                    };
                    _context.Installations.Add(inst);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,SubProjectId,TimeStamp,MeasPointId,wellname,CablesFound,Comments,DoneBy,Large,VariationOrderId,NewCover,RemovalOldManShaft")] PreExcavation preExcavation,IFormFile[] photo1s,IFormFile[] photo2s,IFormFile[] photo3s)
        {
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {

                if (photo1s.Count() != 0 && photo2s.Count() != 0 && photo3s.Count() != 0)
                {
                    preExcavation.DoneBy = user.full_name();
                    _context.Add(preExcavation);
                    await _context.SaveChangesAsync();

                    var model = await _context.PreExcavations.LastAsync();
                    if (photo1s != null)
                    {

                        var directory = _env.WebRootPath + "\\PreExcavations\\Before\\" + model.Id.ToString() + "\\";
                        if (!Directory.Exists(directory) && photo1s.Count() != 0)
                        {
                            Directory.CreateDirectory(directory);
                        }
                        foreach (var photo1 in photo1s) {
                            var path = Path.Combine(directory, photo1.FileName);
                            PreExcavationBeforePhoto pic = new PreExcavationBeforePhoto { PreExcavationId = model.Id, TimeStamp = DateTime.Now, path = path };
                            _context.Add(pic);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await photo1.CopyToAsync(stream);
                            };
                            if (path.ToLower().Contains(".jpg") || path.ToLower().Contains(".jpeg"))
                            {
                                PhotoExtensions.SaveAndCompressJpeg(path, 95);
                            }
                        }
                    }
                    if (photo2s != null)
                    {
                        var directory = _env.WebRootPath + "\\PreExcavations\\During\\" + model.Id.ToString() + "\\";
                        if (!Directory.Exists(directory) && photo2s.Count() != 0)
                        {
                            Directory.CreateDirectory(directory);
                        }
                        foreach (var photo2 in photo2s) {
                            var path = Path.Combine(directory, photo2.FileName);
                            PreExcavationPhoto pic = new PreExcavationPhoto { PreExcavationId = model.Id, TimeStamp = DateTime.Now, path = path };
                            _context.Add(pic);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await photo2.CopyToAsync(stream);
                            };
                            if (path.ToLower().Contains(".jpg") || path.ToLower().Contains(".jpeg"))
                            {
                                PhotoExtensions.SaveAndCompressJpeg(path, 95);
                            }
                        }
                    }
                    if (photo3s != null)
                    {
                        var directory = _env.WebRootPath + "\\PreExcavations\\After\\" + model.Id.ToString() + "\\";
                        if (!Directory.Exists(directory) && photo3s.Count() != 0)
                        {
                            Directory.CreateDirectory(directory);
                        }
                        foreach (var photo3 in photo3s)
                        {

                            var path = Path.Combine(directory, photo3.FileName);
                            PreExcavationAfterPhoto pic = new PreExcavationAfterPhoto { PreExcavationId = model.Id, TimeStamp = DateTime.Now, path = path };
                            _context.Add(pic);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await photo3.CopyToAsync(stream);
                            };
                            if (path.ToLower().Contains(".jpg") || path.ToLower().Contains(".jpeg"))
                            {
                                PhotoExtensions.SaveAndCompressJpeg(path, 95);
                            }
                        }
                    }
                    if (model.Large == false) {
                        var itemtype = await _context.ItemTypes.Include(x => x.ReportType).Where(x => x.ProjectId.Equals(model.ProjectId) && x.ReportType.Type.Equals("PreExcavation")).SingleOrDefaultAsync();
                        if (itemtype != null)
                        {
                            var prev_install = await _context.Installations.Where(x => x.ItemTypeId.Equals(itemtype.Id) && x.UniqueID.Equals(model.wellname)).ToListAsync();
                            if (prev_install.Count() == 0) {

                                Install inst = new Install
                                {
                                    ToBePaid = true,
                                    ItemTypeId = itemtype.Id,
                                    Latitude = 0,
                                    Longitude = 0,
                                    TimeStamp = model.TimeStamp,
                                    InvoiceDate = DateTime.Now,
                                    RentalStartDate = model.TimeStamp,
                                    Install_Text = model.wellname + " : " + model.Comments,
                                    isInstalled = true,
                                    Amount = 1,
                                    UniqueID = model.wellname,
                                    ProjectId = model.ProjectId,
                                    SubProjectId = model.SubProjectId,
                                    EnteredIntoDataBase = DateTime.Now,
                                    LastEditedInDataBase = DateTime.Now,
                                    DoneBy = model.DoneBy,
                                    VariationOrderId = preExcavation.VariationOrderId
                                };
                                _context.Installations.Add(inst);
                            }
                            else
                            {
                                var itemtypeExtra = await _context.ItemTypes.Include(x => x.ReportType).Where(x => x.ProjectId.Equals(model.ProjectId) && x.ReportType.Type.Equals("PreExcavationExtra")).SingleOrDefaultAsync();
                                if (itemtypeExtra != null)
                                {
                                    Install inst = new Install
                                    {
                                        ToBePaid = true,
                                        ItemTypeId = itemtypeExtra.Id,
                                        Latitude = 0,
                                        Longitude = 0,
                                        TimeStamp = model.TimeStamp,
                                        InvoiceDate = DateTime.Now,
                                        RentalStartDate = model.TimeStamp,
                                        Install_Text = model.wellname + " : " + model.Comments,
                                        isInstalled = true,
                                        Amount = 1,
                                        UniqueID = model.wellname + " no:" + (prev_install.Count() + 1).ToString(),
                                        ProjectId = model.ProjectId,
                                        SubProjectId = model.SubProjectId,
                                        EnteredIntoDataBase = DateTime.Now,
                                        LastEditedInDataBase = DateTime.Now,
                                        DoneBy = model.DoneBy,
                                        VariationOrderId = preExcavation.VariationOrderId
                                    };
                                    _context.Installations.Add(inst);
                                }
                                else
                                {
                                    Install inst = new Install
                                    {
                                        ToBePaid = true,
                                        ItemTypeId = itemtype.Id,
                                        Latitude = 0,
                                        Longitude = 0,
                                        TimeStamp = model.TimeStamp,
                                        InvoiceDate = DateTime.Now,
                                        RentalStartDate = model.TimeStamp,
                                        Install_Text = model.wellname + " : " + model.Comments,
                                        isInstalled = true,
                                        Amount = 1,
                                        UniqueID = model.wellname,
                                        ProjectId = model.ProjectId,
                                        SubProjectId = model.SubProjectId,
                                        EnteredIntoDataBase = DateTime.Now,
                                        LastEditedInDataBase = DateTime.Now,
                                        DoneBy = model.DoneBy,
                                        VariationOrderId = preExcavation.VariationOrderId
                                    };
                                    _context.Installations.Add(inst);
                                }
                            }

                        }
                    }
                    else
                    {
                        var itemtype = await _context.ItemTypes.Include(x => x.ReportType).Where(x => x.ProjectId.Equals(model.ProjectId) && x.ReportType.Type.Equals("PreExcavationLarge")).SingleOrDefaultAsync();
                        if (itemtype != null)
                        {
                            var prev_install = await _context.Installations.Where(x => x.ItemTypeId.Equals(itemtype.Id) && x.UniqueID.Equals(model.wellname)).ToListAsync();
                            if (prev_install.Count() == 0)
                            {

                                Install inst = new Install
                                {
                                    ToBePaid = true,
                                    ItemTypeId = itemtype.Id,
                                    Latitude = 0,
                                    Longitude = 0,
                                    TimeStamp = model.TimeStamp,
                                    RentalStartDate = model.TimeStamp,
                                    InvoiceDate = DateTime.Now,
                                    Install_Text = model.wellname + " : " + model.Comments,
                                    isInstalled = true,
                                    Amount = 1,
                                    UniqueID = model.wellname,
                                    ProjectId = model.ProjectId,
                                    SubProjectId = model.SubProjectId,
                                    EnteredIntoDataBase = DateTime.Now,
                                    LastEditedInDataBase = DateTime.Now,
                                    DoneBy = model.DoneBy,
                                    VariationOrderId = preExcavation.VariationOrderId
                                };
                                _context.Installations.Add(inst);
                            }
                            else
                            {
                                var itemtypeExtra = await _context.ItemTypes.Include(x => x.ReportType).Where(x => x.ProjectId.Equals(model.ProjectId) && x.ReportType.Type.Equals("PreExcavationExtra")).SingleOrDefaultAsync();
                                if (itemtypeExtra != null)
                                {
                                    Install inst = new Install
                                    {
                                        ToBePaid = true,
                                        ItemTypeId = itemtypeExtra.Id,
                                        Latitude = 0,
                                        Longitude = 0,
                                        TimeStamp = model.TimeStamp,
                                        RentalStartDate = model.TimeStamp,
                                        InvoiceDate = DateTime.Now,
                                        Install_Text = model.wellname + " : " + model.Comments,
                                        isInstalled = true,
                                        Amount = 1,
                                        UniqueID = model.wellname + " no:" + (prev_install.Count() + 1).ToString(),
                                        ProjectId = model.ProjectId,
                                        SubProjectId = model.SubProjectId,
                                        EnteredIntoDataBase = DateTime.Now,
                                        LastEditedInDataBase = DateTime.Now,
                                        DoneBy = model.DoneBy,
                                        VariationOrderId = preExcavation.VariationOrderId
                                    };
                                    _context.Installations.Add(inst);
                                }
                                else
                                {
                                    Install inst = new Install
                                    {
                                        ToBePaid = true,
                                        ItemTypeId = itemtype.Id,
                                        Latitude = 0,
                                        Longitude = 0,
                                        TimeStamp = model.TimeStamp,
                                        RentalStartDate = model.TimeStamp,
                                        InvoiceDate = DateTime.Now,
                                        Install_Text = model.wellname + " : " + model.Comments,
                                        isInstalled = true,
                                        Amount = 1,
                                        UniqueID = model.wellname,
                                        ProjectId = model.ProjectId,
                                        SubProjectId = model.SubProjectId,
                                        EnteredIntoDataBase = DateTime.Now,
                                        LastEditedInDataBase = DateTime.Now,
                                        DoneBy = model.DoneBy,
                                        VariationOrderId = preExcavation.VariationOrderId
                                    };
                                    _context.Installations.Add(inst);
                                }
                            }

                        }
                    }
                    if (model.NewCover.Equals(true) && model.ProjectId == 437)
                    {
                        var prev_install_cover = await _context.Installations.Where(x => x.ItemTypeId.Equals(2190) && x.UniqueID.Equals(model.wellname)).ToListAsync();
                        if (prev_install_cover.Count < 1)
                        {
                            Install inst = new Install
                            {
                                ToBePaid = true,
                                ItemTypeId = 2190,
                                Latitude = 0,
                                Longitude = 0,
                                TimeStamp = model.TimeStamp,
                                InvoiceDate = DateTime.Now,
                                RentalStartDate = model.TimeStamp,
                                Install_Text = model.wellname + " : " + model.Comments,
                                isInstalled = true,
                                Amount = 1,
                                UniqueID = model.wellname,
                                ProjectId = model.ProjectId,
                                SubProjectId = model.SubProjectId,
                                EnteredIntoDataBase = DateTime.Now,
                                LastEditedInDataBase = DateTime.Now,
                                DoneBy = model.DoneBy,
                                VariationOrderId = preExcavation.VariationOrderId
                            };
                            _context.Installations.Add(inst);
                            
                        }
                    }
                    if (model.RemovalOldManShaft.Equals(true) && model.ProjectId == 437)
                    {
                        var prev_install_removalmanshaft = await _context.Installations.Where(x => x.ItemTypeId.Equals(2191) && x.UniqueID.Equals(model.wellname)).ToListAsync();
                        if (prev_install_removalmanshaft.Count < 1)
                        {
                            Install inst = new Install
                            {
                                ToBePaid = true,
                                ItemTypeId = 2191,
                                Latitude = 0,
                                Longitude = 0,
                                TimeStamp = model.TimeStamp,
                                InvoiceDate = DateTime.Now,
                                RentalStartDate = model.TimeStamp,
                                Install_Text = model.wellname + " : " + model.Comments,
                                isInstalled = true,
                                Amount = 1,
                                UniqueID = model.wellname,
                                ProjectId = model.ProjectId,
                                SubProjectId = model.SubProjectId,
                                EnteredIntoDataBase = DateTime.Now,
                                LastEditedInDataBase = DateTime.Now,
                                DoneBy = model.DoneBy,
                                VariationOrderId = preExcavation.VariationOrderId
                            };
                            _context.Installations.Add(inst);
                            
                        }
                    }
                
                    await _context.SaveChangesAsync();
                }
                else
                {

                    ViewData["ProjectId"] = await GetProjectList();
                    if (User.IsInRole("ExternalDriller"))
                    {
                        var subprojects = await (from pu in _context.ProjectUsers
                                                 join p in _context.Projects.Include(x => x.SubProjects).ThenInclude(x => x.Project) on pu.projectId equals p.Id
                                                 join sp in _context.SubProjects on p.Id equals sp.ProjectId
                                                 where pu.userId.Equals(user.Id)
                                                 select sp).ToListAsync();
                        var measpoints = await (from pu in _context.ProjectUsers
                                                join p in _context.Projects on pu.projectId equals p.Id
                                                join mp in _context.MeasPoints.Include(x => x.Project) on p.Id equals mp.ProjectId
                                                where pu.userId.Equals(user.Id)
                                                select mp).ToListAsync();
                        ViewData["SubProjectId"] = new SelectList(subprojects, "Id", "Name");
                        ViewData["MeasPointId"] = new SelectList(measpoints, "Id", "Name");

                    }
                    else
                    {
                        ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
                        ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
                    }
                    ViewData["ErrorMessage"] = "Please add all pictures! if some are missing, add something random...";
                    return View(preExcavation);
                }
                

            }
            else
            {
                ViewData["ProjectId"] = await GetProjectList();

                if (User.IsInRole("ExternalDriller"))
                {
                    var subprojects = await (from pu in _context.ProjectUsers
                                             join p in _context.Projects.Include(x => x.SubProjects).ThenInclude(x => x.Project) on pu.projectId equals p.Id
                                             join sp in _context.SubProjects on p.Id equals sp.ProjectId
                                             where pu.userId.Equals(user.Id)
                                             select sp).ToListAsync();
                    var measpoints = await (from pu in _context.ProjectUsers
                                            join p in _context.Projects on pu.projectId equals p.Id
                                            join mp in _context.MeasPoints.Include(x => x.Project) on p.Id equals mp.ProjectId
                                            where pu.userId.Equals(user.Id)
                                            select mp).ToListAsync();
                    ViewData["SubProjectId"] = new SelectList(subprojects, "Id", "Name");
                    ViewData["MeasPointId"] = new SelectList(measpoints, "Id", "Name");

                }
                else
                {
                    ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
                    ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
                }
                return View(preExcavation);
            }
            //ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
            //ViewData["ProjectId"] = await GetProjectList();
            //ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
            return RedirectToAction("MainMenu", "TrackItems");
        }

        // GET: PreExcavations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (id == null)
            {
                return NotFound();
            }

            var preExcavation = await _context.PreExcavations.FindAsync(id);
            if (preExcavation == null)
            {
                return NotFound();
            }
            else if (User.IsInRole("ExternalDriller"))
            {
                
                if (!preExcavation.DoneBy.Equals(user.full_name()))
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "you do not have access to this resource" });
                }
            }
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Include(x => x.MeasType).Where(x => x.ProjectId.Equals(preExcavation.ProjectId) && x.MeasType.Type.ToLower().Equals("water level")), "Id", "Name");
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x => x.Project).Where(x => x.ProjectId.Equals(preExcavation.ProjectId)), "Id", "Name");


            return View(preExcavation);
        }

        // POST: PreExcavations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,SubProjectId,TimeStamp,MeasPointId,wellname,CablesFound,Comments,DoneBy,Large,VariationOrderId,NewCover,RemovalOldManShaft")] PreExcavation preExcavation)
        {
            var user = await _userManager.GetUserAsync(User);
            if (id != preExcavation.Id)
            {
                return NotFound();
            }
            
            else if (User.IsInRole("ExternalDriller"))
            {
                
                if (!preExcavation.DoneBy.Equals(user.full_name()))
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "you do not have access to alter this resource" });
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(preExcavation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PreExcavationExists(preExcavation.Id))
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
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Abbreviation", preExcavation.ProjectId);
            if (User.IsInRole("ExternalDriller"))
            {
                var subprojects = await (from pu in _context.ProjectUsers
                                         join p in _context.Projects.Include(x => x.SubProjects).ThenInclude(x => x.Project) on pu.projectId equals p.Id
                                         join sp in _context.SubProjects on p.Id equals sp.ProjectId
                                         where pu.userId.Equals(user.Id)
                                         select sp).ToListAsync();
                var measpoints = await (from pu in _context.ProjectUsers
                                        join p in _context.Projects on pu.projectId equals p.Id
                                        join mp in _context.MeasPoints.Include(x => x.Project) on p.Id equals mp.ProjectId
                                        where pu.userId.Equals(user.Id)
                                        select mp).ToListAsync();
                ViewData["SubProjectId"] = new SelectList(subprojects, "Id", "Name");
                ViewData["MeasPointId"] = new SelectList(measpoints, "Id", "Name");

            }
            else
            {
                ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
                ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
            }
            return View(preExcavation);
        }

        // GET: PreExcavations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var preExcavation = await _context.PreExcavations
                .Include(p => p.MeasPoint)
                .Include(p => p.Project)
                .Include(p => p.SubProject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (preExcavation == null)
            {
                return NotFound();
            }
            else if (User.IsInRole("ExternalDriller"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (!preExcavation.DoneBy.Equals(user.full_name()))
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "you do not have access to this resource" });
                }
            }

            return View(preExcavation);
        }

        // POST: PreExcavations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var preExcavation = await _context.PreExcavations.FindAsync(id);
            if (User.IsInRole("ExternalDriller"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (!preExcavation.DoneBy.Equals(user.full_name()))
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "you do not have access to this resource" });
                }
            }
            var photos1 = await _context.PreExcavationAfterPhotos.Where(x => x.PreExcavationId.Equals(preExcavation.Id)).ToListAsync();
            var photos2 = await _context.PreExcavationBeforePhotos.Where(x => x.PreExcavationId.Equals(preExcavation.Id)).ToListAsync();
            var photos3 = await _context.PreExcavationPhotos.Where(x => x.PreExcavationId.Equals(preExcavation.Id)).ToListAsync();
            foreach(var photo in photos1)
            {
                _context.Remove(photo);
            }
            foreach (var photo in photos2)
            {
                _context.Remove(photo);
            }
            foreach (var photo in photos3)
            {
                _context.Remove(photo);
            }
            await _context.SaveChangesAsync();
            _context.PreExcavations.Remove(preExcavation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PreExcavationExists(int id)
        {
            return _context.PreExcavations.Any(e => e.Id == id);
        }
    }
}
