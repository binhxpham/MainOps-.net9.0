using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class StockRentalItem
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Rental Item")]
        public int? HJItemId { get; set; }
        [Display(Name = "Rental Item number")]
        public string ItemNumber { get; set; }
        [Display(Name = "Rental Item")]
        public virtual HJItem HJItem { get; set; }
        [Display(Name = "Company")]
        public string Company { get; set; }
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Phone]
        [Display(Name = "Phone")]
        public string PhoneNr { get; set; }
        [Display(Name = "Start of Rent")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; }
        [Display(Name = "End of Rent")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EndTime { get; set; }
        [Display(Name = "DataBase Entry")]
        public DateTime? TimeSetup { get; set; }
        [Display(Name = "Rental Price")]
        public decimal RentalFee { get; set; }
        public int? Rental_UnitId { get; set; }
        [Display(Name = "Rental Unit")]
        public virtual Unit Rental_Unit { get; set; }
        [Display(Name = "Photos Delivery")]
        public ICollection<StockRentalItemPhotoDelivery> Photos_Delivery { get; set; }
        [Display(Name = "Photos Return")]
        public ICollection<StockRentalItemPhotoReturn> Photos_Return { get; set; }
        public bool IsReturned { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? LastInvoiced { get; set; }
        [Display(Name = "Total Rental Earning")]
        [NotMapped]
        public decimal? TotalCost
        {
            get
            {
                if (Rental_UnitId.Equals(3))
                {
                    if (this.EndTime != null)
                    {
                        return (decimal)((this.EndTime.Value.Date - this.StartTime.Date).TotalDays + 1) * this.RentalFee;
                    }
                    else
                    {
                        return (decimal)((DateTime.Now.Date - this.StartTime.Date).TotalDays + 1) * this.RentalFee;
                    }
                }
                else if (Rental_UnitId.Equals(17))
                {
                    if (this.EndTime != null)
                    {
                        return (decimal)(Math.Ceiling((this.EndTime.Value.Date - this.StartTime.Date).TotalDays + 1) / 7.0) * this.RentalFee;
                    }
                    else
                    {
                        return (decimal)(Math.Ceiling((DateTime.Now.Date - this.StartTime.Date).TotalDays + 1) / 7.0) * this.RentalFee;
                    }
                }
                else if (Rental_UnitId.Equals(16))
                {
                    if (this.EndTime != null)
                    {
                        return (decimal)((this.EndTime.Value.Month - this.StartTime.Month) + 1) * this.RentalFee;
                    }
                    else
                    {
                        return (decimal)((DateTime.Now.Month - this.StartTime.Month) + 1) * this.RentalFee + (decimal)((DateTime.Now.Year - this.StartTime.Year) * 12.0) * this.RentalFee;
                    }
                }
                else
                {
                    return (decimal)0.0;
                }

            }
        }
    }
   
    public class StockRentalItemPhotoDelivery
    {
        [Key]
        public int Id { get; set; }
        public int? StockRentalItemId { get; set; }
        public virtual StockRentalItem StockRentalItem { get; set; }
        public string path { get; set; }
    }
    public class StockRentalItemPhotoReturn
    {
        [Key]
        public int Id { get; set; }
        public int? StockRentalItemId { get; set; }
        public virtual StockRentalItem StockRentalItem { get; set; }
        public string path { get; set; }
    }
}
