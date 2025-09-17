using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace MainOps.Models
{
    public class WatercareCheck
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
        [Display(Name = "Timestamp")]
        public DateTime TimeStamp { get; set; }

        [Display(Name = "1. Check Watercare for Leakages (inside + outside)")] //1.
        public string? Leakages { get; set; }
        [Display(Name = "Comments")]
        public string? Leakages_Comment { get; set; }
                
        [Display(Name = "2. Check water level in tanks")] //2.
        public string? Tanks { get; set; }
        [Display(Name = "Comments")]
        public string? Tanks_Comment { get; set; }
                
        [Display(Name = "3. All filters is OK and not overflowing")] //3.
        public bool Filters_Operation { get; set; }
        [Display(Name = "Comments")]
        public string? Filters_Operation_Comment { get; set; }


        [Display(Name = "4. Water level in filter is OK?")] //4.
        public bool Filter_WaterLevel { get; set; }
        [Display(Name = "Comments")]
        public string? Filter_WaterLevel_Comment { get; set; }

        [Display(Name = "5. All valves in correct position?")] //5.
        public bool Valves { get; set; }
        [Display(Name = "Comments")]
        public string? Valves_Comment { get; set; }
                
        [Display(Name = "6. Is water clear after filtration?")] //6.
        public bool Water_Clear { get; set; }
        [Display(Name = "Comments")]
        public string? Water_Clear_Comment { get; set; }

        [Display(Name = "7. Do flowmeters work?")]//7.
        public bool FlowMeters { get; set; }
        [Display(Name = "Comments")]
        public string? FlowMeters_Comment { get; set; }

        [Display(Name = "8. Generator test done? (Remember Generator report)")]//8.
        public bool Generator_Test { get; set; }
        [Display(Name = "Comments")]
        public string? Generator_Test_Comment { get; set; }
        
        [Display(Name = "Check Performed By")]
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        [Display(Name = "Done by")]
        public string? DoneBy { get; set; }
        [Display(Name = "Signature")]
        public string? Signature { get; set; }
        public DateTime? EnteredIntoDataBase { get; set; }
        [Display(Name = "General Comments")]
        public string? GeneralComments { get; set; }
        public ICollection<PhotoFileWtcCheck>? Photos { get; set; }
    }
}
