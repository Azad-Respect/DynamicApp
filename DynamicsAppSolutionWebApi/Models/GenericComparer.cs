using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Helpers;

namespace DynamicsAppSolutionWebApi.Models
{
    public class GenericComparer
    {
        private int level = 0;

        public List<GenericComparer> comparers { get; set; }

        public PropertyInfo PropertyInfo { get; set; }

        public SortDirection SortDirection { get; set; }

        public int Compare<T>(T t1, T t2)
        {
            if (this.level >= this.comparers.Count)
                return 0;
            object obj1 = this.comparers[this.level].PropertyInfo.GetValue((object)t1, (object[])null);
            object obj2 = this.comparers[this.level].PropertyInfo.GetValue((object)t2, (object[])null);
            int num = (object)t1 != null && obj1 != null ? ((object)t2 != null && obj2 != null ? ((IComparable)obj1).CompareTo((object)(IComparable)obj2) : 1) : ((object)t2 != null && obj2 != null ? -1 : 0);
            if (num == 0)
            {
                ++this.level;
                num = this.Compare<T>(t1, t2);
                --this.level;
            }
            //else if (this.comparers[this.level].SortDirection == SortDirection.Descending)
            //    num *= -1;
            return num;
        }
    }
}