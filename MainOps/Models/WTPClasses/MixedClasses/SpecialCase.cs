using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MainOps.Models.WTPClasses.MixedClasses
{
    public class SpecialCase
    {
        [Key]
        public int id { get; set; }
        public string cont_name { get; set; }
        public string new_filter { get; set; }
        public List<string> new_filters(string s)
        {
            var newfilts = s.Trim().Split(',');
            return newfilts.ToList();
        }
    }
}
