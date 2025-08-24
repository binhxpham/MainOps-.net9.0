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
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Identity;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,DivisionAdmin")]
    public class ProjectUsersController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        

        public ProjectUsersController(DataContext context, UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: ProjectUsers
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> Index()
        {
            var theuser = await _userManager.GetUserAsync(User);
            List<ProjectUserVM> model = new List<ProjectUserVM>();
            if (User.IsInRole("Admin"))
            {
                var dataContextAdmin = await _context.ProjectUsers.Include(p => p.Project).OrderBy(x=>x.Project.Name).ToListAsync();
                foreach(var pu in dataContextAdmin)
                {
                    var project = await _context.Projects.Where(x => x.Id.Equals(pu.projectId)).SingleOrDefaultAsync();
                    var user = await _userManager.FindByIdAsync(pu.userId);
                    if(user == null)
                    {
                        _context.ProjectUsers.Remove(pu);
                        await _context.SaveChangesAsync();
                    }
                    else { 
                        ProjectUserVM PUVM = new ProjectUserVM(pu, user, project);
                        model.Add(PUVM);
                    }
                }
                return View(model);
            }
            var dataContext = await _context.ProjectUsers.Include(p => p.Project).Where(x=>x.Project.DivisionId.Equals(theuser.DivisionId)).OrderBy(x => x.Project.Name).ToListAsync();
            foreach (var pu in dataContext)
            {
                var project = await _context.Projects.Where(x => x.Id.Equals(pu.projectId)).SingleOrDefaultAsync();
                var user = await _userManager.FindByIdAsync(pu.userId);
                ProjectUserVM PUVM = new ProjectUserVM(pu, user, project);
                model.Add(PUVM);
            }

            return View(model);
        }

        // GET: ProjectUsers/Details/5
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectUser = await _context.ProjectUsers
                .Include(p => p.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectUser == null)
            {
                return NotFound();
            }
            
            return View(projectUser);
        }

        // GET: ProjectUsers/Create
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> Create()
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["userId"] = new SelectList(_context.Users.OrderBy(x=>x.Email), "Id", "Email");
                ViewData["projectId"] = new SelectList(_context.Projects.Include(x=>x.Division).OrderBy(x=>x.Division.Name).ThenBy(x=>x.Name), "Id", "Name");
            }
            else
            {
                ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)).OrderBy(x => x.Email), "Id", "Email");
                ViewData["projectId"] = new SelectList(_context.Projects.Include(x => x.Division).Where(x => x.DivisionId.Equals(theuser.DivisionId)).OrderBy(x => x.Division.Name).ThenBy(x => x.Name), "Id", "Name");
            }

            return View();
        }

        // POST: ProjectUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> Create([Bind("Id,projectId,userId")] ProjectUser projectUser)
        {
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                _context.Add(projectUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            if (!User.IsInRole("Admin"))
            {
                ViewData["projectId"] = new SelectList(_context.Projects.Where(x => x.DivisionId.Equals(user.DivisionId)), "Id", "Abbreviation", projectUser.projectId);
                return View(projectUser);
            }
            else
            {
                ViewData["projectId"] = new SelectList(_context.Projects, "Id", "Abbreviation", projectUser.projectId);
                return View(projectUser);
            }
            
        }

        // GET: ProjectUsers/Edit/5
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectUser = await _context.ProjectUsers.FindAsync(id);
            if (projectUser == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin"))
            {
                ViewData["projectId"] = new SelectList(_context.Projects.Where(x => x.DivisionId.Equals(user.DivisionId)), "Id", "Name", projectUser.projectId);
                return View(projectUser);
            }
            else
            {
                ViewData["projectId"] = new SelectList(_context.Projects, "Id", "Name", projectUser.projectId);
                return View(projectUser);
            }
        }

        // POST: ProjectUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,projectId,userId")] ProjectUser projectUser)
        {
            if (id != projectUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projectUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectUserExists(projectUser.Id))
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
            var user = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin"))
            {
                ViewData["projectId"] = new SelectList(_context.Projects.Where(x => x.DivisionId.Equals(user.DivisionId)), "Id", "Abbreviation", projectUser.projectId);
                return View(projectUser);
            }
            else
            {
                ViewData["projectId"] = new SelectList(_context.Projects, "Id", "Abbreviation", projectUser.projectId);
                return View(projectUser);
            }
        }

        // GET: ProjectUsers/Delete/5
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectUser = await _context.ProjectUsers
                .Include(p => p.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectUser == null)
            {
                return NotFound();
            }
         
            return View(projectUser);
        }

        // POST: ProjectUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projectUser = await _context.ProjectUsers.FindAsync(id);
            _context.ProjectUsers.Remove(projectUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectUserExists(int id)
        {
            return _context.ProjectUsers.Any(e => e.Id == id);
        }
    }
}
