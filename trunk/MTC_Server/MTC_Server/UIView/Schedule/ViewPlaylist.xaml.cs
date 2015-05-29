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
using Alta.Plugin;
using Alta.Class;

namespace MTC_Server.UIView.Schedule
{
    /// <summary>
    /// Interaction logic for ViewPlaylist.xaml
    /// </summary>
    public partial class ViewPlaylist : UserControl
    {
       
        public Code.Playlist.Playlist Playlist
        {
            get
            {
                return this.UIPlaylist.Playlist;

            }
            set
            {
                this.UIPlaylist.Playlist = value;
            }
        }

        private Code.Schedule.Event _event;
        public Code.Schedule.Event Event
        {
            get
            {
                return this._event;
            }
            set
            {
                this._event = value;
                if (this._event == null)
                {
                    this.UIPlaylist.Animation_Translate_Frame(double.NaN, double.NaN, 400, double.NaN, 500, () => { this.Playlist = null;});
                }
                else
                {                   
                    this.Playlist = this._event.Playlist;
                }
            }
        }

        public ViewPlaylist()
        {
            InitializeComponent();
            this.UIPlaylist.ScheduleView = true;
        }
        public List<DataAutoComplete> SearchPlaylist(string key)
        {
            List<DataAutoComplete> result = new List<DataAutoComplete>();
            int total;
            int to = 10;
            result = ConvertToDataComplete(App.curUser.SearchPlaylist(key, 0, to, out total));
            return result;
        }

        private List<DataAutoComplete> ConvertToDataComplete(List<Code.Playlist.Playlist> list)
        {
            if (list == null)
                return null;
            List<DataAutoComplete> result = new List<DataAutoComplete>();
            foreach (Code.Playlist.Playlist d in list)
            {
                result.Add(new DataAutoComplete() { key = d.ID.ToString(), label = d.Name });
            }
            return result;
        }

        internal void showViewInput()
        {
            if (this.UIPlaylist.getLeft() == 60)
            {
                this.UIPlaylist.Animation_Translate_Frame(double.NaN, double.NaN, 400, double.NaN, 500);
            }
            if (this.UIInput.getLeft() != 0)
            this.UIInput.Animation_Translate_Frame(-400, double.NaN, 0, double.NaN, 500);
        }

        internal void showPlaylist()
        {
            if (this.Playlist == null)
            {
                this.UIPlaylist.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.UIPlaylist.Visibility = Visibility.Visible;
            }
            if (this.UIPlaylist.getLeft() != 60)
            this.UIPlaylist.Animation_Translate_Frame(-400, double.NaN, 60, double.NaN, 500);
            if (this.UIInput.getLeft() == 0)
                this.UIInput.Animation_Translate_Frame(double.NaN, double.NaN, 400, double.NaN, 500);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.UIFindPlaylist.SelectedItem != null)
            {
                this.Event.setPlaylist(Convert.ToInt32(this.UIFindPlaylist.SelectedItem.key));
                this.Playlist = this.Event.Playlist;
                this.showPlaylist();
            }
        }

        private void UIPlaylist_UnLinkScheduleEvent(object sender, Code.Playlist.Playlist e)
        {
            if (App.curUser.ID == this.Event.user_id && this.Event.playlist_id!=0)
            {
                this.Event.setPlaylist(0);
                this.Playlist = this.Event.Playlist;
                this.showPlaylist();
            }
        }
    }
}
