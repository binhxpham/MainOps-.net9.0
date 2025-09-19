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
using Microsoft.AspNetCore.Hosting;
using System.IO;
using MainOps.ExtensionMethods;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Microsoft.AspNetCore.Identity;

namespace MainOps.Controllers
{
    public class CGJItemClassesController : BaseController
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;

        public CGJItemClassesController(DataContext context, IWebHostEnvironment env,UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }
        [HttpPost]
        public async Task<IActionResult> ChangeClasses(int? id,int? newMasterId,int? newClassNum)
        {
            if (id != null && newMasterId != null)
            {
                var masterclass = await _context.HJItemMasterClasses.FindAsync(newMasterId);
                var classitem = await _context.HJItemClasses.FindAsync(id);
                classitem.HJItemMasterClassId = newMasterId;
                classitem.ClassNumber = newClassNum.ToString();
                
                _context.Update(classitem);
                var allitems = await _context.HJItems.Where(x => x.HJItemClassId.Equals(id)).OrderBy(x => x.HJId).ToListAsync();
                var prev_item = await _context.HJItems.Include(x => x.HJItemClass).Where(x => x.HJItemClass.ClassNumber.Equals(classitem.ClassNumber) && x.HJItemClass.HJItemMasterClassId.Equals(newMasterId)).ToListAsync();
                int startnum = 0;
                if (prev_item.Count > 0)
                {

                    startnum = Convert.ToInt32(prev_item.OrderByDescending(x => x.HJId).First().HJId.Split("-")[2]);
                }
                foreach (var item in allitems)
                {
                    item.SetHJNumber(startnum, masterclass.ClassNumber, newClassNum.ToString());
                    _context.Update(item);
                    startnum += 1;

                }
            }
            else
            {
                return NotFound();
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // GET: HJItemClasses
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["CGJItemMasterClassId"] = new SelectList(_context.CGJItemMasterClasses.Where(x => x.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.ClassNumber), "Id", "ClassName");
            if (User.IsInRole("Admin"))
            {
                return View(await _context.CGJItemClasses.Include(x => x.CGJItemMasterClass)
                    .OrderBy(x => x.CGJItemMasterClass.ClassNumber).ThenBy(x => x.ClassNumber).ToListAsync());
            }
            else
            {
                return View(await _context.CGJItemClasses.Include(x => x.CGJItemMasterClass).Where(x=>x.CGJItemMasterClass.DivisionId.Equals(user.DivisionId))
                    .OrderBy(x => x.CGJItemMasterClass.ClassNumber).ThenBy(x=>x.ClassNumber).ToListAsync());
            }
        }

        // GET: CGJItemClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var cgJItemClass = await _context.CGJItemClasses.Include(x=>x.CGJItemMasterClass)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cgJItemClass == null)
            {
                return NotFound();
            }
            //else if(!cgJItemClass.CGJItemMasterClass.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            else if ((cgJItemClass?.CGJItemMasterClass?.DivisionId != user?.DivisionId) && !User.IsInRole("Admin"))         

            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
            }
            return View(cgJItemClass);
        }

        // GET: CGJItemClasses/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["CGJItemMasterClassId"] = new SelectList(_context.CGJItemMasterClasses.Where(x=>x.DivisionId.Equals(user.DivisionId)), "Id", "ClassName");
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "DivisionAdmin,Admin,Member,StorageManager")]
        public async Task<IActionResult> UploadPhotoHJClassItem(int? id)
        {
            if (id != null)
            {

                var item = await _context.CGJItemClasses.FindAsync(id);
                //var user = await _userManager.GetUserAsync(User);
                var directory = Path.Combine(_env.WebRootPath.ReplaceFirst("/", ""), "Images", "ItemClasses", "Photos", id.ToString());
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                if (HttpContext.Request.Form.Files != null)
                {
                    var files = HttpContext.Request.Form.Files;

                    foreach (IFormFile photo in files)
                    {

                        var path = Path.Combine(directory, photo.FileName);

                        var stream = new FileStream(path, FileMode.Create);
                        await photo.CopyToAsync(stream);
                        stream.Close();
                        if (path.Contains(".jpg") || path.Contains(".jpeg"))
                        {
                            SaveAndCompressJpeg(path, 80);
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }
        [RequestSizeLimit(900000000)]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember")]
        public static void SaveAndCompressJpeg(string inputPath, int qualityIn)
        {

            int size = 600;
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClassName,ClassNumber,CGJItemMasterClassId,Service_Maintenance_Freq,Safety_Maintenance_Freq,Electrical_Maintenance_Freq,Internal_Rent")] CGJItemClass cgJItemClass)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cgJItemClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CGJItemMasterClassId"] = new SelectList(_context.CGJItemMasterClasses, "Id", "ClassName");
            return View(cgJItemClass);
        }

        // GET: CGJItemClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var cgJItemClass = await _context.CGJItemClasses.Include(x=>x.CGJItemMasterClass).FirstOrDefaultAsync(x=>x.Id.Equals(id));
            if (cgJItemClass == null)
            {
                return NotFound();
            }
            else if (cgJItemClass.CGJItemMasterClass?.DivisionId !=(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
            }
            ViewData["CGJItemMasterClassId"] = new SelectList(_context.CGJItemMasterClasses, "Id", "ClassName");
            return View(cgJItemClass);
        }

        // POST: CGJItemClasses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClassName,ClassNumber,CGJItemMasterClassId,Service_Maintenance_Freq,Safety_Maintenance_Freq,Electrical_Maintenance_Freq,Internal_Rent")] CGJItemClass cgJItemClass)
        {
            if (id != cgJItemClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cgJItemClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CGJItemClassExists(cgJItemClass.Id))
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
            ViewData["CGJItemMasterClassId"] = new SelectList(_context.CGJItemMasterClasses, "Id", "ClassName");
            return View(cgJItemClass);
        }

        // GET: CGJItemClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var cgJItemClass = await _context.CGJItemClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cgJItemClass == null)
            {
                return NotFound();
            }
            else if (cgJItemClass.CGJItemMasterClass?.DivisionId != (user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
            }

            return View(cgJItemClass);
        }

        // POST: CGJItemClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hJItemClass = await _context.CGJItemClasses.FindAsync(id);
            _context.CGJItemClasses.Remove(hJItemClass);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CGJItemClassExists(int id)
        {
            return _context.CGJItemClasses.Any(e => e.Id == id);
        }
    }
}
