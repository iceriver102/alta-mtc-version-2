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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MTC_Server.UIView.User
{
    /// <summary>
    /// Interaction logic for FingerPrinter.xaml
    /// </summary>
    public partial class FingerPrinter : UserControl, DPFP.Capture.EventHandler
    {
        private DPFP.Processing.Enrollment Enroller;
        private DPFP.Capture.Capture Capturer;
        public event EventHandler<DPFP.Template> OnTemplateEvent;
        private AnimationClock ClockShow, ClockHide;
        public FingerPrinter()
        {
            InitializeComponent();
            this.UIStatus.Foreground = new SolidColorBrush(Colors.White);
            Enroller = new DPFP.Processing.Enrollment();
            try
            {
                Capturer = new DPFP.Capture.Capture(); 			// Create a capture operation.
                if (null != Capturer)
                    Capturer.EventHandler = this;					// Subscribe for capturing events.

            }
            catch
            {
                MessageBox.Show("Can't initiate capture operation!", "Error");
            }
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

        public void OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
        {

            Parse(Sample);
        }

        public void OnFingerGone(object Capture, string ReaderSerialNumber)
        {

            this.Dispatcher.Invoke(() =>
            {
                if (this.ClockShow != null)
                    this.ClockShow.Controller.Pause();
                ColorAnimation animation = new ColorAnimation();
                animation.From = (this.UIStatus.Foreground as SolidColorBrush).Color;
                animation.To = Colors.White;
                animation.Duration = new Duration(TimeSpan.FromMilliseconds(450));
                animation.EasingFunction = new PowerEase() { Power = 5, EasingMode = EasingMode.EaseInOut };
                this.ClockHide = animation.CreateClock();
                this.UIStatus.Foreground.ApplyAnimationClock(SolidColorBrush.ColorProperty, ClockHide);
            });

        }

        public void OnFingerTouch(object Capture, string ReaderSerialNumber)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (this.ClockHide != null)
                    this.ClockHide.Controller.Pause();
                ColorAnimation animation = new ColorAnimation();
                animation.From = (this.UIStatus.Foreground as SolidColorBrush).Color;
                animation.To = Colors.Red;
                animation.Duration = new Duration(TimeSpan.FromMilliseconds(450));
                animation.EasingFunction = new PowerEase() { Power = 5, EasingMode = EasingMode.EaseInOut };
                this.ClockShow = animation.CreateClock();
                this.UIStatus.Foreground.ApplyAnimationClock(SolidColorBrush.ColorProperty, ClockShow);
            });
        }

        public void OnReaderConnect(object Capture, string ReaderSerialNumber)
        {
            this.Dispatcher.Invoke(() =>
            {
                ColorAnimation animation = new ColorAnimation();
                animation.From = (this.UIStatus.Foreground as SolidColorBrush).Color;
                animation.To = Colors.Blue;
                animation.Duration = new Duration(TimeSpan.FromMilliseconds(450));
                animation.EasingFunction = new PowerEase() { Power = 5, EasingMode = EasingMode.EaseInOut };
                this.ClockHide = animation.CreateClock();
                this.UIStatus.Foreground.ApplyAnimationClock(SolidColorBrush.ColorProperty, ClockHide);
            });
        }

        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (this.ClockHide != null)
                    this.ClockHide.Controller.Pause();
                ColorAnimation animation = new ColorAnimation();
                animation.From = (this.UIStatus.Foreground as SolidColorBrush).Color;
                animation.To = Colors.Black;
                animation.Duration = new Duration(TimeSpan.FromMilliseconds(450));
                animation.EasingFunction = new PowerEase() { Power = 5, EasingMode = EasingMode.EaseInOut };
                this.ClockShow = animation.CreateClock();
                this.UIStatus.Foreground.ApplyAnimationClock(SolidColorBrush.ColorProperty, ClockShow);
            });
        }

        public void OnSampleQuality(object Capture, string ReaderSerialNumber, DPFP.Capture.CaptureFeedback CaptureFeedback)
        {

        }

        private void Parse(DPFP.Sample Sample)
        {
            //MessageBox.Show("OK");

            DPFP.FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Enrollment);

            // Check quality of the sample and add to enroller if it's good
            if (features != null) try
                {
                    try
                    {
                        Enroller.AddFeatures(features);		// Add feature set to template.
                    }
                    catch
                    {

                    }
                }
                finally
                {

                    UpdateStatus();
                    // Check if template has been created.
                    switch (Enroller.TemplateStatus)
                    {
                        case DPFP.Processing.Enrollment.Status.Ready:	// report success and stop capturing
                            Stop();
                            this.Dispatcher.Invoke(() =>
                            {
                                if (this.ClockHide != null)
                                    this.ClockHide.Controller.Pause();
                                ColorAnimation animation = new ColorAnimation();
                                animation.From = (this.UIStatus.Foreground as SolidColorBrush).Color;
                                animation.To = Colors.Red;
                                animation.Duration = new Duration(TimeSpan.FromMilliseconds(450));
                                animation.EasingFunction = new PowerEase() { Power = 5, EasingMode = EasingMode.EaseInOut };
                                this.ClockShow = animation.CreateClock();
                                this.UIStatus.Foreground.ApplyAnimationClock(SolidColorBrush.ColorProperty, ClockShow);
                            });
                            if (OnTemplateEvent != null)
                            {
                                OnTemplateEvent(this, Enroller.Template);
                            }
                            break;

                        case DPFP.Processing.Enrollment.Status.Failed:	// report failure and restart capturing
                            Enroller.Clear();
                            Stop();
                            this.Dispatcher.Invoke(() =>
                            {
                                this.UITime.Text = "Try again ...";
                                this.UITime.FontSize = 15.333;
                                this.UITime.FontWeight = FontWeights.Normal;
                                this.UITime.Foreground = new SolidColorBrush(Colors.White);
                            });
                            Start();
                            break;

                    }
                }
        }

        private void UpdateStatus()
        {
            this.Dispatcher.Invoke(() =>
            {
                this.UITime.Text = string.Format("{0}", Enroller.FeaturesNeeded);
                this.UITime.FontSize = 24.333;
                this.UITime.FontWeight = FontWeights.SemiBold;
                this.UITime.Foreground = new SolidColorBrush(Colors.OrangeRed);
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Start();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Enroller.Clear();
            this.Stop();
        }
    }
}
