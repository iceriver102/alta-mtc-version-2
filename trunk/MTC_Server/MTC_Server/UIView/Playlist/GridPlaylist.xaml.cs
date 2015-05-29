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
using MTC_Server.UIView.Playlist.UI;

namespace MTC_Server.UIView.Playlist
{
    /// <summary>
    /// Interaction logic for GridPlaylist.xaml
    /// </summary>
    public partial class GridPlaylist : UserControl
    {
        public event EventHandler<Code.Playlist.Playlist> ViewMediaEvent;
        List<Code.Playlist.Playlist> Datas;
        public int totalUser;
        public int to = 10;
        public int from = 0;
        public string key
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
        public GridPlaylist()
        {
            InitializeComponent();
        }

        private void UIRootView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            this.Datas = App.curUser.LoadPlaylist(this.from, this.to, out this.totalUser);
            LoadGUI();
        }

        private void LoadGUI()
        {
            this.list_Box_Item.Items.Clear();
            if (this.Datas != null)
            {
                foreach (Code.Playlist.Playlist data in this.Datas)
                {
                    UI.UIPlaylist item = new UI.UIPlaylist();
                    item.Playlist = data;
                    item.Height = 260;
                    item.Width = 200;
                    item.ViewInfoPlaylistEvent += Item_ViewInfoPlaylistEvent;
                    item.DeletePlaylistEvent += Item_DeletePlaylistEvent;
                    item.ChangeDefaultEvent += Item_ChangeDefaultEvent;
                    item.ViewMediaEvent += item_ViewMediaEvent;
                    //item.ViewPermison += Item_ViewPermison;
                    this.list_Box_Item.Items.Add(item);
                }
            }
            if (to < totalUser)
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
        }

        void item_ViewMediaEvent(object sender, Code.Playlist.Playlist e)
        {
            if (e != null)
            {
                Playlist_Media view = new Playlist_Media();
                view.Width = 1366;
                view.Height = 668;
                view.Playlist = e;
                view.setLeft(0);
                view.setTop(0);
                view.CloseEvent += view_CloseEvent;
                this.UIRoot.Children.Add(view);
            }
        }

        void view_CloseEvent(object sender, EventArgs e)
        {
            this.UIRoot.Children.Remove(sender as Playlist_Media);
        }

        private void Item_ChangeDefaultEvent(object sender, Code.Playlist.Playlist e)
        {
            if (e != null && e.Default)
            {
                this.Datas = App.curUser.LoadPlaylist(from, to, out totalUser);
                LoadGUI();
            }
        }

        private void Item_DeletePlaylistEvent(object sender, Code.Playlist.Playlist e)
        {
            if (e != null)
            {
                UI.UIPlaylist uiU = this.list_Box_Item.SelectedItem as UI.UIPlaylist;
                this.list_Box_Item.SelectedIndex = -1;
                uiU.Animation_Opacity_View_Frame(false, () =>
                {
                    this.list_Box_Item.Items.Remove(uiU);
                    this.Datas.Remove(uiU.Playlist);
                    if (this.to < this.totalUser)
                    {
                        this.Datas = App.curUser.LoadPlaylist(from, to, out totalUser);
                        LoadGUI();
                    }
                });
            }
        }

        private void Item_ViewInfoPlaylistEvent(object sender, Code.Playlist.Playlist e)
        {
            if (e != null)
            {
                PlaylistEdit item = new PlaylistEdit();
                item.setLeft(0);
                item.setTop(90);
                item.Width = 1366;
                item.Height = 578;
                item.Playlist = e;
                item.CloseEvent += Form_CloseEvent;
                item.DeletePlaylistEvent += Item_DeleteUserEvent1;
                this.UIRoot.Children.Add(item);
            }
        }

        private void Item_DeleteUserEvent1(object sender, Code.Playlist.Playlist e)
        {
            if (e != null)
            {
                UI.UIPlaylist uiU = this.list_Box_Item.SelectedItem as UI.UIPlaylist;
                this.list_Box_Item.SelectedIndex = -1;
                uiU.Animation_Opacity_View_Frame(false, () =>
                {
                    this.list_Box_Item.Items.Remove(uiU);
                    this.Datas.Remove(uiU.Playlist);
                    if (this.to < this.totalUser)
                    {
                        this.Datas = App.curUser.LoadPlaylist(from, to, out totalUser);
                        LoadGUI();
                    }
                });

            }
            this.UIRoot.Children.Remove(sender as PlaylistEdit);
        }

        private void UIRootView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                UILeftBtn_MouseLeftButtonUp(null, null);
                this.Focus();
            }
            else if (e.Key == Key.Right)
            {
                UIRightBtn_MouseLeftButtonUp(null, null);
                this.Focus();
            }
            e.Handled = true;
        }

        private void UIReload_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.UISearchEdit.Text = "";
            this.Focus();
            this.to = 10;
            this.from = 0;
            this.Datas = App.curUser.LoadPlaylist(this.from, this.to, out this.totalUser);
            this.LoadGUI();
        }

        private void UISearchEdit_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && App.curUser.Permision.mana_user)
            {
                string key = this.UISearchEdit.Text.Trim();
                if (key.Length > 3)
                {
                    to = 10;
                    from = 0;
                    this.Datas = App.curUser.SearchPlaylist(this.key, from, to, out this.totalUser);
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
                    this.Datas = App.curUser.LoadPlaylist(this.from, this.to, out this.totalUser);
                    this.LoadGUI();
                }
                this.Focus();
            }
        }

        private void UIRightBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.to < this.totalUser)
            {
                this.from = this.to;
                this.to += 10;
                if (string.IsNullOrEmpty(this.key))
                    this.Datas = App.curUser.LoadPlaylist(this.from, this.to, out this.totalUser);
                else
                    this.Datas = App.curUser.SearchPlaylist(this.key, this.from, this.to, out this.totalUser);

                LoadGUI();
            }
        }

        private void UILeftBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.from > 0)
            {
                this.from -= 10;
                if (this.from < 0)
                    this.from = 0;
                this.to -= 10;
                if (string.IsNullOrEmpty(this.key))
                    this.Datas = App.curUser.LoadPlaylist(this.from, this.to, out this.totalUser);
                else
                    this.Datas = App.curUser.SearchPlaylist(this.key, this.from, this.to, out this.totalUser);

                LoadGUI();
            }
        }

        private void UISearchEdit_LostFocus(object sender, RoutedEventArgs e)
        {
            this.UIOverText.Animation_Opacity_View_Frame(true);
        }

        private void UIBtnAdd_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PlaylistEdit form = new PlaylistEdit();
            form.setLeft(0);
            form.setTop(90);
            form.Width = 1366;
            form.Height = 578;
            form.CloseEvent += Form_CloseEvent;
            this.UIRoot.Children.Add(form);
        }

        private void Form_CloseEvent(object sender, Code.Playlist.Playlist e)
        {
            PlaylistEdit form = sender as PlaylistEdit;
            if (e != null)
            {
                bool isEditUser = false;
                foreach (UI.UIPlaylist item in this.list_Box_Item.Items)
                {
                    if (item.Playlist.ID == e.ID)
                    {
                        item.Playlist = e;
                        isEditUser = true;
                        break;
                    }
                }
                if (!isEditUser)
                {
                    this.Datas = App.curUser.LoadPlaylist(this.from, this.to, out this.totalUser);
                    LoadGUI();
                }
            }
            this.UIRoot.Children.Remove(form);
        }

        private void UISearchEdit_GotFocus(object sender, RoutedEventArgs e)
        {
            this.UIOverText.Animation_Opacity_View_Frame(false);
        }
    }
}
