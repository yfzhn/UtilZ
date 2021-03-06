﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UtilZ.DotnetCore.WindowEx.WPF.Base
{
    /// <summary>
    /// WPF辅助类
    /// </summary>
    public static class WPFHelper
    {
        /// <summary>
        /// 判断UI是否处于设计模式[处理设计模式返回true;否则返回false]
        /// </summary>
        /// <param name="ele">要判断的UI元素</param>
        /// <returns>处理设计模式返回true;否则返回false</returns>
        public static bool IsInDesignMode(this UIElement ele)
        {
            if (ele == null)
            {
                return false;
            }

            //非UI对象，要判断是否处于设计器模式
            //bool isInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());

            //UI对象，要判断是否处于设计器模式
            //bool isInDesignMode = DesignerProperties.GetIsInDesignMode(ele);

            //这两种方式有时会失效（具体什么情况下会失效不明）
            //return (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;

            return DesignerProperties.GetIsInDesignMode(ele);
        }

        /// <summary>
        /// 判断当前窗口是否已释放[返回值:true:已释放Invoke;false:未释放]
        /// </summary>
        /// <param name="window">要判判断的窗口</param>
        /// <returns>返回值:true:已释放Invoke;false:未释放</returns>
        public static bool IsDisposed(this Window window)
        {
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            return new System.Windows.Interop.WindowInteropHelper(window).Handle == IntPtr.Zero;
        }

        /// <summary>
        /// 判断当前操作是否需要调用Invoke[返回值:true:调用Invoke;false:不需要]
        /// </summary>
        /// <param name="dispatcher">判断的对象</param>
        /// <returns>返回值:true:调用Invoke;false:不需要</returns>
        public static bool InvokeRequired(this System.Windows.Threading.DispatcherObject dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            return !dispatcher.Dispatcher.CheckAccess();
        }

        /// <summary>
        /// 查找元素根窗口
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static Window FindRootWindow(FrameworkElement element)
        {
            Window rootWindow = null;
            while (element.Parent != null)
            {
                element = (FrameworkElement)element.Parent;
                if (element is Window)
                {
                    rootWindow = (Window)element;
                    break;
                }
            }

            return rootWindow;
        }
    }
}
