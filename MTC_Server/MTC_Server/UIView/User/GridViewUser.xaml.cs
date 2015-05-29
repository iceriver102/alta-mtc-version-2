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
using MTC_Server.Code.User;
using Alta.Class;

namespace MTC_Server.UIView.User
{
    /// <summary>
    /// Interaction logic for GridViewUser.xaml
    /// </summary>
    public partial class GridViewUser : UserControl
    {
        List<UserData> Datas;
        public int totalUser;
        public int to = 10;
        public int from = 0;
        public event EventHandler<UserData> BrowserMediaEvent;
        public event EventHandler<UserData> BrowserCameraEvent;
        public string key
        {
            get
            {
                if (string.IsNullOrEmpty(this.UISearchEdit.Text))
                {
                    return string.Empty;
                }
                return string.Format("%{0}%", this.UISearchEdit.Text.Trim());
            }
        }
        public GridViewUser()
        {
            InitializeComponent();
        }

        private void UIRootView_Loaded(object sender, RoutedEventArgs e)
        {
            Datas = App.curUser.getListUser(from, to, out totalUser);
            this.Focus();
            LoadGUI();
            UIBtnAddUser.IsEnabled = App.curUser.Permision.mana_user;
        }
        private void LoadGUI()
        {
            this.list_Box_Item.Items.Clear();
            if (this.Datas != null)
            {
                foreach (UserData data in this.Datas)
                {
                    UIUser item = new UIUser();
                    item.User = data;
                    item.Height = 270;
                    item.Width = 200;
                    item.ViewInfoUserEvent += Item_ViewInfouserEvent;
                    item.DeleteUserEvent += Item_DeleteUserEvent;
                    item.ViewPermison += Item_ViewPermison;
                    item.BrowserMediaEvent += item_BroswerMediaEvent;
                    item.BrowserCameraEvent += item_BrowserCameraEvent;
                    this.list_Box_Item.Items.Add(item);
                }
            }
            if (to < totalUser)
            {
                this.UIRightBtn.Foreground = Brushes.DarkOrange;
            }
            else
            {
                this.UIRightBtn.Foreground = Brushes.Gray;
            }
            if (this.from > 0)
            {
                this.UILeftBtn.Foreground = Brushes.DarkOrange;
            }
            else
            {
                this.UILeftBtn.Foreground = Brushes.Gray;
            }
        }

        void item_BrowserCameraEvent(object sender, UserData e)
        {
            if (e != null && BrowserCameraEvent != null)
            {
                BrowserCameraEvent(this, e);
            }
        }

        void item_BroswerMediaEvent(object sender, UserData e)
        {
            if (e != null && BrowserMediaEvent!=null)
            {
                BrowserMediaEvent(this, e);
            }
        }

        private void Item_ViewPermison(object sender, UserData e)
        {
            if (e != null)
            {
                UIUserEdit item = new UIUserEdit();
                item.setLeft(0);
                item.setTop(90);
                item.Width = 1366;
                item.Height = 578;
                item.User = e;
                item.isPermison = true;
                item.CloseEvent += Item_CloseEvent;
                item.DeleteUserEvent += Item_DeleteUserEvent1;
               
                this.UIRoot.Children.Add(item);
            }
        }
        private void Item_DeleteUserEvent(object sender, UserData e)
        {
            if (e != null)
            {
                UIUser uiU = this.list_Box_Item.SelectedItem as UIUser;
                this.list_Box_Item.SelectedIndex = -1;
                uiU.Animation_Opacity_View_Frame(false, () =>
                {
                    this.list_Box_Item.Items.Remove(uiU);
                    this.Datas.Remove(uiU.User);
                    if (this.to < this.totalUser)
                    {
                        this.Datas = App.curUser.getListUser(from, to, out totalUser);
                        LoadGUI();
                    }
                });
            }
        }

        private void Item_ViewInfouserEvent(object sender, UserData e)
        {
            if (e != null)
            {
                UIUserEdit item = new UIUserEdit();
                item.setLeft(0);
                item.setTop(90);
                item.Width = 1366;
                item.Height = 578;
                item.User = e;
                item.CloseEvent += Item_CloseEvent;
                item.DeleteUserEvent += Item_DeleteUserEvent1;
              
                this.UIRoot.Children.Add(item);
            }
        }

