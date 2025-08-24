using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ViewModels
{
    public class MeasPointSeriesVM
    {
        public string PrelimName { get; set; }
        public int numDigits { get; set; }
        public int startnum { get; set; }
        public int endnum { get; set; }
        public int MonitorTypeId { get; set; }
        public int MeasTypeId { get; set; }
        public int ProjectId { get; set; }
        public int? SubProjectId { get; set; }
        public bool AddWM { get; set; }
        public bool AddFlow { get; set; }

    }
}
