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

namespace MainOps.Controllers
{
    public class MaintenanceSubTypesController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MaintenanceSubTypesController(DataContext context,UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: MaintenanceSubTypes
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.MaintenanceSubTypes.Include(m => m.MaintenanceType);
            return View(await dataContext.ToListAsync());
        }

        // GET: MaintenanceSubTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenanceSubType = await _context.MaintenanceSubTypes
                .Include(m => m.MaintenanceType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (maintenanceSubType == null)
            {
                return NotFound();
            }

            return View(maintenanceSubType);
        }

        // GET: MaintenanceSubTypes/Create
        public IActionResult Create()
        {
            ViewData["MaintenanceTypeId"] = new SelectList(_context.MaintenanceTypes, "Id", "Type");
            return View();
        }

        // POST: MaintenanceSubTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type,MaintenanceTypeId")] MaintenanceSubType maintenanceSubType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(maintenanceSubType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaintenanceTypeId"] = new SelectList(_context.MaintenanceTypes, "Id", "Type", maintenanceSubType.MaintenanceTypeId);
            return View(maintenanceSubType);
        }

        // GET: MaintenanceSubTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenanceSubType = await _context.MaintenanceSubTypes.FindAsync(id);
            if (maintenanceSubType == null)
            {
                return NotFound();
            }
            ViewData["MaintenanceTypeId"] = new SelectList(_context.MaintenanceTypes, "Id", "Id", maintenanceSubType.MaintenanceTypeId);
            return View(maintenanceSubType);
        }

        // POST: MaintenanceSubTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,MaintenanceTypeId")] MaintenanceSubType maintenanceSubType)
        {
            if (id != maintenanceSubType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(maintenanceSubType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaintenanceSubTypeExists(maintenanceSubType.Id))
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
            ViewData["MaintenanceTypeId"] = new SelectList(_context.MaintenanceTypes, "Id", "Id", maintenanceSubType.MaintenanceTypeId);
            return View(maintenanceSubType);
        }

        // GET: MaintenanceSubTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenanceSubType = await _context.MaintenanceSubTypes
                .Include(m => m.MaintenanceType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (maintenanceSubType == null)
            {
                return NotFound();
            }

            return View(maintenanceSubType);
        }

        // POST: MaintenanceSubTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var maintenanceSubType = await _context.MaintenanceSubTypes.FindAsync(id);
            _context.MaintenanceSubTypes.Remove(maintenanceSubType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaintenanceSubTypeExists(int id)
        {
            return _context.MaintenanceSubTypes.Any(e => e.Id == id);
        }
    }
}
