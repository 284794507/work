namespace SecurityTokens
{
    partial class FrmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCreate = new System.Windows.Forms.Button();
            this.txtVerificationCode = new System.Windows.Forms.TextBox();
            this.proBar = new System.Windows.Forms.ProgressBar();
            this.btnAlterKey = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCreate
            // 
            this.btnCreate.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCreate.Location = new System.Drawing.Point(220, 169);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(100, 38);
            this.btnCreate.TabIndex = 2;
            this.btnCreate.Text = "生成验证码";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // txtVerificationCode
            // 
            this.txtVerificationCode.Font = new System.Drawing.Font("微软雅黑", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtVerificationCode.ForeColor = System.Drawing.SystemColors.Highlight;
            this.txtVerificationCode.Location = new System.Drawing.Point(49, 12);
            this.txtVerificationCode.Name = "txtVerificationCode";
            this.txtVerificationCode.ReadOnly = true;
            this.txtVerificationCode.Size = new System.Drawing.Size(307, 46);
            this.txtVerificationCode.TabIndex = 3;
            this.txtVerificationCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // proBar
            // 
            this.proBar.Location = new System.Drawing.Point(49, 98);
            this.proBar.Name = "proBar";
            this.proBar.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.proBar.Size = new System.Drawing.Size(307, 29);
            this.proBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.proBar.TabIndex = 5;
            // 
            // btnAlterKey
            // 
            this.btnAlterKey.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnAlterKey.Location = new System.Drawing.Point(78, 169);
            this.btnAlterKey.Name = "btnAlterKey";
            this.btnAlterKey.Size = new System.Drawing.Size(90, 38);
            this.btnAlterKey.TabIndex = 6;
            this.btnAlterKey.Text = "修改密钥";
            this.btnAlterKey.UseVisualStyleBackColor = true;
            this.btnAlterKey.Click += new System.EventHandler(this.btnAlterKey_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 232);
            this.Controls.Add(this.btnAlterKey);
            this.Controls.Add(this.proBar);
            this.Controls.Add(this.txtVerificationCode);
            this.Controls.Add(this.btnCreate);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "安全令牌";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.TextBox txtVerificationCode;
        private System.Windows.Forms.ProgressBar proBar;
        private System.Windows.Forms.Button btnAlterKey;
    }
}

