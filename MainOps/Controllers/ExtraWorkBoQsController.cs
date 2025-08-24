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
using Microsoft.AspNetCore.Hosting;

namespace MainOps.Controllers
{
    public class ExtraWorkBoQsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private IWebHostEnvironment _env;

        public ExtraWorkBoQsController(DataContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // GET: ExtraWorkBoQs
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.ExtraWorkBoQs.Include(e => e.BoQHeadLine);
            return View(await dataContext.ToListAsync());
        }

        // GET: ExtraWorkBoQs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var extraWorkBoQ = await _context.ExtraWorkBoQs
                .Include(e => e.BoQHeadLine)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (extraWorkBoQ == null)
            {
                return NotFound();
            }

            return View(extraWorkBoQ);
        }

        // GET: ExtraWorkBoQs/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["BoQHeadLineId"] = new SelectList(_context.BoQHeadLines.Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "HeadLine");
            ViewData["UnitId"] = new SelectList(_context.Units, "Id", "TheUnit");
            ExtraWorkBoQVM model = new ExtraWorkBoQVM();
            model.BoQItems = new List<ExtraWorkBoQItem>();
            model.Descriptions = new List<ExtraWorkBoQDescription>();
            model.Headers = new List<ExtraWorkBoQHeader>();
            for(int i = 1; i<= 20; i++)
            {
                model.BoQItems.Add(new ExtraWorkBoQItem());
            }
            for(int i = 1; i <= 8; i++)
            {
                model.Descriptions.Add(new ExtraWorkBoQDescription());
            }
            for(int i = 1; i <= 5; i++)
            {
                model.Headers.Add(new ExtraWorkBoQHeader());
            }
            return View(model);
        }

        // POST: ExtraWorkBoQs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExtraWorkBoQVM model)
        {
            if (ModelState.IsValid)
            {
                ExtraWorkBoQ extraWorkBoQ = new ExtraWorkBoQ(model);
                _context.Add(extraWorkBoQ);
                await _context.SaveChangesAsync();
                var lastadded = await _context.ExtraWorkBoQs.OrderBy(x => x.Id).LastAsync();
                foreach (var item in model.Headers)
                {
                    if(item.Number != null && item.Title != null) { 
                        item.ExtraWorkBoQId = lastadded.Id;
                        _context.Add(item);
                    }
                }
                foreach(var item in model.Descriptions)
                {
                    if(item.Description != null) { 
                        item.ExtraWorkBoQId = lastadded.Id;
                        _context.Add(item);
                    }
                }
                await _context.SaveChangesAsync();
                for(int i = 0; i <= model.Headers.Where(x => x.Number != null).Count() - 1;i++)
                {
                    foreach(var item in model.BoQItems.Where(x => x.ExtraWorkBoQHeaderId.Equals(model.Headers[i].Number) || x.ExtraWorkBoQHeaderId.Equals(i+1)))
                    {
                        var theheader = await _context.ExtraWorkBoQHeaders.SingleOrDefaultAsync(x => x.ExtraWorkBoQId.Equals(lastadded.Id) && x.Title.Equals(model.Headers[i].Title) && x.Number.Equals(model.Headers[i].Number));
                        item.ExtraWorkBoQHeaderId = theheader.Id;
                       
                        _context.Add(item);
                        //add boq nubmer and boq rental number from original or from new...
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BoQHeadLineId"] = new SelectList(_context.BoQHeadLines, "Id", "HeadLine", model.BoQHeadLineId);
            return View(model);
        }

        // GET: ExtraWorkBoQs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var extraWorkBoQ = await _context.ExtraWorkBoQs.FindAsync(id);
            if (extraWorkBoQ == null)
            {
                return NotFound();
            }
            ViewData["BoQHeadLineId"] = new SelectList(_context.BoQHeadLines, "Id", "Id", extraWorkBoQ.BoQHeadLineId);
            return View(extraWorkBoQ);
        }

        // POST: ExtraWorkBoQs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,BoQHeadLineId")] ExtraWorkBoQ extraWorkBoQ)
        {
            if (id != extraWorkBoQ.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(extraWorkBoQ);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExtraWorkBoQExists(extraWorkBoQ.Id))
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
            ViewData["BoQHeadLineId"] = new SelectList(_context.BoQHeadLines, "Id", "Id", extraWorkBoQ.BoQHeadLineId);
            return View(extraWorkBoQ);
        }

        // GET: ExtraWorkBoQs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var extraWorkBoQ = await _context.ExtraWorkBoQs
                .Include(e => e.BoQHeadLine)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (extraWorkBoQ == null)
            {
                return NotFound();
            }

            return View(extraWorkBoQ);
        }

        // POST: ExtraWorkBoQs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var extraworkboqdescriptions = await _context.ExtraWorkBoQDescriptions.Where(x => x.ExtraWorkBoQId.Equals(id)).ToListAsync();
            foreach(var item in extraworkboqdescriptions)
            {
                _context.ExtraWorkBoQDescriptions.Remove(item);
            }
            var extraworkboqheaders = await _context.ExtraWorkBoQHeaders.Where(x => x.ExtraWorkBoQId.Equals(id)).ToListAsync();
            foreach(var item in extraworkboqheaders)
            {
                var data = await _context.ExtraWorkBoQItems.Where(x => x.ExtraWorkBoQHeaderId.Equals(item.Id)).ToListAsync();
                foreach(var dat in data)
                {
                    _context.ExtraWorkBoQItems.Remove(dat);
                }
                _context.ExtraWorkBoQHeaders.Remove(item);
            }
            await _context.SaveChangesAsync();
            

            var extraWorkBoQ = await _context.ExtraWorkBoQs.FindAsync(id);

            _context.ExtraWorkBoQs.Remove(extraWorkBoQ);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExtraWorkBoQExists(int? id)
        {
            return _context.ExtraWorkBoQs.Any(e => e.Id == id);
        }
    }
}
