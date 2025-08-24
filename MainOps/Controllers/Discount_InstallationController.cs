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
using Microsoft.AspNetCore.Hosting;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
    public class Discounts_InstallationController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public Discounts_InstallationController(DataContext context,UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Discount_Installation
        public async Task<IActionResult> Index()     
        {
            if (!User.IsInRole("Admin")) { 
            var user = await _userManager.GetUserAsync(User);
            var dataContext =  _context.Discount_Installations.Include(d => d.ItemType).ThenInclude(x => x.Project).Where(x => x.ItemType.Project.DivisionId.Equals(user.DivisionId));
            return View(await dataContext.ToListAsync());
            }
            else
            {
                var dataContext = _context.Discount_Installations.Include(d => d.ItemType).ThenInclude(x => x.Project);
                return View(await dataContext.ToListAsync());
            }
        }

        // GET: Discount_Installation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discount_Installation = await _context.Discount_Installations
                .Include(d => d.ItemType).ThenInclude(x => x.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (discount_Installation == null)
            {
                return NotFound();
            }

            return View(discount_Installation);
        }

        // GET: Discount_Installation/Create
        public async Task<IActionResult> Create(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            var itemtype = await _context.ItemTypes.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId) && x.Id.Equals(id)).SingleOrDefaultAsync();
            if (itemtype != null)
            {
                Discount_Installation disc = new Discount_Installation(itemtype);
                ViewData["Project"] = itemtype.Project.Name;
                ViewData["ItemTypeName"] = itemtype.Item_Type;
                return View(disc);
            }
            else
            {
                return NotFound();
            }
        }

        // POST: Discount_Installation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ItemTypeId,Rate,StartDate,EndDate")] Discount_Installation discount_Installation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(discount_Installation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemTypeId"] = new SelectList(_context.ItemTypes, "Id", "Id", discount_Installation.ItemTypeId);
            return View(discount_Installation);
        }

        // GET: Discount_Installation/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discount_Installation = await _context.Discount_Installations.FindAsync(id);
            if (discount_Installation == null)
            {
                return NotFound();
            }
            ViewData["ItemTypeId"] = new SelectList(_context.ItemTypes, "Id", "Id", discount_Installation.ItemTypeId);
            return View(discount_Installation);
        }

        // POST: Discount_Installation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ItemTypeId,Rate,StartDate,EndDate")] Discount_Installation discount_Installation)
        {
            if (id != discount_Installation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(discount_Installation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Discount_InstallationExists(discount_Installation.Id))
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
            ViewData["ItemTypeId"] = new SelectList(_context.ItemTypes, "Id", "Id", discount_Installation.ItemTypeId);
            return View(discount_Installation);
        }

        // GET: Discount_Installation/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discount_Installation = await _context.Discount_Installations
                .Include(d => d.ItemType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (discount_Installation == null)
            {
                return NotFound();
            }

            return View(discount_Installation);
        }

        // POST: Discount_Installation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discount_Installation = await _context.Discount_Installations.FindAsync(id);
            _context.Discount_Installations.Remove(discount_Installation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Discount_InstallationExists(int id)
        {
            return _context.Discount_Installations.Any(e => e.Id == id);
        }
    }
}
