using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class SystemCheck
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Time of Check")]
        public DateTime? TimeChecked { get; set; }
        [Display(Name = "Check Done By")]
        public string DoneBy { get; set; }
        [Display(Name = "Department")]
        public int? DivisionId { get; set; }
        [Display(Name = "Department")]
        public virtual Division Division { get; set; }
        public string Comments { get; set; }
    }
}
