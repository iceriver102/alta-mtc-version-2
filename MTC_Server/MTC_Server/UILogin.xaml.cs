using Alta.Class;
using MTC_Server.Code.User;
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

namespace MTC_Server
{
    /// <summary>
    /// Interaction logic for UILogin.xaml
    /// </summary>
    public partial class UILogin : Window
    {
        public UILogin()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void WindowClose(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void CheckBox_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock tmp = (sender as CheckBox).Content as TextBlock;
            tmp.TextDecorations = TextDecorations.Underline;
        }

        private void UIAutoLogin_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock tmp = (sender as CheckBox).Content as TextBlock;
            tmp.TextDecorations = null;
        }
        public void LoadData()
        {
            EasyTimer.SetTimeout(() =>
            {
                this.Dispatcher.Invoke(() => {
                    Application.Current.MainWindow = new MainWindow();
                    Application.Current.MainWindow.Show();
                    this.Close();
                });
            }, 3,System.Threading.ApartmentState.STA);
           
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
            catch (MySqlException)
            {
                MessageBox.Show("Không thể kết nối với csdl");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return result;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.UITxtName.Text))
            {                
                this.UIErr.Text = "* Tên đăng nhập không được để trống!";
                this.UIErr.Animation_Opacity_View_Frame(true);
                return;
            }
            if (this.UIErr.Visibility == System.Windows.Visibility.Visible && this.UIErr.Opacity!=0)
            {
                this.UIErr.Animation_Opacity_View_Frame(false);
            }
            if (string.IsNullOrEmpty(UIPassword.Password))
            {
                this.UIErPass.Animation_Opacity_View_Frame(true);
                return;
            }
            if (this.UIErPass.Visibility == System.Windows.Visibility.Visible && this.UIErPass.Opacity != 0)
            {
                this.UIErPass.Animation_Opacity_View_Frame(false);
            }
            int result = LoginData(UITxtName.Text, FunctionStatics.MD5String(UIPassword.Password));
            if (result == 0)
            {
                this.UIErr.Text = "* Tên đăng nhập hoặc mật khẩu không đúng!";
                this.UIErr.Animation_Opacity_View_Frame(true);
                return;
            }
            else
            {
                this.UIErr.Text = string.Empty;
                this.UIErr.Animation_Opacity_View_Frame(false);
                App.curUserID = result; 
                App.curUser = UserData.Info(App.curUserID);                
                if (this.UIAutoLogin.IsChecked.Value)
                {
                    App.cache.hashUserName = App.getHash(result);
                    App.cache.userName = Encrypt.EncryptString(this.UITxtName.Text.Trim(), FunctionStatics.getCPUID());
                    AltaCache.Write(App.CacheName, App.cache);
                }
                else
                {
                    App.cache.userName = string.Empty;
                    App.cache.hashUserName = string.Empty;
                }
                this.UIAvatar.Text = App.curUser.Full_Name[0].ToString();
                this.UIFullName.Text = App.curUser.Full_Name;
                this.UILoginForm.Animation_Translate_Frame(double.NaN, double.NaN, 400, double.NaN,500);
                this.UILoginSusscess.Animation_Translate_Frame(-400, double.NaN, 0, double.NaN, 500, () => { LoadData(); });
                
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.cache.autoLogin)
            {
                this.UIAutoLogin.IsChecked = true;
            }
            if (App.curUserID > 0)
            {
                if (App.curUser == null)
                {
                    App.curUser = UserData.Info(App.curUserID);
                    this.UIAvatar.Text = App.curUser.Full_Name[0].ToString().ToUpper();
                    this.UIFullName.Text = App.curUser.Full_Name;
                    this.UILoginSusscess.Animation_Translate_Frame(-400, double.NaN, 0, double.NaN, 500, () => { LoadData(); });
                }
                else
                {
                    this.UIAvatar_off.Text = App.curUser.Full_Name[0].ToString().ToUpper();
                    this.UIFullName_off.Text = App.curUser.Full_Name;
                    this.UILoginLogOff.Animation_Translate_Frame(-400, double.NaN, 0, double.NaN, 500);
                }
                this.UILoginForm.Animation_Translate_Frame(double.NaN, double.NaN, 400, double.NaN, 500);
                
            }
            if (!string.IsNullOrEmpty(App.cache.userName))
            {
                this.UITxtName.Text = Encrypt.DecryptString(App.cache.userName, FunctionStatics.getCPUID());
                this.UIMaskUserName.Animation_Opacity_View_Frame(false, null, 200);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.cache.autoLogin = this.UIAutoLogin.IsChecked.Value;
            AltaCache.Write(App.CacheName, App.cache);
        }

