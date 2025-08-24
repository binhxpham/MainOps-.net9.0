using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainOps.Data;
using MainOps.Models.WTPClasses.MainClasses;
using MainOps.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace App2.Controllers.MainControllers
{
    [Authorize(Roles = ("Admin"))]
    public class WtpBlockController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly LocService _localizer;
        /// <summary>
        /// Constructor for the WtpBlockController
        /// </summary>
        /// <param name="context"></param>
        /// <param name="hostingEnvironment"></param>
        /// <param name="localizer"></param>
        public WtpBlockController(DataContext context, IWebHostEnvironment hostingEnvironment, LocService localizer)
        {
            _context = context;
            _environment = hostingEnvironment;
            _localizer = localizer;
        }
        /// <summary>
        /// Redirects to the FileUpload/Index page
        /// </summary>
        /// <returns>FileUpload/Index</returns>
        public IActionResult UploadPicture()
        {
            return RedirectToAction("Index", "FileUpload");
        }
        /// <summary>
        /// Show the WtpBlock/Index Page with all WtpBlocks in the datacontext
        /// </summary>
        /// <returns>WtpBlock/Index Page with all WtpBlocks</returns>
        public async Task<IActionResult> Index(string returnUrl = null)
        {
            //var selList = await CreateRentalCategoryList();
            //ViewData["rentalcategories"] = new SelectList(selList, "Value", "Text");
            //var selList2 = await CreateFilterlist();
            //ViewData["Filterchoices"] = new SelectList(selList2, "Value", "Text");
            ViewData["returnUrl"] = returnUrl;
            var wtpBlocks = await _context.WTP_blocks
                .Include(b=>b.unit_size)
                .OrderBy(b => b.name).ThenBy(b => b.size)
                .ToListAsync();
            return View(wtpBlocks);
        }
        /// <summary>
        /// Creates the List of all Categories
        /// </summary>
        /// <returns>Gives back the List of all Categories</returns>
        //public async Task<IEnumerable<SelectListItem>> CreateFilterlist()
        //{
        //    var filternames = await _context.Item_categories
        //        .OrderBy(b => b.item_category).ToListAsync();
        //    var selList = from s in filternames
        //                                          select new SelectListItem
        //                                          {
        //                                              Value = s.Id.ToString(),
        //                                              Text = s.item_category
        //                                          };
        //    return selList;
        //}

        public async Task<IEnumerable<SelectListItem>> CreateUnitlist()
        {
            var filternames = await _context.WTPUnits
                .OrderBy(b => b.the_unit).ToListAsync();
            foreach (var u in filternames)
            {
                u.the_unit = u.the_unit.Replace("&sup2;", "2").Replace("&sup3;", "3");
            }
            var selList = from s in filternames
                                                  select new SelectListItem
                                                  {
                                                      Value = s.id.ToString(),
                                                      Text = _localizer.GetLocalizedHtmlString(s.the_unit)
                                                  };
            return selList;
        }

        [HttpPost]
        public async Task<IActionResult> Combsearch(string searchstring)
        {
           
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(searchstring))
                {
                    return View(nameof(Index), await _context.WTP_blocks.OrderBy(p => p.name)
                        .Include(x => x.unit_size)
                        .ToListAsync());              
                }
                
                else if (!string.IsNullOrEmpty(searchstring))
                {
                    var wtpBlocks = await _context.WTP_blocks
                        .Include(x => x.unit_size)
                        .Where(b => b.name.ToLower().Contains(searchstring.ToLower())).OrderBy(b => b.name).ThenBy(b => b.size).ToListAsync();
                    return View(nameof(Index), wtpBlocks);
                }
                else
                {
                    var wtpBlocks = await _context.WTP_blocks
                        .Include(x => x.unit_size)
                       .ToListAsync();
                    return View(nameof(Index), wtpBlocks);
                }
            }
            return View(nameof(Index));
        }
       
        [HttpGet]
        public JsonResult AutoComplete(string search)
        {
            var wtpblocks =  _context.WTP_blocks.Where(b => b.name.ToUpper().Contains(search.ToUpper())).ToList();
            return Json(wtpblocks.Select(m => new
            {
              m.id,
              value = m.name,
              label = m.name + '_' + m.size.ToString()
            }).OrderBy(b => b.label));
        }
       
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var wtpBlock = await _context.WTP_blocks
                .Include(b=>b.unit_size)
                .Include(b=>b.Division)
                .SingleOrDefaultAsync(m => m.id == id);
            if (wtpBlock == null)
            {
                return NotFound();
            }
            return View(wtpBlock);
        }
       
        public async Task<IActionResult> Create()
        {
            var divisions = await _context.Divisions
                .OrderBy(x=>x.Name).ToListAsync();            
            ViewData["units"] = new SelectList(await CreateUnitlist(), "Value", "Text");
            ViewData["divisions"] = new SelectList(divisions, "Id", "Division_name");            
            return View();
        }
        /// <summary>
        /// Checks if the Userinput is Valid and then Creates a new WtpBlock for the Userinput.
        /// </summary>
        /// <param name="wtpBlock">WtpBlock with binded Attributes</param>
        /// <returns>WtpBlock/Index Page or the WtpBlock/Create Page again</returns>
        // POST: WtpBlock/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name,size,unit_sizeid,weight,length,width,height,necessity,divisionId,quantity,Description,UnitId,Pow_Con,ger_name,GerDescription")] WTP_block wtpBlock,string germanNameVersion,string germanDescVersion)
        {
            if (ModelState.IsValid)
            {
                var sizeunit = await _context.WTPUnits.SingleOrDefaultAsync(x => x.id.Equals(wtpBlock.unit_sizeid));
                wtpBlock.unit_size = sizeunit;
                wtpBlock.unit_size.the_unit = MainOps.ExtensionMethods.UrlHelperExtensions.ToHTML(wtpBlock.unit_size.the_unit);
                _context.Add(wtpBlock);
                var resFileName = @"" + _environment.ContentRootPath + "\\Resources\\SharedResource.de-DE.resx";
                
                await _context.SaveChangesAsync();
                // check if price exists on device
                var potentialPrice = await _context.Prices.SingleOrDefaultAsync(p => (p.name.ToLower().Trim() == wtpBlock.name.ToLower().Trim()));
                if (potentialPrice == null)
                {
                    var newPrice = new Price
                    {
                        name = wtpBlock.name,
                        EkdTid = 2,
                        size = wtpBlock.size,
                        unitid = Convert.ToInt32(wtpBlock.unit_sizeid),
                        price = 0.0,
                        unit_pid = 17,
                        rent = 1.0,
                        unit_rid = 24,
                        divisionid = wtpBlock.DivisionId
                    };
                    _context.Prices.Add(newPrice);
                    await _context.SaveChangesAsync();
                    var addedPrice = await _context.Prices.SingleOrDefaultAsync(p => (p.name.ToLower().Trim() == wtpBlock.name.ToLower().Trim()) && (p.size == wtpBlock.size));
                    return RedirectToAction("EditFirst", "Prices", new { addedPrice.id });
                }
                return RedirectToAction(nameof(Index));
            }
            return View(wtpBlock);
        }

        
        
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var divisions = await _context.Divisions
                .OrderBy(x => x.Name).ToListAsync();
          
            ViewData["units"] = new SelectList(await CreateUnitlist(), "Value", "Text");
            ViewData["divisions"] = new SelectList(divisions, "Id", "Division_name");
            var wtpBlock = await _context.WTP_blocks.SingleOrDefaultAsync(m => m.id == id);
            if (wtpBlock == null)
            {
                return NotFound();
            }
            return View(wtpBlock);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,size,unit_sizeid,weight,length,width,height,necessity,divisionId,quantity,Description,UnitId,Pow_Con,ger_name,GerDescription,packable")] WTP_block wtpBlock)
        {
            string wtpblockname;
            double wtpblocksize;
            if (id != wtpBlock.id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var sizeunit = await _context.WTPUnits.SingleOrDefaultAsync(x => x.id.Equals(wtpBlock.unit_sizeid));
                wtpBlock.unit_size = sizeunit;
                wtpBlock.unit_size.the_unit = MainOps.ExtensionMethods.UrlHelperExtensions.ToHTML(wtpBlock.unit_size.the_unit);
                try
                {
                    var wtpBlockOld = await _context.WTP_blocks.SingleOrDefaultAsync(b => b.id == id);
                    if(wtpBlockOld.name == null)
                    {
                        _context.Entry(wtpBlockOld).State = EntityState.Detached;
                        _context.Update(wtpBlock);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    wtpblockname = wtpBlockOld.name;
                    wtpblocksize = wtpBlockOld.size;
                    _context.Entry(wtpBlockOld).State = EntityState.Detached;
                    var efforts = await _context.Efforts.Where(e => e.nameparts(e.WTP_block_name)[0].ToLower() == wtpblockname.ToLower() &&
                    e.nameparts(e.WTP_block_name)[1] == wtpblocksize.ToString()).ToListAsync();
                    var uniqueEffort = await _context.Efforts.Where(e => e.nameparts(e.WTP_block_name)[0].ToLower() == wtpblockname.ToLower() 
                    && e.nameparts(e.WTP_block_name)[1] == wtpblocksize.ToString()
                    && e.Name == wtpblockname).ToListAsync();
                    var filtermats = await _context.FilterMaterials.Where(f => f.device == wtpblockname).ToListAsync();
                    var prices = await _context.Prices.Where(p => p.name.ToLower() == wtpblockname.ToLower()).ToListAsync();
                    //await _context.SaveChangesAsync();
                    foreach (var e in efforts)
                    {
                        e.WTP_block_name = String.Concat(wtpBlock.name, "_", wtpBlock.size.ToString());
                        _context.Update(e);
                    }
                    /*foreach (Effort e in unique_effort){
                         e.Name = wtpBlock.name;
                        _context.Update(e);
                    }*/
                    foreach (var f in filtermats)
                    {
                        f.device = wtpBlock.name;
                        _context.Update(f);
                    }
                    foreach (var p in prices)
                    {
                        p.name = wtpBlock.name;
                        _context.Update(p);
                    }
                    _context.Update(wtpBlock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WtpBlockExists(wtpBlock.id))
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
            return View(wtpBlock);
        }
       
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var wtpBlock = await _context.WTP_blocks
                .Include(b=>b.unit_size)
                .SingleOrDefaultAsync(m => m.id == id);
            if (wtpBlock == null)
            {
                return NotFound();
            }
            return View(wtpBlock);
        }
       
        public async Task<IActionResult> GetEfforts(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var wtpBlock = await _context.WTP_blocks.FindAsync(id);
            
            if(wtpBlock == null)
            {
                return NotFound();
            }
            //return RedirectToAction("Index", "RentalShop");
            return RedirectToAction("Effortblock", "DeviceEffort", new { id });
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wtpBlock = await _context.WTP_blocks.SingleOrDefaultAsync(m => m.id == id);
            _context.WTP_blocks.Remove(wtpBlock);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        /// <summary>
        /// Checks if the WtpBlock with the given Id is existing
        /// </summary>
        /// <param name="id">Id of the WtpBlock to be checked</param>
        /// <returns>True if the WtpBlock is existing and false if not</returns>
        private bool WtpBlockExists(int id)
        {
            return _context.WTP_blocks.Any(e => e.id == id);
        }
        
    }
}
