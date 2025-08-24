using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class HIHItem
    {
        [Key]
        public int Id { get; set; }
        public int DivisionId { get; set; }
        public virtual Division Division { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }
        [NotMapped]
        public List<string> documentpaths { get; set; }        
        public int? ItemTypeId { get; set; }
        public virtual ItemType ItemType { get; set; }
    }
}
