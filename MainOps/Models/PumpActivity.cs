using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class PumpActivity
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("MeasPoint")]
        public int MeasPointId { get; set; }
        public virtual MeasPoint MeasPoint { get; set; }
        public DateTime Start_activity { get; set; }
        public DateTime? End_activity { get; set; }
        public PumpActivity()
        {

        }
        public PumpActivity(MeasPoint mp, DateTime startdate)
        {
            this.MeasPointId = mp.Id;
            this.Start_activity = startdate;
        }
    }
}
