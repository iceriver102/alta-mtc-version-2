using System;
using System.Collections.Generic;
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
using Alta.Plugin;
using MTC_Server.Code.Schedule;
using Alta.Class;
namespace MTC_Server.UIView.Schedule
{
    public class CloseUIEventData : EventArgs
    {
        public Event Data { get; set; }
    }
    /// <summary>
    /// Interaction logic for UIAddEvent.xaml
    /// </summary>
    public partial class UIAddEvent : UserControl
    {
        public event EventHandler<CloseUIEventData> CloseEvent;
        private Event _parent;
        public Event ParentEvent
        {
            get
            {
                return this._parent;
            }
            set
            {
                this._parent = value;
                if (this._parent != null)
                {
                    this.UIDevice.IsEnabled = false;
                    this.UIDevice.SelectedItem = new DataAutoComplete() { key = this._parent.device_id.ToString(), label = this._parent.Device.IP };
                }
            }
        }
        private Event e = null;
        public Event @Event
        {
            get
            {
                return this.e;
            }
            set
            {
                this.e = value;
            }
        }

        public void LoadUI(Event e)
        {
            if (e != null)
            {
                this.UILoop.IsChecked = e.loop;
                this.UITimeBegin.curDate = e.Begin;
                this.UITimeEnd.curDate = e.End;
                this.UITitle.Text = "Sửa lịch chiếu";
                MTC_Server.Code.User.UserData tmpUser = e.User;
                this.UIUser.SelectedItem = new DataAutoComplete() { key = tmpUser.ID.ToString(), label = tmpUser.Full_Name };
                MTC_Server.Code.Device.DeviceData tmpDevice = e.Device;
                this.UIDevice.SelectedItem = new DataAutoComplete() { key = tmpDevice.ID.ToString(), label = tmpDevice.IP };
                this.UIComment.Text = e.Content;
            }
            else
            {
                this.UILoop.IsChecked = false;
                this.UITitle.Text = "Thêm lịch chiếu";
                this.UIComment.Text = string.Empty;
                this.UIUser.SelectedItem = null;
                if (this.ParentEvent == null)
                    this.UIDevice.SelectedItem = null;
                this.UITimeBegin.curDate = DateTime.Now;
                this.UITimeEnd.curDate = DateTime.Now.AddMinutes(1);
            }
        }

