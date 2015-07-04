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
using Alta.Class;
using Vlc.DotNet.Core.Medias;
using System.Threading;

namespace Camera_Final.UIView
{
    /// <summary>
    /// Interaction logic for CameraCCTV.xaml
    /// </summary>
    public partial class CameraCCTV : UserControl
    {
        private PlayCtrl.DISPLAYCBFUN m_fDisplayFun = null;
        private CHCNetSDK.REALDATACALLBACK m_fRealData = null;
        private IntPtr m_ptrRealHandle;

        private bool m_bJpegCapture = false;
        public Code.Camera Camera;
        public int Postion;

        public CameraCCTV()
        {
            InitializeComponent();
           // myVlcControl.Playing += myVlcControl_Playing;
           // myVlcControl.Stopped += myVlcControl_Stopped;
           // myVlcControl.PlaybackMode = PlaybackModes.Loop;
           // this.UIView.Stretch = App.setting.Stretch;
        }
        private void myVlcControl_Stopped(Vlc.DotNet.Wpf.VlcControl sender, Vlc.DotNet.Core.VlcEventArgs<EventArgs> e)
        {

        }

        private void myVlcControl_Playing(Vlc.DotNet.Wpf.VlcControl sender, Vlc.DotNet.Core.VlcEventArgs<EventArgs> e)
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.DataCamera[this.Postion] != null)
            {
                App.DataCamera[this.Postion].Login();
                if (App.DataCamera[this.Postion].m_lUserID != -1)
                {
                    this.Preview();
                    Border.BorderBrush = Brushes.DarkGreen;
                }
                else
                {
                    Border.BorderBrush = Brushes.Orange;
                }
            }

        }
        public async void Preview()
        {
           // if (App.DataCamera[this.Postion].m_lRealHandle > 0)
            
                CHCNetSDK.NET_DVR_StopRealPlay(App.DataCamera[this.Postion].m_lRealHandle);
                await Task.Delay(50);
            
            //LocationMedia media = new LocationMedia(App.DataCamera[this.Postion].rtsp);
           // media.ParsedChanged += media_ParsedChanged;
           // this.myVlcControl.Media = media;
            CHCNetSDK.NET_DVR_CLIENTINFO lpClientInfo = new CHCNetSDK.NET_DVR_CLIENTINFO();
            lpClientInfo.lChannel = App.DataCamera[this.Postion].channel;
            lpClientInfo.lLinkMode = 0x0000;
            lpClientInfo.sMultiCastIP = "";
            lpClientInfo.hPlayWnd = IntPtr.Zero; // todo!!! 这边应该做2中情况考虑去编写代码
            m_ptrRealHandle = pictureBox1.Handle;
            m_fRealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);
            IntPtr pUser = new IntPtr();
            App.DataCamera[this.Postion].m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V30(App.DataCamera[this.Postion].m_lUserID, ref lpClientInfo, m_fRealData, pUser, 1);

            if (App.DataCamera[this.Postion].m_lRealHandle == -1)
            {
                uint nError = CHCNetSDK.NET_DVR_GetLastError();
                MessageBox.Show("Lỗi không thể truy cập camera");
                return;
            }


        }

        private void media_ParsedChanged(MediaBase sender, Vlc.DotNet.Core.VlcEventArgs<int> e)
        {
            //throw new NotImplementedException();
        }


        public void RemoteDisplayCBFun(int nPort, IntPtr pBuf, int nSize, int nWidth, int nHeight, int nStamp, int nType, int nReserved)
        {
            //MyDebugInfo AlarmInfo = new MyDebugInfo(DebugInfo);
            if (!m_bJpegCapture)
            {
                return;
            }
            else
            {
                uint nLastErr = 100;
                // save picture as you want
                if (!PlayCtrl.PlayM4_ConvertToJpegFile(pBuf, nSize, nWidth, nHeight, nType, "D:/Capture.jpg"))
                {
                    //Debug.WriteLine("PlayM4_ConvertToJpegFile fail");
                    nLastErr = PlayCtrl.PlayM4_GetLastError(App.DataCamera[this.Postion].m_lPort);
                    //this.BeginInvoke(AlarmInfo, "Jpeg Capture fail");
                }
            }

            m_bJpegCapture = false;
        }

        public void RealDataCallBack(Int32 lRealHandle, UInt32 dwDataType, ref byte pBuffer, UInt32 dwBufSize, IntPtr pUser)
        {
            //MyDebugInfo AlarmInfo = new MyDebugInfo(DebugInfo);
            switch (dwDataType)
            {
                case CHCNetSDK.NET_DVR_SYSHEAD:     // sys head
                    if (!PlayCtrl.PlayM4_GetPort(ref App.DataCamera[this.Postion].m_lPort))
                    {
                        MessageBox.Show("Get Port Fail");
                    }

                    if (dwBufSize > 0)
                    {
                        //set as stream mode, real-time stream under preview
                        if (!PlayCtrl.PlayM4_SetStreamOpenMode(App.DataCamera[this.Postion].m_lPort, PlayCtrl.STREAME_REALTIME))
                        {
                            //this.BeginInvoke(AlarmInfo, "PlayM4_SetStreamOpenMode fail");
                        }
                        //start player
                        if (!PlayCtrl.PlayM4_OpenStream(App.DataCamera[this.Postion].m_lPort, ref pBuffer, dwBufSize, 1024 * 1024))
                        {
                            App.DataCamera[this.Postion].m_lPort = -1;
                            // this.BeginInvoke(AlarmInfo, "PlayM4_OpenStream fail");
                            break;
                        }
                        //set soft decode display callback function to capture
                        this.m_fDisplayFun = new PlayCtrl.DISPLAYCBFUN(RemoteDisplayCBFun);
                        if (!PlayCtrl.PlayM4_SetDisplayCallBack(App.DataCamera[this.Postion].m_lPort, m_fDisplayFun))
                        {
                            //this.BeginInvoke(AlarmInfo, "PlayM4_SetDisplayCallBack fail");
                        }

                        //start play, set play window
                        // this.BeginInvoke(AlarmInfo, "About to call PlayM4_Play");
                        
                        if (!PlayCtrl.PlayM4_Play(App.DataCamera[this.Postion].m_lPort, m_ptrRealHandle))
                        {
                            App.DataCamera[this.Postion].m_lPort = -1;
                            //this.BeginInvoke(AlarmInfo, "PlayM4_Play fail");
                            break;
                        }
                        
                        //set frame buffer number

                        if (!PlayCtrl.PlayM4_SetDisplayBuf(App.DataCamera[this.Postion].m_lPort, 15))
                        {
                            //this.BeginInvoke(AlarmInfo, "PlayM4_SetDisplayBuf fail");
                        }

                        //set display mode
                        if (!PlayCtrl.PlayM4_SetOverlayMode(App.DataCamera[this.Postion].m_lPort, 0, 0/* COLORREF(0)*/))//play off screen // todo!!!
                        {
                            //this.BeginInvoke(AlarmInfo, " PlayM4_SetOverlayMode fail");
                        }
                    }

                    break;
                case CHCNetSDK.NET_DVR_STREAMDATA:     // video stream data
                    if (dwBufSize > 0 && App.DataCamera[this.Postion].m_lPort != -1)
                    {
                        if (!PlayCtrl.PlayM4_InputData(App.DataCamera[this.Postion].m_lPort, ref pBuffer, dwBufSize))
                        {
                            // this.BeginInvoke(AlarmInfo, " PlayM4_InputData fail");
                        }
                    }
                    break;

                case CHCNetSDK.NET_DVR_AUDIOSTREAMDATA:     //  Audio Stream Data
                    if (dwBufSize > 0 && App.DataCamera[this.Postion].m_lPort != -1)
                    {
                        if (!PlayCtrl.PlayM4_InputVideoData(App.DataCamera[this.Postion].m_lPort, ref pBuffer, dwBufSize))
                        {
                            // this.BeginInvoke(AlarmInfo, "PlayM4_InputVideoData Fail");
                        }
                    }

                    break;
                default:
                    break;
            }

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
           
        }

        public void Stop()
        {
            if (this.Camera.m_lRealHandle > 0)
            {
                CHCNetSDK.NET_DVR_StopRealPlay(this.Camera.m_lRealHandle);
            }
        }

        public void RecordFile()
        {
            string fileName = string.Format("{0}_{1}.mp4", this.Camera.ip, DateTime.Now.Ticks);
            CHCNetSDK.NET_DVR_SaveRealData(this.Camera.m_lRealHandle, System.IO.Path.Combine(App.curFolder, fileName));
        }

        public void StopRecord()
        {

            CHCNetSDK.NET_DVR_StopSaveRealData(this.Camera.m_lRealHandle);

        }
        bool full = false;
        double w;
        double h;
        double t, l;
        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!this.full)
            {
                w = this.Width;
                h = this.Height;
                l = this.getLeft();
                t = this.getTop();
                this.Width = 1366;
                this.Height = 768;
                this.UIRoot.RenderTransform = new ScaleTransform(this.Width / 1366, this.Height / 768);
                this.setLeft(0);
                this.setTop(0);
                this.setZIndex(999);
                full = true;
                this.UIFull.Text = App.Fonts["fa-compress"].Code;
            }
            else
            {
                this.full = false;
                this.Width = w;
                this.Height = h;
                this.UIRoot.RenderTransform = new ScaleTransform(this.Width / 1366, this.Height / 768);
                this.setLeft(l);
                this.setTop(t);
                this.setZIndex(1);
                this.UIFull.Text = App.Fonts["fa-expand"].Code;
            }
            
        }

        private void UIRootView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!this.full)
            {
                w = this.Width;
                h = this.Height;
                l = this.getLeft();
                t = this.getTop();
                this.Width = 1366;
                this.Height = 768;
                this.UIRoot.RenderTransform = new ScaleTransform(this.Width / 1366, this.Height / 768);
                this.setLeft(0);
                this.setTop(0);
                this.setZIndex(999);
                full = true;
                this.UIFull.Text = App.Fonts["fa-compress"].Code;
            }
            else
            {
                this.full = false;
                this.Width = w;
                this.Height = h;
                this.UIRoot.RenderTransform = new ScaleTransform(this.Width / 1366, this.Height / 768);
                this.setLeft(l);
                this.setTop(t);
                this.setZIndex(1);
                this.UIFull.Text = App.Fonts["fa-expand"].Code;
            }
        }
    }
}
