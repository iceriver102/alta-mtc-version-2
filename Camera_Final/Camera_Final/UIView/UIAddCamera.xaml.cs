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
    /// Interaction logic for UIAddCamera.xaml
    /// </summary>
    public partial class UIAddCamera : UserControl
    {
        public event EventHandler CloseEvent;
        public UIAddCamera()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (CloseEvent != null)
            {
                CloseEvent(this, new EventArgs());
            }
        }

        private void SaveCamera(object sender, MouseButtonEventArgs e)
        {
            if (this.UIName.isEmpty())
            {
                MessageBox.Show("Tên Camera không được để trống", "Thông báo");
                return;
            }
            if (this.UIIP.isEmpty())
            {
                MessageBox.Show("Địa chỉ ip không được để trống", "Thông báo");
                return;
            }
            if (this.UIUser.isEmpty())
            {
                MessageBox.Show("Tên đăng nhập không được phép để trống","Thông báo");
                return;
            }
            if (this.UIPass.Password == "")
            {
                MessageBox.Show("Mật khấu đăng nhập không được phép để trống", "Thông báo");
                return;
            }
            if (this.UIPass.Password != UIPass_Copy.Password)
            {
                MessageBox.Show("Xác nhập mật khẩu không khớp", "Thông báo");
                return;
            }
            if (this.UIPort.isEmpty())
            {
                MessageBox.Show("Cổng truy câp camera không được phép để trống", "Thông báo");
                return;
            }
            if (this.UIPortRtsp.isEmpty())
            {
                MessageBox.Show("Cổng truy cập rtsp không được phép để trống", "Thông báo");
                return;
            }
            Code.Camera cam = new Code.Camera(this.UIIP.Text.Trim());
            cam.admin = this.UIUser.Text.Trim();
            cam.pass = this.UIPass.Password;
            cam.name = this.UIName.Text;
            cam.port = Convert.ToInt32(this.UIPort.Text);
            cam.port_rtsp = Convert.ToInt32(this.UIPortRtsp.Text);
            App.DataCamera.Add(cam);
            if (this.CloseEvent != null)
            {
                this.CloseEvent(this, new EventArgs());
            }
        }

        private void TestConnectCamera(object sender, MouseButtonEventArgs e)
        {
            if (this.UIName.isEmpty())
            {
                MessageBox.Show("Tên Camera không được để trống", "Kết nối không thành công");
                return;
            }
            if (this.UIIP.isEmpty())
            {
                MessageBox.Show("Địa chỉ ip không được để trống", "Kết nối không thành công");
                return;
            }
            if (this.UIUser.isEmpty())
            {
                MessageBox.Show("Tên đăng nhập không được phép để trống", "Kết nối không thành công");
                return;
            }
            if (this.UIPass.Password == "")
            {
                MessageBox.Show("Mật khấu đăng nhập không được phép để trống", "Kết nối không thành công");
                return;
            }
            if (this.UIPass.Password != UIPass_Copy.Password)
            {
                MessageBox.Show("Xác nhập mật khẩu không khớp", "Kết nối không thành công");
                return;
            }
            if (this.UIPort.isEmpty())
            {
                MessageBox.Show("Cổng truy câp camera không được phép để trống", "Kết nối không thành công");
                return;
            }
            if (this.UIPortRtsp.isEmpty())
            {
                MessageBox.Show("Cổng truy cập rtsp không được phép để trống", "Kết nối không thành công");
                return;
            }
            if(this.tryLoginCamera(this.UIIP.Text.Trim(),this.UIPass.Password,this.UIUser.Text.Trim(),Convert.ToInt32(this.UIPort.Text)))
            {
                MessageBox.Show("Kết nối thành công", "thông báo");
            }
            else
            {
                MessageBox.Show("Kết nối không thành công", "Thông báo");
            }
            
        }
        private CHCNetSDK.NET_DVR_DEVICEINFO_V30 m_struDeviceInfo;
        private bool tryLoginCamera(string ip, string pass, string admin, int port)
        {
            if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(admin))
                return false;
           
            int m_lUserID = CHCNetSDK.NET_DVR_Login_V30(ip, port, admin, pass, ref m_struDeviceInfo);
            if (m_lUserID == -1)
                return false;
            return true;
        }
    }
}
