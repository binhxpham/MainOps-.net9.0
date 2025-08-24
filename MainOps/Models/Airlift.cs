using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class Airlift
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Project")]
        public int? ProjectId { get; set; }
        [Display(Name = "Project")]
        public virtual Project? Project { get; set; }
        [Display(Name = "SubProject")]
        public int? SubProjectId { get; set; }
        [Display(Name = "SubProject")]
        public virtual SubProject? SubProject { get; set; }
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Bottom of well before(m)")]
        public double? BottomWellBefore { get; set; }
        [Display(Name = "Water level before(m)")]
        public double? WaterLevelBefore { get; set; }
        [Display(Name = "Bottom of well after(m)")]
        public double? BottomWellAfter { get; set; }
        [Display(Name = "Water level after(m)")]
        public double? WaterLevelAfter { get; set; }
        [Display(Name = "Wellname")]
        public int? MeasPointId { get; set; }
        [Display(Name = "Wellname")]
        public virtual MeasPoint? MeasPoint { get; set; }
        public string? Comments { get; set; }
        [Display(Name = "Done By")]
        public string? DoneBy { get; set; }
        public ICollection<AirliftPhoto>? Photos { get; set; }
        [Display(Name = "Recovery Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan RecoveryTime { get; set; }


    }
    public class AirliftPhoto
    {
        [Key]
        public int Id { get; set; }
        public int AirliftId { get; set; }
        public virtual Airlift? AirLift { get; set; }
        public string? path { get; set; }
        public DateTime TimeStamp { get; set; }

    }
}
