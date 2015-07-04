using System;
using System.Collections.Generic;
using System.IO;
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
using Camera_Final.UIView;
using System.Windows.Media.Animation;
using System.Threading;
using Camera_Final.Code;
using System.Diagnostics;
namespace Camera_Final
{
    public enum Mode
    {
        MAP = 1, CCTV = 2, NONE = 0
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // private bool m_bInitSDK = false;
        private bool isMenuShow
        {
            get
            {
                return this.ismenushow;
            }
            set
            {
                this.ismenushow = value;
                if (this.ismenushow)
                {
                    UIMenuBtn.Foreground = new SolidColorBrush(Colors.Gray);
                    (UIMenuBtn.Foreground as SolidColorBrush).Animation_Color(Colors.Gray, Colors.White, 250);
                }
                else
                {
                    UIMenuBtn.Foreground = new SolidColorBrush(Colors.White);
                    (UIMenuBtn.Foreground as SolidColorBrush).Animation_Color(Colors.White, Colors.Gray, 250);
                }
            }
        }
        private bool isRecord = false;
        public int count = 1;
        // private Code.Camera cam;
        private bool ismenushow = false;
        public Mode Mode = Mode.MAP;
        private Thread TimeCheck;
        private string curFolder;
        private Thread RunCmdThread;
        public MainWindow()
        {
            InitializeComponent();
            this.Mode = App.Mode;
            this.count = App.setting.resetTime;
            this.curFolder = App.curFolder;
            TimeCheck = new Thread(TimeCheckFunctionThread);
            TimeCheck.IsBackground = true;
            RunCmdThread = new Thread(RunCmdThreadFunction);
            RunCmdThread.IsBackground = true;
            TimeCheck.Start();
            RunCmdThread.Start();
            if (App.User.type == 2)
            {
                this.UIMapBtn.IsEnabled = false;
                this.UIAddCameraBtn.IsEnabled = false;
                this.UIAlarmBtn.IsEnabled = false;
                UIInfoBtn.IsEnabled = false;
            }
        }


        private void RunCmdThreadFunction(object obj)
        {
            while (true)
            {
                if (App.QueueCMD != null && App.QueueCMD.Count > 0)
                {
                    string cmd = App.QueueCMD.Dequeue();
                    string[] element = cmd.Split(' ');
                    if (element.Length == 4)
                    {
                        if ((element[1] == "A" || element[1] == "B") && (element[3] == "1") && element.Length < 5)
                        {
                            try
                            {
                                int board = Convert.ToInt32(element[0]);
                                int pos = Convert.ToInt32(element[2]);
                                this.Dispatcher.Invoke(() =>
                                {
                                    foreach (UIElement uiE in this.UIMapContent.Children)
                                    {
                                        if (uiE is UIMapAlarm)
                                        {
                                            UIMapAlarm uiMap = uiE as UIMapAlarm;
                                            if (uiMap.Alarm.board == board && uiMap.Alarm.pos == pos && !App.DataAlarm[uiMap.Postion].alert)
                                            {
                                                uiMap.RunAlarm();
                                                App.DataAlarm[uiMap.Postion].alert = true;
                                            }
                                        }
                                    }
                                });
                            }
                            catch (Exception)
                            {                                
                               
                            }

                        }
                    }

                }
                Thread.Sleep(500);
            }
        }

