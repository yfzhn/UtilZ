﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilZ.DotnetCore.WindowEx.AsynWait;
using UtilZ.DotnetCore.WindowEx.Base.PartAsynWait;
using UtilZ.DotnetCore.WindowEx.Base.PartAsynWait.Excute;
using UtilZ.DotnetCore.WindowEx.Base.PartAsynWait.Interface;
using UtilZ.DotnetCore.WindowEx.Base.PartAsynWait.Model;
using UtilZ.Dotnet.WindowEx.Winform.Controls.PartAsynWait;
using UtilZ.Dotnet.WindowEx.Winform.Controls.PartAsynWait.Excute;
using UtilZ.DotnetStd.Ex.Base;

namespace UtilZ.Dotnet.WindowEx.Winform.Controls.PartAsynWait
{
    /// <summary>
    /// Winform异步等待辅助类
    /// </summary>
    public class WinformPartAsynWaitHelper : PartAsynWaitHelperBase
    {
        /// <summary>
        /// 静态构造函数创建异步执行对象创建工厂对象
        /// </summary>
        static WinformPartAsynWaitHelper()
        {
            _partAsynExcuteFactory = new PartAsynExcuteFactoryWinform();
        }

        /// <summary>
        /// 异步执行对象创建工厂对象
        /// </summary>
        private static PartAsynExcuteFactoryBase _partAsynExcuteFactory;

        /// <summary>
        /// 获取或设置异步执行对象创建工厂对象
        /// </summary>
        public static PartAsynExcuteFactoryBase PartAsynExcuteFactory
        {
            get { return _partAsynExcuteFactory; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _partAsynExcuteFactory = value;
            }
        }

        /// <summary>
        /// 异步等待
        /// </summary>
        /// <typeparam name="T">异步执行参数类型</typeparam>
        /// <typeparam name="TResult">异步执行返回值类型</typeparam>
        ///<param name="asynWaitPara">异步等待执行参数</param>
        ///<param name="containerControl">容器控件</param>
        public static void Wait<T, TResult>(PartAsynWaitPara<T, TResult> asynWaitPara, System.Windows.Forms.Control containerControl)
        {
            Wait<T, TResult>(asynWaitPara, containerControl, null);
        }

        /// <summary>
        /// 异步等待
        /// </summary>
        /// <typeparam name="T">异步执行参数类型</typeparam>
        /// <typeparam name="TResult">异步执行返回值类型</typeparam>
        ///<param name="asynWaitPara">异步等待执行参数</param>
        ///<param name="containerControl">容器控件</param>
        ///<param name="asynWait">异步等待UI</param>
        public static void Wait<T, TResult>(PartAsynWaitPara<T, TResult> asynWaitPara, System.Windows.Forms.Control containerControl, IPartAsynWait asynWait)
        {
            ParaValidate(asynWaitPara, containerControl);
            var asynExcute = _partAsynExcuteFactory.CreateExcute<T, System.Windows.Forms.Control, TResult>();
            PartAsynUIParaProxy.SetAsynWait(asynWaitPara, asynWait);
            asynExcute.Excute(asynWaitPara, containerControl);
        }
    }
}
