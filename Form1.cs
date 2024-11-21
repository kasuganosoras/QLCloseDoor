using AdvancedSharpAdbClient.Models;
using AdvancedSharpAdbClient;
using AdvancedSharpAdbClient.DeviceCommands;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Web;
using DarkModeForms;
using System.Drawing.Imaging;
using Microsoft.Win32;
using System.Diagnostics;
using System.Text;

namespace RemoteAndroid {
    public partial class RemoteAndroid : Form {

        private AdbClient adbClient;
        private DeviceClient deviceClient;
        private DeviceData deviceData;
        private bool isConnected;
        private Config config;
        private HttpListener httpListener;
        private bool isLooping = true;
        private Thread checkThread, httpThread, restartThread, softwareThread;
        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenu;
        private Process afterPro;
        private Bitmap lastScreenshot;
        private DateTime lastScreenshotTime = DateTime.MinValue;
        private object cachedInfo;
        private DateTime lastCacheInfo = DateTime.MinValue;
        /* Configuration */
        private string apiToken;
        private int apiPort = 14190;
        private int closeWait = 2500;
        private bool autoLockScreen = false;
        private string afterLaunch, afterLaunchArgs;
        private string packageName, activityName;
        /* Misc */
        private static int SCRBUFLEN = 0;

        public RemoteAndroid() {
            // 初始化 NotifyIcon
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = this.Icon;
            notifyIcon.Text = "Remote Android";
            notifyIcon.Visible = true;

            // 创建右键菜单
            contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("连接设备", null, SwitchConnect_Click);
            // 切换 App
            var switchAppMenu = new ToolStripMenuItem("启动应用", null, SwitchApp_Click);
            switchAppMenu.Enabled = false;
            contextMenu.Items.Add(switchAppMenu);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("显示主窗口", null, Open_Click);
            contextMenu.Items.Add("退出", null, Exit_Click);
            notifyIcon.ContextMenuStrip = contextMenu;
            notifyIcon.DoubleClick += OnIconDoubleClick;

            // 处理窗体最小化事件
            this.Resize += new EventHandler(Form_Resize);

            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            if (IsUsingDarkTheme()) {
                _ = new DarkModeCS(this);
                groupBox1.Paint += groupBox1_Paint;
                //groupBox2.Paint += groupBox1_Paint;
                groupBox3.Paint += groupBox1_Paint;
                groupBox4.Paint += groupBox1_Paint;
                groupBox5.Paint += groupBox1_Paint;
                splitContainer1.BackColor = Color.FromArgb(33, 33, 33);
                // splitContainer1.Panel1.BackColor = Color.FromArgb(33, 33, 33);
                // splitContainer1.Panel2.BackColor = Color.FromArgb(33, 33, 33);
                // splitContainer1.Paint += SplitterPaint;
            } else {
                groupBox1.Paint -= groupBox1_Paint;
                // groupBox2.Paint -= groupBox1_Paint;
                groupBox3.Paint -= groupBox1_Paint;
                groupBox4.Paint -= groupBox1_Paint;
                groupBox5.Paint -= groupBox1_Paint;
            }
            config = new Config();
        }

