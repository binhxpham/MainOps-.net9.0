using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class SedimentationSiteReport : BaseReportInfo
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Plant Name")]
        [Required]
        [StringLength(1500, MinimumLength = 1, ErrorMessage = "Please Name the Plant")]
        public string PlantId { get; set; }
        [Display(Name = "Sedimenation emptied and cleaned?")]
        public bool SedimenationCleanedAndEmptied { get; set; }
        [Display(Name = "Sedimenation should be emptied and cleaned?")]
        public bool SedimentationShouldBeEmptiedAndCleaned { get; set; }
        [Display(Name = "Sedimentation Container Outlet compartment has minimum 20cm of free flowing water?")]
        public bool SedimentationMinimumWater { get; set; }
        [Display(Name = "Sedimentation Tank water does not show visual signs of running too quickly without cleaning prior to outlet")]
        public bool SedimenationFlowRate { get; set; }
        [Display(Name  ="Is Oil separator clogged? Does the water flow over the top instead of through the filter?")]
        public bool OilSeperatorClogged { get; set; }
        [Display(Name = "GeoTube exchanged?")]
        public bool GeoTubeExchanged { get; set; }
        [Display(Name = "GeoTube should be exchanged soon?")]
        public bool GeoTubeShouldBeExchanged { get; set; }
        [Display(Name = "Site Check?")]
        public bool SiteCheckPerformed { get; set; }
        [Display(Name = "Any Leakages?")]
        public bool Leakages { get; set; }
        [Display(Name = "Access Ways ok?")]
        public bool AccessWays { get; set; }
        [Display(Name = "Safety Issues?")]
        public bool Safety { get; set; }
        [Display(Name = "Acid Container exchanged?")]
        public bool AcidExchanged { get; set; }
        [Display(Name = "Acid Container should be exchanged soon?")]
        public bool AcidShouldBeExchanged { get; set; }
        [Display(Name = "Alarm System Functional?")]
        public bool AlarmFunction { get; set; }
        public ICollection<SedimentationSiteReportPhoto> Photos { get; set; }

    }
    public class SedimentationSiteReportPhoto
    {
        public int Id { get; set; }
        public int SedimentationSiteReportId { get; set; }
        public virtual SedimentationSiteReport SedimentationSiteReport { get; set; }
        public string Path { get; set; }
    }
}
