//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFIDModuleScan.Core.Services;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using XLabs.Platform.Services;
using Xamarin.Forms;
using System.IO;
using RFIDModuleScan.Core.Data;
using Microsoft.Azure.Documents.Client;
using RFIDModuleScan.Droid.DocumentDb;

[assembly: Dependency(typeof(RFIDModuleScan.Droid.CloudDataService))]
namespace RFIDModuleScan.Droid
{
    public class CloudDataService : ICloudDataService
    {
        public CloudDataService()
        {
            
        }

        public void Init(string endPoint, string authKey)
        {
            if (!DocumentDBContext.Initialized)
            {
                DocumentDBContext.Initialize(endPoint, authKey);
            }
        }

        public List<Client> GetAllClients()
        {
            return DocumentDBContext.GetAllItems<Client>(p => p.EntityType == "CLIENT").OrderBy(c => c.Name).ToList();
        }

        public List<Farm> GetAllFarms()
        {
            return DocumentDBContext.GetAllItems<Farm>(p => p.EntityType == "FARM").OrderBy(c => c.Name).ToList();
        }

        public List<Field> GetAllFields()
        {
            return DocumentDBContext.GetAllItems<Field>(p => p.EntityType == "FIELD").OrderBy(c => c.Name).ToList();
        }

        public List<ModuleOwnership> GetModuleOwnershipChanges(DateTime lastSyncTime)
        {
            return DocumentDBContext.GetAllItems<ModuleOwnership>(m => m.EntityType == "MODULE_OWNERSHIP" && (m.Created > lastSyncTime ||
            (m.Updated.HasValue && m.Updated > lastSyncTime))).ToList();
        }
    }
}
