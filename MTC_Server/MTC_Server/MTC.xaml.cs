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
using System.Windows.Shapes;
using Alta.Class;
namespace MTC_Server
{
    /// <summary>
    /// Interaction logic for MTC.xaml
    /// </summary>
    public partial class MTC : Window
    {
        public MTC()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.UIName.isEmpty())
            {
                MessageBox.Show("Tên người sử dụng không được phép để trống","Thông báo");
                return;
            }
            if (this.UIKey.isEmpty())
            {
                MessageBox.Show("Key Phần mềm không được phép để trống", "Thông báo");
                return;
            }

            string name = this.UIName.Text;
            string key = this.UIKey.Text;
            Config setting = Config.Read(App.FileName,key);
            if (setting != null)
            {
                if (setting.EndDate != App.Zero)
                {
                    if (DateTime.Now >= setting.EndDate)
                    {
                        MessageBox.Show("Phần mềm đã hết hạn sử dụng");
                        return;
                    }
                }
                MessageBox.Show("Đăng kí thành công!");
                App.Registry.Write("MTC_KEY", key);
                App.Registry.Write("MTC_USER", name);
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
            else
            {
                MessageBox.Show("Đăng kí không thành công key sử dụng phần mềm không đúng.", "Thông báo");
            }
        }
    }
}
