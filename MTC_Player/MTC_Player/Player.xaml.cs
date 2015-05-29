using Code.Media;
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
using System.Windows.Shapes;
using Alta.Class;
using MTC_Player.Code;
using System.Windows.Media.Animation;
using System.Threading;
using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Medias;
using Vlc.DotNet.Wpf;
using Microsoft.Win32;
using System.IO;
using Vlc.DotNet.Core.Interops.Signatures.LibVlc.MediaListPlayer;
using Code;
namespace MTC_Player
{
    public enum MediaState
    {
        Dowloading, Playing, Stop, None,Local
    }
    /// <summary>
    /// Interaction logic for Player.xaml
    /// </summary>
    public partial class Player : Window
    {
        private MediaState MediaState = MediaState.None;
        private bool flag_Load_Data = false;
        private alta_class_net Tcp_Server;
        private AnimationClock clock { get; set; }
        // private AnimationClock mediaClock { get; set; }
        private AnimationClock ClockMouse, mainClock;
        //  private Thread sendImgThread;
        private Thread localThread;

        public Player()
        {
            InitializeComponent();
            Define.keySerect = DateTime.Now.ToString().MD5String();
            this.imgVideo.Stretch = App.setting.ModeVideo;
            this.Topmost = App.setting.Topmost;
            this.Top = App.setting.Top;
            this.Left = App.setting.Left;
            this.alta_volume.Value = App.setting.Volume;
            this.Tcp_Server = new alta_class_net(Define.keySerect);
            this.Tcp_Server.StopVlc += Tcp_Server_StopVlc;
            this.Tcp_Server.PlayVlc += Tcp_Server_PlayVlc;
            this.Tcp_Server.TurnOffApp += Tcp_Server_TurnOffApp;
            this.Tcp_Server.SendPlaying += Tcp_Server_SendPlaying;
            this.WindowState = App.setting.WindowState;
            myVlcControl.PositionChanged += VlcControlOnPositionChanged;
            myVlcControl.TimeChanged += VlcControlOnTimeChanged;
            myVlcControl.Paused += myVlcControl_Paused;
            myVlcControl.Playing += myVlcControl_Playing;
            myVlcControl.Stopped += myVlcControl_Stopped;
            myVlcControl.PlaybackMode = PlaybackModes.Loop;
            this.flag_Load_Data = true;
        }

        void Tcp_Server_SendPlaying(object sender, System.Net.EndPoint e)
        {
            try
            {
                this.Tcp_Server.SendMsg(string.Format("PLAY|{0}_{1}_{2}_{3}", App.curDevice.User.ID, App.curDevice.Detail_ID_Media, App.curDevice.Media.ID,myVlcControl.Position));
            }
            catch (LogoutException )
            {
                MessageBox.Show("Thiết bị đã bị khóa! Hãy liên hệ với admin");
                this.Close();
            }
        }

        void Tcp_Server_TurnOffApp(object sender, System.Net.EndPoint e)
        {
            this.Close();
        }

        void Tcp_Server_PlayVlc(object sender, System.Net.EndPoint e)
        {
            this.Dispatcher.Invoke(() =>
            {
                topImage.Animation_Opacity_View_Frame(false);
            });
            this.MediaState = MTC_Player.MediaState.None;
            this.mainClock.Controller.Resume();
        }

        void Tcp_Server_StopVlc(object sender, System.Net.EndPoint e)
        {
            this.MediaState = MTC_Player.MediaState.Stop;
            this.mainClock.Controller.Pause();
            myVlcControl.Stop();
        }
        private bool? checkFile(MediaData m)
        {
            if (m == null)
                return null;
            if (m.LocalFile != null && m.TypeMedia.Code.ToUpper() == "FILE" && (!m.LocalFile.Exists || m.LocalFile.toMD5() != m.MD5))
                return false;
            return true;
        }
        private void LogOut(object sender, RoutedEventArgs e)
        {
            App.cache.userName = string.Empty;
            App.cache.hashUserName = string.Empty;
            App.curDevice = null;
            this.Dispatcher.Invoke(() =>
            {
                MainWindow Form = new MainWindow();
                Application.Current.MainWindow = Form;
                Application.Current.MainWindow.Show();
                this.Close();
            });
        }
        public void ClockPlay()
        {
            DoubleAnimation tmp = new DoubleAnimation(1, 1, TimeSpan.FromMinutes(3));
            tmp.RepeatBehavior = RepeatBehavior.Forever;
            mainClock = tmp.CreateClock();
            mainClock.CurrentTimeInvalidated += mainClock_CurrentTimeInvalidated;
            demoTxt.ApplyAnimationClock(OpacityProperty, clock);
        }

