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
using System.IO;

namespace MTC_Server
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Config setting;
        public static string FileName = "setting.mtc";
        public static int curUserID = 0;
        public static UserData curUser;
        public static AltaCache cache;
        public static string CacheName = "cache.xml";
        public static List<UserTypeData> TypeUsers;
        public static MediaTypeArray TypeMedias;
        public static DeviceTypeArray TypeDevices;
        public static string key="";
        public static ModifyRegistry Registry;
        public static DateTime Zero;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Define.Fonts = ExCss.ReadFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Asset\Fonts\font-awesome.min.css"));
            if (!File.Exists(FileName))
            {
                MessageBox.Show("Không tìm thấy file cấu hình!");
                Application.Current.Shutdown();
                return;
            }
            Registry = new ModifyRegistry();
            key = Registry.Read("MTC_KEY");
            if (string.IsNullOrEmpty(key))
            {
                this.MainWindow = new MTC();
                this.MainWindow.Show();
                return;
            }           
            setting = Config.Read(FileName,key);
            if (setting == null || setting.EndDate.Date< DateTime.Now.Date)
            {
                MessageBox.Show("Phần mềm đã hết hạn sử dụng.");
                this.MainWindow = new MTC();
                this.MainWindow.Show();
                return;

            }
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
                int tmpResult = UserData.getUserIdByHash(cache.hashUserName);
                UILogin form = new UILogin();
                if (tmpResult != -1)
                {
                    byte[] tmp = UserData.getFingerPrinter(tmpResult);
                    form.cacheName = UserData.getUserName(tmpResult);
                    if (tmp != null)
                    {
                        form.Template = new DPFP.Template();
                        form.Template.DeSerialize(tmp);
                    }
                    form.Show();
                }
                else
                {
                    form.Show();
                }
                return;
               
            }
            this.MainWindow = new UILogin();
            this.MainWindow.Show();
            Console.WriteLine("Debug");
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
                    string query = "SELECT  `fc_get_hash_user` (@user_id) AS " + Define.hash;

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
        
        public void initVLC()
        {
            if (Environment.Is64BitOperatingSystem)
            {
                VlcContext.LibVlcDllsPath = @"VLC\x86";
                // Set the vlc plugins directory path
                VlcContext.LibVlcPluginsPath = @"VLC\x86\plugins";
            }
            else
            {
                // Set libvlc.dll and libvlccore.dll directory path
                VlcContext.LibVlcDllsPath = @"VLC\x86";
                // Set the vlc plugins directory path
                VlcContext.LibVlcPluginsPath = @"VLC\x86\plugins";
            }

            // Ignore the VLC configuration file
            VlcContext.StartupOptions.IgnoreConfig = true;

            // Enable file based logging
            VlcContext.StartupOptions.LogOptions.LogInFile = true;

            // Shows the VLC log console (in addition to the applications window)
             VlcContext.StartupOptions.LogOptions.ShowLoggerConsole = false;

            // Set the log level for the VLC instance
            //VlcContext.StartupOptions.LogOptions.Verbosity = VlcLogVerbosities.Warnings;
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
            VlcContext.CloseAll();
            // Initialize the VlcContext
            VlcContext.Initialize();
        }
    }
}
