using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class TrackItem
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "TAG Nr.")]
        public string TAGnr { get; set; }
        [Display(Name = "PID Nr.")]
        public string PIDnr { get; set; }
        [Display(Name = "Rum Nr.")]
        public string RUMnr { get; set; }
        [Display(Name = "HOFOR Description")]
        public string HOFOR_Description { get; set; }
        [Display(Name = "Producer")]
        public string Producent { get; set; }
        [Display(Name = "Supplier")]
        public string  Supplier { get; set; }
        [Display(Name = "Type Nr. 1")]
        public string TypeNr { get; set; }
        [Display(Name = "Type Nr. 2")]
        public string TypeNr2 { get; set; }
        [Display(Name = "Product Nr. / EAN Nr.")]
        public string EANnr { get; set; }
        [Display(Name = "Dimensions / Attachment")]
        public string Dimension { get; set; }
        [Display(Name = "Certificate Availability")]
        public string Certificate { get; set; }
        [Display(Name = "Comments")]
        public string Comments { get; set; }
        [Display(Name = "P2[kW]")]
        public string P2_kW { get; set; }
        [Display(Name = "[V]")]
        public string Volts { get; set; }
        [Display(Name = "[A]")]
        public string Ampere { get; set; }
        [Display(Name = "Kom. Bus")]
        public string KomBus { get; set; }
        [Display(Name = "MCC Tavle")]
        public string Tavle { get; set; }
        [Display(Name = "Power supply")]
        public string Supply { get; set; }
        [Display(Name = "Measuremnet Area")]
        public string MeasArea { get; set; }
        [Display(Name = "Unit")]
        public string Units { get; set; }
        [Display(Name = "Enterprice")]
        public string Enterprice { get; set; }
        [Display(Name = "Item Packed")]
        public bool Packed { get; set; }
        [Display(Name = "Item Sent")]
        public bool Sent { get; set; }
        [Display(Name = "Item Received")]
        public bool Received { get; set; }
        [Display(Name = "Item Installed")]
        public bool Installed { get; set; }
        [Display(Name = "Coordinates")]
        public ICollection<CoordTrack> TrackedCoordinates { get; set; }
        [Display(Name = "Photos Packing")]
        public ICollection<PhotoFilePack> PackedPhotos { get; set; }
        [Display(Name = "Photos Sent")]
        public ICollection<PhotoFileSent> SentPhotos { get; set; }
        [Display(Name = "Photos Received")]
        public ICollection<PhotoFileReceived> ReceivedPhotos { get; set; }
        [Display(Name = "Photos Installed")]
        public ICollection<PhotoFileInstalled> InstalledPhotos { get; set; }
        [Display(Name = "Photos Error")]
        public ICollection<PhotoError> ErrorPhotos { get; set; }
        [Display(Name = "Log book")]
        public ICollection<Log> Logs { get; set; }
        [Display(Name = "Error Status")]
        public bool IsError { get; set; }
        [Display(Name = "Project")]
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public string Category
        {
            get
            {
                string b = "";
                for (int i = 12; i < this.TAGnr.Length; i++)
                {
                    if (!Char.IsDigit(this.TAGnr[i]))
                        b += this.TAGnr[i];
                }
                return b;
            }
        }
    }
}
