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
using MainOps.Models.ViewModels;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
    public class InvoicesController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public InvoicesController(DataContext context,UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Invoices
        public async Task<IActionResult> Index(int? id = null)
        {
            ViewData["ProjectId"] = await GetProjectList();
            var user = await _userManager.GetUserAsync(User);
            if(id == null) { 
                var dataContext = _context.Invoices.Include(i => i.Project).Include(i => i.SubProject)
                    .Where(x => x.Project.DivisionId.Equals(user.DivisionId));
                return View(await dataContext.ToListAsync());
            }
            else
            {
                var dataContext = _context.Invoices.Include(i => i.Project).Include(i => i.SubProject)
                .Where(x => x.Project.DivisionId.Equals(user.DivisionId) && x.ProjectId.Equals(id));
                return View(await dataContext.ToListAsync());
            }
        }
        public async Task<IActionResult> InvoicesSearch(int? ProjectId = null,int? SubProjectId = null,bool Taxes = true)
        {
            ViewData["ProjectId"] = await GetProjectList();
            var user = await _userManager.GetUserAsync(User);
            if(ProjectId != null && SubProjectId != null && Taxes.Equals(true))
            {
                var invoices = _context.Invoices
                    .Include(i => i.Project).Include(i => i.SubProject)
                    .Where(x => x.ProjectId.Equals(ProjectId) && x.SubProjectId.Equals(SubProjectId) && x.Taxes.Equals(true) && x.Project.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.InvoiceDate);
                return View(await invoices.ToListAsync());
            }
            else if (ProjectId != null && SubProjectId != null && Taxes.Equals(false))
            {
                var invoices = _context.Invoices
                    .Include(i => i.Project).Include(i => i.SubProject)
                    .Where(x => x.ProjectId.Equals(ProjectId) && x.SubProjectId.Equals(SubProjectId) && x.Taxes.Equals(false) && x.Project.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.InvoiceDate);
                return View(await invoices.ToListAsync());
            }
            else if (ProjectId == null && SubProjectId != null && Taxes.Equals(true))
            {
                var invoices = _context.Invoices
                    .Include(i => i.Project).Include(i => i.SubProject)
                    .Where(x => x.SubProjectId.Equals(SubProjectId) && x.Taxes.Equals(true) && x.Project.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.InvoiceDate);
                return View(await invoices.ToListAsync());
            }
            else if (ProjectId == null && SubProjectId != null && Taxes.Equals(false))
            {
                var invoices = _context.Invoices
                    .Include(i => i.Project).Include(i => i.SubProject)
                    .Where(x => x.SubProjectId.Equals(SubProjectId) && x.Taxes.Equals(false) && x.Project.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.InvoiceDate);
                return View(await invoices.ToListAsync());
            }
            else if (ProjectId != null && SubProjectId == null && Taxes.Equals(true))
            {
                var invoices = _context.Invoices
                    .Include(i => i.Project).Include(i => i.SubProject)
                    .Where(x => x.ProjectId.Equals(ProjectId) && x.Taxes.Equals(true) && x.Project.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.InvoiceDate);
                return View(await invoices.ToListAsync());
            }
            else if (ProjectId != null && SubProjectId == null && Taxes.Equals(false))
            {
                var invoices = _context.Invoices
                    .Include(i => i.Project).Include(i => i.SubProject)
                    .Where(x => x.ProjectId.Equals(ProjectId) && x.Taxes.Equals(false) && x.Project.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.InvoiceDate);
                return View(await invoices.ToListAsync());
            }
            else if (ProjectId == null && SubProjectId == null && Taxes.Equals(true))
            {
                var invoices = _context.Invoices
                    .Include(i => i.Project).Include(i => i.SubProject)
                    .Where(x => x.Taxes.Equals(true) && x.Project.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.InvoiceDate);
                return View(await invoices.ToListAsync());
            }
            else if (ProjectId == null && SubProjectId == null && Taxes.Equals(false))
            {
                var invoices = _context.Invoices
                    .Include(i => i.Project).Include(i => i.SubProject)
                    .Where(x => x.Taxes.Equals(false) && x.Project.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.InvoiceDate);
                return View(await invoices.ToListAsync());
            }
            else
            {
                var invoices = _context.Invoices.Where(x => x.Project.DivisionId.Equals(user.DivisionId))
                    .Include(i => i.Project).Include(i => i.SubProject);
                return View(await invoices.ToListAsync());
            }
        }
        public async Task<decimal> InvoicedUntilDate(DateTime d, GenerateInvoiceVM model)
        {
            List<Invoice> invoices = new List<Invoice>();
            if(model.SubProjectId == null) {
                invoices = await _context.Invoices.Where(x => x.ProjectId.Equals(model.ProjectId) && x.TimeStampEnd.Date <= d.Date).ToListAsync();
            }
            else
            {
                invoices = await _context.Invoices.Where(x => x.SubProjectId.Equals(model.SubProjectId) && x.TimeStampEnd.Date <= d.Date).ToListAsync();
            }
            return invoices.Sum(x => x.Amount);
        }
        public async Task<bool> AreAllInvoicesBeforeSent(DateTime d, GenerateInvoiceVM model)
        {
            List<Invoice> invoices = new List<Invoice>();
            if (model.SubProjectId == null)
            {
                invoices = await _context.Invoices.Where(x => x.ProjectId.Equals(model.ProjectId) && x.TimeStampEnd.Date <= d.Date).ToListAsync();
            }
            else
            {
                invoices = await _context.Invoices.Where(x => x.SubProjectId.Equals(model.SubProjectId) && x.TimeStampEnd.Date <= d.Date).ToListAsync();
            }
            for(DateTime date = invoices.OrderByDescending(x=>x.TimeStampStart).First().TimeStampStart.Date; date <= invoices.OrderByDescending(x => x.TimeStampStart).Last().TimeStampStart.Date; date.AddDays(1))
            {
                bool datewasinrange = false;
                foreach(var span in invoices)
                {
                    if(date <= span.TimeStampEnd && date >= span.TimeStampStart)
                    {
                        datewasinrange = true;
                        break;
                    }
                }
                if(datewasinrange == false)
                {
                    return false;
                }
            }
            return true;
        }
        // GET: Invoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Project)
                .Include(i => i.SubProject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // GET: Invoices/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ProjectId"] = await GetProjectList();
            return View();
        } 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,InvoiceID,Amount,InvoiceDate,TimeStampStart,TimeStampEnd,ProjectId,SubProjectId,Taxes,Surcharge,TaxesPercent")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                _context.Add(invoice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Abbreviation", invoice.ProjectId);
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Id", invoice.SubProjectId);
            return View(invoice);
        }

        // GET: Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => x.ProjectId.Equals(invoice.ProjectId)), "Id", "Name", invoice.SubProjectId);
            return View(invoice);
        }

        // POST: Invoices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,InvoiceID,Amount,InvoiceDate,TimeStampStart,TimeStampEnd,ProjectId,SubProjectId,Taxes,Surcharge,TaxesPercent")] Invoice invoice)
        {
            if (id != invoice.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoiceExists(invoice.Id))
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
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Abbreviation", invoice.ProjectId);
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Id", invoice.SubProjectId);
            return View(invoice);
        }

        // GET: Invoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Project)
                .Include(i => i.SubProject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(e => e.Id == id);
        }
    }
}
