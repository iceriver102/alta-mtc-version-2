﻿using System;
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
namespace Camera_Final.UIView
{
    /// <summary>
    /// Interaction logic for AddPreset.xaml
    /// </summary>
    public partial class AddPreset : UserControl
    {
        public event EventHandler CloseEvent;
        private PlayCtrl.DISPLAYCBFUN m_fDisplayFun = null;
        private CHCNetSDK.REALDATACALLBACK m_fRealData = null;
        private IntPtr m_ptrRealHandle;

        private bool m_bJpegCapture = false;
        public Code.Camera Camera;
        private Code.Camera_Goto _preset;
        private int preset_pos = -1;
        public Code.Camera_Goto Preset
        {
            get
            {
                return this._preset;
            }
            set
            {
                this._preset = value;
                if (value != null)
                {
                    this.UIName.Text = value.name;
                   Code.Camera_Preset tmp= value.getCamera(Camera.id);
                   if (tmp != null)
                   {
                       preset_pos = tmp.Postion;
                       if (this.Camera.m_lUserID == -1)
                       {
                           this.Camera.Login();
                       }
                       if (this.Camera.m_lUserID != -1)
                       {
                           CHCNetSDK.NET_DVR_PTZPreset_Other(this.Camera.m_lUserID,1, CHCNetSDK.GOTO_PRESET, (uint)this.preset_pos);
                       }
                   }
                }
                else
                {
                    this.UIName.empty();
                    preset_pos = -1;
                }
            }
        }
        public AddPreset()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Camera != null )
            {
                if (this.Camera.m_lUserID == -1)
                    this.Camera.Login();
                if (this.Camera.m_lUserID != -1)
                {
                    this.Preview();
                }
                else
                {
                    MessageBox.Show("Không thể đăng nhập camera");
                }
            }
        }
        public void Preview()
        {
            CHCNetSDK.NET_DVR_CLIENTINFO lpClientInfo = new CHCNetSDK.NET_DVR_CLIENTINFO();
            lpClientInfo.lChannel =this.Camera.channel;
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
            else if (1 ==this.Camera.m_iPreviewType)
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

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (CloseEvent != null)
            {
                this.CloseEvent(this, new EventArgs());
            }
        }

        private void SaveCamera(object sender, MouseButtonEventArgs e)
        {
            if (this.preset_pos == -1)
            {
                this.preset_pos = this.Camera.FindFreePreset();
            }
            if (this.UIName.isEmpty())
            {
                MessageBox.Show("Tên preset không được để trống!","Thông báo");
                return;
            }
            if (this.Preset == null)
            {
                if (CHCNetSDK.NET_DVR_PTZPreset(this.Camera.m_lRealHandle, CHCNetSDK.SET_PRESET, (uint)this.preset_pos))
                {
                    Code.Camera_Preset tmp = new Code.Camera_Preset(this.Camera.id, this.preset_pos);
                    Code.Camera_Goto p = new Code.Camera_Goto();
                    p.name = this.UIName.Text;
                    p.Camera.Add(tmp);
                    App.DataPreset.Add(p);
                    if (this.CloseEvent != null)
                    {
                        this.CloseEvent(this, new EventArgs());
                    }
                }
                else
                {
                    MessageBox.Show("Không thể set dữ liệu lên camera", "Thông báo");
                }
                
            }
            else
            {
                MessageBox.Show("Tinh năng đang xây dựng","Thông báo");
            }

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.Camera.m_lRealHandle>0)
            {
                CHCNetSDK.NET_DVR_StopRealPlay(this.Camera.m_lRealHandle);
            }
        }

        private bool PtzControl(UInt32 dwPtzCommand, UInt32 dwStop)
        {
            if (CHCNetSDK.NET_DVR_PTZControl(this.Camera.m_lRealHandle, dwPtzCommand, dwStop))
            {

            }
            else
            {
                MessageBox.Show("Không thể điều khiển camera");
                return false;
            }
            return true;
        }

        private void MoveRightBegin(object sender, MouseButtonEventArgs e)
        {
            PtzControl(CHCNetSDK.PAN_RIGHT, 0);
        }

        private void MoveRightEnd(object sender, MouseButtonEventArgs e)
        {
            PtzControl(CHCNetSDK.PAN_RIGHT, 1);
        }

        private void MoveLeftEnd(object sender, MouseButtonEventArgs e)
        {
            PtzControl(CHCNetSDK.PAN_LEFT, 1);
        }

        private void MoveLeftBegin(object sender, MouseButtonEventArgs e)
        {
            PtzControl(CHCNetSDK.PAN_LEFT, 0);
        }

        private void MoveDownEnd(object sender, MouseButtonEventArgs e)
        {
            PtzControl(CHCNetSDK.TILT_DOWN, 1);
        }

        private void MoveDownBegin(object sender, MouseButtonEventArgs e)
        {
            PtzControl(CHCNetSDK.TILT_DOWN, 0);
        }

        private void MoveUpBegin(object sender, MouseButtonEventArgs e)
        {
            PtzControl(CHCNetSDK.TILT_UP, 0);
        }

        private void MoveUpEnd(object sender, MouseButtonEventArgs e)
        {
            PtzControl(CHCNetSDK.TILT_UP, 1);
        }
    }
}
