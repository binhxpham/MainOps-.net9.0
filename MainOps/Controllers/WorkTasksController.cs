using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MainOps.Data;
using MainOps.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Microsoft.AspNetCore.Authorization;
using ImageMagick;
using Docnet.Core;
using MainOps.ExtensionMethods;
using System.Text;
using MainOps.Models.ReportClasses;

namespace MainOps.Controllers
{
    public class WorkTasksController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public WorkTasksController(DataContext context,UserManager<ApplicationUser> userManager, IWebHostEnvironment env):base(context,userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }
        public async Task<IActionResult> TextView()
        {
            ViewData["ProjectId"] = await GetProjectList3();
            return View("WorkTaskText");
        }
        [HttpGet]
        public async Task<string> GetTaskText(int ProjectId,int? SubProjectId,string dato2,string userid)
        {
            var user = await _userManager.GetUserAsync(User);
            DateTime dato = Convert.ToDateTime(dato2);
            string thetext = "";
            List<WorkTask> tasks = new List<WorkTask>();
            if(SubProjectId == null)
            {
                tasks = await _context.WorkTasks
                .Include(x => x.WorkItems).ThenInclude(x => x.Feedbacks).ThenInclude(x => x.Photos)
                .Where(x =>
                    x.ProjectId.Equals(ProjectId) &&
                    x.IsStarted.Equals(true) &&
                    x.WorkerId.Equals(userid) &&
                    Convert.ToDateTime(x.TimeStarted).Date <= dato.Date)
                    .OrderBy(x => x.TimeStarted).ThenBy(x => x.TimeFinished)
                .ToListAsync();
            }
            else
            {
                tasks = await _context.WorkTasks
                .Include(x => x.WorkItems).ThenInclude(x => x.Feedbacks).ThenInclude(x => x.Photos)
                .Where(x =>
                    x.ProjectId.Equals(ProjectId) &&
                    x.SubProjectId.Equals(SubProjectId) &&
                    x.IsStarted.Equals(true) &&
                    x.WorkerId.Equals(userid) &&
                    Convert.ToDateTime(x.TimeStarted).Date <= dato.Date)
                    .OrderBy(x => x.TimeStarted).ThenBy(x => x.TimeFinished)
                .ToListAsync();
            }
            //tasks = tasks.Where(x => x.WorkItems.Where(y => Convert.ToDateTime(y.TimeFinished).Date.Equals(dato.Date)).Count() >= 1 || Convert.ToDateTime(x.TimeFinished).Date.Equals(dato.Date)).ToList();
            int taskcount = 1;
            int numtasks = tasks.Count();
            foreach(var t in tasks)
            {
                if (t.IsFinished) {
                    if (Convert.ToDateTime(t.TimeFinished).Date.Equals(dato.Date))
                    {
                        thetext += "Task " + taskcount.ToString() + ": " + t.Comments_Office + ". Finished Today at: " + Convert.ToDateTime(t.TimeFinished).ToLocalTime().ToString() + Environment.NewLine;
                    }
                    else
                    {
                        thetext += "Task " + taskcount.ToString() + ": " + t.Comments_Office + ". Finished Previously." + Environment.NewLine;
                    }
                }
                else
                {
                    thetext += "Task " + taskcount.ToString() + ": " + t.Comments_Office + ":" + Environment.NewLine;
                }
                var feedbacks = await _context.Feedbacks.Where(x => x.WorkTaskId.Equals(t.Id) && x.WorkItemId == null).ToListAsync();
                foreach (var f in feedbacks)// add feedback from total task
                {
                    int counterf = 1;
                    if(f.Text != "" && f.Text != null) {
                        if (!thetext.Contains(f.Text.Replace("\n", "").Replace("\r", ".")))
                        { 
                            thetext += "        feedback " + counterf.ToString() + ": " + f.Text.Replace("\n", "").Replace("\r", ".") + ". At " + Convert.ToDateTime(f.TimeStamp).ToLocalTime().ToString() + Environment.NewLine;
                        }
                        counterf += 1;
                    }
                }
                int counter = 1;
                foreach (var wt in t.WorkItems)
                {                    
                    if (wt.IsFinished)
                    {
                        if (Convert.ToDateTime(wt.TimeFinished).Date.Equals(dato.Date))
                        {
                            thetext += "    item " + counter.ToString() + ": " + wt.Description + ". Finished Today at: " + Convert.ToDateTime(wt.TimeFinished).ToLocalTime().ToString() + Environment.NewLine;
                            thetext.Replace("..", ".");
                            int counterwt = 1;
                            foreach (var fwt in wt.Feedbacks) // add feedback from specific work item
                            {
                                if (!thetext.Contains(fwt.Text.Replace("\n", "").Replace("\r", ".")))
                                {
                                    thetext += "        feedback " + counterwt.ToString() + ": " + fwt.Text.Replace("\n", "").Replace("\r", ".") + ". At " + Convert.ToDateTime(fwt.TimeStamp).ToLocalTime().ToString() + Environment.NewLine;
                                    counterwt += 1;
                                }
                            }
                            counter += 1;
                        }
                        else
                        {
                            thetext += "    item " + counter.ToString() + ": " + wt.Description + ". Finished Previously." + Environment.NewLine;
                            thetext.Replace("..", ".");
                            int counterwt = 1;
                            foreach (var fwt in wt.Feedbacks) // add feedback from specific work item
                            {
                                if (!thetext.Contains(fwt.Text.Replace("\n", "").Replace("\r", ".")))
                                {
                                    thetext += "        feedback " + counterwt.ToString() + ": " + fwt.Text.Replace("\n", "").Replace("\r", ".") + ". At " + Convert.ToDateTime(fwt.TimeStamp).ToLocalTime().ToString() + Environment.NewLine;
                                    counterwt += 1;
                                }
                            }
                            counter += 1;
                        }
                        
                    }
                }
                taskcount += 1;
                if(taskcount <= numtasks) { 
                    thetext += Environment.NewLine;
                }
            }
            thetext = thetext.Replace("..", ".").Replace(". .",".");
            //return Json(thetext);
            return thetext;
        }
        public async Task<IActionResult> GenerateDailyReportFromTasks(int? id)
        {

                if (id != null)
                {
                var worktask = await _context.WorkTasks.Include(x => x.WorkItems).SingleOrDefaultAsync(x => x.Id.Equals(id));
                if (worktask.IsFinished)
                {
                    
                    var theworker = await _context.Users.SingleOrDefaultAsync(x => x.Id.Equals(worktask.WorkerId));
                    var lastdailyreportuser = await _context.Daily_Report_2s.Where(x => x.ProjectId.Equals(worktask.ProjectId) && x.DoneBy.Equals(theworker.full_name())).OrderByDescending(x => x.Report_Date).FirstOrDefaultAsync();
                    var lastdailyreportwithsignature = await _context.Daily_Report_2s.Where(x => x.DoneBy.Equals(theworker.full_name()) && x.Signature != null).FirstOrDefaultAsync();
                    List<WorkItem> actualWorkItems = worktask.WorkItems.Where(x => x.TimeFinished != null).ToList();
                    int usedTitle;
                    if (lastdailyreportuser != null)
                    {
                       usedTitle = lastdailyreportuser.TitleId;

                    }
                    else
                    {
                        var othertitle = await _context.Titles.Where(x => x.ProjectId.Equals(worktask.ProjectId) && x.TheTitle.ToLower().Contains("groundwater") || x.TheTitle.ToLower().Contains("grundva")).FirstOrDefaultAsync();
                        if (othertitle != null)
                        {
                            usedTitle = othertitle.Id;
                        }
                        else
                        {
                            var lasttitle = await _context.Titles.Where(x => x.ProjectId.Equals(worktask.ProjectId)).FirstOrDefaultAsync();
                            usedTitle = lasttitle.Id;
                        }
                    }
                    foreach (var day in actualWorkItems.Select(x => x.TimeFinished.Value.Date).Distinct())
                    {
                        Daily_Report_2 dr = new Daily_Report_2();
                        var var1 = actualWorkItems.Where(x => x.TimeFinished.Value.Date.Equals(day)).ToList();
                        var var2 = var1.Min(x => x.TimeFinished);
                        var var3 = var2.Value.AddHours(-1).Hour;
                        dr.StartHour = new TimeSpan(var3, var2.Value.Minute, 0);
                        var1 = actualWorkItems.Where(x => x.TimeFinished.Value.Date.Equals(day)).ToList();
                        var2 = var1.Max(x => x.TimeFinished);
                        var3 = var2.Value.Hour;
                        dr.EndHour = new TimeSpan(var3, var2.Value.Minute, 0);
                        dr.TitleId = usedTitle;
                        dr.DoneBy = theworker.full_name();
                        dr.ProjectId = worktask.ProjectId;
                        dr.SubProjectId = worktask.SubProjectId;
                        dr.Work_Performed = await GetTaskText(worktask.ProjectId, worktask.SubProjectId, Convert.ToDateTime(worktask.TimeFinished).ToString(), worktask.WorkerId);
                        dr.Amount = 1;
                        dr.EnteredIntoDataBase = DateTime.Now;
                        dr.short_Description = worktask.Comments_Office;
                        dr.Report_Date = var2.Value.Date;
                        if (lastdailyreportwithsignature != null)
                        {
                            dr.Signature = lastdailyreportwithsignature.Signature;
                        }
                        else
                        {
                            dr.Signature = "";
                        }
                        _context.Add(dr);
                        
                    }
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "Worktask is not finished"});
                }
                
