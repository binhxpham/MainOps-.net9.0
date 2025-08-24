using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ViewModels
{
    public class ExportSamplesVM
    {
        public int? WaterSamplePlaceId { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
    }
}
