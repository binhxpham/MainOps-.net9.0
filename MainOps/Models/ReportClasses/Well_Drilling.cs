using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ReportClasses
{
    public class Well_Drilling
    {
        [Key]
        public int Id { get; set; }
        public string Test_name
        {
            get
            {
                return "Well Drilling Record";
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
        [Display(Name = "Filter scetch")]
        public string Scetch_path { get; set; }
        [Display(Name = "Resting Water Level")]
        public double? resting_water_level { get; set; }
        public double? resting_water_level_actual { get; set; }
        public bool resting_water_level_pass { get; set; }
        [Display(Name = "Depth to Base of Well")]
        public double? Base_depth { get; set; }
        public double? Base_depth_actual { get; set; }
        public bool Base_depth_pass { get; set; }
        [Display(Name = "Drilling Method")]
        public string Drill_Method { get; set; }
        public string Drill_Method_actual { get; set; }
        public bool Drill_Method_pass { get; set; }
        [Display(Name = "Drill Casing Type")]
        public string Drill_Casing{ get; set; }
        [Display(Name = "Casing size(inch)")]
        public double? Drill_Casing_size { get; set; }
        [Display(Name = "Drill Fluids")]
        public string Drill_Fluid { get; set; }
        [Display(Name = "Etc. / More info")]
        public string Drill_Info { get; set; }
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
        [MaxLength(1024)]
        [Display(Name = "Description of ground encountered when drilling")]
        public string Ground_Notes { get; set; }
        [MaxLength(1024)]
        [Display(Name = "Depths where change of ground is observed")]
        public string Change_Notes { get; set; }
        [MaxLength(1024)]
        [Display(Name = "Each time groundwater is encountered, when possible instruct the drillers to leave the water level to stabilize, record the level it stabilizes at, then instruct the drillers to continue with work")]
        public string WL_Notes { get; set; }
        [MaxLength(1024)]
        [Display(Name = "Water levels at start of each shift")]
        public string WL_Start_Notes { get; set; }
        [MaxLength(1024)]
        [Display(Name = "Water levels at end of each shift")]
        public string WL_End_Notes { get; set; }
        [Display(Name = "Number of filter sand bags should be used")]
        public int? Filter_Sand_Bags { get; set; }
        [Display(Name = "Number of filter sand bags used")]
        public int? Filter_Sand_Bags_actual { get; set; }
        [Display(Name = "Filter Pack Length")]
        public double? Filter_Pack { get; set; }
        [Display(Name = "Filter pack installed from (m below ground)")]
        public double? Filter_Pack_Start { get; set; }
        [Display(Name = "Filter pack installed to (m below ground)")]
        public double? Filter_Pack_End { get; set; }
        [Display(Name = "Filter pack fits design?")]
        public bool Filter_Pack_pass { get; set; }
        [Display(Name = "Number of bentonite pellet bags should be used")]
        public int? Bentonite_Pellet_Bags { get; set; }
        [Display(Name = "Number of bentonite pellet bags used")]
        public int? Bentonite_Pellet_Bags_actual { get; set; }
        [Display(Name = "Casing Length")]
        public double? Casing_length { get; set; }
        [Display(Name = "Casing length used")]
        public double? Casing_length_actual { get; set; }
        [Display(Name = "Fits design?")]
        public bool Casing_length_pass { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"(([0-9][0-9])(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Standing Time must be between 00:00 to 99:59")]
        public TimeSpan Standing_Time { get; set; }
        [MaxLength(1024)]
        [Display(Name = "Standing time reason(s)")]
        public string Standing_Time_Notes { get; set; }
        [MaxLength(1024)]
        [Display(Name = "Notes")]
        public string Notes { get; set; }
        [Display(Name = "Done By")]
        public string Name { get; set; }
        [Display(Name="Date Installation")]
        public DateTime Date_Done { get; set; }
    }
}
