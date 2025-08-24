using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class GeneratorCheck
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        [Display(Name = "Project")]
        public virtual Project? Project { get; set; }
        [Display(Name = "Sub Project")]
        public int? SubProjectId { get; set; }
        [Display(Name = "Sub Project")]
        public virtual SubProject? SubProject { get; set; }
        [Display(Name = "Generator Name/Location")]
        public string? GeneratorNameLocation { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Report Done By")]
        public string? DoneBy { get; set; }
        [Display(Name = "Signature")]
        public string? Signature { get; set; }
        [Display(Name = "Is AdBlue Level ok?")]
        public bool AdBlue_Level { get; set; }
        [Display(Name = "Comments")]
        public string? AdBlue_Level_Comment { get; set; }
        [Display(Name = "Is Oil level ok?")]
        public bool Oil_Level { get; set; }
        [Display(Name = "Comments")]
        public string? Oil_Level_Comment { get; set; }
        [Display(Name = "What is the diesel level?")]
        public double Diesel_Level { get; set; }
        [Display(Name = "Comments")]
        public string? Diesel_Level_Comment { get; set; }
        [Display(Name = "Is coolant ok?")]
        public bool Coolant { get; set; }
        [Display(Name = "Comments")]
        public string? Coolant_Comment { get; set; }
        [Display(Name = "Generator started without problems?")]
        public string? Generator_Started { get; set; }
        [Display(Name = "Comments")]
        public string? Generator_Started_Comment { get; set; }
        [Display(Name = "Pumps + WTP + Infiltration started during test?")]
        public string? Equipment_Started { get; set; }
        [Display(Name = "Comments")]
        public string? Equipment_Started_Comment { get; set; }
        [Display(Name = "Generator switched off without problems?")]
        public string? Generator_Stopped { get; set; }
        [Display(Name = "Comments")]
        public string? Generator_Stopped_Comment { get; set; }
        [Display(Name = "Pumps + WTP + Infiltratoin started after test?")]
        public string? Equipment_Started_After { get; set; }
        [Display(Name = "Comments")]
        public string? Equipment_Started_After_Comment { get; set; }
        [Display(Name = "Generator Hours (after test)")]
        public double Generator_Hours { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime? EnteredIntoDataBase { get; set; }
        [Display(Name = "General Comments")]
        public string? GeneralComments { get; set; }
        public ICollection<PhotoFileGeneratorCheck>? Photos { get; set; }
    }
}
