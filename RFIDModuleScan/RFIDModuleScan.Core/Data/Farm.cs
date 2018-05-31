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
    public class Farm : IListEntity
    {
        [PrimaryKey]
        [JsonProperty(PropertyName = "id")]
        public Guid ID { get; set; }

        [JsonIgnore]
        public Guid? PreviousID { get; set; }

        [JsonProperty(PropertyName = "clientid")]
        public string ClientId { get; set; }

        [JsonIgnore]
        [Ignore]
        public Client Client { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonIgnore]
        public string Source { get; set; }

        [JsonProperty(PropertyName = "entitytype")]
        public string EntityType { get; set; }

        [Ignore]
        [JsonIgnore]
        public string NormalizedName
        {
            get
            {
                return Name.Trim().ToLower();
            }
        }

        public string GetCSVLine()
        {
            return "";
        }

        public string GetTableHeader()
        {
            return "";
        }

        public void CopyValues(object srcObject)
        {
            Farm src = (Farm)srcObject;
            ID = src.ID;
            Name = src.Name;
            ClientId = src.ClientId;
        }

        public string CompareKey
        {
            get
            {
                if (Client == null)
                {
                    return "";
                }
                else
                    return Client.CompareKey + "_" + Name.Trim().ToLower();
            }
        }
    }
}
