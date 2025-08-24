using MainOps.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class SafetyProblem
    {
        [Key]
        public int Id { get; set; }
        public string LogText { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime TimeStamp { get; set; }
        [NotMapped]
        public IList<string> pictures { get; set; }
        public string DoneBy { get; set; }
        [ForeignKey("Project")]
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public string safetyproblemtype { get; set; }
        public SafetyProblem()
        {

        }
        public SafetyProblem(SafetyProblemVM vm, ApplicationUser user)
        {
            this.LogText = vm.LogText;
            this.Latitude = Convert.ToDouble(vm.Latitude);
            this.Longitude = Convert.ToDouble(vm.Longitude);
            this.TimeStamp = vm.TimeStamp;
            this.DoneBy = user.full_name();
            this.ProjectId = vm.ProjectId;
            this.safetyproblemtype = vm.safetyproblemtype;
        }
    }
}
