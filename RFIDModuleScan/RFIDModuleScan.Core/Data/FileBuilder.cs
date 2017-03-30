//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RFIDModuleScan.Core.Data;
using RFIDModuleScan.Core.Enums;
namespace RFIDModuleScan.Core.Data
{
    public class FileHelper
    {
        private const string QUOTE = "\"";
        private const string ESCAPED_QUOTE = "\"\"";
        private static char[] SPECIAL_CHARACTERS = { ',', '"', '\n' };

        public static string EscapeForCSV(string fieldValue)
        {
            if (fieldValue == null) fieldValue = string.Empty;

            if (fieldValue.ToCharArray().Any(c => SPECIAL_CHARACTERS.Contains(c)))
            {
                fieldValue = fieldValue.Replace("\"", "\"\"");
                fieldValue = string.Format("\"{0}\"", fieldValue);
            }
            return fieldValue;
        }

        public static Dictionary<string, string> DumpDBTables(IModuleDataService dataService)
        {
            var appSettings = dataService.GetAll<AppSetting>().ToArray();
            var fieldScans = dataService.GetAll<FieldScan>().ToArray();
            var loads = dataService.GetAll<Load>().ToArray();
            var modules = dataService.GetAll<ModuleScan>().ToArray();

            StringBuilder appSettingsFile = new StringBuilder();
            StringBuilder fieldScansFile = new StringBuilder();
            StringBuilder loadsFile = new StringBuilder();
            StringBuilder modulesFile = new StringBuilder();

            for (int i=0; i < appSettings.Length; i++)
            {
                if (i == 0)
                    appSettingsFile.AppendLine(appSettings[i].GetTableHeader());
                
                appSettingsFile.AppendLine(appSettings[i].GetCSVLine());
            }

            for (int i = 0; i < fieldScans.Length; i++)
            {
                if (i == 0)
                    fieldScansFile.AppendLine(fieldScans[i].GetTableHeader());
                
                fieldScansFile.AppendLine(fieldScans[i].GetCSVLine());
            }

            for (int i = 0; i < loads.Length; i++)
            {
                if (i == 0)
                    loadsFile.AppendLine(loads[i].GetTableHeader());

                loadsFile.AppendLine(loads[i].GetCSVLine());
            }

            for (int i = 0; i < modules.Length; i++)
            {
                if (i == 0)
                    modulesFile.AppendLine(modules[i].GetTableHeader());
                
                modulesFile.AppendLine(modules[i].GetCSVLine());
            }

            Dictionary<string, string> fileDictionary = new Dictionary<string, string>();
            fileDictionary.Add("AppSettings", appSettingsFile.ToString().TrimEnd());
            fileDictionary.Add("FieldScans", fieldScansFile.ToString().TrimEnd());
            fileDictionary.Add("Loads", loadsFile.ToString().TrimEnd());
            fileDictionary.Add("Modules", modulesFile.ToString().TrimEnd());

            return fileDictionary;
        }

        public static string GetCSVFileString(List<FieldScan> scans, IModuleDataService dataService)
        {
            StringBuilder sb = new StringBuilder();

            //write header
            sb.AppendLine("Grower,Farm,Field,SerialNumber,Load,ScanLocation,ScanType,Timestamp,Latitude,Longitude,TabletID,Notes");

            var allModuleScans = dataService.GetAll<ModuleScan>().ToList();
            var allLoads = dataService.GetAll<Load>().ToList();

            foreach (var scan in scans) {

                var loads = allLoads.Where(x => x.FieldScanID == scan.ID).OrderBy(x => x.Created);

                foreach(var load in loads)
                {
                    var moduleScans = allModuleScans.Where(x => x.LoadID == load.ID && x.FieldScanID == scan.ID).OrderBy(x => x.TimeStamp);

                    foreach(var module in moduleScans)
                    {
                        sb.Append(EscapeForCSV(scan.Grower));
                        sb.Append(",");

                        sb.Append(EscapeForCSV(scan.Farm));
                        sb.Append(",");

                        sb.Append(EscapeForCSV(scan.Field));
                        sb.Append(",");

                        sb.Append(EscapeForCSV(module.SerialNumber));
                        sb.Append(",");

                        sb.Append(EscapeForCSV(load.LoadNumber.ToString()));
                        sb.Append(",");

                        sb.Append(EscapeForCSV(scan.ScanLocation));
                        sb.Append(",");

                        if (module.BarcodeType == (int) BarCodeTypeEnum.GenericBarcode)
                        {
                            sb.Append("Barcode");
                        }
                        else if (module.BarcodeType == (int)BarCodeTypeEnum.GenericRFTag)
                        {
                            sb.Append("RFID");
                        }
                        else if (module.BarcodeType == (int)BarCodeTypeEnum.JohnDeereBarCode)
                        {
                            sb.Append("John Deere Barcode");
                        }
                        else if (module.BarcodeType == (int)BarCodeTypeEnum.JohnDeereRFTag)
                        {
                            sb.Append("John Deere RFID");
                        }
                        sb.Append(",");

                        sb.Append(EscapeForCSV(module.TimeStamp.ToString("MM/dd/yyy hh:mm tt")));
                        sb.Append(",");

                        if (module.Latitude.ToString() == "0")
                        {
                            sb.Append(",");
                        }
                        else
                        {
                            sb.Append(EscapeForCSV(module.Latitude.ToString()));
                            sb.Append(",");
                        }

                        if (module.Longitude.ToString() == "0")
                        {
                            sb.Append(",");
                        }
                        else
                        {
                            sb.Append(EscapeForCSV(module.Longitude.ToString()));
                            sb.Append(",");
                        }

                        sb.Append(EscapeForCSV(Configuration.TabletID));
                        sb.Append(",");

                        if (scan.ListTypeID == (int)ListTypeEnum.Staging)
                        {
                            sb.Append(EscapeForCSV(load.Notes));
                        }
                        else
                        {
                            sb.Append(EscapeForCSV(module.Note));
                        }
                        sb.Append("\n");

                        module.Transmitted = true;
                        dataService.Save<ModuleScan>(module);
                    }
                }

                scan.Transmitted = DateTime.Now;
                dataService.Save<FieldScan>(scan);
            }         

            return sb.ToString();
        }
    }
}
