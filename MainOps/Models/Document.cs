using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class Document
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string path { get; set; }
        [Required]
        [ForeignKey("DocumentType")]
        public int? DocumentTypeId { get; set; }
        public virtual DocumentType DocumentType {get; set;}
        public int? MeasPointId { get; set; }
        public virtual MeasPoint MeasPoint { get; set; }
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public int? SubProjectId { get; set; }
        public virtual SubProject SubProject { get; set; }
        public int? DivisionId { get; set; }
        public virtual Division Division { get; set; }
        public string ExternalUrl { get; set; }
    }
}
