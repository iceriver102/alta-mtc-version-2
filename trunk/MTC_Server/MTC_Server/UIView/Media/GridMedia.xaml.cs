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
using MTC_Server.Code.Media;
using MTC_Server.Code.User;

namespace MTC_Server.UIView.Media
{
    /// <summary>
    /// Interaction logic for GridMedia.xaml
    /// </summary>
    public partial class GridMedia : UserControl
    {
        public event EventHandler BackEvent;
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

        private UserData u;

        public UserData User
        {
            get
            {
                return this.u;
            }
            set
            {
                this.u = value;
                if (this.u == null)
                {
                    UIBtnAddMedia.setLeft(15);
                    UIBtnBack.Visibility = Visibility.Hidden;
                    UIReload.setLeft(60);
                }
                else
                {
                    UIBtnAddMedia.setLeft(55);
                    UIBtnBack.Visibility = Visibility.Visible;
                    UIReload.setLeft(100);
                }
            }
        }
        private List<Code.Media.MediaData> Datas;
        private int to = 12;
        private int from = 0;
        private int totalMedia = 0;
        private int _type = 1;
        public int TypeMedia
        {
            get
            {
                return this._type;
            }
            set
            {
                this._type = value;
            }
        }
        public GridMedia()
        {
            InitializeComponent();
        }

        private void UIBtnAddUser_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UIMediaEdit item = new UIMediaEdit();
            item.Type = this.TypeMedia;
            item.setLeft(0);
            item.setTop(90);
            item.Width = 1366;
            item.Height = 578;
            item.CloseEvent += Item_CloseEvent;
            this.UIRoot.Children.Add(item);
        }

