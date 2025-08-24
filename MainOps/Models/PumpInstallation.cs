using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class PumpInstallation
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        [Display(Name = "Project")]
        public virtual Project? Project { get; set; }
        [Display(Name = "SubProject")]
        public int? SubProjectId { get; set; }
        [Display(Name = "SubProject")]
        public virtual SubProject? SubProject { get; set; }
        [Display(Name = "Well Name (from list)")]
        public int? MeasPointId { get; set; }
        [Display(Name = "Well Name (from list)")]
        public virtual MeasPoint? MeasPoint { get; set; }
        [Display(Name = "Well Name (type in)")]
        public string? WellName { get; set; }
        [Display(Name = "Date and Time")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Pump Type")]
        public string? PumpTypeWritten { get; set; }
        [Display(Name = "Sensor Range")]
        public string? SensorRange { get; set; }
        [Display(Name = "Depth of Well [m]")]
        public double? WellDepth { get; set; }
        [Display(Name = "Depth of Pump [m]")]
        public double? PumpDepth { get; set; }
        [Display(Name = "Depth of Sensor [m]")]
        public double? SensorDepth { get; set; }
        [Display(Name = "Hose/Pipe diameter")]
        public string? DiameterHose { get; set; }
        [Display(Name = "Water level from top of pipe")]
        public double? WaterLevel { get; set; }
        [Display(Name = "If cut, how much (negative if extended) [m]")]
        public double? PipeCut { get; set; }
        public string? Comments { get; set; }
        [Display(Name = "Variation Order / Extra Work Header")]
        public int? VariationOrderId { get; set; }
        public virtual BoQHeadLine? VariationOrder { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Accuracy { get; set; }
        public int? PumpTypeId { get; set; }
        public virtual ItemType? PumpType { get; set; }


    }
    public class ReinfiltrationInstallation : BaseReportInfo
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Well Name (from list)")]
        public int? MeasPointId { get; set; }
        [Display(Name = "Well Name (from list)")]
        public virtual MeasPoint? MeasPoint { get; set; }
        [Display(Name = "Well Name (type in)")]
        public string? WellName { get; set; }
        [Display(Name = "Observation Type")]
        public string? ReinfiltrationTypeWritten { get; set; }
        [Display(Name = "Sensor Range")]
        public string? SensorRange { get; set; }
        [Display(Name = "Depth of Well [m]")]
        public double? WellDepth { get; set; }
        [Display(Name = "Depth of Sensor [m]")]
        public double? SensorDepth { get; set; }
        [Display(Name = "Hose/Pipe diameter")]
        public string? DiameterHose { get; set; }
        [Display(Name = "Water level from top of pipe")]
        public double? WaterLevel { get; set; }
        [Display(Name = "Variation Order / Extra Work Header")]
        public int? VariationOrderId { get; set; }
        public virtual BoQHeadLine? VariationOrder { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Accuracy { get; set; }
        public int? ReinfiltrationTypeId { get; set; }
        public virtual ItemType? ReinfiltrationType { get; set; }


    }
    public class ObservationInstallation : BaseReportInfo
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Well Name (from list)")]
        public int? MeasPointId { get; set; }
        [Display(Name = "Well Name (from list)")]
        public virtual MeasPoint? MeasPoint { get; set; }
        [Display(Name = "Well Name (type in)")]
        public string? WellName { get; set; }
        [Display(Name = "Reinfiltratoin Type")]
        public string? ObservationTypeWritten { get; set; }
        [Display(Name = "Sensor Range")]
        public string?  SensorRange { get; set; }
        [Display(Name = "Depth of Well [m]")]
        public double? WellDepth { get; set; }
        [Display(Name = "Depth of Sensor [m]")]
        public double? SensorDepth { get; set; }
        [Display(Name = "Water level from top of pipe")]
        public double? WaterLevel { get; set; }
        [Display(Name = "Variation Order / Extra Work Header")]
        public int? VariationOrderId { get; set; }
        public virtual BoQHeadLine? VariationOrder { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Accuracy { get; set; }
        [Display(Name = "If cut, how much (negative if extended) [m]")]
        public double? PipeCut { get; set; }
        public int? ObservationTypeId { get; set; }
        public virtual ItemType? ObservationType { get; set; }


    }
}
