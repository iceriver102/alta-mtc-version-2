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
using MTC_Server.Code;
using Alta.Class;

namespace MTC_Server.UIView.User
{
    /// <summary>
    /// Interaction logic for UIPermision.xaml
    /// </summary>
    public partial class UIPermision : UserControl
    {
        private Permision p;
        public Permision Permision
        {
            get
            {
                return this.p;
            }
            set
            {
                this.p = value;
                this.LoadGUI(value);
            }
        }   
        public UIPermision()
        {
            InitializeComponent();
        }
        private void LoadGUI(Permision p)
        {
            this.UIConfirmMedia.IsChecked = p.confirm_media;
            this.UIManaUser.IsChecked = p.mana_user;
            this.UISchedule.IsChecked = p.mana_schedule;
            this.UIViewAllMedia.IsChecked = p.view_all_media;
            this.UIDEvice.IsChecked = p.mana_device;
        }

        private void UIManaUser_Checked(object sender, RoutedEventArgs e)
        {
            this.Permision.mana_user = true;
        }

        private void UIManaUser_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Permision.mana_user = false;
        }

        private void UIViewAllMedia_Checked(object sender, RoutedEventArgs e)
        {
            this.Permision.view_all_media = true;
        }

        private void UIViewAllMedia_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Permision.view_all_media = false;
        }

        private void UIConfirmMedia_Checked(object sender, RoutedEventArgs e)
        {
            this.Permision.confirm_media = true;
        }

        private void UIConfirmMedia_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Permision.confirm_media = false;
        }

        private void UISchedule_Checked(object sender, RoutedEventArgs e)
        {
            this.Permision.mana_schedule = true;
        }

        private void UISchedule_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Permision.mana_schedule = false;
        }

        private void UIDEvice_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Permision.mana_device = false;
        }

        private void UIDEvice_Checked(object sender, RoutedEventArgs e)
        {
            this.Permision.mana_device = true;
        }
    }
}
