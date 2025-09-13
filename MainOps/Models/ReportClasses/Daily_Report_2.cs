using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ReportClasses
{
    public class Daily_Report_2
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Title")]
        [Display(Name = "Title")]
        public int TitleId { get; set; }
        public Title? Title { get; set; }
        [ForeignKey("Project")]
        [Display(Name = "Project")]
        [Required]
        public int ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        [Display(Name = "Key Words")]
        public string? short_Description { get; set; }
        [Display(Name = "Paid worK?")]
        public int? tobepaid { get; set; }
        public DateTime? EnteredIntoDataBase { get; set; }
        public DateTime? LastEditedInDataBase { get; set; }
        public bool HasPhotos { get; set; }
        public string? Report_name
        {
            get
            {
                return "Daily Report";
            }
        }
        [Display(Name = "Report Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/M/yy}")]
        public DateTime Report_Date { get; set; }
        [Display(Name = "Start Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan StartHour { get; set; }
        [Display(Name = "End Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan EndHour { get; set; }
        [Display(Name = "Work Performed")]
        [Required]
        [StringLength(1500, MinimumLength = 1, ErrorMessage = "Please write something")]
        public string? Work_Performed { get; set; }
        [Display(Name = "Extra Works")]
        public string? Extra_Works { get; set; }
        public string? DoneBy { get; set; }
        [NotMapped]
        public IList<string>? pictures { get; set; }
        public string? Signature { get; set; }
        [Display(Name = "Amount of People")]
        [DefaultValue(1)]
        public int Amount { get; set; }
        [Display(Name = "Machinery List")]
        [DefaultValue("None")]
        public string? Machinery { get; set; }
        [Display(Name = "Downtime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan? StandingTime { get; set; }
        [Display(Name = "Hours for safety setup")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan? SafetyHours { get; set; }
        [Display(Name = "Is Report Checked")]
        public bool Report_Checked { get; set; }
        [Display(Name = "Report checked by")]
        public string? Checked_By { get; set; }
        [Display(Name = "Sub Project")]
        public int? SubProjectId { get; set; }
        public virtual SubProject? SubProject { get; set; }
        [Display(Name = "Other people in Daily Report")]
        public string? OtherPeople { get; set; }
        [Display(Name = "Other people IDs in Daily Report")]
        public string? OtherPeopleIDs { get; set; }
        public TimeSpan Hours
        {
            get
            {
                return EndHour - StartHour;
            }
        }
        [Display(Name = "Variation Order")]
        public int? VariationOrderId { get; set; }
        [Display(Name = "Variation Order")]
        public virtual BoQHeadLine? VariationOrder { get; set; }
        public DateTime? InvoiceDate { get; set; }

        public Daily_Report_2() { }

    }
    public class Daily_Report_2_temp
    {
        [Key]
        public int Id { get; set; }
        public int? TitleId { get; set; }
        public int? ProjectId { get; set; }
        public int? VariationOrderId { get; set; }
        public string? short_Description { get; set; }
        public int? tobepaid { get; set; }
        public DateTime? Report_Date { get; set; }        
        public TimeSpan StartHour { get; set; }
        public TimeSpan EndHour { get; set; }
        public string? Work_Performed { get; set; }
        public string? Extra_Works { get; set; }
        public string? DoneBy { get; set; }
        [DefaultValue(1)]
        public int? Amount { get; set; }
        [DefaultValue("None")]
        public string? Machinery { get; set; }
        public TimeSpan? StandingTime { get; set; }
        public TimeSpan? SafetyHours { get; set; }
        public int? SubProjectId { get; set; }
        public string? OtherPeople { get; set; }
        public string? OtherPeopleIDs { get; set; }
        public DateTime? InvoiceDate { get; set; }
    }
    public class Daily_Report_2Backup
    {
        [Key]
        public int Id { get; set; }
        public int DrId { get; set; }
        [ForeignKey("Title")]
        [Display(Name = "Title")]
        public int TitleId { get; set; }
        public Title? Title { get; set; }
        [ForeignKey("Project")]
        [Display(Name = "Project")]
        [Required]
        public int ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        [Display(Name = "Key Words")]
        public string? short_Description { get; set; }
        [Display(Name = "Paid worK?")]
        public int? tobepaid { get; set; }
        public DateTime? EnteredIntoDataBase { get; set; }
        public DateTime? LastEditedInDataBase { get; set; }
        public bool HasPhotos { get; set; }
        public string? Report_name
        {
            get
            {
                return "Daily Report";
            }
        }
        [Display(Name = "Report Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/M/yy}")]
        public DateTime Report_Date { get; set; }
        [Display(Name = "Start Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan StartHour { get; set; }
        [Display(Name = "End Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan EndHour { get; set; }
        [Display(Name = "Work Performed")]
        [Required]
        [StringLength(1500, MinimumLength = 1, ErrorMessage = "Please write something")]
        public string? Work_Performed { get; set; }
        [Display(Name = "Extra Works")]
        public string? Extra_Works { get; set; }
        public string? DoneBy { get; set; }
        [NotMapped]
        public IList<string>? pictures { get; set; }
        public string? Signature { get; set; }
        [Display(Name = "Amount of People")]
        [DefaultValue(1)]
        public int Amount { get; set; }
        [Display(Name = "Machinery List")]
        [DefaultValue("None")]
        public string? Machinery { get; set; }
        [Display(Name = "Downtime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan? StandingTime { get; set; }
        [Display(Name = "Hours for safety setup")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan? SafetyHours { get; set; }
        [Display(Name = "Is Report Checked")]
        public bool Report_Checked { get; set; }
        [Display(Name = "Report checked by")]
        public string? Checked_By { get; set; }
        [Display(Name = "Sub Project")]
        public int? SubProjectId { get; set; }
        public virtual SubProject? SubProject { get; set; }
        [Display(Name = "Other people in Daily Report")]
        public string? OtherPeople { get; set; }
        [Display(Name = "Other people IDs in Daily Report")]
        public string? OtherPeopleIDs { get; set; }
        public TimeSpan Hours
        {
            get
            {
                return EndHour - StartHour;
            }
        }
        [Display(Name = "Variation Order")]
        public int? VariationOrderId { get; set; }
        [Display(Name = "Variation Order")]
        public virtual BoQHeadLine? VariationOrder { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public Daily_Report_2Backup()
        {
            
        }
        public Daily_Report_2Backup(Daily_Report_2 dr)
        {
            this.SafetyHours = dr.SafetyHours;
            this.ProjectId = dr.ProjectId;
            this.OtherPeople = dr.OtherPeople;
            this.OtherPeopleIDs = dr.OtherPeopleIDs;
            this.InvoiceDate = dr.InvoiceDate;
            this.SubProjectId = dr.SubProjectId;
            this.DrId = dr.Id;
            this.HasPhotos = dr.HasPhotos;
            this.Extra_Works = dr.Extra_Works;
            this.EndHour = dr.EndHour;
            this.StandingTime = dr.StandingTime;
            this.StartHour = dr.StartHour;
            this.short_Description = dr.short_Description;
            this.Report_Date = dr.Report_Date;
            this.Report_Checked = dr.Report_Checked;
            this.tobepaid = dr.tobepaid;
            this.TitleId = dr.TitleId;
            this.VariationOrderId = dr.VariationOrderId;
            this.Work_Performed = dr.Work_Performed;
            this.Signature = dr.Signature;
            this.DoneBy = dr.DoneBy;
            this.Checked_By = dr.Checked_By;
            this.Amount = dr.Amount;
            this.EnteredIntoDataBase = dr.EnteredIntoDataBase;
            this.LastEditedInDataBase = dr.LastEditedInDataBase;
            this.Machinery = dr.Machinery;
        }
    }
}
