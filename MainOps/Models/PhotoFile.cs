using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainOps.Models
{
    public class PumpTestPhoto
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Step Test")]
        public int ThreeStepTestId { get; set; }
        public virtual ThreeStepTest ThreeStep { get; set; }
    }
    public class ClearPumpTestPhoto
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Step Test")]
        public int ClearPumpTestId { get; set; }
        public virtual ClearPumpTest ClearPumpTest { get; set; }
    }
    public class GroutWMBeforePhoto
    {
        [Key]
        public int Id { get; set; }
        public string path { get; set; }
        public DateTime TimeStamp { get; set; }
        [ForeignKey("Grouting")]
        public int? GroutingId { get; set; }
    }
    public class GroutWMAfterPhoto
    {
        [Key]
        public int Id { get; set; }
        public string path { get; set; }
        public DateTime TimeStamp { get; set; }
        [ForeignKey("Grouting")]
        public int? GroutingId { get; set; }
    }
    public class GroutBeforePhoto
    {
        [Key]
        public int Id { get; set; }
        public string path { get; set; }
        public DateTime TimeStamp { get; set; }
        [ForeignKey("Grouting")]
        public int? GroutingId { get; set; }
    }
    public class GroutGroutPhoto
    {
        [Key]
        public int Id { get; set; }
        public string path { get; set; }
        public DateTime TimeStamp { get; set; }
        [ForeignKey("Grouting")]
        public int? GroutingId { get; set; }
    }
    public class GroutAfterPhoto
    {
        [Key]
        public int Id { get; set; }
        public string path { get; set; }
        public DateTime TimeStamp { get; set; }
        [ForeignKey("Grouting")]
        public int? GroutingId { get; set; }
    }
    public class PhotoFilePack
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Tracked item")]
        public int TrackItemId { get; set; }
        public virtual TrackItem TrackItem { get; set; }
    }
    public class PhotoFileSent
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Tracked item")]
        public int TrackItemId { get; set; }
        public virtual TrackItem TrackItem { get; set; }
    }
    public class PhotoFileReceived
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Tracked item")]
        public int TrackItemId { get; set; }
        public virtual TrackItem TrackItem { get; set; }
    }
    public class PhotoFileMobilized
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Tracked item")]
        public int MobilizeId { get; set; }
        public virtual Mobilize Mobilize { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
    public class PhotoFileAlarmCall
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int AlarmCallId { get; set; }
        public virtual AlarmCall AlarmCall { get; set; }
    }
    public class PhotoFileDataLoggerInstall
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int DataLoggerInstallId { get; set; }
        public virtual DataLoggerInstall DataLoggerInstall { get; set; }
    }
    public class PhotoFileInstalled2
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Tracked item")]
        public int InstallId { get; set; }
        public virtual Install Install { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
    public class PhotoFileArrival
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Tracked item")]
        public int ArrivalId { get; set; }
        public virtual Arrival Arrival { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
    public class PhotoFileDeinstalled
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Tracked item")]
        public int DeinstallId { get; set; }
        public virtual DeInstall DeInstall { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
    public class PhotoFileInstalled
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Tracked item")]
        public int TrackItemId { get; set; }
        public virtual TrackItem TrackItem { get; set; }

    }

    public class PhotoError
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Tracked item")]
        public int TrackItemId { get; set; }
        public virtual TrackItem TrackItem { get; set; }
    }
    public class PhotoFileMaintenance
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Tracked item")]
        public int MaintenanceId { get; set; }
        public virtual Maintenance Maintenance { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
    public class PhotoFileWTPCheck
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Tracked item")]
        public int WTPCheckId { get; set; }
        public virtual WTPCheck WTPCheck { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
    public class PhotoFileGeneratorCheck
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Tracked item")]
        public int GeneratorCheckId { get; set; }
        public virtual GeneratorCheck GeneratorCheck { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
    public class PhotoFileSensorsCheck
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Tracked item")]
        public int SensorsCheckId { get; set; }
        public virtual SensorsCheck SensorsCheck { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
    public class PhotoFileConstructionSiteInspection
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Tracked item")]
        public int ConstructionSiteInspectionId { get; set; }
        public virtual ConstructionSiteInspection ConstructionSiteInspection { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
    public class PhotoFileDecommission
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Tracked item")]
        public int DecommissionId { get; set; }
        public virtual Decommission Decommission { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
    public class PhotoFileSiteCheck
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Tracked item")]
        public int SiteCheckId { get; set; }
        public virtual SiteCheck SiteCheck { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}