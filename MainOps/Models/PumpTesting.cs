using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class PumpTesting
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Name")]
        public string DoneBy { get; set; }
        [Display(Name = "Test Date")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Pump size")]
        [Required]
        [StringLength(64, MinimumLength = 1, ErrorMessage = "Please choose pumptype")]
        public string PumpType { get; set; }
        [Display(Name = "Pump Number")]
        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "Please write Pump ID")]
        public string PumpID { get; set; }
        [Display(Name = "Length of the cable")]
        public double CableLength { get; set; }
        [Display(Name = "Is cable damaged?")]
        public bool CableDamaged { get; set; }
        [Display(Name = "Which cable is damaged?")]
        public string WhichCableDamaged { get; set; }
        [Display(Name = "Can it be fixed?")]
        public bool CanBeFixed { get; set; }
        [Display(Name = "Complete exchange?")]
        public bool CompleteExchange { get; set; }
        [Display(Name = "Cable extended?")]
        public bool CableExtended { get; set; }
        [Display(Name = "Cable extended by (x meters)")]
        public double CableExtendedm { get; set; }
        [Display(Name = "Insulation tested?")]
        public bool InsulationTested { get; set; }
        [Display(Name = "Is insulation ok?")]
        public bool InsulationOK { get; set; }
        [Display(Name = "Is all cable submerged?")]
        public bool AllCableSubmerged { get; set; }
        [Display(Name = "Signature")]
        public string Signature { get; set; }
        public int? DivisionId { get; set; }
        public virtual Division Division { get; set; }
        public ICollection<PumptestingData> TestData { get; set; }
        public ICollection<PumptestingPhoto> photos { get; set; }


    }
    public class PumptestingPhoto
    {
        public int Id { get; set; }
        public string path { get; set; }
        public int? PumptestingId { get; set; }
        public virtual PumpTesting PumpTesting { get; set; }
    }
    public class PumptestingData
    {
        public int Id { get; set; }
        public int? PumpTestingId { get; set; }
        public virtual PumpTesting PumpTesting { get; set; }
        [Display(Name = "Pressure(bar)")]
        public double? Pressure { get; set; }
        [Display(Name = "Flow m3/h")]
        public double? Flow { get; set; }
        [Display(Name = "Duration test (minutes)")]
        public int? Duration { get; set; }
    }
    public class PumptestingVM
    {
        public PumpTesting test { get; set; }
        public List<PumptestingData> data { get; set; }
        public PumptestingVM()
        {
            this.data = new List<PumptestingData>();
            for(int i = 0; i < 10; i++)
            {
                PumptestingData d = new PumptestingData();
                this.data.Add(d);
            }
            this.test = new PumpTesting();
        }
        public PumptestingVM(PumpTesting pt,List<PumptestingData> dat)
        {
            this.test = pt;
            this.data = dat;
        }
    }
}
