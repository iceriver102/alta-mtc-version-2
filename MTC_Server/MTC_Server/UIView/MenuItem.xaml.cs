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

namespace MTC_Server.UIView
{
    /// <summary>
    /// Interaction logic for MenuItem.xaml
    /// </summary>
    public partial class MenuItem : UserControl
    {
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
        }
    }
}
