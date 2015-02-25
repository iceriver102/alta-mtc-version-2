using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
