using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class SensorsCheck
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
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Report Done By")]
        public string? DoneBy { get; set; }
        [Display(Name = "Signature")]
        public string? Signature { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime? EnteredIntoDataBase { get; set; }
        [Display(Name = "General Comments")]
        public string? GeneralComments { get; set; }
        public ICollection<PhotoFileSensorsCheck>? Photos { get; set; }
        [Display(Name = "Turbidity SCADA Inlet")]
        public double? Turbidity_SCADA_Inlet { get; set; }
        [Display(Name = "pH SCADA Inlet")]
        public double? PH_SCADA_Inlet { get; set; }
        [Display(Name = "Conductiviy SCADA Inlet")]
        public double? Conductivity_SCADA_Inlet { get; set; }
        [Display(Name = "Turbidity sensor cleaned at inlet?")]
        public bool Turbidity_Cleaned_Inlet { get; set; }
        [Display(Name = "pH sensor cleaned at inlet?")]
        public bool PH_Cleaned_Inlet { get; set; }
        [Display(Name = "Conductiviy sensor cleaned at inlet?")]
        public bool Conductivity_Cleaned_Inlet { get; set; }
        [Display(Name = "Turbidity SCADA Inlet after")]
        public double? Turbidity_SCADA_Inlet_After { get; set; }
        [Display(Name = "pH SCADA Inlet after")]
        public double? PH_SCADA_Inlet_After { get; set; }
        [Display(Name = "Conductiviy SCADA Inlet after")]
        public double? Conductivity_SCADA_Inlet_After { get; set; }
        [Display(Name = "pH sensor calibrated at ínlet?")]
        public bool PH_Calibrated_Inlet { get; set; }
        [Display(Name = "Did you close bypass valve at inlet?")]
        public bool Closed_Bypass_Valve_Inlet { get; set; }
        [Display(Name = "Turbidity SCADA Outlet")]
        public double? Turbidity_SCADA_Outlet { get; set; }
        [Display(Name = "pH SCADA Outlet")]
        public double? PH_SCADA_Outlet { get; set; }
        [Display(Name = "Conductivity SCADA Outlet")]
        public double? Conductivity_SCADA_Outlet { get; set; }
        [Display(Name = "Turbidity sensor cleaned at outlet?")]
        public bool Turbidity_Cleaned_Outlet { get; set; }
        [Display(Name = "pH sensor cleaned at outlet?")]
        public bool PH_Cleaned_Outlet { get; set; }
        [Display(Name = "Conductivity sensor cleaned at outlet?")]
        public bool Conductivity_Cleaned_Outlet { get; set; }
        [Display(Name = "Turbidity SCADA Outlet after")]
        public double? Turbidity_SCADA_Outlet_After { get; set; }
        [Display(Name = "pH SCADA Outlet after")]
        public double? PH_SCADA_Outlet_After { get; set; }
        [Display(Name = "Conductivity SCADA Outlet after")]
        public double? Conductivity_SCADA_Outlet_After { get; set; }
        [Display(Name = "pH sensor calibrated at outlet?")]
        public bool PH_Calibrated_Outlet { get; set; }
        [Display(Name = "Did you close bypass valve at outlet?")]
        public bool Closed_Bypass_Valve_Outlet { get; set; }
    }
}