        public UIAddEvent()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.CloseEvent != null)
            {
                this.CloseEvent(this, new CloseUIEventData());
            }
        }
        public List<DataAutoComplete> SearchUser(string key)
        {
            List<DataAutoComplete> result = new List<DataAutoComplete>();
            int total;
            result = ConvertToDataComplete(Code.User.UserData.SearchUser(key, 0, 10, out total));
            return result;
        }

        public List<DataAutoComplete> SearchDevice(string key)
        {
            List<DataAutoComplete> result = new List<DataAutoComplete>();
            int total;
            int to = 10;
            result = ConvertToDataComplete(App.curUser.FindDevices(key, 0, ref to, out total));
            return result;
        }
        private List<DataAutoComplete> ConvertToDataComplete(List<Code.Device.DeviceData> listDevice)
        {
            if (listDevice == null)
                return null;
            List<DataAutoComplete> result = new List<DataAutoComplete>();
            foreach (Code.Device.DeviceData d in listDevice)
            {
                result.Add(new DataAutoComplete() { key = d.ID.ToString(), label = d.IP });
            }
            return result;
        }

        private List<DataAutoComplete> ConvertToDataComplete(List<Code.User.UserData> listUser)
        {
            if (listUser == null)
                return null;
            List<DataAutoComplete> result = new List<DataAutoComplete>();
            foreach (Code.User.UserData u in listUser)
            {
                result.Add(new DataAutoComplete() { key = u.ID.ToString(), label = u.Full_Name });
            }
            return result;
        }

        private void ResetButtonClick(object sender, MouseButtonEventArgs e)
        {
            this.UIContent.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 38, 400);
            UIErr.Animation_Opacity_View_Frame(false, null, 300);
            UIErr.Text = string.Empty;
            this.LoadUI(this.e);
        }

        private void UIRootView_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoadUI(this.e);
            UISaveBtn.IsEnabled = App.curUser.Permision.mana_schedule;
        }

        private void SaveButtonClick(object sender, MouseButtonEventArgs e)
        {
            if(!App.curUser.Permision.mana_schedule)
            {
                this.UIContent.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 67, 500);
                UIErr.Animation_Opacity_View_Frame(true, null, 550);
                UIErr.Text = "* Bạn không có quyền thực hiện chức năng này!";
                return;
            }
            if (this.UITimeEnd <= this.UITimeBegin)
            {
                this.UIContent.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 67, 500);
                UIErr.Animation_Opacity_View_Frame(true, null, 550);
                UIErr.Text = "* Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc";
                return;
            }
            if (this.UIComment.isEmpty())
            {
                this.UIContent.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 67, 500);
                UIErr.Animation_Opacity_View_Frame(true, null, 550);
                UIErr.Text = "* Tên lịch chiếu không được để trống";

                return;
            }
            if (this.UIUser.SelectedItem == null)
            {
                this.UIContent.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 67, 500);
                UIErr.Animation_Opacity_View_Frame(true, null, 550);
                UIErr.Text = "* Hãy nhập tên User bạn muốn đặt lịch";
                return;
            }
            if (this.UIDevice.SelectedItem == null)
            {
                this.UIContent.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 67, 500);
                UIErr.Animation_Opacity_View_Frame(true, null, 550);
                UIErr.Text = "* Hãy nhập IP thiết bị bạn muốn đặt lịch";
                return;
            }

            #region Validate
            Event tmp = new Event();
            tmp.Content = this.UIComment.Text;
            tmp.Begin = UITimeBegin.Time;
            tmp.End = UITimeEnd.Time;
            tmp.device_id = Convert.ToInt32(this.UIDevice.SelectedItem.key);
            tmp.parent_id = (this.ParentEvent == null) ? 0 : this.ParentEvent.Id;
            tmp.user_id = Convert.ToInt32(this.UIUser.SelectedItem.key);
            tmp.loop = this.UILoop.IsChecked.Value;
            if (this.ParentEvent != null)
            {

                if (this.ParentEvent.loop && tmp.loop)
                {
                    if (this.ParentEvent.Begin.TimeOfDay > tmp.Begin.TimeOfDay)
                    {
                        this.UIContent.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 67, 500);
                        UIErr.Animation_Opacity_View_Frame(true, null, 550);
                        UIErr.Text = string.Format("* Thời gian bắt đầu phải lớn hơn {0:HH:mm:ss}", this.ParentEvent.Begin);
                        return;
                    }
                    else if (this.ParentEvent.End.TimeOfDay < tmp.End.TimeOfDay)
                    {
                        this.UIContent.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 67, 500);
                        UIErr.Animation_Opacity_View_Frame(true, null, 550);
                        UIErr.Text = string.Format("* Thời gian kết thúc phải nhỏ hơn {0:HH:mm:ss}", this.ParentEvent.End);
                        return;
                    }
                }
                else if (this.ParentEvent.loop && !tmp.loop)
                {
                    if (tmp.Begin.Date != tmp.End.Date)
                    {
                        this.UIContent.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 67, 500);
                        UIErr.Animation_Opacity_View_Frame(true, null, 550);
                        UIErr.Text = string.Format("* Ngày bắt đầu phải bằng ngày kết thúc");
                        return;
                    }
                    else if (this.ParentEvent.Begin.TimeOfDay > tmp.Begin.TimeOfDay)
                    {
                        this.UIContent.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 67, 500);
                        UIErr.Animation_Opacity_View_Frame(true, null, 550);
                        UIErr.Text = string.Format("* Thời gian bắt đầu phải lớn hơn {0:HH:mm:ss} {1:dd/MM/yyyy}", this.ParentEvent.Begin, tmp.Begin);
                        return;
                    }
                    else if (this.ParentEvent.End.TimeOfDay < tmp.End.TimeOfDay)
                    {
                        this.UIContent.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 67, 500);
                        UIErr.Animation_Opacity_View_Frame(true, null, 550);
                        UIErr.Text = string.Format("* Thời gian kết thúc phải nhỏ hơn {0:HH:mm:ss}  {1:dd/MM/yyyy}", this.ParentEvent.End, tmp.Begin);
                        return;
                    }
                }
                else if (!this.ParentEvent.loop && tmp.loop)
                {
                    if (this.ParentEvent.Begin.Date == this.ParentEvent.End.Date && this.ParentEvent.Begin.TimeOfDay > tmp.Begin.TimeOfDay)
                    {
                        this.UIContent.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 67, 500);
                        UIErr.Animation_Opacity_View_Frame(true, null, 550);
                        UIErr.Text = string.Format("* Thời gian bắt đầu phải lớn hơn {0:HH:mm:ss}", this.ParentEvent.Begin);
                        return;
                    }
                    else if (this.ParentEvent.Begin.Date == this.ParentEvent.End.Date && this.ParentEvent.End.TimeOfDay < tmp.End.TimeOfDay)
                    {
                        this.UIContent.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 67, 500);
                        UIErr.Animation_Opacity_View_Frame(true, null, 550);
                        UIErr.Text = string.Format("* Thời gian kết thúc phải nhỏ hơn {0:HH:mm:ss}", this.ParentEvent.End);
                        return;
                    }
                }
                else
                {
                    if (this.ParentEvent.Begin > tmp.Begin)
                    {
                        this.UIContent.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 67, 500);
                        UIErr.Animation_Opacity_View_Frame(true, null, 550);
                        UIErr.Text = string.Format("* Thời gian bắt đầu phải lớn hơn {0:HH:mm:ss dd/MM/yyyy}", this.ParentEvent.Begin);
                        return;
                    }
                    else if (this.ParentEvent.End < tmp.End)
                    {
                        this.UIContent.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 67, 500);
                        UIErr.Animation_Opacity_View_Frame(true, null, 550);
                        UIErr.Text = string.Format("* Thời gian kết thúc phải nhỏ hơn {0:HH:mm:ss dd/MM/yyyy}", this.ParentEvent.End);
                        return;
                    }
                }
            }
            this.UIContent.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 38, 400);
            UIErr.Animation_Opacity_View_Frame(false, null, 300);
            UIErr.Text = string.Empty;
            #endregion

            if (this.e == null)
            {
                #region Insert Event
                int result = Event.Insert(tmp);
                if (result != 1)
                {
                    this.UIContent.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 67, 500);
                    UIErr.Animation_Opacity_View_Frame(true, null, 550);
                    UIErr.Text = string.Format("* Không thể thêm lịch phát vào khoảng thời gian này.");
                    return;

                }
                else
                {
                    if (this.CloseEvent != null)
                    {
                        this.CloseEvent(this, new CloseUIEventData() { Data = tmp });
                    }
                }
                #endregion  Insert Event
            }
            else
            {
                #region Edit Event
                tmp.Id = this.e.Id;
                tmp.parent_id = this.e.parent_id;
                int result = tmp.Save();
                if (result != 1)
                {
                    this.UIContent.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 67, 500);
                    UIErr.Animation_Opacity_View_Frame(true, null, 550);
                    UIErr.Text = string.Format("* Không thể thêm lịch phát vào khoảng thời gian này.");
                    return;
                }
                else
                {
                    if (this.CloseEvent != null)
                    {
                        this.CloseEvent(this, new CloseUIEventData() { Data = tmp });
                    }
                }
                #endregion  Edit Event
            }

        }
    }
}
