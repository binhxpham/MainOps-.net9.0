using MainOps.Data;
using MainOps.Models.WTPClasses.MixedClasses;
using MainOps.Models.WTPClasses.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainOps.Controllers.WTPControllers
{
    [Authorize(Roles = ("Admin"))]
    public class VMMediaController : Controller
    {
        private readonly DataContext _context;
        public VMMediaController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Skip()
        {
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> Create(VMMedia mEff)
        {
            var fmat = await _context.FilterMaterials
                .SingleOrDefaultAsync(x => x.id == mEff.m_efx.filtermaterialid);
            if(fmat == null)
            {
                return NotFound();
            }
            var contan = await _context.Contaminations.FindAsync(mEff.m_efx.contaminationId);
            fmat.contaminations = string.Join(", ", fmat.contaminations, contan.Name);
            try
            {
                _context.Update(fmat);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FilterMaterialExists(fmat.id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var mof = new MediaEfficiency(mEff);
            _context.MediaEfficiencies.Add(mof);
            _context.SaveChanges();
            return RedirectToAction("Mediablock", "VMMedia", new { fmat.id });
        }
        [HttpPost]
        public async Task<IActionResult> CreateTwo(VMMediaTwo mEff)
        {
            var fmat = await _context.FilterMaterials
                .SingleOrDefaultAsync(x => x.id == mEff.m_efx.filtermaterialid);
            if (fmat == null)
            {
                return NotFound();
            }
            var contan = await _context.Contaminations.FindAsync(mEff.m_efx.contaminationId);
            fmat.contaminations = string.Join(", ", fmat.contaminations, contan.Name);
            try
            {
                _context.Update(fmat);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FilterMaterialExists(fmat.id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var mof = new MediaEfficiency(mEff);
            _context.MediaEfficiencies.Add(mof);
           // _context.MediaEfficiencies.Add(new MediaEfficiency { filtermaterialid = m_eff.m_efx.filtermaterialid, contaminationName = m_eff.m_efx.contaminationName, efficiency = m_eff.m_efx.efficiency });
            _context.SaveChanges();
            return RedirectToAction("MediablockTwo", "VMMedia", new { contan.Id });
        }
        private bool FilterMaterialExists(int id)
        {
            return _context.FilterMaterials.Any(e => e.id == id);
        }
        public async Task<IActionResult> Mediablock(int? id)
        {
            //var mList = new List<MediaEfficiency>();
            VMMedia vM;
            if (id == null)
            {
                return NotFound();
            }

            var filterMaterial = await _context.FilterMaterials.SingleOrDefaultAsync(m => m.id == id);
            if(filterMaterial == null)
            {
                return NotFound();
            }
            var MEFFS = await _context.MediaEfficiencies.Include(x => x.Contamination).Include(x => x.filtermaterial)
                .Where(x => x.filtermaterial.Equals(filterMaterial))
                .OrderBy(x=>x.filtermaterial.Name)
                .ToListAsync();
            if(MEFFS == null || MEFFS.Count == 0)
            {
                vM = new VMMedia(filterMaterial);
            }
            else
            {
                vM = new VMMedia(filterMaterial, MEFFS);
            }
            var dbFilters = await _context.FilterMaterials
                .OrderBy(f => f.Name).ToListAsync();
            var dbContas = await _context.Contaminations
                .OrderBy(x => x.Name).ToListAsync();
            var dbDosings = await _context.Dosings
                .OrderBy(x => x.dosing).ToListAsync();
            var dbeffectTypes = await _context.Effect_types
                .OrderBy(x => x.description).ToListAsync();
            ViewData["Dosing Choices"] = new SelectList(dbDosings, "Id", "dosing");
            ViewData["Effect Choices"] = new SelectList(dbeffectTypes, "id", "description");
            ViewData["Filter Choices"] = new SelectList(dbFilters, "id", "Name");
            ViewData["Conta Choices"] = new SelectList(dbContas, "Id", "Name");
            return View(vM);
        }
        public async Task<IActionResult> MediablockTwo(int? id)
        {
            //var mList = new List<MediaEfficiency>();
            VMMediaTwo vM;
            if (id == null)
            {
                return NotFound();
            }

            var contamination = await _context.Contaminations.FindAsync(id);
            if (contamination == null)
            {
                return NotFound();
            }
            contamination.Unit_limit = await _context.WTPUnits.FindAsync(contamination.Unit_limitid);
            
            var MEFFS = await _context.MediaEfficiencies
                .Where(x => x.Contamination.Equals(contamination))
                .OrderBy(x => x.filtermaterial)
                .ToListAsync();
            if (MEFFS.Count() == 0 || MEFFS == null)
            {
                vM = new VMMediaTwo(contamination);
            }
            else
            {
                vM = new VMMediaTwo(contamination, MEFFS);
            }
            var dbFilters = await _context.FilterMaterials
                .OrderBy(f => f.Name).ToListAsync();
            var dbContas = await _context.Contaminations
                .OrderBy(x => x.Name).ToListAsync();
            var dbDosings = await _context.Dosings
                .OrderBy(x => x.dosing).ToListAsync();
            var dbeffectTypes = await _context.Effect_types
                .OrderBy(x => x.description).ToListAsync();
            ViewData["Dosing Choices"] = new SelectList(dbDosings, "Id", "dosing");
            ViewData["Effect Choices"] = new SelectList(dbeffectTypes, "id", "description");
            ViewData["Filter Choices"] = new SelectList(dbFilters, "id", "Name");
            ViewData["Conta Choices"] = new SelectList(dbContas, "Id", "Name");
            return View(vM);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mediaEffeciency = await _context.MediaEfficiencies.Include(x=>x.Contamination).Include(x=>x.filtermaterial)
                .SingleOrDefaultAsync(m => m.Id == id);
            //mediaEffeciency.filtermaterial = await _context.FilterMaterials.SingleOrDefaultAsync(x => x.id == mediaEffeciency.filtermaterialid);
            //mediaEffeciency.Contamination = await _context.Contaminations.SingleOrDefaultAsync(x => x.Name.ToLower() == mediaEffeciency.Contamination.Name.ToLower());
            if (mediaEffeciency == null)
            {
                return NotFound();
            }

            return View(mediaEffeciency);
        }

        // POST: Efforts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            
            var mediaEffeciency = await _context.MediaEfficiencies.FindAsync(id);
            var filtMat = await _context.FilterMaterials.SingleOrDefaultAsync(f => f.id == mediaEffeciency.filtermaterialid);
            var sb = new StringBuilder();
            var contParts = filtMat.contaminations.Split(',');
            foreach(var s in contParts)
            {
                if(sb.Length != 0)
                {
                    sb.Append(", ");
                }
                if(s.Trim().ToLower() == mediaEffeciency.Contamination.Name.Trim().ToLower())
                {
                    sb.Remove(sb.Length - 2, 2);
                }
                else
                {
                    sb.Append(s.Trim());
                }
            }
            if(sb[sb.Length-2] == ',')
            {
                sb.Remove(sb.Length - 2, 2);
            }
            filtMat.contaminations = sb.ToString();
            _context.Update(filtMat);
            _context.MediaEfficiencies.Remove(mediaEffeciency);
            await _context.SaveChangesAsync();
            return RedirectToAction("Mediablock", "VMMedia", new { id = mediaEffeciency.filtermaterialid });
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var effort = await _context.MediaEfficiencies
                .Include(x=>x.Contamination)
                .Include(x=>x.filtermaterial)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (effort == null)
            {
                return NotFound();
            }
            //effort.filtermaterial = await _context.FilterMaterials.SingleOrDefaultAsync(x => x.id == effort.filtermaterialid);
            //effort.Contamination = await _context.Contaminations.SingleOrDefaultAsync(x => x.Name.ToLower() == effort.Contamination.Name.ToLower());
            var dbDosings = await _context.Dosings
                .OrderBy(x => x.dosing).ToListAsync();
            var dbeffectTypes = await _context.Effect_types
                .OrderBy(x => x.description).ToListAsync();
            ViewData["Dosing Choices"] = new SelectList(dbDosings, "Id", "dosing");
            ViewData["Effect Choices"] = new SelectList(dbeffectTypes, "id", "description");
            return View(effort);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,filtermaterialid,contaminationName,dosing_ofId,dosing_relation,efficiency,has_concentration_effect,lower_limit_aeration,lower_limit_pH,need_Aeration,need_dosing,need_pH_control,upper_limit_aeration,upper_limit_pH,effect_typeid")] MediaEfficiency mediaEfficiency)
        {
            if (id != mediaEfficiency.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mediaEfficiency);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MediaEfficiencyExists(mediaEfficiency.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Mediablock","VMMedia",new { id = mediaEfficiency.filtermaterialid });
            }
            return View(mediaEfficiency);
        }
        private bool MediaEfficiencyExists(int id)
        {
            return _context.MediaEfficiencies.Any(e => e.Id == id);
        }
    }



    
    
    
}
