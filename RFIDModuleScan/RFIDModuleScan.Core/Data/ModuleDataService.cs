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

namespace RFIDModuleScan.Core.Data
{
    public class ModuleDataService : IModuleDataService
    {
        private SQLiteConnection database;

        private static object locker = new object();

        public ModuleDataService()
        {
            database = DependencyService.Get<ISqlite>().GetConnection();
            database.CreateTable<AppSetting>();
            database.CreateTable<FieldScan>();
            database.CreateTable<Load>();
            database.CreateTable<ModuleScan>();
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
            return database.Table<TObject>().ToList();
        }

        #endregion

        public void ClearData()
        {
            database.DeleteAll<FieldScan>();
            database.DeleteAll<Load>();
            database.DeleteAll<ModuleScan>();         
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

    }
}
