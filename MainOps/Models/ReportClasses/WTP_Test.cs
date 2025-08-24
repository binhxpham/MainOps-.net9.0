using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ReportClasses
{
    public class WTP_Test
    {
        [Key]
        public int Id { get; set; }
        public string Test_name
        {
            get
            {
                return "Inspection WTP";
            }
        }
        public string DocId
        {
            get
            {
                return "CT-CL-22 Dewatering Checklist";
            }
        }
        public string VersionNr
        {
            get
            {
                return "0.0a";
            }
        }
        [Display(Name = "Equipment Name")]
        public string EquipmentName
        {
            get
            {
                return "Water Treatment Plant";
            }
        }
        [Display(Name = "Project")]
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        [Display(Name = "Visual check of leakage, function and cleanliness of WTP")]
        public bool Visual_Check { get; set; }
        [Display(Name = "Comment")]
        public string Visual_Comment { get; set; }
        [Display(Name = "Visual and organoleptic check of raw and clean water")]
        public bool Organoleptic_Check { get; set; }
        [Display(Name = "Comment")]
        public string Organoleptic_Comment { get; set; }
        [Display(Name = "Check and regulate the operation mode of filters")]
        public bool Filters_Check { get; set; }
        [Display(Name = "Comment")]
        public string Filters_Comment { get; set; }
        [Display(Name = "Check of manifolds")]
        public bool Manifolds_Check { get; set; }
        [Display(Name = "Comment")]
        public string Manifolds_Comment { get; set; }
        [Display(Name = "Check of the functionality of valves/ non-return valves/ air-release valves")]
        public bool Valves_Check { get; set; }
        [Display(Name = "Comment")]
        public string Valves_Comment { get; set; }
        [Display(Name = "Check of the water levels in storage tanks, sludge container")]
        public bool Water_Levels_Check { get; set; }
        [Display(Name = "Comment")]
        public string Water_Levels_Comment { get; set; }
        [Display(Name = "Check of control cabinet")]
        public bool Control_Cabinet_Check { get; set; }
        [Display(Name = "Comment")]
        public string Control_Cabinet_Comment { get; set; }
        [Display(Name = "Check and documentation of power consumption")]
        public bool Power_Check { get; set; }
        [Display(Name = "Comment")]
        public string Power_Comment { get; set; }
        [Display(Name = "Check of alams archive")]
        public bool Alarm_Check { get; set; }
        [Display(Name = "Comment")]
        public string Alarm_Comment { get; set; }
        [Display(Name = "Coordination, dispatching and handling of: change of filter material and disposal,	waste disposal, slurry transfer for disposal, spare parts")]
        public string Coordination_Description { get; set; }       
        [Display(Name = "Calibration of measurement equipment")]
        public bool Calibration_Check { get; set; }
        [Display(Name = "Comment")]
        public string Calibration_Comment { get; set; }
        [Display(Name = "Monthly check of electrical FI- switch")]
        public bool Electrical_Check { get; set; }
        [Display(Name = "Comment")]
        public string Electrical_Comment { get; set; }
        [Display(Name = "General cleaning of the plant and worksite")]
        public bool Cleaing_Check { get; set; }
        [Display(Name = "Comment")]
        public string Cleaing_Comment { get; set; }
        [Display(Name = "Water sampling and analyzing (self control)")]
        public bool Sample_Check { get; set; }
        [Display(Name = "Comment")]
        public string Sample_Comment { get; set; }
        [Display(Name = "Manganese Inlet")]
        public double? Manganese_Inlet { get; set; }
        [Display(Name = "Manganese Outlet")]
        public double? Manganese_outlet { get; set; }
        [Display(Name = "pH Inlet")]
        public double? pH_Inlet { get; set; }
        [Display(Name = "pH Outlet")]
        public double? pH_outlet { get; set; }
        [Display(Name = "Level mud Silbuster 1")]
        public double? S1_Check { get; set; }
        [Display(Name = "Mud Pumped from Silterbuster 1?")]
        public bool S1_Check_1 { get; set; }
        [Display(Name = "Comment Siltbuster 1")]
        public string S1_Comment { get; set; }
        [Display(Name = "Level mud Silbuster 2")]
        public double? S2_Check { get; set; }
        [Display(Name = "Mud Pumped from Silterbuster 1?")]
        public bool S2_Check_2 { get; set; }
        [Display(Name = "Comment Siltbuster 2")]
        public string S2_Comment { get; set; }
        [Display(Name = "Level mud Silbuster 3")]
        public double? S3_Check { get; set; }
        [Display(Name = "Mud Pumped from Silterbuster 1?")]
        public bool S3_Check_3 { get; set; }
        [Display(Name = "Comment Siltbuster 3")]
        public string S3_Comment { get; set; }
        [Display(Name = "Back washing of filter 1")]
        public bool F1_Check { get; set; }        
        [Display(Name = "Back washing of filter 2")]
        public bool F2_Check { get; set; }
        [Display(Name = "Back washing of filter 3")]
        public bool F3_Check { get; set; }
        [Display(Name = "Done/OK")]
        public bool F_Check { get; set; }
        [Display(Name = "Comment")]
        public string F_Comment { get; set; }
        [Display(Name = "Activated Green sand filter 1")]
        public bool AG1_Check { get; set; }
        [Display(Name = "Activated Green sand filter 2")]
        public bool AG2_Check { get; set; }
        [Display(Name = "Activated Green sand filter 3")]
        public bool AG3_Check { get; set; }
        [Display(Name = "Done/OK")]
        public bool AG_Check { get; set; }
        [Display(Name = "Comment")]
        public string AG_Comment { get; set; }
        [Display(Name = "Exchange of depleted filter material (sand/gravel/activated carbon/ resin)")]
        public bool Exchange_Check { get; set; }
        [Display(Name = "Comment")]
        public string Exchange_Comment { get; set; }
        [Display(Name = "Sand filter")]
        public bool SandFilter_Check { get; set; }
        [Display(Name = "Quantity [m3]")]
        public double SandFilter_Amount {get; set;}
        [Display(Name = "Comment")]
        public string SandFilter_Comment { get; set; }
        [Display(Name = "Activated Carbon Filter")]
        public bool GACFilter_Check { get; set; }
        [Display(Name = "Quantity [m3]")]
        public double GACFilter_Amount { get; set; }
        [Display(Name = "Comment")]
        public string GACFilter_Comment { get; set; }
        [Display(Name = "Executed Works")]
        public string Executed_Works { get; set; }
        [Display(Name = "Unusual Event")]
        public string Unusual_Event { get; set; }
        [Display(Name = "Date")]
        public DateTime Date_Done { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
    }
}
