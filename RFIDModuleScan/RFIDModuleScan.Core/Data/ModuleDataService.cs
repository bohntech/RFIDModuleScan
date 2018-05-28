//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;
using RFIDModuleScan.Core.Models;
using System.Linq.Expressions;
using RFIDModuleScan.Core.Services;

namespace RFIDModuleScan.Core.Data
{
    public class ModuleDataService : IModuleDataService
    {
        private SQLiteConnection database;

        private static object locker = new object();

        private IConnectedService _connectedSvc = null;

        public ModuleDataService()
        {
            database = DependencyService.Get<ISqlite>().GetConnection();
            database.CreateTable<AppSetting>();
            database.CreateTable<FieldScan>();
            database.CreateTable<Load>();
            database.CreateTable<ModuleScan>();

            database.CreateTable<Client>();
            database.CreateTable<Farm>();
            database.CreateTable<Field>();

            /*database.Execute("UPDATE FieldScan Set GrowerID=?, FarmID=?, FieldID=?", null, null, null);
            DeleteSetting(AppSettingID.Version);
            database.DropTable<Client>();
            database.DropTable<Farm>();
            database.DropTable<Field>();*/

            var versionSetting = GetSetting(AppSettingID.Version);

            if (versionSetting == null)
            {
                //build tables from field scans
                buildTablesFromFieldScans();

                SaveSetting(AppSettingID.Version, "2");
            }

            _connectedSvc = Xamarin.Forms.DependencyService.Get<IConnectedService>();
        }

        #region Settings
        public IEnumerable<AppSetting> GetSettings()
        {
            lock (locker)
            {
                return (from i in database.Table<AppSetting>() select i).ToList();
            }
        }

        public AppSetting GetSetting(Guid setting)
        {
            lock (locker)
            {                
                return database.Table<AppSetting>().FirstOrDefault(x => x.ID == setting);
            }
        }      

        public Guid SaveSetting(Guid setting, string val)
        {
            lock (locker)
            {                
                var existingSetting = database.Table<AppSetting>().FirstOrDefault(x => x.ID == setting);
                

                if (existingSetting != null)
                {
                    existingSetting.Value = val;
                    database.Update(existingSetting);
                    return existingSetting.ID;
                }
                else
                {
                    AppSetting newSetting = new AppSetting { ID = setting, Name = "", Value = val };
                    database.Insert(newSetting);
                    return setting;
                }
            }
        }

        public int DeleteSetting(Guid id)
        {
            lock (locker)
            {
                return database.Delete<AppSetting>(id);
            }
        }
        #endregion

        #region Generic Methods
        public TObject GetByID<TObject>(Guid id) where TObject: IDBEntity, new()
        {
            lock (locker)
            {               
                return database.Table<TObject>().FirstOrDefault(x => x.ID == id);
            }
        }

        public int DeleteByID<TObject>(Guid id) where TObject : IDBEntity, new()
        {
            lock (locker)
            {
                return database.Delete<TObject>(id);
            }
        }

        public Guid Save<TObject>(TObject objectToSave) where TObject : IDBEntity, new()
        {
            lock (locker)
            {
                if (objectToSave.ID == Guid.Empty)
                {
                    objectToSave.ID = Guid.NewGuid();
                    database.Insert(objectToSave);
                    return objectToSave.ID;
                }
                else
                {
                    database.Update(objectToSave);
                    return objectToSave.ID;
                }
            }
        }

        public int InsertAll<TObject>(IEnumerable<TObject> objectsToAdd) where TObject : IDBEntity, new()
        {
            lock (locker)
            {
                if (objectsToAdd.Count() > 0)
                {
                    return database.InsertAll(objectsToAdd);
                }
                else
                {
                    return 0;
                }
            }
        }


        public IList<TObject> Find<TObject, U>(Expression<Func<TObject, bool>> expression, Expression<Func<TObject, U>> sortExpr=null) where TObject : IDBEntity, new()
        {
            lock (locker)
            {
                if (sortExpr != null)
                    return database.Table<TObject>().Where(expression).OrderBy(sortExpr).ToList();
                else
                {
                    return database.Table<TObject>().Where(expression).OrderBy(x => x.ID).ToList();
                }
            }            
        }

