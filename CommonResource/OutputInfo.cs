using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace SerialOutputEx
{
    /// <summary>
    /// 設定クラス
    /// </summary>
    [DataContract(Namespace = "")]
    public class OutputInfo
    {
        /// <summary>
        /// ポート番号
        /// </summary>
        [XmlElement(Order = 0)]
        public int PortNum { get; set; }

        /// <summary>
        /// BaudRate
        /// </summary>
        [XmlElement(Order = 1)]
        public int BaudRate { get; set; }

        /// <summary>
        /// BaudRate
        /// </summary>
        [XmlElement(Order = 2)]
        public Parity Parity { get; set; }

        /// <summary>
        /// BaudRate
        /// </summary>
        [XmlElement(Order = 3)]
        public StopBits StopBits { get; set; }

        /// <summary>
        /// BaudRate
        /// </summary>
        [XmlElement(Order = 4)]

        public int DataBits { get; set; }

        /// <summary>
        /// デバッグ
        /// </summary>
        [XmlElement(Order = 5)]

        public bool Debug { get; set; }

        /// <summary>
        /// データリスト
        /// </summary>
        [XmlArray(Order = 6)]

        public List<OutputInfoData> outputInfoDataList = new List<OutputInfoData>();


    }
}
