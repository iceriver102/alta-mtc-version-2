using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alta.Class
{
    public static class DateTimeExtensions
    {
        public static string format(this DateTime d, string format = "dd-MM-yyyy")
        {
            return string.Format("{0:" + format + "}", d);
        }
    }
}
