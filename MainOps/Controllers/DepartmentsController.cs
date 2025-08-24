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

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
    public class DepartmentsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DepartmentsController(DataContext context,UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            await CheckUser(user);
            if (!User.IsInRole("Admin")) {
                var dataContext = _context.Departments.Include(d => d.Division).Where(x => x.DivisionId.Equals(user.DivisionId));
                return View(await dataContext.ToListAsync());
            }
            else
            {
                var dataContext = _context.Departments.Include(d => d.Division);
                return View(await dataContext.ToListAsync());
            }
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            await CheckUser(user);
            if (User.IsInRole("Admin"))
            {
                var department = await _context.Departments
                .Include(d => d.Division)
                .FirstOrDefaultAsync(m => m.Id == id);

                if (department == null)
                {
                    return NotFound();
                }

                return View(department);
            }
            else
            {
                var department = await _context.Departments
                .Include(d => d.Division)
                .SingleOrDefaultAsync(x =>x.Id.Equals(id) && x.DivisionId.Equals(user.DivisionId));

                if (department == null)
                {
                    return NotFound();
                }

                return View(department);
            }
            
        }

        // GET: Departments/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            await CheckUser(user);
            if (User.IsInRole("Admin"))
            {
                ViewData["DivisionId"] = new SelectList(_context.Divisions, "Id", "Name");
            }
            else
            {
                ViewData["DivisionId"] = new SelectList(_context.Divisions.Where(x => x.Id.Equals(user.DivisionId)), "Id", "Name");
            }
            
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DepartmentName,DivisionId")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DivisionId"] = new SelectList(_context.Divisions, "Id", "Name", department.DivisionId);
            return View(department);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            await CheckUser(user);
            if (User.IsInRole("Admin")) { 
                var department = await _context.Departments.FindAsync(id);
                if (department == null)
                {
                    return NotFound();
                }
                if (User.IsInRole("Admin"))
                {
                    ViewData["DivisionId"] = new SelectList(_context.Divisions, "Id", "Name");
                }
                else
                {
                    ViewData["DivisionId"] = new SelectList(_context.Divisions.Where(x => x.Id.Equals(user.DivisionId)), "Id", "Name");
                }
                return View(department);
            }
            else {
                var department = await _context.Departments.SingleOrDefaultAsync(x => x.Id.Equals(id) && x.Id.Equals(user.DivisionId));
                if (department == null)
                {
                    return NotFound();
                }
                if (User.IsInRole("Admin"))
                {
                    ViewData["DivisionId"] = new SelectList(_context.Divisions, "Id", "Name");
                }
                else
                {
                    ViewData["DivisionId"] = new SelectList(_context.Divisions.Where(x => x.Id.Equals(user.DivisionId)), "Id", "Name");
                }
                return View(department);
            }
            
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DepartmentName,DivisionId")] Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.Id))
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
            ViewData["DivisionId"] = new SelectList(_context.Divisions, "Id", "Name", department.DivisionId);
            return View(department);
        }

        // GET: Departments/Delete/5
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            await CheckUser(user);
            if (User.IsInRole("Admin"))
            {
                var department = await _context.Departments
                .Include(d => d.Division)
                .FirstOrDefaultAsync(m => m.Id == id);
                if (department == null)
                {
                    return NotFound();
                }

                return View(department);
            }
            else
            {
                var department = await _context.Departments
                .Include(d => d.Division)
                .FirstOrDefaultAsync(m => m.Id == id && m.Id.Equals(user.DivisionId));
                if (department == null)
                {
                    return NotFound();
                }

                return View(department);
            }
            
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}
