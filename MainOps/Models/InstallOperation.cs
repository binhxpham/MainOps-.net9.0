using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainOps.Models
{
    public class InstallOperation
    {
        public int Id { get; set; }
        public int? InstallId { get; set; }
        public virtual Install Install { get; set; }
        public DateTime Started { get; set; }
        public DateTime? Stopped { get; set; }
        public string BaseID
        {
            get
            {
                return new String(this.Install.UniqueID.Where(Char.IsDigit).ToArray());
            }
        }
    }
    public class LifeCycle
    {
        public int? InstallId { get; set; }
        public virtual Install Install { get; set; }
        public IList<CoordTrack2> Coordinates { get; set; }
        public IList<InstallOperation> OperationTimes { get; set; }
        public IList<Install> Installations { get; set; }
        public string UniqueID { get; set; }
        public string BaseID { get
            {
                return new String(this.UniqueID.Where(Char.IsDigit).ToArray());
            }
        }
        public LifeCycle()
        {
            
        }
        public LifeCycle(Install inst)
        {
            this.InstallId = inst.Id;
            this.Install = inst;
        }
    }
}
