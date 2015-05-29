using Alta.Class;
using MTC_Server.Code.User;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MTC_Server
{
    /// <summary>
    /// Interaction logic for UILogin.xaml
    /// </summary>
    public partial class UILogin : Window, DPFP.Capture.EventHandler
    {
        private DPFP.Capture.Capture Capturer;
        private byte[] CacheFingerPrinter;
        public new DPFP.Template Template;
        private DPFP.Verification.Verification Verificator;
        protected DPFP.FeatureSet ExtractFeatures(DPFP.Sample Sample, DPFP.Processing.DataPurpose Purpose)
        {
            DPFP.Processing.FeatureExtraction Extractor = new DPFP.Processing.FeatureExtraction();	// Create a feature extractor
            DPFP.Capture.CaptureFeedback feedback = DPFP.Capture.CaptureFeedback.None;
            DPFP.FeatureSet features = new DPFP.FeatureSet();
            Extractor.CreateFeatureSet(Sample, Purpose, ref feedback, ref features);			// TODO: return features as a result?
            if (feedback == DPFP.Capture.CaptureFeedback.Good)
                return features;
            else
                return null;
        }
        public UILogin()
        {
            InitializeComponent();
            
            UIFinger_Printer.Foreground = new SolidColorBrush(Colors.White);
            Verificator = new DPFP.Verification.Verification();
            try
            {
                Capturer = new DPFP.Capture.Capture();				// Create a capture operation.
                if (null != Capturer)
                    Capturer.EventHandler = this;					// Subscribe for capturing events.
               
            }
            catch
            {
               
            }
           
        }
        public void Verify(DPFP.Sample Sample)
        {
            DPFP.FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Verification);

            // Check quality of the sample and start verification if it's good
            // TODO: move to a separate task
            if (features != null)
            {
                // Compare the feature set with our template
                DPFP.Verification.Verification.Result result = new DPFP.Verification.Verification.Result();
                Verificator.Verify(features, Template, ref result);
                if (result.Verified)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        if (this.ClockShow != null)
                            this.ClockShow.Controller.Pause();
                        ColorAnimation animation = new ColorAnimation();
                        animation.From = (this.UIFinger_Printer.Foreground as SolidColorBrush).Color;
                        animation.To = Colors.Green;
                        animation.Duration = new Duration(TimeSpan.FromMilliseconds(450));
                        animation.EasingFunction = new PowerEase() { Power = 5, EasingMode = EasingMode.EaseInOut };
                        animation.Completed += (s,e) => 
                        {
                            App.curUser = UserData.Info(this.cacheName);
                            App.curUserID = App.curUser.ID;
                            if (this.UIAutoLogin.IsChecked.Value)
                            {
                                App.cache.hashUserName = App.getHash(App.curUserID);
                                App.cache.userName = Encrypt.EncryptString(this.UITxtName.Text.Trim(), FunctionStatics.getCPUID());
                                AltaCache.Write(App.CacheName, App.cache);
                            }
                            else
                            {
                                App.cache.userName = string.Empty;
                                App.cache.hashUserName = string.Empty;
                            }
                            this.UIFullName.Text = App.curUser.Full_Name;
                            this.UILoginFinger.Animation_Translate_Frame(double.NaN, double.NaN, 400, double.NaN, 500);
                            this.UILoginSusscess.Animation_Translate_Frame(-400, double.NaN, 0, double.NaN, 500, () => { LoadData(); });
                        };
                        this.ClockHide = animation.CreateClock();
                        this.UIFinger_Printer.Foreground.ApplyAnimationClock(SolidColorBrush.ColorProperty, ClockHide);
                        this.UIFinger_Status.Text = string.Empty;
                    });                   
                    this.Stop();
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.UIFinger_Status.Text = "Try again ...";
                    });
                }
            }
        }

        private void TextBlock_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void WindowClose(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void CheckBox_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock tmp = (sender as CheckBox).Content as TextBlock;
            tmp.TextDecorations = TextDecorations.Underline;
        }

        private void UIAutoLogin_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock tmp = (sender as CheckBox).Content as TextBlock;
            tmp.TextDecorations = null;
        }
        public void LoadData()
        {
            EasyTimer.SetTimeout(() =>
            {
                this.Dispatcher.Invoke(() => {
                    Application.Current.MainWindow = new MainWindow();
                    Application.Current.MainWindow.Show();
                    this.Close();
                });
            }, 3,System.Threading.ApartmentState.STA);
           
        }

        private int LoginData(string _username, string _password)
        {
            int result = 0;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "SELECT  `Fc_Login` (@name_user , @pass_user) AS  `user_id`";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name_user", _username);
                        cmd.Parameters.AddWithValue("@pass_user", _password);
                        var tmp = cmd.ExecuteScalar();
                        result = (int)tmp;
                    };
                    conn.Close();
                };
            }
            catch (MySqlException)
            {
                MessageBox.Show("Không thể kết nối với csdl");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return result;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.UITxtName.Text))
            {                
                this.UIErr.Text = "* Tên đăng nhập không được để trống!";
                this.UIErr.Animation_Opacity_View_Frame(true);
                return;
            }
            if (this.UIErr.Visibility == System.Windows.Visibility.Visible && this.UIErr.Opacity!=0)
            {
                this.UIErr.Animation_Opacity_View_Frame(false);
            }
            if (string.IsNullOrEmpty(UIPassword.Password))
            {
                this.UIErPass.Animation_Opacity_View_Frame(true);
                return;
            }
            if (this.UIErPass.Visibility == System.Windows.Visibility.Visible && this.UIErPass.Opacity != 0)
            {
                this.UIErPass.Animation_Opacity_View_Frame(false);
            }
            int result = LoginData(UITxtName.Text, FunctionStatics.MD5String(UIPassword.Password));
            if (result == 0)
            {
                this.UIErr.Text = "* Tên đăng nhập hoặc mật khẩu không đúng!";
                this.UIErr.Animation_Opacity_View_Frame(true);
                return;
            }
            else
            {
                this.UIErr.Text = string.Empty;
                this.UIErr.Animation_Opacity_View_Frame(false);
                App.curUserID = result; 
                App.curUser = UserData.Info(App.curUserID);                
                if (this.UIAutoLogin.IsChecked.Value)
                {
                    App.cache.hashUserName = App.getHash(result);
                    App.cache.userName = Encrypt.EncryptString(this.UITxtName.Text.Trim(), FunctionStatics.getCPUID());
                    AltaCache.Write(App.CacheName, App.cache);
                }
                else
                {
                    App.cache.userName = string.Empty;
                    App.cache.hashUserName = string.Empty;
                }
                this.UIFullName.Text = App.curUser.Full_Name;
                this.UILoginForm.Animation_Translate_Frame(double.NaN, double.NaN, 400, double.NaN,500);
                this.UILoginSusscess.Animation_Translate_Frame(-400, double.NaN, 0, double.NaN, 500, () => { LoadData(); });
                
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.cache.autoLogin)
            {
                this.UIAutoLogin.IsChecked = true;
            }

            //if (App.curUserID > 0)
            //{
            //    if (App.curUser == null)
            //    {
            //        App.curUser = UserData.Info(App.curUserID);
            //        this.UIAvatar.Text = App.curUser.Full_Name[0].ToString().ToUpper();
            //        this.UIFullName.Text = App.curUser.Full_Name;
            //        this.UILoginSusscess.Animation_Translate_Frame(-400, double.NaN, 0, double.NaN, 500, () => { LoadData(); });
            //    }
            //    else
            //    {
                   
            //        this.UIFullName_off.Text = App.curUser.Full_Name;
            //        this.UILoginLogOff.Animation_Translate_Frame(-400, double.NaN, 0, double.NaN, 500);
            //    }
            //    this.UILoginForm.Animation_Translate_Frame(double.NaN, double.NaN, 400, double.NaN, 500);
                
            //}

            if (!string.IsNullOrEmpty(this.cacheName))
            {
                if (this.Template != null)
                {
                    this.UIFullName_Finger.Text = UserData.getFullName(cacheName);                   
                    this.UILoginFinger.Animation_Translate_Frame(-400, double.NaN, 0, double.NaN, 500);
                    this.Start();
                }
                else
                {
                    this.UIFullName_off.Text = UserData.getFullName(cacheName);
                    this.UILoginLogOff.Animation_Translate_Frame(-400, double.NaN, 0, double.NaN, 500);

                }
                this.UILoginForm.Animation_Translate_Frame(double.NaN, double.NaN, 400, double.NaN, 500);
            }

            if (!string.IsNullOrEmpty(App.cache.userName))
            {
                this.UITxtName.Text = Encrypt.DecryptString(App.cache.userName, FunctionStatics.getCPUID());
                this.UIMaskUserName.Animation_Opacity_View_Frame(false, null, 200);
            }
            this.UIFinger_Status.Text = string.Empty;
            this.UIFinger_Status.Foreground = new SolidColorBrush(Colors.White);
            ColorAnimation animation = new ColorAnimation();
            animation.From = Colors.Black;
            animation.To = Colors.OrangeRed;
            animation.AutoReverse = true;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(450));
            animation.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut, Power = 3 };
            animation.RepeatBehavior = RepeatBehavior.Forever;
            this.UIFinger_Status.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.cache.autoLogin = this.UIAutoLogin.IsChecked.Value;
            AltaCache.Write(App.CacheName, App.cache);
            Stop();
        }
        protected void Start()
        {
            if (null != Capturer)
            {
                try
                {
                    Capturer.StartCapture();
                }
                catch
                {
                }
            }
        }

        protected void Stop()
        {
            if (null != Capturer)
            {
                try
                {
                    Capturer.StopCapture();
                }
                catch
                {
                  
                }
            }
        }

        private void UITxtName_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(this.UITxtName.Text) || string.IsNullOrEmpty(this.UIPassword.Password))
            {
                this.UILoginButton.IsEnabled = false;
                return;
            }
            else
            {
                UILoginButton.IsEnabled = true;
               
            }
            if (e.Key == Key.Enter)
            {
                this.Button_Click(null, null);
            }
        }

        private void UITxtName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.UITxtName.Text))
            {
                this.UIMaskUserName.Animation_Opacity_View_Frame(true, null, 200);
            }
        }

        private void UITxtName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.UITxtName.Text))
                this.UIMaskUserName.Animation_Opacity_View_Frame(false, null, 200);
        }

        private void UIPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.UIPassword.Password))
            {
                this.UIMaskPassword.Animation_Opacity_View_Frame(true, null, 200);
            }
        }

        private void UIPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.UIPassword.Password))
                this.UIMaskPassword.Animation_Opacity_View_Frame(false, null, 200);
        }

        private void ReLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(UIPassword_off.Password))
            {
                this.UIErrRelogin.Text = "* Mật khẩu đăng nhập không được để trống!";
                this.UIErrRelogin.Animation_Opacity_View_Frame(true);
                return;
            }
            int result = LoginData(this.cacheName, FunctionStatics.MD5String(UIPassword_off.Password));
            if (result == 0)
            {
                this.UIErrRelogin.Text = "* Mật khẩu đăng nhập không đúng!";
                this.UIErrRelogin.Animation_Opacity_View_Frame(true);
                return;
            }
            else
            {
                App.curUserID = result;
                App.curUser = UserData.Info(App.curUserID);
                if (App.cache.autoLogin)
                {
                    App.cache.hashUserName = App.getHash(result);
                    App.cache.userName = Encrypt.EncryptString(this.UITxtName.Text.Trim(), FunctionStatics.getCPUID());
                    AltaCache.Write(App.CacheName, App.cache);
                }
                else
                {
                    App.cache.userName = string.Empty;
                    App.cache.hashUserName = string.Empty;
                }
                this.UIAvatar.Text = App.curUser.Full_Name[0].ToString();
                this.UIFullName.Text = App.curUser.Full_Name;
                this.UILoginLogOff.Animation_Translate_Frame(double.NaN, double.NaN, 400, double.NaN, 500);
                this.UILoginSusscess.Animation_Translate_Frame(-400, double.NaN, 0, double.NaN, 500, () => { LoadData(); });

            }
        }

        private void UIPassword_off_KeyUp(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.UIPassword_off.Password) && e.Key== Key.Enter)
            {
                this.ReLogin_Click(null, null);
            }
            else if (!string.IsNullOrEmpty(this.UIPassword_off.Password))
            {
                UILoginButton_Off.IsEnabled = true;
            }
        }

        private void UIMaskPassword_off_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.UIPassword_off.Password))
                this.UIMaskPassword_off.Animation_Opacity_View_Frame(false, null, 200);
        }

        private void UIMaskPassword_off_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.UIPassword_off.Password))
                this.UIMaskPassword_off.Animation_Opacity_View_Frame(true, null, 200);
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            App.curUserID = 0;
            App.curUser = null;
            this.UITxtName.Focus();
            this.UITxtName.Text = string.Empty;
            this.UILoginLogOff.Animation_Translate_Frame(double.NaN, double.NaN, 400, double.NaN, 500);
            this.UILoginForm.Animation_Translate_Frame(-400, double.NaN, 0, double.NaN, 500);
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.CacheFingerPrinter != null)
            {
                this.Template = new DPFP.Template();
                this.Template.DeSerialize(this.CacheFingerPrinter);
                this.UIFullName_Finger.Text = UserData.getFullName(cacheName);
                this.UILoginForm.Animation_Translate_Frame(double.NaN, double.NaN, 400, double.NaN, 500);
                this.UILoginFinger.Animation_Translate_Frame(-400, double.NaN, 0, double.NaN, 500);
                this.Start();
            }
        }
        public string cacheName { get; set; }
        private void UITxtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (!string.IsNullOrEmpty(txt.Text))
            {
                CacheFingerPrinter = UserData.getFingerPrinter(txt.Text);
                if (CacheFingerPrinter != null)
                {
                    cacheName = txt.Text;
                }
            }
        }

        private void TextBlock_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            this.UILoginFinger.Animation_Translate_Frame(double.NaN, double.NaN, 400, double.NaN, 500);
            this.UILoginForm.Animation_Translate_Frame(-400, double.NaN, 0, double.NaN, 500);
            this.Stop();
        }


        public void OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
        {
            this.Verify(Sample);
        }

        public void OnFingerGone(object Capture, string ReaderSerialNumber)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (this.ClockShow != null)
                    this.ClockShow.Controller.Pause();
                ColorAnimation animation = new ColorAnimation();
                animation.From = (this.UIFinger_Printer.Foreground as SolidColorBrush).Color;
                animation.To = Colors.Blue;
                animation.Duration = new Duration(TimeSpan.FromMilliseconds(450));
                animation.EasingFunction = new PowerEase() { Power = 5, EasingMode = EasingMode.EaseInOut };
                this.ClockHide = animation.CreateClock();
                this.UIFinger_Printer.Foreground.ApplyAnimationClock(SolidColorBrush.ColorProperty, ClockHide);
            });
        }

        public void OnFingerTouch(object Capture, string ReaderSerialNumber)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (this.ClockHide != null)
                    this.ClockHide.Controller.Pause();
                ColorAnimation animation = new ColorAnimation();
                animation.From = (this.UIFinger_Printer.Foreground as SolidColorBrush).Color;
                animation.To = Colors.Red;
                animation.Duration = new Duration(TimeSpan.FromMilliseconds(450));
                animation.EasingFunction = new PowerEase() { Power = 5, EasingMode = EasingMode.EaseInOut };
                this.ClockShow = animation.CreateClock();
                this.UIFinger_Printer.Foreground.ApplyAnimationClock(SolidColorBrush.ColorProperty, ClockShow);
            });
        }

        public void OnReaderConnect(object Capture, string ReaderSerialNumber)
        {
            this.Dispatcher.Invoke(() =>
            {
                ColorAnimation animation = new ColorAnimation();
                animation.From = (this.UIFinger_Printer.Foreground as SolidColorBrush).Color;
                animation.To = Colors.Blue;
                animation.Duration = new Duration(TimeSpan.FromMilliseconds(450));
                animation.EasingFunction = new PowerEase() { Power = 5, EasingMode = EasingMode.EaseInOut };
                this.ClockHide = animation.CreateClock();
                this.UIFinger_Printer.Foreground.ApplyAnimationClock(SolidColorBrush.ColorProperty, ClockHide);
            });
        }

        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (this.ClockHide != null)
                    this.ClockHide.Controller.Pause();
                ColorAnimation animation = new ColorAnimation();
                animation.From = (this.UIFinger_Printer.Foreground as SolidColorBrush).Color;
                animation.To = Colors.Black;
                animation.Duration = new Duration(TimeSpan.FromMilliseconds(450));
                animation.EasingFunction = new PowerEase() { Power = 5, EasingMode = EasingMode.EaseInOut };
                this.ClockShow = animation.CreateClock();
                this.UIFinger_Printer.Foreground.ApplyAnimationClock(SolidColorBrush.ColorProperty, ClockShow);
            });
        }

        public void OnSampleQuality(object Capture, string ReaderSerialNumber, DPFP.Capture.CaptureFeedback CaptureFeedback)
        {
            
        }
        private AnimationClock ClockShow, ClockHide;
    }
}
