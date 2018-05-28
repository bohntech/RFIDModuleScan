//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace RFIDModuleScan.Core.Data
{
    public interface IModuleDataService
    {
        IEnumerable<AppSetting> GetSettings();
        AppSetting GetSetting(Guid setting);
        Guid SaveSetting(Guid setting, string val);
        int DeleteSetting(Guid id);

        TObject GetByID<TObject>(Guid id) where TObject : IDBEntity, new();
        int DeleteByID<TObject>(Guid id) where TObject : IDBEntity, new();
        Guid Save<TObject>(TObject objectToSave) where TObject : IDBEntity, new();

        IList<TObject> Find<TObject, U>(Expression<Func<TObject, bool>> expression, Expression<Func<TObject, U>> sortExpr = null)
            where TObject : IDBEntity, new();

        int InsertAll<TObject>(IEnumerable<TObject> objectsToAdd) where TObject : IDBEntity, new();

        IList<TObject> GetAll<TObject>() where TObject : IDBEntity, new();

        void ClearData();
        void DeleteScanAndRelatedData(Guid fieldScanID);
        void UpdateLoadNumber(Guid LoadID, int LoadNumber);
        void GetLocalLists(List<Client> clients, List<Farm> farms, List<Field> fields);

        bool ClientNameExists(string clientName);
        bool FarmNameExists(string clientName, string farmName);        
        bool FieldNameExists(string clientName, string farmName, string fieldName);

        void AddClient(string clientName);
        void AddFarm(string clientName, string farmName);
        void AddField(string clientName, string farmName, string fieldName);

        void SyncRemoteLists();
        void CleanUpLists();
    }
}
