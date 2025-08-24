using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MainOps.Data;
using MainOps.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Http;
using ImageMagick;
using Docnet.Core;
using System.Text.RegularExpressions;
using MainOps.ExtensionMethods;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,DivisionAdmin,Manager,Member,ProjectMember,Guest")]
    public class ExtraWorksController : BaseController
    {
        private readonly DataContext _context;
        private IWebHostEnvironment _env;
        private UserManager<ApplicationUser> _userManager;

        public ExtraWorksController(DataContext context, IWebHostEnvironment env, UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        // GET: ExtraWorks
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,Member,ProjectMember")]
        public async Task<IActionResult> Index()
        {
            IEnumerable<SelectListItem> selList = await createFilterlist();
            ViewData["Filterchoices"] = new SelectList(selList, "Value", "Text");
            ViewData["ExtraWorkTypeId"] = new SelectList(_context.BoQHeadLines.Where(x=>x.Type.Equals("ExtraWork")),"Id","HeadLine");
            var dataContext = _context.ExtraWorks.Include(e => e.Project).Include(e => e.SubProject).Include(x=>x.BoQHeadLine);
            if (User.IsInRole("Admin"))
            {
                return View(await dataContext.OrderByDescending(x => x.TimeStamp).ToListAsync());
            }
            var user = await _userManager.GetUserAsync(User);
            return View(await dataContext.Where(x=>x.Project.DivisionId.Equals(user.DivisionId)).OrderByDescending(x=>x.TimeStamp).ToListAsync());
        }
        public async Task<IEnumerable<SelectListItem>> createFilterlist()
        {

            List<Project> filternames = new List<Project>();
            var theuser = await _userManager.GetUserAsync(HttpContext.User);
            if (HttpContext.User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
            {

                filternames = await (from pr in _context.Projects
                                     join pu in _context.ProjectUsers on pr.Id
                                     equals pu.projectId
                                     where pr.Active.Equals(true) && pu.userId == theuser.Id && theuser.DivisionId.Equals(pr.DivisionId)
                                     select pr).OrderBy(x => x.Name).ToListAsync();
            }
            else
            {
                if (User.IsInRole("Admin"))
                {
                    filternames = await _context.Projects.Include(x => x.Division).OrderBy(x => x.Division.Name).OrderBy(b => b.Name).ToListAsync();
                }
                else
                {
                    filternames = await _context.Projects.Where(x => x.DivisionId.Equals(theuser.DivisionId) && x.Active.Equals(true))
                .OrderBy(b => b.Name).ToListAsync();
                }

            }

            IEnumerable<SelectListItem> selList = from s in filternames
                                                  select new SelectListItem
                                                  {
                                                      Value = s.Id.ToString(),
                                                      Text = s.Name
                                                  };
            return selList;
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
        [HttpGet]
        public IActionResult DeleteFiles(int? Id)
        {
            if(Id == null)
            {
                return NotFound();
            }
            string path = _env.WebRootPath + "/AHAK/ExtraWorks/" + Id.ToString() + "/";
            var folder = Directory.EnumerateFiles(path)
                                     .Select(fn => Path.GetFileName(fn));

            foreach (string file in folder)
            {
                System.IO.File.Delete(path + file);
            }
            return RedirectToAction(nameof(Index));
        }
        // GET: ExtraWorks/Details/5
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,Member,ProjectMember,Guest")]
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = await _context.ExtraWorks
                .Include(e => e.Project).ThenInclude(x=>x.Division)
                .Include(x => x.SubProject)
                .Include(x => x.BoQHeadLine)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (model == null)
            {
                return NotFound();
            }
            var theuser = await _userManager.GetUserAsync(User);
            if (((from p in _context.ProjectUsers where p.userId.Equals(theuser.Id) select p.projectId).ToList().Contains(Convert.ToInt32(model.ProjectId))) || !User.IsInRole("Guest"))
            {
                string path = _env.WebRootPath + "/AHAK/ExtraWorks/" + model.Id.ToString() + "/";
                List<string> pictures = new List<string>();
                List<string> pdf2s = new List<string>();
                if (Directory.Exists(path))
                {
                    var folder = Directory.EnumerateFiles(path)
                                     .Select(fn => Path.GetFileName(fn));

                    foreach (string file in folder)
                    {
                        if (file.Contains(".png"))
                        {
                            pdf2s.Add(file.Replace(".png", ".pdf"));
                        }
                        else if (file.Contains("_edit"))
                        {
                            if (file.Contains(".pdf"))
                            {
                                int numberofpages = PhotoExtensions.getNumberOfPdfPages(path + file);
                                if (numberofpages < 1)
                                {
                                    numberofpages = 30;
                                }
                                for (int i = 0; i < numberofpages; i++)
                                {
                                    try
                                    {
                                        if (!System.IO.File.Exists(path + file.Split(".pdf")[0] + "_" + i.ToString() + ".png"))
                                        {
                                            byte[] bytes = System.IO.File.ReadAllBytes(path + file);
                                            MemoryStream ms = PhotoExtensions.PdfToImage(bytes, i);
                                            string filename = file.Split(".pdf")[0] + "_" + i.ToString() + ".pdf";
                                            using (FileStream fileToSave = new FileStream(path + filename.Replace(".pdf", ".png"), FileMode.Create, System.IO.FileAccess.Write))
                                            {
                                                ms.CopyTo(fileToSave);
                                            }
                                            ms.Close();
                                            pdf2s.Add(filename);
                                        }
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                            else
                            {
                                if (!System.IO.File.Exists(path + file.Replace(".png", ".pdf")))
                                {
                                    pictures.Add(file);
                                }
                            }

                        }
                        else
                        {
                            string[] fileparts = file.Split(".");
                            if (!folder.Contains(fileparts[0] + "_edit." + fileparts[1]))
                            {
                                if (file.Contains(".pdf"))
                                {
                                    int numberofpages = PhotoExtensions.getNumberOfPdfPages(path + file);
                                    if(numberofpages < 1)
                                    {
                                        numberofpages = 30;
                                    }
                                    for (int i = 0; i < numberofpages; i++)
                                    {
                                        try { 
                                            if (!System.IO.File.Exists(path + file.Split(".pdf")[0] + "_" + i.ToString() + ".png"))
                                            {
                                                byte[] bytes = System.IO.File.ReadAllBytes(path + file);
                                                MemoryStream ms = PhotoExtensions.PdfToImage(bytes, i);
                                                string filename = file.Split(".pdf")[0] + "_" + i.ToString() + ".pdf";
                                                using (FileStream fileToSave = new FileStream(path + filename.Replace(".pdf", ".png"), FileMode.Create, System.IO.FileAccess.Write))
                                                {
                                                    ms.CopyTo(fileToSave);
                                                }
                                                ms.Close();
                                                pdf2s.Add(filename);
                                            }
                                        }
                                        catch {

                                        }
                                    }
                                }
                                else
                                {
                                    if (!System.IO.File.Exists(path + file.Replace(".png", ".pdf")))
                                    {
                                        pictures.Add(file);
                                    }
                                }
                            }
                        }
                    }
                }
                model.pictures = pictures;
                model.pdf2s = pdf2s;
                return new ViewAsPdf("_ExtraWork", model);
            }
            else
            {
                return NotFound();
            }

        }

        // GET: ExtraWorks/Create
        [HttpGet]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember")]
        public async Task<IActionResult> Create()
        {
            ViewData["ProjectId"] = await GetProjectList();
            //ViewData["BoQHeadlineId"] = new SelectList(_context.BoQHeadLines.Where(x => x.ProjectId.Equals(extraWork.ProjectId) && x.Type.Equals("ExtraWork")), "Id", "HeadLine");
            return View();
        }

        // POST: ExtraWorks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember")]
        public async Task<IActionResult> UploadPhotoOrPdf(int? id,IFormFile[] files)
        {
            if(id != null)
            {
                var ew = await _context.ExtraWorks.Where(x=>x.Id.Equals(id)).SingleOrDefaultAsync();
                if(ew != null)
                {
                    var folderpath = _env.WebRootPath + "\\AHAK\\ExtraWorks\\" + ew.Id.ToString() + "\\";
                    if (!Directory.Exists(folderpath) && files != null)
                    {
                        Directory.CreateDirectory(folderpath);
                    }
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            //Getting FileName
                            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            fileName = folderpath + $@"\{fileName}";

                            using (FileStream fs = System.IO.File.Create(fileName))
                            {
                                await file.CopyToAsync(fs);
                                fs.Flush();
                            }
                            if (fileName.ToLower().Contains(".jpg") || fileName.ToLower().Contains(".jpeg"))
                            {
                                PhotoExtensions.SaveAndCompressJpeg(fileName, 85);
                            }
                        }
                    }
                    return RedirectToAction(nameof(Index));
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEndStamps()
        {
            var ews = await _context.ExtraWorks.Where(x => x.Rental_Price < (decimal)0.01 && x.EndStamp == null).ToListAsync();
            foreach(ExtraWork ew in ews)
            {
                ew.EndStamp = ew.TimeStamp;
                _context.Update(ew);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(500000000)]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,SubProjectId,TimeStamp,EndStamp,Description,Price,Rental_Price,BoQHeadLineId,Valuta,VAT_Liftable")] ExtraWork extraWork)
        {
            if (ModelState.IsValid)
            {
                if(extraWork.Rental_Price < (decimal)0.01 && extraWork.EndStamp == null)
                {
                    extraWork.EndStamp = extraWork.TimeStamp;
                }
                extraWork.InvoiceDate = DateTime.Now;
                _context.Add(extraWork);
                await _context.SaveChangesAsync();
                var folderpath = _env.WebRootPath + "\\AHAK\\ExtraWorks\\" + extraWork.Id.ToString() + "\\";
                if (!Directory.Exists(folderpath) && HttpContext.Request.Form.Files != null)
                {
                    Directory.CreateDirectory(folderpath);
                }
                if (HttpContext.Request.Form.Files != null)
                {
                    var files = HttpContext.Request.Form.Files;
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            //Getting FileName
                            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            fileName = folderpath + $@"\{fileName}";

                            using (FileStream fs = System.IO.File.Create(fileName))
                            {
                                await file.CopyToAsync(fs);
                                fs.Flush();
                            }
                            if (fileName.Contains(".jpg") || fileName.Contains(".jpeg"))
                            {
                                PhotoExtensions.SaveAndCompressJpeg(fileName, 95);
                            }
                        }
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = await GetProjectList();
            return View(extraWork);
        }
        // GET: ExtraWorks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var extraWork = await _context.ExtraWorks.FindAsync(id);
            if (extraWork == null)
            {
                return NotFound();
            }
            ViewData["BoQHeadlineId"] = new SelectList(_context.BoQHeadLines.Where(x => x.ProjectId.Equals(extraWork.ProjectId) && x.Type.Equals("ExtraWork")), "Id", "HeadLine");
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => x.ProjectId.Equals(extraWork.ProjectId)), "Id", "Name");
            ViewData["ProjectId"] = await GetProjectList();
            return View(extraWork);
        }

        // POST: ExtraWorks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(500000000)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,SubProjectId,TimeStamp,EndStamp,Description,Price,Rental_Price,BoQHeadLineId,Valuta,VAT_Liftable,InvoiceDate,PaidAmount,PaidAmountRental")] ExtraWork extraWork, IFormFile[] files)
        {
            if (id != extraWork.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(extraWork);
                    await _context.SaveChangesAsync();
                    var folderpath = _env.WebRootPath + "\\AHAK\\ExtraWorks\\" + extraWork.Id.ToString() + "\\";
                    if (!Directory.Exists(folderpath) && files.Count() > 0)
                    {
                        Directory.CreateDirectory(folderpath);
                    }
                    if (files.Count() > 0)
                    {
                        //var files = HttpContext.Request.Form.Files;
                        foreach (var file in files)
                        {
                            if (file.Length > 0)
                            {
                                //Getting FileName
                                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                                fileName = folderpath + $@"\{fileName}";

                                using (FileStream fs = System.IO.File.Create(fileName))
                                {
                                    await file.CopyToAsync(fs);
                                    fs.Flush();
                                }
                                if (fileName.ToLower().Contains(".jpg") || fileName.ToLower().Contains(".jpeg"))
                                {
                                    PhotoExtensions.SaveAndCompressJpeg(fileName, 85);
                                }
                            }
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExtraWorkExists(extraWork.Id))
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
            return View(extraWork);
        }

        // GET: ExtraWorks/Delete/5
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            await CheckUser(user);
            var extraWork = await _context.ExtraWorks
                .Include(e => e.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if(!extraWork.Project.DivisionId.Equals(user) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this resource" });
            }
            if (extraWork == null)
            {
                return NotFound();
            }

            return View(extraWork);
        }

        // POST: ExtraWorks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var extraWork = await _context.ExtraWorks.FindAsync(id);
            _context.ExtraWorks.Remove(extraWork);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExtraWorkExists(int id)
        {
            return _context.ExtraWorks.Any(e => e.Id == id);
        }
        
        public async Task<JsonResult> AutoComplete(string search)
        {
            var user = await _userManager.GetUserAsync(User);
            List<ExtraWork> results = new List<ExtraWork>();
            if (User.IsInRole("Admin"))
            {
                results = _context.ExtraWorks
                                    .Include(x => x.Project).ThenInclude(x => x.Division)
                                    .Where(x => x.Description.ToLower().Contains(search.ToLower()) || x.Project.Name.ToLower().Contains(search.ToLower())).ToList();
            }
            else
            {
                results = _context.ExtraWorks
                .Include(x => x.Project).ThenInclude(x => x.Division)
                .Where(x => x.Project.DivisionId.Equals(user.DivisionId) && x.Description.ToLower().Contains(search.ToLower()) || x.Project.Name.ToLower().Contains(search.ToLower())).ToList();
            }


            return Json(results.Select(m => new
            {
                id = m.Id,
                value = m.Description,
                label = m.Project.Name + '_' + m.Description
            }).OrderBy(x => x.label));
        }
        [HttpPost]
        public async Task<IActionResult> Combsearch(string searchstring, string filterchoice, int? ExtraWorkType,DateTime startDate,DateTime endDate)
        {
            int f_c_converted;
            f_c_converted = Convert.ToInt32(filterchoice);
            var theuser = await _userManager.GetUserAsync(HttpContext.User);
            IEnumerable<SelectListItem> selList = await createFilterlist();
            ViewData["Filterchoices"] = new SelectList(selList, "Value", "Text");
            if(filterchoice != null)
            {
                ViewData["ExtraWorkTypeId"] = new SelectList(_context.BoQHeadLines.Where(x => x.ProjectId.Equals(f_c_converted) && x.Type.Equals("ExtraWork")), "Id", "HeadLine");
            }
            
            List<ExtraWork> items = new List<ExtraWork>();
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(searchstring) && (string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
                {
                    return RedirectToAction(nameof(Index));
                }
                else if (string.IsNullOrEmpty(searchstring) && (!string.IsNullOrEmpty(filterchoice) || !filterchoice.Equals("All")))
                {

                    if (User.IsInRole("Admin"))
                    {
                        items = await _context.ExtraWorks
                    .Include(m => m.Project).ThenInclude(x => x.Division).Include(x => x.BoQHeadLine)
                   .Where(x => x.ProjectId.Equals(f_c_converted) && x.TimeStamp.Date >= startDate.Date && x.TimeStamp.Date <= endDate.Date)
                   .OrderByDescending(x => x.TimeStamp).ToListAsync();
                    }
                    else
                    {
                        items = await _context.ExtraWorks
                    .Include(m => m.Project).ThenInclude(x => x.Division).Include(x => x.BoQHeadLine)
                    .Where(x => x.ProjectId.Equals(f_c_converted) && x.Project.DivisionId.Equals(theuser.DivisionId) && x.TimeStamp.Date >= startDate.Date && x.TimeStamp.Date <= endDate.Date)
                    .OrderByDescending(x => x.TimeStamp).ToListAsync();
                    }

                }
                else if (!string.IsNullOrEmpty(searchstring) && (string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
                {

                    if (User.IsInRole("Admin"))
                    {
                        items = await _context.ExtraWorks
                        .Include(m => m.Project).ThenInclude(x => x.Division).Include(x => x.BoQHeadLine)
                        .Where(x => x.Description.ToLower().Contains(searchstring.ToLower()) && x.TimeStamp.Date >= startDate.Date && x.TimeStamp.Date <= endDate.Date)
                        .OrderByDescending(x => x.TimeStamp).ToListAsync();
                    }
                    else
                    {
                        items = await _context.ExtraWorks
                        .Include(m => m.Project).ThenInclude(x => x.Division).Include(x => x.BoQHeadLine)
                        .Where(x => x.Description.ToLower().Contains(searchstring.ToLower()) && x.Project.DivisionId.Equals(theuser.DivisionId) && x.TimeStamp.Date >= startDate.Date && x.TimeStamp.Date <= endDate.Date)
                        .OrderByDescending(x => x.TimeStamp).ToListAsync();
                    }
                }
                else
                {
                    if (User.IsInRole("Admin"))
                    {
                        items = await _context.ExtraWorks
                        .Include(m => m.Project).ThenInclude(x => x.Division).Include(x => x.BoQHeadLine)
                        .Where(x => x.Description.ToLower().Contains(searchstring.ToLower()) && x.ProjectId.Equals(f_c_converted) && x.TimeStamp.Date >= startDate.Date && x.TimeStamp.Date <= endDate.Date) 
                        .OrderByDescending(x => x.TimeStamp).ToListAsync();
                    }
                    else
                    {
                        items = await _context.ExtraWorks
                        .Include(m => m.Project).ThenInclude(x => x.Division).Include(x => x.BoQHeadLine)
                        .Where(x => x.Description.ToLower().Contains(searchstring.ToLower()) && x.ProjectId.Equals(f_c_converted) && x.Project.DivisionId.Equals(theuser.DivisionId) && x.TimeStamp.Date >= startDate.Date && x.TimeStamp.Date <= endDate.Date)
                        .OrderByDescending(x => x.TimeStamp).ToListAsync();
                    }
                    
                }
                if(ExtraWorkType != null)
                {
                    items = items.Where(x => x.BoQHeadLineId.Equals(ExtraWorkType)).ToList();
                }
                return View(nameof(Index), items);
            }
            return View(nameof(Index));
        }
        
    }
}
