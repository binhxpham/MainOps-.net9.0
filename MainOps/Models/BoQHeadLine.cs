using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class BoQHeadLine
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        [Display(Name = "BoQ Headline number")]
        public decimal BoQnum { get; set; }
        [Display(Name = "BoQ HeadLine")]
        public string HeadLine { get; set; }
        [Display(Name = "Type of Invoice items")]
        public string Type { get; set; }

        public ICollection<ExtraWorkBoQ> ExtraWorkBoQs { get; set; }
        public BoQHeadLine()
        {

        }
        public BoQHeadLine(BoQHeadLine boq, int projid)
        {
            this.ProjectId = projid;
            this.BoQnum = boq.BoQnum;
            this.HeadLine = boq.HeadLine;
            this.Type = boq.Type;
        }
    }
}
