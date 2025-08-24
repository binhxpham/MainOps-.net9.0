using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class Logger
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Logger Number")]
        public int LoggerNo { get; set; }
        [Display(Name = "Serial Number")]
        public string? SerialNo { get; set; }
        [Display(Name = "Sim Card Number")]
        public string? SimCardNo { get; set; }
        [ForeignKey("Division")]
        public int? DivisionId { get; set; }
        public virtual Division? Division {get;set;}
    }
}
