using MainOps.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MainOps.Models.ViewModels
{
    public class MobilizeItemVM
    {
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public int? SubProjectId { get; set; }
        public virtual SubProject SubProject { get; set; }
        public int? VariationOrderId { get; set; }
        public virtual BoQHeadLine VariationOrder { get; set; }
        [Required]
        public int ItemTypeId { get; set; }
        public virtual ItemType ItemType { get; set; }
        [Display(Name = "Description")]
        public string LogText { get; set; }
        public double Amount { get; set; }
        public DateTime TimeStamp { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string UniqueID { get; set; }

    }
    public class InstallItem2VM
    {
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public int? SubProjectId { get; set; }
        public virtual SubProject SubProject { get; set; }
        [Display(Name = "Item Type")]
        public int ItemTypeId { get; set; }
        public virtual ItemType ItemType { get; set; }
        [Display(Name = "Description")]
        public string LogText { get; set; }
        [Display(Name = "Amount")]
        public int Amount { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime TimeStamp { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        [Display(Name = "Unique ID")]
        public string UniqueID { get; set; }
    }
    public class InstallItem3VM
    {
        [Display(Name = "Project")]
        public int? ProjectId { get; set; }
        [Display(Name = "Project")]
        public virtual Project Project { get; set; }
        [Display(Name = "Sub Project")]
        public int? SubProjectId { get; set; }
        [Display(Name = "Sub Project")]
        public virtual SubProject SubProject { get; set; }
        [Display(Name = "Variation Order")]
        public int? VariationOrderId { get; set; }
        [Display(Name = "Variation Order")]
        public virtual BoQHeadLine VariationOrder { get; set; }
        [Display(Name = "Item Type")]
        public string ItemTypeIds { get; set; }        
        [Display(Name = "Description")]
        public string LogText { get; set; }
        [Display(Name = "Amount")]
        public string Amounts { get; set; }
        [Display(Name = "TimeStamp")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime TimeStamp { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        [Display(Name = "Unique ID")]
        public string UniqueIDs { get; set; }
        public DateTime? InvoiceDate { get; set; }
    }
    
    public class ArrivalItemVM
    {
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public int? SubProjectId { get; set; }
        public virtual SubProject SubProject { get; set; }
        public int? MobilisationId { get; set; }
        public virtual Mobilize Mobilize { get; set; }
        public int ItemTypeId { get; set; }
        public virtual ItemType ItemType { get; set; }
        [Display(Name = "Description")]
        public string LogText { get; set; }
        public int Amount { get; set; }
        public DateTime TimeStamp { get; set; }
        public DateTime InvoiceDate { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
    public class ArrivalItem3VM
    {
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public int? SubProjectId { get; set; }
        public virtual SubProject SubProject { get; set; }
        public int? VariationOrderId { get; set; }
        public virtual BoQHeadLine VariationOrder { get; set; }
        [Display(Name = "From")]
        public int? MobilisationId { get; set; }
        public virtual Mobilize Mobilize { get; set; }
        [Display(Name = "Item Type")]
        public string ItemTypeIds { get; set; }
        [Display(Name = "Description")]
        public string LogText { get; set; }
        [Display(Name = "Amount")]
        public string Amounts { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime TimeStamp { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string UniqueIDs { get; set; }
        public DateTime? InvoiceDate { get; set; }
    }
    public class DeInstallItemVM
    {
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public int ItemTypeId { get; set; }
        public virtual ItemType ItemType { get; set; }
        [Display(Name = "Description")]
        public string LogText { get; set; }
        public int? Amount { get; set; }
        public DateTime TimeStamp { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int DeInstallItemId { get; set; }
        [Display(Name = "Coordinates")]
        public ICollection<Install> InstalledCoordinates { get; set; }
    }
    public class MeasurePipeVM
    {
        [Display(Name = "Description")]
        public double? Amount { get; set; }
        public double? Amount_raw { get; set; }
        public DateTime TimeStamp { get; set; }
        public int InstallationsId { get; set; }
        public virtual Install Install { get; set; }
        public List<double> Latitudes { get; set; }
        public List<double> Longitudes { get; set; }
        [Display(Name = "Coordinates")]
        public ICollection<Install> InstalledCoordinates { get; set; }
    }
    public class InstallSmallVM
    {
        public int Amount { get; set; }
        public DateTime TimeStamp { get; set; }
        public int? InstallationsId { get; set; }
        public virtual Install Install { get; set; }
        [Display(Name = "Coordinates")]
        public ICollection<Install> InstalledCoordinates { get; set; }
    }
    
}