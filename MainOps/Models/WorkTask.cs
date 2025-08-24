using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class WorkTaskEnd
    {
        public WorkTask? WorkTask { get; set; }
        public List<WorkItem> WorkItems { get; set; }
        public WorkTaskEnd()
        {
            WorkItems = new List<WorkItem>();
        }
    }
    public class WorkTask
    {
        public int Id { get; set; }
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        [Display(Name = "Project")]
        public virtual Project? Project { get; set; }
        [Display(Name = "SubProject")]
        public int? SubProjectId { get; set; }
        [Display(Name = "SubProject")]
        public virtual SubProject? SubProject { get; set; }
        [Display(Name = "Is Started?")]
        public bool IsStarted { get; set; }
        [Display(Name = "Is Finished?")]
        public bool IsFinished { get; set; }
        public DateTime? TimeFinished { get; set; }
        public DateTime? TimeStarted { get; set; }
        [Display(Name = "Date")]
        public DateTime DateToDo { get; set; }
        [Display(Name = "Comments")]
        public string? Comments_Office { get; set; }
        [Display(Name = "Comments")]
        public string? Comments_Workers { get; set; }
        [Display(Name = "In Charge")]
        public string? InChargeId { get; set; }
        [Display(Name = "Worker")]
        public string? WorkerId { get; set; }
        public ICollection<WorkItem>? WorkItems { get; set; }
        public ICollection<Feedback>? Feedbacks { get; set; }
        public WorkTask()
        {

        }
        public WorkTask(WorkTaskVM modelIn,ApplicationUser InChargeUser,string Worker)
        {
            ProjectId = modelIn.ProjectId;
            SubProjectId = modelIn.SubProjectId;
            IsStarted = false;
            IsFinished = false;
            TimeFinished = null;
            TimeStarted = null;
            DateToDo = modelIn.DateToDo;
            Comments_Office = modelIn.Comments_Office;
            Comments_Workers = "";
            InChargeId = InChargeUser.Id;
            WorkerId = Worker;
        }
    }
    public class WorkItem
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        //public double? Amount { get; set; }
        public ICollection<PhotoFileWorkItem>? Photos { get; set; }
        public string? Comment_Worker { get; set; }
        public int? WorkTaskId { get; set; }
        public virtual WorkTask? WorkTask { get; set; }
        public bool IsFinished { get; set; }
        public DateTime? TimeFinished { get; set; }
        public ICollection<Feedback>? Feedbacks { get; set; }
        public WorkItem(WorkItem i,int worktaskid)
        {
            this.Description = i.Description;
            this.Comment_Worker = i.Comment_Worker;
            this.IsFinished = i.IsFinished;
            this.TimeFinished = i.TimeFinished;
            this.WorkTaskId = worktaskid;
        }
        public WorkItem()
        {

        }

    }
    public class Feedback
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public DateTime TimeStamp { get; set; }
        public int? WorkTaskId { get; set; }
        public virtual WorkTask? WorkTask { get; set; }
        public int? WorkItemId { get; set; }
        public virtual WorkItem? WorkItem { get; set; }
        public ICollection<PhotoFileFeedback>? Photos { get; set; }
        public Feedback()
        {

        }
        public Feedback(int WorkTaskId)
        {
            this.WorkTaskId = WorkTaskId;
        }
        public Feedback(int WorkTaskId,int WorkItemId)
        {
            this.WorkTaskId = WorkTaskId;
            this.WorkItemId = WorkItemId;
        }
    }
    public class PhotoFileFeedback
    {
        public int Id { get; set; }
        public int? FeedbackId { get; set; }
        public virtual Feedback? Feedback { get; set; }
        public string? path { get; set; }
    }
    public class PhotoFileWorkItem
    {
        public int Id { get; set; }
        public int? WorkItemId { get; set; }
        public virtual WorkItem? WorkItem { get; set; }
        public string? path { get; set; }
    }
    public class WorkTaskVM
    {
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        [Display(Name = "Project")]
        public virtual Project? Project { get; set; }
        [Display(Name = "SubProject")]
        public int? SubProjectId { get; set; }
        [Display(Name = "SubProject")]
        public virtual SubProject? SubProject { get; set; }
        [Display(Name = "Date")]
        public DateTime DateToDo { get; set; }
        [Display(Name = "Comments")]
        public string? Comments_Office { get; set; }
        public string? users { get; set; }
        public List<WorkItem> WorkItems { get; set; }
        public WorkTaskVM()
        {
            WorkItems = new List<WorkItem>();
        }
    }
}
