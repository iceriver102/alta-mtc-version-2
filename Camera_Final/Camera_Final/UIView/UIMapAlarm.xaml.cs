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
                    this.Alarm = App.DataAlarm[this._postion];
                }
            }
        }

        public UIMapAlarm()
        {
            InitializeComponent();
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

        private void UserControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                this.ReleaseMouseCapture();
            }
            if (ConnectCameraEvent != null)
            {
                this.ConnectCameraEvent(this, this._postion);
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
            this._alarm.RunCommand(App.DefineCommand.DISABLE);
            App.DataAlarm[this._postion].status = false;
            if (DisableAlarmEvent != null)
            {
                this.DisableAlarmEvent(this, this._postion);
            }
        }

        private void AlarmOff(object sender, RoutedEventArgs e)
        {
            this._alarm.RunCommand(App.DefineCommand.ALARM_OFF);
            App.DataAlarm[this._postion].alert = false;
        }

        private void EnableAlarm(object sender, RoutedEventArgs e)
        {
            this._alarm.RunCommand(App.DefineCommand.ENABLE);
            App.DataAlarm[this._postion].status = true;
            if (DisableAlarmEvent != null)
            {
                this.DisableAlarmEvent(this, this._postion);
            }
        }

    }
}
