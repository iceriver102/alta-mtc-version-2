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
    public enum ModeDay
    {
        Disable,Enable,CurentDay
    }
    /// <summary>
    /// Interaction logic for UIDay.xaml
    /// </summary>
    public partial class UIDay : UserControl
    {
        public event EventHandler<DateTime> SelectDateEvent;
        public event EventHandler PreviewSelectDateEvent;
        public bool isCurDay
        {
            get
            {
                return this.UIcurDay.Visibility == System.Windows.Visibility.Visible;
            }
            set
            {
                if (value)
                {
                    this.UIcurDay.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    this.UIcurDay.Visibility = System.Windows.Visibility.Hidden;
                }
            }
        }
        private ModeDay _mode= ModeDay.Enable;
        public ModeDay Mode
        {
            get
            {
                return this._mode;
            }
            set
            {
                this._mode = value;
                if (this._mode == ModeDay.Disable)
                {
                    this.TextDay.Foreground = Brushes.DarkGray;
                }
                else if (this._mode == ModeDay.Enable)
                {
                    if (this._date != null && this._date.DayOfWeek == 0)
                    {
                        this.TextDay.Foreground = Brushes.Red;
                    }
                    else
                    {
                        this.TextDay.Foreground = Brushes.Black;
                    }
                }

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
                this.TextDay.Text = string.Format("{0}", value.Day);
                if (this._date.Date == DateTime.Now.Date)
                {
                    this.UINow.Visibility = Visibility.Visible;
                }
                else
                {
                    this.UINow.Visibility = Visibility.Hidden;
                }
            }
        }
        public bool hasEvent
        {
            get
            {
                return this.Icon.Visibility == System.Windows.Visibility.Visible;
            }
            set
            {
                if (value)
                    this.Icon.Visibility = System.Windows.Visibility.Visible;
                else
                    this.Icon.Visibility = System.Windows.Visibility.Hidden;
            }
        }
        public UIDay()
        {
            InitializeComponent();
        }

        private void RootView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.SelectDateEvent != null && this.Mode == ModeDay.Enable && this.isCurDay==false)
            {
                this.SelectDateEvent(this, this._date);
            }
        }

        private void RootView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.PreviewSelectDateEvent != null)
            {
                this.PreviewSelectDateEvent(this, new EventArgs());
            }
        }
    }
}
