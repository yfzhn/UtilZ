﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.SEx.Log.Config;
using UtilZ.Dotnet.SEx.Log.Model;

namespace UtilZ.Dotnet.SEx.Log.Appender
{
    internal class FileAppenderFixFileNameBuilder : FileAppenderPathBuilderBase
    {
        private readonly string _fileNameWithoutExtension;
        private readonly string _extension;
        private int _index = 1;

        public FileAppenderFixFileNameBuilder(FileAppenderConfig config, string[] paths, char[] pathSplitChars) :
             base(config, paths, pathSplitChars)
        {
            var fileName = paths.Last();
            this._fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName) + "_";
            this._extension = Path.GetExtension(fileName);
            this.RecoverIndex();
        }

        private void RecoverIndex()
        {
            try
            {
                string[] logFilePaths = Directory.GetFiles(base._rootDir, base._searchPattern, SearchOption.TopDirectoryOnly);
                int index = 1, tmpIndex;
                string indexStr;
                foreach (var logFilePath in logFilePaths)
                {
                    try
                    {
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(logFilePath);
                        indexStr = fileNameWithoutExtension.Substring(this._fileNameWithoutExtension.Length);
                        if (int.TryParse(indexStr, out tmpIndex))
                        {
                            if (tmpIndex > index)
                            {
                                index = tmpIndex;
                            }
                        }
                    }
                    catch (Exception exi)
                    {
                        LogSysInnerLog.OnRaiseLog(this, exi);
                    }
                }

                this._index = index + 1;
            }
            catch (Exception ex)
            {
                LogSysInnerLog.OnRaiseLog(this, ex);
            }
        }

        public override string CreateLogFilePath()
        {
            string tmpFilePath = base.PrimitiveCreateLogFilePath();
            string dir = Path.GetDirectoryName(tmpFilePath);
            if (this._isFirstGetFilePath && this._config.IsAppend)
            {
                this._isFirstGetFilePath = false;
                if (Directory.Exists(dir))
                {
                    this.ClearExpireLogFile(dir);
                    tmpFilePath = base.GetLastLogFilePath(dir);
                    if (!string.IsNullOrWhiteSpace(tmpFilePath))
                    {
                        return tmpFilePath;
                    }
                }
                else
                {
                    Directory.CreateDirectory(dir);
                }
            }
            else
            {
                if (Directory.Exists(dir))
                {
                    this.ClearExpireLogFile(dir);
                }
                else
                {
                    Directory.CreateDirectory(dir);
                }
            }

            //abc_1.log
            string fileName = string.Format(@"{0}{1}{2}", this._fileNameWithoutExtension, this._index++, this._extension);
            return Path.Combine(dir, fileName);
        }

        protected override bool CompareLastLogFilePath(FileInfo lastLogFileInfo)
        {
            var time = DateTime.Now;
            var createTime = lastLogFileInfo.CreationTime;
            if (lastLogFileInfo.Length < this._config.MaxFileSize)
            {
                //最后一个文件是当天创建且小于目标大小
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检查日志文件路径是否是有效路径[有效返回true;无效返回false]
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected override bool CheckPath(string filePath)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            if (!fileNameWithoutExtension.StartsWith(this._fileNameWithoutExtension))
            {
                return false;
            }

            string indexStr = fileNameWithoutExtension.Substring(this._fileNameWithoutExtension.Length);
            int index;
            if (int.TryParse(indexStr, out index))
            {
                if (index > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void ClearExpireLogFile(string currentLogDir)
        {
            try
            {
                List<FileInfo> fileInfos = this.GetAllLogFileInfos();

                //按日志保留天数删除
                base.DeleteLogFileByDays(fileInfos, null);

                //按日志文件个数删除日志
                base.DeleteLogFileByFileCount(fileInfos, null);
            }
            catch (Exception ex)
            {
                LogSysInnerLog.OnRaiseLog("清除过期日志异常", ex);
            }
        }

        private List<FileInfo> GetAllLogFileInfos()
        {
            FileInfo[] srcFileInfos = base.RootDirectoryInfo.GetFiles(base._searchPattern, SearchOption.TopDirectoryOnly);
            List<FileInfo> fileInfos = new List<FileInfo>();
            foreach (var fileInfo in srcFileInfos)
            {
                if (this.CheckPath(fileInfo.FullName))
                {
                    fileInfos.Add(fileInfo);
                }
            }

            return fileInfos;
        }
    }
}
