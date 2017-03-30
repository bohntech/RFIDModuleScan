//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDModuleScan.Core.Scanners
{
    public class ScanEventData
    {
        public string RawData { get; set; }
        public bool MoreLines { get; set; }
        public bool IsBarcode { get; set; }
        public bool IsFlush { get; set; }

        private string ExtractJohnDeereSN(string hex)
        {
            //extract serial number
            string serialHex = hex.Substring(15);
            return Convert.ToInt64(serialHex, 16).ToString();
        }

        private bool IsJohnDeereEncoding(string hex)
        {
            return hex.StartsWith("3500B9880611");
        }

        public string SerialNumber
        {
            get
            {
                if (IsJohnDeereEncoding(RawData))
                {
                    return ExtractJohnDeereSN(RawData);
                }
                else return (RawData);
            }
        }

        public bool IsJohnDeereTag { get { return IsJohnDeereEncoding(RawData); } }
                
    }
}
