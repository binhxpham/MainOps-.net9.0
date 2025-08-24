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
using System.Net.Http.Headers;
using MainOps.ExtensionMethods;
using Rotativa.AspNetCore;

namespace MainOps.Controllers
{
    public class PhotoDocumenationsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public PhotoDocumenationsController(DataContext context,UserManager<ApplicationUser> userManager, IWebHostEnvironment env):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // GET: PhotoDocumenations
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.PhotoDocumenations.Include(p => p.Project).Include(p => p.SubProject);
            return View(await dataContext.ToListAsync());
        }

        // GET: PhotoDocumenations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var photoDocumenation = await _context.PhotoDocumenations
                .Include(p => p.Project).ThenInclude(x => x.Division)
                .Include(p => p.SubProject)
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (photoDocumenation == null)
            {
                return NotFound();
            }

            return new ViewAsPdf("Details",photoDocumenation);
        }

        // GET: PhotoDocumenations/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ProjectId"] = await GetProjectList();
            //ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Id");
            return View();
        }

        // POST: PhotoDocumenations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,SubProjectId,Description,Latitude,Longitude")] PhotoDocumenation photoDocumenation,IFormFile[] files)
        {
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                _context.Add(photoDocumenation);
                await _context.SaveChangesAsync();
                //
                var itemadded = await _context.PhotoDocumenations.LastAsync();
                var directory = _env.WebRootPath + "\\AHAK\\PhotoDocs\\" + itemadded.Id.ToString() + "\\";
                
                if (!Directory.Exists(directory) && files != null)
                {
                    Directory.CreateDirectory(directory);
                }
                foreach (IFormFile photo in files)
                {
                    var path = Path.Combine(directory, photo.FileName);
                    
                    PhotoDoc photomodel = new PhotoDoc { Path = path, PhotoDocumentationId = itemadded.Id};
                    _context.Add(photomodel);
                    
                    var stream = new FileStream(path, FileMode.Create);
                    await photo.CopyToAsync(stream);
                    stream.Close();

                    if (path.Contains(".jpg") || path.Contains(".jpeg") || path.Contains(".JPG") || path.Contains(".JPEG"))
                    {
                        PhotoExtensions.SaveAndCompressJpeg(path, 95);
                    }
                }
                foreach (var file in files.Where(x => x.FileName.ToLower().Contains(".heic")))
                {
                    PhotoExtensions.ConvertHEICtoJPG(_env.WebRootPath + "/AHAK/PhotoDocs/" + itemadded.Id.ToString() + "/" + file.FileName);
                }

                //
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x=>x.Project).Where(x=>x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Id", photoDocumenation.SubProjectId);
            return View(photoDocumenation);
        }

        // GET: PhotoDocumenations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (id == null)
            {
                return NotFound();
            }

            var photoDocumenation = await _context.PhotoDocumenations.FindAsync(id);
            if (photoDocumenation == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Id", photoDocumenation.SubProjectId);
            return View(photoDocumenation);
        }

        // POST: PhotoDocumenations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,SubProjectId,Description,Latitude,Longitude")] PhotoDocumenation photoDocumenation,IFormFile[] files)
        {

            if (id != photoDocumenation.Id)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(photoDocumenation);
                    await _context.SaveChangesAsync();
                    var directory = _env.WebRootPath + "\\AHAK\\PhotoDocs\\" + photoDocumenation.Id.ToString() + "\\";

                    if (!Directory.Exists(directory) && files != null)
                    {
                        Directory.CreateDirectory(directory);
                    }
                    foreach (IFormFile photo in files)
                    {
                        var path = Path.Combine(directory, photo.FileName);

                        PhotoDoc photomodel = new PhotoDoc { Path = path, PhotoDocumentationId = photoDocumenation.Id };
                        _context.Add(photomodel);

                        var stream = new FileStream(path, FileMode.Create);
                        await photo.CopyToAsync(stream);
                        stream.Close();

                        if (path.Contains(".jpg") || path.Contains(".jpeg"))
                        {
                            PhotoExtensions.SaveAndCompressJpeg(path, 80);
                        }
                    }
                    foreach (var file in files.Where(x => x.FileName.ToLower().Contains(".heic")))
                    {
                        PhotoExtensions.ConvertHEICtoJPG(_env.WebRootPath + "/AHAK/PhotoDocs/" + photoDocumenation.Id.ToString() + "/" + file.FileName);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhotoDocumenationExists(photoDocumenation.Id))
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
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Id", photoDocumenation.SubProjectId);
            return View(photoDocumenation);
        }

        // GET: PhotoDocumenations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var photoDocumenation = await _context.PhotoDocumenations
                .Include(p => p.Project)
                .Include(p => p.SubProject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if(!photoDocumenation.Project.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to delte this item" });
            }
            if (photoDocumenation == null)
            {
                return NotFound();
            }

            return View(photoDocumenation);
        }

        // POST: PhotoDocumenations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var photoDocumenation = await _context.PhotoDocumenations.FindAsync(id);
            var photos = await _context.PhotoDocs.Where(x => x.PhotoDocumentationId.Equals(photoDocumenation.Id)).ToListAsync();
            foreach(var photo in photos)
            {
                _context.Remove(photo);
            }
            await _context.SaveChangesAsync();
            _context.PhotoDocumenations.Remove(photoDocumenation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhotoDocumenationExists(int id)
        {
            return _context.PhotoDocumenations.Any(e => e.Id == id);
        }
    }
}
