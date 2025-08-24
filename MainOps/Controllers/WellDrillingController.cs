using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MainOps.Data;
using MainOps.Models;
using MainOps.Resources;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Rotativa.AspNetCore;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,ProjectMember,Member,DivisionAdmin,Supervisor,ExternalDriller")]
    public class WellDrillingController : BaseController
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LocService _SharedLocalizer;

        public WellDrillingController(DataContext context, IWebHostEnvironment env, UserManager<ApplicationUser> userManager, LocService loc):base(context,userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
            _SharedLocalizer = loc;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("ExternalDriller"))
            {
                var wells = await _context.Wells.Include(x => x.Project).Include(x => x.SubProject).Include(x => x.WellType)
                         .Where(x => x.Done_By.Equals(user.full_name())).OrderByDescending(x => x.Drill_Date_Start).ToListAsync();

                return View("Wells", wells);
            }
            else//\Views\\WellDrilling\\DrillWell.cshtml:line 279\r\n  
            {
                var wells = await _context.Wells.Include(x => x.Project).Include(x => x.SubProject).Include(x => x.WellType)
                        .Where(x => x.DivisionId.Equals(user.DivisionId)).OrderByDescending(x => x.Drill_Date_Start).ToListAsync();

                return View("Wells", wells);

            }

        }
        [Authorize(Roles = "Admin,ProjectMember,Member,DivisionAdmin,Supervisor")]
        public async Task<IActionResult> Create_Instruction()
        {
            WellDrillingInstructionVM instruction = new WellDrillingInstructionVM();
            var terrain = await _context.Layers.Where(x => x.LayerType.Contains("Terr")).FirstOrDefaultAsync();
            List<LayerType> Layers = new List<LayerType>();
            LayerType Layer1 = new LayerType();
            Layer1.LayerNumber = 1;
            Layer1.Percent = (decimal)1.0;
            Layer1.LayerId = terrain.Id;
            Layer1.Layer = terrain;
            Layers.Add(Layer1);
            for(int i = 2;i <= 10; i++)
            {
                LayerType Lay = new LayerType();
                Lay.LayerNumber = i;
                Lay.Percent = (decimal)10.0;
                Layers.Add(Lay);
            }
            List<BentoniteLayer> BentoniteLayers = new List<BentoniteLayer>();
            for (int i = 1; i <= 10; i++)
            {
                BentoniteLayer Lay = new BentoniteLayer();
                Lay.Over = 0.0;
                Lay.Under = 0.0;
                BentoniteLayers.Add(Lay);
            }
            List<Filter1Layer> Filter1Layers = new List<Filter1Layer>();
            List<Filter2Layer> Filter2Layers = new List<Filter2Layer>();
            for (int i = 1; i <= 1; i++)
            {
                Filter1Layer Lay = new Filter1Layer();
                Lay.Under = 0.0;
                Lay.Over = 0.0;
                Lay.Length = 0.0;
                Filter1Layers.Add(Lay);
            }
            for (int i = 1; i <= 1; i++)
            {
                Filter2Layer Lay = new Filter2Layer();
                Lay.Under = 0.0;
                Lay.Over = 0.0;
                Lay.Length = 0.0;
                Filter2Layers.Add(Lay);
            }
            instruction.BentoniteLayers = BentoniteLayers;
            instruction.Layers = Layers;
            instruction.Filter1_Layers = Filter1Layers;
            instruction.Filter2_Layers = Filter2Layers;
            ViewData["Layers"] = new SelectList(_context.Layers, "Id", "LayerType");
            ViewData["ProjectId"] = await GetProjectList();
            return View(instruction);
        }
        [HttpGet]
        [Authorize(Roles = "Admin,ProjectMember,Member,DivisionAdmin,Supervisor")]
        public async Task<IActionResult> DeleteInstruction(int? id)
        {
            if (id != null)
            {
                var user = await _userManager.GetUserAsync(User);
                var instruction = await _context.WellDrillingInstructions.Include(x => x.Project).SingleOrDefaultAsync(x => x.Id.Equals(id));
                if (instruction != null)
                {
                    if (instruction.Project.DivisionId.Equals(user.DivisionId) && (User.IsInRole("Admin") || User.IsInRole("DivisionAdmin") || User.IsInRole("Manager")))
                    {

                        return View("DeleteInstruction", instruction);
                    }
                    else
                    {
                        return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to delete this well" });
                    }
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
        [HttpGet]
        [Authorize(Roles = "Admin,ProjectMember,Member,DivisionAdmin,Supervisor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if(id != null)
            {
                var user = await _userManager.GetUserAsync(User);
                var well = await _context.Wells.Include(x => x.Project).SingleOrDefaultAsync(x => x.Id.Equals(id));
                if (well != null)
                {
                    if (well.Project.DivisionId.Equals(user.DivisionId) && (User.IsInRole("Admin") || User.IsInRole("DivisionAdmin") || User.IsInRole("Manager"))) {

                        return View("Delete", well);
                    }
                    else
                    {
                        return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to delete this well" });
                    }
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
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,DivisionAdmin,Supervisor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var well = await _context.Wells
                        .Include(x => x.BentoniteLayers).ThenInclude(x => x.CastingType)
                        .Include(x => x.SoilSamples)
                        .Include(x => x.FilterLayers)
                        .Include(x => x.SandLayers).ThenInclude(x => x.SandType)
                        .Include(x => x.WellLayers)
                        .SingleOrDefaultAsync(x => x.Id.Equals(id));
            foreach(var item in well.BentoniteLayers)
            {
                _context.Remove(item);
            }
            foreach(var item in well.SoilSamples)
            {
                _context.Remove(item);
            }
            foreach(var item in well.FilterLayers)
            {
                _context.Remove(item);
            }
            foreach(var item in well.SandLayers)
            {
                _context.Remove(item);
            }
            foreach(var item in well.WellLayers)
            {
                _context.Remove(item);
            }
            await _context.SaveChangesAsync();
            _context.Wells.Remove(well);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost, ActionName("DeleteInstruction")]
        [Authorize(Roles = "Admin,DivisionAdmin,Supervisor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInstructionConfirmed(int id)
        {
            var instruction = await _context.WellDrillingInstructions.SingleOrDefaultAsync(x => x.Id.Equals(id));
            var BentoniteLayers = await _context.BentoniteLayers.Where(x => x.WellDrillingInstructionId.Equals(id)).ToListAsync();
            var f1layers = await _context.Filter1Layers.Where(x => x.WellDrillingInstructionId.Equals(id)).ToListAsync();
            var f2layers = await _context.Filter2Layers.Where(x => x.WellDrillingInstructionId.Equals(id)).ToListAsync();
            var layertypes = await _context.LayerTypes.Where(x => x.WellDrillingInstructionId.Equals(id)).ToListAsync();
            foreach (var l in f2layers)
            {
                _context.Remove(l);
            }
            foreach (var l in f1layers)
            {
                _context.Remove(l);
            }
            foreach (var l in BentoniteLayers)
            {
                _context.Remove(l);
            }
            foreach (var lt in layertypes)
            {
                _context.Remove(lt);
            }
            await _context.SaveChangesAsync();
            _context.Remove(instruction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Instructions()
        {
            var user = await _userManager.GetUserAsync(User);
            var instructions = await _context.WellDrillingInstructions.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)).ToListAsync();
            return View(instructions);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAllInstructions()
        {
            if (User.IsInRole("Admin"))
            {
                var instructions = await _context.WellDrillingInstructions.ToListAsync();
                var BentoniteLayers = await _context.BentoniteLayers.ToListAsync();
                var f1layers = await _context.Filter1Layers.ToListAsync();
                var f2layers = await _context.Filter2Layers.ToListAsync();
                var layertypes = await _context.LayerTypes.ToListAsync();
                foreach(var l in f2layers)
                {
                    _context.Remove(l);
                }
                foreach (var l in f1layers)
                {
                    _context.Remove(l);
                }
                foreach (var l in BentoniteLayers)
                {
                    _context.Remove(l);
                }
                foreach(var lt in layertypes)
                {
                    _context.Remove(lt);
                }
                await _context.SaveChangesAsync();
                foreach(var ins in instructions)
                {
                    _context.Remove(ins);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin,DivisionAdmin,Supervisor")]
        public async Task<IActionResult> Create_Instruction(WellDrillingInstructionVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                WellDrillingInstruction new_Model = new WellDrillingInstruction(model);
                new_Model.DoneBy = user.full_name();
                _context.Add(new_Model);
                await _context.SaveChangesAsync();
                var lastadded = await _context.WellDrillingInstructions.LastAsync();
                foreach(LayerType l in model.Layers.Where(x=>x.LayerId != null))
                {
                    l.WellDrillingInstructionId = lastadded.Id;
                    _context.Add(l);
                }
                foreach(BentoniteLayer bl in model.BentoniteLayers.Where(x=>x.LayerTypeOverId != null && x.LayerTypeUnderId != null))
                {
                    bl.WellDrillingInstructionId = lastadded.Id;
                    _context.Add(bl);
                }
                foreach(Filter1Layer f1 in model.Filter1_Layers.Where(x => x.LayerTypeOverId != null || x.LayerTypeUnderId != null))
                {
                    f1.WellDrillingInstructionId = lastadded.Id;
                    _context.Add(f1);
                }
                foreach(Filter2Layer f2 in model.Filter2_Layers.Where(x=>x.LayerTypeOverId != null || x.LayerTypeUnderId != null))
                {
                    f2.WellDrillingInstructionId = lastadded.Id;
                    _context.Add(f2);
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> GetDrillingInstruction(int? id)
        {
            if(id != null)
            {
                var instruction = await _context.WellDrillingInstructions.Include(x=>x.Project).ThenInclude(x=>x.Division).Where(x => x.Id.Equals(id)).SingleOrDefaultAsync();
                var layertypes = await _context.LayerTypes.Include(x=>x.Layer).Where(x => x.WellDrillingInstructionId.Equals(id)).OrderBy(x=>x.LayerNumber).ToListAsync();
                var bentonitelayers = await _context.BentoniteLayers.Where(x => x.WellDrillingInstructionId.Equals(id)).ToListAsync();
                var filter1layers = await _context.Filter1Layers.Where(x => x.WellDrillingInstructionId.Equals(id)).ToListAsync();
                var filter2layers = await _context.Filter2Layers.Where(x => x.WellDrillingInstructionId.Equals(id)).ToListAsync();
                instruction.BentoniteLayers = bentonitelayers;
                instruction.Layers = layertypes;
                instruction.Filter1_Layers = filter1layers;
                instruction.Filter2_Layers = filter2layers;
                ViewData["Layers"] = new SelectList(_context.Layers, "Id", "LayerType");
                ViewData["ProjectId"] = await GetProjectList();
                return View("Instruction", instruction);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetDrillingInstructionPDF(int? id)
        {
            if (id != null)
            {
                var instruction = await _context.WellDrillingInstructions.Include(x => x.Project).ThenInclude(x => x.Division).Where(x => x.Id.Equals(id)).SingleOrDefaultAsync();
                var layertypes = await _context.LayerTypes.Include(x => x.Layer).Where(x => x.WellDrillingInstructionId.Equals(id)).OrderBy(x => x.LayerNumber).ToListAsync();
                var bentonitelayers = await _context.BentoniteLayers.Where(x => x.WellDrillingInstructionId.Equals(id)).ToListAsync();
                var filter1layers = await _context.Filter1Layers.Where(x => x.WellDrillingInstructionId.Equals(id)).ToListAsync();
                var filter2layers = await _context.Filter2Layers.Where(x => x.WellDrillingInstructionId.Equals(id)).ToListAsync();
                instruction.BentoniteLayers = bentonitelayers;
                instruction.Layers = layertypes;
                instruction.Filter1_Layers = filter1layers;
                instruction.Filter2_Layers = filter2layers;
                //ViewData["Layers"] = new SelectList(_context.Layers, "Id", "LayerType");

                //ViewData["ProjectId"] = await getProjectList();

                //string customSwitches = string.Format("--debug-javascript --no-stop-slow-scripts --javascript-delay 60000");
                return new ViewAsPdf("_Instruction", instruction);
                //{
                //    CustomSwitches = customSwitches
                //};
                //return View("_Instruction", instruction);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpGet]
        public async Task<IActionResult> ContinueDrillWell(int? id)
        {
            if(id != null)
            {
                var user = await _userManager.GetUserAsync(User);
                var well = await _context.Wells.Include(x => x.WellType)
                        //.Include(x => x.BentoniteLayers.OrderBy(y => y.meter_start))
                        //.Include(x => x.SoilSamples.OrderBy(y => y.sample_meter))Well
                        //.Include(x => x.FilterLayers.OrderBy(y => y.meter_start))
                        //.Include(x => x.SandLayers.OrderBy(y => y.meter_start))
                        //.Include(x => x.WellLayers.OrderBy(y => y.Start_m))
                        .SingleOrDefaultAsync(x => x.Id.Equals(id));
                if (User.IsInRole("ExternalDriller"))
                {
                    if (!well.Done_By.Equals(user.full_name()))
                    {
                        return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this object" });
                    }
                }
                well.BentoniteLayers = await _context.BentoniteWellLayers.Include(x => x.CastingType).Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.meter_start).ToListAsync();
                well.SoilSamples = await _context.SoilSamples.Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.sample_meter).ToListAsync();
                well.FilterLayers = await _context.FilterWellLayers.Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.meter_start).ToListAsync();
                well.SandLayers = await _context.SandWellLayers.Include(x => x.SandType).Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.meter_start).ToListAsync();
                well.WellLayers = await _context.WellLayers.Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.Start_m).ToListAsync();
                WellVM model = new WellVM(well);
                ViewData["Layers"] = new SelectList(_context.Layers.OrderBy(x => x.LayerType), "Id", "LayerType");
                ViewData["ProjectId"] = await GetProjectList();
                if(well.ProjectId != null)
                {
                    
                    if(well.SubProjectId != null)
                    {
                        ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => x.ProjectId.Equals(well.ProjectId)), "Id", "Name");
                        ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Include(x => x.Project).Include(x => x.MeasType)
                            .Where(x => 
                            x.MeasType.Type.ToLower().Equals("water level")
                            && x.ProjectId.Equals(well.ProjectId)
                            && (x.SubProjectId.HasValue == false || x.SubProjectId.Equals(well.SubProjectId))), "Name", "Name");
                    }
                    else
                    {
                        ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => x.ProjectId.Equals(well.ProjectId)), "Id", "Name");
                        ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Include(x => x.Project).Include(x => x.MeasType)
                            .Where(x => x.ProjectId.Equals(well.ProjectId) 
                        && x.MeasType.Type.ToLower().Equals("water level")), "Name", "Name");
                    }
                }
                ViewData["CoordSystems"] = new SelectList(_context.CoordSystems.OrderBy(x => x.system), "Id", "system");
                ViewData["SandTypeId"] = new SelectList(_context.SandTypes, "Id", "TypeOfSand");
                ViewData["CastingTypeId"] = new SelectList(_context.CastingTypes, "Id", "TypeOfCasting");
                ViewData["WellTypeId"] = new SelectList(_context.WellTypes.Where(x => x.ProjectId.Equals(well.ProjectId)), "Id", "Type");
                return View("DrillWell",model);
            }
            else
            {
                return NotFound();
            }
            
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GeneratePDF(int? id, string scale = null)
        {
            if (id != null)
            {
                if (scale != null)
                {
                    ViewData["heightfactor"] = Convert.ToDouble(scale);
                }
                else
                {
                    ViewData["heightfactor"] = 3.0;
                }
                var user = await _userManager.GetUserAsync(User);
                var well = await _context.Wells
                    .Include(x => x.Project).ThenInclude(x => x.Division)
                    .Include(x => x.CoordSystem)
                    .Include(x => x.SubProject)
                    .SingleOrDefaultAsync(x => x.Id.Equals(id));
                if (user.DivisionId.Equals(well.Project.DivisionId))
                {
                    well.BentoniteLayers = await _context.BentoniteWellLayers.Include(x => x.CastingType).Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.meter_start).ToListAsync();
                    well.SoilSamples = await _context.SoilSamples.Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.sample_meter).ToListAsync();
                    well.FilterLayers = await _context.FilterWellLayers.Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.meter_start).ToListAsync();
                    well.SandLayers = await _context.SandWellLayers.Include(x => x.SandType).Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.meter_start).ToListAsync();
                    well.WellLayers = await _context.WellLayers.Include(x => x.Layer).Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.Start_m).ToListAsync();
                    WellPdf model = new WellPdf(well);
                    model.heightfactor = Convert.ToDouble(scale);
                    string footer = "--footer-center \"" + "  " + _SharedLocalizer.GetLocalizedHtmlString("Page").Value + ":" + " [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 ";
                    return new ViewAsPdf("_BoreProfile", model)
                    {
                        PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                        PageSize = Rotativa.AspNetCore.Options.Size.A4,
                        CustomSwitches = footer
                    };
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this well" });
                }

            }
            else
            {
                return NotFound();
            }
            
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateSelectPDF(int? id, string scale = null)
        {
            if (id != null)
            {
                if (scale != null)
                {
                    ViewData["heightfactor"] = Convert.ToDouble(scale);
                }
                else
                {
                    ViewData["heightfactor"] = 3.0;
                }
                var user = await _userManager.GetUserAsync(User);
                var well = await _context.Wells
                    .Include(x => x.Project).ThenInclude(x => x.Division)
                    .Include(x => x.CoordSystem)
                    .Include(x => x.SubProject)
                    .SingleOrDefaultAsync(x => x.Id.Equals(id));
                if (user.DivisionId.Equals(well.Project.DivisionId))
                {
                    well.BentoniteLayers = await _context.BentoniteWellLayers.Include(x => x.CastingType).Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.meter_start).ToListAsync();
                    well.SoilSamples = await _context.SoilSamples.Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.sample_meter).ToListAsync();
                    well.FilterLayers = await _context.FilterWellLayers.Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.meter_start).ToListAsync();
                    well.SandLayers = await _context.SandWellLayers.Include(x => x.SandType).Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.meter_start).ToListAsync();
                    well.WellLayers = await _context.WellLayers.Include(x => x.Layer).Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.Start_m).ToListAsync();
                    WellPdf model = new WellPdf(well);
                    model.heightfactor = Convert.ToDouble(scale);
                    string footer = "--footer-center \"" + "  " + _SharedLocalizer.GetLocalizedHtmlString("Page").Value + ":" + " [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --zoom 1.2 ";
                    return View("_BoreProfile", model);
                   
                        
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this well" });
                }

            }
            else
            {
                return NotFound();
            }

        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateHTML(int? id, string scale = null)
        {
            if (id != null)
            {
                if (scale != null)
                {
                    ViewData["heightfactor"] = Convert.ToDouble(scale);
                }
                else
                {
                    ViewData["heightfactor"] = 3.0;
                }
                var user = await _userManager.GetUserAsync(User);
                var well = await _context.Wells
                    .Include(x => x.Project).ThenInclude(x => x.Division)
                    .Include(x => x.CoordSystem)
                    .Include(x => x.SubProject)
                    .SingleOrDefaultAsync(x => x.Id.Equals(id));
                if (user.DivisionId.Equals(well.Project.DivisionId))
                {
                    well.BentoniteLayers = await _context.BentoniteWellLayers.Include(x => x.CastingType).Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.meter_start).ToListAsync();
                    well.SoilSamples = await _context.SoilSamples.Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.sample_meter).ToListAsync();
                    well.FilterLayers = await _context.FilterWellLayers.Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.meter_start).ToListAsync();
                    well.SandLayers = await _context.SandWellLayers.Include(x => x.SandType).Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.meter_start).ToListAsync();
                    well.WellLayers = await _context.WellLayers.Include(x => x.Layer).Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.Start_m).ToListAsync();
                    WellPdf model = new WellPdf(well);
                    model.heightfactor = Convert.ToDouble(scale);
                    string footer = "--footer-center \"" + "  " + _SharedLocalizer.GetLocalizedHtmlString("Page").Value + ":" + " [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 ";
                    return View("_BoreProfile", model);
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this well" });
                }

            }
            else
            {
                return NotFound();
            }

        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if(id != null)
            {

                var user = await _userManager.GetUserAsync(User);
                var well = await _context.Wells.SingleOrDefaultAsync(x => x.Id.Equals(id));
                ViewData["WellTypeId"] = new SelectList(_context.WellTypes.Where(x => x.ProjectId.Equals(well.ProjectId)), "Id", "Type");
                if (User.IsInRole("ExternalDriller"))
                {
                    if (!well.Done_By.Equals(user.full_name()))
                    {
                        return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this object" });
                    }
                }
                ViewData["ProjectId"] = await GetProjectList();
                if (User.IsInRole("ExternalDriller"))
                {
                    var subprojects = await (from pu in _context.ProjectUsers
                                             join p in _context.Projects.Include(x => x.SubProjects) on pu.projectId equals p.Id
                                             where pu.userId.Equals(user.Id)
                                             select p.SubProjects).ToListAsync();
                    ViewData["SubProjectId"] = new SelectList(subprojects, "Id", "Name");

                }
                else
                {
                    ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
                }
                ViewData["CoordSystemId"] = new SelectList(_context.CoordSystems, "Id", "system");
                ViewData["WellDrillingInstructionId"] = new SelectList(_context.WellDrillingInstructions.Where(x => x.ProjectId.Equals(well.ProjectId)), "Id", "WellID");
                return View(well);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Well model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                if (User.IsInRole("ExternalDriller"))
                {
                    
                    if (!model.Done_By.Equals(user.full_name()))
                    {
                        return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this object" });
                    }
                }
                if(model.WellTypeId != null && (model.Well_Type == null || model.Well_Type == ""))
                {
                    var welltype = await _context.WellTypes.SingleOrDefaultAsync(x => x.Id.Equals(model.WellTypeId));
                    model.Well_Type = welltype.Type;
                }
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewData["ProjectId"] = await GetProjectList();
                if (User.IsInRole("ExternalDriller"))
                {
                    var subprojects = await (from pu in _context.ProjectUsers
                                             join p in _context.Projects.Include(x => x.SubProjects) on pu.projectId equals p.Id
                                             where pu.userId.Equals(user.Id)
                                             select p.SubProjects).ToListAsync();
                    ViewData["SubProjectId"] = new SelectList(subprojects, "Id", "Name");

                }
                else
                {
                    ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
                }
                ViewData["CoordSystemId"] = new SelectList(_context.CoordSystems, "Id", "system");
                ViewData["WellDrillingInstructionId"] = new SelectList(_context.WellDrillingInstructions.Where(x => x.ProjectId.Equals(model.ProjectId)), "Id", "WellID");
                return View(model);
            }

        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GeneratePDFOTHTERATTEMPT(int? id)
        {

            if (id != null)
            {
                var user = await _userManager.GetUserAsync(User);
                var well = await _context.Wells
                    .Include(x => x.Project).ThenInclude(x => x.Division)
                    .Include(x => x.CoordSystem)
                    .Include(x => x.SubProject)
                    .SingleOrDefaultAsync(x => x.Id.Equals(id));
                if (user.DivisionId.Equals(well.Project.DivisionId))
                {
                    well.BentoniteLayers = await _context.BentoniteWellLayers.Include(x => x.CastingType).Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.meter_start).ToListAsync();
                    well.SoilSamples = await _context.SoilSamples.Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.sample_meter).ToListAsync();
                    well.FilterLayers = await _context.FilterWellLayers.Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.meter_start).ToListAsync();
                    well.SandLayers = await _context.SandWellLayers.Include(x => x.SandType).Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.meter_start).ToListAsync();
                    well.WellLayers = await _context.WellLayers.Include(x => x.Layer).Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.Start_m).ToListAsync();
                    //WellFooterInfo footermodel = new WellFooterInfo(well);
                    string customSwitches = string.Format(" --print-media-type --footer-center [page] --allow {0} --footer-html {0} --footer-spacing -180 ",
            Url.Action("Footer", "home", new { id = well.Id }, "https"));
                    //return new ViewAsPdf("_BoreProfile", well) { CustomSwitches = customSwitches };

                    return new ViewAsPdf("_BoreProfile", well)
                    {
                        PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                        PageSize = Rotativa.AspNetCore.Options.Size.A4,
                        //PageMargins = { Left = 5, Bottom = 50, Right = 5, Top = 5 },
                        CustomSwitches = customSwitches
                    };
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this well" });
                }

            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet]
        public async Task<IActionResult> GeneratePDFHtml(int? id)
        {
            if (id != null)
            {
                var user = await _userManager.GetUserAsync(User);
                var well = await _context.Wells
                    .Include(x => x.Project).ThenInclude(x => x.Division)
                    .Include(x => x.CoordSystem)
                    .Include(x => x.SubProject)
                    .SingleOrDefaultAsync(x => x.Id.Equals(id));
                if (user.DivisionId.Equals(well.Project.DivisionId))
                {
                    well.BentoniteLayers = await _context.BentoniteWellLayers.Include(x => x.CastingType).Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.meter_start).ToListAsync();
                    well.SoilSamples = await _context.SoilSamples.Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.sample_meter).ToListAsync();
                    well.FilterLayers = await _context.FilterWellLayers.Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.meter_start).ToListAsync();
                    well.SandLayers = await _context.SandWellLayers.Include(x => x.SandType).Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.meter_start).ToListAsync();
                    well.WellLayers = await _context.WellLayers.Include(x => x.Layer).Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.Start_m).ToListAsync();
                    return View("_BoreProfile", well);
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this well" });
                }

            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet]
        public async Task<IActionResult> BoreProfile(int? id, string scale = null)
        {
            if(scale != null)
            {
                ViewData["heightfactor"] = Convert.ToDouble(scale);
            }
            else
            {
                ViewData["heightfactor"] = 3.0;
            }
            
            //ViewData["maxheightprofile"] = "50cm !important";
            if(id != null)
            {
                var user = await _userManager.GetUserAsync(User);
                var well = await _context.Wells
                    .Include(x => x.Project).ThenInclude(x => x.Division)
                    .Include(x => x.CoordSystem)
                    .Include(x => x.SubProject)
                    .SingleOrDefaultAsync(x => x.Id.Equals(id));
                if (user.DivisionId.Equals(well.Project.DivisionId))
                {
                    well.BentoniteLayers = await _context.BentoniteWellLayers.Include(x => x.CastingType).Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.meter_start).ToListAsync();
                    well.SoilSamples = await _context.SoilSamples.Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.sample_meter).ToListAsync();
                    well.FilterLayers = await _context.FilterWellLayers.Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.meter_start).ToListAsync();
                    well.SandLayers = await _context.SandWellLayers.Include(x => x.SandType).Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.meter_start).ToListAsync();
                    well.WellLayers = await _context.WellLayers.Include(x => x.Layer).Where(x => x.WellId.Equals(well.Id)).OrderBy(x => x.Start_m).ToListAsync();
                    return View(well);
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this well" });
                }
                
            }
            else
            {
                return NotFound();
            }
           
        }
        [HttpGet]
        public async Task<IActionResult> DrillWell(string TheWellName = null)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["Layers"] = new SelectList(_context.Layers.OrderBy(x => x.LayerType), "Id", "LayerType");
            ViewData["ProjectId"] = await GetProjectList();
            if (User.IsInRole("ExternalDriller"))
            {
                var subprojects = await (from pu in _context.ProjectUsers
                                         join p in _context.Projects.Include(x => x.SubProjects).ThenInclude(x => x.Project) on pu.projectId equals p.Id
                                         join sp in _context.SubProjects on p.Id equals sp.ProjectId
                                         where pu.userId.Equals(user.Id)
                                         select sp).ToListAsync();
                var measpoints = await (from pu in _context.ProjectUsers
                                        join p in _context.Projects on pu.projectId equals p.Id
                                        join mp in _context.MeasPoints.Include(x => x.Project) on p.Id equals mp.ProjectId
                                        where pu.userId.Equals(user.Id)
                                        select mp).ToListAsync();
                ViewData["SubProjectId"] = new SelectList(subprojects, "Id", "Name");
                ViewData["MeasPointId"] = new SelectList(measpoints, "Name", "Name");

            }
            else
            {
                ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Name", "Name");
                ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
            }
            ViewData["CoordSystems"] = new SelectList(_context.CoordSystems.OrderBy(x => x.system), "Id", "system");
            ViewData["SandTypeId"] = new SelectList(_context.SandTypes, "Id", "TypeOfSand");
            ViewData["CastingTypeId"] = new SelectList(_context.CastingTypes, "Id", "TypeOfCasting");
            if (TheWellName == null)
            {
                WellVM model = new WellVM();
                return View(model);
                }
            else
            {
                string lal = TheWellName.Split("_")[0];
                var otherwell = await _context.Wells
                    .Include(x => x.BentoniteLayers)
                    .Include(x => x.WellLayers)
                    .Include(x => x.SandLayers)
                    .Include(x => x.FilterLayers)
                    .Include(x => x.SoilSamples)
                    .Where(x => x.WellName.Contains(lal) && !x.WellName.Equals(TheWellName)).FirstOrDefaultAsync();
                if(otherwell != null) { 
                    WellVM model = new WellVM(otherwell);
                    model.FilterLayers = new List<FilterLayerWell>(7);
                    return View(model);
                }
                else
                {
                    WellVM model = new WellVM();
                    
                    
                    return View(model);
                }
            }
        }
       
        [HttpPost]
        public async Task<IActionResult> DrillWell(WellVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (model.WellId == null || model.WellId == 0)
                {
                    Well well = new Well(model,user);
                    //var measpoint = await _context.MeasPoints.SingleOrDefaultAsync(x => x.Name.Equals(model.WellName) && x.ProjectId.Equals(model.ProjectId));
                    //if(measpoint != null)
                    //{
                    //    if(measpoint.Coordx != 0)
                    //    {
                    //        well.Coord_x = measpoint.Coordx;
                    //        well.Coord_y = measpoint.Coordy;
                    //        well.Coord_z = measpoint.Coordz;
                    //    }
                    //}
                    _context.Wells.Add(well);
                    await _context.SaveChangesAsync();
                    Well last_added;
                    last_added = await _context.Wells.Include(x => x.WellType).Where(x => x.WellTypeId.Equals(model.WellTypeId)).LastOrDefaultAsync();
                    if(last_added == null)
                    {
                        last_added = await _context.Wells.Include(x => x.WellType).LastOrDefaultAsync();
                    }
                    double startm = 0.0;
                    foreach(var item in model.Layers)
                    {
                        if(item.Start_m > 0 || item.Start_m >= startm) {
                            item.WellId = last_added.Id;
                            _context.Add(item);
                            startm = Convert.ToDouble(item.Start_m);
                        }
                    }
                    startm = 0.0;
                    foreach (var item in model.BentoniteLayers)
                    {
                        if (item.meter_start> 0 || item.meter_start >= startm)
                        {
                            item.WellId = last_added.Id;
                            _context.Add(item);
                            startm = Convert.ToDouble(item.meter_start);
                        }
                    }
                    startm = 0.0;
                    foreach (var item in model.FilterLayers)
                    {
                        if (item.meter_start > 0 || item.meter_start >= startm)
                        {
                            item.WellId = last_added.Id;
                            _context.Add(item);
                            startm = Convert.ToDouble(item.meter_start);
                        }
                    }
                    startm = 0.0;
                    foreach (var item in model.SandLayers)
                    {
                        if (item.meter_start > 0 || item.meter_start >= startm)
                        {
                            item.WellId = last_added.Id;
                            _context.Add(item);
                            startm = Convert.ToDouble(item.meter_start);
                        }
                    }
                    startm = 0.0;
                    foreach (var item in model.Samples)
                    {
                        if (item.sample_meter > 0 || item.sample_meter >= startm)
                        {
                            item.WellId = last_added.Id;
                            _context.Add(item);
                            startm = Convert.ToDouble(item.sample_meter);
                        }
                    }
                    await _context.SaveChangesAsync();
                    model.WellId = last_added.Id;
                    if(last_added.WellTypeId != null)
                    {
                        var welltype = await _context.WellTypes.SingleOrDefaultAsync(x => x.Id.Equals(well.WellTypeId));
                        if (welltype.ItemTypeId != null)
                        {
                            var itemtype = await _context.ItemTypes.SingleOrDefaultAsync(x => x.ReportTypeId.Equals(13) && x.Id.Equals(last_added.WellType.ItemTypeId));
                           
                            if(itemtype != null)
                            {
                                string welluniqueid = String.Concat(welltype.Type, " : ", model.WellName);
                                var prev_install = await _context.Installations.FirstOrDefaultAsync(x => x.UniqueID.Equals(welluniqueid));
                                if(prev_install == null) {
                                    Install inst;
                                    if (model.WellDiameter != null)
                                    {
                                        inst = new Install
                                        {
                                            ToBePaid = true,
                                            ItemTypeId = itemtype.Id,
                                            Latitude = 0,
                                            Longitude = 0,
                                            TimeStamp = model.Drill_Date_End,
                                            RentalStartDate = model.Drill_Date_End,
                                            InvoiceDate = DateTime.Now,
                                            Install_Text = welltype.Type + " " + model.WellDiameter,
                                            isInstalled = true,
                                            Amount = 1,
                                            UniqueID = welluniqueid,
                                            ProjectId = model.ProjectId,
                                            SubProjectId = model.SubProjectId,
                                            EnteredIntoDataBase = DateTime.Now,
                                            LastEditedInDataBase = DateTime.Now,
                                            DoneBy = user.full_name(),
                                            PayedAmount = 0
                                        };
                                        _context.Add(inst);
                                        await _context.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        inst = new Install
                                        {
                                            ToBePaid = true,
                                            ItemTypeId = itemtype.Id,
                                            Latitude = 0,
                                            Longitude = 0,
                                            TimeStamp = model.Drill_Date_End,
                                            RentalStartDate = model.Drill_Date_End,
                                            InvoiceDate = DateTime.Now,
                                            Install_Text = welltype.Type,
                                            isInstalled = true,
                                            Amount = 1,
                                            UniqueID = welluniqueid,
                                            ProjectId = model.ProjectId,
                                            SubProjectId = model.SubProjectId,
                                            EnteredIntoDataBase = DateTime.Now,
                                            LastEditedInDataBase = DateTime.Now,
                                            DoneBy = user.full_name(),
                                             PayedAmount = 0
                                };
                                        _context.Add(inst);
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }
                        }
                    }
                    
                    ViewData["Layers"] = new SelectList(_context.Layers, "Id", "LayerType");
                    ViewData["ProjectId"] = await GetProjectList();
                    ViewData["SandTypeId"] = new SelectList(_context.SandTypes, "Id", "TypeOfSand");
                    ViewData["CastingTypeId"] = new SelectList(_context.CastingTypes, "Id", "TypeOfCasting");
                    //ViewData["WellTypes"] = new SelectList(_context.WellTypes)
                    if (model.ProjectId != null)
                    {
                        ViewData["WellTypeId"] = new SelectList(_context.WellTypes.Where(x => x.ProjectId.Equals(model.ProjectId)), "Id", "Type");
                        ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => x.ProjectId.Equals(model.ProjectId)), "Id", "Name");
                    }
                    else
                    {
                        ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Name");
                    }
                    if (model.ProjectId != null)
                    {
                        if (model.SubProjectId != null)
                        {
                            ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Where(x => x.SubProjectId.Equals(model.SubProjectId) && x.ToBeHidden.Equals(false)), "Name", "Name");
                        }
                        else
                        {
                            ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Where(x => x.ProjectId.Equals(model.ProjectId) && x.ToBeHidden.Equals(false)), "Name", "Name");
                        }
                    }
                    else
                    {
                        ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Where(x => x.Project.DivisionId.Equals(user.DivisionId) && x.Project.Active.Equals(true) && x.ToBeHidden.Equals(false)), "Name", "Name");
                    }
                    ViewData["CoordSystems"] = new SelectList(_context.CoordSystems, "Id", "system");
                    return View(model);
                }
                else
                {
                    var well = await _context.Wells
                        .Include(x => x.BentoniteLayers).ThenInclude(x => x.CastingType)
                        .Include(x => x.SoilSamples)
                        .Include(x => x.FilterLayers)
                        .Include(x => x.SandLayers).ThenInclude(x => x.SandType)
                        .Include(x => x.WellLayers)
                        .SingleOrDefaultAsync(x => x.Id.Equals(model.WellId));
                    well.WellName = model.WellName;
                    well.WellTypeId = model.WellTypeId;
                    well.WellDiameter = model.WellDiameter;
                    well.WaterLevel = model.WaterLevel;
                    well.ProjectId = model.ProjectId;
                    well.SubProjectId = model.SubProjectId;
                    well.Well_Depth = model.Layers.OrderByDescending(x => x.End_m).First().End_m;
                    well.PipeDiameter = model.PipeDiameter;
                    well.DGU_Number = model.DGU;
                    well.CoordSystemId = model.CoordSystemId;
                    well.Coord_x = model.Coord_x;
                    well.Coord_y = model.Coord_y;
                    well.Coord_z = model.Coord_z;
                    well.Drill_Method = model.DrillMethod;
                    well.Assessed_By = model.Assessed_By;
                    well.Approved_By = model.Approved_By;
                    well.Approved_Date = model.Approved_Date;
                    well.Assessed_Date = model.Assessed_Date;
                    well.Drill_Date_Start = model.Drill_Date_Start;
                    well.Drill_Date_End = model.Drill_Date_End;
                    //Deleting old
                    foreach (var item in well.WellLayers)
                    {
                        _context.Remove(item);
                    }
                    foreach (var item in well.BentoniteLayers)
                    {
                        _context.Remove(item);
                    }
                    foreach (var item in well.FilterLayers)
                    {
                        _context.Remove(item);
                    }
                    foreach (var item in well.SandLayers)
                    {
                        _context.Remove(item);
                    }
                    foreach (var item in well.SoilSamples)
                    {
                        _context.Remove(item);
                    }
                    await _context.SaveChangesAsync();
                    //Adding again
                    double startm = 0.0;
                    foreach (var item in model.Layers)
                    {
                        if (item.Start_m >= startm || item.Start_m > 0)
                        {
                            item.WellId = model.WellId;
                            _context.Add(item);
                            startm = Convert.ToDouble(item.Start_m);
                        }
                    }
                    startm = 0.0;
                    foreach (var item in model.BentoniteLayers)
                    {
                        if (item.meter_start >= startm || item.meter_start > 0)
                        {
                            item.WellId = model.WellId;
                            _context.Add(item);
                            startm = Convert.ToDouble(item.meter_start);
                        }
                    }
                    startm = 0.0;
                    foreach (var item in model.FilterLayers)
                    {
                        if (item.meter_start >= startm || item.meter_start > 0)
                        {
                            item.WellId = model.WellId;
                            _context.Add(item);
                            startm = Convert.ToDouble(item.meter_start);
                        }
                    }
                    startm = 0.0;
                    foreach (var item in model.SandLayers)
                    {
                        if (item.meter_start >= startm || item.meter_start > 0)
                        {
                            item.WellId = model.WellId;
                            _context.Add(item);
                            startm = Convert.ToDouble(item.meter_start);
                        }
                    }
                    startm = 0.0;
                    foreach (var item in model.Samples)
                    {
                        if (item.sample_meter >= startm || item.sample_meter > 0)
                        {
                            item.WellId = model.WellId;
                            _context.Add(item);
                            startm = Convert.ToDouble(item.sample_meter);
                        }
                    }
                    _context.Update(well);
                    await _context.SaveChangesAsync();
                    if (well.WellTypeId != null && well.Drill_Date_End != null)
                    {
                        var welltype = await _context.WellTypes.SingleOrDefaultAsync(x => x.Id.Equals(well.WellTypeId));
                        if (welltype.ItemTypeId != null)
                        {
                            var itemtype = await _context.ItemTypes.SingleOrDefaultAsync(x => x.ReportTypeId.Equals(13) && x.Id.Equals(welltype.ItemTypeId));

                            if (itemtype != null)
                            {
                                string welluniqueid = String.Concat(welltype.Type, " : ", model.WellName);
                                var prev_install = await _context.Installations.FirstOrDefaultAsync(x => x.UniqueID.Equals(welluniqueid));
                                if (prev_install == null)
                                {
                                    Install inst;
                                    if(model.WellDiameter != null) { 
                                        inst = new Install
                                        {
                                            ToBePaid = true,
                                            ItemTypeId = itemtype.Id,
                                            Latitude = 0,
                                            Longitude = 0,
                                            TimeStamp = model.Drill_Date_End,
                                            RentalStartDate = model.Drill_Date_End,
                                            InvoiceDate = DateTime.Now,
                                            Install_Text = welltype.Type + " " + model.WellDiameter,
                                            isInstalled = true,
                                            Amount = 1,
                                            UniqueID = welluniqueid,
                                            ProjectId = model.ProjectId,
                                            SubProjectId = model.SubProjectId,
                                            EnteredIntoDataBase = DateTime.Now,
                                            LastEditedInDataBase = DateTime.Now,
                                            DoneBy = user.full_name()
                                        };
                                    }
                                    else
                                    {
                                        inst = new Install
                                        {
                                            ToBePaid = true,
                                            ItemTypeId = itemtype.Id,
                                            Latitude = 0,
                                            Longitude = 0,
                                            TimeStamp = model.Drill_Date_End,
                                            RentalStartDate = model.Drill_Date_End,
                                            InvoiceDate = DateTime.Now,
                                            Install_Text = welltype.Type,
                                            isInstalled = true,
                                            Amount = 1,
                                            UniqueID = welluniqueid,
                                            ProjectId = model.ProjectId,
                                            SubProjectId = model.SubProjectId,
                                            EnteredIntoDataBase = DateTime.Now,
                                            LastEditedInDataBase = DateTime.Now,
                                            DoneBy = user.full_name()
                                        };
                                    }
                                    _context.Installations.Add(inst);
                                    await _context.SaveChangesAsync();
                                }
                                else
                                {
                                    prev_install.TimeStamp = well.Drill_Date_End;
                                    prev_install.RentalStartDate = well.Drill_Date_End;
                                    prev_install.Install_Text = welltype.Type + " " + model.WellDiameter;
                                    prev_install.SubProjectId = model.SubProjectId;
                                    _context.Update(prev_install);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }
                    }
                    ViewData["Layers"] = new SelectList(_context.Layers, "Id", "LayerType");
                    ViewData["ProjectId"] = await GetProjectList();
                    ViewData["SandTypeId"] = new SelectList(_context.SandTypes, "Id", "TypeOfSand");
                    ViewData["CastingTypeId"] = new SelectList(_context.CastingTypes, "Id", "TypeOfCasting");
                    if (model.ProjectId != null)
                    {
                        ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => x.ProjectId.Equals(model.ProjectId)), "Id", "Name");
                        ViewData["WellTypeId"] = new SelectList(_context.WellTypes.Where(x => x.ProjectId.Equals(model.ProjectId)), "Id", "Type");
                    }
                    else
                    {
                        ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Name");
                        if(model.WellTypeId != null)
                        {
                            ViewData["WellTypeId"] = new SelectList(_context.WellTypes.Where(x => x.Id.Equals(model.WellTypeId)), "Id", "Type");
                        }
                    }
                    if (model.ProjectId != null)
                    {
                        if (model.SubProjectId != null)
                        {
                            ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Where(x => x.SubProjectId.Equals(model.SubProjectId) && x.ToBeHidden.Equals(false)), "Name", "Name");
                        }
                        else
                        {
                            ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Where(x => x.ProjectId.Equals(model.ProjectId) && x.ToBeHidden.Equals(false)), "Name", "Name");
                        }
                    }
                    else
                    {
                        ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Where(x => x.Project.DivisionId.Equals(user.DivisionId) && x.Project.Active.Equals(true) && x.ToBeHidden.Equals(false)), "Name", "Name");
                    }
                    ViewData["CoordSystems"] = new SelectList(_context.CoordSystems, "Id", "system");
                    return View(model);
                }

                
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                ViewData["Layers"] = new SelectList(_context.Layers, "Id", "LayerType");
                ViewData["ProjectId"] = await GetProjectList();
                if (User.IsInRole("ExternalDriller"))
                {
                    var measpoints = await (from pu in _context.ProjectUsers
                                            join p in _context.Projects on pu.projectId equals p.Id
                                            join mp in _context.MeasPoints.Include(x => x.Project) on p.Id equals mp.ProjectId
                                            where pu.userId.Equals(user.Id)
                                            select mp).ToListAsync();
                    ViewData["MeasPointId"] = new SelectList(measpoints, "Id", "Name");

                }
                else
                {
                    ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Name", "Name");
                }
                ViewData["CoordSystems"] = new SelectList(_context.CoordSystems, "Id", "system");
                ViewData["SandTypeId"] = new SelectList(_context.SandTypes, "Id", "TypeOfSand");
                ViewData["CastingTypeId"] = new SelectList(_context.CastingTypes, "Id", "TypeOfCasting");
                if (model.ProjectId != null)
                {
                    ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Where(x => x.ProjectId.Equals(model.ProjectId)), "Id", "Name");
                    ViewData["WellTypeId"] = new SelectList(_context.WellTypes.Where(x => x.ProjectId.Equals(model.ProjectId)), "Id", "Type");
                }
                else
                {
                    ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Name");
                    if (model.WellTypeId != null)
                    {
                        ViewData["WellTypeId"] = new SelectList(_context.WellTypes.Where(x => x.Id.Equals(model.WellTypeId)), "Id", "Type");
                    }
                }
                if (model.ProjectId != null)
                {
                    if (model.SubProjectId != null)
                    {
                        ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Where(x => x.SubProjectId.Equals(model.SubProjectId) && x.ToBeHidden.Equals(false)), "Name", "Name");
                    }
                    else
                    {
                        ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Where(x => x.ProjectId.Equals(model.ProjectId) && x.ToBeHidden.Equals(false)), "Name", "Name");
                    }
                }
                else
                {
                    ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Where(x => x.Project.DivisionId.Equals(user.DivisionId) && x.Project.Active.Equals(true) && x.ToBeHidden.Equals(false)), "Name", "Name");
                }
                return View(model);
            }
        }
        [HttpGet]
        public IActionResult ApplyForDGU(int? id)
        {
            return View();
        }
        [HttpGet]
        public IActionResult UploadDrillLog(int? id)
        {
            return View();
        }
        [HttpPost]
        public IActionResult UplaodDrillLog()
        {
            return RedirectToAction(nameof(Index));
        }
        public async Task<FileResult> DownloadBackGroundImage(int id)
        {
            var path = _env.WebRootPath + "\\images\\LayerBackGroundImages\\" + id.ToString() + ".jpg";
            MemoryStream memory = new MemoryStream();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "image/jpg","layer_"+id.ToString()+".jpg");
        }
        public async Task<IActionResult> UploadBackGroundImage(int id,IFormFile photo)
        {
            var directory = _env.WebRootPath + "\\images\\LayerBackGroundImages\\";
            var layer = await _context.Layers.FindAsync(id);
            if (!Directory.Exists(directory) && photo != null)
            {
                Directory.CreateDirectory(directory);
            }
            if(photo != null) {
                var path = Path.Combine(directory, String.Concat(id.ToString(),".jpg"));
                layer.BackgroundUrl = "https://hj-mainops.com/WellDrilling/DownloadBackGroundImage/" + id.ToString();// + ".jpg";
                _context.Update(layer);
                var stream = new FileStream(path, FileMode.Create);
                await photo.CopyToAsync(stream);
                await _context.SaveChangesAsync();
                stream.Close();
                
            }
            return RedirectToAction("index","Layers");
        }
        public async Task<IActionResult> Layers()
        {
            var layers = await _context.Layers.ToListAsync();
            return View(layers);
        }
        

    }
}