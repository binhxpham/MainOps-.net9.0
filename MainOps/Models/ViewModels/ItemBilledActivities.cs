using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ViewModels
{
    public class ItemBilledActivities
    {
        public List<Install> Installations { get; set; }
        public List<Arrival> Rentals { get; set; }
        public List<Well> Drillings { get; set; }
        public List<WellCheck> WellChecks { get; set; }
        public List<Airlift> Airlifts { get; set; }
        public List<ClearPumpTest> ClearPumpTests { get; set; }
        public List<Install> WaterSamples { get; set; }
        public List<Install> PipeWorks { get; set; }
        public List<Install> TrafficDivs { get; set; }
        public List<Decommission> Decommissions { get; set; }
        
        public List<PreExcavation> PreExcavations { get; set; }
        public ItemBilledActivities()
        {
            Installations = new List<Install>();
            Rentals = new List<Arrival>();
            WellChecks = new List<WellCheck>();
            Airlifts = new List<Airlift>();
            ClearPumpTests = new List<ClearPumpTest>();
            WaterSamples = new List<Install>();
            PreExcavations = new List<PreExcavation>();
            PipeWorks = new List<Install>();
            TrafficDivs = new List<Install>();
            Drillings = new List<Well>();
            Decommissions = new List<Decommission>();
        }
    }
}
