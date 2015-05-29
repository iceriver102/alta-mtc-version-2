using MTC_Server.Code;
using MySql.Data.MySqlClient;
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
using MTC_Server.Code.User;
using MTC_Server.UIView.User;
using MTC_Server.UIView.Media;
using MTC_Server.UIView.Device;
using System.Threading;
using MTC_Server.UIView.Schedule;
using MTC_Server.UIView.Playlist.UI;

namespace MTC_Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double W = 1366;
        private const double H = 768;
        private bool _menu_active = false;
        public int Time = 3;
        public bool menuActivate
        {
            get
            {
                return this._menu_active;
            }
            set
            {
                this._menu_active = value;
                if (this._menu_active)
                {
                    UIMenu.Animation_Translate_Frame(double.NaN, double.NaN, 0, 0);
                    MenuText.Foreground = Brushes.DarkOrange;
                    UIBtnClose.Foreground = Brushes.DarkOrange;
                    UIBtnMinimize.Foreground = Brushes.DarkOrange;
                    UILogOut.Foreground = Brushes.DarkOrange;
                    UIContent.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 164);
                    foreach (UIElement e in this.UIMenu.Children)
                    {
                        if (e is UIView.MenuItem)
                        {
                            UIView.MenuItem item = e as UIView.MenuItem;
                            item.Animation_Opacity_View_Frame(true);
                        }
                    }
                }
                else
                {
                    UILogOut.Foreground = Brushes.Black;
                    MenuText.Foreground = Brushes.Black;
                    UIBtnMinimize.Foreground = Brushes.Black;
                    UIBtnClose.Foreground = Brushes.Black;
                    UIMenu.Animation_Translate_Frame(double.NaN, double.NaN, 0, -64);
                    UIContent.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 100);
                    foreach (UIElement e in this.UIMenu.Children)
                    {
                        if (e is UIView.MenuItem)
                        {
                            UIView.MenuItem item = e as UIView.MenuItem;
                            item.Animation_Opacity_View_Frame(false);
                        }
                    }
                }
            }
        }
        public Thread mainThread, StaThread;

        public MainWindow()
        {
            InitializeComponent();
            this.Time = App.setting.secondAutoLogout;
            this.mainThread = new Thread(RunMainThread);
            this.mainThread.IsBackground = true;
            this.StaThread = new Thread(AutoLogOff);
            this.StaThread.SetApartmentState(ApartmentState.STA);
            this.StaThread.IsBackground = true;
        }

        public void RunMainThread()
        {
            while (this.Time > 0)
            {
                Thread.Sleep(1000);
                this.Time--;
                if (this.StaThread.ThreadState != ThreadState.Running && this.Time <= 0)
                {
                    this.StaThread.Start();
                }
            }
        }
        public void AutoLogOff()
        {
            this.Dispatcher.Invoke(() =>
            {
                UILogin form = new UILogin();
                form.cacheName = App.curUser.User_Name;
                form.Template = new DPFP.Template();
                form.Template.DeSerialize(App.curUser.Finger_Print);
                Application.Current.MainWindow = form;
                Application.Current.MainWindow.Show();
                this.StaThread.Abort();
                this.StaThread = null;
                this.Close();
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.UIRoot.RenderTransform = new ScaleTransform(this.Width / W, this.Height / H);
            this.menuActivate = false;
            this.mainThread.Start();

        }

        private void Asset_Images_btn_menu_png_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.menuActivate = !this.menuActivate;
        }

        private void MenuItem_ClickEvent(object sender, EventArgs e)
        {
            UIView.MenuItem item = sender as UIView.MenuItem;
            this.disableMenu(item);
            double time = 100;
            double position = this.UIContent.getLeft();
            double itemPostion = 0;
            if (UIContent.Children.Count > 0)
            {
                itemPostion = UIContent.Children[UIContent.Children.Count - 1].getLeft();
            }

            UIContent.Animation_Translate_Frame(double.NaN, double.NaN, position, double.NaN, time, () =>
            {
                if (UIContent.Children.Count > 1)
                {
                    this.UIContent.Children.RemoveAt(0);
                }
                switch (item.Code)
                {
                    case "User":
                        // this.UIContent.Children.Clear();
                        GridViewUser tmpItem = new GridViewUser();
                        tmpItem.BrowserMediaEvent += tmpItem_BrowserMediaEvent;
                        tmpItem.BrowserCameraEvent += tmpItem_BrowserCameraEvent;
                        tmpItem.Width = 1366;
                        tmpItem.Height = 668;
                        tmpItem.setLeft(itemPostion + 1366);
                        tmpItem.setTop(0);
                        this.UIContent.Children.Add(tmpItem);
                        break;
                    case "Media":
                        // this.UIContent.Children.Clear();
                        GridMedia tmpMediaItem = new GridMedia();
                        tmpMediaItem.Width = 1366;
                        tmpMediaItem.Height = 668;
                        tmpMediaItem.setLeft(itemPostion + 1366);
                        tmpMediaItem.setTop(0);
                        tmpMediaItem.User = null;
                        this.UIContent.Children.Add(tmpMediaItem);
                        break;
                    case "Camera":
                        // this.UIContent.Children.Clear();
                        GridMedia tmpCameraItem = new GridMedia();
                        tmpCameraItem.Width = 1366;
                        tmpCameraItem.Height = 668;
                        tmpCameraItem.setLeft(itemPostion + 1366);
                        tmpCameraItem.setTop(0);
                        tmpCameraItem.TypeMedia = 2;
                        this.UIContent.Children.Add(tmpCameraItem);
                        break;
                    case "Device":
                        // this.UIContent.Children.Clear();
                        GridViewDevice tmpDeviceItem = new GridViewDevice();
                        tmpDeviceItem.Width = 1366;
                        tmpDeviceItem.Height = 668;
                        tmpDeviceItem.setLeft(itemPostion + 1366);
                        tmpDeviceItem.setTop(0);
                        this.UIContent.Children.Add(tmpDeviceItem);
                        break;
                    case "Schedule":
                        // this.UIContent.Children.Clear();
                        MTC_Server.UIView.Schedule.UISchedule tmpCalendar = new UIView.Schedule.UISchedule();
                        tmpCalendar.Width = 1366;
                        tmpCalendar.Height = 668;
                        tmpCalendar.setLeft(itemPostion + 1366);
                        tmpCalendar.setTop(0);
                        this.UIContent.Children.Add(tmpCalendar);
                        break;
                    case "Playlist":
                        // this.UIContent.Children.Clear();
                        MTC_Server.UIView.Playlist.GridPlaylist tmpPlaylist = new MTC_Server.UIView.Playlist.GridPlaylist();
                        tmpPlaylist.Width = 1366;
                        tmpPlaylist.Height = 668;
                        tmpPlaylist.setLeft(itemPostion + 1366);
                        tmpPlaylist.setTop(0);
                        this.UIContent.Children.Add(tmpPlaylist);
                        break;
                    case "About":
                        About window = new About();
                        window.Show();
                        return;
                }
                this.UIContent.Animation_Translate_Frame(double.NaN, double.NaN, -itemPostion - 1366, double.NaN, 600,
                () =>
                {
                    if (UIContent.Children.Count != 0)
                        this.UIContent.Children.RemoveAt(0);
                });
            });
        }

       

        void tmpItem_BrowserCameraEvent(object sender, UserData e)
        {
            GridMedia tmpMediaItem = new GridMedia();
            tmpMediaItem.Width = 1366;
            tmpMediaItem.Height = 668;
            double itemPostion = 0;
            if (UIContent.Children.Count > 0)
                itemPostion = UIContent.Children[UIContent.Children.Count - 1].getLeft();
            tmpMediaItem.setLeft(itemPostion - 1366);
            tmpMediaItem.setTop(0);
            tmpMediaItem.User = e;
            tmpMediaItem.TypeMedia = 2;
            tmpMediaItem.BackEvent += tmpMediaItem_BackEvent;
            this.UIContent.Children.Add(tmpMediaItem);
            UIContent.Animation_Translate_Frame(double.NaN, double.NaN, -itemPostion + 1366, double.NaN, 500);
        }

        void tmpMediaItem_BackEvent(object sender, EventArgs e)
        {
            double itemPostion = UIContent.getLeft();
            UIContent.Animation_Translate_Frame(double.NaN, double.NaN, itemPostion - 1366, double.NaN, 500, () => { this.UIContent.Children.RemoveAt(1); });
        }

        void tmpItem_BrowserMediaEvent(object sender, UserData e)
        {
            GridMedia tmpMediaItem = new GridMedia();
            tmpMediaItem.BackEvent += tmpMediaItem_BackEvent;
            tmpMediaItem.Width = 1366;
            tmpMediaItem.Height = 668;
            double itemPostion = 0;
            if (UIContent.Children.Count > 0)
                itemPostion = UIContent.Children[UIContent.Children.Count - 1].getLeft();
            tmpMediaItem.setLeft(itemPostion - 1366);
            tmpMediaItem.setTop(0);
            tmpMediaItem.User = e;
            this.UIContent.Children.Add(tmpMediaItem);
            UIContent.Animation_Translate_Frame(double.NaN, double.NaN, -itemPostion + 1366, double.NaN, 500);
        }
        private void disableMenu(UIView.MenuItem item)
        {
            foreach (UIElement E in this.UIMenu.Children)
            {
                if (E is UIView.MenuItem)
                {
                    UIView.MenuItem i = E as UIView.MenuItem;
                    if (i.isActive && i != item)
                    {
                        i.isActive = false;
                    }
                }
            }
        }

        private void UIBtnClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void UIBtnMinimize_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void UILogOut_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            App.cache.hashUserName = string.Empty;
            AltaCache.Write(App.CacheName, App.cache);
            App.curUserID = 0;
            App.curUser = null;
            Application.Current.MainWindow = new UILogin();
            Application.Current.MainWindow.Show();
            this.Close();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            this.Time = App.setting.secondAutoLogout;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            this.Time = App.setting.secondAutoLogout;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Time = App.setting.secondAutoLogout;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.mainThread.Abort();
            this.mainThread = null;
        }
    }
}
