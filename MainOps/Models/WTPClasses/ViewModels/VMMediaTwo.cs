
using MainOps.Models.WTPClasses.MainClasses;
using MainOps.Models.WTPClasses.MixedClasses;
using System.Collections.Generic;

namespace MainOps.Models.WTPClasses.ViewModels
{
    public class VMMediaTwo
    {
        public string Name { get; set; }
        public string contam_group { get; set; }
        public double? default_limit { get; set; }
        public int unit_limitid { get; set; }
        public List<MediaEfficiency> m_effs { get; set; }
        public MediaEfficiency m_efx { get; set; }
        public VMMediaTwo()
        {
            m_efx = new MediaEfficiency();
            m_effs = new List<MediaEfficiency>();
        }
        public VMMediaTwo(Contamination c)
        {
            Name = c.Name;
            contam_group = c.contam_group;
            default_limit = c.default_limit;
            unit_limitid = c.Unit_limitid;

            m_efx = new MediaEfficiency {contaminationId = c.Id};
            m_effs = new List<MediaEfficiency>();
        }
        public VMMediaTwo(Contamination c, List<MediaEfficiency> me)
        {
            Name = c.Name;
            contam_group = c.contam_group;
            default_limit = c.default_limit;
            unit_limitid = c.Unit_limitid;
            m_efx = new MediaEfficiency {contaminationId = c.Id};
            m_effs = me;
        }

    }
}