        float TimeDelay = 2f;
        float TimeDelta = 0.02f;

        private void mainClock_CurrentTimeInvalidated(object sender, EventArgs e)
        {
            TimeDelay -= TimeDelta;
            if (TimeDelay <= 0)
            {
                TimeDelay =2f;
                if (this.MediaState == MTC_Player.MediaState.None || MediaState == MTC_Player.MediaState.Playing)
                {
                    MediaData media= null;
                    try
                    {
                        media = App.curDevice.Media;
                    }
                    catch (LogoutException )
                    {
                        MessageBox.Show("Thiết bị đã bị khóa! Hãy liên hệ với admin");
                        this.Close();
                    }
                    if (media != null && media.ID != 0)
                    {
                        if (this.checkFile(media).Value)
                        {
                            if (App.curDevice.MediaChange)
                            {
                                this.PlayMedia(media.Media);
                                this.txt_alta_media_name.Text = media.Name;
                            }
                            else if (!myVlcControl.IsPlaying)
                            {
                                myVlcControl.Position = 0;
                                myVlcControl.Play();
                            }
                        }
                        else
                        {
                            this.MediaState = MTC_Player.MediaState.Dowloading;
                            myVlcControl.Stop();
                            barTimeSeek.Value = 0;
                            UIFtp.Local = media.LocalFile.FullName;
                            UIFtp.Url = media.Url;
                            UIFtp.FtpUser = App.setting.ftp_user;
                            UIFtp.FtpPassword = App.setting.ftp_password;
                            UIFtp.RunDownLoad();
                            this.UIFtp.Visibility = Visibility.Visible;
                            this.txt_alta_media_name.Text = media.Name + " download ...";
                        }
                    }
                    else
                    {
                        this.MediaState = MTC_Player.MediaState.None;
                        myVlcControl.Stop();
                        this.txt_alta_media_name.Text = string.Empty;
                        barTimeSeek.Value = 0;
                    }
                }
            }
        }

        private void PlayMedia(MediaBase media)
        {
            if (media != null)
            {
                if (myVlcControl.Medias != null && myVlcControl.Medias.Count > 0)
                    myVlcControl.Medias.Clear();
                media.StateChanged -= media_StateChanged;
                media.StateChanged += media_StateChanged;
                media.ParsedChanged -= media_ParsedChanged;
                media.ParsedChanged += media_ParsedChanged;
                myVlcControl.Media = media;
                myVlcControl.Play();
                this.MediaState = MTC_Player.MediaState.Playing;
            }
        }

        void media_ParsedChanged(MediaBase sender, VlcEventArgs<int> e)
        {
            this.durationMedia = TimeSpan.Zero;
        }

        private void media_StateChanged(MediaBase sender, VlcEventArgs<Vlc.DotNet.Core.Interops.Signatures.LibVlc.Media.States> e)
        {
            if (e.Data == Vlc.DotNet.Core.Interops.Signatures.LibVlc.Media.States.Stopped)
            {
                this.btn_play.Content = "\uf04b";
            }
            else if (e.Data == Vlc.DotNet.Core.Interops.Signatures.LibVlc.Media.States.Paused)
            {
                if (this.MediaState == MTC_Player.MediaState.Playing)
                {
                    if (sender.Duration.TotalSeconds - this.durationMedia.TotalSeconds < 1)
                    {
                        myVlcControl.Position = 0f;
                        myVlcControl.Play();
                    }
                    else
                    {
                        this.btn_play.Content = "\uf04b";
                        myVlcControl.Stop();
                    }
                }
            }
            else if (e.Data == Vlc.DotNet.Core.Interops.Signatures.LibVlc.Media.States.Playing)
            {
                this.btn_play.Content = "\uf04c";
            }
        }

