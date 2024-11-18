
namespace QLCloseDoor
{
    partial class QLCloseDoor
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
            splitContainer1 = new SplitContainer();
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
            groupBox4 = new GroupBox();
            button3 = new Button();
            button2 = new Button();
            button1 = new Button();
            groupBox3 = new GroupBox();
            appStatus = new Label();
            stopApp = new Button();
            startApp = new Button();
            groupBox1 = new GroupBox();
            disconnectBtn = new Button();
            connectBtn = new Button();
            label2 = new Label();
            adbPort = new TextBox();
            label1 = new Label();
            adbHost = new TextBox();
            logTextbox = new TextBox();
            statusStrip1 = new StatusStrip();
            connectStatus = new ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            groupBox5.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(groupBox5);
            splitContainer1.Panel1.Controls.Add(groupBox4);
            splitContainer1.Panel1.Controls.Add(groupBox3);
            splitContainer1.Panel1.Controls.Add(groupBox1);
            splitContainer1.Panel1.ForeColor = SystemColors.Window;
            splitContainer1.Panel1MinSize = 290;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(logTextbox);
            splitContainer1.Panel2.ForeColor = SystemColors.Window;
            splitContainer1.Panel2MinSize = 290;
            splitContainer1.Size = new Size(898, 581);
            splitContainer1.SplitterDistance = 298;
            splitContainer1.TabIndex = 5;
            // 
            // groupBox5
            // 
            groupBox5.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
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
            groupBox5.Location = new Point(10, 412);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(281, 160);
            groupBox5.TabIndex = 8;
            groupBox5.TabStop = false;
            groupBox5.Text = "按钮坐标 (X, Y)";
            // 
            // saveBtn
            // 
            saveBtn.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            saveBtn.ForeColor = SystemColors.WindowText;
            saveBtn.Location = new Point(7, 117);
            saveBtn.Name = "saveBtn";
            saveBtn.Size = new Size(268, 33);
            saveBtn.TabIndex = 9;
            saveBtn.Text = "保存配置";
            saveBtn.UseVisualStyleBackColor = true;
            saveBtn.Click += saveBtn_Click;
            // 
            // btn3Y
            // 
            btn3Y.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn3Y.Location = new Point(172, 83);
            btn3Y.Name = "btn3Y";
            btn3Y.Size = new Size(103, 23);
            btn3Y.TabIndex = 8;
            btn3Y.Text = "938";
            // 
            // btn3X
            // 
            btn3X.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btn3X.Location = new Point(70, 83);
            btn3X.Name = "btn3X";
            btn3X.Size = new Size(96, 23);
            btn3X.TabIndex = 7;
            btn3X.Text = "541";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.ForeColor = SystemColors.WindowText;
            label5.Location = new Point(6, 86);
            label5.Name = "label5";
            label5.Size = new Size(43, 17);
            label5.TabIndex = 6;
            label5.Text = "按钮 3";
            // 
            // btn2Y
            // 
            btn2Y.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn2Y.Location = new Point(172, 54);
            btn2Y.Name = "btn2Y";
            btn2Y.Size = new Size(103, 23);
            btn2Y.TabIndex = 5;
            btn2Y.Text = "938";
            // 
            // btn2X
            // 
            btn2X.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btn2X.Location = new Point(70, 54);
            btn2X.Name = "btn2X";
            btn2X.Size = new Size(96, 23);
            btn2X.TabIndex = 4;
            btn2X.Text = "205";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = SystemColors.WindowText;
            label4.Location = new Point(6, 57);
            label4.Name = "label4";
            label4.Size = new Size(43, 17);
            label4.TabIndex = 3;
            label4.Text = "按钮 2";
            // 
            // btn1Y
            // 
            btn1Y.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn1Y.Location = new Point(172, 25);
            btn1Y.Name = "btn1Y";
            btn1Y.Size = new Size(103, 23);
            btn1Y.TabIndex = 2;
            btn1Y.Text = "641";
            // 
            // btn1X
            // 
            btn1X.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btn1X.Location = new Point(70, 25);
            btn1X.Name = "btn1X";
            btn1X.Size = new Size(96, 23);
            btn1X.TabIndex = 1;
            btn1X.Text = "541";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = SystemColors.WindowText;
            label3.Location = new Point(6, 28);
            label3.Name = "label3";
            label3.Size = new Size(43, 17);
            label3.TabIndex = 0;
            label3.Text = "按钮 1";
            // 
            // groupBox4
            // 
            groupBox4.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox4.Controls.Add(button3);
            groupBox4.Controls.Add(button2);
            groupBox4.Controls.Add(button1);
            groupBox4.Location = new Point(10, 262);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(281, 141);
            groupBox4.TabIndex = 7;
            groupBox4.TabStop = false;
            groupBox4.Text = "开门控制";
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button3.ForeColor = SystemColors.WindowText;
            button3.Location = new Point(7, 100);
            button3.Name = "button3";
            button3.Size = new Size(268, 33);
            button3.TabIndex = 6;
            button3.Text = "开门按钮 3";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button2.ForeColor = SystemColors.WindowText;
            button2.Location = new Point(7, 61);
            button2.Name = "button2";
            button2.Size = new Size(268, 33);
            button2.TabIndex = 5;
            button2.Text = "开门按钮 2";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button1.ForeColor = SystemColors.WindowText;
            button1.Location = new Point(7, 22);
            button1.Name = "button1";
            button1.Size = new Size(268, 33);
            button1.TabIndex = 4;
            button1.Text = "开门按钮 1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // groupBox3
            // 
            groupBox3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox3.Controls.Add(appStatus);
            groupBox3.Controls.Add(stopApp);
            groupBox3.Controls.Add(startApp);
            groupBox3.Location = new Point(9, 128);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(282, 126);
            groupBox3.TabIndex = 6;
            groupBox3.TabStop = false;
            groupBox3.Text = "应用控制";
            // 
            // appStatus
            // 
            appStatus.AutoSize = true;
            appStatus.ForeColor = SystemColors.WindowText;
            appStatus.Location = new Point(8, 25);
            appStatus.Name = "appStatus";
            appStatus.Size = new Size(68, 17);
            appStatus.TabIndex = 2;
            appStatus.Text = "应用未运行";
            // 
            // stopApp
            // 
            stopApp.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            stopApp.Enabled = false;
            stopApp.ForeColor = SystemColors.WindowText;
            stopApp.Location = new Point(7, 83);
            stopApp.Name = "stopApp";
            stopApp.Size = new Size(268, 33);
            stopApp.TabIndex = 1;
            stopApp.Text = "停止应用";
            stopApp.UseVisualStyleBackColor = true;
            stopApp.Click += stopApp_Click;
            // 
            // startApp
            // 
            startApp.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            startApp.Enabled = false;
            startApp.ForeColor = SystemColors.WindowText;
            startApp.Location = new Point(7, 45);
            startApp.Name = "startApp";
            startApp.Size = new Size(268, 33);
            startApp.TabIndex = 0;
            startApp.Text = "启动应用";
            startApp.UseVisualStyleBackColor = true;
            startApp.Click += startApp_Click;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(disconnectBtn);
            groupBox1.Controls.Add(connectBtn);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(adbPort);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(adbHost);
            groupBox1.Location = new Point(8, 7);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(283, 115);
            groupBox1.TabIndex = 5;
            groupBox1.TabStop = false;
            groupBox1.Text = "ADB  配置";
            // 
            // disconnectBtn
            // 
            disconnectBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            disconnectBtn.Enabled = false;
            disconnectBtn.ForeColor = SystemColors.WindowText;
            disconnectBtn.Location = new Point(146, 68);
            disconnectBtn.Name = "disconnectBtn";
            disconnectBtn.Size = new Size(130, 38);
            disconnectBtn.TabIndex = 5;
            disconnectBtn.Text = "断开";
            disconnectBtn.UseVisualStyleBackColor = true;
            disconnectBtn.Click += disconnectBtn_Click;
            // 
            // connectBtn
            // 
            connectBtn.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            connectBtn.ForeColor = SystemColors.WindowText;
            connectBtn.Location = new Point(6, 68);
            connectBtn.Name = "connectBtn";
            connectBtn.Size = new Size(130, 38);
            connectBtn.TabIndex = 4;
            connectBtn.Text = "连接";
            connectBtn.UseVisualStyleBackColor = true;
            connectBtn.Click += connectBtn_Click;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.ForeColor = SystemColors.WindowText;
            label2.Location = new Point(197, 19);
            label2.Name = "label2";
            label2.Size = new Size(61, 17);
            label2.TabIndex = 3;
            label2.Text = "ADB 端口";
            // 
            // adbPort
            // 
            adbPort.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            adbPort.Location = new Point(197, 39);
            adbPort.Name = "adbPort";
            adbPort.Size = new Size(79, 23);
            adbPort.TabIndex = 2;
            adbPort.Text = "62001";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.ForeColor = SystemColors.WindowText;
            label1.Location = new Point(6, 19);
            label1.Name = "label1";
            label1.Size = new Size(61, 17);
            label1.TabIndex = 1;
            label1.Text = "ADB 地址";
            // 
            // adbHost
            // 
            adbHost.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            adbHost.Location = new Point(6, 39);
            adbHost.Name = "adbHost";
            adbHost.Size = new Size(185, 23);
            adbHost.TabIndex = 0;
            adbHost.Text = "127.0.0.1";
            // 
            // logTextbox
            // 
            logTextbox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            logTextbox.BackColor = SystemColors.Control;
            logTextbox.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            logTextbox.ForeColor = SystemColors.WindowText;
            logTextbox.Location = new Point(-1, -1);
            logTextbox.Multiline = true;
            logTextbox.Name = "logTextbox";
            logTextbox.ReadOnly = true;
            logTextbox.ScrollBars = ScrollBars.Vertical;
            logTextbox.Size = new Size(598, 583);
            logTextbox.TabIndex = 1;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { connectStatus });
            statusStrip1.Location = new Point(0, 582);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(898, 22);
            statusStrip1.TabIndex = 6;
            statusStrip1.Text = "statusStrip1";
            // 
            // connectStatus
            // 
            connectStatus.Name = "connectStatus";
            connectStatus.Size = new Size(109, 17);
            connectStatus.Text = "状态：未连接 ADB";
            // 
            // QLCloseDoor
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(898, 604);
            Controls.Add(statusStrip1);
            Controls.Add(splitContainer1);
            MinimumSize = new Size(914, 643);
            Name = "QLCloseDoor";
            Text = "亲邻不开门 by Akkariin";
            FormClosing += FuckQL_FormClosing;
            Load += Form1_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private SplitContainer splitContainer1;
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
        private GroupBox groupBox4;
        private Button button3;
        private Button button2;
        private Button button1;
        private GroupBox groupBox3;
        private Label appStatus;
        private Button stopApp;
        private Button startApp;
        private GroupBox groupBox1;
        private Button disconnectBtn;
        private Button connectBtn;
        private Label label2;
        private TextBox adbPort;
        private Label label1;
        private TextBox adbHost;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel connectStatus;
        private TextBox logTextbox;
    }
}
