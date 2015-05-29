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
using System.Windows.Shapes;

namespace MTC_Server
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.UIUser.Text = App.Registry.Read("MTC_USER");
            this.UIKey.Text = App.Registry.Read("MTC_KEY");
            if(App.setting.EndDate!= App.Zero)
            {
                this.UIDate.Text = App.setting.EndDate.ToString("dd-MM-yyyy");
            }
        }
    }
}
