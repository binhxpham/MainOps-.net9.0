using MainOps.Models.WTPClasses.MainClasses;
using MainOps.Models.WTPClasses.MixedClasses;
using System.Collections.Generic;

namespace MainOps.Models.WTPClasses.ViewModels
{
    public class VMMedia
    {
        public string Name { get; set; }
        public string contaminations { get; set; }
        public string device { get; set; }
        public int? water_typeid { get; set; }
        public List<MediaEfficiency> m_effs { get; set; }
        public MediaEfficiency m_efx { get; set; }
        public VMMedia()
        {
            m_efx = new MediaEfficiency();
            m_effs = new List<MediaEfficiency>();
        }
        public VMMedia(FilterMaterial f)
        {
            Name = f.Name;
            device = f.device;
            water_typeid = f.water_typeid;
            m_efx = new MediaEfficiency {filtermaterial = f};
            m_effs = new List<MediaEfficiency>();
        }
        public VMMedia(FilterMaterial f, List<MediaEfficiency> me)
        {
            Name = f.Name;
            device = f.device;
            water_typeid = f.water_typeid;
            m_efx = new MediaEfficiency {filtermaterial = f};
            m_effs = me;
            foreach (var m in me)
            {
                contaminations = string.Join(", ", contaminations, m.Contamination.Name);
            }
        }

    }
}
