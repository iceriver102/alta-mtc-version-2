using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Alta.Class;
using MySql.Data.MySqlClient;
using MTC_Server.Code;
using MTC_Server.Code.User;
using MTC_Server.Code.Media;
using Vlc.DotNet.Core;
using MTC_Server.Code.Device;

namespace MTC_Server
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Config setting;
        public static string FileName = "setting.xml";
        public static int curUserID = 0;
        public static UserData curUser;
        public static AltaCache cache;
        public static string CacheName = "cache.xml";
        public static List<UserTypeData> TypeUsers;
        public static MediaTypeArray TypeMedias;
        public static DeviceTypeArray TypeDevices;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Define.Fonts = ExCss.ReadFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,@"Asset\Fonts\font-awesome.min.css"));
            setting = Config.Read(FileName);
            if (setting.temp_folder.IndexOf(@"://") < 0 || setting.temp_folder.IndexOf(@"\\")<0)
            {
                setting.temp_folder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, setting.temp_folder);
            }
            if (!System.IO.Directory.Exists(setting.temp_folder))
            {
                System.IO.Directory.CreateDirectory(setting.temp_folder);
            }
            cache = AltaCache.Read(CacheName);
            TypeUsers = MysqlHelper.getTypeUserAll();
            TypeMedias = new MediaTypeArray(TypeMedia.getList());
            TypeDevices = new DeviceTypeArray(TypeDevice.getList());
            initVLC();
            if (cache.autoLogin && !string.IsNullOrEmpty(cache.hashUserName))
            {
                int tmpResult = Login(cache.hashUserName);
                if (tmpResult != 0)
                {
                    curUserID = tmpResult;
                    this.MainWindow = new MainWindow();
                    this.MainWindow.Show();
                }
                else
                {
                    this.MainWindow = new Login();
                    this.MainWindow.Show();
                }
            }
            else
            {
                this.MainWindow = new Login();
                this.MainWindow.Show();
            }
        }
        public static string getHash(int id)
        {
            //SELECT  `fc_get_hash_user` ( @user_id) AS  `hash` ;
            string result = string.Empty;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "SELECT  `fc_get_hash_user` ( @user_id) AS " + Define.hash;

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user_id", id);
                        var tmp = cmd.ExecuteScalar();
                        result = (string)tmp;
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

        public static UserTypeData selectType(int id)
        {
            if (TypeUsers != null)
            {
                foreach (UserTypeData data in TypeUsers)
                {
                    if (data.Id == id)
                        return data;
                }
            }
            return null;
        }

        public int Login(string hash)
        {

            int result = 0;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(App.setting.connectString))
                {
                    conn.Open();
                    string query = "SELECT  `fc_login_hash` (@hash) AS  `user_id`";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@hash", hash);
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
        public void initVLC()
        {
            // Set libvlc.dll and libvlccore.dll directory path
            VlcContext.LibVlcDllsPath = @"VLC";

            // Set the vlc plugins directory path
            VlcContext.LibVlcPluginsPath = @"VLC\plugins";


            // Ignore the VLC configuration file
            VlcContext.StartupOptions.IgnoreConfig = true;

            // Enable file based logging
            VlcContext.StartupOptions.LogOptions.LogInFile = true;

            // Shows the VLC log console (in addition to the applications window)
            // VlcContext.StartupOptions.LogOptions.ShowLoggerConsole = true;

            // Set the log level for the VLC instance
            //   VlcContext.StartupOptions.LogOptions.Verbosity = VlcLogVerbosities.Debug;
            VlcContext.StartupOptions.AddOption("--ffmpeg-hw");
            // Disable showing the movie file name as an overlay
            VlcContext.StartupOptions.AddOption("--no-video-title-show");
            VlcContext.StartupOptions.AddOption("--rtsp-tcp");
            VlcContext.StartupOptions.AddOption("--rtsp-mcast");
            // VlcContext.StartupOptions.AddOption("--rtsp-host=192.168.10.35");
            // VlcContext.StartupOptions.AddOption("--sap-addr=192.168.10.35");
            VlcContext.StartupOptions.AddOption("--rtsp-port=8554");
            VlcContext.StartupOptions.AddOption("--rtp-client-port=8554");
            VlcContext.StartupOptions.AddOption("--sout-rtp-rtcp-mux");
            VlcContext.StartupOptions.AddOption("--rtsp-wmserver");


            VlcContext.StartupOptions.AddOption("--file-caching=18000");
            VlcContext.StartupOptions.AddOption("--sout-rtp-caching=18000");
            VlcContext.StartupOptions.AddOption("--sout-rtp-port=8554");
            VlcContext.StartupOptions.AddOption("--sout-rtp-proto=tcp");
            VlcContext.StartupOptions.AddOption("--network-caching=1000");

            // Pauses the playback of a movie on the last frame
            VlcContext.StartupOptions.AddOption("--play-and-pause");

            // Initialize the VlcContext
            VlcContext.Initialize();
        }
    }
}
