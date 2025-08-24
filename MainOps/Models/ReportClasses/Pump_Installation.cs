using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ReportClasses
{
    public class Pump_Installation
    {
        [Key]
        public int Id { get; set; }
        public string Test_name
        {
            get
            {
                return "Pump Installation Record";
            }
        }

        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        [Display(Name = "Project Location")]
        public string ProjectLocation { get; set; }
        [Display(Name = "Hoelscher Dewatering Project Nr")]
        public string HD_ProjectNr { get; set; }
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [Display(Name = "Well Name")]
        public string WellName { get; set; }
        [Display(Name = "Version Nr")]
        public string VersionNr { get; set; }
        [Display(Name = "Reason for Revision")]
        public string RevisionReason { get; set; }
        [Display(Name = "Scetch")]
        public string Scetch_path { get; set; }
        [Display(Name = "Resting Water Level")]
        public double? resting_water_level { get; set; }
        public double? resting_water_level_actual { get; set; }
        public bool resting_water_level_pass { get; set; }
        [Display(Name = "Depth to Base of Well")]
        public double? Base_depth { get; set; }
        public double? Base_depth_actual { get; set; }
        public bool Base_depth_pass { get; set; }       
        [Display(Name = "Sample tap/Control valve/Pressure Gauge fitted")]
        public string Fitted { get; set; }
        public string Fitted_actual { get; set; }
        public bool Fitted_pass { get; set; }
        [Display(Name = "Pump/Motor type")]
        public string Pump { get; set; }
        public string Pump_actual { get; set; }
        public bool Pump_pass { get; set; }
        [Display(Name = "Pump installation depth")]
        public double? install_depth { get; set; }
        public double? install_depth_actual { get; set; }
        public bool install_depth_pass { get; set; }
        [Display(Name = "Dip tube installed")]
        public string dip_tube { get; set; }
        public string dip_tube_actual { get; set; }
        public bool dip_tube_pass { get; set; }
        [Display(Name = "Check pump rotation")]
        public string rotation { get; set; }
        public string rotation_actual { get; set; }
        public bool rotation_pass { get; set; }
        [Display(Name = "Initial colour/Consistency of discharge water (Pump to ground/TOTE)")]
        public string colour { get; set; }
        public string colour_actual { get; set; }
        public bool colour_pass { get; set; }
        [Display(Name = "Well clearing/Pumping clear (Pump to ground/TOTE)")]
        public string clearing { get; set; }
        public string clearing_actual { get; set; }
        public bool clearing_pass { get; set; }
        [Display(Name = "IMHOFF / HACK tet results (Pump to ground/TOTE)")]
        public string imhoff { get; set; }
        public string imhoff_actual { get; set; }
        public bool imhoff_pass { get; set; }
        [MaxLength(512)]
        [Display(Name = "Notes")]
        public string Notes { get; set; }
        [Display(Name = "Done By")]
        public string Name { get; set; }
        [Display(Name="Date Installation")]
        public DateTime Date_Done { get; set; }
    }
}
