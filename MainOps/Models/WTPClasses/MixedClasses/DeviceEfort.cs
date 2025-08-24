using MainOps.Models.WTPClasses.MainClasses;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MainOps.Models.WTPClasses.MixedClasses
{
    public class DeviceEffort
    {
        [Key]
        public int wtp_block_id { get; set; }
        [Required]
        [Display(Name="Name")]
        public string wtp_block_name { get; set; }
        [Display(Name="Name")]
        public string wtp_block_name_ger { get; set; }
        [Required]
        public double wtp_block_size { get; set; }
        public List<Effort> effortslist { get; set; }
        public Effort efx { get; set; }
        public DeviceEffort()
        {
            effortslist = new List<Effort>();
        }
        public DeviceEffort(WTP_block b)
        {
            wtp_block_id = b.id;
            wtp_block_name = b.name;
            wtp_block_size = b.size;
            effortslist = new List<Effort>();
        }
        public DeviceEffort(WTP_block b, List<Effort> efforts, bool shop)
        {
            wtp_block_id = b.id;
            wtp_block_name = b.name;
            wtp_block_size = b.size;
            if (shop)
            {
                effortslist = efforts.Where(x => x.Category.category.ToLower().Equals("materials")).ToList();
            }
            else
            {
                effortslist = efforts;
            }
        }
    }
}
