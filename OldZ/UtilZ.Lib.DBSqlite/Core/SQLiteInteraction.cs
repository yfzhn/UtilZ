﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using UtilZ.Lib.DBBase.Base;
using UtilZ.Lib.Base.Ex;
using UtilZ.Lib.DBModel.Common;
using UtilZ.Lib.DBModel.Config;
using UtilZ.Lib.DBModel.Constant;
using UtilZ.Lib.DBModel.Model;

namespace UtilZ.Lib.DBSqlite.Core
{
    /// <summary>
    /// SQLite数据库交互类
    /// </summary>
    public class SQLiteInteraction : DBInteractioBase
    {
        /// <summary>
        /// 数据库程序集名称
        /// </summary>
        private readonly string _databaseName = typeof(System.Data.SQLite.SQLiteConnection).Assembly.FullName;

        /// <summary>
        /// 数据库类型名称
        /// </summary>
        public string DatabaseTypeName
        {
            get { return this._databaseName; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config">配置项</param>
        public SQLiteInteraction(DBConfigElement config)
            : base()
        {

        }

        /// <summary>
        /// 创建数据库读连接对象
        /// </summary>
        /// <param name="config">数据库配置</param>
        /// <param name="visitType">访问类型</param>
        /// <returns>数据库连接对象</returns>
        public override IDbConnection CreateConnection(DBConfigElement config, DBVisitType visitType)
        {
            return new SQLiteConnection(this.GetDBConStr(config, visitType));
        }

        /// <summary>
        /// 创建DbDataAdapter
        /// </summary>
        /// <returns>创建好的DbDataAdapter</returns>
        public override IDbDataAdapter CreateDbDataAdapter()
        {
            return new SQLiteDataAdapter();
        }

        /// <summary>
        /// 生成数据库连接字符串
        /// </summary>
        /// <param name="config">数据库配置</param>
        /// <param name="visitType">访问类型</param>
        /// <returns>数据库连接字符串</returns>
        public override string GenerateDBConStr(DBConfigElement config, DBVisitType visitType)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            SQLiteConnectionStringBuilder scsb;
            if (config.DBConInfoType == 0)
            {
                scsb = new SQLiteConnectionStringBuilder(config.ConStr);
            }
            else
            {
                scsb = new SQLiteConnectionStringBuilder();
                scsb.Pooling = true;
                scsb.DataSource = DirectoryInfoEx.GetFullPath(config.DatabaseName);
                if (!string.IsNullOrEmpty(config.Password))
                {
                    scsb.Password = config.Password;
                }
            }

            string dbDir = Path.GetDirectoryName(scsb.DataSource);
            if (!Directory.Exists(dbDir))
            {
                Directory.CreateDirectory(dbDir);
            }

            if (visitType == DBVisitType.R)
            {
                scsb.ReadOnly = true;
            }
            else
            {
                scsb.ReadOnly = false;
            }

            return scsb.ConnectionString;
        }

        /// <summary>
        /// 设置命令参数
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="collection">参数集合</param>
        /// <returns>创建好的命令参数</returns>
        public override void SetParameter(IDbCommand cmd, NDbParameterCollection collection)
        {
            SQLiteCommand sqliteCmd = cmd as SQLiteCommand;
            if (sqliteCmd == null || collection == null || collection.Count == 0)
            {
                return;
            }

            object value;
            foreach (NDbParameter parameter in collection)
            {
                value = parameter.Value;
                if (value == null)
                {
                    value = DBNull.Value;
                }

                sqliteCmd.Parameters.AddWithValue(parameter.ParameterName, value);
            }
        }

        /// <summary>
        /// 设置命令参数
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="parameters">参数集合</param>
        /// <returns>创建好的命令参数</returns>
        public override void SetParameter(IDbCommand cmd, IEnumerable<IDbDataParameter> parameters)
        {
            SQLiteCommand sqliteCmd = cmd as SQLiteCommand;
            if (sqliteCmd == null || parameters == null || parameters.Count() == 0)
            {
                return;
            }

            object value;
            foreach (var parameter in parameters)
            {
                value = parameter.Value;
                if (value == null)
                {
                    value = DBNull.Value;
                }

                sqliteCmd.Parameters.AddWithValue(parameter.ParameterName, value);
            }
        }

        /// <summary>
        /// 设置命令参数
        /// </summary>
        /// <param name="cmd">命令参数</param>
        /// <param name="paraValues">参数名及值字典集合</param>
        public override void SetParameter(IDbCommand cmd, Dictionary<string, object> paraValues)
        {
            SQLiteCommand sqliteCmd = cmd as SQLiteCommand;
            if (sqliteCmd == null || paraValues == null || paraValues.Count == 0)
            {
                return;
            }

            object value;
            foreach (var kv in paraValues)
            {
                value = kv.Value;
                if (value == null)
                {
                    value = DBNull.Value;
                }

                sqliteCmd.Parameters.AddWithValue(kv.Key, value);
            }
        }

        /// <summary>
        /// 创建命令参数
        /// </summary>
        /// <param name="parameter">命令参数</param>
        /// <returns>创建好的命令参数</returns>
        public override IDbDataParameter CreateDbParameter(NDbParameter parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            object value = parameter.Value;
            if (value == null)
            {
                value = DBNull.Value;
            }

            return new SQLiteParameter(parameter.ParameterName, value);
        }
    }
}