using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Alta.Class;
namespace Camera_Final.Code
{
    [Serializable]
    public class Camera_Preset
    {
        [XmlAttribute("Camera")]
        public int camera_id;
        [XmlAttribute("Postion")]
        public int Postion;
        public Camera_Preset()
        {

        }
        public Camera_Preset(int id, int position)
        {
            this.camera_id = id;
            this.Postion = position;
        }
    }

    [Serializable]
    public class Camera : IEquatable<Camera>, IComparable<Camera>
    {
        [XmlAttribute]
        public int id;
        [XmlAttribute("name")]
        public string name;
        [XmlAttribute("ip")]
        public string ip;
        [XmlAttribute("port")]
        public int port = 8000;
        [XmlAttribute("admin")]
        public string admin;
        [XmlAttribute("pass")]
        public string pass;
        [XmlAttribute("time")]
        public DateTime Time;
        [XmlAttribute("channel")]
        public int channel = 1;
        [XmlAttribute("port_rtsp")]
        public int port_rtsp = 554;
        [XmlAttribute("left")]
        public double Left;
        [XmlAttribute("top")]
        public double Top;
        [XmlAttribute("icon")]
        public string icon = "fa-video-camera";
        [XmlAttribute("MaxPreset")]
        public int MaxPreset=255;
        [XmlIgnore]
        public List<int> Preset
        {
            get
            {
              
                List<int> tmp = new List<int>();
                if (App.DataPreset != null)
                {
                   tmp= App.DataPreset.getPresetOfCamera(this.id);
                }
               
                if (App.DataAlarm != null)
                {
                    if (tmp == null)
                        tmp = new List<int>();
                    for (int i = 0; i< App.DataAlarm.Count; i++)
                    {
                        if(App.DataAlarm[i].Cameras!=null)
                        {
                            for (int j = 0; j < App.DataAlarm[i].Cameras.Count;j++ )
                            {
                                    if (App.DataAlarm[i].Cameras[j].camera_id == this.id)
                                    {
                                        tmp.Add(App.DataAlarm[i].Cameras[j].Postion);
                                        break;
                                    }
                                
                            }
                        }
                    }
                }
                return tmp;
            }
        }
        [XmlIgnore]
        public string rtsp
        {
            get
            {
                return string.Format("rtsp://{0}:{1}@{2}:{3}{4}",this.admin,this.pass,this.ip,this.port_rtsp,App.setting.suffix_rtsp);
            }
        }

        [XmlIgnore]
        public CHCNetSDK.NET_DVR_DEVICEINFO_V30 m_struDeviceInfo;
        [XmlIgnore]
        public Int32 m_lUserID = -1;
        [XmlIgnore]
        public Int32 m_iPreviewType = 0;
        [XmlIgnore]
        public Int32 m_lRealHandle = -1;
        [XmlIgnore]
        public Int32 m_lPort = -1;
        [XmlIgnore]
        public bool view = false;

        public Camera()
        {

        }

        ~Camera()
        {

            if (m_lRealHandle >= 0)
            {
                CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle);
            }
            if (m_lUserID >= 0)
            {
                CHCNetSDK.NET_DVR_Logout_V30(m_lUserID);
            }
        }

        public bool Login()
        {
            if (string.IsNullOrEmpty(this.ip) || string.IsNullOrEmpty(this.admin))
                return false;
            this.m_lUserID = CHCNetSDK.NET_DVR_Login_V30(this.ip, this.port, this.admin, this.pass, ref m_struDeviceInfo);
            if (this.m_lUserID == -1)
                return false;
            return true;
        }

        public Camera(string ip)
        {
            this.ip = ip;
            Time = DateTime.Now;
        }
        public int CompareTo(Camera comparePart)
        {
            // A null value means that this object is greater. 
            if (comparePart == null)
                return 1;

            else
                return this.id.CompareTo(comparePart.id);
        }
        public override int GetHashCode()
        {
            return this.id;
        }
        public bool Equals(Camera other)
        {
            if (other == null) return false;
                return (this.id.Equals(other.id));
        }

