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
using MTC_Server.Code;
using MTC_Server.Code.Device;
using Alta.Class;
using System.Threading;
using System.Net.NetworkInformation;
using System.Net;
using System.Windows.Threading;
using System.Globalization;

namespace MTC_Server.UIView.Device
{
    public class PlayEvent
    {
        public Code.Media.MediaData Media { get; set; }
        public float Postion { get; set; }
    }
    /// <summary>
    /// Interaction logic for UIDevice.xaml
    /// </summary>
    public partial class UIDevice : UserControl
    {

        private Thread th_OnlineDevice,th_MediaPlay;
        public event EventHandler<DeviceData> ViewInfoEvent;
        public event EventHandler<DeviceData> DeleteEvent;
        public event EventHandler<PlayEvent> PlayMediaEvent;
        private DeviceData d;
        private alta_client client;
        private string key = string.Empty;
        private bool isPlay = true;
        private PlayEvent media;
        public DeviceData Device
        {
            get
            {
                return d;
            }
            set
            {
                d = value;
                this.LoadGUI(value);
            }
        }
        public UIDevice()
        {
            InitializeComponent();
            th_OnlineDevice = new Thread(RunThreadFunctionOnlineDevice);
            th_OnlineDevice.IsBackground = true;
            th_MediaPlay = new Thread(RunThreadFunctionMediaPlay);
            th_MediaPlay.IsBackground = true;
            th_MediaPlay.SetApartmentState(ApartmentState.STA);
            client = new alta_client();
            client.ConnectErr += client_ConnectErr;
            client.ConnectSuccess += client_ConnectSuccess;
            client.RecieveDataEvent += client_RecieveDataEvent;
            this.media = null;
        }
        private void RunThreadFunctionMediaPlay()
        {
            while (true)
            {
                if (media != null )
                {
                    if (this.PlayMediaEvent != null)
                    {
                        this.PlayMediaEvent(this, this.media);
                    }
                    media = null;
                }
                Thread.Sleep(200);
            }
        }

