using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class SensorCalibration
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Project")]
        public int? ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        [Display(Name = "Sub Project")]
        public int? SubProjectId { get; set; }
        public virtual SubProject? SubProject { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime TimeStamp { get; set; }
        public int? WellId { get; set; }
        public virtual Well? Well { get; set; }

        [Display(Name = "Reference Level (TOP) [mDVR]")]
        public double? RefLevel { get; set; }

        [Display(Name = "Hand dip [m]")]
        public double? Hand_dip { get; set; }

        [Display(Name = "Expected Water Level [mDVR]")]
        public double? ExpectedWaterlevel { get; set; }

        [Display(Name = "Online SCADA Water Level [mDVR]")]
        public double? ScadaWaterlevel { get; set; }

        [Display(Name = "Match SCADA value?")]
        public bool SCADA_LevelMatch { get; set; }

        [Display(Name = "Comment")]
        public string? Comment { get; set; }

        public ICollection<PhotoFileSensorCalibration> Photos { get; set; } = new List<PhotoFileSensorCalibration>();

        [Required]
        public string? Signature { get; set; }

        public DateTime EnteredIntoDataBase { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? DoneBy { get; set; }

        /*[Display(Name = "Maintenance Point")]
        public string MaintenancePoint { get; set; }

        [Display(Name = "Hours Spent")]
        public TimeSpan HoursSpent { get; set; }
        
        [Display(Name = "SensorCalibration Photos")]
        public ICollection<PhotoFileSensorCalibration> SensorCalibrationPhotos { get; set; }
        
        [Display(Name = "Maintenance Entries")]
        public ICollection<MaintenanceEntry> MaintenanceEntries { get; set; }

        [Display(Name = "Stock Item")]
        public int? HJItemId { get; set; }
        
        public virtual HJItem HJItem { get; set; }*/

    }
    //public class MaintenanceWithEntry
    //{
    //    public Maintenance Maintenance { get; set; }
    //    public MaintenanceEntry Entry { get; set; }
    //    public MaintenanceWithEntry(Maintenance m,MaintenanceEntry e)
    //    {
    //        this.Maintenance = m;
    //        this.Entry = e;
    //    }
    //    public bool HasPhotos { get; set; }
    //}

    //public class MaintenanceEntry
    //{
    //    public int Id { get; set; }
    //    public int? MaintenanceId { get; set; }
    //    public virtual Maintenance Maintenance { get; set; }
    //    [Display(Name = "Type of Maintenance")]
    //    public int? MaintenanceTypeId { get; set; }
    //    public virtual MaintenanceType MaintenanceType { get; set; }
    //    [Display(Name = "Maintenance Action")]
    //    public int? MaintenanceSubTypeId { get; set; }
    //    public virtual MaintenanceSubType MaintenanceSubType { get; set; }
    //}
    public class SensorCalibrationVM
    {
        [Display(Name = "Project")]
        [Required]
        public int ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        [Display(Name = "Sub Project")]
        public int? SubProjectId { get; set; }
        public virtual SubProject? SubProject { get; set; }
        //[Display(Name = "Title")]
        //public int? TitleId { get; set; }
        //public virtual Title Title { get; set; }
        
        [Display(Name = "Well Name")]
        public int? WellId { get; set; }

        [Display(Name = "Well Name")] 
        public virtual Well? Well { get; set; }

        [Display(Name = "Reference Level (TOP) [mDVR]")]
        [Required]
        public double? RefLevel { get; set; }


        [Display(Name = "Hand dip [m]")]
        public double? Hand_dip { get; set; }

        [Display(Name = "Expected Water Level [mDVR]")]
        public double? ExpectedWaterlevel { get; set; }

        [Display(Name = "Online SCADA Water Level [mDVR]")]
        public double? ScadaWaterlevel { get; set; }


        [Display(Name = "Does the water level match SCADA value?")]
        public bool SCADA_LevelMatch { get; set; }


        //[Display(Name = "Item")]
        //public int? MeasPointId { get; set; }
        //public virtual MeasPoint MeasPoint { get; set; }
        //[Display(Name = "Chosen Item")]
        //[Required]
        //[StringLength(250, MinimumLength = 1, ErrorMessage = "Please Fill")]
        //public string MaintenancePoint { get; set; }

        [Display(Name = "Comment")]
        public string? Comment { get; set; }

        [Display(Name = "Time of dip")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime TimeStamp { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        /* public virtual MaintenanceSubType MaintenanceSubType { get; set; }

         [StringLength(1250, MinimumLength = 1, ErrorMessage = "Please Fill")]
         public string MaintenanceTypeList { get; set; }
         public string MaintenanceSubTypeList { get; set; }

         [Display(Name = "Time Spent")]
         [Required]
         public TimeSpan HoursSpent { get; set; }*/

        [Display(Name = "Your Signature")]
        public string? Signature { get; set; }

        /*
        [Display(Name = "Stock Item For Service Maintenance")]
        public int? HJItemId { get; set; }
        public List<CoordTrack2> installations { get; set; }
        public List<MeasPoint> MeasPoints { get; set; }*/
    }
    //public class MaintenanceType
    //{
    //    [Key]
    //    public int Id { get; set; }
    //    public string Type { get; set; }
    //}
    //public class MaintenanceSubType
    //{
    //    [Key]
    //    public int Id { get; set; }
    //    public string Type { get; set; }
    //    public int? MaintenanceTypeId { get; set; }
    //    public virtual MaintenanceType MaintenanceType { get; set; }
    //}
}
