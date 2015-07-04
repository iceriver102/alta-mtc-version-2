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
using Vlc.DotNet.Core.Interops.Signatures.LibVlc.MediaListPlayer;
using Vlc.DotNet.Core.Medias;

namespace Camera_Final.UIView
{
    /// <summary>
    /// Interaction logic for Camera.xaml
    /// </summary>
    public partial class Camera : UserControl
    {
        public string File
        {
            get;
            set;
        }

        void media_ParsedChanged(MediaBase sender, Vlc.DotNet.Core.VlcEventArgs<int> e)
        {
          

        }


        //System.Windows.Forms.PictureBox picturebox1;
        public Camera()
        {
            InitializeComponent();
            myVlcControl.Playing += myVlcControl_Playing;
            myVlcControl.Stopped += myVlcControl_Stopped;
            myVlcControl.PlaybackMode = PlaybackModes.Loop;
            this.UIView.Stretch = App.setting.Stretch;
           
        }

        private void myVlcControl_Stopped(Vlc.DotNet.Wpf.VlcControl sender, Vlc.DotNet.Core.VlcEventArgs<EventArgs> e)
        {
            
        }

        private void myVlcControl_Playing(Vlc.DotNet.Wpf.VlcControl sender, Vlc.DotNet.Core.VlcEventArgs<EventArgs> e)
        {
           
        }
        private void UIRootView_Loaded(object sender, RoutedEventArgs e)
        {
            //this.UIRoot.RenderTransform = new ScaleTransform(this.Width / 1366, this.Height / 768);
            this.Preview();  
        }
        public void Preview()
        {
            if (!string.IsNullOrEmpty(this.File))
            {
                MediaBase media = new PathMedia(File);
                media.ParsedChanged+=media_ParsedChanged;
                myVlcControl.AudioProperties.IsMute = true;
                myVlcControl.AudioProperties.Volume = 0;
                myVlcControl.Media = media;
                myVlcControl.Play();
            }
        }      

        private void UIRootView_Unloaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
