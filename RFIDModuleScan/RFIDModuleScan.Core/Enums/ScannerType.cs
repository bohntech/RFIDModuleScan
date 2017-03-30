//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDModuleScan.Core.Enums
{
    public enum ScannerType { TSL=0, OTHER=1}
    public enum ScannerState { Disconnecting=0, Disconnnected=1, Connected=2, Connecting=3}
}
