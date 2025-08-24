using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class ExtraWork
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        public int? SubProjectId { get; set; }
        public virtual SubProject? SubProject { get; set; }
        public int? BoQHeadLineId { get; set; }
        public virtual BoQHeadLine? BoQHeadLine { get; set; }
        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "End Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? EndStamp { get; set; }
        public string? Description { get; set; }
        [Display(Name = "Price total")]
        public decimal Price { get; set; }
        [Display(Name = "Rental Price pr. day")]
        public decimal Rental_Price { get; set; }
        [Display(Name = "Currency")]
        public string? Valuta { get; set; }
        [Display(Name = "Is VAT Refundable?")]
        public bool VAT_Liftable { get; set; }
        [NotMapped]
        public IList<string>? pictures { get; set; }
        [NotMapped]
        public IList<string>? pdf2s { get; set; }
        public DateTime? InvoiceDate { get; set; }
        [Display(Name = "Paid Amount Initial Price")]
        public double? PaidAmount { get; set; }
        [Display(Name = "Paid Amount of Rental")]
        public double? PaidAmountRental { get; set; }
    }
}
