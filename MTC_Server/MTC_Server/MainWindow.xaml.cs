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
                    UIContent.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 164);
                    foreach (UIElement e in this.UIMenu.Children)
                    {
                        if(e is UIView.MenuItem)
                        {
                            UIView.MenuItem item = e as UIView.MenuItem;
                            item.Animation_Opacity_View_Frame(true);
                        }
                    }
                }
                else
                {
                    MenuText.Foreground = Brushes.Black;
                    UIBtnMinimize.Foreground = Brushes.Black;
                    UIBtnClose.Foreground = Brushes.Black;
                    UIMenu.Animation_Translate_Frame(double.NaN,double.NaN, 0, -64);
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.UIRoot.RenderTransform = new ScaleTransform(this.Width / W, this.Height / H);
            this.menuActivate = false;
            App.curUser = UserData.Info(App.curUserID);
        }

        private void Asset_Images_btn_menu_png_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.menuActivate = !this.menuActivate;
        }

        private void MenuItem_ClickEvent(object sender, EventArgs e)
        {
            UIView.MenuItem item = sender as UIView.MenuItem;
            this.disableMenu(item);
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
            AltaCache.Write(App.CacheName,App.cache);
            Application.Current.MainWindow = new Login();
            Application.Current.MainWindow.Show();
            this.Close();
        }
    }
}
