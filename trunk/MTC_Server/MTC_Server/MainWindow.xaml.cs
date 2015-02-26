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
        private const double W = 1366;
        private const double H = 768;
        private bool _menu_active = false;
        public User u;
        public bool menuActivate
        {
            get
            {
                return this._menu_active;
            }
            set
            {
                this._menu_active = value;
                if (this._menu_active)
                {
                    UIMenu.Animation_Translate_Frame(double.NaN, double.NaN, 0, 0);
                    foreach(UIElement e in this.UIMenu.Children)
                    {
                        if(e is UIView.MenuItem)
                        {
                            UIView.MenuItem item = e as UIView.MenuItem;
                            item.Animation_Opacity_View_Frame(true);
                        }
                    }
                }
                else
                {
                    UIMenu.Animation_Translate_Frame(double.NaN,double.NaN, 0, -64);
                    foreach (UIElement e in this.UIMenu.Children)
                    {
                        if (e is UIView.MenuItem)
                        {
                            UIView.MenuItem item = e as UIView.MenuItem;
                            item.Animation_Opacity_View_Frame(false);
                        }
                    }
                }
            }
        }
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
            this.menuActivate = false;
            u = Info(App.curUserID);
        }

        private void Asset_Images_btn_menu_png_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.menuActivate = !this.menuActivate;
        }
    }
}