                return RedirectToAction(nameof(Index));

            }
            else { return NotFound(); }

            
        }
        public async Task<int> GetAmountWorkTasks()
        {
            var user = await _userManager.GetUserAsync(User);
            int tasks = _context.WorkTasks.Include(x => x.Project).Include(x => x.SubProject).Include(x => x.WorkItems).ThenInclude(x => x.Photos).Where(x => x.WorkerId.Equals(user.Id) && x.IsFinished.Equals(false)).Count();
            return tasks;
        }
        // GET: WorkTasks
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin"))
            {
                var dataContext = await _context.WorkTasks.Include(w => w.Project).
                    Include(w => w.SubProject)
                    .Include(x=>x.WorkItems).ThenInclude(x=>x.Feedbacks).ThenInclude(x=>x.Photos)
                    .Include(x=>x.Feedbacks).ThenInclude(x=>x.Photos)
                    .Where(x => x.InChargeId.Equals(user.Id) || x.WorkerId.Equals(user.Id)).OrderByDescending(x => x.Id).ToListAsync();
                return View(dataContext);
            }
            else
            {
                var dataContext = await _context.WorkTasks
                    .Include(w => w.Project)
                    .Include(w => w.SubProject)
                    .Include(x => x.WorkItems).ThenInclude(x => x.Feedbacks).ThenInclude(x => x.Photos)
                    .Include(x => x.Feedbacks).ThenInclude(x=>x.Photos)
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();
                return View(dataContext);
            }
        }
        [HttpGet]
        public async Task<IActionResult> EndTask3(int? id)
        {
            if (id != null)
            {
                var user = await _userManager.GetUserAsync(User);
                
                var task = await _context.WorkTasks.FindAsync(id);
                if (user.Id.Equals(task.InChargeId) || User.IsInRole("Admin"))
                {
                    task.TimeFinished = DateTime.Now;
                    task.IsFinished = true;
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "This is not your task to end" });
                }
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet]
        public async Task<IActionResult> EndTask2(int? id)
        {
            if(id != null)
            {
                var task = await _context.WorkTasks.FindAsync(id);
                var items = await _context.WorkItems.Where(x => x.WorkTaskId.Equals(task.Id)).ToListAsync();
                var user = await _userManager.GetUserAsync(User);
                if (user.Id.Equals(task.WorkerId) || user.Id.Equals(task.InChargeId))
                {
                    WorkTaskEnd model = new WorkTaskEnd();
                    model.WorkItems = items;
                    model.WorkTask = task;
                    return PartialView("_EnfdTask", model);
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "This is not your task to end" });
                }
            }
            else{
                return NotFound();
            }
        }
        [HttpGet]
        public async Task<IActionResult> EndWorkITem2(int? id)
        {
            if (id != null)
            {
                var workitem = await _context.WorkItems.FindAsync(id);
                return PartialView("_EndWorkItem", workitem);
            }
            else
            {
                return NotFound();
            }
        }
        public async Task<string> StartTask(int? id)
        {
            if(id != null)
            {
                var user = await _userManager.GetUserAsync(User);                
                var worktask = await _context.WorkTasks.FindAsync(id);
                if (worktask.WorkerId.Equals(user.Id) || User.IsInRole("Admin") || worktask.InChargeId.Equals(user.Id))
                { 
                    worktask.IsStarted = true;
                    worktask.TimeStarted = DateTime.Now;
                    _context.Update(worktask);
                    await _context.SaveChangesAsync();
                    
                    return "orange";
                }
                else
                {
                    return "";
                }
            }
            else { return ""; }
        }
        [HttpPost]
        public async Task<IActionResult> EndTask(WorkTaskEnd model, IFormFile[] files)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if(model.WorkTask.WorkerId.Equals(user.Id) || model.WorkTask.InChargeId.Equals(user.Id))
                {
                    model.WorkTask.IsFinished = true;
                    model.WorkTask.TimeFinished = DateTime.Now;
                    foreach(var item in model.WorkItems)
                    {
                        _context.Update(item);
                    }
                    _context.Update(model.WorkTask);
                    await _context.SaveChangesAsync();
                    //var color = await EndTask(model.Id);
                    if (files != null)
                    {

                        Feedback feedback = new Feedback { WorkTaskId = model.WorkTask.Id, WorkItemId = null, TimeStamp = DateTime.Now, Text = model.WorkTask.Comments_Workers };
                        _context.Add(feedback);
                        await _context.SaveChangesAsync();
                        var lastadded = await _context.Feedbacks.LastAsync();
                        var directory = _env.WebRootPath + "\\Tasks\\Feedbacks\\" + lastadded.Id.ToString() + "\\";
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }
                        foreach (var thephoto in files)
                        {
                            var path = Path.Combine(directory, thephoto.FileName);

                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await thephoto.CopyToAsync(stream);
                            };
                            if (path.Contains(".jpg") || path.Contains(".jpeg"))
                            {
                                PhotoExtensions.SaveAndCompressJpeg(path, 95);
                            }
                            else if (path.Contains(".pdf"))
                            {
                                PhotoExtensions.ConvertPdfToPng(path);
                                System.IO.File.Delete(path);
                            }
                            if (path.Contains(".pdf"))
                            {
                                PhotoFileFeedback photo = new PhotoFileFeedback { FeedbackId = lastadded.Id, path = path.Replace(".pdf", ".png") };
                                _context.FeedbackPhotos.Add(photo);
                            }
                            else
                            {
                                PhotoFileFeedback photo = new PhotoFileFeedback { FeedbackId = lastadded.Id, path = path };
                                _context.FeedbackPhotos.Add(photo);
                            }
                            await _context.SaveChangesAsync();
                        }
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "This task is not yours to end" });
                }
            }
            else
            {
                return PartialView("_EndTask", model);
            }
            
        }
        [HttpPost]
        public async Task<IActionResult> EndWorkItem(WorkItem model,IFormFile[] files)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var worktask = await _context.WorkTasks.FindAsync(model.WorkTaskId);
                if (worktask.WorkerId.Equals(user.Id) || worktask.InChargeId.Equals(user.Id))
                {
                    model.IsFinished = true;
                    model.TimeFinished = DateTime.Now;
                    _context.Update(model);
                    if(worktask.IsStarted != true)
                    {
                        worktask.IsStarted = true;
                    }
                    if(worktask.TimeStarted == null)
                    {
                        worktask.TimeStarted = DateTime.Now;
                    }
                    _context.Update(worktask);
                    await _context.SaveChangesAsync();
                    //var color = await EndWorkItem(model.Id);
                    if(files != null) {                        
                        Feedback feedback = new Feedback { WorkTaskId = model.WorkTaskId, WorkItemId = model.Id, TimeStamp = DateTime.Now, Text = model.Comment_Worker };
                        _context.Add(feedback);
                        await _context.SaveChangesAsync();
                        var lastadded = await _context.Feedbacks.LastAsync();
                        var directory = _env.WebRootPath + "\\Tasks\\Feedbacks\\" + lastadded.Id.ToString() + "\\";
                        if (!Directory.Exists(directory) && files != null)
                        {
                            Directory.CreateDirectory(directory);
                        }
                        foreach (var thephoto in files)
                        {
                            var path = Path.Combine(directory, thephoto.FileName);
                            
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await thephoto.CopyToAsync(stream);
                            };
                            if (path.Contains(".jpg") || path.Contains(".jpeg"))
                            {
                                PhotoExtensions.SaveAndCompressJpeg(path, 95);
                            }
                            else if (path.Contains(".pdf"))
                            {
                                PhotoExtensions.ConvertPdfToPng(path);
                                System.IO.File.Delete(path);
                            }
                            if (path.Contains(".pdf"))
                            {
                                PhotoFileFeedback photo = new PhotoFileFeedback { FeedbackId = lastadded.Id, path = path.Replace(".pdf", ".png") };
                                _context.FeedbackPhotos.Add(photo);
                            }
                            else
                            {
                                PhotoFileFeedback photo = new PhotoFileFeedback { FeedbackId = lastadded.Id, path = path };
                                _context.FeedbackPhotos.Add(photo);
                            }
                            await _context.SaveChangesAsync();
                        }
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("ErrorMessage", "Home", new { text = "This task is not yours to end" });
                }
            }
            else
            {
                return PartialView("_EndTask", model);
            }

        }
        //public async Task<string> EndTask(int? id)
        //{
        //    if (id != null)
        //    {
        //        var user = await _userManager.GetUserAsync(User);
        //        var worktask = await _context.WorkTasks.FindAsync(id);
        //        if (worktask.WorkerId.Equals(user.Id) || User.IsInRole("Admin") || worktask.InChargeId.Equals(user.Id))
        //        {
        //            worktask.IsFinished = true;
        //            worktask.TimeFinished = DateTime.Now;
        //            _context.Update(worktask);
        //            await _context.SaveChangesAsync();
        //            return "red";
        //        }
        //        else
        //        {
        //            return "";
        //        }
        //    }
        //    else { return ""; }
        //}
        //public async Task<string> EndWorkItem(int? id)
        //{
        //    if (id != null)
        //    {
        //        var user = await _userManager.GetUserAsync(User);
        //        var workitem = await _context.WorkItems.Include(x => x.WorkTask).Where(x=>x.Id.Equals(id)).SingleOrDefaultAsync();
        //        if (workitem.WorkTask.WorkerId.Equals(user.Id) || User.IsInRole("Admin") || workitem.WorkTask.InChargeId.Equals(user.Id))
        //        {
        //            workitem.IsFinished = true;
        //            workitem.TimeFinished = DateTime.Now;
        //            _context.Update(workitem);
        //            await _context.SaveChangesAsync();
        //            return "lightgreen";
        //        }
        //        else
        //        {
        //            return "";
        //        }
        //    }
        //    else { return ""; }
        //}
        [HttpGet]
        public IActionResult Feedback(int? WorkTaskId,int? WorkItemId)
        {
            if(WorkTaskId != null)
            { 
                if(WorkItemId != null)
                {
                    Feedback model = new Feedback(Convert.ToInt32(WorkTaskId),Convert.ToInt32(WorkItemId));
                    return PartialView("_Feedback",model);
                }
                else
                {
                    Feedback model = new Feedback(Convert.ToInt32(WorkTaskId));
                    return PartialView("_Feedback",model);
                }
            }
            else {
                return NotFound();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Feedback(Feedback model,IFormFile[] files)
        {
            if (ModelState.IsValid)
            {
                model.TimeStamp = DateTime.Now;
                _context.Feedbacks.Add(model);
                await _context.SaveChangesAsync();
                if(files != null)
                {
                    var lastadded = await _context.Feedbacks.LastAsync();
                    var directory = _env.WebRootPath + "\\Tasks\\Feedbacks\\" + lastadded.Id.ToString() + "\\";
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    foreach (var thephoto in files)
                    {
                        var path = Path.Combine(directory, thephoto.FileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await thephoto.CopyToAsync(stream);
                        };
                        if (path.Contains(".jpg") || path.Contains(".jpeg"))
                        {
                            PhotoExtensions.SaveAndCompressJpeg(path, 95);
                        }
                        else if (path.Contains(".pdf"))
                        {
                            PhotoExtensions.ConvertPdfToPng(path);
                            System.IO.File.Delete(path);
                        }
                        if (path.Contains(".pdf"))
                        {
                            PhotoFileFeedback photo = new PhotoFileFeedback { FeedbackId = lastadded.Id, path = path.Replace(".pdf", ".png") };
                            _context.FeedbackPhotos.Add(photo);
                        }
                        else
                        {
                            PhotoFileFeedback photo = new PhotoFileFeedback { FeedbackId = lastadded.Id, path = path };
                            _context.FeedbackPhotos.Add(photo);
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                return RedirectToAction("Index","Home");

            }
            else
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "Error while creating feedback" });
            }
        }
        public async Task<IActionResult> GetUserTasks()
        {
            var user = await _userManager.GetUserAsync(User);
            var tasks = await _context.WorkTasks.Include(x=>x.Project).Include(x=>x.SubProject).Include(x=>x.WorkItems).ThenInclude(x=>x.Photos).Where(x => x.WorkerId.Equals(user.Id) && x.IsFinished.Equals(false)).ToListAsync();
            return PartialView("_WorkTasks",tasks);
        }
        // GET: WorkTasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workTask = await _context.WorkTasks
                .Include(w => w.Project)
                .Include(w => w.SubProject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workTask == null)
            {
                return NotFound();
            }

            return View(workTask);
        }
        [HttpPost]
        public async Task<IActionResult> Create(WorkTaskVM model,IFormFile[] Photos_1,IFormFile[] Photos_2, IFormFile[] Photos_3, IFormFile[] Photos_4, IFormFile[] Photos_5
            , IFormFile[] Photos_6, IFormFile[] Photos_7, IFormFile[] Photos_8, IFormFile[] Photos_9, IFormFile[] Photos_10)
        {
            if (ModelState.IsValid) {
                var user = await _userManager.GetUserAsync(User);
                foreach (string userid in model.users.Split(" "))
                {
                    if(userid.Trim() != "") { 
                        WorkTask workTask = new WorkTask(model, user, userid);
                        _context.Add(workTask);
                        await _context.SaveChangesAsync();
                        var lastadded = await _context.WorkTasks.LastAsync();
                        int counter = 1;
                        foreach (WorkItem item in model.WorkItems.Where(x => x.Description != "" && x.Description != null))
                        {
                            WorkItem newWorkItem = new WorkItem(item,lastadded.Id);
                            _context.Add(newWorkItem);
                            await _context.SaveChangesAsync();
                            var lastitemadded = await _context.WorkItems.LastAsync();
                            IFormFile[] files = Photos_1;
                            if (counter == 1)
                            {
                                files = Photos_1;
                            }
                            else if (counter == 2)
                            {
                                files = Photos_2;
                            }
                            else if (counter == 3)
                            {
                                files = Photos_3;
                            }
                            else if (counter == 4)
                            {
                                files = Photos_4;
                            }
                            else if (counter == 5)
                            {
                                files = Photos_5;
                            }
                            else if (counter == 6)
                            {
                                files = Photos_6;
                            }
                            else if (counter == 7)
                            {
                                files = Photos_7;
                            }
                            else if (counter == 8)
                            {
                                files = Photos_8;
                            }
                            else if (counter == 9)
                            {
                                files = Photos_9;
                            }
                            else if (counter == 10)
                            {
                                files = Photos_10;
                            }
                            if (files != null)
                            {
                                var directory = _env.WebRootPath + "\\Tasks\\WorkTasks\\" + lastadded.Id.ToString() + "\\" + lastitemadded.Id.ToString() + "\\";
                                if (!Directory.Exists(directory))
                                {
                                    Directory.CreateDirectory(directory);
                                }
                                foreach (var thephoto in files)
                                {
                                    var path = Path.Combine(directory, thephoto.FileName);

                                    using (var stream = new FileStream(path, FileMode.Create))
                                    {
                                        await thephoto.CopyToAsync(stream);
                                    };
                                    if (path.Contains(".jpg") || path.Contains(".jpeg"))
                                    {
                                        PhotoExtensions.SaveAndCompressJpeg(path, 90);
                                    }
                                    else if (path.Contains(".pdf"))
                                    {
                                        PhotoExtensions.ConvertPdfToPng(path);
                                        System.IO.File.Delete(path);
                                    }
                                    if (path.Contains(".pdf"))
                                    {
                                        PhotoFileWorkItem photo = new PhotoFileWorkItem { WorkItemId = lastitemadded.Id, path = path.Replace(".pdf", ".png") };
                                        _context.WorkItemPhotos.Add(photo);
                                    }
                                    else
                                    {
                                        PhotoFileWorkItem photo = new PhotoFileWorkItem { WorkItemId = lastitemadded.Id, path = path };
                                        _context.WorkItemPhotos.Add(photo);
                                    }

                                    await _context.SaveChangesAsync();
                                }

                            }
                            counter += 1;

                        }
                    }
                }
                return RedirectToAction("MainMenu", "TrackItems");
            }
            else
            {
                return RedirectToAction("Create", "WorkTasks", model);
            }

        }
        // GET: WorkTasks/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var theuser = await _userManager.GetUserAsync(User);
            WorkTaskVM model = new WorkTaskVM();
            for(int i = 0; i < 10; i++)
            {
                WorkItem newItem = new WorkItem();
                model.WorkItems.Add(newItem);
            }
            ViewData["ProjectId"] = await GetProjectList3();
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Name");
            var obj = (from user in _context.Users.Where(x => x.DivisionId.Equals(theuser.DivisionId) && x.Active.Equals(true))
                       select new
                       {
                           UserId = user.Id,
                           FullName = user.full_name(),                                 
                           RoleNames = string.Join(",", (from userRole in _context.UserRoles
                                                                join role in _context.Roles on userRole.RoleId
                                                                equals role.Id
                                                                where userRole.UserId == user.Id
                                                                select role.Name).ToList())
                       }).ToList();
            IEnumerable<SelectListItem> selList = from s in obj
                                                  where !(
                                                  s.RoleNames.Contains("Manager") 
                                                  || s.RoleNames.Contains("Guest")
                                                  || s.RoleNames.Contains("MemberGuest")
                                                  || s.RoleNames.Contains("Admin")
                                                  || s.RoleNames.Contains("DivisionAdmin")
                                                  )
                                                  select new SelectListItem
                                                  {
                                                      Value = s.UserId,
                                                      Text = s.FullName
                                                  };

            
            ViewData["Users"] = selList;
            //
            return View(model);
        }

        // GET: WorkTasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workTask = await _context.WorkTasks.FindAsync(id);
            if (workTask == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Abbreviation", workTask.ProjectId);
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Id", workTask.SubProjectId);
            return View(workTask);
        }

        // POST: WorkTasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,SubProjectId,IsStarted,IsFinished,TimeFinished,TimeStarted,DateToDo,Comments_Office,Comments_Workers,InChargeId,WorkerId")] WorkTask workTask)
        {
            if (id != workTask.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkTaskExists(workTask.Id))
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
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Abbreviation", workTask.ProjectId);
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects, "Id", "Id", workTask.SubProjectId);
            return View(workTask);
        }

        // GET: WorkTasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var workTask = await _context.WorkTasks.SingleOrDefaultAsync(x => x.Id.Equals(id));
            if(workTask == null)
            {
                return NotFound();
            }
            return View(workTask);
        }

        // POST: WorkTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workTask = await _context.WorkTasks.Include(x => x.Feedbacks).Include(x => x.WorkItems).FirstOrDefaultAsync(m => m.Id == id);
            var feedbacksWorktask = await _context.Feedbacks.Include(x=>x.Photos).Where(x => x.WorkTaskId.Equals(id)).ToListAsync();
            var workitemsworktask = await _context.WorkItems.Include(x => x.Photos).Where(x => x.WorkTaskId.Equals(id)).ToListAsync();
            foreach (var feedback in feedbacksWorktask)
            {
                foreach (var photo in feedback.Photos)
                {
                    _context.Remove(photo);
                }
                await _context.SaveChangesAsync();
                _context.Remove(feedback);
            }
            await _context.SaveChangesAsync();

            foreach (var workitem in workitemsworktask)
            {
                foreach (var photo in workitem.Photos)
                {
                    _context.Remove(photo);
                }
                await _context.SaveChangesAsync();
                var feedbacks = await _context.Feedbacks.Where(x => x.WorkItemId.Equals(workitem.Id)).ToListAsync();
                foreach (var feedback in feedbacks)
                {
                    var photosfeedback = await _context.FeedbackPhotos.Where(x => x.FeedbackId.Equals(feedback.Id)).ToListAsync();
                    foreach (var photo in photosfeedback)
                    {
                        _context.Remove(photo);
                    }
                    await _context.SaveChangesAsync();
                    _context.Remove(feedback);
                }
                await _context.SaveChangesAsync();
                _context.Remove(workitem);
            }
            await _context.SaveChangesAsync();
            _context.Remove(workTask);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkTaskExists(int id)
        {
            return _context.WorkTasks.Any(e => e.Id == id);
        } 
    }
}
