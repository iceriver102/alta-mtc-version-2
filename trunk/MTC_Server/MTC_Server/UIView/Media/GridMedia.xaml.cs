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

namespace MTC_Server.UIView.Media
{
    /// <summary>
    /// Interaction logic for GridMedia.xaml
    /// </summary>
    public partial class GridMedia : UserControl
    {
        private string key = string.Empty;
        private List<Code.Media.MediaData> Datas;
        private int to = 10;
        private int from = 0;
        private int totalMedia = 0;
        public GridMedia()
        {
            InitializeComponent();
        }

        private void UIBtnAddUser_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UIMediaEdit item = new UIMediaEdit();
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
                foreach(UIMedia m  in this.list_Box_Item.Items)
                {
                    if(m.Media.ID == e.ID)
                    {
                        m.Media = e;
                        isEdit = true;
                        break;
                    }
                }
                if (!isEdit)
                {
                    this.Datas = App.curUser.LoadMedias(this.from, this.to, out this.totalMedia);
                    this.LoadGUI();
                }
            }
            this.UIRoot.Children.Remove(sender as UIMediaEdit);
        }

        private void UIRootView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Datas=  App.curUser.LoadMedias(from,to,out totalMedia);
            this.LoadGUI();
        }
        public void LoadGUI()
        {
            this.list_Box_Item.Items.Clear();
            if (this.Datas != null)
            {
                foreach (Code.Media.MediaData m in this.Datas)
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
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Bạn có muốn xoá file video này không?", "Xoá video", System.Windows.MessageBoxButton.YesNo);
                if(messageBoxResult== MessageBoxResult.Yes)
                {
                    foreach (UIMedia m in this.list_Box_Item.Items)
                    {
                        if (m.Media.ID == e.ID)
                        {
                            alta_class_ftp.deleteFile(e.Url, App.setting.ftp_user, App.setting.ftp_password);
                            Code.Media.MediaData.Delete(e);
                            this.Datas.Remove(e);
                            if (this.list_Box_Item.Items.Count > 1)
                            {
                                if (this.list_Box_Item.SelectedIndex != 0)
                                    this.list_Box_Item.SelectedIndex = 0;
                                else
                                    this.list_Box_Item.SelectedIndex = 1;
                            }
                            m.Animation_Opacity_View_Frame(false, () => {
                                this.Datas.Remove(e);
                                this.list_Box_Item.Items.Remove(m);
                            },600);                           
                            return;
                        }
                    }

                }
               
            }
        }

        private void Item_ViewInfoMediaEvent(object sender, Code.Media.MediaData e)
        {
            if (e != null)
            {
                UIMediaEdit item = new UIMediaEdit();
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
            this.to = 10;
            this.Datas = App.curUser.LoadMedias(from, to, out totalMedia);
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
                    this.Datas = App.curUser.FindMedias(this.key, from, to, out this.totalMedia);
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
                    this.Datas = App.curUser.LoadMedias(this.from, this.to, out this.totalMedia);
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
