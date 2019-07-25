using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicsAppSolutionWebApi.Models
{
    public class Attachment
    {
        public double RecId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string FileSize { get; set; }
    }
}