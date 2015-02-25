using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Alta.Class;

namespace MTC_Server
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Config setting;
        public static string FileName="setting.xml";
        public static int curUserID = 0;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            setting = Config.Read(FileName);
            this.MainWindow = new Login();
            this.MainWindow.Show();
        }
    }
}
