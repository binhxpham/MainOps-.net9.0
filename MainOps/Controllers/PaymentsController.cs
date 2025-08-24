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
using Microsoft.AspNetCore.Authorization;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,DivisionAdmin,International")]
    public class PaymentsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public PaymentsController(DataContext context,UserManager<ApplicationUser> userManager, IWebHostEnvironment env):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }
        
        // GET: Payments
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if(User.IsInRole("International") && !User.IsInRole("Admin"))
                {
                var payments = _context.Payments.Include(x => x.SubProject).Include(p => p.Project).ThenInclude(x => x.Division)
                    .Where(x => x.Project.Name.Contains("STOCK") && x.Project.DivisionId.Equals(user.DivisionId))
                    .OrderByDescending(x => x.TimeStamp);
                return View(await payments.ToListAsync());
            }
            if (User.IsInRole("Admin"))
            {
                var dataContext = _context.Payments.Include(x => x.SubProject).Include(p => p.Project).ThenInclude(x => x.Division).OrderByDescending(x => x.TimeStamp);
                return View(await dataContext.ToListAsync());
            }
            else
            {
                var dataContext = _context.Payments.Include(x => x.SubProject).Include(p => p.Project).ThenInclude(x => x.Division).Where(x => x.Project.DivisionId.Equals(user.DivisionId)).OrderByDescending(x => x.TimeStamp);
                return View(await dataContext.ToListAsync());
            }
        }

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payment.Project.DivisionId.Equals(user.DivisionId) || User.IsInRole("Admin"))
            {
                if (payment == null)
                {
                    return NotFound();
                }

                return View(payment);
            }
            else
            {
                return NotFound();
            }
            
        }

        // GET: Payments/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["ProjectId"] = await GetProjectList();
                return View();
            }
            else
            {
                ViewData["ProjectId"] = await GetProjectList();
                return View();
            }
            
            
        }

        // POST: Payments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Amount,PayDate,TimeStamp,ProjectId,SubProjectId,PaymentID,Taxes")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(payment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["ProjectId"] = await GetProjectList();
            }
            else
            {
                var projects = await _context.Projects.Where(x => x.DivisionId.Equals(user.DivisionId) && x.Name.Contains("STOCK")).ToListAsync();
                ViewData["ProjectId"] = new SelectList(projects, "Id", "Name",payment.ProjectId);
            }
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Name", payment.SubProjectId);
            return View(payment);
        }

        // GET: Payments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var payment = await _context.Payments.Include(x=>x.Project).Where(x=>x.Id.Equals(id)).SingleOrDefaultAsync();
            if (payment == null)
            {
                return NotFound();
            }
            if (User.IsInRole("International") && !User.IsInRole("Admin"))
            {
                if((payment.Project.Name.Contains("STOCK") && payment.Project.DivisionId.Equals(user.DivisionId)))
                {
                    var projects = await _context.Projects.Where(x => x.DivisionId.Equals(user.DivisionId) && x.Name.Contains("STOCK")).ToListAsync();
                    ViewData["ProjectId"] = new SelectList(projects, "Id", "Name", payment.ProjectId);
                    ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Name", payment.ProjectId);
                    return View(payment);
                }
                else
                {
                    return NotFound();
                }
            }
            if (payment.Project.DivisionId.Equals(user.DivisionId) || User.IsInRole("Admin"))
            {
                
                ViewData["ProjectId"] = await GetProjectList();
                ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Name", payment.ProjectId);
                return View(payment);
            }
            else
            {
                return NotFound();
            }
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Amount,PayDate,TimeStamp,ProjectId,SubProjectId,PaymentID,Taxes")] Payment payment)
        {
            if (id != payment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.Id))
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
            if(User.IsInRole("International") && !User.IsInRole("Admin"))
            {
                var projects = await _context.Projects.Where(x => x.DivisionId.Equals(user.DivisionId) && x.Name.Contains("STOCK")).ToListAsync();
                ViewData["ProjectId"] = new SelectList(projects, "Id", "Name", payment.ProjectId);
                ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Name");
                return View(payment);
            }
            else
            {
                ViewData["ProjectId"] = await GetProjectList();
                ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Name");
                return View(payment);
            }
        }

        // GET: Payments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var payment = await _context.Payments
                .Include(p => p.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payment.Project.DivisionId.Equals(user.DivisionId) || User.IsInRole("Admin"))
            {
                if (payment == null)
                {
                    return NotFound();
                }
                if (User.IsInRole("International") && !User.IsInRole("Admin"))
                {
                    var projects = await _context.Projects.Where(x => x.DivisionId.Equals(user.DivisionId) && x.Name.Contains("STOCK")).ToListAsync();
                    ViewData["ProjectId"] = new SelectList(projects, "Id", "Name", payment.ProjectId);
                    ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Name");
                   
                }
                else
                {
                    ViewData["ProjectId"] = await GetProjectList();
                    ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Name");
                }
                return View(payment);
            }
            else
            {
                return NotFound();
            }
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.Id == id);
        }
    }
}