        public int FindFreePreset()
        {
            if (this.Preset == null || this.Preset.Count == 0)
                return 1;
            for (int i = 1; i < MaxPreset + 1; i++)
            {
                bool canFree= true;
                for (int j = 0; j < this.Preset.Count; j++)
                {
                    if(this.Preset[j]==i)
                    {
                        canFree = false;
                        break;
                    }
                }
                if (canFree)
                {
                    return i;
                }
            }
            return -1;
        }

    }
    [Serializable]
    public class Camera_Goto : IEquatable<Camera_Goto>, IComparable<Camera_Goto>
    {
        [XmlAttribute("name")]
        public string name;
        [XmlAttribute("postion")]
        public int id;
        [XmlAttribute("left")]
        public double left;
        [XmlAttribute("top")]
        public double top;
        [XmlAttribute("icon")]
        public string icon="fa-anchor";
        [XmlElement ("Camera")]
        public List<Camera_Preset> Camera;
        [XmlElement("Comment")]
        public string comment;

        public Camera_Goto()
        {
            if (this.Camera == null)
                this.Camera = new List<Camera_Preset>();
        }

        public int CompareTo(Camera_Goto comparePart)
        {
            // A null value means that this object is greater. 
            if (comparePart == null)
                return 1;

            else
                return this.id.CompareTo(comparePart.id);
        }
        public override int GetHashCode()
        {
            return this.id;
        }
        public bool Equals(Camera_Goto other)
        {
            if (other == null) return false;
            return (this.id.Equals(other.id));
        }

        public Camera_Preset getCamera(int id)
        {
            if (this.Camera != null && this.Camera.Count > 0)
            {
                for (int i = 0; i < this.Camera.Count; i++)
                {
                    if (id == this.Camera[i].camera_id)
                    {
                        return new Camera_Preset(this.Camera[i].camera_id,this.Camera[i].Postion);
                    }
                }
            }
            return null;
        }
    }
    public class Presets
    {
        [XmlElement("Preset")]
        public List<Camera_Goto> Children;
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


        public List<int> getPresetOfCamera(int camera)
        {
            if (this.Children != null && this.Children.Count > 0)
            {
                List<int> result = new List<int>();
                for (int i = 0; i < this.Count; i++)
                {
                    if (this.Children[i].Camera != null && this.Children[i].Camera.Count > 0)
                    {
                        for (int j = 0; j < this.Children[i].Camera.Count; j++)
                        {
                            if (this.Children[i].Camera[j].camera_id == camera)
                            {
                                result.Add(this.Children[i].Camera[j].Postion);
                            }
                        }
                    }
                }
                return result;
            }
            return null;
        }

        public Presets()
        {
            this.Children = new List<Camera_Goto>();
        }


        public Camera_Goto this[int index]
        {
            get
            {
                if (this.Children == null)
                    throw new Exception("Không tìm thấy");
                if (index > -1 && index < this.Children.Count)
                {
                    return this.Children[index];
                }
                throw new Exception("Không tìm thấy");
            }
        }

        public void Add(Camera_Goto preset)
        {
            if (preset.id == 0)
            {
                if (this.Children.Count == 0)
                    preset.id = 1;
                else
                    preset.id = this.Children[this.Count - 1].id + 1;
            }

            for (int i = 0; i < this.Children.Count; i++)
            {
                if (this.Children[i].id == preset.id)
                {
                    return;
                }
            }
            this.Children.Add(preset);
            this.Children.Sort();
        }

        public void Remove(Camera_Goto cam)
        {
            this.Children.Remove(cam);
            this.Children.Sort();
        }
        public void RemoveAt(int index)
        {
            this.Children.RemoveAt(index);
            this.Children.Sort();
        }
        public static Presets Read(string file)
        {
            if (!File.Exists(file))
            {
                Write(file, new Presets());
                return new Presets();
            }
            else
            {
                Presets tmp;
                FileInfo inf = new FileInfo(file);
                while (inf.IsFileLocked()) { Console.WriteLine("Wait..."); };
                try
                {
                    using (Stream s = File.Open(file, FileMode.Open))
                    {
                        System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Presets));
                        tmp = (Presets)reader.Deserialize(s);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.GetBaseException().ToString());
                    return new Presets();
                }
                return tmp;

            }
        }

        public static void Write(string file, Presets overview)
        {
            if (string.IsNullOrEmpty(file))
                throw new Exception("File Not Empty");
            System.Xml.Serialization.XmlSerializer writer =
            new System.Xml.Serialization.XmlSerializer(typeof(Presets));
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


    [Serializable]
    public class Cameras
    {
        [XmlElement("Camera")]
        public List<Camera> Datas;
        [XmlIgnore]
        public int Count
        {
            get
            {
                if (this.Datas == null)
                    return 0;
                return this.Datas.Count;
            }
        }

        public Camera this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Datas.Count || this.Datas == null)
                    throw new Exception("Out of Range");
                return this.Datas[index];

            }
        }

        public Camera this[string ip]
        {
            get
            {
                if (this.Datas == null)
                    return null;
                foreach (Camera cam in this.Datas)
                {
                    if (cam.ip == ip)
                        return cam;
                }
                return null;
            }
        }


        public Cameras()
        {
            this.Datas = new List<Camera>();
        }
        public void Add(Camera cam)
        {
            if (cam.id == 0)
            {
                if (this.Datas.Count == 0)
                {
                    cam.id = 1;
                }
                else
                {
                    cam.id = this.Datas[this.Count - 1].id + 1;
                }

            }
            else
            {
                for (int i = 0; i < this.Count;i++ )
                {
                    if (this.Datas[i].Equals(cam))
                    {
                        throw new Exception("Camera đã tồn tại");
                    }
                }
            }
            this.Datas.Add(cam);
            this.Datas.Sort();
        }

        public void Remove(Camera cam)
        {
            this.Datas.Remove(cam);
            this.Datas.Sort();
        }
        public void RemoveAt(int index)
        {
            this.Datas.RemoveAt(index);
            this.Datas.Sort();
        }
        public static Cameras Read(string file)
        {
            if (!File.Exists(file))
            {
                Write(file, new Cameras());
                return new Cameras();
            }
            else
            {
                FileInfo inf = new FileInfo(file);
                while (inf.IsFileLocked()) { Console.WriteLine("Wait..."); };
                try
                {
                    using (Stream s = File.Open(file, FileMode.Open))
                    {
                        System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Cameras));
                        return (Cameras)reader.Deserialize(s);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.GetBaseException().ToString());
                    return new Cameras();
                }

            }
        }

        public static void Write(string file, Cameras overview)
        {
            if (string.IsNullOrEmpty(file))
                throw new Exception("File Not Empty");
            System.Xml.Serialization.XmlSerializer writer =
            new System.Xml.Serialization.XmlSerializer(typeof(Cameras));
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
