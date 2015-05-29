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
namespace Alta.Plugin
{
    public class DataAutoComplete
    {
        public string key { get; set; }
        public string label { get; set; }
        public string icon { get; set; }
    }
    /// <summary>
    /// Interaction logic for AutoComplete.xaml
    /// </summary>
    public partial class AutoComplete : UserControl
    {
        private string _icon = string.Empty;
        public string Icon
        {
            get
            {
                return this._icon;
            }
            set
            {
                this._icon = value;
            }
        }

        public bool isEnabled
        {
            get
            {
                return this.UISearch.IsEnabled;
            }
            set
            {
                this.UISearch.IsEnabled = value;
            }
        }
        public DataAutoComplete _select;
        public DataAutoComplete SelectedItem
        {
            get
            {
                return this._select;
            }
            set
            {
                this._select = value;
                this.UISearch.TextChanged -= AutoSearchFuntion;
                if (value != null)
                {
                    this.UISearch.Text = value.label;
                }
                else
                {
                    this.UISearch.empty();
                }
                this.UISearch.TextChanged += AutoSearchFuntion;
            }
        }
        public Thickness PaddingInside
        {
            get
            {
                return this.UISearch.Padding;
            }
            set
            {
                this.UISearch.Padding = value;
            }
        }
        public string Text
        {
            get
            {
                return this.UISearch.Text;
            }
            set
            {
                this.UISearch.Text = value;
            }
        }

        private int _min = 3;
        public int minLength
        {
            get
            {
                return this._min;
            }
            set
            {
                this._min = value;
            }
        }

        public delegate List<DataAutoComplete> SearchFunction(string key_search);
        public SearchFunction SearchAction
        {
            get;
            set;
        }

        public AutoComplete()
        {
            InitializeComponent();
            this.UISearch.TextChanged += AutoSearchFuntion;
            this.lbSuggestion.SelectionChanged += lbSuggestion_SelectionChanged;
            this.LostFocus += RootView_LostFocus;
        }

        private void AutoSearchFuntion(object sender, TextChangedEventArgs e)
        {
            string typedString = this.UISearch.Text;
            this._select = null;
            if (typedString.Length > _min)
            {
                List<DataAutoComplete> autoList = new List<DataAutoComplete>();
                autoList = SearchAction(this.UISearch.Text);
                if (autoList != null && autoList.Count > 0)
                {
                    if (!string.IsNullOrEmpty(this._icon))
                    {
                        foreach (DataAutoComplete item in autoList)
                        {
                            if (string.IsNullOrEmpty(item.icon))
                                item.icon = this._icon;
                        }
                    }
                    lbSuggestion.ItemsSource = autoList;
                    lbSuggestion.Visibility = Visibility.Visible;
                }
                else
                {
                    lbSuggestion.ItemsSource = null;
                    lbSuggestion.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                lbSuggestion.ItemsSource = null;
                lbSuggestion.Visibility = Visibility.Collapsed;
            }
        }

        private void lbSuggestion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataAutoComplete data = lbSuggestion.SelectedItem as DataAutoComplete;
            this.SelectedItem = data;
            lbSuggestion.Visibility = Visibility.Collapsed;
        }

        private void RootView_Loaded(object sender, RoutedEventArgs e)
        {
            this.lbSuggestion.setTop(this.Height);
        }

        private void RootView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                if (lbSuggestion.Visibility == System.Windows.Visibility.Visible)
                {
                    this.LostFocus -= RootView_LostFocus;
                    if (this.UISearch.IsFocused)
                    {
                        ListBoxItem container = lbSuggestion.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
                        container.Focus();
                    }
                    this.LostFocus += RootView_LostFocus;
                }
            }
            else if (e.Key == Key.Enter)
            {
                if (lbSuggestion.Visibility == System.Windows.Visibility.Visible)
                {
                    foreach (var item in lbSuggestion.Items)
                    {
                        ListBoxItem container = lbSuggestion.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                        if (container.IsFocused)
                        {
                            this.lbSuggestion.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
        }

        private void RootView_LostFocus(object sender, RoutedEventArgs e)
        {
            this.lbSuggestion.Visibility = Visibility.Collapsed;
        }
    }
}
