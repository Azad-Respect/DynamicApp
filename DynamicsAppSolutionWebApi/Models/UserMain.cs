using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicsAppSolutionWebApi.Models
{
    public class UserMain
    {
        public string Domain { get; set; }
        public string URLWebService { get; set; }
        public string Logo { get; set; }
        public string LicensedTo { get; set; }
        public bool IsLicenseValid { get; set; }
        public string UserCompleteName { get; set; }
        public string UserImage { get; set; }
        public string Language { get; set; }
        public int QuantityTransactions { get; set; }
        public List<DataArea> Companies { get; set; }
    }
}