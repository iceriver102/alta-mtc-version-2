﻿using System;
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
        public int from=0;
        public GridViewUser()
        {
            InitializeComponent();
        }

        private void UIRootView_Loaded(object sender, RoutedEventArgs e)
        {
            Datas = App.curUser.getListUser(from,to,out totalUser);
            this.Focus();
            LoadGUI();
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
                    this.list_Box_Item.Items.Add(item);
                }
            }
            if (to < totalUser)
            {
                this.UIRightBtn.Foreground = Brushes.DarkOrange;
            }else
            {
                this.UIRightBtn.Foreground = Brushes.Gray;
            }
            if (this.from > 0)
            {
                this.UILeftBtn.Foreground = Brushes.DarkOrange; 
            }else
            {
                this.UILeftBtn.Foreground = Brushes.Gray; 
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
                foreach(UIUser u in this.list_Box_Item.Items)
                {
                    if(u.User.ID == e.ID)
                    {
                        this.list_Box_Item.Items.Remove(u);
                        this.Datas.Remove(u.User);
                        return;
                    }
                }
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
                foreach (UIUser u in this.list_Box_Item.Items)
                {
                    if (u.User.ID == e.ID)
                    {
                        this.list_Box_Item.Items.Remove(u);
                        this.Datas.Remove(u.User);
                        break;
                    }
                }
            }
            this.UIRoot.Children.Remove(sender as UIUserEdit);
        }

        private void Item_CloseEvent(object sender, UserData e)
        {
            if (e != null)
            {
                foreach(UIUser item in this.list_Box_Item.Items)
                {
                    if(item.User.ID== e.ID)
                    {
                        item.User = e;
                        break;
                    }
                    
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
                this.Datas = App.curUser.getListUser(this.from, this.to, out this.totalUser);
                LoadGUI();
            }
        }

        private void UIRightBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(this.to< this.totalUser)
            {
                this.from = this.to;
                this.to += 10;
                this.Datas = App.curUser.getListUser(this.from, this.to, out this.totalUser);
                LoadGUI();
            }
        }

        private void UIRootView_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key== Key.Left)
            {
                UILeftBtn_MouseLeftButtonUp(null, null);
                this.Focus();
            }
            else if(e.Key== Key.Right)
            {
                UIRightBtn_MouseLeftButtonUp(null, null);
                this.Focus();
            }
        }
    }
}
