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
using Microsoft.AspNetCore.Authorization;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,Member,ProjectMember,DivisionAdmin,StorageAdmin")]
    public class InformationEntriesController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public InformationEntriesController(DataContext context,UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: InformationEntries
        public async Task<IActionResult> Index()
        {

            var tobedeleted = await _context.InformationEntries.Where(x => x.DateEntered < DateTime.Now.AddDays(-7)).ToListAsync();
            if(tobedeleted != null)
            {
                if(tobedeleted.Count > 0)
                {
                    foreach (var entry in tobedeleted)
                    {
                        _context.Remove(entry);
                    }
                }
                await _context.SaveChangesAsync();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user.Active == false)
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You are inactive" });
            }
            var dataContext = _context.InformationEntries.Include(i => i.Division).Where(x=>x.DivisionId.Equals(user.DivisionId)).OrderByDescending(x=>x.DateEntered);
            return View(await dataContext.ToListAsync());
        }
        public async Task<IActionResult> SharedDocuments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.Active == false)
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You are inactive" });
            }
            var docs = await _context.Documents
                .Include(x=>x.DocumentType)
                .Include(x=>x.Project)
                .Where(x => x.DocumentType.Type.ToLower().Equals("shared") 
                && (x.Project.DivisionId.Equals(user.DivisionId) || x.DivisionId.Equals(user.DivisionId))).ToListAsync();
            return View(docs);
        }
        // GET: InformationEntries/Details/5

        // GET: InformationEntries/Create
        [HttpGet]
        [Authorize(Roles ="Admin,DivisionAdmin,Manager")]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.Active == false)
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You are inactive" });
            }
            return View();
        }

        // POST: InformationEntries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Text")] InformationEntry informationEntry)
        {
            var user = await _userManager.GetUserAsync(User);      
            if (ModelState.IsValid)
            {
                informationEntry.DivisionId = user.DivisionId;
                informationEntry.DateEntered = DateTime.Now;
                informationEntry.DoneBy = user.full_name();
                informationEntry.Text = informationEntry.Text.Replace("\r\n", "<br />");
                _context.Add(informationEntry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(informationEntry);
        }

        // GET: InformationEntries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user.Active == false)
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You are inactive" });
            }
            var informationEntry = await _context.InformationEntries.FindAsync(id);
            informationEntry.Text = informationEntry.Text.Replace("<br />", "\r\n");
            if (informationEntry == null)
            {
                return NotFound();
            }
            return View(informationEntry);
        }

        // POST: InformationEntries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Text")] InformationEntry informationEntry)
        {
            var user = await _userManager.GetUserAsync(User);
            if (id != informationEntry.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    informationEntry.DivisionId = user.DivisionId;
                    informationEntry.DateEntered = DateTime.Now;
                    informationEntry.DoneBy = user.full_name();
                    informationEntry.Text = informationEntry.Text.Replace("\r\n", "<br />");
                    _context.Update(informationEntry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InformationEntryExists(informationEntry.Id))
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
            return View(informationEntry);
        }

        // GET: InformationEntries/Delete/5
        [Authorize(Roles ="Admin,DivisionAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var informationEntry = await _context.InformationEntries
                .Include(i => i.Division)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (informationEntry == null)
            {
                return NotFound();
            }

            return View(informationEntry);
        }

        // POST: InformationEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var informationEntry = await _context.InformationEntries.FindAsync(id);
            _context.InformationEntries.Remove(informationEntry);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InformationEntryExists(int id)
        {
            return _context.InformationEntries.Any(e => e.Id == id);
        }
    }
}
