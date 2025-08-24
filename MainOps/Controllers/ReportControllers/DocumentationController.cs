using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MainOps.Data;
using MainOps.ExtensionMethods;
using MainOps.Models;
using MainOps.Models.ReportClasses;
using MainOps.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,DivisionAdmin,Member")]
    public class DocumentationController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public DocumentationController(DataContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostingenv)
        {
            _context = context;
            _userManager = userManager;
            _env = hostingenv;
        }
        public List<SelectListItem> generateList()
        {
            List<SelectListItem> mySkills = new List<SelectListItem>();
            var kstypes = _context.KSTypes.ToList();
            foreach(KSType kS in kstypes)
            {
                mySkills.Add(new SelectListItem
                {
                    Text = kS.name,
                    Value = kS.Id.ToString()
                });
            }
            return mySkills;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["MySkills"] =  generateList();
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
            }
            else
            {
                ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Email");
            }
            
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Generator_Test()
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
                return View();
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Name");
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Generator_Test(Generator_Test model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
            }
            else
            {
                ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Email");
            }
            return View(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> WTP_Test()
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
                return View();
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Name");
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> WTP_Test(WTP_Test model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
            }
            else
            {
                ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Email");
            }
            return View(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Well_Installation()
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
                return View();
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Name");
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Well_Installation(Well_Installation model)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
            }
            else
            {
                ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Email");
            }
            if (ModelState.IsValid)
            {
                var project = await _context.Projects.Include(x=>x.Division).Where(x=>x.Id.Equals(model.ProjectId)).FirstAsync();
                model.CustomerName = project.Client;
                model.HD_ProjectNr = project.ProjectNr;
                _context.Add(model);
                await _context.SaveChangesAsync();
                model = null;
                var item = await _context.Well_Installations.LastAsync();
                var folderpath = Path.Combine(_env.WebRootPath.ReplaceFirst("/", ""), "Images","Divisions", "WellInstallations",project.DivisionId.ToString(), item.Id.ToString());
                if (!Directory.Exists(folderpath))
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
                            item.Scetch_path = fileName;
                            fileName = folderpath + $@"\{fileName}";
                            
                            using (FileStream fs = System.IO.File.Create(fileName))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }
                            _context.Update(item);
                            await _context.SaveChangesAsync();

                        }
                    }
                }
                
                return RedirectToAction(nameof(Index));
            }

            return View(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Well_Drilling()
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
                return View();
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Name");
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Well_Drilling(Well_Drilling model)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
            }
            else
            {
                ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Email");
            }
            if (ModelState.IsValid)
            {
                var project = await _context.Projects.Include(x => x.Division).Where(x => x.Id.Equals(model.ProjectId)).FirstAsync();
                model.CustomerName = project.Client;
                model.HD_ProjectNr = project.ProjectNr;
                _context.Add(model);
                await _context.SaveChangesAsync();
                model = null;
                var item = await _context.Well_Drillings.LastAsync();
                var folderpath = Path.Combine(_env.WebRootPath.ReplaceFirst("/", ""), "Images", "Divisions", "WellDrillings", project.DivisionId.ToString(), item.Id.ToString());
                if (!Directory.Exists(folderpath))
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
                            item.Scetch_path = fileName;
                            fileName = folderpath + $@"\{fileName}";

                            using (FileStream fs = System.IO.File.Create(fileName))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }
                            _context.Update(item);
                            await _context.SaveChangesAsync();

                        }
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Pump_Installation()
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
                return View();
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Name");
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Pump_Installation(Pump_Installation model)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
            }
            else
            {
                ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Email");
            }
            if (ModelState.IsValid)
            {
                var project = await _context.Projects.Include(x => x.Division).Where(x => x.Id.Equals(model.ProjectId)).FirstAsync();
                model.CustomerName = project.Client;
                model.HD_ProjectNr = project.ProjectNr;
                _context.Add(model);
                await _context.SaveChangesAsync();
                model = null;
                var item = await _context.Pump_Installations.LastAsync();
                var folderpath = Path.Combine(_env.WebRootPath.ReplaceFirst("/", ""), "Images", "Divisions", "PumpInstallations", project.DivisionId.ToString(), item.Id.ToString());
                if (!Directory.Exists(folderpath))
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
                            item.Scetch_path = fileName;
                            fileName = folderpath + $@"\{fileName}";

                            using (FileStream fs = System.IO.File.Create(fileName))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }
                            _context.Update(item);
                            await _context.SaveChangesAsync();

                        }
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Pump_Commission()
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
                return View();
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Name");
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Pump_Commission(Pump_Commission model)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
            }
            else
            {
                ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Email");
            }
            if (ModelState.IsValid)
            {
                var project = await _context.Projects.Include(x => x.Division).Where(x => x.Id.Equals(model.ProjectId)).FirstAsync();
                model.CustomerName = project.Client;
                model.HD_ProjectNr = project.ProjectNr;
                _context.Add(model);
                await _context.SaveChangesAsync();
                model = null;
                var item = await _context.Pump_Commissions.LastAsync();
                var folderpath = Path.Combine(_env.WebRootPath.ReplaceFirst("/", ""), "Images", "Divisions", "PumpCommissions", project.DivisionId.ToString(), item.Id.ToString());
                if (!Directory.Exists(folderpath))
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
                            item.Scetch_path = fileName;
                            fileName = folderpath + $@"\{fileName}";

                            using (FileStream fs = System.IO.File.Create(fileName))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }
                            _context.Update(item);
                            await _context.SaveChangesAsync();

                        }
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Well_Development()
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
                return View();
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Name");
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Well_Development(Well_Development model)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
            }
            else
            {
                ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Email");
            }
            if (ModelState.IsValid)
            {
                var project = await _context.Projects.Include(x => x.Division).Where(x => x.Id.Equals(model.ProjectId)).FirstAsync();
                model.CustomerName = project.Client;
                model.HD_ProjectNr = project.ProjectNr;
                _context.Add(model);
                await _context.SaveChangesAsync();
                model = null;
                var item = await _context.Well_Developments.LastAsync();
                var folderpath = Path.Combine(_env.WebRootPath.ReplaceFirst("/", ""), "Images", "Divisions", "WellDevelopments", project.DivisionId.ToString(), item.Id.ToString());
                if (!Directory.Exists(folderpath))
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
                            item.Scetch_path = fileName;
                            fileName = folderpath + $@"\{fileName}";

                            using (FileStream fs = System.IO.File.Create(fileName))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }
                            _context.Update(item);
                            await _context.SaveChangesAsync();

                        }
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            
            return View(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Daily_Report()
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division), "Id", "Name");
                ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
                var machinery = _context.Machinery.Select(m => new {
                    Id = m.Id,
                    Name = m.MachineryName
                }).ToList();
                ViewBag.Machinery = new MultiSelectList(machinery, "Id", "Name");
                return View();
            }
            else
            {
                ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Project.DivisionId.Equals(theuser.DivisionId)), "Id", "Name");
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Name");
                var machinery = (from machineries in _context.Machinery select machineries).ToList();
                if(machinery != null)
                {
                    ViewBag.Machinery = machinery.Select(N => new SelectListItem { Text = N.MachineryName, Value = N.Id.ToString() });
                }
                //ViewBag.Machinery = new SelectList(machinery, "Id", "Name");
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Daily_Report(Daily_Report model)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
            }
            else
            {
                ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Email");
            }
            if (ModelState.IsValid)
            {
                if (model.MeasPointId.Equals(-1))
                {
                    model.MeasPointId = null;
                }
                var project = await _context.Projects.FindAsync(model.ProjectId);
                model.DoneBy = theuser.FirstName + " " + theuser.LastName;
                _context.Add(model);
                await _context.SaveChangesAsync();
                var item = await _context.Daily_Reports.LastAsync();
                var folderpath = Path.Combine(_env.WebRootPath.ReplaceFirst("/", ""), "Images", "Divisions", "Daily_Reports", project.DivisionId.ToString(), item.Id.ToString());
                if (!Directory.Exists(folderpath))
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
                                file.CopyTo(fs);
                                fs.Flush();
                            }
                        }
                    }
                }
                return RedirectToAction(nameof(Index));
            }


            return View(nameof(Index));
        }
        public async Task<IActionResult> Generator_DetailsPdf(int? id)
        {
            if(id != null)
            {
                var model = await _context.Generator_Test.Include(x => x.Project).Where(x => x.Id.Equals(id)).FirstAsync();
                return new ViewAsPdf("_Generator_Test_Report", model);
            }
            else
            {
                var theuser = await _userManager.GetUserAsync(User);
                if (User.IsInRole("Admin"))
                {
                    ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
                }
                else
                {
                    ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Email");
                }
                return RedirectToAction(nameof(Index));
            }
           
        }
        public async Task<IActionResult> WTP_DetailsPdf(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                var model = await _context.WTP_Tests.Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Id.Equals(id)).FirstAsync();
                return new ViewAsPdf("_WTP_Test_Report", model);
            }
            else
            {
                var theuser = await _userManager.GetUserAsync(User);
                if (User.IsInRole("Admin"))
                {
                    ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
                }
                else
                {
                    ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Email");
                }
                return RedirectToAction(nameof(Index));
            }

        }
        public async Task<IActionResult> Well_Installation_DetailsPdf(int? id)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
            }
            else
            {
                ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Email");
            }
            if (id != null)
            {
                var model = await _context.Well_Installations.Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Id.Equals(id)).FirstAsync();
                return new ViewAsPdf("_Well_Installation_Report", model);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
        public async Task<IActionResult> Well_Drilling_DetailsPdf(int? id)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
            }
            else
            {
                ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Email");
            }
            if (id != null)
            {
                var model = await _context.Well_Drillings.Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Id.Equals(id)).FirstAsync();
                return new ViewAsPdf("_Well_Drilling_Report", model);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
        public async Task<IActionResult> Pump_Installation_DetailsPdf(int? id)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
            }
            else
            {
                ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Email");
            }
            if (id != null)
            {
                var model = await _context.Pump_Installations.Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Id.Equals(id)).FirstAsync();
                return new ViewAsPdf("_Pump_Installation_Report", model);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }

        }
        public async Task<IActionResult> Pump_Commission_DetailsPdf(int? id)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
            }
            else
            {
                ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Email");
            }
            if (id != null)
            {
                var model = await _context.Pump_Commissions.Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Id.Equals(id)).FirstAsync();
                return new ViewAsPdf("_Pump_Commission_Report", model);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }

        }
        public async Task<IActionResult> Well_Development_DetailsPdf(int id)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
            }
            else
            {
                ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Email");
            }
           

                var model = await _context.Well_Developments.Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Id.Equals(id)).FirstAsync();
                return new ViewAsPdf("_Well_Development_Report", model);
        }
        public async Task<IActionResult> Daily_Report_DetailsPdf(int id)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
            }
            else
            {
                ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Email");
            }
            

            var model = await _context.Daily_Reports
                .Include(x => x.Project).ThenInclude(x => x.Division)
                .Include(x=>x.MeasPoint)
                .Where(x => x.Id.Equals(id)).FirstAsync();
            List<string> photos = new List<string>();
            string path = _env.WebRootPath + "/images/Divisions/Daily_Reports/" + model.Project.DivisionId.ToString() + "/" + model.Id.ToString() + "/";
            var folder = Directory.EnumerateFiles(path)
                             .Select(fn => Path.GetFileName(fn));
            List<string> pictures = new List<string>();
            foreach (string file in folder)
            {
                pictures.Add(file);
            }
            model.pictures = pictures;
            return new ViewAsPdf("_Daily_Report", model);
        }
        [HttpPost]
        public async Task<FileResult> DownloadPhotos(int Id)
        {
            var archive = _env.WebRootPath + "\\images\\archive.zip";
            var temp =  _env.WebRootPath + "\\images\\tmp\\Files";

            // clear any existing archive
            if (System.IO.File.Exists(archive))
            {
                System.IO.File.Delete(archive);
            }
            // empty the temp folder
            if (!Directory.Exists(temp))
            {
                Directory.CreateDirectory(temp);
            }
            Directory.EnumerateFiles(temp).ToList().ForEach(f => System.IO.File.Delete(f));

            var dailyreport = await _context.Daily_Reports.Include(x => x.Project).Where(x => x.Id.Equals(Id)).FirstAsync();
            string path = _env.WebRootPath + "/images/Divisions/Daily_Reports/" + dailyreport.Project.DivisionId.ToString() + "/" + dailyreport.Id.ToString() + "/";
            var folder = Directory.EnumerateFiles(path)
                             .Select(fn => Path.GetFileName(fn));
            List<string> files = new List<string>();
            foreach (string file in folder)
            {
                files.Add(file);
            }
            // copy the selected files to the temp folder
            foreach(string f in files)
            {
                string fromname = _env.WebRootPath + "\\images\\Divisions\\Daily_Reports\\" + dailyreport.Project.DivisionId.ToString() + "\\" + dailyreport.Id.ToString() + "\\" + f;
                string toname = Path.Combine(temp, f);
                System.IO.File.Copy(fromname, toname);
            }
            

            // create a new archive
            ZipFile.CreateFromDirectory(temp, archive);
            string name = "DailyReport_" + dailyreport.Id.ToString() + "_" + dailyreport.Report_Date.ToString("yyyy-MM-dd")+".zip";
            return PhysicalFile(archive, "application/zip", name);
        }
        public async Task<IActionResult> Generator_Test_Report(int Id)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
            }
            else
            {
                ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Email");
            }

            var report = await _context.Generator_Test.Include(x=>x.Project).Where(x=>x.Id.Equals(Id)).FirstAsync();
            return View(report);

        }
        public async Task<IActionResult> WTP_Test_Report(int? Id)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
            }
            else
            {
                ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Email");
            }
            if (Id != null)
            {
                var report = await _context.WTP_Tests.Include(x => x.Project).ThenInclude(x=>x.Division).Where(x => x.Id.Equals(Id)).FirstAsync();
                return View(report);
            }
            else { return View(nameof(Index)); }
        }
        public async Task<IActionResult> Well_Installation_Report(int? Id)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
            }
            else
            {
                ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Email");
            }
            if (Id != null)
            {
                var report = await _context.Well_Installations.Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Id.Equals(Id)).FirstAsync();
                return View(report);
            }
            else { return View(nameof(Index)); }
        }
        public async Task<IActionResult> Well_Drilling_Report(int? Id)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
            }
            else
            {
                ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Email");
            }
            if (Id != null)
            {
                var report = await _context.Well_Drillings.Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Id.Equals(Id)).FirstAsync();
                return View(report);
            }
            else { return View(nameof(Index)); }
        }
        public async Task<IActionResult> Pump_Installation_Report(int? Id)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
            }
            else
            {
                ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Email");
            }
            if (Id != null)
            {
                var report = await _context.Pump_Installations.Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Id.Equals(Id)).FirstAsync();
                return View(report);
            }
            else { return View(nameof(Index)); }
        }
        public async Task<IActionResult> Pump_Commission_Report(int? Id)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
            }
            else
            {
                ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Email");
            }
            if (Id != null)
            {
                var report = await _context.Pump_Commissions.Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Id.Equals(Id)).FirstAsync();
                return View(report);
            }
            else { return View(nameof(Index)); }
        }
        public async Task<IActionResult> Well_Development_Report(int? Id)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
            }
            else
            {
                ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Email");
            }
            if (Id != null)
            {
                var report = await _context.Well_Developments.Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Id.Equals(Id)).FirstAsync();
                return View(report);
            }
            else { return View(nameof(Index)); }
        }
        public async Task<IActionResult> Daily_Report(int? Id)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
            }
            else
            {
                ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)), "Id", "Email");
            }
            if (Id != null)
            {
                var report = await _context.Daily_Reports.Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Id.Equals(Id)).FirstAsync();
                return View(report);
            }
            else { return View(nameof(Index)); }
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Member,DivisionAdmin")]
        public async Task<IActionResult> GetDocuments(int? id,string startdate,string enddate,string who)
        {
            DateTime thestartdate = DateTime.Now.AddDays(-5000);
            DateTime theenddate = DateTime.Now;
            string thewho = "All";
            if(startdate != "")
            {
                thestartdate = Convert.ToDateTime(startdate);
            }
            if(enddate != "")
            {
                theenddate = Convert.ToDateTime(enddate);
            }
            if (who != null && who != "")
            {
                thewho = who;
            }
            else
            {
                thewho = "All";
            }
            if (id != null)
            {
                var kstype = await _context.KSTypes.FindAsync(id);
                if(kstype.name == "Generator Test")
                {
                    if(thewho == "All")
                    {
                        var generatortests = await _context.Generator_Test.Include(x=>x.Project)
                            .Where(x=>x.Date_Done >= thestartdate && x.Date_Done < theenddate.AddDays(1))
                            .ToListAsync();
                        return PartialView("_GeneratorTests", generatortests);
                    }
                    else
                    {
                        var generatortests = await _context.Generator_Test.Include(x => x.Project)
                        .Where(x => x.Date_Done >= thestartdate && x.Date_Done < theenddate.AddDays(1) && x.Name.ToLower().Contains(thewho.ToLower()))
                        .ToListAsync();
                        return PartialView("_GeneratorTests", generatortests);
                    }
                }
                else if(kstype.name == "Inspection WTP")
                {
                    if(thewho == "All")
                    {
                        var wtptests = await _context.WTP_Tests.Include(x => x.Project).ThenInclude(x => x.Division)
                        .Where(x => x.Date_Done >= thestartdate && x.Date_Done < theenddate.AddDays(1))
                        .ToListAsync();
                        return PartialView("_WTPTests", wtptests);
                    }
                    else
                    {
                        var wtptests = await _context.WTP_Tests.Include(x => x.Project).ThenInclude(x => x.Division)
                        .Where(x => x.Date_Done >= thestartdate && x.Date_Done < theenddate.AddDays(1) && x.Name.ToLower().Contains(thewho.ToLower()))
                        .ToListAsync();
                        return PartialView("_WTPTests", wtptests);
                    }
                    
                }
                else if (kstype.name == "Well Installation")
                {
                    if(thewho == "All"){
                        var wellinstallations = await _context.Well_Installations.Include(x => x.Project).ThenInclude(x => x.Division)
                        .Where(x => x.Date_Done >= thestartdate && x.Date_Done < theenddate.AddDays(1))
                        .ToListAsync();
                        return PartialView("_WellInstallations", wellinstallations);
                    }
                    else{
                        var wellinstallations = await _context.Well_Installations.Include(x => x.Project).ThenInclude(x => x.Division)
                        .Where(x => x.Date_Done >= thestartdate && x.Date_Done < theenddate.AddDays(1) && x.Name.ToLower().Contains(thewho.ToLower()))
                        .ToListAsync();
                        return PartialView("_WellInstallations", wellinstallations);
                    }
                    
                }
                else if (kstype.name == "Well Drilling")
                {
                    if (thewho == "All")
                    {
                        var welldrillings = await _context.Well_Drillings.Include(x => x.Project).ThenInclude(x => x.Division)
                        .Where(x => x.Date_Done >= thestartdate && x.Date_Done < theenddate.AddDays(1))
                        .ToListAsync();
                        return PartialView("_WellDrillings", welldrillings);
                    }
                    else
                    {
                        var welldrillings = await _context.Well_Drillings.Include(x => x.Project).ThenInclude(x => x.Division)
                        .Where(x => x.Date_Done >= thestartdate && x.Date_Done < theenddate.AddDays(1) && x.Name.ToLower().Contains(thewho.ToLower()))
                        .ToListAsync();
                        return PartialView("_WellDrillings", welldrillings);
                    }

                }
                else if (kstype.name == "Well Development")
                {
                    if(thewho == "All")
                    {
                        var welldevelopments = await _context.Well_Developments.Include(x => x.Project)
                        .Where(x => x.Date_Done >= thestartdate && x.Date_Done < theenddate.AddDays(1))
                        .ToListAsync();
                        return PartialView("_WellDevelopments", welldevelopments);
                    }
                    else
                    {
                        var welldevelopments = await _context.Well_Developments.Include(x => x.Project)
                        .Where(x => x.Date_Done >= thestartdate && x.Date_Done < theenddate.AddDays(1) && x.Name.ToLower().Contains(thewho.ToLower()))
                        .ToListAsync();
                        return PartialView("_WellDevelopments", welldevelopments);
                    }
                    
                }
                else if (kstype.name == "Pump Installation")
                {
                    if(thewho == "All")
                    {
                        var pumpinstallations = await _context.Pump_Installations.Include(x => x.Project)
                        .Where(x => x.Date_Done >= thestartdate && x.Date_Done < theenddate.AddDays(1))
                        .ToListAsync();
                        return PartialView("_PumpInstallations", pumpinstallations);
                    }
                    else
                    {
                        var pumpinstallations = await _context.Pump_Installations.Include(x => x.Project)
                        .Where(x => x.Date_Done >= thestartdate && x.Date_Done < theenddate.AddDays(1) && x.Name.ToLower().Contains(thewho.ToLower()))
                        .ToListAsync();
                        return PartialView("_PumpInstallations", pumpinstallations);
                    }

                    
                }
                else if (kstype.name == "Pump Commission")
                {
                    if(thewho == "All")
                    {
                        var pumpcommissions = await _context.Pump_Commissions.Include(x => x.Project)
                        .Where(x => x.Date_Done >= thestartdate && x.Date_Done < theenddate.AddDays(1))
                        .ToListAsync();
                        return PartialView("_PumpCommissions", pumpcommissions);
                    }
                    else
                    {
                        var pumpcommissions = await _context.Pump_Commissions.Include(x => x.Project)
                        .Where(x => x.Date_Done >= thestartdate && x.Date_Done < theenddate.AddDays(1) && x.Name.ToLower().Contains(thewho.ToLower()))
                        .ToListAsync();
                        return PartialView("_PumpCommissions", pumpcommissions);
                    }
                    
                    
                }
                else if (kstype.name == "Well Drilling")
                {
                    if(thewho == "All")
                    {
                        var generatortests = await _context.Generator_Test.Include(x => x.Project)
                        .Where(x => x.Date_Done >= thestartdate && x.Date_Done < theenddate.AddDays(1))
                        .ToListAsync();
                        return PartialView("_GeneratorTests", generatortests);
                    }
                    else
                    {
                        var generatortests = await _context.Generator_Test.Include(x => x.Project)
                        .Where(x => x.Date_Done >= thestartdate && x.Date_Done < theenddate.AddDays(1) && x.Name.ToLower().Contains(thewho.ToLower()))
                        .ToListAsync();
                        return PartialView("_GeneratorTests", generatortests);
                    }
                    
                }
                else if (kstype.name == "Daily Report")
                {
                    if (thewho == "All")
                    {
                        var dailyreports = await _context.Daily_Reports.Include(x => x.Project).Include(x=>x.MeasPoint)
                        .Where(x => x.Report_Date >= thestartdate && x.Report_Date < theenddate.AddDays(1))
                        .ToListAsync();
                        return PartialView("_DailyReports", dailyreports);
                    }
                    else
                    {
                        var dailyreports = await _context.Daily_Reports.Include(x => x.Project).Include(x => x.MeasPoint)
                        .Where(x => x.Report_Date >= thestartdate && x.Report_Date < theenddate.AddDays(1) && x.DoneBy.ToLower().Contains(thewho.ToLower()))
                        .ToListAsync();
                        return PartialView("_DailyReports", dailyreports);
                    }

                }
                else 
                {
                    var generatortests = await _context.Generator_Test.ToListAsync();
                    return PartialView("_GeneratorTests", generatortests);
                }

                
            }
            else
            {
                List<Generator_Test> nodocu = new List<Generator_Test>();
                return PartialView("_GeneratorTests", nodocu);
            }
        }
        
    }
}