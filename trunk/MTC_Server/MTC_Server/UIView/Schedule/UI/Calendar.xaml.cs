using MTC_Server.Code.Schedule;
using MTC_Server.UIView.Schedule.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Alta.Class;

namespace MTC_Server.UIView.Schedule
{
    /// <summary>
    /// Interaction logic for Calendar.xaml
    /// </summary>
    public partial class Calendar : UserControl
    {
        private List<Event> Events;
        public double margin = 2;
        private double _w = 202;
        public double widthCol
        {
            get
            {
                return this._w;
            }
            set
            {
                this._w= value;
            }
        }
        private int _numCol = 6;
        public int Column
        {
            get
            {
                return this._numCol;
            }
            set
            {
                this._numCol = value;
            }
        }
        public List<GroupEvent> Groups;
        private DateTime _curDate;
        public DateTime curDateView
        {
            get
            {
                return this._curDate;
            }
            set
            {
                if (value.Date != this._curDate.Date)
                {
                    this._curDate = value;
                    this.LoadUI();
                }
                
                
               
            }
        }
        
        public Calendar()
        {
            this.Events = new List<Event>();
            InitializeComponent();
        }

        public void ChangeDay(int day)
        {
            this.curDateView = this.curDateView.AddDays(day);
            this.LoadUI();
        }
        public void UpdateLayout()
        {
            double curTop = 0;
            foreach (UIElement UITime in this.UICalendar.Children)
            {
                if (UITime is UITime)
                {
                    UITime uiT = UITime as UITime;
                    uiT.setTop(curTop);
                    curTop += uiT.Height;
                }
            }
            this.UICalendar.Height=curTop;
        }

        public void setData(List<Event> events)
        {
            this.Events = events;
        }
        public void Clear()
        {
            foreach (UIElement UIE in this.UICalendar.Children)
            {
                if (UIE is UITime)
                {
                    UITime tmp = UIE as UITime;
                    tmp.Clear();
                }
            }
            if (this.Groups != null)
            {
                foreach (GroupEvent gE in this.Groups)
                {
                    gE.Clear();
                }
            }
        }

        public void LoadUI()
        {
            this.Clear();
            if (this.Events == null)
                return;
            foreach (Event e in this.Events)
            {
                if (e.End.Date >= this.curDateView.Date && e.Begin.Date<= this.curDateView.Date)
                {
                    for (int i = e.beginIndex(this.curDateView); i <= e.EndIndex(this.curDateView); i++)
                    {
                        UITime time = this.UICalendar.Children[i] as UITime;
                        if (time != null && time.hasEvent == false)
                        {
                            time.hasEvent = true;
                        }
                    }
                }
            }
            this.UpdateLayout();
            for (int i = 0; i < this.Events.Count; i++)
            {
                Event e = this.Events[i];
                if (e.End.Date >= this.curDateView.Date && e.Begin.Date <= this.curDateView.Date)
                {
                    UIEvent UIE = new UIEvent();
                    if (e.isAllDay(this.curDateView))
                    {
                        e.Width = this.UICalendar.Width - 90;
                        e.Height = 30;
                        e.Left = 0;
                        UIE.Event = e;
                        this.Time_0.AddChild(UIE);
                    }
                    else
                    {
                        e.Top = e.getTop(this.curDateView);
                        e.Height = e.getHeight(this.curDateView);
                        foreach (GroupEvent G in this.Groups)
                        {
                            if (G.Add(ref e))
                            {
                                UIE.Event = e;
                                break;
                            }
                        }
                        int index = e.beginIndex(this.curDateView);
                        (this.UICalendar.Children[index] as UITime).AddChild(UIE);
                    }
                }
            }
        }

        private void RootView_Loaded(object sender, RoutedEventArgs e)
        {
            this.widthCol = (this.UICalendar.Width - 90) / this._numCol;
            this.Groups = new List<GroupEvent>();
            for (int i = 0; i < this._numCol; i++)
            {
                GroupEvent gE = new GroupEvent(ModeGroup.Vertical, 90 + (i * this.widthCol), this.widthCol-this.margin);
                this.Groups.Add(gE);
            }
            this.LoadUI();
        }

        private void UIBtnBack_Click(object sender, RoutedEventArgs e)
        {
            ChangeDay(-1);
        }

        private void UIBtnNext_Click(object sender, RoutedEventArgs e)
        {
            ChangeDay(1);
        }
    }
}
