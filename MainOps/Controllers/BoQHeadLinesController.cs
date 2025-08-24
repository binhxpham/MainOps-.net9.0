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
using System.Text.RegularExpressions;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,Manager,DivisionAdmin,ProjectMember")]
    public class BoQHeadLinesController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BoQHeadLinesController(DataContext context, UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public JsonResult GetVariationOrdersProject(string theId)
        {
            int ProjectId = Convert.ToInt32(theId);
            List<BoQHeadLine> headlinelist = new List<BoQHeadLine>();
            if(theId != null)
            {
                headlinelist =  _context.BoQHeadLines.Where(x => x.ProjectId.Equals(ProjectId) && x.Type.Equals("ExtraWork")).ToList();
                return Json(headlinelist);
            }
            return Json(headlinelist);
        }
        // GET: BoQHeadLines
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {    
                var dataContext = _context.BoQHeadLines.Include(b => b.Project).OrderBy(x => x.ProjectId).ThenBy(x => x.BoQnum);
                return View(await dataContext.ToListAsync());
            }
            else
            {
                var dataContext = _context.BoQHeadLines.Include(x => x.Project).Where(x=>x.Project.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.ProjectId).ThenBy(x => x.BoQnum);
                return View(await dataContext.ToListAsync());
            }
            
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,Guest,International")]
        [HttpGet]
        public JsonResult GetHeadLinesProject(string theId)
        {
            int Id = Convert.ToInt32(theId);
            var thedata = _context.BoQHeadLines.Where(x => x.ProjectId.Equals(Id) && !x.Type.Equals("Rental")).OrderBy(x=>x.BoQnum).ToList();
            return Json(thedata);
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,Guest,International")]
        [HttpGet]
        public JsonResult GetHeadLinesProjectRental(string theId)
        {
            int Id = Convert.ToInt32(theId);
            var thedata = _context.BoQHeadLines.Where(x => x.ProjectId.Equals(Id) && x.Type.Equals("Rental")).OrderBy(x => x.BoQnum).ToList();
            return Json(thedata);
        }
        [HttpGet]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,Guest,International")]
        public JsonResult GetBoQNum(string theId)
        {
            int Id = Convert.ToInt32(theId);
            var thedata = (from it in _context.ItemTypes
                           join bqh in _context.BoQHeadLines
                           on Convert.ToInt32(it.BoQnr) equals Convert.ToInt32(bqh.BoQnum)
                           where bqh.Id.Equals(Id)
                           && it.ProjectId.Equals(bqh.ProjectId)
                           select it)
                           .OrderBy(x => x.BoQnr).Last();
            return Json(thedata);
        }
        [HttpGet]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,Guest,International")]
        public JsonResult GetBoQNumRental(string theId)
        {
            int Id = Convert.ToInt32(theId);
            var thedata = (from it in _context.ItemTypes
                           join bqh in _context.BoQHeadLines
                           on Convert.ToInt32(it.BoQnr_Rental) equals Convert.ToInt32(bqh.BoQnum)
                           where bqh.Id.Equals(Id)
                           && it.ProjectId.Equals(bqh.ProjectId)
                           select it)
                           .OrderBy(x => x.BoQnr_Rental).Last();
            return Json(thedata);
        }
        // GET: BoQHeadLines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boQHeadLine = await _context.BoQHeadLines
                .Include(b => b.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boQHeadLine == null)
            {
                return NotFound();
            }

            return View(boQHeadLine);
        }

        // GET: BoQHeadLines/Create
        [Authorize(Roles = "Admin,Manager,DivisionAdmin")]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects.OrderBy(x=>x.Name), "Id", "Name");
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x=>x.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.Name), "Id", "Name");
            }
                
            return View();
        }

        // POST: BoQHeadLines/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,DivisionAdmin")]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,BoQnum,HeadLine,Type")] BoQHeadLine boQHeadLine)
        {
            if (ModelState.IsValid)
            {
                _context.Add(boQHeadLine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects.OrderBy(x => x.Name), "Id", "Name", boQHeadLine.ProjectId);
            return View(boQHeadLine);
        }

        // GET: BoQHeadLines/Edit/5
        [Authorize(Roles = "Admin,Manager,DivisionAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boQHeadLine = await _context.BoQHeadLines.FindAsync(id);
            if (boQHeadLine == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects.OrderBy(x => x.Name), "Id", "Name", boQHeadLine.ProjectId);
            return View(boQHeadLine);
        }

        // POST: BoQHeadLines/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,DivisionAdmin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,BoQnum,HeadLine,Type")] BoQHeadLine boQHeadLine)
        {
            if (id != boQHeadLine.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boQHeadLine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoQHeadLineExists(boQHeadLine.Id))
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
            ViewData["ProjectId"] = new SelectList(_context.Projects.OrderBy(x => x.Name), "Id", "Name", boQHeadLine.ProjectId);
            return View(boQHeadLine);
        }

        // GET: BoQHeadLines/Delete/5
        [Authorize(Roles = "Admin,Manager,DivisionAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boQHeadLine = await _context.BoQHeadLines
                .Include(b => b.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boQHeadLine == null)
            {
                return NotFound();
            }

            return View(boQHeadLine);
        }

        // POST: BoQHeadLines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var boQHeadLine = await _context.BoQHeadLines.FindAsync(id);
            _context.BoQHeadLines.Remove(boQHeadLine);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoQHeadLineExists(int id)
        {
            return _context.BoQHeadLines.Any(e => e.Id == id);
        }
        public JsonResult GetProjectHeadlines(string theId)
        {
            int Id = Convert.ToInt32(theId);
            List<BoQHeadLine> thedata = new List<BoQHeadLine>();
            thedata = _context.BoQHeadLines.Where(x => x.ProjectId.Equals(Id) && x.Type.Equals("ExtraWork")).OrderByDescending(x => x.BoQnum).ThenBy(x=>x.HeadLine).ToList();
            return Json(thedata);
        }
        
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,DivisionAdmin")]
        public async Task<IActionResult> NewExtraWorkHeadlineProject(int? ProjectId2 = null)
        {
            if(ProjectId2 == null)
            {
                return RedirectToAction(nameof(Create));
            }
            else
            {
                BoQHeadLine model = new BoQHeadLine();
                ViewData["ProjectId"] = await GetProjectList();
                var header = await _context.BoQHeadLines.Where(x => x.Type.Equals("ExtraWork") && x.ProjectId.Equals(ProjectId2)).LastOrDefaultAsync();
                if(header == null) {
                    model.Type = "ExtraWork";
                    model.ProjectId = Convert.ToInt32(ProjectId2);
                    model.HeadLine = "11.0001 XXXXX";
                    model.BoQnum = (decimal)11.0001;
                    return View("Create", model);
                }
                else
                {
                    string numberstring = "";
                    Regex re = new Regex(@"\d+");
                    foreach(var c in header.HeadLine)
                    {
                        Match match = re.Match(c.ToString());
                        if (match.Success)
                        {
                            numberstring += c.ToString();
                        }
                        else if (c.ToString().Equals("."))
                        {
                            numberstring += c.ToString();

                        }
                        else
                        {
                            break;
                        }
                    }
                    model.Type = "ExtraWork";
                    model.ProjectId = Convert.ToInt32(ProjectId2);
                    model.BoQnum = header.BoQnum;
                    try
                    {
                        decimal checker = Convert.ToDecimal(numberstring);
                    }
                    catch
                    {
                        return RedirectToAction("ErrorMessage", "Home", new { text = "Someone made a bad BoQHeadline name. please edit the previous headline for this project to contain a number" });
                    }
                    decimal newnumber = Convert.ToDecimal(numberstring);
                    newnumber += (decimal)0.0001;
                    model.HeadLine = newnumber.ToString("#.####") + " XXXXX";
                    return View("Create", model);
                }
            }
        }
    }
}
