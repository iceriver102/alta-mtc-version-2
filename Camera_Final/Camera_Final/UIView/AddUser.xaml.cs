using Camera_Final.Code;
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

namespace Camera_Final.UIView
{
    /// <summary>
    /// Interaction logic for AddUser.xaml
    /// </summary>
    public partial class AddUser : UserControl
    {
        public event EventHandler CloseEvent;
        private User u;
        public User User
        {
            get
            {
                return u;
            }
            set
            {
                u= value;
                if (u != null)
                {
                    UITitle.Text = "Chỉnh sửa User";
                    UIName.Text = u.FullName;
                    UIUserName.Text = u.user_name;
                    UIType.Text = u.type.ToString();
                    UIUserName.IsReadOnly = true;
                    
                }
                else
                {
                    UITitle.Text = "Thêm User";
                    this.UIOldPass.Visibility = System.Windows.Visibility.Hidden;
                    this.UITitleOldPass.Visibility = System.Windows.Visibility.Hidden;
                }
            }
        }
        public AddUser()
        {
            InitializeComponent();
        }

        private void SaveUser(object sender, MouseButtonEventArgs e)
        {
            if (this.UIName.isEmpty())
            {
                MessageBox.Show("Tên User không được phép để trống", "Thông báo");
                return;
            }
            if (this.UIUserName.isEmpty())
            {
                MessageBox.Show("Tên đăng nhập không được phép để trống", "Thông báo");
                return;
            }
            if (this.UIType.isEmpty())
            {
                MessageBox.Show("Loại User không được phép để trống", "Thông báo");
                return;
            }
            if (this.u == null && this.UIPass.Password == string.Empty)
            {
                MessageBox.Show("Mật khẩu đăng nhập không được phép để trống", "Thông báo");
                return;
            }
            if (this.UIPass.Password != this.UIPassAgain.Password)
            {
                MessageBox.Show("Mật khẩu không khớp vui lòng kiểm tra lại", "Thông báo");
                return;
            }
            if (this.u != null && this.UIOldPass.Password.toMD5() != this.u.Pass)
            {
                MessageBox.Show("Mật khẩu cũ không đúng vui lòng kiểm tra lại", "Thông báo");
            }
            if (this.u == null)
            {
                User u = new User(0);
                u.FullName = this.UIName.Text;
                u.Pass = this.UIPass.Password.toMD5();
                u.type = Convert.ToInt32(this.UIType.Text);
                u.user_name = this.UIUserName.Text;
            }
            else
            {
                u.FullName = this.UIName.Text;
                if(this.UIPass.Password!=string.Empty)
                u.Pass = this.UIPass.Password.toMD5();
                u.type = Convert.ToInt32(this.UIType.Text);
            }
            if (this.CloseEvent != null)
            {
                this.CloseEvent(this, new EventArgs());
            }
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.CloseEvent != null)
            {
                this.CloseEvent(this, new EventArgs());
            }
        }
    }
}
