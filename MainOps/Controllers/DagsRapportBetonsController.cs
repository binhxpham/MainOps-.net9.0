using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MainOps.Data;
using MainOps.Models.CGJClassesBeton;
using MainOps.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.IO;
using MainOps.ExtensionMethods;
using Microsoft.AspNetCore.Hosting;
using Rotativa.AspNetCore;
using System.IO.Compression;
using Microsoft.AspNetCore.Authorization;

namespace MainOps.Controllers
{
    public class DagsRapportBetonsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public DagsRapportBetonsController(DataContext context,UserManager<ApplicationUser> userManager, IWebHostEnvironment env):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // GET: DagsRapportBetons
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.DivisionId != 10 && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this part of MainOps" });
            }
            ViewData["ProjectId"] = await GetProjectList();
            var dataContext = _context.DagsRapporterBeton.Include(d => d.Project).Include(d => d.SubProject).Include(x => x.Fotos);
            return View(await dataContext.ToListAsync());
        }
        [HttpGet]
        [Authorize(Roles = "Admin,DivisionAdmin,Manager")]
        public async Task<IActionResult> GetZipFile(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.Active == false)
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You are inactive" });
            }
            if (id != null)
            {
                string path = _env.WebRootPath + "/CGJensen/Betonrenovering/Dagsrapporter/" + id.ToString() + "/";

                var botsFolderPath = Path.Combine(_env.WebRootPath, "CGJensen", "Betonrenovering", "Dagsrapporter", id.ToString());
                var botFilePaths = Directory.GetFiles(botsFolderPath);
                var zipFileMemoryStream = new MemoryStream();
                using (ZipArchive archive = new ZipArchive(zipFileMemoryStream, ZipArchiveMode.Update, leaveOpen: true))
                {
                    foreach (var botFilePath in botFilePaths)
                    {
                        var botFileName = Path.GetFileName(botFilePath);
                        var entry = archive.CreateEntry(botFileName);
                        using (var entryStream = entry.Open())
                        using (var fileStream = System.IO.File.OpenRead(botFilePath))
                        {
                            await fileStream.CopyToAsync(entryStream);
                        }
                    }
                }

                zipFileMemoryStream.Seek(0, SeekOrigin.Begin);
                return File(zipFileMemoryStream, "application/octet-stream", "Fotos.zip");
            }
            else { return NotFound(); }

        }
        // GET: DagsRapportBetons/Details/5
        public async Task<IActionResult> DagsseddelPDF(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.DivisionId != 10 && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this part of MainOps" });
            }
            if (id == null)
            {
                return NotFound();
            }

            var dagsRapportBeton = await _context.DagsRapporterBeton
                .Include(d => d.Project).ThenInclude(x => x.Division)
                .Include(d => d.SubProject)
                .Include(x => x.Timer)
                .Include(x => x.TimerEkstraarbejder)
                .Include(x => x.Materiel).ThenInclude(x => x.Materiel)
                .Include(x => x.Kontraktarbejder)
                .Include(x => x.Fotos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dagsRapportBeton == null)
            {
                return NotFound();
            }

            return new ViewAsPdf("_details",dagsRapportBeton);
        }
        public async Task<IActionResult> UdskrivDagssedler(int? ProjectId,DateTime? startdate,DateTime? enddate)
        {
            if(ProjectId == null)
            {
                return NotFound();
            }
            DateTime start;
            DateTime end;
            if(startdate != null)
            {
                start = Convert.ToDateTime(startdate).Date;
            }
            else
            {
                start = DateTime.Now.AddYears(-10).Date;
            }
            if (enddate != null)
            {
                end = Convert.ToDateTime(enddate).Date;
            }
            else
            {
                end = DateTime.Now.Date;
            }
            var dagssedler = await _context.DagsRapporterBeton
                .Include(d => d.Project).ThenInclude(x => x.Division)
                .Include(d => d.SubProject)
                .Include(x => x.Timer)
                .Include(x => x.TimerEkstraarbejder)
                .Include(x => x.Materiel).ThenInclude(x => x.Materiel)
                .Include(x => x.Kontraktarbejder)
                .Include(x => x.Fotos)
                .Where(x => x.Dato.Date >= start.Date && x.Dato.Date <= end.Date && x.ProjectId.Equals(ProjectId))
                .ToListAsync();
            return new ViewAsPdf("_dagssedler", dagssedler);
        }
        // GET: DagsRapportBetons/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.DivisionId != 10 && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this part of MainOps" });
            }
            ViewData["ProjectId"] = await GetProjectList();
            DagsrapportBetonVM model = new DagsrapportBetonVM();
            var materieller = await _context.Materieller.ToListAsync();
            foreach(var mat in materieller.OrderBy(x => x.Materiellet))
            {
                MaterielNumber MN = new MaterielNumber();
                MN.Antal = 0;
                MN.MaterielId = mat.Id;
                MN.Materiel = mat;
                model.Materiel.Add(MN);
            }
            //ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Id");
            return View(model);
        }

        // POST: DagsRapportBetons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(900000000)]
        public async Task<IActionResult> Create(DagsrapportBetonVM dagsRapportBeton, IFormFile[] files)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.DivisionId != 10 && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this part of MainOps" });
            }
            if (ModelState.IsValid)
            {
                dagsRapportBeton.UdarbjedetAf = user.full_name();
                DagsRapportBeton DR = new DagsRapportBeton(dagsRapportBeton);
                _context.Add(DR);
                await _context.SaveChangesAsync();
                var lastadded = await _context.DagsRapporterBeton.LastAsync();
                foreach(var item in dagsRapportBeton.Kontraktarbejder)
                {
                    if(item.EgetMateriel != "" || item.UL_Materiel != "") { 
                        item.DagsRapportBetonId = lastadded.Id;
                        _context.Add(item);
                    }
                }
                foreach (var item in dagsRapportBeton.Timer)
                {
                    if (item.Navn != null && item.Navn != "")
                    {
                        item.DagsRapportBetonId = lastadded.Id;
                        _context.Add(item);
                    }
                }
                foreach (var item in dagsRapportBeton.TimerEkstraarbejder)
                {
                    if(item.Navn != null && item.Navn != "") {
                        item.DagsRapportBetonId = lastadded.Id;
                        _context.Add(item);
                    }
                }
                foreach (var item in dagsRapportBeton.Materiel)
                {
                   
                        item.DagsRapportBetonId = lastadded.Id;
                        _context.Add(item);
                    
                }

                await _context.SaveChangesAsync();
                var directory2 = _env.WebRootPath + "\\CGJensen\\Betonrenovering\\Dagsrapporter\\" + lastadded.Id.ToString() + "\\";
                if (!Directory.Exists(directory2))
                {
                    Directory.CreateDirectory(directory2);
                }
                foreach (IFormFile photo in files)
                {
                    var path = Path.Combine(directory2, photo.FileName);
                    var path2 = Path.Combine(directory2, photo.FileName.Split(".")[0] + "_edit." + photo.FileName.Split(".")[1]);
                    PhotoFileBeton betonphoto = new PhotoFileBeton { Path = path, TimeStamp = lastadded.Dato, DagsRapportBetonId = lastadded.Id };
                    _context.Add(betonphoto);
                    var stream = new FileStream(path, FileMode.Create);
                    await photo.CopyToAsync(stream);
                    stream.Close();
                    if (path.ToLower().Contains(".jpg") || path.ToLower().Contains(".jpeg"))
                    {
                        PhotoExtensions.SaveAndCompressJpeg(path, 85);
                    }
                }
                await _context.SaveChangesAsync();
                foreach (var file in files.Where(x => x.FileName.ToLower().Contains(".heic")))
                {
                    PhotoExtensions.ConvertHEICtoJPG(_env.WebRootPath + "/CGJensen/Betonrenovering/Dagsrapporter/" + lastadded.Id.ToString() + "/" + file.FileName);
                }
               
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = await GetProjectList();// new SelectList(_context.Projects, "Id", "Abbreviation", dagsRapportBeton.ProjectId);
            //ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Id", dagsRapportBeton.SubProjectId);
            return View(dagsRapportBeton);
        }

        // GET: DagsRapportBetons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.DivisionId != 10 && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this part of MainOps" });
            }
            if (id == null)
            {
                return NotFound();
            }

            var dagsRapportBeton = await _context.DagsRapporterBeton
                .Include(x => x.Timer)
                .Include(x => x.TimerEkstraarbejder)
                .Include(x => x.Kontraktarbejder)
                .Include(x => x.Materiel).ThenInclude(x => x.Materiel)
                .SingleOrDefaultAsync(x => x.Id.Equals(id));
            if (dagsRapportBeton == null)
            {
                return NotFound();
            }
            dagsRapportBeton.Timer.OrderBy(x => x.Navn);
            dagsRapportBeton.TimerEkstraarbejder.OrderBy(x => x.Navn);
            dagsRapportBeton.Kontraktarbejder.OrderBy(x => x.EgetMateriel).ThenBy(x => x.UL_Firma).ThenBy(x => x.UL_Materiel);
            var materieller = await _context.Materieller.ToListAsync();
            foreach (var mat in materieller.OrderBy(x => x.Materiellet))
            {
                if(dagsRapportBeton.Materiel.SingleOrDefault(x => x.MaterielId.Equals(mat.Id)) == null){
                    MaterielNumber MN = new MaterielNumber();
                    MN.Antal = 0;
                    MN.MaterielId = mat.Id;
                    MN.Materiel = mat;
                    dagsRapportBeton.Materiel.Add(MN);
                }
            }
            dagsRapportBeton.Materiel.OrderBy(x => x.Materiel);
            
            DagsrapportBetonVM model = new DagsrapportBetonVM(dagsRapportBeton);
            
            ViewData["ProjectId"] = await GetProjectList(); //new SelectList(_context.Projects, "Id", "Abbreviation", dagsRapportBeton.ProjectId);
            //ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Id", dagsRapportBeton.SubProjectId);
            return View(model);
        }

        // POST: DagsRapportBetons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(900000000)]
        public async Task<IActionResult> Edit(DagsrapportBetonVM model,IFormFile[] files)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.DivisionId != 10 && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this part of MainOps" });
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    var dr = await _context.DagsRapporterBeton.Include(x => x.Kontraktarbejder).Include(x => x.Timer).Include(x => x.TimerEkstraarbejder).Include(x => x.Materiel).ThenInclude(x => x.Materiel).SingleOrDefaultAsync(x => x.Id.Equals(model.Id));
                    dr.Id = model.Id;
                    dr.Dato = model.Dato;
                    dr.ForSaetter = model.ForSaetter;
                    dr.Haard_Vind = model.Haard_Vind;
                    dr.Jaevn_Vind = model.Jaevn_Vind;
                    dr.ProjectId = model.ProjectId;
                    dr.SubProjectId = model.SubProjectId;
                    dr.Svag_Vind = model.Svag_Vind;
                    dr.Temperatur_kl_otte = model.Temperatur_kl_otte;
                    dr.Temperatur_kl_tolv = model.Temperatur_kl_tolv;
                    dr.UdarbjedetAf = model.UdarbjedetAf;
                    dr.Vindstille = model.Vindstille;
                    dr.Vejr = model.Vejr;
                    dr.Regn_sne = model.Regn_sne;
                    dr.Skyet = model.Skyet;
                    dr.Sol = model.Sol;
                    dr.UddybendeNoter = model.UddybendeNoter;
                    dr.OverSkyet = model.OverSkyet;
                    _context.Update(dr);
                    foreach (var item in dr.Materiel)
                    {
                        _context.Remove(item);
                    }
                    foreach(var item in dr.Kontraktarbejder)
                    {
                        _context.Remove(item);
                    }
                    foreach(var item in dr.Timer)
                    {
                        _context.Remove(item);

                    }
                    foreach(var item in dr.TimerEkstraarbejder)
                    {
                        _context.Remove(item);
                    }
                    foreach(var item in dr.Materiel)
                    {
                        _context.Remove(item);
                    }
                    await _context.SaveChangesAsync();
                    foreach(var item in model.Timer)
                    {
                        if(item.Navn != null && item.Navn != "") {
                            item.DagsRapportBetonId = dr.Id;
                            _context.Add(item);
                        }
                    }
                    foreach (var item in model.TimerEkstraarbejder)
                    {
                        if (item.Navn != null && item.Navn != "")
                        {
                            item.DagsRapportBetonId = dr.Id;
                            _context.Add(item);
                        }
                    }
                    foreach (var item in model.Kontraktarbejder)
                    {
                        if ((item.EgetMateriel != null && item.EgetMateriel != "") || (item.UL_Firma != null && item.UL_Firma != "") || (item.UL_Materiel != null && item.UL_Materiel != ""))
                        {
                            item.DagsRapportBetonId = dr.Id;
                            _context.Add(item);
                        }
                    }
                    foreach(var item in model.Materiel)
                    {
                        if(item.Antal > 0) {
                            item.DagsRapportBetonId = dr.Id;
                            _context.Add(item);
                        }
                    }
                    await _context.SaveChangesAsync();
                    var directory2 = _env.WebRootPath + "\\CGJensen\\Betonrenovering\\Dagsrapporter\\" + model.Id.ToString() + "\\";
                    if (!Directory.Exists(directory2))
                    {
                        Directory.CreateDirectory(directory2);
                    }
                    foreach (IFormFile photo in files)
                    {
                        var path = Path.Combine(directory2, photo.FileName);
                        var path2 = Path.Combine(directory2, photo.FileName.Split(".")[0] + "_edit." + photo.FileName.Split(".")[1]);
                        PhotoFileBeton betonphoto = new PhotoFileBeton { Path = path, TimeStamp = model.Dato, DagsRapportBetonId = model.Id };
                        _context.Add(betonphoto);
                        var stream = new FileStream(path, FileMode.Create);
                        await photo.CopyToAsync(stream);
                        stream.Close();
                        if (path.ToLower().Contains(".jpg") || path.ToLower().Contains(".jpeg"))
                        {
                            PhotoExtensions.SaveAndCompressJpeg(path, 85);
                        }
                    }
                    await _context.SaveChangesAsync();
                    foreach (var file in files.Where(x => x.FileName.ToLower().Contains(".heic")))
                    {
                        PhotoExtensions.ConvertHEICtoJPG(_env.WebRootPath + "/CGJensen/Betonrenovering/Dagsrapporter/" + model.Id.ToString() + "/" + file.FileName);
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DagsRapportBetonExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                ViewData["ProjectId"] = await GetProjectList();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = await GetProjectList(); // new SelectList(_context.Projects, "Id", "Abbreviation", dagsRapportBeton.ProjectId);
            //ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Id", dagsRapportBeton.SubProjectId);
            return View(model);
        }

        // GET: DagsRapportBetons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.DivisionId != 10 && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this part of MainOps" });
            }
            if (id == null)
            {
                return NotFound();
            }

            var dagsRapportBeton = await _context.DagsRapporterBeton
                .Include(d => d.Project)
                .Include(d => d.SubProject)          
                .FirstOrDefaultAsync(m => m.Id == id);
           
            if (dagsRapportBeton == null)
            {
                return NotFound();
            }

            return View(dagsRapportBeton);
        }

        // POST: DagsRapportBetons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.DivisionId != 10 && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this part of MainOps" });
            }
            var dagsRapportBeton = await _context.DagsRapporterBeton
                .Include(x => x.Timer)
                .Include(x => x.TimerEkstraarbejder)
                .Include(x => x.Kontraktarbejder)
                .Include(x => x.Materiel).ThenInclude(x => x.Materiel)
                .SingleOrDefaultAsync(x => x.Id.Equals(id));
            foreach (var item in dagsRapportBeton.Kontraktarbejder)
            {
                _context.Remove(item);
            }
            foreach (var item in dagsRapportBeton.Timer)
            {
                _context.Remove(item);
            }
            foreach (var item in dagsRapportBeton.TimerEkstraarbejder)
            {
                _context.Remove(item);
            }
            foreach (var item in dagsRapportBeton.Materiel)
            {
                _context.Remove(item);
            }
            await _context.SaveChangesAsync();
            _context.DagsRapporterBeton.Remove(dagsRapportBeton);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DagsRapportBetonExists(int id)
        {
            return _context.DagsRapporterBeton.Any(e => e.Id == id);
        }
    }
}
