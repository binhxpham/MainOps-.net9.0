using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class Maintenance
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
        public int? InstallId { get; set; }
        public virtual Install? Install { get; set; }
        public int? MeasPointId { get; set; }
        public virtual MeasPoint? MeasPoint { get; set; }
        [Display(Name = "Maintenance Point")]
        public string? MaintenancePoint { get; set; }
        [Display(Name = "Describe the Maintenance")]
        public string? LogText { get; set; }
        [Display(Name = "Hours Spent")]
        public TimeSpan HoursSpent { get; set; }
        [Display(Name = "Maintenance Photos")]
        public ICollection<PhotoFileMaintenance>? MaintenancePhotos { get; set; }
        [Display(Name = "Maintenance Entries")]
        public ICollection<MaintenanceEntry>? MaintenanceEntries { get; set; }
        public DateTime EnteredIntoDataBase { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? DoneBy { get; set; }
        [Display(Name = "Stock Item")]
        public int? HJItemId { get; set; }
        public virtual  HJItem? HJItem { get; set; }
        [Required]
        public string? Signature { get; set; }
    }
    public class MaintenanceWithEntry
    {
        public Maintenance Maintenance { get; set; }
        public MaintenanceEntry Entry { get; set; }
        public MaintenanceWithEntry(Maintenance m,MaintenanceEntry e)
        {
            this.Maintenance = m;
            this.Entry = e;
        }
        public bool HasPhotos { get; set; }
    }
    public class MaintenanceEntry
    {
        public int Id { get; set; }
        public int? MaintenanceId { get; set; }
        public virtual Maintenance? Maintenance { get; set; }
        [Display(Name = "Type of Maintenance")]
        public int? MaintenanceTypeId { get; set; }
        public virtual MaintenanceType? MaintenanceType { get; set; }
        [Display(Name = "Maintenance Action")]
        public int? MaintenanceSubTypeId { get; set; }
        public virtual MaintenanceSubType? MaintenanceSubType { get; set; }
    }
    public class MaintenanceVM
    {
        [Display(Name = "Project")]
        [Required]
        public int ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        [Display(Name = "Sub Project")]
        public int? SubProjectId { get; set; }
        public virtual SubProject? SubProject { get; set; }
        [Display(Name = "Title")]
        public int? TitleId { get; set; }
        public virtual Title? Title { get; set; }
        [Display(Name = "Item")]
        public int? InstallId { get; set; }
        public virtual Install? Install { get; set; }
        [Display(Name = "Item")]
        public int? MeasPointId { get; set; }
        public virtual MeasPoint? MeasPoint { get; set; }
        [Display(Name = "Chosen Item")]
        [Required]
        [StringLength(250, MinimumLength = 1, ErrorMessage = "Please Fill")]
        public string? MaintenancePoint { get; set; }
        [Display(Name = "Description")]
        public string? LogText { get; set; }
        [Display(Name = "Time of Maintenance")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime TimeStamp { get; set; }       
        public virtual MaintenanceSubType? MaintenanceSubType { get; set; }
        [StringLength(1250, MinimumLength = 1, ErrorMessage = "Please Fill")]
        public string? MaintenanceTypeList { get; set; }
        public string? MaintenanceSubTypeList { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        [Display(Name = "Time Spent")]
        [Required]
        public TimeSpan HoursSpent { get; set; }
        [Display(Name = "Your Signature")]
        public string? Signature { get; set; }
        [Display(Name = "Stock Item For Service Maintenance")]
        public int? HJItemId { get; set; }
        public List<CoordTrack2>?  installations { get; set; }
        public List<MeasPoint>? MeasPoints { get; set; }
    }
    public class MaintenanceType
    {
        [Key]
        public int Id { get; set; }
        public string? Type { get; set; }
    }
    public class MaintenanceSubType
    {
        [Key]
        public int Id { get; set; }
        public string? Type { get; set; }
        public int? MaintenanceTypeId { get; set; }
        public virtual MaintenanceType? MaintenanceType { get; set; }
    }
}
