using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accenture.SerialPort
{
    class AsEquipData
    {
        /// <summary>
        /// 是否补发
        /// </summary>
        public int isRpm { get; set; }

        /// <summary>
        /// 检测类型
        /// </summary>
        public int testType { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public string macid { get; set; }

        /// <summary>
        /// 仓位编码
        /// </summary>
        public string WPCode { get; set; }

        /// <summary>
        /// 温度值
        /// </summary>
        public string AirTemp { get; set; }

        /// <summary>
        /// 湿度值
        /// </summary>
        public string AirHum { get; set; }

        /// <summary>
        /// 设备电量
        /// </summary>
        public string power { get; set; }

        /// <summary>
        /// 采集评率
        /// </summary>
        public string freq { get; set; }

        /// <summary>
        /// 采集时间
        /// </summary>
        public DateTime collectionTime { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public int dataType { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public int deviceType { get; set; }
    }
}
