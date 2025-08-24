using MainOps.Models.ReportClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ViewModels
{
    public class SiteDocumentationVM
    {
        public int DivisionId { get; set; }
        public string LogoPath { get; set; }
        public Project Project { get; set; }
        public SubProject SubProject { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IList<Daily_Report_2> DailyReports { get; set; }
        public IList<AlarmCall> AlarmCalls { get; set; }
        public IList<Maintenance> Maintenances { get; set; }
        public IList<GeneratorCheck> GeneratorChecks { get; set; }
        public IList<SiteCheck> SitesChecks { get; set; }
        public IList<ConstructionSiteInspection> ConstructionSiteInspections { get; set; }
        public IList<WTPCheck> WTPChecks { get; set; }
        public IList<SensorsCheck> SensorsCheck { get; set; } 
        public IList<Decommission> Decommissions { get; set; }
    }
}
