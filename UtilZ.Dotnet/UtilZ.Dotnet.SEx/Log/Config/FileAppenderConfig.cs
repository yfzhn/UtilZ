﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace UtilZ.Dotnet.SEx.Log.Config
{
    /// <summary>
    /// 日志追加器配置
    /// </summary>
    [Serializable]
    public class FileAppenderConfig : BaseConfig
    {
        private int _days = 7;
        /// <summary>
        /// 日志保留天数
        /// </summary>
        public int Days
        {
            get { return _days; }
            set { _days = value; }
        }

        private int _maxFileCount = -1;
        /// <summary>
        /// 最多产生的日志文件数，超过则只保留最新的n个,－1为不限文件数
        /// </summary>
        public int MaxFileCount
        {
            get { return _maxFileCount; }
            set { _maxFileCount = value; }
        }

        private int _maxFileSize = 10 * 1024 * 1024;
        /// <summary>
        /// 日志文件上限大小,当文件超过此值则分隔成多个日志文件,单位/MB
        /// </summary>
        public int MaxFileSize
        {
            get { return _maxFileSize; }
            set { _maxFileSize = value; }
        }

        /********************************************************************
         * Log\yyyy-MM-dd_HH_mm_ss;_flow.log  =>  Log\2018-08-19_17_05_12_flow.log
         * yyyy-MM-dd\info.log  =>  2018-08-19\info_1.log 或 2018-08-19\info_n.log
         * yyyy-MM-dd\yyyy-MM-dd_HH_mm_ss;_flow.log  =>  2018-08-19\2018-08-19_17_05_12_flow.log
         * 或
         * yyyy-MM-dd\HH_mm_ss;_flow.log  =>  2018-08-19\17_05_12_flow.log
         ********************************************************************/

        private string _filePath = @"Log/*yyyy-MM-dd_HH_mm_ss*.log";
        /// <summary>
        /// 日志存放路径
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        private bool _isAppend = true;
        /// <summary>
        /// 是否追加日志
        /// </summary>
        public bool IsAppend
        {
            get { return _isAppend; }
            set { _isAppend = value; }
        }

        /// <summary>
        /// 日志安全策略,该类型为实现接口ILogSecurityPolicy的子类,必须实现Encryption方法
        /// </summary>
        public string SecurityPolicy { get; set; }

        /// <summary>
        /// 进程同步锁名称
        /// </summary>
        public string MutexName { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ele">配置元素节点</param>
        public FileAppenderConfig(XElement ele) : base(ele)
        {
            if (ele == null)
            {
                return;
            }

            if (int.TryParse(base.GetChildXElementValue(ele, "Days"), out this._days))
            {
                if (this._days < 1)
                {
                    this._days = 7;
                }
            }

            if (int.TryParse(base.GetChildXElementValue(ele, "MaxFileCount"), out this._maxFileCount))
            {
                if (this._maxFileCount < 1 && this._maxFileCount != -1)
                {
                    this._maxFileCount = -1;
                }
            }

            int maxFileSize;
            if (int.TryParse(base.GetChildXElementValue(ele, "MaxFileSize"), out maxFileSize))
            {
                if (maxFileSize < 1)
                {
                    maxFileSize = 10;
                }

                this._maxFileSize = maxFileSize * 1024 * 1024;
            }

            string filePath = base.GetChildXElementValue(ele, "FilePath").Trim();
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                this._filePath = filePath;
            }

            bool.TryParse(base.GetChildXElementValue(ele, "IsAppend").Trim(), out this._isAppend);
            this.SecurityPolicy = base.GetChildXElementValue(ele, "SecurityPolicy").Trim();
            this.MutexName = base.GetChildXElementValue(ele, "MutexName ").Trim();
        }
    }
}
