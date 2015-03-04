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
using MTC_Server.Code.Media;
using Alta.Class;

namespace MTC_Server.UIView.Media
{
    /// <summary>
    /// Interaction logic for UIcamera.xaml
    /// </summary>
    public partial class UIcamera : UserControl
    {
        public event EventHandler<Code.Media.MediaData> ViewInfoMediaEvent;
        public event EventHandler<Code.Media.MediaData> DeleteMediaEvent;
        public event EventHandler<Code.Media.MediaData> PlayMediaEvent;
        private MediaData m;
        public MediaData Media
        {
            get
            {
                return this.m;
            }
            set
            {
                this.m = value;
                this.UITitle.Text = this.m.Name;
                this.UIUser.Text = this.m.User.Full_Name;
                this.UIUrl.Text = this.m.Url.toIP();
            }
        }
        public UIcamera()
        {
            InitializeComponent();
        }

        private void UIBtnInfo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.ViewInfoMediaEvent != null)
            {
                this.ViewInfoMediaEvent(this, this.Media);
            }
        }

        private void UIBtnDelete_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.DeleteMediaEvent != null)
            {
                this.DeleteMediaEvent(this, this.Media);
            }
        }

        private void UIBtnPlay_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.PlayMediaEvent != null)
            {
                this.PlayMediaEvent(this, this.Media);
            }
        }

        private void UIRootView_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
