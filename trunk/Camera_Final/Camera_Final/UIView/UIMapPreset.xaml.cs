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
    /// Interaction logic for UIMapPreset.xaml
    /// </summary>
    public partial class UIMapPreset : UserControl
    {
        public event EventHandler<Code.Camera_Goto> DeletePresetEvent;
        public event EventHandler<Code.Camera_Goto> MovePresetEvent;
        private Code.Camera_Goto preset;
        public Code.Camera_Goto Preset
        {
            get
            {
                return this.preset;
            }
            set
            {
                this.preset = value;
                if (this.preset != null)
                {
                    this.Icon.Text = App.Fonts[value.icon].Code;
                    this.UIName.Text = value.name;
                    this.setLeft(value.left);
                    this.setTop(value.top);                    
                }
            }
        }
       

        public UIMapPreset()
        {
            InitializeComponent();
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                if (this.MovePresetEvent != null)
                {
                    this.MovePresetEvent(this, this.preset);
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
            if (this.IsMouseCaptured)
            {
                this.ReleaseMouseCapture();
            }
            if (App.DataCamera != null)
            {
                if(this.preset.Camera!=null)
                {
                    for (int j = 0; j < this.Preset.Camera.Count; j++)
                    {
                        for (int i = 0; i < App.DataCamera.Count; i++)
                        {

                            if (App.DataCamera[i].id == this.Preset.Camera[j].camera_id)
                            {
                                if (App.DataCamera[i].m_lUserID == -1)
                                {
                                    App.DataCamera[i].Login();
                                }
                                if(App.DataCamera[i].m_lUserID!=-1)
                                {
                                    CHCNetSDK.NET_DVR_PTZPreset_Other(App.DataCamera[i].m_lUserID, App.DataCamera[i].channel, CHCNetSDK.GOTO_PRESET,(uint) this.preset.Camera[j].Postion);
                                  
                                }
                               
                            }
                        }
                    }
                }
            }
        }

        private void GotoPreset(object sender, RoutedEventArgs e)
        {
            this.UserControl_MouseDoubleClick(null,null);
        }

        private void DeletePreset(object sender, RoutedEventArgs e)
        {
            if (this.DeletePresetEvent != null)
            {
                this.DeletePresetEvent(this, this.preset);
            }
        }
        public event EventHandler DebugEvent;
        private void DebugPreset(object sender, RoutedEventArgs e)
        {
            if (DebugEvent != null)
            {
                DebugEvent(this, new EventArgs());
            }
        }

      
    }
}
