using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class BaseReportInfo
    {
        [Display(Name = "Project" )]
        public int ProjectId { get; set; }
        [Display(Name = "Project")]
        public virtual Project Project { get; set; }
        [Display(Name = "SubProject")]
        public int? SubProjectId { get; set; }
        [Display(Name = "SubProject")]
        public virtual SubProject SubProject { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Comments")]
        public string Comments { get; set; }
        [Display(Name = "Made By")]
        public string DoneBy { get; set; }
    }
}
