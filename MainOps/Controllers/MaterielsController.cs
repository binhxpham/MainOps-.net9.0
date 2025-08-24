using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MainOps.Data;
using MainOps.Models.CGJClassesBeton;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MainOps.Models;

namespace MainOps.Controllers

{
    [Authorize(Roles = "Manager")]
    public class MaterielsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MaterielsController(DataContext context, UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            
            _context = context;
            _userManager = userManager;
        }

        // GET: Materiels
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user.DivisionId != 10 && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this part of MainOps" });
            }
            return View(await _context.Materieller.ToListAsync());
        }

        // GET: Materiels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.DivisionId != 10 && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this part of MainOps" });
            }
            if (id == null)
            {
                return NotFound();
            }

            var materiel = await _context.Materieller
                .FirstOrDefaultAsync(m => m.Id == id);
            if (materiel == null)
            {
                return NotFound();
            }

            return View(materiel);
        }

        // GET: Materiels/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.DivisionId != 10 && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this part of MainOps" });
            }
            return View();
        }

        // POST: Materiels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Materiellet")] Materiel materiel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.DivisionId != 10 && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this part of MainOps" });
            }
            if (ModelState.IsValid)
            {
                _context.Add(materiel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(materiel);
        }

        // GET: Materiels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.DivisionId != 10 && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this part of MainOps" });
            }
            if (id == null)
            {
                return NotFound();
            }

            var materiel = await _context.Materieller.FindAsync(id);
            if (materiel == null)
            {
                return NotFound();
            }
            return View(materiel);
        }

        // POST: Materiels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Materiellet")] Materiel materiel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.DivisionId != 10 && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this part of MainOps" });
            }
            if (id != materiel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(materiel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaterielExists(materiel.Id))
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
            return View(materiel);
        }

        // GET: Materiels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.DivisionId != 10 && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this part of MainOps" });
            }
            if (id == null)
            {
                return NotFound();
            }

            var materiel = await _context.Materieller
                .FirstOrDefaultAsync(m => m.Id == id);
            if (materiel == null)
            {
                return NotFound();
            }

            return View(materiel);
        }

        // POST: Materiels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.DivisionId != 10 && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this part of MainOps" });
            }
            var materiel = await _context.Materieller.FindAsync(id);
            _context.Materieller.Remove(materiel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaterielExists(int id)
        {

            return _context.Materieller.Any(e => e.Id == id);
        }
    }
}
