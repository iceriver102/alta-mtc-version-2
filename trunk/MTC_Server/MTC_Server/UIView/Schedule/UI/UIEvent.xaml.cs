using MTC_Server.Code.Schedule;
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
using Alta.Class;

namespace MTC_Server.UIView.Schedule.UI
{
    
    /// <summary>
    /// Interaction logic for UIEvent.xaml
    /// </summary>
    public partial class UIEvent : UserControl
    {
        public event EventHandler<Event> AddChildEvent;
        public event EventHandler<Event> EditEvent;
        public event EventHandler<Event> UnLinkEvent;
        public event EventHandler<Event> LinkEvent;
        public event EventHandler<Event> SelectEvent;
        private Event @event;        
        public Event Event
        {
            get
            {
                return this.@event;
            }
            set
            {
                this.@event = value;
                if (value != null)
                {
                    this.Content.Text = this.@event.Content;
                    this.Width = value.Width;
                    this.setLeft(value.Left);
                    this.setTop(value.Top);
                    this.Height = value.Height;
                    this.UIRoot.setZIndex(100);
                    this.T_Title.Text = value.Content;
                    if (value.loop)
                    {
                        this.T_Date.Text = string.Format("{0:HH:mm:ss} - {1:HH:mm:ss}", value.Begin, value.End);
                        this.UITime.Text = string.Format("{0:HH:mm:ss} - {1:HH:mm:ss}", value.Begin, value.End);
                    }
                    else
                    {
                        this.T_Date.Text = string.Format("{0:dd/MM/yyyy HH:mm:ss} - {1:dd/MM/yyyy HH:mm:ss}", value.Begin, value.End);
                        this.UITime.Text = string.Format("{0:dd/MM/yyyy HH:mm:ss} - {1:dd/MM/yyyy HH:mm:ss}", value.Begin, value.End);
                    }
                    this.UIUser.Text = value.NameUser;
                    this.T_User.Text = value.NameUser;
                    this.UINameDevice.Text = value.NameDevice;
                    this.T_NameDevice.Text = value.NameDevice;

                    if (value.parent_id == 0)
                    {
                        this.T_Parent.Text = "Root";
                    }
                    else
                    {
                        this.T_Parent.Text = value.NameParent;
                    }
                    if (value.parent_id == 0)
                    {
                        UIDelete.Visibility = System.Windows.Visibility.Hidden;
                        UIEdit.Visibility = System.Windows.Visibility.Hidden;
                        UILink.Visibility = System.Windows.Visibility.Hidden;
                    }
                    else
                    {
                        UIDelete.Visibility = System.Windows.Visibility.Visible;
                        UIEdit.Visibility = System.Windows.Visibility.Visible;
                        UILink.Visibility = System.Windows.Visibility.Visible;
                    }
                }
            }
        }

        public event EventHandler DeleteEvent;
       
        public Brush Fill
        {
            get
            {
                return this.UIRoot.Background;
            }
            set
            {
                this.UIRoot.Background = value;
               
            }
        }
        public Orientation Orientation
        {
            get
            {
                return this.UIMenuButton.Orientation;
            }
            set
            {
                this.UIMenuButton.Orientation = value;
            }
        }
        public UIEvent()
        {
            InitializeComponent();
        }

        public void delete()
        {
            if (this.Event != null && App.curUser.Permision.mana_schedule)
            {
                Event.delete(this.Event);
                if (DeleteEvent != null)
                {
                    DeleteEvent(this, new EventArgs());
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.UIAdd.IsEnabled = (this.Event.user_id == App.curUser.ID || (App.curUser.Permision.mana_schedule && this.Event.parent_id==0));
            if(App.curUser.Permision.mana_schedule)
                this.UIMenuAction.Visibility = System.Windows.Visibility.Visible;
        }

        private void UIRootView_MouseEnter(object sender, MouseEventArgs e)
        {
            
                UIMenuButton.Visibility = System.Windows.Visibility.Visible;
                UIMenuButton.Animation_Opacity_View_Frame(true);
            
        }

        private void UIRootView_MouseLeave(object sender, MouseEventArgs e)
        {
            UIMenuButton.Animation_Opacity_View_Frame(false, () => { UIMenuButton.Visibility = System.Windows.Visibility.Collapsed; });
        }

        private void UIDelete_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Bạn có muốn xóa lịch phát này không?","Thông Báo", MessageBoxButton.YesNo)== MessageBoxResult.Yes)
            {
                this.delete();
            }
        }

        private void UIEdit_Click(object sender, RoutedEventArgs e)
        {
            if (EditEvent != null && App.curUser.Permision.mana_schedule)
            {
                EditEvent(this, this.@event);
            }
        }

        private void UIAdd_Click(object sender, RoutedEventArgs e)
        {
            if (AddChildEvent != null && App.curUser.Permision.mana_schedule)
            {
                this.AddChildEvent(this, this.@event);
            }
        }

        private void UILink_Click(object sender, RoutedEventArgs e)
        {
            if (App.curUser.ID == this.Event.user_id)
            {
                if (this.LinkEvent != null)
                {
                    this.LinkEvent(this, this.Event);
                }
            }
        }

        private void UIRootView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (SelectEvent != null)
            {
                this.SelectEvent(this, this.@event);
            }
        }

    }
}
