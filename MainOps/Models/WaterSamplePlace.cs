using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class WaterSamplePlace
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        public int? WaterSamplePlaceTypeId { get; set; }
        public virtual WaterSamplePlaceType? WaterSamplePlaceType { get; set; }
    }
    public class WaterSamplePlaceType
    {
        public int Id { get; set; }
        public string? Type { get; set; }
    }
    public class WaterSampleStandardLimit
    {
        public int Id { get; set; }
        public int WaterSamplePlaceTypeId { get; set; }
        public virtual WaterSamplePlaceType? WaterSamplePlaceType { get; set; }
        public double? MaxLimit { get; set; }
        public double? MeanLimit { get; set; }
        public string? Komponent { get; set; }
    }
}
