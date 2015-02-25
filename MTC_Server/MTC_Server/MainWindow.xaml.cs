using MTC_Server.Code;
using MySql.Data.MySqlClient;
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

namespace MTC_Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double W = 1600;
        private const double H = 900;
        public User u;
        public MainWindow()
        {
            InitializeComponent();
        }

        public User Info(int id)
        {
            User u = null;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "CALL `InfoUser` (@userid);";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userid", id);
                        var tmp = cmd.ExecuteScalar();
                        if (Convert.ToInt32(tmp) > 0)
                        {
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                u = reader.toUser();
                            }
                        }
                    };
                    conn.Close();
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return u;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.UIRoot.RenderTransform = new ScaleTransform(this.Width / W, this.Height / H);
            u = Info(App.curUserID);
            MessageBox.Show(u.Full_Name);
        }
    }
}
