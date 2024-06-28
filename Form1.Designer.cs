
namespace FuckQL
{
    partial class FuckQL
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            groupBox1 = new GroupBox();
            disconnectBtn = new Button();
            connectBtn = new Button();
            label2 = new Label();
            adbPort = new TextBox();
            label1 = new Label();
            adbHost = new TextBox();
            groupBox2 = new GroupBox();
            logTextbox = new TextBox();
            groupBox3 = new GroupBox();
            appStatus = new Label();
            stopApp = new Button();
            startApp = new Button();
            groupBox4 = new GroupBox();
            button3 = new Button();
            button2 = new Button();
            button1 = new Button();
            groupBox5 = new GroupBox();
            saveBtn = new Button();
            btn3Y = new TextBox();
            btn3X = new TextBox();
            label5 = new Label();
            btn2Y = new TextBox();
            btn2X = new TextBox();
            label4 = new Label();
            btn1Y = new TextBox();
            btn1X = new TextBox();
            label3 = new Label();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox5.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(disconnectBtn);
            groupBox1.Controls.Add(connectBtn);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(adbPort);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(adbHost);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(256, 115);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "ADB  配置";
            groupBox1.Paint += groupBox1_Paint;
            // 
            // disconnectBtn
            // 
            disconnectBtn.Enabled = false;
            disconnectBtn.Location = new Point(133, 68);
            disconnectBtn.Name = "disconnectBtn";
            disconnectBtn.Size = new Size(117, 38);
            disconnectBtn.TabIndex = 5;
            disconnectBtn.Text = "断开";
            disconnectBtn.UseVisualStyleBackColor = true;
            disconnectBtn.Click += disconnectBtn_Click;
            // 
            // connectBtn
            // 
            connectBtn.Location = new Point(6, 68);
            connectBtn.Name = "connectBtn";
            connectBtn.Size = new Size(117, 38);
            connectBtn.TabIndex = 4;
            connectBtn.Text = "连接";
            connectBtn.UseVisualStyleBackColor = true;
            connectBtn.Click += connectBtn_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(172, 19);
            label2.Name = "label2";
            label2.Size = new Size(61, 17);
            label2.TabIndex = 3;
            label2.Text = "ADB 端口";
            // 
            // adbPort
            // 
            adbPort.Location = new Point(172, 39);
            adbPort.Name = "adbPort";
            adbPort.Size = new Size(77, 23);
            adbPort.TabIndex = 2;
            adbPort.Text = "62001";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 19);
            label1.Name = "label1";
            label1.Size = new Size(61, 17);
            label1.TabIndex = 1;
            label1.Text = "ADB 地址";
            // 
            // adbHost
            // 
            adbHost.Location = new Point(5, 39);
            adbHost.Name = "adbHost";
            adbHost.Size = new Size(161, 23);
            adbHost.TabIndex = 0;
            adbHost.Text = "127.0.0.1";
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox2.Controls.Add(logTextbox);
            groupBox2.Location = new Point(277, 12);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(611, 565);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "日志";
            groupBox2.Paint += groupBox1_Paint;
            // 
            // logTextbox
            // 
            logTextbox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            logTextbox.BackColor = SystemColors.Control;
            logTextbox.Location = new Point(10, 19);
            logTextbox.Multiline = true;
            logTextbox.Name = "logTextbox";
            logTextbox.ReadOnly = true;
            logTextbox.ScrollBars = ScrollBars.Vertical;
            logTextbox.Size = new Size(595, 536);
            logTextbox.TabIndex = 0;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(appStatus);
            groupBox3.Controls.Add(stopApp);
            groupBox3.Controls.Add(startApp);
            groupBox3.Location = new Point(13, 133);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(255, 126);
            groupBox3.TabIndex = 2;
            groupBox3.TabStop = false;
            groupBox3.Text = "应用控制";
            groupBox3.Paint += groupBox1_Paint;
            // 
            // appStatus
            // 
            appStatus.AutoSize = true;
            appStatus.Location = new Point(8, 25);
            appStatus.Name = "appStatus";
            appStatus.Size = new Size(68, 17);
            appStatus.TabIndex = 2;
            appStatus.Text = "应用未运行";
            // 
            // stopApp
            // 
            stopApp.Enabled = false;
            stopApp.Location = new Point(7, 83);
            stopApp.Name = "stopApp";
            stopApp.Size = new Size(241, 33);
            stopApp.TabIndex = 1;
            stopApp.Text = "停止应用";
            stopApp.UseVisualStyleBackColor = true;
            stopApp.Click += stopApp_Click;
            // 
            // startApp
            // 
            startApp.Enabled = false;
            startApp.Location = new Point(7, 45);
            startApp.Name = "startApp";
            startApp.Size = new Size(241, 33);
            startApp.TabIndex = 0;
            startApp.Text = "启动应用";
            startApp.UseVisualStyleBackColor = true;
            startApp.Click += startApp_Click;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(button3);
            groupBox4.Controls.Add(button2);
            groupBox4.Controls.Add(button1);
            groupBox4.Location = new Point(14, 267);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(254, 141);
            groupBox4.TabIndex = 3;
            groupBox4.TabStop = false;
            groupBox4.Text = "开门控制";
            groupBox4.Paint += groupBox1_Paint;
            // 
            // button3
            // 
            button3.Location = new Point(6, 100);
            button3.Name = "button3";
            button3.Size = new Size(241, 33);
            button3.TabIndex = 6;
            button3.Text = "开门按钮 3";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button2
            // 
            button2.Location = new Point(7, 61);
            button2.Name = "button2";
            button2.Size = new Size(241, 33);
            button2.TabIndex = 5;
            button2.Text = "开门按钮 2";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.Location = new Point(7, 22);
            button1.Name = "button1";
            button1.Size = new Size(241, 33);
            button1.TabIndex = 4;
            button1.Text = "开门按钮 1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(saveBtn);
            groupBox5.Controls.Add(btn3Y);
            groupBox5.Controls.Add(btn3X);
            groupBox5.Controls.Add(label5);
            groupBox5.Controls.Add(btn2Y);
            groupBox5.Controls.Add(btn2X);
            groupBox5.Controls.Add(label4);
            groupBox5.Controls.Add(btn1Y);
            groupBox5.Controls.Add(btn1X);
            groupBox5.Controls.Add(label3);
            groupBox5.Location = new Point(14, 417);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(254, 160);
            groupBox5.TabIndex = 4;
            groupBox5.TabStop = false;
            groupBox5.Text = "按钮坐标 (X, Y)";
            groupBox5.Paint += groupBox1_Paint;
            // 
            // saveBtn
            // 
            saveBtn.Location = new Point(6, 117);
            saveBtn.Name = "saveBtn";
            saveBtn.Size = new Size(241, 33);
            saveBtn.TabIndex = 9;
            saveBtn.Text = "保存配置";
            saveBtn.UseVisualStyleBackColor = true;
            saveBtn.Click += saveBtn_Click;
            // 
            // btn3Y
            // 
            btn3Y.Location = new Point(159, 83);
            btn3Y.Name = "btn3Y";
            btn3Y.Size = new Size(87, 23);
            btn3Y.TabIndex = 8;
            btn3Y.Text = "938";
            // 
            // btn3X
            // 
            btn3X.Location = new Point(66, 83);
            btn3X.Name = "btn3X";
            btn3X.Size = new Size(87, 23);
            btn3X.TabIndex = 7;
            btn3X.Text = "541";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(6, 86);
            label5.Name = "label5";
            label5.Size = new Size(43, 17);
            label5.TabIndex = 6;
            label5.Text = "按钮 3";
            // 
            // btn2Y
            // 
            btn2Y.Location = new Point(159, 54);
            btn2Y.Name = "btn2Y";
            btn2Y.Size = new Size(87, 23);
            btn2Y.TabIndex = 5;
            btn2Y.Text = "938";
            // 
            // btn2X
            // 
            btn2X.Location = new Point(66, 54);
            btn2X.Name = "btn2X";
            btn2X.Size = new Size(87, 23);
            btn2X.TabIndex = 4;
            btn2X.Text = "205";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(6, 57);
            label4.Name = "label4";
            label4.Size = new Size(43, 17);
            label4.TabIndex = 3;
            label4.Text = "按钮 2";
            // 
            // btn1Y
            // 
            btn1Y.Location = new Point(159, 25);
            btn1Y.Name = "btn1Y";
            btn1Y.Size = new Size(87, 23);
            btn1Y.TabIndex = 2;
            btn1Y.Text = "641";
            // 
            // btn1X
            // 
            btn1X.Location = new Point(66, 25);
            btn1X.Name = "btn1X";
            btn1X.Size = new Size(87, 23);
            btn1X.TabIndex = 1;
            btn1X.Text = "541";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 28);
            label3.Name = "label3";
            label3.Size = new Size(43, 17);
            label3.TabIndex = 0;
            label3.Text = "按钮 1";
            // 
            // FuckQL
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(898, 587);
            Controls.Add(groupBox5);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            MinimumSize = new Size(914, 626);
            Name = "FuckQL";
            Text = "FuckQL";
            FormClosing += FuckQL_FormClosing;
            Load += Form1_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            ResumeLayout(false);
        }


        #endregion

        private GroupBox groupBox1;
        private TextBox adbHost;
        private Button disconnectBtn;
        private Button connectBtn;
        private Label label2;
        private TextBox adbPort;
        private Label label1;
        private GroupBox groupBox2;
        private TextBox logTextbox;
        private GroupBox groupBox3;
        private Button stopApp;
        private Button startApp;
        private Label appStatus;
        private GroupBox groupBox4;
        private Button button3;
        private Button button2;
        private Button button1;
        private GroupBox groupBox5;
        private Button saveBtn;
        private TextBox btn3Y;
        private TextBox btn3X;
        private Label label5;
        private TextBox btn2Y;
        private TextBox btn2X;
        private Label label4;
        private TextBox btn1Y;
        private TextBox btn1X;
        private Label label3;
    }
}