        private void OnIconDoubleClick(object? sender, EventArgs e) {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void Form_Resize(object sender, EventArgs e) {
            if (this.WindowState == FormWindowState.Minimized) {
                this.Hide();
                notifyIcon.ShowBalloonTip(1000, "提示", "应用程序已最小化到托盘", ToolTipIcon.Info);
            }
        }

        private void SwitchApp_Click(object sender, EventArgs e) {
            if (!isConnected) {
                notifyIcon.ShowBalloonTip(1000, "错误", "当前还未连接到设备", ToolTipIcon.Error);
                return;
            }
            var isStarted = deviceClient.IsAppRunning(packageName);
            if (isStarted) {
                stopAppCmd(packageName);
                notifyIcon.ShowBalloonTip(1000, "提示", "已尝试停止应用", ToolTipIcon.Info);
            } else {
                startAppCmd(packageName + "/" + activityName);
                notifyIcon.ShowBalloonTip(1000, "提示", "已尝试启动应用", ToolTipIcon.Info);
            }
        }

        private void SwitchConnect_Click(object sender, EventArgs e) {
            if (isConnected) {
                var result = adbClient.Disconnect(String.Format("{0}:{1}", adbHost.Text, adbPort.Text));
                PrintLog(LogLevel.Info, "已断开连接：" + result);
                SetAdbConfigEnabled(true);
                isConnected = false;
                notifyIcon.ShowBalloonTip(1000, "提示", "已断开连接", ToolTipIcon.Info);
            } else {
                if (adbClient == null) {
                    adbClient = new AdbClient();
                }
                PrintLog(LogLevel.Info, "正在尝试连接：" + adbHost.Text + ":" + adbPort.Text);
                var result = adbClient.Connect(String.Format("{0}:{1}", adbHost.Text, adbPort.Text));
                PrintLog(LogLevel.Info, "连接到 ADB 服务器：" + result);
                deviceData = adbClient.GetDevices().First();
                if (deviceData != null) {
                    deviceClient = new DeviceClient(adbClient, deviceData);
                    PrintLog(LogLevel.Info, "已连接：" + deviceData.Name);
                    SetAdbConfigEnabled(false);
                    isConnected = true;
                    notifyIcon.ShowBalloonTip(1000, "提示", "已连接：" + deviceData.Name, ToolTipIcon.Info);
                } else {
                    PrintLog(LogLevel.Error, "未找到可用的设备，请检查模拟器是否在运行中！");
                    SetAdbConfigEnabled(true);
                    isConnected = false;
                    notifyIcon.ShowBalloonTip(1000, "错误", "未找到可用的设备，请检查模拟器是否在运行中！", ToolTipIcon.Error);
                }
            }
        }

        private void Open_Click(object sender, EventArgs e) {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void Exit_Click(object sender, EventArgs e) {
            notifyIcon.Visible = false;
            Application.Exit();
        }

        private bool IsUsingDarkTheme() {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            if (key != null) {
                var theme = key.GetValue("AppsUseLightTheme");
                if (theme != null) {
                    return theme.ToString() == "0";
                }
            }
            return false;
        }

        private void Form1_Load(object sender, EventArgs e) {
            if (!File.Exists(@"adb\adb.exe")) {
                MessageBox.Show("未找到 adb.exe，请检查 adb 目录及 adb.exe 是否存在。", "未找到 adb");
                Application.Exit();
                return;
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
            autoLockScreen = config.GetBoolConfig("autoLockScreen", false);

            // after launch
            afterLaunch = config.GetConfig("afterLaunch", "");
            afterLaunchArgs = config.GetConfig("afterLaunchArgs", "");

            // package
            packageName = config.GetConfig("packageName", "com.qinlin.edoor");
            activityName = config.GetConfig("activityName", ".MainActivity");

            // Check is config file exists
            if (!File.Exists("config.ini")) {
                PrintLog(LogLevel.Info, "配置文件不存在，正在创建...");
                SaveConfigToFile();
            }

            PrintLog(LogLevel.Info, "配置项加载成功");

            // 1 Second Tick
            checkThread = new Thread(() =>
            {
                USBMode.Visible = true;
                USBMode.Text = "正在加载...";

                if (!AdbServer.Instance.GetStatus().IsRunning) {
                    USBMode.Visible = true;
                    USBMode.Text = "正在启动 ADB 服务器...";
                    AdbServer server = new AdbServer();
                    StartServerResult result = server.StartServer(@"adb\adb.exe", false);
                    if (result != StartServerResult.Started) {
                        PrintLog(LogLevel.Error, "无法启动 ADB 服务器");
                    } else {
                        PrintLog(LogLevel.Info, "ADB 服务器已启动");
                    }
                } else {
                    PrintLog(LogLevel.Info, "ADB 服务器已在运行中");
                }

                USBMode.Text = "USB 设备模式";

                if (adbClient == null) {
                    adbClient = new AdbClient();
                }

                Thread.Sleep(1000);
                
                if (adbClient?.GetDevices().Count() > 0) {
                    deviceData = adbClient.GetDevices().First();
                    if (deviceData != null && deviceData.Name != "") {
                        deviceClient = new DeviceClient(adbClient, deviceData);
                        PrintLog(LogLevel.Info, "已连接：" + deviceData.Name);
                        SetAdbConfigEnabled(false);
                        isConnected = true;
                        USBMode.Visible = true;
                    } else {
                        USBMode.Visible = false;
                    }
                } else {
                    USBMode.Visible = false;
                }

                while (isLooping) {
                    if (isConnected && adbClient != null) {
                        ProcessUpdate();
                    }
                    Thread.Sleep(1000);
                    if (!isLooping) {
                        break;
                    }
                    if (isConnected) {
                        connectStatus.Text = "状态：已连接 ADB";
                        contextMenu.Items[0].Text = "断开连接";
                    } else {
                        connectStatus.Text = "状态：未连接 ADB";
                        contextMenu.Items[0].Text = "连接设备";
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
                        var isStarted = deviceClient.IsAppRunning(packageName);
                        if (isStarted) {
                            stopAppCmd(packageName);
                        }
                        Thread.Sleep(1000);
                        startAppCmd(packageName + "/" + activityName);
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
                PrintLog(LogLevel.Info, String.Format("API 服务器监听端口：0.0.0.0:{0}", apiPort));
                while (isLooping) {
                    var context = httpListener.GetContext();
                    var request = context.Request;
                    var response = context.Response;
                    var output = response.OutputStream;
                    var url = request.Url.AbsolutePath;
                    var query = request.Url.Query;
                    // verify token
                    var token = HttpUtility.ParseQueryString(query).Get("token");
                    if (token != apiToken) {
                        response.StatusCode = 403;
                        response.StatusDescription = "Forbidden";
                        response.Close();
                        continue;
                    } else if (url == "/") {
                        string[] lines = logTextbox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                        int startLine = Math.Max(0, lines.Length - 1000);
                        string last1000Lines = string.Join(Environment.NewLine, lines.Skip(startLine));
                        var html = ReadFile("adb\\web.html");
                        byte[] bytes = Encoding.UTF8.GetBytes(html);
                        response.StatusCode = 200;
                        response.StatusDescription = "OK";
                        response.ContentType = "text/html";
                        response.ContentLength64 = bytes.Length;
                        // output.Write(bytes, 0, bytes.Length);
                        response.Close(bytes, true);
                        continue;
                    } else if (url == "/info") {
                        try {
                            if (DateTime.Now.Subtract(lastCacheInfo).TotalMilliseconds > 2000) {
                                var adbStatus = isConnected ? "已连接" : "已断开";
                                var appStatus = isConnected && deviceClient != null && deviceClient.IsAppRunning(packageName) ? "运行中" : "已停止";
                                var deviceName = deviceData != null && deviceData.State == DeviceState.Online ? deviceData.Name : "未知";
                                var logLines = logTextbox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                                var startLine = Math.Max(0, logLines.Length - 1000);
                                var last1000Lines = string.Join(Environment.NewLine, logLines.Skip(startLine));

                                var jsonOutput = new {
                                    AdbStatus = adbStatus,
                                    AppStatus = appStatus,
                                    DeviceName = deviceName,
                                    Log = last1000Lines
                                };

                                cachedInfo = jsonOutput;

                                var json = Newtonsoft.Json.JsonConvert.SerializeObject(jsonOutput);
                                byte[] bytes = Encoding.UTF8.GetBytes(json);
                                response.StatusCode = 200;
                                response.StatusDescription = "OK";
                                response.ContentType = "application/json";
                                response.ContentLength64 = bytes.Length;
                                // output.Write(bytes, 0, bytes.Length);
                                response.Close(bytes, true);
                                continue;
                            } else {
                                var json = Newtonsoft.Json.JsonConvert.SerializeObject(cachedInfo);
                                byte[] bytes = Encoding.UTF8.GetBytes(json);
                                response.StatusCode = 200;
                                response.StatusDescription = "OK";
                                response.ContentType = "application/json";
                                response.ContentLength64 = bytes.Length;
                                // output.Write(bytes, 0, bytes.Length);
                                response.Close(bytes, true);
                                continue;
                            }
                        } catch (Exception ex) {
                            var logLines = logTextbox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                            var startLine = Math.Max(0, logLines.Length - 1000);
                            var last1000Lines = string.Join(Environment.NewLine, logLines.Skip(startLine));
                            var jsonOutput = new {
                                AdbStatus = false,
                                AppStatus = false,
                                DeviceName = "Unknown",
                                Log = last1000Lines
                            };
                            var json = Newtonsoft.Json.JsonConvert.SerializeObject(jsonOutput);
                            byte[] bytes = Encoding.UTF8.GetBytes(json);
                            response.StatusCode = 200;
                            response.StatusDescription = "OK";
                            response.ContentType = "application/json";
                            response.ContentLength64 = bytes.Length;
                            // output.Write(bytes, 0, bytes.Length);
                            response.Close(bytes, true);
                            continue;
                        }
                    } else if (url == "/click") {
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
                    } else if (url == "/swipe") {
                        var x1 = HttpUtility.ParseQueryString(query).Get("x1");
                        var y1 = HttpUtility.ParseQueryString(query).Get("y1");
                        var x2 = HttpUtility.ParseQueryString(query).Get("x2");
                        var y2 = HttpUtility.ParseQueryString(query).Get("y2");
                        var sp = HttpUtility.ParseQueryString(query).Get("sp");
                        if (x1 != null && y1 != null && x2 != null & y2 != null) {
                            int intX1, intY1, intX2, intY2, speed;
                            if (int.TryParse(x1, out intX1) && int.TryParse(y1, out intY1) && int.TryParse(x2, out intX2) && int.TryParse(y2, out intY2) && int.TryParse(sp, out speed)) {
                                deviceClient.SwipeAsync(intX1, intY1, intX2, intY2, speed);
                                // executeAdbCmd(string.Format("shell input swipe {0} {1} {2} {3} {4}", intX1, intY1, intX2, intY2, speed));
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
                    } else if (url == "/screenshot") {
                        // take screenshot
                        if (deviceClient != null) {
                            response.ContentType = "image/jpeg";
                            response.StatusCode = 200;
                            response.StatusDescription = "OK";
                            response.AddHeader("Cache-Control", "max-age=0, must-revalidate");
                            var now = DateTime.Now;
                            if (now.Subtract(lastScreenshotTime).TotalMicroseconds < 1000) {
                                // lastScreenshot.Save(output, ImageFormat.Jpeg);
                                byte[] bytes = BitmapToBytes(lastScreenshot);
                                response.Close(bytes, true);
                            } else {
                                try {
                                    Bitmap bitmap = TakeSnapshot();
                                    // bitmap.Save(output, ImageFormat.Jpeg);
                                    byte[] bytes = BitmapToBytes(bitmap);
                                    lastScreenshot = bitmap;
                                    lastScreenshotTime = now;
                                    response.Close(bytes, true);
                                } catch (Exception ex) {
                                    PrintLog(LogLevel.Error, ex.ToString());
                                }
                            }
                            continue;
                        }
                    } else if (url == "/restart") {
                        // restart app
                        if (deviceClient != null) {
                            stopAppCmd(packageName);
                            Thread.Sleep(1000);
                            startAppCmd(packageName + "/" + activityName);
                            response.StatusCode = 200;
                            response.StatusDescription = "OK";
                        } else {
                            response.StatusCode = 404;
                            response.StatusDescription = "Not Found";
                        }
                    } else if (url == "/powerbtn") {
                        if (deviceClient != null) {
                            powerBtn();
                        }
                    } else if (url == "/unlock") {
                        if (deviceClient != null) {
                            unlockScreen();
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

            if (afterLaunch != null && afterLaunch != "") {
                if (!File.Exists(afterLaunch)) {
                    PrintLog(LogLevel.Warning, "自定义程序不存在，请检查路径是否配置正确：" + afterLaunch);
                } else {
                    PrintLog(LogLevel.Info, "执行自定义程序：" + afterLaunch);
                    // after launch
                    softwareThread = new Thread(() =>
                    {
                        // 创建一个新的进程启动信息
                        afterPro = new System.Diagnostics.Process();
                        afterPro.StartInfo.FileName = afterLaunch;
                        afterPro.StartInfo.Arguments = afterLaunchArgs;
                        afterPro.StartInfo.RedirectStandardOutput = true;
                        afterPro.StartInfo.RedirectStandardError = true;
                        afterPro.StartInfo.UseShellExecute = false;
                        afterPro.StartInfo.CreateNoWindow = true;
                        afterPro.Start();

                        // 启动进程并读取输出
                        while (!afterPro.HasExited) {
                            string x = afterPro.StandardOutput.ReadLine();
                            PrintLog(LogLevel.Info, x);
                        }
                    });
                    softwareThread.IsBackground = true;
                    softwareThread.Start();
                }
            }
        }

        private void ProcessUpdate() {
            try {
                if (adbClient != null && deviceClient != null) {
                    if (isConnected) {
                        var isStarted = deviceClient.IsAppRunning(packageName);
                        if (isStarted) {
                            appStatus.Text = "应用运行中";
                            startApp.Enabled = false;
                            stopApp.Enabled = true;
                            contextMenu.Items[1].Enabled = true;
                            contextMenu.Items[1].Text = "停止应用";
                        } else {
                            appStatus.Text = "应用未运行";
                            startApp.Enabled = true;
                            stopApp.Enabled = false;
                            contextMenu.Items[1].Enabled = true;
                            contextMenu.Items[1].Text = "启动应用";
                        }
                    } else {
                        appStatus.Text = "应用未运行";
                        startApp.Enabled = false;
                        stopApp.Enabled = false;
                        contextMenu.Items[1].Enabled = false;
                        contextMenu.Items[1].Text = "启动应用";
                    }
                }
            } catch (Exception e) {
                PrintLog(LogLevel.Error, e.Message);
                stopApp.Enabled = false;
                startApp.Enabled = false;
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

        public byte[] BitmapToBytes(Bitmap bitmap) {
            using (MemoryStream stream = new MemoryStream()) {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        private string ReadFile(string path) {
            if (!File.Exists(path)) return "";
            var result = File.ReadAllText(path);
            return result;
        }

        private void connectBtn_Click(object sender, EventArgs e) {
            if (adbClient == null) {
                adbClient = new AdbClient();
            }
            PrintLog(LogLevel.Info, String.Format("正在尝试连接：{0}:{1}", adbHost.Text, adbPort.Text));
            try {
                var result = adbClient.Connect(String.Format("{0}:{1}", adbHost.Text, adbPort.Text));
                PrintLog(LogLevel.Info, "Adb 返回信息：" + result);
                deviceData = adbClient.GetDevices().FirstOrDefault();
                if (deviceData != null && deviceData != default && !result.Contains("cannot")) {
                    deviceClient = new DeviceClient(adbClient, deviceData);
                    PrintLog(LogLevel.Info, "已连接：" + deviceData.Name);
                    SetAdbConfigEnabled(false);
                    isConnected = true;
                } else {
                    PrintLog(LogLevel.Error, "无法连接到设备，请检查 IP 地址和端口是否正确，模拟器/设备是否在运行中！");
                    SetAdbConfigEnabled(true);
                    isConnected = false;
                }
            } catch (Exception ex) {
                PrintLog(LogLevel.Error, ex.Message);
            }
        }

        private void startApp_Click(object sender, EventArgs e) {
            startAppCmd(packageName + "/" + activityName);
            PrintLog(LogLevel.Info, "已尝试启动应用");
        }

        private void stopApp_Click(object sender, EventArgs e) {
            stopAppCmd(packageName);
            PrintLog(LogLevel.Info, "已尝试停止应用");
        }

        private void startAppCmd(string packageName) {
            executeAdbCmd(String.Format("shell am start {0}", packageName));
        }

        private void stopAppCmd(string packageName) {
            executeAdbCmd(String.Format("shell am force-stop {0}", packageName));
        }

        private void powerBtn() {
            // executeAdbCmd("shell input keyevent 26");
            deviceClient.SendKeyEvent("KEYCODE_POWER");
        }

        private void unlockScreen() {
            // executeAdbCmd("shell input swipe 300 1000 300 500");
            deviceClient.Swipe(300, 1000, 300, 500, 200);
        }

        private int getScreenState() {
            string result = executeAdbCmd("shell dumpsys window policy");
            int state = 0;
            if (result.Contains("showing=true")) {
                state = 1;
                if (result.Contains("screenState=2")) {
                    state = 2;
                }
            }
            return state;
        }

        private Bitmap TakeSnapshot() {
            /* MemoryStream scrbf = new MemoryStream();
            Process cmd = new Process();
            cmd.StartInfo.FileName = "adb";
            cmd.StartInfo.Arguments = "exec-out screencap";
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.Start();
            Bitmap bitmap;
            using (MemoryStream ms = new MemoryStream()) {
                cmd.StandardOutput.BaseStream.CopyTo(ms);
                byte[] bytes = ms.ToArray();
                bitmap = ConvertToBitmap(bytes);
            }
            cmd.WaitForExit();
            return bitmap; */
            Bitmap bitmap = adbClient.GetFrameBuffer(deviceData).ToImage();
            return bitmap;
        }

        private Bitmap ConvertToBitmap(byte[] data) {
            using (MemoryStream ms = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(ms)) {
                uint width = reader.ReadUInt32();
                uint height = reader.ReadUInt32();
                uint pixelFormat = reader.ReadUInt32();
                int bytesPerPixel = 4; // Assuming RGBA_8888 format
                int imageSize = (int)(width * height * bytesPerPixel);
                byte[] imageData = reader.ReadBytes(imageSize);
                Bitmap bitmap = new Bitmap((int)width, (int)height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                int index = 0;
                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        byte r = imageData[index++];
                        byte g = imageData[index++];
                        byte b = imageData[index++];
                        byte a = imageData[index++];
                        Color color = Color.FromArgb(a, r, g, b);
                        bitmap.SetPixel(x, y, color);
                    }
                }

                return bitmap;
            }
        }

        private void executeAdbCmd(string cmd, bool waitForExit = false) {
            if (!isConnected) return;

            // 创建一个新的进程启动信息
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "adb\\adb.exe";
            startInfo.Arguments = cmd;
            startInfo.RedirectStandardOutput = false;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            // 启动进程
            Process process = Process.Start(startInfo);
            if (waitForExit && process != null && process.Id > 0) {
                process.WaitForExit();
            }
        }

        private string executeAdbCmd(string cmd) {
            if (!isConnected) return "";

            // 创建一个新的进程启动信息
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "adb\\adb.exe";
            startInfo.Arguments = cmd;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            string result = "";
            // 启动进程并读取输出
            using (Process process = Process.Start(startInfo)) {
                using (System.IO.StreamReader reader = process.StandardOutput) {
                    result += reader.ReadToEnd();
                }
            }

            return result;
        }

        private void disconnectBtn_Click(object sender, EventArgs e) {
            try {
                var result = adbClient.Disconnect(String.Format("{0}:{1}", adbHost.Text, adbPort.Text));
                PrintLog(LogLevel.Info, "已断开连接：" + result);
            } catch (Exception ex) {
                PrintLog(LogLevel.Error, ex.Message);
            }
            SetAdbConfigEnabled(true);
            isConnected = false;
        }

        private void CloseAd() {
            Thread thread = new Thread(() =>
            {
                Thread.Sleep(closeWait);
                stopAppCmd(packageName);
                Thread.Sleep(300);
                startAppCmd(packageName + "/" + activityName);
                if (autoLockScreen) {
                    Thread.Sleep(1000);
                    powerBtn();
                }
            });
            thread.Start();
        }

        private void ClickButton(int BtnId) {
            if (deviceClient == null) {
                PrintLog(LogLevel.Error, "当前未连接到设备。");
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
                int state = getScreenState();
                if (state == 1) {
                    powerBtn();
                    Thread.Sleep(10);
                    unlockScreen();
                    Thread.Sleep(500);
                } else if (state == 2) {
                    unlockScreen();
                    Thread.Sleep(500);
                }
                deviceClient.ClickAsync(pos.X, pos.Y);
                PrintLog(LogLevel.Info, String.Format("已发送屏幕点击请求：Btn ({0})", BtnId));
                CloseAd();
            } else {
                PrintLog(LogLevel.Error, String.Format("无效的按钮：{0}（无法获取坐标）", BtnId));
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
            PrintLog(LogLevel.Info, "配置文件已保存");
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
            config.SetConfig("autoLockScreen", autoLockScreen.ToString());
            // after launch
            config.SetConfig("afterLaunch", afterLaunch);
            config.SetConfig("afterLaunchArgs", afterLaunchArgs);
            // package
            config.SetConfig("packageName", packageName);
            config.SetConfig("activityName", activityName);

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
                g.Clear(Color.FromArgb(33, 33, 33));

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

        private void SplitterPaint(object sender, PaintEventArgs e) {
            SplitContainer s = sender as SplitContainer;
            if (s != null) {
                int top = 5;
                int bottom = s.Height - 5;
                int left = s.SplitterDistance;
                int right = left + s.SplitterWidth - 1;
                e.Graphics.DrawLine(Pens.Silver, left, top, left, bottom);
                e.Graphics.DrawLine(Pens.Silver, right, top, right, bottom);
            }
        }

        private void ExecuteCmd(string cmd, string args) {
            try {
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = cmd;
                processStartInfo.Arguments = args;
                processStartInfo.UseShellExecute = true;
                Process process = Process.Start(processStartInfo);
                process.WaitForExit();
            } catch (Exception ex) {
                PrintLog(LogLevel.Error, ex.Message);
            }
        }

        private void FuckQL_FormClosing(object sender, FormClosingEventArgs e) {
            isConnected = false;
            isLooping = false;
        }

        private void RemoteAndroid_FormClosed(object sender, FormClosedEventArgs e) {
            try {
                if (adbClient != null && adbHost != null && adbPort != null) {
                    adbClient.Disconnect(String.Format("{0}:{1}", adbHost.Text, adbPort.Text));
                }
            } catch { }
            try {
                AdbServer.Instance.StopServer();
            } catch { }
            try {
                afterPro.Kill();
            } catch { }
            notifyIcon?.Icon?.Dispose();
            notifyIcon?.Dispose();
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
