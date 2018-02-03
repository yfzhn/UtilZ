﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using UtilZ.Lib.Base.Extend;

namespace TestUtilZ
{
    public partial class FTestFTP : Form
    {
        private readonly NExtendFTP _extendFTP;
        public FTestFTP()
        {
            InitializeComponent();

            string ftpUrl = @"ftp://192.168.2.6/";
            string ftpUserName = string.Empty;
            string ftpPassword = string.Empty;
            this._extendFTP = new NExtendFTP(ftpUrl, ftpUserName, ftpPassword);
        }

        private void FTestFTP_Load(object sender, EventArgs e)
        {
            string regStr = @"^\d{2}-\d{2}-\d{2}\s+\d{2}:\d{2}[A,P]{1}M\s+?<length>\d+\s+?<name>\.+$";
            string str = @"02-03-18  05:31PM                15854 调用微软的WINDOWS操作系统本身附带的G723 ACM API函数.docx";
            regStr = @"^\d{2}-\d{2}-\d{2}\s+\d{2}:\d{2}[A,P,a,p]{1}[M,m]{1}\s+";
            Match match = Regex.Match(str, regStr);

            regStr = @"^\d{2}-\d{2}-\d{2}\s+\d{2}:\d{2}[A,P,a,p]{1}[M,m]{1}\s+(?<length>\d+)";

            regStr = @"^\d{2}-\d{2}-\d{2}\s+\d{2}:\d{2}[A,P,a,p]{1}[M,m]{1}\s+(?<length>\d+)\s+(?<name>\.+)$";

            regStr = @"(?<pre>^\d{2}-\d{2}-\d{2}\s+\d{2}:\d{2}[A,P,a,p]{1}[M,m]{1}\s+(?<length>\d+)\s+)";
            match = Regex.Match(str, regStr);


            str = @"02-02-18  09:10PM       <DIR>          w 2";
            regStr = @"(?<pre>^(?<year>\d{2}-\d{2}-\d{2})\s+(?<time>\d{2}:\d{2}[A,P,a,p]{1}[M,m]{1})\s+<[D,d]{1}[I,i]{1}[R,r]{1}>\s+)";
            match = Regex.Match(str, regStr);
            if (match.Success)
            {

            }
        }

        private void btnDirExists_Click(object sender, EventArgs e)
        {
            try
            {
                string dir = @"q1";
                bool ret = this._extendFTP.DirectoryExists(dir);

                dir = @"q2";
                bool ret2 = this._extendFTP.DirectoryExists(dir);

                dir = @"q1\aa";
                bool ret3 = this._extendFTP.DirectoryExists(dir);

                dir = @"q1\qq";
                bool ret4 = this._extendFTP.DirectoryExists(dir);
            }
            catch (Exception ex)
            {
                rtxtMsg.AppendText(ex.Message);
                rtxtMsg.AppendText(Environment.NewLine);
            }
        }

        private void btnCreateDir_Click(object sender, EventArgs e)
        {
            try
            {
                string dir = @"bb";
                this._extendFTP.CreateDirectory(dir);

                dir = @"a\b\c";
                this._extendFTP.CreateDirectory(dir);
            }
            catch (Exception ex)
            {
                rtxtMsg.AppendText(ex.Message);
                rtxtMsg.AppendText(Environment.NewLine);
            }
        }

        private void btnFileExist_Click(object sender, EventArgs e)
        {
            try
            {
                string remoteFilePath = @"mongodb-win32-x86_64-2008plus-ssl-3.4.9-signed.msi";
                bool ret = this._extendFTP.FileExists(remoteFilePath);

                remoteFilePath = @"abc.msi";
                bool ret2 = this._extendFTP.FileExists(remoteFilePath);
            }
            catch (Exception ex)
            {
                rtxtMsg.AppendText(ex.Message);
                rtxtMsg.AppendText(Environment.NewLine);
            }
        }

        private void btnFileLength_Click(object sender, EventArgs e)
        {
            try
            {
                string remoteFilePath = @"mongodb-win32-x86_64-2008plus-ssl-3.4.9-signed.msi";
                long length = this._extendFTP.GetFileLength(remoteFilePath);
            }
            catch (Exception ex)
            {
                rtxtMsg.AppendText(ex.Message);
                rtxtMsg.AppendText(Environment.NewLine);
            }
        }