        public IList<TObject> GetAll<TObject>() where TObject : IDBEntity, new()
        {
            lock (locker)
            {
                return database.Table<TObject>().ToList();
            }
        }

        #endregion

        private void buildTablesFromFieldScans()
        {
            var fieldScans = GetAll<FieldScan>().ToList();

            List<Client> clients = new List<Client>();
            List<Farm> farms = new List<Farm>();
            List<Field> fields = new List<Field>();

            //build client list
            foreach(var scan in fieldScans)
            {
                if (!clients.Any(c => c.NormalizedName == scan.Grower.Trim().ToLower()))
                {
                    Client c = new Client();
                    c.Name = scan.Grower.Trim();
                    c.Source = "Local";
                    c.EntityType = "CLIENT";
                    Save<Client>(c);
                    clients.Add(c);                    
                }                
            }

            //build farm list
            foreach(var scan in fieldScans)
            {
                var client = clients.SingleOrDefault(c => c.NormalizedName == scan.Grower.Trim().ToLower());
                
                if (!farms.Any(f => f.NormalizedName == scan.Farm.Trim().ToLower() && f.ClientId == client.ID.ToString()))
                {
                    Farm farm = new Farm();
                    farm.Name = scan.Farm.Trim();
                    farm.Source = "Local";
                    farm.EntityType = "FARM";
                    farm.ClientId = client.ID.ToString();
                    Save<Farm>(farm);
                    farms.Add(farm);
                }
            }

            //build field list
            foreach (var scan in fieldScans)
            {
                var client = clients.SingleOrDefault(c => c.NormalizedName == scan.Grower.Trim().ToLower());
                var farm = farms.SingleOrDefault(f => f.NormalizedName == scan.Farm.Trim().ToLower() && f.ClientId == client.ID.ToString());

                if (!fields.Any(f => f.NormalizedName == scan.Field.Trim().ToLower() && f.FarmId == farm.ID.ToString()))
                {
                    Field field = new Field();
                    field.Name = scan.Field.Trim();
                    field.Source = "Local";
                    field.EntityType = "FIELD";
                    field.FarmId = farm.ID.ToString();
                    Save<Field>(field);
                    fields.Add(field);
                }

                //ADD IDS FOR EVERY SCAN
                var dbField = fields.FirstOrDefault(f => f.NormalizedName == scan.Field.Trim().ToLower() && f.FarmId == farm.ID.ToString());
                if (dbField != null)
                {
                    scan.GrowerID = client.ID.ToString();
                    scan.FarmID = farm.ID.ToString();
                    scan.FieldID = dbField.ID.ToString();
                    Save<FieldScan>(scan);
                }
            }            
        }

        private List<TObject> MergeLists<TObject>(List<TObject> remoteList, List<TObject> localList, List<TObject> itemsRemoved) where TObject : IListEntity
        {
            //make a first pass and update names of entities with matching ids - otherwise items get deleted when a name is changed
            foreach(var remoteItem in remoteList)
            {                
                    foreach (var localItem in localList.Where(x => x.ID == remoteItem.ID))
                    {
                        localItem.Source = "Cloud";
                        localItem.Name = remoteItem.Name;
                    }               
            }

            foreach (var remoteItem in remoteList)
            {
                //update local items that are also in cloud
                if (localList.Any(x => x.CompareKey == remoteItem.CompareKey))
                {
                    foreach (var localItem in localList)
                    {
                        if (localItem.CompareKey == remoteItem.CompareKey)
                        {
                            localItem.Source = "Cloud";
                            localItem.PreviousID = localItem.ID;  //save a copy of the old id this helps us update names on field scans
                            localItem.ID = remoteItem.ID;
                        }
                    }
                }
                else  //remote item isn't in local list so add
                {
                    var newLocalItem = remoteItem;
                    newLocalItem.Source = "Cloud";
                    localList.Add(newLocalItem);
                }
            }

            //remove local items with Source cloud that no longer exist in remoteList            
            var updatedList = new List<TObject>();
            foreach (var localItem in localList)
            {
                if (localItem.Source == "Cloud" && !remoteList.Any(x => x.CompareKey == localItem.CompareKey)) {
                    //do nothing - skip to item isn't added
                    itemsRemoved.Add(localItem);
                }
                else
                {
                    updatedList.Add(localItem);
                }
            }
            return updatedList;
        }

