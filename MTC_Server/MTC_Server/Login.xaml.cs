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
using System.Windows.Shapes;
using Alta.Class;
using MTC_Server.Code;

namespace MTC_Server
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        Brush tmpColor;
        public Login()
        {
            InitializeComponent();
            tmpColor = UIUserName.Background;
            UIUserName.Focus();
        }

        private void UIUserName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox txt = sender as TextBox;
                txt.Background = Brushes.White;
                UIPlaceHolder.Visibility = Visibility.Collapsed;
            }
            else if (sender is PasswordBox)
            {
                PasswordBox pass = sender as PasswordBox;
                pass.Background = Brushes.White;
                UIPlaceHolder_Copy.Visibility = Visibility.Collapsed;
            }
        }

        private void UIPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox txt = sender as TextBox;
                txt.Background = tmpColor;
                if (string.IsNullOrEmpty(txt.Text))
                    UIPlaceHolder.Visibility = Visibility.Visible;
            }
            else if (sender is PasswordBox)
            {
                PasswordBox pass = sender as PasswordBox;
                pass.Background = tmpColor;
                if (string.IsNullOrEmpty(pass.Password))
                    UIPlaceHolder_Copy.Visibility = Visibility.Visible;
            }
        }

        private void UIUserName_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(this.UIUserName.Text) || string.IsNullOrEmpty(this.UIPassword.Password))
            {
                this.UILoginButton.IsEnabled = false;
            }
            else
            {
                UILoginButton.IsEnabled = true;
            }
            if(e.Key== Key.Enter)
            {
                this.UILoginButton_Click(null, null);
            }
        }

        private void UILoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(UIUserName.Text))
            {
                MessageBox.Show("Tên đăng nhập không được để trống!");
                return;
            }
            if (string.IsNullOrEmpty(UIPassword.Password))
            {
                MessageBox.Show("Mật khẩu không được để trống!");
                return;
            }
            int result = LoginData(UIUserName.Text, FunctionStatics.MD5String(UIPassword.Password));
            if (result == 0)
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không khớp nhau!");
                return;
            }
            else
            {
                App.curUserID = result;
                MessageBox.Show("Đăng nhập thành công!");   
                Application.Current.MainWindow = new MainWindow();
                Application.Current.MainWindow.Show();
                if (App.cache.autoLogin)
                {
                    App.cache.hashUserName = App.getHash(result);
                    AltaCache.Write(App.CacheName, App.cache);
                }
                this.Close();
            }
        }

       

        private int LoginData(string _username, string _password)
        {
            int result = 0;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "SELECT  `Fc_Login` (@name_user , @pass_user) AS  `user_id`";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name_user", _username);
                        cmd.Parameters.AddWithValue("@pass_user", _password);
                        var tmp = cmd.ExecuteScalar();
                        result = (int)tmp;
                    };
                    conn.Close();
                };
            }
            catch(MySqlException)
            {
                MessageBox.Show("Không thể kết nối với csdl");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return result;
        }
    }
}
