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
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Identity;
using MainOps.Resources;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using MainOps.Models.ViewModels;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,Guest,International")]
    public class ItemTypesController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LocService _localizer;
        private IWebHostEnvironment _env;

        public ItemTypesController(DataContext context,UserManager<ApplicationUser> userManager,LocService Localizer, IWebHostEnvironment env):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
            _localizer = Localizer;
            _env = env;
        }

        // GET: ItemTypes
        [HttpGet]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,International")]
        public async Task<IActionResult> Index(int? ProjectId = null)
        {
            var user = await _userManager.GetUserAsync(User);
            IEnumerable<SelectListItem> selList = await createFilterlist();
            //ViewData["Filterchoices"] = new SelectList(selList, "Value", "Text");
            ViewData["ProjectId"] = await GetProjectList();
            var data = await _context.ItemTypes.Include(x => x.Rental_Unit).Include(x => x.Install_Unit).Include(x => x.Project).Where(x => x.Project.Active.Equals(true)).OrderBy(x => x.Item_Type).ThenBy(x=>x.Project.Name)
                .ToListAsync();

            if (User.IsInRole("International") && !User.IsInRole("Admin"))
            {
                return View(data.Where(x => x.Project.Name.Contains("STOCK")));
            }
            if(ProjectId != null)
            {
                data = data.Where(x => x.ProjectId.Equals(ProjectId)).ToList();
            }
            if (User.IsInRole("Admin"))
            {
                return View(data);
            }
            else
            {
                return View(data.Where(x=>x.Project.DivisionId.Equals(user.DivisionId)));
            }
        }
        
        [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
        [HttpPost]
        public async Task<IActionResult> PopulateFromTemplate(int? ProjectId,int? TemplateProjectId)
        {
            if(ProjectId == null || TemplateProjectId == null)
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "Insufficient parameters for this function call" });
            }
            //Preload info
            var user = await _userManager.GetUserAsync(User);           
            int oldprojectid = Convert.ToInt32(ProjectId); //project to be populated
            int newprojectid = Convert.ToInt32(TemplateProjectId); //template to use
            var oldproject = await _context.Projects.SingleOrDefaultAsync(x => x.Id.Equals(oldprojectid));
            var newproject = await _context.Projects.SingleOrDefaultAsync(x => x.Id.Equals(newprojectid));
            //check if authorized to use projects!
            if((!oldproject.DivisionId.Equals(user.DivisionId) || newproject.DivisionId.Equals(user.DivisionId)) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You cannot populate a project you are not in the same division of or use a template from another division" });
            }
            //get new BoQs, titles, headlines
            var boqheadlines = await _context.BoQHeadLines.Where(x => x.ProjectId.Equals(oldprojectid)).ToListAsync();
            var itemtypes = await _context.ItemTypes.Where(x => x.ProjectId.Equals(oldprojectid)).ToListAsync();
            var titles = await _context.Titles.Where(x => x.ProjectId.Equals(oldprojectid)).ToListAsync();
            //get old BoQ, titles and headlines for deletion
            var boqheadlinesold = await _context.BoQHeadLines.Where(x => x.ProjectId.Equals(newprojectid)).ToListAsync();
            var itemtypesold = await _context.ItemTypes.Where(x => x.ProjectId.Equals(newprojectid)).ToListAsync();
            var titlesold = await _context.Titles.Where(x => x.ProjectId.Equals(newprojectid)).ToListAsync();
            foreach (var item in boqheadlinesold)
            {
                _context.Remove(item);
            }
            await _context.SaveChangesAsync();
            foreach (var item in titlesold)
            {
                _context.Remove(item);
            }
            await _context.SaveChangesAsync();
            foreach (var item in itemtypesold)
            {
                _context.Remove(item);
            }

            await _context.SaveChangesAsync();
            foreach (var item in boqheadlines)
            {
                BoQHeadLine boq = new BoQHeadLine(item, newprojectid);
                _context.Add(boq);
            }
            await _context.SaveChangesAsync();
            foreach (var item in itemtypes)
            {
                ItemType it = new ItemType(item, newprojectid);
                _context.Add(it);
            }
            await _context.SaveChangesAsync();
            foreach (var item in titles)
            {
                var itemtyp = await _context.ItemTypes.Where(x => x.Item_Type.Equals(item.TheTitle) && x.ProjectId.Equals(newprojectid)).SingleOrDefaultAsync();
                Title tit = new Title(item, newprojectid, itemtyp.Id);
                _context.Add(tit);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember")]
        [HttpPost]
        public async Task<IActionResult> IndexAsPdf(int? ProjectIdPdf = null)
        {
            if(ProjectIdPdf != null)
            {
                var data = await _context.ItemTypes.Where(x=>x.ProjectId.Equals(ProjectIdPdf)).Include(x => x.Rental_Unit).Include(x => x.Install_Unit).OrderBy(x => x.BoQnr).ToListAsync();
                return new ViewAsPdf("_Index", data);
            }
            else
            {
                var data = await _context.ItemTypes.Include(x => x.Rental_Unit).Include(x => x.Install_Unit).OrderBy(x => x.BoQnr).ToListAsync();
                return new ViewAsPdf("_Index", data);
            }
            
        }
        [HttpGet]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember")]
        public async Task<string> GetUnit(int theId)
        {
            var itemtype = await _context.ItemTypes.Include(x => x.Install_Unit).Where(x => x.Id.Equals(theId)).SingleOrDefaultAsync();
            return itemtype.Install_Unit.TheUnit;
        }
        // GET: ItemTypes/Details/5
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemType = await _context.ItemTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (itemType == null)
            {
                return NotFound();
            }

            return View(itemType);
        }

        // GET: ItemTypes/Create
        [HttpGet]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,International")]
        public async Task<IActionResult> Create()
        {
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["UnitId"] = new SelectList(_context.Units, "Id", "TheUnit");
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "Type");
            return View();
        }
        public async Task<IActionResult> GetItemTypes(string theId,string searchfield)
        {
            int ProjectId = Convert.ToInt32(theId);
            var itemtypes = await _context.ItemTypes.Where(x => x.ProjectId.Equals(ProjectId)).ToListAsync();
            if(searchfield != null && searchfield != "")
            {
                itemtypes = itemtypes.Where(x => x.BoQnr.ToString().ToLower().Contains(searchfield.ToLower()) || x.Item_Type.ToLower().Contains(searchfield.ToLower())).ToList();
            }
            return View("_itemtypes", itemtypes);

        }

        // POST: ItemTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,International")]
        public async Task<IActionResult> Create([Bind("Id,Item_Type,BoQnr,BoQnr_Rental,price,rental_price,Install_UnitId,Rental_UnitId,initial_cost,Daily_cost,ProjectId,Valuta,ReportTypeId,AgreementNumber,AgreementDate,ExpectedAmount,ExpectedAmountRental")] ItemType itemType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(itemType);
                await _context.SaveChangesAsync();
                if(itemType.Rental_UnitId != null)
                {
                    var lastadded = await _context.ItemTypes.Include(x=>x.Rental_Unit).LastAsync();
                    if(lastadded.Rental_Unit.TheUnit.Equals("pr. hour"))
                    {
                        Title newTitle = new Title();
                        newTitle.ItemTypeId = lastadded.Id;
                        newTitle.ProjectId = lastadded.ProjectId;
                        newTitle.TheTitle = lastadded.Item_Type;
                        if(lastadded.Item_Type.ToLower().Contains("elec") || lastadded.Item_Type.ToLower().Contains("elek") 
                            || lastadded.Item_Type.ToLower().Contains("tech") || lastadded.Item_Type.ToLower().Contains("tekn")
                            || lastadded.Item_Type.ToLower().Contains("driv") || lastadded.Item_Type.ToLower().Contains("chauf"))
                        {
                            newTitle.Worker = true;
                        }
                        else
                        {
                            newTitle.Worker = false;
                        }
                        _context.Titles.Add(newTitle);
                        await _context.SaveChangesAsync();
                    }
                }
                return RedirectToAction(nameof(Index), new { ProjectId = itemType.ProjectId });
            }
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["UnitId"] = new SelectList(_context.Units, "Id", "TheUnit");
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "Type");
            return View(itemType);
        }

        // GET: ItemTypes/Edit/5
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,International")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemType = await _context.ItemTypes.Include(x => x.Install_Unit).Include(x => x.Rental_Unit).Include(x=>x.Project).Where(x=>x.Id.Equals(id)).FirstAsync();
            if (itemType == null)
            {
                return NotFound();
            }
            if (User.IsInRole("International"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user.DivisionId.Equals(itemType.Project.DivisionId))
                {
                    ViewData["ProjectId"] = await GetProjectList();
                    ViewData["UnitId"] = new SelectList(_context.Units, "Id", "TheUnit");
                    return View(itemType);
                }
                else { return NotFound(); }
            }
            else
            {
                ViewData["ProjectId"] = await GetProjectList();
                ViewData["UnitId"] = new SelectList(_context.Units, "Id", "TheUnit");
                ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "Type");
                return View(itemType);
            }
            
            
        }
        [HttpGet]
        public async Task<decimal> GetRent(int theId)
        {
            var arrival = await _context.Arrivals.FindAsync(theId);
            var itemtype =  await _context.ItemTypes.FindAsync(arrival.ItemTypeId);
            return Convert.ToDecimal(itemtype.rental_price);
        }
        [HttpGet]
        public async Task<string> GetRentalUnit(int theId)
        {
            var arrival = await _context.Arrivals.FindAsync(theId);
            var itemtype = await _context.ItemTypes.Include(x=>x.Rental_Unit).Where(x=>x.Id.Equals(arrival.ItemTypeId)).SingleOrDefaultAsync();
            return itemtype.Rental_Unit.TheUnit;
        }
        // POST: ItemTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,International")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Item_Type,BoQnr,BoQnr_Rental,price,rental_price,Install_UnitId,Rental_UnitId,initial_cost,Daily_cost,ProjectId,Valuta,ReportTypeId,AgreementNumber,AgreementDate,MarkerPicture,ExpectedAmount,ExpectedAmountRental")] ItemType itemType)
        {
            if (id != itemType.Id)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                var previous = await _context.ItemTypes.AsNoTracking().SingleAsync(x => x.Id.Equals(id));
                if (previous.Rental_UnitId.Equals(5) || previous.Rental_UnitId.Equals(6))
                {
                    var title = await _context.Titles.SingleOrDefaultAsync(x => x.ItemTypeId.Equals(previous.Id));
                    if (title != null)
                    {
                        title.TheTitle = itemType.Item_Type;
                        _context.Update(title);
                    }
                }
                previous = null;
                try
                {
                    _context.Update(itemType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemTypeExists(itemType.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { ProjectId = itemType.ProjectId });
            }
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["UnitId"] = new SelectList(_context.Units, "Id", "TheUnit");
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "Type");
            return View(itemType);
        }

        // GET: ItemTypes/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemType = await _context.ItemTypes.Include(x=>x.Install_Unit).Include(x=>x.Rental_Unit)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (itemType == null)
            {
                return NotFound();
            }

            return View(itemType);
        }

        // POST: ItemTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var title = await _context.Titles.SingleOrDefaultAsync(x => x.ItemTypeId.Equals(id));
            if(title != null)
            {
                var daily_reports = await _context.Daily_Report_2s.Where(x => x.TitleId.Equals(title.Id)).ToListAsync();
                if(daily_reports.Count() > 0)
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "Daily reports exist with this BoQ/title. You cannto delete it." });
                }
                _context.Remove(title);
                await _context.SaveChangesAsync();
            }
            var itemType = await _context.ItemTypes.FindAsync(id);
            var logs = await _context.Log2s.Where(x => x.ItemTypeId.Equals(id)).ToListAsync();
            foreach(var log in logs)
            {
                log.ItemTypeId = null;
                log.Description = log.Description + " - ItemType removed from system: " + itemType.Item_Type + " on " + DateTime.Now.ToShortDateString();
                _context.Update(log);
            }
            await _context.SaveChangesAsync();
        
            _context.ItemTypes.Remove(itemType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemTypeExists(int id)
        {
            return _context.ItemTypes.Any(e => e.Id == id);
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,International")]
        public async Task<IActionResult> Combsearch(string searchstring, string filterchoice)
        {
            int f_c_converted;
            f_c_converted = Convert.ToInt32(filterchoice);
            ViewData["ChosenProjectId"] = f_c_converted;
            var theuser = await _userManager.GetUserAsync(HttpContext.User);
            ViewData["ProjectId"] = await GetProjectList();
            if (ModelState.IsValid)
            {
                List<ItemType> items = new List<ItemType>();
                if (string.IsNullOrEmpty(searchstring) && (string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
                {
                    return RedirectToAction(nameof(Index));
                }
                else if (string.IsNullOrEmpty(searchstring) && (!string.IsNullOrEmpty(filterchoice) || !filterchoice.Equals("All")))
                {
                            items = await _context.ItemTypes
                        .Include(m => m.Project).ThenInclude(x => x.Division)
                        .Include(x => x.Rental_Unit)
                        .Include(x => x.Install_Unit)
                       .Where(x => x.ProjectId.Equals(f_c_converted))
                       .OrderBy(x => x.BoQnr).ToListAsync();
                    

                }
                else if (!string.IsNullOrEmpty(searchstring) && (string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
                {

                        items = await _context.ItemTypes
                        .Include(m => m.Project).ThenInclude(x => x.Division)
                        .Include(x => x.Rental_Unit)
                        .Include(x => x.Install_Unit)
                        .Where(x => x.Item_Type.ToLower().Contains(searchstring.ToLower()))
                        .OrderBy(x => x.BoQnr).ToListAsync();
                    

                }
                else
                {

                        items = await _context.ItemTypes
                        .Include(m => m.Project).ThenInclude(x => x.Division)
                        .Include(x => x.Rental_Unit)
                        .Include(x => x.Install_Unit)
                        .Where(x => x.Item_Type.ToLower().Contains(searchstring.ToLower()) && x.ProjectId.Equals(f_c_converted))
                        .OrderBy(x => x.BoQnr).ToListAsync();

                }
                if(User.IsInRole("International") && !User.IsInRole("Admin"))
                {
                    return View(nameof(Index), items.Where(x=>x.Project.Name.Contains("STOCK")).OrderBy(x=>x.Item_Type).ThenBy(x=>x.Project.DivisionId));
                }
                if (!User.IsInRole("Admin"))
                {
                    items = items.Where(x => x.Project.DivisionId.Equals(theuser.DivisionId)).ToList();
                }
                return View(nameof(Index), items);
            }
            return View(nameof(Index));
        }
        public async Task<IEnumerable<SelectListItem>> createFilterlist()
        {

            List<Project> filternames = new List<Project>();
            var theuser = await _userManager.GetUserAsync(HttpContext.User);
            if (HttpContext.User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
            {

                filternames = await (from pr in _context.Projects
                                     join pu in _context.ProjectUsers on pr.Id
                                     equals pu.projectId
                                     where pu.userId == theuser.Id && theuser.DivisionId.Equals(pr.DivisionId)
                                     select pr).OrderBy(x => x.Name).ToListAsync();
            }
            else
            {
                if (User.IsInRole("Admin"))
                {
                    filternames = await _context.Projects.Include(x => x.Division).OrderBy(x => x.Division.Name).OrderBy(b => b.Name).ToListAsync();
                }
                else
                {
                    filternames = await _context.Projects.Where(x => x.DivisionId.Equals(theuser.DivisionId))
                .OrderBy(b => b.Name).ToListAsync();
                }

            }

            IEnumerable<SelectListItem> selList = from s in filternames
                                                  select new SelectListItem
                                                  {
                                                      Value = s.Id.ToString(),
                                                      Text = s.ProjectNr + " : " + s.Name
                                                  };
            return selList;
        }
        public async Task<JsonResult> AutoComplete(string search)
        {
            var user = await _userManager.GetUserAsync(User);
            List<ItemType> results = new List<ItemType>();
                if (User.IsInRole("Admin"))
                {
                    results = _context.ItemTypes
                                        .Include(x => x.Project).ThenInclude(x => x.Division)
                                        .Where(x => x.Item_Type.ToLower().Contains(search.ToLower()) || x.Project.Name.ToLower().Contains(search.ToLower())).ToList();
                }
                else
                {
                    results = _context.ItemTypes
                    .Include(x => x.Project).ThenInclude(x => x.Division)
                    .Where(x => x.Project.DivisionId.Equals(user.DivisionId) && x.Item_Type.ToLower().Contains(search.ToLower()) || x.Project.Name.ToLower().Contains(search.ToLower())).ToList();
                }


            return Json(results.Select(m => new
            {
                id = m.Id,
                value = m.Item_Type,
                label = m.Project.Name + '_' + m.Item_Type
            }).OrderBy(x => x.label));
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,Guest,International")]
        [HttpGet]
        public JsonResult GetItemsHeadLine(string theId)
        {
            int Id = Convert.ToInt32(theId);
            var thedata = (from it in _context.ItemTypes
                           join bqh in _context.BoQHeadLines
                           on Math.Floor(it.BoQnr) equals Math.Floor(bqh.BoQnum)
                           where bqh.Id.Equals(Id)
                           && it.ProjectId.Equals(bqh.ProjectId)
                           select it)
                           .OrderBy(x => x.BoQnr).ToList();
            return Json(thedata);
        }
        public JsonResult GetItem(string theId)
        {
            int Id = Convert.ToInt32(theId);
            var thedata = _context.ItemTypes.SingleOrDefault(x => x.Id.Equals(Id));
            return Json(thedata);
        }
        [Authorize(Roles = "Admin,Manager,DivisionAdmin")]
        public async Task<IActionResult> MakeDecom(int? id)
        {
            if(id != null)
            {
                var itemtype = await _context.ItemTypes.SingleOrDefaultAsync(x => x.Id.Equals(id));
                if(itemtype != null)
                {
                    itemtype.ReportTypeId = 1;
                    _context.Update(itemtype);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,Guest,International")]
        [HttpGet]
        public JsonResult GetItemsHeadLineRental(string theId)
        {
            int Id = Convert.ToInt32(theId);
            var thedata = (from it in _context.ItemTypes
                           join bqh in _context.BoQHeadLines
                           on Math.Floor(it.BoQnr_Rental.Value) equals Math.Floor(bqh.BoQnum)
                           where bqh.Id.Equals(Id)
                           && it.ProjectId.Equals(bqh.ProjectId)
                           select it)
                           .OrderBy(x => x.BoQnr).ToList();
            return Json(thedata);
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,Guest,International")]
        [HttpGet]
        public JsonResult GetDecomTypes(int? theId)
        {
            int Id = Convert.ToInt32(theId);
            List<ItemType> thedata = new List<ItemType>();
            thedata = _context.ItemTypes.Include(x=>x.ReportType).Where(x => x.ProjectId.Equals(Id) && x.ReportType.Type.Equals("Decommission")).ToList();
            return Json(thedata);
        }
        public async Task<FileResult> DownloadMarkerImage(int id)
        {
            var path = _env.WebRootPath + "\\Markers\\" + id.ToString() + ".jpg";
            MemoryStream memory = new MemoryStream();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "image/jpg", "layer_" + id.ToString() + ".jpg");
        }
        public async Task<IActionResult> UploadMarkerImage(int id, IFormFile photo)
        {
            var directory = _env.WebRootPath + "\\Markers\\";
            var item = await _context.ItemTypes.FindAsync(id);
            if (!Directory.Exists(directory) && photo != null)
            {
                Directory.CreateDirectory(directory);
            }
            if (photo != null)
            {
                var path = Path.Combine(directory, String.Concat(id.ToString(), ".jpg"));
                item.MarkerPicture = "https://hj-mainops.com/ItemTypes/DownloadMarkerImage/" + id.ToString();// + ".jpg";
                _context.Update(item);
                var stream = new FileStream(path, FileMode.Create);
                await photo.CopyToAsync(stream);
                await _context.SaveChangesAsync();
                stream.Close();

            }
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,Guest,International")]
        [HttpGet]
        public JsonResult JsonWellTypes(string theId)
        {
            int Id = Convert.ToInt32(theId);
            List<WellType> thedata = _context.WellTypes.Where(x => x.ProjectId.Equals(Id)).ToList();
            
            return Json(thedata);
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,Guest,International")]
        [HttpGet]
        public JsonResult GetItemsProject(string theId,string type)
        {
            int Id = Convert.ToInt32(theId);
            List<ItemType> thedata = new List<ItemType>();
            List<ItemType> thenewdata = new List<ItemType>();
            switch (type)
            {
                case "install":
                    //thedata = (from it in _context.ItemTypes 
                    //           where 
                    //           it.ProjectId.Equals(Id)
                    //           && !it.Item_Type.ToLower().Contains("discount") && !it.Item_Type.ToLower().Contains(" idle") && !it.Item_Type.ToLower().Contains(" idxle") && !it.Item_Type.ToLower().Contains(" excluded grouting l") && !it.Item_Type.ToLower().Contains("decommission")
                    //           && (!it.Rental_UnitId.Equals(5) && !it.Rental_UnitId.Equals(6)) && it.ReportTypeId == null
                    //           select it).OrderBy(x => x.BoQnr).ToList();
                    thedata = _context.ItemTypes.Where(x => x.ProjectId.Equals(Id)).ToList();
                    thedata = thedata.Where(x => !x.Item_Type.ToLower().Contains("discount") && !x.Item_Type.ToLower().Contains(" idle") && !x.Item_Type.ToLower().Contains(" idxle") && !x.Item_Type.ToLower().Contains(" excluded grouting l") && !x.Item_Type.ToLower().Contains("decommission")).ToList();
                    thedata = thedata.Where(x => x.price > 0 && !x.Rental_UnitId.Equals(5) && !x.Rental_UnitId.Equals(6) && !x.Rental_UnitId.Equals(19) && !x.Install_UnitId.Equals(19) && x.ReportTypeId == null).ToList();
                    var boqheadlines =_context.BoQHeadLines.Where(x => x.ProjectId.Equals(Id) && x.Type.ToLower().Equals("installation")).ToList();
                    foreach(var item in thedata)
                    {
                        if(boqheadlines.Select(x => Convert.ToInt32(Math.Floor(x.BoQnum))).Distinct().Contains(Convert.ToInt32(Math.Floor(item.BoQnr))))
                        {
                            thenewdata.Add(item);
                        }
                    }
                    thedata = thenewdata.OrderBy(x => x.Item_Type).ThenBy(x => x.BoQnr).ToList();
                    //thedata = (from it in _context.ItemTypes
                    //           join bqh in _context.BoQHeadLines on Convert.ToInt32(it.BoQnr) equals Convert.ToInt32(bqh.BoQnum)
                    //           where bqh.Type.Equals("Installation") && it.ProjectId.Equals(Id) && bqh.ProjectId.Equals(Id)
                    //            && !it.Item_Type.ToLower().Contains("discount") && !it.Item_Type.ToLower().Contains(" idle") && !it.Item_Type.ToLower().Contains(" idxle") && !it.Item_Type.ToLower().Contains(" excluded grouting l") && !it.Item_Type.ToLower().Contains("decommission")
                    //           && (!it.Rental_UnitId.Equals(5) && !it.Rental_UnitId.Equals(6)) && it.ReportTypeId == null
                    //           select it).OrderBy(x => x.BoQnr).ToList();
                    foreach (var item in thedata)
                    {
                        item.Item_Type = _localizer.GetLocalizedHtmlString(item.Item_Type);
                    }
                    return Json(thedata);
                case "installedit":
                  
                    thedata = (from it in _context.ItemTypes
                               join bqh in _context.BoQHeadLines on Convert.ToInt32(Math.Floor(it.BoQnr)) equals Convert.ToInt32(Math.Floor(bqh.BoQnum))
                               where bqh.Type.Equals("Installation") && it.ProjectId.Equals(Id) && bqh.ProjectId.Equals(Id)
                                && !it.Item_Type.ToLower().Contains("discount") && !it.Item_Type.ToLower().Contains(" idle") && !it.Item_Type.ToLower().Contains(" idxle") && !it.Item_Type.ToLower().Contains(" excluded grouting l")
                               && (!it.Rental_UnitId.Equals(5) && !it.Rental_UnitId.Equals(6))
                               select it).OrderBy(x => x.Item_Type).ThenBy(x => x.BoQnr).ToList();
                    foreach (var item in thedata)
                    {
                        item.Item_Type = _localizer.GetLocalizedHtmlString(item.Item_Type);
                    }
                    return Json(thedata);
                case "pump":

                    thedata = (from it in _context.ItemTypes
                               where it.ProjectId.Equals(Id) && it.ReportTypeId.Equals(20)
                           
                               select it).OrderBy(x => x.Item_Type).ThenBy(x => x.BoQnr).ToList();
                    return Json(thedata);
                case "reinf":

                    thedata = (from it in _context.ItemTypes
                               where it.ProjectId.Equals(Id) && it.ReportTypeId.Equals(21)

                               select it).OrderBy(x => x.Item_Type).ThenBy(x => x.BoQnr).ToList();
                    return Json(thedata);
                case "obs":

                    thedata = (from it in _context.ItemTypes
                               where it.ProjectId.Equals(Id) && it.ReportTypeId.Equals(22)

                               select it).OrderBy(x => x.BoQnr).ToList();
                    return Json(thedata);
                case "mobilize":
                    thedata = (from it in _context.ItemTypes join bqh in _context.BoQHeadLines on Convert.ToInt32(it.BoQnr) equals Convert.ToInt32(bqh.BoQnum) where bqh.Type.Equals("Mobilization") 
                               && it.ProjectId.Equals(Id) && bqh.ProjectId.Equals(Id) select it).OrderBy(x => x.Item_Type).ThenBy(x => x.BoQnr).ToList();
                    
                    return Json(thedata);
                case "arrival":
                    if(User.IsInRole("International") && !User.IsInRole("Admin"))
                    {
                        thedata = (from it in _context.ItemTypes
                                   .OrderBy(x => x.Item_Type).ThenBy(x => x.BoQnr)
                                    join bqh in _context.BoQHeadLines on Convert.ToInt32(Math.Floor(it.BoQnr)) equals Convert.ToInt32(Math.Floor(bqh.BoQnum)) where bqh.Type.Equals("Installation") && it.ProjectId.Equals(Id) && bqh.ProjectId.Equals(Id) select it).ToList();
                    }
                    else
                    {
                        List<ItemType> thedata2 = new List<ItemType>();
                        thedata = (from it in _context.ItemTypes
                                             where it.ProjectId.Equals(Id)
                                             && it.ReportTypeId == null
                                             && !it.Item_Type.ToLower().Contains("discount") 
                                             && !it.Item_Type.ToLower().Contains(" idle")
                                             && (it.rental_price > (decimal)0.01 || it.rental_price < (decimal)-0.01)
                                             && (it.Rental_UnitId.Equals(3) || it.Rental_UnitId.Equals(4) || it.Rental_UnitId.Equals(11) || it.Rental_UnitId.Equals(12) || it.Rental_UnitId.Equals(13) || it.Rental_UnitId.Equals(14) || it.Rental_UnitId.Equals(16) || it.Rental_UnitId.Equals(17) || it.Rental_UnitId.Equals(18))
                                   select it).OrderBy(x => x.Item_Type).ThenBy(x => x.BoQnr_Rental).ToList();
                    }
                    
                    
                    return Json(thedata);
                case "arrivaledit":

                    thedata = (from it in _context.ItemTypes
                               join bqh in _context.BoQHeadLines on Math.Floor(Math.Floor(it.BoQnr_Rental.Value)) equals Math.Floor(Math.Floor(bqh.BoQnum))
                               where bqh.Type.Equals("Rental") && it.ProjectId.Equals(Id) && bqh.ProjectId.Equals(Id)
                                && !it.Item_Type.ToLower().Contains("discount") && !it.Item_Type.ToLower().Contains(" idle") && !it.Item_Type.ToLower().Contains(" idxle")
                                && (it.rental_price > (decimal)0.01 || it.rental_price < (decimal)-0.01)
                                && (it.Rental_UnitId.Equals(3) || it.Rental_UnitId.Equals(4) || it.Rental_UnitId.Equals(11) || it.Rental_UnitId.Equals(12) || it.Rental_UnitId.Equals(13) || it.Rental_UnitId.Equals(14) || it.Rental_UnitId.Equals(16) || it.Rental_UnitId.Equals(17) || it.Rental_UnitId.Equals(18))
                               select it).OrderBy(x => x.Item_Type).ThenBy(x => x.BoQnr_Rental).ToList();
                    foreach (var item in thedata)
                    {
                        item.Item_Type = _localizer.GetLocalizedHtmlString(item.Item_Type);
                    }
                    return Json(thedata);
                case "deinstall":
                   
                    thedata = _context.ItemTypes.Where(x => x.ProjectId.Equals(Id)).ToList();
                    thedata = thedata.Where(x => !x.Item_Type.ToLower().Contains("discount") && !x.Item_Type.ToLower().Contains(" idle") && !x.Item_Type.ToLower().Contains(" idxle") && !x.Item_Type.ToLower().Contains(" excluded grouting l") && !x.Item_Type.ToLower().Contains("decommission")).ToList();
                    thedata = thedata.Where(x => x.price > 0 && !x.Rental_UnitId.Equals(5) && !x.Rental_UnitId.Equals(6) && !x.Rental_UnitId.Equals(19) && !x.Install_UnitId.Equals(19)).ToList();
                    var boqheadlines2 = _context.BoQHeadLines.Where(x => x.ProjectId.Equals(Id) && x.Type.ToLower().Equals("installation")).ToList();
                    foreach (var item in thedata)
                    {
                        if (boqheadlines2.Select(x => Convert.ToInt32(Math.Floor(x.BoQnum))).Distinct().Contains(Convert.ToInt32(Math.Floor(item.BoQnr))))
                        {
                            thenewdata.Add(item);
                        }
                    }
                    thedata = thenewdata.OrderBy(x => x.Item_Type).ThenBy(x => x.BoQnr).ToList();
                    foreach (var item in thedata)
                    {
                        item.Item_Type = _localizer.GetLocalizedHtmlString(item?.Item_Type);
                    }
                    return Json(thedata);
                case "hours":
                    thedata = (from it in _context.ItemTypes
                               where
                               it.ProjectId.Equals(Id)
                               && !it.Item_Type.ToLower().Contains("discount") && !it.Item_Type.ToLower().Contains(" idle") && !it.Item_Type.ToLower().Contains(" idxle") && !it.Item_Type.ToLower().Contains(" excluded grouting l")
                               && (it.Rental_UnitId.Equals(5) || it.Rental_UnitId.Equals(6))
                               select it).OrderBy(x => x.Item_Type).ThenBy(x => x.BoQnr).ToList(); 
                    return Json(thedata);
                case "drill":
                    thedata = (from it in _context.ItemTypes where it.ProjectId.Equals(Id) && it.ReportTypeId.Equals(13) select it).OrderBy(x => x.Item_Type).ThenBy(x => x.BoQnr).ToList();
                    return Json(thedata);
                case "decom":
                    thedata = (from it in _context.ItemTypes join di in _context.DecommissionableItems on it.Id equals di.InstalledItemTypeId where it.ProjectId.Equals(Id) && it.ReportTypeId.Equals(1) select it).OrderBy(x => x.Item_Type).ThenBy(x => x.BoQnr).ToList();
                    return Json(thedata);
                default:
                    thedata = _context.ItemTypes.Where(x => x.ProjectId.Equals(Id)).OrderBy(x => x.Item_Type).ThenBy(x => x.BoQnr).ToList();
                    return Json(thedata);
            }
        }
        
        [HttpGet]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,International")]
        public async Task<IActionResult> UploadDataSheets(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            UploadDataSheetsVM model = new UploadDataSheetsVM();
            if(id != null)
            {
                model.id = Convert.ToInt32(id);
            }
            List<ItemType> itemtypes = new List<ItemType>();
            if (User.IsInRole("International") && !User.IsInRole("Admin"))
            {
                itemtypes =  await (from it in _context.ItemTypes.Include(x => x.Project).ThenInclude(x => x.Division)
                               join bqh in _context.BoQHeadLines on Math.Floor(it.BoQnr) equals Math.Floor(bqh.BoQnum)
                               where !bqh.Type.Equals("Mobilization") 
                               && !bqh.Type.Equals("Hours")
                               && bqh.ProjectId.Equals(it.ProjectId)
                               && !it.Item_Type.ToLower().Contains("discount")
                               && it.Project.Name.Contains("STOCK")
                               && it.Project.DivisionId.Equals(user.DivisionId)
                               select it).OrderBy(x => x.Project.Division.Name).ThenBy(x => x.Item_Type).ToListAsync();
            }
            else
            {
                if (User.IsInRole("Admin"))
                {
                    itemtypes = await (from it in _context.ItemTypes.Include(x => x.Project).ThenInclude(x => x.Division)
                                       join bqh in _context.BoQHeadLines on Math.Floor(it.BoQnr) equals Math.Floor(bqh.BoQnum)
                                       where !bqh.Type.Equals("Mobilization")
                                       && !bqh.Type.Equals("Hours")
                                       && bqh.ProjectId.Equals(it.ProjectId)
                                       && !it.Item_Type.ToLower().Contains("discount")
                                       select it).OrderBy(x => x.Project.Division.Name).ThenBy(x => x.Project.Name).ThenBy(x => x.BoQnr).ToListAsync();
                }
                else
                {
                    itemtypes = await (from it in _context.ItemTypes.Include(x => x.Project).ThenInclude(x => x.Division)
                                       join bqh in _context.BoQHeadLines on Math.Floor(it.BoQnr) equals Math.Floor(bqh.BoQnum)
                                       where !bqh.Type.Equals("Mobilization")
                                       && !bqh.Type.Equals("Hours")
                                       && bqh.ProjectId.Equals(it.ProjectId)
                                       && !it.Item_Type.ToLower().Contains("discount")
                                       && it.Project.DivisionId.Equals(user.DivisionId)
                                       select it).OrderBy(x => x.Project.Division.Name).ThenBy(x => x.Project.Name).ThenBy(x => x.BoQnr).ToListAsync();
                }
            }
            itemtypes = itemtypes.GroupBy(test => test.Id)
               .Select(grp => grp.First())
               .ToList();
            IEnumerable<SelectListItem> selList = from s in itemtypes
                                                  select new SelectListItem
                                                  {
                                                      Value = s.Id.ToString(),
                                                      Text = s.Project.Name + " : " + String.Format("{0:0.##}", s.BoQnr) + " : " + s.Item_Type
                                                  };
            ViewData["ItemTypeId"] = selList;
            return View(model);



        }
        [HttpGet]
        public ActionResult ShowFilePdf(string path)
        {
            if (System.IO.File.Exists(path)) {
                Response.Headers.Add("Content-Disposition", "inline; filename=results.pdf");
                string[] pathparts = path.Split("\\");
                string realpath = "~/AHAK/DataSheets/" + pathparts[pathparts.Length - 2] + "/" + pathparts[pathparts.Length - 1];
                return File(realpath, "application/pdf");
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet]
        public ActionResult ShowFileJpg(string path)
        {
            if (System.IO.File.Exists(path))
            {
                Response.Headers.Add("Content-Disposition", "inline; filename=results.jpg");
                string[] pathparts = path.Split("\\");
                string realpath = "~/AHAK/DataSheets/" + pathparts[pathparts.Length - 2] + "/" + pathparts[pathparts.Length - 1];
                return File(realpath, "image/jpeg");
            }
            else
            {
                return NotFound();
            }
            
        }
        [HttpGet]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,International")]
        public IActionResult ShowDataSheets(int? id)
        {
            if(id != null)
            {
                FileListVM model = new FileListVM();
                var directory = _env.WebRootPath + "\\AHAK\\DataSheets\\" + id.ToString() + "\\";
                List<string> pictures = new List<string>();
                if (Directory.Exists(directory))
                {
                    var folder = Directory.EnumerateFiles(directory)
                                     .Select(fn => Path.GetFileName(fn));

                    foreach (string file in folder)
                    {
                        pictures.Add(directory + file);
                    }
                    model.pictures = pictures;
                }
                return PartialView("_ShowDataSheets",model);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        [RequestSizeLimit(900000000)]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,International")]
        public async Task<IActionResult> UploadDataSheets(int? id, IFormFile[] files)
        {
            if(id != null)
            {
                var item = await _context.ItemTypes.Include(x=>x.Project).Where(x => x.Id.Equals(id)).SingleOrDefaultAsync();
                var user = await _userManager.GetUserAsync(User);
                if (item.Project.DivisionId.Equals(user.DivisionId))
                { 
                    var directory = _env.WebRootPath + "\\AHAK\\DataSheets\\" + id.ToString() + "\\";
                    if (!Directory.Exists(directory) && files != null)
                    {
                        Directory.CreateDirectory(directory);
                    }
                    foreach (IFormFile fil in files)
                    {
                        var path = Path.Combine(directory, fil.FileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await fil.CopyToAsync(stream);
                        }
                    }
                    List<ItemType> itemtypes = new List<ItemType>();
                    if (User.IsInRole("International") && !User.IsInRole("Admin"))
                    {
                        itemtypes = await (from it in _context.ItemTypes.Include(x => x.Project).ThenInclude(x => x.Division)
                                           join bqh in _context.BoQHeadLines on Math.Floor(it.BoQnr) equals Math.Floor(bqh.BoQnum)
                                           where !bqh.Type.Equals("Mobilization")
                                           && !bqh.Type.Equals("Hours")
                                           && bqh.ProjectId.Equals(it.ProjectId)
                                           && !it.Item_Type.ToLower().Contains("discount")
                                           && it.Project.Name.Contains("STOCK")
                                           && it.Project.DivisionId.Equals(user.DivisionId)
                                           select it).OrderBy(x => x.Project.Division.Name).ThenBy(x => x.Project.Name).ThenBy(x => x.BoQnr).ToListAsync();
                    }
                    else
                    {
                        if (User.IsInRole("Admin"))
                        {
                            itemtypes = await (from it in _context.ItemTypes.Include(x => x.Project).ThenInclude(x => x.Division)
                                               join bqh in _context.BoQHeadLines on Math.Floor(it.BoQnr) equals Math.Floor(bqh.BoQnum)
                                               where !bqh.Type.Equals("Mobilization")
                                               && !bqh.Type.Equals("Hours")
                                               && bqh.ProjectId.Equals(it.ProjectId)
                                               && !it.Item_Type.ToLower().Contains("discount")
                                               select it).OrderBy(x => x.Project.Division.Name).ThenBy(x => x.Project.Name).ThenBy(x => x.BoQnr).ToListAsync();
                        }
                        else
                        {
                            itemtypes = await (from it in _context.ItemTypes.Include(x => x.Project).ThenInclude(x => x.Division)
                                               join bqh in _context.BoQHeadLines on Math.Floor(it.BoQnr) equals Math.Floor(bqh.BoQnum)
                                               where !bqh.Type.Equals("Mobilization")
                                               && !bqh.Type.Equals("Hours")
                                               && bqh.ProjectId.Equals(it.ProjectId)
                                               && !it.Item_Type.ToLower().Contains("discount")
                                               && it.Project.DivisionId.Equals(user.DivisionId)
                                               select it).OrderBy(x => x.Project.Division.Name).ThenBy(x => x.Project.Name).ThenBy(x => x.BoQnr).ToListAsync();
                        }
                    }
                    itemtypes = itemtypes.GroupBy(test => test.Id)
                       .Select(grp => grp.First())
                       .ToList();
                    IEnumerable<SelectListItem> selList = from s in itemtypes
                                                          select new SelectListItem
                                                          {
                                                              Value = s.Id.ToString(),
                                                              Text = s.Project.Name + " : " + String.Format("{0:0.##}", s.BoQnr) + " : " + s.Item_Type
                                                          };
                    ViewData["ItemTypeId"] = selList;
                    return View();
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }
        }
        [RequestSizeLimit(900000000)]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,International")]
        public static void SaveAndCompressJpeg(string inputPath, int qualityIn)
        {

            int size = 300;
            int quality = qualityIn;
            string[] fileparts = inputPath.Split(".");
            string path2 = fileparts[0] + "_edit." + fileparts[1];
            using (var image = new Bitmap(System.Drawing.Image.FromFile(inputPath)))
            {
                int width, height;
                if (image.Width > image.Height)
                {
                    width = size;
                    height = Convert.ToInt32(image.Height * size / (double)image.Width);
                }
                else
                {
                    width = Convert.ToInt32(image.Width * size / (double)image.Height);
                    height = size;
                }
                var resized = new Bitmap(width, height);
                using (var graphics = Graphics.FromImage(resized))
                {
                    graphics.CompositingQuality = CompositingQuality.HighSpeed;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.DrawImage(image, 0, 0, width, height);
                    using (var output = System.IO.File.Open(path2, FileMode.Create))
                    {
                        var qualityParamId = System.Drawing.Imaging.Encoder.Quality;
                        var encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0] = new EncoderParameter(qualityParamId, quality);
                        var codec = ImageCodecInfo.GetImageDecoders().FirstOrDefault(codec2 => codec2.FormatID == System.Drawing.Imaging.ImageFormat.Jpeg.Guid);
                        resized.Save(output, codec, encoderParameters);
                    }
                }
            }
        }

        /// Returns the image codec with the given mime type
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        }
    }
}
