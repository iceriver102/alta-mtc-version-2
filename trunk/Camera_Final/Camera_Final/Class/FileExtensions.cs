using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Alta.Class
{
    public static class FileExtensions
    {
        public static bool IsFileLocked(this FileInfo file)
        {
            FileStream stream = null;
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }
        /*
        public static string toTimeMedia(this FileInfo file)
        {
            ShellFile so = ShellFile.FromFilePath(file.FullName);
            double nanoseconds;
            double.TryParse(so.Properties.System.Media.Duration.Value.ToString(), out nanoseconds);
            TimeSpan tmp = new TimeSpan();
            if (nanoseconds > 0)
            {
                double seconds = nanoseconds / 10000000;
                tmp = seconds.secondToTimeSpan();

            }
            return tmp.Format();
        }
        */
        public static string toMD5(this FileInfo file)
        {
            if (file.Exists)
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(file.FullName))
                    {
                         byte[] data= md5.ComputeHash(stream);
                         String tmp = BitConverter.ToString(data).Replace("-", "");
                         return tmp.ToLower();
                    }
                }
            }
            return string.Empty;
        }

        /*
        public static string ConvertImageToBase64(this FileInfo inf)
        {
            if (!inf.Exists)
            {
                return string.Empty;
            }
            using (Image image = Image.FromFile(inf.FullName))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
        }
        */
        public static string toBase64(this FileInfo file)
        {
            byte[] bytes = File.ReadAllBytes(file.FullName);
            return  Convert.ToBase64String(bytes);
        }
    }
}
