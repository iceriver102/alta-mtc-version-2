using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
        public DateTime EndDate;
        public int secondAutoLogout = 1800;  
        public string connectString
        {
            get
            {

                return @"server=" + this.db_server + ";uid=" + this.db_user + ";pwd=" + this.db_password + ";database=" + this.db_name + ";charset=utf8;";
            }
        }
        /*
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
         * */
        public static void Write(string filename, Config data, string key)
        {
            using (RijndaelManaged RMCrypto = new RijndaelManaged())
            {
                byte[] Key = ("Teppy@MTC_1.0_2015"+key).toMD5().Clone(0.5f).toBytes();
                byte[] IV = { 0x01, 0x02, 0x03, 0x04, 0x32, 0x06, 0x07, 0x08, 0x92, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x76 };
                RMCrypto.Mode = CipherMode.CBC;
                RMCrypto.Padding = PaddingMode.Zeros;
                //byte[] input = data.toByteArray();
                if (!File.Exists(filename))
                {
                    using (Stream s = File.Open(filename, FileMode.OpenOrCreate))
                    {
                        using (CryptoStream CryptStream = new CryptoStream(s, RMCrypto.CreateEncryptor(Key, IV), CryptoStreamMode.Write))
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            formatter.Serialize(CryptStream, data);
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
                            BinaryFormatter formatter = new BinaryFormatter();
                            formatter.Serialize(CryptStream, data);
                            CryptStream.FlushFinalBlock();
                            CryptStream.Close();
                        }
                        s.Close();
                    }
                }
            }
        }

        public static Config Read(string file,string key)
        {
            if (!File.Exists(file))
            {
                return null;
            }
            Config data = null;
            using (RijndaelManaged RMCrypto = new RijndaelManaged())
            {
                byte[] Key = ("Teppy@MTC_1.0_2015"+key.Trim()).toMD5().Clone(0.5f).toBytes();
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
                            BinaryFormatter bin = new BinaryFormatter();

                            try
                            {
                                data = (Config)bin.Deserialize(CryptStream);
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
    }
}
