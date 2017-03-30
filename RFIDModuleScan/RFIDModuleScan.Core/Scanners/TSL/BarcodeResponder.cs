//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RFIDModuleScan.Core.Models;
using TechnologySolutions.Rfid.AsciiProtocol;
using TechnologySolutions.Rfid.AsciiProtocol.Extensions;
using System.IO;

namespace RFIDModuleScan.Core.Scanners
{
   

    public class ScanResponder : IAsciiCommandResponder
    {
        Action<ScanEventData> _callback = null;

        private static string HexStringToCharacterArray(string hex)
        {
            byte[] bytes = Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                StreamReader sr = new StreamReader(ms, Encoding.UTF8);
                return sr.ReadToEnd();
            }
        }

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

        public ScanResponder(Action<ScanEventData> callback)
        {
            _callback = callback;
        }

        public bool ProcessReceivedLine(IAsciiResponseLine line, bool moreLinesAvailable)
        {            
            if (line.Header.ToUpper() == "EP" || line.Header.ToUpper() == "BC")
            {
                bool isBarcodeRead = (line.Header.ToUpper() == "BC");
                _callback(new ScanEventData { IsBarcode= isBarcodeRead, RawData = line.Value, MoreLines = true, IsFlush=false });
            }
            else if (line.Header.ToUpper().StartsWith("OK"))
            {
                _callback(new ScanEventData { IsBarcode = false, RawData = "", MoreLines = false, IsFlush=true });
            }
            return true;
        }
    }
}
