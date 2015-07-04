using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alta.Class;
using System.Xml.Serialization;
namespace Camera_Final.Code
{

    public class TimeOff
    {
        [XmlAttribute("from")]
        public DateTime beginTime;
        [XmlAttribute("to")]
        public DateTime EndTime;
    }

    [Serializable]
    public class Alarm
    {
        [XmlAttribute("name")]
        public string Name;
        [XmlAttribute("com")]
        public string Com;
        [XmlAttribute("left")]
        public double Left;
        [XmlAttribute("top")]
        public double Top;
        [XmlIgnore]
        public string icon
        {
            get
            {
                if(!this.status)
                {
                    return "fa-bell-slash";
                }
                else
                {
                    return "fa-bell";
                }
            }
        }
        [XmlAttribute("board")]
        public int board;
        [XmlAttribute("pos")]
        public int pos;
        [XmlElement("Camera")]
        public List<Camera_Preset> Cameras;
        [XmlElement("Comment")]
        public string Comment;
        [XmlIgnore]
        public bool status = true;
        [XmlIgnore]
        public bool alert = false;
        [XmlElement("TimeOff")]
        public List<TimeOff> TimeOffs;
        [XmlIgnore]
        public bool isDisable;
        [XmlIgnore]
        public bool isEnable;
        [XmlIgnore]
        public bool Auto = true;
        public void RunCommand(string cmd)
        {
            CsvRow row = new CsvRow();
            row.Add(DateTime.Now.format("dd-MM-yyyy HH:mm:ss"));
            row.Add(this.Com);
            row.Add(this.Name);           
            try
            {
                string msg = string.Format(cmd, this.board, this.pos);
                 row.Add(msg);
                if (App.listSerialPort != null)
                {
                    for (int i = 0; i < App.listSerialPort.Count; i++)
                    {
                        if (this.Com.ToLower() == App.listSerialPort[i].PortName.ToLower())
                        {
                            App.listSerialPort[i].sendCommand(msg);
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
               
            }
            
            App.RowsSend.Add(row);
        }



        internal void Disable()
        {
            this.RunCommand(App.DefineCommand.DISABLE);
            EasyTimer.SetTimeout(() => { this.RunCommand(App.DefineCommand.ALARM_OFF); }, 500);
          
            this.status = false;
        }

        internal void Enable()
        {
            this.RunCommand(App.DefineCommand.ENABLE);
            this.status = true;
        }
    }
    [Serializable]
    public class Alarms
    {
        [XmlElement("Alarm")]
        public List<Alarm> Children;
        public Alarms()
        {
            if (this.Children == null)
            {
                this.Children = new List<Alarm>();
            }
        }

        public Alarm this[int index]
        {
            get
            {
                return this.Children[index];
            }
            set
            {
                this.Children[index] = value;
            }
        }
        [XmlIgnore]
        public int Count
        {
            get
            {
                if (this.Children == null)
                    return 0;
                return this.Children.Count;
            }
        }
        public static Alarms Read(string file)
        {
            if (!File.Exists(file))
            {
                Write(file, new Alarms());
                return new Alarms();
            }
            else
            {
                FileInfo inf = new FileInfo(file);
                while (inf.IsFileLocked()) { Console.WriteLine("Wait..."); };
                try
                {
                    using (Stream s = File.Open(file, FileMode.Open))
                    {
                        System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Alarms));
                        return (Alarms)reader.Deserialize(s);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.GetBaseException().ToString());
                    return new Alarms();
                }

            }
        }

        public static void Write(string file, Alarms overview)
        {
            if (string.IsNullOrEmpty(file))
                throw new Exception("File Not Empty");
            System.Xml.Serialization.XmlSerializer writer =
            new System.Xml.Serialization.XmlSerializer(typeof(Alarms));
            System.Xml.XmlWriterSettings setting = new System.Xml.XmlWriterSettings();
            setting.Encoding = Encoding.UTF8;
            setting.CloseOutput = true;
            setting.NewLineChars = "\r\n";
            setting.Indent = true;
            if (!File.Exists(file))
            {
                using (Stream s = File.Open(file, FileMode.OpenOrCreate))
                {
                    System.Xml.XmlWriter tmp = System.Xml.XmlWriter.Create(s, setting);
                    writer.Serialize(tmp, overview);
                }
            }
            else
            {
                using (Stream s = File.Open(file, FileMode.Truncate))
                {
                    System.Xml.XmlWriter tmp = System.Xml.XmlWriter.Create(s, setting);
                    writer.Serialize(tmp, overview);
                }
            }
        }
    }
}
