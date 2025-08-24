using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MainOps.Data;
using MainOps.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,DivisionAdmin")]
    public class DataPointsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DataPointsController(DataContext context,UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpPost]
        public async Task<ActionResult> Index(IFormFile postedFile)
        {
            if (postedFile != null)
            {
                try
                {
                    string fileExtension = Path.GetExtension(postedFile.FileName);

                    //Validate uploaded file and return error.
                    if (fileExtension != ".csv")
                    {
                        ViewBag.Message = "Please select the csv file with .csv extension";
                        return View("Index");
                    }

                    int mpId = 0;
                    var datapoints = new List<DataPoint>();
                    var user = await _userManager.GetUserAsync(User);
                    using (var sreader = new StreamReader(postedFile.OpenReadStream()))
                    {
                        //First line is header. If header is not passed in csv then we can neglect the below line.
                        string[] headers = sreader.ReadLine().Split(';');
                        string varname = headers[1].Split('@')[1].Trim();
                        var themeaspoint = await _context.MeasPoints.Include(x=>x.Project).ThenInclude(x=>x.Division).Where(x =>x.Project.Division.Id.Equals(user.DivisionId) && x.Name.ToLower().Equals(varname.ToLower())).FirstAsync();
                        //Loop through the records
                        while (!sreader.EndOfStream)
                        {
                            try
                            {
                                string[] rows = sreader.ReadLine().Split(';');
                                mpId = themeaspoint.Id;
                                if(rows[1] != "")
                                {
                                    
                                    DataPoint DP = new DataPoint
                                    {
                                        MeasPointId = themeaspoint.Id,
                                        MessWert = Convert.ToDouble(rows[1]),
                                        Datum =  Convert.ToDateTime(rows[0])
                                };
                                    _context.DataPoints.Add(DP);
                                }
                                
                            }
                            catch
                            {
                                
                            }
                           
                        }
                        await _context.SaveChangesAsync();

                    }
                    var datapoints2 = _context.DataPoints.Where(x => x.MeasPointId.Equals(mpId)).OrderByDescending(x => x.Id).Take(10).ToList();
                    return View("Index", datapoints2);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
            }
            else
            {
                ViewBag.Message = "Please select the file first to upload.";
            }
            return View();
        }
        [HttpPost]
        public async Task<int> AddMeasurement(double thevalue, DateTime thetime,int theid)
        {
            var measpoint = await _context.MeasPoints.FindAsync(theid);
            Offset theoffset = new Offset();
            var theoffsets = await _context.Offsets.Where(x => x.MeasPointId.Equals(measpoint.Id)).OrderByDescending(x => x.starttime).ToListAsync();
            if (theoffsets.Count() == 0)
            {
                theoffset.offset = 0;
            }
            else if (theoffsets.Count() == 1)
            {
                theoffset = theoffsets.First();
            }
            else
            {
                try
                {
                    theoffset = theoffsets.Where(x => x.starttime <= thetime).OrderByDescending(x => x.starttime).First();
                }
                catch
                {
                    try
                    {
                        theoffset = theoffsets.OrderBy(x => x.starttime).First();
                    }
                    catch
                    {
                        theoffset.offset = measpoint.Offset;
                    }

                }
            }
            var meas = new Meas();
            Random r = new Random();
            meas.DoneBy = "Rho";
            meas.MeasPointId = measpoint.Id;
            meas.TheMeasurement = Math.Round(((theoffset.offset - thevalue) + (r.NextDouble() * 2 - 1) * 0.05), 2, MidpointRounding.AwayFromZero);
            thetime.AddHours(2);
            meas.When = thetime.ToLocalTime();
            _context.Measures.Add(meas);
            await _context.SaveChangesAsync();
            return 0;
        }
        // GET: DataPoints
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var dataContext = _context.DataPoints.Include(d => d.MeasPoint).ThenInclude(x => x.Project).ThenInclude(x => x.Division).Where(x=>x.MeasPoint.Project.Division.Id.Equals(user.DivisionId)).OrderByDescending(x=>x.Id).Take(10);
            return View(await dataContext.ToListAsync());
        }

        // GET: DataPoints/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dataPoint = await _context.DataPoints
                .Include(d => d.MeasPoint)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dataPoint == null)
            {
                return NotFound();
            }

            return View(dataPoint);
        }

        // GET: DataPoints/Create
        public IActionResult Create()
        {
            ViewData["MeasPointId"] = new SelectList(_context.MeasPoints, "Id", "Name");
            return View();
        }

        // POST: DataPoints/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Datum,MessWert,MeasPointId")] DataPoint dataPoint)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dataPoint);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var user = await _userManager.GetUserAsync(User);
            ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Project.Division.Id.Equals(user.DivisionId)).ToList(), "Id", "Name", dataPoint.MeasPointId);
            return View(dataPoint);
        }

        // GET: DataPoints/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dataPoint = await _context.DataPoints.FindAsync(id);
            if (dataPoint == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Project.Division.Id.Equals(user.DivisionId)).ToList(), "Id", "Name", dataPoint.MeasPointId);
            return View(dataPoint);
        }

        // POST: DataPoints/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Datum,MessWert,MeasPointId")] DataPoint dataPoint)
        {
            if (id != dataPoint.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dataPoint);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DataPointExists(dataPoint.Id))
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
            ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Project.Division.Id.Equals(user.DivisionId)).ToList(), "Id", "Name", dataPoint.MeasPointId);
            return View(dataPoint);
        }

        // GET: DataPoints/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dataPoint = await _context.DataPoints
                .Include(d => d.MeasPoint)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dataPoint == null)
            {
                return NotFound();
            }

            return View(dataPoint);
        }

        // POST: DataPoints/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dataPoint = await _context.DataPoints.FindAsync(id);
            _context.DataPoints.Remove(dataPoint);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DataPointExists(int id)
        {
            return _context.DataPoints.Any(e => e.Id == id);
        }
    }
}
