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
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using MainOps.ExtensionMethods;
using Rotativa.AspNetCore;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember")]
    public class PumpTestingsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public PumpTestingsController(DataContext context,UserManager<ApplicationUser> userManager, IWebHostEnvironment env) :base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }
        
        // GET: PumpTestings
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin")) {
                ViewData["filterchoices"] = new SelectList(_context.PumpTestings.GroupBy(x=>x.PumpID).Select(y=>y.First()), "Id", "PumpID");
                return View(await _context.PumpTestings.ToListAsync());
            }
            else
            {
                var tests = await _context.PumpTestings.Where(x=>x.DivisionId.Equals(user.DivisionId)).ToListAsync();
                
                ViewData["filterchoices"] = new SelectList(tests.GroupBy(x=>x.PumpID).Select(y=> y.First()), "Id", "PumpID");
                return View(tests);
            }
        }
        public async Task<IActionResult> Combsearch(int? filterchoice,string searchstring)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                ViewData["filterchoices"] = new SelectList(_context.PumpTestings.GroupBy(x => x.PumpID).Select(y => y.First()), "Id", "PumpID");
                if (filterchoice != null)
                {
                    var pumptest = await _context.PumpTestings.FindAsync(Convert.ToInt32(filterchoice));
                    var pumptestings = await _context.PumpTestings.Where(x => x.PumpID.Equals(pumptest.PumpID)).ToListAsync();
                    return View("Index", pumptestings);
                }
                else if(searchstring != "")
                {
                    var pumptestings = await _context.PumpTestings.Where(x => 
                    x.PumpID.ToLower().Contains(searchstring.ToLower()) || 
                    x.DoneBy.ToLower().Contains(searchstring.ToLower()) ||
                    x.PumpType.ToLower().Contains(searchstring.ToLower())).ToListAsync();
                    return View("Index", pumptestings);
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                var tests = await _context.PumpTestings.Where(x=>x.DivisionId.Equals(user.DivisionId)).ToListAsync();
                ViewData["filterchoices"] = new SelectList(tests.GroupBy(x => x.PumpID).Select(y => y.First()), "Id", "PumpID");
                if(filterchoice != null)
                {
                    var pumptest = await _context.PumpTestings.FindAsync(Convert.ToInt32(filterchoice));
                    var pumptestings = tests.Where(x => x.PumpID.Equals(pumptest.PumpID));
                    return View("Index", pumptestings);
                }
                else if(searchstring != "")
                {
                    var pumptestings = tests.Where(x =>
                    x.PumpID.ToLower().Contains(searchstring.ToLower()) ||
                    x.DoneBy.ToLower().Contains(searchstring.ToLower()) ||
                    x.PumpType.ToLower().Contains(searchstring.ToLower())).ToList();
                    return View("Index", pumptestings);
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }

        }
        [HttpGet]
        public async Task<JsonResult> AutoComplete(string search)
        {
            var user = await _userManager.GetUserAsync(User);
            List<PumpTesting> results = new List<PumpTesting>();
            if (User.IsInRole("Admin"))
            {
                results = await _context.PumpTestings.Where(x =>
                    x.PumpID.ToLower().Contains(search.ToLower()) ||
                    x.DoneBy.ToLower().Contains(search.ToLower()) ||
                    x.PumpType.ToLower().Contains(search.ToLower())).ToListAsync();
            }
            else
            {
                var tests = await _context.PumpTestings.Where(x=>x.DivisionId.Equals(user.DivisionId)).ToListAsync();
                results = tests.Where(x =>
                    x.PumpID.ToLower().Contains(search.ToLower()) ||
                    x.DoneBy.ToLower().Contains(search.ToLower()) ||
                    x.PumpType.ToLower().Contains(search.ToLower())).ToList();
            }

            return Json(results.Select(m => new
            {
                id = m.Id,
                value = m.PumpID,
                label = m.PumpID + " : " + m.PumpType
            }).OrderBy(x => x.label));
        }
        // GET: PumpTestings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var pumpTesting = await _context.PumpTestings.Include(x=>x.TestData)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pumpTesting == null)
            {
                return NotFound();
            }
            else if(!pumpTesting.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
            }

            return new ViewAsPdf("Details",pumpTesting);
        }

        // GET: PumpTestings/Create
        [HttpGet]
        public IActionResult Create()
        {
            PumptestingVM model = new PumptestingVM();
            return View(model);
        }

        // POST: PumpTestings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(900000000)]
        public async Task<IActionResult> Create(PumptestingVM model, IFormFile[] files)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                model.test.DoneBy = user.full_name();
                model.test.DivisionId = user.DivisionId;
                _context.Add(model.test);
                await _context.SaveChangesAsync();
                var lastadded = await _context.PumpTestings.LastAsync();
                foreach(PumptestingData d in model.data.Where(x=>x.Pressure != null))
                {
                    d.PumpTestingId = lastadded.Id;
                    _context.Add(d);
                }
                if(files != null)
                {
                    var directory = _env.WebRootPath + "\\AHAK\\Pumptesting\\" + lastadded.Id.ToString() + "\\";
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    foreach (var file in files)
                    {
                        var path = Path.Combine(directory, file.FileName);
                        PumptestingPhoto photo = new PumptestingPhoto { path = path, PumptestingId = lastadded.Id };
                        _context.Add(photo);
                        var stream = new FileStream(path, FileMode.Create);
                        await file.CopyToAsync(stream);
                        stream.Close();
                        if (path.Contains(".jpg") || path.Contains(".jpeg"))
                        {
                            PhotoExtensions.SaveAndCompressJpeg(path, 95);
                        }
                    }                    
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: PumpTestings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var pumpTesting = await _context.PumpTestings.Include(x => x.TestData).Where(x => x.Id.Equals(id)).SingleOrDefaultAsync();
            if(pumpTesting == null)
            {
                return NotFound();
            }
            else if(!pumpTesting.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
            }
            PumptestingVM model = new PumptestingVM(pumpTesting,pumpTesting.TestData.ToList());
            if (pumpTesting == null)
            {
                return NotFound();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(900000000)]
        public async Task<IActionResult> Edit(PumptestingVM model,IFormFile[] files)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model.test);
                    foreach(PumptestingData d in model.data)
                    {
                        d.PumpTestingId = model.test.Id;
                        _context.Update(d);
                    }
                    if (files != null)
                    {
                        var directory = _env.WebRootPath + "\\AHAK\\Pumptesting\\" + model.test.Id.ToString() + "\\";
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }
                        foreach (var file in files)
                        {
                            var path = Path.Combine(directory, file.FileName);
                            PumptestingPhoto photo = new PumptestingPhoto { path = path, PumptestingId = model.test.Id };
                            _context.Add(photo);
                            var stream = new FileStream(path, FileMode.Create);
                            await file.CopyToAsync(stream);
                            stream.Close();
                            if (path.Contains(".jpg") || path.Contains(".jpeg"))
                            {
                                PhotoExtensions.SaveAndCompressJpeg(path, 95);
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PumpTestingExists(model.test.Id))
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
            return View(model);
        }

        // GET: PumpTestings/Delete/5
        [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var pumpTesting = await _context.PumpTestings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pumpTesting == null)
            {
                return NotFound();
            }
            else if (!pumpTesting.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
            }

            return View(pumpTesting);
        }

        // POST: PumpTestings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pumpTesting = await _context.PumpTestings.FindAsync(id);
            var testdata = await _context.PumptestingDatas.Where(x => x.PumpTestingId.Equals(id)).ToListAsync();
            foreach(PumptestingData d in testdata)
            {
                _context.Remove(d);
            }
            await _context.SaveChangesAsync();
            _context.PumpTestings.Remove(pumpTesting);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PumpTestingExists(int id)
        {
            return _context.PumpTestings.Any(e => e.Id == id);
        }
    }
}
