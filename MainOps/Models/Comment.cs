using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [StringLength(128, MinimumLength = 2)]
        [Display(Name = "Comment")]
        public string comment { get; set; }
    }
}
