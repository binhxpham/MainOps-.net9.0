using MainOps.Data;
using MainOps.ExtensionMethods;
using MainOps.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Controllers
{
   
    public abstract class BaseController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private readonly DataContext _context;
        public BaseController(DataContext context, UserManager<ApplicationUser> userManager) 
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> CheckUser(ApplicationUser user) { 
            if(user.Active == false)
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You are inactivated" });
            }
            else { return Ok(); }

        }
        public async Task<IEnumerable<SelectListItem>> GetProjectListNoChoice()
        {
            if (User.IsInRole("Admin"))
            {
                var projects = _context.Projects.Where(x => x.Active.Equals(true)).OrderBy(x => x.DivisionId).ThenBy(x => x.Name).ToList();
                IEnumerable<SelectListItem> selList = from s in projects
                                                      select new SelectListItem
                                                      {
                                                          Value = s.Id.ToString(),
                                                          Text = s.Name + " : " + s.ProjectNr
                                                      };
                return selList;
            }
            else if (User.IsInRole("Guest") || User.IsInRole("MemberGuest") || User.IsInRole("ExternalDriller"))
            {
                var user = await _userManager.GetUserAsync(User);
                var projects = await (from p in _context.Projects.OrderBy(x => x.Name) join pu in _context.ProjectUsers on p.Id equals pu.projectId where pu.userId.Equals(user.Id) && p.Active.Equals(true) select p).OrderBy(x => x.DivisionId).ThenBy(x => x.Name).ToListAsync();
                IEnumerable<SelectListItem> selList = from s in projects
                                                      select new SelectListItem
                                                      {
                                                          Value = s.Id.ToString(),
                                                          Text = s.Name + " : " + s.ProjectNr 
                                                      };
                return selList;
            }
            else if (User.IsInRole("International"))
            {
                var user = await _userManager.GetUserAsync(User);
                var projects = _context.Projects.Where(x => x.Name.Contains("STOCK") && x.Active.Equals(true) && user.DivisionId.Equals(x.DivisionId)).OrderBy(x => x.DivisionId).ThenBy(x => x.Name).ToList();
                IEnumerable<SelectListItem> selList = from s in projects
                                                      select new SelectListItem
                                                      {
                                                          Value = s.Id.ToString(),
                                                          Text = s.Name + " : " + s.ProjectNr
                                                      };
                return selList;
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                var projects = _context.Projects.Where(x => x.DivisionId.Equals(user.DivisionId) && x.Active.Equals(true)).OrderBy(x => x.DivisionId).ThenBy(x => x.Name).ToList();
                IEnumerable<SelectListItem> selList = from s in projects
                                                      select new SelectListItem
                                                      {
                                                          Value = s.Id.ToString(),
                                                          Text = s.Name + " : " + s.ProjectNr
                                                      };
                return selList;
            }
        }
        public async Task<IEnumerable<SelectListItem>> GetProjectList()
        {

            if (User.IsInRole("Admin"))
            {
                var projects = _context.Projects.Where(x => x.Active.Equals(true)).OrderBy(x => x.DivisionId).ThenBy(x => x.Name).ToList();
                IEnumerable<SelectListItem> selList = from s in projects
                                                      select new SelectListItem
                                                      {
                                                          Value = s.Id.ToString(),
                                                          Text = s.Name + " : " + s.ProjectNr,
                                                          Selected = s.Id == 437 ? true : false
                                                      };
                //if(projects.FindIndex(x => x.Id.Equals(437)) >= 0)
                //{
                //    selList.SingleOrDefault(x => x.Value.Equals("437")).Selected = true;
                //}
                return selList;
            }
            else if (User.IsInRole("Guest") || User.IsInRole("MemberGuest") || User.IsInRole("ExternalDriller"))
            {
                var user = await _userManager.GetUserAsync(User);
                var projects = await (from p in _context.Projects.OrderBy(x => x.Name) join pu in _context.ProjectUsers on p.Id equals pu.projectId where pu.userId.Equals(user.Id) && p.Active.Equals(true) select p).OrderBy(x => x.DivisionId).ThenBy(x => x.Name).ToListAsync();
                IEnumerable<SelectListItem> selList = from s in projects
                                                      select new SelectListItem
                                                      {
                                                          Value = s.Id.ToString(),
                                                          Text = s.Name + " : " + s.ProjectNr,
                                                          Selected = s.Id == 437 ? true : false
                                                      };
                return selList;
            }
            else if (User.IsInRole("International"))
            {
                var user = await _userManager.GetUserAsync(User);
                var projects = _context.Projects.Where(x => x.Name.Contains("STOCK") && x.Active.Equals(true) && user.DivisionId.Equals(x.DivisionId)).OrderBy(x => x.DivisionId).ThenBy(x => x.Name).ToList();
                IEnumerable<SelectListItem> selList = from s in projects
                                                      select new SelectListItem
                                                      {
                                                          Value = s.Id.ToString(),
                                                          Text = s.Name + " : " + s.ProjectNr,
                                                          Selected = s.Id == 437 ? true : false
                                                      };
                return selList;
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                var projects = _context.Projects.Where(x => x.DivisionId.Equals(user.DivisionId) && x.Active.Equals(true)).OrderBy(x => x.DivisionId).ThenBy(x => x.Name).ToList();
                IEnumerable<SelectListItem> selList = from s in projects
                                                      select new SelectListItem
                                                      {
                                                          Value = s.Id.ToString(),
                                                          Text = s.Name + " : " + s.ProjectNr,
                                                          Selected = s.Id == 437 ? true : false
                                                      };
                //if (projects.FindIndex(x => x.Id.Equals(437)) >= 0)
                //{
                //    selList.SingleOrDefault(x => x.Value.Equals("437")).Selected = true;
                //}
                return selList;
            }
        }
        public async Task<IEnumerable<SelectListItem>> GetHJItems(int? ClassId = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if(ClassId == null)
            {
                if (User.IsInRole("Admin"))
                {

                    IEnumerable<SelectListItem> selList = from s in _context.HJItems.OrderBy(x => x.DivisionId).OrderBy(x => x.HJId)
                                                          select new SelectListItem
                                                          {
                                                              Value = s.Id.ToString(),
                                                              Text = s.HJItemClass.ClassName + " (" + s.HJId + ")"
                                                          };
                    return selList;
                }
                else
                {
                    IEnumerable<SelectListItem> selList = from s in _context.HJItems.Where(x => x.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.DivisionId).OrderBy(x => x.HJId)
                                                          select new SelectListItem
                                                          {
                                                              Value = s.Id.ToString(),
                                                              Text = s.HJItemClass.ClassName + " (" + s.HJId + ")"
                                                          };
                    return selList;
                }
            }
            else
            {
                if (User.IsInRole("Admin"))
                {

                    IEnumerable<SelectListItem> selList = from s in _context.HJItems.Where(x => x.HJItemClassId.Equals(ClassId)).OrderBy(x => x.DivisionId).OrderBy(x => x.HJId)
                                                          select new SelectListItem
                                                          {
                                                              Value = s.Id.ToString(),
                                                              Text = s.HJItemClass.ClassName + " (" + s.HJId + ")"
                                                          };
                    return selList;
                }
                else
                {
                    IEnumerable<SelectListItem> selList = from s in _context.HJItems.Where(x => x.DivisionId.Equals(user.DivisionId) && x.HJItemClassId.Equals(ClassId)).OrderBy(x => x.DivisionId).OrderBy(x => x.HJId)
                                                          select new SelectListItem
                                                          {
                                                              Value = s.Id.ToString(),
                                                              Text = s.HJItemClass.ClassName + " (" + s.HJId + ")"
                                                          };
                    return selList;
                }
            }
           
        }
        public async Task<IEnumerable<SelectListItem>> GetHJItemMasterClasses()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {

                IEnumerable<SelectListItem> selList = from s in _context.HJItemMasterClasses.OrderBy(x => x.DivisionId).OrderBy(x => x.ClassNumber)
                                                      select new SelectListItem
                                                      {
                                                          Value = s.Id.ToString(),
                                                          Text = s.NameAndNumber
                                                      };
                return selList;
            }
            else
            {
                IEnumerable<SelectListItem> selList = from s in _context.HJItemMasterClasses.Where(x => x.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.DivisionId).OrderBy(x => x.ClassNumber)
                                                      select new SelectListItem
                                                      {
                                                          Value = s.Id.ToString(),
                                                          Text = s.NameAndNumber
                                                      };
                return selList;
            }
        }
        public async Task<IEnumerable<SelectListItem>> GetHJItemClasses(string MasterClass = null)
        {
            var user = await _userManager.GetUserAsync(User);
            int? MasterClassId = Convert.ToInt32(MasterClass);
            if (User.IsInRole("Admin"))
            {

               
                if(MasterClassId != null)
                {
                    IEnumerable<SelectListItem> selList = from s in _context.HJItemClasses.Include(x => x.HJItemMasterClass).Where(x => x.HJItemMasterClassId.Equals(MasterClassId)).OrderBy(x => x.HJItemMasterClass.DivisionId).ThenBy(x => x.ClassNumber)
                                                          select new SelectListItem
                                                          {
                                                              Value = s.Id.ToString(),
                                                              Text = s.NameAndNumber
                                                          };
                    return selList;
                }
                else
                {
                    IEnumerable<SelectListItem> selList = from s in _context.HJItemClasses.Include(x => x.HJItemMasterClass).OrderBy(x => x.HJItemMasterClass.DivisionId).ThenBy(x => x.ClassNumber)
                                                          select new SelectListItem
                                                          {
                                                              Value = s.Id.ToString(),
                                                              Text = s.NameAndNumber
                                                          };
                    return selList;
                }
                
            }
            else
            {
                if (MasterClassId != null)
                {
                    IEnumerable<SelectListItem> selList = from s in _context.HJItemClasses.Include(x => x.HJItemMasterClass).Where(x => x.HJItemMasterClassId.Equals(MasterClassId) && x.HJItemMasterClass.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.HJItemMasterClass.DivisionId).ThenBy(x => x.ClassNumber)
                                                          select new SelectListItem
                                                          {
                                                              Value = s.Id.ToString(),
                                                              Text = s.NameAndNumber
                                                          };
                    return selList;
                }
                else
                {
                    IEnumerable<SelectListItem> selList = from s in _context.HJItemClasses.Include(x => x.HJItemMasterClass).Where(x => x.HJItemMasterClass.DivisionId.Equals(user.DivisionId)).OrderBy(x => x.HJItemMasterClass.DivisionId).ThenBy(x => x.ClassNumber)
                                                          select new SelectListItem
                                                          {
                                                              Value = s.Id.ToString(),
                                                              Text = s.NameAndNumber
                                                          };
                    return selList;
                }
            }
        }
       
        public async Task<IEnumerable<SelectListItem>> GetProjectList2()
        {

            if (User.IsInRole("Admin"))
            {
                
                IEnumerable<SelectListItem> selList = from s in _context.Projects.OrderBy(x => x.DivisionId).ThenBy(x => x.Name).Where(x => x.Active.Equals(true))
                                                      select new SelectListItem
                                                      {
                                                          Value = s.Id.ToString(),
                                                          Text = s.Name + " : " + s.ProjectNr
                                                      };
                return selList;
            }
            else if (User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
            {
                var user = await _userManager.GetUserAsync(User);
                var projects = await (from p in _context.Projects.OrderBy(x => x.DivisionId).ThenBy(x => x.Name) join pu in _context.ProjectUsers on p.Id equals pu.projectId where pu.userId.Equals(user.Id) && p.Active.Equals(true) select p).ToListAsync();
                IEnumerable<SelectListItem> selList = from s in projects
                                                      select new SelectListItem
                                                      {
                                                          Value = s.Id.ToString(),
                                                          Text = s.Name + " : " + s.ProjectNr
                                                      };
                return selList;
            }
            else if (User.IsInRole("International"))
            {
                var user = await _userManager.GetUserAsync(User);
                var projects = _context.Projects.Where(x => x.Name.Contains("STOCK") && x.Active.Equals(true)).OrderBy(x => x.DivisionId).ThenBy(x => x.Name).ToList();
                IEnumerable<SelectListItem> selList = from s in projects.Where(x => x.Active.Equals(true)).OrderBy(x => x.Name)
                                                      select new SelectListItem
                                                      {
                                                          Value = s.Id.ToString(),
                                                          Text = s.Name + " : " + s.ProjectNr
                                                      };
                return selList;
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                var projects = _context.Projects.Where(x => x.DivisionId.Equals(user.DivisionId) && x.Active.Equals(true)).OrderBy(x => x.DivisionId).ThenBy(x => x.Name).ToList();
                IEnumerable<SelectListItem> selList = from s in projects
                                                      select new SelectListItem
                                                      {
                                                          Value = s.Id.ToString(),
                                                          Text = s.Name + " : " + s.ProjectNr
                                                      };
                return selList;
            }
        }
        public async Task<IEnumerable<SelectListItem>> GetProjectList3()
        {

            if (User.IsInRole("Admin"))
            {
                var projects = _context.Projects.OrderBy(x => x.DivisionId).ThenBy(x => x.Name).ToList();
                IEnumerable<SelectListItem> selList = from s in projects.Where(x => x.Active.Equals(true))
                                                      select new SelectListItem
                                                      {
                                                          Value = s.Id.ToString(),
                                                          Text = s.Name + " : " + s.ProjectNr
                                                      };
                return selList;
            }
            else if (User.IsInRole("Guest") || User.IsInRole("MemberGuest"))
            {
                var user = await _userManager.GetUserAsync(User);
                var projects = await (from p in _context.Projects.OrderBy(x => x.DivisionId).ThenBy(x => x.Name) join pu in _context.ProjectUsers on p.Id equals pu.projectId where pu.userId.Equals(user.Id) && p.Active.Equals(true) select p).ToListAsync();
                IEnumerable<SelectListItem> selList = from s in projects
                                                      select new SelectListItem
                                                      {
                                                          Value = s.Id.ToString(),
                                                          Text = s.Name + " : " + s.ProjectNr
                                                      };
                return selList;
            }
            else if (User.IsInRole("International"))
            {
                var user = await _userManager.GetUserAsync(User);
                var projects = _context.Projects.Where(x => x.Name.Contains("STOCK") && x.Active.Equals(true) && user.DivisionId.Equals(x.DivisionId)).OrderBy(x => x.DivisionId).ThenBy(x => x.Name).ToList();
                IEnumerable<SelectListItem> selList = from s in projects.Where(x => x.Active.Equals(true)).OrderBy(x => x.Name)
                                                      select new SelectListItem
                                                      {
                                                          Value = s.Id.ToString(),
                                                          Text = s.Name + " : " + s.ProjectNr
                                                      };
                return selList;
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                var projects = _context.Projects.Where(x => x.DivisionId.Equals(user.DivisionId) && x.Active.Equals(true)).OrderBy(x => x.Name).OrderBy(x => x.DivisionId).ThenBy(x => x.Name).ToList();
                IEnumerable<SelectListItem> selList = from s in projects
                                                      select new SelectListItem
                                                      {
                                                          Value = s.Id.ToString(),
                                                          Text = s.Name + " : " + s.ProjectNr
                                                      };
                return selList;
            }
        }

    }
}