using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTC_Server.Code.Schedule
{
    public enum ModeGroup
    {
        Vertical, Horizontal, None
    }
    public class GroupEvent
    {
        List<Event> Events;
        public ModeGroup Mode = ModeGroup.None;
        public double Left;
        public double Width;
        public static List<int> ExpansionIndex;

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
            if (E.Id == 7)
            {
                Console.Write(E.Id);
            }
            if (this.Mode == ModeGroup.Vertical)
            {
                bool canAdd = true;
                foreach (Event e in this.Events)
                {
                    double h = 0;
                    int min = (e.BeginIndex <= E.BeginIndex) ? e.BeginIndex : E.BeginIndex;
                    int max = (e.BeginIndex <= E.BeginIndex) ? E.BeginIndex : e.BeginIndex;
                    for (int i = min; i < max; i++)
                    {
                        bool isEx = false;
                        foreach (int j in ExpansionIndex)
                        {
                            if (i == j)
                            {
                                isEx = true;
                                break;
                            }
                        }
                        if (isEx)
                            h += 120;
                        else h += 60;
                    }

                    if (e.BeginIndex < E.BeginIndex || (e.BeginIndex == E.BeginIndex && e.Top <= E.Top))
                    {
                        h -= e.Top;
                        h += E.Top;
                        if (e.Height >= h)
                        {
                            canAdd = false;
                            break;
                        }
                    }
                    else
                    {
                        h += e.Top;
                        h -= E.Top;
                        if (E.Height >= h)
                        {
                            canAdd = false;
                            break;
                        }
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
