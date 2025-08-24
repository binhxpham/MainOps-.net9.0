using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class ClearPumpTestData
    {
        public int Id { get; set; }
        [Display(Name = "Time Stamp")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan TimeStamp { get; set; }
        [Display(Name = "Flow")]
        public double? Flow { get; set; }
        [Display(Name = "m3")]
        public double? m3 { get; set; }
        [Display(Name = "Dip")]
        public double? Dip { get; set; }
        [Display(Name = "Comment")]
        public string Comment { get; set; }
        [ForeignKey("ClearPumpTest")]
        public int ClearPumpTestId { get; set; }
    }
    public class ClearPumpTestDataVM
    {
        [Display(Name = "Time Stamp")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Flow")]
        public double? Flow { get; set; }
        [Display(Name = "m3")]
        public double? m3 { get; set; }
        [Display(Name = "Dip")]
        public double? Dip { get; set; }
        [Display(Name = "Comment")]
        public string Comment { get; set; }
    }
    public class ClearPumpTest
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Project")]
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        [Display(Name = "Sub Project")]
        [ForeignKey("SubProject")]
        public int? SubProjectId { get; set; }
        public virtual SubProject SubProject { get; set; }
        [Display(Name = "Well")]
        public string Wellname { get; set; }
        [Display(Name = "Known Well")]
        [ForeignKey("MeasPoint")]
        public int? MeasPointId { get; set; }
        [Display(Name = "Filter diameter(mm)")]
        public double? Filter_Diameter { get; set; }
        [Display(Name = "Pump Type")]
        public string PumpType { get; set; }
        [Display(Name = "Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/M/yy}")]
        public DateTime Report_Date { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        [Display(Name = "Start Time")]
        public TimeSpan starttime { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        [Display(Name = "End Time")]
        public TimeSpan endtime { get; set; }
        [Display(Name = "Water Meter Before")]
        public double? Water_Meter_Before { get; set; }
        [Display(Name = "Water Meter After")]
        public double? Water_Meter_After { get; set; }
        [Display(Name = "Reference Point")]
        public string Ref_Point { get; set; }
        [Display(Name = "Reference Level")]
        public double? Ref_Level { get; set; }
        [Display(Name = "Bottom Well")]
        public double? Bottom_well { get; set; }
        [Display(Name = "Before or After Acid Treatment")]
        public string Before_After_Acid { get; set; }
        [Display(Name = "Water Level")]
        public double? Water_level { get; set; }
        [Display(Name = "Time Initial Dip")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan? Init_Meas_Time { get; set; }
        public string imagepath { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        [Display(Name = "Comments")]
        public string GeneralComments { get; set; }
        [Display(Name = "Done by")]
        public string DoneBy { get; set; }
        [Display(Name = "Measuremetns")]
        public ICollection<ClearPumpTestData> Measurements { get; set; }
        [Display(Name = "Discharge Point Available?")]
        public bool DischargeAvailable { get; set; }
        [Display(Name = "Total m3")]
        public double Totalm3
        {
            get
            {
                if (this.Water_Meter_Before != null && this.Water_Meter_After != null)
                {
                    return Convert.ToDouble(this.Water_Meter_After) - Convert.ToDouble(this.Water_Meter_Before);
                }
                else if (this.Water_Meter_After != null)
                {
                    return Convert.ToDouble(this.Water_Meter_After);
                }
                else
                {
                    return 0.0;
                }
            }
        }
        public ClearPumpTest()
        {

        }
    }
}
