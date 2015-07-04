using Camera_Final.Code;
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
    /// Interaction logic for UIMapAlarm.xaml
    /// </summary>
    public partial class UIMapAlarm : UserControl
    {
        public event EventHandler<int> ViewAlarmEvent;
        public event EventHandler<int> MoveEvent;
        public event EventHandler<int> DeleteAlarmEvent;
        public event EventHandler<int> ConnectCameraEvent;
        public event EventHandler<int> DisableAlarmEvent;
        public event EventHandler<Tuple<int, int>> EditConnectEvent;
        public event EventHandler<int> ScheduleEvent;
        public bool isDisable = false;
        //Thread TimeThread;
        private Alarm _alarm;
        public Alarm Alarm
        {
            get
            {
                return this._alarm;
            }
            set
            {
                this._alarm = value;
                if (value != null)
                {
                    this.Icon.Text = App.Fonts[value.icon].Code;
                    this.UIName.Text = value.Name;
                    this.setLeft(value.Left);
                    this.setTop(value.Top);
                    this.ToolTip = value.Com;
                    if (value.Cameras == null || value.Cameras.Count == 0)
                    {
                        UIM_Link.Header = "Kết nối Camera";
                    }
                    else
                    {
                        UIM_Link.Header = "Chỉnh sửa kết nối";
                    }
                }
            }
        }
        private int _postion;
        public int Postion
        {
            get
            {
                return this._postion;
            }
            set
            {
                this._postion = value;
                if (this._postion >= 0)
                {

                    if (App.DataAlarm[this._postion].Auto)
                    {
                        if (App.DataAlarm[this._postion].TimeOffs.Count > 0)
                        {
                            if (DateTime.Now.TimeOfDay < App.DataAlarm[this._postion].TimeOffs[0].EndTime.TimeOfDay && DateTime.Now.TimeOfDay >= App.DataAlarm[this._postion].TimeOffs[0].beginTime.TimeOfDay)
                            {
                                App.DataAlarm[this._postion].Disable();
                            }
                            else
                            {
                                App.DataAlarm[this._postion].Enable();
                            }
                        }
                    }
                    this.Alarm = App.DataAlarm[this._postion];
                }
            }
        }
        Color from;
        public UIMapAlarm()
        {
            InitializeComponent();
            from = (this.Icon.Foreground as SolidColorBrush).Color;
            
        }
        public bool isEnable = false;
      

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

        private void UserControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                this.ReleaseMouseCapture();
            }
           
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.CaptureMouse();
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ViewAlarmEvent != null)
            {
                this.ViewAlarmEvent(this, this._postion);
            }
        }

        private void DeleteAlarm(object sender, RoutedEventArgs e)
        {
            if (this.DeleteAlarmEvent != null)
            {
                this.DeleteAlarmEvent(this, this.Postion);
            }
        }        

        private void DisableAlarm(object sender, RoutedEventArgs e)
        {
            this.Disable();
            this.Alarm.Auto = false;
            if (DisableAlarmEvent != null)
            {
                this.DisableAlarmEvent(this, this._postion);
            }
        }

        public void Disable()
        {
            App.DataAlarm[this._postion].Disable();
            this.Alarm.status = false;
        }

        public void Enable()
        {
            App.DataAlarm[this._postion].Enable();
            this.Alarm.status = true;            
        }


        private void AlarmOff(object sender, RoutedEventArgs e)
        {
            this._alarm.RunCommand(App.DefineCommand.ALARM_OFF);
            App.DataAlarm[this._postion].alert = false;
            this.RunAlarm(false);
        }

        private void EnableAlarm(object sender, RoutedEventArgs e)
        {
            this.Enable();
            this.Alarm.Auto = false;
            if (DisableAlarmEvent != null)
            {
                this.DisableAlarmEvent(this, this._postion);
            }
        }

        public event EventHandler<Tuple<Alarm,bool>> AlertEvent;

        internal void RunAlarm(bool flag= true)
        {
            
            this.Dispatcher.Invoke(() =>
            {
                if (flag)
                {
                    (this.Icon.Foreground as SolidColorBrush).Animation_Color_Repeat(from, Colors.Red);
                    this._alarm.RunCommand(App.DefineCommand.ALARM_ON);
                    if (AlertEvent != null)
                    {
                        AlertEvent(this, new Tuple<Alarm,bool>(this._alarm,true));
                    }
                }
                else
                {
                    (this.Icon.Foreground as SolidColorBrush).BeginAnimation(SolidColorBrush.ColorProperty, null);
                    this.Icon.Foreground = new SolidColorBrush(this.from);
                    if (AlertEvent != null)
                    {
                        AlertEvent(this, new Tuple<Alarm, bool>(this._alarm, false));
                    }
                }

            });
        }      

        private void ConnectCamera(object sender, RoutedEventArgs e)
        {
            if (this._alarm.Cameras == null || this._alarm.Cameras.Count == 0)
            {
                if (this.ConnectCameraEvent != null)
                {
                    ConnectCameraEvent(this, this._postion);
                }
            }
            else if (EditConnectEvent!=null)
            {
                EditConnectEvent(this, new Tuple<int, int>(0, this._postion));
            }
        }
        private void ScheduleAlarm(object sender, RoutedEventArgs e)
        {
            if (ScheduleEvent != null)
            {
                ScheduleEvent(this, this._postion);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
          
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
