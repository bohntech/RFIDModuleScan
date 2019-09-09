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
    public class ModuleOwnership 
    {
        [PrimaryKey]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "entitytype")]
        public string EntityType { get; set; }

        public string Source { get; set; }

        [JsonProperty(PropertyName = "ginTagLoadNumber")]
        public string GinTagLoadNumber
        {
            get; set;
        }

        [JsonProperty(PropertyName = "bridgeLoadNumber")]
        public int BridgeLoadNumber
        {
            get; set;
        }

        [JsonProperty(PropertyName = "importedLoadNumber")]
        public string ImportedLoadNumber
        {
            get; set;
        }

        [JsonProperty(PropertyName = "truckLoadNumber")]
        public string TruckLoadNumber
        {
            get; set;
        }

        [JsonProperty(PropertyName = "status")]
        public string Status
        {
            get; set;
        }

        [JsonProperty(PropertyName = "client")]
        public string Client
        {
            get; set;
        }

        [JsonProperty(PropertyName = "farm")]
        public string Farm
        {
            get; set;
        }

        [JsonProperty(PropertyName = "field")]
        public string Field
        {
            get; set;
        }

        [JsonProperty(PropertyName = "variety")]
        public string Variety
        {
            get; set;
        }

        [JsonProperty(PropertyName = "pickedby")]
        public string PickedBy
        {
            get; set;
        }

        [JsonProperty(PropertyName = "location")]
        public string Location
        {
            get; set;
        }

        [JsonProperty(PropertyName = "TrailerNumber")]
        public string TrailerNumber
        {
            get; set;
        }

        [JsonProperty(PropertyName = "truckId")]
        public string TruckID
        {
            get; set;
        }

        [JsonProperty(PropertyName = "grossWeight")]
        public decimal LoadGrossWeight { get; set; }

        [JsonProperty(PropertyName = "netWeight")]
        public decimal LoadNetWeight { get; set; }

        [JsonProperty(PropertyName = "splitWeight1")]
        public decimal? LoadSplitWeight1 { get; set; }

        [JsonProperty(PropertyName = "splitWeight2")]
        public decimal? LoadSplitWeight2 { get; set; }

        [JsonProperty(PropertyName = "deleted")]
        public bool Deleted
        {
            get; set;
        }

        [JsonProperty(PropertyName = "created")]
        public DateTime Created
        {
            get; set;
        }

        [JsonProperty(PropertyName = "updated")]
        public DateTime? Updated
        {
            get; set;
        }

        public DateTime LastCreatedOrUpdated
        {
            get
            {
                if (Updated.HasValue) return Updated.Value;
                else return Created;
            }
        }

        public ModuleOwnership()
        {
            ID = string.Empty;            
        }
    
    }
}
