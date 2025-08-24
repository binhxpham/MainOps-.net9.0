using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ReportClasses
{
    public class Machinery
    {
        [Key]
        public int Id { get; set;}
        [Display(Name = "Machinery")]
        public string MachineryName { get; set; }
        public int DivisionId { get; set; }
        public virtual Division Division { get; set; }
    }
}
