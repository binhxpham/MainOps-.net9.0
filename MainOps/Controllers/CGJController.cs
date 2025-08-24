using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Docnet.Core;
using ImageMagick;
using MainOps.Data;
using MainOps.Models;
using MainOps.Models.CGJClasses;
using MainOps.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using Microsoft.Extensions.Hosting;

namespace MainOps.Controllers
{
    [Authorize]
    public class CGJensenController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly IEmailSender _emailSender;

        public CGJensenController(DataContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
            _emailSender = emailSender;

        }

        // GET: InformationEntries
        public IActionResult CGJensenMainMenu()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Protocols()
        {
            var items = await _context.Protocols.ToListAsync();
            return View(items);
        }
        [HttpGet]
        public async Task<IActionResult> Structures()
        {
            var items = await _context.Structures.Include(x => x.Protocol).ToListAsync();
            return View(items);
        }
        [HttpGet]
        public async Task<IActionResult> Buildings()
        {
            var items = await _context.Buildings.Include(x => x.Protocol).ToListAsync();
            return View(items);
        }
        [HttpGet]
        public async Task<IActionResult> Junctions()
        {
            var items = await _context.Junctions.Include(x => x.Protocol).ToListAsync();
            return View(items);
        }
        [HttpGet]
        public async Task<IActionResult> RoadSections()
        {
            var items = await _context.RoadSections.Include(x => x.Protocol).ToListAsync();
            return View(items);
        }
        [HttpGet]
        public async Task<IActionResult> AccommodationWorks()
        {
            var items = await _context.AccomodationWorks.Include(x => x.Protocol).ToListAsync();
            return View(items);
        }
        [HttpGet]
        public async Task<IActionResult> SiteClearancesBetweenContractBorders()
        {
            var items = await _context.SiteClearancesBetweenContractBorders.Include(x => x.Protocol).ToListAsync();
            return View(items);
        }
        [HttpGet]
        public async Task<IActionResult> SiteClearanceForUtilityRelocations()
        {
            var items = await _context.SiteClearancesForUtilityReolocations.Include(x => x.Protocol).ToListAsync();
            return View(items);
        }
        [HttpGet]
        public async Task<IActionResult> Create_EA()
        {
            var subprojects = await (from sp in _context.SubProjects.Include(x => x.Project).Where(x => Convert.ToInt32(x.Project.Id).Equals(56))
                                     select new
                                     { Id = sp.Project.ProjectNr + "-" + sp.SubProjectNr, Text = sp.Project.ProjectNr + "-" + sp.SubProjectNr + " : " + sp.Name })
                                    .ToListAsync();
            ViewData["Sagsnummer"] = new SelectList(subprojects, "Id", "Text");

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create_EA(EA model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("EkstraArbejder");
        }
        [HttpGet]
        public async Task<IActionResult> Ekstraarbejder()
        {
            var items = await _context.EAs.OrderBy(x => x.EAnr).ToListAsync();
            return View(items);
        }
        [HttpGet]
        public async Task<IActionResult> Edit_EA(int? id)
        {
            if (id != null)
            {
                var item = await _context.EAs.FindAsync(id);
                var subprojects = await (from sp in _context.SubProjects.Include(x => x.Project).Where(x => Convert.ToInt32(x.Project.Id).Equals(56))
                                         select new
                                         { Id = sp.Project.ProjectNr + "-" + sp.SubProjectNr, Text = sp.Project.ProjectNr + "-" + sp.SubProjectNr + " : " + sp.Name }).ToListAsync();
                ViewData["Sagsnummer"] = new SelectList(subprojects, "Id", "Text");
                return View(item);
            }
            else
            {
                return NotFound();
            }
        }
        public async Task<IActionResult> Delete_EA(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ea = await _context.EAs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ea == null)
            {
                return NotFound();
            }

            return View(ea);
        }

