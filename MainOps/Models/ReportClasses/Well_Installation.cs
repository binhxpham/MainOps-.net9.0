using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ReportClasses
{
    public class Well_Installation
    {
        [Key]
        public int Id { get; set; }
        public string Test_name
        {
            get
            {
                return "Well Installation Record";
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
        [Display(Name = "X-Coordiante")]
        public double? x_coord { get; set; }
        public double? x_coord_actual { get; set; }       
        [Display(Name = "Y-Coordinate")]
        public double? y_coord { get; set; }
        public double? y_coord_actual { get; set; }
        [Display(Name = "Ground Level")]
        public double? z_coord { get; set; }
        public double? z_coord_actual { get; set; }
        public bool coord_pass { get; set; }
        [Display(Name = "Drilling Method")]
        public string Drill_Method { get; set; }
        public string Drill_Method_actual { get; set; }
        public bool Drill_Method_pass { get; set; }
        [MaxLength(512)]
        [Display(Name = "Notes")]
        public string Notes { get; set; }
        [Display(Name = "Done By")]
        public string Name { get; set; }
        [Display(Name="Date Installation")]
        public DateTime Date_Done { get; set; }
    }
}
