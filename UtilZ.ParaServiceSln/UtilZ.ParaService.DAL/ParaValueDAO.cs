﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Transactions;
using UtilZ.Dotnet.DBBase.Core;
using UtilZ.Dotnet.DBBase.Interfaces;
using UtilZ.Dotnet.DBIBase.DBModel.Model;
using UtilZ.ParaService.DBModel;
using UtilZ.ParaService.Model;

namespace UtilZ.ParaService.DAL
{
    public class ParaValueDAO : BaseDAO
    {
        public ParaValueDAO() : base()
        {

        }

        /// <summary>
        /// 添加参数值，成功返回参数值版本号
        /// </summary>
        /// <param name="paraValues"></param>
        /// <returns></returns>
        public long AddParaValue(ParaValueSettingPost para)
        {
            var projectId = para.PID;
            var paraValues = para.ToParaValues();
            IDBAccess dbAccess = base.GetDBAccess();
            string paraSign = dbAccess.ParaSign;

            using (var conInfo = dbAccess.CreateConnection(Dotnet.DBBase.Model.DBVisitType.W))
            {
                using (var transaction = conInfo.Connection.BeginTransaction())
                {
                    try
                    {
                        //查询最大版本号
                        object obj = this.QueryBestNewVersion(dbAccess, conInfo, transaction, projectId);
                        long paraVersion;
                        if (obj == null || obj == DBNull.Value)
                        {
                            paraVersion = 1;
                        }
                        else
                        {
                            paraVersion = (long)obj + 1;
                        }


                        //没有设置过值则插入版本号
                        var insertParaVersionCmd = conInfo.Connection.CreateCommand();
                        insertParaVersionCmd.Transaction = transaction;
                        insertParaVersionCmd.CommandText = string.Format(@"INSERT INTO ParaVersion(ProjectID,Version) VALUES ({0}ProjectID,{0}Version)", paraSign);
                        dbAccess.AddCommandParameter(insertParaVersionCmd, "ProjectID", projectId);
                        dbAccess.AddCommandParameter(insertParaVersionCmd, "Version", paraVersion);
                        if (insertParaVersionCmd.ExecuteNonQuery() != 1)
                        {
                            throw new DBException(ParaServiceConstant.DB_FAIL, "插入参数值版本号失败，原因未知");
                        }

                        //插入参数值
                        var insertCmd = conInfo.Connection.CreateCommand();
                        insertCmd.Transaction = transaction;
                        insertCmd.CommandText = string.Format(@"INSERT INTO ParaValue (ParaID,ProjectID,Version,Value) VALUES ({0}ParaID,{0}ProjectID,{0}Version,{0}Value)", paraSign);

                        foreach (var paraValue in paraValues)
                        {
                            dbAccess.AddCommandParameter(insertCmd, "ParaID", paraValue.ParaID);
                            dbAccess.AddCommandParameter(insertCmd, "ProjectID", projectId);
                            dbAccess.AddCommandParameter(insertCmd, "Version", paraVersion);
                            dbAccess.AddCommandParameter(insertCmd, "Value", paraValue.Value);
                            if (insertCmd.ExecuteNonQuery() != 1)
                            {
                                throw new DBException(ParaServiceConstant.DB_FAIL, "写入数据库失败，原因未知");
                            }
                        }

                        transaction.Commit();
                        return paraVersion;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public long QueryBestNewVersion(long projectId)
        {
            IDBAccess dbAccess = base.GetDBAccess();
            using (var conInfo = dbAccess.CreateConnection(Dotnet.DBBase.Model.DBVisitType.R))
            {
                object obj = this.QueryBestNewVersion(dbAccess, conInfo, null, projectId);
                if (obj == null || obj == DBNull.Value)
                {
                    throw new DBException(ParaServiceConstant.DB_FAIL, "参数值未设置");
                }
                else
                {
                    return (long)obj;
                }
            }
        }

        private object QueryBestNewVersion(IDBAccess dbAccess, DbConnectionInfo conInfo, DbTransaction transaction, long projectId)
        {
            var queryParaVersionCmd = conInfo.Connection.CreateCommand();
            queryParaVersionCmd.Transaction = transaction;
            queryParaVersionCmd.CommandText = string.Format(@"SELECT MAX(Version) FROM ParaVersion WHERE ProjectID={0}ProjectID", dbAccess.ParaSign);
            dbAccess.AddCommandParameter(queryParaVersionCmd, "ProjectID", projectId);
            return queryParaVersionCmd.ExecuteScalar();
        }

        public List<long> QueryVersions(long projectId)
        {
            IDBAccess dbAccess = base.GetDBAccess();
            using (var conInfo = dbAccess.CreateConnection(Dotnet.DBBase.Model.DBVisitType.R))
            {
                var queryParaVersionCmd = conInfo.Connection.CreateCommand();
                queryParaVersionCmd.CommandText = string.Format(@"SELECT Version FROM ParaVersion WHERE ProjectID={0}ProjectID", dbAccess.ParaSign);
                dbAccess.AddCommandParameter(queryParaVersionCmd, "ProjectID", projectId);
                var reader = queryParaVersionCmd.ExecuteReader();
                var versions = new List<long>();
                while (reader.Read())
                {
                    versions.Add(reader.GetInt64(0));
                }

                return versions;
            }
        }

        public ServicePara QueryParaValues(long projectId, long moduleId, long version)
        {
            IDBAccess dbAccess = base.GetDBAccess();
            string paraSign = dbAccess.ParaSign;
            var servicePara = new ServicePara();
            using (var conInfo = dbAccess.CreateConnection(Dotnet.DBBase.Model.DBVisitType.R))
            {
                servicePara.Version = version;

                var queryParaValueCmd = conInfo.Connection.CreateCommand();
                //queryParaValueCmd.CommandText = string.Format(@"SELECT ParaID,Value FROM ParaValue WHERE ProjectID={0}ProjectID AND Version={0}Version", paraSign);
                //SELECT Key,Value FROM (SELECT ParaValue.ProjectID,ParaValue.Version,Key,Value FROM ParaValue INNER JOIN Para ON ParaValue.ParaID=Para.ID) WHERE ProjectID=8 AND Version=1
                //queryParaValueCmd.CommandText = string.Format(@"SELECT Key,Value FROM (SELECT ParaValue.ProjectID,ParaValue.Version,Key,Value FROM ParaValue INNER JOIN Para ON ParaValue.ParaID=Para.ID) WHERE ProjectID={0}ProjectID AND Version={0}Version", paraSign);
                queryParaValueCmd.CommandText = string.Format(@"SELECT Key,Value FROM 
(SELECT ParaID,Key,Value FROM (SELECT ParaValue.ParaID,ParaValue.ProjectID,ParaValue.Version,Key,Value FROM ParaValue INNER JOIN Para ON ParaValue.ParaID=Para.ID) WHERE ProjectID={0}ProjectID AND Version={0}Version) t 
INNER JOIN ModulePara ON ModulePara.ParaID=t.ParaID WHERE ModuleID={0}ModuleID", paraSign);
                dbAccess.AddCommandParameter(queryParaValueCmd, "ProjectID", projectId);
                dbAccess.AddCommandParameter(queryParaValueCmd, "Version", version);
                dbAccess.AddCommandParameter(queryParaValueCmd, "ModuleID", moduleId);
                var paraValueReader = queryParaValueCmd.ExecuteReader();
                while (paraValueReader.Read())
                {
                    var serviceParaItem = new ServiceParaItem();
                    serviceParaItem.Key = paraValueReader.GetString(0);
                    serviceParaItem.Value = paraValueReader.GetString(1);
                    servicePara.Items.Add(serviceParaItem);
                }
            }

            return servicePara;
        }

        public List<ParaValue> QueryParaValues(long projectId, long version)
        {
            IDBAccess dbAccess = base.GetDBAccess();
            string paraSign = dbAccess.ParaSign;
            using (var conInfo = dbAccess.CreateConnection(Dotnet.DBBase.Model.DBVisitType.R))
            {
                var queryParaValueCmd = conInfo.Connection.CreateCommand();
                queryParaValueCmd.CommandText = string.Format(@"SELECT ParaID,Value FROM ParaValue WHERE ProjectID={0}ProjectID AND Version={0}Version", paraSign);
                dbAccess.AddCommandParameter(queryParaValueCmd, "ProjectID", projectId);
                dbAccess.AddCommandParameter(queryParaValueCmd, "Version", version);
                var paraValueReader = queryParaValueCmd.ExecuteReader();

                var serviceParas = new List<ParaValue>();
                while (paraValueReader.Read())
                {
                    var serviceParaItem = new ParaValue();
                    serviceParaItem.ParaID = paraValueReader.GetInt64(0);
                    serviceParaItem.Value = paraValueReader.GetString(1);
                    serviceParas.Add(serviceParaItem);
                }

                return serviceParas;
            }
        }

        public int DeleteParaValue(long projectId, long beginVer, long endVer)
        {
            IDBAccess dbAccess = base.GetDBAccess();
            string paraSign = dbAccess.ParaSign;

            using (var conInfo = dbAccess.CreateConnection(Dotnet.DBBase.Model.DBVisitType.W))
            {
                using (var transaction = conInfo.Connection.BeginTransaction())
                {
                    try
                    {
                        var deleteParaValueCmd = conInfo.Connection.CreateCommand();
                        deleteParaValueCmd.Transaction = transaction;
                        deleteParaValueCmd.CommandText = string.Format(@"DELETE FROM ParaValue WHERE ProjectID={0}ProjectID AND Version>={0}BeginVer AND Version<={0}EndVer", paraSign);
                        dbAccess.AddCommandParameter(deleteParaValueCmd, "ProjectID", projectId);
                        dbAccess.AddCommandParameter(deleteParaValueCmd, "BeginVer", beginVer);
                        dbAccess.AddCommandParameter(deleteParaValueCmd, "EndVer", endVer);
                        int ret = deleteParaValueCmd.ExecuteNonQuery();

                        var deleteParaValueVersionCmd = conInfo.Connection.CreateCommand();
                        deleteParaValueVersionCmd.Transaction = transaction;
                        deleteParaValueVersionCmd.CommandText = string.Format(@"DELETE FROM ParaVersion WHERE ProjectID={0}ProjectID AND Version>={0}BeginVer AND Version<={0}EndVer", paraSign);
                        dbAccess.AddCommandParameter(deleteParaValueVersionCmd, "ProjectID", projectId);
                        dbAccess.AddCommandParameter(deleteParaValueVersionCmd, "BeginVer", beginVer);
                        dbAccess.AddCommandParameter(deleteParaValueVersionCmd, "EndVer", endVer);
                        ret += deleteParaValueVersionCmd.ExecuteNonQuery();

                        transaction.Commit();
                        return ret;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public List<VerionParaValueItem> QueryVersionParas(long projectId, long version)
        {
            IDBAccess dbAccess = base.GetDBAccess();
            string paraSign = dbAccess.ParaSign;
            using (var conInfo = dbAccess.CreateConnection(Dotnet.DBBase.Model.DBVisitType.R))
            {
                var queryParaValueCmd = conInfo.Connection.CreateCommand();
                //queryParaValueCmd.CommandText = string.Format(@"SELECT ID,GroupID,Key,Name,Des,Value FROM Para INNER JOIN ParaValue ON Para.ID=ParaValue.ParaID WHERE ParaValue.ProjectID={0}ProjectID AND ParaValue.Version={0}Version;", paraSign);
                queryParaValueCmd.CommandText = string.Format(@"SELECT ID,GroupID,Key,Name,Des,Value FROM Para INNER JOIN ParaValue ON Para.ID=ParaValue.ParaID WHERE ParaValue.ProjectID={0}ProjectID AND ParaValue.Version={0}Version;", paraSign);
                dbAccess.AddCommandParameter(queryParaValueCmd, "ProjectID", projectId);
                dbAccess.AddCommandParameter(queryParaValueCmd, "Version", version);
                var reader = queryParaValueCmd.ExecuteReader();

                var verionParaValueItems = new List<VerionParaValueItem>();
                while (reader.Read())
                {
                    var verionParaValueItem = new VerionParaValueItem();
                    verionParaValueItem.ID = reader.GetInt64(0);
                    verionParaValueItem.GroupID = reader.GetInt64(1);
                    verionParaValueItem.Key = reader.GetString(2);
                    verionParaValueItem.Name = reader.GetString(3);
                    verionParaValueItem.Des = reader.GetString(4);
                    verionParaValueItem.Value = reader.GetString(5);
                    verionParaValueItems.Add(verionParaValueItem);
                }

                return verionParaValueItems;
            }
        }
    }
}
