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




namespace ClipboardImageOCR
{
    public partial class ClipboardImageOCR : Form
    {
        private Configuration _config;

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
                        case 100:    //按下的是Shift+V
                                     //此处填写快捷键响应代码 
                            if (_config.inputmode)
                            {
                                send_api_txt(richTextBox1.Text);
                            }
                            else
                            {
                                Clipboard.SetDataObject(richTextBox1.Text, true);
                            }
                            
                            break;

                    }
                    break;
            }
            base.WndProc(ref m);
        }
        #endregion

        delegate void AsynUpdateUI(String step);

        public ClipboardImageOCR()
        {
            _config = Configuration.Load();
            _config.FixConfiguration();
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
            

        }


        /// <summary>
        /// 处理剪切板数据
        /// </summary>
        private void DisplayClipboardData()
        {
            try
            {
                if (checkBox1.Checked == false)
                    return;

                //显示剪贴板中的图片信息
                if (Clipboard.ContainsImage())
                   // if (iData.GetDataPresent(DataFormats.Bitmap))
                {
                    Image image = Clipboard.GetImage();

                    if (image.Height < 15)
                    {
                        int newHeight = 15;
                        double magnification = (double)newHeight / (double)image.Height;
                        int newWidth = (int)(image.Width * magnification);
                        Bitmap bitmap = new Bitmap(image,  newWidth, newHeight);
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

                    pictureBox1.Image = image;

                    pictureBox1.Update();
                    DateTime currentTime = DateTime.Now;

                    if (pictureBox1.Image.Height > _config.Max_image_Height || pictureBox1.Image.Height < _config.Min_image_Height ||
                        pictureBox1.Image.Width > _config.Max_image_Width || pictureBox1.Image.Width < _config.Min_image_Width
                        )
                    {
                        label3.Text = @"超设置尺寸,本次不识别. W:" + pictureBox1.Image.Width.ToString() + @" H" + pictureBox1.Image.Height.ToString();
                        return;
                    }
                    else
                    {
                        label3.Text = @"";
                    }

                    label1.Text = currentTime.ToString("HH:mm:ss") + @" 从剪切板提取:";

                    Baidu_ocr baidu_Ocr = new Baidu_ocr(pictureBox1.Image, radioButton_Precision_True.Checked);

                    baidu_Ocr.API_KEY = _config.Baidu_ocr_API_KEY;
                    baidu_Ocr.APP_ID = _config.Baidu_ocr_APP_ID;
                    baidu_Ocr.SECRET_KEY = _config.Baidu_ocr_SECRET_KEY;

                    baidu_Ocr.UpdateUIDelegate += UpdataUIStatus;//绑定更新任务状态的委托


                    //baidu_Ocr.OCR(pictureBox1.Image, radioButton2.Checked);

                    Thread OCR_thread = new Thread(baidu_Ocr.ThreadOCR)
                    {
                        IsBackground = true
                    };

                    OCR_thread.Start();

                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
                Logging.LogUsefulException(e);
            }
        }
        /// <summary>
        /// 更新UI
        /// </summary>
        /// <param name="step"></param>
        /// 

        private void UpdataUIStatus(String OCRString)
        {
            if (InvokeRequired)
            {
                this.Invoke(new AsynUpdateUI(delegate (String s)
                {
                    if (s != "")
                    {
                        this.richTextBox1.Text = s;
                        DateTime currentTime = DateTime.Now;
                        this.label2.Text = currentTime.ToString("HH:mm:ss") + @"完成OCR识别:";
                        string temp = s.Length >= 20 ? s.Substring(0, 20) + "..." : s;

                        
                        notifyIcon1.Text = "OCR识别" + "\r\n"; ;
                        notifyIcon1.BalloonTipTitle = currentTime.ToString("HH:mm:ss") + "完成OCR识别";
                        notifyIcon1.BalloonTipText =  @"OCR结果:" + "\r\n" + temp + "\r\n" ;
                        notifyIcon1.ShowBalloonTip(1000);//消失时间
                    }
                    else
                    {
                        notifyIcon1.Text = "OCR识别" + "\r\n"; ;
                        notifyIcon1.BalloonTipTitle = "OCR没返回任何东西";
                        notifyIcon1.BalloonTipText = " ";
                        notifyIcon1.ShowBalloonTip(1000);//消失时间
                    }
                }), OCRString);
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

        private void ClipboardImageOCR_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Move;
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

                        pictureBox1.Image = image;
                        DateTime currentTime = DateTime.Now;
                        label1.Text = currentTime.ToString("HH:mm:ss") + @" 拖拽图片:";

                        if (pictureBox1.Image.Height > _config.Max_image_Height || pictureBox1.Image.Height < _config.Min_image_Height ||
                            pictureBox1.Image.Width > _config.Max_image_Width || pictureBox1.Image.Width < _config.Min_image_Width
                            )
                        {
                            label3.Text = @"超设置尺寸,本次不识别. W:" + pictureBox1.Image.Width.ToString() + @" H" + pictureBox1.Image.Height.ToString();
                            return;
                        }
                        else
                        {
                            label3.Text = @"";
                        }



                        Baidu_ocr baidu_Ocr = new Baidu_ocr(pictureBox1.Image, radioButton_Precision_True.Checked);

                        baidu_Ocr.API_KEY = _config.Baidu_ocr_API_KEY;
                        baidu_Ocr.APP_ID = _config.Baidu_ocr_APP_ID;
                        baidu_Ocr.SECRET_KEY = _config.Baidu_ocr_SECRET_KEY;

                        baidu_Ocr.UpdateUIDelegate  += UpdataUIStatus;//绑定更新任务状态的委托


                        //baidu_Ocr.OCR(pictureBox1.Image, radioButton2.Checked);

                        Thread OCR_thread = new Thread(baidu_Ocr.ThreadOCR)
                        {
                            IsBackground = true
                        };

                        OCR_thread.Start();

                    }
                    catch (Exception e1)
                    {
                        //MessageBox.Show(e1.ToString());
                        Logging.LogUsefulException(e1);
                    }
                }
            }
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(richTextBox1.Text, true);
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

        private void button2_Click(object sender, EventArgs e)
        {
            IDataObject iData = new DataObject();
            iData = Clipboard.GetDataObject();

            //显示剪贴板中的图片信息
            if (iData.GetDataPresent(DataFormats.Bitmap))
            {
                Image image = Clipboard.GetImage();

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

                pictureBox1.Image = image;
                pictureBox1.Update();
            }

            if (pictureBox1.Image == null)
            {
                MessageBox.Show("没有需要OCR识别的图片");
                return;
            }

            try
            {
                DateTime currentTime = DateTime.Now;
                if (pictureBox1.Image.Height > _config.Max_image_Height || pictureBox1.Image.Height < _config.Min_image_Height ||
                    pictureBox1.Image.Width > _config.Max_image_Width || pictureBox1.Image.Width < _config.Min_image_Width
                    )
                {
                    label3.Text = @"超设置尺寸,本次不识别. W:" + pictureBox1.Image.Width.ToString() + @" H" + pictureBox1.Image.Height.ToString();
                    return;
                }
                else
                {
                    label3.Text = @"";
                }

                label1.Text = currentTime.ToString("HH:mm:ss") + "再次识别的图片:";

                Baidu_ocr baidu_Ocr = new Baidu_ocr(pictureBox1.Image, radioButton_Precision_True.Checked);
                baidu_Ocr.API_KEY = _config.Baidu_ocr_API_KEY;
                baidu_Ocr.APP_ID = _config.Baidu_ocr_APP_ID;
                baidu_Ocr.SECRET_KEY = _config.Baidu_ocr_SECRET_KEY;
                baidu_Ocr.UpdateUIDelegate += UpdataUIStatus;//绑定更新任务状态的委托

                Thread OCR_thread = new Thread(baidu_Ocr.ThreadOCR)
                {
                    IsBackground = true
                };

                OCR_thread.Start();
            }
            catch (Exception e1)
            {
                //MessageBox.Show(e1.ToString());
                Logging.LogUsefulException(e1);
            }
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

            Configuration.Save(config);
        }
        
        public Configuration GetCurrentConfiguration()
        {
            return _config;
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

                    pictureBox1.Image = image;
                    DateTime currentTime = DateTime.Now;
                    label1.Text = currentTime.ToString("HH:mm:ss") + @" 手工打开图片:";

                    if (pictureBox1.Image.Height > _config.Max_image_Height || pictureBox1.Image.Height < _config.Min_image_Height ||
                        pictureBox1.Image.Width > _config.Max_image_Width || pictureBox1.Image.Width < _config.Min_image_Width
                        )
                    {
                        label3.Text = @"超设置尺寸,本次不识别. W:" + pictureBox1.Image.Width.ToString() + @" H" + pictureBox1.Image.Height.ToString();
                        return;
                    }
                    else
                    {
                        label3.Text = @"";
                    }



                    Baidu_ocr baidu_Ocr = new Baidu_ocr(pictureBox1.Image, radioButton_Precision_True.Checked);

                    baidu_Ocr.API_KEY = _config.Baidu_ocr_API_KEY;
                    baidu_Ocr.APP_ID = _config.Baidu_ocr_APP_ID;
                    baidu_Ocr.SECRET_KEY = _config.Baidu_ocr_SECRET_KEY;

                    baidu_Ocr.UpdateUIDelegate += UpdataUIStatus;//绑定更新任务状态的委托


                    //baidu_Ocr.OCR(pictureBox1.Image, radioButton2.Checked);

                    Thread OCR_thread = new Thread(baidu_Ocr.ThreadOCR)
                    {
                        IsBackground = true
                    };

                    OCR_thread.Start();

                }
                catch (Exception e1)
                {
                    //MessageBox.Show(e1.ToString());
                    Logging.LogUsefulException(e1);
                }
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

        public delegate void UpdateUI(string OCRString);//声明一个更新主线程的委托
        public UpdateUI UpdateUIDelegate;



        public Baidu_ocr(Image Image, bool accurate)
        {
            this.Image = Image;


            this.accurate = accurate;
        }

        public void ThreadOCR()
        {
            try
            {
                string temp = OCR_AccurateBasic(Image, accurate);
                UpdateUIDelegate(temp);
            }
            catch (Exception e1)
            {
                //MessageBox.Show(e1.ToString());
                Logging.LogUsefulException(e1);
            }
        }

        private string OCR_AccurateBasic(Image image, bool GeneralOrAccurate)
        {
            if (image.Width > 4096 || image.Height > 4096)
                return "图片尺寸太大了,图片尺寸改小些再试试.最大边长不能超过4096像素 Width:" + image.Width.ToString() + " Height:" + image.Height.ToString();


            var client = new Baidu.Aip.Ocr.Ocr(API_KEY, SECRET_KEY);
            client.Timeout = 60000;  // 修改超时时间
            var imageByte = BmpConvertByte(image);

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

}
