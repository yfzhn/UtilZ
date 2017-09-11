﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UtilZ.Components.ConfigBLL;
using UtilZ.Components.ConfigModel;
using UtilZ.Lib.Base.DataStruct.UIBinding;
using UtilZ.Lib.Base.Log;

namespace UtilZ.Components.ConfigManager.UCViews
{
    public partial class FServiceConfigParaKeyValueEdit : Form
    {
        private readonly ConfigLogic _configLogic;
        private ConfigParaServiceMap _serviceMap;
        private readonly UIBindingList<ConfigParaKeyValue2> _paraList = new UIBindingList<ConfigParaKeyValue2>();
        private List<int> _srcConfigParaIds;

        public FServiceConfigParaKeyValueEdit()
        {
            InitializeComponent();
        }

        public FServiceConfigParaKeyValueEdit(ConfigLogic configLogic, ConfigParaServiceMap serviceMap) : this()
        {
            this._configLogic = configLogic;
            this._serviceMap = serviceMap;
        }

        private void FServiceConfigParaKeyValueEdit_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }

            try
            {
                this.Text = string.Format("{0}-{1}", this.Text, this._serviceMap.Name);
                txtServiceId.Text = this._serviceMap.ServiceMapID.ToString();
                txtServiceName.Text = this._serviceMap.Name;
                txtServiceDes.Text = this._serviceMap.Des;

                List<ConfigParaKeyValue> allConfigParaKeyValues = this._configLogic.GetAllConfigParaKeyValue();
                var serviceConfigParas = this._configLogic.GetConfigParaKeyValueByServiceId(this._serviceMap.ID);
                this._srcConfigParaIds = (from tmpItem in serviceConfigParas select tmpItem.ID).ToList();

                foreach (var item in allConfigParaKeyValues)
                {
                    this._paraList.Add(new ConfigParaKeyValue2(item, this._srcConfigParaIds.Contains(item.ID)));
                }

                pgConfigParaKeyValue.ShowData("FServiceConfigParaKeyValueEdit.ConfigParaKeyValue2", this._paraList, null, new string[] { nameof(ConfigParaKeyValue2.IsSelected) });
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void tsmiCheckAll_Click(object sender, EventArgs e)
        {
            foreach (var item in this._paraList)
            {
                if (!item.IsSelected)
                {
                    item.IsSelected = true;
                }
            }
        }

        private void tsmiDirectionCheck_Click(object sender, EventArgs e)
        {
            foreach (var item in this._paraList)
            {
                item.IsSelected = !item.IsSelected;
            }
        }

        private void tsmiUnCheck_Click(object sender, EventArgs e)
        {
            foreach (var item in this._paraList)
            {
                if (item.IsSelected)
                {
                    item.IsSelected = false;
                }
            }
        }

        private void tsmiSave_Click(object sender, EventArgs e)
        {
            try
            {
                var newParas = (from tmpItem in this._paraList where tmpItem.IsSelected select tmpItem).ToList();
                var newParaIds = (from tmpItem in newParas select tmpItem.ID).ToList();

                List<int> addIDs = (from tmpItem in newParas where !this._srcConfigParaIds.Contains(tmpItem.ID) select tmpItem.ID).ToList();
                List<int> delIDs = (from tmpID in this._srcConfigParaIds where !newParaIds.Contains(tmpID) select tmpID).ToList();

                this._configLogic.ModifyValidDomain(addIDs, delIDs, _serviceMap);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Loger.Error(ex);
            }

        }
    }
}
