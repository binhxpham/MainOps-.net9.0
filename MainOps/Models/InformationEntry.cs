using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class InformationEntry
    {
        [Key]
        public int Id { get; set; }
        [Display(Name ="Text")]
        public string Text { get; set; }
        [Display(Name = "Entry Date")]
        public DateTime DateEntered { get; set; }
        [Display(Name ="Done By")]
        public string DoneBy { get; set; }
        [Display(Name = "Division")]
        public int DivisionId { get; set; }
        public virtual Division Division { get; set; }
    }
}
