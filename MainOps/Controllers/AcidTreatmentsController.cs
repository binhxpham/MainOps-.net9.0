using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MainOps.Data;
using MainOps.Models;
using MainOps.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Rotativa.AspNetCore;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember")]
    public class AcidTreatmentsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AcidTreatmentsController(DataContext context,UserManager<ApplicationUser> userManager) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: AcidTreatments
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var dataContext = _context.AcidTreatments.Include(a => a.Project).Include(a => a.SubProject).Where(x => x.Project.DivisionId.Equals(user.DivisionId)).OrderByDescending(x => x.Report_Date);
            return View(await dataContext.ToListAsync());
        }

        public async Task<IActionResult> UploadData(int? id)
        {
            if(id != null)
            {
                var user = await _userManager.GetUserAsync(User);
                var at = await _context.AcidTreatments.Include(x => x.Project).SingleOrDefaultAsync(x => x.Id.Equals(id));
                if(at.Project.DivisionId != user.DivisionId && !User.IsInRole("Admin"))
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this resource" });
                }
                AcidDataUploadVM model = new AcidDataUploadVM();
                model.AcidTreatmentId = Convert.ToInt32(id);
                return View("UploadAcidData",model);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        public async Task<IActionResult> UploadAcidTreatmentData(AcidDataUploadVM model, IFormFile postedFile)
        {
            if (postedFile != null)
            {
                string fileExtension = Path.GetExtension(postedFile.FileName);

                //Validate uploaded file and return error.
                if (fileExtension != ".csv")
                {
                    return NotFound();
                }
                using (var sreader = new StreamReader(postedFile.OpenReadStream()))
                {
                    while (!sreader.EndOfStream)
                    {
                        AcidData datapoint = new AcidData();
                        string[] rows = sreader.ReadLine().Split(',');
                        if (rows[0] != "")
                        {
                            try
                            {
                                datapoint.TimeStamp = Convert.ToDateTime(rows[0]); //expects yyyy-MM-dd HH:mm:ss.ffffff (local time)
                            }
                            catch
                            {
                                try
                                {
                                    datapoint.TimeStamp = DateTime.ParseExact(rows[0].Replace(",", "."), "yyyy-MM-dd HH:mm:ss.ffffff zzz", null);
                                }
                                catch
                                {
                                    try
                                    {
                                        datapoint.TimeStamp = DateTime.ParseExact(rows[0].Replace(",", "."), "dd-MM-yyyy HH:mm:ss.fff", null);
                                    }
                                    catch
                                    {
                                        try
                                        {
                                            datapoint.TimeStamp = DateTime.ParseExact(rows[0].Replace(",", "."), "dd-MM-yy HH:mm:ss", null);
                                        }
                                        catch
                                        {
                                            RedirectToAction("ErrorMessage", "Home", new { text = "Please provide the date time in one of the following formats {yyyy-MM-dd HH:mm:ss.ffffff ; yyyy-MM-dd HH:mm:ss.ffffff zzz ; dd-MM-yyyy HH:mm:ss.fff ; dd-MM-yy HH:mm:ss" });
                                        }

                                    }
                                }
                                
                            }
                            
                            
                            //datapoint.Level = Convert.ToDouble(rows[1]);
                            datapoint.Flow = Convert.ToDouble(rows[3]);
                            datapoint.m3_total = Convert.ToDouble(rows[1]);
                            //datapoint.Min_Counter_Water = Convert.ToDouble(rows[4]);
                            //datapoint.Min_Counter_HoseCounter = Convert.ToDouble(rows[5]);
                            //datapoint.Set_Dosing_Percent = Convert.ToDouble(rows[6]);
                            //datapoint.Set_Dosing_Time = Convert.ToDouble(rows[7]);
                            //datapoint.Set_Dosing_m3 = Convert.ToDouble(rows[8]);
                            datapoint.Acid_m3_total = Convert.ToDouble(rows[2]);
                            //datapoint.Hour_Counter_HoseCounter = Convert.ToDouble(rows[10]);
                            //datapoint.Hour_Counter_Water = Convert.ToDouble(rows[11]);
                            //datapoint.Pressure = Convert.ToDouble(rows[12]);
                            datapoint.AcidTreatmentId = model.AcidTreatmentId;
                            _context.Add(datapoint);
                        }
                        //Aktuel_Niveau_Boring
                        //Boring_aktuel_flow
                        //Boring_m3_total
                        //Min_Tæller_Vand_Pumpe
                        //Min_Tæller_slangepumpe
                        //Set_Dosering_Procent
                        //Set_Dosering_Tid
                        //Set_Dosering_m3
                        //Syre_m3_total
                        //Time_Tæller_Slangepumpe
                        //Time_Tæller_Vand_Pumpe
                        //Tryk_Boring
                    }
                    await _context.SaveChangesAsync();
                }
            }

            //return RedirectToAction("Groutings");
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> JsonDataAcidTreatment(int id)
        {
            var datalist = new List<PumpTestDataDevice>();
            var user = await _userManager.GetUserAsync(User);
            var acidtreatment = await _context.AcidTreatments.Include(x => x.Project).Include(x => x.TestData).SingleOrDefaultAsync(x => x.Id.Equals(id));
            if (!acidtreatment.Project.DivisionId.Equals(user.DivisionId)) {
                return Json(datalist);
            }
            
            //foreach (var item in acidtreatment.TestData)
            //{
            //    AcidData dat = new AcidData();
            //    dat.Id = item.Id;
            //    dat.TimeStamp = item.TimeStamp;
            //    dat.PumpLevelData = item.PumpLevelData;
            //    dat.Moni1LevelData = item.Moni1LevelData;
            //    dat.Moni2LevelData = item.Moni2LevelData;
            //    dat.Moni3LevelData = item.Moni3LevelData;
            //    dat.Moni4LevelData = item.Moni4LevelData;
            //    dat.FlowData = item.FlowData;

            //    datalist.Add(dat);
            //}
            //var data = datalist.ToArray();
            return Json(acidtreatment.TestData.OrderBy(x => x.TimeStamp));
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AcidTreatmentReport(int? id)
        {
            if (id != null)
            {
                var user = await _userManager.GetUserAsync(User);
                var test = await _context.AcidTreatments.Include(x => x.Project).ThenInclude(x => x.Division)
                    .Include(x => x.SubProject)
                    .Include(x => x.TestData).Where(x => x.Id.Equals(id)).SingleOrDefaultAsync();
                if (!user.DivisionId.Equals(test.Project.DivisionId))
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this reprot" });
                }
                return View("_AcidTreatment", test);
            }
            else { return NotFound(); }
        }
        [HttpPost]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember")]
        public async Task<IActionResult> AcidTreatmentReport_PDF(int? id, string theimage)
        {
            if (id != null)
            {
                var user = await _userManager.GetUserAsync(User);
                var acidtreatment = await _context.AcidTreatments.Include(x => x.TestData).Include(x => x.Project).ThenInclude(x => x.Division).Include(x => x.SubProject).Where(x => x.Id.Equals(id)).SingleOrDefaultAsync();
                if (!user.DivisionId.Equals(acidtreatment.Project.DivisionId))
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this reprot" });
                }
                acidtreatment.imagepath = theimage;
                _context.Update(acidtreatment);
                await _context.SaveChangesAsync();
                return new ViewAsPdf("_AcidTreatmentPDF", acidtreatment);
            }
            else { return NotFound(); }
        }
        // GET: AcidTreatments/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var acidTreatment = await _context.AcidTreatments
        //        .Include(a => a.Project)
        //        .Include(a => a.SubProject)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (acidTreatment == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(acidTreatment);
        //}

        // GET: AcidTreatments/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
            return View();
        }

        // POST: AcidTreatments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,SubProjectId,Wellname,MeasPointId,Report_Date,Water_Meter_Before,starttime,endtime,Ref_Level,Water_Meter_After,Bottom_well,Water_level,Coments,DoneBy,Total_m3_acid_Manual")] AcidTreatment acidTreatment)
        {
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
               
                acidTreatment.DoneBy = user.full_name();
                _context.Add(acidTreatment);
                await _context.SaveChangesAsync();
                var lastadded = await _context.AcidTreatments.LastAsync();
                var itemtype = await _context.ItemTypes.Include(x => x.ReportType).Where(x => x.ProjectId.Equals(lastadded.ProjectId) && x.ReportType.Type.Equals("AcidTreatment")).SingleOrDefaultAsync();

                var mp = await _context.MeasPoints.SingleOrDefaultAsync(x => x.Id.Equals(acidTreatment.MeasPointId));

                if (mp.Lati != null && mp.Longi != null)
                {
                    if (itemtype != null)
                    {
                        Install inst = new Install
                        {
                            ToBePaid = true,
                            ItemTypeId = itemtype.Id,
                            Latitude = Convert.ToDouble(mp.Lati),
                            Longitude = Convert.ToDouble(mp.Longi),
                            TimeStamp = lastadded.Report_Date,
                            InvoiceDate = DateTime.Now,
                            RentalStartDate = lastadded.Report_Date,
                            Install_Text = lastadded.Comments,
                            UniqueID = "AcidTreatment : " + mp.Name,
                            isInstalled = true,
                            Amount = 1,
                            ProjectId = lastadded.ProjectId,
                            SubProjectId = lastadded.SubProjectId,
                            EnteredIntoDataBase = DateTime.Now,
                            LastEditedInDataBase = DateTime.Now,
                            DoneBy = user.full_name()
                        };
                        _context.Installations.Add(inst);
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    if (itemtype != null)
                    {
                        Install inst = new Install
                        {
                            ToBePaid = true,
                            ItemTypeId = itemtype.Id,
                            Latitude = 0,
                            Longitude = 0,
                            TimeStamp = lastadded.Report_Date,
                            InvoiceDate = DateTime.Now,
                            RentalStartDate = lastadded.Report_Date,
                            Install_Text = lastadded.Comments,
                            UniqueID = "AcidTreatment : " + mp.Name,
                            isInstalled = true,
                            Amount = 1,
                            ProjectId = lastadded.ProjectId,
                            SubProjectId = lastadded.SubProjectId,
                            EnteredIntoDataBase = DateTime.Now,
                            LastEditedInDataBase = DateTime.Now,
                            DoneBy = user.full_name()
                        };
                        _context.Installations.Add(inst);
                        await _context.SaveChangesAsync();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name", acidTreatment.SubProjectId);
            return View(acidTreatment);
        }

        // GET: AcidTreatments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var acidTreatment = await _context.AcidTreatments.Include(x => x.TestData).Include(x => x.Project).SingleOrDefaultAsync(x=> x.Id.Equals(id));
           
            if (acidTreatment == null)
            {
                return NotFound();
            }
            if (!user.DivisionId.Equals(acidTreatment.Project.DivisionId))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this reprot" });
            }
            ViewData["ProjectId"] = await GetProjectList();
            //ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Id", acidTreatment.SubProjectId);
            return View(acidTreatment);
        }

        // POST: AcidTreatments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,SubProjectId,Wellname,MeasPointId,Report_Date,Water_Meter_Before,starttime,,Ref_Level,endtime,Water_Meter_After,Bottom_well,Water_level,Coments,DoneBy,Total_m3_acid_Manual")] AcidTreatment acidTreatment)
        {
            if (id != acidTreatment.Id)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(acidTreatment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AcidTreatmentExists(acidTreatment.Id))
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
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name", acidTreatment.SubProjectId);
            return View(acidTreatment);
        }

        // GET: AcidTreatments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var acidTreatment = await _context.AcidTreatments
                .Include(a => a.Project)
                .Include(a => a.SubProject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (acidTreatment == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (!user.DivisionId.Equals(acidTreatment.Project.DivisionId))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this reprot" });
            }

            return View(acidTreatment);
        }

        // POST: AcidTreatments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var acidTreatment = await _context.AcidTreatments.FindAsync(id);
            _context.AcidTreatments.Remove(acidTreatment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AcidTreatmentExists(int id)
        {
            return _context.AcidTreatments.Any(e => e.Id == id);
        }
    }
}
