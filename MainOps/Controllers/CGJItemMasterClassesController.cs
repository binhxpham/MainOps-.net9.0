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
    public class CGJItemMasterClassesController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CGJItemMasterClassesController(DataContext context,UserManager<ApplicationUser> userManager):base(context,userManager)
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
                return View(await _context.CGJItemMasterClasses.OrderBy(x => x.ClassNumber).ToListAsync());
            }
            else 
            {
                return View(await _context.CGJItemMasterClasses.Where(x => x.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.ClassNumber).ToListAsync());
            }                
            
        }

        // GET: CGJItemMasterClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var cgJItemMasterClass = await _context.CGJItemMasterClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cgJItemMasterClass == null)
            {
                return NotFound();
            }
            else if(cgJItemMasterClass?.DivisionId!=user.DivisionId && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have acccess to this item" });
            }

            return View(cgJItemMasterClass);
        }

        // GET: HJItemMasterClasses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CGJItemMasterClasses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClassName,ClassNumber")] CGJItemMasterClass cgJItemMasterClass)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                cgJItemMasterClass.DivisionId = user.DivisionId;
                _context.Add(cgJItemMasterClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cgJItemMasterClass);
        }

        // GET: CGJItemMasterClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var cgJItemMasterClass = await _context.CGJItemMasterClasses.FindAsync(id);
            if (cgJItemMasterClass == null)
            {
                return NotFound();
            }
            else if (cgJItemMasterClass?.DivisionId!=(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have acccess to this item" });
            }
            return View(cgJItemMasterClass);
        }

        // POST: CGJItemMasterClasses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClassName,ClassNumber,DivisionId")] CGJItemMasterClass cgJItemMasterClass)
        {
            if (id != cgJItemMasterClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cgJItemMasterClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CGJItemMasterClassExists(cgJItemMasterClass.Id))
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
            return View(cgJItemMasterClass);
        }

        // GET: HJItemMasterClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var cgJItemMasterClass = await _context.CGJItemMasterClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cgJItemMasterClass == null)
            {
                return NotFound();
            }
            else if (!cgJItemMasterClass.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have acccess to this item" });
            }

            return View(cgJItemMasterClass);
        }

        // POST: HJItemMasterClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cgJItemMasterClass = await _context.CGJItemMasterClasses.FindAsync(id);
            _context.CGJItemMasterClasses.Remove(cgJItemMasterClass);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CGJItemMasterClassExists(int id)
        {
            return _context.CGJItemMasterClasses.Any(e => e.Id == id);
        }
    }
}
