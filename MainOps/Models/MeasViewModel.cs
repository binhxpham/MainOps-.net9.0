using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class MeasViewModel
    {
        public int MeasId { get; set; }
        public virtual Meas Meas { get; set; }
        public int OffsetId { get; set; }
        public virtual Offset Offset { get; set; }
        [Display(Name = "Meas in Reference level")]
        public double CalculatedMeas
        {
            get
            {
                if(Meas.TheMeasurement != null)
                {
                    return Offset.offset - Convert.ToDouble(Meas.TheMeasurement);
                }
                else
                {
                    try { 
                    return Convert.ToDouble(Meas.TheMeasurement);
                    }
                    catch
                    {
                        return 0.0;
                    }
                }
            }
        }
        public MeasViewModel()
        {

        }
        public MeasViewModel(Meas m, Offset o)
        {
            MeasId = m.Id;
            OffsetId = o.Id;
            Meas = m;
            Offset = o;
        }
    }
}
