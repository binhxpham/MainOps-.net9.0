using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class PhotoDocumenation
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public int? SubProjectId { get; set; }
        public virtual SubProject SubProject { get; set; }
        
        public string Description { get; set; }
        public ICollection<PhotoDoc> Photos { get; set; }
        public DateTime TimeStamp { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Accuracy { get; set; }
    }
    public class PhotoDoc
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public int? PhotoDocumentationId { get; set; }
        public virtual PhotoDocumenation PhotoDocumentation { get; set; }
    }
}
