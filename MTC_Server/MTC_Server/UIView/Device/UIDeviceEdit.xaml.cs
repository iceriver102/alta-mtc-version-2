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
                // this.UIName.Text = string.Empty;
                this.UIName.reset();
                this.UIIP.reset();
                this.UIComment.reset();
                this.UIType.SelectedIndex =- 1;
            }
            else
            {
                this.UIName.Text = d.Name;
                this.UIIP.Text = d.IP;
                this.UITitle.Text = d.Name;
                this.UIComment.Text = d.Comment;
                this.UIType.SelectedItem = d.TypeDevice;
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
            if(this.Device== null)
            {
                DeviceData d = new DeviceData();
                d.Name = this.UIName.Text;
                d.IP = this.UIIP.Text;
                d.Comment = this.UIComment.Text;
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
                this.d.Comment = this.UIComment.Text;
                this.d.Type = (this.UIType.SelectedItem as TypeDevice).ID;
                this.d.IP = this.UIIP.Text;
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
