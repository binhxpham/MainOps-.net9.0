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
    public class SmallPartsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SmallPartsController(DataContext context,UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: SmallParts
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.SmallParts.Include(s => s.ItemType);
            return View(await dataContext.ToListAsync());
        }

        // GET: SmallParts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var smallPart = await _context.SmallParts
                .Include(s => s.ItemType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (smallPart == null)
            {
                return NotFound();
            }

            return View(smallPart);
        }

        // GET: SmallParts/Create
        public IActionResult Create()
        {
            ViewData["ItemTypeId"] = new SelectList(_context.ItemTypes.Where(x=>x.Item_Type.Contains("Small")), "Id", "Item_Type");
            return View();
        }

        // POST: SmallParts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ItemTypeId")] SmallPart smallPart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(smallPart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemTypeId"] = new SelectList(_context.ItemTypes, "Id", "Id", smallPart.ItemTypeId);
            return View(smallPart);
        }

        // GET: SmallParts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var smallPart = await _context.SmallParts.FindAsync(id);
            if (smallPart == null)
            {
                return NotFound();
            }
            ViewData["ItemTypeId"] = new SelectList(_context.ItemTypes, "Id", "Id", smallPart.ItemTypeId);
            return View(smallPart);
        }

        // POST: SmallParts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ItemTypeId")] SmallPart smallPart)
        {
            if (id != smallPart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(smallPart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SmallPartExists(smallPart.Id))
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
            ViewData["ItemTypeId"] = new SelectList(_context.ItemTypes, "Id", "Id", smallPart.ItemTypeId);
            return View(smallPart);
        }

        // GET: SmallParts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var smallPart = await _context.SmallParts
                .Include(s => s.ItemType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (smallPart == null)
            {
                return NotFound();
            }

            return View(smallPart);
        }

        // POST: SmallParts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var smallPart = await _context.SmallParts.FindAsync(id);
            _context.SmallParts.Remove(smallPart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SmallPartExists(int id)
        {
            return _context.SmallParts.Any(e => e.Id == id);
        }
    }
}
