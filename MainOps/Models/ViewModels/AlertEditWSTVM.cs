using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ViewModels
{
    public class AlertEditWSTVM
    {
        [Display(Name = "Water Sample Type")]
        public int WaterSampleTypeId { get; set; }
        public virtual WaterSampleType WaterSampleType { get; set; }
        public int WaterSamplePlaceId { get; set; }
        public virtual WaterSamplePlace WaterSamplePlace { get; set; }
        [Display(Name = "Max Limit")]
        public double? Limit { get; set; }
        [Display(Name = "Mean Limit")]
        public double? MeanLimit { get; set; }
        public AlertEditWSTVM()
        {

        }
        public AlertEditWSTVM(WaterSampleLimit limit)
        {
            WaterSamplePlaceId = limit.WaterSamplePlaceId;
            WaterSampleTypeId = limit.WaterSampleTypeId;
            WaterSamplePlace = limit.WaterSamplePlace;
            WaterSampleType = limit.WaterSampleType;
            Limit = limit.Limit;
            MeanLimit = limit.MeanLimit;
        }
    }
}
