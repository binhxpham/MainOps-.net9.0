using MainOps.Models.ReportClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class SummaryReport
    {
        [Key]
        public int Id { get; set; }
        [NotMapped]
        public IList<Daily_Report_2>? Daily_Reports { get; set; }
        [NotMapped]
        public IList<Install>? Installations { get; set; }
        [NotMapped]
        public IList<Mobilize>? Mobilizations { get; set; }
        [NotMapped]
        public IList<Arrival>? Arrivals { get; set; }
        [NotMapped]
        public IList<DeInstall>? Deinstallations { get; set; }
        [NotMapped]
        public IList<Maintenance>? Maintenances { get; set; }
        [NotMapped]
        public IList<AlarmCall>? AlarmCalls { get; set; }
        [Display(Name = "Report Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Report_Date { get; set; }
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        [Display(Name = "Project")]
        public virtual Project? Project { get; set; }
        [Display(Name = "SubProject")]
        public int? SubProjectId { get; set; }
        [Display(Name = "SubProject")]
        public virtual SubProject? SubProject { get; set; }
        [Display(Name = "People")]
        public string? People { get; set; }
        [Display(Name = "Machinery")]
        [NotMapped]
        public List<string>? Machinery { get; set; }
        [Display(Name = "Signed HJ")]
        public bool IsSignedByHJ { get; set; }
        [Display(Name = "Signed Client")]
        public bool IsSignedByClient { get; set; }
        [Display(Name = "Signature HJ")]
        public string? signatureHJ { get; set; }
        [Display(Name = "Signature Client")]
        public string? signatureClient { get; set; }
        [Display(Name = "Date/Time of Signature HJ")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime SignatureHJDateTime { get; set; }
        [Display(Name = "Date/Time of Signature Client")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime SignatureClientDateTime { get; set; }
        [Display(Name = "Daily Summary")]
        public string? TheDailyText { get; set; }
        [Display(Name = "Has been sent to client")]
        public bool SentToClient { get; set; }
        [Display(Name = "Client Email")]
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? ClientEmail { get; set; }
        [NotMapped]
        public IList<GeneratorCheck>? GeneratorChecks { get; set; }
        [NotMapped]
        public IList<WTPCheck>? WTPChecks { get; set; }
        [NotMapped]
        public IList<SensorsCheck>? SensorsChecks { get; set; }
        [NotMapped]
        public IList<ConstructionSiteInspection>? ConstructionSiteChecks { get; set; }
        [NotMapped]
        public IList<ThreeStepTest>? ThreeStepTests { get; set; }
        [NotMapped]
        public IList<ClearPumpTest>? ClearPumpTests { get; set; }
        [NotMapped]
        public IList<Decommission>? Decommissions { get; set; }
        [NotMapped]
        public IList<SiteCheck>? SiteChecks { get; set; }
        [NotMapped]
        public bool ShowDailyReports { get; set; }
    }
    public class UserSummaryVM
    {
        public ApplicationUser? user { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [Display(Name = "Daily Summary")]
        public string? TheDailyText { get; set; }
        public string? LogText { get; set; }
        public IList<Daily_Report_2> Daily_Reports { get; set; }
        public IList<GeneratorCheck> GeneratorChecks { get; set; }
        public IList<WTPCheck> WTPChecks { get; set; }
        public IList<SensorsCheck> SensorsChecks { get; set; }
        public IList<ConstructionSiteInspection> ConstructionSiteChecks { get; set; }
        public IList<ThreeStepTest> ThreeStepTests { get; set; }
        public IList<ClearPumpTest> ClearPumpTests { get; set; }
        public IList<Decommission> Decommissions { get; set; }
        public IList<SiteCheck> SiteChecks { get; set; }
        public IList<Log2> Logs { get; set; }
        public UserSummaryVM()
        {
            Daily_Reports = new List<Daily_Report_2>();
            GeneratorChecks = new List<GeneratorCheck>();
            WTPChecks = new List<WTPCheck>();
            SensorsChecks = new List<SensorsCheck>();
            ConstructionSiteChecks = new List<ConstructionSiteInspection>();
            ThreeStepTests = new List<ThreeStepTest>();
            Decommissions = new List<Decommission>();
            SiteChecks = new List<SiteCheck>();
            ClearPumpTests = new List<ClearPumpTest>();
            Logs = new List<Log2>();
            StartDate = DateTime.Now.AddYears(-10);
            EndDate = DateTime.Now;
        }
        public UserSummaryVM(ApplicationUser _user, DateTime _start,DateTime _end)
        {
            user = _user;
            StartDate = _start;
            EndDate = _end;
        }
    }
    

}
