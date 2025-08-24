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
    public class CoordSystemsController : BaseController
    {
        private readonly DataContext _context;
        private readonly IStringLocalizer _localizer;
        private readonly UserManager<ApplicationUser> _userManager;

        public CoordSystemsController(DataContext context,IStringLocalizer<CoordSystemsController> localizer, UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            _context = context;
            _localizer = localizer;
            _userManager = userManager;
        }

        // GET: CoordSystems
        public async Task<IActionResult> Index()
        {
            //we test alex
            ViewData["Title"] = _localizer["Index"];
            return View(await _context.CoordSystems.ToListAsync());
        }
        // GET: CoordSystems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coordSystem = await _context.CoordSystems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (coordSystem == null)
            {
                return NotFound();
            }
            ViewData["Title"] = _localizer["Details"];
            return View(coordSystem);
        }

        // GET: CoordSystems/Create
        public IActionResult Create()
        {
            ViewData["Title"] = _localizer["Create"];
            ViewData["CoordSystemId"] = new SelectList(_context.CoordSystems, "Id", "system");
            return View();
        }

        // POST: CoordSystems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,system")] CoordSystem coordSystem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(coordSystem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Title"] = _localizer["Create"];
            return View(coordSystem);
        }

        // GET: CoordSystems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coordSystem = await _context.CoordSystems.FindAsync(id);
            if (coordSystem == null)
            {
                return NotFound();
            }
            ViewData["Title"] = _localizer["Edit"];
            ViewData["CoordSystemId"] = new SelectList(_context.CoordSystems, "Id", "system");
            return View(coordSystem);
        }

        // POST: CoordSystems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,system")] CoordSystem coordSystem)
        {
            if (id != coordSystem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(coordSystem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoordSystemExists(coordSystem.Id))
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
            ViewData["Title"] = _localizer["Edit"];
            return View(coordSystem);
        }

        // GET: CoordSystems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coordSystem = await _context.CoordSystems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (coordSystem == null)
            {
                return NotFound();
            }
            ViewData["Title"] = _localizer["Delete"];
            return View(coordSystem);
        }

        // POST: CoordSystems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coordSystem = await _context.CoordSystems.FindAsync(id);
            _context.CoordSystems.Remove(coordSystem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CoordSystemExists(int id)
        {
            return _context.CoordSystems.Any(e => e.Id == id);
        }
    }
}
