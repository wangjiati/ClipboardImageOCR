using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;//IsoDateTimeConverter
using System.Threading;

using WindowsInput.Native;
using WindowsInput;
using System.Drawing.Imaging;

namespace ClipboardImageOCR
{
    public partial class ClipboardImageOCR : Form
    {
        private Configuration _config;

        public static DataTable ClipboardData = new DataTable("ClipboardData"); //新建一个存储监听剪切板数据的表格.;
        public DataView ClipboardDataView;

        bool InitializationComponentCompleted = false;

        public static System.Windows.Forms.Timer ShiftVTimer = new System.Windows.Forms.Timer();
        public static int ShiftVCount = 0;
        public static int ShiftVTimerCount = 0;

        #region Definitions
        //Constants for API Calls...
        private const int WM_DRAWCLIPBOARD = 0x308;
        private const int WM_CHANGECBCHAIN = 0x30D;
        private const int WM_HOTKEY = 0x0312;

        //Handle for next clipboard viewer...
        private IntPtr mNextClipBoardViewerHWnd;

        //API declarations...
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern bool ChangeClipboardChain(IntPtr HWnd, IntPtr HWndNext);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        #endregion

        #region Contructor
        public void NewViewer()
        {
            //InitializeComponent()
            //To register this form as a clipboard viewer...
            mNextClipBoardViewerHWnd = SetClipboardViewer(this.Handle);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            ChangeClipboardChain(this.Handle, mNextClipBoardViewerHWnd);
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion


        #region Message Process
        //Override WndProc to get messages...
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    {
                        //The clipboard has changed...
                        //##########################################################################
                        // Process Clipboard Here :)........................
                        //##########################################################################

                        DisplayClipboardData();

                        SendMessage(mNextClipBoardViewerHWnd, m.Msg, m.WParam.ToInt32(), m.LParam.ToInt32());


                        break;
                    }
                case WM_CHANGECBCHAIN:
                    {
                        //Another clipboard viewer has removed itself...
                        if (m.WParam == (IntPtr)mNextClipBoardViewerHWnd)
                        {
                            mNextClipBoardViewerHWnd = m.LParam;
                        }
                        else
                        {
                            SendMessage(mNextClipBoardViewerHWnd, m.Msg, m.WParam.ToInt32(), m.LParam.ToInt32());
                        }
                        break;
                    }
                case WM_HOTKEY:
                    switch (m.WParam.ToInt32())
                    {
                        case 100:    //按下的是Shift+V 此处填写快捷键响应代码 
                            string send_txt = "";

                            if (ShiftVTimer.Enabled == true)
                            {
                                if (ShiftVCount >= ClipboardData.Rows.Count)
                                {
                                    ShiftVCount = 0;
                                }

                                if (ClipboardData.Rows.Count > 1)
                                {
                                    send_txt = ClipboardData.Rows[ClipboardData.Rows.Count - 1 - ShiftVCount][0].ToString();
                                }
                                else
                                {
                                    send_txt = richTextBox1.Text;
                                }
                            }
                            else
                            {
                                ShiftVCount = 0;
                                
                                if (ClipboardData.Rows.Count>1)
                                {
                                    send_txt = ClipboardData.Rows[ClipboardData.Rows.Count - 1 - ShiftVCount][0].ToString();
                                }
                                else
                                {
                                    send_txt = richTextBox1.Text;
                                }
                            }

                            if (!string.IsNullOrEmpty(send_txt))
                            {
                                if (_config.inputmode)
                                {
                                    send_api_txt(send_txt);
                                }
                                else
                                {
                                    Clipboard.SetDataObject(send_txt, true);
                                }
                            }
                            ShiftVCount++;
                            label6.Text = ShiftVCount.ToString();

                            ShiftVTimerCount = 0;
                            label7.Text = "30";
                            ShiftVTimer.Start();

                            break;

                    }
                    break;
            }
            base.WndProc(ref m);
        }
        #endregion

        delegate void AsynUpdateUI(string OCRString, string consumingT, string StartTime);

