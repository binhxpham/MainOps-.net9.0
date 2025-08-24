using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class ExtraWorkBoQ
    {
        [Key]
        public int? Id { get; set; }
        [Display(Name = "Title of Extra Work BoQ")]
        public string ExtraWorkSubTitle { get; set; }
        [Display(Name = "Extra Work Main Header")]
        public int? BoQHeadLineId { get; set; }
        [Display(Name = "Extra Work Main Header")]
        public virtual BoQHeadLine BoQHeadLine {get; set;}
        [Display(Name = "Sub Headers")]
        public ICollection<ExtraWorkBoQHeader> Headers { get; set; }
        [Display(Name = "Bullet Point Descriptions")]
        public ICollection<ExtraWorkBoQDescription> Descriptions { get; set; }
        public ExtraWorkBoQ()
        {

        }
        public ExtraWorkBoQ(ExtraWorkBoQVM modelin)
        {
            this.BoQHeadLineId = modelin.BoQHeadLineId;
            this.ExtraWorkSubTitle = modelin.ExtraWorkSubTitle;
        }
    }
    public class ExtraWorkBoQVM
    {
        [Display(Name = "Title of Extra Work BoQ")]
        public string ExtraWorkSubTitle { get; set; }
        public int? BoQHeadLineId { get; set; }
        [Display(Name = "Extra Work Main Header")]
        public virtual BoQHeadLine BoQHeadLine { get; set; }
        [Display(Name = "Sub Headers")]
        public List<ExtraWorkBoQHeader> Headers { get; set; }
        [Display(Name = "Bullet Point Descriptions")]
        public List<ExtraWorkBoQDescription> Descriptions { get; set; }
        [Display(Name = "BoQ Items")]
        public List<ExtraWorkBoQItem> BoQItems { get; set; }
    }
    public class ExtraWorkBoQHeader
    {
        [Key]
        public int Id { get; set; }
        public int? ExtraWorkBoQId { get; set; }
        public virtual ExtraWorkBoQ ExtraWorkBoQ { get; set; }
        [Display(Name = "Sub Header Title")]
        public string Title { get; set; }
        public string Number { get; set; }
        [Display(Name = "BoQ Items")]
        public ICollection<ExtraWorkBoQItem> BoQItems { get; set; }
    }
    public class ExtraWorkBoQDescription
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "ExtraWork BoQ")]
        public int? ExtraWorkBoQId { get; set; }
        [Display(Name = "ExtraWork BoQ")]
        public virtual ExtraWorkBoQ ExtraWorkBoQ { get; set; }
        [Display(Name = "Descriptive bullet point for this BoQ")]
        public string Description { get; set; }
        public bool IsRelatedToBoQ { get; set; }
        public string Topic { get; set; }
    }
    public class ExtraWorkBoQItem
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "New BoQ Number")]
        public string NewBoQNr { get; set; }
        [Display(Name = "New BoQ Rental Number")]
        public string NewRentalBoQNr { get; set; }
        [Display(Name = "Description of specific item")]
        public string BoQItemDescription { get; set; }
        [Display(Name = "Extra Work Sub Header")]
        public int? ExtraWorkBoQHeaderId { get; set; }
        [Display(Name = "Extra Work Sub Header")]
        public virtual ExtraWorkBoQHeader ExtraWorkBoQHeader { get; set; }

        //ItemType values needed:
        [Display(Name = "Item Name")]
        public string Item_Type { get; set; }
        [Display(Name = "BoQ Number")]
        public decimal BoQnr { get; set; }
        [Display(Name = "Rental BoQ Number")]
        public decimal? BoQnr_Rental { get; set; }
        [Display(Name = "Price")]
        public decimal? price { get; set; }
        [Display(Name = "Rental Price")]
        public decimal? rental_price { get; set; }
        [Display(Name = "Install Unit")]
        public int? Install_UnitId { get; set; }
        public virtual Unit Install_Unit { get; set; }
        [Display(Name = "Rental Unit")]
        public int? Rental_UnitId { get; set; }
        public virtual Unit Rental_Unit { get; set; }
        public string Valuta { get; set; }
        [Display(Name = "Report Type")]
        public int? ReportTypeId { get; set; }
        public virtual ReportType ReportType { get; set; }
        public string MarkerPicture { get; set; }
        //public int ExpectedAmounts { get; set; }
        [Display(Name = "Expected Quantity Installed")]
        public double? ExpectedAmount { get; set; }
        [Display(Name = "Expected Quantity Rental")]
        public double? ExpectedAmountRental { get; set; }
        public int? ItemTypeId { get; set; }
        public virtual ItemType ItemType { get; set; }
        public ExtraWorkBoQItem()
        {

        }
        public ExtraWorkBoQItem(ItemType it)
        {
            this.Item_Type = it.Item_Type;
            this.ExpectedAmount = 0;
            this.ExpectedAmountRental = 0;
            this.BoQnr = it.BoQnr;
            this.BoQnr_Rental = it.BoQnr_Rental;
            this.Install_UnitId = it.Install_UnitId;
            this.Rental_UnitId = it.Rental_UnitId;
            this.Valuta = it.Valuta;
            this.ReportTypeId = null;
            this.rental_price = it.rental_price;
            this.price = it.price;
            this.NewBoQNr = it.BoQnr.ToString("#2f");
            if (it.BoQnr_Rental != null)
            {
                this.NewRentalBoQNr = it.BoQnr_Rental.Value.ToString("0.000");
            }
            this.NewBoQNr = it.BoQnr.ToString("0.000");

        }
    }
}
