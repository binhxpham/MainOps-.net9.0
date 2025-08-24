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
    public class AlarmReportReceiversController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        

        public AlarmReportReceiversController(DataContext context, UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: ProjectUsers
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> Index()
        {
            var theuser = await _userManager.GetUserAsync(User);
            List<AlarmReportReceiverVM> model = new List<AlarmReportReceiverVM>();
            if (User.IsInRole("Admin"))
            {
                var dataContextAdmin = await _context.AlarmReportReceivers.Include(p => p.Project).OrderBy(x=>x.Project.Name).ToListAsync();
                foreach(var pu in dataContextAdmin)
                {
                    var project = await _context.Projects.Where(x => x.Id.Equals(pu.ProjectId)).SingleOrDefaultAsync();
                    var user = await _userManager.FindByIdAsync(pu.UserId);
                    if(user == null)
                    {
                        _context.AlarmReportReceivers.Remove(pu);
                        await _context.SaveChangesAsync();
                    }
                    else {
                        AlarmReportReceiverVM PUVM = new AlarmReportReceiverVM(pu, user, project);
                        model.Add(PUVM);
                    }
                }
                return View(model);
            }
            var dataContext = await _context.AlarmReportReceivers.Include(p => p.Project).Where(x=>x.Project.DivisionId.Equals(theuser.DivisionId)).OrderBy(x => x.Project.Name).ToListAsync();
            foreach (var pu in dataContext)
            {
                var project = await _context.Projects.Where(x => x.Id.Equals(pu.ProjectId)).SingleOrDefaultAsync();
                var user = await _userManager.FindByIdAsync(pu.UserId);
                AlarmReportReceiverVM PUVM = new AlarmReportReceiverVM(pu, user, project);
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

            var projectUser = await _context.AlarmReportReceivers
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
                ViewData["ProjectId"] = await GetProjectList();
            }
            else
            {
                ViewData["userId"] = new SelectList(_context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId)).OrderBy(x => x.Email), "Id", "Email");
                ViewData["ProjectId"] = await GetProjectList();
            }

            return View();
        }

        // POST: ProjectUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,UserId")] AlarmReportReceiver projectUser)
        {
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                _context.Add(projectUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = await GetProjectList();
            return View(projectUser);
        }

        // GET: ProjectUsers/Edit/5
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectUser = await _context.AlarmReportReceivers.FindAsync(id);
            if (projectUser == null)
            {
                return NotFound();
            }

            ViewData["ProjectId"] = await GetProjectList();
            return View(projectUser);
        }

        // POST: ProjectUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,UserId")] AlarmReportReceiver projectUser)
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

            ViewData["ProjectId"] = await GetProjectList();
                return View(projectUser);
            
        }

        // GET: ProjectUsers/Delete/5
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectUser = await _context.AlarmReportReceivers
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
            var projectUser = await _context.AlarmReportReceivers.FindAsync(id);
            _context.AlarmReportReceivers.Remove(projectUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectUserExists(int id)
        {
            return _context.AlarmReportReceivers.Any(e => e.Id == id);
        }
    }
}
