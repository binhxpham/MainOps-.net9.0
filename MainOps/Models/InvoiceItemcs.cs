using MainOps.Models.ReportClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class InvoiceItem
    {
        [Display(Name = "Item Name")]
        public string Item_Name { get; set; }
        [Display(Name = "Amount")]
        public double Amount { get; set; }
        [Display(Name = "Days")]
        public double Days { get; set; }
        [Display(Name = "Unit Price")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal? price { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Rental Price")]
        public decimal? rental_price { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Date")]
        public DateTime Install_date { get; set; }
        public DateTime Invoice_date { get; set; }
        public int? ItemTypeId { get; set; }
        public virtual ItemType ItemType { get; set; }
        public int? MobilizationId { get; set; }
        public virtual Mobilize Mobilize { get; set; }
        [ForeignKey("Install")]
        public int? InstallationId { get; set; }
        public virtual Install Install { get; set; }
        public int? ArrivalId { get; set; }
        public virtual Arrival Arrival { get; set; }
        public int? ExtraWorkId { get; set; }
        public virtual ExtraWork ExtraWork { get; set; }
        public int? Daily_Report_2Id { get; set; }
        public virtual Daily_Report_2 Daily_Report_2 { get; set; }
        [NotMapped]
        public List<string> doc_paths { get; set; }
        [Display(Name = "BoQ Nr")]
        public decimal BoQNr { get; set; }
        [Display(Name = "BoQ Nr")]
        public decimal? BoQNr_Rental { get; set; }
        public string location { get; set; }
        public int? SubProjectId { get; set; }
        [NotMapped]
        public IList<Discount> Discounts { get; set; }
        [NotMapped]
        public IList<Discount_Installation> Discounts_Installation { get; set; }
        [Display(Name = "Total Discount")]
        public decimal? Total_Discount { get; set; }
        [Display(Name = "Total Discount")]
        public decimal? Total_Discount_Installation { get; set; }
        public InvoiceItem()
        {
            Discounts = new List<Discount>();
        }
        public InvoiceItem(InvoiceItem ii)
        {
            this.Item_Name = ii.Item_Name;
            this.Amount = ii.Amount;
            this.Days = ii.Days;
            this.Daily_Report_2Id = ii.Daily_Report_2Id;
            this.ExtraWorkId = ii.ExtraWorkId;
            this.InstallationId = ii.InstallationId;
            this.MobilizationId = ii.MobilizationId;
            this.ArrivalId = ii.ArrivalId;
            this.Install_date = ii.Install_date;
            this.location = ii.location;
            this.SubProjectId = ii.SubProjectId;
            this.ItemTypeId = ii.ItemTypeId;
            this.Install = ii.Install;
            this.Mobilize = ii.Mobilize;
            this.Arrival = ii.Arrival;
            this.ExtraWork = ii.ExtraWork;
            this.Daily_Report_2 = ii.Daily_Report_2;
            this.BoQNr = ii.BoQNr;
            this.BoQNr_Rental = ii.BoQNr_Rental;
            this.doc_paths = ii.doc_paths;
            this.price = ii.price;
            this.rental_price = ii.rental_price;
            this.Discounts = ii.Discounts;
            this.Invoice_date = ii.Invoice_date;
        }
    }
}
