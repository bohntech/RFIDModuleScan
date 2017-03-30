//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RFIDModuleScan.Core.Models;

namespace RFIDModuleScan.Core.Scanners
{
    public class ItemScannedEventArgs : EventArgs
    {
        public ScanEventData EventData { get; set; }

        
    }
}
