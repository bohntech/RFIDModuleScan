//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RFIDModuleScan.Core.Models;
using RFIDModuleScan.Core.Enums;

namespace RFIDModuleScan.Core.Scanners
{
    public class ConnectionStateEventArgs : EventArgs
    {
        public ScannerState State { get; set; }
    }
}
