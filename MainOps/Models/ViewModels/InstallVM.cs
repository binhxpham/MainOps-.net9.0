using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models.ViewModels
{
    public class InstallVM
    {
        public Install Install { get; set; }
        public List<PhotoFileInstalled2> Install_Photos { get; set; }
    }
    public class ArrivalVM
    {
        public Arrival Arrival { get; set; }
        public List<PhotoFileArrival> Arrival_Photos { get; set; }
    }
    public class MobVM
    {
        public Mobilize Mobilize { get; set; }
        public List<PhotoFileMobilized> Mob_Photos { get; set; }
    }
    public class MaintenanceVM2
    {
        public Maintenance Maintenance { get; set; }
        public List<MaintenanceEntry> Maintenances { get; set; }
        public List<PhotoFileMaintenance> Maintenance_Photos { get; set; }
    }
}
