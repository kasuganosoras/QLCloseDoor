using AdvancedSharpAdbClient.Models;
using AdvancedSharpAdbClient;
using AdvancedSharpAdbClient.DeviceCommands;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Web;
using DarkModeForms;
using System.Drawing.Imaging;

namespace FuckQL {
    public partial class FuckQL : Form {

        private AdbClient adbClient;
        private DeviceClient deviceClient;
        private DeviceData deviceData;
        private bool isConnected;
        private Config config;
        private HttpListener httpListener;
        private string apiToken;
        private bool isLooping = true;
        private Thread checkThread, httpThread, restartThread;
        private int closeWait = 2500;
        private int apiPort = 14190;

        public FuckQL() {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            _ = new DarkModeCS(this);
            config = new Config();
        }

        private void Form1_Load(object sender, EventArgs e) {
            if (!File.Exists(@"adb\adb.exe")) {
                MessageBox.Show("未找到 adb.exe，请检查 adb 目录及 adb.exe 是否存在。", "未找到 adb");
                Application.Exit();
                return;
            }
            if (!AdbServer.Instance.GetStatus().IsRunning) {
                AdbServer server = new AdbServer();
                StartServerResult result = server.StartServer(@"adb\adb.exe", false);
                if (result != StartServerResult.Started) {
                    PrintLog(LogLevel.Error, "Can't start adb server");
                } else {
                    PrintLog(LogLevel.Info, "adb server started");
                }
            } else {
                PrintLog(LogLevel.Info, "adb server is running");
            }
            // config
            btn1X.Text = config.GetConfig("btn1X", "541");
            btn1Y.Text = config.GetConfig("btn1Y", "641");
            btn2X.Text = config.GetConfig("btn2X", "205");
            btn2Y.Text = config.GetConfig("btn2Y", "938");
            btn3X.Text = config.GetConfig("btn3X", "541");
            btn3Y.Text = config.GetConfig("btn3Y", "938");

            // adb config
            adbHost.Text = config.GetConfig("adbHost", "127.0.0.1");
            adbPort.Text = config.GetConfig("adbPort", "62001");

            // api config
            apiToken = config.GetConfig("apiToken", "123456789");
            apiPort = config.GetIntConfig("apiPort", 14190);
            closeWait = config.GetIntConfig("closeWait", 2500);

            // Check is config file exists
            if (!File.Exists("config.ini")) {
                PrintLog(LogLevel.Info, "Config file not found, creating new one");
                SaveConfigToFile();
            }

            // 1 Second Tick
            checkThread = new Thread(() =>
            {
                while (isLooping) {
                    if (isConnected && adbClient != null) {
                        ProcessUpdate();
                    }
                    Thread.Sleep(1000);
                    if (!isLooping) {
                        break;
                    }
                }
            });
            checkThread.IsBackground = true;
            checkThread.Start();

            // Restart interval
            restartThread = new Thread(() =>
            {
                while (isLooping) {
                    if (isConnected && adbClient != null) {
                        var isStarted = deviceClient.IsAppRunning("com.qinlin.edoor");
                        if (isStarted) {
                            deviceClient.StopApp("com.qinlin.edoor");
                        }
                        Thread.Sleep(1000);
                        deviceClient.StartApp("com.qinlin.edoor");
                    }
                    Thread.Sleep(60000 * 60);
                    if (!isLooping) {
                        break;
                    }
                }
            });
            restartThread.IsBackground = true;
            restartThread.Start();

            // Http Listener
            httpListener = new HttpListener();
            httpListener.Prefixes.Add(String.Format("http://*:{0}/", apiPort));
            httpListener.Start();
            httpThread = new Thread(() =>
            {
                PrintLog(LogLevel.Info, String.Format("Http server listening on port 0.0.0.0:{0}", apiPort));
                while (isLooping) {
                    var context = httpListener.GetContext();
                    var request = context.Request;
                    var response = context.Response;
                    var url = request.Url.AbsolutePath;
                    var query = request.Url.Query;
                    if (url == "/click") {
                        // verify token
                        var token = HttpUtility.ParseQueryString(query).Get("token");
                        if (token != apiToken) {
                            response.StatusCode = 403;
                            response.StatusDescription = "Forbidden";
                            response.Close();
                            continue;
                        }
                        // click button
                        var btnId = HttpUtility.ParseQueryString(query).Get("btn");
                        if (btnId != null) {
                            int id;
                            if (int.TryParse(btnId, out id)) {
                                ClickButton(id);
                                response.StatusCode = 200;
                                response.StatusDescription = "OK";
                            } else {
                                response.StatusCode = 400;
                                response.StatusDescription = "Bad Request";
                            }
                        } else {
                            var x = HttpUtility.ParseQueryString(query).Get("x");
                            var y = HttpUtility.ParseQueryString(query).Get("y");
                            if (x != null && y != null) {
                                int intX, intY;
                                if (int.TryParse(x, out intX) && int.TryParse(y, out intY)) {
                                    deviceClient.ClickAsync(intX, intY);
                                    response.StatusCode = 200;
                                    response.StatusDescription = "OK";
                                } else {
                                    response.StatusCode = 400;
                                    response.StatusDescription = "Bad Request";
                                }
                            } else {
                                response.StatusCode = 400;
                                response.StatusDescription = "Bad Request";
                            }
                        }
                    } else if (url == "/screenshot") {
                        // verify token
                        var token = HttpUtility.ParseQueryString(query).Get("token");
                        if (token != apiToken) {
                            response.StatusCode = 403;
                            response.StatusDescription = "Forbidden";
                            response.Close();
                            continue;
                        }
                        // take screenshot
                        if (deviceClient != null) {
                            var screenshot = adbClient.GetFrameBuffer(deviceData);
                            response.ContentType = "image/png";
                            response.StatusCode = 200;
                            response.StatusDescription = "OK";
                            Image image = screenshot.ToImage();
                            image.Save(response.OutputStream, ImageFormat.Png);
                        }
                    } else if (url == "/restart") {
                        // verify token
                        var token = HttpUtility.ParseQueryString(query).Get("token");
                        if (token != apiToken) {
                            response.StatusCode = 403;
                            response.StatusDescription = "Forbidden";
                            response.Close();
                            continue;
                        }
                        // restart app
                        if (deviceClient != null) {
                            deviceClient.StopApp("com.qinlin.edoor");
                            Thread.Sleep(1000);
                            deviceClient.StartApp("com.qinlin.edoor");
                            response.StatusCode = 200;
                            response.StatusDescription = "OK";
                        } else {
                            response.StatusCode = 404;
                            response.StatusDescription = "Not Found";
                        }
                    } else {
                        response.StatusCode = 404;
                        response.StatusDescription = "Not Found";
                    }
                    response.Close();
                }
            });
            httpThread.IsBackground = true;
            httpThread.Start();
        }