        private void TimeCheckFunctionThread()
        {
            while (true)
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.UIUnLock.IsEnabled = false;
                    this.UILock.IsEnabled = false;
                    try
                    {

                        bool isChange = false;
                        foreach (UIElement uiE in this.UIMapContent.Children)
                        {
                            if (uiE is UIMapAlarm)
                            {
                                UIMapAlarm uiAlarm = uiE as UIMapAlarm;
                                if (uiAlarm.Alarm != null && uiAlarm.Alarm.TimeOffs.Count > 0 && uiAlarm.Alarm.Auto)
                                {
                                    
                                    if (DateTime.Now.TimeOfDay >= uiAlarm.Alarm.TimeOffs[0].beginTime.TimeOfDay && DateTime.Now.TimeOfDay < uiAlarm.Alarm.TimeOffs[0].EndTime.TimeOfDay)
                                    {
                                        if (uiAlarm.Alarm.status )
                                        {
                                            uiAlarm.Disable();                                          
                                        }
                                    }
                                    else if ((DateTime.Now.TimeOfDay < uiAlarm.Alarm.TimeOffs[0].beginTime.TimeOfDay || DateTime.Now.TimeOfDay > uiAlarm.Alarm.TimeOffs[0].EndTime.TimeOfDay))
                                    {
                                        if (!uiAlarm.Alarm.status)
                                        {
                                            uiAlarm.Enable();                                          
                                        }
                                    }
                                }

                            }
                        }
                        if (isChange)
                        {
                            this.DrawMap();
                        }
                    }
                    catch (Exception)
                    {
                    }
                    this.UIUnLock.IsEnabled = true;
                    this.UILock.IsEnabled = true;

                    if (App.curFolder != this.curFolder)
                    {
                        this.curFolder = App.curFolder;

                        if (this.isRecord)
                        {

                            foreach (UIElement uiE in this.UICCTVContent.Children)
                            {
                                if (uiE is CameraCCTV)
                                {
                                    CameraCCTV uiCamera = uiE as CameraCCTV;
                                    uiCamera.StopRecord();
                                }
                            }
                            Thread.Sleep(1500);
                            if (App.DataAlarm != null)
                            {
                                for (int i = 0; i < App.DataAlarm.Count; i++)
                                {
                                    App.DataAlarm[i].isDisable = false;
                                    App.DataAlarm[i].isEnable = false;
                                }
                            }

                            foreach (UIElement uiE in this.UICCTVContent.Children)
                            {
                                if (uiE is CameraCCTV)
                                {
                                    CameraCCTV uiCamera = uiE as CameraCCTV;
                                    uiCamera.RecordFile();

                                }
                            }
                            UIRecordBtn.Foreground = Brushes.Red;
                        }
                    }
                });

