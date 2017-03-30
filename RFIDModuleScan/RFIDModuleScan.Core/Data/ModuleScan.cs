//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;


namespace RFIDModuleScan.Core.Data
{
    public class ModuleScan : IDBEntity
    {
        [PrimaryKey]
        public Guid ID { get; set; }                
        public Guid FieldScanID { get; set; }
        public Guid LoadID { get; set; }
        public string SerialNumber { get; set; }
        public int BarcodeType { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime TimeStamp { get; set; } 
        public DateTime TransmitTime { get; set; }  
        public bool Transmitted { get; set; }
        public string Note { get; set; }
        
        public ModuleScan()
        {
            ID = Guid.Empty;
            Transmitted = false;
        }

        public string GetTableHeader()
        {
            return "ID,FieldScanID,LoadID,SerialNumber,BarcodeType,Latitude,Longitude,Timestamp,TransmitTime,Transmitted,Note";
        }

        public string GetCSVLine()
        {
            string result = "";
            result += FileHelper.EscapeForCSV(ID.ToString()) + ",";
            result += FileHelper.EscapeForCSV(FieldScanID.ToString()) + ",";
            result += FileHelper.EscapeForCSV(LoadID.ToString()) + ",";
            result += FileHelper.EscapeForCSV(SerialNumber) + ",";
            result += FileHelper.EscapeForCSV(BarcodeType.ToString()) + ",";

            if (Latitude.ToString() == "0")
            {
                result += ",";
            }
            else
            {
                result += FileHelper.EscapeForCSV(Latitude.ToString()) + ",";
            }

            if (Longitude.ToString() == "0")
            {
                result += ",";
            }
            else
            {
                result += FileHelper.EscapeForCSV(Longitude.ToString()) + ",";
            }

            result += FileHelper.EscapeForCSV(TimeStamp.ToString("MM/dd/yyyy hh:mm tt")) + ",";
            result += FileHelper.EscapeForCSV(TransmitTime.ToString("MM/dd/yyyy hh:mm tt")) + ",";
            result += FileHelper.EscapeForCSV(Transmitted.ToString()) + ",";
            result += FileHelper.EscapeForCSV(Note);

            return result;

        }
    }
}
