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
using System.Threading;

namespace Camera_Final.UIView
{
    /// <summary>
    /// Interaction logic for UIMapCamera.xaml
    /// </summary>
    public partial class UIMapCamera : UserControl
    {
        public event EventHandler<int> ViewCameraEvent;
        public event EventHandler<int> MoveEvent;
        public event EventHandler<int> DeleteCameraEvent;
        public event EventHandler<Code.Camera> AddPresetEvent;
        public event EventHandler<Code.Camera> ConnectCameraEvent;
        private Color From;
        private int _postion;
        private Code.Camera _cam;
        public bool alert = false;
        public int Time = 3;
        public int Postion
        {
            get
            {
                return this._postion;
            }
            set
            {
                this._postion = value;
                if (App.DataCamera[this._postion].m_lUserID == 0)
                {
                    App.DataCamera[this._postion].Login();
                }
                this._cam = App.DataCamera[this._postion];
                this.Icon.Text = App.Fonts[this._cam.icon].Code;
                this.UIName.Text = this._cam.name;
                this.setLeft(this._cam.Left);
                this.setTop(this._cam.Top);
            }
        }
        public UIMapCamera()
        {
            InitializeComponent();
            Time = App.setting.TimeCount;
            this.From = (this.Icon.Foreground as SolidColorBrush).Color;
            TimeCountThread = new Thread(TimeCountFunctionThread);
            TimeCountThread.IsBackground = true;
            TimeCountThread.Start();
        }

        private void TimeCountFunctionThread(object obj)
        {
            while (true)
            {
                Thread.Sleep(1000);
                if (this.alert)
                {
                    Time--;
                    if (Time < 0)
                    {
                        this.alert = true;
                        Time = App.setting.TimeCount;
                        this.Alarm(false);
                        this.RunAround();                       
                    }
                }
            }
        }

        private void UserControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                this.ReleaseMouseCapture();
            }
            if (ConnectCameraEvent != null)
            {
                this.ConnectCameraEvent(this, App.DataCamera[this._postion]);
            }
        }        

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                if (this.MoveEvent != null)
                {
                    MoveEvent(this, this.Postion);
                }
            }
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.CaptureMouse();
        }

        private void ViewCamera(object sender, RoutedEventArgs e)
        {
            if (ViewCameraEvent != null)
            {
                ViewCameraEvent(this, this.Postion);
            }
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ViewCameraEvent != null)
            {
                ViewCameraEvent(this, this.Postion);
            }
        }

        private void DeleteCamera(object sender, RoutedEventArgs e)
        {
            if (this.DeleteCameraEvent != null)
            {
                this.DeleteCameraEvent(this, this._postion);
            }
        }

        private void AddPreset(object sender, RoutedEventArgs e)
        {
            if (AddPresetEvent != null)
            {
                AddPresetEvent(this, App.DataCamera[this._postion]);
            }
        }

        

        public bool Goto(uint preset)
        {
            if(this._cam!=null)
            {
                if (this._cam.m_lUserID == -1)
                {
                    this._cam.Login();
                }
                if (this._cam.m_lUserID != -1)
                {
                    CHCNetSDK.NET_DVR_PTZPreset_Other(this._cam.m_lUserID,this._cam.channel,CHCNetSDK.GOTO_PRESET,preset);
                    return true;
                }
            }
            return false;
        }

        Thread TimeCountThread;


        public void Alarm(bool flag= true)
        {
            if (flag)
            {
                if (!this.alert)
                {
                    this.alert = true;
                    this.Dispatcher.Invoke(() =>
                    {
                        (this.Icon.Foreground as SolidColorBrush).Animation_Color_Repeat(From, Colors.White);
                    });
                }
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                   {
                       (this.Icon.Foreground as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, null);
                       this.Icon.Foreground = new SolidColorBrush(this.From);
                   });
                this.alert = false;
                Time = App.setting.TimeCount;
            }
        }

        private void ConnectCamera(object sender, RoutedEventArgs e)
        {
            if (ConnectCameraEvent != null)
            {
                this.ConnectCameraEvent(this, App.DataCamera[this._postion]);
            }
        }

        internal void RunAround()
        {
            if (this._cam.m_lUserID == 0)
            {
                this._cam.Login();
            }
            if (this._cam.m_lUserID!=0)
            {
                CHCNetSDK.NET_DVR_PTZControl_Other(this._cam.m_lUserID, this._cam.channel, CHCNetSDK.PAN_AUTO, 0);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (TimeCountThread != null)
            {
                TimeCountThread.Abort();
                TimeCountThread = null;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.ToolTip = this._cam.ip;
        }
    }
}