        private void Item_DeleteUserEvent1(object sender, UserData e)
        {
            if (e != null)
            {
                UIUser uiU = this.list_Box_Item.SelectedItem as UIUser;
                this.list_Box_Item.SelectedIndex = -1;
                uiU.Animation_Opacity_View_Frame(false, () =>
                {
                    this.list_Box_Item.Items.Remove(uiU);
                    this.Datas.Remove(uiU.User);
                    if (this.to < this.totalUser)
                    {
                        this.Datas = App.curUser.getListUser(from, to, out totalUser);
                        LoadGUI();
                    }
                });

            }
            this.UIRoot.Children.Remove(sender as UIUserEdit);
        }

        private void Item_CloseEvent(object sender, UserData e)
        {
            if (e != null)
            {
                bool isEditUser = false;
                foreach (UIUser item in this.list_Box_Item.Items)
                {
                    if (item.User.ID == e.ID)
                    {
                        item.User = e;
                        isEditUser = true;
                        break;
                    }
                }
                if (!isEditUser)
                {
                    this.Datas = App.curUser.getListUser(this.from, this.to, out this.totalUser);
                    LoadGUI();
                }
            }
            this.UIRoot.Children.Remove(sender as UIUserEdit);
        }

        private void UILeftBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.from > 0)
            {
                this.from -= 10;
                if (this.from < 0)
                    this.from = 0;
                this.to -= 10;
                if (string.IsNullOrEmpty(this.key))
                    this.Datas = App.curUser.getListUser(this.from, this.to, out this.totalUser);
                else
                    this.Datas = UserData.SearchUser(this.key, this.from, this.to, out this.totalUser);

                LoadGUI();
            }
        }

        private void UIRightBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.to < this.totalUser)
            {
                this.from = this.to;
                this.to += 10;
                if (string.IsNullOrEmpty(this.key))
                    this.Datas = App.curUser.getListUser(this.from, this.to, out this.totalUser);
                else
                    this.Datas = UserData.SearchUser(this.key, this.from, this.to, out this.totalUser);

                LoadGUI();
            }
        }

        private void UIRootView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                UILeftBtn_MouseLeftButtonUp(null, null);
                this.Focus();
            }
            else if (e.Key == Key.Right)
            {
                UIRightBtn_MouseLeftButtonUp(null, null);
                this.Focus();
            }
            e.Handled = true;
        }

        private void UISearchEdit_GotFocus(object sender, RoutedEventArgs e)
        {
            this.UIOverText.Animation_Opacity_View_Frame(false);
        }

        private void UISearchEdit_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && App.curUser.Permision.mana_user)
            {
                string key = this.UISearchEdit.Text.Trim();
                if (key.Length > 3)
                {
                    to = 10;
                    from = 0;
                    this.Datas = UserData.SearchUser(this.key, from, to, out this.totalUser);
                    this.LoadGUI();
                }
            }
            else if (e.Key == Key.Escape)
            {
                this.UISearchEdit.Text = "";
                if (!string.IsNullOrEmpty(this.key))
                {
                    this.to = 10;
                    this.from = 0;
                    this.Datas = App.curUser.getListUser(this.from, this.to, out this.totalUser);
                    this.LoadGUI();
                }
                this.Focus();
            }
        }

        private void UISearchEdit_LostFocus(object sender, RoutedEventArgs e)
        {
            this.UIOverText.Animation_Opacity_View_Frame(true);
        }

        private void UIReload_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.UISearchEdit.Text = "";
            this.Focus();
            this.to = 10;
            this.from = 0;
            this.Datas = App.curUser.getListUser(this.from, this.to, out this.totalUser);
            this.LoadGUI();
        }

        private void UIBtnAddUser_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UIUserEdit item = new UIUserEdit();
            item.setLeft(0);
            item.setTop(90);
            item.Width = 1366;
            item.Height = 578;
            item.CloseEvent += Item_CloseEvent;
           
            this.UIRoot.Children.Add(item);
        }
    }
}