        public ClipboardImageOCR()
        {
            _config = Configuration.Load();
            _config.FixConfiguration();

            ClipboardData.Columns.Add("内容");
            ClipboardData.Columns.Add("时间");
            ClipboardData.Columns.Add("类型");
            ClipboardData.Columns.Add("OCR耗时");
            ClipboardData.Columns.Add("图片", Type.GetType("System.Byte[]"));

            ClipboardData.Rows.Clear();

            ClipboardDataView = ClipboardData.DefaultView; //创建默认视图

            InitializeComponent();
            NewViewer();
            radioButton_Precision_False.Checked = !_config.Baidu_ocr_Precision;
            radioButton_Precision_True.Checked = _config.Baidu_ocr_Precision;
            if (_config.inputmode)
            {
                button1.Text = "插入光标位置 " + getKeyEventArgsString(_config.HotKey);
            }
            else
            {
                button1.Text = "放入剪切板 " + getKeyEventArgsString(_config.HotKey);
            }

            label5.Text = "当快速连续按"+ getKeyEventArgsString(_config.HotKey) + "时" + "\r\n" + "按后进先出逐个提取监听记录";


            dataGridViewClipboardDataView.AllowUserToAddRows = false;

            dataGridViewClipboardDataView.DataSource = ClipboardDataView;
            InitializationComponentCompleted = true;

            ShiftVTimer.Interval = 100;
            ShiftVTimer.Tick += new EventHandler(TimerEventProcessor);
            label6.Text = "";
            label7.Text = "";
            this.TopMost = true;
        }


        
        /// <summary>
        /// 更新UI
        /// </summary>
        /// <param name="step"></param>
        /// 

        private void UpdataUIStatus(string OCRString,string consumingT, string StartTime)
        {
            if (InvokeRequired)
            {
                this.Invoke(new AsynUpdateUI(delegate (string s, string cT, string ST)
                {
                    if (s != "")
                    {
                        richTextBox1.Text = s;
                        DateTime currentTime = DateTime.Now;
                        this.label2.Text = currentTime.ToString("HH:mm:ss") + @"完成OCR识别:";
                        string temp = s.Length >= 20 ? s.Substring(0, 20) + "..." : s;

                        
                        notifyIcon1.Text = "OCR识别" + "\r\n"; ;
                        notifyIcon1.BalloonTipTitle = currentTime.ToString("HH:mm:ss") + "完成OCR识别";
                        notifyIcon1.BalloonTipText =  @"OCR结果:" + "\r\n" + temp + "\r\n" ;
                        notifyIcon1.ShowBalloonTip(1000);//消失时间

                        foreach (DataRow item in ClipboardData.Rows)
                        {
                            if (item["时间"].ToString() == ST)
                            {
                                item["内容"] = OCRString;
                                item["OCR耗时"] = cT;

                            }
                        }
                    }
                    else
                    {
                        notifyIcon1.Text = "OCR识别" + "\r\n"; ;
                        notifyIcon1.BalloonTipTitle = "OCR没返回任何东西";
                        notifyIcon1.BalloonTipText = " ";
                        notifyIcon1.ShowBalloonTip(1000);//消失时间
                    }
                }), OCRString, consumingT, StartTime);
            }
            else
            {
                if (OCRString != "")
                {
                    this.richTextBox1.Text = OCRString;
                    DateTime currentTime = DateTime.Now;
                    this.label2.Text = currentTime.ToString("HH:mm:ss") + @"完成OCR识别:";
                    string temp = OCRString.Length >= 20 ? OCRString.Substring(0, 20)+"..." : OCRString;

                    notifyIcon1.Text = "OCR识别" + "\r\n"; ;
                    notifyIcon1.BalloonTipTitle = currentTime.ToString("HH:mm:ss") + "完成OCR识别";
                    notifyIcon1.BalloonTipText =  @"OCR结果:" + "\r\n" + temp + "\r\n";
                    notifyIcon1.ShowBalloonTip(1000);//消失时间

                    foreach (DataRow item in ClipboardData.Rows)
                    {
                        if (item["时间"].ToString() == StartTime)
                        {
                            item["内容"] = OCRString;
                            item["OCR耗时"] = consumingT;

                        } 
                    }
                }
                else
                {
                    notifyIcon1.Text = "OCR识别" + "\r\n"; ;
                    notifyIcon1.BalloonTipTitle = "OCR没返回任何东西";
                    notifyIcon1.BalloonTipText = " ";
                    notifyIcon1.ShowBalloonTip(1000);//消失时间
                }

            }
        }

