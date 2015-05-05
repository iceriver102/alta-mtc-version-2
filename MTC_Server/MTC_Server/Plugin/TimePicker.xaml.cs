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

namespace Alta.Plugin
{
    /// <summary>
    /// Interaction logic for TimePicker.xaml
    /// </summary>
    public partial class TimePicker : UserControl
    {
        public event EventHandler<TimeSpan> TimeChange;
        public TimePicker()
        {
            InitializeComponent();
           
        }

        public TimeSpan getTime()
        {
            return new TimeSpan(Convert.ToInt32(this.UIHour.Value),Convert.ToInt32(this.UIMinute.Value),Convert.ToInt32(this.UISecond.Value));
        }

        private void RootView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void UIHour_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (TimeChange != null)
            {
                TimeChange(this, this.getTime());
            }
        }
    }
}
