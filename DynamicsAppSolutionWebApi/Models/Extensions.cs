using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace DynamicsAppSolutionWebApi.Models
{
    public static class Extensions
    {
        public static void Sort<T>(this List<T> list, string sortExpression)
        {
            string[] strArray = sortExpression
                                .Split(new string[1] { "," }, StringSplitOptions.RemoveEmptyEntries);

            List<GenericComparer> genericComparerList = new List<GenericComparer>();
            foreach (string str1 in strArray)
            {
                string name = str1.Trim().Split(' ')[0].Trim();
                string str2 = str1.Trim().Split(' ')[1].Trim();
                Type type = typeof(T);
                PropertyInfo propertyInfo = type.GetProperty(name);
                if (propertyInfo == (PropertyInfo)null)
                {
                    foreach (PropertyInfo property in type.GetProperties())
                    {
                        if (property.Name.ToString().ToLower() == name.ToLower())
                        {
                            propertyInfo = property;
                            break;
                        }
                    }
                    if (propertyInfo == (PropertyInfo)null)
                        throw new Exception(string.Format("{0} is not a valid property of type: \"{1}\"", (object)name, (object)type.Name));
                }
                SortDirection sortDirection;
                if (str2.ToLower() == "asc" || str2.ToLower() == "ascending")
                {
                    sortDirection = SortDirection.Ascending;
                }
                else
                {
                    if (!(str2.ToLower() == "desc") && !(str2.ToLower() == "descending"))
                        throw new Exception("Valid SortDirections are: asc, ascending, desc and descending");
                    sortDirection = SortDirection.Descending;
                }
                genericComparerList.Add(new GenericComparer()
                {
                    SortDirection = sortDirection,
                    PropertyInfo = propertyInfo,
                    comparers = genericComparerList
                });
            }
            list.Sort(new Comparison<T>(genericComparerList[0].Compare<T>));
        }
    }
}