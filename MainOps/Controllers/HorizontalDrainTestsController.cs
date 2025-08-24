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
using System.Drawing.Imaging;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ImageMagick;
using Docnet.Core;
using Rotativa.AspNetCore;

namespace MainOps.Controllers
{
    public class HorizontalDrainTestsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public HorizontalDrainTestsController(DataContext context,UserManager<ApplicationUser> userManager, IWebHostEnvironment env):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // GET: HorizontalDrainTests
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.HorizontalDrainTests.Include(h => h.Install).Include(h => h.Project).Include(h => h.SubProject);
            return View(await dataContext.ToListAsync());
        }

        // GET: HorizontalDrainTests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            
            var horizontalDrainTest = await _context.HorizontalDrainTests
                .Include(h => h.Install)
                .Include(h => h.Project).ThenInclude(x=>x.Division)
                .Include(h => h.SubProject)
                .Include(h => h.Photos)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (horizontalDrainTest == null)
            {
                return NotFound();
            }
            if (!horizontalDrainTest.Project.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("Error", "Home", new { text = "You do not have permission to view this test" });
            }
            return new ViewAsPdf(horizontalDrainTest);
        }

        // GET: HorizontalDrainTests/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            HorizontalDrainTest model = new HorizontalDrainTest();
            if (user.DivisionId.Equals(1)) { 
                var installations = await _context.Installations.Include(x => x.ItemType).Where(x => x.ItemType.Item_Type.ToLower().Contains("horizontal") && x.ProjectId.Equals(60)).ToListAsync();
                ViewData["InstallId"] = new SelectList(_context.Installations.Include(x => x.Project).Include(x => x.ItemType).Where(x => x.ItemType.Item_Type.ToLower().Contains("horizontal") && x.ProjectId.Equals(60)), "Id", "Id");
                model.Installations = installations;
                model.ProjectId = 60;
            }
            else
            {
                var installations = await _context.Installations.Include(x => x.Project).Include(x => x.ItemType).Where(x => x.ItemType.Item_Type.ToLower().Contains("horizontal") && x.Project.DivisionId.Equals(user.DivisionId)).ToListAsync();
                ViewData["InstallId"] = new SelectList(_context.Installations.Include(x=>x.Project).Include(x => x.ItemType).Where(x => x.ItemType.Item_Type.ToLower().Contains("horizontal") && x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Id");
                model.Installations = installations;
            }
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Name");
            return View(model);
        }

        // POST: HorizontalDrainTests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(900000000)]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,SubProjectId,InstallId,Location,Distance,TimeStamp,Duration,LogText,DoneBy,EnteredIntoDataBase,LastEditedInDataBase,Signature")] HorizontalDrainTest horizontalDrainTest,IFormFile[] files)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                horizontalDrainTest.DoneBy = user.full_name();
                horizontalDrainTest.EnteredIntoDataBase = DateTime.Now;
                horizontalDrainTest.LastEditedInDataBase = DateTime.Now;
                _context.Add(horizontalDrainTest);
                await _context.SaveChangesAsync();
                //
                if (files != null)
                {
                    var lastadded = await _context.HorizontalDrainTests.LastAsync();
                    var directory = _env.WebRootPath + "\\AHAK\\DrainTests\\" + lastadded.Id.ToString() + "\\";
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    foreach (IFormFile photo in files)
                    {
                        var path = Path.Combine(directory, photo.FileName);
                        var path2 = Path.Combine(directory, photo.FileName.Split(".")[0] + "_edit." + photo.FileName.Split(".")[1]);
                        if (!path.Contains(".pdf"))
                        {
                            PhotoFileHorizontalDrainTest photofile = new PhotoFileHorizontalDrainTest { Path = path, TimeStamp = lastadded.TimeStamp, HorizontalDrainTestId = lastadded.Id, Latitude = 0, Longitude = 0 };
                            _context.Add(photofile);
                        }
                        else
                        {
                            PhotoFileHorizontalDrainTest newphoto = new PhotoFileHorizontalDrainTest { Latitude = 0, Longitude = 0, HorizontalDrainTestId = lastadded.Id, Path = path.Replace(".pdf", ".png"), TimeStamp = lastadded.TimeStamp };
                            _context.Add(newphoto);
                        }
                        var stream = new FileStream(path, FileMode.Create);
                        await photo.CopyToAsync(stream);
                        stream.Close();

                        if (path.ToLower().Contains(".jpg") || path.ToLower().Contains(".jpeg"))
                        {
                            SaveAndCompressJpeg(path, 80);
                        }
                        else if (path.Contains(".pdf"))
                        {
                            ConvertPdfToPng(path);
                            System.IO.File.Delete(path);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                //
                return RedirectToAction("MainMenu","TrackItems");
            }
            ViewData["InstallId"] = new SelectList(_context.Installations, "Id", "Id", horizontalDrainTest.InstallId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Abbreviation", horizontalDrainTest.ProjectId);
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Name", horizontalDrainTest.SubProjectId);
            return View(horizontalDrainTest);
        }

        // GET: HorizontalDrainTests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var horizontalDrainTest = await _context.HorizontalDrainTests.FindAsync(id);
            if (horizontalDrainTest == null)
            {
                return NotFound();
            }
            ViewData["InstallId"] = new SelectList(_context.Installations.Where(x=>x.ProjectId.Equals(horizontalDrainTest.ProjectId)), "Id", "Id", horizontalDrainTest.InstallId);
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x=>x.ProjectId.Equals(horizontalDrainTest.ProjectId)), "Id", "Name", horizontalDrainTest.SubProjectId);
            return View(horizontalDrainTest);
        }

        // POST: HorizontalDrainTests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(900000000)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,SubProjectId,InstallId,Location,Distance,TimeStamp,Duration,LogText,DoneBy,EnteredIntoDataBase,LastEditedInDataBase,Signature")] HorizontalDrainTest horizontalDrainTest, IFormFile[] files)
        {
            if (id != horizontalDrainTest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    horizontalDrainTest.LastEditedInDataBase = DateTime.Now;
                    _context.Update(horizontalDrainTest);
                    await _context.SaveChangesAsync();
                    if (files != null)
                    {
                        var lastadded = horizontalDrainTest;
                        var directory = _env.WebRootPath + "\\AHAK\\DrainTests\\" + lastadded.Id.ToString() + "\\";
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }
                        foreach (IFormFile photo in files)
                        {
                            var path = Path.Combine(directory, photo.FileName);
                            var path2 = Path.Combine(directory, photo.FileName.Split(".")[0] + "_edit." + photo.FileName.Split(".")[1]);
                            if (!path.Contains(".pdf"))
                            {
                                PhotoFileHorizontalDrainTest photofile = new PhotoFileHorizontalDrainTest { Path = path, TimeStamp = lastadded.TimeStamp, HorizontalDrainTestId = lastadded.Id, Latitude = 0, Longitude = 0 };
                                _context.Add(photofile);
                            }
                            else
                            {
                                PhotoFileHorizontalDrainTest newphoto = new PhotoFileHorizontalDrainTest { Latitude = 0, Longitude = 0, HorizontalDrainTestId = lastadded.Id, Path = path.Replace(".pdf", ".png"), TimeStamp = lastadded.TimeStamp };
                                _context.Add(newphoto);
                            }
                            var stream = new FileStream(path, FileMode.Create);
                            await photo.CopyToAsync(stream);
                            stream.Close();

                            if (path.ToLower().Contains(".jpg") || path.ToLower().Contains(".jpeg"))
                            {
                                SaveAndCompressJpeg(path, 80);
                            }
                            else if (path.Contains(".pdf"))
                            {
                                ConvertPdfToPng(path);
                                System.IO.File.Delete(path);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HorizontalDrainTestExists(horizontalDrainTest.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["InstallId"] = new SelectList(_context.Installations, "Id", "Id", horizontalDrainTest.InstallId);
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Name", horizontalDrainTest.SubProjectId);
            return View(horizontalDrainTest);
        }

        // GET: HorizontalDrainTests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var horizontalDrainTest = await _context.HorizontalDrainTests
                .Include(h => h.Install)
                .Include(h => h.Project)
                .Include(h => h.SubProject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (horizontalDrainTest == null)
            {
                return NotFound();
            }

            return View(horizontalDrainTest);
        }

        // POST: HorizontalDrainTests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var horizontalDrainTest = await _context.HorizontalDrainTests.FindAsync(id);
            var photos = await _context.PhotoFileHorizontalDrainTests.Where(x => x.HorizontalDrainTestId.Equals(horizontalDrainTest.Id)).ToListAsync();
            foreach(var photo in photos)
            {
                _context.Remove(photo);
            }
            await _context.SaveChangesAsync();
            _context.HorizontalDrainTests.Remove(horizontalDrainTest);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HorizontalDrainTestExists(int id)
        {
            return _context.HorizontalDrainTests.Any(e => e.Id == id);
        }
        private byte[] RearrangeBytesToRGBA(byte[] BGRABytes)
        {
            var max = BGRABytes.Length;
            var RGBABytes = new byte[max];
            var idx = 0;
            byte r;
            byte g;
            byte b;
            byte a;
            while (idx < max)
            {
                // get colors in original order: B G R A
                b = BGRABytes[idx];
                g = BGRABytes[idx + 1];
                r = BGRABytes[idx + 2];
                a = BGRABytes[idx + 3];

                // re-arrange to be in new order: R G B A
                RGBABytes[idx] = r;
                RGBABytes[idx + 1] = g;
                RGBABytes[idx + 2] = b;
                RGBABytes[idx + 3] = a;

                idx += 4;
            }
            return RGBABytes;
        }
        public MemoryStream PdfToImage(byte[] pdfBytes)
        {
            MemoryStream memoryStream = new MemoryStream();
            MagickImage imgBackdrop;
            MagickColor backdropColor = MagickColors.White; // replace transparent pixels with this color 
            int pdfPageNum = 0; // first page is 0

            using (IDocLib pdfLibrary = DocLib.Instance)
            {
                using (var docReader = pdfLibrary.GetDocReader(pdfBytes, new Docnet.Core.Models.PageDimensions(1.0d)))
                {
                    using (var pageReader = docReader.GetPageReader(pdfPageNum))
                    {
                        var rawBytes = pageReader.GetImage(); // Returns image bytes as B-G-R-A ordered list.
                        rawBytes = RearrangeBytesToRGBA(rawBytes);
                        var width = pageReader.GetPageWidth();
                        var height = pageReader.GetPageHeight();

                        // specify that we are reading a byte array of colors in R-G-B-A order.
                        PixelReadSettings pixelReadSettings = new PixelReadSettings(width, height, StorageType.Char, PixelMapping.RGBA);
                        using (MagickImage imgPdfOverlay = new MagickImage(rawBytes, pixelReadSettings))
                        {
                            // turn transparent pixels into backdrop color using composite: http://www.imagemagick.org/Usage/compose/#compose
                            imgBackdrop = new MagickImage(backdropColor, width, height);
                            imgBackdrop.Composite(imgPdfOverlay, CompositeOperator.Over);
                        }
                    }
                }
            }


            imgBackdrop.Write(memoryStream, MagickFormat.Png);
            imgBackdrop.Dispose();
            memoryStream.Position = 0;
            return memoryStream;
        }
        public void ConvertPdfToPng(string path)
        {
            if (path.Contains(".pdf"))
            {
                string[] fileparts = path.Split(".");
                string newname = fileparts[0] + "_edit" + ".png";
                if (!System.IO.File.Exists(path.Replace(".pdf", ".png")) && !System.IO.File.Exists(newname))
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(path);
                    MemoryStream ms = PdfToImage(bytes);
                    using (FileStream fileToSave = new FileStream(newname, FileMode.Create, System.IO.FileAccess.Write))
                    {
                        ms.CopyTo(fileToSave);
                    }
                    ms.Close();
                }
            }
        }
        
        [RequestSizeLimit(900000000)]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember")]
        public static void SaveAndCompressJpeg(string inputPath, int qualityIn)
        {

            int size = 300;
            int quality;
            if (qualityIn >= 80)
            {
                quality = qualityIn;
            }
            else
            {
                quality = 80;
            }
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
    }
}
