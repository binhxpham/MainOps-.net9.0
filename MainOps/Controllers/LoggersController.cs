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
    public class LoggersController : BaseController
    {
        private readonly DataContext _context;
        private readonly IStringLocalizer _localizer;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoggersController(DataContext context,IStringLocalizer<LoggersController> localizer,UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            _context = context;
            _localizer = localizer;
            _userManager = userManager;
        }

        // GET: Loggers
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = _localizer["Index"];
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                return View(await _context.Loggers.Include(x => x.Division).ToListAsync());
            }
            return View(await _context.Loggers.Include(x=>x.Division).Where(x=>x.Division.Id.Equals(user.DivisionId)).ToListAsync());
        }

        // GET: Loggers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var logger = await _context.Loggers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (logger == null)
            {
                return NotFound();
            }
            ViewData["Title"] = _localizer["Details"];
            return View(logger);
        }

        // GET: Loggers/Create
        public IActionResult Create()
        {
            ViewData["Title"] = _localizer["Create"];
            return View();
        }

        // POST: Loggers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LoggerNo,SerialNo,SimCardNo")] Logger logger)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                logger.DivisionId = user.DivisionId;
                _context.Add(logger);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Title"] = _localizer["Create"];
            return View(logger);
        }

        // GET: Loggers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var logger = await _context.Loggers.FindAsync(id);
            if (logger == null)
            {
                return NotFound();
            }
            ViewData["Title"] = _localizer["Edit"];
            return View(logger);
        }

        // POST: Loggers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LoggerNo,SerialNo,SimCardNo,DivisionId")] Logger logger)
        {
            if (id != logger.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(logger);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoggerExists(logger.Id))
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
            ViewData["Title"] = _localizer["Edit"];
            return View(logger);
        }

        // GET: Loggers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var logger = await _context.Loggers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (logger == null)
            {
                return NotFound();
            }
            ViewData["Title"] = _localizer["Delete"];
            return View(logger);
        }

        // POST: Loggers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var logger = await _context.Loggers.FindAsync(id);
            _context.Loggers.Remove(logger);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoggerExists(int id)
        {
            return _context.Loggers.Any(e => e.Id == id);
        }
    }
}
