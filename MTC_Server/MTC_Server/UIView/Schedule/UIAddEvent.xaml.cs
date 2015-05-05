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
        
    }
    /// <summary>
    /// Interaction logic for UIAddEvent.xaml
    /// </summary>
    public partial class UIAddEvent : UserControl
    {
        public event EventHandler<CloseUIEventData> CloseEvent;
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
                this.UITimeBegin.Text = e.Begin.format("HH:mm:ss dd/MM/yyyy");
                this.UITimeBegin.curDate = e.Begin;
                this.UITimeEnd.Text = e.End.format("HH:mm:ss dd/MM/yyyy");
                this.UITimeEnd.curDate = e.End;
                this.UITitle.Text = "Sửa lịch chiếu";
                MTC_Server.Code.User.UserData tmpUser = e.User;
                this.UIUser.Text = tmpUser.Full_Name;
                this.UIUser.SelectedItem = new DataAutoComplete() { key = tmpUser.ID.ToString(), label = tmpUser.Full_Name };
                MTC_Server.Code.Device.DeviceData tmpDevice = e.Device;
                this.UIDevice.Text = tmpDevice.IP;
                this.UIDevice.SelectedItem = new DataAutoComplete() { key =tmpDevice.ID.ToString(), label = tmpDevice.IP };
                this.UIComment.Text = string.Empty;
            }
            else
            {
                this.UILoop.IsChecked = false;
                this.UITitle.Text = "Thêm lịch chiếu";
                this.UIUser.Text = string.Empty;
                this.UIDevice.Text = string.Empty;
                this.UIComment.Text = string.Empty;
                this.UIUser.SelectedItem = null;
                this.UIDevice.SelectedItem = null;
                this.UITimeBegin.Text = DateTime.Now.format("HH:mm:ss dd/MM/yyyy");
                this.UITimeEnd.Text = DateTime.Now.AddMinutes(1).format("HH:mm:ss dd/MM/yyyy");
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
            result = ConvertToDataComplete(App.curUser.FindDevices(key,0,ref to,out total));
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
            this.LoadUI(this.e);
        }

        private void UIRootView_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoadUI(this.e);
        }

        private void SaveButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (this.e == null)
            {

                if (this.UITimeEnd <= this.UITimeBegin)
                {
                    UIErr.Text = "* Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc";
                    return;
                }
                if (this.UIComment.isEmpty())
                {
                    UIErr.Text = "* Tên lịch chiếu không được để trống";
                    return;
                }
                if (this.UIUser.SelectedItem == null)
                {
                    UIErr.Text = "* Hãy nhập tên User bạn muốn đặt lịch";
                    return;
                }
                if (this.UIDevice.SelectedItem == null)
                {
                    UIErr.Text = "* Hãy IP thiết bị bạn muốn đặt lịch";
                    return;
                }
                UIErr.Text = string.Empty;
                this.e = new Event();
                this.e.Content = this.UIComment.Text;
                this.e.Begin = UITimeBegin.Time;
                this.e.End = UITimeEnd.Time;
                this.e.device_id = Convert.ToInt32(this.UIDevice.SelectedItem.key);
                this.e.parent_id = App.curUser.ID;
                this.e.user_id = Convert.ToInt32(this.UIUser.SelectedItem.key);
            }
            
        }
    }
}
