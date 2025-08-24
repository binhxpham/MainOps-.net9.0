using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ViewModels
{
    public class ToolBoxVM
    {
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        [Display(Name = "Report Date")]
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
        public bool Work_Instructed { get; set; }
        public bool Safety_Instructed { get; set; }
        public bool Dangerous_Work_Instructed { get; set; }
        public ToolBoxVM()
        {

        }
    }
}
