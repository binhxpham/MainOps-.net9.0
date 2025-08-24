using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MainOps.Models;
using MainOps.Data;
using Microsoft.EntityFrameworkCore;
using MainOps.Controllers;
using Microsoft.Extensions.Configuration;
using MainOps.ExtensionMethods;
using MainOps.Models.WTPClasses.HelperClasses;
using MainOps.Models.WTPClasses.MainClasses;
using MainOps.Models.WTPClasses.ViewModels;
using LpSolveDotNet;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin")]
    public class WtpBuilderController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WtpBuilderController(DataContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }
        private void CreateRdlc(WTPBuilder wtp)
        {
            var query = _context.Prices.Where(x => x.name.ToLower().Contains("worker")).ToListAsync();
        }

       
        public string PipeType(int daysOperation)
        {
            string thetype;
            if (daysOperation > 180)
            {
                thetype = "electro";
            }
            else
            {
                thetype = "fast coupling";
            }
            return thetype;
        }
        public async Task<Price> findFiltermaterial(WTPBuilder wtp, Effort e)
        {
            var returnPrice = new Price
            { name = "NO PRICE",
                EkdT = new Category { category = "NOT DEFINED" },
                size = 100000,
                price = 100000,
                unit_p = new WTPUnit { the_unit = "NOT DEFINED" },
                rent = 100000,
                unit_r = new WTPUnit { the_unit = "NOT DEFINED" },
                division = await _context.Divisions.SingleOrDefaultAsync(x=>x.Id == 7)
            };
            var filtermaterials =  wtp.filtermaterials
                .Where(f => e.WTP_block_name.Contains(f.device)).ToList();
            if (filtermaterials.Count < 2)
            {
                returnPrice = await _context.Prices.SingleOrDefaultAsync(p => p.name.ToLower().Contains(filtermaterials[0].Name.ToLower()) && p.unit.the_unit.ToLower() == "bags");
            }
            else
            {
                var vesselCount = wtp.WTPblocks.Where(b => b.name == "Vessel").ToList().Count;
                var prFilter = vesselCount / filtermaterials.Count;
                foreach (var f in filtermaterials)
                {
                    var price = await _context.Prices.Where(p => p.name.ToLower().Contains(f.Name.ToLower()) && p.unit.the_unit.ToLower() == "bags").FirstAsync();
                    if (price == null)
                    {
                        returnPrice.name = e.Name;
                        returnPrice.price = vesselCount;
                        returnPrice.rent = filtermaterials.Count;
                        return returnPrice;
                    }
                    if (wtp.priceList.prices_mob.ContainsKey(price.name) && e.Temp_section.section == "Mobilisation")
                    {
                        if (!(wtp.priceList.prices_mob[price.name] == Convert.ToDecimal(price.price) * Convert.ToDecimal(prFilter) * Convert.ToDecimal(e.effort)))
                        {
                            returnPrice = price;
                        }
                    }
                    else if (wtp.priceList.prices_demob.ContainsKey(price.name) && e.Temp_section.section == "Demobilisation")
                    {
                        if (!(wtp.priceList.prices_mob[price.name] == Convert.ToDecimal(price.price) * Convert.ToDecimal(prFilter) * Convert.ToDecimal(e.effort)))
                        {
                            returnPrice = price;
                        }
                    }
                    else
                    {
                        returnPrice = price;
                        break;
                    }

                }
            }
            return returnPrice;
        }
        public double pipeSize(double uFlow, string pType)
        {
            var v = 1.0;

            var dia = Math.Sqrt(Convert.ToDouble(4.0 * (uFlow / 3600.0)) / Convert.ToDouble((Math.PI * v)));
            dia = dia * 1000.0;
            var diaInch = dia / (double)25.40599;

            var pipeSize = (from p in _context.Prices
                             where p.name.ToLower().Contains("pipe") &&
                             ((p.size >= dia && p.name.ToLower().Contains(pType.ToLower()) && p.unit.the_unit.ToLower() == "mm")
                             || (p.size >= diaInch && p.name.ToLower().Contains(pType.ToLower()) && p.unit.the_unit.ToLower() == "inch"))
                             orderby p.size
                             select p.size).FirstOrDefault();
            if (pipeSize != null)
            {
                return Convert.ToDouble(pipeSize);
            }
            else
            {
                pipeSize = (from p in _context.Prices
                             where p.name.ToLower().Contains("pipe") && p.name.ToLower().Contains(pType.ToLower())
                             orderby p.size
                             select p.size).Last();

                if (pipeSize != null)
                {
                    return Convert.ToDouble(pipeSize);
                }
                else
                {
                    return 1000.0;
                }
            }
        }
        public IActionResult EditEffort(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            return RedirectToAction("Edit", "Efforts", new { id });
        }
        public IActionResult getEfforts(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            return RedirectToAction("GetEfforts", "WtpBlock", new { id });
        }
        public IActionResult editThePrice(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            return RedirectToAction("Edit", "Prices", new { id });
        }

        [HttpPost]
        public IActionResult remove_wtp_part(int? id)
        {
            if (id != null)
            {
                var wtp = HttpContext.Session.GetObjectFromJson<WTPBuilder>("THEWTP");
                if(wtp.newWTPblock == null)
                {
                    wtp.newWTPblock = "test";
                }
                var wtpblock = wtp.WTPblocks.Where(b => b.id == id).First();
                wtp.WTPblocks.Remove(wtpblock);
                HttpContext.Session.Remove("THEWTP");
                HttpContext.Session.SetObjectAsJson("THEWTP", wtp);
                return RedirectToAction("CreateOldWTP", "WtpBuilder");
            }
            else
            {
                return NotFound();
            }
        }
        public WTPBuilder true_remove_filt(WTPBuilder wtp, int? id)
        {
            if (id != null)
            {
                var filtmat = wtp.filtermaterials.Where(f => f.id == id).First();
                if (filtmat != null)
                {
                    var countFiltertypes = wtp.filtermaterials.Where(fi => fi.device == filtmat.device).Count();
                    wtp.filtermaterials.Remove(filtmat);
                    var blocks = wtp.WTPblocks.Where(d => d.name == filtmat.device).ToList();
                    var countBlocks = blocks.Count();
                    for (int i = 0; i < countBlocks / countFiltertypes; i++)
                    {
                        var wtpblock = wtp.WTPblocks.Where(d => d.name == filtmat.device).First();
                        if (wtpblock != null)
                        {
                            wtp.WTPblocks.Remove(wtpblock);
                        }
                    }
                }
            }
            return wtp;
        }
        [HttpPost]
        public IActionResult Remove_filtermaterial(int? id)
        {
            var wtp = HttpContext.Session.GetObjectFromJson<WTPBuilder>("THEWTP");
            HttpContext.Session.Remove("THEWTP");
            if (id != null)
            {
                wtp = true_remove_filt(wtp, id);
            }
            HttpContext.Session.SetObjectAsJson("THEWTP", wtp);
            return RedirectToAction("CreateOldWTP", "WtpBuilder");
        }
        public async Task<List<WTP_block>> findRightSizeWTPblock(double flow, string wtpBlockName)
        {
            //EITHER FIX HERE THAT VESSELS ARE AT LEAST 2 IF SAND BUT NOT WITH CARBON MAYBE OUTSIDE!!!!
            var blocks = new List<WTP_block>();
            var theFactor = 1.0;
            var obj = 10000.0;
            var objChosen = 1000000.0;
            var allSizes = await _context.WTP_blocks
                .Where(x => x.name.ToLower() == wtpBlockName.ToLower()).OrderBy(x => x.size).ToListAsync();
            foreach (var b in allSizes)
            {
                var newLeftovers = -1.0;
                var multiplier = 1.0;
                while (newLeftovers - 1e-14 <= 0.0)
                {
                    newLeftovers = (b.size * theFactor) * multiplier - flow;
                    obj = ((b.size * theFactor) - flow) + Math.Pow(multiplier, 2.5);
                    multiplier += 1;
                }

                if (obj <= objChosen)
                {
                    objChosen = obj;
                    var chosenMulti = multiplier - 1;
                    blocks.RemoveAll(x => x.id != b.id);
                    for (var i = 0; i < chosenMulti; i++)
                    {
                        blocks.Add(b);
                    }
                }
            }
            return blocks;

        }
        [HttpPost]
        public async Task<IActionResult> Add_Filtermat(string posFiltermats)
        {
            if (posFiltermats != null)
            {
                var filterId = Convert.ToInt32(posFiltermats);
                var wtp = HttpContext.Session.GetObjectFromJson<WTPBuilder>("THEWTP");
                if (wtp.newWTPblock == null)
                {
                    wtp.newWTPblock = "test";
                }
                HttpContext.Session.Remove("THEWTP");
                var filtermat = await _context.FilterMaterials.SingleOrDefaultAsync(f => f.id == filterId);
                wtp.filtermaterials.Add(filtermat);
                var wtpblock = await _context.WTP_blocks.Where(x => x.name.ToLower() == filtermat.device.ToLower()).FirstOrDefaultAsync();
                var newBlocks = await findRightSizeWTPblock(wtp.flow, wtpblock.name);
                foreach (var b in newBlocks)
                {
                    wtp.WTPblocks.Add(b);
                }
                HttpContext.Session.SetObjectAsJson("THEWTP", wtp);
                return RedirectToAction("CreateOldWTP", "WtpBuilder");
            }
            else
            {
                return RedirectToAction("CreateOldWTP", "WtpBuilder");
            }
        }
        [HttpPost]
        public async Task<IActionResult> FindReplacementFiltermat(string posFiltermats, string chosenContaId)
        {
            if (posFiltermats != null && chosenContaId != null)
            {
                var counter = 0;
                var fps = new List<FilterPrice>(); // hmm
                var filterId = Convert.ToInt32(posFiltermats);
                
                var wtp = HttpContext.Session.GetObjectFromJson<WTPBuilder>("THEWTP");
                if (wtp.newWTPblock == null)
                {
                    wtp.newWTPblock = "test";
                }
                HttpContext.Session.Remove("THEWTP");
                var prevFilt = await _context.FilterMaterials.SingleOrDefaultAsync(x => x.id == filterId);
                var chosenConta = await _context.Contaminations.SingleOrDefaultAsync(x => x.Name == chosenContaId);
                //var oldPrice = await findPrice(wtp, prevFilt);
                var filtermats = await (from f in _context.FilterMaterials
                                        join m in _context.MediaEfficiencies on f.id equals m.filtermaterialid
                                        where m.filtermaterialid != prevFilt.id
                                        && m.contaminationId == chosenConta.Id
                                        select f)
                                    .ToListAsync();
                var prices = await Task.WhenAll(filtermats.Select(f => findPrice(wtp, f)));
                foreach (var f in filtermats)
                {
                    var fp = new FilterPrice { name = f.Name, price = prices[counter] };
                    fps.Add(fp);
                    counter += 1;
                }
                //var validchanges = prices.Where(result => result <= old_price).ToList();
                //var realvalidchanges = fps.Where(result => result.price <= old_price).ToList();
                var realvalidchanges = fps.ToList();
                FilterMaterial filtermat;
                if (realvalidchanges != null)
                {
                    if (realvalidchanges.Count > 0)
                    {
                        filtermat = await _context.FilterMaterials.SingleOrDefaultAsync(
                        f => f.Name == realvalidchanges.First(y => y.price == realvalidchanges.Min(x => x.price)).name);
                        if (filtermat == null)
                        {
                            filtermat = prevFilt;
                        }
                    }
                    else
                    {
                        filtermat = prevFilt;
                    }
                }
                else
                {
                    filtermat = prevFilt;
                }
                //saving = await findPrice(wtp, filtermat) - await findPrice(wtp, prev_filt);
                wtp.filtermaterials.Add(filtermat);
                true_remove_filt(wtp, filterId);
                var wtpblock = await _context.WTP_blocks.Where(x => x.name.ToLower() == filtermat.device.ToLower()).FirstOrDefaultAsync();
                if (wtpblock != null)
                {
                    var newBlocks = await findRightSizeWTPblock(wtp.flow, wtpblock.name);
                    foreach (var b in newBlocks)
                    {
                        wtp.WTPblocks.Add(b);
                    }
                    if (newBlocks[0].name.ToLower() == "vessel" && !filtermat.Name.ToLower().Contains("carb") && newBlocks.Count == 1)
                    {
                        wtp.WTPblocks.Add(newBlocks[0]);
                    }
                }
                HttpContext.Session.SetObjectAsJson("THEWTP", wtp);
                return RedirectToAction("CreateOldWTP", "WtpBuilder");
            }
            else
            {
                return RedirectToAction("CreateOldWTP", "WtpBuilder");
            }
        }
        [HttpPost]
        public IActionResult Add_WTPblock(string posWtPblock)
        {
            if (posWtPblock != "Please Select")
            {
                var wtpblockId = Convert.ToInt32(posWtPblock);
                var wtp = HttpContext.Session.GetObjectFromJson<WTPBuilder>("THEWTP");
                if (wtp.newWTPblock == null)
                {
                    wtp.newWTPblock = "test";
                }
                HttpContext.Session.Remove("THEWTP");
                var wtpblock = _context.WTP_blocks.SingleOrDefault(b => b.id == wtpblockId);
                wtp.WTPblocks.Add(wtpblock);
                HttpContext.Session.SetObjectAsJson("THEWTP", wtp);
                return RedirectToAction("CreateOldWTP", "WtpBuilder");
            }
            else
            {
                return RedirectToAction("CreateOldWTP", "WtpBuilder");
            }
        }
        public async Task<IActionResult> CreateOldWTP()
        {
            var wtp = HttpContext.Session.GetObjectFromJson<WTPBuilder>("THEWTP");
            if (wtp.newWTPblock == null)
            {
                wtp.newWTPblock = "test";
            }
            wtp = await addTheEfforts(wtp);
            wtp.priceList = new PriceList("test", "HW-Prime");
            wtp = await addThePrices(wtp);
            var selList = await createWTPblocklist();
            ViewData["WTPblocknames"] = new SelectList(selList, "Value", "Text");
            var selList2 = await createFilterlist();
            ViewData["Filtermats"] = new SelectList(selList2, "Value", "Text");
            var selList3 = createContamlist(wtp);
            ViewData["Contaminations"] = new SelectList(selList3, "Value", "Text");
            var selList4 = createFiltlist2(wtp);
            ViewData["ChosenFilts"] = new SelectList(selList4, "Value", "Text");
            wtp.longestList = getMaxVal(wtp.contams.Count, wtp.filtermaterials.Count(), wtp.WTPblocks.Count());
            return View("InputUse", wtp);
        }
        public int getMinVal(params int[] list)
        {
            var minimum = list[0];
            for (var i = 1; i < list.Length; i++)
            {
                if (list[i] < minimum)
                {
                    minimum = list[i];
                }
            }
            return minimum;
        }
        public int getMaxVal(params int[] list)
        {
            var maximum = list[0];
            for (var i = 1; i < list.Length; i++)
            {
                if (list[i] > maximum)
                {
                    maximum = list[i];
                }
            }
            return maximum;
        }
        public async Task<IActionResult> Index()
        {
            var dQuery = await _context.Contaminations.OrderBy(x => x.Name).ToListAsync();
            var dbLuxurities = await _context.Luxurities
                .OrderBy(l => l.wtp_luxurity).ToListAsync();
            var dbWaterTypes = await _context.Water_types
                .OrderBy(x => x.water_type).ToListAsync();
            ViewData["luxurity choices"] = new SelectList(dbLuxurities, "id", "wtp_luxurity");
            ViewData["watertype choices"] = new SelectList(dbWaterTypes, "id", "water_type");
            ViewBag.title = "WTP Input";
            var vmWTP = new VMWTP(dQuery);
            return View(vmWTP);
        }
        public async Task<double> findPriceFactor(Price p,WTPBuilder w,Effort e)
        {
            var mob = await _context.Mobilisations.FirstAsync();
            return Convert.ToDouble(mob.Amount);
        }
        public async Task<WTPBuilder> addThePrices(WTPBuilder wtp)
        {
            var effortsToRemove = new List<Effort>();
            foreach (var e in wtp.efforts)
            {
                Price thePrice;
                var effortparts = e.nameparts(e.WTP_block_name);
                var thePrices = await _context.Prices.Where(p => p.name == effortparts[0]).Include(p => p.EkdT)
                .Include(p => p.unit)
                .Include(p => p.unit_p)
                .Include(p => p.unit_r)
                .Include(p => p.division).ToListAsync();
                if (thePrices.Count.Equals(1))
                {
                    thePrice = thePrices[0];
                }
                else
                {
                    thePrice = await _context.Prices
                        .Include(p => p.EkdT)
                        .Include(p => p.unit)
                        .Include(p => p.unit_p)
                        .Include(p => p.unit_r)
                         .Include(p => p.division)
                        .FirstOrDefaultAsync(p =>
                        p.name.Contains(e.Name)
                        && p.name.Contains(wtp.pipe_type)
                        && p.size == wtp.pipe_size);
                    if (thePrice == null && (e.Category.category == "Machinery"))
                    {
                        thePrice = await _context.Prices
                            .Include(p => p.EkdT)
                            .Include(p => p.unit)
                            .Include(p => p.unit_p)
                            .Include(p => p.unit_r)
                            .Include(p => p.division)
                            .FirstOrDefaultAsync(p => e.Name.ToLower() == p.name.ToLower());
                    }
                    if (thePrice == null && (e.Name != "Pipe"))
                    {
                        thePrice = await _context.Prices
                        .FirstOrDefaultAsync(p =>
                        (effortparts[0].ToLower().Contains(p.name.ToLower()) && Convert.ToDecimal(p.size).ToString("#.##") == Convert.ToDecimal(effortparts[1]).ToString("#.##") && e.Category.category == "devices")
                        );
                    }
                }  
                if (thePrice == null)
                {
                    thePrice = await _context.Prices
                        .Include(p => p.EkdT)
                        .Include(p => p.unit)
                        .Include(p => p.unit_p)
                        .Include(p => p.unit_r)
                         .Include(p => p.division)
                         .FirstOrDefaultAsync(p => p.name == e.Name);
                }
                if (thePrice == null && e.Name == "filtermaterial")
                {
                    thePrice = await findFiltermaterial(wtp, e);
                }
                if (thePrice == null)
                {
                    thePrice = new Price
                    {
                        id = 1000000,
                        name = e.Name,
                        EkdT = new Category { category = e.WTP_block_name },
                        size = 10000.0,
                        unit = new WTPUnit { the_unit = "bags" },
                        price = 10000.0,
                        unit_p = new WTPUnit { the_unit = "euro" },
                        rent = 0.00,
                        unit_r = new WTPUnit { the_unit = "/d" },
                        division = await _context.Divisions.SingleOrDefaultAsync(x => x.Id == 7)
                    };
                }
                wtp.priceList.prices_all.Add(thePrice);
                if (e.Temp_section.section.ToLower() == "mobilisation")
                {
                    //lump sum of work hours, machinery etc. for installing
                    if (thePrice.price != null && thePrice.price != 0)
                    {
                        if (wtp.priceList.prices_mob.ContainsKey(thePrice.name))
                        {
                            if (thePrice.rent != 0 && thePrice.rent != null)
                            {
                                wtp.priceList.prices_mob[thePrice.name] = wtp.priceList.prices_mob[thePrice.name] + (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp,e)));
                                wtp.priceList.total_price_mob = wtp.priceList.total_price_mob + (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                                wtp.priceList.quantities_mob[thePrice.name] = wtp.priceList.quantities_mob[thePrice.name] + Convert.ToDecimal(e.effort);
                            }
                            else
                            {
                                wtp.priceList.prices_mob[thePrice.name] = wtp.priceList.prices_mob[thePrice.name] + (Convert.ToDecimal(thePrice.price) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                                wtp.priceList.quantities_mob[thePrice.name] = wtp.priceList.quantities_mob[thePrice.name] + Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e));
                                wtp.priceList.total_price_mob = wtp.priceList.total_price_mob + (Convert.ToDecimal(thePrice.price) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));

                            }
                        }
                        else
                        {
                            if (thePrice.rent != 0 && thePrice.rent != null)
                            {
                                wtp.priceList.prices_mob.Add(thePrice.name, Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                                wtp.priceList.total_price_mob += (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                                wtp.priceList.quantities_mob.Add(thePrice.name, Convert.ToDecimal(e.effort));
                            }
                            else
                            {
                                wtp.priceList.prices_mob.Add(thePrice.name, Convert.ToDecimal(thePrice.price) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                                wtp.priceList.total_price_mob += (Convert.ToDecimal(thePrice.price) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                                wtp.priceList.quantities_mob.Add(thePrice.name, Convert.ToDecimal(e.effort));
                            }
                        }
                    }
                    else if (thePrice.rent != null && thePrice.rent != 0)
                    {
                        if (wtp.priceList.prices_mob.ContainsKey(thePrice.name))
                        {
                            wtp.priceList.prices_mob[thePrice.name] = wtp.priceList.prices_mob[thePrice.name] + (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                            wtp.priceList.total_price_mob = wtp.priceList.total_price_mob + (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                            wtp.priceList.quantities_mob[thePrice.name] = wtp.priceList.quantities_mob[thePrice.name] + Convert.ToDecimal(e.effort);
                        }
                        else
                        {
                            wtp.priceList.prices_mob.Add(thePrice.name, Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                            wtp.priceList.total_price_mob += (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                            wtp.priceList.quantities_mob.Add(thePrice.name, Convert.ToDecimal(e.effort));
                        }
                    }
                }
                else if (e.Temp_section.section.ToLower() == "demobilisation")
                {
                    //lump sum of work hours, machinery etc. for deinstalling
                    if (thePrice.price != null && thePrice.price != 0)
                    {
                        if (wtp.priceList.prices_demob.ContainsKey(thePrice.name))
                        {
                            if (thePrice.rent != 0 && thePrice.rent != null && e.Category.category.ToLower() == "machinery")
                            {
                                wtp.priceList.prices_demob[thePrice.name] = wtp.priceList.prices_demob[thePrice.name] + (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                                wtp.priceList.total_price_demob = wtp.priceList.total_price_demob + (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                                wtp.priceList.quantities_demob[thePrice.name] = wtp.priceList.quantities_demob[thePrice.name] + Convert.ToDecimal(e.effort);
                            }
                            else if (thePrice.rent != 0 && thePrice.rent != null)
                            {
                                wtp.priceList.prices_demob[thePrice.name] = wtp.priceList.prices_demob[thePrice.name] + (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                                wtp.priceList.total_price_demob = wtp.priceList.total_price_demob + (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                                wtp.priceList.quantities_demob[thePrice.name] = wtp.priceList.quantities_demob[thePrice.name] + Convert.ToDecimal(e.effort);

                            }
                            else
                            {
                                wtp.priceList.prices_demob[thePrice.name] = wtp.priceList.prices_demob[thePrice.name] + (Convert.ToDecimal(thePrice.price) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                                wtp.priceList.total_price_demob = wtp.priceList.total_price_demob + (Convert.ToDecimal(thePrice.price) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                                wtp.priceList.quantities_demob[thePrice.name] = wtp.priceList.quantities_demob[thePrice.name] + Convert.ToDecimal(e.effort);
                            }
                        }
                        else
                        {
                            if (thePrice.rent != 0 && thePrice.rent != null && e.Category.category.ToLower() == "machinery")
                            {
                                wtp.priceList.prices_demob.Add(thePrice.name, Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                                wtp.priceList.total_price_demob += (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                                wtp.priceList.quantities_demob.Add(thePrice.name, Convert.ToDecimal(e.effort));
                            }
                            else if (thePrice.rent != 0 && thePrice.rent != null)
                            {
                                wtp.priceList.prices_demob.Add(thePrice.name, Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort));
                                wtp.priceList.total_price_demob += (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort));
                                wtp.priceList.quantities_demob.Add(thePrice.name, Convert.ToDecimal(e.effort));
                            }
                        }
                    }
                    else if (thePrice.rent != null && thePrice.rent != 0)
                    {
                        if (wtp.priceList.prices_demob.ContainsKey(thePrice.name))
                        {
                            if (e.Category.category.ToLower() == "machinery")
                            {

                                wtp.priceList.prices_demob[thePrice.name] = wtp.priceList.prices_demob[thePrice.name] + (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                                wtp.priceList.total_price_demob = wtp.priceList.total_price_demob + (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                                wtp.priceList.quantities_demob[thePrice.name] = wtp.priceList.quantities_demob[thePrice.name] + Convert.ToDecimal(e.effort);
                            }
                            else
                            {
                                wtp.priceList.prices_demob[thePrice.name] = wtp.priceList.prices_demob[thePrice.name] + (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                                wtp.priceList.total_price_demob = wtp.priceList.total_price_demob + (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                                wtp.priceList.quantities_demob[thePrice.name] = wtp.priceList.quantities_demob[thePrice.name] + Convert.ToDecimal(e.effort);
                            }

                        }
                        else
                        {
                            if (e.Category.category.ToLower() == "machinery")
                            {
                                wtp.priceList.prices_demob.Add(thePrice.name, Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                                wtp.priceList.total_price_demob += (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                                wtp.priceList.quantities_demob.Add(thePrice.name, Convert.ToDecimal(e.effort));
                            }
                            else
                            {
                                wtp.priceList.prices_demob.Add(thePrice.name, Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                                wtp.priceList.total_price_demob += (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                                wtp.priceList.quantities_demob.Add(thePrice.name, Convert.ToDecimal(e.effort));
                            }
                        }
                    }
                }
                else if (e.Temp_section.section.ToLower() == "operation/maintenance" && (e.Category.category != "devices" && e.Category.category != "materials"))
                 {
                    if (thePrice.price != null && thePrice.price != 0)
                    {
                        if (wtp.priceList.prices_operation.ContainsKey(thePrice.name))
                        {
                            if (thePrice.rent != 0 && thePrice.rent != null)
                            {
                                wtp.priceList.prices_operation[thePrice.name] = wtp.priceList.prices_operation[thePrice.name] + (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(wtp.duration_days));
                                wtp.priceList.total_price_operation = wtp.priceList.total_price_operation + (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(wtp.duration_days));
                                wtp.priceList.quantities_operation[thePrice.name] = wtp.priceList.quantities_operation[thePrice.name] + Convert.ToDecimal(e.effort);
                            }
                            else if (thePrice.price != 0 && thePrice.price != null)
                            {
                                wtp.priceList.prices_operation[thePrice.name] = wtp.priceList.prices_operation[thePrice.name] + (Convert.ToDecimal(thePrice.price) * Convert.ToDecimal(e.effort));
                                wtp.priceList.total_price_operation = wtp.priceList.total_price_operation + (Convert.ToDecimal(thePrice.price) * Convert.ToDecimal(e.effort));
                                wtp.priceList.quantities_operation[thePrice.name] = wtp.priceList.quantities_operation[thePrice.name] + Convert.ToDecimal(e.effort);
                            }
                        }
                        else
                        {
                            if (thePrice.rent != 0 && thePrice.rent != null)
                            {
                                wtp.priceList.prices_operation.Add(thePrice.name, Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(wtp.duration_days));
                                wtp.priceList.total_price_operation += (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(wtp.duration_days));
                                wtp.priceList.quantities_operation.Add(thePrice.name, Convert.ToDecimal(e.effort));
                            }
                            else if (thePrice.price != 0 && thePrice.price != null)
                            {
                                wtp.priceList.prices_operation.Add(thePrice.name, Convert.ToDecimal(thePrice.price) * Convert.ToDecimal(e.effort));
                                wtp.priceList.total_price_operation += (Convert.ToDecimal(thePrice.price) * Convert.ToDecimal(e.effort));
                                wtp.priceList.quantities_operation.Add(thePrice.name, Convert.ToDecimal(e.effort));
                            }
                        }
                    }
                    else if (thePrice.rent != null && thePrice.rent != 0)
                    {
                        if (wtp.priceList.prices_operation.ContainsKey(thePrice.name))
                        {
                            if (thePrice.rent != 0 && thePrice.rent != null)
                            {
                                wtp.priceList.prices_operation[thePrice.name] = wtp.priceList.prices_operation[thePrice.name] + (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(wtp.duration_days));
                                wtp.priceList.total_price_operation = wtp.priceList.total_price_operation + (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(wtp.duration_days));
                                wtp.priceList.quantities_operation[thePrice.name] = wtp.priceList.quantities_operation[thePrice.name] + Convert.ToDecimal(e.effort) * Convert.ToDecimal(wtp.duration_days);
                            }
                            else if (thePrice.price != 0 && thePrice.price != null)
                            {
                                wtp.priceList.prices_operation[thePrice.name] = wtp.priceList.prices_operation[thePrice.name] + (Convert.ToDecimal(thePrice.price) * Convert.ToDecimal(e.effort));
                                wtp.priceList.total_price_operation = wtp.priceList.total_price_operation + (Convert.ToDecimal(thePrice.price) * Convert.ToDecimal(e.effort));
                                wtp.priceList.quantities_operation[thePrice.name] = wtp.priceList.quantities_operation[thePrice.name] + Convert.ToDecimal(e.effort);
                            }
                        }
                        else
                        {
                            if (thePrice.rent != 0 && thePrice.rent != null)
                            {
                                wtp.priceList.prices_operation.Add(thePrice.name, Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(wtp.duration_days));
                                wtp.priceList.total_price_operation += (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(wtp.duration_days));
                                wtp.priceList.quantities_operation.Add(thePrice.name, Convert.ToDecimal(e.effort) * Convert.ToDecimal(wtp.duration_days));
                            }
                            else if (thePrice.price != 0 && thePrice.price != null)
                            {
                                wtp.priceList.prices_operation.Add(thePrice.name, Convert.ToDecimal(thePrice.price) * Convert.ToDecimal(e.effort));
                                wtp.priceList.total_price_operation += (Convert.ToDecimal(thePrice.price) * Convert.ToDecimal(e.effort));
                                wtp.priceList.quantities_operation.Add(thePrice.name, Convert.ToDecimal(e.effort));
                            }
                        }
                    }
                }
                else
                {
                    if (thePrice.price != null && thePrice.price != 0)
                    {
                        if (wtp.priceList.prices_rent.ContainsKey(thePrice.name))
                        {
                            if (thePrice.rent != 0 && thePrice.rent != null)
                            {
                                wtp.priceList.prices_rent[thePrice.name] = wtp.priceList.prices_rent[thePrice.name] + (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(wtp.duration_days));
                                wtp.priceList.total_price_rent = wtp.priceList.total_price_rent + (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(wtp.duration_days));
                                wtp.priceList.quantities_rent[thePrice.name] = wtp.priceList.quantities_rent[thePrice.name] + Convert.ToDecimal(e.effort);
                            }
                            else if (thePrice.price != 0 && thePrice.price != null)
                            {
                                wtp.priceList.prices_rent[thePrice.name] = wtp.priceList.prices_rent[thePrice.name] + (Convert.ToDecimal(thePrice.price) * Convert.ToDecimal(e.effort));
                                wtp.priceList.total_price_rent = wtp.priceList.total_price_rent + (Convert.ToDecimal(thePrice.price) * Convert.ToDecimal(e.effort));
                                wtp.priceList.quantities_rent[thePrice.name] = wtp.priceList.quantities_rent[thePrice.name] + Convert.ToDecimal(e.effort);
                            }
                        }
                        else
                        {
                            if (thePrice.rent != 0 && thePrice.rent != null)
                            {
                                wtp.priceList.prices_rent.Add(thePrice.name, Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(wtp.duration_days));
                                wtp.priceList.total_price_rent += (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(wtp.duration_days));
                                wtp.priceList.quantities_rent.Add(thePrice.name, Convert.ToDecimal(e.effort));
                            }
                            else if (thePrice.price != 0 && thePrice.price != null)
                            {
                                wtp.priceList.prices_rent.Add(thePrice.name, Convert.ToDecimal(thePrice.price) * Convert.ToDecimal(e.effort));
                                wtp.priceList.total_price_rent += (Convert.ToDecimal(thePrice.price) * Convert.ToDecimal(e.effort));
                                wtp.priceList.quantities_rent.Add(thePrice.name, Convert.ToDecimal(e.effort));
                            }
                        }
                    }
                    else if (thePrice.rent != null && thePrice.rent != 0)
                    {
                        if (wtp.priceList.prices_rent.ContainsKey(thePrice.name))
                        {
                            if (thePrice.rent != 0 && thePrice.rent != null)
                            {
                                wtp.priceList.prices_rent[thePrice.name] = wtp.priceList.prices_rent[thePrice.name] + (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(wtp.duration_days));
                                wtp.priceList.total_price_rent = wtp.priceList.total_price_rent + (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(wtp.duration_days));
                                wtp.priceList.quantities_rent[thePrice.name] = wtp.priceList.quantities_rent[thePrice.name] + Convert.ToDecimal(e.effort);
                            }
                            else if (thePrice.price != 0 && thePrice.price != null)
                            {
                                wtp.priceList.prices_rent[thePrice.name] = wtp.priceList.prices_rent[thePrice.name] + (Convert.ToDecimal(thePrice.price) * Convert.ToDecimal(e.effort));
                                wtp.priceList.total_price_rent = wtp.priceList.total_price_rent + (Convert.ToDecimal(thePrice.price) * Convert.ToDecimal(e.effort));
                                wtp.priceList.quantities_rent[thePrice.name] = wtp.priceList.quantities_rent[thePrice.name] + Convert.ToDecimal(e.effort);
                            }
                        }
                        else
                        {
                            if (thePrice.rent != 0 && thePrice.rent != null)
                            {
                                wtp.priceList.prices_rent.Add(thePrice.name, Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(wtp.duration_days));
                                wtp.priceList.total_price_rent += (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(wtp.duration_days));
                                wtp.priceList.quantities_rent.Add(thePrice.name, Convert.ToDecimal(e.effort));
                            }
                            else if (thePrice.price != 0 && thePrice.price != null)
                            {
                                wtp.priceList.prices_rent.Add(thePrice.name, Convert.ToDecimal(thePrice.price) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(wtp.duration_days));
                                wtp.priceList.total_price_rent += (Convert.ToDecimal(thePrice.price) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(wtp.duration_days));
                                wtp.priceList.quantities_rent.Add(thePrice.name, Convert.ToDecimal(e.effort));
                            }
                        }
                    }
                }
            }
            var thePriceSmall = await _context.Prices.SingleOrDefaultAsync(p => p.name == "small parts");
            for (var i = 1; i <= wtp.WTPblocks.Count(); i++)
            {
                if (wtp.priceList.prices_mob.ContainsKey(thePriceSmall.name))
                {
                    wtp.priceList.prices_all.Add(thePriceSmall);
                    wtp.priceList.prices_mob[thePriceSmall.name] = wtp.priceList.prices_mob[thePriceSmall.name] + Convert.ToDecimal(thePriceSmall.price);
                    wtp.priceList.total_price_mob = wtp.priceList.total_price_mob + Convert.ToDecimal(thePriceSmall.price);
                    wtp.priceList.quantities_mob[thePriceSmall.name] = wtp.priceList.quantities_mob[thePriceSmall.name] + 1;
                }
                else
                {
                    wtp.priceList.prices_all.Add(thePriceSmall);
                    wtp.priceList.prices_mob[thePriceSmall.name] = Convert.ToDecimal(thePriceSmall.price);
                    wtp.priceList.total_price_mob = wtp.priceList.total_price_mob + Convert.ToDecimal(thePriceSmall.price);
                    wtp.priceList.quantities_mob[thePriceSmall.name] = 1;
                }
            }
            return wtp;
        }
        public async Task<WTPBuilder> addTheEfforts(WTPBuilder wtp)
        {
            wtp.efforts = new List<Effort>();
            foreach (var b in wtp.WTPblocks)
            {
                List<Effort> efforts;
                if (wtp.wtp_luxury.wtp_luxurity == "Basic")
                {
                    var effortsne = _context.Efforts.Include(x=>x.Temp_section).Include(x=>x.WTPUnit).Include(x=>x.Wtp_luxurity).Include(x=>x.Category).Include(x=>x.Division)
                    .Where(e => (e.WTP_block_name.IndexOf(b.uniquename(), StringComparison.CurrentCultureIgnoreCase) >= 0)
                    && e.Wtp_luxurity.wtp_luxurity.Equals("Basic"));

                    efforts = await effortsne.ToListAsync();
                }
                else if (wtp.wtp_luxury.wtp_luxurity == "Medium")
                {
                    var effortsne = _context.Efforts.Include(x => x.Temp_section).Include(x => x.WTPUnit).Include(x => x.Wtp_luxurity).Include(x => x.Category).Include(x => x.Division)
                    .Where(e => (e.WTP_block_name.IndexOf(b.uniquename(), StringComparison.CurrentCultureIgnoreCase) >= 0)
                    && (e.Wtp_luxurity.wtp_luxurity.Equals("Basic") || e.Wtp_luxurity.wtp_luxurity.Equals("Medium")));
                    efforts = await effortsne.ToListAsync();
                }
                else
                {
                    var effortsne = _context.Efforts.Include(x => x.Temp_section).Include(x => x.WTPUnit).Include(x => x.Wtp_luxurity).Include(x => x.Category).Include(x => x.Division)
                    .Where(e => (e.WTP_block_name.IndexOf(b.uniquename(), StringComparison.CurrentCultureIgnoreCase) >= 0)
                    && (e.Wtp_luxurity.wtp_luxurity.Equals("Basic") || e.Wtp_luxurity.wtp_luxurity.Equals("Medium") || e.Wtp_luxurity.wtp_luxurity.Equals("Deluxe")));
                    efforts = await effortsne.ToListAsync();
                    var idxflow = efforts.FindIndex(e => e.Name.ToLower() == "flow-meter");
                    if (idxflow > -1)
                    {
                        efforts.RemoveAt(idxflow);
                    }
                }
                if (efforts.Count == 0)
                {

                }
                else
                {
                    foreach (var e in efforts)
                    {
                        if (!(
                            (
                            e.Name.ToLower().Contains("pipe") || e.Name.ToLower().Contains("bend") || e.Name.ToLower().Contains("flange") || e.Name.ToLower().Contains("t-piece") || e.Name.ToLower().Contains("sleeve") || e.Name.ToLower().Contains("butterfly")
                            )
                            &&
                            (
                            (e.Name.ToLower().Contains("electro") && wtp.pipe_type.ToLower() == "fast coupling") ||
                            (e.Name.ToLower().Contains("fast coupl") && wtp.pipe_type.ToLower() == "electro")
                            )
                            ))
                        {
                            wtp.efforts.Add(e);
                        }
                        else
                        {

                        }
                    }
                }
            }
            return wtp;
        }
        public async Task<WTPBuilder> VM_to_WTP(VMWTP inWtp)
        {
            var wtp = new WTPBuilder
            {
                WTPblocks = new List<WTP_block>(),
                filtermaterials = new List<FilterMaterial>(),
                contams = new List<Contamination>(),
                efforts = new List<Effort>(),
                contams_concentrations = inWtp.Selected_Contas_vals,
                contams_concentrations_out = inWtp.Selected_Contas_out_vals,
                duration_days = inWtp.Duration_days,
                typeofdew = await _context.Water_types.SingleOrDefaultAsync(w =>
                    w.id == Convert.ToInt32(inWtp.typeofdew)),
                wtp_luxury =
                    await _context.Luxurities.SingleOrDefaultAsync(l => l.id == Convert.ToInt32(inWtp.wtp_luxury)),
                distance = inWtp.distance,
                flow = inWtp.flow
            };
            // look into these functions
            var thePipeType = PipeType(inWtp.Duration_days);
            var pipesize = pipeSize(wtp.flow, thePipeType);
            if (pipesize >= 999.0)
            {
                thePipeType = "electro";
                pipesize = pipeSize(wtp.flow, thePipeType);
            }
            wtp.pipe_size = pipesize;
            wtp.pipe_type = thePipeType;
            return wtp;
        }
        //public async Task<int> findNumberOfFilts(Contamination c, FilterMaterial f, string dpoint, double conc)
        public async Task<int> findNumberOfFilts(Contamination c, FilterMaterial f, double conc, double concOut)
        {
            double? dLimit;
            var numberFilts = 1;
            if (conc < 0.0000001 || concOut < 0.0000001)
            {
                return 1;
            }
            //if client has set limit, take that, else use default_value 
            if ( 1e-15 < concOut && concOut <= conc)
            {
                dLimit = concOut;
            }
            else
            {
                if (concOut >= conc)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            var mef = await _context.MediaEfficiencies.SingleOrDefaultAsync(m => m.filtermaterial == f && m.Contamination == c);
            if (mef == null)
            {
                return 1;
            }
            var enough = false;
            while (!enough)
            {
                var loweredTo = conc * Math.Pow((1-Convert.ToDouble(mef.efficiency) / 100.0) ,numberFilts);
                if (loweredTo < dLimit)
                {
                    enough = true;
                }
                else
                {
                    numberFilts += 1;
                }
            }
            return numberFilts;
        }
        public async Task<List<FilterMaterial>> HandleSpecialFilters(WTPBuilder wtp,Contamination cont,double concentration, double concentrationOut, List<FilterMaterial> ffS,Dictionary<string,int> filtDict)
        {
            if (ffS.FindIndex(x => x.Name.ToLower() == "air stripping") > -1)
            {
                var qSpecial = await _context.Special_Cases_Air_Strippers
                    .FirstOrDefaultAsync(x => x.cont_name == cont.Name);
                if (qSpecial != null)
                {
                    if (qSpecial.new_filter != "")
                    {
                        var newFilt = await _context.FilterMaterials.SingleOrDefaultAsync(x => x.Name.ToLower() == "sand");//q_special.new_filter);
                        var numberFilt = await findNumberOfFilts(cont, newFilt, concentration, concentrationOut);
                        if (filtDict.ContainsKey(newFilt.Name))
                        {
                            if (numberFilt > filtDict[newFilt.Name])
                            {
                                filtDict[newFilt.Name] = numberFilt;
                            }
                            numberFilt = numberFilt - filtDict[newFilt.Name];
                            for (var j = 1; j <= numberFilt; j++)
                            {
                                ffS.Add(newFilt);
                            }
                        }
                        else
                        {
                            filtDict.Add(newFilt.Name, numberFilt);
                            for (var j = 1; j <= numberFilt; j++)
                            {
                                ffS.Add(newFilt);
                            }
                        }
                    }
                }
                else
                {
                    var newFilt = await _context.FilterMaterials.SingleOrDefaultAsync(x => x.Name.ToLower().Contains("air") && x.Name.ToLower().Contains("carb"));
                    var numberFilt = await findNumberOfFilts(cont, newFilt, concentration, concentrationOut);
                    if (filtDict.ContainsKey(newFilt.Name))
                    {
                        if (numberFilt > filtDict[newFilt.Name])
                        {
                            filtDict[newFilt.Name] = numberFilt;
                        }
                        numberFilt = numberFilt - filtDict[newFilt.Name];
                        for (var j = 1; j <= numberFilt; j++)
                        {
                            ffS.Add(newFilt);
                        }
                    }
                    else
                    {
                        filtDict.Add(newFilt.Name, numberFilt);
                        for (var j = 1; j <= numberFilt; j++)
                        {
                           ffS.Add(newFilt);
                        }
                    }
                }
            }
            return ffS;
        }
        public async Task<WTPBuilder> HandleOilSkimmerCount(WTPBuilder wtp)
        {
            var countSedi = wtp.WTPblocks.Count(b => b.name.ToLower() == "sedimentation tank");
            var countOilSep = wtp.WTPblocks.Count(b => b.name.ToLower().Contains("oil") && b.name.ToLower().Contains("sep"));
            var countCollection = wtp.WTPblocks.Count(b => b.name.ToLower() == "collection tank");
            if (countOilSep != 0 && (countOilSep < getMaxVal(countSedi, countCollection)))
            {
                var oilSep = await _context.WTP_blocks.SingleOrDefaultAsync(b => b.name.ToLower().Contains("oil") && b.name.ToLower().Contains("sep"));
                for (var i = 1; i <= (getMaxVal(countSedi, countCollection) - countOilSep); i++)
                {
                    wtp.WTPblocks.Add(oilSep);
                }
            }
            return wtp;
        }
        public async Task<WTPBuilder> HandleBackflushingParts(WTPBuilder wtp)
        {
            var backflushingParts = await _context.FilterMaterials
                        .Where(f => f.contaminations.Equals("backflushing", StringComparison.OrdinalIgnoreCase)).ToListAsync();
            foreach (var f in backflushingParts)
            {
                var device = await _context.WTP_blocks
                    .SingleOrDefaultAsync(d => d.name.Equals(f.device, StringComparison.OrdinalIgnoreCase));
                wtp.WTPblocks.Add(device);
                if ((device.name.ToLower().Contains("slurry") || device.name.ToLower().Contains("sludge")) && wtp.typeofdew.water_type.ToLower() == "dirty")
                {
                    wtp.WTPblocks.Add(device);
                }
            }
            return wtp;
        }
        public async Task<WTPBuilder> HandleScadaAndElectronics(WTPBuilder wtp)
        {
            if (wtp.wtp_luxury.wtp_luxurity == "Deluxe")
            {
                if (wtp.flow < 25)
                {
                    var scadaParts = await _context.WTP_blocks
                        .Where(d => d.name.Equals("FU_steering_basic", StringComparison.OrdinalIgnoreCase) && d.size == 7.5 || d.name.Equals("Analytical Monitoring Station", StringComparison.OrdinalIgnoreCase)).ToListAsync();
                    foreach (var b in scadaParts)
                    {
                        wtp.WTPblocks.Add(b);
                    }
                }
                else
                {
                    var scadaParts = await _context.WTP_blocks
                        .Where(d => d.name.Equals("FU steering cabinet basic", StringComparison.OrdinalIgnoreCase) && d.size == 75 || d.name.Equals("Analytical Monitoring Station", StringComparison.OrdinalIgnoreCase)).ToListAsync();
                    foreach (var b in scadaParts)
                    {
                        wtp.WTPblocks.Add(b);
                    }
                }
            }
            else if (wtp.wtp_luxury.wtp_luxurity == "Medium")
            {
                if (wtp.flow < 50)
                {
                    var scadaParts = await _context.WTP_blocks
                        .Where(d => d.name.Equals("FU_steering_basic", StringComparison.OrdinalIgnoreCase) && d.size == 7.5).ToListAsync();
                    foreach (var b in scadaParts)
                    {
                        wtp.WTPblocks.Add(b);
                    }
                }
                else
                {
                    var scadaParts = await _context.WTP_blocks
                        .Where(d => d.name.Equals("FU_steering_basic", StringComparison.OrdinalIgnoreCase) && d.size == 75).ToListAsync();
                    foreach (var b in scadaParts)
                    {
                        wtp.WTPblocks.Add(b);
                    }
                }
            }
            return wtp;
        }
        public async Task<WTPBuilder> createWTPfromLPModel(VMWTP inWtp)
        {
            var wtp = await VM_to_WTP(inWtp);
            var selectedContaminations = inWtp.Selected_Contas;
            foreach (var contI in selectedContaminations)
            {
                var cont = await _context.Contaminations
                    .FirstOrDefaultAsync(m => m.Name == contI);
                wtp.contams.Add(cont);
            }
            var filtDict = new Dictionary<string, int>();
            var fmats2 = await FindSetCover(wtp, inWtp.Selected_Contas, inWtp.Selected_Contas_vals, inWtp.Selected_Contas_out_vals);
            var filtermaterialsen = new List<FilterMaterial>();
            foreach (var f in fmats2)
            {
                foreach(var c in wtp.contams)
                {
                    var numberFilt = await findNumberOfFilts(c, f, inWtp.Selected_Contas_vals[wtp.contams.IndexOf(c)], inWtp.Selected_Contas_out_vals[wtp.contams.IndexOf(c)]);
                    if (filtDict.ContainsKey(f.Name))
                    {
                        if (numberFilt > filtDict[f.Name])
                        {
                            filtDict[f.Name] = numberFilt;
                            numberFilt = numberFilt - filtDict[f.Name];
                            for (var j = 1; j <= numberFilt; j++)
                            {
                                filtermaterialsen.Add(f);
                            }
                        }
                    }
                    else
                    {
                        filtDict.Add(f.Name, numberFilt);
                        for (var j = 1; j <= numberFilt; j++)
                        {
                            filtermaterialsen.Add(f);
                        }
                    }
                }
                foreach(var c in wtp.contams)
                {
                    filtermaterialsen = await HandleSpecialFilters(wtp, c, inWtp.Selected_Contas_vals[wtp.contams.IndexOf(c)], inWtp.Selected_Contas_out_vals[wtp.contams.IndexOf(c)], filtermaterialsen, filtDict);
                }
            }
            foreach (var f in filtermaterialsen)
            {
                wtp.filtermaterials.Add(f);
                var wtpBlock = await _context.WTP_blocks
                    .FirstOrDefaultAsync(b => b.name.Equals(f.device, StringComparison.OrdinalIgnoreCase) &&
                    !b.name.Equals("x", StringComparison.OrdinalIgnoreCase));
                if (wtpBlock != null)
                {
                    var wBlocks = await findRightSizeWTPblock(wtp.flow, wtpBlock.name);
                    foreach (var b in wBlocks)
                    {
                        wtp.WTPblocks.Add(b);
                    }
                    if (wBlocks[0].name == "Vessel" && !f.Name.ToLower().Contains("carb") && wBlocks.Count == 1)
                    {
                        wtp.WTPblocks.Add(wBlocks[0]);
                    }
                }
            }
            var wBlocks2 = await _context.WTP_blocks
                   .Where(d => d.necessity).OrderBy(x => x.name).GroupBy(x => x.name).Select(group => group.First()).ToListAsync();
            foreach (var b in wBlocks2)
            {
                wtp.WTPblocks.Add(b);
            }
            wtp = await HandleOilSkimmerCount(wtp);
            wtp = await HandleBackflushingParts(wtp);
            wtp = await HandleScadaAndElectronics(wtp);
            return wtp;
        }
        public async Task<WTPBuilder> createWTPfromVMModel(VMWTP inWtp)
        {
            var wtp = await VM_to_WTP(inWtp);
            var selectedContaminations = inWtp.Selected_Contas;
            var concIdx = 0;
            var concOutIdx = 0;
           var filtDict = new Dictionary<string, int>();
            foreach (var contI in selectedContaminations)
            {
                var cont = await _context.Contaminations
                    .FirstOrDefaultAsync(m => m.Name == contI);
                wtp.contams.Add(cont);
                var concentration = inWtp.Selected_Contas_vals[concIdx];
                var concentrationOut = inWtp.Selected_Contas_out_vals[concOutIdx];
                var filtermaterialsen = new List<FilterMaterial>();
                List<FilterMaterial> fmats;
                if (wtp.typeofdew.water_type == "Clean")
                {
                    fmats = await (from f in _context.FilterMaterials
                                   join m in _context.MediaEfficiencies on f equals m.filtermaterial
                                   where m.Contamination.Equals(cont.Name) &&
                                   f.water_typeid == wtp.typeofdew.id &&
                                   m.efficiency == _context.MediaEfficiencies
                                   .Where(x => x.Contamination.Equals(cont.Name))
                                   .Max(x => x.efficiency)
                                   select f).ToListAsync();
                }
                else
                {
                    fmats = await (from f in _context.FilterMaterials
                                   join m in _context.MediaEfficiencies on f equals m.filtermaterial
                                   where m.Contamination.Name.Equals(cont.Name) &&
                                   m.efficiency == _context.MediaEfficiencies
                                   .Where(x => x.Contamination.Name.Equals(cont.Name))
                                   .Max(x => x.efficiency)
                                   select f).ToListAsync();
                }
                if (fmats == null)
                {
                    fmats = await (from f in _context.FilterMaterials
                                   join m in _context.MediaEfficiencies on f equals m.filtermaterial
                                   where m.Contamination.Name.Contains(cont.Name)
                                   select f).ToListAsync();
                }
                if (fmats == null)
                {
                    fmats = await _context.FilterMaterials
                        .Where(m => m.contaminations.Contains(cont.contam_group) && m.water_typeid == wtp.typeofdew.id).ToListAsync();
                }
                if (fmats == null)
                {
                    fmats = await _context.FilterMaterials
                        .Where(m => m.contaminations.Contains(cont.contam_group)).ToListAsync();
                }
                if (fmats == null)
                {
                    //No Filtermaterials to counter one of the contaminations
                    NotFound();
                }
                
                foreach (var f in fmats)
                {
                    var numberFilt = await findNumberOfFilts(cont, f, concentration, concentrationOut);
                    if (filtDict.ContainsKey(f.Name))
                    {
                        if (numberFilt > filtDict[f.Name])
                        {
                            filtDict[f.Name] = numberFilt;
                            numberFilt = numberFilt - filtDict[f.Name];
                            for (var j = 1; j <= numberFilt; j++)
                            {
                                filtermaterialsen.Add(f);
                            }
                        }
                    }
                    else
                    {
                        filtDict.Add(f.Name, numberFilt);
                        for (var j = 1; j <= numberFilt; j++)
                        {
                            filtermaterialsen.Add(f);
                        }
                    }
                }
                filtermaterialsen = await HandleSpecialFilters(wtp, cont, concentration,concentrationOut, filtermaterialsen, filtDict);
                foreach (var f in filtermaterialsen)
                {
                    if (true)
                    {
                        wtp.filtermaterials.Add(f);
                        var wtpBlock = await _context.WTP_blocks
                            .FirstOrDefaultAsync(b => b.name.Equals(f.device, StringComparison.OrdinalIgnoreCase) &&
                            !b.name.Equals("x", StringComparison.OrdinalIgnoreCase));
                        if (wtpBlock != null)
                        {
                            var wBlocks = await findRightSizeWTPblock(wtp.flow, wtpBlock.name);
                            foreach (var b in wBlocks)
                            {
                                wtp.WTPblocks.Add(b);
                            }
                            if (wBlocks[0].name == "Vessel" && !f.Name.ToLower().Contains("carb") && wBlocks.Count == 1)
                            {
                                wtp.WTPblocks.Add(wBlocks[0]);
                            }
                        }
                    }
                }
            }
            var wBlocks2 = await _context.WTP_blocks
                   .Where(d => d.necessity == true).OrderBy(x => x.name).GroupBy(x => x.name).Select(group => group.First()).ToListAsync();
            foreach (var b in wBlocks2)
            {
                wtp.WTPblocks.Add(b);
            }
            wtp = await HandleOilSkimmerCount(wtp);
            wtp = await HandleBackflushingParts(wtp);
            wtp = await HandleScadaAndElectronics(wtp);
            return wtp;
        }
        [HttpPost]
        public IActionResult Add_Concentrations(VMWTP inWtp)
        {
            if (ModelState.IsValid)
            {
                var selectedContaminations = inWtp.Selected_Contas;
                var conts = selectedContaminations.ToList();
                var virtualConc = new VirtualConc(conts);
                HttpContext.Session.SetObjectAsJson("virtualWTP", inWtp);
                return View(virtualConc);
            }
            else
            {
                return RedirectToAction("Index", "WtpBuilder");
            }           
        }
        [HttpPost]
        public async Task<IActionResult> InputUse(VirtualConc vC)
        {
            var inWtp = HttpContext.Session.GetObjectFromJson<VMWTP>("virtualWTP");
            HttpContext.Session.Remove("virtualWTP");
            var cVals = new List<double>();
            var cOutVals = new List<double>();
            foreach (var v in vC.concentrations)
            {
                cVals.Add(v);
            }
            inWtp.Selected_Contas_vals = cVals;
            foreach (var v in vC.concentrations_out)
            {
                cOutVals.Add(v);
            }
            inWtp.Selected_Contas_out_vals = cOutVals;
            //WTPBuilder wtp = await createWTPfromVMModel(inWTP);
            var wtp = await createWTPfromLPModel(inWtp);
            wtp = await addTheEfforts(wtp);
            wtp.priceList = new PriceList("test", "HW-Prime");
            if(wtp.newWTPblock == null)
            {
                wtp.newWTPblock = "test";
            }
            wtp = await addThePrices(wtp);
            wtp.longestList = getMaxVal(wtp.WTPblocks.Count, wtp.filtermaterials.Count, wtp.contams.Count);
            HttpContext.Session.SetObjectAsJson("THEWTP", wtp);
            var selList = await createWTPblocklist();
            ViewData["WTPblocknames"] = new SelectList(selList, "Value", "Text");
            var selList2 = await createFilterlist();
            ViewData["Filtermats"] = new SelectList(selList2, "Value", "Text");
            var selList3 = createContamlist(wtp);
            ViewData["Contaminations"] = new SelectList(selList3, "Value", "Text");
            var selList4 = createFiltlist2(wtp);
            ViewData["ChosenFilts"] = new SelectList(selList4, "Value", "Text");
            return View(wtp);
        }

        public async Task<IEnumerable<SelectListItem>> createWTPblocklist()
        {
            var wtpblocknames = await _context.WTP_blocks
                .OrderBy(b => b.name).ToListAsync();
            var selList = from s in wtpblocknames
                                                  select new SelectListItem
                                                  {
                                                      Value = s.id.ToString(),
                                                      Text = s.name + "_" + s.size.ToString()
                                                  };
            return selList;
        }
        public async Task<IEnumerable<SelectListItem>> createFilterlist()
        {
            var filtermats = await _context.FilterMaterials
                .OrderBy(f => f.Name).ToListAsync();
            var selList = from f in filtermats
                                                  select new SelectListItem
                                                  {
                                                      Value = f.id.ToString(),
                                                      Text = f.Name
                                                  };
            return selList;
        }
        public IEnumerable<SelectListItem> createContamlist(WTPBuilder wtp)
        {
            var contams = wtp.contams
                .OrderBy(f => f.Name);
            var selList = from f in contams
                                                  select new SelectListItem
                                                  {
                                                      Value = f.Name.ToString(),
                                                      Text = f.Name
                                                  };
            return selList;
        }
        public IEnumerable<SelectListItem> createFiltlist2(WTPBuilder wtp)
        {
            var filts = wtp.filtermaterials
                .OrderBy(f => f.Name);
            var selList = from f in filts
                                                  select new SelectListItem
                                                  {
                                                      Value = f.id.ToString(),
                                                      Text = f.Name
                                                  };
            return selList;
        }
        public async Task<List<FilterMaterial>> FindSetCover(WTPBuilder wtp,List<string> conts,List<double> concentrations, List<double> concentrationsOut)
        {
            var theUser = await _userManager.GetUserAsync(User);

            //string thepath = String.Concat("/LPSolveResults/",the_user.UserName,"_", DateTime.Now.ToShortDateString(),"_",DateTime.Now.ToShortTimeString());
            string thepath;
            try
            {
                var randnum = new Random();
                thepath = String.Concat(theUser.UserName, "_", DateTime.Now.ToString("dd-mm-yyyy"), "_", Math.Round(randnum.NextDouble(), 2).ToString().Replace(".", "_"), ".txt");
            }
            catch
            {
                var randnum = new Random();
                thepath = String.Concat(Math.Round(randnum.NextDouble(), 2).ToString().Replace(".", "_"), "_", DateTime.Now.ToShortDateString(), "_", Math.Round(randnum.NextDouble(), 2).ToString().Replace(".", "_"), ".txt");
            }
            finally
            {
            }
            var posFilts = await (from f in _context.MediaEfficiencies
                             where conts.IndexOf(f.Contamination.Name) >= 0
                             group f by f.filtermaterial into me
                             select new
                             {
                                 filtermaterial = me.Key,
                                 TotalConts = me.Count(),
                                 AverageEff = me.Sum(item => item.efficiency) / me.Count(),
                                 Contams = string.Join(",", me.Select(i => i.Contamination.Name))
                             }).ToListAsync();
            var solution = new List<FilterMaterial>();            
            var w1 = new List<double>();

            for (var l = 0; l < posFilts.Count(); l++)
            {
                var filtermat = await _context.FilterMaterials.SingleOrDefaultAsync(f => f.Name.ToLower() == posFilts[l].filtermaterial.Name.ToLower());
                var pricer = await findPrice(wtp, filtermat);
                w1.Add(pricer);
            }
            using (var lp = LpSolve.make_lp(0, posFilts.Count()))
            {
                var minval = 500000000.0;
                lp.set_outputfile(String.Concat(AppDomain.CurrentDomain.BaseDirectory,"/LPsolveResults/",thepath));
                for (var k = 0; k < posFilts.Count(); k++)
                {
                    lp.set_col_name(k + 1, posFilts[k].filtermaterial.Name);
                }
                var objfuncList = new List<double>();
                for (var j = 0; j < conts.Count(); j++)
                {
                    var rowInit = new List<double> {0};
                    for (var i = 0; i < posFilts.Count(); i++)
                    {
                        if (posFilts[i].Contams.Contains(conts.ElementAt(j)))
                        {
                            //this section checks serialization of filtermaterial
                            var c = await _context.Contaminations.SingleOrDefaultAsync(x => x.Name == conts[j]);
                            var f = await _context.FilterMaterials.SingleOrDefaultAsync(x => x.Name.ToLower() == posFilts[i].filtermaterial.Name.ToLower());
                            var numFilts = await findNumberOfFilts(c, f, concentrations[j], concentrationsOut[j]);
                            //
                            var theval = numFilts * Convert.ToDouble(w1.ElementAt(i));
                            if (theval < minval && theval > 0)
                            {
                                // update rhs
                                minval = theval;
                            }
                            rowInit.Add(theval);
                        }
                        else
                        {
                            rowInit.Add(0.00);
                        }
                    }
                    var row = rowInit.ToArray();
                    lp.add_constraint(row, lpsolve_constr_types.GE, minval-0.01);
                }
                objfuncList.Add(0);
                for (var i = 1; i <= posFilts.Count(); i++)
                {
                    lp.set_binary(i, true);
                    objfuncList.Add(Convert.ToDouble(w1.ElementAt(i - 1)));
                }
                var objFu = objfuncList.ToArray();
                lp.set_obj_fn(objFu);
                lp.set_minim();
                lp.print_lp();
                lp.solve();
                lp.print_objective();
                lp.print_solution(1);
                lp.print_constraints(1);
                lp.get_variables(objFu);
                for (var i = 0; i < posFilts.Count(); i++)
                {
                    var variablen = objFu[i];
                    if (variablen == 1)
                    {
                        var filtermat = await _context.FilterMaterials.SingleOrDefaultAsync(f => f.Name.ToLower() == posFilts[i].filtermaterial.Name.ToLower());
                        solution.Add(filtermat);
                    }
                }
            }
            return solution;
        }
        public async Task<double> findPrice(WTPBuilder wtp, FilterMaterial f)
        {
            var numblocks = 1;
            decimal price = 0;
            var wtpBlock = await _context.WTP_blocks.Where(x => x.name.ToLower() == f.device.ToLower()).FirstOrDefaultAsync();
            if (wtpBlock != null)
            {
                var wBlocks = await findRightSizeWTPblock(wtp.flow, wtpBlock.name);
                numblocks = wBlocks.Count;
                if (wBlocks[0].name == "Vessel" && !f.Name.ToLower().Contains("carb") && wBlocks.Count == 1)
                {
                    numblocks += 1;
                }
            }
            if (wtpBlock != null)
            {
                var efforts = await _context.Efforts.Where(e => e.WTP_block_name == String.Concat(wtpBlock.name, "_", wtpBlock.size.ToString())).ToListAsync();
                foreach (var e in efforts)
                {                    
                    e.Category = await _context.Categories.SingleOrDefaultAsync(c => c.id == e.CategoryId);
                    var effortparts = e.nameparts(e.WTP_block_name);
                    var thePrice = await _context.Prices
                        .FirstOrDefaultAsync(p =>
                        p.name.Contains(e.Name) && p.name.Contains(wtp.pipe_type) && p.size == wtp.pipe_size);
                    if (thePrice == null && (e.Category.category == "Machinery"))
                    {
                        thePrice = await _context.Prices
                            .FirstOrDefaultAsync(p => e.Name.ToLower() == p.name.ToLower());
                    }
                    if (thePrice == null && (e.Name != "Pipe")) // || e.Name != "Bend 45/90 degree" || e.Name != "Bend 90 degree")
                    {
                        thePrice = await _context.Prices
                        .FirstOrDefaultAsync(p =>
                        (effortparts[0].ToLower().Contains(p.name.ToLower()) && Convert.ToDecimal(p.size).ToString("#.##") == Convert.ToDecimal(effortparts[1]).ToString("#.##") && e.Category.category == "devices")
                        );
                    }
                    if (thePrice == null)
                    {
                        thePrice = await _context.Prices.FirstOrDefaultAsync(p => p.name == e.Name);
                    }
                    if (thePrice == null && e.Name == "filtermaterial")
                    {
                        thePrice = await _context.Prices.SingleOrDefaultAsync(p => p.name.ToLower().Contains(f.Name.ToLower()) && p.unit.the_unit == "bags");
                        //var trueprice = findFiltermaterial(wtp, e);
                       // string lol = "LOL";
                    }
                    if (thePrice == null)
                    {
                        thePrice = new Price
                        {
                            id = 1000000,
                            name = e.Name,
                            EkdT = new Category { category = e.WTP_block_name },
                            size = 10000.0,
                            unit = new WTPUnit { the_unit = "CASH" },
                            price = 10000.0,
                            unit_p = new WTPUnit { the_unit = "CASH" },
                            rent = 0.00,
                            unit_r = new WTPUnit { the_unit = "CASH" }
                        };
                    }
                    //REMOVE ALL WTP PRICELIST STUFF
                    e.Temp_section = await _context.Temporal_sections.SingleOrDefaultAsync(t => t.id == e.Temp_sectionid);
                    if (e.Temp_section.section.ToLower() == "mobilisation" || e.Temp_section.section.ToLower() == "demobilisation")
                    {
                        //lump sum of work hours, machinery etc. for installing
                        if (thePrice.price != null && thePrice.price != 0)
                        {
                            if (thePrice.rent != 0 && thePrice.rent != null)
                            {
                                price += (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                            }
                            else
                            {
                                price += (Convert.ToDecimal(thePrice.price) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                            }
                        }
                        else if (thePrice.rent != null && thePrice.rent != 0)
                        {
                            price += (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(await findPriceFactor(thePrice, wtp, e)));
                        }
                    }
                    else if (e.Temp_section.section.ToLower() == "operation/maintenance" && (e.Category.category != "devices" && e.Category.category != "materials"))
                    {
                        if (thePrice.rent != 0 && thePrice.rent != null)
                        {
                            price += (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(wtp.duration_days));
                        }
                        else if (thePrice.price != 0 && thePrice.price != null)
                        {
                            price += (Convert.ToDecimal(thePrice.price) * Convert.ToDecimal(e.effort));
                        }
                    }
                    else
                    {
                        if (thePrice.rent != 0 && thePrice.rent != null)
                        {
                            price += (Convert.ToDecimal(thePrice.rent) * Convert.ToDecimal(e.effort) * Convert.ToDecimal(wtp.duration_days));
                        }
                        else if (thePrice.price != 0 && thePrice.price != null)
                        {
                            price += (Convert.ToDecimal(thePrice.price) * Convert.ToDecimal(e.effort));
                        }
                    }
                }
            }
            return Convert.ToDouble(price) * numblocks;
        }
    }
}



