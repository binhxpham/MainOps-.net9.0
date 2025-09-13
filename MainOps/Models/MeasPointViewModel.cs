using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class MeasPointViewModel
    {
        [ForeignKey("MeasPoint")]
        public int MeasPointId { get; set; }
        public virtual MeasPoint? MeasPoint { get; set; }
        [ForeignKey("Offset")]
        public int? OffsetId { get; set; }
        public virtual Offset? Offset { get; set; }
        public MeasPointViewModel()
        {

        }
        public MeasPointViewModel(MeasPoint mp)
        {
            MeasPointId = mp.Id;
            MeasPoint = mp;
        }
        //public MeasPointViewModel(MeasPoint mp,Offset o)
        //{
        //    MeasPointId = mp.Id;
        //    MeasPoint = mp;
        //    OffsetId = o.Id;
        //    Offset = o;
        //}

        public MeasPointViewModel(MeasPoint mp, Offset? o)
        {
            MeasPointId = mp.Id;
            MeasPoint = mp;

            if (o != null)
            {
                OffsetId = o.Id;
                Offset = o;
            }
            else
            {
                OffsetId = 0; // or -1, depending on your logic
                Offset = null!;
            }
        }


    }
}
