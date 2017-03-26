namespace TerminalConfigTool
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
            this.txtIPOrDomain = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.btnSet = new System.Windows.Forms.Button();
            this.btnQuery = new System.Windows.Forms.Button();
            this.btnCkeckTime = new System.Windows.Forms.Button();
            this.dtPick = new System.Windows.Forms.DateTimePicker();
            this.txtTime = new System.Windows.Forms.TextBox();
            this.chkHand = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkLock = new System.Windows.Forms.CheckBox();
            this.txtChNo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCloseLamp = new System.Windows.Forms.Button();
            this.btnOpenLamp = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnQueryElecData = new System.Windows.Forms.Button();
            this.txtElecData = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.txtTerminalAddr = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btnSetGateway = new System.Windows.Forms.Button();
            this.btnQueryGateway = new System.Windows.Forms.Button();
            this.txtMac = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtGateway = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtMaskCode = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtStaticIP = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.combType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.txtSendNo = new System.Windows.Forms.TextBox();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btnStartUpgrade = new System.Windows.Forms.Button();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtIPOrDomain
            // 
            this.txtIPOrDomain.Location = new System.Drawing.Point(99, 24);
            this.txtIPOrDomain.Name = "txtIPOrDomain";
            this.txtIPOrDomain.Size = new System.Drawing.Size(194, 21);
            this.txtIPOrDomain.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "IP/域名：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "端口：";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(100, 60);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(194, 21);
            this.txtPort.TabIndex = 5;
            this.txtPort.TextChanged += new System.EventHandler(this.txtPort_TextChanged);
            this.txtPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPort_KeyPress);
            // 
            // btnSet
            // 
            this.btnSet.Location = new System.Drawing.Point(214, 87);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(79, 23);
            this.btnSet.TabIndex = 6;
            this.btnSet.Text = "设  置";
            this.btnSet.UseVisualStyleBackColor = true;
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // btnQuery
            // 
            this.btnQuery.Location = new System.Drawing.Point(100, 87);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(83, 23);
            this.btnQuery.TabIndex = 7;
            this.btnQuery.Text = "查  询";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // btnCkeckTime
            // 
            this.btnCkeckTime.Location = new System.Drawing.Point(181, 89);
            this.btnCkeckTime.Name = "btnCkeckTime";
            this.btnCkeckTime.Size = new System.Drawing.Size(105, 30);
            this.btnCkeckTime.TabIndex = 8;
            this.btnCkeckTime.Text = "校  时";
            this.btnCkeckTime.UseVisualStyleBackColor = true;
            this.btnCkeckTime.Click += new System.EventHandler(this.btnCkeckTime_Click);
            // 
            // dtPick
            // 
            this.dtPick.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtPick.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtPick.Location = new System.Drawing.Point(19, 30);
            this.dtPick.Name = "dtPick";
            this.dtPick.ShowUpDown = true;
            this.dtPick.Size = new System.Drawing.Size(267, 21);
            this.dtPick.TabIndex = 9;
            // 
            // txtTime
            // 
            this.txtTime.Location = new System.Drawing.Point(19, 57);
            this.txtTime.Name = "txtTime";
            this.txtTime.Size = new System.Drawing.Size(267, 21);
            this.txtTime.TabIndex = 10;
            this.txtTime.TextChanged += new System.EventHandler(this.txtTime_TextChanged);
            // 
            // chkHand
            // 
            this.chkHand.AutoSize = true;
            this.chkHand.Location = new System.Drawing.Point(19, 96);
            this.chkHand.Name = "chkHand";
            this.chkHand.Size = new System.Drawing.Size(72, 16);
            this.chkHand.TabIndex = 11;
            this.chkHand.Text = "手动输入";
            this.chkHand.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtIPOrDomain);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtPort);
            this.groupBox1.Controls.Add(this.btnSet);
            this.groupBox1.Controls.Add(this.btnQuery);
            this.groupBox1.Location = new System.Drawing.Point(37, 237);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(327, 123);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "网络参数";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtTime);
            this.groupBox2.Controls.Add(this.btnCkeckTime);
            this.groupBox2.Controls.Add(this.chkHand);
            this.groupBox2.Controls.Add(this.dtPick);
            this.groupBox2.Location = new System.Drawing.Point(389, 30);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(297, 131);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "校时";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkLock);
            this.groupBox3.Controls.Add(this.txtChNo);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.btnCloseLamp);
            this.groupBox3.Controls.Add(this.btnOpenLamp);
            this.groupBox3.Location = new System.Drawing.Point(37, 128);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(327, 103);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "开关灯";
            // 
            // chkLock
            // 
            this.chkLock.AutoSize = true;
            this.chkLock.Location = new System.Drawing.Point(221, 34);
            this.chkLock.Name = "chkLock";
            this.chkLock.Size = new System.Drawing.Size(72, 16);
            this.chkLock.TabIndex = 4;
            this.chkLock.Text = "是否上锁";
            this.chkLock.UseVisualStyleBackColor = true;
            // 
            // txtChNo
            // 
            this.txtChNo.Location = new System.Drawing.Point(80, 30);
            this.txtChNo.Name = "txtChNo";
            this.txtChNo.Size = new System.Drawing.Size(100, 21);
            this.txtChNo.TabIndex = 3;
            this.txtChNo.Text = "1";
            this.txtChNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtChNo_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "回路号：";
            // 
            // btnCloseLamp
            // 
            this.btnCloseLamp.Location = new System.Drawing.Point(224, 68);
            this.btnCloseLamp.Name = "btnCloseLamp";
            this.btnCloseLamp.Size = new System.Drawing.Size(70, 24);
            this.btnCloseLamp.TabIndex = 1;
            this.btnCloseLamp.Text = "关  灯";
            this.btnCloseLamp.UseVisualStyleBackColor = true;
            this.btnCloseLamp.Click += new System.EventHandler(this.btnCloseLamp_Click);
            // 
            // btnOpenLamp
            // 
            this.btnOpenLamp.Location = new System.Drawing.Point(127, 68);
            this.btnOpenLamp.Name = "btnOpenLamp";
            this.btnOpenLamp.Size = new System.Drawing.Size(70, 24);
            this.btnOpenLamp.TabIndex = 0;
            this.btnOpenLamp.Text = "开  灯";
            this.btnOpenLamp.UseVisualStyleBackColor = true;
            this.btnOpenLamp.Click += new System.EventHandler(this.btnOpenLamp_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnQueryElecData);
            this.groupBox4.Controls.Add(this.txtElecData);
            this.groupBox4.Location = new System.Drawing.Point(389, 166);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(297, 194);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "电参数";
            // 
            // btnQueryElecData
            // 
            this.btnQueryElecData.Location = new System.Drawing.Point(194, 158);
            this.btnQueryElecData.Name = "btnQueryElecData";
            this.btnQueryElecData.Size = new System.Drawing.Size(75, 23);
            this.btnQueryElecData.TabIndex = 1;
            this.btnQueryElecData.Text = "查  询";
            this.btnQueryElecData.UseVisualStyleBackColor = true;
            this.btnQueryElecData.Click += new System.EventHandler(this.btnQueryElecData_Click);
            // 
            // txtElecData
            // 
            this.txtElecData.Location = new System.Drawing.Point(18, 25);
            this.txtElecData.Multiline = true;
            this.txtElecData.Name = "txtElecData";
            this.txtElecData.Size = new System.Drawing.Size(251, 121);
            this.txtElecData.TabIndex = 0;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.txtTerminalAddr);
            this.groupBox5.Location = new System.Drawing.Point(37, 30);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(327, 92);
            this.groupBox5.TabIndex = 16;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "登录设备地址";
            // 
            // txtTerminalAddr
            // 
            this.txtTerminalAddr.Location = new System.Drawing.Point(16, 30);
            this.txtTerminalAddr.Name = "txtTerminalAddr";
            this.txtTerminalAddr.Size = new System.Drawing.Size(270, 21);
            this.txtTerminalAddr.TabIndex = 0;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btnSetGateway);
            this.groupBox6.Controls.Add(this.btnQueryGateway);
            this.groupBox6.Controls.Add(this.txtMac);
            this.groupBox6.Controls.Add(this.label10);
            this.groupBox6.Controls.Add(this.txtGateway);
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Controls.Add(this.txtMaskCode);
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Controls.Add(this.txtStaticIP);
            this.groupBox6.Controls.Add(this.label5);
            this.groupBox6.Controls.Add(this.combType);
            this.groupBox6.Controls.Add(this.label4);
            this.groupBox6.Location = new System.Drawing.Point(37, 376);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(327, 237);
            this.groupBox6.TabIndex = 17;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "网关参数";
            // 
            // btnSetGateway
            // 
            this.btnSetGateway.Location = new System.Drawing.Point(214, 194);
            this.btnSetGateway.Name = "btnSetGateway";
            this.btnSetGateway.Size = new System.Drawing.Size(79, 23);
            this.btnSetGateway.TabIndex = 8;
            this.btnSetGateway.Text = "设  置";
            this.btnSetGateway.UseVisualStyleBackColor = true;
            this.btnSetGateway.Click += new System.EventHandler(this.btnSetGateway_Click);
            // 
            // btnQueryGateway
            // 
            this.btnQueryGateway.Location = new System.Drawing.Point(100, 194);
            this.btnQueryGateway.Name = "btnQueryGateway";
            this.btnQueryGateway.Size = new System.Drawing.Size(83, 23);
            this.btnQueryGateway.TabIndex = 8;
            this.btnQueryGateway.Text = "查  询";
            this.btnQueryGateway.UseVisualStyleBackColor = true;
            this.btnQueryGateway.Click += new System.EventHandler(this.btnQueryGateway_Click);
            // 
            // txtMac
            // 
            this.txtMac.Location = new System.Drawing.Point(99, 160);
            this.txtMac.Name = "txtMac";
            this.txtMac.Size = new System.Drawing.Size(194, 21);
            this.txtMac.TabIndex = 8;
            this.txtMac.Text = "1:1:1:1:1:1";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(14, 163);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(59, 12);
            this.label10.TabIndex = 5;
            this.label10.Text = "MAC地址：";
            // 
            // txtGateway
            // 
            this.txtGateway.Location = new System.Drawing.Point(100, 122);
            this.txtGateway.Name = "txtGateway";
            this.txtGateway.Size = new System.Drawing.Size(194, 21);
            this.txtGateway.TabIndex = 14;
            this.txtGateway.Text = "192.168.1.1";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 125);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 13;
            this.label7.Text = "网关：";
            // 
            // txtMaskCode
            // 
            this.txtMaskCode.Location = new System.Drawing.Point(100, 90);
            this.txtMaskCode.Name = "txtMaskCode";
            this.txtMaskCode.Size = new System.Drawing.Size(194, 21);
            this.txtMaskCode.TabIndex = 12;
            this.txtMaskCode.Text = "255.255.255.0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 93);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 11;
            this.label6.Text = "子网掩码：";
            // 
            // txtStaticIP
            // 
            this.txtStaticIP.Location = new System.Drawing.Point(100, 58);
            this.txtStaticIP.Name = "txtStaticIP";
            this.txtStaticIP.Size = new System.Drawing.Size(194, 21);
            this.txtStaticIP.TabIndex = 8;
            this.txtStaticIP.Text = "192.168.1.100";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "静态IP：";
            // 
            // combType
            // 
            this.combType.FormattingEnabled = true;
            this.combType.Items.AddRange(new object[] {
            "静态IP",
            "动态IP"});
            this.combType.Location = new System.Drawing.Point(100, 26);
            this.combType.Name = "combType";
            this.combType.Size = new System.Drawing.Size(121, 20);
            this.combType.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "类型：";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.txtSendNo);
            this.groupBox7.Controls.Add(this.txtTotal);
            this.groupBox7.Controls.Add(this.label9);
            this.groupBox7.Controls.Add(this.label8);
            this.groupBox7.Controls.Add(this.btnStartUpgrade);
            this.groupBox7.Controls.Add(this.btnSelectFile);
            this.groupBox7.Controls.Add(this.txtFileName);
            this.groupBox7.Location = new System.Drawing.Point(389, 376);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(297, 237);
            this.groupBox7.TabIndex = 18;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "远程升级";
            // 
            // txtSendNo
            // 
            this.txtSendNo.Location = new System.Drawing.Point(111, 119);
            this.txtSendNo.Name = "txtSendNo";
            this.txtSendNo.ReadOnly = true;
            this.txtSendNo.Size = new System.Drawing.Size(58, 21);
            this.txtSendNo.TabIndex = 17;
            // 
            // txtTotal
            // 
            this.txtTotal.Location = new System.Drawing.Point(111, 79);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.ReadOnly = true;
            this.txtTotal.Size = new System.Drawing.Size(58, 21);
            this.txtTotal.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(17, 122);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(77, 12);
            this.label9.TabIndex = 16;
            this.label9.Text = "已发送包数：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 82);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 15;
            this.label8.Text = "总包数：";
            // 
            // btnStartUpgrade
            // 
            this.btnStartUpgrade.Location = new System.Drawing.Point(194, 117);
            this.btnStartUpgrade.Name = "btnStartUpgrade";
            this.btnStartUpgrade.Size = new System.Drawing.Size(75, 23);
            this.btnStartUpgrade.TabIndex = 9;
            this.btnStartUpgrade.Text = "开始升级";
            this.btnStartUpgrade.UseVisualStyleBackColor = true;
            this.btnStartUpgrade.Click += new System.EventHandler(this.btnStartUpgrade_Click);
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Location = new System.Drawing.Point(194, 77);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(75, 23);
            this.btnSelectFile.TabIndex = 2;
            this.btnSelectFile.Text = "选择文件";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(19, 35);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(250, 21);
            this.txtFileName.TabIndex = 8;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(718, 632);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmMain";
            this.Text = "终端测试工具";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmMain_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TextBox txtIPOrDomain;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Button btnSet;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.Button btnCkeckTime;
        private System.Windows.Forms.DateTimePicker dtPick;
        private System.Windows.Forms.TextBox txtTime;
        private System.Windows.Forms.CheckBox chkHand;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnCloseLamp;
        private System.Windows.Forms.Button btnOpenLamp;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnQueryElecData;
        private System.Windows.Forms.TextBox txtElecData;
        private System.Windows.Forms.CheckBox chkLock;
        private System.Windows.Forms.TextBox txtChNo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox txtTerminalAddr;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox txtStaticIP;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox combType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSetGateway;
        private System.Windows.Forms.Button btnQueryGateway;
        private System.Windows.Forms.TextBox txtGateway;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtMaskCode;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Button btnStartUpgrade;
        private System.Windows.Forms.TextBox txtSendNo;
        private System.Windows.Forms.TextBox txtTotal;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtMac;
        private System.Windows.Forms.Label label10;
    }
}

