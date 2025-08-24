using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ReportClasses
{
    public class Daily_Report
    {
        [Key]
        public int Id { get; set; }
        [Display(Name ="Location")]
        public string short_Description { get; set; }
        [Display(Name = "Well Location")]
        [ForeignKey("MeasPoint")]
        public int? MeasPointId { get; set; }
        public virtual MeasPoint MeasPoint { get; set; }
        public string Report_name
        {
            get
            {
                return "Daily Report";
            }
        }
        [Display(Name = "Report Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/M/yy}")]
        public DateTime Report_Date { get; set; }
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        [Display(Name = "Start Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan StartHour { get; set; }
        [Display(Name = "End Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan EndHour { get; set; }
        [Display(Name = "Machinery")]
        public string Machinery { get; set; }
        [Display(Name = "Work Performed")]
        public string Work_Performed { get; set; }
        [Display(Name = "Extra Works")]
        public string Extra_Works { get; set; }
        [Display(Name = "Was Alarm Call?")]
        public bool WasAlarmCall { get; set; }
        public string DoneBy { get; set; }
        [NotMapped]
        public IList<string> pictures { get; set; }
        public TimeSpan Hours
        {
            get
            {
                return EndHour - StartHour;
            }
        }
    }
}
