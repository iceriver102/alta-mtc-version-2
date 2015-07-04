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
using Camera_Final.Code;
using Alta.Class;
namespace Camera_Final.UIView
{
    /// <summary>
    /// Interaction logic for UITimeOff.xaml
    /// </summary>
    public partial class UITimeOff : UserControl
    {
        public event EventHandler CloseEvent;
        public event EventHandler<Tuple<int, TimeOff>> SaveDataEvent;
        public int Postion;
        private TimeOff _time;
        public TimeOff Time
        {
            get
            {
                return this._time;
            }
            set
            {
                this._time = value;
                this.UIBegin.Text = string.Format("{0:HH:mm:ss}", this._time.beginTime);
                this.UIEnd.Text = string.Format("{0:HH:mm:ss}", this._time.EndTime);

            }
        }
        public UITimeOff()
        {
            InitializeComponent();
        }

        private void Label_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.UIBegin.isEmpty())
            {
                MessageBox.Show("Thời gian bắt đầu không được để trống", "Thông báo");
                return;
            }
            if (this.UIEnd.isEmpty())
            {
                MessageBox.Show("Thời gian kết thúc không được để trống", "Thông báo");
                return;
            }

            TimeSpan begin = TimeSpan.Parse(this.UIBegin.Text);
            TimeSpan End = TimeSpan.Parse(this.UIEnd.Text);
            if (begin >= End)
            {
                MessageBox.Show("Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc", "Thông báo");
                return;
            }
            if (this.Time == null)
            {
                TimeOff time = new TimeOff();
                time.beginTime = DateTime.Now.setTime(begin);
                time.EndTime = DateTime.Now.setTime(End);
                if (this.SaveDataEvent != null)
                {
                    this.SaveDataEvent(this, new Tuple<int, TimeOff>(this.Postion, time));
                }
            }
            else
            {
                this.Time.beginTime = DateTime.Now.setTime(begin);
                this.Time.EndTime = DateTime.Now.setTime(End);
                if (this.SaveDataEvent != null)
                {
                    this.SaveDataEvent(this, new Tuple<int, TimeOff>(this.Postion, this.Time));
                }
            }

        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.CloseEvent != null)
            {
                this.CloseEvent(this, new EventArgs());
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Postion >= 0)
            {
                if (App.DataAlarm[this.Postion].TimeOffs != null && App.DataAlarm[this.Postion].TimeOffs.Count > 0)
                {
                    this.Time = App.DataAlarm[this.Postion].TimeOffs[0];
                }
                else
                {
                    App.DataAlarm[this.Postion].TimeOffs = new List<TimeOff>();
                }
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
