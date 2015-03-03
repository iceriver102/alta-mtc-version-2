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

namespace MTC_Server.UIView.Media
{
    /// <summary>
    /// Interaction logic for GridMedia.xaml
    /// </summary>
    public partial class GridMedia : UserControl
    {
        private List<Code.Media.MediaData> Datas;
        private int to = 10;
        private int from = 0;
        private int totalMedia = 0;
        public GridMedia()
        {
            InitializeComponent();
        }

        private void UIBtnAddUser_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UIMediaEdit item = new UIMediaEdit();
            item.setLeft(0);
            item.setTop(90);
            item.Width = 1366;
            item.Height = 578;
            item.CloseEvent += Item_CloseEvent;
            this.UIRoot.Children.Add(item);
        }

        private void Item_CloseEvent(object sender, Code.Media.MediaData e)
        {
            if (e != null)
            {
                bool isEdit = false;
                foreach(UIMedia m  in this.list_Box_Item.Items)
                {
                    if(m.Media.ID == e.ID)
                    {
                        m.Media = e;
                        isEdit = true;
                        break;
                    }
                }
                if (!isEdit)
                {
                    this.Datas = App.curUser.LoadMedias(this.from, this.to, out this.totalMedia);
                    this.LoadGUI();
                }
            }
            this.UIRoot.Children.Remove(sender as UIMediaEdit);
        }

        private void UIRootView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Datas=  App.curUser.LoadMedias(from,to,out totalMedia);
            this.LoadGUI();
        }
        public void LoadGUI()
        {
            this.list_Box_Item.Items.Clear();
            foreach(Code.Media.MediaData m in this.Datas)
            {
                UIMedia item = new UIMedia();
                item.Media = m;
                item.Height = 240;
                item.Width = 170;
                item.ViewInfoMediaEvent += Item_ViewInfoMediaEvent;
                this.list_Box_Item.Items.Add(item);
            }
        }

        private void Item_ViewInfoMediaEvent(object sender, Code.Media.MediaData e)
        {
            if (e != null)
            {
                UIMediaEdit item = new UIMediaEdit();
                item.CloseEvent += Item_CloseEvent;
                item.setLeft(0);
                item.setTop(90);
                item.Width = 1366;
                item.Height = 578;
                item.Media = e;
                this.UIRoot.Children.Add(item);

            }
        }

       

        private void UIReload_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.from = 0;
            this.to = 10;
            this.Datas = App.curUser.LoadMedias(from, to, out totalMedia);
            this.LoadGUI();
        }
    }
}
