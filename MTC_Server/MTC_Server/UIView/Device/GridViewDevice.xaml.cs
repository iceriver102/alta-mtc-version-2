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
        private int to = 12;
        private int from = 0;
        private int total = 0;
        private bool isPlayingMedia = false;
        private string key
        {
            get
            {
                if (string.IsNullOrEmpty(this.UISearchEdit.Text))
                {
                    return string.Empty;
                }
                return string.Format("%{0}%", this.UISearchEdit.Text.Trim());
            }
        }
        public GridViewDevice()
        {
            InitializeComponent();
        }

        private void UIRootView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Datas = App.curUser.LoadDevices(this.from,ref this.to, out this.total);
            this.LoadGUI();
            this.Focus();
            UIBtnAddDevice.IsEnabled = App.curUser.Permision.mana_device;
        }

        private void LoadGUI()
        {
            this.list_Box_Item.Items.Clear();
            if (to < this.total)
            {
                this.UIRightBtn.Foreground = Brushes.DarkOrange;
            }
            else
            {
                this.UIRightBtn.Foreground = Brushes.Gray;
            }
            if (this.from > 0)
            {
                this.UILeftBtn.Foreground = Brushes.DarkOrange;
            }
            else
            {
                this.UILeftBtn.Foreground = Brushes.Gray;
            }
            if (this.Datas != null)
            {
                foreach (DeviceData d in this.Datas)
                {
                    UIDevice item = new UIDevice();
                    item.Width = 170;
                    item.Height = 240;
                    item.Device = d;
                    item.ViewInfoEvent += Item_ViewInfoEvent;
                    item.DeleteEvent += Item_DeleteEvent;
                    item.PlayMediaEvent += item_PlayMediaEvent;
                    this.list_Box_Item.Items.Add(item);
                }
            }
        }

        void item_PlayMediaEvent(object sender, PlayEvent e)
        {
            if (e.Media != null && !this.isPlayingMedia)
            {
                this.Dispatcher.Invoke(() =>
                {
                    UIView.Media.MediaPlayer player = new UIView.Media.MediaPlayer();
                    player.Width = 1366;
                    player.Height = 668;
                    player.setLeft(0);
                    player.setTop(90);
                    player.Media = e.Media;
                    player.Position = e.Postion;
                    player.CloseEvent += Player_CloseEvent;
                    this.UIRoot.Children.Add(player);
                    this.isPlayingMedia = true;
                });
                
            }
        }

        private void Player_CloseEvent(object sender, Code.Media.MediaData e)
        {
            this.UIRoot.Children.Remove(sender as UIView.Media.MediaPlayer);
            this.isPlayingMedia = false;
        }

        private void Item_DeleteEvent(object sender, DeviceData e)
        {
            if (e != null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Bạn có muốn xoá thiết bị này không?", "Xoá thiết bị", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {

                    UIDevice uid = (this.list_Box_Item.SelectedItem as UIDevice);
                    if (this.list_Box_Item.SelectedIndex != 0)
                    {
                        this.list_Box_Item.SelectedIndex = 0;
                    }
                    else if (this.list_Box_Item.Items.Count > 1)
                    {
                        this.list_Box_Item.SelectedIndex = 1;
                    }
                    uid.Animation_Opacity_View_Frame(false, () =>
                    {
                        this.Datas.Remove(uid.Device);
                        this.list_Box_Item.Items.Remove(uid);
                        if (this.to < this.total)
                        {
                            this.Datas = App.curUser.LoadDevices(this.from, ref this.to, out this.total);
                            this.LoadGUI();
                        }
                    });

                }
            }
        }

        private void Item_ViewInfoEvent(object sender, DeviceData e)
        {
            if (e != null)
            {
                UIDeviceEdit item = new UIDeviceEdit();
                item.setLeft(0);
                item.setTop(90);
                item.Height = 578;
                item.Width = 1366;
                item.Device = e;
                item.CloseEvent += Item_CloseEvent;
                this.UIRoot.Children.Add(item);
            }
        }

        private void UIBtnAddUser_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UIDeviceEdit item = new UIDeviceEdit();
            item.setLeft(0);
            item.setTop(90);
            item.Height = 578;
            item.Width = 1366;
            item.CloseEvent += Item_CloseEvent;
            this.UIRoot.Children.Add(item);
        }

        private void Item_CloseEvent(object sender, DeviceData e)
        {
            if (e != null)
            {
                bool isEdit = false;
                foreach (UIDevice uid in this.list_Box_Item.Items)
                {
                    if (uid.Device.ID == e.ID)
                    {
                        isEdit = true;
                        uid.Device = e;
                    }
                }
                if (!isEdit)
                {
                    this.Datas = App.curUser.LoadDevices(this.from, ref this.to, out this.total);
                    LoadGUI();
                }
            }
            this.UIRoot.Children.Remove(sender as UIDeviceEdit);
        }

        private void UIReload_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.UISearchEdit.reset();
            this.to = 12;
            this.from = 0;
            this.Datas = App.curUser.LoadDevices(this.from, ref this.to, out this.total);
            this.LoadGUI();
        }

        private void UISearchEdit_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string key = this.UISearchEdit.Text.Trim();
                if (key.Length > 3)
                {
                    to = 12;
                    from = 0;
                    this.Datas = App.curUser.FindDevices(this.key, from, ref to, out this.total);
                    this.LoadGUI();
                }
            }
            else if (e.Key == Key.Escape)
            {
                this.UISearchEdit.reset();
                if (!string.IsNullOrEmpty(this.key))
                {
                    this.to = 12;
                    this.from = 0;
                    this.Datas = App.curUser.LoadDevices(this.from, ref this.to, out this.total);
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

        private void UIRootView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                this.UILeftBtn_MouseLeftButtonUp(null,null);
                this.Focus();
            }
            else if (e.Key == Key.Right)
            {
                this.UIRightBtn_MouseLeftButtonUp(null, null);
                this.Focus();
            }
        }

        private void UILeftBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.from > 0)
            {
                this.to = this.from;
                this.from -= 12;
                if (string.IsNullOrEmpty(this.key))
                    this.Datas = App.curUser.LoadDevices(this.from, ref this.to, out this.total);
                else
                    this.Datas = App.curUser.FindDevices(this.key, this.from, ref this.to, out this.total);
                LoadGUI();
            }
        }

        private void UIRightBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.to < this.total)
            {                
                this.from = this.to;
                this.to -= 12;
                if (string.IsNullOrEmpty(this.key))
                    this.Datas = App.curUser.LoadDevices(this.from, ref this.to, out this.total);
                else
                    this.Datas = App.curUser.FindDevices(this.key, this.from,ref this.to, out this.total);
                LoadGUI();
            }
        }
    }
}
