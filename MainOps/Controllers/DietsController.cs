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
using Rotativa.AspNetCore;
using MainOps.Services;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,Manager,DivisionAdmin,ProjectMember")]
    public class DietsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public DietsController(DataContext context,UserManager<ApplicationUser> userManager,IEmailSender emailSender):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // GET: Diets
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,Supervisor,ProjectMember")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if(User.IsInRole("Admin") || User.IsInRole("Manager") || User.IsInRole("Supervisor")) { 
            var dataContext = _context.Diets.Include(d => d.Project).Where(x=>x.Project.DivisionId.Equals(user.DivisionId));
            return View(await dataContext.ToListAsync());
            }
            else
            {
                var dataContext = _context.Diets.Include(d => d.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId) && x.UserId.Equals(user.Id));
                return View(await dataContext.ToListAsync());
            }
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,Supervisor,ProjectMember")]
        // GET: Diets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var diet = await _context.Diets
                .Include(d => d.Project).ThenInclude(x=>x.Division)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (User.IsInRole("Admin") || User.IsInRole("Manager") || User.IsInRole("Supervisor"))
            {
                if (!diet.Project.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item." });
                }
            }
            else
            {
                if (diet.UserId.Equals(user.Id))
                {
                    return new ViewAsPdf("_Details", diet);
                }
            }
            if (diet == null)
            {
                return NotFound();
            }

            return new ViewAsPdf("_Details", diet);
        }

        // GET: Diets/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ProjectId"] = await GetProjectList();
            Diet model = new Diet();
            model.HourlyPaid = true;
            return View(model);
        }

        // POST: Diets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,FullName,EmployeeNumber,PayPeriod,ProjectId,Day1_start,Day2_start,Day3_start,Day4_start,Day5_start,Day6_start,Day1_end,Day2_end,Day3_end,Day4_end,Day5_end,Day6_end," +
            "WorkPlaceName1,WorkPlaceName2,WorkPlaceName3,WorkPlaceName4,WorkPlaceName5,WorkPlaceName6," +
            "SelfContainedExpenses,LivingInCamperWagon,CalculationOfDietsAndSmallNecessities,HourAddon,DeductionBreakFast,DeductionLunch,DeductionDinner," +
            "SelfContainedExpensesSubTotal,LivingInCamperWagonSubTotal,CalculationOfDietsAndSmallNecessitiesSubTotal,HourAddonSubTotal,DeductionBreakFastSubTotal,DeductionLunchSubTotal,DeductionDinnerSubTotal,Total," +
            "SignatureEmployee,SignatureSupervisor,HourlyPaid")] Diet diet)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                diet.UserId = user.Id;
                _context.Add(diet);
                await _context.SaveChangesAsync();
                var project = await _context.Projects.SingleOrDefaultAsync(x => x.Id.Equals(diet.ProjectId));
                var usersupervisor = await _context.Users.Where(x => x.full_name().Equals(project.Responsible_Person) && x.DivisionId.Equals(project.DivisionId)).SingleOrDefaultAsync();
                if(usersupervisor != null)
                {
                    string footerstringHTML = "<br />Hölscher Jensen A/S<br />Fabriksparken 37<br />2600 Glostrup<br />Denmark";
                    string footerstringPLAIN = "\r\n\r\nHölscher Jensen A/S \r\nFabriksparken 37\r\n2600 Glostrup\r\nDenmark";
                    var lastadded = await _context.Diets.LastAsync();
                    await _emailSender.SendEmailAsync3(usersupervisor.Email, "Diets/Allowances", "Dear Project Manager,<br /><br />" + "New allowance added on one of your projects: " + project.Name + " for the user: " + user.full_name() + "<br /><br />Go to sign on: https://hj-mainops.com/Diets/Edit/" + lastadded.Id + "?", footerstringHTML, footerstringPLAIN);
                }
                else
                {
                    string footerstringHTML = "<br />Hölscher Jensen A/S<br />Fabriksparken 37<br />2600 Glostrup<br />Denmark";
                    string footerstringPLAIN = "\r\n\r\nHölscher Jensen A/S \r\nFabriksparken 37\r\n2600 Glostrup\r\nDenmark";
                    var lastadded = await _context.Diets.LastAsync();
                    await _emailSender.SendEmailAsync3("rml@hj-as.dk", "Diets/Allowances", "Dear Project Manager,<br /><br />" + "New allowance added on one of your projects: " + project.Name + " for the user: " + user.full_name() + "<br /><br />Go to sign on: https://hj-mainops.com/Diets/Edit/" + lastadded.Id + "?", footerstringHTML, footerstringPLAIN);
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = await GetProjectList();
            return View(diet);
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,Supervisor")]
        // GET: Diets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var diet = await _context.Diets.Include(x=>x.Project).SingleOrDefaultAsync(x => x.Id.Equals(id));

            if (!diet.Project.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item." });
            }
            if (diet == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = await GetProjectList();
            return View(diet);
        }

        // POST: Diets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,Supervisor")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,UserId,EmployeeNumber,PayPeriod,ProjectId,Day1_start,Day2_start,Day3_start,Day4_start,Day5_start,Day6_start,Day1_end,Day2_end,Day3_end,Day4_end,Day5_end,Day6_end," +
            "WorkPlaceName1,WorkPlaceName2,WorkPlaceName3,WorkPlaceName4,WorkPlaceName5,WorkPlaceName6," +
            "SelfContainedExpenses,LivingInCamperWagon,CalculationOfDietsAndSmallNecessities,HourAddon,DeductionBreakFast,DeductionLunch,DeductionDinner," +
            "SelfContainedExpensesSubTotal,LivingInCamperWagonSubTotal,CalculationOfDietsAndSmallNecessitiesSubTotal,HourAddonSubTotal,DeductionBreakFastSubTotal,DeductionLunchSubTotal,DeductionDinnerSubTotal,Total," +
            "SignatureEmployee,SignatureSupervisor,HourlyPaid")] Diet diet)
        {
            if (id != diet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(diet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DietExists(diet.Id))
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
            ViewData["ProjectId"] = await GetProjectList();
            return View(diet);
        }
        [Authorize(Roles = "Admin,DivisionAdmin")]
        // GET: Diets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var diet = await _context.Diets
                .Include(d => d.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (!diet.Project.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item." });
            }
            if (diet == null)
            {
                return NotFound();
            }

            return View(diet);
        }
        [Authorize(Roles = "Admin,DivisionAdmin")]
        // POST: Diets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var diet = await _context.Diets.FindAsync(id);
            _context.Diets.Remove(diet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DietExists(int id)
        {
            return _context.Diets.Any(e => e.Id == id);
        }
    }
}
