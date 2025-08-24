using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ViewModels
{
    public class PumpTestDataUploadVM
    {
        public int ThreeStepTestId { get; set; }
        public bool WasysData { get; set; }
        public PumpTestDataUploadVM()
        {

        }
    }
    public class GroutTestDataUploadVM
    {
        public int GroutingId { get; set; }
        public GroutTestDataUploadVM()
        {

        }
    }
    public class AcidDataUploadVM
    {
        public int AcidTreatmentId { get; set; }
        public AcidDataUploadVM()
        {

        }
    }
}
