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
using MTC_Server.Code;
using MTC_Server.Code.Device;
using Alta.Class;

namespace MTC_Server.UIView.Device
{
    /// <summary>
    /// Interaction logic for UIDevice.xaml
    /// </summary>
    public partial class UIDevice : UserControl
    {
        public event EventHandler<DeviceData> ViewInfoEvent;
        public event EventHandler<DeviceData> DeleteEvent;
        public event EventHandler<DeviceData> PlayMediaEvent;
        private DeviceData d;
        public DeviceData Device
        {
            get
            {
                return d;
            }
            set
            {
                d = value;
                this.LoadGUI(value);
            }
        }
        public UIDevice()
        {
            InitializeComponent();
        }

        private void UIRootView_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoadGUI(this.d);
        }
        private void LoadGUI(DeviceData d)
        {
            if(d.TypeDevice!=null && !string.IsNullOrEmpty(d.TypeDevice.Icon))
            {
                this.UIIcon.Text = d.TypeDevice.Icon;
            }
            this.UIIP.Text = d.IP;
            this.UITitle.Text = d.Name;
            this.UITime.Text = d.Time.format();
            this.UIType.Text = d.TypeDevice.Name;
            if (d.Status)
            {
                this.UIStatus.Text = Define.Fonts["fa-unlock"].Code;
            }else
            {
                this.UIStatus.Text = Define.Fonts["fa-lock"].Code;
            }
        }

        private void UIStatus_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (App.curUser.Permision.mana_device)
            {
                this.Device.Status ^= true;
                if (this.Device.Status)
                {
                    this.UIStatus.Text = Define.Fonts["fa-unlock"].Code;
                }
                else
                {
                    this.UIStatus.Text = Define.Fonts["fa-lock"].Code;
                }
                this.Device.setStatus();
            }
        }

        private void UIBtnInfo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.ViewInfoEvent != null)
                this.ViewInfoEvent(this, this.Device);
        }
        private void UIBtnDelete_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.DeleteEvent != null)
            {
                this.DeleteEvent(this, this.Device);
            }
        }
    }
}
