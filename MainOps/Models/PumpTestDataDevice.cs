using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class PumpTestDataDevice
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("ThreeStepTest")]
        public int? ThreeStepTestId { get; set; }
        public virtual ThreeStepTest ThreeStepTest { get; set; }
        public DateTime TimeStamp { get; set; }
        public double? PumpLevelData { get; set; }
        public double? Moni1LevelData { get; set; }
        public double? Moni2LevelData { get; set; }
        public double? Moni3LevelData { get; set; }
        public double? Moni4LevelData { get; set; }
        public double? FlowData { get; set; }

    }
    public class ClearPumpTestDataDevice
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("ClearPumpTest")]
        public int? ClearPumpTestId { get; set; }
        public virtual ClearPumpTest ClearPumpTest { get; set; }
        public DateTime TimeStamp { get; set; }
        public double? PumpLevelData { get; set; }
        public double? Moni1LevelData { get; set; }
        public double? Moni2LevelData { get; set; }
        public double? FlowData { get; set; }

    }
    
}
