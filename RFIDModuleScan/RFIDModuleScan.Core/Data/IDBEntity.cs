//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDModuleScan.Core.Data
{
    public interface IDBEntity
    {
        Guid ID { get; set; }

        string GetTableHeader();
        string GetCSVLine();
    }

    public interface IListEntity : IDBEntity
    {
        string Name { get; set; }
        string Source { get; set; }        
        string EntityType { get; set; }

        Guid? PreviousID { get; set; }

        string CompareKey { get; }
    }
}
