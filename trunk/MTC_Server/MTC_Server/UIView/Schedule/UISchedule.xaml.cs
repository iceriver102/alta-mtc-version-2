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
                this.UITitle.Text = string.Format("Ngày {0} Tháng {1} {2}", value.Day, value.Month, value.Year);
                this.UIMonth.Text = string.Format("Tháng {0} {1}", value.Month, value.Year);
                UIWeekElement.Date = this.curDateTime;
            }
        }
        public UISchedule()
        {
            InitializeComponent();
            this.UICalendar.curDateView = DateTime.Now;
            List<Event> tmp = new List<Event>();
            tmp.Add(new Event() { Content = "demo", Begin = DateTime.Now.AddDays(-1), End = DateTime.Now.AddDays(1) });
            tmp.Add(new Event() { Content = "demo 2", Begin = DateTime.Now.AddDays(-1), End = DateTime.Now.AddDays(1) });
            tmp.Add(new Event() { Content = "demo", Begin = DateTime.Now, End = DateTime.Now.AddMinutes(1) });
            tmp.Add(new Event() { Content = "demo", Begin = DateTime.Now, End = DateTime.Now.AddMinutes(10) });
            tmp.Add(new Event() { Content = "demo", Begin = DateTime.Now.AddMinutes(30), End = DateTime.Now.AddHours(1) });
            this.UICalendar.setData(tmp);
        }

        private void UIRootView_Loaded(object sender, RoutedEventArgs e)
        {
            this.curDateTime = DateTime.Now;
            

        }

        private void NextEvent(object sender, MouseButtonEventArgs e)
        {
            this.curDateTime=this.curDateTime.AddDays(1);
        }

        private void BackEvent(object sender, MouseButtonEventArgs e)
        {
            this.curDateTime=this.curDateTime.AddDays(-1);
        }
    }
}