        private void ProcessUpdate() {
            try {
                if (adbClient != null && deviceClient != null) {
                    var isStarted = deviceClient.IsAppRunning("com.qinlin.edoor");
                    if (isStarted) {
                        appStatus.Text = "应用运行中";
                        startApp.Enabled = false;
                        stopApp.Enabled = true;
                    } else {
                        appStatus.Text = "应用未运行";
                        startApp.Enabled = true;
                        stopApp.Enabled = false;
                    }
                }
            } catch (Exception e) {
                PrintLog(LogLevel.Error, e.Message);
            }
        }

        private void PrintLog(LogLevel l, string message) {
            Console.WriteLine(message);
            var time = DateTime.Now;
            string hour = time.Hour < 10 ? "0" + time.Hour : time.Hour.ToString();
            string mins = time.Minute < 10 ? "0" + time.Minute : time.Minute.ToString();
            string secs = time.Second < 10 ? "0" + time.Second : time.Second.ToString();
            var msgs = String.Format("[{0}:{1}:{2}][{3}] {4}\r\n", hour, mins, secs, l.ToString(), message);
            logTextbox.AppendText(msgs);
        }

        private void SetAdbConfigEnabled(bool enabled) {
            adbHost.Enabled = enabled;
            adbPort.Enabled = enabled;
            connectBtn.Enabled = enabled;
            disconnectBtn.Enabled = !enabled;
        }

