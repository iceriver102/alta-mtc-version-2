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
        public static DateTime setTime(this DateTime d, TimeSpan time)
        {
            DateTime temp = d.Date;
            return temp.AddSeconds(time.TotalSeconds);
        }
        public static DateTime setDate(this DateTime d, DateTime date)
        {
            DateTime tmp = date.Date;
            tmp = tmp.setTime(d.TimeOfDay);
            d = tmp;
            return tmp;
        }
    }
}
