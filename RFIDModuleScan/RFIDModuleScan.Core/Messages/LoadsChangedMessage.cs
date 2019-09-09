//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDModuleScan.Core.Messages
{
    public class LoadsChangedMessage
    {
        public bool Scroll { get; set; }

        public LoadsChangedMessage()
        {
            Scroll = true;
        }
    }

    public class LoadTagScannedMessage
    {
        public string GinTagLoadNumber { get; set; }
    }
}
