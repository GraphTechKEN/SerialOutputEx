using System.Management;
using System.Text.RegularExpressions;
using System;
using Microsoft.Win32;
using System.Text;


public class GetDeviceNames
{
    public GetDeviceNames(out string[] devices)
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
            devices = deviceNames;
        }
        else
        {
            devices = null;
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
}