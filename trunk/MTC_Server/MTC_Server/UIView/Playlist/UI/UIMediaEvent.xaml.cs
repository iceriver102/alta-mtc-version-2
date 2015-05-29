using MTC_Server.Code;
using MTC_Server.Code.Media;
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

namespace MTC_Server.UIView.Playlist.UI
{
    /// <summary>
    /// Interaction logic for UIMediaEvent.xaml
    /// </summary>
    public partial class UIMediaEvent : UserControl
    {
        public event EventHandler<MediaEvent> DeleteEvent;
        public event EventHandler<MediaEvent> ViewMediaEvent;
        public UIMediaEvent()
        {
            InitializeComponent();
        }

        public MediaData _media;
        public MediaData Media
        {
            get
            {
                return this._media;
            }
            private set
            {
                this._media = value;
                if (value != null)
                {
                    if (this._media.TypeMedia != null && !string.IsNullOrEmpty(this._media.TypeMedia.Icon))
                        this.UIIcon.Text = this._media.TypeMedia.Icon;
                    this.UIName.Text = value.Name;

                }
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
                    this.Media = this._event.Media;
                    this.UITime.Text = string.Format("({0:hh\\:mm\\:ss} - {1:hh\\:mm\\:ss})", value.TimeBegin, value.TimeEnd);
                }
            }
        }

        private void UIBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (App.curUser.ID == this.Media.User_ID && this.DeleteEvent != null)
            {
                this.DeleteEvent(this, this.Event);
            }
        }

        private void RootView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Layout.Height = this.Height - 4;
            this.Layout.Width = this.Width - 4;
        }

        private void RootView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.ViewMediaEvent != null)
            {
                this.ViewMediaEvent(this, this.Event);
            }
        }

    }
}
