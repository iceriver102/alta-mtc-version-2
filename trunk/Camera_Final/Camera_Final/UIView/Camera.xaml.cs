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
        private PlayCtrl.DISPLAYCBFUN m_fDisplayFun = null;
        private CHCNetSDK.NET_DVR_DEVICEINFO_V30 m_struDeviceInfo;
        private CHCNetSDK.REALDATACALLBACK m_fRealData = null;
        private IntPtr m_ptrRealHandle;
        private int postion=-1;
        public int Postion
        {
            get
            {
                return this.postion;
            }
            set
            {
                this.postion = value;
                if (App.DataCamera[this.postion] != null)
                {
                    if (App.DataCamera[this.postion].m_lUserID == -1)
                    {
                        if (!App.DataCamera[this.postion].Login())
                        {
                            MessageBox.Show(string.Format("Không thể đăng nhập camera {0}", App.DataCamera[this.postion].ip));
                        }
                        else
                        {
                            LocationMedia media = new LocationMedia(App.DataCamera[this.postion].rtsp);
                            media.ParsedChanged += media_ParsedChanged;
                            this.myVlcControl.Media = media;
                        }
                    }
                    else
                    {
                        LocationMedia media = new LocationMedia(App.DataCamera[this.postion].rtsp);
                        media.ParsedChanged += media_ParsedChanged;
                        this.myVlcControl.Media = media;
                    }
                }
            }
        }

        void media_ParsedChanged(MediaBase sender, Vlc.DotNet.Core.VlcEventArgs<int> e)
        {
            if(!App.DataCamera[this.postion].view)
            {
                this.myVlcControl.Stop();
            }
            else
            {
                this.myVlcControl.Play();
            }

        }

       
        public bool VideoPlay
        {
            get
            {
                if (this.postion == -1)
                    return false;
                if (App.DataCamera[this.postion] == null)
                    return false;
                return App.DataCamera[this.postion].view;
            }
            set
            {
                if (this.postion == -1)
                    return;
                if (App.DataCamera[this.postion] == null)
                    return;
                App.DataCamera[this.postion].view = value;
                if (value)
                {
                    this.myVlcControl.Play();
                }
                else
                {
                    this.myVlcControl.Stop();
                }
            }
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
            if (App.DataCamera[this.postion] != null && App.DataCamera[this.postion].m_lUserID != -1)
            {
                this.Preview();
            }
           
          
        }
        public void Preview()
        {
            CHCNetSDK.NET_DVR_CLIENTINFO lpClientInfo = new CHCNetSDK.NET_DVR_CLIENTINFO();
            lpClientInfo.lChannel = App.DataCamera[this.postion].channel;
            lpClientInfo.lLinkMode = 0x0000;
            lpClientInfo.sMultiCastIP = "";
            if (App.DataCamera[this.postion].m_iPreviewType == 0) // use by callback
            {
                lpClientInfo.hPlayWnd = IntPtr.Zero;// todo!!! 这边应该做2中情况考虑去编写代码
                IntPtr pUser = new IntPtr();
                App.DataCamera[this.postion].m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V30(App.DataCamera[this.postion].m_lUserID, ref lpClientInfo, m_fRealData, pUser, 1);
            }
            else if (1 == App.DataCamera[this.postion].m_iPreviewType)
            {
                IntPtr pUser = new IntPtr();
                App.DataCamera[this.postion].m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V30(App.DataCamera[this.postion].m_lUserID, ref lpClientInfo, null, pUser, 1);
            }
            if (App.DataCamera[this.postion].m_lRealHandle == -1)
            {
                uint nError = CHCNetSDK.NET_DVR_GetLastError();
                MessageBox.Show("Lỗi không thể truy cập camera");
                return;
            }
        }
       

        private void UIRootView_Unloaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
