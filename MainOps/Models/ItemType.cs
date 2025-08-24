using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class ItemType
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Item")]
        public string? Item_Type { get; set; }
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
        public virtual Unit? Install_Unit { get; set; }
        [Display(Name = "Rental Unit")]
        public int? Rental_UnitId { get; set; }
        public virtual Unit? Rental_Unit { get; set; }
        public decimal? daily_cost { get; set; }
        public decimal? initial_cost { get; set; }
        [ForeignKey("Project")]
        [Display(Name = "Project")]
        public int? ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        [Display(Name = "Currency")]
        public string? Valuta { get; set; }
        [Display(Name = "Report Type")]
        public int? ReportTypeId { get; set; }
        public virtual ReportType? ReportType { get; set; }
        public ICollection<Discount>? Discounts { get; set; }
        public string? AgreementNumber { get; set; }
        public DateTime? AgreementDate { get; set; }
        public ICollection<Discount_Installation>? Discounts_Installation { get; set; }
        public ICollection<DecommissionableItem>? InstalledDecommissionableItems { get; set; }
        public ICollection<DecommissionableItem>? BoQDecommissionableItems { get; set; }
        public string? MarkerPicture { get; set; }
        //public int ExpectedAmounts { get; set; }
        [Display(Name = "Expected Quantity Installed")]
        public double? ExpectedAmount { get; set; }
        [Display(Name = "Expected Quantity Rental")]
        public double? ExpectedAmountRental { get; set; }
        public ItemType()
        {

        }
        public ItemType(ItemType it, int projid)
        {
            this.Item_Type = it.Item_Type;
            this.ProjectId = projid;
            this.ReportTypeId = it.ReportTypeId;
            this.Valuta = it.Valuta;
            this.initial_cost = it.initial_cost;
            this.daily_cost = it.daily_cost;
            this.Rental_UnitId = it.Rental_UnitId;
            this.Install_UnitId = it.Install_UnitId;
            this.price = it.price;
            this.rental_price = it.rental_price;
            this.BoQnr = it.BoQnr;
            this.BoQnr_Rental = it.BoQnr_Rental;
            this.AgreementDate = it.AgreementDate;
            this.AgreementNumber = it.AgreementNumber;
            this.ExpectedAmount = it.ExpectedAmount;
            this.ExpectedAmountRental = it.ExpectedAmountRental;
        }
    }
}
