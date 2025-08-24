using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MainOps.Data;
using MainOps.Models;
using MainOps.Models.ReportClasses;

namespace MainOps.Controllers
{
    public class KSTypesController : Controller
    {
        private readonly DataContext _context;

        public KSTypesController(DataContext context)
        {
            _context = context;
        }

        // GET: KSTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.KSTypes.ToListAsync());
        }

        // GET: KSTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kSType = await _context.KSTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (kSType == null)
            {
                return NotFound();
            }

            return View(kSType);
        }

        // GET: KSTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: KSTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,name")] KSType kSType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(kSType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(kSType);
        }

        // GET: KSTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kSType = await _context.KSTypes.FindAsync(id);
            if (kSType == null)
            {
                return NotFound();
            }
            return View(kSType);
        }

        // POST: KSTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name")] KSType kSType)
        {
            if (id != kSType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kSType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KSTypeExists(kSType.Id))
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
            return View(kSType);
        }

        // GET: KSTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kSType = await _context.KSTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (kSType == null)
            {
                return NotFound();
            }

            return View(kSType);
        }

        // POST: KSTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kSType = await _context.KSTypes.FindAsync(id);
            _context.KSTypes.Remove(kSType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KSTypeExists(int id)
        {
            return _context.KSTypes.Any(e => e.Id == id);
        }
    }
}
