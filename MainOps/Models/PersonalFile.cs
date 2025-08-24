using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class PersonalFile
    {
        [Key]
        public int Id { get; set; }
        public string path { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public bool Downloaded { get; set; }
        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId {get;set;}
        public virtual ApplicationUser TheUser { get; set; }
    }
}
