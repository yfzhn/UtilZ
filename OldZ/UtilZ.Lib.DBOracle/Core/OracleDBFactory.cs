﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilZ.Lib.DBBase.Base;
using UtilZ.Lib.DBBase.Factory;
using UtilZ.Lib.DBBase.Interface;
using UtilZ.Lib.DBModel.Config;

namespace UtilZ.Lib.DBOracle.Core
{
    /// <summary>
    /// Oracle数据访问工厂类
    /// </summary>
    public class OracleDBFactory : DBFactoryBase
    {
        /// <summary>
        /// 数据库交互实例 数据库访问实例字典[key:dbid;value:数据库访问实例]
        /// </summary>
        private readonly DBInteractioBase _dbAccess = new OracleInteraction();

        /// <summary>
        /// 构造函数
        /// </summary>
        public OracleDBFactory() : base()
        {

        }

        /// <summary>
        /// 获取数据库交互实例
        /// </summary>
        /// <param name="config">数据库配置</param>
        /// <returns>数据库交互实例</returns>
        public override DBInteractioBase GetDBInteraction(DBConfigElement config)
        {
            return this._dbAccess;
        }

        /// <summary>
        /// 获取数据库访问实例
        /// </summary>
        /// <param name="dbid">数据库编号ID</param>
        /// <returns>数据库访问实例</returns>
        public override IDBAccess GetDBAccess(int dbid)
        {
            return new OracleDBAccess(dbid);
        }
    }
}