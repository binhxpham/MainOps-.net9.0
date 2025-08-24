using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class Unit
    {
        [Key]
        public int Id { get; set; }
        public string? TheUnit { get; set; }
    }
}
