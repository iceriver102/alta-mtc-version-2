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
    /// Interaction logic for UIWeek.xaml
    /// </summary>
    public partial class UIWeek : UserControl
    {
        private DateTime _sunday;
        public DateTime Sunday
        {
            get
            {
                return this._sunday;
            }
            private set
            {
                this._sunday = value;
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
                if (this._date != null)
                {
                    int tmp =(int) this._date.DayOfWeek;
                    this.Sunday = this._date.AddDays(-tmp);
                    this.UpdateLayout();
                }
            }
        }
        public UIWeek()
        {
            InitializeComponent();
        }

        public void UpdateLayout()
        {
            if (this.Sunday != null)
            {
                int i = 0;
                foreach (UIElement E in this.UIRoot.Children)
                {
                    if (E is UIDayOfWeek)
                    {
                        UIDayOfWeek Day = E as UIDayOfWeek;
                        Day.Date = this.Sunday.AddDays(i++);
                        if (Day.Date.Date == this.Date.Date)
                        {
                            Day.Active = true;
                        }
                        else
                        {
                            Day.Active = false;
                        }
                    }
                }
            }
        }

        private void RootView_Loaded(object sender, RoutedEventArgs e)
        {
            this.UpdateLayout();
        }
    }
}
