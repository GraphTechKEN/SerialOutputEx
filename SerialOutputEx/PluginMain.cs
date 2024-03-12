using System;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

using BveTypes.ClassWrappers;

using AtsEx.PluginHost.Plugins;
using AtsEx.PluginHost.Plugins.Extensions;
using AtsEx.Extensions.ContextMenuHacker;
using AtsEx.PluginHost;

namespace SerialOutputEx
{
    [PluginType(PluginType.Extension)]
    [HideExtensionMain]
    internal class PluginMain : AssemblyPluginBase, IExtension
    {
        private SerialPort serialPort = new SerialPort();
        private bool Debug = false;
        private OutputInfo outputInfo = new OutputInfo();

        private string str_send_latch = "";
        private DateTime dateTime;

        private bool flgScenarioOpened = false;
        private bool flgConsoleOpened = false;
        private bool flgAnotherPortOpen = false;

        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        private ToolStripMenuItem tsiOutput;
        private ToolStripMenuItem tsiConsole;
        private ToolStripMenuItem tsiPorts;
        private ToolStripMenuItem tsiOpenEditor;

        private string portName = "";
        private string editorPath = "";

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

        }

        private void Extensions_AllExtensionsLoaded(object sender, EventArgs e)
        {
            string openedPortName = Open(portName);
            tsiOutput = Extensions.GetExtension<IContextMenuHacker>().AddCheckableMenuItem("SerialOutputEx 連動", SerialOutputEx_Change, ContextMenuItemType.CoreAndExtensions);
            tsiConsole = Extensions.GetExtension<IContextMenuHacker>().AddCheckableMenuItem("デバッグコンソール 表示", DebugConsoleDisp_Change, ContextMenuItemType.CoreAndExtensions);
            tsiOpenEditor = Extensions.GetExtension<IContextMenuHacker>().AddClickableMenuItem("SerialOutputEx 設定", SerialOutputExEdit_Open, ContextMenuItemType.CoreAndExtensions);

            if (Debug)
            {
                tsiConsole.Checked = true;
            }
            if (serialPort.IsOpen)
            {
                tsiOutput.Checked = true;
            }

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
                    portName = mi.Text;
                    flgAnotherPortOpen = true;
                    Open(portName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void SerialOutputEx_Change(object sender, EventArgs e)
        {
            if (tsiOutput.Checked)
            {
                if (!serialPort.IsOpen)
                {
                    Open(portName);
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

        private void DebugConsoleDisp_Change(object sender, EventArgs e)
        {
            if (tsiConsole.Checked)
            {
                Debug = true;
                ConsoleOpen();
            }
            else
            {
                Debug = false;
                ConsoleClose();
            }
        }


        private void BveHacker_ScenarioOpened(ScenarioOpenedEventArgs e)
        {
            flgScenarioOpened = true;
        }


        private void ContextMenu_Opened(object sender, EventArgs e)
        {
            tsiOutput.Enabled = !flgScenarioOpened;
            tsiOutput.Checked = serialPort.IsOpen;

            tsiOutput.DropDownItems.Clear();
            foreach (var _portName in SerialPort.GetPortNames())
            {
                try
                {
                    tsiPorts = new ToolStripMenuItem(_portName);

                    // クリックイベントを追加
                    tsiPorts.Click += TsiPorts_Click;
                    //使用中ポートにチェック、Enableとする
                    if ((_portName == serialPort.PortName) && serialPort.IsOpen)
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

        private void ContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //シナリオを閉じるメニューをクリックした時のイベント
            if (e.ClickedItem.Name == "closeToolStripMenuItem")
            {
                flgScenarioOpened = false;
            }
        }
        private string Open(string portName)
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

            string dir = Path.GetDirectoryName(dllPath);
            string fileName = Path.GetFileNameWithoutExtension(dllPath);　//0.2.31217.1 追記 設定ファイル名をdllと同じとする。
            string path = dir + @"\" + fileName + ".xml";

            try
            {
                if (!File.Exists(path))
                {
                    MessageBox.Show("設定ファイル " + fileName + ".xml が見つかりません");　//0.2.31217.1 変更 設定ファイル名をdllと同じとする。
                }
                else
                {
                    portName = OutputOpen(path, portName);
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

        private void ConsoleClose()
        {
            FreeConsole();
            flgConsoleOpened = false;
        }

        private string OutputOpen(string path , string _portName)
        {
            //XmlSerializerオブジェクトを作成
            XmlSerializer serializer = new XmlSerializer(typeof(OutputInfo));

            //読み込むファイルを開く
            XmlReader reader = XmlReader.Create(path);

            //XMLファイルから読み込み、逆シリアル化する
            outputInfo = (OutputInfo)serializer.Deserialize(reader);
            reader.Close();
            if (!flgAnotherPortOpen || _portName == "")
            {
                serialPort.PortName = "COM" + outputInfo.PortNum.ToString();
            }
            else
            {
                serialPort.PortName = _portName;
            }
            serialPort.BaudRate = outputInfo.BaudRate;
            serialPort.Parity = outputInfo.Parity;
            serialPort.StopBits = outputInfo.StopBits;
            serialPort.DataBits = outputInfo.DataBits;
            serialPort.WriteTimeout = 1000;
            serialPort.ReadTimeout = 1000;
            Debug = outputInfo.Debug;

            //シリアルポートを開く
            serialPort.Open();

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
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }

        public override TickResult Tick(TimeSpan elapsed)
        {
            if (serialPort.IsOpen)
            {
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
            HandleSet handles = BveHacker.Scenario.Vehicle.Instruments.Cab.Handles;
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
                    str = Right(string.Format(format, handles.PowerNotch), _data.Digit);
                    break;

                case 11://レバーサ位置
                    str = handles.ReverserPosition.ToString();
                    break;

                case 12://ドア状態
                    str = BveHacker.Scenario.Vehicle.Doors.AreAllClosingOrClosed ? "0" : "1";
                    break;

                case 13://パネル状態
                    str = Right(ats.PanelArray[_data.PanelNum].ToString(), 1);
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

    }
}
