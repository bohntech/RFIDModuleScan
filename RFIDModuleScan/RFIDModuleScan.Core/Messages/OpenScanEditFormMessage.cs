using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDModuleScan.Core.Messages
{
    public class OpenScanEditFormMessage
    {
        public string Grower { get; set;}
        public string Farm { get; set; }
        public string Field { get; set; }
    }
}
