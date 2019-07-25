using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicsAppSolutionWebApi.Models
{
    public class UserInfo
    {
        public bool HasError { get; set; }
        public string ErrorDetails { get; set; }
        public string Password { get; set; }
    }
}