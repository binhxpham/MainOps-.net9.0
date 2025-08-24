using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class TelefonListe
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Fornavn(e)")]
        public string ForNavn { get; set; }
        [Required]
        [Display(Name = "Efternavn")]
        public string Efternavn { get; set; }
        public string Titel { get; set; }
        [Phone]
        [Display(Name = "Telefonnummer")]
        public string Telefonnr { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Display(Name = "Loen Nr")]
        public string Loennr { get; set; }
        [Display(Name ="Leder")]
        public string Leder { get; set; }
        public int? DivisionId { get; set; }
        public virtual Division Division { get; set; }
    }
}
