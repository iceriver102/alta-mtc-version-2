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
namespace Key_Management
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public string CreateKey(int number=10)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, number)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Config setting = new Config();
            if (!string.IsNullOrEmpty(this.UIEndDate.Text))
            {
                IFormatProvider culture = new System.Globalization.CultureInfo("vi", true);
                setting.EndDate = DateTime.Parse(this.UIEndDate.Text, culture, System.Globalization.DateTimeStyles.AssumeLocal);
                
            }

            setting.db_password = this.UIPass.Text;
            setting.db_server = this.UIIP.Text;
            setting.db_user = this.UIUser.Text;
            setting.ftp_server = this.UIFtp.Text;
            setting.ftp_password = this.UIFtpPass.Text;
            setting.ftp_folder = this.UIFtpFolder.Text;
            setting.ftp_user = this.UIFtpUser.Text;
            setting.ftp_port = Convert.ToInt32(this.UIFtpPort.Text);
            string key = CreateKey(30);
            this.UIKey.Text = key;
            Config.Write("setting.mtc", setting,key);
            MessageBox.Show("Tạo key thành công: " + key, "Thông báo");
        }
    }
}
