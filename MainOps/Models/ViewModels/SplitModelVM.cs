using MainOps.Models.ReportClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ViewModels
{
    public class SplitModelVM
    {
        public Daily_Report_2 DRold { get; set; }
        public int DRId { get; set; }
        [Display(Name = "Project 1")]
        public int ProjectId1 { get; set; }
        [Display(Name = "SubProject 1")]
        public int? SubProjectId1 { get; set; }
        [Display(Name = "Project 2")]
        public int ProjectId2 { get; set; }
        [Display(Name = "SubProject 2")]
        public int? SubProjectId2 { get; set; }
        [Display(Name = "Keywords 1")]
        public string ShortDescription1 { get; set; }
        [Display(Name = "Keywords 2")]
        public string ShortDescription2 { get; set; }
        [Display(Name = "Title 1")]
        public int TitleId1 { get; set; }
        [Display(Name = "Title 2")]
        public int TitleId2 { get; set; }
        [Display(Name = "Start Time 1")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan HourStart1 { get; set; }
        [Display(Name = "End Time 1")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan HourEnd1 { get; set; }
        [Display(Name = "DownTime 1")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan DownTime1 { get; set; }
        [Display(Name = "Safety Hours 1")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan HoursSafetySetup1 { get; set; }
        [Display(Name = "Report 1 Checked?")]
        public bool Checked1 { get; set; }
        [Display(Name = "Report 1 To be paid?")]
        public int? MakePaid1 { get; set; }
        [Display(Name = "Report 1 Variation Order")]
        public int? VariationOrderId1 { get; set; }
        [Display(Name = "Report 1 Text")]
        public string report_text1 { get; set; }
        [Display(Name = "Start Time 2")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan HourStart2 { get; set; }
        [Display(Name = "End Time 2")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan HourEnd2 { get; set; }
        [Display(Name = "DownTime 2")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan DownTime2 { get; set; }
        [Display(Name = "Safety Hours 2")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan HoursSafetySetup2 { get; set; }
        [Display(Name = "Report 2 Checked?")]
        public bool Checked2{ get; set; }
        [Display(Name = "Report 2 To be paid?")]
        public int? MakePaid2 { get; set; }
        [Display(Name = "Report 2 Variation Order")]
        public int? VariationOrderId2 { get; set; }
        [Display(Name = "Report 2 Text")]
        public string report_text2 { get; set; }

        public SplitModelVM()
        {
            DRId = -1;
            ProjectId1 = -1;
            SubProjectId1 = -1;
            ProjectId2 = -1;
            SubProjectId2 = -1;
            DRold = new Daily_Report_2();
            HourStart1 = new TimeSpan(0);
            HourEnd1 = new TimeSpan(0);
            DownTime1 = new TimeSpan(0);
            HoursSafetySetup1 = new TimeSpan(0);
            Checked1 = false;
            MakePaid1 = 1;
            VariationOrderId1 = null;
            report_text1 = "";
            HourStart2 = new TimeSpan(0);
            HourEnd2 = new TimeSpan(0);
            DownTime2 = new TimeSpan(0);
            HoursSafetySetup2 = new TimeSpan(0);
            Checked2 = false;
            MakePaid2 = 1;
            VariationOrderId2 = null;
            report_text2 = "";
            ShortDescription1 = "";
            ShortDescription2 = "";
        }
        public SplitModelVM(Daily_Report_2 dr)
        {
            DRId = dr.Id;
            DRold = dr;
            ProjectId1 = dr.ProjectId;
            ProjectId2 = dr.ProjectId;
            ShortDescription1 = dr.short_Description;
            ShortDescription2 = dr.short_Description;
            SubProjectId1 = dr.SubProjectId;
            SubProjectId2 = dr.SubProjectId;
            TitleId1 = dr.TitleId;
            TitleId2 = dr.TitleId;
            HourStart1 = dr.StartHour;
            HourEnd1 = dr.EndHour;
            if(dr.StandingTime == null)
            {
                DownTime1 = new TimeSpan(0);
            }
            else
            {
                DownTime1 = (TimeSpan)dr.StandingTime;
            }
            if (dr.SafetyHours == null)
            {
                HoursSafetySetup1 = new TimeSpan(0);
            }
            else
            {
                HoursSafetySetup1 = (TimeSpan)dr.SafetyHours;
            }
            Checked1 = dr.Report_Checked;
            MakePaid1 = dr.tobepaid;
            VariationOrderId1 = dr.VariationOrderId;
            report_text1 = dr.Work_Performed;
            HourStart2 = new TimeSpan(0);
            HourEnd2 = new TimeSpan(0);
            DownTime2 = new TimeSpan(0);
            HoursSafetySetup2 = new TimeSpan(0);
            Checked2 = dr.Report_Checked;
            MakePaid2 = dr.tobepaid;
            VariationOrderId2 = dr.VariationOrderId;
            report_text2 = dr.Work_Performed;
        }
    }
    
}
