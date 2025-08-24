using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class MeasType
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }
        [Required]
        [StringLength(128, MinimumLength = 2)]
        public string? Type { get; set; }
        [ForeignKey("Divisions")]
        public int? DivisionId { get; set; }
        public virtual Division? Division { get; set; }
        public MeasType()
        {

        }
        public MeasType(MeasType mt)
        {
            this.Type = mt.Type;
        }
        public MeasType(MeasType mt, Division div)
        {
            this.Type = mt.Type;
            this.DivisionId = div.Id;
        }
    }
}
