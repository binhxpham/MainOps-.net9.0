using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class SiteCheck
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        [Display(Name = "Project")]
        public virtual Project Project { get; set; }
        [Display(Name = "Sub Project")]
        public int? SubProjectId { get; set; }
        [Display(Name = "Sub Project")]
        public virtual SubProject SubProject { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Report Done By")]
        public string DoneBy { get; set; }
        [Display(Name = "Signature")]
        public string Signature { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        [Display(Name = "Entered into Database")]
        public DateTime? EnteredIntoDataBase { get; set; }
        [Display(Name = "General Comments")]
        public string GeneralComments { get; set; }
        public ICollection<PhotoFileSiteCheck> Photos { get; set; }
        [Display(Name = "Check All levels and flows in SCADA. Are they plausible?")]
        public string CheckAllLevels { get; set; }
        [Display(Name = "Comment")]
        public string CheckAllLevels_Comment { get; set; }
        [Display(Name = "SCADA: All pumps running that should run?")]
        public string SCADA_PUMPS_RUNNING { get; set; }
        [Display(Name = "Comment")]
        public string SCADA_PUMPS_RUNNING_Comment { get; set; }
        [Display(Name = "Any leakages in hoses/manifold/pipes?")]
        public string Leakages { get; set; }
        [Display(Name = "Comment")]
        public string Leakages_Comment { get; set; }
        [Display(Name = "Are all main valves in correct position?")]
        public string Valves { get; set; }
        [Display(Name = "Comment")]
        public string Valves_Comment { get; set; }
        [Display(Name = "Damages at wells")]
        public string WellDamages { get; set; }
        [Display(Name = "Comment")]
        public string WellDamages_Comment { get; set; }
        [Display(Name = "(winter)Heating + insulation ok ?")]
        public string Insulation { get; set; }
        [Display(Name = "Comment")]
        public string Insulation_Comment { get; set; }
        [Display(Name = "Are areas clear?")]
        public string AreasClear { get; set; }
        [Display(Name = "Comment")]
        public string AreasClear_Comment { get; set; }
    }
}
