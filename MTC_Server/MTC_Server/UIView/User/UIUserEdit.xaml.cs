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

namespace MTC_Server.UIView.User
{
    /// <summary>
    /// Interaction logic for UIUserEdit.xaml
    /// </summary>
    public partial class UIUserEdit : UserControl
    {
        public event EventHandler<Code.User.UserData> CloseEvent;
        public event EventHandler<Code.User.UserData> DeleteUserEvent;
        private bool _isPermison = false;
        public bool isPermison
        {
            get
            {
                return this._isPermison;
            }
            set
            {
                this._isPermison = value;
                if(value)
                    this.UILayout.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, -100);
                else
                    this.UILayout.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 0);
            }
        }
        private Code.User.UserData u;
        public Code.User.UserData User
        {
            get
            {
                return this.u;

            }
            set
            {
                this.u = value;
                this.UIName.Text = string.Format("{0} ({1})", this.u.Full_Name, this.u.User_Name);
                this.UIFullnameEdit.Text = this.u.Full_Name;
                this.UIEmailEdit.Text = this.u.Email;
                this.UIPhoneEdit.Text = this.u.Phone;
                this.viewPermision.Permision = this.u.Permision;
                foreach(Code.User.UserTypeData type in this.UITypeUser.Items)
                {
                    if(type== this.u.TypeUser)
                    {
                        this.UITypeUser.SelectedItem = type;
                    }
                }
            }
        }
        public UIUserEdit()
        {
            InitializeComponent();
            foreach(Code.User.UserTypeData type in App.TypeUsers)
            {
                UITypeUser.Items.Add(type);
            }
          
        }

        private void CloseDialog(object sender, MouseButtonEventArgs e)
        {
            if (this.CloseEvent != null)
            {
                this.CloseEvent(this, null);
            }
        }

        private void RefillEvent(object sender, MouseButtonEventArgs e)
        {
            this.UIFullnameEdit.Text = this.u.Full_Name;
            this.UIEmailEdit.Text = this.u.Email;
            this.UIPhoneEdit.Text = this.u.Phone;
            this.UIOldPaswordEdit.Password = "";
            this.UIPaswordConfirmEdit.Password = "";
            this.UINewPaswordEdit.Password = "";
            foreach (Code.User.UserTypeData type in this.UITypeUser.Items)
            {
                if (type == this.u.TypeUser)
                {
                    this.UITypeUser.SelectedItem = type;
                }
            }
        }

        private void SaveUser(object sender, MouseButtonEventArgs e)
        {
            string oldPass = this.UIOldPaswordEdit.Password.MD5String();
            if(this.u.Pass!= oldPass)
            {
                MessageBox.Show("Mật khẩu không đúng vui lòng kiểm tra lại!");
                return;
            }
            string newPass = this.UINewPaswordEdit.Password;
            string confirmPass = this.UIPaswordConfirmEdit.Password;
            if (!string.IsNullOrEmpty(newPass) && newPass != confirmPass)
            {
                MessageBox.Show("Mật khẩu không khớp nhau vui lòng kiểm tra lại!");
                return;
            }
            if (string.IsNullOrEmpty(newPass))
            {
                newPass = oldPass;
            }else
            {
                newPass = newPass.MD5String();
            }
            this.u.setDirectpass(newPass);
            this.u.Full_Name = this.UIFullnameEdit.Text;
            this.u.Email = this.UIEmailEdit.Text;
            this.u.Phone = this.UIPhoneEdit.Text;
            this.u.Type = (this.UITypeUser.SelectedItem as Code.User.UserTypeData).Id;
            this.u.Permision = this.viewPermision.Permision;
            this.u.Save();
            if (this.CloseEvent != null)
                this.CloseEvent(this, this.u);
        }

        private void SubmitForm(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.SaveUser(null, null);
            }
        }

        private void ViewPermision(object sender, MouseButtonEventArgs e)
        {
            this.isPermison = !this.isPermison;
        }

        private void DeleteUser(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Bạn có muốn xoá tài khoản này không?", "Xoá tài khoản", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Code.User.UserData.deleteUser(this.User);
                if (this.DeleteUserEvent != null)
                {
                    this.DeleteUserEvent(this, this.u);
                }
            }
        }

        private void UIRootView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
        }

        private void UIRootView_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key== Key.Escape)
            {
                this.CloseDialog(null, null);
            }
        }

        private void ResetPassword(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Bạn có muốn tạo mật khẩu tự động cho tài khoản này không?", "Tạo mật khẩu", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                string Pass = StringExtensions.RandomString(6);
                this.u.Pass = Pass;
                this.u.Save();
                MessageBox.Show(string.Format("Mật khẩu: {0}",Pass),"Tạo mật khẩu tự động");
            }
        }
    }
}