        private void Item_CloseEvent(object sender, Code.Media.MediaData e)
        {
            if (e != null)
            {
                bool isEdit = false;
                if (e.TypeMedia.Code.ToUpper() == "FILE")
                {

                    foreach (UIMedia m in this.list_Box_Item.Items)
                    {
                        if (m.Media.ID == e.ID)
                        {
                            m.Media = e;
                            isEdit = true;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (UIcamera m in this.list_Box_Item.Items)
                    {
                        if (m.Media.ID == e.ID)
                        {
                            m.Media = e;
                            isEdit = true;
                            break;
                        }
                    }
                }
                if (!isEdit)
                {
                    if (this.User == null)
                    {
                        this.Datas = App.curUser.LoadMedias(this.from, this.to, out this.totalMedia, this.TypeMedia);
                    }
                    else
                    {
                        this.Datas = this.User.LoadMedias(this.from, this.to, out this.totalMedia, this.TypeMedia,true);
                    }
                    this.LoadGUI();
                }
            }
            this.UIRoot.Children.Remove(sender as UIMediaEdit);
        }

        private void UIRootView_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.User == null)
            {
                this.Datas = App.curUser.LoadMedias(from, to, out totalMedia, this.TypeMedia);
            }
            else
            {
                this.Datas = this.User.LoadMedias(this.from, this.to, out this.totalMedia, this.TypeMedia,true);
            }
            this.LoadGUI();
            this.Focus();
        }
        public void LoadGUI()
        {
            this.list_Box_Item.Items.Clear();
            if (to < this.totalMedia)
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
                foreach (Code.Media.MediaData m in this.Datas)
                {
                    if (m.TypeMedia.Code.ToUpper() == "FILE")
                    {
                        UIMedia item = new UIMedia();
                        item.Media = m;
                        item.Height = 240;
                        item.Width = 170;
                        item.ViewInfoMediaEvent += Item_ViewInfoMediaEvent;
                        item.DeleteMediaEvent += Item_DeleteMediaEvent;
                        item.PlayMediaEvent += Item_PlayMediaEvent;
                        this.list_Box_Item.Items.Add(item);
                    }
                    else
                    {
                        UIcamera item = new UIcamera();
                        item.Media = m;
                        item.Height = 240;
                        item.Width = 170;
                        item.ViewInfoMediaEvent += Item_ViewInfoMediaEvent;
                        item.DeleteMediaEvent += Item_DeleteCameraEvent;
                        item.PlayMediaEvent += Item_PlayMediaEvent;
                        this.list_Box_Item.Items.Add(item);
                    }

                }
            }
        }

        private void Item_DeleteCameraEvent(object sender, MediaData e)
        {
            if (e != null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Bạn có muốn xoá camera này không?", "Xoá camera", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    Code.Media.MediaData.Delete(e);
                    UIcamera m = this.list_Box_Item.SelectedItem as UIcamera;
                    m.Animation_Opacity_View_Frame(false, () =>
                    {
                        this.list_Box_Item.Items.Remove(m);
                        this.Datas.Remove(e);
                        this.totalMedia -= 1;
                        if (this.to < this.totalMedia)
                        {
                            if (this.User == null)
                            {
                                this.Datas = App.curUser.LoadMedias(this.from, this.to, out this.totalMedia, this.TypeMedia);
                            }
                            else
                            {
                                this.Datas = this.User.LoadMedias(this.from, this.to, out this.totalMedia, this.TypeMedia,true);
                            }
                            this.LoadGUI();
                        }
                    }, 600);
                    this.list_Box_Item.SelectedIndex = -1;
                }
            }
        }

        private void Item_PlayMediaEvent(object sender, Code.Media.MediaData e)
        {
            if (e != null)
            {
                MediaPlayer player = new MediaPlayer();
                player.Width = 1366;
                player.Height = 668;
                player.setLeft(0);
                player.setTop(90);
                player.Medias = this.Datas;
                player.Media = e;
                player.CloseEvent += Player_CloseEvent;
                this.UIRoot.Children.Add(player);
            }
        }

        private void Player_CloseEvent(object sender, Code.Media.MediaData e)
        {
            this.UIRoot.Children.Remove(sender as MediaPlayer);
        }

        private void Item_DeleteMediaEvent(object sender, Code.Media.MediaData e)
        {
            if (e != null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Bạn có muốn xoá file video này không?", "Xoá video", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    alta_class_ftp.deleteFile(e.Url, App.setting.ftp_user, App.setting.ftp_password);
                    MediaData.Delete(e);
                    UIMedia m = this.list_Box_Item.SelectedItem as UIMedia;
                    m.Animation_Opacity_View_Frame(false, () =>
                    {
                        this.list_Box_Item.Items.Remove(m);
                        this.Datas.Remove(e);
                        this.totalMedia -= 1;
                        if (this.to < this.totalMedia)
                        {
                            if (this.User == null)
                            {
                                this.Datas = App.curUser.LoadMedias(this.from, this.to, out this.totalMedia, this.TypeMedia);
                            }
                            else
                            {
                                this.Datas= this.User.LoadMedias(this.from, this.to, out this.totalMedia, this.TypeMedia,true);
                            }
                            this.LoadGUI();
                        }
                    }, 600);
                    this.list_Box_Item.SelectedIndex = -1;
                }

            }
        }

        private void Item_ViewInfoMediaEvent(object sender, Code.Media.MediaData e)
        {
            if (e != null)
            {
                UIMediaEdit item = new UIMediaEdit();
                item.Type = e.Type;
                item.CloseEvent += Item_CloseEvent;
                item.setLeft(0);
                item.setTop(90);
                item.Width = 1366;
                item.Height = 578;
                item.Media = e;
                this.UIRoot.Children.Add(item);

            }
        }
        private void UIReload_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.UISearchEdit.Text = "";
            this.Focus();
            this.from = 0;
            this.to = 12;
            if (this.User == null)
            {
                this.Datas = App.curUser.LoadMedias(from, to, out totalMedia, this.TypeMedia);
            }
            else
            {
                this.Datas = this.User.LoadMedias(this.from, this.to, out this.totalMedia, this.TypeMedia,true);
            }
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
                    if(this.User==null)
                        this.Datas = App.curUser.FindMedias(this.key, from, to, out this.totalMedia, this.TypeMedia);
                    else
                    {
                        this.Datas = this.User.FindMedias(this.key, from, to, out this.totalMedia, this.TypeMedia,true);
                    }
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
                    if (this.User == null)
                    {
                        this.Datas = App.curUser.LoadMedias(this.from, this.to, out this.totalMedia, this.TypeMedia);
                    }
                    else
                    {
                        this.Datas = this.User.LoadMedias(this.from, this.to, out this.totalMedia, this.TypeMedia,true);
                    }
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

        private void UILeftBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.from > 0)
            {
                this.to = this.from;
                this.from -= 12;
                if (string.IsNullOrEmpty(this.key))
                {
                    if (this.User == null)
                    {
                        this.Datas = App.curUser.LoadMedias(this.from, this.to, out this.totalMedia, this.TypeMedia);
                    }
                    else
                    {
                        this.Datas = this.User.LoadMedias(this.from, this.to, out this.totalMedia, this.TypeMedia,true);
                    }
                }
                else
                {
                    if (this.User == null)
                    {
                        this.Datas = App.curUser.FindMedias(this.key, this.from, this.to, out this.totalMedia, this.TypeMedia);
                    }
                    else
                    {
                        this.Datas = this.User.FindMedias(this.key, this.from, this.to, out this.totalMedia, this.TypeMedia,true);
                    }
                }
                this.LoadGUI();
            }
        }

        private void UIRightBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.to < this.totalMedia)
            {
                this.from = this.to;
                this.to += 12;
                if (string.IsNullOrEmpty(this.key))
                {
                    if (this.User == null)
                    {
                        this.Datas = App.curUser.LoadMedias(this.from, this.to, out this.totalMedia, this.TypeMedia);
                    }
                    else
                    {
                        this.Datas = this.User.LoadMedias(this.from, this.to, out this.totalMedia, this.TypeMedia,true);
                    }
                }
                else
                {
                    if (this.User == null)
                    {
                        this.Datas = App.curUser.FindMedias(this.key, this.from, this.to, out this.totalMedia, this.TypeMedia);
                    }
                    else
                    {
                        this.Datas = this.User.FindMedias(this.key, this.from, this.to, out this.totalMedia, this.TypeMedia,true);
                    }
                }
                this.LoadGUI();
            }
        }
        private void UIRootView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                this.UILeftBtn_MouseLeftButtonUp(null, null);
                this.Focus();
            }
            else if (e.Key == Key.Right)
            {
                this.UIRightBtn_MouseLeftButtonUp(null, null);
                this.Focus();
            }
        }
        private void UIBtnBack_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (BackEvent != null)
            {
                BackEvent(this, new EventArgs());
            }
        }
    }
}
