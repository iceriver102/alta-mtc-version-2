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
    /// Interaction logic for UIDayOfWeek.xaml
    /// </summary>
    public partial class UIDayOfWeek : UserControl
    {
        public Thickness BorderThickness
        {
            get
            {
                return this.UIRoot.BorderThickness;
            }
            set
            {
                this.UIRoot.BorderThickness = value;
            }
        }
        public event EventHandler<DateTime> SelectDateEvent;
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
                this.UIDay.Text = string.Format("{0:00}", this._date.Day);
                this.UITextDayOfWeek.Text = string.Format("{0}", this._date.DayOfWeek);
            }
        }
        private bool _active = false;
        public bool Active
        {
            get
            {
                return this._active;
            }
            set
            {
                this._active = value;
                if (!this._active)
                {
                    this.UIRoot.Background = Brushes.Transparent;
                }
                else
                {
                    this.UIRoot.Background = Brushes.OrangeRed;
                }
            }
        }
        public UIDayOfWeek()
        {
            InitializeComponent();
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!this._active && this.SelectDateEvent != null)
            {
                this.SelectDateEvent(this, this._date);
            }
        }
    }
}
