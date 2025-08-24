using MainOps.Models.ReportClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MainOps.Models
{
    public class WaterHandling
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Well ID")]
        public int? MeasPointId { get; set; }
        public virtual MeasPoint MeasPoint { get; set; }
        [Display(Name = "Clear Pump ID")]
        public int? ClearPumpTestId { get; set; }
        public virtual ClearPumpTest ClearPumpTest { get; set; }
        [Display(Name = "Drill Water ID")]
        public int? DrillWaterId { get; set; }
        public virtual DrillWater DrillWater { get; set; }
        [Display(Name = "Well Name")]
        public string WellName { get; set; }
        [Display(Name = "Drill ID")]
        public int? WellId { get; set; }
        public virtual Well Well { get; set; }
        [Display(Name = "ClearPump m3 Start")]
        public double? PumpWaterStart { get; set; }
        [Display(Name = "ClearPump m3 End")]
        public double? PumpWaterEnd { get; set; }
        [Display(Name = "Drill m3 Start")]
        public double? DrillWaterStart { get; set; }
        [Display(Name = "Drill m3 End")]
        public double? DrillWaterEnd { get; set; }
        [Display(Name = "Amount of containers for water during drilling")]
        public int AmountContainerDrill { get; set; }
        [Display(Name = "Amount of containers for water during clear pumping")]
        public int AmountContainerPump { get; set; }
        [Display(Name = "ClearPump Start Date")]
        public DateTime PumpDateStart { get; set; }
        [Display(Name = "ClearPump End Date")]
        public DateTime PumpDateEnd { get; set; }
        [Display(Name = "Drill Start Date")]
        public DateTime DrillStart { get; set; }
        [Display(Name = "Drill End Date")]
        public DateTime DrillEnd { get; set; }
        [Display(Name = "Total Water")]

        public double? TotalWater { get
            {
                return (Convert.ToDouble(this.PumpWaterEnd) - Convert.ToDouble(this.PumpWaterStart)) + (Convert.ToDouble(this.DrillWaterEnd) - Convert.ToDouble(this.DrillWaterStart));
            } }
    }
    public class DrillWater
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        [Display(Name = "Project")]
        public virtual Project Project { get; set; }
        [Display(Name = "SubProject")]
        public int? SubProjectId { get; set; }
        [Display(Name = "SubProject")]
        public virtual SubProject SubProject { get; set; }
        [Display(Name = "Well")]
        public int? MeasPointId { get; set; }
        public virtual MeasPoint MeasPoint { get; set; }
        [Display(Name = "Start of Drilling m3")]
        public double? DrillWaterStart { get; set; }
        [Display(Name = "End of Drilling m3")]
        public double? DrillWaterEnd { get; set; }
        [Display(Name = "Amount of Water Containers")]
        public int? AmountContainers { get; set; }
        [Display(Name = "Photos of tanks/water levels/m3")]
        public ICollection<DrillWaterPhoto> Photos { get; set; }
    }
    public class DrillWaterPhoto
    {
        public int? Id { get; set; }
        public string path { get; set; }
        public int? DrillWaterId { get; set; }
        public virtual DrillWater DrillWater {get;set;}
        public DateTime TimeStamp { get; set; }
    }
    
}
