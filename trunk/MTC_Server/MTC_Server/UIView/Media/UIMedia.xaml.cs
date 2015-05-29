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
using MTC_Server.Code;
using MTC_Server.Code.Media;
namespace MTC_Server.UIView.Media
{
    /// <summary>
    /// Interaction logic for UIMedia.xaml
    /// </summary>
    public partial class UIMedia : UserControl
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
                if (this.m.TypeMedia != null && !string.IsNullOrEmpty(this.m.TypeMedia.Icon))
                    this.UIIcon.Text = this.m.TypeMedia.Icon;
                if (!this.Media.Status)
                {
                    this.UIBtnStatus.Text = Define.Fonts["fa-lock"].Code;
                }
                else
                {
                    this.UIBtnStatus.Text = Define.Fonts["fa-unlock"].Code;
                }
                this.UITitle.Text = this.m.Name;
                this.UITime.Text = this.m.Duration.ToString();
                this.UIFileSize.Text = this.m.FileSize;
                this.UIUser.Text = this.m.User.Full_Name;
                this.UIUrl.Text = this.m.Url.toFileName();
            }
        }
        public UIMedia()
        {
            InitializeComponent();
            if (App.curUser.Permision.confirm_media)
            {
                this.UIBtnStatus.Cursor = Cursors.Hand;
            }else
            {
                this.UIBtnStatus.Cursor = Cursors.Arrow;
            }

        }

        private void UIRootView_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void UIBtnStatus_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (App.curUser.Permision.confirm_media)
            {
                this.Media.Status = !this.Media.Status;
                this.Media.saveStatus();
                if (!this.Media.Status)
                {
                    this.UIBtnStatus.Text = Define.Fonts["fa-lock"].Code;
                }
                else
                {
                    this.UIBtnStatus.Text = Define.Fonts["fa-unlock"].Code;
                }
            }
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
            if (this.Media.LocalFile.Exists)
            {
                this.Media.LocalFile.Delete();
            }

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
    }
}
