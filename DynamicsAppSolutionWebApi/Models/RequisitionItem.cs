using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicsAppSolutionWebApi.Models
{
    public class RequisitionItem
    {
        public string Item { get; set; }
        public string CurrencyCode { get; set; }
        public string ItemDate { get; set; }
        public string LineAmount { get; set; }
        public Decimal Qty { get; set; }
        public string UnitPrice { get; set; }
        public List<Dimension> Dimensions { get; set; }
    }
}