using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class ConstructionSiteInspection
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Project")]
        public int ProjectId { get; set; }
        [Display(Name = "Project")]
        public virtual Project? Project { get; set; }
        [Display(Name = "Sub Project")]
        public int? SubProjectId { get; set; }
        [Display(Name = "Sub Project")]
        public virtual SubProject? SubProject { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Report Done By")]
        public string? DoneBy { get; set; }
        [Display(Name = "Signature")]
        public string? Signature { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        [Display(Name = "Entered into Database")]
        public DateTime? EnteredIntoDataBase { get; set; }
        public ICollection<PhotoFileConstructionSiteInspection>? Photos { get; set; }
        [Display(Name = "Branch")]
        public string? Branch { get; set; }
        [Display(Name = "Number of own people")]
        public int NumberOfPeople { get; set; }
        [Display(Name = "WPA (Work Place Assesment) Performed?")]
        public bool? WPA_Done { get; set; }
        [Display(Name = "Tool box performed")]
        public bool? ToolBox_Done { get; set; }
        [Display(Name = "Is there particularly hazardous work to be performed?")]
        public bool? Hazardous_Work { get; set; }
        [Display(Name = "State of overall construction site ok?")]
        public bool? OverAllState { get; set; }
        [Display(Name = "Walkway and roads kept clear")]
        public bool? Walkways { get; set; }
        [Display(Name = "Conditions of ladders/steps/platforms ok?")]
        public bool? LaddersStepsPlatforms { get; set; }
        [Display(Name = "Sufficient number of first aiders on site?")]
        public bool? FirstAiders { get; set; }
        [Display(Name = "Correct and safe storage of materials")]
        public bool? SafeStorage { get; set; }
        [Display(Name = "Known contaminations")]
        public bool? KnownContaminants { get; set; }
        [Display(Name = "Emergency measures / facillities (first aid kit fire extinguishers, etc.) ok?")]
        public bool? EmergencyMeasures { get; set; }
        [Display(Name = "Unknown contaminations")]
        public bool? UnKnownContaminants { get; set; }
        [Display(Name = "Sufficient lighting")]
        public bool? SufficientLighting { get; set; }
        [Display(Name = "Is the workplace tidy")]
        public bool? Tidy { get; set; }
        [Display(Name = "Temporary ground support systems in order?")]
        public bool? GroundSupport { get; set; }
        [Display(Name = "Floor openings?")]
        public bool? FloorOpenings { get; set; }
        [Display(Name = "Sloaping areas")]
        public bool? SloapingAreas { get; set; }
        [Display(Name = "Safe access to construction site possible?")]
        public bool? SafeAccessSite { get; set; }
        [Display(Name = "Waste disposal neccesary")]
        public bool? WasteDisposal { get; set; }
        [Display(Name = "Waste seperation incl. verfication")]
        public bool? WasteSeperation { get; set; }
        [Display(Name = "Measures to prevent environmental harm")]
        public bool? MeasuresPreventEnvHarm { get; set; }
        [Display(Name = "All normal personal protective equipment (helmet, ear plugs, reflective clothing, safety boots, gloves and glasses)")]
        public bool? AllPPE { get; set; }
        [Display(Name = "Special PPE required for this site")]
        public bool? SpecialPPE { get; set; }
        [Display(Name = "Operating instructions in order")]
        public bool? OperatingInstructions { get; set; }
        [Display(Name = "Safety data sheets")]
        public bool? SafetyDataSheets { get; set; }
        [Display(Name = "Oil binding agent present?")]
        public bool? OilBindingAgent { get; set; }
        [Display(Name = "Safety condition (Inspection plates on equipment) ok?")]
        public bool? SafetyCondition { get; set; }
        [Display(Name = "Marking of work devices ( equipment and load handling devices) ok?")]
        public bool? MarkingDevices { get; set; }
        [Display(Name = "Condition of load handling devices (e.g. chains, synthetic fibre webbing, strings etc.) ok?")]
        public bool? ConditionLoadHandling { get; set; }
        [Display(Name = "Personal Protection equipped?")]
        public bool? Sub_PPE { get; set; }
        [Display(Name = "Safety condition machinery / equipment ok?")]
        public bool? Sub_SafetyCond { get; set; }
        [Display(Name = "Material storage / tidiness in the workplace ok?")]
        public bool? Sub_MaterialStorage { get; set; }
        [Display(Name = "Other information / comments")]
        public string? GeneralComments { get; set; }
        [Display(Name = "Subcontractors used?")]
        public bool? SubContractorsUsed { get; set; }
        [Display(Name = "Involvement of Project manager neccesary?")]
        public bool? ProjectManager { get; set; }
        [Display(Name = "Additional instruction neccesary")]
        public bool? AdditionalInstruction { get; set; }
        [Display(Name = "Problems from previous sections rectified by person + deadline date")]
        public string? PreviousProblems { get; set; }

    }
}
