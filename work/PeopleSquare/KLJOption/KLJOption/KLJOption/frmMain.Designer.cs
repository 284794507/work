namespace KLJOption
{
    partial class frmMain
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
            this.btnReadme = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lvwDev = new System.Windows.Forms.ListView();
            this.colDevMac = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLastTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboSecretWay = new System.Windows.Forms.ComboBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtAPName = new System.Windows.Forms.TextBox();
            this.btnApplyAP = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cboReportCycle = new System.Windows.Forms.ComboBox();
            this.cboReportMode = new System.Windows.Forms.ComboBox();
            this.cboWifiChannel = new System.Windows.Forms.ComboBox();
            this.cboModuleNo = new System.Windows.Forms.ComboBox();
            this.btnApplyModule = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnRestart = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnServer = new System.Windows.Forms.Button();
            this.lblMsg = new System.Windows.Forms.Label();
            this.txtDevMAC = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnReadme
            // 
            this.btnReadme.Location = new System.Drawing.Point(449, 12);
            this.btnReadme.Margin = new System.Windows.Forms.Padding(2);
            this.btnReadme.Name = "btnReadme";
            this.btnReadme.Size = new System.Drawing.Size(86, 30);
            this.btnReadme.TabIndex = 0;
            this.btnReadme.Text = "版本说明(&V)";
            this.btnReadme.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "选择设备";
            // 
            // lvwDev
            // 
            this.lvwDev.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colDevMac,
            this.colLastTime});
            this.lvwDev.Location = new System.Drawing.Point(9, 46);
            this.lvwDev.Margin = new System.Windows.Forms.Padding(2);
            this.lvwDev.MultiSelect = false;
            this.lvwDev.Name = "lvwDev";
            this.lvwDev.Size = new System.Drawing.Size(294, 326);
            this.lvwDev.TabIndex = 2;
            this.lvwDev.UseCompatibleStateImageBehavior = false;
            this.lvwDev.View = System.Windows.Forms.View.Details;
            this.lvwDev.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvwDev_ItemSelectionChanged);
            // 
            // colDevMac
            // 
            this.colDevMac.Text = "采样设备MAC地址";
            this.colDevMac.Width = 169;
            // 
            // colLastTime
            // 
            this.colLastTime.Text = "最新获取数据时间";
            this.colLastTime.Width = 132;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cboSecretWay);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.txtAPName);
            this.groupBox1.Controls.Add(this.btnApplyAP);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(310, 46);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(328, 95);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "设置应用AP";
            // 
            // cboSecretWay
            // 
            this.cboSecretWay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSecretWay.FormattingEnabled = true;
            this.cboSecretWay.Items.AddRange(new object[] {
            "WPA/WPA2",
            "WPA-PSK/WPA2-PSK"});
            this.cboSecretWay.Location = new System.Drawing.Point(67, 66);
            this.cboSecretWay.Margin = new System.Windows.Forms.Padding(2);
            this.cboSecretWay.Name = "cboSecretWay";
            this.cboSecretWay.Size = new System.Drawing.Size(180, 20);
            this.cboSecretWay.TabIndex = 6;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(68, 41);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(2);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(179, 21);
            this.txtPassword.TabIndex = 5;
            // 
            // txtAPName
            // 
            this.txtAPName.Location = new System.Drawing.Point(68, 15);
            this.txtAPName.Margin = new System.Windows.Forms.Padding(2);
            this.txtAPName.Name = "txtAPName";
            this.txtAPName.Size = new System.Drawing.Size(179, 21);
            this.txtAPName.TabIndex = 4;
            // 
            // btnApplyAP
            // 
            this.btnApplyAP.Location = new System.Drawing.Point(255, 66);
            this.btnApplyAP.Margin = new System.Windows.Forms.Padding(2);
            this.btnApplyAP.Name = "btnApplyAP";
            this.btnApplyAP.Size = new System.Drawing.Size(62, 22);
            this.btnApplyAP.TabIndex = 3;
            this.btnApplyAP.Text = "应用";
            this.btnApplyAP.UseVisualStyleBackColor = true;
            this.btnApplyAP.Click += new System.EventHandler(this.btnApplyAP_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 73);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 2;
            this.label8.Text = "加密方式";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 48);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "密码";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 23);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "AP名称";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cboReportCycle);
            this.groupBox2.Controls.Add(this.cboReportMode);
            this.groupBox2.Controls.Add(this.cboWifiChannel);
            this.groupBox2.Controls.Add(this.cboModuleNo);
            this.groupBox2.Controls.Add(this.btnApplyModule);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(310, 146);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(328, 132);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "设置模块参数";
            // 
            // cboReportCycle
            // 
            this.cboReportCycle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboReportCycle.FormattingEnabled = true;
            this.cboReportCycle.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9"});
            this.cboReportCycle.Location = new System.Drawing.Point(107, 101);
            this.cboReportCycle.Margin = new System.Windows.Forms.Padding(2);
            this.cboReportCycle.Name = "cboReportCycle";
            this.cboReportCycle.Size = new System.Drawing.Size(90, 20);
            this.cboReportCycle.TabIndex = 8;
            // 
            // cboReportMode
            // 
            this.cboReportMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboReportMode.FormattingEnabled = true;
            this.cboReportMode.Items.AddRange(new object[] {
            "1"});
            this.cboReportMode.Location = new System.Drawing.Point(106, 75);
            this.cboReportMode.Margin = new System.Windows.Forms.Padding(2);
            this.cboReportMode.Name = "cboReportMode";
            this.cboReportMode.Size = new System.Drawing.Size(91, 20);
            this.cboReportMode.TabIndex = 7;
            // 
            // cboWifiChannel
            // 
            this.cboWifiChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboWifiChannel.FormattingEnabled = true;
            this.cboWifiChannel.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13"});
            this.cboWifiChannel.Location = new System.Drawing.Point(107, 50);
            this.cboWifiChannel.Margin = new System.Windows.Forms.Padding(2);
            this.cboWifiChannel.Name = "cboWifiChannel";
            this.cboWifiChannel.Size = new System.Drawing.Size(90, 20);
            this.cboWifiChannel.TabIndex = 6;
            // 
            // cboModuleNo
            // 
            this.cboModuleNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboModuleNo.FormattingEnabled = true;
            this.cboModuleNo.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.cboModuleNo.Location = new System.Drawing.Point(106, 25);
            this.cboModuleNo.Margin = new System.Windows.Forms.Padding(2);
            this.cboModuleNo.Name = "cboModuleNo";
            this.cboModuleNo.Size = new System.Drawing.Size(92, 20);
            this.cboModuleNo.TabIndex = 5;
            // 
            // btnApplyModule
            // 
            this.btnApplyModule.Location = new System.Drawing.Point(255, 99);
            this.btnApplyModule.Margin = new System.Windows.Forms.Padding(2);
            this.btnApplyModule.Name = "btnApplyModule";
            this.btnApplyModule.Size = new System.Drawing.Size(59, 22);
            this.btnApplyModule.TabIndex = 4;
            this.btnApplyModule.Text = "应用";
            this.btnApplyModule.UseVisualStyleBackColor = true;
            this.btnApplyModule.Click += new System.EventHandler(this.btnApplyModule_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 106);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 12);
            this.label7.TabIndex = 3;
            this.label7.Text = "汇报间隔(分钟)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 80);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "汇报模式";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 52);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "WIFI频道";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 27);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "选择模块(1..3)";
            // 
            // btnRestart
            // 
            this.btnRestart.Location = new System.Drawing.Point(310, 343);
            this.btnRestart.Margin = new System.Windows.Forms.Padding(2);
            this.btnRestart.Name = "btnRestart";
            this.btnRestart.Size = new System.Drawing.Size(89, 29);
            this.btnRestart.TabIndex = 5;
            this.btnRestart.Text = "重启设备(&R)";
            this.btnRestart.UseVisualStyleBackColor = true;
            this.btnRestart.Click += new System.EventHandler(this.btnRestart_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(563, 13);
            this.btnExit.Margin = new System.Windows.Forms.Padding(2);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(76, 29);
            this.btnExit.TabIndex = 6;
            this.btnExit.Text = "退出(&X)";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(423, 343);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(2);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(89, 29);
            this.btnConnect.TabIndex = 7;
            this.btnConnect.Text = "连接通讯网关";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnServer
            // 
            this.btnServer.Location = new System.Drawing.Point(536, 343);
            this.btnServer.Margin = new System.Windows.Forms.Padding(2);
            this.btnServer.Name = "btnServer";
            this.btnServer.Size = new System.Drawing.Size(101, 29);
            this.btnServer.TabIndex = 8;
            this.btnServer.Text = "测试通讯服务器";
            this.btnServer.UseVisualStyleBackColor = true;
            this.btnServer.Click += new System.EventHandler(this.btnServer_Click);
            // 
            // lblMsg
            // 
            this.lblMsg.Location = new System.Drawing.Point(310, 312);
            this.lblMsg.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(328, 22);
            this.lblMsg.TabIndex = 9;
            this.lblMsg.Click += new System.EventHandler(this.lblMsg_Click);
            // 
            // txtDevMAC
            // 
            this.txtDevMAC.Location = new System.Drawing.Point(423, 290);
            this.txtDevMAC.Margin = new System.Windows.Forms.Padding(2);
            this.txtDevMAC.Name = "txtDevMAC";
            this.txtDevMAC.Size = new System.Drawing.Size(217, 21);
            this.txtDevMAC.TabIndex = 11;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(310, 292);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(113, 12);
            this.label9.TabIndex = 12;
            this.label9.Text = "选用此设备MAC地址:";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 405);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtDevMAC);
            this.Controls.Add(this.lblMsg);
            this.Controls.Add(this.btnServer);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnRestart);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lvwDev);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnReadme);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmMain";
            this.Text = "路辉客流计设置与测试工具V1.00";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnReadme;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView lvwDev;
        private System.Windows.Forms.ColumnHeader colDevMac;
        private System.Windows.Forms.ColumnHeader colLastTime;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnApplyAP;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnApplyModule;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnRestart;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.ComboBox cboSecretWay;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtAPName;
        private System.Windows.Forms.ComboBox cboReportCycle;
        private System.Windows.Forms.ComboBox cboReportMode;
        private System.Windows.Forms.ComboBox cboWifiChannel;
        private System.Windows.Forms.ComboBox cboModuleNo;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnServer;
        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.TextBox txtDevMAC;
        private System.Windows.Forms.Label label9;
    }
}