        private void btnUploadFile_Click(object sender, EventArgs e)
        {
            try
            {
                string localFilePath, remoteFilePath;

                localFilePath = @"F:\Soft\KKSetup_2000.exe";
                remoteFilePath = @"KKSetup_2000.exe";
                this._extendFTP.UploadFile(localFilePath, remoteFilePath, 2048, true);

                localFilePath = @"F:\Soft\dotNetFx40_Full_x86_x64.exe";
                remoteFilePath = @"Soft\dotNetFx40_Full_x86_x64.exe";
                this._extendFTP.UploadFile(localFilePath, remoteFilePath, 2048, true);



                localFilePath = @"F:\Soft\feiq.rar";
                remoteFilePath = @"feiq.rar";
                this._extendFTP.UploadFile(localFilePath, remoteFilePath, 2048, true);
            }
            catch (Exception ex)
            {
                rtxtMsg.AppendText(ex.Message);
                rtxtMsg.AppendText(Environment.NewLine);
            }
        }

        private void btnDownloadFile_Click(object sender, EventArgs e)
        {
            try
            {
                string localFilePath = @"G:\Tmp\test\dotNetFx40_Full_x86_x64.exe";
                string remoteFilePath = @"Soft\dotNetFx40_Full_x86_x64.exe";
                this._extendFTP.DownloadFile(localFilePath, remoteFilePath, 2048, true);

                localFilePath = @"G:\Tmp\test\feiq.rar";
                remoteFilePath = @"feiq.rar";
                this._extendFTP.DownloadFile(localFilePath, remoteFilePath, 2048, false);
            }
            catch (Exception ex)
            {
                rtxtMsg.AppendText(ex.Message);
                rtxtMsg.AppendText(Environment.NewLine);
            }
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            string oldFileName = "流程图.vsd";
            string newFileName = "FlowImg.vsd";
            try
            {
                this._extendFTP.Rename(oldFileName, newFileName);
            }
            catch (Exception ex)
            {
                rtxtMsg.AppendText(ex.Message);
                rtxtMsg.AppendText(Environment.NewLine);
            }
        }

        private void btnDeleteFile_Click(object sender, EventArgs e)
        {
            try
            {
                string remoteFilePath = @"feiq.rar";
                this._extendFTP.DeleteFile(remoteFilePath);

                remoteFilePath = @"Soft/dotNetFx40_Full_x86_x64.exe";
                this._extendFTP.DeleteFile(remoteFilePath);
            }
            catch (Exception ex)
            {
                rtxtMsg.AppendText(ex.Message);
                rtxtMsg.AppendText(Environment.NewLine);
            }
        }

        private void btnDirList_Click(object sender, EventArgs e)
        {
            try
            {
                string remoteDir = null;
                var dirList = this._extendFTP.GetDirectoryList(remoteDir);

                remoteDir = @"a";
                var dirList2 = this._extendFTP.GetDirectoryList(remoteDir);
            }
            catch (Exception ex)
            {
                rtxtMsg.AppendText(ex.Message);
                rtxtMsg.AppendText(Environment.NewLine);
            }
        }

        private void btnFileList_Click(object sender, EventArgs e)
        {
            try
            {
                string remoteDir = null;
                var dirList = this._extendFTP.GetFileList(remoteDir);

                remoteDir = @"a";
                var dirList2 = this._extendFTP.GetFileList(remoteDir);
            }
            catch (Exception ex)
            {
                rtxtMsg.AppendText(ex.Message);
                rtxtMsg.AppendText(Environment.NewLine);
            }
        }

        private void btnUploadDir_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                string localDir = @"G:\Tmp_Study\2014-11-4";
                try
                {
                    string remoteDir = @"2014-11-4";
                    this._extendFTP.UploadDirectory(localDir, remoteDir);
                }
                catch (Exception ex)
                {
                    this.Invoke(new Action(() =>
                    {
                        rtxtMsg.AppendText(ex.Message);
                        rtxtMsg.AppendText(Environment.NewLine);
                    }));
                }
            });
        }

        private void btnDownloadDir_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                string localDir = @"G:\Tmp\test\2014-11-4";
                try
                {
                    string remoteDir = @"a\2014-11-4";
                    this._extendFTP.DownloadDirectory(localDir, remoteDir);
                }
                catch (Exception ex)
                {
                    this.Invoke(new Action(() =>
                    {
                        rtxtMsg.AppendText(ex.Message);
                        rtxtMsg.AppendText(Environment.NewLine);
                    }));
                }
            });
        }

        private void btnDeleteDir_Click(object sender, EventArgs e)
        {
            try
            {
                string remoteDir = @"Soft";
                this._extendFTP.DeleteDirectory(remoteDir);
            }
            catch (Exception ex)
            {
                rtxtMsg.AppendText(ex.Message);
                rtxtMsg.AppendText(Environment.NewLine);
            }

        }
    }
}