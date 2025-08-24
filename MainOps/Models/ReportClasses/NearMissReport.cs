using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ReportClasses
{
    public class AccidentReport 
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Project")]
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        [Display(Name = "Project")]
        public virtual Project? Project { get; set; }
        public int SafetyProblemId { get; set; }
        public virtual SafetyProblem? SafetyProblem { get; set; }
        [Display(Name = "Accident With Absence?")]
        public bool AccidentWithAbsence { get; set; }
        [Display(Name = "Unsafe Conditions?")]
        public bool UnsafeConditions { get; set; }
        [Display(Name = "Accident Without Absence?")]
        public bool AccidentWithoutAbsence { get; set; }
        [Display(Name = "Environmental Accident?")]
        public bool EnvironmentAccident { get; set; }
        [Display(Name = "Inuction or Prohibition?")]
        public string? InjuctionProhibition { get; set; }
        [Display(Name = "Near Miss?")]
        public bool NearMiss { get; set; }
        [Display(Name = "HJ or Subcontractor?")]
        public string? HJSubContractor { get; set; }
        [Display(Name = "Other")]
        public bool Other { get; set; }
        [Display(Name = "What Happened?")]
        public string? WhatHappened { get; set; }
        [Display(Name = "Name of worker")]
        public string? NameOfInjured { get; set; }
        [Display(Name = "Where and When")]
        public string? WhereAndWhen { get; set; }
        [Display(Name = "Names of witnesses")]
        public string? NameofWitnesses { get; set; }
        [Display(Name = "Any injuries?")]
        public string? WhatInjury { get; set; }
        [Display(Name = "What was done to resolve the situation")]
        public string? WhatWasDoneToFix { get; set; }
        [Display(Name = "What influenced the accident")]
        public string? WhatCausedAccident { get; set; }
        [Display(Name = "How could it happen")]
        public string? HowCouldItHappen { get; set; }
        [Display(Name = "Was the safety process for the task in order")]
        public bool WasSafetyProcessInOrder { get; set; }
        [Display(Name = "What is being done to prevent future situations")]
        public string? WhatIsDoneToPrevent { get; set; }
        [Display(Name = "Cause for change in APV?")]
        public bool CauseForChangeInAPV { get; set; }
        [Display(Name = "Lessons learned")]
        public string? WhatCanBeLearned { get; set; }
        [Display(Name = "Is a light job offered to the injured")]
        public bool IsOfferedLightJob { get; set; }
        [Display(Name = "What light job")]
        public string? WhatLightJob { get; set; }
        [Display(Name = "Start of light job")]
        public DateTime? StartLightJob { get; set; }
        [Display(Name = "Name of reportee")]
        public string? DoneBy { get; set; }
        public AccidentReport()
        {
            AccidentWithAbsence = false;
            AccidentWithoutAbsence = true;
            EnvironmentAccident = false;
        }
        public AccidentReport(SafetyProblem SP)
        {
            SafetyProblemId = SP.Id;
            SafetyProblem = SP;
            ProjectId = Convert.ToInt32(SP.ProjectId);
            if(SP.safetyproblemtype.Equals("Near Miss"))
            {
                NearMiss = true;
                AccidentWithAbsence = false;
                AccidentWithoutAbsence = false;
                EnvironmentAccident = false;
                Other = false;

            }
            else if (SP.safetyproblemtype.Equals("Accident"))
            {
                NearMiss = false;
                AccidentWithAbsence = false;
                AccidentWithoutAbsence = true;
                EnvironmentAccident = false;
                Other = false;
            }
            else if (SP.safetyproblemtype.Equals("Environment Accident"))
            {
                NearMiss = false;
                AccidentWithAbsence = false;
                AccidentWithoutAbsence = false;
                EnvironmentAccident = true;
                Other = false;
            }
            else
            {
                if(SP.safetyproblemtype.Equals("Unsafe Conditions"))
                {
                    NearMiss = false;
                    AccidentWithAbsence = false;
                    AccidentWithoutAbsence = false;
                    EnvironmentAccident = false;
                    Other = true;
                    UnsafeConditions = true;
                    
                }
                
            }
            WhatHappened = SP.LogText;
            DoneBy = SP.DoneBy;
            if(SP.Latitude > 1)
            {
                WhereAndWhen = SP.Project.Name + " : Coords: {" + SP.Latitude.ToString() + ";" + SP.Longitude.ToString() + "}" + " " + SP.TimeStamp.ToString("yyyy-MM-dd hh:mm");
            }
            else
            {
                WhereAndWhen = SP.Project.Name +" : " + SP.TimeStamp.ToString("yyyy-MM-dd hh:mm");
            }
        }
    }
    
}
