using MainOps.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class ToolBox
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        [Display(Name = "Report Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Report_Date { get; set; }
        [Display(Name = "Performed By")]
        public string DoneBy { get; set; }
        [Display(Name = "Work tasks coming week")]
        public string Work_Tasks { get; set; }
        [Display(Name = "Safety connected to aforementioned tasks")]
        public string Safety_Aspects { get; set; }
        [Display(Name = "Mapping of especially dangerous work")]
        public string Dangerous_Work { get; set; }
        [Display(Name = "Comments")]
        public string Comments { get; set; }
        [Display(Name = "Work Instructed?")]
        public bool Work_Instructed { get; set; }
        [Display(Name = "Safety Instructed?")]
        public bool Safety_Instructed { get; set; }
        [Display(Name = "Dangerous Work Instructed?")]
        public bool Dangerous_Work_Instructed { get; set; }
        [Display(Name = "Participants")]
        public IList<ToolBoxUser> users { get; set; }
        public ToolBox()
        {

        }
        public ToolBox(ToolBoxVM model)
        {
            this.DoneBy = model.DoneBy;
            this.Report_Date = model.Report_Date;
            this.Work_Tasks = model.Work_Tasks;
            this.Safety_Aspects = model.Safety_Aspects;
            this.Dangerous_Work = model.Dangerous_Work;
            this.Work_Instructed = model.Work_Instructed;
            this.Safety_Instructed = model.Safety_Instructed;
            this.Dangerous_Work_Instructed = model.Dangerous_Work_Instructed;
            this.Comments = model.Comments;
            this.ProjectId = model.ProjectId;
        }
    }
    public class ToolBoxUser
    {
        [Key]
        public int Id { get; set; }
        public string name { get; set; }
        public string signature { get; set; }
        [ForeignKey("ToolBox")]
        public int ToolBoxId { get; set; }
    }
}
