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

        public static string GetHIDFileString(List<FieldScan> scans, IModuleDataService dataService)
        {
            StringBuilder sb = new StringBuilder();

            //write header
            sb.AppendLine("Module ID,Module SN,Lat,Lon,GMT Date,GMT Time,Tag Count,Client,Farm,Field,Variety,Machine PIN,Operator,Gin ID,Producer ID,Local Time,Field Area (Sq m),Season Total,Moisture (%),Diameter (cm),Weight (kg),Drop Lat,Drop Lon,Field Total,Incremental Area (Sq m),Local Date,Comment");

            string templateLine = "{ModuleID},{SerialNumber},{Latitude},{Longitude},{GMTDate},{GMTTime},1,{Client},{Farm},{Field},,{TabletID},{Operator},{GinID},{ProducerID},{LocalTime},,,,,,{DropLat},{DropLon},{FieldTotal},,{LocalDate},{Comments}";

            var allModuleScans = dataService.GetAll<ModuleScan>().ToList();
            var allLoads = dataService.GetAll<Load>().ToList();
                        
            foreach (var scan in scans)
            {
                int scanCount = 1;

                var loads = allLoads.Where(x => x.FieldScanID == scan.ID).OrderBy(x => x.Created);

                foreach (var load in loads)
                {
                    var moduleScans = allModuleScans.Where(x => x.LoadID == load.ID && x.FieldScanID == scan.ID).OrderBy(x => x.TimeStamp);

                    foreach (var module in moduleScans)
                    {
                        var dataString = templateLine;

                        dataString = dataString.Replace("{ModuleID}", module.AdjustedModuleID);
                        dataString = dataString.Replace("{SerialNumber}", EscapeForCSV(module.SerialNumber));
                        dataString = dataString.Replace("{Latitude}", module.Latitude.ToString());
                        dataString = dataString.Replace("{Longitude}", module.Longitude.ToString());
                        dataString = dataString.Replace("{GMTDate}", module.TimeStamp.ToUniversalTime().ToString("yyyy/MM/dd"));
                        dataString = dataString.Replace("{GMTTime}", module.TimeStamp.ToUniversalTime().ToString("HH:mm:ss"));
                        dataString = dataString.Replace("{Client}", EscapeForCSV(scan.Grower));
                        dataString = dataString.Replace("{Farm}", EscapeForCSV(scan.Farm));
                        dataString = dataString.Replace("{Field}", EscapeForCSV(scan.Field));
                        dataString = dataString.Replace("{TabletID}", EscapeForCSV(Configuration.TabletID));
                        dataString = dataString.Replace("{LocalTime}", module.TimeStamp.ToString("HH:mm:ss"));
                        dataString = dataString.Replace("{DropLat}", module.Latitude.ToString());
                        dataString = dataString.Replace("{DropLon}", module.Longitude.ToString());
                        dataString = dataString.Replace("{Operator}", "");
                        dataString = dataString.Replace("{GinID}", "");
                        dataString = dataString.Replace("{ProducerID}", EscapeForCSV(scan.Grower));
                        dataString = dataString.Replace("{FieldTotal}", scanCount.ToString());
                        dataString = dataString.Replace("{Comments}", EscapeForCSV(load.Notes));
                        dataString = dataString.Replace("{LocalDate}", module.TimeStamp.ToString("yyyy/MM/dd"));
                        sb.AppendLine(dataString);
                        scanCount++;
                        module.Transmitted = true;
                        dataService.Save<ModuleScan>(module);
                    }
                }

                scan.Transmitted = DateTime.Now;
                dataService.Save<FieldScan>(scan);
            }

            return sb.ToString();
        }

        public static string GetCSVFileString(List<FieldScan> scans, IModuleDataService dataService)
        {
            StringBuilder sb = new StringBuilder();

            //write header
            sb.AppendLine("Grower,Farm,Field,SerialNumber,ModuleID,Load,ScanLocation,ScanType,Timestamp,Latitude,Longitude,TabletID,Notes");

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

                        sb.Append(EscapeForCSV(module.AdjustedModuleID));
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
