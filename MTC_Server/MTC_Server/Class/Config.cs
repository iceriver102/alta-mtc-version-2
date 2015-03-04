﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Alta.Class
{
    [Serializable]
    public class Config
    {
        public string db_name = "mtc_db";
        public string db_server = "localhost";
        public string db_user = "root";
        public string db_password = "";

        public string ftp_server = "127.0.0.1";
        public string ftp_user = "demo";
        public string ftp_password = "";
        public int ftp_port = 21;
        public string ftp_folder = "Medias";
        public string temp_folder = "Medias";
  
        public string connectString
        {
            get
            {

                return @"server=" + this.db_server + ";uid=" + this.db_user + ";pwd=" + this.db_password + ";database=" + this.db_name + ";";
            }
        }

        public static Config Read(string file)
        {
            if (!File.Exists(file))
            {
                Write(file, new Config());
                return new Config();
            }
            else
            {
                FileInfo inf = new FileInfo(file);
                while (inf.IsFileLocked()) { Console.WriteLine("Wait..."); };
                try
                {
                    using (Stream s = File.Open(file, FileMode.Open))
                    {
                        System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Config));
                        return (Config)reader.Deserialize(s);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.GetBaseException().ToString());
                    return new Config();
                }

            }
        }

        public static void Write(string file, Config overview )
        {
            if (string.IsNullOrEmpty(file))
                throw new Exception("File Not Empty");
            System.Xml.Serialization.XmlSerializer writer =
            new System.Xml.Serialization.XmlSerializer(typeof(Config));
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
