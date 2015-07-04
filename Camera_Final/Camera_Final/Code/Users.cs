using Alta.Class;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Camera_Final.Code
{
    [Serializable]
    public class User
    {
        public int id;
        public string user_name;
        public string FullName;
        public string Pass;
        public DateTime time;
        public int type = 0;
        public string Hash
        {
            get
            {
                return (this.user_name + FunctionStatics.getCPUID() + this.Pass).toMD5();
            }
        }
        public bool Login(string hash)
        {
            string _hash = (this.user_name + FunctionStatics.getCPUID() + this.Pass).toMD5();
            if (_hash == hash)
            {
                return true;
            }
            return false;
        }
        public User(int id)
        {
            this.id = id;
            this.time = DateTime.Now;
        }

        public User(string user_name, int id)
        {
            this.user_name = user_name;
            this.id = id;
            this.time = DateTime.Now;
        }
    }
    [Serializable]
    public class Users : IEnumerable
    {
        public List<User> Datas;
        public int maxId
        {
            get
            {
                if (this.Datas == null)
                    return 1;
                int max = 0;
                foreach (User u in this.Datas)
                {
                    if (u.id > max)
                        max = u.id;
                }
                return max;
            }
        }
        public User this[int id]
        {
            get
            {
                if (this.Datas == null)
                    return null;
                foreach (User u in this.Datas)
                {
                    if (u.id == id)
                        return u;
                }
                return null;
            }
        }
        public User this[string user_name]
        {
            get
            {
                if (this.Datas == null)
                    return null;
                foreach (User u in this.Datas)
                {
                    if (u.user_name == user_name)
                        return u;
                }
                return null;
            }
        }

        public IEnumerator GetEnumerator()
        {
            foreach (User u in this.Datas)
            {
                yield return u;
            }
        }

        public bool Add(User u)
        {
            if (this.Datas == null)
                this.Datas = new List<User>();
            if (u.id == 0 || string.IsNullOrEmpty(u.user_name))
                return false;
            foreach (User tmpU in this.Datas)
            {
                if (u.user_name == tmpU.user_name || u.id == tmpU.id)
                {
                    return false;
                }
            }

            this.Datas.Add(u);
            return true;
        }

        public bool RemoveAt(int index)
        {
            if (this.Datas == null)
                return false;
            if (index < this.Datas.Count && index >= 0)
            {
                this.Datas.RemoveAt(index);
                return true;
            }
            return false;
        }
        public bool Remove(User u)
        {
            return this.Datas.Remove(u);
        }

        public User Login(string _hash)
        {
            if (this.Datas == null)
                return null;
            foreach (User u in this.Datas)
            {
                if (u.Login(_hash))
                    return u;
            }
            return null;
        }




        public static void write(string filename, Users data)
        {
            using (RijndaelManaged RMCrypto = new RijndaelManaged())
            {
                byte[] Key = ("Teppy@MTC_1.0_2015").toMD5().Clone(0.5f).toBytes();
                byte[] IV = { 0x01, 0x02, 0x03, 0x04, 0x32, 0x06, 0x07, 0x08, 0x92, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x76 };
                RMCrypto.Mode = CipherMode.CBC;
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

        public static Users read(string file)
        {
            if (!File.Exists(file))
            {
                return null;
            }
            Users data = null;
            using (RijndaelManaged RMCrypto = new RijndaelManaged())
            {
                byte[] Key = ("Teppy@MTC_1.0_2015").toMD5().Clone(0.5f).toBytes();
                byte[] IV = { 0x01, 0x02, 0x03, 0x04, 0x32, 0x06, 0x07, 0x08, 0x92, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x76 };
                RMCrypto.Mode = CipherMode.CBC;
                var keyRMC = RMCrypto.CreateDecryptor(Key, IV);
                using (Stream s = File.Open(file, FileMode.Open))
                {
                    if (s.Length > 0)
                    {
                        using (CryptoStream CryptStream = new CryptoStream(s, keyRMC, CryptoStreamMode.Read))
                        {
                            BinaryFormatter bin = new BinaryFormatter();
                            data = (Users)bin.Deserialize(CryptStream);
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
