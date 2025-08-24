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
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using MainOps.ExtensionMethods;
using MainOps.Models.ReportClasses;
using Microsoft.AspNetCore.Http;

namespace MainOps.Controllers
{
    [Authorize]
    public class DocumentsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public DocumentsController(DataContext context,UserManager<ApplicationUser> userManager, IWebHostEnvironment environment):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }
        [HttpGet]
        public async Task<ActionResult> DownloadDocument(int? id)
        {
            var document = await _context.Documents.FindAsync(id);
            string filePath = document.path;
            string fileName = document.Name;

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/force-download", fileName);

        }
        // GET: Documents
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            List<SelectListItem> selList = await createMeasPointList(); 
            ViewData["MeasPointId"] = selList;
            ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.DivisionId.Equals(user.DivisionId) && x.Active.Equals(true)).OrderBy(x => x.Name), "Id", "Name");
            if (User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
            {
                var data1 = await (from doc in _context.Documents.Include(x => x.Project).Include(x => x.DocumentType).Include(d => d.MeasPoint).ThenInclude(x => x.Project)
                                  join pu in _context.ProjectUsers on doc.ProjectId
                                  equals pu.projectId
                                  where pu.userId == user.Id && doc.Project.Active.Equals(true)
                                  select doc).ToListAsync();
                var data2 = await (from doc in _context.Documents.Include(x => x.Project).Include(x => x.DocumentType).Include(d => d.MeasPoint).ThenInclude(x => x.Project)
                                   join pu in _context.ProjectUsers on doc.MeasPoint.ProjectId
                                   equals pu.projectId
                                   where pu.userId == user.Id && doc.MeasPoint.Project.Active.Equals(true)
                                   select doc).ToListAsync();
                var data3 = data1.Concat(data2).ToList();
                return View(data3);
            }
            if (User.IsInRole("Admin"))
            {
                var dataContext2 = _context.Documents.Include(x => x.Project).Include(x => x.DocumentType).Include(d => d.MeasPoint).ThenInclude(x => x.Project);
                return View(await dataContext2.ToListAsync());
            }
            else
            {
                var dataContext2 = _context.Documents.Include(x => x.Project).Include(x => x.DocumentType).Include(d => d.MeasPoint).ThenInclude(x => x.Project)
                .Where(x => x.MeasPoint.Project.DivisionId.Equals(user.DivisionId) || x.Project.DivisionId.Equals(user.DivisionId));
                return View(await dataContext2.ToListAsync());
            }
            
        }
        public async Task<IEnumerable<SelectListItem>> createFilterlist()
        {
            var theuser = await _userManager.GetUserAsync(HttpContext.User);
            
            var filternames2 = await _context.Projects.Include(x => x.Division).Where(x => x.Division.Id.Equals(theuser.DivisionId)).OrderBy(b => b.Name).ToListAsync();

            IEnumerable<SelectListItem> selList = from s in filternames2
                                                  select new SelectListItem
                                                  {
                                                      Value = s.Id.ToString(),
                                                      Text = s.Name
                                                  };
            return selList;
        }
        
        public async Task<List<SelectListItem>> createMeasPointList()
        {
            var user = await _userManager.GetUserAsync(User);
            List<Project> groups = new List<Project>();
            if (User.IsInRole("Admin"))
            {
                groups = await _context.Projects.Include(x => x.Division).ToListAsync();
            }
            else if (User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
            {
                groups = await (from p in _context.Projects.Include(x => x.Division)
                                   join pu in _context.ProjectUsers on p.Id
                                   equals pu.projectId
                                   where pu.userId == user.Id && p.Active.Equals(true)
                                   select p).ToListAsync();
            }
            else
            {
                groups = await _context.Projects.Include(x => x.Division)
                .Where(x => x.Division.Id.Equals(user.DivisionId)).ToListAsync();
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
                monpoints = await _context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division)
                .OrderBy(b => b.Project.Name).ThenBy(x => x.Name).ToListAsync();
            }
            else if(User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
            {
                monpoints = await (from mp in _context.MeasPoints.Include(x => x.Project).ThenInclude(x=>x.Division)
                                join pu in _context.ProjectUsers on mp.ProjectId
                                equals pu.projectId
                                where pu.userId == user.Id && mp.Project.Active.Equals(true)
                                select mp).OrderBy(b => b.Project.Name).ThenBy(x => x.Name).ToListAsync();
            }
            else
            {
                monpoints = await _context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division)
                .Where(x => x.Project.Division.Id.Equals(user.DivisionId))
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
        [HttpGet]
        public async Task<JsonResult> AutoComplete(string search)
        {
            var theuser = await _userManager.GetUserAsync(HttpContext.User);
                
                var results = _context.Documents
                    .Include(x => x.MeasPoint).ThenInclude(x => x.Project).ThenInclude(x => x.Division)
                    .Where(x => (x.MeasPoint.Name.ToLower().Contains(search.ToLower()) || x.Name.ToLower().Contains(search.ToLower())) && x.MeasPoint.Project.Division.Id.Equals(theuser.DivisionId) || (x.Project.Name.ToLower().Contains(search.ToLower()) && x.MeasPoint.Project.Division.Id.Equals(theuser.DivisionId))).ToList();
                return Json(results.Select(m => new
                {
                    id = m.Id,
                    value = m.Name,
                    label = m.Project.Name + '_' + m.Name
                }).OrderBy(x => x.label));

        }
        public async Task<IActionResult> Combsearch(string searchstring, string filterchoice, string filterchoice2)
        {
            int f_c_converted;
            int f_c_converted2;
            f_c_converted = Convert.ToInt32(filterchoice);
            f_c_converted2 = Convert.ToInt32(filterchoice2);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            List<SelectListItem> selList = await createMeasPointList();
            ViewData["MeasPointId"] = selList;
            List<Document> data = new List<Document>();
            if (User.IsInRole("Admin")) {
                ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.DivisionId.Equals(user.DivisionId) && x.Active.Equals(true)).OrderBy(x => x.Name), "Id", "Name");
            }
            if (searchstring != null && filterchoice != null && filterchoice2 != null)
            {

                data = await _context.Documents
                    .Include(x => x.DocumentType)
                    .Include(d => d.MeasPoint).ThenInclude(x => x.Project)
                    .Where(x => x.Name.ToLower().Contains(searchstring.ToLower()) && x.MeasPointId.Equals(f_c_converted) && x.ProjectId.Equals(f_c_converted2))
                    .ToListAsync();
            }
            else if (searchstring != null && (filterchoice == null || filterchoice == "All") && (filterchoice2 == null || filterchoice2 == "All"))
            {
                data = await _context.Documents.Include(x => x.DocumentType)
                    .Include(d => d.MeasPoint).ThenInclude(x => x.Project)
                    .Where(x => x.Name.ToLower().Contains(searchstring.ToLower()))
                    .ToListAsync();
            }
            else if (searchstring != null && (filterchoice != null && (filterchoice2 == null) || filterchoice2 == "All"))
            {
                data = await _context.Documents.Include(x => x.DocumentType)
                    .Include(d => d.MeasPoint).ThenInclude(x => x.Project)
                    .Where(x => x.Name.ToLower().Contains(searchstring.ToLower()) || x.MeasPointId.Equals(f_c_converted))
                    .ToListAsync();
            }
            else if (searchstring != null && (filterchoice == null || filterchoice == "All") && filterchoice2 != null)
            {
                data = await _context.Documents.Include(x => x.DocumentType)
                    .Include(d => d.MeasPoint).ThenInclude(x => x.Project)
                    .Where(x => x.Name.ToLower().Contains(searchstring.ToLower()) && (x.ProjectId.Equals(f_c_converted2) || x.MeasPoint.ProjectId.Equals(f_c_converted2)))
                    .ToListAsync();
            }
            else if (searchstring == null && filterchoice != null && (filterchoice2 == null || filterchoice2 == "All"))
            {
                data = await _context.Documents
                .Include(x => x.DocumentType)
                .Include(d => d.MeasPoint).ThenInclude(x => x.Project)
                .Where(x => x.MeasPointId.Equals(f_c_converted) || x.MeasPoint.ProjectId.Equals(f_c_converted2))
                .ToListAsync();

            }
            else if (searchstring == null && (filterchoice == null || filterchoice == "All") && filterchoice2 != null)
            {
                data = await _context.Documents
                .Include(x => x.DocumentType)
                .Include(d => d.MeasPoint).ThenInclude(x => x.Project)
                .Where(x => x.ProjectId.Equals(f_c_converted2) || x.MeasPoint.ProjectId.Equals(f_c_converted2))
                .ToListAsync();
            }
            else if (searchstring == null && filterchoice != null && filterchoice2 != null)
            {
                data = await _context.Documents
                .Include(x => x.DocumentType)
                .Include(d => d.MeasPoint).ThenInclude(x => x.Project)
                .Where(x => (x.MeasPoint.ProjectId.Equals(f_c_converted2) || x.ProjectId.Equals(f_c_converted2)) && x.MeasPointId.Equals(f_c_converted))
                .ToListAsync();
            }
            else
            {

                data = await _context.Documents
                .Include(x => x.DocumentType)
                .Include(x => x.MeasPoint).ThenInclude(x => x.Project)
                .Include(x => x.Project)
                .ToListAsync();

            }
            if(data.Count < 1)
            {
                return RedirectToAction(nameof(Index));
            }
            if (User.IsInRole("Admin"))
            {
                return View("Index", data);
            }
            else if (User.IsInRole("MemberGuest") || User.IsInRole("Guest"))
            {
                var projects = await (from proj in _context.Projects join pu in _context.ProjectUsers
                                      on proj.Id equals pu.projectId
                                      where pu.userId.Equals(user.Id)
                                      select proj).ToListAsync();
                if(projects.Count > 0)
                {
                    return View("Index", data.Where(x => projects.Contains(x.Project) || projects.Contains(x.MeasPoint.Project)));
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                var projects = await _context.Projects.Where(x => x.DivisionId.Equals(user.DivisionId)).ToListAsync();
                if(projects.Count > 0)
                {
                    return View("Index", data.Where(x => projects.Contains(x.Project) || projects.Contains(x.MeasPoint.Project)));
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            
            /**
            if (User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
                {
                    var data1 = await (from doc in _context.Documents.Include(x => x.Project).Include(x => x.DocumentType).Include(d => d.MeasPoint).ThenInclude(x => x.Project)
                                       join pu in _context.ProjectUsers on doc.ProjectId
                                       equals pu.projectId
                                       where pu.userId == user.Id && doc.Project.Active.Equals(true)
                                       select doc).ToListAsync();
                    var data2 = await (from doc in _context.Documents.Include(x => x.Project).Include(x => x.DocumentType).Include(d => d.MeasPoint).ThenInclude(x => x.Project)
                                       join pu in _context.ProjectUsers on doc.MeasPoint.ProjectId
                                       equals pu.projectId
                                       where pu.userId == user.Id && doc.MeasPoint.Project.Active.Equals(true)
                                       select doc).ToListAsync();
                    var data3 = data1.Concat(data2).ToList();
                    return View(data3);
                }
                else
                {
                    var data = await _context.Documents.Include(x => x.DocumentType).Include(d => d.MeasPoint).ThenInclude(x => x.Project).Where(x => x.MeasPoint.Project.DivisionId.Equals(user.DivisionId)).ToListAsync();
                    return View("Index", data);
                }
            **/

        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var document = await _context.Documents.Include(x=>x.DocumentType).Where(x=>x.Id.Equals(id)).SingleOrDefaultAsync();
            if(document.ExternalUrl == null || document.ExternalUrl == "") {
                var stream2 = new FileStream(document.path, FileMode.Open);
                if(document.path.EndsWith(".jpg") || document.path.EndsWith(".jpeg"))
                {
                    return new FileStreamResult(stream2, "image/jpeg");
                }
                if (document.path.EndsWith(".png"))
                {
                    return new FileStreamResult(stream2, "image/png");
                }
                if (document.DocumentType.Type.ToLower().Equals("photo"))
                {
                    return new FileStreamResult(stream2, "image/jpeg");
                }
                else
                {
                    return new FileStreamResult(stream2, "application/pdf");
                }
            }
            else
            { 
                return Redirect(document.ExternalUrl);
            }


        }
        public async Task<IActionResult> BoQProjectDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var document = await _context.Documents.Include(x => x.DocumentType).Where(x => x.ProjectId.Equals(id) && x.DocumentTypeId.Equals(32)).OrderBy(x => x.Id).LastOrDefaultAsync();
            if (document.ExternalUrl == null || document.ExternalUrl == "")
            {
                var stream2 = new FileStream(document.path, FileMode.Open);
                if (document.path.EndsWith(".jpg") || document.path.EndsWith(".jpeg"))
                {
                    return new FileStreamResult(stream2, "image/jpeg");
                }
                if (document.path.EndsWith(".png"))
                {
                    return new FileStreamResult(stream2, "image/png");
                }
                if (document.DocumentType.Type.ToLower().Equals("photo"))
                {
                    return new FileStreamResult(stream2, "image/jpeg");
                }
                else
                {
                    return new FileStreamResult(stream2, "application/pdf");
                }
            }
            else
            {
                return Redirect(document.ExternalUrl);
            }


        }
        // GET: Documents/Create
        [Authorize(Roles = "Admin,DivisionAdmin,Member")]
        public async Task<IActionResult> Create(int? id)
        {
            var user = await _userManager.GetUserAsync(User);

            ViewData["ProjectId"] = await GetProjectList();
            
            
            ViewData["DocumentTypeId"] = new SelectList(_context.DocumentTypes, "Id", "Type");
            List<SelectListItem> selList = await createMeasPointList();
            ViewData["MeasPointId"] = selList;
            if (id == null)
            {
                
                return View();
            }
            else
            {
                Document doc = new Document();
                doc.MeasPointId = id;
                return View(doc);
            }
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateTwo(int? AlarmBoxId)
        {
            if(AlarmBoxId != null)
            {
                Document document = new Document();
                document.MeasPointId = AlarmBoxId;
                document.DocumentTypeId = 16;
                document.ProjectId = null;
                var folderpath = Path.Combine(_environment.WebRootPath.ReplaceFirst("/", ""), "Documents", "AlarmBoxInfo");
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
                            document.Name = fileName;
                            // Combines two strings into a path.
                            fileName = folderpath + $@"\{fileName}";
                            //var filePath1 = Path.Combine(floorPlanPath, _environment.WebRootPath.ReplaceFirst("/", ""));
                            document.path = fileName;
                            using (FileStream fs = System.IO.File.Create(fileName))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }

                        }
                    }


                }
                _context.Add(document);
                await _context.SaveChangesAsync();
                return RedirectToAction("AlarmBoxes","MeasPoints");
            }
            else
            {
                return RedirectToAction("AlarmBoxes", "MeasPoints");
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DivisionAdmin,Member")]
        public async Task<IActionResult> Create([Bind("Name,MeasPointId,ProjectId,SubProjectId,DocumentTypeId,ExternalUrl")] Document document)
        {
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                document.DivisionId = user.DivisionId;
                if(document.ExternalUrl != null && document.ExternalUrl != "")
                {
                    document.path = document.ExternalUrl;
                    _context.Add(document);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var doctype = await _context.DocumentTypes.FindAsync(document.DocumentTypeId);
                    var folderpath = Path.Combine(_environment.WebRootPath.ReplaceFirst("/", ""), "Documents", doctype.Type);
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
                                if(document.Name == null || document.Name == "") { 
                                    document.Name = fileName;
                                }
                                // Combines two strings into a path.
                                fileName = folderpath + $@"\{fileName}";
                                //var filePath1 = Path.Combine(floorPlanPath, _environment.WebRootPath.ReplaceFirst("/", ""));
                                document.path = fileName;
                                using (FileStream fs = System.IO.File.Create(fileName))
                                {
                                    file.CopyTo(fs);
                                    fs.Flush();
                                }

                            }
                        }


                    }
                    _context.Add(document);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.DivisionId.Equals(user.DivisionId)), "Id", "Name");
            ViewData["DocumentTypeId"] = new SelectList(_context.DocumentTypes, "Id", "Type");
            List<SelectListItem> selList = await createMeasPointList();
            ViewData["MeasPointId"] = selList;
            return View();
        }

        // GET: Documents/Edit/5
        [Authorize(Roles = "Admin,DivisionAdmin,Member")]
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.DivisionId.Equals(user.DivisionId)), "Id", "Name");
            ViewData["DocumentTypeId"] = new SelectList(_context.DocumentTypes, "Id", "Type");
            List<SelectListItem> selList = await createMeasPointList();
            ViewData["MeasPointId"] = selList;
            return View(document);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DivisionAdmin,Member")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ProjectId,SubProjectId,MeasPointId,ExternalUrl,path,DivisionId")] Document document)
        {
            var user = await _userManager.GetUserAsync(User);
            if (id != document.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(document);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentExists(document.Id))
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
            ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.DivisionId.Equals(user.DivisionId)), "Id", "Name");
            ViewData["DocumentTypeId"] = new SelectList(_context.DocumentTypes, "Id", "Type");
            List<SelectListItem> selList = await createMeasPointList();
            ViewData["MeasPointId"] = selList;
            return View(document);
        }

        // GET: Documents/Delete/5
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Documents
                .Include(d => d.MeasPoint)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            System.IO.File.Delete(document.path);
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocumentExists(int id)
        {
            return _context.Documents.Any(e => e.Id == id);
        }
    }
}
