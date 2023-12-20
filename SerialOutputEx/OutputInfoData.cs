using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialOutputEx
{
    public class OutputInfoData
    {
        /// <summary>
        /// ID
        /// </summary>
        public int infoId { get; set; }

        /// <summary>
        /// 乗数
        /// </summary>
        public int Pow { get; set; }

        /// <summary>
        /// 桁数
        /// </summary>
        public int Digit { get; set; }

        /// <summary>
        /// 基数
        /// </summary>
        public string Base { get; set; }

        /// <summary>
        /// 値
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// パネル番号
        /// </summary>
        public int PanelNum { get; set; }

        /// <summary>
        /// サウンド番号
        /// </summary>
        public int SoundNum { get; set; }　//0.2.31217.1 準備
    }
}