        private void populateFarmClients(List<Client> clients, List<Farm> farms)
        {
            foreach(var farm in farms)
            {
                var client = clients.SingleOrDefault(x => x.ID.ToString() == farm.ClientId);

                if (client != null)
                {
                    farm.Client = new Client();
                    farm.Client.Source = client.Source;
                    farm.Client.Name = client.Name;
                    farm.Client.ID = client.ID;                    
                }
            }
        }

        private void populateFieldFarms(List<Farm> farms, List<Field> fields)
        {
            foreach (var field in fields)
            {
                field.Farm = farms.SingleOrDefault(x => x.ID.ToString() == field.FarmId);                
            }
        }

        private void insertFieldsAndParentsLocal(List<Field> updatedFieldList)
        {
            var newClients = new List<Client>();
            var newFarms = new List<Farm>();
            var newFields = new List<Field>();

            foreach (var f in updatedFieldList)
            {
                if (f.Farm != null && f.Farm.Client != null && !newClients.Any(x => x.CompareKey == f.Farm.Client.CompareKey))
                    newClients.Add(f.Farm.Client);

                if (f.Farm != null && !newFarms.Any(x => x.CompareKey == f.Farm.CompareKey))
                {
                    newFarms.Add(f.Farm);
                }

                if (!newFields.Any(x => x.CompareKey == f.CompareKey))
                {
                    newFields.Add(f);
                }
            }

            InsertAll<Client>(newClients);
            InsertAll<Farm>(newFarms);
            InsertAll<Field>(newFields);
        }

        private void insertFarmsAndParentsLocal(List<Farm> updatedFarmList)
        {
            var existingClients = GetAll<Client>().ToList();
            var existingFarms = GetAll<Farm>().ToList();
            populateFarmClients(existingClients, existingFarms);

            var newClients = new List<Client>();
            var newFarms = new List<Farm>();

            foreach (var f in updatedFarmList)
            {
                if (f.Client != null && !existingClients.Any(x => x.CompareKey == f.Client.CompareKey))
                {
                    existingClients.Add(f.Client);
                    newClients.Add(f.Client);
                }

                if (!existingFarms.Any(x => x.CompareKey == f.CompareKey))
                {
                    existingFarms.Add(f);
                    newFarms.Add(f);
                }
            }

            InsertAll<Client>(newClients);
            InsertAll<Farm>(newFarms);         
        }

        private void insertClientsLocal(List<Client> updatedClientList)
        {
            var existingClients = GetAll<Client>().ToList();
            var newClients = new List<Client>();
            
            foreach (var c in updatedClientList)
            {
                if (!existingClients.Any(x => x.CompareKey == c.CompareKey))
                {
                    existingClients.Add(c);
                    newClients.Add(c);
                }                
            }
            InsertAll<Client>(newClients);            
        }

      

        private void updateFieldScans()
        {
            var growers = GetAll<Client>().ToList();
            var farms = GetAll<Farm>().ToList();
            var fields = GetAll<Field>().ToList();
            var fieldScans = GetAll<FieldScan>().ToList();

            var scanIDsUpdated = new List<Guid>();

            foreach (var item in growers)
            {
                foreach (var scan in fieldScans.Where(s => s.GrowerID == item.ID.ToString() || (item.PreviousID.HasValue && s.GrowerID == item.PreviousID.Value.ToString())))
                {
                    scan.Grower = item.Name.Trim();
                    scanIDsUpdated.Add(scan.ID);
                }
            }

            foreach (var item in farms)
            {
                foreach (var scan in fieldScans.Where(s => s.FarmID == item.ID.ToString() || (item.PreviousID.HasValue && s.FarmID == item.PreviousID.Value.ToString())))
                {
                    scan.Farm = item.Name.Trim();
                    scanIDsUpdated.Add(scan.ID);
                }
            }

            foreach (var item in fields)
            {
                foreach (var scan in fieldScans.Where(s => s.FieldID == item.ID.ToString() || (item.PreviousID.HasValue && s.FieldID == item.PreviousID.Value.ToString())))
                {
                    scan.Field = item.Name.Trim();
                    scanIDsUpdated.Add(scan.ID);
                }
            }           

            foreach(var scan in fieldScans.Where(i => scanIDsUpdated.Contains(i.ID)))
            {
                Save<FieldScan>(scan);
            }
        }

