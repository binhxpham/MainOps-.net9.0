using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ViewModels
{
    public class UploadDataMeasPointVM
    {
        public int MeasPointId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        [Display(Name = "Comment Added To Existing Comments")]
        public string CommentToAdd { get; set; }
    }
}
