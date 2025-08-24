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
using Microsoft.AspNetCore.Hosting;
using MainOps.ExtensionMethods;
using System.IO;
using System.Net.Http.Headers;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,DivisionAdmin")]
    public class DivisionsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public DivisionsController(DataContext context,UserManager<ApplicationUser> userManager, IWebHostEnvironment hostingenv):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
            _env = hostingenv;
        }

        // GET: Divisions
        [Authorize(Roles = "Admin,DivisionAdmin")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            await CheckUser(user);
            if (User.IsInRole("Admin")) { 
                return View(await _context.Divisions.Include(x => x.Users).ToListAsync());
            }
            else
            {
                return View(await _context.Divisions.Where(x => x.Id.Equals(user.DivisionId)).Include(x => x.Users).ToListAsync());
            }
        }

        // GET: Divisions/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            await CheckUser(user);
            if (User.IsInRole("Admin")) { 
                var division = await _context.Divisions
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (division == null)
                {
                    return NotFound();
                }

                return View(division);
            }
            else
            {
                var division = await _context.Divisions.Where(x => x.Id.Equals(user.DivisionId))
                    .SingleOrDefaultAsync(m => m.Id == id);
                if (division == null)
                {
                    return NotFound();
                }

                return View(division);
            }
        }

        // GET: Divisions/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {

            return View();
        }

        // POST: Divisions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        public async Task<bool> addStuffToDivision(string divisionname)
        {
            var div = await _context.Divisions.Where(x => x.Name.Equals(divisionname)).FirstAsync();
            var user = await _userManager.GetUserAsync(User);
            await CheckUser(user);
            if(div.Id.Equals(user.DivisionId) || User.IsInRole("Admin")) { 
                var allMonitorTypes = await _context.MonitorType.Where(x => x.DivisionId.Equals(1)).ToListAsync();
                foreach(MonitorType mt in allMonitorTypes)
                {
                    MonitorType mt_new = new MonitorType(mt,div);
                    _context.MonitorType.Add(mt_new);
                    await _context.SaveChangesAsync();
                }
                var allMeasTypes = await _context.MeasTypes.Where(x => x.DivisionId.Equals(1)).ToListAsync();
                foreach(MeasType mt in allMeasTypes)
                {
                    MeasType mt_new = new MeasType(mt, div);
                    _context.MeasTypes.Add(mt_new);
                    await _context.SaveChangesAsync();
                }
            }
            return true;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Currency,CurrencyDecimalSeperator,CurrencyGroupSeperator,HourSheetEmail")] Division division)
        {
            if (ModelState.IsValid)
            {
                _context.Add(division);
                await _context.SaveChangesAsync();
                var success = await addStuffToDivision(division.Name);
                return RedirectToAction(nameof(Index));
                
            }
            return View(division);
        }

        // GET: Divisions/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            await CheckUser(user);
            var division = await _context.Divisions.FindAsync(id);
            if (division == null)
            {
                return NotFound();
            }
            return View(division);
        }

        // POST: Divisions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Currency,CurrencyDecimalSeperator,CurrencyGroupSeperator,HourSheetEmail,LogoPath")] Division division)
        {
            if (id != division.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(division);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DivisionExists(division.Id))
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
            return View(division);
        }
        [HttpPost]
        [Authorize(Roles = "DivisionAdmin,Admin")]
        public async Task<IActionResult> UploadPhotoDivision(int? id)
        {
            if (id != null)
            {
                var user = await _userManager.GetUserAsync(User);
                await CheckUser(user);
                var division = await _context.Divisions.FindAsync(id);
                //var user = await _userManager.GetUserAsync(User);
                var folderpath = Path.Combine(_env.WebRootPath.ReplaceFirst("/", ""), "Images", "Divisions", "Photos", id.ToString());
                if (!Directory.Exists(folderpath))
                {
                    Directory.CreateDirectory(folderpath);
                }
                if (HttpContext.Request.Form.Files != null)
                {
                    var files = HttpContext.Request.Form.Files;

                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            //Getting FileName
                            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            //fileName = user.FirstName + user.LastName + fileName;
                            // Combines two strings into a path.
                            division.LogoPath = fileName;
                            fileName = folderpath + $@"\{fileName}";
                            using (FileStream fs = System.IO.File.Create(fileName))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }
                            _context.Update(division);
                            await _context.SaveChangesAsync();

                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }
        // GET: Divisions/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            await CheckUser(user);
            var division = await _context.Divisions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (division == null)
            {
                return NotFound();
            }

            return View(division);
        }

        // POST: Divisions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var division = await _context.Divisions.FindAsync(id);
            _context.Divisions.Remove(division);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DivisionExists(int id)
        {
            return _context.Divisions.Any(e => e.Id == id);
        }
    }
}
