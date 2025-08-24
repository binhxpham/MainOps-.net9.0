using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ViewModels
{
    public class AlertEditVM
    {
        [Display(Name = "MeasPoint")]
        public int MeasPointId { get; set; }
        public virtual MeasPoint MeasPoint { get; set; }
        [Display(Name = "Lower Limit")]
        public double? Lower_Lower_Limit { get; set; }
        [Display(Name = "Middle Lower Limit")]
        public double? Middle_Lower_Limit { get; set; }
        [Display(Name = "Middle Upper Limit")]
        public double? Middle_Upper_Limit { get; set; }
        [Display(Name = "Upper Limit")]
        public double? Upper_Upper_Limit { get; set; }
        [Display(Name = "Lower Limit Flowrate")]
        public double? Flow_Lower_Lower_Limit { get; set; }
        [Display(Name = "Middle Lower Limit Flowrate")]
        public double? Flow_Middle_Lower_Limit { get; set; }
        [Display(Name = "Middle Upper Limit Flowrate")]
        public double? Flow_Middle_Upper_Limit { get; set; }
        [Display(Name = "Upper Limit Flowrate")]
        public double? Flow_Upper_Upper_Limit { get; set; }
        public AlertEditVM()
        {

        }
        public AlertEditVM(MeasPoint measPoint)
        {
            MeasPointId = measPoint.Id;
            MeasPoint = measPoint;
            Lower_Lower_Limit = measPoint.Lower_Lower_Limit;
            Middle_Lower_Limit = measPoint.Middle_Lower_Limit;
            Middle_Upper_Limit = measPoint.Middle_Upper_Limit;
            Upper_Upper_Limit = measPoint.Upper_Upper_Limit;
            Flow_Lower_Lower_Limit = measPoint.Flow_Lower_Lower_Limit;
            Flow_Middle_Lower_Limit = measPoint.Flow_Middle_Lower_Limit;
            Flow_Middle_Upper_Limit = measPoint.Flow_Middle_Upper_Limit;
            Flow_Upper_Upper_Limit = measPoint.Flow_Upper_Upper_Limit;
        }
    }
}
