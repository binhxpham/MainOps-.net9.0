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
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using MainOps.ExtensionMethods;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Authorization;

namespace MainOps.Controllers
{
    [Authorize(Roles = "ProjectMember,Manager,DivisionAdmin,Admin")]
    public class AirliftsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public AirliftsController(DataContext context,UserManager<ApplicationUser> userManager, IWebHostEnvironment env) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
            _env = env;

        }

        // GET: Airlifts
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            await CheckUser(user);
            if (User.IsInRole("Admin")) {
            var dataContext = _context.Airlifts.Include(a => a.MeasPoint).Include(x => x.Project).Include(x => x.SubProject);
            return View(await dataContext.ToListAsync());
            }
            else
            {
                
                var dataContext = _context.Airlifts.Include(a => a.MeasPoint).Include(x => x.Project).Include(x => x.SubProject)
                    .Where(x => x.Project.DivisionId.Equals(user.DivisionId));
                return View(await dataContext.ToListAsync());
            }
        }

        // GET: Airlifts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var airlift = await _context.Airlifts
                .Include(a => a.MeasPoint).Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.SubProject).Include(x => x.Photos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if(!User.IsInRole("Admin") && !user.DivisionId.Equals(airlift.Project.DivisionId))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "you do not have access to this form" });
            }
            if (airlift == null)
            {
                return NotFound();
            }

            return new ViewAsPdf("_Airlift",airlift);
        }

        // GET: Airlifts/Create
        public async Task<IActionResult> Create()
        {
            Airlift ar = new Airlift();
            ar.TimeStamp = DateTime.Now;
            ViewData["ProjectId"] = await GetProjectList();
            return View(ar);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAirliftInstalls()
        {
            var airlifts = await _context.Airlifts.ToListAsync();

            foreach(var al in airlifts)
            {

                var itemtype = await _context.ItemTypes.Include(x => x.ReportType).Where(x => x.ProjectId.Equals(al.ProjectId) && x.ReportType.Type.Equals("AirLifting")).SingleOrDefaultAsync();
                var mp = await _context.MeasPoints.SingleOrDefaultAsync(x => x.Id.Equals(al.MeasPointId));
                var prev_install = await _context.Installations.FirstOrDefaultAsync(x => x.ItemTypeId.Equals(itemtype.Id) && x.ProjectId.Equals(al.ProjectId) && x.UniqueID.Equals("Airlift : " + mp.Name));
                if(prev_install == null) {
                if (mp.Lati != null && mp.Longi != null)
                {
                    if (itemtype != null)
                    {
                        Install inst = new Install
                        {
                            ToBePaid = true,
                            ItemTypeId = itemtype.Id,
                            Latitude = Convert.ToDouble(mp.Lati),
                            Longitude = Convert.ToDouble(mp.Longi),
                            TimeStamp = al.TimeStamp,
                            InvoiceDate = DateTime.Now,
                            RentalStartDate = al.TimeStamp,
                            Install_Text = al.Comments,
                            isInstalled = true,
                            UniqueID = "Airlift : " + mp.Name,
                            Amount = 1,
                            ProjectId = al.ProjectId,
                            SubProjectId = al.SubProjectId,
                            EnteredIntoDataBase = DateTime.Now,
                            LastEditedInDataBase = DateTime.Now,
                            DoneBy = al.DoneBy
                        };
                        _context.Installations.Add(inst);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        if (itemtype != null)
                        {
                            Install inst = new Install
                            {
                                ToBePaid = true,
                                ItemTypeId = itemtype.Id,
                                Latitude = 0,
                                Longitude = 0,
                                TimeStamp = al.TimeStamp,
                                InvoiceDate = DateTime.Now,
                                RentalStartDate = al.TimeStamp,
                                Install_Text = al.Comments,
                                isInstalled = true,
                                UniqueID = "Airlift : " + mp.Name,
                                Amount = 1,
                                ProjectId = al.ProjectId,
                                SubProjectId = al.SubProjectId,
                                EnteredIntoDataBase = DateTime.Now,
                                LastEditedInDataBase = DateTime.Now,
                                DoneBy = al.DoneBy
                            };
                            _context.Installations.Add(inst);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                }
            }
            return RedirectToAction(nameof(Index));
        }
        // POST: Airlifts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,SubProjectId,TimeStamp,BottomWellBefore,WaterLevelBefore,BottomWellAfter,WaterLevelAfter,MeasPointId,Comments,DoneBy,RecoveryTime")] Airlift airlift, IFormFile[] photos)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                airlift.DoneBy = user.full_name();
                _context.Add(airlift);
                await _context.SaveChangesAsync();
                var lastadded = await _context.Airlifts.LastAsync();
                if (photos != null)
                {
                    
                    var directory = _env.WebRootPath + "\\Airlifts\\" + lastadded.Id.ToString() + "\\";
                    if (!Directory.Exists(directory) && photos.Count() != 0)
                    {
                        Directory.CreateDirectory(directory);
                    }
                    foreach (var photo1 in photos)
                    {
                        var path = Path.Combine(directory, photo1.FileName);
                        AirliftPhoto pic = new AirliftPhoto { AirliftId = lastadded.Id, TimeStamp = DateTime.Now, path = path };
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
                    var itemtype = await _context.ItemTypes.Include(x => x.ReportType).Where(x => x.ProjectId.Equals(lastadded.ProjectId) && x.ReportType.Type.Equals("AirLifting")).SingleOrDefaultAsync();
                    
                    var mp = await _context.MeasPoints.SingleOrDefaultAsync(x => x.Id.Equals(airlift.MeasPointId));
                    
                    if(mp.Lati != null && mp.Longi != null) 
                    { 
                        if (itemtype != null)
                        {
                            Install inst = new Install
                            {
                                ToBePaid = true,
                                ItemTypeId = itemtype.Id,
                                Latitude = Convert.ToDouble(mp.Lati),
                                Longitude = Convert.ToDouble(mp.Longi),
                                TimeStamp = lastadded.TimeStamp,
                                InvoiceDate = lastadded.TimeStamp,
                                RentalStartDate = lastadded.TimeStamp,
                                Install_Text = lastadded.Comments,
                                UniqueID = "Airlift : " + mp.Name,
                                isInstalled = true,
                                Amount = 1,
                                ProjectId = lastadded.ProjectId,
                                SubProjectId = lastadded.SubProjectId,
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
                        if (itemtype != null)
                        {
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
                                UniqueID = "Airlift : " + mp.Name,
                                isInstalled = true,
                                Amount = 1,
                                ProjectId = lastadded.ProjectId,
                                SubProjectId = lastadded.SubProjectId,
                                EnteredIntoDataBase = DateTime.Now,
                                LastEditedInDataBase = DateTime.Now,
                                DoneBy = user.full_name()
                            };
                            _context.Installations.Add(inst);
                            await _context.SaveChangesAsync();
                        }
                    }
                    return RedirectToAction(nameof(Index));
            }
            if(airlift.ProjectId != null)
            {
                ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Include(x => x.MeasType).Where(x => x.ProjectId.Equals(airlift.ProjectId) && x.MeasType.Type.ToLower().Equals("water level")),"Id","Name");
                ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => x.ProjectId.Equals(airlift.ProjectId)), "Id", "Name");
            }
            ViewData["ProjectId"] = await GetProjectList();
            return View(airlift);
        }

        // GET: Airlifts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var airlift = await _context.Airlifts.Include(x => x.Project).Include(x => x.MeasPoint).ThenInclude(x => x.MeasType).SingleOrDefaultAsync(x => x.Id.Equals(id));
            if (airlift == null)
            {
                return NotFound();
            }
            if(!User.IsInRole("Admin") && !user.DivisionId.Equals(airlift.Project.DivisionId))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "you do not have access to this item" });
            }
            else if(!User.IsInRole("Admin") && !User.IsInRole("Manager") && !User.IsInRole("DivisionAdmin"))
            {
                if(user.full_name() != airlift.DoneBy)
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "you do not have access to this item" });
                }
            }
            if (airlift.ProjectId != null) { 
                ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Include(x => x.MeasType).Where(x => x.ProjectId.Equals(airlift.ProjectId) && x.MeasType.Type.ToLower().Equals("water level")), "Id", "Name");
                ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => x.ProjectId.Equals(airlift.ProjectId)), "Id", "Name");
            }

            ViewData["ProjectId"] = await GetProjectList();

            return View(airlift);
        }

        // POST: Airlifts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,SubProjectId,TimeStamp,BottomWellBefore,WaterLevelBefore,BottomWellAfter,WaterLevelAfter,MeasPointId,Comments,DoneBy,RecoveryTime")] Airlift airlift, IFormFile[] photos)
        {
            if (id != airlift.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(airlift);
                    await _context.SaveChangesAsync();
                    if (photos != null)
                    {

                        var directory = _env.WebRootPath + "\\Airlifts\\" + airlift.Id.ToString() + "\\";
                        if (!Directory.Exists(directory) && photos.Count() != 0)
                        {
                            Directory.CreateDirectory(directory);
                        }
                        foreach (var photo1 in photos)
                        {
                            var path = Path.Combine(directory, photo1.FileName);
                            AirliftPhoto pic = new AirliftPhoto { AirliftId = airlift.Id, TimeStamp = DateTime.Now, path = path };
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
                    var itemtype = await _context.ItemTypes.Include(x => x.ReportType).Where(x => x.ProjectId.Equals(airlift.ProjectId) && x.ReportType.Type.Equals("AirLifting")).SingleOrDefaultAsync();

                    var mp = await _context.MeasPoints.SingleOrDefaultAsync(x => x.Id.Equals(airlift.MeasPointId));
                    var prev_inst = await _context.Installations.Where(x => x.UniqueID.Equals("Airlift : " + mp.Name) && x.TimeStamp.Equals(airlift.TimeStamp)).ToListAsync();
                    if(prev_inst.Count < 1)
                    {
                        if (mp.Lati != null && mp.Longi != null)
                        {
                            if (itemtype != null)
                            {
                                Install inst = new Install
                                {
                                    ToBePaid = true,
                                    ItemTypeId = itemtype.Id,
                                    Latitude = Convert.ToDouble(mp.Lati),
                                    Longitude = Convert.ToDouble(mp.Longi),
                                    TimeStamp = airlift.TimeStamp,
                                    InvoiceDate = airlift.TimeStamp,
                                    RentalStartDate = airlift.TimeStamp,
                                    Install_Text = airlift.Comments,
                                    UniqueID = "Airlift : " + mp.Name,
                                    isInstalled = true,
                                    Amount = 1,
                                    ProjectId = airlift.ProjectId,
                                    SubProjectId = airlift.SubProjectId,
                                    EnteredIntoDataBase = DateTime.Now,
                                    LastEditedInDataBase = DateTime.Now,
                                    DoneBy = airlift.DoneBy
                                };
                                _context.Installations.Add(inst);
                                await _context.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            if (itemtype != null)
                            {
                                Install inst = new Install
                                {
                                    ToBePaid = true,
                                    ItemTypeId = itemtype.Id,
                                    Latitude = 0,
                                    Longitude = 0,
                                    TimeStamp = airlift.TimeStamp,
                                    InvoiceDate = airlift.TimeStamp,
                                    RentalStartDate = airlift.TimeStamp,
                                    Install_Text = airlift.Comments,
                                    UniqueID = "Airlift : " + mp.Name,
                                    isInstalled = true,
                                    Amount = 1,
                                    ProjectId = airlift.ProjectId,
                                    SubProjectId = airlift.SubProjectId,
                                    EnteredIntoDataBase = DateTime.Now,
                                    LastEditedInDataBase = DateTime.Now,
                                    DoneBy = airlift.DoneBy
                                };
                                _context.Installations.Add(inst);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AirliftExists(airlift.Id))
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
            if (airlift.ProjectId != null)
            {
                ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Include(x => x.MeasType).Where(x => x.ProjectId.Equals(airlift.ProjectId) && x.MeasType.Type.ToLower().Equals("water level")), "Id", "Name");
                ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => x.ProjectId.Equals(airlift.ProjectId)), "Id", "Name");
            }
            ViewData["ProjectId"] = await GetProjectList();
            return View(airlift);
        }

        // GET: Airlifts/Delete/5
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airlift = await _context.Airlifts
                .Include(a => a.MeasPoint).Include(x => x.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            
            if (airlift == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (!airlift.Project.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "you do not have access to this item" });
            }

            return View(airlift);
        }

        // POST: Airlifts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var airlift = await _context.Airlifts.FindAsync(id);
            var photos = await _context.AirliftPhotos.Where(x => x.AirliftId.Equals(id)).ToListAsync();
            foreach(var photo in photos)
            {
                try { 
                System.IO.File.Delete(photo.path);
                }
                catch
                {

                }
            }
            foreach (var photo in photos)
            {
                try
                {
                    _context.Remove(photo);
                }
                catch
                {

                }
            }
            await _context.SaveChangesAsync();
            _context.Airlifts.Remove(airlift);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AirliftExists(int id)
        {
            return _context.Airlifts.Any(e => e.Id == id);
        }
    }
}
