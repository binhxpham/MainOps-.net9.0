using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ViewModels
{
    public class GantInstall
    {
        public Install Install { get; set; }
        public DateTime StartOperation { get; set; }
        public DateTime EndOperation { get; set; }
        public DateTime MovedToDestination { get; set; }
        public DateTime MovedFromDestination { get; set; }
        public string KMmark { get; set; }
        public string Stretch { get; set; }
        public double KMstart { get; set; }
        public double KMend { get; set; }
        public double KM { get; set; }
        public decimal RealPrice { get; set; }
        public decimal PlannedPrice { get; set; }
        public int totaldays { get; set; }
        public int operationdays { get; set; }
        public int idledays { get; set; }
    }
    public class Stretch
    {
        public double start { get; set; }
        public double end { get; set; }
        public int rentaldays { get; set; }
    }
    public class StrechList
    {
        public List<GantInstall> GantInstalls { get; set; }
        public Stretch Strectch { get; set; }
    }
}
