using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MainOps.Data;
using MainOps.Models;

namespace MainOps.Controllers
{
    public class LayersController : Controller
    {
        private readonly DataContext _context;

        public LayersController(DataContext context)
        {
            _context = context;
        }

        // GET: Layers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Layers.ToListAsync());
        }

        // GET: Layers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var layer = await _context.Layers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (layer == null)
            {
                return NotFound();
            }

            return View(layer);
        }

        // GET: Layers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Layers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LayerType,BackgroundUrl,TextColor")] Layer layer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(layer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(layer);
        }

        // GET: Layers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var layer = await _context.Layers.FindAsync(id);
            if (layer == null)
            {
                return NotFound();
            }
            return View(layer);
        }

        // POST: Layers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LayerType,BackgroundUrl,TextColor")] Layer layer)
        {
            if (id != layer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(layer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LayerExists(layer.Id))
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
            return View(layer);
        }

        // GET: Layers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var layer = await _context.Layers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (layer == null)
            {
                return NotFound();
            }

            return View(layer);
        }

        // POST: Layers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var layer = await _context.Layers.FindAsync(id);
            _context.Layers.Remove(layer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LayerExists(int id)
        {
            return _context.Layers.Any(e => e.Id == id);
        }
    }
}
