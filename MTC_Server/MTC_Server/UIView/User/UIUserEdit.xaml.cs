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
using System.Windows.Media.Animation;

namespace MTC_Server.UIView.User
{
    /// <summary>
    /// Interaction logic for UIUserEdit.xaml
    /// </summary>
    public partial class UIUserEdit : UserControl, DPFP.Capture.EventHandler
    {
        public event EventHandler<Code.User.UserData> CloseEvent;
        public event EventHandler<Code.User.UserData> DeleteUserEvent;
        private DPFP.Capture.Capture Capturer;
        private DPFP.Verification.Verification Verificator;
        public new DPFP.Template Template;
        private DPFP.Template curTemplate;
        private bool _isPermison = false;
        public bool isPermison
        {
            get
            {
                return this._isPermison;
            }
            set
            {
                this._isPermison = value;
                if(value)
                    this.UILayout.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, -100);
                else
                    this.UILayout.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 0);
            }
        }
        private Code.User.UserData u;
        public Code.User.UserData User
        {
            get
            {
                return this.u;

            }
            set
            {
                this.u = value;
                if (this.u != null)
                {
                    this.UIName.Text = string.Format("{0} ({1})", this.u.Full_Name, this.u.User_Name);
                    this.UIFullnameEdit.Text = this.u.Full_Name;
                    this.UIEmailEdit.Text = this.u.Email;
                    this.UIPhoneEdit.Text = this.u.Phone;
                    this.viewPermision.Permision = this.u.Permision;
                    this.UIUserNameEdit.Visibility = Visibility.Hidden;
                    this.UITypeUser.SelectedItem = this.u.TypeUser;
                    if (this.u.Finger_Print != null)
                    {
                        curTemplate = new DPFP.Template();
                        curTemplate.DeSerialize(this.u.Finger_Print);
                    }
                }
            }
        }
        public UIUserEdit()
        {
            InitializeComponent();
            UIFinger_Printer.Foreground = new SolidColorBrush(Colors.Black);
            foreach(Code.User.UserTypeData type in App.TypeUsers)
            {
                UITypeUser.Items.Add(type);
            }
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

        private void CloseDialog(object sender, MouseButtonEventArgs e)
        {
            if (this.CloseEvent != null)
            {
                this.CloseEvent(this, null);
            }
        }

        private void RefillEvent(object sender, MouseButtonEventArgs e)
        {
            this.UIFullnameEdit.Text = this.u.Full_Name;
            this.UIEmailEdit.Text = this.u.Email;
            this.UIPhoneEdit.Text = this.u.Phone;
            this.UIOldPaswordEdit.Password = "";
            this.UIPaswordConfirmEdit.Password = "";
            this.UINewPaswordEdit.Password = "";
            this.UITypeUser.SelectedItem = this.u.TypeUser;
        }
        bool flagFinger = false;
        private void SaveUser(object sender, MouseButtonEventArgs e)
        {
            string newPass = this.UINewPaswordEdit.Password;
            string confirmPass = this.UIPaswordConfirmEdit.Password;
            if (this.u != null)
            {
                string oldPass = this.UIOldPaswordEdit.Password.MD5String();
                if (this.u.Pass != oldPass && flagFinger== false)
                {
                    MessageBox.Show("Mật khẩu không đúng vui lòng kiểm tra lại!");
                    return;
                }
                
                if (!string.IsNullOrEmpty(newPass) && newPass != confirmPass)
                {
                    MessageBox.Show("Mật khẩu không khớp nhau vui lòng kiểm tra lại!");
                    return;
                }
                
                if (this.UITypeUser.SelectedIndex ==-1)
                {
                    MessageBox.Show("Vui lòng chọn loại tài khoản!");
                    return;
                }
                if (string.IsNullOrEmpty(newPass))
                {
                    newPass = this.u.Pass;
                }
                else
                {
                    newPass = newPass.MD5String();
                }
                this.u.setDirectpass(newPass);
                this.u.Full_Name = this.UIFullnameEdit.Text;
                this.u.Email = this.UIEmailEdit.Text;
                this.u.Phone = this.UIPhoneEdit.Text;
                this.u.Type =(this.UITypeUser.SelectedItem as Code.User.UserTypeData).Id;
                this.u.Permision = this.viewPermision.Permision;
                if(this.Template!=null)
                    this.Template.Serialize(ref u.Finger_Print);
                if (this.u.Save() != -1)
                {
                    if (this.CloseEvent != null)
                        this.CloseEvent(this, this.u);
                }
                else
                {
                    MessageBox.Show("Tên đăng nhập đã tồn tại. Vui lòng kiểm tra lại!", "Thông Báo");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(UIUserNameEdit.Text))
                {
                    MessageBox.Show("Tên đăng nhập không được để trống!");
                    return;
                }
                if (!UIUserNameEdit.Text.isUserName())
                {
                    MessageBox.Show("Tên đăng nhập không đúng định dạng!");
                    return;
                }
                if (!string.IsNullOrEmpty(newPass) && newPass != confirmPass)
                {
                    MessageBox.Show("Mật khẩu không khớp nhau vui lòng kiểm tra lại!");
                    return;
                }
                Code.User.UserData u = new Code.User.UserData();
                u.Full_Name = this.UIFullnameEdit.Text;
                u.Email = this.UIEmailEdit.Text;
                u.Phone = this.UIPhoneEdit.Text;
                u.Type = (this.UITypeUser.SelectedItem as Code.User.UserTypeData).Id;
                u.Pass = newPass;
                u.User_Name = UIUserNameEdit.Text;
                if (this.Template == null)
                {
                    u.Finger_Print = null;
                }
                else
                {
                    this.Template.Serialize(ref u.Finger_Print);
                }
                int result=Code.User.UserData.insertUser(u);
                if (result <= 0)
                {
                    MessageBox.Show("Tên đăng nhập đã tồn tại. Vui lòng kiểm tra lại!","Thông Báo");
                    return;
                }
                u.ID = result;
                if (this.CloseEvent != null)
                    this.CloseEvent(this, u);
            }
            
        }

        private void SubmitForm(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.SaveUser(null, null);
            }
        }

        private void ViewPermision(object sender, MouseButtonEventArgs e)
        {
            this.isPermison = !this.isPermison;
        }

        private void DeleteUser(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Bạn có muốn xoá tài khoản này không?", "Xoá tài khoản", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Code.User.UserData.deleteUser(this.User);
                if (this.DeleteUserEvent != null)
                {
                    this.DeleteUserEvent(this, this.u);
                }
            }
        }

        private void UIRootView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            if (this.u == null)
            {
                this.UIBtnDelete.Visibility = Visibility.Collapsed;
                this.UIBtnReset.Visibility = Visibility.Collapsed;
                this.UIBtnPermision.Visibility = Visibility.Collapsed;
                this.UIOldPaswordEdit.IsEnabled = false;
                this.UIOldPaswordEdit.Visibility = Visibility.Hidden;
                this.UILBChange.Text = "Tên đăng nhập:";
                this.UIName.Text = "Thêm người dùng mới.";
            }
            if (App.curUser.Permision.mana_user)
            {
                UIBtnReset.IsEnabled = true;
                UIBtnDelete.IsEnabled = true;
                UIBtnPermision.IsEnabled = true;
            }
            else
            {
                UIBtnReset.IsEnabled = false;
                UIBtnDelete.IsEnabled = false;
                UIBtnPermision.IsEnabled = false;
            }
        }

        private void UIRootView_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key== Key.Escape)
            {
                this.CloseDialog(null, null);
            }
        }

        private void ResetPassword(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Bạn có muốn tạo mật khẩu tự động cho tài khoản này không?", "Tạo mật khẩu", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                string Pass = StringExtensions.RandomString(6);
                this.u.Pass = Pass;
                this.u.Save();
                Clipboard.SetText(string.Format("{0}:{1}", this.u.User_Name, Pass));
                MessageBox.Show(string.Format("Mật khẩu: {0}",Pass),"Tạo mật khẩu tự động");
            }
        }

        private void FingerPrint(object sender, MouseButtonEventArgs e)
        {
            this.Dim.Visibility = System.Windows.Visibility.Visible;
            FingerPrinter UIFingerPrinter = new FingerPrinter();
            UIFingerPrinter.setLeft(640);
            UIFingerPrinter.setTop(360);
            UIFingerPrinter.OnTemplateEvent += UIFingerPrinter_OnTemplateEvent;
            this.Dim.Children.Add(UIFingerPrinter);
            this.Dim.Animation_Opacity_View_Frame(true);
        }

        void UIFingerPrinter_OnTemplateEvent(object sender, DPFP.Template e)
        {
            if (e != null)
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.Dim.Animation_Opacity_View_Frame(false, () => { this.Dim.Visibility = System.Windows.Visibility.Hidden; });
                });
                this.Template = e;
            }
        }

        private void UIOldFingerPrinter_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Start();
        }


        public void OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
        {
            this.Verify(Sample);
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
                try
                {

                    Verificator.Verify(features, this.curTemplate , ref result);
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
                            this.ClockHide = animation.CreateClock();
                            this.UIFinger_Printer.Foreground.ApplyAnimationClock(SolidColorBrush.ColorProperty, ClockHide);
                            flagFinger = true;
                        });
                        this.Stop();
                    }
                    else
                    {

                    }
                }
                catch (Exception)
                {
                  
                }
            }
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

        public void OnSampleQuality(object Capture, string ReaderSerialNumber, DPFP.Capture.CaptureFeedback CaptureFeedback)
        {

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
        private AnimationClock ClockShow, ClockHide;

        private void UIRootView_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Stop();
        }
    }
}
