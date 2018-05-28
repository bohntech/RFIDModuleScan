using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace RFIDModuleScan.Core.Models
{
    [DataContract]
    public class GinConnectionModel
    {
        [DataMember(Name ="url")]
        public string Url { get; set; }

        [DataMember(Name = "key")]
        public string Key { get; set; }

        [DataMember(Name = "gin")]
        public string Gin { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }
    }
}
