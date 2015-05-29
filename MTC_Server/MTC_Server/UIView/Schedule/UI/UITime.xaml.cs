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

namespace MTC_Server.UIView.Schedule.UI
{
    public enum ModeOfTime
    {
        HOUR, DAY, NONE
    }
    /// <summary>
    /// Interaction logic for UITime.xaml
    /// </summary>
    public partial class UITime : UserControl
    {
        public List<UIEvent> UIEvents;
        private ModeOfTime mode = ModeOfTime.HOUR;
        public ModeOfTime Mode
        {
            get
            {
                return this.mode;
            }
            set
            {
                this.mode = value;
                if (this.mode == ModeOfTime.HOUR)
                {
                    this.UIContent.ClipToBounds = true;
                    this.UIContent.Visibility = System.Windows.Visibility.Visible;
                    this.UIContentScroll.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    this.UIContentScroll.Visibility = System.Windows.Visibility.Visible;
                    this.UIContent.Visibility = System.Windows.Visibility.Hidden;
                    this.UIContent.ClipToBounds = false;
                }
            }
        }
        public bool TopLineView
        {
            get
            {
                return this.LineTop.Visibility == System.Windows.Visibility.Visible;
            }
            set
            {
                if (value)
                    this.LineTop.Visibility = System.Windows.Visibility.Visible;
                else this.LineTop.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        public bool BottomLineView
        {
            get
            {
                return this.LineBottom.Visibility == System.Windows.Visibility.Visible;
            }
            set
            {
                if (value)
                    this.LineBottom.Visibility = System.Windows.Visibility.Visible;
                else this.LineBottom.Visibility = System.Windows.Visibility.Hidden;
            }
        }
        public string Text
        {
            get
            {
                return this.UIText.Text;
            }
            set
            {
                this.UIText.Text = value;
            }
        }

        private bool _hasEvent = false;
        public bool hasEvent
        {
            get
            {
                return this._hasEvent;
            }
            set
            {
                this._hasEvent = value;
                if (!this._hasEvent)
                {
                    this.Height = 60;
                    this.LineCenter.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    this.Height = 120;
                    this.LineCenter.Visibility = System.Windows.Visibility.Visible;
                }
                this.LineCenter.Width = this.Width - 76;
                this.LineCenter.setTop(this.Height / 2);
                this.UIText.setTop(this.Height / 2 - 9);
                this.UIContent.Width = this.Width - 90;
                this.UIContent.Height = this.Height -2;
            }
        }

        public UITime()
        {
            InitializeComponent();
            UIEvents = new List<UIEvent>();
        }

        private void RootView_Loaded(object sender, RoutedEventArgs e)
        {
            this.LineCenter.Width = this.Width - 76;
            this.LineCenter.setTop(this.Height / 2);
            UIText.setTop(this.Height / 2 - 9);
            UIContent.Width = this.Width;
            UIContent.Height = this.Height-2;
        }
        public void Clear()
        {
            this.hasEvent = false;
            this.UIContentScroll.Children.Clear();
            this.UIContent.Children.Clear();
            this.UIContentScroll.Height = 0;
        }
        public void AddChild(UIEvent E)
        {
            UIEvents.Add(E);
            this.hasEvent = true;
            if (this.Mode == ModeOfTime.HOUR)
            {
                this.UIContent.Children.Add(E);
            }
            else
            {
                double tmpTop = 0;
                foreach (UIElement uiE in this.UIContentScroll.Children)
                {
                    tmpTop = uiE.getTop() +(double) uiE.GetValue(HeightProperty);
                }
                E.setTop(tmpTop + 2);
                this.UIContentScroll.Children.Add(E);
                UIContentScroll.Height = tmpTop + 2 + E.Height;
            }
        }

        public void RemoveChild(UIEvent E)
        {
            foreach (UIElement uiE in this.UIContent.Children)
            {
                if (uiE is UIEvent)
                {
                    UIEvent e = uiE as UIEvent;
                    if (e == E)
                    {
                        this.UIContent.Children.Remove(uiE);
                        UIEvents.Remove(E);
                        return;
                    }
                }
            }
            foreach (UIElement uiE in this.UIContentScroll.Children)
            {
                if (uiE is UIEvent)
                {
                    UIEvent e = uiE as UIEvent;
                    if (e == E)
                    {
                        this.UIContentScroll.Children.Remove(uiE);
                        UIEvents.Remove(E);
                        return;
                    }
                }
            }
        }
    }
}
