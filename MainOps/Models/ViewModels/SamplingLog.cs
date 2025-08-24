using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ViewModels
{
    public class SamplingLog
    {
        public int? WaterSamplePlaceId { get; set; }
        public virtual WaterSamplePlace WaterSamplePlace { get; set; }
        public int WeekNumber { get; set; }
        public DateTime Dato { get; set; }
        public List<WaterSamplePackage> WaterSamplePackages { get; set; }
        public string SampleFileName { get; set; }
        public SamplingLog()
        {
            this.WaterSamplePackages = new List<WaterSamplePackage>();
        }
    }
}
