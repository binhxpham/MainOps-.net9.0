using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ViewModels
{
    public class DipListVM
    {
        public MeasPoint MeasPoint { get; set; }
        public Offset Offset { get; set; }
        public Meas LastMeas { get; set; }
    }
}
