using MainOps.Models.ReportClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ReportClasses
{
    public class AllDocumentation
    {
        public List<Daily_Report_2> Daily_Reports { get; set; }
        public List<Install> Installations { get; set; }
        public List<Mobilize> Mobilizations { get; set; }
        public List<ThreeStepTest> PumpTests { get; set; }
        public List<ClearPumpTest> ClearPumpTests { get; set; }
        public List<AcidTreatment> AcidTreatments { get; set; }

        public List<Arrival> Arrivals { get; set; }
        public List<DeInstall> Deinstallations { get; set; }
        public List<ExtraWork> ExtraWorks { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public int? SubProjectId { get; set; }
        public virtual SubProject SubProject { get; set; }
        public List<PhotoFileInstalled2> Install_Photos { get; set; }
        public List<PhotoFileMobilized> Mobilized_Photos { get; set; }
        public List<PhotoFileArrival> Arrival_Photos { get; set; }
        public List<PhotoFileDeinstalled> Deinstall_Photos { get; set; }
        public List<PumpTestPhoto> PumpTest_Photos { get; set; }
        public List<ClearPumpTestPhoto> ClearPumpTest_Photos { get; set; }
        public string logopath { get; set; }
        public AllDocumentation()
        {
            AcidTreatments = new List<AcidTreatment>();
            PumpTests = new List<ThreeStepTest>();
            ClearPumpTests = new List<ClearPumpTest>();
            Daily_Reports = new List<Daily_Report_2>();
            Installations = new List<Install>();
            Mobilizations = new List<Mobilize>();
            Arrivals = new List<Arrival>();
            Deinstallations = new List<DeInstall>();
            ExtraWorks = new List<ExtraWork>();
            Install_Photos = new List<PhotoFileInstalled2>();
            Mobilized_Photos = new List<PhotoFileMobilized>();
            Arrival_Photos = new List<PhotoFileArrival>();
            Deinstall_Photos = new List<PhotoFileDeinstalled>();
    }
        public AllDocumentation(List<Daily_Report_2> drs,List<Mobilize> mobs,List<Install> insts, List<Arrival> arrs, List<ExtraWork> ews)
        {
            this.Daily_Reports = drs;
            this.Mobilizations = mobs;
            this.Installations = insts;
            this.Arrivals = arrs;
            this.ExtraWorks = ews;
        }
        public AllDocumentation(AllDocumentation model,SubProject sb)
        {
            this.Daily_Reports = model.Daily_Reports.Where(x => x.SubProjectId.Equals(sb.Id)).ToList();
            try { 
            this.ClearPumpTests = model.ClearPumpTests.Where(x => x.SubProjectId.Equals(sb.Id)).ToList();
            }
            catch
            {
                this.ClearPumpTests = new List<ClearPumpTest>();
            }
            try {
                this.PumpTests = model.PumpTests.Where(x => x.SubProjectId.Equals(sb.Id)).ToList();
            }
            catch
            {
                this.PumpTests = new List<ThreeStepTest>();
                
            }
            try
            {
                this.AcidTreatments = model.AcidTreatments.Where(x => x.SubProjectId.Equals(sb.Id)).ToList();
            }
            catch
            {
                this.AcidTreatments = new List<AcidTreatment>();
            }
            
            this.Installations = model.Installations.Where(x => x.SubProjectId.Equals(sb.Id)).ToList();
            this.Mobilizations = model.Mobilizations.Where(x => x.SubProjectId.Equals(sb.Id)).ToList();
            this.Arrivals = model.Arrivals.Where(x => x.SubProjectId.Equals(sb.Id)).ToList();
            this.Deinstallations = model.Deinstallations.Where(x => x.SubProjectId.Equals(sb.Id)).ToList();
            this.ExtraWorks = model.ExtraWorks.Where(x => x.SubProjectId.Equals(sb.Id)).ToList();
            this.Install_Photos = model.Install_Photos;
            this.Mobilized_Photos = model.Mobilized_Photos;
            this.Arrival_Photos = model.Arrival_Photos;
            this.Deinstall_Photos = model.Deinstall_Photos;
            this.ProjectId = model.ProjectId;
            this.SubProjectId = sb.Id;
            this.starttime = model.starttime;
            this.endtime = model.endtime;
            this.Project = model.Project;
            this.SubProject = sb;
            
        }
    }
}
