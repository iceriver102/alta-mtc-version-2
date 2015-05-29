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
    public class Map
    {
        [XmlAttribute("Name")]
        public string Name;
        [XmlElement("FILE")]
        public string FileImage;
        public Cameras Camera;
        public Map()
        {
            
        }
        public static Map Read(string file)
        {
            if (!File.Exists(file))
            {
                Write(file, new Map());
                return new Map();
            }
            else
            {
                FileInfo inf = new FileInfo(file);
                while (inf.IsFileLocked()) { Console.WriteLine("Wait..."); };
                try
                {
                    using (Stream s = File.Open(file, FileMode.Open))
                    {
                        System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Map));
                        return (Map)reader.Deserialize(s);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.GetBaseException().ToString());
                    return new Map();
                }

            }
        }

        public static void Write(string file, Map overview)
        {
            if (string.IsNullOrEmpty(file))
                throw new Exception("File Not Empty");
            System.Xml.Serialization.XmlSerializer writer =
            new System.Xml.Serialization.XmlSerializer(typeof(Map));
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
