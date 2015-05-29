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

namespace MTC_Server.UIView.Playlist
{
    /// <summary>
    /// Interaction logic for PlaylistEdit.xaml
    /// </summary>
    public partial class PlaylistEdit : UserControl
    {
        public event EventHandler<Code.Playlist.Playlist> CloseEvent;
        public event EventHandler<Code.Playlist.Playlist> DeletePlaylistEvent;

        private Code.Playlist.Playlist _playlist;
        public Code.Playlist.Playlist Playlist
        {
            get
            {
                return this._playlist;
            }
            set
            {
                this._playlist = value;
                if (value != null)
                {
                    this.UIFullnameEdit.Text = this._playlist.Name;
                    this.UICommentEdit.Text = this._playlist.Comment;
                    this.UIBtnDelete.Visibility = Visibility.Visible;
                }
                else
                {
                    this.UIBtnDelete.Visibility = Visibility.Hidden;
                }
            }
        }


        public PlaylistEdit()
        {
            InitializeComponent();
        }

        public void LoadUI(Code.Playlist.Playlist p)
        {
            if (p == null)
            {
                this.UICommentEdit.empty();
                this.UIName.Text = "Thêm Playlist";
                this.UIFullnameEdit.empty();
            }
            else
            {
                this.UIFullnameEdit.Text = p.Name;
                this.UICommentEdit.Text = p.Comment;
                this.UIName.Text = "Chỉnh sửa Playlist";
            }
        }

        private void ResetFill(object sender, MouseButtonEventArgs e)
        {
            this.LoadUI(this._playlist);
        }

        private void SubmitForm(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.SavePlaylist(null, null);
            }
        }

        private void DeletePlaylist(object sender, MouseButtonEventArgs e)
        {
            if (this._playlist == null)
                return;
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Bạn có muốn xoá playlist này không?", "Xoá playlist", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                this.Playlist.delete();
                if (this.DeletePlaylistEvent != null)
                {
                    this.DeletePlaylistEvent(this, this._playlist);
                }
            }
        }

        private void UIRootView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            this.LoadUI(this._playlist);
        }
        private void CloseDialog(object sender, MouseButtonEventArgs e)
        {
            if (this.CloseEvent != null)
            {
                this.CloseEvent(this, null);
            }
        }

        private void SavePlaylist(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(this.UIFullnameEdit.Text))
            {
                MessageBox.Show("Tên playlist không được để trống!");
                return;
            }
            Code.Playlist.Playlist tmp = new Code.Playlist.Playlist();
            tmp.Name = this.UIFullnameEdit.Text;
            tmp.Comment = this.UICommentEdit.Text;
            tmp.user_id = App.curUser.ID;
            int retult = -1;
            if (this._playlist == null)
            {
               retult= Code.Playlist.Playlist.Insert(tmp);
            }
            else
            {
                tmp.ID = this._playlist.ID;
                retult= tmp.save();
            }
            if (retult == 1)
            {
                if (this.CloseEvent != null)
                {
                    this.CloseEvent(this, tmp);
                }
            }
            else
            {
                MessageBox.Show("Lỗi không thể thêm Playlist");
            }
        }

        private void UIRootView_KeyUp(object sender, KeyEventArgs e)
        {
          
            if (Key.Escape == e.Key)
            {
                this.CloseDialog(null, null);
            }
        }
    }
}