        private void connectBtn_Click(object sender, EventArgs e) {
            if (adbClient == null) {
                adbClient = new AdbClient();
            }
            var result = adbClient.Connect(String.Format("{0}:{1}", adbHost.Text, adbPort.Text));
            PrintLog(LogLevel.Info, "Connected to adb server: " + result);
            deviceData = adbClient.GetDevices().FirstOrDefault();
            if (deviceData != null) {
                deviceClient = new DeviceClient(adbClient, deviceData);
                PrintLog(LogLevel.Info, "Connect: " + deviceData.Name);
                SetAdbConfigEnabled(false);
                isConnected = true;
            } else {
                PrintLog(LogLevel.Error, "No device connected");
                SetAdbConfigEnabled(true);
                isConnected = false;
            }
        }

        private void startApp_Click(object sender, EventArgs e) {
            deviceClient.StartApp("com.qinlin.edoor");
        }

        private void stopApp_Click(object sender, EventArgs e) {
            deviceClient.StopApp("com.qinlin.edoor");
        }

        private void disconnectBtn_Click(object sender, EventArgs e) {
            var result = adbClient.Disconnect(String.Format("{0}:{1}", adbHost.Text, adbPort.Text));
            PrintLog(LogLevel.Info, "Disconnect: " + result);
            SetAdbConfigEnabled(true);
            isConnected = false;
        }

        private void CloseAd() {
            Thread thread = new Thread(() =>
            {
                Thread.Sleep(closeWait);
                deviceClient.StopApp("com.qinlin.edoor");
                Thread.Sleep(1000);
                deviceClient.StartApp("com.qinlin.edoor");
            });
            thread.Start();
        }

        private void ClickButton(int BtnId) {
            if (deviceClient == null) {
                PrintLog(LogLevel.Error, "No device connected");
                return;
            }
            ScreenPos pos = null;
            switch (BtnId) {
                case 1:
                    pos = GetBtn1Pos();
                    break;
                case 2:
                    pos = GetBtn2Pos();
                    break;
                case 3:
                    pos = GetBtn3Pos();
                    break;
            }
            if (pos != null) {
                deviceClient.ClickAsync(pos.X, pos.Y);
                PrintLog(LogLevel.Info, "Click button " + BtnId);
                CloseAd();
            } else {
                PrintLog(LogLevel.Error, "Invalid button " + BtnId + " position");
            }
        }

        private ScreenPos GetBtn1Pos() {
            var x = btn1X.Text;
            var y = btn1Y.Text;
            var intX = 0;
            var intY = 0;
            if (int.TryParse(x, out intX) && int.TryParse(y, out intY)) {
                return new ScreenPos(intX, intY);
            }
            return null;
        }

        private ScreenPos GetBtn2Pos() {
            var x = btn2X.Text;
            var y = btn2Y.Text;
            var intX = 0;
            var intY = 0;
            if (int.TryParse(x, out intX) && int.TryParse(y, out intY)) {
                return new ScreenPos(intX, intY);
            }
            return null;
        }

        private ScreenPos GetBtn3Pos() {
            var x = btn3X.Text;
            var y = btn3Y.Text;
            var intX = 0;
            var intY = 0;
            if (int.TryParse(x, out intX) && int.TryParse(y, out intY)) {
                return new ScreenPos(intX, intY);
            }
            return null;
        }

        private void button1_Click_1(object sender, EventArgs e) {
            ClickButton(1);
        }

        private void button2_Click(object sender, EventArgs e) {
            ClickButton(2);
        }

        private void button3_Click(object sender, EventArgs e) {
            ClickButton(3);
        }

        private void saveBtn_Click(object sender, EventArgs e) {
            SaveConfigToFile();
        }

        private void SaveConfigToFile() {
            // button config
            config.SetConfig("btn1X", btn1X.Text);
            config.SetConfig("btn1Y", btn1Y.Text);
            config.SetConfig("btn2X", btn2X.Text);
            config.SetConfig("btn2Y", btn2Y.Text);
            config.SetConfig("btn3X", btn3X.Text);
            config.SetConfig("btn3Y", btn3Y.Text);
            // adb config
            config.SetConfig("adbHost", adbHost.Text);
            config.SetConfig("adbPort", adbPort.Text);
            // api config
            config.SetConfig("apiToken", apiToken);
            config.SetConfig("apiPort", apiPort.ToString());
            config.SetConfig("closeWait", closeWait.ToString());

            config.SaveConfig();
        }

