using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClipboardImageOCR
{
    public partial class ConfigForm : Form
    {
        private Configuration _modifiedConfiguration;
        private ClipboardImageOCR _clipboardImageOCR;

        KeyEventArgs HotKey;

        public ConfigForm(ClipboardImageOCR imageOCR)
        {
            InitializeComponent();
            this._clipboardImageOCR = imageOCR;
            _modifiedConfiguration = _clipboardImageOCR.GetCurrentConfiguration();
            textBox_AppID.Text = _modifiedConfiguration.Baidu_ocr_APP_ID;
            textBox_APIKey.Text = _modifiedConfiguration.Baidu_ocr_API_KEY;
            textBox_SecretKey.Text = _modifiedConfiguration.Baidu_ocr_SECRET_KEY;
            checkAutoStartup.Checked = AutoStartup.Check();

            radioButton_Precision_False.Checked = !_modifiedConfiguration.Baidu_ocr_Precision;
            radioButton_Precision_True.Checked = _modifiedConfiguration.Baidu_ocr_Precision;

            radioButton_cursorInput.Checked = _modifiedConfiguration.inputmode;
            radioButton_Clipboard.Checked = !_modifiedConfiguration.inputmode;

            textBox_Max_image_Width.Text = _modifiedConfiguration.Max_image_Width.ToString();
            textBox_Min_image_Width.Text = _modifiedConfiguration.Min_image_Width.ToString() ;
            textBox_Max_image_Height.Text = _modifiedConfiguration.Max_image_Height.ToString();
            textBox_Min_image_Height.Text = _modifiedConfiguration.Min_image_Height.ToString() ;

            HotKey = _modifiedConfiguration.HotKey;
            textBox4.Text = getKeyEventArgsString(_modifiedConfiguration.HotKey);

            radioButtonEnter.Checked = _modifiedConfiguration.autoAddEnter;
            radioButtonTab.Checked = _modifiedConfiguration.autoAddTab;
            if (_modifiedConfiguration.autoAddEnter==false  && _modifiedConfiguration.autoAddTab == false)
            {
                radioButtonN.Checked = true;
            }

        }


        //确定
        private void button1_Click(object sender, EventArgs e)
        {

            _modifiedConfiguration.autoBan = checkAutoStartup.Checked;
            _modifiedConfiguration.Baidu_ocr_APP_ID = textBox_AppID.Text;
            _modifiedConfiguration.Baidu_ocr_API_KEY = textBox_APIKey.Text;
            _modifiedConfiguration.Baidu_ocr_SECRET_KEY = textBox_SecretKey.Text;
            _modifiedConfiguration.Baidu_ocr_Precision = !radioButton_Precision_False.Checked;


            if (checkAutoStartup.Checked != AutoStartup.Check() && !AutoStartup.Set(checkAutoStartup.Checked))
            {
                MessageBox.Show("开机自启动,无法更新注册表");
            }

            if (radioButton_cursorInput.Checked)
            {
                _modifiedConfiguration.inputmode = true;
            }
            else
            {
                _modifiedConfiguration.inputmode = false;
            }


            _modifiedConfiguration.Max_image_Width = Convert.ToInt32(textBox_Max_image_Width.Text);
            _modifiedConfiguration.Min_image_Width = Convert.ToInt32(textBox_Min_image_Width.Text);
            _modifiedConfiguration.Max_image_Height = Convert.ToInt32(textBox_Max_image_Height.Text);
            _modifiedConfiguration.Min_image_Height = Convert.ToInt32(textBox_Min_image_Height.Text);

            _modifiedConfiguration.HotKey = HotKey;

            _modifiedConfiguration.autoAddEnter = radioButtonEnter.Checked;
            _modifiedConfiguration.autoAddTab = radioButtonTab.Checked;

            _clipboardImageOCR.SaveServersConfig(_modifiedConfiguration);
            
            this.Close();
        }
        //关闭
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode != Keys.Delete && e.KeyCode != Keys.Back)
            {
                HotKey = e;
                //_modifiedConfiguration.HotKey = e;
                textBox4.Text = getKeyEventArgsString(e);
            }
            else
            {
                textBox4.Text = "";
            }
            
            e.Handled = true;
        }

        private string getKeyEventArgsString(KeyEventArgs e)
        {
            string G_str_Mode = "";

            string G_str_text = "" + e.KeyCode;


            if (e.Shift == true)
            {
                G_str_Mode = "Shift";
            }

            if (e.Control == true)
            {
                if (G_str_Mode != "")
                {
                    G_str_Mode = G_str_Mode + @" + Ctrl";
                }
                else
                {
                    G_str_Mode = @"Ctrl";
                }
            }

            if (e.Alt == true)
            {
                if (G_str_Mode != "")
                {
                    G_str_Mode = G_str_Mode + @" + Alt";
                }
                else
                {
                    G_str_Mode = @" Alt";
                }
            }
            if (G_str_Mode != "")
            {
                return G_str_Mode + " + " + G_str_text;
            }
            else
            {
                return  G_str_text;
            }
        }

        //恢复默认热键
        private void button3_Click(object sender, EventArgs e)
        {
            HotKey  = new KeyEventArgs(Keys.Shift | Keys.V);
            textBox4.Text = getKeyEventArgsString(HotKey);
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
        private void textBox4_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        private void ConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _clipboardImageOCR.RegisterHotKey();
        }
    }
}
