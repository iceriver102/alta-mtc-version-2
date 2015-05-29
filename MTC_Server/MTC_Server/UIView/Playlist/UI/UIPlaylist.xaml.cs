using MTC_Server.Code;
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

namespace MTC_Server.UIView.Playlist.UI
{
    /// <summary>
    /// Interaction logic for UIPlaylist.xaml
    /// </summary>
    public partial class UIPlaylist : UserControl
    {
        public event EventHandler<Code.Playlist.Playlist> ViewInfoPlaylistEvent;
        public event EventHandler<Code.Playlist.Playlist> DeletePlaylistEvent;
        public event EventHandler<Code.Playlist.Playlist> ChangeDefaultEvent;
        public event EventHandler<Code.Playlist.Playlist> UnLinkScheduleEvent;
        public event EventHandler<Code.Playlist.Playlist> ViewMediaEvent;

        private Code.Playlist.Playlist _playlist;
        public bool ScheduleView = false;
        private bool _select = false;
        public bool isSelect { get { return this._select; } set { this._select = value; } }
        public Code.Playlist.Playlist Playlist
        {
            get { return this._playlist; }
            set
            {
                this._playlist = value;
                if (this._playlist != null)
                {
                    this.UIFullName.Text = this._playlist.Name;
                    if (this._playlist.Status)
                    {
                        this.UIStatus.Text = Define.Fonts["fa-unlock"].Code;
                    }
                    else
                    {
                        this.UIStatus.Text = Define.Fonts["fa-lock"].Code;
                    }
                    if (this._playlist.Default)
                    {
                        this.UIDefault.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    }
                    else
                    {
                        this.UIDefault.Foreground = new SolidColorBrush(Colors.Gray);
                    }
                    this.UIDate.Text = this._playlist.Time.format("dd/MM/yyyy");
                    this.UIUser.Text = this._playlist.User.Full_Name;
                }
            }
        }

        public UIPlaylist()
        {
            InitializeComponent();
        }

        private void UIRootView_MouseLeave(object sender, MouseEventArgs e)
        {
            this.UIBar.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 250);
        }

        private void UIRootView_MouseEnter(object sender, MouseEventArgs e)
        {
            this.UIBar.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 230);
        }

        private void UIRootView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.isSelect = true;
        }

        private void UIStatus_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (App.curUser.ID == this._playlist.user_id)
            {
                this._playlist.ChangeStatus(!this._playlist.Status);
                if (this._playlist.Status)
                {
                    this.UIStatus.Text = Define.Fonts["fa-unlock"].Code;
                }
                else
                {
                    this.UIStatus.Text = Define.Fonts["fa-lock"].Code;
                }
            }
        }

        private void ViewInfoUser(object sender, MouseButtonEventArgs e)
        {
            if (this.ViewInfoPlaylistEvent != null )
            {
                this.ViewInfoPlaylistEvent(this, this._playlist);
            }
        }

        private void DeleteUser(object sender, MouseButtonEventArgs e)
        {
            if (App.curUser.ID == this._playlist.user_id)
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Bạn có muốn xoá Playlist này không?", "Xoá Playlist", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    this._playlist.delete();
                    if (this.DeletePlaylistEvent != null)
                    {
                        this.DeletePlaylistEvent(this, this.Playlist);
                    }
                }
            }
        }

        private void UIDefault_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (App.curUser.ID == this._playlist.user_id)
            {
                this._playlist.ChangeDefault(!this._playlist.Default);
                if (this._playlist.Default && ChangeDefaultEvent != null)
                {
                    ChangeDefaultEvent(this, this._playlist);
                }
                else
                {
                    this.UIDefault.Foreground = new SolidColorBrush(Colors.Gray);
                }
            }
        }

        private void UIRootView_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ScheduleView)
            {
                UIBtnUnlink.Visibility = System.Windows.Visibility.Visible;
                UIBtnDel.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                UIBtnUnlink.Visibility = System.Windows.Visibility.Hidden;
                UIBtnDel.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void UnlinkSchedule(object sender, MouseButtonEventArgs e)
        {
            if (App.curUser.ID == this._playlist.user_id && this.UnLinkScheduleEvent!=null)
            {
                UnLinkScheduleEvent(this, this.Playlist);
            }
        }

        private void UIBtnMedia_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ViewMediaEvent != null)
            {
                ViewMediaEvent(this, this.Playlist);
            }
        }
    }
}
