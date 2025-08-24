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
    [Authorize(Roles = "Admin,DivisionAdmin")]
    public class MonitorTypesController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MonitorTypesController(DataContext context, UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: MonitorTypes
        public async Task<IActionResult> Index()
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                return View(await _context.MonitorType.Include(x => x.Division).OrderBy(x=>x.Division.Name).ThenBy(x=>x.MonitorTypeName).ToListAsync());
            }
            return View(await _context.MonitorType.Include(x => x.Division).Where(x=>x.DivisionId.Equals(theuser.DivisionId)).OrderBy(x => x.Division.Name).ThenBy(x => x.MonitorTypeName).ToListAsync());
        }

        // GET: MonitorTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monitorType = await _context.MonitorType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (monitorType == null)
            {
                return NotFound();
            }

            return View(monitorType);
        }

        // GET: MonitorTypes/Create
        public IActionResult Create()
        {
            ViewData["DivisionId"] = new SelectList(_context.Divisions.OrderBy(x => x.Name), "Id", "Name");
            return View();
        }

        // POST: MonitorTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MonitorTypeName,DivisionId,frequency,alertfrequency")] MonitorType monitorType)
        {
            if (ModelState.IsValid)
            {
                var theuser = await _userManager.GetUserAsync(User);
                if (!User.IsInRole("Admin"))
                {
                    monitorType.DivisionId = theuser.DivisionId;
                }
                
                _context.Add(monitorType);
                await _context.SaveChangesAsync();
                ViewData["DivisionId"] = new SelectList(_context.Divisions.OrderBy(x => x.Name), "Id", "Name");
                return RedirectToAction(nameof(Index));
            }
            ViewData["DivisionId"] = new SelectList(_context.Divisions.OrderBy(x => x.Name), "Id", "Name");
            return View(monitorType);
        }

        // GET: MonitorTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monitorType = await _context.MonitorType.FindAsync(id);
            if (monitorType == null)
            {
                return NotFound();
            }
            ViewData["DivisionId"] = new SelectList(_context.Divisions.OrderBy(x => x.Name), "Id", "Name");
            return View(monitorType);
        }

        // POST: MonitorTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MonitorTypeName,DivisionId,frequency,alertfrequency")] MonitorType monitorType)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (id != monitorType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!User.IsInRole("Admin"))
                    {
                        monitorType.DivisionId = theuser.DivisionId;
                    }
                    _context.Update(monitorType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MonitorTypeExists(monitorType.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                ViewData["DivisionId"] = new SelectList(_context.Divisions.OrderBy(x => x.Name), "Id", "Name");
                return RedirectToAction(nameof(Index));
            }
            ViewData["DivisionId"] = new SelectList(_context.Divisions.OrderBy(x => x.Name), "Id", "Name");
            return View(monitorType);
        }

        // GET: MonitorTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monitorType = await _context.MonitorType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (monitorType == null)
            {
                return NotFound();
            }

            return View(monitorType);
        }

        // POST: MonitorTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var monitorType = await _context.MonitorType.FindAsync(id);
            _context.MonitorType.Remove(monitorType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MonitorTypeExists(int id)
        {
            return _context.MonitorType.Any(e => e.Id == id);
        }
    }
}
