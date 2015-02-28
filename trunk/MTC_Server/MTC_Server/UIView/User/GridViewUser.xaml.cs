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
        public GridViewUser()
        {
            InitializeComponent();
        }

        private void UIRootView_Loaded(object sender, RoutedEventArgs e)
        {
            Datas = App.curUser.getListUser();
            LoadGUI();
        }
        private void LoadGUI()
        {
            foreach (UserData data in this.Datas)
            {
                UIUser item = new UIUser();
                item.User = data;
                item.Height = 230;
                item.Width = 180;
                item.ViewInfouserEvent += Item_ViewInfouserEvent;
                this.list_Box_Item.Items.Add(item);    
            }
        }

        private void Item_ViewInfouserEvent(object sender, UserData e)
        {
            if (e != null)
            {
                UIUserEdit item = new UIUserEdit();
                item.setLeft(0);
                item.setTop(120);
                item.Width = 1366;
                item.Height = 500;
                item.User = e;
                item.CloseEvent += Item_CloseEvent;
                this.UIRoot.Children.Add(item);
            }
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
    }
}
