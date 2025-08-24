using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    //public class WTPCheck
    //{
    //    [Key]
    //    public int Id { get; set; }
    //    [Display(Name = "Project")]
    //    public int ProjectId { get; set; }
    //    [Display(Name = "Project")]
    //    public virtual Project Project { get; set; }
    //    [Display(Name = "Sub Project")]
    //    public int? SubProjectId { get; set; }
    //    [Display(Name = "Sub Project")]
    //    public virtual SubProject SubProject { get; set; }
    //    [Display(Name = "Timestamp")]
    //    public DateTime TimeStamp { get; set; }
    //    [Display(Name = "Check WTP for leakages (inside + outside)")]
    //    public string Leakages { get; set; }
    //    [Display(Name = "Comments")]
    //    public string Leakages_Comment { get; set; }
    //    [Display(Name = "Check pressure of filters and infiltration line")]
    //    public string Pressures { get; set; }
    //    [Display(Name = "Comments")]
    //    public string Preassures_Comment { get; set; }
    //    [Display(Name = "Check water level in tanks")]
    //    public string Tanks { get; set; }
    //    [Display(Name = "Comments")]
    //    public string Tanks_Comment { get; set; }
    //    [Display(Name = "Do we need a suction truck for slurry?")]
    //    public bool Suction_Tank { get; set; }
    //    [Display(Name = "Comments")]
    //    public string Suction_Tank_Comment { get; set; }
    //    [Display(Name = "Clear water pump above slurry?")]
    //    public bool Clear_Water_Pump { get; set; }
    //    [Display(Name = "Comments")]
    //    public string Clear_Water_Pump_Comment { get; set; }
    //    [Display(Name = "All filters in operation?")]
    //    public bool Filters_Operation { get; set; }
    //    [Display(Name = "Comments")]
    //    public string Filters_Operation_Comment { get; set; }
    //    [Display(Name = "All valves in correct position?")]
    //    public bool Valves { get; set; }
    //    [Display(Name = "Comments")]
    //    public string Valves_Comment { get; set; }
    //    [Display(Name = "Heating / Insulation in place?")]
    //    public bool Heating { get; set; }
    //    [Display(Name = "Comments")]
    //    public string Heating_Comment { get; set; }
    //    [Display(Name = "Is water clear after filtration?")]
    //    public bool Water_Clear { get; set; }
    //    [Display(Name = "Comments")]
    //    public string Water_Clear_Comment { get; set; }
    //    [Display(Name = "Do flowmeters work? and fit with SCADA?")]
    //    public bool FlowMeters { get; set; }
    //    [Display(Name = "Comments")]
    //    public string FlowMeters_Comment { get; set; }
    //    [Display(Name = "Check Performed By")]
    //    public double? Latitude { get; set; }
    //    public double? Longitude { get; set; }
    //    [Display(Name = "Done by")]
    //    public string DoneBy { get; set; }
    //    [Display(Name = "Signature")]
    //    public string Signature { get; set; }
    //    public DateTime? EnteredIntoDataBase { get; set; }
    //    [Display(Name = "General Comments")]
    //    public string GeneralComments { get; set; }
    //    public ICollection<PhotoFileWTPCheck> Photos { get; set; }
    //}

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class WTPCheck
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Project")]
        public int ProjectId { get; set; }

        [Display(Name = "Project")]
        public virtual Project? Project { get; set; }

        [Display(Name = "Sub Project")]
        public int? SubProjectId { get; set; }

        [Display(Name = "Sub Project")]
        public virtual SubProject? SubProject { get; set; }

        [Display(Name = "Timestamp")]
        public DateTime TimeStamp { get; set; }

        [Display(Name = "Check WTP for leakages (inside + outside)")]
        public string? Leakages { get; set; }

        [Display(Name = "Comments")]
        public string? Leakages_Comment { get; set; }

        [Display(Name = "Check pressure of filters and infiltration line")]
        public string? Pressures { get; set; }

        [Display(Name = "Comments")]
        public string? Preassures_Comment { get; set; }

        [Display(Name = "Check water level in tanks")]
        public string? Tanks { get; set; }

        [Display(Name = "Comments")]
        public string? Tanks_Comment { get; set; }

        [Display(Name = "Do we need a suction truck for slurry?")]
        public bool Suction_Tank { get; set; }

        [Display(Name = "Comments")]
        public string? Suction_Tank_Comment { get; set; }

        [Display(Name = "Clear water pump above slurry?")]
        public bool Clear_Water_Pump { get; set; }

        [Display(Name = "Comments")]
        public string? Clear_Water_Pump_Comment { get; set; }

        [Display(Name = "All filters in operation?")]
        public bool Filters_Operation { get; set; }

        [Display(Name = "Comments")]
        public string? Filters_Operation_Comment { get; set; }

        [Display(Name = "All valves in correct position?")]
        public bool Valves { get; set; }

        [Display(Name = "Comments")]
        public string? Valves_Comment { get; set; }

        [Display(Name = "Heating / Insulation in place?")]
        public bool Heating { get; set; }

        [Display(Name = "Comments")]
        public string? Heating_Comment { get; set; }

        [Display(Name = "Is water clear after filtration?")]
        public bool Water_Clear { get; set; }

        [Display(Name = "Comments")]
        public string? Water_Clear_Comment { get; set; }

        [Display(Name = "Do flowmeters work? and fit with SCADA?")]
        public bool FlowMeters { get; set; }

        [Display(Name = "Comments")]
        public string? FlowMeters_Comment { get; set; }

        [Display(Name = "Latitude")]
        public double? Latitude { get; set; }

        [Display(Name = "Longitude")]
        public double? Longitude { get; set; }

        [Display(Name = "Done by")]
        public string? DoneBy { get; set; }

        [Display(Name = "Signature")]
        public string? Signature { get; set; }

        public DateTime? EnteredIntoDataBase { get; set; }

        [Display(Name = "General Comments")]
        public string? GeneralComments { get; set; }

        public ICollection<PhotoFileWTPCheck> Photos { get; set; } = new List<PhotoFileWTPCheck>();
    }

}
