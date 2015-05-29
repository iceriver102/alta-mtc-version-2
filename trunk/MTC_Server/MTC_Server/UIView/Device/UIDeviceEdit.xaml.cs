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
using MTC_Server.Code.Device;
using Alta.Class;

namespace MTC_Server.UIView.Device
{
    /// <summary>
    /// Interaction logic for UIDeviceEdit.xaml
    /// </summary>
    public partial class UIDeviceEdit : UserControl
    {
        public event EventHandler<DeviceData> CloseEvent;
        private DeviceData d;
        public DeviceData Device
        {
            get
            {
                return this.d;
            }
            set
            {
                this.d = value;
            }
        }
        public UIDeviceEdit()
        {
            InitializeComponent();
        }

        private void UIRootView_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.TypeDevices != null)
            {
                foreach(TypeDevice type in App.TypeDevices)
                {
                    this.UIType.Items.Add(type);
                }
            }
            this.LoadGUI(this.Device);
            this.Focus();
        }
        private void LoadGUI(DeviceData d)
        {
            if (this.Device == null)
            {
                this.UITitle.Text = "Thêm thiết bị mới";
                this.UIName.reset();
                this.UIIP.reset();
                this.UIType.SelectedIndex =- 1;
                UIContent.Height = 445;
            }
            else
            {
                this.UIName.Text = d.Name;
                this.UIIP.Text = d.IP;
                this.UITitle.Text = d.Name;
                this.UIType.SelectedItem = d.TypeDevice;
                UIContent.Height = 505;
            }
        }

        private void UIBtnClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.CloseEvent != null)
            {
                this.CloseEvent(this, null);
            }
        }

        private void UIBtnSave_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(this.UIName.Text))
            {
                MessageBox.Show("Tên thiết bị không được để trống");
                return;
            }
            if (this.UIType.SelectedIndex == -1)
            {
                MessageBox.Show("Chon loại thiết bị");
                return;
            }
            if (!this.UIIP.Text.isIP())
            {
                MessageBox.Show("Địa chỉ IP không đúng định dạng");
                return;
            }
            if (string.IsNullOrEmpty(this.UIPass.Password) && this.Device == null)
            {
                MessageBox.Show("Mật khẩu không được để trống");
                return;
            }
            if (this.UIPass.Password != this.UIPass_Again.Password) 
            {
                MessageBox.Show("Mật khẩu không khớp. Kiểm tra lại!");
                return;
            }
            if (this.Device != null && this.Device.Pass != this.UIPass_Old.Password.MD5String())
            {
                MessageBox.Show("Mật khẩu đúng. Vui lòng kiểm tra lại!");
                return;
            }
            if(this.Device== null)
            {
                DeviceData d = new DeviceData();
                d.Name = this.UIName.Text;
                d.IP = this.UIIP.Text;
                d.Pass = this.UIPass.Password.MD5String();
                d.Type = (this.UIType.SelectedItem as TypeDevice).ID;
                int result= DeviceData.Insert(d);
                if(result <=0)
                {
                    MessageBox.Show("Không thể kết nối CSDL");
                    return;
                }
                d.ID = result;
                if (this.CloseEvent != null)
                {
                    this.CloseEvent(this, d);
                }
            }else
            {
                this.d.Name = this.UIName.Text;
                this.d.Type = (this.UIType.SelectedItem as TypeDevice).ID;
                this.d.IP = this.UIIP.Text;
                if (!string.IsNullOrEmpty(this.UIPass.Password))
                {
                    this.d.Pass = this.UIPass.Password.MD5String();
                }
                this.d.save();
                if (this.CloseEvent != null)
                {
                    this.CloseEvent(this, this.d);
                }
            }
        }

        private void UIBtnReset_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.LoadGUI(this.Device);
        }

        private void UIRootView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.UIBtnClose_MouseLeftButtonUp(null, null);
            }
        }
    }
}
