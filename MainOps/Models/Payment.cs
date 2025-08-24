using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Payment ID")]
        public string PaymentID { get; set; }
        [Display(Name = "Payment Amount")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal Amount { get; set; }
        [Display(Name = "Payed On")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime PayDate { get; set; }
        [Display(Name = "Applied Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime TimeStamp { get; set; }       
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        [Display(Name = "Project")]
        public virtual Project Project { get; set; }
        [Display(Name = "SubProject")]
        public int? SubProjectId { get; set; }
        [Display(Name = "SubProject")]
        public virtual SubProject SubProject { get; set; }
        public bool Taxes { get; set; }
        public Payment()
        {

        }
    }
    public class Invoice
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Invoice ID")]
        public string InvoiceID { get; set; }
        [Display(Name = "Invoice Amount")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal Amount { get; set; }
        [Display(Name = "Invoiced On")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime InvoiceDate { get; set; }
        [Display(Name = "Applied Date Start")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime TimeStampStart { get; set; }
        [Display(Name = "Applied Date End")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime TimeStampEnd { get; set; }
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        [Display(Name = "Project")]
        public virtual Project Project { get; set; }
        [Display(Name = "SubProject")]
        public int? SubProjectId { get; set; }
        [Display(Name = "SubProject")]
        public virtual SubProject SubProject { get; set; }
        public bool Taxes { get; set; }
        [Display(Name = "Surcharge")]
        public decimal Surchage { get; set; }
        [Display(Name = "VAT %")]
        public decimal TaxesPercent { get; set; }
        public decimal InvoicableAmount { get
            {
                if(this.Surchage > (decimal)0.01 )
                {
                    if (this.Taxes.Equals(true) && this.TaxesPercent > (decimal)0.01)
                    {
                        return this.Amount * (1 + this.Surchage) * (1 + this.TaxesPercent);
                    }
                    else
                    {
                        return this.Amount * (1 + this.Surchage);
                    }
                }
                else
                {
                    return this.Amount;
                }
                
            } 
        }
        public Invoice()
        {

        }
    }
}
