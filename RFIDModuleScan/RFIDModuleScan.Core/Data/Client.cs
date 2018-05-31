//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Newtonsoft.Json;

namespace RFIDModuleScan.Core.Data
{
    public class Client : IListEntity
    {
        [PrimaryKey]
        [JsonProperty(PropertyName = "id")]
        public Guid ID { get; set; }

        [JsonIgnore]
        public Guid? PreviousID { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "entitytype")]
        public string EntityType { get; set; }

        public string Source { get; set; }

        public string GetCSVLine()
        {
            return "";
        }

        public string GetTableHeader()
        {
            return "";
        }

        [Ignore]
        [JsonIgnore]
        public string NormalizedName
        {
            get
            {
                return Name.Trim().ToLower();
            }
        }

        public void CopyValues(object srcObject)
        {
            Client src = (Client)srcObject;
            ID = src.ID;
            Name = src.Name;
        }

        public string CompareKey
        {
            get
            {
                if (Name != null)
                    return Name.Trim().ToLower();
                else
                    return "";
            }
        }
    }
}
