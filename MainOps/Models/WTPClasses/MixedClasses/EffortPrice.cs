using MainOps.Models.WTPClasses.MainClasses;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MainOps.Models.WTPClasses.MixedClasses
{
    public class EffortPrice
    {
        [Key]
        public Effort effort { get; set; }
        public List<Price> px { get; set; }
        public Price pxx { get; set; }
        public EffortPrice()
        {
            px = new List<Price>();
        }
        public EffortPrice(Effort e)
        {
            effort = e;
            px = new List<Price>();
        }
        public EffortPrice(Effort e, List<Price> p)
        {
            effort = e;
            px = p;
        }
    }
}
