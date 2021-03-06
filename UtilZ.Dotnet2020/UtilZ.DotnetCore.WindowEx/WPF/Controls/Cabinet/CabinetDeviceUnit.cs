﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace UtilZ.DotnetCore.WindowEx.WPF.Controls
{

    /// <summary>
    /// 机柜组-对应一行机柜
    /// </summary>
    public class CabinetInfoGroup
    {
        /// <summary>
        /// 机柜组
        /// </summary>
        public List<CabinetInfo> Group { get; set; } = new List<CabinetInfo>();

        /// <summary>
        /// 组名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CabinetInfoGroup()
        {

        }
    }

    /// <summary>
    /// 机柜信息
    /// </summary>
    public class CabinetInfo
    {
        /// <summary>
        /// 机柜名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 机柜高度,总U数
        /// </summary>
        public byte Height { get; set; } = 42;

        /// <summary>
        /// 设备单元列表
        /// </summary>
        public List<CabinetDeviceUnit> CabinetDeviceUnitList { get; set; } = new List<CabinetDeviceUnit>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public CabinetInfo()
        {

        }
    }

    /// <summary>
    /// 设备单元
    /// </summary>
    public class CabinetDeviceUnit
    {
        private int _beginLocation = 0;
        /// <summary>
        /// 设备U起始索引
        /// </summary>
        public int BeginLocation
        {
            get { return _beginLocation; }
            set { _beginLocation = value; }
        }

        private int _height = 0;
        /// <summary>
        /// 设备高度,占U数
        /// </summary>
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        /// <summary>
        /// 设备列表,为null或Count=0,表示空U
        /// </summary>
        public List<CabinetDevice> DeviceList { get; set; } = new List<CabinetDevice>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public CabinetDeviceUnit()
        {

        }
    }

    /// <summary>
    /// 设备
    /// </summary>
    public class CabinetDevice : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        /// <summary>
        /// 属性值改变通知事件
        /// </summary>
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 属性值改变通知方法
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected void OnRaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private string _deviceName = string.Empty;
        /// <summary>
        /// 获取或设置该U对应的设备名称
        /// </summary>
        public string DeviceName
        {
            get { return _deviceName; }
            set
            {
                _deviceName = value;
                this.OnRaisePropertyChanged(nameof(DeviceName));
            }
        }

        private Brush _deviceBackground = Brushes.Transparent;
        /// <summary>
        /// 获取或设置该U背景
        /// </summary>
        public Brush DeviceBackground
        {
            get { return _deviceBackground; }
            set
            {
                _deviceBackground = value;
                this.OnRaisePropertyChanged(nameof(DeviceBackground));
            }
        }

        private Brush _deviceStatusBrush = null;
        /// <summary>
        /// 设备状态元素
        /// </summary>
        public Brush DeviceStatusBrush
        {
            get { return _deviceStatusBrush; }
            set
            {
                _deviceStatusBrush = value;
                this.OnRaisePropertyChanged(nameof(DeviceStatusBrush));
            }
        }

        private Thickness _deviceBorderThickness = new Thickness(2.0d);
        /// <summary>
        /// 设备边框宽度
        /// </summary>
        public Thickness DeviceBorderThickness
        {
            get { return _deviceBorderThickness; }
            set
            {
                _deviceBorderThickness = value;
                this.OnRaisePropertyChanged(nameof(DeviceBorderThickness));
            }
        }

        private Brush _deviceBorderBrush = Brushes.Transparent;
        /// <summary>
        /// 设备边框Brush
        /// </summary>
        public Brush DeviceBorderBrush
        {
            get { return _deviceBorderBrush; }
            set
            {
                _deviceBorderBrush = value;
                this.OnRaisePropertyChanged(nameof(DeviceBorderBrush));
            }
        }

        /// <summary>
        /// 机柜共用位置设备方向
        /// </summary>
        public DeviceOrientation Orientation { get; set; } = DeviceOrientation.None;

        /// <summary>
        /// Tag
        /// </summary>
        public object Tag { get; set; }



        /// <summary>
        /// 构造函数
        /// </summary>
        public CabinetDevice()
        {

        }
    }

    /// <summary>
    /// 机柜共用位置设备方向
    /// </summary>
    public enum DeviceOrientation
    {
        /// <summary>
        /// 无共用
        /// </summary>
        None = 0,

        /// <summary>
        /// 左
        /// </summary>
        Left = 1,

        /// <summary>
        /// 右
        /// </summary>
        Right = 2,

        /// <summary>
        /// 前
        /// </summary>
        Front = 3,

        /// <summary>
        /// 后
        /// </summary>
        Back = 4
    }


    internal class CabinetUnit
    {
        public int Index { get; private set; }

        public CabinetUnit(int index)
        {
            this.Index = index;
        }
    }

    internal class CabinetConstant
    {
        public const double SINGLE_U_HEIGHT = 22.225d;

        //public const double SINGLE_BOTTOM_HEIGHT = 40d;
    }
}
