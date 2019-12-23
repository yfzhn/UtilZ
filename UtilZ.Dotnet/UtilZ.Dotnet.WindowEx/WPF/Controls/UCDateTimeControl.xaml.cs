﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.WindowEx.WPF.Controls
{
    /// <summary>
    /// UCDateTimeControl.xaml 的交互逻辑
    /// </summary>
    public partial class UCDateTimeControl : UserControl
    {
        #region 依赖属性
        /// <summary>
        /// 值依赖属性
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(DateTime), typeof(UCDateTimeControl),
                new FrameworkPropertyMetadata(default(DateTime), new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 获取或设置值
        /// </summary>
        public DateTime Value
        {
            get
            {
                return (DateTime)base.GetValue(ValueProperty);
            }
            set
            {
                base.SetValue(ValueProperty, value);
            }
        }

        private static void OnPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selfControl = (UCDateTimeControl)d;
            if (e.Property == ValueProperty)
            {
                selfControl.SetTime((DateTime)e.NewValue);
                selfControl.DateTimeChangedCallback(new DateTimeValueChangedArgs((DateTime)e.NewValue, (DateTime)e.OldValue));
            }
        }
        #endregion

        /// <summary>
        /// 日期时间值改变后事件
        /// </summary>

        public event EventHandler<DateTimeValueChangedArgs> DateTimeChanged;
        private void DateTimeChangedCallback(DateTimeValueChangedArgs e)
        {
            var handler = this.DateTimeChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public UCDateTimeControl()
        {
            InitializeComponent();

            this.Value = DateTime.Now;
        }



        private void SetTime(DateTime time)
        {
            this.RemoveEvent();
            //yyyy-MM-dd HH:mm:ss
            this.UpdateDayMaxByMonth(time.Year, time.Month);
            calendar.SelectedDate = time;
            calendar.DisplayDate = time;
            txtYear.Value = time.Year;
            txtMonth.Value = time.Month;
            txtDay.Value = time.Day;
            txtHour.Value = time.Hour;
            txtMinute.Value = time.Minute;
            txtSecond.Value = time.Second;
            this.AddEvent();
        }



        private DateTime? _closeTime = null;
        private void btnOpenPopup_Click(object sender, RoutedEventArgs e)
        {
            if (!PopupEx.HasOpen(this._closeTime))
            {
                return;
            }

            var time = this.Value;
            calendar.SelectedDate = time;
            calendar.DisplayDate = time;
            txtCalendarHour.Value = txtHour.Value;
            txtCalendarMinute.Value = txtMinute.Value;
            txtCalendarSecond.Value = txtSecond.Value;
            popup.IsOpen = true;
        }

        private void popup_Opened(object sender, EventArgs e)
        {
            //calendar.CaptureMouse();
            //calendar.ReleaseMouseCapture();

            //if (!popup.Focusable)
            //{
            //    popup.Focusable = true;
            //}

            //calendarBorder.RaiseEvent(new RoutedEventArgs(Border.MouseDownEvent));

            //Loger.Info($"1.calendarBorder Focusable:{calendarBorder.Focusable},IsFocused:{calendarBorder.IsFocused}");
            calendarBorder.Focus();
            //Loger.Info($"2.calendarBorder.IsFocused:{calendarBorder.IsFocused}");
        }

        private void popup_Closed(object sender, EventArgs e)
        {
            this._closeTime = DateTime.Now;
        }

        private void btnCalendarOk_Click(object sender, RoutedEventArgs e)
        {
            DateTime selectedDate = calendar.SelectedDate.Value;
            int hour = (int)txtCalendarHour.Value;
            int minute = (int)txtCalendarMinute.Value;
            int second = (int)txtCalendarSecond.Value;
            this.Value = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, hour, minute, second);
            popup.IsOpen = false;
        }

        private void btnCalendarCancell_Click(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = false;
        }


        private void AddEvent()
        {
            txtYear.ValueChanged += NumberControl_ValueChanged;
            txtMonth.ValueChanged += NumberControl_ValueChanged;
            txtDay.ValueChanged += NumberControl_ValueChanged;
            txtHour.ValueChanged += NumberControl_ValueChanged;
            txtMinute.ValueChanged += NumberControl_ValueChanged;
            txtSecond.ValueChanged += NumberControl_ValueChanged;
        }

        private void RemoveEvent()
        {
            txtYear.ValueChanged -= NumberControl_ValueChanged;
            txtMonth.ValueChanged -= NumberControl_ValueChanged;
            txtDay.ValueChanged -= NumberControl_ValueChanged;
            txtHour.ValueChanged -= NumberControl_ValueChanged;
            txtMinute.ValueChanged -= NumberControl_ValueChanged;
            txtSecond.ValueChanged -= NumberControl_ValueChanged;
        }

        private void NumberControl_ValueChanged(object sender, NumberValueChangedArgs e)
        {
            var control = (NumberControl)sender;
            if (control == txtYear || control == txtMonth)
            {
                this.UpdateDayMaxByMonth((int)txtYear.Value, (int)txtMonth.Value);
            }

            this.UpdateValue();
        }

        private void UpdateValue()
        {
            int year = (int)txtYear.Value;
            int month = (int)txtMonth.Value;
            int day = (int)txtDay.Value;
            int hour = (int)txtHour.Value;
            int minute = (int)txtMinute.Value;
            int second = (int)txtSecond.Value;
            this.Value = new DateTime(year, month, day, hour, minute, second);
        }

        private void UpdateDayMaxByMonth(int year, int month)
        {
            int maxDayCount;
            switch (month)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    maxDayCount = 31;
                    break;
                case 2:
                    if (year % 400 == 0 ||
                        year % 100 != 0 && year % 4 == 0)
                    {
                        maxDayCount = 29;
                    }
                    else
                    {
                        maxDayCount = 28;
                    }
                    break;
                default:
                    maxDayCount = 30;
                    break;
            }

            txtDay.Maximum = maxDayCount;
        }





        private void calendarBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.MouseClickInControl(e, calendarBorder))
            {
                this.btnCalendarCancell_Click(sender, e);
                return;
            }

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            if (this.MouseClickInControl(e, btnCalendarOk))
            {
                this.btnCalendarOk_Click(sender, e);
                return;
            }

            if (this.MouseClickInControl(e, btnCalendarCancell))
            {
                this.btnCalendarCancell_Click(sender, e);
                return;
            }

            if (this.MouseClickInControl(e, txtCalendarHour))
            {
                txtCalendarHour.Focus();
                return;
            }

            if (this.MouseClickInControl(e, txtCalendarMinute))
            {
                txtCalendarMinute.Focus();
                return;
            }

            if (this.MouseClickInControl(e, txtCalendarSecond))
            {
                txtCalendarSecond.Focus();
                return;
            }
        }

        private bool MouseClickInControl(MouseButtonEventArgs e, FrameworkElement relativeTo)
        {
            Point point;
            if (e == null)
            {
                point = Mouse.GetPosition(calendarBorder);
            }
            else
            {
                point = e.GetPosition(relativeTo);
            }

            if (point.X >= 0 &&
                point.X <= relativeTo.ActualWidth &&
                point.Y >= 0 &&
                point.Y <= relativeTo.ActualHeight)
            {
                return true;
            }

            return false;
        }

        private void calendarBorder_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!this.MouseClickInControl(null, calendarBorder))
            {
                this.btnCalendarCancell_Click(sender, e);
            }
        }
    }




    /// <summary>
    /// 时期时间值改变事件
    /// </summary>
    public class DateTimeValueChangedArgs : EventArgs
    {
        /// <summary>
        /// 新值
        /// </summary>
        public DateTime NewValue { get; private set; }

        /// <summary>
        /// 旧值
        /// </summary>
        public DateTime OldValue { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="newValue">新值</param>
        /// <param name="oldValue">旧值</param>
        public DateTimeValueChangedArgs(DateTime newValue, DateTime oldValue)
        {
            this.NewValue = newValue;
            this.OldValue = oldValue;
        }
    }
}
