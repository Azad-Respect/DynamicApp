using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicsAppSolutionWebApi.Models
{
    public class Expense
    {
        public string Name { get; set; }

        public string CreatedDateTime { get; set; }

        public string Location { get; set; }

        public string Purpose { get; set; }

        public string Status { get; set; }
    }
}