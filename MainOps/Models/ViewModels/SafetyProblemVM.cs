using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ViewModels
{
    public class SafetyProblemVM
    {
        public string LogText { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime TimeStamp { get; set; }
        public int? ProjectId { get; set; }
        public string safetyproblemtype { get; set; }
    }
}
