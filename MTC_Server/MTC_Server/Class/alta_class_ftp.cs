using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Alta.Class
{
    public class alta_class_ftp
    {
        public static String downLoadFile(String fptPath, string server, string user, string pass, String downLoadPath = @"tmp")
        {
            FileStream outputStream = null;
            string[] expode = fptPath.Split(new char[] { '/', '\\' });
            string fileName = expode[expode.Length - 1];
            if (!File.Exists(downLoadPath + @"\" + fileName))
            {
                try
                {
                    // Get the object used to communicate with the server.
                    var reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(server + fptPath));
                    reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;

                    // This example assumes the FTP site uses anonymous logon.
                    reqFTP.Credentials = new NetworkCredential(user,pass);

                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    Stream responseStream = response.GetResponseStream();
                    outputStream = new FileStream(downLoadPath + @"\" + fileName, FileMode.Create);
                    long cl = response.ContentLength;
                    int bufferSize = 2048;
                    byte[] buffer = new byte[bufferSize];
                    int readCount = responseStream.Read(buffer, 0, bufferSize);

                    while (readCount > 0)
                    {
                        outputStream.Write(buffer, 0, readCount);
                        readCount = responseStream.Read(buffer, 0, bufferSize);
                    }
                    outputStream.Close();
                    response.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    // reqFTP.Abort();
                }
            }
            return downLoadPath + @"\" + fileName;
        }
        public string UploadLocalFiles(string localPath,string url,string user,string pass, string toPath = @"/DataFtp/")
        {
            FileInfo fileInfo = new FileInfo(localPath);
            try
            {
                if (toPath == @"/DataFtp/")
                {
                    toPath = toPath + fileInfo.Name;
                }
                var reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(url + toPath));
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                reqFTP.Credentials = new NetworkCredential(user, pass);
                FileStream sourceStream = fileInfo.OpenRead();
                byte[] fileContents = new byte[fileInfo.Length];
                sourceStream.Read(fileContents, 0, Convert.ToInt32(fileInfo.Length));
                sourceStream.Close();
                reqFTP.ContentLength = fileContents.Length;
                Stream requestStream = reqFTP.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
            }
            catch (Exception e)
            {
                toPath = string.Empty;
            }
            finally
            {

            }
            return toPath;
        }

        public static void deleteFile(string fileName,string url, string user,string pass)
        {
            try
            {
                var reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(url + fileName));
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
                reqFTP.Credentials = new NetworkCredential(user, pass);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();

            }
            catch (Exception ex)
            {

            }
        }
        public static string CreateFolder(string name, string url,string user,string pass)
        {
            try
            {
                var reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(url + name));
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFTP.Credentials = new NetworkCredential(user, pass);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
                return name;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static bool DeleteFolder(string name, string url, string user, string pass)
        {
            // name = @"/Image_1";
            try
            {
                var reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(url + name));
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(user, pass);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                while (!reader.EndOfStream)
                {
                    string tmp = @"/DataFtp/" + reader.ReadLine();
                    try
                    {
                        deleteFile(tmp,url,user,pass);
                    }
                    catch (Exception)
                    {
                    }
                }

                response.Close();
                reqFTP.Abort();
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(url + name));
                reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;
                reqFTP.Credentials = new NetworkCredential(user, pass);
                response = (FtpWebResponse)reqFTP.GetResponse();

                response.Close();
                reqFTP.Abort();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void RenameFileName(string currentFilename, string newFilename,string url,string user,string pass)
        {

            // FtpWebRequest reqFTP = null;
            Stream ftpStream = null;
            try
            {
                var reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(url + currentFilename));
                reqFTP.Method = WebRequestMethods.Ftp.Rename;
                reqFTP.RenameTo = newFilename;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(user, pass);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                ftpStream = response.GetResponseStream();
                ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                if (ftpStream != null)
                {
                    ftpStream.Close();
                    ftpStream.Dispose();
                }
                throw new Exception(ex.Message.ToString());
            }
        }

    }
    public static class SSLValidator
    {
        private static bool OnValidateCertificate(object sender, X509Certificate certificate, X509Chain chain,
                                                  SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        public static void OverrideValidation()
        {
            ServicePointManager.ServerCertificateValidationCallback =
                OnValidateCertificate;
            ServicePointManager.Expect100Continue = true;
        }
    }
}
