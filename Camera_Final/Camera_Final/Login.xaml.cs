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
using Camera_Final.Code;

namespace Camera_Final
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Title_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) 
            {
                this.DragMove();
            }
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(this.UIUser.Text!=string.Empty)
            this.Place_Holder_User.Opacity = 0;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.UIPass.Password != "")
            {
                this.Place_Holder_Pass.Opacity = 0;
            }
        }
        private void UIBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.UIUser.isEmpty())
            {
                UIWaring.Text = "* Tên đăng nhập không được để trống!";
                return;
            }
            if (this.UIPass.Password == "")
            {
                UIWaring.Text = "* Mật khẩu không được để trống!";
                return;
            }

            string hash = (this.UIUser.Text + FunctionStatics.getCPUID() + this.UIPass.Password.toMD5()).toMD5();
            User u = App.DataUser.Login(hash);
            if (u == null)
            {
                UIWaring.Text = "* Tên đăng nhập hoặc mật khẩu không đúng!";
                return;
            }
            UIWaring.Text = string.Empty;
            App.User = u;
            Application.Current.MainWindow = new MainWindow();
            Application.Current.MainWindow.Show();
            this.Close();

        }

        private void UIBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            this.UIBtn.Background = new SolidColorBrush(new Color() { A = 100, B = 3, R = 6, G = 4 });
        }

        private void UIBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            this.UIBtn.Background = new SolidColorBrush(new Color() { A = 255, B = 0, R = 255, G = 85 });
        }

        private void UIBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.UIBtn.Background = new SolidColorBrush(new Color() { A = 180, B = 0, R = 255, G = 85 });
        }

        private void UIBtn_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.UIBtn.Background = new SolidColorBrush(new Color() { A = 100, B = 3, R = 6, G = 4 });
        }
    }
}
