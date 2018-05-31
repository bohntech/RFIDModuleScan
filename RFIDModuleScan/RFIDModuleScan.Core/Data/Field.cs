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
    public class Field : IListEntity
    {
        [PrimaryKey]
        [JsonProperty(PropertyName = "id")]
        public Guid ID { get; set; }

        [JsonIgnore]
        public Guid? PreviousID { get; set; }

        [JsonProperty(PropertyName = "farmid")]
        public string FarmId { get; set; }

        [JsonIgnore]
        [Ignore]
        public Farm Farm { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

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
            Field src = (Field)srcObject;
            ID = src.ID;
            Name = src.Name;
            FarmId = src.FarmId;
        }

        public string CompareKey
        {
            get
            {
                if (Farm == null)
                    return "";
                else
                    return Farm.CompareKey + "_" + Name.Trim().ToLower();
            }
        }
    }
}
