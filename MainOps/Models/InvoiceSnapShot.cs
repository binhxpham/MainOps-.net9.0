using MainOps.Models.ReportClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class CompareInvoiceToSnapShot
    {
        public InvoiceSnapShot SnapShot { get; set; }
        public InvoiceModel Model { get; set; }
    }
    public class CompareTwoSnapShots
    {
        public InvoiceSnapShot SnapShot1 { get; set; }
        public InvoiceSnapShot SnapShot2 { get; set; }
    }
    public class InvoiceSnapShot
    {
        [Key]
        public int? Id { get; set; }
        public string SnapShotName { get; set; }
        public ICollection<InvoiceItemDB> Items { get; set; }
        public DateTime SnapShotStartDate { get; set; }
        public DateTime SnapShotEndDate { get; set; }
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public int? SubProjectId { get; set; }
        public virtual SubProject SubProject { get; set; }
        public DateTime SnapShotTimeStamp { get; set; }
        public InvoiceSnapShot()
        {

        }
        public InvoiceSnapShot(DateTime StartDate,DateTime EndDate, int ProjectId, int? SubProjectId)
        {
            this.SnapShotStartDate = StartDate;
            this.SnapShotEndDate = EndDate;
            this.SnapShotTimeStamp = DateTime.Now;
            this.ProjectId = ProjectId;
            this.SubProjectId = SubProjectId;
        }


    }
    public class InvoiceItemDB : InvoiceItem
    {
        [Key]
        public int? Id { get; set; }
        [ForeignKey("InvoiceSnapShot")]
        public int? InvoiceSnapShotId { get; set; }
        public virtual InvoiceSnapShot InvoiceSnapShot { get; set; }
        public bool? FullPeriod { get; set; }
        public DateTime? EndStamp { get; set; }
        public bool IsNew { get; set; }
        public bool IsAltered { get; set; }
        public string ChangeLog { get; set; }
        public bool isIdleItem { get; set; }
        public bool WasRemoved { get; set; }
        
     
        public InvoiceItemDB(InvoiceItem ii,int SnapShotId,bool fullPeriod,bool isIdleItem)
        {
            if(ii.ArrivalId == 10000)
            {
                double meh = 2.0;
            }
            this.FullPeriod = fullPeriod;
            this.InvoiceSnapShotId = SnapShotId;
            this.ExtraWorkId = ii.ExtraWorkId;
            this.InstallationId = ii.InstallationId;
            this.Amount = ii.Amount;
            this.Days = ii.Days;
            this.Install_date = ii.Install_date;
            this.ItemTypeId = ii.ItemTypeId;
            this.Item_Name = ii.Item_Name;
            this.location = ii.location;
            this.MobilizationId = ii.MobilizationId;
            this.price = ii.price;
            this.rental_price = ii.rental_price;
            this.SubProjectId = ii.SubProjectId;
            this.Total_Discount = ii.Total_Discount;
            this.Total_Discount_Installation = ii.Total_Discount_Installation;
            this.ArrivalId = ii.ArrivalId;
            this.BoQNr = ii.BoQNr;
            this.BoQNr_Rental = ii.BoQNr_Rental;
            this.Daily_Report_2Id = ii.Daily_Report_2Id;
            this.isIdleItem = isIdleItem;
            
            if(ii.ArrivalId != null)
            {
                if(ii.Arrival.EndStamp != null)
                {
                    this.EndStamp = ii.Arrival.EndStamp;
                }
            }
            else if (ii.InstallationId != null)
            {
                if (ii.Install.DeinstallDate != null)
                {
                    this.EndStamp = ii.Install.DeinstallDate;
                }
            }
        }
        public InvoiceItemDB()
        {

        }
    }
}
