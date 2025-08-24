using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class Discount_Installation
    {
        [Key]
        public int Id { get; set; }
        public int? ItemTypeId { get; set; }
        public virtual ItemType ItemType { get; set; }
        public decimal Rate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Total_Discount2(DateTime date, decimal price,double amount)
        {
            if(date >= StartDate.Date)
            {
                if(this.EndDate != null)
                {
                    if(date <= Convert.ToDateTime(this.EndDate).Date)
                    {
                        return price * Convert.ToDecimal(amount) * Rate;
                    }
                    else
                    {
                        return (decimal)0.00;
                    }
                }
                else
                {
                    return price * Rate;
                }

            }
            else
            {
                return (decimal)0.00;
            }

        }
        public Discount_Installation()
        {

        }
        public Discount_Installation(ItemType it)
        {
            this.ItemType = it;
            this.ItemTypeId = it.Id;
        }
    }
    public class Discount
    {
        [Key]
        public int Id { get; set; }
        public int? ItemTypeId { get; set; }
        public virtual ItemType ItemType { get; set; }
        public decimal Rate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Total_Discount2(DateTime start,DateTime end,decimal rent,double amount)
        {
            decimal Total = (decimal)0.0;
            if(this.EndDate != null)
            {
                if(end > this.EndDate)
                {
                    end = Convert.ToDateTime(this.EndDate);
                }
            }
            if(start < this.StartDate)
            {
                start = this.StartDate;
            }
            for (DateTime dt = start.Date; dt <= end.Date; dt = dt.AddDays(1).Date)
            {
                Total += rent * this.Rate * (decimal)amount;
            }
            return Total;
        }
        public decimal Total_Discount(DateTime start,DateTime? end, decimal rent,double amount)
        {
            decimal Total = (decimal)0.0;
            if (this.StartDate < start)
            {
                this.StartDate = start;
            }
            if (this.EndDate == null)
            {
                this.EndDate = end;
            }
            else if (this.EndDate > end)
            {
                this.EndDate = end;
            }
            for (DateTime dt = this.StartDate.Date; dt <= Convert.ToDateTime(this.EndDate).Date; dt = dt.AddDays(1).Date)
            {
                Total += rent * this.Rate * (decimal)amount;
            }
            return Total;
        }
    }
}
