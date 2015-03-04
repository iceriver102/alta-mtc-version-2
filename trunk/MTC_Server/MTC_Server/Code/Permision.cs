using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTC_Server.Code
{
    public class Permision
    {
        public bool mana_user = false;
        public bool view_all_media = false;
        public bool mana_schedule = false;
        public bool confirm_media = false;
        public bool mana_device = false;
        public static Permision Read(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return new Permision();
            }
            else
            {
                using (TextReader sr = new StringReader(xml))
                {
                    System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Permision));
                    return (Permision)reader.Deserialize(sr);
                }
            }
        }
        public static string Write(Permision overview)
        {
            if (overview==null)
                throw new Exception("File Not Empty");
            System.Xml.Serialization.XmlSerializer writer =
           new System.Xml.Serialization.XmlSerializer(typeof(Permision));
            System.Xml.XmlWriterSettings setting = new System.Xml.XmlWriterSettings();
            setting.Encoding = Encoding.UTF8;
            setting.CloseOutput = true;
            using (var sw = new StringWriter())
            {
                using (var xw = System.Xml.XmlWriter.Create(sw, setting))
                {
                    writer.Serialize(xw, overview);
                }
                return sw.ToString();
            }
        }
    }
}
