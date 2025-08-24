using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MainOps.Models.WTPClasses.HelperClasses;
using MainOps.Models.WTPClasses.MainClasses;
using MainOps.Models.WTPClasses.ViewModels;

namespace MainOps.Models.WTPClasses.MixedClasses
{
    public class MediaEfficiency
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Filtermaterial")]
        public int filtermaterialid { get; set; }
        [Display(Name = "Filtermaterial")]
        public virtual FilterMaterial filtermaterial { get; set; }
        [ForeignKey("Contamination")]
        public int? contaminationId { get; set; }
        [Display(Name = "Contamination")]
        public virtual Contamination Contamination { get; set; }
        [Required]
        [Display(Name = "Efficiency")]
        public sbyte efficiency { get; set; }
        [ForeignKey("Dosing")]
        [Display(Name = "Dosing")]
        public int? dosing_ofid { get; set; }
        [Required]
        [Display(Name = "Need of Dosing")]
        public bool need_dosing { get; set; }
        public virtual Dosing dosing_of { get; set; }
        [Display(Name = "Dosing Relation")]
        public double dosing_relation { get; set; }
        [Required]
        [Display(Name = "Need of Aeration")]
        public bool need_Aeration { get; set; }
        [Display(Name = "Lower Limit Aeration")]
        public double lower_limit_aeration { get; set; }
        [Display(Name = "Upper Limit Aeration")]
        public double upper_limit_aeration { get; set; }
        [Required]
        [Display(Name = "Need of pH Control")]
        public bool need_pH_control { get; set; }
        [Display(Name = "Lower pH Limit")]
        public double lower_limit_pH { get; set; }
        [Display(Name = "Upper pH Limit")]
        public double upper_limit_pH { get; set; }
        [Required]
        [Display(Name = "Concentration Effect")]
        public bool has_concentration_effect { get; set; }
        [ForeignKey("Effect_type")]
        public int? effect_typeid { get; set; }
        [Display(Name = "Effect Type")]
        public virtual Effect_type effect_type { get; set; }
        public MediaEfficiency()
        {

        }
        public MediaEfficiency(FilterMaterial f, Contamination c)
        {
            filtermaterialid = f.id;
            efficiency = 0;
            contaminationId = c.Id;
            need_Aeration = false;
            need_dosing = false;
            need_pH_control = false;
            has_concentration_effect = false;
            lower_limit_aeration = 0;
            lower_limit_pH = 0;
            upper_limit_aeration = 0;
            upper_limit_pH = 0;
            dosing_ofid = null;
            dosing_relation = 0;
            effect_typeid = null;
        }
        public MediaEfficiency(VMMedia vm)
        {
            filtermaterialid = vm.m_efx.filtermaterialid;
            contaminationId = vm.m_efx.contaminationId;
            efficiency = vm.m_efx.efficiency;
            need_dosing = vm.m_efx.need_dosing;
            dosing_ofid = vm.m_efx.dosing_ofid;
            dosing_relation = vm.m_efx.dosing_relation;
            need_Aeration = vm.m_efx.need_pH_control;
            lower_limit_aeration = vm.m_efx.lower_limit_aeration;
            upper_limit_aeration = vm.m_efx.upper_limit_aeration;
            need_pH_control = vm.m_efx.need_pH_control;
            lower_limit_pH = vm.m_efx.lower_limit_pH;
            upper_limit_pH = vm.m_efx.upper_limit_pH;
            has_concentration_effect = vm.m_efx.has_concentration_effect;
            effect_typeid = vm.m_efx.effect_typeid;
        }
        public MediaEfficiency(VMMediaTwo vm)
        {
            filtermaterialid = vm.m_efx.filtermaterialid;
            contaminationId = vm.m_efx.contaminationId;
            efficiency = vm.m_efx.efficiency;
            need_dosing = vm.m_efx.need_dosing;
            dosing_ofid = vm.m_efx.dosing_ofid;
            dosing_relation = vm.m_efx.dosing_relation;
            need_Aeration = vm.m_efx.need_pH_control;
            lower_limit_aeration = vm.m_efx.lower_limit_aeration;
            upper_limit_aeration = vm.m_efx.upper_limit_aeration;
            need_pH_control = vm.m_efx.need_pH_control;
            lower_limit_pH = vm.m_efx.lower_limit_pH;
            upper_limit_pH = vm.m_efx.upper_limit_pH;
            has_concentration_effect = vm.m_efx.has_concentration_effect;
            effect_typeid = vm.m_efx.effect_typeid;
        }
    }
}
