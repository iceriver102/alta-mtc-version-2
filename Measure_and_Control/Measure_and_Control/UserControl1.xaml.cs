using Camera_Final;
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

namespace Measure_and_Control
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        string ip = "192.168.10.199";
        string admin = "admin";
        string pass = "admin";
        int port = 8000;
        bool m_bJpegCapture;
        public CHCNetSDK.NET_DVR_DEVICEINFO_V30 m_struDeviceInfo;

        public Int32 m_lUserID = -1;

        public Int32 m_iPreviewType = 0;

        public Int32 m_lRealHandle = -1;

        public IntPtr m_ptrRealHandle;

        public CHCNetSDK.REALDATACALLBACK m_fRealData = null;

        public Int32 m_lPort = -1;

        public bool view = false;

        private PlayCtrl.DISPLAYCBFUN m_fDisplayFun = null;
        public UserControl1()
        {
            InitializeComponent();
        }
        public bool Login()
        {
            if (string.IsNullOrEmpty(this.ip) || string.IsNullOrEmpty(this.admin))
                return false;
            this.m_lUserID = CHCNetSDK.NET_DVR_Login_V30(this.ip, this.port, this.admin, this.pass, ref m_struDeviceInfo);
            if (this.m_lUserID == -1)
                return false;
            return true;
        }


        private void btnPreview_Click()
        {
            CHCNetSDK.NET_DVR_CLIENTINFO lpClientInfo = new CHCNetSDK.NET_DVR_CLIENTINFO();

            int channel = 1;
            lpClientInfo.lChannel = channel;
            lpClientInfo.lLinkMode = 0x0000;
            lpClientInfo.sMultiCastIP = "";
            if (m_iPreviewType == 0) // use by callback
            {
                lpClientInfo.hPlayWnd = IntPtr.Zero;// todo!!! 这边应该做2中情况考虑去编写代码
                m_ptrRealHandle = pictureBox1.Handle;
                m_fRealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);
                IntPtr pUser = new IntPtr();
                m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V30(m_lUserID, ref lpClientInfo, m_fRealData, pUser, 1);

            }
            else if (1 == m_iPreviewType)
            {
                lpClientInfo.hPlayWnd = pictureBox1.Handle;
                IntPtr pUser = new IntPtr();
                m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V30(m_lUserID, ref lpClientInfo, null, pUser, 1);
            }
            if (m_lRealHandle == -1)
            {
                // uint nError = CHCNetSDK.NET_DVR_GetLastError();
                // DebugInfo("NET_DVR_RealPlay fail %d!");
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
                if (!PlayCtrl.PlayM4_ConvertToJpegFile(pBuf, nSize, nWidth, nHeight, nType, "C:/Capture.jpg"))
                {
                    //Debug.WriteLine("PlayM4_ConvertToJpegFile fail");
                    nLastErr = PlayCtrl.PlayM4_GetLastError(m_lPort);
                    //this.BeginInvoke(AlarmInfo, "Jpeg Capture fail");
                }
                else
                {
                    // this.BeginInvoke(AlarmInfo, "Jpeg Capture Succ");
                    //Debug.WriteLine("PlayM4_ConvertToJpegFile Succ");
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
                    if (!PlayCtrl.PlayM4_GetPort(ref m_lPort))
                    {
                        MessageBox.Show("Get Port Fail");
                    }

                    if (dwBufSize > 0)
                    {
                        //set as stream mode, real-time stream under preview
                        if (!PlayCtrl.PlayM4_SetStreamOpenMode(m_lPort, PlayCtrl.STREAME_REALTIME))
                        {
                            //this.BeginInvoke(AlarmInfo, "PlayM4_SetStreamOpenMode fail");
                        }
                        //start player
                        if (!PlayCtrl.PlayM4_OpenStream(m_lPort, ref pBuffer, dwBufSize, 1024 * 1024))
                        {
                            m_lPort = -1;
                            // this.BeginInvoke(AlarmInfo, "PlayM4_OpenStream fail");
                            break;
                        }
                        //set soft decode display callback function to capture
                        m_fDisplayFun = new PlayCtrl.DISPLAYCBFUN(RemoteDisplayCBFun);
                        if (!PlayCtrl.PlayM4_SetDisplayCallBack(m_lPort, m_fDisplayFun))
                        {
                            //this.BeginInvoke(AlarmInfo, "PlayM4_SetDisplayCallBack fail");
                        }

                        //start play, set play window
                        // this.BeginInvoke(AlarmInfo, "About to call PlayM4_Play");

                        if (!PlayCtrl.PlayM4_Play(m_lPort, m_ptrRealHandle))
                        {
                            //m_lPort = -1;
                            //this.BeginInvoke(AlarmInfo, "PlayM4_Play fail");
                            //break;
                        }

                        //set frame buffer number

                        if (!PlayCtrl.PlayM4_SetDisplayBuf(m_lPort, 15))
                        {
                            //this.BeginInvoke(AlarmInfo, "PlayM4_SetDisplayBuf fail");
                            MessageBox.Show("PlayM4_SetDisplayBuf fail");
                        }

                        //set display mode
                        if (!PlayCtrl.PlayM4_SetOverlayMode(m_lPort, 0, 0/* COLORREF(0)*/))//play off screen // todo!!!
                        {
                            //this.BeginInvoke(AlarmInfo, " PlayM4_SetOverlayMode fail");
                        }
                    }

                    break;
                case CHCNetSDK.NET_DVR_STREAMDATA:     // video stream data
                    if (dwBufSize > 0 && m_lPort != -1)
                    {
                        if (!PlayCtrl.PlayM4_InputData(m_lPort, ref pBuffer, dwBufSize))
                        {
                            // this.BeginInvoke(AlarmInfo, " PlayM4_InputData fail");
                        }
                    }
                    break;

                case CHCNetSDK.NET_DVR_AUDIOSTREAMDATA:     //  Audio Stream Data
                    if (dwBufSize > 0 && m_lPort != -1)
                    {
                        if (!PlayCtrl.PlayM4_InputVideoData(m_lPort, ref pBuffer, dwBufSize))
                        {
                            // this.BeginInvoke(AlarmInfo, "PlayM4_InputVideoData Fail");
                        }
                    }

                    break;
                default:
                    break;
            }

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Login())
            {
                this.btnPreview_Click();
            }
            else
            {
                MessageBox.Show("Khong the dang nhajp");
            }
        }
    }
}
