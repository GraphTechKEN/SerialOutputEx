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

namespace SerialOutputEx
{
    [PluginType(PluginType.Extension)]
    [HideExtensionMain]
    internal class PluginMain : AssemblyPluginBase, IExtension
    {
        private readonly SerialPort serialPort = new SerialPort();
        private readonly bool Debug = false;
        private readonly OutputInfo outputInfo = new OutputInfo();

        private string str_send_latch = "";
        private DateTime dateTime;


        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        public PluginMain(PluginBuilder builder) : base(builder)
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
            string dirParent = Directory.GetParent(dir).FullName;
            string fileName = Path.GetFileNameWithoutExtension(dllPath);　//0.2.31217.1 追記 設定ファイル名をdllと同じとする。
            string path = dir + @"\" + fileName + ".xml";
            string pathParent = dirParent + @"\" + fileName + ".xml";

            try
            {
                if (!File.Exists(path))
                {
                    path = pathParent;
                    if(!File.Exists(path))
                    {
                        MessageBox.Show("設定ファイル " + fileName + ".xml が見つかりません");　//0.2.31217.1 変更 設定ファイル名をdllと同じとする。
                        Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Dispose();
            }

            //XmlSerializerオブジェクトを作成
            XmlSerializer serializer = new XmlSerializer(typeof(OutputInfo));

            //読み込むファイルを開く
            XmlReader reader = XmlReader.Create(path);

            //XMLファイルから読み込み、逆シリアル化する
            outputInfo = (OutputInfo)serializer.Deserialize(reader);
            reader.Close();

            serialPort.PortName = "COM" + outputInfo.PortNum.ToString();
            serialPort.BaudRate = outputInfo.BaudRate;
            serialPort.Parity = outputInfo.Parity;
            serialPort.StopBits = outputInfo.StopBits;
            serialPort.DataBits = outputInfo.DataBits;
            serialPort.WriteTimeout = 1000;
            serialPort.ReadTimeout = 1000;
            Debug = outputInfo.Debug;

            try
            {
                //シリアルポートを開く
                serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Dispose();
            }
            if (Debug)
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
            }
            dateTime = DateTime.Now;
        }

        public override void Dispose()
        {
            //AtsEXを選択解除したときに
            //コンソールを非表示とする //0.2.31217.1 追記
            if (Debug)
            {
                FreeConsole();
            } 
            //シリアルポートを閉じる
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }

        public override TickResult Tick(TimeSpan elapsed)
        {
            //If you want to change the Handle state, please access to Ats.Handle
            //送信コマンド用
            string str_send = "";
            //コマンド解析用
            string str = "";
            //コマンド連結
            for (int i = 0; i < outputInfo.outputInfoDataList.Count; i++)
            {
                str += SendCommandGenerator(outputInfo.outputInfoDataList[i]);
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
                    str = Right(string.Format(format, (int)(Math.Abs(BveHacker.Scenario.LocationManager.SpeedMeterPerSecond * 3.6) * Math.Pow(10, _data.Pow))), _data.Digit);
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