                Thread.Sleep(2000);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.Map == null || !File.Exists(App.Map.FileImage))
            {
                MessageBox.Show("Không tìm thấy bản đồ camera", "Thông báo");
                this.Close();
                return;
            }

            if (App.User != null)
            {
                Avartar.Text = App.User.FullName[0].ToString();
                UIName.Text = App.User.FullName;
            }

            UIRoot.RenderTransform = new ScaleTransform(this.Width / 1366, this.Height / 768);
            BitmapImage bit = new BitmapImage();
            bit.BeginInit();
            bit.UriSource = new Uri(App.Map.FileImage, UriKind.RelativeOrAbsolute);
            bit.EndInit();
            this.UIMapImage.Source = bit;
            DrawMap();
            if (App.User.type == 2)
            {
                // this.Mode = Camera_Final.Mode.CCTV;
                this.ViewCCTV(null, null);
            }
            else
            {
                if (this.Mode == Camera_Final.Mode.CCTV)
                {
                    this.Mode = Camera_Final.Mode.NONE;
                    this.ViewCCTV(null, null);
                }
            }
        }

        public bool isLoadCCTV = false;

        private void LoadCCTVLayout()
        {
            isLoadCCTV = true;
            this.Dispatcher.Invoke(() =>
            {
                this.UICCTVContent.Children.Clear();
                if (App.DataCamera.Count > 0)
                {
                    if (App.DataCamera.Count == 1)
                    {
                        CameraCCTV View = new CameraCCTV();
                        View.Height = this.UICCTVContent.Height;
                        View.Width = 1366;
                        View.Camera = App.DataCamera[0];
                        View.Postion = 0;
                        View.setTop(0);
                        View.setLeft(0);
                        this.UICCTVContent.Children.Add(View);
                    }
                    else if (App.DataCamera.Count < 5)
                    {
                        for (int i = 0; i < App.DataCamera.Count; i++)
                        {
                            CameraCCTV View = new CameraCCTV();
                            View.Height = this.UICCTVContent.Height / 2;
                            View.Width = 683;
                            View.Camera = App.DataCamera[i];
                            View.Postion = i;
                            View.setTop((i < 2) ? 0 : this.UICCTVContent.Height / 2.0);
                            View.setLeft((i % 2) * 683);
                            this.UICCTVContent.Children.Add(View);
                        }
                    }
                    else if (App.DataCamera.Count < 10)
                    {
                        int k = 0;
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 3; j++, k++)
                            {
                                if (k < App.DataCamera.Count)
                                {
                                    CameraCCTV View = new CameraCCTV();
                                    View.Height = this.UICCTVContent.Height / 3f;
                                    View.Width = 1366 / 3f;
                                    View.Camera = App.DataCamera[k];
                                    View.Postion = k;
                                    View.setTop(i * this.UICCTVContent.Height / 3.0);
                                    View.setLeft(j * 1366 / 3.0);
                                    this.UICCTVContent.Children.Add(View);
                                }
                            }
                        }
                    }
                    else
                    {
                        int row = App.DataCamera.Count / 4;
                        if (App.DataAlarm.Count % 4 != 0)
                        {
                            row++;
                        }
                        int k = 0;
                        for (int i = 0; i < row; i++)
                        {
                            for (int j = 0; j < 4; j++, k++)
                            {
                                if (k < App.DataCamera.Count)
                                {
                                    CameraCCTV View = new CameraCCTV();
                                    View.Height = this.UICCTVContent.Height / row;
                                    View.Width = 1360 / 4f;
                                    View.Camera = App.DataCamera[k];
                                    View.Postion = k;
                                    View.setTop(i * this.UICCTVContent.Height / row);
                                    View.setLeft(j * 1360 / 4f);
                                    this.UICCTVContent.Children.Add(View);

                                }
                            }
                        }
                    }

                }
            });
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void UIMenuClick(object sender, MouseButtonEventArgs e)
        {
            if (isMenuShow)
            {
                UIContent.Animation_Translate_Frame(double.NaN, double.NaN, -264, 0, 500, () => { isMenuShow = false; });

            }
            else
            {
                UIContent.Animation_Translate_Frame(double.NaN, double.NaN, 0, 0, 500, () => { isMenuShow = true; });

            }
        }

        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            StackPanel Panel = sender as StackPanel;
            Panel.Background = new SolidColorBrush(new Color() { A = 175, R = 155, G = 152, B = 152 });
        }

        private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            StackPanel Panel = sender as StackPanel;
            Panel.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void Logout(object sender, MouseButtonEventArgs e)
        {
            App.Current.MainWindow = new Login();
            App.Current.MainWindow.Show();
            this.Close();
        }

        private void ViewMap(object sender, MouseButtonEventArgs e)
        {
            foreach (Code.Camera Camera in App.DataCamera.Datas)
            {
                CHCNetSDK.NET_DVR_StopRealPlay(Camera.m_lRealHandle);
            }

            if (this.Mode != Camera_Final.Mode.MAP)
            {
                UIContent.Animation_Translate_Frame(double.NaN, double.NaN, -264, 0, 500, () =>
                {
                    isMenuShow = false;
                    this.UIMap.Animation_Translate_Frame(double.NaN, -768, double.NaN, 0);
                    this.UICCTV.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 768);
                    this.Mode = Camera_Final.Mode.MAP;
                });
            }
        }

        private void ViewCCTV(object sender, MouseButtonEventArgs e)
        {
            if (this.Mode != Camera_Final.Mode.CCTV)
            {
                this.count--;
                if (this.count < 0)
                {
                    //System.Diagnostics.Process p= System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                    ProcessStartInfo startInfo = new ProcessStartInfo(Application.ResourceAssembly.Location);
                    startInfo.Arguments = string.Format("-u {0} -mode 2", App.User.Hash);
                    System.Diagnostics.Process.Start(startInfo);
                    Application.Current.Shutdown();
                }
                // if (!isLoadCCTV)
                this.LoadCCTVLayout();

                EasyTimer.SetTimeout(() =>
                {
                    if (App.setting.AutoRecord)
                    {
                        this.autoRecord();
                    }
                }, 3500);

                UIContent.Animation_Translate_Frame(double.NaN, double.NaN, -264, 0, 500, () =>
                {
                    isMenuShow = false;
                    this.UIMap.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 768);
                    this.UICCTV.Animation_Translate_Frame(double.NaN, -768, double.NaN, 0);
                    this.Mode = Camera_Final.Mode.CCTV;
                });
            }
        }

        public void DrawMap()
        {
            this.UIMapContent.Children.Clear();
            if (App.DataCamera != null && App.DataCamera.Count > 0)
            {
                for (int i = 0; i < App.DataCamera.Count; i++)
                {
                    UIMapCamera uiCamera = new UIMapCamera();
                    uiCamera.Width = 34;
                    uiCamera.Height = 170;
                    uiCamera.Postion = i;
                    uiCamera.PreviewMouseLeftButtonDown += uiCamera_PreviewMouseLeftButtonDown;
                    uiCamera.MoveEvent += uiCamera_MoveEvent;
                    uiCamera.ViewCameraEvent += uiCamera_ViewCameraEvent;
                    uiCamera.DeleteCameraEvent += uiCamera_DeleteCameraEvent;
                    uiCamera.AddPresetEvent += uiCamera_AddPresetEvent;
                    uiCamera.ConnectCameraEvent += uiCamera_ConnectCameraEvent;
                    this.UIMapContent.Children.Add(uiCamera);
                }
            }
            if (App.DataPreset != null && App.DataPreset.Count > 0)
            {
                for (int i = 0; i < App.DataPreset.Count; i++)
                {
                    UIMapPreset uiPreset = new UIMapPreset();
                    uiPreset.Width = 34;
                    uiPreset.Height = 170;
                    uiPreset.Preset = App.DataPreset[i];
                    uiPreset.MovePresetEvent += uiPreset_MovePresetEvent;
                    uiPreset.PreviewMouseLeftButtonDown += uiPreset_PreviewMouseDown;
                    uiPreset.DeletePresetEvent += uiPreset_DeletePresetEvent;
                    uiPreset.DebugEvent += uiPreset_DebugEvent;
                    this.UIMapContent.Children.Add(uiPreset);
                }
            }
            if (App.DataAlarm != null && App.DataAlarm.Count > 0)
            {
                for (int i = 0; i < App.DataAlarm.Count; i++)
                {
                    UIMapAlarm uiAlarm = new UIMapAlarm();
                    uiAlarm.Width = 34;
                    uiAlarm.Height = 170;
                    uiAlarm.Postion = i;
                    uiAlarm.MoveEvent += uiAlarm_MoveEvent;
                    uiAlarm.PreviewMouseLeftButtonDown += uiAlarm_PreviewMouseLeftButtonDown;
                    uiAlarm.ViewAlarmEvent += uiAlarm_ViewAlarmEvent;
                    uiAlarm.DeleteAlarmEvent += uiAlarm_DeleteAlarmEvent;
                    uiAlarm.ConnectCameraEvent += uiAlarm_ConnectCameraEvent;
                    uiAlarm.DisableAlarmEvent += uiAlarm_DisableAlarmEvent;
                    uiAlarm.AlertEvent += uiAlarm_AlertEvent;
                    uiAlarm.EditConnectEvent += uiAlarm_EditConnectEvent;
                    uiAlarm.ScheduleEvent += uiAlarm_ScheduleEvent;
                    this.UIMapContent.Children.Add(uiAlarm);
                }
            }
        }

        void uiAlarm_ScheduleEvent(object sender, int e)
        {
            UITimeOff timeview = new UITimeOff();
            timeview.Postion = e;
            timeview.setLeft(500);
            timeview.setTop(1000);
            timeview.Height = 50;
            timeview.Width = 350;
            timeview.CloseEvent += timeview_CloseEvent;
            timeview.SaveDataEvent += timeview_SaveDataEvent;
            this.UIMapContent.Children.Add(timeview);
            timeview.Animation_Translate_Frame(double.NaN, 1000, double.NaN, 500);
        }

        void timeview_SaveDataEvent(object sender, Tuple<int, TimeOff> e)
        {
            if (App.DataAlarm[e.Item1].TimeOffs.Count > 0)
            {
                App.DataAlarm[e.Item1].TimeOffs[0] = e.Item2;
            }
            else
            {
                App.DataAlarm[e.Item1].TimeOffs.Add(e.Item2);
            }
            UITimeOff uiTime = sender as UITimeOff;
            uiTime.Animation_Translate_Frame(double.NaN, 500, double.NaN, 1000, 500, () => { this.UIMapContent.Children.Remove(uiTime); });
        }

        void timeview_CloseEvent(object sender, EventArgs e)
        {
            UITimeOff uiTime = sender as UITimeOff;
            uiTime.Animation_Translate_Frame(double.NaN, 500, double.NaN, 1000, 500, () => { this.UIMapContent.Children.Remove(uiTime); });

        }

        void uiPreset_DebugEvent(object sender, EventArgs e)
        {
            UIMapPreset uiPreset = sender as UIMapPreset;
            this.Dispatcher.Invoke(() =>
            {
                foreach (UIElement uiE in this.UIMapContent.Children)
                {
                    if (uiE is UIMapCamera)
                    {
                        UIMapCamera uiCamera = uiE as UIMapCamera;
                        if (App.DataCamera[uiCamera.Postion].id == uiPreset.Preset.Camera[0].camera_id)
                        {
                            if (uiCamera.alert)
                            {
                                uiCamera.Alarm(false);
                            }
                            else
                            {
                                uiCamera.Alarm(true);
                            }

                            break;
                        }
                    }
                }
            });
        }

        void uiAlarm_EditConnectEvent(object sender, Tuple<int, int> e)
        {
            SmallControlCamera view = new SmallControlCamera();
            if (App.DataAlarm[e.Item2].Cameras != null && App.DataAlarm[e.Item2].Cameras.Count > 0)
                view.preset = App.DataAlarm[e.Item2].Cameras[e.Item1];
            view.Alarm = e.Item2;
            view.Width = 1366;
            view.Height = 768;
            view.setLeft(0);
            view.setTop(0);
            view.CloseEvent += view_CloseEvent;
            this.UIMapContent.Children.Add(view);
            this._alarm = -1;
        }

        private void uiAlarm_AlertEvent(object sender, Tuple<Alarm, bool> e)
        {
            if (e.Item1 != null)
            {
                foreach (Camera_Preset preset in e.Item1.Cameras)
                {
                    foreach (UIElement uiE in this.UIMapContent.Children)
                    {
                        if (uiE is UIMapCamera)
                        {
                            UIMapCamera uiCamera = uiE as UIMapCamera;
                            if (preset.camera_id == App.DataCamera[uiCamera.Postion].id)
                            {
                                if (e.Item2)
                                {
                                    if (uiCamera.Goto((uint)preset.Postion))
                                    {
                                        uiCamera.Alarm();
                                    }
                                }                              

                            }
                        }
                    }
                }
            }
        }

        private int _alarm = -1;

        void uiAlarm_DisableAlarmEvent(object sender, int e)
        {
            this.DrawMap();
        }

        void uiAlarm_ConnectCameraEvent(object sender, int e)
        {
            this._alarm = e;
            MessageBox.Show("hãy click vào camera bạn muốn kết nối", "Thông báo");
        }

        void view_CloseEvent(object sender, EventArgs e)
        {
            this._alarm = -1;
            this.UIMapContent.Children.Remove(sender as SmallControlCamera);
        }

        void uiCamera_ConnectCameraEvent(object sender, Code.Camera e)
        {
            if (e != null)
            {
                if (this._alarm > -1)
                {
                    Code.Camera_Preset alarmPos = new Code.Camera_Preset() { camera_id = e.id, Postion = e.FindFreePreset() };
                    SmallControlCamera view = new SmallControlCamera();
                    view.preset = alarmPos;
                    view.Alarm = this._alarm;
                    view.Camera = e;
                    view.Width = 1366;
                    view.Height = 768;
                    view.setLeft(0);
                    view.setTop(0);
                    view.CloseEvent += view_CloseEvent;
                    this.UIMapContent.Children.Add(view);
                    this._alarm = -1;
                }

            }
        }

        void uiAlarm_DeleteAlarmEvent(object sender, int e)
        {
            if (MessageBox.Show("Bạn có muốn xóa đầu dò này không?", "Thông báo", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                App.DataAlarm.Children.RemoveAt(e);
                this.DrawMap();
            }
        }

        void uiAlarm_ViewAlarmEvent(object sender, int e)
        {
            UIAddAlarm uiAlarm = new UIAddAlarm();
            uiAlarm.setLeft(0);
            uiAlarm.setTop(0);
            uiAlarm.Width = 1366;
            uiAlarm.Height = 768;
            uiAlarm.Alarm = App.DataAlarm[e];
            uiAlarm.CloseEvent += uiAlarm_CloseEvent;
            this.UIMapContent.Children.Add(uiAlarm);
        }

        void uiAlarm_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UIMapAlarm uiAlarm = sender as UIMapAlarm;
            origContentMousePoint = Mouse.GetPosition(uiAlarm);

        }

        void uiAlarm_MoveEvent(object sender, int e)
        {
            UIMapAlarm uiMap = sender as UIMapAlarm;
            Point p = Mouse.GetPosition(this.UIMapContent);
            App.DataAlarm[e].Left = p.X - origContentMousePoint.X;
            App.DataAlarm[e].Top = p.Y - origContentMousePoint.Y;
            uiMap.setLeft(p.X - origContentMousePoint.X);
            uiMap.setTop(p.Y - origContentMousePoint.Y);

        }

        void uiPreset_DeletePresetEvent(object sender, Code.Camera_Goto e)
        {
            if (MessageBox.Show("Bạn có muốn xóa Preset này không?", "Thông báo", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                for (int j = 0; j < e.Camera.Count; j++)
                {
                    for (int i = 0; i < App.DataCamera.Count; i++)
                    {

                        if (App.DataCamera[i].id == e.Camera[j].camera_id)
                        {
                            if (App.DataCamera[i].m_lUserID == -1)
                            {
                                App.DataCamera[i].Login();
                            }
                            if (App.DataCamera[i].m_lUserID != -1)
                            {
                                CHCNetSDK.NET_DVR_PTZPreset_Other(App.DataCamera[i].m_lUserID, 1, CHCNetSDK.CLE_PRESET, (uint)e.Camera[j].Postion);

                            }

                        }
                    }
                }
                App.DataPreset.Remove(e);
                this.DrawMap();
            }
        }

        void uiPreset_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UIMapPreset uiPreset = sender as UIMapPreset;
            origContentMousePoint = Mouse.GetPosition(uiPreset);
        }

        void uiPreset_MovePresetEvent(object sender, Code.Camera_Goto e)
        {
            UIMapPreset uiPreset = sender as UIMapPreset;
            Point p = Mouse.GetPosition(this.UIMapContent);
            uiPreset.setLeft(p.X - origContentMousePoint.X);
            uiPreset.setTop(p.Y - origContentMousePoint.Y);
            for (int i = 0; i < App.DataPreset.Count; i++)
            {
                if (App.DataPreset[i].id == e.id)
                {
                    App.DataPreset[i].left = p.X;
                    App.DataPreset[i].top = p.Y;
                }
            }
        }

        void uiCamera_AddPresetEvent(object sender, Code.Camera e)
        {
            if (e != null)
            {
                AddPreset uiPreset = new AddPreset();
                uiPreset.Camera = e;
                uiPreset.Width = 1366;
                uiPreset.Height = 768;
                uiPreset.setLeft(0);
                uiPreset.setTop(0);
                uiPreset.CloseEvent += uiPreset_CloseEvent;
                this.UIMapContent.Children.Add(uiPreset);
            }
        }

        void uiPreset_CloseEvent(object sender, EventArgs e)
        {
            AddPreset uiPreset = sender as AddPreset;
            this.UIMapContent.Children.Remove(uiPreset);
            this.DrawMap();
        }

        void uiCamera_DeleteCameraEvent(object sender, int e)
        {
            if (MessageBox.Show("Bạn có muốn xóa camera này không?", "Thông báo", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                App.DataCamera.RemoveAt(e);
                this.DrawMap();
            }
        }

        void uiCamera_ViewCameraEvent(object sender, int e)
        {
            ControlCamera uiCamera = new ControlCamera();
            uiCamera.Width = 1366;
            uiCamera.Height = 768;
            uiCamera.Postion = e;
            uiCamera.setLeft(0);
            uiCamera.setTop(0);
            uiCamera.CloseEvent += uiCamera_CloseEvent;
            this.UIMapContent.Children.Add(uiCamera);
        }

        void uiCamera_CloseEvent(object sender, EventArgs e)
        {
            ControlCamera uiCamera = sender as ControlCamera;
            this.UIMapContent.Children.Remove(uiCamera);
        }
        Point origContentMousePoint;
        void uiCamera_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UIMapCamera uiCamera = sender as UIMapCamera;
            origContentMousePoint = Mouse.GetPosition(uiCamera);
        }

        void uiCamera_MoveEvent(object sender, int e)
        {
            UIMapCamera uiCamera = sender as UIMapCamera;
            Point p = Mouse.GetPosition(this.UIMapContent);
            uiCamera.setLeft(p.X - origContentMousePoint.X);
            uiCamera.setTop(p.Y - origContentMousePoint.Y);
            App.DataCamera[e].Left = p.X;
            App.DataCamera[e].Top = p.Y;

        }

        private void AddCamera(object sender, MouseButtonEventArgs e)
        {
            UIContent.Animation_Translate_Frame(double.NaN, double.NaN, -264, 0, 500, () =>
            {
                isMenuShow = false;
                UIAddCamera uiCamera = new UIAddCamera();
                uiCamera.setLeft(0);
                uiCamera.setTop(0);
                uiCamera.Width = 1366;
                uiCamera.Height = 768;
                uiCamera.CloseEvent += uiAddCamera_CloseEvent;
                this.UIMapContent.Children.Add(uiCamera);
            });

        }

        private void uiAddCamera_CloseEvent(object sender, EventArgs e)
        {
            this.DrawMap();
        }

        private void AddAlarm(object sender, MouseButtonEventArgs e)
        {
            UIContent.Animation_Translate_Frame(double.NaN, double.NaN, -264, 0, 500, () =>
            {
                isMenuShow = false;
                UIAddAlarm uiAlarm = new UIAddAlarm();
                uiAlarm.setLeft(0);
                uiAlarm.setTop(0);
                uiAlarm.Width = 1366;
                uiAlarm.Height = 768;
                uiAlarm.CloseEvent += uiAlarm_CloseEvent;
                this.UIMapContent.Children.Add(uiAlarm);
            });
        }

        private void uiAlarm_CloseEvent(object sender, EventArgs e)
        {
            this.UIMapContent.Children.Remove(sender as UIAddAlarm);
            this.DrawMap();
        }

        public void autoRecord()
        {
            this.Dispatcher.Invoke(() =>
            {

                isRecord = true;
                foreach (UIElement uiE in this.UICCTVContent.Children)
                {
                    if (uiE is CameraCCTV)
                    {
                        CameraCCTV uiCamera = uiE as CameraCCTV;
                        uiCamera.RecordFile();

                    }
                }
                UIRecordBtn.Foreground = Brushes.Red;

            });
        }

        private void RecordFile(object sender, MouseButtonEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (!this.isRecord)
                {
                    isRecord = true;
                    foreach (UIElement uiE in this.UICCTVContent.Children)
                    {
                        if (uiE is CameraCCTV)
                        {
                            CameraCCTV uiCamera = uiE as CameraCCTV;
                            uiCamera.RecordFile();

                        }
                    }
                    UIRecordBtn.Foreground = Brushes.Red;
                }
                else
                {
                    foreach (UIElement uiE in this.UICCTVContent.Children)
                    {
                        if (uiE is CameraCCTV)
                        {
                            CameraCCTV uiCamera = uiE as CameraCCTV;
                            uiCamera.StopRecord();

                        }
                    }
                    UIRecordBtn.Foreground = Brushes.White;
                    isRecord = false;
                }
            });

        }

        private void UIWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.TimeCheck != null)
            {
                this.TimeCheck.Abort();
                this.TimeCheck = null;
            }
            if (this.RunCmdThread != null)
            {
                this.RunCmdThread.Abort();
                this.RunCmdThread = null;
            }
        }

        private void UserInfo(object sender, MouseButtonEventArgs e)
        {
            UIContent.Animation_Translate_Frame(double.NaN, double.NaN, -264, 0, 500, () =>
           {
               isMenuShow = false;
               AddUser View = new AddUser();
               View.User = App.User;
               View.CloseEvent += View_CloseEvent;
               View.setLeft(0);
               View.setTop(0);
               View.Width = 1366;
               View.Height = 768;
               this.UIMapContent.Children.Add(View);
           });
        }

        void View_CloseEvent(object sender, EventArgs e)
        {
            this.UIMapContent.Children.Remove(sender as AddUser);
        }

        private async void LockAllAlarm(object sender, RoutedEventArgs e)
        {
            UIUnLock.IsEnabled = false;
            UILock.IsEnabled = false;
            try
            {
                foreach (UIElement uiE in this.UIMapContent.Children)
                {
                    if (uiE is UIMapAlarm)
                    {
                        UIMapAlarm uiAlarm = uiE as UIMapAlarm;
                        if (uiAlarm.Alarm.status)
                        {
                            // await Task.Delay(500);
                            uiAlarm.Alarm.Auto = false;
                            uiAlarm.Disable();
                            await Task.Delay(100);

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetBaseException().ToString());
            }
            this.DrawMap();
            UIUnLock.IsEnabled = true;
            UILock.IsEnabled = true;
        }

        private async void UnLockAllAlarm(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            btn.IsEnabled = false;
            UILock.IsEnabled = false;
            try
            {
                foreach (UIElement uiE in this.UIMapContent.Children)
                {
                    if (uiE is UIMapAlarm)
                    {
                        UIMapAlarm uiAlarm = uiE as UIMapAlarm;
                        if (!uiAlarm.Alarm.status)
                        {
                            uiAlarm.Alarm.Auto = false;
                            uiAlarm.Enable();
                            await Task.Delay(100);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetBaseException().ToString());
            }
            this.DrawMap();
            btn.IsEnabled = true;
            UILock.IsEnabled = true;
        }

        private void Canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            this.UIBarContent.Animation_Translate_Frame(double.NaN, double.NaN, 0, 0);
        }

        private void Canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            this.UIBarContent.Animation_Translate_Frame(double.NaN, double.NaN, 0, 50);
        }

        private void UIWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(Application.ResourceAssembly.Location);
                startInfo.Arguments = string.Format("-u {0} -mode {1}", App.User.Hash, (int)this.Mode);
                System.Diagnostics.Process.Start(startInfo);
                Application.Current.Shutdown();
            }
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.L)
                {
                    using (CsvFileWriter LogCSV = new CsvFileWriter(File.Open(App._FILE_CSV_COMMAND,FileMode.Append),Encoding.UTF8))
                    {
                        foreach (CsvRow r in App.Rows)
                        {
                            LogCSV.WriteRow(r);                         
                        }
                        App.Rows.Clear();
                    }
                }
            }
        }
    }
}
