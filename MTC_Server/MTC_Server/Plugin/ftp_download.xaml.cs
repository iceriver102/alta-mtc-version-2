using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MTC_Server.Plugin
{
    /// <summary>
    /// Interaction logic for ftp_download.xaml
    /// </summary>
    public partial class ftp_download : UserControl
    {
        public event EventHandler<string> CompleteEvent;
        public string FtpUser
        {
            get;set;
        }
        public string FtpPassword
        {
            get;set;
        }
        public string _local;
        public string Local
        {
            get
            {
                return this._local;
            }
            set
            {
                this._local = value;
            }
        }

        private string _url;

        public string Url
        {
            get
            {
                return this._url;
            }
            set
            {
                this._url = value;
            }
        }

        private bool _complete = false;
        public bool isUploaded
        {
            get
            {
                return this._complete;
            }
            private set
            {
                this._complete = value;
                if (this.CompleteEvent != null)
                    this.CompleteEvent(this, this.Url);
            }
        }
        public bool isDowloaded
        {
            get
            {
                return this._complete;
            }
            set
            {
                this._complete = value;
                if (this.CompleteEvent != null)
                    this.CompleteEvent(this, this.Local);
            }
        }
        private int mode = 0;

        private BackgroundWorker bw = new BackgroundWorker();

        public ftp_download()
        {
            InitializeComponent();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.mode < 0)
            {
                this.isDowloaded = true;
            }
            else if (this.mode > 0)
            {
                this.isUploaded = true;
            }
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.UIProgess.Value = e.ProgressPercentage;
        }

        public void RunDownLoad()
        {
            
            if (bw.IsBusy != true)
            {
                bw.DoWork -= new DoWorkEventHandler(bw_RunUpLoad);
                bw.DoWork -= new DoWorkEventHandler(bw_RunDownLoad);
                bw.DoWork += new DoWorkEventHandler(bw_RunDownLoad); ;
                bw.RunWorkerAsync();
                this.mode = -1;
            }
            
        }

        public void RunUpLoad()
        {
            if (bw.IsBusy != true)
            {
                bw.DoWork -= new DoWorkEventHandler(bw_RunUpLoad);
                bw.DoWork -= new DoWorkEventHandler(bw_RunDownLoad);
                bw.DoWork += new DoWorkEventHandler(bw_RunUpLoad); ;
                bw.RunWorkerAsync();
                this.mode = 1;
            }

        }

        private void bw_RunUpLoad(object sender, DoWorkEventArgs e)
        {
            try
            {
                BackgroundWorker worker = sender as BackgroundWorker;
                var ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(this.Url));
                ftpWebRequest.Credentials = new NetworkCredential(this.FtpUser, this.FtpPassword);
                ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
                FileInfo fileInfo = new FileInfo(this.Local);
                using (FileStream inputStream = fileInfo.OpenRead())
                {
                    using (Stream outputStream = ftpWebRequest.GetRequestStream())
                    {
                        byte[] buffer = new byte[1024 * 1024];
                        int totalReadBytesCount = 0;
                        int readBytesCount;
                        while ((readBytesCount = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            outputStream.Write(buffer, 0, readBytesCount);
                            totalReadBytesCount += readBytesCount;
                            var progress = totalReadBytesCount * 100.0 / inputStream.Length;
                            worker.ReportProgress((int)progress);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void bw_RunDownLoad(object sender, DoWorkEventArgs e)
        {
            try
            {
                BackgroundWorker worker = sender as BackgroundWorker;
                var ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(this.Url));
                ftpWebRequest.Credentials = new NetworkCredential(this.FtpUser, this.FtpPassword);
                ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                FileInfo fileInfo = new FileInfo(this.Local);
                ftpWebRequest.UseBinary = true;

                using (Stream outputStream = ftpWebRequest.GetResponse().GetResponseStream())
                {

                    using (FileStream inputStream = fileInfo.Create())
                    {
                        byte[] buffer = new byte[1024 * 1024];
                        int totalReadBytesCount = 0;
                        int readBytesCount;
                        while ((readBytesCount = outputStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            inputStream.Write(buffer, 0, readBytesCount);
                            totalReadBytesCount += readBytesCount;
                            var progress = totalReadBytesCount * 100.0 / outputStream.Length;
                            worker.ReportProgress((int)progress);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
