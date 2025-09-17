using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class CGJItemMasterClass
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Item Master Class")]
        public string ClassName { get; set; }
        public string ClassNumber { get; set; }
        public int? DivisionId { get; set; }
        public virtual Division Division { get; set; }
        public string NameAndNumber
        {
            get
            {
                return String.Concat(this.ClassNumber, " : ", this.ClassName);
            }
        }

    }
}
