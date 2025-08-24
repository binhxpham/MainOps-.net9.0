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
    public class DiscountsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public DiscountsController(DataContext context,UserManager<ApplicationUser> userManager, IWebHostEnvironment env):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // GET: Discounts
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var dataContext = _context.Discounts.Include(d => d.ItemType).ThenInclude(x=>x.Project).Where(x=>x.ItemType.Project.DivisionId.Equals(user.DivisionId));
            return View(await dataContext.ToListAsync());
        }


        // GET: Discounts/Create
        public async Task<IActionResult> Create(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            var itemtype = await _context.ItemTypes.Include(x=>x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId) && x.Id.Equals(id)).SingleOrDefaultAsync();
            if(itemtype != null)
            {
                Discount disc = new Discount();
                disc.ItemTypeId = itemtype.Id;
                ViewData["Project"] = itemtype.Project.Name;
                ViewData["ItemTypeName"] = itemtype.Item_Type;
                return View(disc);
            }
            else
            {
                return NotFound();
            }
        }

        // POST: Discounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ItemTypeId,Rate,StartDate,EndDate")] Discount discount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(discount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(discount);
        }

        // GET: Discounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discount = await _context.Discounts.Include(x => x.ItemType).Where(x => x.Id.Equals(id)).SingleOrDefaultAsync();
            ViewData["ItemTypeId"] = new SelectList(_context.ItemTypes.Where(x => x.ProjectId.Equals(discount.ItemType.ProjectId)), "Id", "Item_Type", discount.ItemTypeId);
            if (discount == null)
            {
                return NotFound();
            }
            return View(discount);
        }

        // POST: Discounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ItemTypeId,Rate,StartDate,EndDate")] Discount discount)
        {
            if (id != discount.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(discount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiscountExists(discount.Id))
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
            ViewData["ItemTypeId"] = new SelectList(_context.ItemTypes, "Id", "Id", discount.ItemTypeId);
            return View(discount);
        }

        // GET: Discounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discount = await _context.Discounts
                .Include(d => d.ItemType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (discount == null)
            {
                return NotFound();
            }

            return View(discount);
        }

        // POST: Discounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discount = await _context.Discounts.FindAsync(id);
            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiscountExists(int id)
        {
            return _context.Discounts.Any(e => e.Id == id);
        }
    }
}
