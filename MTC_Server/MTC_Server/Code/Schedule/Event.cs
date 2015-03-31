using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTC_Server.Code.Schedule
{
    public class Event
    {
        public string Content { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public int Col
        {
            get;
            set;
        }
        public int beginIndex(DateTime curTime)
        {
            if (this.isAllDay(curTime))
                return 0;
            if (this.Begin.Date < curTime.Date)
                return 1;
            return this.Begin.Hour + 1;
        }

        public double Top { get; set; }
        public double Left { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }

        public int EndIndex(DateTime curTime)
        {
            if (this.isAllDay(curTime))
            {
                return 0;
            }
            if (this.End.Date > curTime.Date)
                return 24;
            return this.End.Hour + 1;
        }

        public bool isAllDay(DateTime curDateView)
        {
            if ((this.End.Date > curDateView.Date || this.End.TimeOfDay == new TimeSpan(23, 59, 59)) && (this.Begin < curDateView.Date || this.Begin.TimeOfDay == new TimeSpan(23, 59, 59)))
                return true;
            return false;
        }

        public double getHeight(DateTime curDateView)
        {
            if (this.Begin == null)
                return double.NaN;
            if (this.End == null)
                return 0;
            if (this.isAllDay(curDateView))
                return 30;
            TimeSpan tmp;
            if (this.End.Date > curDateView.Date)
            {
                curDateView = new DateTime(curDateView.Year, curDateView.Month, curDateView.Day, 23, 59, 59);
                tmp = curDateView - this.Begin;
            }
            else if(this.Begin.Date == curDateView.Date)
            {
                tmp = this.End - this.Begin;
            }
            else
            {
                tmp = this.End - curDateView.Date;
            }
            return (tmp.TotalMinutes * 2 < 20) ? 20 : tmp.TotalMinutes * 2;
        }

        public double getTop(DateTime curDateView)
        {
            if (this.Begin == null)
                return double.NaN;
            if (curDateView == null)
            {
                curDateView = DateTime.Now;
            }
            if (this.isAllDay(curDateView))
                return 0;
            if (curDateView.Date > this.Begin.Date)
                return 0;
            return this.Begin.TimeOfDay.Minutes*2;
        }

    }
}
