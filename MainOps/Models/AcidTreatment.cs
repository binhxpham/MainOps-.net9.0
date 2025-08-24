using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class AcidTreatment
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Project")]
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        [Display(Name = "Sub Project")]
        [ForeignKey("SubProject")]
        public int? SubProjectId { get; set; }
        public virtual SubProject? SubProject { get; set; }
        [Display(Name = "Well")]
        public string? Wellname { get; set; }
        [Display(Name = "Known Well")]
        [ForeignKey("MeasPoint")]
        public int? MeasPointId { get; set; }
        [Display(Name = "Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/M/yy}")]
        public DateTime Report_Date { get; set; }
        
        [Display(Name = "Water Meter Before")]
        public double? Water_Meter_Before { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        [Display(Name = "Start Time")]
        public TimeSpan starttime { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        [Display(Name = "End Time")]
        public TimeSpan endtime { get; set; }
        [Display(Name = "Water Meter After")]
        public double? Water_Meter_After { get; set; }
        [Display(Name = "Bottom Well")]
        public double? Bottom_well { get; set; }
        [Display(Name = "Reference Level")]
        public double? Ref_Level { get; set; }
        [Display(Name = "Water Level")]
        public double? Water_level { get; set; }
        public string? Comments { get; set; }
        [Display(Name = "Acid Total (Manual)")]
        public double? Total_m3_acid_Manual { get; set; }
        public ICollection<AcidData>? TestData { get; set; }
        public string? DoneBy { get; set; }
        public string? imagepath { get; set; }
        [Display(Name = "Total m3")]
        public double Totalm3
        {
            get
            {
                if (this.Water_Meter_Before != null && this.Water_Meter_After != null)
                {
                    return Convert.ToDouble(this.Water_Meter_After) - Convert.ToDouble(this.Water_Meter_Before) + this.AcidTotalm3;
                }
                else if (this.Water_Meter_After != null)
                {
                    return Convert.ToDouble(this.Water_Meter_After) + this.AcidTotalm3;
                }
                else
                {
                    return 0.0;
                }
            }
        }
        [Display(Name = "Total Acid m3")]
        public double AcidTotalm3
        {
            get
            {
                if(this.TestData != null)
                {

                
                    if(this.TestData.LastOrDefault() == null)
                    {
                        return 0.0;
                    }
                    else {
                        return (Convert.ToDouble(this.TestData.OrderBy(x => x.TimeStamp).LastOrDefault().Acid_m3_total) - Convert.ToDouble(this.TestData.OrderBy(x => x.TimeStamp).FirstOrDefault().Acid_m3_total));
                    }
                }
                else
                {
                    return 0.0;
                }
            }
        }
        public AcidTreatment()
        {

        }
    }
    public class AcidData
    {
        public int Id { get; set; }
        [Display(Name = "Time Stamp")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Pressure Well")]
        public double? Pressure { get; set; }
        public double? Min_Counter_Water { get; set; }
        public double? Min_Counter_HoseCounter { get; set; }
        public double? Hour_Counter_Water { get; set; }
        public double? Hour_Counter_HoseCounter { get; set; }
        [Display(Name = "Acid m3")]
        public double? Acid_m3_total { get; set; }
        [Display(Name = "Total m3")]
        public double? m3_total { get; set; }
        [Display(Name = "Dose Time")]
        public double? Set_Dosing_Time { get; set; }
        [Display(Name = "Dosing %")]
        public double? Set_Dosing_Percent { get; set; }
        [Display(Name = "Dosing m3")]
        public double? Set_Dosing_m3 { get; set; }
        public double? Level { get; set; }
        public double? Flow { get; set; }

        [ForeignKey("AcidTreatment")]
        public int AcidTreatmentId { get; set; }
    }
}
