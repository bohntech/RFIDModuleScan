//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RFIDModuleScan.Core.Data;

namespace RFIDModuleScan.Core
{
    public static class Configuration
    {
        public static int MaxModulesPerLoad
        {
            get
            {
                ModuleDataService db = new ModuleDataService();
                var setting = db.GetSetting(AppSettingID.MaxModulesPerLoad);
                var result = 4;
                if (setting == null)
                {
                    db.SaveSetting(AppSettingID.MaxModulesPerLoad, result.ToString());
                }
                else
                {
                    result = int.Parse(setting.Value);
                }
                return result;
            }
        }

        public static string TabletID
        {
            get
            {
                ModuleDataService db = new ModuleDataService();
                var setting = db.GetSetting(AppSettingID.TabletID);
                var result = "tablet-1";
                if (setting == null)
                {
                    db.SaveSetting(AppSettingID.TabletID, result);
                }
                else
                {
                    result = setting.Value;
                }
                return result;
            }
        }

        public static string LastScannerDisplayName
        {
            get
            {
                ModuleDataService db = new ModuleDataService();
                var setting = db.GetSetting(AppSettingID.LastScannerName);
             
                if (setting == null)
                {
                    return null;
                }
                else
                {
                    return setting.Value;
                }                
            }
        }

        public static string LastScannerID
        {
            get
            {
                ModuleDataService db = new ModuleDataService();
                var setting = db.GetSetting(AppSettingID.LastScannerID);

                if (setting == null)
                {
                    return null;
                }
                else
                {
                    return setting.Value;
                }
            }
        }
    }
}
