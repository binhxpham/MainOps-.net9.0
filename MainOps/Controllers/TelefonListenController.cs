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
using Microsoft.AspNetCore.Identity;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,Manager,ProjectMember,Member,DivisionAdmin,StorageManager")]
    public class TelefonListenController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TelefonListenController(DataContext context,UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: TelefonListen
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                return View(await _context.TelefonListen.Include(x=>x.Division).OrderBy(x => x.ForNavn).ThenBy(x => x.Efternavn).ToListAsync());
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                return View(await _context.TelefonListen.Include(x => x.Division).Where(x=>x.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.ForNavn).ThenBy(x => x.Efternavn).ToListAsync());
            }
            
        }

        // GET: TelefonListen/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var telefonListe = await _context.TelefonListen
                .FirstOrDefaultAsync(m => m.Id == id);
            if (telefonListe == null)
            {
                return NotFound();
            }
            else if (!telefonListe.DivisionId.Equals(user.DivisionId))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
            }

            return View(telefonListe);
        }

        // GET: TelefonListen/Create
        [Authorize(Roles = "Admin,Manager,DivisionAdmin")]
        public async Task<IActionResult> Create()
        {
            if (User.IsInRole("Admin"))
            {
                ViewData["DivisionId"] = new SelectList(_context.Divisions, "Id", "Name");
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                ViewData["DivisionId"] = new SelectList(_context.Divisions.Where(x=>x.Id.Equals(user.DivisionId)), "Id", "Name");
            }
            return View();
        }

        // POST: TelefonListen/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,DivisionAdmin")]
        public async Task<IActionResult> Create([Bind("Id,ForNavn,Efternavn,Titel,Telefonnr,Email,Leder,Loennr")] TelefonListe telefonListe)
        {
            if (ModelState.IsValid)
            {
                _context.Add(telefonListe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(telefonListe);
        }

        // GET: TelefonListen/Edit/5
        [Authorize(Roles = "Admin,Manager,DivisionAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var telefonListe = await _context.TelefonListen.FindAsync(id);
            if (telefonListe == null)
            {
                return NotFound();
            }
            else if (!telefonListe.DivisionId.Equals(user.DivisionId))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
            }
            return View(telefonListe);
        }

        // POST: TelefonListen/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,DivisionAdmin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ForNavn,Efternavn,Titel,Telefonnr,Email,Leder,Loennr")] TelefonListe telefonListe)
        {
            if (id != telefonListe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(telefonListe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TelefonListeExists(telefonListe.Id))
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
            return View(telefonListe);
        }

        // GET: TelefonListen/Delete/5
        [Authorize(Roles = "Admin,Manager,DivisionAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var telefonListe = await _context.TelefonListen
                .FirstOrDefaultAsync(m => m.Id == id);
            if (telefonListe == null)
            {
                return NotFound();
            }
            else if (!telefonListe.DivisionId.Equals(user.DivisionId))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
            }

            return View(telefonListe);
        }

        // POST: TelefonListen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,DivisionAdmin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var telefonListe = await _context.TelefonListen.FindAsync(id);
            _context.TelefonListen.Remove(telefonListe);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TelefonListeExists(int id)
        {
            return _context.TelefonListen.Any(e => e.Id == id);
        }
    }
}
