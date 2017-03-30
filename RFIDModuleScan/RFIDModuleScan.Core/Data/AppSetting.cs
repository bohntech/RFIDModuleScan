//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace RFIDModuleScan.Core.Data
{
    public class AppSettingID
    {
        public static Guid MaxModulesPerLoad = Guid.Parse("00000000-0000-0000-0000-000000000001");
        public static Guid TabletID = Guid.Parse("00000000-0000-0000-0000-000000000002");
        public static Guid LastScannerName = Guid.Parse("00000000-0000-0000-0000-000000000003");
        public static Guid LastScannerID = Guid.Parse("00000000-0000-0000-0000-000000000004");
    }

    public class AppSetting : IDBEntity
    {        

        [PrimaryKey]
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public string GetTableHeader()
        {
            return "ID,Name,Value";
        }

        public string GetCSVLine()
        {
            return string.Format("{0},{1},{2}", ID, FileHelper.EscapeForCSV(Name), FileHelper.EscapeForCSV(Value));
        }
    }
}
