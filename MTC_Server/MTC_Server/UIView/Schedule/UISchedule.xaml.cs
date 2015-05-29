using MTC_Server.Code.Schedule;
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
    /// Interaction logic for UISchedule.xaml
    /// </summary>
    public partial class UISchedule : UserControl
    {

        public DateTime curDateTime
        {
            get
            {
                return this.UICalendar.curDateView;
            }
            set
            {
                this.UICalendar.curDateView = value;
                this.UITitle.Text = string.Format("{0:dd/MM/yyyy}", value);
                this.UIMonth.Text = string.Format("Tháng {0}  {1}", value.Month, value.Year);
                UIWeekElement.Date = this.curDateTime;
            }
        }
        public UISchedule()
        {
            InitializeComponent();
            this.UICalendar.curDateView = DateTime.Now;
            List<Event> tmp = App.curUser.getEvents(DateTime.Now);
            this.UICalendar.setData(tmp);
        }

        private void UIRootView_Loaded(object sender, RoutedEventArgs e)
        {
            this.UICalendar.AddChildEvent += UICalendar_AddChildEvent;
            this.UICalendar.EditChildEvent += UICalendar_EditChildEvent;
            this.UICalendar.SelectEvent += UICalendar_SelectEvent;
            this.UICalendar.LinkEvent += UICalendar_LinkEvent;
            this.curDateTime = DateTime.Now;
        }

        void UICalendar_LinkEvent(object sender, Event e)
        {
            if (e != null)
            {
                UIViewPlaylist.Event = e;
                UIViewPlaylist.showViewInput();
            }
        }

        void UICalendar_SelectEvent(object sender, Event e)
        {
            if (e != null)
            {
                UIViewPlaylist.Event = e;
                UIViewPlaylist.showPlaylist();
            }
        }

        void UICalendar_EditChildEvent(object sender, Event e)
        {
            if (e != null)
            {
                UIAddEvent UI = new UIAddEvent();
                UI.Width = 1366;
                UI.Height = 668;
                UI.setLeft(0);
                UI.setTop(0);
                UI.CloseEvent += CloseAddEvent;
                UI.Event = e;
                this.UIRoot.Children.Add(UI);
            }
        }

        void UICalendar_AddChildEvent(object sender, Event e)
        {
            if (e != null)
            {
                UIAddEvent UI = new UIAddEvent();
                UI.Width = 1366;
                UI.Height = 668;
                UI.setLeft(0);
                UI.setTop(0);
                UI.CloseEvent += CloseAddEvent;
                UI.ParentEvent = e;
                this.UIRoot.Children.Add(UI);
            }
        }

        private void NextEvent(object sender, MouseButtonEventArgs e)
        {
            this.curDateTime = this.curDateTime.AddDays(1);
        }

        private void BackEvent(object sender, MouseButtonEventArgs e)
        {
            this.curDateTime = this.curDateTime.AddDays(-1);
        }

        private void UIWeekElement_SelectDateEvent(object sender, DateTime e)
        {
            if (e != null)
            {
                this.curDateTime = e;
            }
        }

        private void UIMonth_SelectDateEvent(object sender, DateTime e)
        {
            if (e != null)
            {
                this.curDateTime = e;
            }
        }
        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UIAddEvent UI = new UIAddEvent();
            UI.Width = 1366;
            UI.Height = 668;
            UI.setLeft(0);
            UI.setTop(0);
            UI.CloseEvent += CloseAddEvent;
            this.UIRoot.Children.Add(UI);
        }

        private void CloseAddEvent(object sender, CloseUIEventData e)
        {
            if (e.Data != null)
            {
                List<Event> tmp = App.curUser.getEvents(this.UICalendar.curDateView);
                this.UICalendar.setData(tmp);
                this.UICalendar.LoadUI();
            }
            this.UIRoot.Children.Remove(sender as UIAddEvent);
        }
    }
}
