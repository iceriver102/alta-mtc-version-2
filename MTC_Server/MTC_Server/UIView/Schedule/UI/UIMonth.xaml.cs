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

namespace MTC_Server.UIView.Schedule.UI
{
    /// <summary>
    /// Interaction logic for UIMonth.xaml
    /// </summary>
    public partial class UIMonth : UserControl
    {
        public event EventHandler<DateTime> SelectDateEvent;
        public int Month
        {
            get
            {
                if (this._date == null)
                    return 0;
                return this._date.Month;
            }
        }
        private DateTime _cuDate;
        public DateTime curDate
        {
            get
            {
                return this._cuDate;
            }
            set
            {
                this._cuDate = value;
                this.Date = value;
            }
        }
        private DateTime _date;
        public DateTime Date
        {
            get
            {
                return this._date;
            }
            set
            {
                this._date = value;
                if (this._date == null)
                    return;
                DateTime tmp = new DateTime(value.Year, value.Month, 1);
                this.DayOFWeek = (int)tmp.DayOfWeek;
                this.firstDateOfMonth = tmp.AddDays(-this.DayOFWeek);
                this.UITitle.Text = string.Format("Tháng {0} {1}",this._date.Month,this._date.Year);
                this.LoadGUI();
            }
        }
        private int DayOFWeek;
        private DateTime firstDateOfMonth;

        public UIMonth()
        {
            InitializeComponent();
        }

        public void LoadGUI()
        {
            int i = 0;
            foreach (UIElement UIE in this.UIDays.Children)
            {
                if (UIE is UIDay)
                {
                    UIDay UIday = UIE as UIDay;
                    UIday.Date = this.firstDateOfMonth.AddDays(i++);
                    if (UIday.Date.Month != this._date.Month)
                    {
                        UIday.Mode = ModeDay.Disable;
                    }
                    else
                    {
                        UIday.Mode = ModeDay.Enable;
                    }
                    if (UIday.Date.Date == this.curDate.Date)
                    {
                        UIday.isCurDay = true;
                    }
                    else
                    {
                        UIday.isCurDay = false;
                    }
                }
            }
        }

        private void RootView_Loaded(object sender, RoutedEventArgs e)
        {
            this.curDate = DateTime.Now;           
        }

        private void Day_SelectDateEvent(object sender, DateTime e)
        {
            if (e != null)
            {
                this.curDate = e;
                foreach (UIElement UIE in this.UIDays.Children)
                {
                    if (UIE is UIDay)
                    {
                        UIDay UIday = UIE as UIDay;
                        if (UIday.Date.Date == this.curDate.Date)
                        {
                            UIday.isCurDay = true;
                        }
                        else
                        {
                            UIday.isCurDay = false;
                        }
                    }
                }
                if (this.SelectDateEvent != null)
                {
                    this.SelectDateEvent(this, this._date);
                }
            }
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Date = this._date.AddMonths(-1);
        }

        private void TextBlock_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            this.Date = this._date.AddMonths(1);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.curDate = DateTime.Now;
            if (this.SelectDateEvent != null)
            {
                this.SelectDateEvent(this, this._date);
            }
        }
    }
}
