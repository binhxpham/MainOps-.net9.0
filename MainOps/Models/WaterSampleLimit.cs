using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class WaterSampleLimit
    {
        public int Id { get; set; }
        public int WaterSamplePlaceId { get; set; }
        public virtual WaterSamplePlace WaterSamplePlace {get; set;}
        public int WaterSampleTypeId { get; set; }
        public virtual WaterSampleType WaterSampleType { get; set; }
        public double? Limit { get; set; }
        public double? MeanLimit { get; set; }

    }
}