        private void DrawGroupBox(GroupBox box, Graphics g, Color textColor, Color borderColor) {
            if (box != null) {
                Brush textBrush = new SolidBrush(textColor);
                Brush borderBrush = new SolidBrush(borderColor);
                Pen borderPen = new Pen(borderBrush);
                SizeF strSize = g.MeasureString(box.Text, box.Font);
                Rectangle rect = new Rectangle(box.ClientRectangle.X,
                                               box.ClientRectangle.Y + (int)(strSize.Height / 2),
                                               box.ClientRectangle.Width - 1,
                                               box.ClientRectangle.Height - (int)(strSize.Height / 2) - 1);

                // Clear text and border
                g.Clear(this.BackColor);

                // Draw text
                g.DrawString(box.Text, box.Font, textBrush, box.Padding.Left, 0);

                // Drawing Border
                //Left
                g.DrawLine(borderPen, rect.Location, new Point(rect.X, rect.Y + rect.Height));
                //Right
                g.DrawLine(borderPen, new Point(rect.X + rect.Width, rect.Y), new Point(rect.X + rect.Width, rect.Y + rect.Height));
                //Bottom
                g.DrawLine(borderPen, new Point(rect.X, rect.Y + rect.Height), new Point(rect.X + rect.Width, rect.Y + rect.Height));
                //Top1
                g.DrawLine(borderPen, new Point(rect.X, rect.Y), new Point(rect.X + box.Padding.Left, rect.Y));
                //Top2
                g.DrawLine(borderPen, new Point(rect.X + box.Padding.Left + (int)(strSize.Width), rect.Y), new Point(rect.X + rect.Width, rect.Y));
            }
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e) {
            GroupBox box = sender as GroupBox;
            DrawGroupBox(box, e.Graphics, Color.FromArgb(200, 200, 200), Color.FromArgb(80, 150, 150, 150));
        }

        private void FuckQL_FormClosing(object sender, FormClosingEventArgs e) {
            isConnected = false;
            isLooping = false;
            if (adbClient == null || adbHost == null || adbPort == null) { return; }
            try {
                adbClient.Disconnect(String.Format("{0}:{1}", adbHost.Text, adbPort.Text));
            } catch { }
            try {
                AdbServer.Instance.StopServer();
            } catch { }
        }
    }

    class Config {
        private static string configFilePath = "config.ini";
        private NameValueCollection settings;

        public Config() {
            LoadConfig();
        }

        public string this[string key] {
            get { return settings[key]; }
            set { settings[key] = value; }
        }

        public string GetConfig(string key, string defaultVal) {
            return settings[key] ?? defaultVal;
        }

        public int GetIntConfig(string key, int defaultVal) {
            var val = settings[key];
            if (val != null) {
                int result;
                if (int.TryParse(val, out result)) {
                    return result;
                }
            }
            return defaultVal;
        }

        public bool GetBoolConfig(string key, bool defaultVal) {
            var val = settings[key];
            if (val != null) {
                bool result;
                if (bool.TryParse(val, out result)) {
                    return result;
                }
            }
            return defaultVal;
        }

        public void SetConfig(string key, string value) {
            settings[key] = value;
        }

        private void LoadConfig() {
            if (File.Exists(configFilePath)) {
                settings = new NameValueCollection();
                foreach (var row in File.ReadAllLines(configFilePath)) {
                    if (!string.IsNullOrEmpty(row)) {
                        var index = row.IndexOf('=');
                        if (index > 0)
                            settings.Add(row.Substring(0, index), row.Substring(index + 1));
                    }
                }
            } else {
                settings = ConfigurationManager.AppSettings;
            }
        }

        public void SaveConfig() {
            using (StreamWriter writer = new StreamWriter(configFilePath)) {
                foreach (var key in settings.AllKeys) {
                    writer.WriteLine($"{key}={settings[key]}");
                }
            }
        }
    }

    class LogLevel {
        public static LogLevel Info = new LogLevel("INFO");
        public static LogLevel Error = new LogLevel("ERROR");
        public static LogLevel Debug = new LogLevel("DEBUG");
        public static LogLevel Warning = new LogLevel("WARNING");

        private string level;

        private LogLevel(string level) {
            this.level = level;
        }

        public override string ToString() {
            return level;
        }
    }

    class ScreenPos {

        private int x;
        private int y;

        private ScreenPos() { }

        public ScreenPos(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public int X {
            get { return x; }
            set { x = value; }
        }

        public int Y {
            get { return y; }
            set { y = value; }
        }
    }
}
