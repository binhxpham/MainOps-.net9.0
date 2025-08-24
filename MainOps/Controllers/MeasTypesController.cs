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
    public class MeasTypesController : BaseController
    {
        private readonly DataContext _context;
        private UserManager<ApplicationUser> _userManager;

        public MeasTypesController(DataContext context,UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
            
        }

        // GET: MeasTypes
        public async Task<IActionResult> Index()
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                return View(await _context.MeasTypes.Include(x => x.Division).ToListAsync());
            }
            return View(await _context.MeasTypes.Where(x=>x.DivisionId.Equals(theuser.DivisionId)).ToListAsync());
        }

        // GET: MeasTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var measType = await _context.MeasTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (measType == null)
            {
                return NotFound();
            }
            return View(measType);
        }

        // GET: MeasTypes/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["DivisionId"] = new SelectList(_context.Divisions, "Id", "Name",user.DivisionId);
            return View();
        }

        // POST: MeasTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type")] MeasType measType)
        {
            var theuser = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                
                //measType.DivisionId = theuser.DivisionId;
                _context.Add(measType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DivisionId"] = new SelectList(_context.Divisions, "Id", "Name",theuser.DivisionId);
            return View(measType);
        }

        // GET: MeasTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var measType = await _context.MeasTypes.FindAsync(id);
            if (measType == null)
            {
                return NotFound();
            }
            var theuser = await _userManager.GetUserAsync(User);
            ViewData["DivisionId"] = new SelectList(_context.Divisions, "Id", "Name", theuser.DivisionId);
            return View(measType);
        }

        // POST: MeasTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,DivisionId")] MeasType measType)
        {
            if (id != measType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(measType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MeasTypeExists(measType.Id))
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
            var theuser = await _userManager.GetUserAsync(User);
            ViewData["DivisionId"] = new SelectList(_context.Divisions, "Id", "Name", theuser.DivisionId);
            return View(measType);
        }

        // GET: MeasTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var measType = await _context.MeasTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (measType == null)
            {
                return NotFound();
            }

            return View(measType);
        }

        // POST: MeasTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var measType = await _context.MeasTypes.FindAsync(id);
            _context.MeasTypes.Remove(measType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MeasTypeExists(int id)
        {
            return _context.MeasTypes.Any(e => e.Id == id);
        }
    }
}
