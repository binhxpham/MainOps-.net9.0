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
    public class HJItemMasterClassesController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HJItemMasterClassesController(DataContext context,UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: HJItemMasterClasses
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                return View(await _context.HJItemMasterClasses.OrderBy(x => x.ClassNumber).ToListAsync());
            }
            else 
            {
                return View(await _context.HJItemMasterClasses.Where(x => x.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.ClassNumber).ToListAsync());
            }                 
            
        }

        // GET: HJItemMasterClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var hJItemMasterClass = await _context.HJItemMasterClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hJItemMasterClass == null)
            {
                return NotFound();
            }
            else if(!hJItemMasterClass.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have acccess to this item" });
            }

            return View(hJItemMasterClass);
        }

        // GET: HJItemMasterClasses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HJItemMasterClasses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClassName,ClassNumber")] HJItemMasterClass hJItemMasterClass)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                hJItemMasterClass.DivisionId = user.DivisionId;
                _context.Add(hJItemMasterClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hJItemMasterClass);
        }

        // GET: HJItemMasterClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var hJItemMasterClass = await _context.HJItemMasterClasses.FindAsync(id);
            if (hJItemMasterClass == null)
            {
                return NotFound();
            }
            else if (!hJItemMasterClass.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have acccess to this item" });
            }
            return View(hJItemMasterClass);
        }

        // POST: HJItemMasterClasses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClassName,ClassNumber,DivisionId")] HJItemMasterClass hJItemMasterClass)
        {
            if (id != hJItemMasterClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hJItemMasterClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HJItemMasterClassExists(hJItemMasterClass.Id))
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
            return View(hJItemMasterClass);
        }

        // GET: HJItemMasterClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var hJItemMasterClass = await _context.HJItemMasterClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hJItemMasterClass == null)
            {
                return NotFound();
            }
            else if (!hJItemMasterClass.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have acccess to this item" });
            }

            return View(hJItemMasterClass);
        }

        // POST: HJItemMasterClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hJItemMasterClass = await _context.HJItemMasterClasses.FindAsync(id);
            _context.HJItemMasterClasses.Remove(hJItemMasterClass);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HJItemMasterClassExists(int id)
        {
            return _context.HJItemMasterClasses.Any(e => e.Id == id);
        }
    }
}
