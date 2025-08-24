using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MainOps.Data;
using MainOps.Models;
using MainOps.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,DivisionAdmin,ProjectMember,Guest,International,ExternalDriller")]
    public class SubProjectsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SubProjectsController(DataContext context, UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: SubProjects
        [Authorize(Roles = "Admin,DivisionAdmin,ProjectMember,Guest,International")]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                var dataContext = _context.SubProjects.Include(s => s.Project);
                return View(await dataContext.ToListAsync());
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                var dataContext = _context.SubProjects.Include(s => s.Project).Where(x=>x.Project.DivisionId.Equals(user.DivisionId));
                return View(await dataContext.ToListAsync());
            }
        }
        [AllowAnonymous]
        public JsonResult GetMeasPointsSubProject(string theId)
        {
            int Id = Convert.ToInt32(theId);
            List<MeasPoint> thedata = new List<MeasPoint>();
            thedata = _context.MeasPoints.Where(x => x.SubProjectId.Equals(Id) && x.ToBeHidden.Equals(false)).OrderBy(x => x.Name).ToList();
            return Json(thedata);
        }
        [AllowAnonymous]
        [HttpGet]
        public JsonResult GetWellsSubProject(string theId)
        {
            int Id = Convert.ToInt32(theId);
            
            var subproject = _context.SubProjects.Include(x => x.Project).SingleOrDefault(x => x.Id.Equals(Id));
            List<MeasPoint> thedata = _context.MeasPoints.Include(x => x.MeasType).Where(x => 
            x.ProjectId.Equals(subproject.ProjectId)  
            && x.ToBeHidden.Equals(false) 
            && x.MeasType.Type.ToLower().Equals("water level")
            ).OrderBy(x => x.Name).ToList();
            thedata = thedata.Where(x => !x.SubProjectId.HasValue || x.SubProjectId.Equals(subproject.Id)).OrderBy(x => x.Name).ToList();
            return Json(thedata);
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Member,DivisionAdmin,Guest,ProjectMember")]
        public async Task<IActionResult> GetDocuments(int? id)
        {
            List<Document> documents = new List<Document>();
            if (id != null)
            {               
                if (User.IsInRole("Admin"))
                {
                    documents = await _context.Documents
                    .Include(x => x.MeasPoint).Include(x => x.DocumentType).Include(x => x.SubProject)
                    .Where(x => x.SubProjectId.Equals(id) || x.MeasPoint.SubProjectId.Equals(id)).ToListAsync();
                    return PartialView("_DocumentInfo", documents);
                }
                else
                {
                    var user = await _userManager.GetUserAsync(User);
                    var subproject = await _context.SubProjects.Include(x => x.Project).Where(x => x.Id.Equals(id)).SingleOrDefaultAsync();
                    if(subproject == null)
                    {
                        return PartialView("_DocumentInfo", documents);
                    }
                    else if (!subproject.Project.DivisionId.Equals(user.DivisionId))
                    {
                        return PartialView("_DocumentInfo", documents);
                    }
                    
                    documents = await _context.Documents
                    .Include(x => x.MeasPoint).Include(x => x.DocumentType).Include(x => x.SubProject)
                    .Where(x => (x.SubProjectId.Equals(id) || x.MeasPoint.SubProjectId.Equals(id))).ToListAsync();
                    return PartialView("_DocumentInfo", documents);
                }
            }
            else
            {
                return PartialView("_DocumentInfo", documents);
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,International")]
        public async Task<IActionResult> CreateSubProject(int? id) {
            if(id != null)
            {
                SubProject SB = new SubProject { ProjectId = id };
                ViewData["ProjectId"] = await GetProjectList();
                return View("Create",SB);
            }
            else
            {
                ViewData["ProjectId"] = await GetProjectList();
                return View("Create");
            }
        }
        // GET: SubProjects/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var subProject = await _context.SubProjects
                .Include(s => s.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subProject == null)
            {
                return NotFound();
            }
            else if (!subProject.Project.DivisionId.Equals(user.DivisionId))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
            }

            return View(subProject);
        }
        [AllowAnonymous]
        public async Task<IActionResult> Details_Partial(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var subProject = await _context.SubProjects
                .Include(s => s.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subProject == null)
            {
                return NotFound();
            }
            else if (!subProject.Project.DivisionId.Equals(user.DivisionId))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
            }

            return PartialView("_Details",subProject);
        }


        // GET: SubProjects/Create
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,International")]
        public async Task<IActionResult> Create()
        {
            ViewData["ProjectId"] = await GetProjectList();
            return View();
        }

        // POST: SubProjects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,Name,SubProjectNr,Latitude,Longitude,Active,ClientContact,ClientEmail,ProtokolId,Type,Address")] SubProject subProject)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subProject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ProjectId"] = await GetProjectList();
            return View(subProject);
        }

        // GET: SubProjects/Edit/5
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,International")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var subProject = await _context.SubProjects.Include(x=>x.Project).SingleOrDefaultAsync(x=> x.Id.Equals(id));
            if (subProject == null)
            {
                return NotFound();
            }
            else if (!subProject.Project.DivisionId.Equals(user.DivisionId))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
            }
            ViewData["ProjectId"] = await GetProjectList();
            return View(subProject);
        }

        // POST: SubProjects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,International")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,Name,SubProjectNr,Latitude,Longitude,Active,ClientContact,ClientEmail,ProtokolId,Type,Address")] SubProject subProject)
        {
            if (id != subProject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subProject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubProjectExists(subProject.Id))
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
            return View(subProject);
        }

        // GET: SubProjects/Delete/5
        [Authorize(Roles = "Admin,DivisionAdmin,International")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var subProject = await _context.SubProjects
                .Include(s => s.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subProject == null)
            {
                return NotFound();
            }
            else if (!subProject.Project.DivisionId.Equals(user.DivisionId))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
            }

            return View(subProject);
        }

        // POST: SubProjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subProject = await _context.SubProjects.FindAsync(id);
            _context.SubProjects.Remove(subProject);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubProjectExists(int id)
        {
            return _context.SubProjects.Any(e => e.Id == id);
        }
        
        public JsonResult GetSubProjectList(string theId)
        {
            int Id = Convert.ToInt32(theId);
            List<SubProject> thedata = new List<SubProject>();
            thedata = _context.SubProjects.Where(x => x.ProjectId.Equals(Id)).OrderBy(x=>x.Name).ToList();
            return Json(thedata);     
        }
        [HttpGet]
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public IActionResult UploadRGHProjects()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> UploadRGHProjects(IFormFile postedFile)
        {
            if (postedFile != null)
            {
                string fileExtension = Path.GetExtension(postedFile.FileName);

                if (fileExtension != ".csv")
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "please provide a CSV file!" });
                }
                using (var sreader = new StreamReader(postedFile.OpenReadStream()))
                {
                    var user = await _userManager.GetUserAsync(User);

                    List<string> headers = sreader.ReadLine().Split(';').ToList();

                    int DivisionId = user.DivisionId;

                    var departments = await _context.Departments.ToListAsync();

                    while (!sreader.EndOfStream)
                    {
                        string[] rows = sreader.ReadLine().Split(';');
                        var prev_proj = await _context.SubProjects.SingleOrDefaultAsync(x => x.SubProjectNr.Equals(rows[0].Replace("\"", "").Replace("'", "")));
                        if (prev_proj == null)
                        {


                        }
                        else // This needs to be an update procedure instead!
                        {

                            prev_proj.ClientContact = rows[6].Replace("\"", "").Replace("'", "");

                            _context.Update(prev_proj);
                        }
                    }
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
