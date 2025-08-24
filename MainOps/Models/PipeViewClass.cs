using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class PipeViewClass
    {
        public int? ProjectId { get; set; }
        public List<double> Latitudes { get; set; }
        public List<double> Longitudes { get; set; }
        public List<string> KMPoints { get; set; }
        public PipeViewClass()
        {
            Latitudes = new List<double>();
            Longitudes = new List<double>();
            KMPoints = new List<string>();
        }
        public PipeViewClass(int ProjectId,List<double> lats, List<double> longs)
        {
            this.ProjectId = ProjectId;
            this.Latitudes = lats;
            this.Longitudes = longs;
        }
    }
}