        // POST: MeasTypes/Delete/5
        [HttpPost, ActionName("Delete_EA")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedEA(int id)
        {
            var ea = await _context.EAs.FindAsync(id);
            _context.EAs.Remove(ea);
            await _context.SaveChangesAsync();
            return RedirectToAction("Ekstraarbejder");
        }

        [HttpPost]
        public async Task<IActionResult> Edit_EA(EA model)
        {
            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("EkstraArbejder");
            }
            else
            {
                ViewData["ProtocolId"] = new SelectList(_context.Protocols, "Id", "Name");
                return View(model);
            }
        }
        [HttpGet]
        public IActionResult Create_Structure()
        {
            ViewData["ProtocolId"] = new SelectList(_context.Protocols, "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create_Structure(Structure model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Structures");
        }
        [HttpGet]
        public async Task<IActionResult> Edit_Structure(int? id)
        {
            if (id != null)
            {
                var item = await _context.Structures.FindAsync(id);
                ViewData["ProtocolId"] = new SelectList(_context.Protocols, "Id", "Name");
                return View(item);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit_Structure(Structure model, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    var directory = _env.WebRootPath + "\\CGJensen\\StructurePhotos\\" + model.Id.ToString() + "\\";
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    var path = Path.Combine(directory, file.FileName);
                    var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                    stream.Close();

                    if (file.FileName.Contains(".pdf"))
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(path + file);

                        MemoryStream ms = PdfToImage(bytes);
                        using (FileStream fileToSave = new FileStream(path + file.FileName.Replace(".pdf", ".png"), FileMode.Create, System.IO.FileAccess.Write))
                        {
                            ms.CopyTo(fileToSave);
                        }
                        ms.Close();
                        model.path = path.Replace(".pdf", ".png");
                    }
                    else
                    {
                        model.path = path;
                    }
                }
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Structures");
            }
            else
            {
                ViewData["ProtocolId"] = new SelectList(_context.Protocols, "Id", "Name");
                return View(model);
            }
        }
        [HttpGet]
        public IActionResult Create_Building()
        {
            ViewData["ProtocolId"] = new SelectList(_context.Protocols, "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create_Building(Building model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Buildings");
        }
        [HttpGet]
        public async Task<IActionResult> Edit_Building(int? id)
        {
            if (id != null)
            {
                var item = await _context.Buildings.FindAsync(id);
                ViewData["ProtocolId"] = new SelectList(_context.Protocols, "Id", "Name");
                return View(item);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit_Building(Building model, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    var directory = _env.WebRootPath + "\\CGJensen\\BuildingPhotos\\" + model.Id.ToString() + "\\";
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    var path = Path.Combine(directory, file.FileName);
                    var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                    stream.Close();

                    if (file.FileName.Contains(".pdf"))
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(path + file);

                        MemoryStream ms = PdfToImage(bytes);
                        using (FileStream fileToSave = new FileStream(path + file.FileName.Replace(".pdf", ".png"), FileMode.Create, System.IO.FileAccess.Write))
                        {
                            ms.CopyTo(fileToSave);
                        }
                        ms.Close();
                        model.path = path.Replace(".pdf", ".png");
                    }
                    else
                    {
                        model.path = path;
                    }
                }
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Buildings");
            }
            else
            {
                ViewData["ProtocolId"] = new SelectList(_context.Protocols, "Id", "Name");
                return View(model);
            }
        }
        [HttpGet]
        public IActionResult Create_Junction()
        {
            ViewData["ProtocolId"] = new SelectList(_context.Protocols, "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create_Junction(Junction model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Junctions");
        }
        [HttpGet]
        public async Task<IActionResult> Edit_Junction(int? id)
        {
            if (id != null)
            {
                var item = await _context.Junctions.FindAsync(id);
                ViewData["ProtocolId"] = new SelectList(_context.Protocols, "Id", "Name");
                return View(item);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit_Junction(Junction model, IFormFile file)
        {
            if (ModelState.IsValid)
            {

                if (file != null)
                {
                    var directory = _env.WebRootPath + "\\CGJensen\\JunctionPhotos\\" + model.Id.ToString() + "\\";
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    var path = Path.Combine(directory, file.FileName);
                    var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                    stream.Close();

                    if (file.FileName.Contains(".pdf"))
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(path + file);

                        MemoryStream ms = PdfToImage(bytes);
                        using (FileStream fileToSave = new FileStream(path + file.FileName.Replace(".pdf", ".png"), FileMode.Create, System.IO.FileAccess.Write))
                        {
                            ms.CopyTo(fileToSave);
                        }
                        ms.Close();
                        model.path = path.Replace(".pdf", ".png");
                    }
                    else
                    {
                        model.path = path;
                    }
                }
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Junctions");
            }
            else
            {
                ViewData["ProtocolId"] = new SelectList(_context.Protocols, "Id", "Name");
                return View(model);
            }
        }
        [HttpGet]
        public IActionResult Create_RoadSection()
        {
            ViewData["ProtocolId"] = new SelectList(_context.Protocols, "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create_RoadSection(RoadSection model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("RoadSections");
        }
        [HttpGet]
        public async Task<IActionResult> Edit_RoadSection(int? id)
        {
            if (id != null)
            {
                var item = await _context.RoadSections.FindAsync(id);
                ViewData["ProtocolId"] = new SelectList(_context.Protocols, "Id", "Name");
                return View(item);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit_RoadSection(RoadSection model, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    var directory = _env.WebRootPath + "\\CGJensen\\RoadSectionPhotos\\" + model.Id.ToString() + "\\";
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    var path = Path.Combine(directory, file.FileName);
                    var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                    stream.Close();

                    if (file.FileName.Contains(".pdf"))
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(path + file);

                        MemoryStream ms = PdfToImage(bytes);
                        using (FileStream fileToSave = new FileStream(path + file.FileName.Replace(".pdf", ".png"), FileMode.Create, System.IO.FileAccess.Write))
                        {
                            ms.CopyTo(fileToSave);
                        }
                        ms.Close();
                        model.path = path.Replace(".pdf", ".png");
                    }
                    else
                    {
                        model.path = path;
                    }
                }
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("RoadSections");
            }
            else
            {
                ViewData["ProtocolId"] = new SelectList(_context.Protocols, "Id", "Name");
                return View(model);
            }
        }
        [HttpGet]
        public IActionResult Create_AccomodationWork()
        {
            ViewData["ProtocolId"] = new SelectList(_context.Protocols, "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create_AccomodationWorks(AccomodationWork model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("AccomodationWorks");
        }
        [HttpGet]
        public async Task<IActionResult> Edit_AccomodationWork(int? id)
        {
            if (id != null)
            {
                var item = await _context.AccomodationWorks.FindAsync(id);
                ViewData["ProtocolId"] = new SelectList(_context.Protocols, "Id", "Name");
                return View(item);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit_AccomodationWork(AccomodationWork model, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    var directory = _env.WebRootPath + "\\CGJensen\\AccommodationWorkPhotos\\" + model.Id.ToString() + "\\";
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    var path = Path.Combine(directory, file.FileName);
                    var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                    stream.Close();

                    if (file.FileName.Contains(".pdf"))
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(path + file);

                        MemoryStream ms = PdfToImage(bytes);
                        using (FileStream fileToSave = new FileStream(path + file.FileName.Replace(".pdf", ".png"), FileMode.Create, System.IO.FileAccess.Write))
                        {
                            ms.CopyTo(fileToSave);
                        }
                        ms.Close();
                        model.path = path.Replace(".pdf", ".png");
                    }
                    else
                    {
                        model.path = path;
                    }
                }
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("AccomodationWorks");
            }
            else
            {
                ViewData["ProtocolId"] = new SelectList(_context.Protocols, "Id", "Name");
                return View(model);
            }
        }
        [HttpGet]
        public IActionResult Create_SiteClearancesBetweenContractBorders()
        {
            ViewData["ProtocolId"] = new SelectList(_context.Protocols, "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create_SiteClearancesBetweenContractBorders(SiteClearancesBetweenContractBorder model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("SiteClearancesBetweenContractBorders");
        }
        [HttpGet]
        public async Task<IActionResult> Edit_SiteClearancesBetweenContractBorders(int? id)
        {
            if (id != null)
            {
                var item = await _context.SiteClearancesBetweenContractBorders.FindAsync(id);
                ViewData["ProtocolId"] = new SelectList(_context.Protocols, "Id", "Name");
                return View(item);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit_SiteClearancesBetweenContractBorders(SiteClearancesBetweenContractBorder model, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    var directory = _env.WebRootPath + "\\CGJensen\\SiteClearBorderPhotos\\" + model.Id.ToString() + "\\";
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    var path = Path.Combine(directory, file.FileName);
                    var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                    stream.Close();

                    if (file.FileName.Contains(".pdf"))
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(path + file);

                        MemoryStream ms = PdfToImage(bytes);
                        using (FileStream fileToSave = new FileStream(path + file.FileName.Replace(".pdf", ".png"), FileMode.Create, System.IO.FileAccess.Write))
                        {
                            ms.CopyTo(fileToSave);
                        }
                        ms.Close();
                        model.path = path.Replace(".pdf", ".png");
                    }
                    else
                    {
                        model.path = path;
                    }
                }
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("SiteClearancesBetweenContractBorders");
            }
            else
            {
                ViewData["ProtocolId"] = new SelectList(_context.Protocols, "Id", "Name");
                return View(model);
            }
        }
        [HttpGet]
        public IActionResult Create_SiteClearanceForUtilityRelocation()
        {
            ViewData["ProtocolId"] = new SelectList(_context.Protocols, "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create_SiteClearanceForUtilityRelocation(SiteClearanceForUtilityRelocation model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("SiteClearancesForUtilityRelocations");
        }
        [HttpGet]
        public async Task<IActionResult> Edit_SiteClearanceForUtilityRelocation(int? id)
        {
            if (id != null)
            {
                var item = await _context.SiteClearancesForUtilityReolocations.FindAsync(id);
                ViewData["ProtocolId"] = new SelectList(_context.Protocols, "Id", "Name");
                return View(item);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit_SiteClearanceForUtilityRelocation(SiteClearanceForUtilityRelocation model, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    var directory = _env.WebRootPath + "\\CGJensen\\SiteClearUtilPhotos\\" + model.Id.ToString() + "\\";
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    var path = Path.Combine(directory, file.FileName);
                    var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                    stream.Close();

                    if (file.FileName.Contains(".pdf"))
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(path + file);

                        MemoryStream ms = PdfToImage(bytes);
                        using (FileStream fileToSave = new FileStream(path + file.FileName.Replace(".pdf", ".png"), FileMode.Create, System.IO.FileAccess.Write))
                        {
                            ms.CopyTo(fileToSave);
                        }
                        ms.Close();
                        model.path = path.Replace(".pdf", ".png");
                    }
                    else
                    {
                        model.path = path;
                    }
                }
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("SiteClearancesForUtilityRelocations");
            }
            else
            {
                ViewData["ProtocolId"] = new SelectList(_context.Protocols, "Id", "Name");
                return View(model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit_DagsRapport(int? id)
        {
            var dr = await _context.Dagsrapporter
                .Include(x => x.Timer)
                .Include(x => x.Project)
                .Include(x => x.SubProject)
                .Include(x => x.Kontraktarbejder)
                .Include(x => x.Ekstraarbejder)
                .Where(x => x.Id.Equals(id))
                .SingleOrDefaultAsync();

            CGJDailyReportEditVM model = new CGJDailyReportEditVM(dr);
            model.Timer = new List<TimeRegistrering>();
            model.Ekstraarbejder = new List<EkstraArbejde>();
            model.Kontraktarbejder = new List<KontraktArbejde>();
            int timecount1 = dr.Timer.Where(x=>x.EANr == null).Count();
            int timecount2 = dr.Timer.Where(x => x.EANr != null).Count();
            int kontraktcount = dr.Kontraktarbejder.Count();
            int Ekstracount = dr.Ekstraarbejder.Count();
            for (int i = 0; i < 10; i++)
            {
                
                if(kontraktcount > i) { 
                    KontraktArbejde ka = new KontraktArbejde(dr.Kontraktarbejder.ElementAt(i));
                    model.Kontraktarbejder.Add(ka);
                }
                else
                {
                    KontraktArbejde ka = new KontraktArbejde();
                    model.Kontraktarbejder.Add(ka);
                }
                
                if (i < 5)
                {
                    if (timecount1 > i)
                    {
                        TimeRegistrering tr = new TimeRegistrering(dr.Timer.Where(x => x.EANr == null).ElementAt(i));
                        model.Timer.Add(tr);
                    }
                    else
                    {
                        TimeRegistrering tr = new TimeRegistrering();
                        model.Timer.Add(tr);
                    }
                    if (Ekstracount > i) {
                        EkstraArbejde ea = new EkstraArbejde(dr.Ekstraarbejder.ElementAt(i));
                        model.Ekstraarbejder.Add(ea);
                    }
                    else
                    {
                        EkstraArbejde ea = new EkstraArbejde();
                        model.Ekstraarbejder.Add(ea);
                    }
                }
                else
                {
                    if (timecount2 > (i-5))
                    {
                        TimeRegistrering tr = new TimeRegistrering(dr.Timer.Where(x=>x.EANr != null).ElementAt(i-5));
                        model.Timer.Add(tr);
                    }
                    else
                    {
                        TimeRegistrering tr = new TimeRegistrering();
                        model.Timer.Add(tr);
                    }
                }
            }
            var Structures = await (from s in _context.Structures select new { Id = s.Id, Text = s.Location + ":" + s.Name }).ToListAsync();
            var Buildings = await (from s in _context.Buildings select new { Id = s.Id, Text = s.Location + ":" + s.Address }).ToListAsync();
            var RoadSections = await (from s in _context.RoadSections select new { Id = s.Id, Text = s.Location + ":" + s.Name }).ToListAsync();
            var Junctions = await (from s in _context.Junctions select new { Id = s.Id, Text = s.Location + ":" + s.Name }).ToListAsync();
            var Accommodations = await (from s in _context.AccomodationWorks select new { Id = s.Id, Text = s.Location + ":" + s.Name }).ToListAsync();
            var SiteClear1 = await (from s in _context.SiteClearancesBetweenContractBorders select new { Id = s.Id, Text = s.Location + ":" + s.Name }).ToListAsync();
            var SiteClear2 = await (from s in _context.SiteClearancesForUtilityReolocations select new { Id = s.Id, Text = s.Location + ":" + s.Name }).ToListAsync();
            var EAs = await (from s in _context.EAs select new { Id = s.EAnr, Text = s.EAnr + ":" + s.Opgave }).OrderBy(x => x.Id).ToListAsync();
            var subprojects = await (from sp in _context.SubProjects.Include(x => x.Project).Where(x => Convert.ToInt32(x.Project.Id).Equals(56))
                                     select new
                                     { Id = sp.Id, Text = sp.Project.ProjectNr + "-" + sp.SubProjectNr + " : " + sp.Name }).ToListAsync();

            ViewData["SubProjectId"] = new SelectList(subprojects, "Id", "Text");
            ViewData["ProtocolId"] = new SelectList(_context.Protocols.OrderBy(x => Convert.ToInt32(x.Protocolnr)), "Id", "Protocolnr");
            ViewData["StructureId"] = new SelectList(Structures, "Id", "Text");
            ViewData["BuildingId"] = new SelectList(Buildings, "Id", "Text");
            ViewData["RoadSectionId"] = new SelectList(RoadSections, "Id", "Text");
            ViewData["JunctionId"] = new SelectList(Junctions, "Id", "Text");
            ViewData["AccomodationWorkId"] = new SelectList(Accommodations, "Id", "Text");
            ViewData["SiteClearancesBetweenContractBorderId"] = new SelectList(SiteClear1, "Id", "Text");
            ViewData["SiteClearanceForUtilityRelocationId"] = new SelectList(SiteClear2, "Id", "Text");
            ViewData["EAId"] = new SelectList(EAs, "Id", "Text");


            return View("Edit_Dagsrapport", model);
        }
        [HttpPost]
        public async Task<IActionResult> DagsRapport(CGJDailyReportVM model, IFormFile[] files)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                Dagsrapport DagsRapport = new Dagsrapport();
                
                var project = await _context.Projects.FindAsync(56);
                var subproject = await _context.SubProjects.FindAsync(Convert.ToInt32(model.Kontraktnr));
                DagsRapport.ProjectId = project.Id;
                DagsRapport.SubProjectId = Convert.ToInt32(model.Kontraktnr);
                DagsRapport.KontraktNr = project.ProjectNr + "-" + subproject.SubProjectNr;
                if(Convert.ToInt32(subproject.SubProjectNr) < 28)
                {
                    DagsRapport.Kontraktnavn = "G";
                }
                else
                {
                    DagsRapport.Kontraktnavn = "RH";
                }
                DagsRapport.Dato = model.Report_Date;
                DagsRapport.UdarbjedetAf = user.full_name();
               

                int dayofweek = Convert.ToInt32(model.Report_Date.Date.DayOfWeek);
                if (dayofweek == 1)
                {
                    DagsRapport.Ugedag = "Mandag";
                }
                else if (dayofweek == 2)
                {
                    DagsRapport.Ugedag = "Tirsdag";
                }
                else if (dayofweek == 3)
                {
                    DagsRapport.Ugedag = "Onsdag";
                }
                else if (dayofweek == 4)
                {
                    DagsRapport.Ugedag = "Torsdag";
                }
                else if (dayofweek == 5)
                {
                    DagsRapport.Ugedag = "Fredag";
                }
                else if (dayofweek == 6)
                {
                    DagsRapport.Ugedag = "Lørdag";
                }
                else
                {
                    DagsRapport.Ugedag = "Søndag";
                }
                DagsRapport.Afdeling = model.Afdeling;
                DagsRapport.Addresse = model.Addresse;
                DagsRapport.ForSaetter = model.Fortsaetter;
                DagsRapport.DagensVejr = model.Vejret;
                DagsRapport.Vejrlig = model.Vejrlig;
                DagsRapport.Underskrift = model.Signature;
                DagsRapport.UddybendeNoter = model.UddybendeNoter;
                if (model.Location.RoadSectionId != null)
                {
                    var loc = await _context.RoadSections.FindAsync(model.Location.RoadSectionId);
                    DagsRapport.Lokation = loc.Location;
                }
                else if (model.Location.BuildingId != null)
                {
                    var loc = await _context.Buildings.FindAsync(model.Location.BuildingId);
                    DagsRapport.Lokation = loc.Location;
                }
                else if (model.Location.AccomodationWorkId != null)
                {
                    var loc = await _context.AccomodationWorks.FindAsync(model.Location.AccomodationWorkId);
                    DagsRapport.Lokation = loc.Location;
                }
                else if (model.Location.StructureId != null)
                {
                    var loc = await _context.Structures.FindAsync(model.Location.StructureId);
                    DagsRapport.Lokation = loc.Location;
                }
                else if (model.Location.JunctionId != null)
                {
                    var loc = await _context.Junctions.FindAsync(model.Location.JunctionId);
                    DagsRapport.Lokation = loc.Location;
                }
                else if (model.Location.SiteClearancesBetweenContractBorderId != null)
                {
                    var loc = await _context.SiteClearancesBetweenContractBorders.FindAsync(model.Location.SiteClearancesBetweenContractBorderId);
                    DagsRapport.Lokation = loc.Location;
                }
                else
                {
                    //siteclearancesforutility
                    var loc = await _context.SiteClearancesForUtilityReolocations.FindAsync(model.Location.SiteClearanceForUtilityRelocationId);
                    DagsRapport.Lokation = loc.Location;
                }
                var lastDagsRapport = await _context.Dagsrapporter.Where(x => x.Lokation.Equals(DagsRapport.Lokation)).OrderBy(x => x.Dato).LastOrDefaultAsync();

                if (lastDagsRapport != null)
                {
                    DagsRapport.RapportNr = lastDagsRapport.RapportNr + 1;
                }
                else
                {
                    DagsRapport.RapportNr = 1;
                }
                _context.Add(DagsRapport);
                await _context.SaveChangesAsync();
                DagsRapport = null;
                var lastadded = await _context.Dagsrapporter.LastAsync();
                //photos
                var folderpath = _env.WebRootPath + "\\CGJensen\\Dagsrapporter\\" + lastadded.Id.ToString() + "\\";
                if (!Directory.Exists(folderpath) && files != null)
                {
                    Directory.CreateDirectory(folderpath);
                }
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            //Getting FileName
                            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            fileName = folderpath + $@"\{fileName}";

                            using (FileStream fs = System.IO.File.Create(fileName))
                            {
                                await file.CopyToAsync(fs);
                                fs.Flush();
                            }
                            if (fileName.Contains(".jpg") || fileName.Contains(".jpeg"))
                            {
                                SaveAndCompressJpeg(fileName, 90);
                            }
                        }
                    }
                }
                //
                foreach (TimeRegistrering t in model.Timer)
                {
                    if (t.AntalPersoner > 0)
                    {
                        DagsRapport_TimeRegistrering t_new = new DagsRapport_TimeRegistrering(lastadded, t);
                        _context.Add(t_new);
                    }
                }
                foreach (KontraktArbejde k in model.Kontraktarbejder)
                {
                    if (k.EgetMateriel != null || k.UL_Firma != null || k.UL_Materiel != null)
                    {
                        DagsRapport_KontrakArbejde k_new = new DagsRapport_KontrakArbejde(lastadded, k);
                        _context.Add(k_new);
                    }
                }
                var cgjpeople = await _context.CGJensenAdmins.ToListAsync();
                foreach (EkstraArbejde e in model.Ekstraarbejder)
                {
                    if (e.EANr != null || e.EANr_skrevet != null)
                    {
                        var EA = await _context.EAs.Where(x => x.EAnr.Equals(e.EANr) || x.EAnr.Equals(e.EANr_skrevet)).SingleOrDefaultAsync();
                        if (EA != null)
                        {
                            DagsRapport_EkstraArbejde eks = new DagsRapport_EkstraArbejde(lastadded, e, EA.Id);
                            _context.Add(eks);
                        }
                        else
                        {
                            DagsRapport_EkstraArbejde eks = new DagsRapport_EkstraArbejde(lastadded, e, null);
                            _context.Add(eks);
                            foreach(var adm in cgjpeople) { 
                                //send email for oprettelse af EA nummer
                                string theaddress = "https://hj-mainops.com/CGJensen/Edit_Dagsrapport/" + lastadded.Id + "?";
                                string theaddress2 = "https://hj-mainops.com/CGJensen/Create_EA";
                                string theaddress3 = "https://hj-mainops.com/CGJensen/Ekstraarbejder?";
                                string Subject = "Oprettelse af EA nummer";
                                string HtmlContent = "<strong>Hejsa " + adm.FirstName + " " + adm.LastName + "</strong><br /><br /> En medarbejder har udfyldt en Dagsseddel med manglende EA nummer på sit ekstraarbejde<br><br />link til redigering af dagsseddel her: " + "<a href='" + theaddress + "'>linkdagsseddel</a><br /><br />link til oprettelse af EA nummer her: " + "<a href='" + theaddress2 + "'>linknytEAnummer</a><br /><br />link til at se alle EA numre her: " +"<a href='" + theaddress3 + "'>linkalleEAnumre</a><br /><br />";
                                string footerstringHTML = "<br />Hölscher Jensen A/S<br />Fabriksparken 37<br />2600 Glostrup<br />Denmark";
                                string footerstringPLAIN = "\r\n\r\nHölscher Jensen A/S \r\nFabriksparken 37\r\n2600 Glostrup\r\nDenmark";
                                await _emailSender.SendEmailAsync3(adm.Email, Subject, HtmlContent, footerstringHTML, footerstringPLAIN);
                            }
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("CGJensenMainMenu");
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit_DagsRapport(CGJDailyReportEditVM model, IFormFile[] files)
        {
            if (ModelState.IsValid)
            {
                var DagsRapport = await _context.Dagsrapporter
                    .Include(x => x.Ekstraarbejder)
                    .Include(x => x.Kontraktarbejder)
                    .Include(x => x.Timer)
                    .Where(x => x.Id.Equals(model.DagsrapportId)).SingleOrDefaultAsync();


                DagsRapport.RapportNr = model.RapportNr;
                var project = await _context.Projects.FindAsync(56);
                SubProject subproject;
                try {
                    string projectnr = model.Kontraktnr.Split("-")[1];
                    subproject = await _context.SubProjects.Where(x => x.SubProjectNr.Equals(projectnr) && x.ProjectId.Equals(56)).SingleOrDefaultAsync();
                    DagsRapport.SubProjectId = Convert.ToInt32(subproject.Id);
                    DagsRapport.SubProject = subproject;
                }
                catch
                {
                    subproject = await _context.SubProjects.FindAsync(DagsRapport.SubProjectId);
                    DagsRapport.SubProjectId = subproject.Id;
                    DagsRapport.SubProject = subproject;
                }
                DagsRapport.ProjectId = project.Id;
                DagsRapport.KontraktNr = model.Kontraktnr;
                if (Convert.ToInt32(subproject.SubProjectNr) < 28)
                {
                    DagsRapport.Kontraktnavn = "G";
                }
                else
                {
                    DagsRapport.Kontraktnavn = "RH";
                }
                DagsRapport.Dato = model.Dato;
                int dayofweek = Convert.ToInt32(model.Dato.Date.DayOfWeek);
                if (dayofweek == 1)
                {
                    DagsRapport.Ugedag = "Mandag";
                }
                else if (dayofweek == 2)
                {
                    DagsRapport.Ugedag = "Tirsdag";
                }
                else if (dayofweek == 3)
                {
                    DagsRapport.Ugedag = "Onsdag";
                }
                else if (dayofweek == 4)
                {
                    DagsRapport.Ugedag = "Torsdag";
                }
                else if (dayofweek == 5)
                {
                    DagsRapport.Ugedag = "Fredag";
                }
                else if (dayofweek == 6)
                {
                    DagsRapport.Ugedag = "Lørdag";
                }
                else
                {
                    DagsRapport.Ugedag = "Søndag";
                }
                DagsRapport.Afdeling = model.Afdeling;
                DagsRapport.Addresse = model.Addresse;
                DagsRapport.ForSaetter = model.Fortsaetter;
                DagsRapport.DagensVejr = model.Vejret;
                DagsRapport.Vejrlig = model.Vejrlig;
                DagsRapport.Underskrift = model.Signature;
                DagsRapport.UddybendeNoter = model.UddybendeNoter;
                DagsRapport.Lokation = model.Lokation;

                List<DagsRapport_EkstraArbejde> new_Ekstra = new List<DagsRapport_EkstraArbejde>();
                List<DagsRapport_KontrakArbejde> new_Kontra = new List<DagsRapport_KontrakArbejde>();
                List<DagsRapport_TimeRegistrering> new_Time1 = new List<DagsRapport_TimeRegistrering>();
                List<DagsRapport_TimeRegistrering> new_Time2 = new List<DagsRapport_TimeRegistrering>();
                await _context.SaveChangesAsync();
                var folderpath = _env.WebRootPath + "\\CGJensen\\Dagsrapporter\\" + model.DagsrapportId.ToString() + "\\";
                if (!Directory.Exists(folderpath) && files != null)
                {
                    Directory.CreateDirectory(folderpath);
                }
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            //Getting FileName
                            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            fileName = folderpath + $@"\{fileName}";

                            using (FileStream fs = System.IO.File.Create(fileName))
                            {
                                await file.CopyToAsync(fs);
                                fs.Flush();
                            }
                            if (fileName.Contains(".jpg") || fileName.Contains(".jpeg"))
                            {
                                SaveAndCompressJpeg(fileName, 90);
                            }
                        }
                    }
                }
                //
                _context.Update(DagsRapport);
                
                foreach (TimeRegistrering t in model.Timer.Where(x=>x.EANr == null))
                {
                    if (t.AntalPersoner > 0)
                    {
                        DagsRapport_TimeRegistrering t_new = new DagsRapport_TimeRegistrering(DagsRapport, t);
                        new_Time1.Add(t_new);
                    }
                }
                foreach (TimeRegistrering t in model.Timer.Where(x => x.EANr != null))
                {
                    if (t.AntalPersoner > 0)
                    {
                        DagsRapport_TimeRegistrering t_new = new DagsRapport_TimeRegistrering(DagsRapport, t);
                        new_Time2.Add(t_new);
                    }
                }
                foreach (KontraktArbejde k in model.Kontraktarbejder)
                {
                    if (k.EgetMateriel != null || k.UL_Firma != null || k.UL_Materiel != null)
                    {
                        DagsRapport_KontrakArbejde k_new = new DagsRapport_KontrakArbejde(DagsRapport, k);
                        new_Kontra.Add(k_new);
                    }
                }
                foreach (EkstraArbejde e in model.Ekstraarbejder)
                {
                    if (e.EANr != null || e.EANr_skrevet != null)
                    {
                        var EA = await _context.EAs.Where(x => x.EAnr.Equals(e.EANr) || x.EAnr.Equals(e.EANr_skrevet)).SingleOrDefaultAsync();
                        if (EA != null)
                        {
                            DagsRapport_EkstraArbejde eks = new DagsRapport_EkstraArbejde(DagsRapport, e, EA.Id);
                            new_Ekstra.Add(eks);
                        }
                        else
                        {
                            DagsRapport_EkstraArbejde eks = new DagsRapport_EkstraArbejde(DagsRapport, e, null);
                            new_Ekstra.Add(eks);
                        }
                    }
                }
                for(int i = 0; i < new_Kontra.Count(); i++)
                {
                    if(DagsRapport.Kontraktarbejder.Count() -1 >= i)
                    {
                        var theone = DagsRapport.Kontraktarbejder.ElementAt(i);
                        theone.EgetMateriel = new_Kontra[i].EgetMateriel;
                        theone.UL_Firma = new_Kontra[i].UL_Firma;
                        theone.UL_Materiel = new_Kontra[i].UL_Materiel;
                        _context.Update(theone);
                    }
                    else
                    {
                        _context.Add(new_Kontra[i]);
                    }
                }
                for (int i = 0; i < new_Ekstra.Count(); i++)
                {
                    if (DagsRapport.Ekstraarbejder.Count() - 1 >= i)
                    {
                        var theone = DagsRapport.Ekstraarbejder.ElementAt(i);
                        theone.EAId = new_Ekstra[i].EAId;
                        theone.EANr = new_Ekstra[i].EANr;
                        theone.EANr_skrevet = new_Ekstra[i].EANr_skrevet;
                        theone.Eget_Materiel = new_Ekstra[i].Eget_Materiel;
                        theone.Fortsaetter = new_Ekstra[i].Fortsaetter;
                        theone.Opgave = new_Ekstra[i].Opgave;
                        theone.UEFirma_Materiel = new_Ekstra[i].UEFirma_Materiel;
                        _context.Update(theone);
                    }
                    else
                    {
                        _context.Add(new_Ekstra[i]);
                    }
                }
                for (int i = 0; i < new_Time1.Count(); i++)
                {
                    if(DagsRapport.Timer.Where(x=>x.EANr == null).Count() - 1 >= i)
                    {
                        var theone = DagsRapport.Timer.Where(x=>x.EANr == null).ElementAt(i);
                        theone.AntalPersoner = new_Time1[i].AntalPersoner;
                        theone.Timer = new_Time1[i].Timer;
                        theone.TimerFoerOvertid = new_Time1[i].TimerFoerOvertid;
                        theone.Overtid_100 = new_Time1[i].Overtid_100;
                        theone.Overtid_50 = new_Time1[i].Overtid_50;
                        theone.CGJ_Timer_Total = new_Time1[i].CGJ_Timer_Total;
                        theone.Note = new_Time1[i].Note;
                        theone.Title = new_Time1[i].Title;
                        theone.EANr = new_Time1[i].EANr;
                        _context.Update(theone);
                    }
                    else
                    {
                        _context.Add(new_Time1[i]);
                    }
                }
                for (int i = 0; i < new_Time2.Count(); i++)
                {
                    if (DagsRapport.Timer.Where(x => x.EANr != null).Count() - 1 >= i)
                    {
                        var theone = DagsRapport.Timer.Where(x=>x.EANr != null).ElementAt(i);
                        theone.AntalPersoner = new_Time2[i].AntalPersoner;
                        theone.Timer = new_Time2[i].Timer;
                        theone.TimerFoerOvertid = new_Time2[i].TimerFoerOvertid;
                        theone.Overtid_100 = new_Time2[i].Overtid_100;
                        theone.Overtid_50 = new_Time2[i].Overtid_50;
                        theone.CGJ_Timer_Total = new_Time2[i].CGJ_Timer_Total;
                        theone.Note = new_Time2[i].Note;
                        theone.Title = new_Time2[i].Title;
                        theone.EANr = new_Time2[i].EANr;
                        _context.Update(theone);
                    }
                    else
                    {
                        _context.Add(new_Time2[i]);
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Dagsrapporter");
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Dagsrapport_PDF(int? id)
        {
            if (id != null)
            {
                var model = await _context.Dagsrapporter
                    .Include(x => x.SubProject)
                    .Include(x => x.Project).ThenInclude(x=>x.Division)
                    .Include(x => x.Ekstraarbejder).ThenInclude(y => y.EA)
                    .Include(x => x.Kontraktarbejder)
                    .Include(x => x.Timer)
                    .Where(x => x.Id.Equals(id)).SingleOrDefaultAsync();
                List<string> photos = new List<string>();
                string path = _env.WebRootPath + "/CGJensen/Dagsrapporter/" + model.Id.ToString() + "/";
                List<string> pictures = new List<string>();
                if (Directory.Exists(path))
                {
                    var folder = Directory.EnumerateFiles(path)
                                     .Select(fn => Path.GetFileName(fn));

                    foreach (string file in folder)
                    {
                        if (file.Contains("_edit"))
                        {
                            pictures.Add(file);
                        }
                        else
                        {
                            string[] fileparts = file.Split(".");
                            if (!folder.Contains(fileparts[0] + "_edit." + fileparts[1]))
                            {
                                pictures.Add(file);
                            }
                        }
                    }
                }
                model.Photos = pictures;
                return new ViewAsPdf("_Dagsrapport", model);
            }
            else
            {
                return NotFound();
            }

        }
        [HttpGet]
        public async Task<IActionResult> Dagsrapport(int? id)
        {
            if (id != null)
            {
                var model = await _context.Dagsrapporter
                    .Include(x => x.SubProject)
                    .Include(x => x.Project).ThenInclude(x => x.Division)
                    .Include(x => x.Ekstraarbejder).ThenInclude(y => y.EA)
                    .Include(x => x.Kontraktarbejder)
                    .Include(x => x.Timer)
                    .Where(x => x.Id.Equals(id)).SingleOrDefaultAsync();
                List<string> photos = new List<string>();
                string path = _env.WebRootPath + "/CGJensen/Dagsrapporter/" + model.Id.ToString() + "/";
                List<string> pictures = new List<string>();
                if (Directory.Exists(path))
                {
                    var folder = Directory.EnumerateFiles(path)
                                     .Select(fn => Path.GetFileName(fn));

                    foreach (string file in folder)
                    {
                        if (file.Contains("_edit"))
                        {
                            pictures.Add(file);
                        }
                        else
                        {
                            string[] fileparts = file.Split(".");
                            if (!folder.Contains(fileparts[0] + "_edit." + fileparts[1]))
                            {
                                pictures.Add(file);
                            }
                        }
                    }
                }
                model.Photos = pictures;
                return View("_Dagsrapportview", model);
            }
            else
            {
                return NotFound();
            }

        }
        [HttpGet]
        public async Task<IActionResult> Dagsrapporter()
        {
            var model = await _context.Dagsrapporter.Include(x => x.Project).Include(x => x.SubProject).ToListAsync();
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Dagsrapporter_PDF(DateTime startdate, DateTime enddate)
        {
            var model = await _context.Dagsrapporter
                    .Include(x => x.SubProject)
                    .Include(x => x.Project).ThenInclude(x => x.Division)
                    .Include(x => x.Ekstraarbejder)
                    .Include(x => x.Kontraktarbejder)
                    .Include(x => x.Timer)
                    .Where(x => x.Dato.Date >= startdate.Date && x.Dato.Date <= enddate.Date).ToListAsync();
            foreach(var DR in model)
            {
                List<string> photos = new List<string>();
                string path = _env.WebRootPath + "/CGJensen/Dagsrapporter/" + DR.Id.ToString() + "/";
                List<string> pictures = new List<string>();
                if (Directory.Exists(path))
                {
                    var folder = Directory.EnumerateFiles(path)
                                     .Select(fn => Path.GetFileName(fn));

                    foreach (string file in folder)
                    {
                        if (file.Contains("_edit"))
                        {
                            pictures.Add(file);
                        }
                        else
                        {
                            string[] fileparts = file.Split(".");
                            if (!folder.Contains(fileparts[0] + "_edit." + fileparts[1]))
                            {
                                pictures.Add(file);
                            }
                        }
                    }
                }
                DR.Photos = pictures;
            }
            return new ViewAsPdf("_Dagsrapporter", model);
        }
        [HttpGet]
        public async Task<IActionResult> Create_Dagsrapport()
        {
            CGJDailyReportVM model = new CGJDailyReportVM();
            model.Timer = new List<TimeRegistrering>();
            model.Ekstraarbejder = new List<EkstraArbejde>();
            model.Kontraktarbejder = new List<KontraktArbejde>();
            for (int i = 0; i < 10; i++)
            {
                TimeRegistrering tr = new TimeRegistrering();
                model.Timer.Add(tr);
                KontraktArbejde ka = new KontraktArbejde();
                model.Kontraktarbejder.Add(ka);
                if (i < 5)
                {
                    EkstraArbejde ea = new EkstraArbejde();
                    model.Ekstraarbejder.Add(ea);
                }
            }
            var Structures = await (from s in _context.Structures select new { Id = s.Id, Text = s.Location + ":" + s.Name }).ToListAsync();
            var Buildings = await (from s in _context.Buildings select new { Id = s.Id, Text = s.Location + ":" + s.Address }).ToListAsync();
            var RoadSections = await (from s in _context.RoadSections select new { Id = s.Id, Text = s.Location + ":" + s.Name }).ToListAsync();
            var Junctions = await (from s in _context.Junctions select new { Id = s.Id, Text = s.Location + ":" + s.Name }).ToListAsync();
            var Accommodations = await (from s in _context.AccomodationWorks select new { Id = s.Id, Text = s.Location + ":" + s.Name }).ToListAsync();
            var SiteClear1 = await (from s in _context.SiteClearancesBetweenContractBorders select new { Id = s.Id, Text = s.Location + ":" + s.Name }).ToListAsync();
            var SiteClear2 = await (from s in _context.SiteClearancesForUtilityReolocations select new { Id = s.Id, Text = s.Location + ":" + s.Name }).ToListAsync();
            var EAs = await (from s in _context.EAs select new { Id = s.EAnr, Text = s.EAnr + ":" + s.Opgave }).OrderBy(x=>x.Id).ToListAsync();
            var subprojects = await (from sp in _context.SubProjects.Include(x => x.Project).Where(x => Convert.ToInt32(x.Project.Id).Equals(56))
                                     select new
                                     { Id = sp.Id, Text = sp.Project.ProjectNr + "-" + sp.SubProjectNr + " : " + sp.Name }).ToListAsync();

            ViewData["SubProjectId"] = new SelectList(subprojects, "Id", "Text");
            ViewData["ProtocolId"] = new SelectList(_context.Protocols.OrderBy(x=>Convert.ToInt32(x.Protocolnr)), "Id", "Protocolnr");
            ViewData["StructureId"] = new SelectList(Structures, "Id", "Text");
            ViewData["BuildingId"] = new SelectList(Buildings, "Id", "Text");
            ViewData["RoadSectionId"] = new SelectList(RoadSections, "Id", "Text");
            ViewData["JunctionId"] = new SelectList(Junctions, "Id", "Text");
            ViewData["AccomodationWorkId"] = new SelectList(Accommodations, "Id", "Text");
            ViewData["SiteClearancesBetweenContractBorderId"] = new SelectList(SiteClear1, "Id", "Text");
            ViewData["SiteClearanceForUtilityRelocationId"] = new SelectList(SiteClear2, "Id", "Text");
            ViewData["EAId"] = new SelectList(EAs, "Id", "Text");
            return View("DagsRapport", model);
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,Guest,International")]
        [HttpGet]
        public string GetMap(string theid, string type)
        {
            int Id = Convert.ToInt32(theid);
            switch (type)
            {
                case "Building":
                    var buildings = _context.Buildings.Where(x => x.Id.Equals(Id)).SingleOrDefault();
                    return buildings.path;
                case "Junction":
                    var junctions = _context.Junctions.Where(x => x.Id.Equals(Id)).SingleOrDefault();
                    return junctions.path;
                case "RoadSection":
                    var roads = _context.RoadSections.Where(x => x.Id.Equals(Id)).SingleOrDefault();
                    return roads.path;
                case "Structure":
                    var structures = _context.Structures.Where(x => x.Id.Equals(Id)).SingleOrDefault();
                    return structures.path;
                case "AccommodationWork":
                    var accommodations = _context.AccomodationWorks.Where(x => x.Id.Equals(Id)).SingleOrDefault();
                    return accommodations.path;
                case "SiteClearBorder":
                    var clearancesa = _context.SiteClearancesBetweenContractBorders.Where(x => x.Id.Equals(Id)).SingleOrDefault();
                    return clearancesa.path;
                case "SiteClearUtil":
                    var clearancesb = _context.SiteClearancesForUtilityReolocations.Where(x => x.Id.Equals(Id)).SingleOrDefault();
                    return clearancesb.path;
                default:
                    return "";
            }
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,Guest,International")]
        [HttpGet]
        public JsonResult GetProtocol(string theId)
        {
            int Id = Convert.ToInt32(theId);
            var subproject = _context.SubProjects.Where(x => x.Id.Equals(Id)).SingleOrDefault();
            var protocol = _context.Protocols.Where(x => x.Protocolnr.Equals(subproject.ProtokolId.ToString())).SingleOrDefault();
            return Json(protocol);
        }
        [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,Guest,International")]
        [HttpGet]
        public JsonResult GetList(string theId, string type)
        {
            int Id = Convert.ToInt32(theId);
            switch (type)
            {
                case "B":
                    var buildings = _context.Buildings.Where(x => x.ProtocolId.Equals(Id)).OrderBy(x => x.Location).ToList();
                    return Json(buildings);
                case "J":
                    var junctions = _context.Junctions.Where(x => x.ProtocolId.Equals(Id)).OrderBy(x => x.Location).ToList();
                    return Json(junctions);
                case "R":
                    var roads = _context.RoadSections.Where(x => x.ProtocolId.Equals(Id)).OrderBy(x => x.Location).ToList();
                    return Json(roads);
                case "C":
                    var structures = _context.Structures.Where(x => x.ProtocolId.Equals(Id)).OrderBy(x => x.Location).ToList();
                    return Json(structures);
                case "A":
                    var accommodations = _context.AccomodationWorks.Where(x => x.ProtocolId.Equals(Id)).OrderBy(x => x.Location).ToList();
                    return Json(accommodations);
                case "SICB":
                    var clearancesa = _context.SiteClearancesBetweenContractBorders.Where(x => x.ProtocolId.Equals(Id)).OrderBy(x => x.Location).ToList();
                    return Json(clearancesa);
                case "SICA":
                    var clearancesb = _context.SiteClearancesForUtilityReolocations.Where(x => x.ProtocolId.Equals(Id)).OrderBy(x => x.Location).ToList();
                    return Json(clearancesb);
                case "P":
                    List<Protocol> protocols = new List<Protocol>();
                    if (Id > 28)
                    {
                        protocols = _context.Protocols.Where(x => x.Protocolnr.Equals("9") || x.Protocolnr.Equals("10") || x.Protocolnr.Equals("11")).OrderBy(x => x.Name).ToList();
                        return Json(protocols);
                    }
                    else
                    {
                        protocols = _context.Protocols.Where(x => x.Protocolnr.Equals("12") || x.Protocolnr.Equals("13") || x.Protocolnr.Equals("14")).OrderBy(x => x.Name).ToList();
                        return Json(protocols);
                    }

                default:
                    var clearancesbb = _context.Junctions.Where(x => x.ProtocolId.Equals(Id)).ToList();
                    return Json(clearancesbb);
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult UploadList()
        {
            return View();
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult UploadListSager()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> UploadListSager(IFormFile postedFile)
        {
            if (postedFile != null)
            {
                string fileExtension = Path.GetExtension(postedFile.FileName);
                if (fileExtension != ".csv")
                {
                    return View("Index");
                }
                using (var sreader = new StreamReader(postedFile.OpenReadStream(), System.Text.Encoding.GetEncoding("iso-8859-1")))
                {
                    while (!sreader.EndOfStream)
                    {
                        string rawrow = sreader.ReadLine();
                        string[] rows = rawrow.Split(';');
                        var subproject = await _context.SubProjects.Where(x => x.ProjectId.Equals(56) && x.SubProjectNr.Equals(rows[0])).FirstOrDefaultAsync();
                        if (subproject == null)
                        {
                            if (rows[2].Contains(","))
                            {
                                string[] protocolids = rows[2].Split(",");
                                for (int i = 0; i < protocolids.Length; i++)
                                {
                                    SubProject sb = new SubProject();
                                    sb.ProjectId = 56;
                                    sb.Name = protocolids[i] + ": " + rows[1];
                                    sb.SubProjectNr = rows[0];
                                    sb.ProtokolId = Convert.ToInt32(protocolids[i]);
                                    sb.Type = "";
                                    sb.Latitude = 0;
                                    sb.Longitude = 0;
                                    sb.ClientContact = "rml@hj-as.dk";
                                    sb.ClientEmail = "rml@hj-as.dk";
                                    _context.Add(sb);
                                    await _context.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                SubProject sb = new SubProject();
                                sb.ProjectId = 56;
                                sb.Name = rows[1];
                                sb.SubProjectNr = rows[0];
                                sb.ProtokolId = Convert.ToInt32(rows[2]);
                                sb.Type = "";
                                sb.Latitude = 0;
                                sb.Longitude = 0;
                                sb.ClientContact = "rml@hj-as.dk";
                                sb.ClientEmail = "rml@hj-as.dk";
                                _context.Add(sb);
                                await _context.SaveChangesAsync();
                            }

                        }
                    }
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                ViewBag.Message = "Please select the file first to upload.";
            }
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> UploadList(IFormFile postedFile)
        {
            if (postedFile != null)
            {
                string fileExtension = Path.GetExtension(postedFile.FileName);
                if (fileExtension != ".csv")
                {
                    return View("Index");
                }
                using (var sreader = new StreamReader(postedFile.OpenReadStream(), System.Text.Encoding.GetEncoding("iso-8859-1")))
                {
                    while (!sreader.EndOfStream)
                    {
                        string rawrow = sreader.ReadLine();
                        string[] rows = rawrow.Split(';');
                        var protocol = await _context.Protocols.Where(x => x.Protocolnr.Equals(rows[0])).FirstOrDefaultAsync();
                        if (protocol == null)
                        {
                            Protocol newProt = new Protocol();
                            newProt.Protocolnr = rows[0];
                            newProt.Name = rows[0];
                            _context.Add(newProt);
                            await _context.SaveChangesAsync();
                            protocol = await _context.Protocols.LastAsync();
                        }
                        string location = rows[1].Split(":")[0].Trim();
                        string Address = rows[1].Split(":")[1].Trim();
                        if (rows[1].StartsWith("B"))
                        {
                            Building b = new Building();
                            b.ProtocolId = protocol.Id;
                            b.Location = location;
                            b.Address = Address;
                            _context.Add(b);
                            await _context.SaveChangesAsync();
                        }
                        else if (rows[1].StartsWith("CON"))
                        {
                            Structure CON = new Structure();
                            CON.ProtocolId = protocol.Id;
                            CON.Location = location;
                            CON.Name = Address;
                            _context.Add(CON);
                            await _context.SaveChangesAsync();
                        }
                        else if (rows[1].StartsWith("J"))
                        {
                            Junction Jun = new Junction();
                            Jun.ProtocolId = protocol.Id;
                            Jun.Location = location;
                            Jun.Name = Address;
                            _context.Add(Jun);
                            await _context.SaveChangesAsync();
                        }
                        else if (rows[1].StartsWith("R"))
                        {
                            RoadSection Road = new RoadSection();
                            Road.ProtocolId = protocol.Id;
                            Road.Location = location;
                            Road.Name = Address;
                            _context.Add(Road);
                            await _context.SaveChangesAsync();
                        }
                        else if (rows[1].StartsWith("A"))
                        {
                            AccomodationWork A = new AccomodationWork();
                            A.ProtocolId = protocol.Id;
                            A.Location = location;
                            A.Name = Address;
                            _context.Add(A);
                            await _context.SaveChangesAsync();
                        }
                        else if (rows[1].StartsWith("SICB"))
                        {
                            SiteClearancesBetweenContractBorder SICB = new SiteClearancesBetweenContractBorder();
                            SICB.ProtocolId = protocol.Id;
                            SICB.Location = location;
                            SICB.Name = Address;
                            _context.Add(SICB);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            SiteClearanceForUtilityRelocation SICA = new SiteClearanceForUtilityRelocation();
                            SICA.ProtocolId = protocol.Id;
                            SICA.Location = location;
                            SICA.Name = Address;
                            _context.Add(SICA);
                            await _context.SaveChangesAsync();
                        }
                    }
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                ViewBag.Message = "Please select the file first to upload.";
            }
            return View();
        }
        public MemoryStream PdfToImage(byte[] pdfBytes)
        {
            MemoryStream memoryStream = new MemoryStream();
            MagickImage imgBackdrop;
            MagickColor backdropColor = MagickColors.White; // replace transparent pixels with this color 
            int pdfPageNum = 0; // first page is 0

            using (IDocLib pdfLibrary = DocLib.Instance)
            {
                using (var docReader = pdfLibrary.GetDocReader(pdfBytes, new Docnet.Core.Models.PageDimensions(1.0d)))
                {
                    using (var pageReader = docReader.GetPageReader(pdfPageNum))
                    {
                        var rawBytes = pageReader.GetImage(); // Returns image bytes as B-G-R-A ordered list.
                        rawBytes = RearrangeBytesToRGBA(rawBytes);
                        var width = pageReader.GetPageWidth();
                        var height = pageReader.GetPageHeight();

                        // specify that we are reading a byte array of colors in R-G-B-A order.
                        PixelReadSettings pixelReadSettings = new PixelReadSettings(width, height, StorageType.Char, PixelMapping.RGBA);
                        using (MagickImage imgPdfOverlay = new MagickImage(rawBytes, pixelReadSettings))
                        {
                            // turn transparent pixels into backdrop color using composite: http://www.imagemagick.org/Usage/compose/#compose
                            imgBackdrop = new MagickImage(backdropColor, width, height);
                            imgBackdrop.Composite(imgPdfOverlay, CompositeOperator.Over);
                        }
                    }
                }
            }


            imgBackdrop.Write(memoryStream, MagickFormat.Png);
            imgBackdrop.Dispose();
            memoryStream.Position = 0;
            return memoryStream;
        }

        private byte[] RearrangeBytesToRGBA(byte[] BGRABytes)
        {
            var max = BGRABytes.Length;
            var RGBABytes = new byte[max];
            var idx = 0;
            byte r;
            byte g;
            byte b;
            byte a;
            while (idx < max)
            {
                // get colors in original order: B G R A
                b = BGRABytes[idx];
                g = BGRABytes[idx + 1];
                r = BGRABytes[idx + 2];
                a = BGRABytes[idx + 3];

                // re-arrange to be in new order: R G B A
                RGBABytes[idx] = r;
                RGBABytes[idx + 1] = g;
                RGBABytes[idx + 2] = b;
                RGBABytes[idx + 3] = a;

                idx += 4;
            }
            return RGBABytes;
        }
        [RequestSizeLimit(900000000)]
        [Authorize]
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
