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
        public int? MeasPointId { get; set; }
        [Display(Name = "Well Name")] 
        public virtual MeasPoint MeasPoint { get; set; }

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

       

    }
   
    public class SensorCalibrationVM
    {
        [Display(Name = "Project")]
        [Required]
        public int ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        [Display(Name = "Sub Project")]
        public int? SubProjectId { get; set; }
        public virtual SubProject? SubProject { get; set; }
              
        [Display(Name = "Well Name")]
        public int? WellId { get; set; }

        [Display(Name = "Well Name")]
        public int? MeasPointId { get; set; }
        [Display(Name = "Well Name")] 
        public virtual MeasPoint? MeasPoint { get; set; }

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

        [Display(Name = "Comment")]
        public string? Comment { get; set; }

        [Display(Name = "Time of dip")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime TimeStamp { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }


        [Display(Name = "Your Signature")]
        public string? Signature { get; set; }

   }
}