        private void UITxtName_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(this.UITxtName.Text) || string.IsNullOrEmpty(this.UIPassword.Password))
            {
                this.UILoginButton.IsEnabled = false;
                return;
            }
            else
            {
                UILoginButton.IsEnabled = true;
            }
            if (e.Key == Key.Enter)
            {
                this.Button_Click(null, null);
            }
        }

        private void UITxtName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.UITxtName.Text))
            {
                this.UIMaskUserName.Animation_Opacity_View_Frame(true, null, 200);
            }
        }

        private void UITxtName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.UITxtName.Text))
                this.UIMaskUserName.Animation_Opacity_View_Frame(false, null, 200);
        }

        private void UIPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.UIPassword.Password))
            {
                this.UIMaskPassword.Animation_Opacity_View_Frame(true, null, 200);
            }
        }

        private void UIPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.UIPassword.Password))
                this.UIMaskPassword.Animation_Opacity_View_Frame(false, null, 200);
        }

        private void ReLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(UIPassword_off.Password))
            {
                this.UIErrRelogin.Text = "* Mật khẩu đăng nhập không được để trống!";
                this.UIErrRelogin.Animation_Opacity_View_Frame(true);
                return;
            }
            int result = LoginData(App.curUser.User_Name, FunctionStatics.MD5String(UIPassword_off.Password));
            if (result == 0)
            {
                this.UIErrRelogin.Text = "* Mật khẩu đăng nhập không đúng!";
                this.UIErrRelogin.Animation_Opacity_View_Frame(true);
                return;
            }
            else
            {
                App.curUserID = result;
                App.curUser = UserData.Info(App.curUserID);
                if (App.cache.autoLogin)
                {
                    App.cache.hashUserName = App.getHash(result);
                    App.cache.userName = Encrypt.EncryptString(this.UITxtName.Text.Trim(), FunctionStatics.getCPUID());
                    AltaCache.Write(App.CacheName, App.cache);
                }
                else
                {
                    App.cache.userName = string.Empty;
                    App.cache.hashUserName = string.Empty;
                }
                this.UIAvatar.Text = App.curUser.Full_Name[0].ToString();
                this.UIFullName.Text = App.curUser.Full_Name;
                this.UILoginLogOff.Animation_Translate_Frame(double.NaN, double.NaN, 400, double.NaN, 500);
                this.UILoginSusscess.Animation_Translate_Frame(-400, double.NaN, 0, double.NaN, 500, () => { LoadData(); });

            }
        }

        private void UIPassword_off_KeyUp(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.UIPassword_off.Password) && e.Key== Key.Enter)
            {
                this.ReLogin_Click(null, null);
            }
            else if (!string.IsNullOrEmpty(this.UIPassword_off.Password))
            {
                UILoginButton_Off.IsEnabled = true;
            }
        }

        private void UIMaskPassword_off_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.UIPassword_off.Password))
                this.UIMaskPassword_off.Animation_Opacity_View_Frame(false, null, 200);
        }

        private void UIMaskPassword_off_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.UIPassword_off.Password))
                this.UIMaskPassword_off.Animation_Opacity_View_Frame(true, null, 200);
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            App.curUserID = 0;
            App.curUser = null;
            this.UITxtName.Focus();
            this.UITxtName.Text = string.Empty;
            this.UILoginLogOff.Animation_Translate_Frame(double.NaN, double.NaN, 400, double.NaN, 500);
            this.UILoginForm.Animation_Translate_Frame(-400, double.NaN, 0, double.NaN, 500);
        }        

    }
}
