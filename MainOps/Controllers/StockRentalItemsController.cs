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
using Microsoft.AspNetCore.Http;
using System.IO;
using MainOps.ExtensionMethods;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,DivisionAdmin,Manager,StorageManager")]
    public class StockRentalItemsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public StockRentalItemsController(DataContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // GET: StockRentalItems
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var dataContext = _context.StockRentalItems.Include(s => s.HJItem).Include(s => s.Rental_Unit)
                .Where(x => x.HJItem.DivisionId.Equals(user.DivisionId) || x.HJItemId == null).OrderByDescending(x => x.StartTime).ThenBy(x => x.HJItem.HJId);
            ViewData["FilterChoices1"] = await GetHJItemMasterClasses();
            ViewData["FilterChoices2"] = await GetHJItemClasses();
            return View(await dataContext.ToListAsync());
        }
        public async Task<IActionResult> SearchItems(string searchtext, string HJItemMasterClassId, string HJItemClassId)
        {
            int MasterClassId;
            int ClassId;
            MasterClassId = Convert.ToInt32(HJItemMasterClassId);
            ClassId = Convert.ToInt32(HJItemClassId);
            List<StockRentalItem> dataContext;
            if (searchtext == null ||searchtext == "")
            {
                dataContext = await _context.StockRentalItems.Include(x => x.HJItem).ThenInclude(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass).Include(x => x.Rental_Unit).OrderByDescending(x => x.StartTime).ThenBy(x => x.HJItem.HJId).ToListAsync();
                if( !string.IsNullOrEmpty(HJItemClassId))
                {
                    dataContext = dataContext.Where(x => x.HJItem.HJItemClassId.Equals(ClassId)).OrderByDescending(x => x.StartTime).ThenBy(x => x.HJItem.HJId).ToList();
                }
                else if(!string.IsNullOrEmpty(HJItemMasterClassId))
                {
                    dataContext = dataContext.Where(x => x.HJItem.HJItemClass.HJItemMasterClassId.Equals(MasterClassId)).OrderByDescending(x => x.StartTime).ThenBy(x => x.HJItem.HJId).ToList();
                }
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);

                dataContext = await _context.StockRentalItems.Include(s => s.HJItem).ThenInclude(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass).Include(s => s.Rental_Unit)
                    .Where(x => (x.HJItem.DivisionId.Equals(user.DivisionId) || x.HJItemId == null) && (x.Company.ToLower().Contains(searchtext.ToLower()) || x.HJItem.HJId.ToLower().Contains(searchtext.ToLower()) || x.HJItem.HJItemClass.ClassName.ToLower().Contains(searchtext.ToLower())
                    || x.HJItem.HJItemClass.HJItemMasterClass.ClassName.ToLower().Contains(searchtext.ToLower()) || x.ContactPerson.ToLower().Contains(searchtext.ToLower()) || x.ItemNumber.ToLower().Contains(searchtext.ToLower()))).OrderByDescending(x => x.StartTime).ThenBy(x => x.HJItem.HJId).ToListAsync();
                if (!string.IsNullOrEmpty(HJItemClassId))
                {
                    dataContext = dataContext.Where(x => x.HJItem.HJItemClassId.Equals(ClassId)).OrderByDescending(x => x.StartTime).ThenBy(x => x.HJItem.HJId).ToList();
                }
                else if (!string.IsNullOrEmpty(HJItemMasterClassId))
                {
                    dataContext = dataContext.Where(x => x.HJItem.HJItemClass.HJItemMasterClassId.Equals(MasterClassId)).OrderByDescending(x => x.StartTime).ThenBy(x => x.HJItem.HJId).ToList();
                }
            }
            ViewData["FilterChoices1"] = await GetHJItemMasterClasses();
            ViewData["FilterChoices2"] = await GetHJItemClasses();
            return View("Index", dataContext);
        }
        // GET: StockRentalItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockRentalItem = await _context.StockRentalItems
                .Include(s => s.HJItem)
                .Include(s => s.Rental_Unit)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stockRentalItem == null)
            {
                return NotFound();
            }

            return View(stockRentalItem);
        }

        // GET: StockRentalItems/Create
        public async Task<IActionResult> Create()
        {
            ViewData["HJItemClassId"] = await GetHJItems();
            ViewData["HJItemMasterClassId"] = await GetHJItemMasterClasses();
            ViewData["Rental_UnitId"] = new SelectList(_context.Units, "Id", "TheUnit");
            return View();
        }

        // POST: StockRentalItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,HJItemId,ItemNumber,Company,ContactPerson,Email,PhoneNr,StartTime,EndTime,TimeSetup,RentalFee,Rental_UnitId,IsReturned,LastInvoice")] StockRentalItem stockRentalItem, IFormFile[] photos_before,IFormFile[] photos_after)
        {
            if (ModelState.IsValid)
            {
                stockRentalItem.TimeSetup = DateTime.Now;
                _context.Add(stockRentalItem);
                await _context.SaveChangesAsync();
                var lastadded = await _context.StockRentalItems.Include(x => x.HJItem).OrderBy(x => x.Id).LastAsync();
                var directory1 = _env.WebRootPath + "\\Rental\\PhotoDelivery\\" + lastadded.Id.ToString() + "\\";
                var directory2 = _env.WebRootPath + "\\Rental\\PhotoReturn\\" + lastadded.Id.ToString() + "\\";
                if (!Directory.Exists(directory1))
                {
                    Directory.CreateDirectory(directory1);
                }
                if (!Directory.Exists(directory2))
                {
                    Directory.CreateDirectory(directory2);
                }
                foreach (IFormFile photo in photos_before)
                {
                    var path = Path.Combine(directory1, photo.FileName);
                    var path2 = Path.Combine(directory1, photo.FileName.Split(".")[0] + "_edit." + photo.FileName.Split(".")[1]);
                    StockRentalItemPhotoDelivery photostart = new StockRentalItemPhotoDelivery{ path = path, StockRentalItemId = lastadded.Id };
                    _context.Add(photostart);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    };
                    if (path.ToLower().Contains(".jpg") || path.ToLower().Contains(".jpeg"))
                    {
                        PhotoExtensions.SaveAndCompressJpeg(path, 80);
                    }

                }
                foreach (IFormFile photo in photos_after)
                {
                    var path = Path.Combine(directory2, photo.FileName);
                    var path2 = Path.Combine(directory2, photo.FileName.Split(".")[0] + "_edit." + photo.FileName.Split(".")[1]);
                    StockRentalItemPhotoReturn photoend = new StockRentalItemPhotoReturn { path = path, StockRentalItemId = lastadded.Id };
                    _context.Add(photoend);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    };
                    if (path.ToLower().Contains(".jpg") || path.ToLower().Contains(".jpeg"))
                    {
                        PhotoExtensions.SaveAndCompressJpeg(path, 80);
                    }

                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["HJItemId"] = await GetHJItems();
            ViewData["Rental_UnitId"] = new SelectList(_context.Units, "Id", "TheUnit", stockRentalItem.Rental_UnitId);
            return View(stockRentalItem);
        }

        // GET: StockRentalItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockRentalItem = await _context.StockRentalItems.Include(x => x.Photos_Delivery).Include(x => x.Photos_Return).SingleOrDefaultAsync(x => x.Id.Equals(id));
            if (stockRentalItem == null)
            {
                return NotFound();
            }
            ViewData["HJItemId"] = await GetHJItems();
            ViewData["Rental_UnitId"] = new SelectList(_context.Units, "Id", "TheUnit", stockRentalItem.Rental_UnitId);
            return View(stockRentalItem);
        }

        // POST: StockRentalItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HJItemId,ItemNumber,Company,ContactPerson,Email,PhoneNr,StartTime,EndTime,TimeSetup,RentalFee,Rental_UnitId,IsReturned,LastInvoice")] StockRentalItem stockRentalItem,IFormFile[] photos_before,IFormFile[] photos_after)
        {
            if (id != stockRentalItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stockRentalItem);
                    await _context.SaveChangesAsync();
                    var directory1 = _env.WebRootPath + "\\Rental\\PhotoDelivery\\" + stockRentalItem.Id.ToString() + "\\";
                    var directory2 = _env.WebRootPath + "\\Rental\\PhotoReturn\\" + stockRentalItem.Id.ToString() + "\\";
                    if (!Directory.Exists(directory1))
                    {
                        Directory.CreateDirectory(directory1);
                    }
                    if (!Directory.Exists(directory2))
                    {
                        Directory.CreateDirectory(directory2);
                    }
                    foreach (IFormFile photo in photos_before)
                    {
                        var path = Path.Combine(directory1, photo.FileName);
                        var path2 = Path.Combine(directory1, photo.FileName.Split(".")[0] + "_edit." + photo.FileName.Split(".")[1]);
                        StockRentalItemPhotoDelivery photostart = new StockRentalItemPhotoDelivery { path = path, StockRentalItemId = stockRentalItem.Id };
                        _context.Add(photostart);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await photo.CopyToAsync(stream);
                        };
                        if (path.ToLower().Contains(".jpg") || path.ToLower().Contains(".jpeg"))
                        {
                            PhotoExtensions.SaveAndCompressJpeg(path, 80);
                        }

                    }
                    foreach (IFormFile photo in photos_after)
                    {
                        var path = Path.Combine(directory2, photo.FileName);
                        var path2 = Path.Combine(directory2, photo.FileName.Split(".")[0] + "_edit." + photo.FileName.Split(".")[1]);
                        StockRentalItemPhotoReturn photoend = new StockRentalItemPhotoReturn { path = path, StockRentalItemId = stockRentalItem.Id };
                        _context.Add(photoend);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await photo.CopyToAsync(stream);
                        };
                        if (path.ToLower().Contains(".jpg") || path.ToLower().Contains(".jpeg"))
                        {
                            PhotoExtensions.SaveAndCompressJpeg(path, 80);
                        }

                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockRentalItemExists(stockRentalItem.Id))
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
            ViewData["HJItemId"] = await GetHJItems();
            ViewData["Rental_UnitId"] = new SelectList(_context.Units, "Id", "TheUnit", stockRentalItem.Rental_UnitId);
            return View(stockRentalItem);
        }

        // GET: StockRentalItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockRentalItem = await _context.StockRentalItems
                .Include(s => s.HJItem)
                .Include(s => s.Rental_Unit)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stockRentalItem == null)
            {
                return NotFound();
            }

            return View(stockRentalItem);
        }

        // POST: StockRentalItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stockRentalItem = await _context.StockRentalItems.FindAsync(id);
            _context.StockRentalItems.Remove(stockRentalItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StockRentalItemExists(int id)
        {
            return _context.StockRentalItems.Any(e => e.Id == id);
        }
        [HttpGet]
        public async Task<JsonResult> GetHJItemClassesJson(string MasterClass = null)
        {
            var user = await _userManager.GetUserAsync(User);
            int? MasterClassId = Convert.ToInt32(MasterClass);
            if (User.IsInRole("Admin"))
            {


                if (MasterClassId != null)
                {
                    var data = _context.HJItemClasses.Include(x => x.HJItemMasterClass).Where(x => x.HJItemMasterClassId.Equals(MasterClassId)).OrderBy(x => x.HJItemMasterClass.DivisionId).ThenBy(x => x.ClassNumber).ToList();

                    return Json(data);
                }
                else
                {
                    var data = _context.HJItemClasses.Include(x => x.HJItemMasterClass).OrderBy(x => x.HJItemMasterClass.DivisionId).ThenBy(x => x.ClassNumber).ToList();
                    return Json(data);
                }

            }
            else
            {
                if (MasterClassId != null)
                {
                    var data = _context.HJItemClasses.Include(x => x.HJItemMasterClass).Where(x => x.HJItemMasterClassId.Equals(MasterClassId) && x.HJItemMasterClass.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.HJItemMasterClass.DivisionId).ThenBy(x => x.ClassNumber).ToList();

                    return Json(data);
                }
                else
                {
                    var data = _context.HJItemClasses.Include(x => x.HJItemMasterClass).Where(x => x.HJItemMasterClass.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.HJItemMasterClass.DivisionId).ThenBy(x => x.ClassNumber).ToList();

                    return Json(data);
                }
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetHJItemsJson(string Class = null)
        {
            var user = await _userManager.GetUserAsync(User);
            int? ClassId = Convert.ToInt32(Class);
            if (User.IsInRole("Admin"))
            {


                if (ClassId != null)
                {
                    var data = _context.HJItems.Include(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass).Where(x => x.HJItemClassId.Equals(ClassId)).OrderBy(x => x.HJItemClass.HJItemMasterClass.DivisionId).ThenBy(x => x.HJId).ToList();
                    var meh = (from dat in data select new { Id = dat.Id, name = dat.Name, theid = dat.HJId }).ToList();
                    return Json(meh);
                }
                else
                {
                    var data = _context.HJItems.Include(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass).OrderBy(x => x.HJItemClass.HJItemMasterClass.DivisionId).ThenBy(x => x.HJId).ToList();
                    var meh = (from dat in data select new { Id = dat.Id, name = dat.Name, theid = dat.HJId }).ToList();
                    return Json(meh);
                }

            }
            else
            {
                if (ClassId != null)
                {
                    var data = _context.HJItems.Include(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass).Where(x => x.HJItemClass.HJItemMasterClass.DivisionId.Equals(user.DivisionId) && x.HJItemClassId.Equals(ClassId)).OrderBy(x => x.HJItemClass.HJItemMasterClass.DivisionId).ThenBy(x => x.HJId).ToList();
                    var meh = (from dat in data select new { Id = dat.Id, name = dat.Name, theid = dat.HJId }).ToList();
                    return Json(meh);
                }
                else
                {
                    var data = _context.HJItems.Include(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass).Where(x => x.HJItemClass.HJItemMasterClass.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.HJItemClass.HJItemMasterClass.DivisionId).ThenBy(x => x.HJId).ToList();
                    var meh = (from dat in data select new { Id = dat.Id, name = dat.Name, theid = dat.HJId }).ToList();
                    return Json(meh);
                }
            }
        }
    }
}
