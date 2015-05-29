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

        public CameraCCTV()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Camera != null)
            {
                if (this.Camera.m_lUserID == -1)
                    this.Camera.Login();
                if (this.Camera.m_lUserID != -1)
                {
                    this.Preview();
                    Border.BorderBrush = Brushes.DarkGreen;
                }
                else
                {
                    Border.BorderBrush = Brushes.Orange;
                    // MessageBox.Show("Không thể đăng nhập camera");
                }
            }

        }
        public void Preview()
        {
            CHCNetSDK.NET_DVR_CLIENTINFO lpClientInfo = new CHCNetSDK.NET_DVR_CLIENTINFO();
            lpClientInfo.lChannel = this.Camera.channel;
            lpClientInfo.lLinkMode = 0x0000;
            lpClientInfo.sMultiCastIP = "";
            if (this.Camera.m_iPreviewType == 0) // use by callback
            {
                lpClientInfo.hPlayWnd = IntPtr.Zero; // todo!!! 这边应该做2中情况考虑去编写代码
                m_ptrRealHandle = pictureBox1.Handle;
                m_fRealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);
                IntPtr pUser = new IntPtr();
                this.Camera.m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V30(this.Camera.m_lUserID, ref lpClientInfo, m_fRealData, pUser, 1);
            }
            else if (1 == this.Camera.m_iPreviewType)
            {
                m_ptrRealHandle = pictureBox1.Handle;
                IntPtr pUser = new IntPtr();
                this.Camera.m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V30(this.Camera.m_lUserID, ref lpClientInfo, null, pUser, 1);
            }
            if (this.Camera.m_lRealHandle == -1)
            {
                uint nError = CHCNetSDK.NET_DVR_GetLastError();
                MessageBox.Show("Lỗi không thể truy cập camera");
                return;
            }


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
                    nLastErr = PlayCtrl.PlayM4_GetLastError(this.Camera.m_lPort);
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
                    if (!PlayCtrl.PlayM4_GetPort(ref this.Camera.m_lPort))
                    {
                        MessageBox.Show("Get Port Fail");
                    }

                    if (dwBufSize > 0)
                    {
                        //set as stream mode, real-time stream under preview
                        if (!PlayCtrl.PlayM4_SetStreamOpenMode(this.Camera.m_lPort, PlayCtrl.STREAME_REALTIME))
                        {
                            //this.BeginInvoke(AlarmInfo, "PlayM4_SetStreamOpenMode fail");
                        }
                        //start player
                        if (!PlayCtrl.PlayM4_OpenStream(this.Camera.m_lPort, ref pBuffer, dwBufSize, 1024 * 1024))
                        {
                            this.Camera.m_lPort = -1;
                            // this.BeginInvoke(AlarmInfo, "PlayM4_OpenStream fail");
                            break;
                        }
                        //set soft decode display callback function to capture
                        this.m_fDisplayFun = new PlayCtrl.DISPLAYCBFUN(RemoteDisplayCBFun);
                        if (!PlayCtrl.PlayM4_SetDisplayCallBack(this.Camera.m_lPort, m_fDisplayFun))
                        {
                            //this.BeginInvoke(AlarmInfo, "PlayM4_SetDisplayCallBack fail");
                        }

                        //start play, set play window
                        // this.BeginInvoke(AlarmInfo, "About to call PlayM4_Play");

                        if (!PlayCtrl.PlayM4_Play(this.Camera.m_lPort, m_ptrRealHandle))
                        {
                            this.Camera.m_lPort = -1;
                            //this.BeginInvoke(AlarmInfo, "PlayM4_Play fail");
                            break;
                        }

                        //set frame buffer number

                        if (!PlayCtrl.PlayM4_SetDisplayBuf(this.Camera.m_lPort, 15))
                        {
                            //this.BeginInvoke(AlarmInfo, "PlayM4_SetDisplayBuf fail");
                        }

                        //set display mode
                        if (!PlayCtrl.PlayM4_SetOverlayMode(this.Camera.m_lPort, 0, 0/* COLORREF(0)*/))//play off screen // todo!!!
                        {
                            //this.BeginInvoke(AlarmInfo, " PlayM4_SetOverlayMode fail");
                        }
                    }

                    break;
                case CHCNetSDK.NET_DVR_STREAMDATA:     // video stream data
                    if (dwBufSize > 0 && this.Camera.m_lPort != -1)
                    {
                        if (!PlayCtrl.PlayM4_InputData(this.Camera.m_lPort, ref pBuffer, dwBufSize))
                        {
                            // this.BeginInvoke(AlarmInfo, " PlayM4_InputData fail");
                        }
                    }
                    break;

                case CHCNetSDK.NET_DVR_AUDIOSTREAMDATA:     //  Audio Stream Data
                    if (dwBufSize > 0 && this.Camera.m_lPort != -1)
                    {
                        if (!PlayCtrl.PlayM4_InputVideoData(this.Camera.m_lPort, ref pBuffer, dwBufSize))
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
    }
}
