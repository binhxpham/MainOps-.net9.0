using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MainOps.Data;
using MainOps.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using MainOps.Resources;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,DivisionAdmin,Manager,Board")]
    public class ProjectStatusController : BaseController
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LocService _sharedLocalizer;

        public ProjectStatusController(DataContext context, IWebHostEnvironment env,UserManager<ApplicationUser> userManager,LocService localizer):base(context,userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
            _sharedLocalizer = localizer;
        }

        // GET: ProjectStatus
        public async Task<IActionResult> Index(DateTime? Start = null,DateTime? End = null,int? ProjectCategoryId = null, int? StatusDescriptionId = null,string ClientName = null)
        {
            var user = await _userManager.GetUserAsync(User);
            var categories = await _context.ProjectCategories.ToListAsync();
            var statusdescriptions = await _context.StatusDescriptions.ToListAsync();
            List<SelectListItem> catlist = await (from cat in _context.ProjectCategories select new SelectListItem 
            { Value = cat.Id.ToString(), Text = _sharedLocalizer.GetLocalizedHtmlString(cat.Text).Value }
            ).ToListAsync();           
            ViewData["ProjectCategoryId"] = new SelectList(catlist, "Value", "Text", ProjectCategoryId);
            List<SelectListItem> statlist = await (from stat in _context.StatusDescriptions
                                                  select new SelectListItem
                                                  { Value = stat.Id.ToString(), Text = _sharedLocalizer.GetLocalizedHtmlString(stat.Description).Value }
            ).ToListAsync();
            ViewData["StatusDescriptionId"] = new SelectList(statlist, "Value", "Text",StatusDescriptionId);
            ViewData["Clients"] = await GetClientList();
            List<ProjectStatus> data;
            if(Start != null && End != null)
            {
                data = await _context.ProjectStatuses.Include(x=> x.Division).Include(p => p.ProjectStatusProjectCategories).Include(p => p.StatusDescription)
                .Where(x => x.DivisionId.Equals(user.DivisionId) && x.StartDate >= Start && x.EndDate <= End).OrderByDescending(x => x.QuoteNumber).ToListAsync();
                
            }
            else if(Start != null && End == null)
            {
                data = await _context.ProjectStatuses.Include(x => x.Division).Include(p => p.ProjectStatusProjectCategories).Include(p => p.StatusDescription)
                .Where(x => x.DivisionId.Equals(user.DivisionId) && x.StartDate >= Start).OrderByDescending(x => x.QuoteNumber).ToListAsync();
                return View();
            }
            else if(End != null && Start == null){
                data = await _context.ProjectStatuses.Include(x => x.Division).Include(p => p.ProjectStatusProjectCategories).Include(p => p.StatusDescription)
                .Where(x => x.DivisionId.Equals(user.DivisionId) && x.EndDate <= End).OrderByDescending(x => x.QuoteNumber).ToListAsync();
            }
            else
            {
                data = await _context.ProjectStatuses.Include(x => x.Division).Include(p => p.ProjectStatusProjectCategories).Include(p => p.StatusDescription)
                .Where(x => x.DivisionId.Equals(user.DivisionId)).OrderByDescending(x => x.QuoteNumber).ToListAsync();
            }
            if(ProjectCategoryId != null)
            {

                data = data.Where(x => x.ProjectStatusProjectCategories.Where(y => y.ProjectCategoryId.Equals(ProjectCategoryId)).Count() >= 1).OrderByDescending(x => x.QuoteNumber).ToList();
            }
            if (StatusDescriptionId != null)
            {
                data = data.Where(x => x.StatusDescriptionId.Equals(StatusDescriptionId)).OrderByDescending(x => x.QuoteNumber).ToList();
            }
            if (ClientName != null)
            {
                data = data.Where(x => x.Client.ToLower().Contains(ClientName.ToLower())).OrderByDescending(x => x.QuoteNumber).ToList();
            }
            
            return View(new ProjectStatusVM(data, categories));
            

        }
        public async Task<IEnumerable<SelectListItem>> GetClientList()
        {
            var user = await _userManager.GetUserAsync(User);
            var clients = await _context.ProjectStatuses.Where(x => x.DivisionId.Equals(user.DivisionId)).Select(x => x.Client).ToListAsync();
            IEnumerable<SelectListItem> selList = from c in clients
                                                  select new SelectListItem
                                                  {
                                                      Value = c,
                                                      Text = c
                                                  };
            return selList;
        }
        // GET: ProjectStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (id == null)
            {
                return NotFound();
            }

            var projectStatus = await _context.ProjectStatuses
                .Include(p => p.ProjectStatusProjectCategories)
                .Include(p => p.StatusDescription)
                .Where(x=>x.DivisionId.Equals(user.DivisionId))
                .SingleOrDefaultAsync(m => m.Id == id);
            if (projectStatus == null)
            {
                return NotFound();
            }

            return View(projectStatus);
        }

        // GET: ProjectStatus/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            var categories = await _context.ProjectCategories.ToListAsync();
            ProjectStatusCreateVM model = new ProjectStatusCreateVM(categories);

            ViewData["StatusDescriptionId"] = new SelectList(_context.StatusDescriptions, "Id", "Description");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProjectStatusCreateVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                model.PS.DivisionId = user.DivisionId;
                _context.Add(model.PS);
                await _context.SaveChangesAsync();
                var lastadded = await _context.ProjectStatuses.LastAsync();
                foreach(var item in model.Cats)
                {
                    if (item.TrueFalse.Equals(true))
                    {
                        ProjectStatusProjectCategory PSPC = new ProjectStatusProjectCategory(lastadded.Id,item.Cat.Id);
                        _context.Add(PSPC);
                        
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StatusDescriptionId"] = new SelectList(_context.StatusDescriptions, "Id", "Description", model.PS.StatusDescriptionId);
            return View(model);
        }

        // GET: ProjectStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var projectStatus = await _context.ProjectStatuses.Include(x => x.ProjectStatusProjectCategories).Where(x=>x.Id.Equals(id) && x.DivisionId.Equals(user.DivisionId)).SingleOrDefaultAsync();
            if (projectStatus == null)
            {
                return NotFound();
            }
            ProjectStatusEditVM model = new ProjectStatusEditVM(projectStatus);
            
            ViewData["ProjectCategories"] = _context.ProjectCategories.ToList();
            ViewData["ProjectCategoryId"] = new SelectList(_context.ProjectCategories, "Id", "Text"); //this needs fixign, projectStatus.ProjectCategoryId);
            ViewData["StatusDescriptionId"] = new SelectList(_context.StatusDescriptions, "Id", "Description", projectStatus.StatusDescriptionId);
            return View(model);
        }

        // POST: ProjectStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<bool> UpdateProjectCategories(int ProjectStatusId,int CategoryId,bool tobeset)
        {
            try
            {
                var currentcategories = await _context.ProjectStatusProjectCategories.Where(x => x.ProjectStatusId.Equals(ProjectStatusId)).ToListAsync();
                if (tobeset)
                {
                    if (currentcategories.FirstOrDefault(x => x.ProjectCategoryId.Equals(CategoryId)) == null)
                    {
                        ProjectStatusProjectCategory PSPC = new ProjectStatusProjectCategory(ProjectStatusId, CategoryId);
                        _context.Add(PSPC);
                    }
                }
                else
                {
                    var cur_PSPC = await _context.ProjectStatusProjectCategories.Where(x => x.ProjectStatusId.Equals(ProjectStatusId) && x.ProjectCategoryId.Equals(CategoryId)).SingleOrDefaultAsync();
                    if (cur_PSPC != null)
                    {
                        _context.Remove(cur_PSPC);
                    }
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProjectStatusEditVM model)
        {
            if (id != model.ProjectStatus.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //if(model.ProjectStatus.StatusDescriptionId == 8)
                    //{
                    //    model.ProjectStatus.WinChance = (decimal)100.00;
                    //}
                    //else if(model.ProjectStatus.StatusDescriptionId == 9 || model.ProjectStatus.StatusDescriptionId == 12)
                    //{
                    //    model.ProjectStatus.WinChance = (decimal)0.00;
                    //}
                    _context.Update(model.ProjectStatus);
                    
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectStatusExists(model.ProjectStatus.Id))
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
            ViewData["ProjectCategoryId"] = new SelectList(_context.ProjectCategories, "Id", "Text"); // this needs to be fixed, projectStatus.ProjectCategoryId);
            ViewData["StatusDescriptionId"] = new SelectList(_context.StatusDescriptions, "Id", "Description", model.ProjectStatus.StatusDescriptionId);
            return View(model);
        }

        // GET: ProjectStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var projectStatus = await _context.ProjectStatuses
                .Include(p => p.ProjectStatusProjectCategories)
                .Include(p => p.StatusDescription)
                .Where(x=>x.Id.Equals(id) && x.DivisionId.Equals(user.DivisionId))
                .SingleOrDefaultAsync();
            if (projectStatus == null)
            {
                return NotFound();
            }

            return View(projectStatus);
        }

        // POST: ProjectStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projectStatus = await _context.ProjectStatuses.FindAsync(id);
            _context.ProjectStatuses.Remove(projectStatus);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectStatusExists(int id)
        {
            return _context.ProjectStatuses.Any(e => e.Id == id);
        }
    }
}
