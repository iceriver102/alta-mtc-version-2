using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alta.Class;

namespace Camera_Final.Code
{
    public class CommandDefine
    {
        public string OFF = "#{0} C {1} 0$";
        public string ON = "#{0} C {1} 1$";
        public string DISABLE = "#{0} F {1} 0$";
        public string ENABLE = "#{0} F {1} 1$";
        public string ALARM_OFF = "#{0} H {1} 0$";
        public string ALARM_ON = "#{0} H {1} 1$";
        public string LED_OFF = "#{0} G {1} 0$";
        public string LED_ON = "#{0} G {1} 1$";
        public static CommandDefine Read(string file)
        {
            if (!File.Exists(file))
            {
                Write(file, new CommandDefine());
                return new CommandDefine();
            }
            else
            {
                FileInfo inf = new FileInfo(file);
                while (inf.IsFileLocked()) { Console.WriteLine("Wait..."); };
                try
                {
                    using (Stream s = File.Open(file, FileMode.Open))
                    {
                        System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(CommandDefine));
                        return (CommandDefine)reader.Deserialize(s);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.GetBaseException().ToString());
                    return new CommandDefine();
                }

            }
        }

        public static void Write(string file, CommandDefine overview)
        {
            if (string.IsNullOrEmpty(file))
                throw new Exception("File Not Empty");
            System.Xml.Serialization.XmlSerializer writer =
            new System.Xml.Serialization.XmlSerializer(typeof(CommandDefine));
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
