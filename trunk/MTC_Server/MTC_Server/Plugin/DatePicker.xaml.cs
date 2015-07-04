﻿using MTC_Server.UIView.Schedule.UI;
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
    public enum ModeView
    {
        Left = 0, Right = 1, Down = 2, Top = 3
    }
    /// <summary>
    /// Interaction logic for DatePicker.xaml
    /// </summary>
    public partial class DatePicker : UserControl
    {
        public string Text
        {
            get
            {
                return this.UIDate.Text;
            }
            set
            {
                this.UIDate.Text = value;
            }
        }
        public Thickness PaddingInside
        {
            get
            {
                return this.UIDate.Padding;
            }
            set
            {
                this.UIDate.Padding = value;
            }
        }
        private ModeView view = ModeView.Down;
        public ModeView View
        {
            get
            {
                return this.view;
            }
            set
            {
                this.view = value;
            }
        }
        public int Month
        {
            get
            {
                if (this._date == null)
                    return 0;
                return this._date.Month;
            }
        }
        private DateTime _cuDate;

        /// <summary>
        /// Thời giang cài đặt hiển thị UI
        /// </summary>
        public DateTime curDate
        {
            get
            {
                return this._cuDate;
            }
            set
            {
                this._cuDate = value;
                this.Date = value;
            }
        }
        private DateTime _date;
        private DateTime Date
        {
            get
            {
                return this._date;
            }
            set
            {
                this._date = value;
                if (this._date == null)
                    return;
                DateTime tmp = new DateTime(value.Year, value.Month, 1);
                this.DayOFWeek = (int)tmp.DayOfWeek;
                this.firstDateOfMonth = tmp.AddDays(-this.DayOFWeek);
                this.UITitle.Text = string.Format("Tháng {0} {1}", this._date.Month, this._date.Year);
                this.tmpDate = this._date.Date;
                this.UITime.Time = this._date.TimeOfDay;
                this.Text = this._date.format("HH:mm:ss dd/MM/yyyy");
                this.LoadGUI();
            }
        }
        /// <summary>
        /// Thời gian lựa chọn
        /// </summary>
        public DateTime Time
        {
            get
            {
                DateTime time = tmpDate;
                return time.setTime(this.UITime.Time);
            }
        }

        private int DayOFWeek;
        private DateTime firstDateOfMonth;
        private DateTime tmpDate;
        public DatePicker()
        {
            InitializeComponent();
            this.curDate = DateTime.Now;
            this.UIDate.LostFocus += DatePicker_LostFocus;
            this.UIDate.GotFocus += DatePicker_GotFocus;
        }

        void DatePicker_GotFocus(object sender, RoutedEventArgs e)
        {
            this.UICalendar.Visibility = System.Windows.Visibility.Visible;
        }

        void DatePicker_LostFocus(object sender, RoutedEventArgs e)
        {
            this.UICalendar.Visibility = Visibility.Hidden;
        }

        private void RootView_Loaded(object sender, RoutedEventArgs e)
        {
          
            this.UICalendar.Visibility = Visibility.Hidden;
            if (this.View == ModeView.Down)
            {
                this.UICalendar.setLeft(this.Width / 2 - 136);
                this.UICalendar.setTop(this.Height);
            }
            else if (this.View == ModeView.Left)
            {
                this.UICalendar.setLeft(-272);
                this.UICalendar.setTop(this.Height / 2 - 207);
            }
            else if (this.View == ModeView.Right)
            {
                this.UICalendar.setLeft(this.Width + 22);
                this.UICalendar.setTop(this.Height / 2 - 307);
            }
            else
            {
                this.UICalendar.setLeft(this.Width);
                this.UICalendar.setTop(-414);
            }
        }

        public void LoadGUI()
        {
            int i = 0;
            foreach (UIElement UIE in this.UIDays.Children)
            {
                if (UIE is UIDay)
                {
                    UIDay UIday = UIE as UIDay;
                    UIday.Date = this.firstDateOfMonth.AddDays(i++);
                    if (UIday.Date.Month != this._date.Month)
                    {
                        UIday.Mode = ModeDay.Disable;
                    }
                    else
                    {
                        UIday.Mode = ModeDay.Enable;
                    }
                    if (UIday.Date.Date == this.curDate.Date)
                    {
                        UIday.isCurDay = true;
                    }
                    else
                    {
                        UIday.isCurDay = false;
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.curDate = DateTime.Now;
            tmpDate = DateTime.Now.Date;
        }

        private void Day_1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.UIDate.LostFocus -= DatePicker_LostFocus;
        }

        private void Day_SelectDateEvent(object sender, DateTime e)
        {
            UIDay tmp = sender as UIDay;
            foreach (UIElement UIE in this.UIDays.Children)
            {
                if (UIE is UIDay)
                {
                    UIDay day = UIE as UIDay;
                    if (day.isCurDay)
                    {
                        day.isCurDay = false;
                    }
                }
            }

            tmp.isCurDay = true;
            this.UIDate.LostFocus += DatePicker_LostFocus;
            tmpDate = e.Date;
            DateTime time = tmpDate;
            time = time.setTime(this.UITime.Time);
            this.UIDate.Text = time.format("HH:mm:ss dd/MM/yyyy");
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.curDate = this._date.AddMonths(-1);
        }

        private void TextBlock_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            this.curDate = this._date.AddMonths(1);
        }

        private void ChooseTime(object sender, RoutedEventArgs e)
        {
            this.UIDate.GotFocus -= DatePicker_GotFocus;
            DateTime time = tmpDate;
            time = time.setTime(this.UITime.Time);
            this.UIDate.Text = time.format("HH:mm:ss dd/MM/yyyy");
            this.UICalendar.Visibility = Visibility.Hidden;
            this.UIDate.LastSelect();
            this.UIDate.GotFocus += DatePicker_GotFocus;
        }

        private void UITime_TimeChange(object sender, TimeSpan e)
        {
            DateTime time = tmpDate;
            time = time.setTime(e);
            this.UIDate.Text = time.format("HH:mm:ss dd/MM/yyyy");
            this.UIDate.LostFocus -= DatePicker_LostFocus;
            this.UIDate.LostFocus += DatePicker_LostFocus;
            this.UIDate.LastSelect();
        }

        private void UITime_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.UIDate.LostFocus -= DatePicker_LostFocus;
        }

        private void UITime_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.UIDate.LostFocus += DatePicker_LostFocus;
        }

        #region operator


        public static bool operator >=(DatePicker a, DatePicker b)
        {
            if (a.Time >= b.Time)
                return true;
            return false;
        }

        public static bool operator >(DatePicker a, DatePicker b)
        {
            if (a.Time > b.Time)
                return true;
            return false;
        }

        public static bool operator <(DatePicker a, DatePicker b)
        {
            if (a.Time < b.Time)
                return true;
            return false;
        }
        public static bool operator <=(DatePicker a, DatePicker b)
        {
            if (a.Time < b.Time)
                return true;
            return false;
        }

        public static bool operator !=(DatePicker a, DatePicker b)
        {
            if (a.Time != b.Time)
                return true;
            return false;
        }

        public static bool operator ==(DatePicker a, DatePicker b)
        {
            if (a.Time == b.Time)
                return true;
            return false;
        }
        #endregion

    }
}