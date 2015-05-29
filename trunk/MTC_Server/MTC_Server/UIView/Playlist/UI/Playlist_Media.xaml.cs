using MTC_Server.Code.Playlist;
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
using Alta.Plugin;

namespace MTC_Server.UIView.Playlist.UI
{
    /// <summary>
    /// Interaction logic for Playlist_Media.xaml
    /// </summary>
    public partial class Playlist_Media : UserControl
    {
        private Code.Playlist.Playlist playlist;
        public Code.Playlist.Playlist Playlist
        {
            get
            {
                return this.playlist;
            }
            set
            {
                this.playlist = value;
                this.LoadUI(this.playlist);
            }
        }

        private MediaEvent _event;
        public MediaEvent Event
        {
            get
            {
                return this._event;
            }
            set
            {
                this._event = value;
                if (value != null)
                {
                    this.MediaSelect.SelectedItem = new DataAutoComplete() { key = value.media_id.ToString(), label = value.Media.Name };
                    this.UITimeBegin.curDate = DateTime.Now.setTime(value.TimeBegin);
                    this.UITimeEnd.curDate = DateTime.Now.setTime(value.TimeEnd);
                }
                else
                {
                    this.MediaSelect.SelectedItem = null;
                }
            }
        }

        private void LoadUI(Code.Playlist.Playlist playlist)
        {
            this.StackMedia.Children.Clear();
            if (playlist != null)
            {
                this.UITitle.Text = playlist.Name;
                List<MediaEvent> Medias = playlist.LoadMediaEvent();
                if (Medias != null)
                {
                    foreach (MediaEvent e in Medias)
                    {
                        UIMediaEvent UIE = new UIMediaEvent();
                        UIE.Height = 50;
                        UIE.Width = 395;
                        UIE.Event = e;
                        UIE.DeleteEvent += UIE_DeleteEvent;
                        UIE.ViewMediaEvent += UIE_ViewMediaEvent;
                        this.StackMedia.Children.Add(UIE);

                    }
                }
            }
            else
            {
                this.UITitle.Text = string.Empty;
            }
            
        }

        void UIE_ViewMediaEvent(object sender, MediaEvent e)
        {
            this.Event = e;
        }

        void UIE_DeleteEvent(object sender, MediaEvent e)
        {
            if (e != null)
            {
                if (MessageBox.Show("Bạn có muốn xóa media này không?", "Thông báo", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    e.delete();
                    this.LoadUI(this.playlist);
                }
            }
        }

        public List<DataAutoComplete> SearchMedia(string key)
        {
            List<DataAutoComplete> result = new List<DataAutoComplete>();
            int total;
            int to = 10;
            result = ConvertToDataComplete(App.curUser.FindMedias(key,0,to,out total,0,true));
            return result;
        }
        private List<DataAutoComplete> ConvertToDataComplete(List<Code.Media.MediaData> list)
        {
            if (list == null)
                return null;
            List<DataAutoComplete> result = new List<DataAutoComplete>();
            foreach (Code.Media.MediaData d in list)
            {
                DataAutoComplete tmp = new DataAutoComplete() { key = d.ID.ToString(), label = d.Name };
                tmp.icon = d.TypeMedia.Icon;
                result.Add(tmp);
            }
            return result;
        }

        public Playlist_Media()
        {
            InitializeComponent();
        }

        private void UIRootView_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void UIRootView_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoadUI(this.playlist);
        }

        private void UIBtnAddUser_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.MediaSelect.SelectedItem == null)
            {
                MessageBox.Show("Hãy chọn Media mà bạn muốn thêm.", "Thông báo");
                return;
            }
            if (this.UITimeBegin >= this.UITimeEnd)
            {
                MessageBox.Show("Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc", "Thông báo");
                return;
            }
            #region Validate
            bool validate = true;
            List<MediaEvent> Medias = this.playlist.LoadMediaEvent();
            if (Medias != null)
            {
                foreach (MediaEvent mE in Medias)
                {
                    if (this.Event == null || ((this.Event != null) && this.Event.ID != mE.ID))
                    {
                        if ((mE.TimeBegin <= this.UITimeBegin.Time.TimeOfDay && mE.TimeEnd >= this.UITimeBegin.Time.TimeOfDay) || (mE.TimeBegin <= this.UITimeEnd.Time.TimeOfDay && mE.TimeEnd >= this.UITimeEnd.Time.TimeOfDay) || (mE.TimeBegin >= this.UITimeBegin.Time.TimeOfDay && mE.TimeEnd <= this.UITimeEnd.Time.TimeOfDay))
                        {
                            validate = false;
                            break;
                        }
                    }
                }
            }
            if (validate)
            {
                MediaEvent tmp = new MediaEvent();
                tmp.media_id = Convert.ToInt32(this.MediaSelect.SelectedItem.key);
                tmp.playlist_id = this.playlist.ID;
                tmp.TimeBegin = this.UITimeBegin.Time.TimeOfDay;
                tmp.TimeEnd = this.UITimeEnd.Time.TimeOfDay;
                int result = 0;
                if (this.Event == null)
                {
                    result = MediaEvent.Insert(tmp);
                }
                else
                {
                    tmp.ID = this._event.ID;
                    result =tmp.Save();

                }
                if (result != 1)
                {
                    MessageBox.Show("Lưu dữ liệu không thành công", "Thông báo");
                    return;
                }
                else
                {
                    this.Event = null;
                    this.LoadUI(this.playlist);
                }
            }
            else
            {
                MessageBox.Show("Không thể thêm media trong thời gian này", "Thông báo");
            }
            #endregion

        }
        public event EventHandler CloseEvent;

        private void Close_click(object sender, RoutedEventArgs e)
        {
            if (CloseEvent != null)
            {
                CloseEvent(this, new EventArgs());
            }
        }
    }
}
