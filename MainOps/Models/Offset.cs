using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class Offset
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Offset")]
        public double offset { get; set; }
        [Required]
        [ForeignKey("MeasPoint")]
        [Display(Name = "Measurement Point ID")]
        public int MeasPointId { get; set; }
        [Display(Name = "Measurement Point")]
        public MeasPoint? measpoint { get; set; }
        [Required]
        [Display(Name = "Start Date")]
        public DateTime starttime { get; set; }


        [Display(Name = "DoneBy")]
        public string? DoneBy { get; set; }
        [Display(Name = "Comment")]
        public string? Comment { get; set; }
        public Offset() { }
    }

}
