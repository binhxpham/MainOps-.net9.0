using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class Division
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? LogoPath { get; set; }
        public string? Currency { get; set; }
        public string? CurrencyGroupSeperator { get; set; }
        public string? CurrencyDecimalSeperator { get; set; }
        public virtual ICollection<ApplicationUser>? Users { get; set; }
        public virtual ICollection<Project>? Projects { get; set; }
        public virtual ICollection<Logger>? Loggers { get; set; }
        public virtual ICollection<Department>? Departments { get; set; }
        [EmailAddress]
        public string? HourSheetEmail { get; set; }
    }
}
