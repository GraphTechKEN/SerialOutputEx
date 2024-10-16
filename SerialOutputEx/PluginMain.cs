using System;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Management;

using BveTypes.ClassWrappers;

using AtsEx.PluginHost.Plugins;
using AtsEx.PluginHost.Plugins.Extensions;
using AtsEx.Extensions.ContextMenuHacker;
using AtsEx.PluginHost;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace SerialOutputEx
{
    [PluginType(PluginType.Extension)]
    [HideExtensionMain]
    internal class PluginMain : AssemblyPluginBase, IExtension
    {

        private SerialPort serialPort;
        
        private bool Debug = false;
        private OutputInfo outputInfo = new OutputInfo();

        private string str_send_latch = "";
        private DateTime dateTime;

        private bool flgScenarioOpened = false;

        private bool flgFirstSend = false;

        private bool flgConsoleOpened = false;

        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        private ToolStripMenuItem tsiOutput;
        private ToolStripMenuItem tsiConsole;
        private ToolStripMenuItem tsiPorts;
        private ToolStripMenuItem tsiOpenEditor;
        private ToolStripMenuItem tsiStartingNotchSet;
        private ToolStripMenuItem tsiAutoBrakeSet;
        private ToolStripMenuItem tsiAutoNotchFit;
        private ToolStripMenuItem tsiSettings;
        private ToolStripSeparator tsiSeperator;

        private string portName = "";
        private string editorPath = "";

        private bool IsStartingNotchSet = false;

        private bool flgAutoBrakeSetChange = false;
        private bool flgAutoNotchFitChange = false;
        private bool IsUseAutoBrake = false;
        private bool IsUseAutoNotchFit = false;
        private string xmlpath;

        public PluginMain(PluginBuilder builder) : base(builder)
        {

            //0.4.31219.1 追記
            Extensions.AllExtensionsLoaded += Extensions_AllExtensionsLoaded;

            string stTarget = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            string dllPath = "";
            //ローカルファイルのパスを表すURI
            string uriPath = stTarget;
            //Uriオブジェクトを作成する
            Uri u = new Uri(uriPath);
            //変換するURIがファイルを表していることを確認する
            if (u.IsFile)
            {
                //Windows形式のパス表現に変換する
                dllPath = u.LocalPath + Uri.UnescapeDataString(u.Fragment);
            }
            else
            {
                MessageBox.Show("ファイルURIではありません。");
            }

            string dir = Path.GetDirectoryName(dllPath);
            string fileName = "シリアル出力エディタ";
            string path = dir + @"\" + fileName + ".exe";

            editorPath = path;

            IsStartingNotchSet = Properties.Settings.Default.IsStartingNotchSet;
            IsUseAutoBrake = Properties.Settings.Default.UseAutoBrake;
            IsUseAutoNotchFit = Properties.Settings.Default.IsUseAutoNotchFit;
            //Properties.Settings.Default.Reload();

        }

        

        private void BveHacker_ScenarioCreated(ScenarioCreatedEventArgs e)
        {
            //
        }

        private void Extensions_AllExtensionsLoaded(object sender, EventArgs e)
        {
            string openedPortName = Open(portName,false);
            tsiOutput = Extensions.GetExtension<IContextMenuHacker>().AddCheckableMenuItem("SerialOutputEx 連動", SerialOutputEx_Change, ContextMenuItemType.CoreAndExtensions);
            tsiConsole = Extensions.GetExtension<IContextMenuHacker>().AddCheckableMenuItem("デバッグコンソール 表示", DebugConsoleDisp_Change, ContextMenuItemType.CoreAndExtensions);
            tsiOpenEditor = Extensions.GetExtension<IContextMenuHacker>().AddClickableMenuItem("SerialOutputEx 設定", SerialOutputExEdit_Open, ContextMenuItemType.CoreAndExtensions);
            tsiStartingNotchSet = Extensions.GetExtension<IContextMenuHacker>().AddCheckableMenuItem("開始時 ノッチ設定転送", SerialOutputExStartingNotchSet_Change, ContextMenuItemType.CoreAndExtensions);
            tsiAutoBrakeSet = Extensions.GetExtension<IContextMenuHacker>().AddCheckableMenuItem("自動ブレーキ帯 使用", AutoBrakeSet_Change, ContextMenuItemType.CoreAndExtensions);
            tsiAutoNotchFit = Extensions.GetExtension<IContextMenuHacker>().AddCheckableMenuItem("自動ノッチ合わせ", AutoNotchFit_Change, ContextMenuItemType.CoreAndExtensions);
            tsiSettings = Extensions.GetExtension<IContextMenuHacker>().AddClickableMenuItem("設定", Settings_Click, ContextMenuItemType.CoreAndExtensions);
            tsiSeperator = Extensions.GetExtension<IContextMenuHacker>().AddSeparator(0);

            if (Debug)
            {
                tsiConsole.Checked = true;
            }
            if (serialPort.IsOpen)
            {
                tsiOutput.Checked = true;
            }

            tsiStartingNotchSet.Checked = IsStartingNotchSet;
            tsiAutoBrakeSet.Checked = IsUseAutoBrake;
            tsiAutoNotchFit.Checked = IsUseAutoNotchFit;

            tsiStartingNotchSet.Enabled = tsiOutput.Checked;
            tsiAutoBrakeSet.Enabled = tsiOutput.Checked;
            tsiAutoNotchFit.Enabled = tsiOutput.Checked;


            //"シナリオを開く"イベント
            BveHacker.ScenarioOpened += BveHacker_ScenarioOpened;

            //コンテクストメニューを開くイベント
            BveHacker.MainForm.ContextMenu.Opened += ContextMenu_Opened;
            //コンテクストメニューのアイテムクリックイベント
            BveHacker.MainForm.ContextMenu.ItemClicked += ContextMenu_ItemClicked;

            //0.3.31218.1 追記ここまで

            try
            {
                if (!File.Exists(editorPath))
                {
                    tsiOpenEditor.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("例外:" + ex.Message);
                Dispose();
            }
        }

        //シリアル出力エディタの起動
        private void SerialOutputExEdit_Open(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(editorPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("例外:" + ex.Message);
                Dispose();
            }
        }

        //ポート選択メニュー操作
        private void TsiPorts_Click(object sender, EventArgs e)
        {
            // sender にはクリックされたメニューの ToolStripMenuItem が入っている
            // ポートを開きなおす処理
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            if (mi.Text != serialPort.PortName)
            {
                try
                {
                    if (serialPort.IsOpen)
                    {
                        serialPort.Close();
                    }
                    Regex regexPortName = new Regex(@"(COM\d+)");
                    portName = regexPortName.Match(mi.Text).Groups[1].ToString();

                    int.TryParse(portName.Substring(3), out int iPortNum);
                    outputInfo.PortNum = iPortNum;

                    //XmlSerializerオブジェクトを作成
                    //オブジェクトの型を指定する
                    XmlSerializer serializer = new XmlSerializer(typeof(OutputInfo));
                    //書き込むファイルを開く（UTF-8 BOM無し）
                    StreamWriter sw = new StreamWriter(xmlpath, false, new UTF8Encoding(false));
                    //シリアル化し、XMLファイルに保存する
                    serializer.Serialize(sw, outputInfo);
                    sw.Close();

                    Open(portName,true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        //シリアル出力(ON/OFF)メニュー変更
        private void SerialOutputEx_Change(object sender, EventArgs e)
        {
            if (tsiOutput.Checked)
            {
                if (!serialPort.IsOpen)
                {
                    Open(portName,false);
                }
            }
            else
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }
            }
        }

        //ブレーキ段数自動設定(ON/OFF)メニュー変更
        private void AutoBrakeSet_Change(object sender, EventArgs e)
        {
            flgAutoBrakeSetChange = true;
            IsUseAutoBrake = tsiAutoBrakeSet.Checked;
            Properties.Settings.Default.UseAutoBrake = IsUseAutoBrake;
        }

        private void AutoNotchFit_Change(object sender, EventArgs e)
        {
            flgAutoNotchFitChange = true;
            IsUseAutoNotchFit = tsiAutoNotchFit.Checked;
            Properties.Settings.Default.IsUseAutoNotchFit = IsUseAutoNotchFit;
        }

        //デバッグコンソール表示(ON/OFF)変更
        private void DebugConsoleDisp_Change(object sender, EventArgs e)
        {
            Debug = tsiConsole.Checked;
            if (tsiConsole.Checked)
            {                
                ConsoleOpen();
            }
            else
            {
                ConsoleClose();
            }
            outputInfo.Debug = Debug;

            //XmlSerializerオブジェクトを作成
            //オブジェクトの型を指定する
            XmlSerializer serializer = new XmlSerializer(typeof(OutputInfo));
            //書き込むファイルを開く（UTF-8 BOM無し）
            StreamWriter sw = new StreamWriter(xmlpath, false, new UTF8Encoding(false));
            //シリアル化し、XMLファイルに保存する
            serializer.Serialize(sw, outputInfo);
            sw.Close();
        }

        private void SerialOutputExStartingNotchSet_Change(object sender, EventArgs e)
        {
            IsStartingNotchSet = tsiStartingNotchSet.Checked;
            Properties.Settings.Default.IsStartingNotchSet = IsStartingNotchSet;

        }


        private void BveHacker_ScenarioOpened(ScenarioOpenedEventArgs e)
        {
            flgScenarioOpened = true;
            flgFirstSend = true;
        }


        private void ContextMenu_Opened(object sender, EventArgs e)
        {
            tsiSettings.DropDownItems.Clear();

            ToolStripSeparator tss = new ToolStripSeparator();
            tsiSettings.DropDownItems.Add(tsiOpenEditor);
            tsiSettings.DropDownItems.Add(tss);
            tsiSettings.DropDownItems.Add(tsiConsole);
            tsiSettings.DropDownItems.Add(tsiStartingNotchSet);
            tsiSettings.DropDownItems.Add(tsiAutoBrakeSet);
            tsiSettings.DropDownItems.Add(tsiAutoNotchFit);

            tsiOutput.Enabled = !flgScenarioOpened;
            tsiOutput.Checked = serialPort.IsOpen;

            tsiStartingNotchSet.Enabled = tsiOutput.Checked;
            tsiAutoBrakeSet.Enabled = tsiOutput.Checked;
            tsiAutoNotchFit.Enabled = tsiOutput.Checked;

            if (!flgScenarioOpened)
            {

                tsiOutput.DropDownItems.Clear();

                string[] ports = GetDeviceNames();


                if (ports != null)
                {
                    foreach (var _portName in ports)
                    {
                        try
                        {
                            tsiPorts = new ToolStripMenuItem(_portName);

                            // クリックイベントを追加
                            tsiPorts.Click += TsiPorts_Click;
                            //使用中ポートにチェック、Enableとする
                            if ((_portName.Contains("("+serialPort.PortName+")")) && serialPort.IsOpen)
                            {
                                tsiPorts.Checked = true;
                                tsiPorts.Enabled = false;
                            }
                            else
                            {
                                tsiPorts.Checked = false;
                                tsiPorts.Enabled = true;
                            }
                            tsiOutput.DropDownItems.Add(tsiPorts);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
        }

        //シリアルポート名称一覧を取得
        public string[] GetDeviceNames()
        {
            var deviceNameList = new System.Collections.ArrayList();
            Regex regexPortName = new Regex(@"(COM\d+)");

            ManagementClass mcPnPEntity = new ManagementClass("Win32_PnPEntity");
            ManagementObjectCollection manageObjCol = mcPnPEntity.GetInstances();

            //全てのPnPデバイスを探索しシリアル通信が行われるデバイスを随時追加する
            foreach (ManagementObject manageObj in manageObjCol)
            {               
                var namePropertyValue = manageObj["Name"];//Nameプロパティを取得
                if (namePropertyValue != null)
                {
                    string classGuid = manageObj["ClassGuid"] as string; // GUID
                    string devicePass = manageObj["DeviceID"] as string; // デバイスインスタンスパス
                    //Nameプロパティ文字列の一部が"(COM1)～(COM999)"と一致するときリストに追加"
                    string name = namePropertyValue.ToString();
                    if (regexPortName.IsMatch(name) && classGuid != null && devicePass != null)
                    {

                        // デバイスインスタンスパスからシリアル通信接続機器のみを抽出
                        // {4d36e978-e325-11ce-bfc1-08002be10318}はシリアル通信接続機器を示す固定値
                        if (String.Equals(classGuid, "{4d36e978-e325-11ce-bfc1-08002be10318}",
                                StringComparison.InvariantCulture))
                        {

                            // デバイスインスタンスパスからデバイスIDを2段階で抜き出す
                            string[] tokens = devicePass.Split('&');

                            //Bluetoothデバイスかその他(USB等)デバイスかを判別
                            //Bluetoothデバイスのとき
                            if (tokens.Length > 4)
                            {
                                string[] addressToken = tokens[4].Split('_');
                                string[] deviceType = tokens[0].Split('\\');
                                string bluetoothAddress = addressToken[0];
                                if (deviceType[0] == "BTHENUM")
                                {
                                    Match m = regexPortName.Match(name);

                                    string comPortNumber = "";
                                    if (m.Success)
                                    {
                                        // COM番号を抜き出す
                                        comPortNumber = m.Groups[1].ToString();
                                    }

                                    if (Convert.ToUInt64(bluetoothAddress, 16) > 0)
                                    {
                                        string bluetoothName = GetBluetoothRegistryName(bluetoothAddress);
                                        deviceNameList.Add(bluetoothName + " (" + comPortNumber + ")");
                                    }
                                }
                                //それ以外のとき
                                else
                                {
                                    deviceNameList.Add(name);
                                }
                            }
                            //それ以外のとき
                            else
                            {
                                deviceNameList.Add(name);
                            }
                        }
                    }
                }
            }

            //戻り値作成
            if (deviceNameList.Count > 0)
            {
                string[] deviceNames = new string[deviceNameList.Count];
                int index = 0;
                foreach (var name in deviceNameList)
                {
                    deviceNames[index++] = name.ToString();
                }
                return deviceNames;
            }
            else
            {
                return null;
            }
        }

        private void ContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //シナリオを閉じるメニューをクリックした時のイベント
            if (e.ClickedItem.Name == "closeToolStripMenuItem")
            {
                flgScenarioOpened = false;
            }
        }
        private string Open(string portName, bool anotherPortOpen)
        {
            string stTarget = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            string dllPath = "";
            //ローカルファイルのパスを表すURI
            string uriPath = stTarget;
            //Uriオブジェクトを作成する
            Uri u = new Uri(uriPath);
            //変換するURIがファイルを表していることを確認する
            if (u.IsFile)
            {
                //Windows形式のパス表現に変換する
                dllPath = u.LocalPath + Uri.UnescapeDataString(u.Fragment);
            }
            else
            {
                MessageBox.Show("ファイルURIではありません。");
            }

            string xmldir = Path.GetDirectoryName(dllPath);
            string xmlfileName = Path.GetFileNameWithoutExtension(dllPath);　//0.2.31217.1 追記 設定ファイル名をdllと同じとする。
            xmlpath = xmldir + @"\" + xmlfileName + ".xml";

            try
            {
                if (!File.Exists(xmlpath))
                {
                    MessageBox.Show("設定ファイル " + xmlfileName + ".xml が見つかりません");　//0.2.31217.1 変更 設定ファイル名をdllと同じとする。
                }
                else
                {
                    portName = OutputOpen(xmlpath, portName, anotherPortOpen);
                }

            }
            catch (IOException ex)
            {
                MessageBox.Show("IOの例外:" + ex.Message);
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("例外:" + ex.Message);
                Dispose();
            }

            if (Debug && !flgConsoleOpened)
            {
                ConsoleOpen();
            }
            dateTime = DateTime.Now;
            return portName;
        }

        //コンソール表示
        private void ConsoleOpen()
        {
            /* 参考ページ：C#(Windows Formアプリケーション)でコンソールの表示、非表示、出力方法(Console.WriteLine())
            * https://github.com/murasuke/AllocConsoleCSharp
            */

            // Console表示
            AllocConsole();
            // コンソールとstdoutの紐づけを行う。無くても初回は出力できるが、表示、非表示を繰り返すとエラーになる。
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
            //コンソールの文字エンコードを指定。これがないとBVE本体からの情報が文字化けする。
            Console.OutputEncoding = System.Text.Encoding.GetEncoding("utf-8");

            flgConsoleOpened = true;
        }

        //コンソール非表示
        private void ConsoleClose()
        {
            FreeConsole();
            flgConsoleOpened = false;
        }

        private string OutputOpen(string path , string _portName , bool _anotherPortOpen)
        {

            //XmlSerializerオブジェクトを作成
            XmlSerializer serializer = new XmlSerializer(typeof(OutputInfo));

            //読み込むファイルを開く
            XmlReader reader = XmlReader.Create(path);

            //XMLファイルから読み込み、逆シリアル化する
            outputInfo = (OutputInfo)serializer.Deserialize(reader);
            reader.Close();

            using (serialPort = new SerialPort()

            {
                BaudRate = outputInfo.BaudRate,
                DataBits = outputInfo.DataBits,
                Parity = outputInfo.Parity,
                StopBits = outputInfo.StopBits,
                DtrEnable = false,
                RtsEnable = true,
                ReadBufferSize = 256,
                WriteBufferSize = 256,
                WriteTimeout = 1000,
                ReadTimeout = 1000,
                Handshake = Handshake.None,
        })
                serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            //同じを開くかポート番号がnullのとき
            if (!_anotherPortOpen || _portName == "")
            {
                serialPort.PortName = "COM" + outputInfo.PortNum.ToString();
            }

            //別のポートを開く指示があるとき
            else
            {
                serialPort.PortName = _portName;
            }

            Debug = outputInfo.Debug;

            //シリアルポートを開く
            try
            {
                serialPort.Open();
            }
            catch(IOException ex)
            {
                MessageBox.Show("シリアルポート "+ serialPort.PortName + " が存在しません。\nシナリオ選択画面を閉じ、右クリックメニューの[SerialOutputEX 連動]メニューから別のポートを選択してください。" );
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return serialPort.PortName;

        }

        public override void Dispose()
        {
            //AtsEXを選択解除したときに
            //コンソールを非表示とする //0.2.31217.1 追記
            if (Debug || flgConsoleOpened)
            {
                ConsoleClose();
            }
            //シリアルポートを閉じる
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }
            }
            catch { }
            Properties.Settings.Default.Save();
        }

        static int brake;

        public override TickResult Tick(TimeSpan elapsed)
        {

            if (serialPort.IsOpen)
            {

                if (flgFirstSend && IsStartingNotchSet)
                {
                    if (serialPort.IsOpen)
                    {
                        serialPort.Write("WR 004 " + BveHacker.Scenario.Vehicle.Instruments.Cab.Handles.NotchInfo.BrakeNotchCount.ToString() + "\r");
                        serialPort.Write("WR 072 " + BveHacker.Scenario.Vehicle.Instruments.Cab.Handles.NotchInfo.PowerNotchCount.ToString() + "\r");
                        flgFirstSend = false;
                    }
                    if (Debug)
                    {
                        Console.Write("ブレーキ段数         WR 004 " + BveHacker.Scenario.Vehicle.Instruments.Cab.Handles.NotchInfo.BrakeNotchCount.ToString() + "\r\n");
                        Console.Write("マスコンノッチ数     WR 072 " + BveHacker.Scenario.Vehicle.Instruments.Cab.Handles.NotchInfo.PowerNotchCount.ToString() + "\r\n");
                    }
                }

                if (flgAutoBrakeSetChange)
                {
                    if (serialPort.IsOpen)
                    {
                        if (IsUseAutoBrake) {
                            serialPort.Write("WR 068 1\r");
                            if (Debug)
                            {
                                Console.Write("自動ブレーキ有効     WR 068 1\r\n");
                            }
                        }
                        else
                        {
                            serialPort.Write("WR 068 0\r");
                            if (Debug)
                            {
                                Console.Write("自動ブレーキ無効     WR 068 0\r\n");
                            }
                        }
                    }
                    flgAutoBrakeSetChange = false;
                }

                if (flgAutoNotchFitChange)
                {
                    if (serialPort.IsOpen)
                    {
                        if (IsUseAutoNotchFit)
                        {
                            serialPort.Write("WR 078 1\r");
                            if (Debug)
                            {
                                Console.Write("自動ノッチ合わせ有効 WR 078 1\r\n");
                            }
                        }
                        else
                        {
                            serialPort.Write("WR 078 0\r");
                            if (Debug)
                            {
                                Console.Write("自動ノッチ合わせ無効 WR 078 0\r\n");
                            }
                        }
                    }
                    flgAutoNotchFitChange = false;
                }

                //If you want to change the Handle state, please access to Ats.Handle
                //送信コマンド用
                string str_send = "";
                //コマンド解析用
                string str = "";
                //コマンド連結
                if (outputInfo.outputInfoDataList != null || outputInfo.outputInfoDataList.Count != 0)
                {
                    for (int i = 0; i < outputInfo.outputInfoDataList.Count; i++)
                    {
                        str += SendCommandGenerator(outputInfo.outputInfoDataList[i]);
                    }
                }

                str_send = str;

                //送信コマンドが前回と異なる場合、かつ前回送信時間から0.02sec以上経過した場合のみ出力
                if ((str_send != str_send_latch) && ((DateTime.Now - dateTime).TotalSeconds > 0.02))
                {
                    dateTime = DateTime.Now;
                    //デバッグモードの時
                    if (Debug)
                    {
                        Console.Write(str_send + "\r\n");
                    }
                    //シリアル通信オープンの時
                    if (serialPort.IsOpen)
                    {
                        serialPort.Write(str_send + "\r");
                    }

                }
                str_send_latch = str_send;
            }
            return new ExtensionTickResult();

        }

        //送信コマンド生成
        private string SendCommandGenerator(OutputInfoData _data)
        {
            HandleSet handles = NewMethod();
            PluginLoader ats = BveHacker.Scenario.Vehicle.Instruments.PluginLoader;
            VehicleStateStore vehicleStateStore = ats.StateStore;

            string str = "";
            string format = "{0:" + _data.Base + _data.Digit.ToString() + "}";
            switch (_data.infoId)
            {

                case 0://列車位置
                    str = Right(string.Format(format, (int)(BveHacker.Scenario.LocationManager.Location * Math.Pow(10, _data.Pow))), _data.Digit);
                    break;

                case 1://列車速度
                    str = Right(string.Format(format, (int)(Math.Abs(vehicleStateStore.Speed[0]) * Math.Pow(10, _data.Pow))), _data.Digit);
                    break;

                case 2://現在時刻
                    str = Right(string.Format(format, (int)((int)BveHacker.Scenario.TimeManager.Time.TotalMilliseconds * Math.Pow(10, _data.Pow))), _data.Digit);
                    break;

                case 3://ブレーキシリンダ圧力
                    str = Right(string.Format(format, (int)(vehicleStateStore.BcPressure[0] * Math.Pow(10, _data.Pow))), _data.Digit);
                    break;

                case 4://元空気溜め圧力
                    str = Right(string.Format(format, (int)(vehicleStateStore.MrPressure[0] * Math.Pow(10, _data.Pow))), _data.Digit);
                    break;

                case 5://ツリアイ空気溜め圧力
                    str = Right(string.Format(format, (int)(vehicleStateStore.ErPressure[0] * Math.Pow(10, _data.Pow))), _data.Digit);
                    break;

                case 6://ブレーキ管圧力
                    str = Right(string.Format(format, (int)(vehicleStateStore.BpPressure[0] * Math.Pow(10, _data.Pow))), _data.Digit);
                    break;

                case 7://直通管圧力
                    str = Right(string.Format(format, (int)(vehicleStateStore.SapPressure[0] * Math.Pow(10, _data.Pow))), _data.Digit);
                    break;

                case 8://電流
                    string sign = " ";
                    if (vehicleStateStore.Current[0] < 0)
                    {
                        sign = "";
                    }
                    else if (vehicleStateStore.Current[0] > 0)
                    {
                        sign = "+";
                    }
                    str = Right(sign + string.Format(format, (int)(vehicleStateStore.Current[0] * Math.Pow(10, _data.Pow))), _data.Digit + 1);
                    break;

                case 9://ブレーキノッチ
                    str = Right(string.Format(format, handles.BrakeNotch), _data.Digit);
                    break;

                case 10://力行ノッチ
                    string prefix = "N";
                    if (handles.PowerNotch > 0)
                    {
                        prefix = "P";
                    }
                    else if (handles.PowerNotch < 0)
                    {
                        prefix = "H";
                    }
                    str = prefix + Right(string.Format(format, Math.Abs(handles.PowerNotch)), _data.Digit);
                    break;

                case 11://レバーサ位置
                    str = "N";
                    if (handles.ReverserPosition > 0)
                    {
                        str = "F";
                    }
                    else if (handles.ReverserPosition < 0)
                    {
                        str = "B";
                    }
                    break;

                case 12://ドア状態
                    str = BveHacker.Scenario.Vehicle.Doors.AreAllClosingOrClosed ? "0" : "1";
                    break;

                case 13://パネル状態
                    str = Right(string.Format(format, (int)(ats.PanelArray[_data.PanelNum])), _data.Digit);
                    break;

                case 14://固定文字列
                    str = _data.Value;
                    break;

                case 15://サウンド状態 　//0.2.31217.1 準備
                    str = Right(ats.SoundArray[_data.SoundNum].ToString(), 1);
                    break;

                default:
                    break;

            }

            return str;
        }

        private HandleSet NewMethod()
        {
            return BveHacker.Scenario.Vehicle.Instruments.Cab.Handles;
        }

        //文字列を末尾から指定文字数切り取るメソッド 0.2.31217.1 追加
        public static string Right(string str, int len)
        {
            if (len < 0)
            {
                throw new ArgumentException("引数'len'は0以上でなければなりません。");
            }
            if (str == null)
            {
                return "";
            }
            if (str.Length <= len)
            {
                return str;
            }
            return str.Substring(str.Length - len, len);
        }

        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            //Arduino Microとの通信で発火させるにはRTSをTrueにすること
            SerialPort sp = (SerialPort)sender;
            //ESP32でBluetoothSPPで落ちたのでtrycatch
            try
            {
                string indata = sp.ReadLine();
                Console.WriteLine("受信:" + indata);
            }
            catch
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>機器名称取得</summary> 
        /// <param name="address">[in] アドレス</param> 
        /// <returns>[out] 機器名称</returns> 
        private string GetBluetoothRegistryName(string address)
        {
            string deviceName = "";
            // 以下のレジストリパスはどのPCでも共通
            string registryPath = @"SYSTEM\CurrentControlSet\Services\BTHPORT\Parameters\Devices";
            string devicePath = String.Format(@"{0}\{1}", registryPath, address);

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(devicePath))
            {
                if (key != null)
                {
                    Object o = key.GetValue("Name");

                    byte[] raw = o as byte[];

                    if (raw != null)
                    {
                        // ASCII変換
                        deviceName = Encoding.ASCII.GetString(raw);
                    }
                }
            }
            // NULL文字をトリミングしてリターン
            return deviceName.TrimEnd('\0');
        }

        //設定メニュークリック
        private void Settings_Click(object sender, EventArgs e)
        {
            
        }

    }
}
