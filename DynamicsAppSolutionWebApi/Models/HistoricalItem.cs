using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicsAppSolutionWebApi.Models
{
    public class HistoricalItem
    {
        public string Item { get; set; }
        public string Comment { get; set; }
        public int Icon { get; set; }
        public string Date { get; set; }
    }
}