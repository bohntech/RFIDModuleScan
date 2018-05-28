using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RFIDModuleScan.Core.Data;

namespace RFIDModuleScan.Core.Services
{
    public interface ICloudDataService
    {
        void Init(string endPoint, string authKey);
        List<Client> GetAllClients();
        List<Farm> GetAllFarms();
        List<Field> GetAllFields();
    }
}
