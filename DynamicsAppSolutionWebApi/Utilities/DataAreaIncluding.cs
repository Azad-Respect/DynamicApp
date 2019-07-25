using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicsAppSolutionWebApi.Utilities
{
    public static class DataAreaIncluding
    {
        public static bool Check(string dataAreaId, string companies)
        {
            char[] chArray = new char[1] { ',' };
            foreach (string company in companies.Split(chArray))
            {
                if (company.ToUpper().Trim() == dataAreaId.ToUpper().Trim() || company == "-1" && dataAreaId == string.Empty)
                    return true;
            }
            return false;
        }
    }
}