using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MainOps.Data;
using MainOps.Models.ReportClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using MainOps.Models;

namespace MainOps.Controllers.ReportControllers
{
    [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
    public class MachineryController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MachineryController(DataContext context, UserManager<ApplicationUser> userManager) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Machineries
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            await CheckUser(user);
            if (User.IsInRole("Admin"))
            {
                return View(await _context.Machinery.Include(x => x.Division).ToListAsync());
            }
            return View(await _context.Machinery.Include(x => x.Division).Where(x => x.DivisionId.Equals(user.DivisionId)).ToListAsync());
        }
        // GET: Machineries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machinery = await _context.Machinery.Include(x => x.Division)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (machinery == null)
            {
                return NotFound();
            }

            return View(machinery);
        }

        // GET: Machineries/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            Machinery model = new Machinery();
            model.DivisionId = user.DivisionId;
            if (User.IsInRole("Admin"))
            {
                ViewData["DivisionId"] = new SelectList(_context.Divisions, "Id", "Name");
            }
            else
            {
                ViewData["DivisionId"] = new SelectList(_context.Divisions.Where(x => x.Id.Equals(user.DivisionId)), "Id", "Name");
            }
            return View(model);
        }

        // POST: Machineries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MachineryName")] Machinery machinery)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                machinery.DivisionId = user.DivisionId;
                _context.Add(machinery);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(machinery);
        }

        // GET: Machineries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
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
            if (User.IsInRole("Admin")) { 
                var machinery = await _context.Machinery.SingleOrDefaultAsync(x => x.Id.Equals(id));
                if (machinery == null)
                {
                    return NotFound();
                }
                return View(machinery);
            }
            else
            {
                var machinery = await _context.Machinery.SingleOrDefaultAsync(x => x.Id.Equals(id) && x.DivisionId.Equals(user.DivisionId));
                if (machinery == null)
                {
                    return NotFound();
                }
                return View(machinery);
            }
        }

        // POST: Machineries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MachineryName")] Machinery machinery)
        {
            if (id != machinery.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(machinery);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MachineryExists(machinery.Id))
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
            return View(machinery);
        }

        // GET: Machineries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                var machinery = await _context.Machinery
               .FirstOrDefaultAsync(m => m.Id == id);
                if (machinery == null)
                {
                    return NotFound();
                }

                return View(machinery);
            }
            else
            {
                var machinery = await _context.Machinery
               .FirstOrDefaultAsync(m => m.Id == id && m.DivisionId.Equals(user.DivisionId));
                if (machinery == null)
                {
                    return NotFound();
                }

                return View(machinery);
            }
           
        }

        // POST: Machineries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var machinery = await _context.Machinery.FindAsync(id);
            _context.Machinery.Remove(machinery);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MachineryExists(int id)
        {
            return _context.Machinery.Any(e => e.Id == id);
        }
    }
}
