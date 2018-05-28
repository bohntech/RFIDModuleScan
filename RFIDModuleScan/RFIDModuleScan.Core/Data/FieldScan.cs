//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace RFIDModuleScan.Core.Data
{
    public class FieldScan : IDBEntity
    {
        [PrimaryKey]
        public Guid ID { get; set; }
        public string Grower { get; set; }
        public string GrowerID { get; set; }  //same as client id

        public string Farm { get; set; }
        public string FarmID { get; set; }

        public string Field { get; set; }
        public string FieldID { get; set; }

        public int MaxModulesPerLoad { get; set; }
        public int ListTypeID { get; set; }
        public string ScanLocation { get; set; }
        public bool AutoLoadAssign { get; set; }
        public int StartingLoadNumber { get; set; }
        public string Note { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastScan { get; set; }
        public int ModuleCount { get; set; }
        public int LoadCount { get; set; }
        public DateTime? Transmitted { get; set; }

        public FieldScan()
        {
            ID = Guid.Empty;
            Created = DateTime.Now;
        }

        public string GetTableHeader()
        {
            return "ID,Grower,Farm,Field,MaxModulesPerLoad,ListTypeID,ScanLocation,AutoLoadAssign,StartingLoadNumber,Note,Created,LastScan,ModuleCount,LoadCount,Transmitted";
        }

        public string GetCSVLine()
        {
            string result = "";
            result += FileHelper.EscapeForCSV(ID.ToString()) + ",";
            result += FileHelper.EscapeForCSV(Grower) + ",";
            result += FileHelper.EscapeForCSV(Farm) + ",";
            result += FileHelper.EscapeForCSV(Field) + ",";
            result += FileHelper.EscapeForCSV(MaxModulesPerLoad.ToString()) + ",";
            result += FileHelper.EscapeForCSV(ListTypeID.ToString()) + ",";
            result += FileHelper.EscapeForCSV(ScanLocation) + ",";
            result += FileHelper.EscapeForCSV(AutoLoadAssign.ToString()) + ",";
            result += FileHelper.EscapeForCSV(StartingLoadNumber.ToString()) + ",";
            result += FileHelper.EscapeForCSV(Note) + ",";
            result += FileHelper.EscapeForCSV(Created.ToString("MM/dd/yyyy hh:mm tt")) + ",";

            if (LastScan.HasValue)
                result += FileHelper.EscapeForCSV(LastScan.Value.ToString("MM/dd/yyyy hh:mm tt")) + ",";
            else
                result += ",";

            result += FileHelper.EscapeForCSV(ModuleCount.ToString()) + ",";

            result += FileHelper.EscapeForCSV(LoadCount.ToString()) + ",";

            if (Transmitted.HasValue)
            {
                result += FileHelper.EscapeForCSV(Transmitted.Value.ToString("MM/dd/yyyy hh:mm tt")) + "";
            }
            else
            {
                result += "";
            }

            return result;

        }
    }
}
