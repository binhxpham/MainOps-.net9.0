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
    public class LoggerChangesController : BaseController
    {
        private readonly DataContext _context;
        private readonly IStringLocalizer _localizer;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoggerChangesController(DataContext context,UserManager<ApplicationUser> userManager,IStringLocalizer<LoggerChangesController> localizer):base(context,userManager)
        {
            _context = context;
            _localizer = localizer;
            _userManager = userManager;
        }
        [HttpPost]
        public async Task<IActionResult> Combsearch(string searchstring)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(searchstring))
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (User.IsInRole("Admin"))
                    {
                        var dataContextAdmin = await _context.LoggerChanges
                       .Include(x => x.Logger).Include(x => x.MeasPoint).ThenInclude(x => x.Project).ThenInclude(x => x.Division).Include(x => x.Logger)
                       .Where(b => (b.MeasPoint.Name.ToLower().Contains(searchstring.ToLower()) 
                       || b.MeasPoint.logger.LoggerNo.ToString().Contains(searchstring))).OrderBy(b => b.MeasPoint.Project.Name).ThenBy(b => b.MeasPoint.Name).ToListAsync();
                        ViewData["Title"] = _localizer["Index"];
                        return View(nameof(Index), dataContextAdmin);
                    }
                    var dataContext = await _context.LoggerChanges
                        .Include(x => x.Logger).Include(x => x.MeasPoint).ThenInclude(x => x.Project).ThenInclude(x=>x.Division).Include(x => x.Logger)
                        .Where(b=>b.MeasPoint.Project.Division.Id.Equals(user.DivisionId) 
                        && (b.MeasPoint.Name.ToLower().Contains(searchstring.ToLower()) 
                        || b.MeasPoint.logger.LoggerNo.ToString().Contains(searchstring))).OrderBy(b => b.MeasPoint.Project.Name).ThenBy(b => b.MeasPoint.Name ).ToListAsync();
                    ViewData["Title"] = _localizer["Index"];
                    return View(nameof(Index), dataContext);
                }


            }
            ViewData["Title"] = _localizer["Index"];
            return View(nameof(Index));
        }
        [HttpGet]
        public async Task<JsonResult> AutoComplete(string search)
        {
            var user = await _userManager.GetUserAsync(User);
            var results = _context.LoggerChanges.Include(x => x.MeasPoint).ThenInclude(x => x.Project).Include(x => x.Logger)
                .Where(b => b.MeasPoint.Project.Division.Id.Equals(user.DivisionId) && (b.MeasPoint.Name.ToLower().Contains(search.ToLower()) || b.MeasPoint.logger.LoggerNo.ToString().Contains(search))).ToList();
            return Json(results.Select(m => new
            {
                id = m.Id,
                value = m.MeasPoint.Name,
                label = m.MeasPoint.logger.LoggerNo.ToString() + '_' + m.MeasPoint.Project.Name + '_' + m.MeasPoint.Name
            }).OrderBy(x => x.label));
        }
        // GET: LoggerChanges
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["Title"] = _localizer["Index"];
            return View(await _context.LoggerChanges.Include(x=>x.MeasPoint).Include(x=>x.Logger).Where(x=>x.MeasPoint.Project.Division.Id.Equals(user.DivisionId)).OrderBy(x=>x.When).ToListAsync());
        }

        // GET: LoggerChanges/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loggerChange = await _context.LoggerChanges
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loggerChange == null)
            {
                return NotFound();
            }
            ViewData["Title"] = _localizer["Details"];
            return View(loggerChange);
        }

        // GET: LoggerChanges/Create
        public IActionResult Create()
        {
            ViewData["Title"] = _localizer["Create"];
            ViewData["LoggerId"] = new SelectList(_context.Loggers, "Id", "LoggerNo");
            ViewData["MeasPointId"] = new SelectList(_context.MeasPoints, "Id", "Name");
            return View();
        }

        // POST: LoggerChanges/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LoggerId,MeasPointId,LoggerAdded,LoggerRemoved,When")] LoggerChange loggerChange)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loggerChange);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Title"] = _localizer["Create"];
            ViewData["LoggerId"] = new SelectList(_context.Loggers, "Id", "LoggerNo",loggerChange.LoggerId);
            ViewData["MeasPointId"] = new SelectList(_context.MeasPoints, "Id", "Name",loggerChange.MeasPointId);
            return View(loggerChange);
        }

        // GET: LoggerChanges/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loggerChange = await _context.LoggerChanges.FindAsync(id);
            if (loggerChange == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            ViewData["Title"] = _localizer["Edit"];
            ViewData["LoggerId"] = new SelectList(_context.Loggers.Include(x=>x.Division).Where(x=>x.Division.Id.Equals(user.DivisionId)), "Id", "LoggerNo");
            ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Include(x=>x.Project).ThenInclude(x=>x.Division).Where(x=>x.Project.Division.Id.Equals(user.DivisionId)).ToList(), "Id", "Name");
            return View(loggerChange);
        }

        // POST: LoggerChanges/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LoggerId,MeasPointId,LoggerAdded,LoggerRemoved,When")] LoggerChange loggerChange)
        {
            if (id != loggerChange.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loggerChange);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoggerChangeExists(loggerChange.Id))
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
            var user = await _userManager.GetUserAsync(User);
            ViewData["Title"] = _localizer["Edit"];
            ViewData["LoggerId"] = new SelectList(_context.Loggers.Include(x => x.Division).Where(x => x.Division.Id.Equals(user.DivisionId)), "Id", "LoggerNo", loggerChange.LoggerId);
            ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Project.Division.Id.Equals(user.DivisionId)).ToList(), "Id", "Name", loggerChange.MeasPointId);
            return View(loggerChange);
        }

        // GET: LoggerChanges/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loggerChange = await _context.LoggerChanges
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loggerChange == null)
            {
                return NotFound();
            }
            ViewData["Title"] = _localizer["Delete"];
            return View(loggerChange);
        }

        // POST: LoggerChanges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loggerChange = await _context.LoggerChanges.FindAsync(id);
            _context.LoggerChanges.Remove(loggerChange);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoggerChangeExists(int id)
        {
            return _context.LoggerChanges.Any(e => e.Id == id);
        }
    }
}
