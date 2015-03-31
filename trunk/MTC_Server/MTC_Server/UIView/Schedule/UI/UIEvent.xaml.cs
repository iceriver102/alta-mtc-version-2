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
                this.Content.Text = this.@event.Content;
                this.Width = value.Width;
                this.setLeft(value.Left);
                this.setTop(value.Top);
                this.Height = value.Height;
                this.UIRoot.setZIndex(100);
            }
        }
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
        public UIEvent()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

    }
}