        private void UIFtp_CompleteEvent(object sender, string e)
        {
            this.UIFtp.Visibility = Visibility.Collapsed;
            this.MediaState = MTC_Player.MediaState.None;
        }

        #region Window UI Event
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {           
            this.Width = App.setting.Width;
            this.Height = App.setting.Height;
            this.fixResoulution();
            myVlcControl.AudioProperties.Volume = App.setting.Volume;
            if (this.flag_Load_Data)
            {
                ClockPlay();
            }
            this.UIRoot.SizeChanged += UIRoot_SizeChanged;
        }

        private void fixResoulution()
        {
            ScaleTransform s = new ScaleTransform(this.Width / 1366, this.Height / 768);
            this.UIRoot.RenderTransform = s;
        }
        private void UIRootView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                WindowCHange_State_btn_Click(null, null);
            }else
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.R:
                        System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                        Application.Current.Shutdown();
                        break;
                    case Key.S:
                        Config.Write(App.FileName,App.setting);
                        break;
                    case Key.T:
                        DateTime? time = MysqlHelper.getServerTime();
                        if (time != null)
                        {
                            App.UpdateTimeSystem(time.Value.ToUniversalTime());
                        }
                        break;
                }
            }
        }


        private void Window_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_move(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
                App.setting.Top = this.Top;
                App.setting.Left = this.Left;
                e.Handled = true;
            }
        }
        private void showMouse(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            if (ClockMouse != null && ClockMouse.CurrentState == ClockState.Active)
            {
                ClockMouse.Controller.Stop();
            }
            DoubleAnimation daHideControl = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(5));
            ClockMouse = daHideControl.CreateClock();
            ClockMouse.Completed += (o, s) =>
            {
                Mouse.OverrideCursor = Cursors.None;
            };
            demoTxt.ApplyAnimationClock(OpacityProperty, ClockMouse);
        }


        /// <summary>
        /// ẩn thanh điều khiển
        /// </summary>
        private void HideControl(int time = 5)
        {
            Title_Layout.Visibility = Visibility.Visible;
            ControlPlayer_Layout.Visibility = Visibility.Visible;
            if (WindowState == WindowState.Maximized || true)
            {
                if (time > 5)
                {
                    localThread = new Thread(localThreadRun);
                    localThread.IsBackground = true;
                    localThread.Start();
                }
                else
                {
                    DoubleAnimation daHideControl = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(2));
                    clock = daHideControl.CreateClock();
                    daHideControl.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut };
                    clock.Completed += (o, s) =>
                    {
                        Mouse.OverrideCursor = Cursors.None;
                        Title_Layout.Visibility = Visibility.Hidden;
                        ControlPlayer_Layout.Visibility = Visibility.Hidden;
                        try
                        {
                            if (localThread != null && localThread.IsAlive)
                                localThread.Abort();
                        }
                        catch (Exception)
                        {

                        }
                    };

                    Title_Layout.ApplyAnimationClock(OpacityProperty, clock);
                    ControlPlayer_Layout.ApplyAnimationClock(OpacityProperty, clock);
                }
            }
            else
            {
                this.VisibleControl();
            }
        }

        private void localThreadRun(object obj)
        {
            while (true)
            {
                Thread.Sleep(10000);
                if (clock != null)
                    clock.Controller.Begin();
                localThread.Abort();
            }
        }

        /// <summary>
        /// hàm hiện thanh điều khiển
        /// </summary>
        private void VisibleControl()
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            Title_Layout.Visibility = Visibility.Visible;
            ControlPlayer_Layout.Visibility = Visibility.Visible;
            try
            {
                if (clock != null)
                    clock.Controller.Stop();
                Title_Layout.Opacity = 1;
                ControlPlayer_Layout.Opacity = 1;
            }
            catch (Exception)
            {
            }
        }

        private void Changed_State_Event(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (localThread != null && localThread.IsAlive)
                    localThread.Abort();
            }
            catch (Exception)
            {

            }
            VisibleControl();
            HideControl(15);
        }
        private void UIRootView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
            if (this.clock != null)
                this.clock.Controller.Stop();
            this.mainClock.Controller.Stop();
            this.ClockMouse.Controller.Stop();
            if (myVlcControl.IsPlaying)
                myVlcControl.Stop();
           // Config.Write(App.FileName, App.setting);
            this.Tcp_Server = null;
            VlcContext.CloseAll();
        }

        private void UIRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            App.setting.Width = this.Width;
            App.setting.Height = this.Height;
            this.fixResoulution();
        }

        #endregion

        #region VLC
        private bool myPositionChanging;
        private TimeSpan durationMedia;

        #region VLC menthod

        private void myVlcControl_Stopped(VlcControl sender, VlcEventArgs<EventArgs> e)
        {
            topImage.Animation_Opacity_View_Frame(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VlcControlOnPositionChanged(Vlc.DotNet.Wpf.VlcControl sender, VlcEventArgs<float> e)
        {
            if (myPositionChanging)
            {
                return;
            }

            barTimeSeek.Value = (int)(e.Data * 100);
        }
        void myVlcControl_Playing(Vlc.DotNet.Wpf.VlcControl sender, VlcEventArgs<EventArgs> e)
        {
            topImage.Animation_Opacity_View_Frame(false);
            HideControl();
        }
        void myVlcControl_Paused(Vlc.DotNet.Wpf.VlcControl sender, VlcEventArgs<EventArgs> e)
        {

          

        }
        private void VlcControlOnTimeChanged(Vlc.DotNet.Wpf.VlcControl sender, VlcEventArgs<TimeSpan> e)
        {
            if (myVlcControl.Media == null)
            {
                return;
            }
            durationMedia = e.Data;
            alta_txt_curTime.Text = string.Format(
                   "{0:00}:{1:00}:{2:00}",
                   e.Data.Hours,
                   e.Data.Minutes,
                   e.Data.Seconds);

        }

        #endregion

        #region VLC Control

        private void Volume_Change_Event(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int tmp = Convert.ToInt32(alta_volume.Value);
            myVlcControl.AudioProperties.Volume = tmp;
            if (tmp <= 0)
            {
                myVlcControl.AudioProperties.IsMute = true;
                this.btn_mute.Content = "\uf026";
            }
            else if (tmp < 50)
            {
                myVlcControl.AudioProperties.IsMute = true;
                this.btn_mute.Content = "\uf027";
            }
            else if(tmp>=50)
            {
                myVlcControl.AudioProperties.IsMute = false;
                this.btn_mute.Content = "\uf028";
            }
            App.setting.Volume = tmp;
        }

        private void Vlc_btn_next_Click(object sender, RoutedEventArgs e)
        {
            if (mainClock != null && mainClock.CurrentState != ClockState.Stopped)
                mainClock.Controller.Stop();
        }

        private void vlc_btn_back_click(object sender, RoutedEventArgs e)
        {
            if (mainClock != null && mainClock.CurrentState != ClockState.Stopped)
                mainClock.Controller.Stop();
        }

        private void Open_btn_Click(object sender, RoutedEventArgs e)
        {
            myVlcControl.Stop();
            if (myVlcControl.Media != null)
            {
                myVlcControl.Media.ParsedChanged -= MediaOnParsedChanged;
            }
            OpenMedia();
        }

        private void OpenMedia()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Open media file for playback",
                Filter = "All files |*.*"
            };
            if (openFileDialog.ShowDialog() != true)
                return;
            // flag_play_local = true;
            myVlcControl.Media = new PathMedia(openFileDialog.FileName);
            myVlcControl.Media.ParsedChanged += MediaOnParsedChanged;
            myVlcControl.Play();
            this.btn_play.Content = "\uf04c";
            FileInfo File_open = new FileInfo(openFileDialog.FileName);
            txt_alta_media_name.Text = File_open.Name;
        }

        private void MediaOnParsedChanged(Vlc.DotNet.Core.Medias.MediaBase sender, VlcEventArgs<int> e)
        {
            alta_txt_curTime.Text = string.Format(
                "{0:00}:{1:00}:{2:00}",
                myVlcControl.Media.Duration.Hours,
                myVlcControl.Media.Duration.Minutes,
                myVlcControl.Media.Duration.Seconds);
        }
        private void btn_Click_Mute(object sender, RoutedEventArgs e)
        {
            if (myVlcControl.AudioProperties.IsMute)
            {
                myVlcControl.AudioProperties.IsMute = false;
                this.btn_mute.Content = "\uf028";
            }
            else
            {
                myVlcControl.AudioProperties.IsMute = true;
                
                this.btn_mute.Content = "\uf026";
            }

        }

        private void btn_Play_Event(object sender, RoutedEventArgs e)
        {
            if (myVlcControl.Medias.Count > 0)
            {
                if (myVlcControl.IsPlaying)
                {
                    myVlcControl.Pause();
                    this.btn_play.Content = "\uf04b";

                    if (this.mainClock != null)
                        this.mainClock.Controller.Pause();
                    // btn_play.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Alta_Media_Player;component/Asset/Themes/ic_action_play.png")));
                }
                else
                {
                    myVlcControl.Play();
                    this.btn_play.Content = "\uf04c";
                    if (this.mainClock != null)
                        this.mainClock.Controller.Resume();
                    //btn_play.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Alta_Media_Player;component/Asset/Themes/ic_action_pause.png")));
                }
            }
           

        }

        private void btn_Click_Stop(object sender, RoutedEventArgs e)
        {
            myVlcControl.Stop();
            //flagPlaying = false;
            btn_play.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Alta_Media_Player;component/Asset/Themes/ic_action_play.png")));
            barTimeSeek.Value = 0;
            if (mainClock != null && mainClock.CurrentState != ClockState.Stopped)
                mainClock.Controller.Stop();

        }

        /// <summary>
        /// sự kiện seekbar change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProgressBarChange(object sender, MouseButtonEventArgs e)
        {
            //Update the current position text when it is in pause
            var duration = myVlcControl.Media == null ? TimeSpan.Zero : myVlcControl.Media.Duration;
            var time = TimeSpan.FromMilliseconds(duration.TotalMilliseconds * myVlcControl.Position);
            double pos;
            pos = (e.GetPosition(barTimeSeek).X / barTimeSeek.ActualWidth) * 100;
            if (pos >= barTimeSeek.Minimum && pos <= barTimeSeek.Maximum)
            {
                barTimeSeek.Value = (int)pos;
                if (myPositionChanging)
                {
                    myVlcControl.Position = (float)pos / 100;
                }
                // var duration = myVlcControl.Media.Duration;
                alta_txt_curTime.Text = string.Format(
                    "{0:00}:{1:00}:{2:00}",
                    time.Hours,
                    time.Minutes,
                    time.Seconds);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SliderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            myPositionChanging = true;
            myVlcControl.PositionChanged -= VlcControlOnPositionChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SliderMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            myVlcControl.Position = (float)barTimeSeek.Value / 100;
            myVlcControl.PositionChanged += VlcControlOnPositionChanged;
            myPositionChanging = false;
        }

        private void WindowCHange_State_btn_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState.Maximized == this.WindowState)
            {
                this.WindowState = WindowState.Normal;
                this.btn_full_screen.Content = "\uf0b2";
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                this.btn_full_screen.Content = "\uf066";
            }
            App.setting.WindowState = this.WindowState;

            HideControl();
        }


        #endregion VLC Control

        


        #endregion VLC
    }
}
