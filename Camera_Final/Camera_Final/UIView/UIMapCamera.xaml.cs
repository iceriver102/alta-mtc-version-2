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
    /// Interaction logic for UIMapCamera.xaml
    /// </summary>
    public partial class UIMapCamera : UserControl
    {
        public event EventHandler<int> ViewCameraEvent;
        public event EventHandler<int> MoveEvent;
        public event EventHandler<int> DeleteCameraEvent;
        public event EventHandler<Code.Camera> AddPresetEvent;
        public event EventHandler<Code.Camera> ConnectCameraEvent;
        private int _postion;
        private Code.Camera _cam;
        public int Postion
        {
            get
            {
                return this._postion;
            }
            set
            {
                this._postion = value;
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
        }

        private void UserControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                this.ReleaseMouseCapture();
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

        private void ConnectCamera(object sender, RoutedEventArgs e)
        {
            if (ConnectCameraEvent != null)
            {
                this.ConnectCameraEvent(this, App.DataCamera[this._postion]);
            }
        }
    }
}
