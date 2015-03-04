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
using MTC_Server.Code.Device;

namespace MTC_Server.UIView.Device
{
    /// <summary>
    /// Interaction logic for GridViewDevice.xaml
    /// </summary>
    public partial class GridViewDevice : UserControl
    {
        private List<DeviceData> Datas;
        private int to = 10;
        private int from = 0;
        private int total = 0;
        private string key = string.Empty;
        public GridViewDevice()
        {
            InitializeComponent();
        }

        private void UIRootView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Datas = App.curUser.LoadDevices(this.from,this.to,out this.total);
            this.LoadGUI();
            this.Focus();
        }

        private void LoadGUI()
        {
            this.list_Box_Item.Items.Clear();
            if (this.Datas != null)
            {
                foreach(DeviceData d in this.Datas)
                {

                }
            }
        }

        private void UIBtnAddUser_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void UIReload_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.to = 10;
            this.from = 0;
            this.Datas = App.curUser.LoadDevices(this.from,this.to,out this.total);
            this.LoadGUI();
        }

        private void UISearchEdit_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string key = this.UISearchEdit.Text.Trim();
                if (key.Length > 3)
                {
                    this.key = string.Format("%{0}%", key);
                    to = 10;
                    from = 0;
                    this.Datas = App.curUser.FindDevices(this.key, from, to, out this.total);
                    this.LoadGUI();
                }
            }
            else if (e.Key == Key.Escape)
            {
                this.UISearchEdit.Text = "";
                if (!string.IsNullOrEmpty(this.key))
                {
                    this.to = 10;
                    this.from = 0;
                    this.Datas = App.curUser.LoadDevices(this.from, this.to, out this.total);
                    this.LoadGUI();
                }
                this.Focus();
            }
        }

        private void UISearchEdit_LostFocus(object sender, RoutedEventArgs e)
        {
            this.UIOverText.Animation_Opacity_View_Frame(true);
        }

        private void UISearchEdit_GotFocus(object sender, RoutedEventArgs e)
        {
            this.UIOverText.Animation_Opacity_View_Frame(false);
        }
    }
}