        void client_RecieveDataEvent(object sender, string e)
        {
            string[] cmds = e.Split('_', '|');

            if (cmds[0].ToUpper() == "NOONCE")
            {
                this.key = cmds[1];
                if (App.curUser.Permision.mana_device)
                {
                    this.client.sendData("ADMIN_" + (App.curUser.User_Name + key + App.curUser.Pass).MD5String());
                }
                else
                {
                    this.client.sendData("USER_" + (App.curUser.User_Name + key + App.curUser.Pass).MD5String());
                }
            }
            else if (cmds.Length > 2 && cmds[2].ToUpper() == "CONTROL" && cmds[0] == "OK")
            {
                this.client.canControl = true;
                this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                   {

                       this.Next.Visibility = System.Windows.Visibility.Visible;
                   }));
            }
            else if (cmds.Length > 2 && cmds[2].ToUpper() == "PLAY" && cmds[0] == "OK")
            {
                this.isPlay = true;
                this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                 {
                     this.UIBtnPlay.Text = "\uf04d";
                 }));
            }
            else if (cmds.Length > 2 && cmds[2].ToUpper() == "STOP" && cmds[0] == "OK")
            {
                this.isPlay = false;
                this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                 {
                     this.UIBtnPlay.Text = "\uf04b";
                 }));
            }
            else if (cmds.Length>4 && cmds[0]=="PLAY")
            {
                //PLAY|1_5_12_0.2292163
                Console.WriteLine(e);
                this.media = new PlayEvent() { Media = Code.Media.MediaData.Info(Convert.ToInt32(cmds[3])), Postion = float.Parse(cmds[4], CultureInfo.InvariantCulture.NumberFormat) };
            }
            else
            {
                Console.WriteLine(e);
            }
        }
        void client_ConnectSuccess(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
             {
                 this.UINetwork.Foreground = new SolidColorBrush(Colors.Green);
             }));
        }

        void client_ConnectErr(object sender, EventArgs e)
        {
            Ping();
            this.Dispatcher.Invoke(DispatcherPriority.Background,
                new Action(() =>
                {
                    this.Next.Visibility = System.Windows.Visibility.Collapsed;
                    this.GridBar.Animation_Translate_Frame(double.NaN, double.NaN, 0, double.NaN);

                })
             );
        }

        private void RunThreadFunctionOnlineDevice(object obj)
        {
            while (true)
            {
                Thread.Sleep(5000);
                if (this.d != null && !this.client.isConnected)
                    this.client.connect(this.d.IP);
                else if (this.client.isConnected)
                {
                    this.client.sendData("PING");
                }
            }
        }

        private void Ping()
        {
            if (this.Device != null && this.Device.ID != 0)
            {
                try
                {
                    AutoResetEvent waiter = new AutoResetEvent(false);
                    Ping pingSender = new Ping();
                    pingSender.PingCompleted += new PingCompletedEventHandler(PingCompletedCallback);
                    IPAddress address = IPAddress.Parse(this.Device.IP); ;
                    int timeout = 3000;
                    pingSender.SendAsync(address, timeout, waiter);
                    waiter.Close();
                }
                catch (Exception)
                {

                }
            }
        }

        private void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            PingReply tmp = e.Reply;
            this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                if (tmp.Status == IPStatus.Success)
                {
                    this.UINetwork.Foreground = new SolidColorBrush(Colors.White);
                    //  this.UIBtnOff.Foreground = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    this.UINetwork.Foreground = new SolidColorBrush(Colors.Gray);
                    // this.UIBtnOff.Foreground = new SolidColorBrush(Colors.Black);
                }
            }));
        }

        private void UIRootView_Loaded(object sender, RoutedEventArgs e)
        {
            //this.LoadGUI(this.d);
            this.th_OnlineDevice.Start();
            this.th_MediaPlay.Start();
        }
        private void LoadGUI(DeviceData d)
        {
            if (d.TypeDevice != null && !string.IsNullOrEmpty(d.TypeDevice.Icon))
            {
                this.UIIcon.Text = d.TypeDevice.Icon;
            }

            this.UIIP.Text = d.IP;
            this.UITitle.Text = d.Name;
            this.UITime.Text = d.Time.format();
            this.UIType.Text = d.TypeDevice.Name;
            if (d.User != null)
                this.UIUser.Text = d.User.Full_Name;
            else
                this.UIUser.Text = string.Empty;

            if (d.Status)
            {
                this.UIStatus.Text = Define.Fonts["fa-unlock"].Code;
            }
            else
            {
                this.UIStatus.Text = Define.Fonts["fa-lock"].Code;
            }
        }

        private void UIStatus_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (App.curUser.Permision.mana_device)
            {
                this.Device.Status ^= true;
                if (this.Device.Status)
                {
                    this.UIStatus.Text = Define.Fonts["fa-unlock"].Code;
                }
                else
                {
                    this.UIStatus.Text = Define.Fonts["fa-lock"].Code;
                }
                this.Device.setStatus();
            }
        }

        private void UIBtnInfo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.ViewInfoEvent != null)
                this.ViewInfoEvent(this, this.Device);
        }
        private void UIBtnDelete_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.DeleteEvent != null)
            {
                this.DeleteEvent(this, this.Device);
            }
        }

        private void UIRootView_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.th_OnlineDevice.Abort();
                this.th_OnlineDevice = null;
            }
            catch (Exception)
            {

            }
            try
            {
                this.th_MediaPlay.Abort();
                this.th_MediaPlay = null;
            }
            catch (Exception)
            {

            }
            this.client = null;
        }

        private void UIBtnOff_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.client.canControl)
            {
                if (MessageBox.Show("Bạn có muốn tắt Client này không?", "Cảnh báo", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    this.client.sendData("TURNOFF");
                }
            }
        }

        private void UIBtnPlay_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.client.canControl)
            {
                if (!this.isPlay)
                {
                    this.client.sendData("PLAY");
                }
                else
                {
                    this.client.sendData("STOP");
                }
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (this.GridBar.getLeft() == 0)
            {
                this.GridBar.Animation_Translate_Frame(0, double.NaN, -120, double.NaN);
                this.Next.Content = Define.Fonts["fa-angle-double-left"].Code;
            }
            else
            {
                this.GridBar.Animation_Translate_Frame(-120, double.NaN, 0, double.NaN);
                this.Next.Content = Define.Fonts["fa-angle-double-right"].Code;
            }
        }

        private void UIBtnMedia_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.client.canControl)
            {
                this.client.sendData("SCREEN");
            }
        }

      
    }
}
