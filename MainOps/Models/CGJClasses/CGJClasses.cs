using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using MainOps.Data;
using Microsoft.EntityFrameworkCore;

namespace MainOps.Models.CGJClasses
{
    public class Dagsrapport
    {
        public int Id { get; set; }
        [Display(Name = "Undersag")]
        [ForeignKey("SubProject")]
        public int? SubProjectId { get; set; }
        [Display(Name = "Undersag")]
        public virtual SubProject? SubProject{get; set;}
        [Display(Name = "Sag")]
        [ForeignKey("Project")]
        public int? ProjectId { get; set; }
        [Display(Name = "Sag")]
        public Project? Project { get; set; }
        [NotMapped]
        public List<string>? Photos { get; set; }        
        [Display(Name = "Kontrakt nr")]
        public string? KontraktNr { get; set; }
        [Display(Name = "Kontrakt navn")]
        public string? Kontraktnavn { get; set; }        
        public DateTime Dato { get; set; }
        [Display(Name = "Udarbejdet af")]
        public string? UdarbjedetAf { get; set; }
        public string? Underskrift { get; set; }
        [Display(Name = "Rapport nr")]
        public int RapportNr { get; set; }
        public string? Ugedag { get; set; }
        public string? Afdeling { get; set; }
        public string? Lokation { get; set; }
        public string? Addresse { get; set; }
        [Display(Name = "Continues")]
        public bool ForSaetter { get; set; }
        [Display(Name = "Dagens vejr")]
        public string? DagensVejr { get; set; }
        public string? Vejrlig { get; set; }
        [Display(Name = "Uddybende noter")]
        public string? UddybendeNoter { get; set; }
        public ICollection<DagsRapport_TimeRegistrering>? Timer { get; set; }
        public ICollection<DagsRapport_EkstraArbejde>? Ekstraarbejder { get; set; }
        public ICollection<DagsRapport_KontrakArbejde>? Kontraktarbejder { get; set; }
        public Dagsrapport()
        {

        }
    }
    public class CGJDailyReportEditVM
    {
        public int DagsrapportId { get; set; }
        [Display(Name = "Kontrakt nr")]
        public string? Kontraktnr { get; set; }
        [Display(Name = "Kontrakt navn")]
        public string? Kontraktnavn { get; set; }
        [Display(Name = "Rapport nr")]
        public int RapportNr { get; set; }
        [Display(Name = "Underskrift")]
        public string? Signature { get; set; }
        public DateTime Dato { get; set; }
        public string? Vejret { get; set; }
        public string? Vejrlig { get; set; }
        [Display(Name = "Uddybende Noter")]
        public string? UddybendeNoter { get; set; }
        public string? Addresse { get; set; }
        public string? Afdeling { get; set; }
        public bool Fortsaetter { get; set; }
        public string? Lokation { get; set; }
        public IList<TimeRegistrering>? Timer { get; set; }
        public IList<EkstraArbejde>? Ekstraarbejder { get; set; }
        public IList<KontraktArbejde>? Kontraktarbejder { get; set; }
        public IList<EA>? EAs { get; set; }
        public CGJDailyReportEditVM()
        {

        }
        public CGJDailyReportEditVM(Dagsrapport d)
        {
            this.DagsrapportId = d.Id;
            this.Kontraktnr = d.KontraktNr;
            this.Kontraktnavn = d.Kontraktnavn;
            this.Dato = d.Dato;
            this.RapportNr = d.RapportNr;
            this.Signature = d.Underskrift;
            this.Vejret = d.DagensVejr;
            this.Vejrlig = d.Vejrlig;
            this.UddybendeNoter = d.UddybendeNoter;
            this.Addresse = d.Addresse;
            this.Afdeling = d.Afdeling;
            this.Fortsaetter = d.ForSaetter;
            this.Lokation = d.Lokation;
        }
    }
    public class CGJDailyReportVM
    {
        [Required(ErrorMessage = "Skal udfyldes")]
        public string? Kontraktnr { get; set; }
        [Required(ErrorMessage = "Skal udfyldes")]
        public string? Kontraktnavn { get; set; }
        [Required(ErrorMessage = "Skal udfyldes")]
        public DateTime Report_Date { get; set; }
        public int RapportNr { get; set; }
        [Required(ErrorMessage = "Skal udfyldes")]
        public string? Afdeling { get; set; }
        [Required(ErrorMessage = "Skal udfyldes")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Skriv Addresse")]
        public string? Addresse { get; set; }
        public bool Fortsaetter { get; set; }
        public string? Work_Performed { get; set; }
        public IList<string>? pictures { get; set; }
        public string? Signature { get; set; }
        public string? UddybendeNoter { get; set; }
        public int LocationId { get; set; }
        public virtual Location? Location { get; set; }
        [Required(ErrorMessage = "Skal udfyldes")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Skriv Vejret")]
        public string? Vejret { get; set; }
        public string? Vejrlig { get; set; }
        public IList<TimeRegistrering>? Timer { get; set; }
        public IList<EkstraArbejde>? Ekstraarbejder { get; set; }
        public IList<KontraktArbejde>? Kontraktarbejder { get; set; }
        public IList<EA>? EAs { get; set; }
        //tilføj arbejde
        public CGJDailyReportVM()
        {

        }
        
    }
    
    public class EA
    {
        public int Id { get; set; }
        [Display(Name = "Ekstraarbejde nr")]
        public string? EAnr { get; set; }
        [Display(Name = "Opgave")]
        public string? Opgave { get; set; }
        [Display(Name = "Sags nr")]
        public string? Sagsnummer { get; set; }
    }
    public class DagsRapport_KontrakArbejde
    {
        [Key]
        public int Id { get; set; }
        public int? DagsrapportId { get; set; }
        public virtual Dagsrapport?  Dagsrapport { get; set; }
        public string? EgetMateriel { get; set; }
        public string? UL_Firma { get; set; }
        public string? UL_Materiel { get; set; }
        public DagsRapport_KontrakArbejde()
        {

        }
        public DagsRapport_KontrakArbejde(Dagsrapport d, KontraktArbejde k)
        {
            this.DagsrapportId = d.Id;
            this.EgetMateriel = k.EgetMateriel;
            this.UL_Firma = k.UL_Firma;
            this.UL_Materiel = k.UL_Materiel;
        }
    }
    public class KontraktArbejde
    {
        public string? EgetMateriel { get; set; }
        public string? UL_Firma { get; set; }
        public string? UL_Materiel { get; set; }
        public KontraktArbejde(DagsRapport_KontrakArbejde k)
        {
            this.EgetMateriel = k.EgetMateriel;
            this.UL_Firma = k.UL_Firma;
            this.UL_Materiel = k.UL_Materiel;
        }
        public KontraktArbejde()
        {

        }
    }
    public class DagsRapport_EkstraArbejde
    {
        [Key]
        public int Id { get; set; }
        public int? DagsrapportId { get; set; }
        public virtual Dagsrapport? Dagsrapport { get; set; }
        [Display(Name = "EA nr.")]
        public string? EANr { get; set; }
        [Display(Name = "EA nr.")]
        public string? EANr_skrevet { get; set; }
        public int? EAId { get; set; }
        public virtual EA? EA { get; set; }
        public string? Opgave { get; set; }
        [Display(Name = "Eget Materiel og Materialer")]
        public string? Eget_Materiel { get; set; }
        [Display(Name = "UE firma, materiel, materialer, koersel mm.")]
        public string? UEFirma_Materiel { get; set; }
        public bool Fortsaetter { get; set; }
        public DagsRapport_EkstraArbejde()
        {

        }
        public DagsRapport_EkstraArbejde(Dagsrapport d,EkstraArbejde e, int? EAid)
        {
            this.DagsrapportId = d.Id;
            this.EANr = e.EANr;
            this.EANr_skrevet = e.EANr_skrevet;
            this.Eget_Materiel = e.Eget_Materiel;
            this.Fortsaetter = e.Fortsaetter;
            this.Opgave = e.Opgave;
            this.UEFirma_Materiel = e.UEFirma_Materiel;
            this.EAId = EAid;
        }
    }
    public class EkstraArbejde
    {
        [Display(Name = "EA nr.")]
        public string? EANr { get; set; }
        public string? EANr_skrevet { get; set; }
        public string? Opgave { get; set; }
        [Display(Name = "Eget Materiel og Materialer")]
        public string? Eget_Materiel { get; set; }
        [Display(Name = "UE firma, materiel, materialer, koersel mm.")]
        public string? UEFirma_Materiel { get; set; }
        public bool Fortsaetter { get; set; }
        public EkstraArbejde(DagsRapport_EkstraArbejde e)
        {
            this.EANr = e.EANr;
            this.EANr_skrevet = e.EANr_skrevet;
            this.Opgave = e.Opgave;
            this.Eget_Materiel = e.Eget_Materiel;
            this.UEFirma_Materiel = e.UEFirma_Materiel;
            this.Fortsaetter = e.Fortsaetter;
        }
        public EkstraArbejde()
        {

        }
        
    }
    public class DagsRapport_TimeRegistrering
    {
        [Key]
        public int Id { get; set; }
        public int? DagsrapportId { get; set; }
        public virtual Dagsrapport? Dagsrapport { get; set; }
        public string? Title { get; set; }
        public int AntalPersoner { get; set; }
        public double Timer { get; set; }
        public double TimerFoerOvertid { get; set; }
        public double Overtid_50 { get; set; }
        public double Overtid_100 { get; set; }
        public double CGJ_Timer_Total { get; set; }
        public string? EANr { get; set; }
        public string? Note { get; set; }
        public DagsRapport_TimeRegistrering()
        {

        }
        public DagsRapport_TimeRegistrering(Dagsrapport d,TimeRegistrering t)
        {
            this.DagsrapportId = d.Id;
            this.AntalPersoner = t.AntalPersoner;
            this.EANr = t.EANr;
            this.CGJ_Timer_Total = t.CGJ_Timer_Total;
            this.Note = t.Note;
            this.Timer = t.Timer;
            this.Overtid_100 = t.Overtid_100;
            this.Overtid_50 = t.Overtid_50;
            this.TimerFoerOvertid = t.TimerFoerOvertid;
            this.Title = t.Title;
        }
    }
    public class TimeRegistrering
    {
        public string? Title { get; set; }
        public int AntalPersoner { get; set; }        
        public double Timer { get; set; }            
        public double TimerFoerOvertid { get; set; }          
        public double Overtid_50 { get; set; }
        public double Overtid_100 { get; set; }
        public double CGJ_Timer_Total { get; set; }
        public string? EANr { get; set; }
        public string? Note { get; set; }
        public TimeRegistrering(DagsRapport_TimeRegistrering t)
        {
            this.Title = t.Title;
            this.AntalPersoner = t.AntalPersoner;
            this.Timer = t.Timer;
            this.TimerFoerOvertid = t.TimerFoerOvertid;
            this.Overtid_50 = t.Overtid_50;
            this.Overtid_100 = t.Overtid_100;
            this.CGJ_Timer_Total = t.CGJ_Timer_Total;
            this.EANr = t.EANr;
            this.Note = t.Note;
        }
        public TimeRegistrering()
        {

        }
    }
    public class Building
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Protokol nr")]
        public int ProtocolId { get; set; }
        [Display(Name = "Protokol nr")]
        public virtual Protocol? Protocol { get; set; }
        [Display(Name = "Lokation")]
        public string? Location { get; set; }
        [Display(Name = "Addresse")]
        public string? Address { get; set; }
        [Display(Name = "Fil sti")]
        public string? path { get; set; }
    }
    public class Structure
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Protokol nr")]
        public int ProtocolId { get; set; }
        [Display(Name = "Protokol nr")]
        public virtual Protocol? Protocol { get; set; }
        [Display(Name = "Lokation")]
        public string? Location { get; set; }
        [Display(Name = "Addresse")]
        public string? Name { get; set; }
        [Display(Name = "Fil sti")]
        public string? path { get; set; }
    }
    public class Junction
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Protokol nr")]
        public int ProtocolId { get; set; }
        [Display(Name = "Protokol nr")]
        public virtual Protocol? Protocol { get; set; }
        [Display(Name = "Lokation")]
        public string? Location { get; set; }
        [Display(Name = "Addresse")]
        public string? Name { get; set; }
        [Display(Name = "Fil sti")]
        public string? path { get; set; }
    }
    public class RoadSection
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Protokol nr")]
        public int ProtocolId { get; set; }
        [Display(Name = "Protokol nr")]
        public virtual Protocol? Protocol { get; set; }
        [Display(Name = "Lokation")]
        public string? Location { get; set; }
        [Display(Name = "Addresse")]
        public string? Name { get; set; }
        [Display(Name = "Fil sti")]
        public string? path { get; set; }
    }
    public class AccomodationWork
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Protokol nr")]
        public int ProtocolId { get; set; }
        [Display(Name = "Protokol nr")]
        public virtual Protocol? Protocol { get; set; }
        [Display(Name = "Lokation")]
        public string? Location { get; set; }
        [Display(Name = "Addresse")]
        public string? Name { get; set; }
        [Display(Name = "Fil sti")]
        public string? path { get; set; }
    }
    public class SiteClearancesBetweenContractBorder
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Protokol nr")]
        public int ProtocolId { get; set; }
        [Display(Name = "Protokol nr")]
        public virtual Protocol? Protocol { get; set; }
        [Display(Name = "Lokation")]
        public string? Location { get; set; }
        [Display(Name = "Addresse")]
        public string? Name { get; set; }
        [Display(Name = "Fil sti")]
        public string? path { get; set; }

    }
    public class SiteClearanceForUtilityRelocation
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Protokol nr")]
        public int ProtocolId { get; set; }
        [Display(Name = "Protokol nr")]
        public virtual Protocol? Protocol { get; set; }
        [Display(Name = "Lokation")]
        public string? Location { get; set; }
        [Display(Name = "Addresse")]
        public string? Name { get; set; }
        [Display(Name = "Fil sti")]
        public string? path { get; set; }
    }
    public class Protocol
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Protokol nr")]
        public string? Protocolnr { get; set; }
        [Display(Name = "Navn")]
        public string? Name { get; set; }
    }
    public class Location
    {
        public int Id { get; set; }
        public int? ProtocolId { get; set; }
        public virtual Protocol? Protocol { get; set; }
        public int? BuildingId { get; set; }
        public virtual Building? Buliding { get; set; }
        public int? StructureId { get; set; }
        public virtual Structure? Structure { get; set; }
        public int? JunctionId { get; set; }
        public virtual Junction? Junction { get; set; }
        public int? RoadSectionId { get; set; }
        public virtual RoadSection? RoadSection { get; set; }
        public int? AccomodationWorkId { get; set; }
        public virtual AccomodationWork? AccommodationWork { get; set; }
        public int? SiteClearancesBetweenContractBorderId { get; set; }
        public virtual SiteClearancesBetweenContractBorder? SiteClearancesBetweenContractBorders { get; set; }
        public int? SiteClearanceForUtilityRelocationId { get; set; }
        public virtual SiteClearanceForUtilityRelocation? SiteClearancesForUtilityRelocation { get; set; }
    }
}
