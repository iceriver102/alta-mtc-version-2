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
using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Interops.Signatures.LibVlc.MediaListPlayer;
using Vlc.DotNet.Core.Medias;
using MTC_Server.Code;
using Vlc.DotNet.Wpf;
using Alta.Class;
using System.Windows.Media.Animation;

namespace MTC_Server.UIView.Media
{
    /// <summary>
    /// Interaction logic for MediaPlayer.xaml
    /// </summary>
    public partial class MediaPlayer : UserControl
    {
        private double TimeDelta = 500;
        private double TimeHideMouse = 3;
        private bool isVolumOff = false;
        public bool myPositionChanging { get; set; }

        private Code.Media.MediaData m;
        public Code.Media.MediaData Media
        {
            get
            {
                return this.m;
            }
            set
            {
                this.m = value;
                this.UITitle.Text = value.Name;
                if (this.Medias != null)
                {
                    this.index = -1;
                    int i = -1;
                    foreach (Code.Media.MediaData m in this.Medias)
                    {
                        i++;
                        if (m.ID == value.ID)
                        {
                            this.index = i;
                            break;
                        }
                    }
                }
            }
        }
        public void AnimationGUI()
        {
            DoubleAnimation da = new DoubleAnimation(1, 1, TimeSpan.FromMilliseconds(this.TimeDelta));
            da.Completed += (s, e) =>
            {
                if (this.myVlcControl.IsPlaying)
                {
                    this.TimeHideMouse -= this.TimeDelta / 1000;
                    if (this.TimeHideMouse < 0)
                    {
                        if (this.UITitle.Visibility == Visibility.Visible)
                        {
                            this.UIBar.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 520);
                            this.UITitle.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, -25, 400, () => { this.UITitle.Visibility = Visibility.Hidden; });
                        }
                    }
                    else if (this.UITitle.Visibility != Visibility.Visible)
                    {
                        this.UIBar.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 460, 500);
                        this.UITitle.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 0, 400, () => { this.UITitle.Visibility = Visibility.Visible; });
                    }

                }
                else
                {
                    if (this.UITitle.Visibility != Visibility.Visible)
                    {
                        this.UIBar.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 460, 500);
                        this.UITitle.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 0, 400, () => { this.UITitle.Visibility = Visibility.Visible; });
                    }
                    this.TimeHideMouse = 3;
                }
                this.UIRoot.BeginAnimation(OpacityProperty, da);
            };
            this.UIRoot.BeginAnimation(OpacityProperty, da);
        }
        public List<Code.Media.MediaData> Medias;
        private int index;
        public event EventHandler<Code.Media.MediaData> CloseEvent;
        public MediaPlayer()
        {
            InitializeComponent();
            myVlcControl.PlaybackMode = PlaybackModes.Loop;
            myVlcControl.PositionChanged += VlcControlOnPositionChanged;
            myVlcControl.TimeChanged += VlcControlOnTimeChanged;
            myVlcControl.Paused += myVlcControl_Paused;
            myVlcControl.Playing += myVlcControl_Playing;
            myVlcControl.AudioProperties.Volume = Convert.ToInt32(alta_volume.Value);
            myVlcControl.AudioProperties.IsMute = isVolumOff;
        }

        private void myVlcControl_Playing(VlcControl sender, VlcEventArgs<EventArgs> e)
        {
            UIBtnPlay.Text = Define.Fonts["fa-pause"].Code;
        }

        private void myVlcControl_Paused(VlcControl sender, VlcEventArgs<EventArgs> e)
        {

        }

        private void VlcControlOnTimeChanged(VlcControl sender, VlcEventArgs<TimeSpan> e)
        {
            if (myVlcControl.Media == null)
            {
                return;
            }
            alta_txt_curTime.Text = e.Data.Format();
        }

        private void VlcControlOnPositionChanged(VlcControl sender, VlcEventArgs<float> e)
        {
            if (myPositionChanging)
            {
                return;
            }
            barTimeSeek.Value = (int)(e.Data * 100);
        }
        private void Volume_Change_Event(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            myVlcControl.AudioProperties.Volume = Convert.ToInt32(alta_volume.Value);
            if (Convert.ToInt32(alta_volume.Value) <= 0)
            {
                myVlcControl.AudioProperties.IsMute = true;
                UIBtnVolume.Text = Define.Fonts["fa-volume-off"].Code;
            }
            else if (Convert.ToInt32(alta_volume.Value) < 50)
            {
                myVlcControl.AudioProperties.IsMute = false;
                UIBtnVolume.Text = Define.Fonts["fa-volume-down"].Code;
            }
            else
            {
                myVlcControl.AudioProperties.IsMute = false;
                UIBtnVolume.Text = Define.Fonts["fa-volume-up"].Code;
            }
        }

        private void UIBtnClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.myVlcControl.Media != null)
            {
                this.myVlcControl.Media.Dispose();
            }
            if (this.myVlcControl.IsPlaying)
            {
                this.myVlcControl.Stop();
            }
            if (this.CloseEvent != null)
            {
                this.CloseEvent(this, null);
            }
        }

        private void ProgressBarChange(object sender, MouseButtonEventArgs e)
        {
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
                alta_txt_curTime.Text = time.Format();
            }
        }

        private void SliderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            myPositionChanging = true;
            myVlcControl.PositionChanged -= VlcControlOnPositionChanged;
        }

        private void SliderMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            myVlcControl.Position = (float)barTimeSeek.Value / 100;
            myVlcControl.PositionChanged += VlcControlOnPositionChanged;
            myPositionChanging = false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoadGUI();
            this.AnimationGUI();
        }
        private void LoadGUI()
        {
            if (Media != null)
            {
                if (Media.LocalFile != null && !Media.LocalFile.Exists)
                {
                    UIFtp.Local = Media.LocalFile.FullName;
                    UIFtp.Url = Media.Url;
                    UIFtp.FtpUser = App.setting.ftp_user;
                    UIFtp.FtpPassword = App.setting.ftp_password;
                    UIFtp.RunDownLoad();
                    this.UITitle.Visibility = Visibility.Collapsed;
                    this.UIFtp.Visibility = Visibility.Visible;
                }
                else
                {
                    this.UIFtp.Visibility = Visibility.Collapsed;
                    this.PlayMedia(this.Media);
                }
            }
        }

        private void UIBtnVolume_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isVolumOff ^= true;
            myVlcControl.AudioProperties.IsMute = isVolumOff;
            if (!isVolumOff)
            {
                if (Convert.ToInt32(alta_volume.Value) < 50)
                    UIBtnVolume.Text = Define.Fonts["fa-volume-down"].Code;
                else
                    UIBtnVolume.Text = Define.Fonts["fa-volume-up"].Code;
            }
            else
            {
                UIBtnVolume.Text = Define.Fonts["fa-volume-off"].Code;
            }
        }
        private void PlayMedia(Code.Media.MediaData m)
        {
            if (m.LocalFile != null)
            {
                PathMedia media = new PathMedia(m.LocalFile.FullName);
                media.ParsedChanged += MediaOnParsedChanged;
                myVlcControl.Media = media;
            }
            else
            {
                LocationMedia media = m.LocationMedia;
                media.ParsedChanged += MediaOnParsedChanged;
                myVlcControl.Media = media;
            }
            myVlcControl.Play();
        }


        private void MediaOnParsedChanged(MediaBase sender, VlcEventArgs<int> e)
        {
            alta_txt_curTime.Text = myVlcControl.Media.Duration.Format();
        }

        private void UIFtp_CompleteEvent(object sender, string e)
        {
            this.UITitle.Visibility = Visibility.Visible;
            this.UIFtp.Visibility = Visibility.Collapsed;
            this.PlayMedia(this.Media);
        }

        private void UIBtnPlay_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (myVlcControl.Medias.Count > 0)
            {
                if (myVlcControl.IsPlaying)
                {
                    myVlcControl.Pause();
                    UIBtnPlay.Text = Define.Fonts["fa-play"].Code;

                }
                else
                {
                    myVlcControl.Play();
                    UIBtnPlay.Text = Define.Fonts["fa-pause"].Code;
                }
            }
        }

        private void UIBtnStop_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.myVlcControl.Stop();
            barTimeSeek.Value = 0;
        }

        private void UIBtnNext_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.index != -1 && this.index < this.Medias.Count - 1)
            {
                this.index++;
                this.Media = this.Medias[this.index];
                if (this.myVlcControl.IsPlaying)
                {
                    this.myVlcControl.Stop();
                }
                LoadGUI();
            }
        }

        private void UIBtnBack_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.index > 0)
            {
                this.index--;
                if (this.myVlcControl.IsPlaying)
                {
                    this.myVlcControl.Stop();
                }
                this.Media = this.Medias[this.index];
                LoadGUI();
            }
        }
        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            this.TimeHideMouse = 3;
        }
    }
}
