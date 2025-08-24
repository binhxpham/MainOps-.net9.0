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

namespace MainOps.Controllers
{
    public class WellChecksController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public WellChecksController(DataContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostingenvironment) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
            _env = hostingenvironment;
        }

        // GET: WellChecks
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                return View(await _context.WellChecks.Include(x => x.Project).ToListAsync());
            }
            else
            {
                return View(await _context.WellChecks.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)).ToListAsync());
            }
            
        }

        // GET: WellChecks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var wellCheck = await _context.WellChecks.Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.Photos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (wellCheck == null)
            {
                return NotFound();
            }
            
            if(user.DivisionId != wellCheck.Project.DivisionId && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this object" });
            }
            return new ViewAsPdf("_WellCheck",wellCheck);
        }

        // GET: WellChecks/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ProjectId"] = await GetProjectList();
            WellCheck wellcheck = new WellCheck();
            wellcheck.ProjectId = 437;
            wellcheck.IsShaftOk = true;
            wellcheck.IsCoverOk = true;
            wellcheck.CanBeFound = true;
            return View(wellcheck);
        }

        // POST: WellChecks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,WellName,CanBeFound,Comments,Dip,BottomWell,DiffTop,Dip2,BottomWell2,DiffTop2,Dip3,BottomWell3,DiffTop3,NumBerOfPipes,IsCoverOk,IsShaftOk,WellHeads,DoneBy,TimeStamp")] WellCheck wellCheck,IFormFile[] files)
        {
            if (ModelState.IsValid)
            {
                var theuser = await _userManager.GetUserAsync(User);
                wellCheck.DoneBy = theuser.full_name();
                _context.Add(wellCheck);
                await _context.SaveChangesAsync();
                var lastadded = await _context.WellChecks.LastAsync();
                var directory = _env.WebRootPath + "\\WellChecks\\" + lastadded.Id.ToString() + "\\";
                if (!Directory.Exists(directory) && files != null)
                {
                    Directory.CreateDirectory(directory);
                }
                if(files != null) {
                    foreach (IFormFile photo in files)
                    {
                        var path = Path.Combine(directory, photo.FileName);
                        var path2 = Path.Combine(directory, photo.FileName.Split(".")[0] + "_edit." + photo.FileName.Split(".")[1]);
                        PhotoFileWellCheck wellcheckphoto = new PhotoFileWellCheck { Path = path, TimeStamp = DateTime.Now,WellCheckId = Convert.ToInt32(lastadded.Id)};
                        _context.Add(wellcheckphoto);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await photo.CopyToAsync(stream);
                        };
                        if (path.ToLower().Contains(".jpg") || path.ToLower().Contains(".jpeg"))
                        {
                            PhotoExtensions.SaveAndCompressJpeg(path, 90);
                        }

                        //
                        //byte[] imageData = new byte[fileUpload.ContentLength];
                        //fileUpload.InputStream.Read(imageData, 0, fileUpload.ContentLength);

                        //MemoryStream ms = new MemoryStream(imageData);
                        //Image originalImage = Image.FromStream(ms);

                        //if (originalImage.PropertyIdList.Contains(0x0112))
                        //{
                        //    int rotationValue = originalImage.GetPropertyItem(0x0112).Value[0];
                        //    switch (rotationValue)
                        //    {
                        //        case 1: // landscape, do nothing
                        //            break;

                        //        case 8: // rotated 90 right
                        //                // de-rotate:
                        //            originalImage.RotateFlip(rotateFlipType: RotateFlipType.Rotate270FlipNone);
                        //            break;

                        //        case 3: // bottoms up
                        //            originalImage.RotateFlip(rotateFlipType: RotateFlipType.Rotate180FlipNone);
                        //            break;

                        //        case 6: // rotated 90 left
                        //            originalImage.RotateFlip(rotateFlipType: RotateFlipType.Rotate90FlipNone);
                        //            break;
                        //    }
                        //}
                        //

                    }
                
                    await _context.SaveChangesAsync();
                }


                return RedirectToAction(nameof(Index));
            }
            return View(wellCheck);
        }

        // GET: WellChecks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["ProjectId"] = await GetProjectList();
            if (id == null)
            {
                return NotFound();
            }

            var wellCheck = await _context.WellChecks.FindAsync(id);
            if (wellCheck == null)
            {
                return NotFound();
            }
            return View(wellCheck);
        }

        // POST: WellChecks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,ProjectId,WellName,CanBeFound,Comments,Dip,BottomWell,DiffTop,Dip2,BottomWell2,DiffTop2,Dip3,BottomWell3,DiffTop3,NumBerOfPipes,IsCoverOk,IsShaftOk,WellHeads,DoneBy,TimeStamp")] WellCheck wellCheck, IFormFile[] files)
        {
            if (id != wellCheck.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wellCheck);
                    await _context.SaveChangesAsync();

                    var lastadded = await _context.WellChecks.SingleOrDefaultAsync(x => x.Id.Equals(wellCheck.Id));
                    var directory = _env.WebRootPath + "\\WellChecks\\" + lastadded.Id.ToString() + "\\";
                    if (!Directory.Exists(directory) && files != null)
                    {
                        Directory.CreateDirectory(directory);
                    }
                    if (files != null)
                    {
                        foreach (IFormFile photo in files)
                        {
                            var path = Path.Combine(directory, photo.FileName);
                            var path2 = Path.Combine(directory, photo.FileName.Split(".")[0] + "_edit." + photo.FileName.Split(".")[1]);
                            PhotoFileWellCheck wellcheckphoto = new PhotoFileWellCheck { Path = path, TimeStamp = DateTime.Now, WellCheckId = Convert.ToInt32(lastadded.Id) };
                            _context.Add(wellcheckphoto);

                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await photo.CopyToAsync(stream);
                            };
                            if (path.ToLower().Contains(".jpg") || path.ToLower().Contains(".jpeg"))
                            {
                                PhotoExtensions.SaveAndCompressJpeg(path, 90);
                            }

                        }

                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WellCheckExists(wellCheck.Id))
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
            return View(wellCheck);
        }

        // GET: WellChecks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wellCheck = await _context.WellChecks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (wellCheck == null)
            {
                return NotFound();
            }

            return View(wellCheck);
        }

        // POST: WellChecks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var wellCheck = await _context.WellChecks.FindAsync(id);
            var photos = await _context.WellCheckPhotos.Where(x => x.WellCheckId.Equals(id)).ToListAsync();
            foreach(var photo in photos)
            {
                _context.Remove(photo);
            }
            await _context.SaveChangesAsync();
            _context.WellChecks.Remove(wellCheck);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WellCheckExists(int? id)
        {
            return _context.WellChecks.Any(e => e.Id == id);
        }
    }
}
