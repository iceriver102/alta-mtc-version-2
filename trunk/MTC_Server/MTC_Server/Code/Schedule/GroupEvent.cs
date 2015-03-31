using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTC_Server.Code.Schedule
{
    public enum ModeGroup
    {
        Vertical, Horizontal,None
    }
    public class GroupEvent
    {
        List<Event> Events;
        public ModeGroup Mode = ModeGroup.None;
        public double Left;
        public double Width;

        public GroupEvent(ModeGroup mode, double left, double w)
        {
            this.Events = new List<Event>();
            this.Left = left;
            this.Width = w;
            this.Mode = mode;
        }
        public void Clear()
        {
            this.Events.Clear();
        }
        public bool Add(ref Event E)
        {
            if (this.Mode == ModeGroup.None)
            {
                this.Events.Add(E);
                E.Left = this.Left;
                E.Width = this.Width;
                return true;
            }
            if (this.Mode == ModeGroup.Vertical)
            {
                bool canAdd= true;
                foreach (Event e in this.Events)
                {
                    if (!((e.Top + e.Height <= E.Top) || (e.Top >= E.Top + E.Height)))
                    {
                        canAdd = false;
                        break;
                    }
                }
                if (canAdd)
                {
                    this.Events.Add(E);
                    E.Left = this.Left;
                    E.Width = this.Width;
                    return true;
                }
            }
            return false;
        }
    }
}
