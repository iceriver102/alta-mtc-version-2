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
    /// Interaction logic for UIAddAlarm.xaml
    /// </summary>
    public partial class UIAddAlarm : UserControl
    {
        public event EventHandler CloseEvent;
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
                    this.UIName.Text = value.Name;
                    this.UIBoard.Text = value.board.ToString();
                    this.UIPOS.Text = value.pos.ToString();
                    this.UIComment.Text = value.Comment;
                }
                else
                {
                    this.UIName.empty();
                    this.UIBoard.empty();
                    this.UIPOS.empty();
                    this.UIComment.empty();
                }
            }
        }
        public UIAddAlarm()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.CloseEvent != null)
            {
                this.CloseEvent(this, new EventArgs());
            }
        }

        private void SaveCamera(object sender, MouseButtonEventArgs e)
        {
            if (this.UIName.isEmpty())
            {
                MessageBox.Show("Tên đầu dò không được phép để trống","Thông báo");
                return;
            }
            if (this.UIBoard.isEmpty())
            {
                MessageBox.Show("ID mạch không được phép để trống","Thông báo");
                return;
            }
            if (this.UIPOS.isEmpty())
            {
                MessageBox.Show("ID đầu dò không được phép để trống", "Thông báo");
                return;
            }
            if (this._alarm == null)
            {
                Alarm tmp = new Alarm();
                tmp.pos = Convert.ToInt32(this.UIPOS.Text);
                tmp.Name = this.UIName.Text;
                tmp.board = Convert.ToInt32(this.UIBoard.Text);
                tmp.Comment = this.UIComment.Text;
                App.DataAlarm.Children.Add(tmp);
                if (this.CloseEvent != null)
                {
                    this.CloseEvent(this, new EventArgs());
                }
            }
            else
            {
                for (int i = 0; i < App.DataAlarm.Count; ++i)
                {
                    if (this._alarm.pos == App.DataAlarm[i].pos && this._alarm.board == App.DataAlarm[i].board)
                    {
                        App.DataAlarm[i].Comment = this.UIComment.Text;
                        App.DataAlarm[i].pos = Convert.ToInt32(this.UIPOS.Text);
                        App.DataAlarm[i].Name = this.UIName.Text;
                        App.DataAlarm[i].board = Convert.ToInt32(this.UIBoard.Text);
                        break;
                    }
                }
                if (this.CloseEvent != null)
                {
                    this.CloseEvent(this, new EventArgs());
                }
            }
        }
    }
}
