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
        public event EventHandler<Event> AddChildEvent;
        public event EventHandler<Event> EditChildEvent;
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
                this._w = value;
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
            this.UICalendar.Height = curTop;
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
            GroupEvent.ExpansionIndex = new List<int>();
            if (this.Events == null)
                return;
            foreach (Event e in this.Events)
            {
                if (e.loop)
                {
                    e.Begin = e.Begin.setDate(this.curDateView.Date);
                    e.End = e.End.setDate(this.curDateView.Date);
                }
                if (e.End.Date >= this.curDateView.Date && e.Begin.Date <= this.curDateView.Date)
                {
                    for (int i = e.beginIndex(this.curDateView); i <= e.EndIndex(this.curDateView); i++)
                    {
                        UITime time = this.UICalendar.Children[i] as UITime;
                        if (time != null && time.hasEvent == false)
                        {
                            time.hasEvent = true;
                            GroupEvent.ExpansionIndex.Add(i);
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
                        e.Width = this.UICalendar.Width - 110;
                        e.Height = 38;
                        e.Left = 0;
                        UIE.Event = e;
                        UIE.Orientation = Orientation.Horizontal;
                        UIE.DeleteEvent += UIE_DeleteEvent;
                        UIE.AddChildEvent += UIE_AddChildEvent;
                        UIE.EditEvent += UIE_EditEvent;
                        UIE.SelectEvent += UIE_SelectEvent;
                        UIE.LinkEvent+=UIE_LinkEvent;
                        this.Time_0.AddChild(UIE);
                    }
                    else
                    {
                        int index = e.comitBeginIndex(this.curDateView);
                        e.Top = e.getTop(this.curDateView);
                        e.Height = e.getHeight(this.curDateView);
                        bool canAdd = false;
                        foreach (GroupEvent G in this.Groups)
                        {
                            if (G.Add(ref e))
                            {
                                UIE.Event = e;
                                canAdd = true;
                                break;
                            }
                        }
                        if(!canAdd)
                        {
                            GroupEvent gE = new GroupEvent(ModeGroup.Vertical, 20 + (this.Groups.Count * this.widthCol), this.widthCol - this.margin);
                            gE.Add(ref e);
                            this.Groups.Add(gE);
                            UIE.Event = e;
                            this.UICalendar.Width = this.Groups[this.Groups.Count - 1].Left + this.widthCol + 100;
                           
                        }
                        if (e.Height > 100)
                            UIE.Orientation = Orientation.Vertical;
                        else UIE.Orientation = Orientation.Horizontal;
                        UIE.DeleteEvent += UIE_DeleteEvent;
                        UIE.EditEvent += UIE_EditEvent;
                        UIE.AddChildEvent += UIE_AddChildEvent;
                        UIE.SelectEvent+=UIE_SelectEvent;
                        UIE.LinkEvent+=UIE_LinkEvent;
                        (this.UICalendar.Children[index] as UITime).AddChild(UIE);
                    }
                }
            }
        }

        private void UIE_LinkEvent(object sender, Event e)
        {

            if (e != null && LinkEvent != null)
            {
                LinkEvent(this, e);
            }
        }
        public event EventHandler<Event> LinkEvent;
        public event EventHandler<Event> SelectEvent;
        void UIE_SelectEvent(object sender, Event e)
        {
            if (e != null && SelectEvent!=null)
            {
                SelectEvent(this, e);
            }
        }

        void UIE_EditEvent(object sender, Event e)
        {
            if (e != null)
            {
                if (EditChildEvent != null)
                {
                    EditChildEvent(this, e);
                }
            }
        }

        void UIE_AddChildEvent(object sender, Event e)
        {
            if (e != null)
            {
                if (AddChildEvent != null)
                {
                    AddChildEvent(this, e);
                }
            }
        }

        void UIE_DeleteEvent(object sender, EventArgs e)
        {
            UIEvent UIE = sender as UIEvent;
            this.Events = App.curUser.getEvents(this.curDateView);
            this.LoadUI();
        }

        private void RootView_Loaded(object sender, RoutedEventArgs e)
        {
            this.UICalendar.Width = this.Width;
            this.widthCol = (this.UICalendar.Width - 40) / this._numCol;
            this.Groups = new List<GroupEvent>();
            for (int i = 0; i < this._numCol; i++)
            {
                GroupEvent gE = new GroupEvent(ModeGroup.Vertical, 20 + (i * this.widthCol), this.widthCol - this.margin);
                this.Groups.Add(gE);
            }
            this.UICalendar.Width = this.Groups[this._numCol-1].Left+this.widthCol+100;
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
