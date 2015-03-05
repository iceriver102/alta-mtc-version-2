using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Alta.Class;

namespace MTC_Server.UIView.Media
{
    /// <summary>
    /// Interaction logic for UIMediaEdit.xaml
    /// </summary>
    public partial class UIMediaEdit : UserControl
    {
        private Code.Media.MediaData tmpMedia;
        private Code.Media.MediaData m;
        public Code.Media.MediaData Media
        {
            get { return this.m; }
            set { this.m = value; }
        }
        public int Type { get; set; } = 1;

        public event EventHandler<Code.Media.MediaData> CloseEvent;
        public UIMediaEdit()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a Video";
            op.Filter = "Video File(*.mov,*.wmv,*.avi,*.mp4,*.mpg,*.flv,*.h264,*.wp3)|*.mov;*.wmv;*.avi;*.mp4;*.mpg;*.flv;*.h264;*.wp3|" +
                "All type(*.*)|*.*";
            if (op.ShowDialog() == true)
            {
                this.UILocalFile.Text = op.FileName;
            }
        }

        private void UIBtnSave_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (tmpMedia == null)
            {
                if (string.IsNullOrEmpty(this.UINameEdit.Text))
                {
                    MessageBox.Show("Tên video không được để trống!");
                    return;
                }
                if (this.Type == 1)
                {
                    #region File
                    if (this.Media == null && string.IsNullOrEmpty(this.UILocalFile.Text))
                    {
                        MessageBox.Show("Hãy chọn file bạn muốn tải lên!");
                        return;
                    }
                    else if (!string.IsNullOrEmpty(this.UILocalFile.Text))
                    {
                        FileInfo file = new FileInfo(this.UILocalFile.Text);
                        if (!file.Exists)
                        {
                            MessageBox.Show("Không tìm thấy file!");
                            return;
                        }
                        this.UIFtp.Local = this.UILocalFile.Text;
                        this.UIFtp.FtpUser = App.setting.ftp_user;
                        this.UIFtp.FtpPassword = App.setting.ftp_password;
                        if (this.Media == null)
                            this.UIFtp.Url = string.Format("{0}/{1}/video_{2}.{3}", App.setting.ftp_server, App.setting.ftp_folder, DateTime.Now.Ticks, file.Extension);
                        else
                        {
                            if (this.Media.LocalFile.Exists)
                                this.Media.LocalFile.Delete();
                            this.UIFtp.Url = this.Media.Url;
                        }
                        this.UIFtp.RunUpLoad();
                    }
                    else if (this.Media != null)
                    {
                        this.Media.Name = this.UINameEdit.Text.Trim();
                        this.Media.Url = this.UIUrlEdit.Text;
                        this.Media.Comment = this.UICommentEdit.Text;
                        this.Media.Save();
                        if (this.CloseEvent != null)
                        {
                            this.CloseEvent(this, this.Media);
                        }
                    }
                    else
                    {
                        if (this.UIFtp.Local == this.UILocalFile.Text)
                        {
                            tmpMedia.Name = this.UINameEdit.Text.Trim();
                            tmpMedia.Comment = this.UICommentEdit.Text;
                            int result = Code.Media.MediaData.Insert(tmpMedia);
                            if (result <= 0)
                            {
                                MessageBox.Show("Không thể kết nối với CSDL!");
                                return;
                            }
                            tmpMedia.ID = result;
                            if (this.CloseEvent != null)
                            {
                                this.CloseEvent(this, tmpMedia);
                            }
                        }
                        else
                        {
                            FileInfo file = new FileInfo(this.UILocalFile.Text);
                            if (!file.Exists)
                            {
                                MessageBox.Show("Không tìm thấy file!");
                                return;
                            }
                            this.UIFtp.Local = this.UILocalFile.Text;
                            this.UIFtp.RunUpLoad();
                        }
                    }

                    #endregion
                }
                else
                {
                    if (this.Media == null)
                    {
                        Code.Media.MediaData tmpMedia = new Code.Media.MediaData();
                        tmpMedia.Name = this.UINameEdit.Text.Trim();
                        tmpMedia.FileSize = string.Format("{0}kb", 0);
                        tmpMedia.Duration = "00:00:00";
                        tmpMedia.Type = this.Type;
                        tmpMedia.Url = this.UIUrlEdit.Text;
                        tmpMedia.User_ID = App.curUser.ID;
                        tmpMedia.Comment = this.UICommentEdit.Text;
                        int result = Code.Media.MediaData.Insert(tmpMedia);
                        if (result <= 0)
                        {
                            MessageBox.Show("Không thể kết nối với CSDL!");
                            return;
                        }
                        tmpMedia.ID = result;
                        if (this.CloseEvent != null)
                        {
                            this.CloseEvent(this, tmpMedia);
                        }
                    }
                    else
                    {
                        this.Media.Name = this.UINameEdit.Text.Trim();
                        this.Media.Url = this.UIUrlEdit.Text;
                        this.Media.Comment = this.UICommentEdit.Text.Trim();
                        this.Media.Save();
                        if (this.CloseEvent != null)
                        {
                            this.CloseEvent(this, this.Media);
                        }
                    }
                }
            }
        }

        private void UIFtp_CompleteEvent(object sender, string e)
        {
            FileInfo file = new FileInfo(this.UILocalFile.Text);
            if (this.Media != null)
            {
                this.Media.FileSize = string.Format("{0}kb", file.Length / 1000);
                this.Media.Duration = file.toTimeMedia();
                this.Media.Url = e;
                this.Media.Name = this.UINameEdit.Text.Trim();
                this.Media.Comment = this.UICommentEdit.Text.Trim();
                this.Media.Save();
                if (this.CloseEvent != null)
                {
                    this.CloseEvent(this, this.Media);
                }
            }
            else
            {
                tmpMedia = new Code.Media.MediaData();
                tmpMedia.Name = this.UINameEdit.Text.Trim();
                tmpMedia.FileSize = string.Format("{0}kb", file.Length / 1000);
                tmpMedia.Duration = file.toTimeMedia();
                tmpMedia.Type = this.Type;
                tmpMedia.Url = e;
                tmpMedia.User_ID = App.curUser.ID;
                tmpMedia.Comment = this.UICommentEdit.Text;
                int result = Code.Media.MediaData.Insert(tmpMedia);
                if (result <= 0)
                {
                    MessageBox.Show("Không thể kết nối với CSDL!");
                    return;
                }
                tmpMedia.ID = result;
                if (this.CloseEvent != null)
                {
                    this.CloseEvent(this, tmpMedia);
                }
            }

        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadGUI(this.Media);
            this.Focus();
        }
        public void LoadGUI(Code.Media.MediaData m)
        {
            if (m != null)
            {
                this.UITitle.Text = m.Name;
                this.UINameEdit.Text = m.Name;
                this.UIUrlEdit.Text = m.Url;
                this.UICommentEdit.Text = m.Comment;
                this.UILocalFile.Text = "";
                if (m.TypeMedia.Code.ToUpper() == "FILE")
                {
                    this.UIFtp.Visibility = Visibility.Visible;
                    this.UIChooseFile.IsEnabled = true;
                    this.UIUrlEdit.IsEnabled = false;
                }
                else
                {
                    this.UIFtp.Visibility = Visibility.Hidden;
                    this.UIChooseFile.IsEnabled = false;
                    this.UIUrlEdit.IsEnabled = true;
                    this.UINameLb.Text = "Tên Camera:";
                    this.UIUrlLb.Text = "Địa chỉ rtsp:";
                }
            }
            else
            {
                if (this.Type == 1)
                {
                    this.UITitle.Text = "Thêm media mới";
                    this.UIFtp.Visibility = Visibility.Visible;
                    this.UIChooseFile.IsEnabled = true;
                    this.UIUrlEdit.IsEnabled = false;
                }
                else
                {
                    this.UITitle.Text = "Thêm camera mới";
                    this.UIFtp.Visibility = Visibility.Hidden;
                    this.UIChooseFile.IsEnabled = false;
                    this.UIUrlEdit.IsEnabled = true;
                    this.UINameLb.Text = "Tên Camera:";
                    this.UIUrlLb.Text = "Địa chỉ rtsp:";
                }
            }
        }

        private void UIBtnClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.CloseEvent != null)
            {
                this.CloseEvent(this, null);
            }
        }
        private void UIBtnReset_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.LoadGUI(this.Media);
        }

        private void UIRootView_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key== Key.Escape)
            {
                this.UIBtnClose_MouseLeftButtonUp(null, null);
            }
        }
    }
}
