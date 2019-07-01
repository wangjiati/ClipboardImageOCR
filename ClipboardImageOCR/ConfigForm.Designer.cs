namespace ClipboardImageOCR
{
    partial class ConfigForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_AppID = new System.Windows.Forms.TextBox();
            this.textBox_APIKey = new System.Windows.Forms.TextBox();
            this.textBox_SecretKey = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioButtonN = new System.Windows.Forms.RadioButton();
            this.radioButtonTab = new System.Windows.Forms.RadioButton();
            this.radioButtonEnter = new System.Windows.Forms.RadioButton();
            this.radioButton_cursorInput = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.radioButton_Clipboard = new System.Windows.Forms.RadioButton();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_Max_image_Height = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox_Min_image_Height = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_Max_image_Width = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_Min_image_Width = new System.Windows.Forms.TextBox();
            this.radioButton_Precision_False = new System.Windows.Forms.RadioButton();
            this.radioButton_Precision_True = new System.Windows.Forms.RadioButton();
            this.checkAutoStartup = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "AppID:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "API Key:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "Secret Key:";
            // 
            // textBox_AppID
            // 
            this.textBox_AppID.Location = new System.Drawing.Point(82, 14);
            this.textBox_AppID.Name = "textBox_AppID";
            this.textBox_AppID.Size = new System.Drawing.Size(265, 21);
            this.textBox_AppID.TabIndex = 3;
            // 
            // textBox_APIKey
            // 
            this.textBox_APIKey.Location = new System.Drawing.Point(82, 52);
            this.textBox_APIKey.Name = "textBox_APIKey";
            this.textBox_APIKey.Size = new System.Drawing.Size(265, 21);
            this.textBox_APIKey.TabIndex = 4;
            // 
            // textBox_SecretKey
            // 
            this.textBox_SecretKey.Location = new System.Drawing.Point(82, 86);
            this.textBox_SecretKey.Name = "textBox_SecretKey";
            this.textBox_SecretKey.Size = new System.Drawing.Size(265, 21);
            this.textBox_SecretKey.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBox_SecretKey);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox_APIKey);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBox_AppID);
            this.groupBox1.Location = new System.Drawing.Point(12, 296);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(360, 127);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Baidu OCR";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.textBox_Max_image_Height);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.textBox_Min_image_Height);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.textBox_Max_image_Width);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.textBox_Min_image_Width);
            this.groupBox2.Controls.Add(this.radioButton_Precision_False);
            this.groupBox2.Controls.Add(this.radioButton_Precision_True);
            this.groupBox2.Controls.Add(this.checkAutoStartup);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(360, 278);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "应用设置";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Controls.Add(this.radioButton_cursorInput);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.radioButton_Clipboard);
            this.groupBox3.Controls.Add(this.textBox4);
            this.groupBox3.Controls.Add(this.button3);
            this.groupBox3.Location = new System.Drawing.Point(4, 36);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(353, 123);
            this.groupBox3.TabIndex = 17;
            this.groupBox3.TabStop = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radioButtonN);
            this.groupBox4.Controls.Add(this.radioButtonTab);
            this.groupBox4.Controls.Add(this.radioButtonEnter);
            this.groupBox4.Location = new System.Drawing.Point(12, 70);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(328, 47);
            this.groupBox4.TabIndex = 20;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "结尾处追加模拟按键(用于模拟条码枪)";
            // 
            // radioButtonN
            // 
            this.radioButtonN.AutoSize = true;
            this.radioButtonN.Location = new System.Drawing.Point(124, 25);
            this.radioButtonN.Name = "radioButtonN";
            this.radioButtonN.Size = new System.Drawing.Size(35, 16);
            this.radioButtonN.TabIndex = 2;
            this.radioButtonN.TabStop = true;
            this.radioButtonN.Text = "空";
            this.radioButtonN.UseVisualStyleBackColor = true;
            // 
            // radioButtonTab
            // 
            this.radioButtonTab.AutoSize = true;
            this.radioButtonTab.Location = new System.Drawing.Point(66, 25);
            this.radioButtonTab.Name = "radioButtonTab";
            this.radioButtonTab.Size = new System.Drawing.Size(41, 16);
            this.radioButtonTab.TabIndex = 1;
            this.radioButtonTab.TabStop = true;
            this.radioButtonTab.Text = "Tab";
            this.radioButtonTab.UseVisualStyleBackColor = true;
            // 
            // radioButtonEnter
            // 
            this.radioButtonEnter.AutoSize = true;
            this.radioButtonEnter.Location = new System.Drawing.Point(7, 25);
            this.radioButtonEnter.Name = "radioButtonEnter";
            this.radioButtonEnter.Size = new System.Drawing.Size(53, 16);
            this.radioButtonEnter.TabIndex = 0;
            this.radioButtonEnter.TabStop = true;
            this.radioButtonEnter.Text = "Enter";
            this.radioButtonEnter.UseVisualStyleBackColor = true;
            // 
            // radioButton_cursorInput
            // 
            this.radioButton_cursorInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioButton_cursorInput.AutoSize = true;
            this.radioButton_cursorInput.Checked = true;
            this.radioButton_cursorInput.Location = new System.Drawing.Point(12, 17);
            this.radioButton_cursorInput.Name = "radioButton_cursorInput";
            this.radioButton_cursorInput.Size = new System.Drawing.Size(71, 16);
            this.radioButton_cursorInput.TabIndex = 18;
            this.radioButton_cursorInput.TabStop = true;
            this.radioButton_cursorInput.Text = "焦点录入";
            this.radioButton_cursorInput.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "快捷键:";
            // 
            // radioButton_Clipboard
            // 
            this.radioButton_Clipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioButton_Clipboard.AutoSize = true;
            this.radioButton_Clipboard.Location = new System.Drawing.Point(95, 17);
            this.radioButton_Clipboard.Name = "radioButton_Clipboard";
            this.radioButton_Clipboard.Size = new System.Drawing.Size(131, 16);
            this.radioButton_Clipboard.TabIndex = 19;
            this.radioButton_Clipboard.Text = "识别结果放入剪切板";
            this.radioButton_Clipboard.UseVisualStyleBackColor = true;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(63, 42);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(211, 21);
            this.textBox4.TabIndex = 6;
            this.textBox4.Text = "Shift+V";
            this.textBox4.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox4_KeyDown);
            this.textBox4.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox4_KeyPress);
            this.textBox4.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBox4_KeyUp);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(287, 40);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(53, 23);
            this.button3.TabIndex = 10;
            this.button3.Text = "默认";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(170, 198);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(11, 12);
            this.label6.TabIndex = 16;
            this.label6.Text = "-";
            // 
            // textBox_Max_image_Height
            // 
            this.textBox_Max_image_Height.Location = new System.Drawing.Point(187, 195);
            this.textBox_Max_image_Height.Name = "textBox_Max_image_Height";
            this.textBox_Max_image_Height.Size = new System.Drawing.Size(71, 21);
            this.textBox_Max_image_Height.TabIndex = 15;
            this.textBox_Max_image_Height.Text = "4096";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(17, 198);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 12);
            this.label8.TabIndex = 13;
            this.label8.Text = "识别尺寸 Y:";
            // 
            // textBox_Min_image_Height
            // 
            this.textBox_Min_image_Height.Location = new System.Drawing.Point(94, 195);
            this.textBox_Min_image_Height.Name = "textBox_Min_image_Height";
            this.textBox_Min_image_Height.Size = new System.Drawing.Size(70, 21);
            this.textBox_Min_image_Height.TabIndex = 14;
            this.textBox_Min_image_Height.Text = "10";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(170, 168);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(11, 12);
            this.label7.TabIndex = 12;
            this.label7.Text = "-";
            // 
            // textBox_Max_image_Width
            // 
            this.textBox_Max_image_Width.Location = new System.Drawing.Point(187, 165);
            this.textBox_Max_image_Width.Name = "textBox_Max_image_Width";
            this.textBox_Max_image_Width.Size = new System.Drawing.Size(71, 21);
            this.textBox_Max_image_Width.TabIndex = 11;
            this.textBox_Max_image_Width.Text = "4096";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 168);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 12);
            this.label5.TabIndex = 6;
            this.label5.Text = "识别尺寸 X:";
            // 
            // textBox_Min_image_Width
            // 
            this.textBox_Min_image_Width.Location = new System.Drawing.Point(94, 165);
            this.textBox_Min_image_Width.Name = "textBox_Min_image_Width";
            this.textBox_Min_image_Width.Size = new System.Drawing.Size(70, 21);
            this.textBox_Min_image_Width.TabIndex = 8;
            this.textBox_Min_image_Width.Text = "10";
            // 
            // radioButton_Precision_False
            // 
            this.radioButton_Precision_False.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioButton_Precision_False.AutoSize = true;
            this.radioButton_Precision_False.Checked = true;
            this.radioButton_Precision_False.Location = new System.Drawing.Point(19, 229);
            this.radioButton_Precision_False.Name = "radioButton_Precision_False";
            this.radioButton_Precision_False.Size = new System.Drawing.Size(95, 16);
            this.radioButton_Precision_False.TabIndex = 8;
            this.radioButton_Precision_False.TabStop = true;
            this.radioButton_Precision_False.Text = "通用文字识别";
            this.radioButton_Precision_False.UseVisualStyleBackColor = true;
            // 
            // radioButton_Precision_True
            // 
            this.radioButton_Precision_True.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioButton_Precision_True.AutoSize = true;
            this.radioButton_Precision_True.Location = new System.Drawing.Point(19, 256);
            this.radioButton_Precision_True.Name = "radioButton_Precision_True";
            this.radioButton_Precision_True.Size = new System.Drawing.Size(197, 16);
            this.radioButton_Precision_True.TabIndex = 9;
            this.radioButton_Precision_True.Text = "通用文字识别（高精度版)速度慢";
            this.radioButton_Precision_True.UseVisualStyleBackColor = true;
            // 
            // checkAutoStartup
            // 
            this.checkAutoStartup.AutoSize = true;
            this.checkAutoStartup.Location = new System.Drawing.Point(19, 20);
            this.checkAutoStartup.Name = "checkAutoStartup";
            this.checkAutoStartup.Size = new System.Drawing.Size(84, 16);
            this.checkAutoStartup.TabIndex = 0;
            this.checkAutoStartup.Text = "开机自启动";
            this.checkAutoStartup.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(202, 428);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "保存";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(284, 428);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 9;
            this.button2.Text = "关闭";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 485);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigForm";
            this.Text = "设置";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigForm_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_AppID;
        private System.Windows.Forms.TextBox textBox_APIKey;
        private System.Windows.Forms.TextBox textBox_SecretKey;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkAutoStartup;
        private System.Windows.Forms.RadioButton radioButton_Precision_False;
        private System.Windows.Forms.RadioButton radioButton_Precision_True;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioButton_cursorInput;
        private System.Windows.Forms.RadioButton radioButton_Clipboard;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_Max_image_Height;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox_Min_image_Height;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox_Max_image_Width;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_Min_image_Width;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton radioButtonN;
        private System.Windows.Forms.RadioButton radioButtonTab;
        private System.Windows.Forms.RadioButton radioButtonEnter;
    }
}