using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MainOps.Models.CGJClassesBeton
{
    public class DagsRapportBeton
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Undersag")]
        [ForeignKey("SubProject")]
        public int SubProjectId { get; set; }
        [Display(Name = "Undersag")]
        public virtual SubProject? SubProject { get; set; }
        [Display(Name = "Sag")]
        public int ProjectId { get; set; }
        [Display(Name = "Sag")]
        public Project? Project { get; set; }
        public DateTime Dato { get; set; }
        [Display(Name = "Udarbejdet af")]
        public string? UdarbjedetAf { get; set; }
        [Display(Name = "Arbejdet fortsættes")]
        public bool ForSaetter { get; set; }
        [Display(Name = "VejrForhold")]
        public bool Vejr { get; set; }

        public bool Sol { get; set; }
        public bool Skyet { get; set; }
        public bool OverSkyet { get; set; }
        [Display(Name = "Regn/Sne")]
        public bool Regn_sne { get; set; }
        public bool Vindstille { get; set; }
        [Display(Name = "Svag Vind")]
        public bool Svag_Vind { get; set; }
        [Display(Name = "Jævn Vind")]
        public bool Jaevn_Vind { get; set; }
        [Display(Name = "Hård Vind")]
        public bool Haard_Vind { get; set; }
        [Display(Name = "Temperatur(kl. 8)")]
        public double? Temperatur_kl_otte { get; set; }
        [Display(Name = "Temperatur(kl. 12)")]
        public double? Temperatur_kl_tolv { get; set; }
        
        [Display(Name = "Uddybende noter")]
        public string? UddybendeNoter { get; set; }
        public ICollection<TimeRegistreringBeton> Timer { get; set; }
        public ICollection<TimeRegistreringEkstraBeton> TimerEkstraarbejder { get; set; }
        public ICollection<KontrakArbejdeBeton> Kontraktarbejder { get; set; }
        public ICollection<PhotoFileBeton> Fotos { get; set; }
        public ICollection<MaterielNumber> Materiel { get; set; }
        public DagsRapportBeton()
        {

        }
        public DagsRapportBeton(DagsrapportBetonVM model)
        {
            this.Id = model.Id;
            this.Dato = model.Dato;
            this.ForSaetter = model.ForSaetter;
            this.Haard_Vind = model.Haard_Vind;
            this.Jaevn_Vind = model.Jaevn_Vind;
            this.ProjectId = model.ProjectId;
            this.SubProjectId = model.SubProjectId;
            this.Svag_Vind = model.Svag_Vind;
            this.Temperatur_kl_otte = model.Temperatur_kl_otte;
            this.Temperatur_kl_tolv = model.Temperatur_kl_tolv;
            this.UdarbjedetAf = model.UdarbjedetAf;
            this.Vindstille = model.Vindstille;
            this.Vejr = model.Vejr;
            this.Regn_sne = model.Regn_sne;
            this.Skyet = model.Skyet;
            this.Sol = model.Sol;
            this.UddybendeNoter = model.UddybendeNoter;
            this.OverSkyet = model.OverSkyet;
        }
    }
    public class DagsrapportBetonVM
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Undersag")]
        public int SubProjectId { get; set; }
        [Display(Name = "Undersag")]
        public virtual SubProject? SubProject { get; set; }
        [Display(Name = "Sag")]
        public int ProjectId { get; set; }
        [Display(Name = "Sag")]
        public Project? Project { get; set; }
        [NotMapped]
        public List<string>? Photos { get; set; }
        public DateTime Dato { get; set; }
        [Display(Name = "Udarbejdet af")]
        public string? UdarbjedetAf { get; set; }
        [Display(Name = "Fortsættes")]
        public bool ForSaetter { get; set; }
        [Display(Name = "VejrForhold")]
        public bool Vejr { get; set; }

        public bool Sol { get; set; }
        public bool Skyet { get; set; }
        public bool OverSkyet { get; set; }
        [Display(Name = "Regn/Sne")]
        public bool Regn_sne { get; set; }
        public bool Vindstille { get; set; }
        [Display(Name = "Svag Vind")]
        public bool Svag_Vind { get; set; }
        [Display(Name = "Jævn Vind")]
        public bool Jaevn_Vind { get; set; }
        [Display(Name = "Hård Vind")]
        public bool Haard_Vind { get; set; }
        [Display(Name = "Temperatur(kl. 8)")]
        public double? Temperatur_kl_otte { get; set; }
        [Display(Name = "Temperatur(kl. 12)")]
        public double? Temperatur_kl_tolv { get; set; }

        [Display(Name = "Uddybende noter")]
        public string? UddybendeNoter { get; set; }
        public List<TimeRegistreringBeton> Timer { get; set; }
        public List<TimeRegistreringEkstraBeton> TimerEkstraarbejder { get; set; }
        public List<KontrakArbejdeBeton> Kontraktarbejder { get; set; }
        //public List<PhotoFileBeton> Fotos { get; set; }
        public List<MaterielNumber> Materiel { get; set; }
        public DagsrapportBetonVM()
        {
            Timer = new List<TimeRegistreringBeton>(10);
            TimerEkstraarbejder = new List<TimeRegistreringEkstraBeton>(10);
            Kontraktarbejder = new List<KontrakArbejdeBeton>(10);
            Materiel = new List<MaterielNumber>();
            for (int i = 0; i < 10; i++)
            {
                Timer.Add(new TimeRegistreringBeton());
            }
            for (int i = 0; i < 10; i++)
            {
                TimerEkstraarbejder.Add(new TimeRegistreringEkstraBeton());
            }
            for (int i = 0; i < 10; i++)
            {
                Kontraktarbejder.Add(new KontrakArbejdeBeton());
            }
           
        }
        public DagsrapportBetonVM(DagsRapportBeton model)
        {
            this.Haard_Vind = model.Haard_Vind;
            this.ForSaetter = model.ForSaetter;
            this.Id = model.Id;
            this.Jaevn_Vind = model.Jaevn_Vind;
            this.OverSkyet = model.OverSkyet;
            this.ProjectId = model.ProjectId;
            this.Project = model.Project;
            this.SubProjectId = model.SubProjectId;
            this.SubProject = model.SubProject;
            this.Svag_Vind = model.Svag_Vind;
            this.Temperatur_kl_otte = model.Temperatur_kl_otte;
            this.Temperatur_kl_tolv = model.Temperatur_kl_tolv;
            this.UdarbjedetAf = model.UdarbjedetAf;
            this.Vejr = model.Vejr;
            this.Vindstille = model.Vindstille;
            this.Regn_sne = model.Regn_sne;
            this.Sol = model.Sol;
            this.Skyet = model.Skyet;
            this.Dato = model.Dato;
            int maxtimer = 10;
            int maxtimerEA = 10;
            int maxK = 10;
            if (model.Timer.Count() - 1 > 10)
            {
                maxtimer = model.Timer.Count();
            }
            if (model.TimerEkstraarbejder.Count() - 1 > 10)
            {
                maxtimerEA = model.TimerEkstraarbejder.Count();
            }
            if (model.Kontraktarbejder.Count() - 1 > 10)
            {
                maxK = model.TimerEkstraarbejder.Count();
            }
            this.Timer = new List<TimeRegistreringBeton>(maxtimer);
            this.TimerEkstraarbejder = new List<TimeRegistreringEkstraBeton>(maxtimerEA);
            this.Kontraktarbejder = new List<KontrakArbejdeBeton>(maxK);
            this.Materiel = new List<MaterielNumber>();
            this.UddybendeNoter = model.UddybendeNoter;
            
            for (int i = 0; i < maxtimer; i++)
            {
                if (model.Timer.Count > i)
                {
                    this.Timer.Add(model.Timer.ElementAt(i));
                }
                else
                {
                    this.Timer.Add(new TimeRegistreringBeton());
                }
            }
            for (int i = 0; i < maxtimerEA; i++)
            {
                if (model.TimerEkstraarbejder.Count > i)
                {
                    this.TimerEkstraarbejder.Add(model.TimerEkstraarbejder.ElementAt(i));
                }
                else
                {
                    this.TimerEkstraarbejder.Add(new TimeRegistreringEkstraBeton());
                }
            }
            for (int i = 0; i < 14; i++)
            {
                if (model.Kontraktarbejder.Count > i)
                {
                    this.Kontraktarbejder.Add(model.Kontraktarbejder.ElementAt(i));
                }
                else
                {
                    this.Kontraktarbejder.Add(new KontrakArbejdeBeton());
                }
            }
            for (int i = 0; i < model.Materiel.Count -1; i++)
            {
                this.Materiel.Add(model.Materiel.ElementAt(i));
            }
        }
    }

    public class Materiel
    {
        [Key]
        public int Id { get; set; }
        public string? Materiellet { get; set; }
    }
    public class MaterielNumber
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Materiel")]
        public int? MaterielId { get; set; }
        public virtual Materiel? Materiel { get; set; }
        public int Antal { get; set; }
        public int? DagsRapportBetonId { get; set; }
        public virtual DagsRapportBeton? DagsRapportBeton { get; set; }
    }
    
    public class KontrakArbejdeBeton
    {
        [Key]
        public int Id { get; set; }
        public int? DagsRapportBetonId { get; set; }
        public virtual DagsRapportBeton? DagsRapportBeton { get; set; }
        [Display(Name  ="Materiale")]
        public string? EgetMateriel { get; set; }
        [Display(Name = "Lev./Vognmand")]
        public string? UL_Firma { get; set; }
        [Display(Name = "Specifikation")]
        public string? UL_Materiel { get; set; }
        public KontrakArbejdeBeton()
        {

        }
        
    }
   
    public class TimeRegistreringBeton
    {
        [Key]
        public int Id { get; set; }
        public int? DagsRapportBetonId { get; set; }
        public virtual DagsRapportBeton? DagsRapportBeton { get; set; }
        public string? Navn { get; set; }
        [Display(Name = "Timer Normal")]
        public double Timer { get; set; }
        [Display(Name = "O. Timer 50/100 %")]
        public double Overtid_50100 { get; set; }
        public double? Timer_EA { get; set; }
        [Display(Name = "Timer på Aktivitet")]
        public double CGJ_Timer_Total { get { return this.Timer + this.Overtid_50100; } }
        [Display(Name = "Beregnet")]
        public double CGJ_Timer_Total_Udregnet { get { return this.Timer + this.Overtid_50100 * 2.0; } }
        public string? Note { get; set; }
        public TimeRegistreringBeton()
        {

        }
       
    }
    public class TimeRegistreringEkstraBeton
    {
        [Key]
        public int Id { get; set; }
        public int? DagsRapportBetonId { get; set; }
        public virtual DagsRapportBeton? DagsRapportBeton { get; set; }
        public string? Navn { get; set; }
        [Display(Name = "Timer Normal")]
        public double Timer { get; set; }
        [Display(Name = "O. Timer 50/100 %")]
        public double Overtid_50100 { get; set; }
        public double? Timer_EA { get; set; }
        [Display(Name = "Timer på Aktivitet")]
        public double CGJ_Timer_Total { get { return this.Timer + this.Overtid_50100; } }
        [Display(Name = "Beregnet")]
        public double CGJ_Timer_Total_Udregnet { get { return this.Timer + this.Overtid_50100 * 2.0; } }
        public string? Note { get; set; }
        public TimeRegistreringEkstraBeton()
        {

        }
        
    }
    
    
    public class PhotoFileBeton
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
        [Display(Name = "Time Stamp")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Tracked item")]
        public int DagsRapportBetonId { get; set; }
        public virtual DagsRapportBeton DagsRapportBeton { get; set; }
    }


}