        public void GetLocalLists(List<Client> clients, List<Farm> farms, List<Field> fields)
        {
            var localClients = GetAll<Client>().OrderBy(c => c.Name).ToList();
            var localFarms   = GetAll<Farm>().OrderBy(c => c.Name).ToList();
            var localFields  = GetAll<Field>().OrderBy(c => c.Name).ToList();

            populateFarmClients(localClients, localFarms);
            populateFieldFarms(localFarms, localFields);

            clients.Clear();
            clients.AddRange(localClients);
            farms.Clear();
            farms.AddRange(localFarms);
            fields.Clear();
            fields.AddRange(localFields);
        }              

        public void SyncRemoteLists()
        {
            try
            {
                //sync if connected and has endpoint and key
                if (Plugin.Connectivity.CrossConnectivity.Current.IsConnected)
                {

                    var endPointSetting = this.GetSetting(AppSettingID.GinDBUrl);
                    var keySetting = this.GetSetting(AppSettingID.GinDBKey);

                    if (endPointSetting != null && keySetting != null &&
                        !string.IsNullOrWhiteSpace(endPointSetting.Value) &&
                        !string.IsNullOrWhiteSpace(keySetting.Value))
                    {
                        ICloudDataService cloudSvc = Xamarin.Forms.DependencyService.Get<ICloudDataService>();
                        cloudSvc.Init(endPointSetting.Value, keySetting.Value);

                        //get lists from cloud
                        var remoteClients = cloudSvc.GetAllClients();
                        var remoteFarms = cloudSvc.GetAllFarms();
                        var remoteFields = cloudSvc.GetAllFields();
                        foreach (var c in remoteClients) c.Source = "Cloud";
                        foreach (var f in remoteFarms) f.Source = "Cloud";
                        foreach (var f in remoteFields) f.Source = "Cloud";

                        populateFarmClients(remoteClients, remoteFarms);
                        populateFieldFarms(remoteFarms, remoteFields);

                        var localClients = GetAll<Client>().ToList();
                        var localFarms = GetAll<Farm>().ToList();
                        var localFields = GetAll<Field>().ToList();

                        foreach (var c in localClients) c.Source = "Local";
                        foreach (var f in localFarms) f.Source = "Local";
                        foreach (var f in localFields) f.Source = "Local";

                        populateFarmClients(localClients, localFarms);
                        populateFieldFarms(localFarms, localFields);

                        var fieldsRemoved = new List<Field>();
                        var farmsRemoved = new List<Farm>();
                        var clientsRemoved = new List<Client>();

                        var updatedFieldList = MergeLists<Field>(remoteFields, localFields, fieldsRemoved);

                        //clear local lists
                        lock (locker)
                        {
                            database.Execute("DELETE FROM Client");
                            database.Execute("DELETE FROM Farm");
                            database.Execute("DELETE FROM Field");
                        }

                        insertFieldsAndParentsLocal(updatedFieldList);

                        var updatedFarmList = MergeLists<Farm>(remoteFarms, localFarms, farmsRemoved);
                        insertFarmsAndParentsLocal(updatedFarmList);

                        var updatedClientList = MergeLists<Client>(remoteClients, localClients, clientsRemoved);
                        insertClientsLocal(updatedClientList);

                        //refetch lists and update scans
                        updateFieldScans();
                    }
                }
            }
            catch (Exception exc)
            {
                string msg = exc.Message;
            }
        }

        public void ClearData()
        {
            database.DeleteAll<FieldScan>();
            database.DeleteAll<Load>();
            database.DeleteAll<ModuleScan>();
            database.DeleteAll<Client>();
            database.DeleteAll<Farm>();
            database.DeleteAll<Field>();         
        }

