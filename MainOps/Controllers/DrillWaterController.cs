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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using MainOps.ExtensionMethods;
using Microsoft.AspNetCore.Authorization;

namespace MainOps.Controllers
{
    [Authorize(Roles = "ExternalDriller,ProjectMember,Manager,DivisionAdmin,Admin")]
    public class DrillWaterController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public DrillWaterController(DataContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // GET: DrillWater
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.DrillWaters.Include(d => d.MeasPoint).Include(d => d.Project).Include(d => d.SubProject);
            return View(await dataContext.ToListAsync());
        }

        // GET: DrillWater/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drillWater = await _context.DrillWaters
                .Include(d => d.MeasPoint)
                .Include(d => d.Project)
                .Include(d => d.SubProject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (drillWater == null)
            {
                return NotFound();
            }

            return View(drillWater);
        }

        // GET: DrillWater/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ProjectId"] = await GetProjectList();
            return View();
        }

        // POST: DrillWater/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,SubProjectId,MeasPointId,DrillWaterStart,DrillWaterEnd,AmountContainers")] DrillWater drillWater, IFormFile[] photos)
        {
            if (ModelState.IsValid)
            {
                if(drillWater.DrillWaterStart == null)
                {
                    drillWater.DrillWaterStart = 0;
                }
                if(drillWater.AmountContainers == null)
                {
                    drillWater.AmountContainers = 1;
                }
                _context.Add(drillWater);
                await _context.SaveChangesAsync();
                var lastadded = await _context.DrillWaters.LastAsync();
                if (photos != null)
                {

                    var directory = _env.WebRootPath + "\\DrillWater\\" + lastadded.Id.ToString() + "\\";
                    if (!Directory.Exists(directory) && photos.Count() != 0)
                    {
                        Directory.CreateDirectory(directory);
                    }
                    foreach (var photo1 in photos)
                    {
                        var path = Path.Combine(directory, photo1.FileName);
                        DrillWaterPhoto pic = new DrillWaterPhoto { DrillWaterId = lastadded.Id, TimeStamp = DateTime.Now, path = path };
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
                return RedirectToAction(nameof(Index));
            }
            if(drillWater.ProjectId != 0 && drillWater.ProjectId != -1)
            {
                ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Where(x => x.ProjectId.Equals(drillWater.ProjectId)), "Id", "Name", drillWater.MeasPointId);
                ViewData["ProjectId"] = GetProjectList();
                ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => x.ProjectId.Equals(drillWater.ProjectId)), "Id", "Id", drillWater.SubProjectId);
            }
            else
            {

                ViewData["ProjectId"] = GetProjectList();

            }
            
            return View(drillWater);
        }

        // GET: DrillWater/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drillWater = await _context.DrillWaters.FindAsync(id);
            if (drillWater == null)
            {
                return NotFound();
            }
            ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Where(x => x.ProjectId.Equals(drillWater.ProjectId)), "Id", "Name", drillWater.MeasPointId);
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => x.ProjectId.Equals(drillWater.ProjectId)), "Id", "Name", drillWater.SubProjectId);
            return View(drillWater);
        }

        // POST: DrillWater/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,SubProjectId,MeasPointId,DrillWaterStart,DrillWaterEnd,AmountContainers")] DrillWater drillWater,IFormFile[] photos)
        {
            if (id != drillWater.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(drillWater);
                    await _context.SaveChangesAsync();
                    if (photos != null)
                    {

                        var directory = _env.WebRootPath + "\\DrillWater\\" + drillWater.Id.ToString() + "\\";
                        if (!Directory.Exists(directory) && photos.Count() != 0)
                        {
                            Directory.CreateDirectory(directory);
                        }
                        foreach (var photo1 in photos)
                        {
                            var path = Path.Combine(directory, photo1.FileName);
                            DrillWaterPhoto pic = new DrillWaterPhoto { DrillWaterId = drillWater.Id, TimeStamp = DateTime.Now, path = path };
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
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DrillWaterExists(drillWater.Id))
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
            if (drillWater.ProjectId != 0 && drillWater.ProjectId != -1)
            {
                ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Where(x => x.ProjectId.Equals(drillWater.ProjectId)), "Id", "Name", drillWater.MeasPointId);
                ViewData["ProjectId"] = GetProjectList();
                ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => x.ProjectId.Equals(drillWater.ProjectId)), "Id", "Id", drillWater.SubProjectId);
            }
            else
            {

                ViewData["ProjectId"] = GetProjectList();

            }
            return View(drillWater);
        }

        // GET: DrillWater/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drillWater = await _context.DrillWaters
                .Include(d => d.MeasPoint)
                .Include(d => d.Project)
                .Include(d => d.SubProject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (drillWater == null)
            {
                return NotFound();
            }

            return View(drillWater);
        }

        // POST: DrillWater/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var drillWater = await _context.DrillWaters.FindAsync(id);
            _context.DrillWaters.Remove(drillWater);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DrillWaterExists(int id)
        {
            return _context.DrillWaters.Any(e => e.Id == id);
        }
    }
}
