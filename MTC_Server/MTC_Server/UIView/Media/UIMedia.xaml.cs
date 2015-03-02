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
using MTC_Server.Code.Media;
namespace MTC_Server.UIView.Media
{
    /// <summary>
    /// Interaction logic for UIMedia.xaml
    /// </summary>
    public partial class UIMedia : UserControl
    {
        private MediaData m;
        public MediaData Media
        {
            get
            {
                return this.m;
            }
            set
            {
                this.m = value;
                if (this.m.TypeMedia != null && !string.IsNullOrEmpty(this.m.TypeMedia.Icon))
                    this.UIIcon.Text = this.m.TypeMedia.Icon.DecodeEncodedNonAsciiCharacters();
                this.UITitle.Text = this.m.Name;
                this.UITime.Text = this.m.Duration.ToString();
                this.UIFileSize.Text = this.m.FileSize;
                this.UIUser.Text = this.m.User.Full_Name;
            }
        }
        public UIMedia()
        {
            InitializeComponent();
        }
    }
}
