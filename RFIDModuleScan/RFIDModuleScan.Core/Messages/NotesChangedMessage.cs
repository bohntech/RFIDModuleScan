//Licensed under MIT License see LICENSE.TXT in project root folder
using RFIDModuleScan.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDModuleScan.Core.Messages
{
    public class NotesChangedMessage
    {
        public string Notes { get; set; }
        public Guid ID { get; set; }
    }

    public class GinTicketLoadNumberChangedMessage
    {
        public string GinTicketLoadNumber { get; set; }
        public Guid ID { get; set; }
    }

    public class LoadFocusedMessage
    {        
        public Guid ID { get; set; }
        public LoadViewModel VM { get; set; }
    }

    public class LoadUnFocusedMessage
    {
        public Guid ID { get; set; }
        public LoadViewModel VM { get; set; }
    }
}