        public void CleanUpLists()
        {
            var fieldScans = GetAll<FieldScan>().ToList();
            var clients = GetAll<Client>().ToList();
            var farms = GetAll<Farm>().ToList();
            var fields = GetAll<Field>().ToList();

            foreach(var field in fields)
            {
                if (!fieldScans.Any(s => s.FieldID == field.ID.ToString()))
                {
                    database.Delete<Field>(field.ID);
                }
            }

            foreach(var farm in farms)
            {
                if (!fieldScans.Any(s => s.FarmID == farm.ID.ToString()))
                {
                    database.Delete<Farm>(farm.ID);
                }
            }

            foreach(var client in clients)
            {
                if (!fieldScans.Any(s => s.GrowerID == client.ID.ToString()))
                {
                    database.Delete<Client>(client.ID);
                }
            }
        }

        public void DeleteScanAndRelatedData(Guid fieldScanID)
        {           
            database.Execute("DELETE FROM Load WHERE FieldScanID=?", fieldScanID);
            database.Execute("DELETE FROM ModuleScan WHERE FieldScanID=?", fieldScanID);         
            database.Delete<FieldScan>(fieldScanID);
        }

        public void UpdateLoadNumber(Guid LoadID, int LoadNumber)
        {
            database.Execute("UPDATE Load SET LoadNumber=? WHERE ID=?", LoadNumber, LoadID);
        }


        public bool FarmNameExists(string clientName, string farmName)
        {
            var clients = GetAll<Client>().Where(i => i.Name.Trim().ToLower() == clientName.Trim().ToLower()).ToList();
            var clientIds = clients.Select(c => c.ID.ToString()).ToList();

            var farms = GetAll<Farm>().Where(f => clientIds.Contains(f.ClientId) && f.Name.ToLower().Trim() == farmName.ToLower().Trim()).ToList();

            return farms.Count() > 0;
        }

        public bool ClientNameExists(string clientName)
        {
            var clients = GetAll<Client>().Where(i => i.Name.Trim().ToLower() == clientName.Trim().ToLower()).ToList();
            return clients.Count() > 0;
        }

        public bool FieldNameExists(string clientName, string farmName, string fieldName)
        {
            var clients = GetAll<Client>().Where(i => i.Name.Trim().ToLower() == clientName.Trim().ToLower()).ToList();
            var clientIds = clients.Select(c => c.ID.ToString()).ToList();

            var farms = GetAll<Farm>().Where(f => clientIds.Contains(f.ClientId) && f.Name.ToLower().Trim() == farmName.ToLower().Trim()).ToList();
            var farmIds = farms.Select(f => f.ID.ToString()).ToList();

            var fields = GetAll<Field>().Where(f => farmIds.Contains(f.FarmId) && f.Name.ToLower().Trim() == fieldName.ToLower().Trim()).ToList();

            return fields.Count() > 0;
        }

        public void AddClient(string clientName)
        {
            Client c = new Client();
            c.Name = clientName;
            c.Source = "Local";
            c.EntityType = "CLIENT";
            Save<Client>(c);            
        }

        public void AddFarm(string clientName, string farmName)
        {
            var client = GetAll<Client>().Where(i => i.Name.Trim().ToLower() == clientName.Trim().ToLower()).FirstOrDefault();

            if (client != null)
            {
                Farm f = new Farm();
                f.Name = farmName;
                f.ClientId = client.ID.ToString();
                f.Source = "Local";
                f.EntityType = "FARM";
                Save<Farm>(f);
            }
        }

        public void AddField(string clientName, string farmName, string fieldName)
        {
            var client = GetAll<Client>().Where(i => i.Name.Trim().ToLower() == clientName.Trim().ToLower()).FirstOrDefault();

            if (client != null)
            {
                var farm = GetAll<Farm>().Where(f => client.ID.ToString() == f.ClientId && f.Name.ToLower().Trim() == farmName.ToLower().Trim()).FirstOrDefault();
                Field field = new Field();
                field.Name = farmName;
                field.FarmId = farm.ID.ToString();
                field.Source = "Local";
                field.EntityType = "FIELD";
                Save<Field>(field);
            }
        }
    }
}
