﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace UtilZ.Dotnet.ILWindowEx.Winform.PageGrid
{
    /// <summary>
    /// 分页显示表格控件显示列设置窗口
    /// </summary>
    public partial class FColumnDisplaySetting : DockContent
    {
        /// <summary>
        /// 获取显示列设置控件
        /// </summary>
        public ListBox ListBoxSettingCols
        {
            get { return listBoxSettingCols; }
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public FColumnDisplaySetting()
        {
            InitializeComponent();
        }
    }
}
