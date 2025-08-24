using MainOps.Models.WTPClasses.MainClasses;
using System.Collections.Generic;

namespace MainOps.Models.WTPClasses.ViewModels
{
    public class PriceList
    {
        public string titlename { get; set; }
        public string projectname { get; set; }
        //public string item { get; set; }
        public Dictionary<string,decimal> prices_mob { get; set; }
        public Dictionary<string, decimal> prices_demob { get; set; }
        public Dictionary<string, decimal> prices_rent { get; set; }
        public Dictionary<string,decimal> prices_operation { get; set; }
        public Dictionary<string,decimal> quantities_mob { get; set; }
        public Dictionary<string, decimal> quantities_demob { get; set; }
        public Dictionary<string, decimal> quantities_rent { get; set; }
        public Dictionary<string, decimal> quantities_operation { get; set; }
        public decimal total_price_mob { get; set; }
        public decimal total_price_demob { get; set; }
        public decimal total_price_rent { get; set; }
        public decimal total_price_operation { get; set; }
        public List<Price> prices_all { get; set; }
        public PriceList()
        {

        }
        public PriceList(string title, string project)
        {
            total_price_mob = (decimal)0.0;
            total_price_demob = (decimal)0.0;
            total_price_rent = (decimal)0.0;
            total_price_operation = (decimal)0.0;
            titlename = title;
            projectname = project;
            prices_mob = new Dictionary<string, decimal>();
            prices_demob = new Dictionary<string, decimal>();
            prices_rent = new Dictionary<string, decimal>();
            prices_operation = new Dictionary<string, decimal>();
            quantities_mob = new Dictionary<string, decimal>();
            quantities_demob = new Dictionary<string, decimal>();
            quantities_rent = new Dictionary<string, decimal>();
            quantities_operation = new Dictionary<string, decimal>();
            prices_all = new List<Price>();
        }
    }

}
