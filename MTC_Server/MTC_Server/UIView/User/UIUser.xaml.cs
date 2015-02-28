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
using MTC_Server.Code.User;
using Alta.Class;

namespace MTC_Server.UIView.User
{
    /// <summary>
    /// Interaction logic for UIUser.xaml
    /// </summary>
    public partial class UIUser : UserControl
    {
        public event EventHandler<UserData> ViewInfouserEvent;

        private UserData _u;
        public UserData User
        {
            get
            {
                return this._u;
            }
            set
            {
                this._u = value;
                this.UIFullName.Text = string.Format("{0} ({1})", this._u.Full_Name, this._u.User_Name);               
                if (this._u.TypeUser != null)
                    this.UIIcon.Text = this._u.TypeUser.Icon.DecodeEncodedNonAsciiCharacters();
                this.UIPhone.Text = this._u.Phone;
                this.UIEmail.Text = this._u.Email;
                this.UIType.Text = this._u.TypeUser.Name;
                this.UIDate.Text = this._u.Time.format();
                if (this._u.Status)
                {
                    this.UIStatus.Text = Define.Fonts["fa-unlock"].asCode;
                }
                else
                {
                    this.UIStatus.Text = Define.Fonts["fa-lock"].asCode;
                }
            }
        }
        private bool _select = false;
        public bool isSelect { get { return this._select; } set { this._select = value; } }

        public UIUser()
        {
            InitializeComponent();
        }

        private void UIRootView_MouseEnter(object sender, MouseEventArgs e)
        {
            this.UIBar.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 200);
        }

        private void UIRootView_MouseLeave(object sender, MouseEventArgs e)
        {
            this.UIBar.Animation_Translate_Frame(double.NaN, double.NaN, double.NaN, 220);
        }

        private void UIRootView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.isSelect = true;
        }

        private void UIStatus_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (App.curUser.Permision.mana_user)
            {
                this._u.Status = !this._u.Status;
                if (this._u.Status)
                {
                    this.UIStatus.Text = Define.Fonts["fa-unlock"].asCode;
                }
                else
                {
                    this.UIStatus.Text = Define.Fonts["fa-lock"].asCode;
                }
                this._u.UpdateStatus();
            }
        }

        private void ViewInfoUser(object sender, MouseButtonEventArgs e)
        {
            if (this.ViewInfouserEvent != null)
            {
                this.ViewInfouserEvent(this, this.User);
            }
        }
    }
}
