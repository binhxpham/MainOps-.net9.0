using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ReportClasses
{
    public class Pump_Commission
    {
        [Key]
        public int Id { get; set; }
        public string Test_name
        {
            get
            {
                return "Pump Commission Record";
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
        [Display(Name = "Water level when pump stops")]
        public double? WL_Pump_Stop { get; set; }
        public double? WL_Pump_Stop_actual { get; set; }
        public bool WL_Pump_Stop_pass { get; set; }       
        [Display(Name = "Amps when pump stops")]
        public double? Amps { get; set; }
        public double? Amps_actual { get; set; }
        public bool Amps_pass { get; set; }
        [Display(Name = "Water level when pump restarts")]
        public double? WL_Pump_Restart { get; set; }
        public double? WL_Pump_Restart_actual { get; set; }
        public bool WL_Pump_Restart_pass { get; set; }
        [Display(Name = "Time delay between pump stop/start")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan? Time_Delay { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan? Time_Delay_actual { get; set; }
        public bool Time_Delay_pass { get; set; }
        [Display(Name = "Amps when pump restarts")]
        public double? Amps_restart { get; set; }
        public double? Amps_restart_actual { get; set; }
        public bool Amps_restart_pass { get; set; }
        [Display(Name = "Achievable water level to maintain constant pumping (not start/stopping)")]
        public double? WL_stable { get; set; }
        public double? WL_stable_actual { get; set; }
        public bool WL_stable_pass { get; set; }
        [Display(Name = "Pressure reading(Before pump off/When maintaining constant pumping)")]
        public double? Pressure { get; set; }
        public double? Pressure_actual { get; set; }
        public bool Pressure_pass { get; set; }
        [Display(Name = "Colour/Consistency of discharge water(Connected to header)")]
        public string colour { get; set; }
        public string colour_actual { get; set; }
        public bool colour_pass { get; set; }
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
