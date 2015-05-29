
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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

        public int secondAutoLogout = 3;
        public string Log = string.Empty;

        public string connectString
        {
            get
            {

                return @"server=" + this.db_server + ";uid=" + this.db_user + ";pwd=" + this.db_password + ";database=" + this.db_name + ";charset=utf8;";
            }
        }

        public System.Windows.Media.Stretch ModeVideo = System.Windows.Media.Stretch.UniformToFill;
        public bool Topmost = false;
        public double Top = 0;
        public double Left = 0;
        public int Volume = 0;
        public double Height = 768;
        public double Width = 1366;
        public System.Windows.WindowState WindowState = System.Windows.WindowState.Normal;

        public static Config Read(string file)
        {
            if (!File.Exists(file))
            {
                return null;
            }
            Config data = null;
            using (RijndaelManaged RMCrypto = new RijndaelManaged())
            {
                byte[] Key = ("Teppy@MTC_1.0_2015").toMD5().Clone(0.5f).toBytes();
                byte[] IV = { 0x01, 0x02, 0x03, 0x04, 0x32, 0x06, 0x07, 0x08, 0x92, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x76 };
                RMCrypto.Mode = CipherMode.CBC;
                RMCrypto.Padding = PaddingMode.Zeros;
                var keyRMC = RMCrypto.CreateDecryptor(Key, IV);
                using (Stream s = File.Open(file, FileMode.Open))
                {
                    if (s.Length > 0)
                    {
                        using (CryptoStream CryptStream = new CryptoStream(s, keyRMC, CryptoStreamMode.Read))
                        {
                            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Config));

                            try
                            {
                                data = (Config)reader.Deserialize(CryptStream);
                            }
                            catch (Exception)
                            {

                                data = null;
                            }
                        }
                        s.Close();
                    }
                    else
                    {
                        s.Close();
                        File.Delete(file);
                    }


                }
            }
            return data;
        }

        public static void Write(string filename, Config data)
        {
            using (RijndaelManaged RMCrypto = new RijndaelManaged())
            {
                byte[] Key = ("Teppy@MTC_1.0_2015").toMD5().Clone(0.5f).toBytes();
                byte[] IV = { 0x01, 0x02, 0x03, 0x04, 0x32, 0x06, 0x07, 0x08, 0x92, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x76 };
                RMCrypto.Mode = CipherMode.CBC;
                RMCrypto.Padding = PaddingMode.Zeros;

                System.Xml.XmlWriterSettings setting = new System.Xml.XmlWriterSettings();
                System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(Config));
                setting.Encoding = Encoding.UTF8;
                setting.CloseOutput = true;
                setting.NewLineChars = "\r\n";
                //byte[] input = data.toByteArray();
                if (!File.Exists(filename))
                {

                    using (Stream s = File.Open(filename, FileMode.OpenOrCreate))
                    {
                        using (CryptoStream CryptStream = new CryptoStream(s, RMCrypto.CreateEncryptor(Key, IV), CryptoStreamMode.Write))
                        {
                            System.Xml.XmlWriter formatter = System.Xml.XmlWriter.Create(CryptStream, setting);
                            writer.Serialize(formatter, data);
                            CryptStream.FlushFinalBlock();
                            CryptStream.Close();
                        }
                        s.Close();
                    }
                }
                else
                {
                    using (Stream s = File.Open(filename, FileMode.Truncate))
                    {
                        using (CryptoStream CryptStream = new CryptoStream(s, RMCrypto.CreateEncryptor(Key, IV), CryptoStreamMode.Write))
                        {
                            System.Xml.XmlWriter formatter = System.Xml.XmlWriter.Create(CryptStream, setting);
                            writer.Serialize(formatter, data);
                            CryptStream.FlushFinalBlock();
                            CryptStream.Close();
                        }
                        s.Close();
                    }
                }
            }
        }
    }
}
