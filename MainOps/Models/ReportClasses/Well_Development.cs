using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ReportClasses
{
    public class Well_Development
    {
        [Key]
        public int Id { get; set; }
        public string Test_name
        {
            get
            {
                return "Well Development Record";
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
        public double? resting_water_level_before { get; set; }
        public double? resting_water_level_after { get; set; }
        public bool resting_water_level_pass { get; set; }
        [Display(Name = "Depth to Base of Well")]
        public double? Base_depth_before { get; set; }
        public double? Base_depth_after { get; set; }
        public bool Base_depth_pass { get; set; }
        [Display(Name = "Colour/Consistency of discharge water")]
        public string Colour_before { get; set; }
        public string Colour_after { get; set; }
        public bool Colour_pass { get; set; }
        [Display(Name = "Well Clearing/Pumping clear")]
        public bool Well_clearing_before { get; set; }
        public bool Well_clearing_after { get; set; }
        public bool Well_clearing_pass { get; set; }
        [Display(Name = "IMHOFF / HACH Test results")]
        public string Test_result_before { get; set; }
        public string Test_result_after { get; set; }
        public bool Test_result_pass { get; set; }
        [Display(Name = "Duration of well development")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan Duration { get; set; }
        [Display(Name = "Method of Well development used")]
        public string method { get; set; }
        [MaxLength(512)]
        [Display(Name = "Notes")]
        public string Notes { get; set; }
        [Display(Name = "Done By")]
        public string Name { get; set; }
        [Display(Name="Date Installation")]
        public DateTime Date_Done { get; set; }
    }
}
