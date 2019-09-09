//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using RFIDModuleScan.Core.Enums;

namespace RFIDModuleScan.Core.ViewModels
{
    public class ExportFormatModel : ObservableObject
    {
        public ExportFormatEnum ExportType { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ScanSettingsViewModel : ObservableObject
    {

        public bool IsDirty { get; set; }

        private string _tabletID = "";
        public string TabletID
        {
            get
            {
                return _tabletID;
            }
            set
            {
                if (Set<string>(() => TabletID, ref _tabletID, value))
                {
                    IsDirty = true;
                }
            }
        }

        private string _maxModulesPerLoad = "4";
        public string MaxModulesPerLoad
        {
            get
            {
                return _maxModulesPerLoad;
            }
            set
            {
                if (Set<string>(() => MaxModulesPerLoad, ref _maxModulesPerLoad, value))
                {
                    IsDirty = true;
                }
            }
        }

        private string _loadTagPrefix = "B";
        public string LoadTagPrefix
        {
            get
            {
                return _loadTagPrefix;
            }
            set
            {
                if (Set<string>(() => LoadTagPrefix, ref _loadTagPrefix, value))
                {
                    IsDirty = true;
                }
            }
        }

        private int _selectedExportIndex = 0;
        public int SelectedExportIndex
        {
            get
            {
                return _selectedExportIndex;
            }
            set
            {
                if (Set<int>(() => SelectedExportIndex, ref _selectedExportIndex, value))
                {
                    IsDirty = true;
                }
            }
        }        

        private string _lastConnectedScannerName = "";
        public string LastConnectedScannerName
        {
            get
            {
                return _lastConnectedScannerName;
            }
            set
            {
                if (Set<string>(() => LastConnectedScannerName, ref _lastConnectedScannerName, value))
                {
                    IsDirty = true;
                }
            }
        }

        private string _lastConnectedScannerID = "";
        public string LastConnectedScannerID
        {
            get
            {
                return _lastConnectedScannerID;
            }
            set
            {
                if (Set<string>(() => LastConnectedScannerID, ref _lastConnectedScannerID, value))
                {
                    IsDirty = true;
                }
            }
        }
    }
}
