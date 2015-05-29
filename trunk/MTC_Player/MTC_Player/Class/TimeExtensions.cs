using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alta.Class
{
    public static class TimeExtensions
    {
        public static string Format(this TimeSpan t, string format = "{0:00}:{1:00}:{2:00}")
        {
            return string.Format(format, t.Hours, t.Minutes, t.Seconds);
        }

        public static TimeSpan secondToTimeSpan(this double number)
        {
            int hour = (int)number / 3600;
            int min = (int)((number - hour * 3600) / 60);
            int second = (int)(number - hour * 3600 - min * 60);
            return new TimeSpan(hour, min, second); 
        }
        public static TimeSpan miniSecondToTimeSpan(this double num)
        {
            int hour = (int)num / 3600000;
            int min = (int)((num - hour * 3600000) / 60000);
            int second = (int)(num - hour * 3600000 - min * 60000);
            return new TimeSpan(hour, min, second);
        }
    }
}
