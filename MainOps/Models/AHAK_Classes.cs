using MainOps.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class Mobilize
    {
        public int Id { get; set; }
        [Display(Name = "Project")]
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        [Display(Name = "Sub Project")]
        public int? SubProjectId { get; set; }
        [Display(Name = "Sub Project")]
        public virtual SubProject SubProject { get; set; }
        [Display(Name = "Item")]
        [Required]
        public int? ItemTypeId {get; set;}
        [Display(Name = "Item")]
        public virtual ItemType ItemType { get; set; }
        [Display(Name = "Description")]
        public string? MobilizeText_Text { get; set; }
        public double? Amount { get; set; }
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime TimeStamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? DoneBy { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime? EnteredIntoDataBase { get; set; }
        public DateTime? LastEditedInDataBase { get; set; }
        public bool ToBePaid { get; set; }
        public double PaidAmount { get; set; } 
        public string? UniqueID { get; set; }
        public int? VariationOrderId { get; set; }
        public virtual BoQHeadLine VariationOrder { get; set; }
        IList<string> pdfs { get; set; }
        public bool Within_Coords(double latitude, double longitude)
        {
            double distance = DistanceAlgorithm.DistanceBetweenPlaces(longitude, latitude, Convert.ToDouble(this.Longitude), Convert.ToDouble(this.Latitude));
            if (distance < 0.01)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class AlarmCall
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Project")]
        public int? ProjectId { get; set; }
        [Display(Name = "Project")]
        public virtual Project Project { get; set; }
        [Display(Name = "Sub Project")]
        public int? SubProjectId { get; set; }
        [Display(Name = "Sub Project")]
        public virtual SubProject SubProject { get; set; }        
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Description")]
        public string? LogText { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime? EnteredIntoDataBase { get; set; }
        public DateTime? LastEditedInDataBase { get; set; }
        [Display(Name = "Hours Used")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan Hours { get; set; }
        [Display(Name = "Time of Alarm")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [RegularExpression(@"((([0-1][0-9])|(2[0-3]))(:[0-5][0-9])(:[0-5][0-9])?)", ErrorMessage = "Time must be between 00:00 to 23:59")]
        public TimeSpan? Alarm_Time { get; set; }
        public string? DoneBy { get; set; }
        public ICollection<PhotoFileAlarmCall> pictures { get; set; }
    }
    public class Install
    {
        public int Id { get; set; }
        [Display(Name = "Project")]
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        [Display(Name = "Sub Project")]
        public int? SubProjectId { get; set; }
        [Display(Name = "Sub Project")]
        public virtual SubProject SubProject { get; set; }
        [Display(Name = "Item")]
        public int ItemTypeId { get; set; }
        [Display(Name = "Item")]
        public virtual ItemType? ItemType { get; set; }
        [Display(Name = "Installation Text")]
        public string? Install_Text { get; set; }        
        public bool isInstalled { get; set; }
        [Display(Name = "Done by")]
        public string? DoneBy { get; set; }
        [Display(Name = "DeInstallation Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DeinstallDate { get; set; }
        public double? Amount { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime TimeStamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Location { get; set; }
        [Display(Name = "Unique ID")]
        public string? UniqueID { get; set; }
        public DateTime? EnteredIntoDataBase { get; set; }
        public DateTime? LastEditedInDataBase { get; set; }
        [Display(Name = "Start of Rental")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RentalStartDate { get; set; }
        public bool ToBePaid { get; set; }
        public bool IsInOperation { get; set; }
        public DateTime InvoiceDate { get; set; }
        public double? PayedAmount { get; set; }
        public ICollection<CoordTrack2>? Coordinates { get; set; }
        public string? BaseID
        {
            get
            {
                return new String(this.UniqueID.Where(Char.IsDigit).ToArray());
            }
        }
        public bool Within_Coords(double latitude, double longitude)
        {
            double distance = DistanceAlgorithm.DistanceBetweenPlaces(longitude, latitude, Convert.ToDouble(this.Longitude), Convert.ToDouble(this.Latitude));
            if (distance < 0.01)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int? VariationOrderId { get; set; }
        public virtual BoQHeadLine? VariationOrder { get; set; }
    }
    public class Arrival
    {
        public int Id { get; set; }
        [Display(Name = "Project")]
        public int? ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        [Display(Name = "Sub Project")]
        public int? SubProjectId { get; set; }
        [Display(Name = "Sub Project")]
        public virtual SubProject? SubProject { get; set; }
        public int? MobilisationId { get; set; }
        public virtual Mobilize? Mobilize { get; set; }
        [Display(Name = "Item")]
        public int? ItemTypeId { get; set; }
        [Display(Name = "Item")]
        public virtual ItemType? ItemType { get; set; }
        [Display(Name = "Description")]
        public string? Arrival_Text { get; set; }
        public double? Amount { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start of Rent")]
        public DateTime TimeStamp { get; set; }
        public string? UniqueID { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime? EnteredIntoDataBase { get; set; }
        public DateTime? LastEditedInDataBase { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "End of Rent")]
        public DateTime? EndStamp { get; set; }
        public DateTime InvoiceDate { get; set; }
        public double? PayedAmount { get; set; }
        public bool ToBePaid { get; set; }
        public int? VariationOrderId { get; set; }
        public virtual BoQHeadLine? VariationOrder { get; set; }
        public bool Within_Coords(double latitude, double longitude)
        {
            double distance = DistanceAlgorithm.DistanceBetweenPlaces(longitude, latitude, Convert.ToDouble(this.Longitude), Convert.ToDouble(this.Latitude));
            if (distance < 0.01)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public Arrival()
        {

        }
        public Arrival(Arrival arr)
        {
            this.Id = arr.Id;
            this.VariationOrder = arr.VariationOrder;
            this.VariationOrderId = arr.VariationOrderId;
            this.InvoiceDate = arr.InvoiceDate;
            this.ItemTypeId = arr.ItemTypeId;
            this.ItemType = arr.ItemType;
            this.LastEditedInDataBase = arr.LastEditedInDataBase;
            this.Latitude = arr.Latitude;
            this.Longitude = arr.Longitude;
            this.MobilisationId = arr.MobilisationId;
            this.Mobilize = arr.Mobilize;
            this.SubProject = arr.SubProject;
            this.SubProjectId = arr.SubProjectId;
            this.UniqueID = arr.UniqueID;
            this.TimeStamp = arr.TimeStamp;
            this.EndStamp = arr.EndStamp;
            this.Arrival_Text = arr.Arrival_Text;
            this.Project = arr.Project;
            this.ProjectId = arr.ProjectId;
            this.ToBePaid = arr.ToBePaid;
            this.Amount = arr.Amount;
            this.PayedAmount = arr.PayedAmount;
        }
        public Arrival(Arrival old,double amount,int ProjectId,int? SubProjectId,DateTime starttime)
        {
            this.ToBePaid = old.ToBePaid;
            this.InvoiceDate = DateTime.Now;
            this.Amount = amount;
            this.Arrival_Text = old.Arrival_Text;
            this.Latitude = old.Latitude;
            this.Longitude = old.Longitude;
            this.ProjectId = ProjectId;
            this.SubProjectId = SubProjectId;
            this.MobilisationId = old.MobilisationId;
            this.ItemTypeId = old.ItemTypeId;
            this.TimeStamp = starttime;
            this.EnteredIntoDataBase = old.EnteredIntoDataBase;
            this.LastEditedInDataBase = old.LastEditedInDataBase;
            this.VariationOrder = old.VariationOrder;
            this.VariationOrderId = old.VariationOrderId;
        }
        public Arrival(Arrival old, double amount, int ProjectId, int? SubProjectId, DateTime starttime,int ItemTypeId)
        {
            this.ToBePaid = old.ToBePaid;
            this.InvoiceDate = DateTime.Now;
            this.Amount = amount;
            this.Arrival_Text = old.Arrival_Text;
            this.Latitude = old.Latitude;
            this.Longitude = old.Longitude;
            this.ProjectId = ProjectId;
            this.SubProjectId = SubProjectId;
            this.MobilisationId = old.MobilisationId;
            this.ItemTypeId = ItemTypeId;
            this.TimeStamp = starttime;
            this.EnteredIntoDataBase = old.EnteredIntoDataBase;
            this.LastEditedInDataBase = old.LastEditedInDataBase;
            this.VariationOrder = old.VariationOrder;
            this.VariationOrderId = old.VariationOrderId;
        }
    }
    public class DeInstallModel
    {
        public int? id { get; set; }
        public DateTime TimeStamp { get; set; }
        public double? Amount { get; set; }
        public string? LogText { get; set; }
    }
    public class DeInstall
    {
        public int Id { get; set; }
        [Display(Name = "Project")]
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        [Display(Name = "Sub Project")]
        public int? SubProjectId { get; set; }
        [Display(Name = "Sub Project")]
        public virtual SubProject SubProject { get; set; }
        [Display(Name = "Item")]
        public int? ItemTypeId {get; set;}
        [Display(Name = "Item")]
        public virtual ItemType ItemType { get; set; }
        public int? InstallId { get; set; }
        public virtual Install Install { get; set; }
        [Display(Name = "Description")]
        public string? DeInstall_Text { get; set; }
        public double? Amount { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Deinstall Date")]
        public DateTime TimeStamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime? EnteredIntoDataBase { get; set; }
        public DateTime? LastEditedInDataBase { get; set; }
        public bool Within_Coords(double latitude, double longitude)
        {
            double distance = DistanceAlgorithm.DistanceBetweenPlaces(longitude, latitude, Convert.ToDouble(this.Longitude), Convert.ToDouble(this.Latitude));
            if (distance < 0.01)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string? DoneBy { get; set; }
    }
    public class SmallPart
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        [ForeignKey("Item")]
        public int ItemTypeId { get; set; }
        [Display(Name = "Item")]
        public virtual ItemType ItemType { get; set; }
    }
    public class InstallInOperation
    {
        public Install install { get; set; }
        public bool operational { get; set; }
        public DateTime TheDate { get; set; }
        public InstallInOperation()
        {

        }
        public InstallInOperation(Install ins,DateTime date,bool operational)
        {
            this.install = ins;
            this.operational = operational;
            this.TheDate = date;
        }
    }

}