        private  void TimerEventProcessor(Object myObject,EventArgs myEventArgs)
        {
            if (++ShiftVTimerCount > 30)
            {
                ShiftVTimer.Stop();

                ShiftVCount = 0;
                ShiftVTimerCount = 0;
                label6.Text = "";
                label7.Text = "";
            }
            else
            {
                label7.Text = (30 - ShiftVTimerCount).ToString();
            }
           
        }

        private void ClipboardImageOCR_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Move;
            }
        }
        /// <summary>
        /// 处理剪切板数据
        /// </summary>
        private void DisplayClipboardData()
        {
            if (InitializationComponentCompleted == false)
            {
                return;
            }
            try
            {
                //显示剪贴板中的图片信息
                if (Clipboard.ContainsImage())
                // if (iData.GetDataPresent(DataFormats.Bitmap))
                {
                    if (checkBox1.Checked == false)
                        return;
                    Image image = Clipboard.GetImage();

                    pictureBox1.Image = image;
                    pictureBox1.Update();

                    DateTime currentTime = DateTime.Now;
                    label1.Text = currentTime.ToString("HH:mm:ss") + @" 从剪切板提取:";
                    OCRthread(image, currentTime);

                }
                else if (Clipboard.ContainsText())
                {
                    string cliStr = Clipboard.GetText();
                    if (string.IsNullOrEmpty(cliStr))
                    {
                        return;
                    }

                    while (ClipboardData.Rows.Count > 20)
                    {
                        ClipboardData.Rows[0].Delete();
                    }
                    DateTime currentTime = DateTime.Now;
                    ClipboardData.Rows.Add(cliStr, currentTime.ToString("HH:mm:ss:fff"), "文字", "", null);

                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
                Logging.LogUsefulException(e);
            }
        }
        private void ClipboardImageOCR_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 1)
            {
                string Extension = Path.GetExtension(files[0]).ToLower();
                /*
                "Image files (JPeg, Gif, Bmp, etc.)|*.jpg;*.jpeg;*.gif;*.bmp;*.tif; *.tiff; *.png|" 
                " JPeg files (*.jpg;*.jpeg)|*.jpg;*.jpeg |GIF files (*.gif)|*.gif |BMP files (*.b"
                "mp)|*.bmp|Tiff files (*.tif;*.tiff)|*.tif;*.tiff|Png files (*.png)| *.png |All f"
                "iles (*.*)|*.*";
                )*/
                string[] temArr = { ".jpg", ".jpeg", ".bmp", ".png" };
                List<string> Image_files_Extension = new List<string>(temArr);
                if (Image_files_Extension.Contains(Extension))
                {
                    try
                    {

                        Image image = Image.FromFile(files[0]);

                        pictureBox1.Image = image;
                        pictureBox1.Update();

                        DateTime currentTime = DateTime.Now;
                        label1.Text = currentTime.ToString("HH:mm:ss") + @" 拖拽图片:";

                        OCRthread(image, currentTime);
                       
                    }
                    catch (Exception e1)
                    {
                        //MessageBox.Show(e1.ToString());
                        Logging.LogUsefulException(e1);
                    }
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Image image = pictureBox1.Image;

            if (image == null)
            {
                MessageBox.Show("没有需要OCR识别的图片");
                return;
            }
            

            DateTime currentTime = DateTime.Now;
            label1.Text = currentTime.ToString("HH:mm:ss") + "再次识别的图片:";
            OCRthread(image, currentTime);
        }
        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.DefaultExt = "*.jpg;*.jpeg;";

            openFile.Filter = "Image files (JPeg, Gif, Bmp, etc.)|*.jpg;*.jpeg;*.gif;*.bmp;*.tif; *.tiff; *.png|" +
                " JPeg files (*.jpg;*.jpeg)|*.jpg;*.jpeg |GIF files (*.gif)|*.gif |BMP files (*.b" +
                "mp)|*.bmp|Tiff files (*.tif;*.tiff)|*.tif;*.tiff|Png files (*.png)| *.png |All f" +
                "iles (*.*)|*.*";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                try
                {

                    Image image = Image.FromFile(openFile.FileName);

                    pictureBox1.Image = image;
                    pictureBox1.Update();

                    DateTime currentTime = DateTime.Now;
                    label1.Text = currentTime.ToString("HH:mm:ss") + @" 手工打开图片:";
                    OCRthread(image, currentTime);


                }
                catch (Exception e1)
                {
                    //MessageBox.Show(e1.ToString());
                    Logging.LogUsefulException(e1);
                }
            }
        }
        private void OCRthread(Image imag, DateTime currentTime)
        {
            Image image = imag;
            if (image == null)
            {
                return;
            }
            if (image.Height < 15)
            {
                int newHeight = 15;
                double magnification = (double)newHeight / (double)image.Height;
                int newWidth = (int)(image.Width * magnification);
                Bitmap bitmap = new Bitmap(image, newWidth, newHeight);
                image = bitmap;
            }
            if (image.Width < 15)
            {
                int newWidth = 15;
                double magnification = (double)newWidth / (double)image.Width;
                int newHeight = (int)(image.Height * magnification);
                Bitmap bitmap = new Bitmap(image, newWidth, newHeight);
                image = bitmap;
            }



            if (image.Height > _config.Max_image_Height || image.Height < _config.Min_image_Height ||
                image.Width > _config.Max_image_Width || image.Width < _config.Min_image_Width
                )
            {
                label3.Text = @"超设置尺寸,本次不识别. W:" + image.Width.ToString() + @" H" + image.Height.ToString();
                return;
            }
            else
            {
                label3.Text = @"";
            }

            label1.Text = currentTime.ToString("HH:mm:ss") + @" 从剪切板提取:";

            while (ClipboardData.Rows.Count > 20)
            {
                ClipboardData.Rows[0].Delete();
            }
            ClipboardData.Rows.Add("", currentTime.ToString("HH:mm:ss:fff"), "图片", "", ImageProcess.ImageToBytes(image));

            Baidu_ocr baidu_Ocr = new Baidu_ocr(image, radioButton_Precision_True.Checked, currentTime);

            baidu_Ocr.API_KEY = _config.Baidu_ocr_API_KEY;
            baidu_Ocr.APP_ID = _config.Baidu_ocr_APP_ID;
            baidu_Ocr.SECRET_KEY = _config.Baidu_ocr_SECRET_KEY;

            baidu_Ocr.UpdateUIDelegate += UpdataUIStatus;//绑定更新任务状态的委托

            //baidu_Ocr.OCR(pictureBox1.Image, radioButton2.Checked);

            Thread OCR_thread = new Thread(baidu_Ocr.ThreadOCR);
            OCR_thread.IsBackground = true;
            OCR_thread.Start();

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (! string.IsNullOrEmpty(richTextBox1.Text))
            {
                Clipboard.SetDataObject(richTextBox1.Text, true);
            }
            
        }



        /// <summary>
        ///  点击最小化按钮时,最小化到系统托盘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClipboardImageOCR_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }
        private void ShowMainForm()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
            this.TopMost = true;
        }



        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                menu_Notify.Show();
            }
            else if(e.Button == MouseButtons.Left)// 鼠标左键单击显示界面
            {
                if (this.WindowState == FormWindowState.Minimized)
                {
                    ShowMainForm();
                }
                else if (this.WindowState == FormWindowState.Normal)
                {
                    this.WindowState = FormWindowState.Minimized;
                    this.Hide();
                }
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 显示剪切板图片状态ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowMainForm();
        }




        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClipboardImageOCR_FormClosing(object sender, FormClosingEventArgs e)
        {
            //注销Id号为100的热键设定
            HotKey.UnregisterHotKey(Handle, 100);
        }

        private void ClipboardImageOCR_Load(object sender, EventArgs e)
        {
            RegisterHotKey();
        }



        public void RegisterHotKey()
        {
            /*
                None = 0,
                Alt = 1,
                Ctrl = 2,
                Shift = 4,
                WindowsKey = 8
             */
            uint keyModifiers = 0;

            if (_config.HotKey.Shift == true)
            {
                keyModifiers = 4;
            }

            if (_config.HotKey.Control == true)
            {
                keyModifiers = keyModifiers | 2;
            }

            if (_config.HotKey.Alt == true)
            {
                keyModifiers = keyModifiers | 1;
            }


            if (keyModifiers > 0)
            {
                //注册热键Shift+V，Id号为100。HotKey.KeyModifiers.Shift也可以直接使用数字4来表示。
                if (HotKey.RegisterHotKey(Handle, 100, keyModifiers, _config.HotKey.KeyCode) == false)
                {
                    MessageBox.Show("ClipboardImageOCR 热键注册失败,热键被其他程序占用");
                }
            }


        }
        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutBox = new AboutBox1();
            aboutBox.Show();
        }

        private void 设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //去设置页面的时候 要 注销Id号为100的热键设定
            HotKey.UnregisterHotKey(Handle, 100);

            ConfigForm configForm = new ConfigForm(this);
            configForm.TopMost = true;
            configForm.Show();
        }

        /// <summary>
        /// 发送字符串
        /// </summary>
        /// <param name="message">要发送的文本信息</param>
        private void send_api_txt(string message)
        {
            var sim = new InputSimulator();
            sim.Keyboard.TextEntry(message);

            //判断是否需要追加回车或者制表符
            if (_config.autoAddEnter)
            {
                sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
            }
            else if (_config.autoAddTab)
            {
                sim.Keyboard.KeyPress(VirtualKeyCode.TAB);
            }

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
                return G_str_text;
            }
        }



        public void SaveServersConfig(Configuration config)
        {
            _config.CopyFrom(config);

            radioButton_Precision_False.Checked = !_config.Baidu_ocr_Precision;
            radioButton_Precision_True.Checked = _config.Baidu_ocr_Precision;
            if (_config.inputmode)
            {
                button1.Text = "插入光标位置 " + getKeyEventArgsString(_config.HotKey);
            }
            else
            {
                button1.Text = "放入剪切板 " + getKeyEventArgsString(_config.HotKey);
            }
            label5.Text = "当快速连续按" + getKeyEventArgsString(_config.HotKey) + "时" + "\r\n" + "按后进先出逐个提取监听记录";
            Configuration.Save(config);
        }
        
        public Configuration GetCurrentConfiguration()
        {
            return _config;
        }

        private void dataGridViewClipboardDataView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0 )
            {
                return;
            }
            if (dataGridViewClipboardDataView.Rows[e.RowIndex].Cells[2].Value.ToString() == "图片")
            {
                Image image = ImageProcess.GetImageByBytes((byte[])dataGridViewClipboardDataView.Rows[e.RowIndex].Cells[4].Value);
                pictureBox1.Image = image;
            }
            richTextBox1.Text = dataGridViewClipboardDataView.Rows[e.RowIndex].Cells[0].Value.ToString();
        }

        private void dataGridViewClipboardDataView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
            {
                return;
            }
            if (dataGridViewClipboardDataView.Rows[e.RowIndex].Cells[2].Value.ToString() == "图片")
            {
                Image image = ImageProcess.GetImageByBytes((byte[])dataGridViewClipboardDataView.Rows[e.RowIndex].Cells[4].Value);
                pictureBox1.Image = image;
            }
            richTextBox1.Text = dataGridViewClipboardDataView.Rows[e.RowIndex].Cells[0].Value.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否要删除全部记录?","删除提示",MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ClipboardData.Clear();
            }
        }
    }


    [Serializable]
    public class Configuration
    {
        private static string CONFIG_FILE = "gui-config.json";

        //自动在结尾处追加Enter
        public bool autoAddEnter;
        //自动在结尾处追加Tab
        public bool autoAddTab;

        public bool autoBan;

        public bool inputmode; //True: 焦点录入  False: 放入剪切板

        //设置识别的尺寸范围不在这个范围内的图片不发去OCR
        public int Max_image_Width;
        public int Min_image_Width;
        public int Max_image_Height;
        public int Min_image_Height;

        public bool Baidu_ocr_Precision; //是否启用精确识别

        // 设置APPID/AK/SK
        public string Baidu_ocr_APP_ID ;  //"你的 App ID";
        public string Baidu_ocr_API_KEY ;// "你的 Api Key";
        public string Baidu_ocr_SECRET_KEY ;// "你的 Secret Key";

        public KeyEventArgs HotKey;

        public Configuration()
        {
            Baidu_ocr_APP_ID = @"11676771";  //"你的 App ID";
            Baidu_ocr_API_KEY = @"YGkeudLKBL3DgSN0GX3GHu1D";// "你的 Api Key";
            Baidu_ocr_SECRET_KEY = @"YMLj2Zim7WOxNslAvZVz5n174FwXBLQt";// "你的 Secret Key";
            Baidu_ocr_Precision = false;

            inputmode = true; //True: 焦点录入  False: 放入剪切板

            //设置识别的尺寸范围不在这个范围内的图片不发去OCR
            Max_image_Width = 4096;
            Min_image_Width = 10;
            Max_image_Height = 4096;
            Min_image_Height = 10;

            autoAddEnter = false;
            autoAddTab = false;

            //HotKey = new KeyEventArgs(Keys.Shift | Keys.V);
        }

        public static Configuration Load()
        {
            return LoadFile(CONFIG_FILE);
        }

        public static Configuration LoadFile(string filename)
        {
            try
            {
                string configContent = File.ReadAllText(filename);
                return Load(configContent);
            }
            catch (Exception e)
            {
                if (!(e is FileNotFoundException))
                {
                    Console.WriteLine(e);
                }
                return new Configuration();
            }
        }


        public static Configuration Load(string config_str)
        {
            try
            {
                Configuration config = JsonConvert.DeserializeObject<Configuration>(config_str);
                return config;
            }
            catch
            {
            }
            return null;
        }
        public static void Save(Configuration config)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(File.Open(CONFIG_FILE, FileMode.Create)))
                {
                    string jsonString = JsonConvert.SerializeObject(config);//SimpleJson.SimpleJson.SerializeObject(config);
                    sw.Write(jsonString);
                    sw.Flush();
                }
            }
            catch (IOException e)
            {
                Console.Error.WriteLine(e);
            }
        }
        public void CopyFrom(Configuration config)
        {
            autoBan = config.autoBan;
            Baidu_ocr_APP_ID = config.Baidu_ocr_APP_ID;
            Baidu_ocr_API_KEY = config.Baidu_ocr_API_KEY;
            Baidu_ocr_SECRET_KEY = config.Baidu_ocr_SECRET_KEY;
            Baidu_ocr_Precision = config.Baidu_ocr_Precision;

            inputmode = config.inputmode; //True: 焦点录入  False: 放入剪切板

            //设置识别的尺寸范围不在这个范围内的图片不发去OCR
            Max_image_Width = config.Max_image_Width;
            Min_image_Width = config.Min_image_Width;
            Max_image_Height = config.Max_image_Height;
            Min_image_Height = config.Min_image_Height;

            HotKey = config.HotKey;

            autoAddEnter = config.autoAddEnter;
            autoAddTab = config.autoAddTab;
        }

        public void FixConfiguration()
        {
            if (Baidu_ocr_APP_ID == "" || Baidu_ocr_APP_ID ==null)
            {
                Baidu_ocr_APP_ID = @"11676771";  //"你的 App ID";
            }
            if (Baidu_ocr_API_KEY == "" || Baidu_ocr_API_KEY == null)
            {
                Baidu_ocr_API_KEY = @"YGkeudLKBL3DgSN0GX3GHu1D";// "你的 Api Key";
            }
            if (Baidu_ocr_SECRET_KEY == "" || Baidu_ocr_SECRET_KEY == null)
            {
                Baidu_ocr_SECRET_KEY = @"YMLj2Zim7WOxNslAvZVz5n174FwXBLQt";// "你的 Secret Key";
            }
            if (Max_image_Width < 10 || Max_image_Width>4096)
            {
                Max_image_Width = 4096;
            }
            if (Max_image_Height < 10 || Max_image_Height > 4096)
            {
                Max_image_Height = 4096;
            }
            if (Min_image_Width < 10 || Min_image_Width > 4096)
            {
                Min_image_Width = 10;
            }
            if (Min_image_Height < 10 || Min_image_Height > 4096)
            {
                Min_image_Height = 10;
            }

            if (HotKey == null)
            {
                HotKey = new KeyEventArgs(Keys.Shift | Keys.V);
            }
        }

    }


    public class Baidu_ocr
    {
        // 设置APPID/AK/SK
        public string APP_ID = @"11676771";  //"你的 App ID";
        public string API_KEY = @"YGkeudLKBL3DgSN0GX3GHu1D";// "你的 Api Key";
        public string SECRET_KEY = @"YMLj2Zim7WOxNslAvZVz5n174FwXBLQt";// "你的 Secret Key";

        private bool accurate;
        private Image Image;

        public delegate void UpdateUI(string OCRString,string consumingT, string StartTime);//声明一个更新主线程的委托
        public UpdateUI UpdateUIDelegate;

        public delegate void AccomplishTask();//声明一个在完成任务时通知主线程的委托
        public AccomplishTask TaskCallBack;

        private DateTime StartTime;

        public Baidu_ocr(Image Image, bool accurate, DateTime StartTime)
        {
            this.Image = Image;
            this.accurate = accurate;
            this.StartTime = StartTime;
        }

        public void ThreadOCR()
        {
            try
            {
                string temp = OCR_AccurateBasic(Image, accurate);
                DateTime EdnTime = DateTime.Now;
                string consumingT = ExecDateDiff(StartTime, EdnTime);

                //调用更新主线程ui状态的委托
                UpdateUIDelegate(temp, consumingT, StartTime.ToString("HH:mm:ss:fff"));
               

                //任务完成时通知主线程作出相应的处理
                //TaskCallBack();
            }
            catch (Exception e1)
            {
                //MessageBox.Show(e1.ToString());
                Logging.LogUsefulException(e1);
            }
        }

        /// <summary>
        /// 程序执行时间测试
        /// </summary>
        /// <param name="dateBegin">开始时间</param>
        /// <param name="dateEnd">结束时间</param>
        /// <returns>返回(秒)单位，比如: 0.00239秒</returns>
        public static string ExecDateDiff(DateTime dateBegin, DateTime dateEnd)
        {
            TimeSpan ts1 = new TimeSpan(dateBegin.Ticks);
            TimeSpan ts2 = new TimeSpan(dateEnd.Ticks);
            TimeSpan ts3 = ts1.Subtract(ts2).Duration();
            //你想转的格式
            return ts3.TotalMilliseconds.ToString();
        }

        private string OCR_AccurateBasic(Image image, bool GeneralOrAccurate)
        {
            if (image.Width > 4096 || image.Height > 4096)
                return "图片尺寸太大了,图片尺寸改小些再试试.最大边长不能超过4096像素 Width:" + image.Width.ToString() + " Height:" + image.Height.ToString();


            var client = new Baidu.Aip.Ocr.Ocr(API_KEY, SECRET_KEY);
            client.Timeout = 60000;  // 修改超时时间
            var imageByte = ImageProcess.ImageToBytes(image);

            /*
            // 如果有可选参数
            var options = new Dictionary<string, object>{
                {"language_type", "CHN_ENG"},
                {"detect_direction", "true"},
                {"detect_language", "true"},
                {"probability", "true"}
            };
            // 带参数调用通用文字识别, 图片参数为本地图片
            var result = client.GeneralBasic(image, options);
            */


            JObject result;
            if (GeneralOrAccurate)
            {
                result = client.AccurateBasic(imageByte);
            }
            else
            {
                // 调用通用文字识别, 图片参数为本地图片，可能会抛出网络等异常，请使用try/catch捕获
                result = client.GeneralBasic(imageByte);
            }

            JToken words_result_num = result.SelectToken("words_result_num");

            if (Convert.ToInt32(words_result_num) > 0)
            {
                var words = result.SelectToken("words_result").Select(p => p["words"]).ToList();
                string temp = "";
                foreach (var word in words)
                {
                    temp = temp + word.ToString() + "\r\n";
                }
                return temp;
            }
            else
            {
                return "";
            }
        }
    }

    public class HotKey
    {
        //如果函数执行成功，返回值不为0。
        //如果函数执行失败，返回值为0。要得到扩展错误信息，调用GetLastError。
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(
            IntPtr hWnd,                //要定义热键的窗口的句柄
            int id,                     //定义热键ID（不能与其它ID重复）
            KeyModifiers fsModifiers,   //标识热键是否在按Alt、Ctrl、Shift、Windows等键时才会生效
            Keys vk                     //定义热键的内容
            );

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(

             IntPtr hWnd, // handle to window   
             int id, // hot key identifier   
             uint fsModifiers, // key-modifier options   
             Keys vk // virtual-key code   
        );
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(
            IntPtr hWnd,                //要取消热键的窗口的句柄
            int id                      //要取消热键的ID
            );
        //定义了辅助键的名称（将数字转变为字符以便于记忆，也可去除此枚举而直接使用数值）
        [Flags()]
        public enum KeyModifiers
        {
            None = 0,
            Alt = 1,
            Ctrl = 2,
            Shift = 4,
            WindowsKey = 8
        }
    }


    public class ImageProcess
    {
        /// <summary>
        ///     图片转为base64编码字符
        /// </summary>
        /// <param name="path">图片路径</param>
        /// <param name="format">图片格式</param>
        /// <returns>base64编码字符</returns>
        public static string ImgToBase64(string path, ImageFormat format)
        {
            try
            {
                var bmp = new Bitmap(path);
                var ms = new MemoryStream();
                bmp.Save(ms, format);
                var arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        ///     图片转为base64编码字符
        /// </summary>
        /// <param name="bytes">图片二进制数据</param>
        public static string ImgToBase64(byte[] bytes)
        {
            if (bytes == null)
            {
                return "";
            }

            try
            {
                return Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        ///     图片转为base64编码字符
        /// </summary>
        /// <param name="img">Image图片</param>
        /// <returns>base64字符串</returns>
        public static string ImgToBase64(Image img)
        {
            try
            {
                var bts = BitmapToBytes(img);
                return ImgToBase64(bts);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 图片转字节流
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private Byte[] BmpConvertByte(Image image)
        {
            MemoryStream ms1 = new MemoryStream();
            image.Save(ms1, System.Drawing.Imaging.ImageFormat.Bmp);
            return ms1.GetBuffer();
        }
        /// <summary>
        /// 字节流转图片
        /// </summary>
        /// <param name="streamByte"></param>
        /// <returns></returns>
        public System.Drawing.Image ReturnPhoto(byte[] streamByte)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream(streamByte);
            System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
            return img;
        }

        /// <summary>
        ///     image转byte[]
        /// </summary>
        /// <param name="image">image</param>
        /// <returns>byte[]</returns>
        public static byte[] ImageToBytes(Image image)
        {
            try
            {
                using (Bitmap bitmap = new Bitmap(image))
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        bitmap.Save(stream, ImageFormat.Jpeg);
                        return stream.GetBuffer();
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            
            //*/
        }


        /// <summary>
        ///     base64编码的文本转为图片
        /// </summary>
        /// <param name="bstring">base64编码图片</param>
        /// <returns>图片</returns>
        public static Bitmap Base64ToImage(string bstring)
        {
            try
            {
                if (string.IsNullOrEmpty(bstring)) return null;
                var arr = Convert.FromBase64String(bstring);
                var ms = new MemoryStream(arr);
                var bmp = new Bitmap(Image.FromStream(ms));
                ms.Close();
                return bmp;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 图片转byte[]
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] BitmapToBytes(Image image)
        {

            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] bytes = ms.GetBuffer();  //byte[]   bytes=   ms.ToArray(); 这两句都可以，至于区别么，下面有解释
            ms.Close();
            return bytes;
        }


        /// <summary>
        ///读取byte[]并转化为图片
        /// </summary>
        /// <param name="bytes">byte[]</param>
        /// <returns>Image</returns>
        public static Image GetImageByBytes(byte[] bytes)
        {
            Image photo = null;
            using (var ms = new MemoryStream(bytes))
            {
                ms.Write(bytes, 0, bytes.Length);
                photo = Image.FromStream(ms, true);
            }
            return photo;
        }


        /// 图片url链接转化为图片
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Bitmap GetURLImage(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            try
            {
                System.Net.WebRequest webreq = System.Net.WebRequest.Create(url);
                System.Net.WebResponse webres = webreq.GetResponse();
                using (System.IO.Stream stream = webres.GetResponseStream())
                {
                    Bitmap tmpImg = new Bitmap(stream);
                    var img = new Bitmap(tmpImg);
                    stream.Close();
                    return img;
                }
            }
            catch (Exception ex)
            {
                //LogHelper.WriteErrorLog(typeof(ImageHelper), ex);
                return null;
            }

        }
        /// <summary>
        /// 图片url链接转化为字节
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static byte[] GetURLImageBytes(string url)
        {
            byte[] img = null;
            try
            {
                System.Net.WebRequest webreq = System.Net.WebRequest.Create(url);
                System.Net.WebResponse webres = webreq.GetResponse();
                using (System.IO.Stream stream = webres.GetResponseStream())
                {
                    using (System.IO.MemoryStream mStream = new MemoryStream())
                    {
                        stream.CopyTo(mStream);
                        img = mStream.GetBuffer();
                        stream.Close();
                        mStream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                //LogHelper.WriteErrorLog(typeof(ImageHelper), ex);
            }
            return img;
        }
    }
}
