//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace RFIDModuleScan.Core.Data
{
    public class Load : IDBEntity
    {
        [PrimaryKey]
        public Guid ID { get; set; }
        public Guid FieldScanID { get; set; }
        public int LoadNumber { get; set; }
        public string Notes { get; set; }
        public DateTime Created { get; set; }

        public Load()
        {
            ID = Guid.Empty;
            Created = DateTime.Now;
        }

        public string GetTableHeader()
        {
            return "ID,FieldScanID,LoadNumber,Notes,Created";
        }

        public string GetCSVLine()
        {
            string result = "";
            result += FileHelper.EscapeForCSV(ID.ToString()) + ",";
            result += FileHelper.EscapeForCSV(FieldScanID.ToString()) + ",";
            result += LoadNumber.ToString() + ",";
            result += Notes + ",";
            result += FileHelper.EscapeForCSV(Created.ToString("MM/dd/yyyy hh:mm tt"));

            return result;

        }
    }
}
