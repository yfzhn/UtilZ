﻿namespace TestE.DB
{
    partial class FDBStruct
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ucdbStructControl = new TestE.DB.UCDBStructControl();
            this.SuspendLayout();
            // 
            // ucdbStructControl
            // 
            this.ucdbStructControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucdbStructControl.Location = new System.Drawing.Point(0, 0);
            this.ucdbStructControl.Name = "ucdbStructControl";
            this.ucdbStructControl.Size = new System.Drawing.Size(1384, 762);
            this.ucdbStructControl.TabIndex = 0;
            // 
            // FDBStruct
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1384, 762);
            this.Controls.Add(this.ucdbStructControl);
            this.Name = "FDBStruct";
            this.Text = "数据库结构";
            this.Load += new System.EventHandler(this.fdbsTRUCT_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private UCDBStructControl ucdbStructControl;
    }
}