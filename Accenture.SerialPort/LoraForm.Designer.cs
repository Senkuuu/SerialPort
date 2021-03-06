﻿namespace Accenture.SerialPort
{
    partial class LoraForm
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
            this.components = new System.ComponentModel.Container();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.txt_ip = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.txt_port = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel6 = new System.Windows.Forms.ToolStripLabel();
            this.tb_appeui = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.btn_kq = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel10 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel11 = new System.Windows.Forms.ToolStripLabel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.time = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lab_img = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cbox_auto = new System.Windows.Forms.CheckBox();
            this.cbox_manual = new System.Windows.Forms.CheckBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.checkedListBox2 = new System.Windows.Forms.CheckedListBox();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timeTest = new System.Windows.Forms.Label();
            this.index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.wakeuptype = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.systime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.moteeui = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.freq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.datr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rssi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lsnr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.power = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.wakeup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.temp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hexdata = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.strdata = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ercode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.JsonDataDemo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.txt_ip,
            this.toolStripLabel2,
            this.txt_port,
            this.toolStripLabel6,
            this.tb_appeui,
            this.toolStripLabel3,
            this.btn_kq,
            this.toolStripLabel4,
            this.toolStripLabel5,
            this.toolStripButton1,
            this.toolStripLabel10,
            this.toolStripButton2,
            this.toolStripLabel11});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1230, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(91, 22);
            this.toolStripLabel1.Text = "服务器（IP）：";
            // 
            // txt_ip
            // 
            this.txt_ip.Name = "txt_ip";
            this.txt_ip.Size = new System.Drawing.Size(121, 25);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(52, 22);
            this.toolStripLabel2.Text = "  端口：";
            // 
            // txt_port
            // 
            this.txt_port.Name = "txt_port";
            this.txt_port.Size = new System.Drawing.Size(100, 25);
            this.txt_port.Text = "1701";
            // 
            // toolStripLabel6
            // 
            this.toolStripLabel6.Name = "toolStripLabel6";
            this.toolStripLabel6.Size = new System.Drawing.Size(52, 22);
            this.toolStripLabel6.Text = "AppEui:";
            // 
            // tb_appeui
            // 
            this.tb_appeui.Name = "tb_appeui";
            this.tb_appeui.Size = new System.Drawing.Size(150, 25);
            this.tb_appeui.Text = "0102030405060700";
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(28, 22);
            this.toolStripLabel3.Text = "     ";
            // 
            // btn_kq
            // 
            this.btn_kq.BackColor = System.Drawing.SystemColors.Control;
            this.btn_kq.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_kq.Image = global::Accenture.SerialPort.Properties.Resources.server;
            this.btn_kq.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_kq.Name = "btn_kq";
            this.btn_kq.Size = new System.Drawing.Size(52, 22);
            this.btn_kq.Text = "开启";
            this.btn_kq.Click += new System.EventHandler(this.Btn_kq_Click);
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(24, 22);
            this.toolStripLabel4.Text = "    ";
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new System.Drawing.Size(52, 22);
            this.toolStripLabel5.Text = "           ";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.toolStripButton1.Image = global::Accenture.SerialPort.Properties.Resources.icon_unsafe_url1;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(100, 22);
            this.toolStripButton1.Text = "清除终端列表";
            this.toolStripButton1.Click += new System.EventHandler(this.ToolStripButton1_Click);
            // 
            // toolStripLabel10
            // 
            this.toolStripLabel10.Name = "toolStripLabel10";
            this.toolStripLabel10.Size = new System.Drawing.Size(20, 22);
            this.toolStripLabel10.Text = "   ";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.toolStripButton2.Image = global::Accenture.SerialPort.Properties.Resources.icon_unsafe_url1;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(100, 22);
            this.toolStripButton2.Text = "清除数据列表";
            this.toolStripButton2.Click += new System.EventHandler(this.ToolStripButton2_Click);
            // 
            // toolStripLabel11
            // 
            this.toolStripLabel11.Name = "toolStripLabel11";
            this.toolStripLabel11.Size = new System.Drawing.Size(16, 22);
            this.toolStripLabel11.Text = "  ";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.time,
            this.toolStripStatusLabel2,
            this.lab_img});
            this.statusStrip1.Location = new System.Drawing.Point(0, 622);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1230, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(68, 17);
            this.toolStripStatusLabel1.Text = "当前时间：";
            // 
            // time
            // 
            this.time.Name = "time";
            this.time.Size = new System.Drawing.Size(131, 17);
            this.time.Text = "toolStripStatusLabel1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(11, 17);
            this.toolStripStatusLabel2.Text = "|";
            // 
            // lab_img
            // 
            this.lab_img.BackColor = System.Drawing.Color.Gray;
            this.lab_img.Name = "lab_img";
            this.lab_img.Size = new System.Drawing.Size(48, 17);
            this.lab_img.Text = "  连接  ";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1230, 597);
            this.panel1.TabIndex = 6;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.White;
            this.groupBox2.Controls.Add(this.listBox1);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox2.Location = new System.Drawing.Point(256, 320);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(974, 277);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "数据详情";
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(669, 17);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(302, 257);
            this.listBox1.TabIndex = 14;
            this.listBox1.DoubleClick += new System.EventHandler(this.ListBox1_DoubleClick);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Font = new System.Drawing.Font("楷体", 13F);
            this.textBox1.Location = new System.Drawing.Point(3, 17);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(668, 257);
            this.textBox1.TabIndex = 13;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.index,
            this.wakeuptype,
            this.systime,
            this.moteeui,
            this.freq,
            this.datr,
            this.rssi,
            this.lsnr,
            this.power,
            this.wakeup,
            this.temp,
            this.hum,
            this.hexdata,
            this.strdata,
            this.ercode,
            this.JsonDataDemo});
            this.dataGridView1.Location = new System.Drawing.Point(256, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.Size = new System.Drawing.Size(974, 314);
            this.dataGridView1.TabIndex = 6;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellContentClick);
            this.dataGridView1.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellContentDoubleClick);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Controls.Add(this.checkedListBox2);
            this.groupBox1.Controls.Add(this.checkedListBox1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(256, 597);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "设备列表";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.cbox_auto);
            this.panel2.Controls.Add(this.cbox_manual);
            this.panel2.Controls.Add(this.textBox3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(3, 17);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(250, 50);
            this.panel2.TabIndex = 8;
            // 
            // cbox_auto
            // 
            this.cbox_auto.AutoSize = true;
            this.cbox_auto.Location = new System.Drawing.Point(130, 5);
            this.cbox_auto.Name = "cbox_auto";
            this.cbox_auto.Size = new System.Drawing.Size(72, 16);
            this.cbox_auto.TabIndex = 14;
            this.cbox_auto.Text = "自动唤醒";
            this.cbox_auto.UseVisualStyleBackColor = true;
            this.cbox_auto.CheckedChanged += new System.EventHandler(this.Cbox_auto_CheckedChanged);
            // 
            // cbox_manual
            // 
            this.cbox_manual.AutoSize = true;
            this.cbox_manual.Location = new System.Drawing.Point(29, 5);
            this.cbox_manual.Name = "cbox_manual";
            this.cbox_manual.Size = new System.Drawing.Size(72, 16);
            this.cbox_manual.TabIndex = 13;
            this.cbox_manual.Text = "手动唤醒";
            this.cbox_manual.UseVisualStyleBackColor = true;
            this.cbox_manual.CheckedChanged += new System.EventHandler(this.Cbox_manual_CheckedChanged);
            // 
            // textBox3
            // 
            this.textBox3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBox3.Font = new System.Drawing.Font("宋体", 10F);
            this.textBox3.Location = new System.Drawing.Point(0, 27);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(250, 23);
            this.textBox3.TabIndex = 12;
            this.textBox3.WordWrap = false;
            this.textBox3.TextChanged += new System.EventHandler(this.TextBox3_TextChanged);
            // 
            // checkedListBox2
            // 
            this.checkedListBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.checkedListBox2.FormattingEnabled = true;
            this.checkedListBox2.Location = new System.Drawing.Point(3, 510);
            this.checkedListBox2.Name = "checkedListBox2";
            this.checkedListBox2.Size = new System.Drawing.Size(250, 84);
            this.checkedListBox2.TabIndex = 13;
            this.checkedListBox2.Visible = false;
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(3, 72);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(250, 516);
            this.checkedListBox1.TabIndex = 12;
            this.checkedListBox1.SelectedIndexChanged += new System.EventHandler(this.CheckedListBox1_SelectedIndexChanged);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // timeTest
            // 
            this.timeTest.AutoSize = true;
            this.timeTest.Font = new System.Drawing.Font("宋体", 12F);
            this.timeTest.Location = new System.Drawing.Point(1116, 2);
            this.timeTest.MinimumSize = new System.Drawing.Size(80, 20);
            this.timeTest.Name = "timeTest";
            this.timeTest.Size = new System.Drawing.Size(80, 20);
            this.timeTest.TabIndex = 7;
            // 
            // index
            // 
            this.index.HeaderText = "序号";
            this.index.Name = "index";
            this.index.ReadOnly = true;
            this.index.Visible = false;
            this.index.Width = 70;
            // 
            // wakeuptype
            // 
            this.wakeuptype.HeaderText = "唤醒方式";
            this.wakeuptype.Name = "wakeuptype";
            this.wakeuptype.ReadOnly = true;
            // 
            // systime
            // 
            this.systime.HeaderText = "时间";
            this.systime.Name = "systime";
            this.systime.ReadOnly = true;
            this.systime.Width = 150;
            // 
            // moteeui
            // 
            this.moteeui.HeaderText = "终端ID";
            this.moteeui.Name = "moteeui";
            this.moteeui.ReadOnly = true;
            this.moteeui.Width = 150;
            // 
            // freq
            // 
            this.freq.HeaderText = "接收频率";
            this.freq.Name = "freq";
            this.freq.ReadOnly = true;
            this.freq.Width = 60;
            // 
            // datr
            // 
            this.datr.HeaderText = "速率";
            this.datr.Name = "datr";
            this.datr.ReadOnly = true;
            this.datr.Visible = false;
            this.datr.Width = 50;
            // 
            // rssi
            // 
            this.rssi.HeaderText = "信号强度";
            this.rssi.Name = "rssi";
            this.rssi.ReadOnly = true;
            this.rssi.Width = 60;
            // 
            // lsnr
            // 
            this.lsnr.HeaderText = "信噪比";
            this.lsnr.Name = "lsnr";
            this.lsnr.ReadOnly = true;
            this.lsnr.Width = 70;
            // 
            // power
            // 
            this.power.HeaderText = "电量";
            this.power.Name = "power";
            this.power.ReadOnly = true;
            this.power.Width = 50;
            // 
            // wakeup
            // 
            this.wakeup.HeaderText = "唤醒周期";
            this.wakeup.Name = "wakeup";
            this.wakeup.ReadOnly = true;
            this.wakeup.Width = 60;
            // 
            // temp
            // 
            this.temp.HeaderText = "温度";
            this.temp.Name = "temp";
            this.temp.ReadOnly = true;
            this.temp.Width = 50;
            // 
            // hum
            // 
            this.hum.HeaderText = "湿度";
            this.hum.Name = "hum";
            this.hum.ReadOnly = true;
            this.hum.Width = 50;
            // 
            // hexdata
            // 
            this.hexdata.HeaderText = "16进制数据";
            this.hexdata.Name = "hexdata";
            this.hexdata.ReadOnly = true;
            this.hexdata.Visible = false;
            // 
            // strdata
            // 
            this.strdata.HeaderText = "字符串数据";
            this.strdata.Name = "strdata";
            this.strdata.ReadOnly = true;
            this.strdata.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.strdata.Visible = false;
            this.strdata.Width = 200;
            // 
            // ercode
            // 
            this.ercode.HeaderText = "错误码";
            this.ercode.Name = "ercode";
            this.ercode.ReadOnly = true;
            // 
            // JsonDataDemo
            // 
            this.JsonDataDemo.HeaderText = "json数据";
            this.JsonDataDemo.Name = "JsonDataDemo";
            this.JsonDataDemo.ReadOnly = true;
            this.JsonDataDemo.Visible = false;
            // 
            // LoraForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1230, 644);
            this.Controls.Add(this.timeTest);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "LoraForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "温湿度检测仪(网关)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LoraForm_FormClosing);
            this.Load += new System.EventHandler(this.LoraForm_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox txt_ip;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripTextBox txt_port;
        private System.Windows.Forms.ToolStripLabel toolStripLabel6;
        private System.Windows.Forms.ToolStripTextBox tb_appeui;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripButton btn_kq;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripLabel toolStripLabel5;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel10;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel11;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel time;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel lab_img;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckedListBox checkedListBox2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox cbox_auto;
        private System.Windows.Forms.CheckBox cbox_manual;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Label timeTest;
        private System.Windows.Forms.DataGridViewTextBoxColumn index;
        private System.Windows.Forms.DataGridViewTextBoxColumn wakeuptype;
        private System.Windows.Forms.DataGridViewTextBoxColumn systime;
        private System.Windows.Forms.DataGridViewTextBoxColumn moteeui;
        private System.Windows.Forms.DataGridViewTextBoxColumn freq;
        private System.Windows.Forms.DataGridViewTextBoxColumn datr;
        private System.Windows.Forms.DataGridViewTextBoxColumn rssi;
        private System.Windows.Forms.DataGridViewTextBoxColumn lsnr;
        private System.Windows.Forms.DataGridViewTextBoxColumn power;
        private System.Windows.Forms.DataGridViewTextBoxColumn wakeup;
        private System.Windows.Forms.DataGridViewTextBoxColumn temp;
        private System.Windows.Forms.DataGridViewTextBoxColumn hum;
        private System.Windows.Forms.DataGridViewTextBoxColumn hexdata;
        private System.Windows.Forms.DataGridViewTextBoxColumn strdata;
        private System.Windows.Forms.DataGridViewTextBoxColumn ercode;
        private System.Windows.Forms.DataGridViewTextBoxColumn JsonDataDemo;
    }
}