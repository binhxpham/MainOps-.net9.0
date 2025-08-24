using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class DataPoint
    {
        [Key]
        public int Id { get; set; }
        public DateTime Datum { get; set; }
        public double MessWert { get; set; }
        [ForeignKey("MeasPoint")]
        public int? MeasPointId { get; set; }
        public virtual MeasPoint MeasPoint { get; set; }
    }
}
