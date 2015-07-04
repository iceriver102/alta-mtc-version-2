using Camera_Final.Code;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Alta.Class;
//using Vlc.DotNet.Core;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Threading;
namespace Camera_Final
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private bool m_bInitSDK = false;
        public static Users DataUser;
        public static User User;
        public static Cameras DataCamera;
        public static Presets DataPreset;
        public static Alarms DataAlarm;
        public static CommandDefine DefineCommand;
        public static Map @Map;
        public static Config setting;
        public static FontIcons Fonts;
        public Thread Timethread;
        private DateTime curDate;
        public static Queue<string> QueueCMD;

        public const int BAUDRATE = 9600;
        public const int DATABITS = 8;

        public const string _FILE_User_Data = @"Data\User.mtc";
        public const string _FILE_Camera_Data = @"Data\Camera.xml";
        public const string _FILE_Map_Data = @"Data\map.xml";
        public const string _FILE_Config = "setting.xml";
        public const string _FILE_Command_Log = @"Data\log.xml";
        public const string _FILE_Alarm_Data = @"Data\alarm.xml";
        public const string _FILE_PRESET_DATA = @"Data\preset.xml";
        public const string _FILE_DEFINE_COMMAND = @"Data\command.xml";
        public static string curFolder;
        private Thread checkFile;

        /*
        public static void initVLC()
        {
            if (Environment.Is64BitOperatingSystem)
            {
                // Set libvlc.dll and libvlccore.dll directory path
                VlcContext.LibVlcDllsPath = @"VLC\";

                // Set the vlc plugins directory path
                VlcContext.LibVlcPluginsPath = @"VLC\plugins";
            }
            else
            {
                // Set libvlc.dll and libvlccore.dll directory path
                VlcContext.LibVlcDllsPath = @"VLC\";

                // Set the vlc plugins directory path
                VlcContext.LibVlcPluginsPath = @"VLC\plugins";
            }

            // Ignore the VLC configuration file
            VlcContext.StartupOptions.IgnoreConfig = true;
#if DEBUG
            // Enable file based logging
            VlcContext.StartupOptions.LogOptions.LogInFile = true;

            // Shows the VLC log console (in addition to the applications window)
            VlcContext.StartupOptions.LogOptions.ShowLoggerConsole = true;
#else
            VlcContext.StartupOptions.LogOptions.ShowLoggerConsole = false;
            VlcContext.StartupOptions.LogOptions.LogInFile = false;
#endif
            // Set the log level for the VLC instance
            VlcContext.StartupOptions.LogOptions.Verbosity = VlcLogVerbosities.Debug;
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

            VlcContext.StartupOptions.AddOption("--vout-filter=wall");
            VlcContext.StartupOptions.AddOption("--wall-cols=2");
            VlcContext.StartupOptions.AddOption("--wall-rows=2");

            // Pauses the playback of a movie on the last frame
            VlcContext.StartupOptions.AddOption("--play-and-pause");
            VlcContext.CloseAll();
            // Initialize the VlcContext
            VlcContext.Initialize();
        }
        */

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            bool login = true;
         
            Fonts = ExCss.ReadFile(@"Asset\Fonts\font-awesome.min.css");
            Timethread = new Thread(CheckTimeFunctionThread);
            Timethread.IsBackground = true;
            listSerialPort = new List<SerialPort>();
            QueueCMD = new Queue<string>();
            m_bInitSDK = CHCNetSDK.NET_DVR_Init();
            if (m_bInitSDK == false)
            {
                MessageBox.Show("NET_DVR_Init error!");
                return;
            }
          //  initVLC();
            DefineCommand = CommandDefine.Read(_FILE_DEFINE_COMMAND);
           
            setting = Config.Read(_FILE_Config);
            if (!Directory.Exists("Data"))
            {
                DirectoryInfo di = Directory.CreateDirectory("Data");
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            if (!Directory.Exists(setting.Folder))
            {
                Directory.CreateDirectory(setting.Folder);
            }
            curDate = DateTime.Now;
            Timethread.Start();
            curFolder=System.IO.Path.Combine(setting.Folder,this.curDate.ToString("dd-MM-yyyy"));
            if (!Directory.Exists(curFolder))
            {
                Directory.CreateDirectory(curFolder);
            }
            DataUser = Users.read(@"Data\User.mtc");
            if (DataUser == null)
            {
                DataUser = new Users();
                User root = new User("root", 1);
                root.Pass = "root".toMD5();
                root.type = 0;
                root.FullName = "Root";
                DataUser.Add(root);
                Users.write(_FILE_User_Data, DataUser);
            }
            DataCamera = Cameras.Read(_FILE_Camera_Data);
            DataPreset = Presets.Read(_FILE_PRESET_DATA);
            DataAlarm = Alarms.Read(_FILE_Alarm_Data);
            this.checkFile = new Thread(checkFileFunctionThread);
            checkFile.IsBackground = true;
            this.checkFile.Start();
#if DEBUG
            if (DataCamera.Count == 0)
            {
                Camera camera = new Camera("192.168.10.199");
                camera.name = "Camera Demo";
                camera.channel = 1;
                camera.port = 8000;
                camera.admin = "admin";
                camera.pass = "admin";
                camera.icon = "fa-video-camera";
                DataCamera.Add(camera);
            }
            
            if (DataUser.Datas.Count < 2)
            {
                User root = new User("admin", 2);
                root.Pass = "dami".toMD5();
                root.type = 1;
                root.FullName = "Admin";
                DataUser.Add(root);

                User root2 = new User("htdm",3);
                root2.Pass = "123".toMD5();
                root2.type = 2;
                root2.FullName = "Camera";
                DataUser.Add(root2);
                Users.write(_FILE_User_Data, DataUser);
            }
            
#endif
            var listCom = getListCOM();
            if (listCom.Length > 0)
            {
                foreach (string i in listCom)
                {
                    try
                    {
                        SerialPort serialPort = new SerialPort();
                        serialPort = new SerialPort();
                        serialPort.openCOM(i, BAUDRATE, DATABITS, StopBits.One);
                        serialPort.DataReceived += serialPort_DataReceived;
                        serialPort.sendCommand("#0$");
                        listSerialPort.Add(serialPort);
                    }
                    catch (Exception)
                    {
                      
                    }
                }

            }
            Map = Map.Read(_FILE_Map_Data);
            for (int i = 0; i != e.Args.Length; i += 2)
            {
                if (e.Args[i] == "-u")
                {
                    string hash = e.Args[i + 1];
                    User u = App.DataUser.Login(hash);
                    if (u != null)
                    {
                        login = false;
                        App.User = u;
                    }
                }
                else if (e.Args[i] == "-mode")
                {
                    Mode = (Camera_Final.Mode)int.Parse(e.Args[i + 1]);
                }
            }
            if (login)
            {
                this.MainWindow = new Login();
            }
            else
            {
                this.MainWindow = new MainWindow();
            }
            this.MainWindow.Show();
        }

        private void checkFileFunctionThread(object obj)
        {
            while (true)
            {
                if (setting.DateFile > 0)
                {
                    List<string> fileRecord = Directory.GetFiles(setting.Folder, "*.mp4", SearchOption.AllDirectories).ToList();
                    if (fileRecord != null && fileRecord.Count > 0)
                    {
                        foreach (string file in fileRecord)
                        {
                            FileInfo inf = new FileInfo(file);
                            if (inf.Exists && inf.CreationTime.Date < DateTime.Now.Date)
                            {
                                double day = (DateTime.Now.Date - inf.CreationTime.Date).TotalDays;
                                if (day > setting.DateFile && !inf.IsFileLocked())
                                {
                                    inf.Delete();
                                }
                            }
                        }
                    }
                }
               Thread.Sleep(24 * 3600 * 1000);
            }
        }

        private void CheckTimeFunctionThread()
        {
            while (true)
            {
                Thread.Sleep(5000);
                if (curDate.Date != DateTime.Now)
                {
                    curDate = DateTime.Now;
                    curFolder = System.IO.Path.Combine(setting.Folder, this.curDate.ToString("dd-MM-yyyy"));
                    if (!Directory.Exists(curFolder))
                    {
                        Directory.CreateDirectory(curFolder);
                    }
                }                
            }
        }
        private string cmd;
        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            bool flag = true;
            char[] charsToTrim = { '\n' };
            SerialPort s = sender as SerialPort;
            string tmp = s.ReadExisting().Trim(charsToTrim);
            if (tmp.IndexOf("#") >= 0)
            {
                this.cmd = string.Empty;
                this.cmd += tmp;
                flag = false;
            }
            if (tmp.IndexOf("$") >= 0)
            {
                this.cmd += tmp;
                int to = this.cmd.IndexOf("#");
                if (to >= 0)
                {
                    int from = this.cmd.IndexOf("$", to + 1);
                    this.cmd = this.cmd.Substring(to + 1, from - to - 1);
                    string[] element = this.cmd.Split(' ');
                    if (element.Length == 4)
                    {
                        if ((element[1] == "A" || element[1] == "B") && (element[3] == "1") && element.Length < 5)
                        {
                            QueueCMD.Enqueue(this.cmd);
                            //alert = true;  
                        }
                    }
                   
                    Regex myRegex = new Regex(@"\d$");
                    if (myRegex.IsMatch(this.cmd) && !string.IsNullOrEmpty(this.cmd))
                    {
                        s.sendCommand(element[0] + " OK$");
                    }
                   // else if (!string.IsNullOrEmpty(this.cmd) && this.command != null)
                   // {
                        //if (s.PortName.ToLower() == command.name.ToLower() && command.mode == CommandMode.WAIT)
                        //{
                        //    command.mode = CommandMode.RUN;
                        //}
                   // }
                    try
                    {
                        int id_board = Convert.ToInt32(element[0]);
                        
                        if (App.DataAlarm != null)
                        {
                            for (int i = 0; i < App.DataAlarm.Count; i++)
                            {
                                if (App.DataAlarm[i].board == id_board)
                                {
                                    App.DataAlarm[i].Com = s.PortName;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.GetBaseException().ToString() + "id: "+this.cmd);
                    }

                    //mysql_helpper.setCommand(this.cmd, s.PortName.ToString());
                }
                flag = false;
                this.cmd = string.Empty;

            }
            if (flag)
                this.cmd += tmp;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (Timethread != null)
            {
                Timethread.Abort();
            }

            if (checkFile != null)
            {
                this.checkFile.Abort();
            }

            Alarms.Write(_FILE_Alarm_Data, DataAlarm);
            Users.write(_FILE_User_Data, DataUser);
            Cameras.Write(_FILE_Camera_Data, DataCamera);
            Map.Write(_FILE_Map_Data, Map);
            Presets.Write(_FILE_PRESET_DATA, DataPreset);
           // Config.Write(_FILE_Config, setting);
            if (m_bInitSDK == true)
            {
                CHCNetSDK.NET_DVR_Cleanup();
            }
            //VlcContext.CloseAll();
        }

        public static List<SerialPort> listSerialPort;

        public static string[] getListCOM()
        {
            try
            {
                List<string> result = new List<string>();
                foreach (COMPortInfo comPort in COMPortInfo.GetCOMPortsInfo())
                {                    
                    if (comPort.Description.IndexOf("Prolific USB-to-Serial Comm Port") >= 0)
                    {
                        result.Add(comPort.Name);
                    }
                }             
                return result.ToArray();
                
            }
            catch (Exception)
            {
                return new string[0];
            }
        }

        public void PlayAllCamera()
        {
            foreach (Camera cam in DataCamera.Datas)
            {
                if (cam != null)
                {
                    cam.view = true;
                }
            }
        }
        public static Mode Mode = Camera_Final.Mode.NONE;
    }
}
