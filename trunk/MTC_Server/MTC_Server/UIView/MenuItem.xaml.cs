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

namespace MTC_Server.UIView
{
    /// <summary>
    /// Interaction logic for MenuItem.xaml
    /// </summary>
    public partial class MenuItem : UserControl
    {
        private Brush orginalColor;
        public event EventHandler ClickEvent;
        public string Code { get; set; }
        public string Icon
        {
            get
            {
                return this.UIIcon.Text;
            }
            set
            {
                this.UIIcon.Text = value;
            }
        }
        private bool _is_active = false;
        public bool isActive
        {
            get
            {
                return this._is_active;
            }
            set
            {
                this._is_active = value;
                if (this.isActive)
                {
                    this.UIIcon.Foreground = new SolidColorBrush( new Color() { A = 255, B = 38, G = 165, R = 224 });
                    this.Cursor = Cursors.Arrow;
                    this.UIText.Foreground = Brushes.White;
                    this.UIText.FontWeight = FontWeights.Normal;
                }
                else
                {
                    this.UIIcon.Foreground = this.orginalColor;
                    this.UIText.Foreground = Brushes.White;
                    this.Cursor = Cursors.Hand;
                }
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
        public MenuItem()
        {
            InitializeComponent();
            this.orginalColor = this.UIIcon.Foreground;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!this.isActive)
            {
                this.UIText.Foreground = this.orginalColor;
                this.UIIcon.Foreground = Brushes.White;
                this.UIText.FontWeight = FontWeights.Bold;
            }
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!this.isActive)
            {
                this.UIText.Foreground = Brushes.White;
                this.UIIcon.Foreground = this.orginalColor;
                this.UIText.FontWeight = FontWeights.Normal;
            }
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.isActive)
            {
                this.Animation_Translate_Frame(double.NaN, 6, 500, true);
                this.isActive = true;
                if (this.ClickEvent != null)
                    this.ClickEvent(this, new EventArgs());
            }
        }
    }
}
