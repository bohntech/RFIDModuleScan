//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RFIDModuleScan.Core.Models;
using System.Collections.ObjectModel;
using RFIDModuleScan.Core.Scanners;
using RFIDModuleScan.Core.Data;
using GalaSoft.MvvmLight.Views;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using RFIDModuleScan.Core.Helpers;
using RFIDModuleScan.Core.Enums;
using RFIDModuleScan.Core.Messages;
using RFIDModuleScan.Core.Services;


namespace RFIDModuleScan.Core.ViewModels
{
    public class ReviewPageViewModel : ViewModelBase
    {
        #region Private Properties
        private INavigationService _navigationService;
        private ScannerModel _connectedReader;

        private IEmailService _emailService;
        
        //private Position _lastPosition;
        private IModuleDataService _dataService;
        private Guid? _fieldScanID = null;

        #endregion

        #region Observable Properties
        private ModuleScanViewModel _searchResult;
        public ModuleScanViewModel SearchResult
        {
            get
            {
                return _searchResult;
            }
            set
            {
                Set<ModuleScanViewModel>(() => SearchResult, ref _searchResult, value);
            }
        }

        private string _grower;
        public string Grower
        {
            get { return _grower; }
            private set { Set<string>(() => Grower, ref _grower, value); }
        }

        private string _farm;
        public string Farm
        {
            get { return _farm; }
            private set { Set<string>(() => Farm, ref _farm, value); }
        }

        private string _field;
        public string Field
        {
            get { return _field; }
            private set { Set<string>(() => Field, ref _field, value);}
        }

        private int _moduleCount;
        public int ModuleCount
        {
            get { return _moduleCount; }
            private set { Set<int>(() => ModuleCount, ref _moduleCount, value); }
        }

        private int _loadCount;
        public int LoadCount
        {
            get { return _loadCount; }
            private set { Set<int>(() => LoadCount, ref _loadCount, value); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                Set<bool>(() => IsBusy, ref _isBusy, value);
            }
        }

        private string _busyMessage;
        public string BusyMessage
        {
            get
            {
                return _busyMessage;
            }
            private set
            {
                Set<string>(() => BusyMessage, ref _busyMessage, value);
            }
        }

        private int _modulesWithNoGPS;
        public int ModulesWithNoGPS
        {
            get
            {
                return _modulesWithNoGPS;
            }
            private set
            {
                Set<int>(() => ModulesWithNoGPS, ref _modulesWithNoGPS, value);
            }                
        }

        private RangeObservableCollection<LoadViewModel> _loads;
        public RangeObservableCollection<LoadViewModel> Loads
        {
            get { return _loads; }
            private set
            {
                _loads = value;                
            }
        }

        private string _connectionMsg;
        public string ConnectionMsg
        {
            get
            {
                return _connectionMsg;
            }
            set
            {
                Set<string>(() => ConnectionMsg, ref _connectionMsg, value);
            }
        }
        #endregion

        #region Contructor and Cleanup
        public ReviewPageViewModel(INavigationService navigationService, IModuleDataService dataService, Guid? FieldScanID = null)
        {
            IsBusy = true;
            BusyMessage = "Loading...";
            _fieldScanID = FieldScanID;
            _dataService = dataService;
            _navigationService = navigationService;
            _emailService = Xamarin.Forms.DependencyService.Get<IEmailService>();

            Loads = new RangeObservableCollection<LoadViewModel>();
            
            ManualSearchCommand = new RelayCommand<string>(this.ExecuteManualSearchCommand);
            TransmitCommand = new RelayCommand(this.ExecuteTransmitCommand);

            FindWithCameraCommand = new RelayCommand(this.ExecuteFindWithCameraCommand);
        }

        public void Dispose()
        {           
            this.Loads.Clear();
                        
            ScannerConnectionManager.ScannerContext.ConnectionStateChanged -= ScannerContext_ConnectionStateChanged;
            ScannerConnectionManager.ScannerContext.ItemScanned -= ScannerContext_ItemScanned;
        }

        public void Initialize()
        {
            IsBusy = true;
            BusyMessage = "Loading...";
            if (ScannerConnectionManager.ScannerContext != null)
            {
                ScannerConnectionManager.ScannerContext.ConnectionStateChanged += ScannerContext_ConnectionStateChanged;
                ScannerConnectionManager.ScannerContext.ItemScanned += ScannerContext_ItemScanned;
                _connectedReader = ScannerConnectionManager.ScannerContext.GetConnectedScanner();
                if (_connectedReader != null)
                {
                    ConnectionMsg = _connectedReader.DisplayName;
                }
                else
                {
                    ConnectionMsg = "Not connected.";
                }
            }
          

            if (_fieldScanID.HasValue)  //existing scan so load scan info
            {
                //_fieldScanID = FieldScanID;
                FieldScan scan = _dataService.GetByID<FieldScan>(_fieldScanID.Value);

                LoadCount = 0;
                ModuleCount = 0;              
                Grower = scan.Grower;
                Field = scan.Field;
                Farm = scan.Farm;
               
                //load loads and modules from data store to observable collection
                var loads = _dataService.Find<Load, int>(x => x.FieldScanID == _fieldScanID.Value, o => o.LoadNumber);
                var modules = _dataService.Find<ModuleScan, DateTime>(x => x.FieldScanID == _fieldScanID.Value, o => o.TimeStamp);

                LoadCount = loads.Count();
                ModuleCount = modules.Count();

                lock (Loads)
                {
                    foreach (var load in loads)
                    {
                        LoadViewModel loadVM = new LoadViewModel();
                        loadVM.LoadNumber = load.LoadNumber;
                        loadVM.Notes = load.Notes;
                        loadVM.Modules = new RangeObservableCollection<ModuleScanViewModel>();
                        loadVM.ID = load.ID;
                        loadVM.IsOpen = true;

                        foreach (var module in modules.Where(x => x.LoadID == load.ID))
                        {
                            ModuleScanViewModel moduleVM = new ModuleScanViewModel();
                            moduleVM.ID = module.ID;
                            moduleVM.LoadNumber = loadVM.LoadNumber;
                            moduleVM.LoadID = loadVM.ID;
                            moduleVM.Latitude = module.Latitude;
                            moduleVM.Longitude = module.Longitude;
                            moduleVM.ModuleType = (Enums.BarCodeTypeEnum)module.BarcodeType;
                            moduleVM.SerialNumber = module.SerialNumber;
                            moduleVM.TimeStamp = module.TimeStamp;
                            loadVM.Modules.AddWithoutNotify(moduleVM);
                        }
                        Loads.AddWithoutNotify(loadVM);
                    }
                    var lastLoad = Loads.OrderBy(l => l.LoadNumber).LastOrDefault();

                    ModulesWithNoGPS = 0;
                    foreach (var l in Loads)
                    {
                        l.Modules.ApplyUpdates();
                        l.RefreshCount();                        
                        ModulesWithNoGPS += l.Modules.Count(m => m.NoLocation);
                    }
                    Loads.ApplyUpdates();          
                }
            }
            IsBusy = false;
        }
        #endregion

        #region Scanner Events      

        private void searchForSerialNumber(string serialNumber)
        {
            SearchResult = null;
            var load = _loads.FirstOrDefault(l => l.Modules.Any(m => m.SerialNumber == serialNumber));

            if (load != null)
            {
                SearchResult = load.Modules.FirstOrDefault(m => m.SerialNumber == serialNumber);
            }            
        }

        private void ScannerContext_ItemScanned(object sender, ItemScannedEventArgs e2)
        {
            if (!IsBusy)
            {
                if (e2.EventData.IsFlush)
                {
                    GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<ModuleScanViewModel>(SearchResult);
                }
                else
                {
                    searchForSerialNumber(e2.EventData.SerialNumber);
                }
            }
        }
        
        private void ScannerContext_ConnectionStateChanged(object sender, ConnectionStateEventArgs e)
        {
            if (e.State == ScannerState.Connected)
            {
                _connectedReader = ScannerConnectionManager.ScannerContext.GetConnectedScanner();
                if (_connectedReader != null)
                {
                    ConnectionMsg = _connectedReader.DisplayName;
                }
                else
                {
                    ConnectionMsg = "Not connected.";
                }
            }
            else if (e.State == ScannerState.Connecting)
            {
                ConnectionMsg = "Connecting...";
            }
            else if (e.State == ScannerState.Disconnecting)
            {
                ConnectionMsg = "Disconnecting...";
            }
            else if (e.State == ScannerState.Disconnnected)
            {
                ConnectionMsg = "Not connected.";
            }
        }
        #endregion

        #region public methods

        public void LocateOpticalSearchResult(string opticalRawData)
        {
            ScanEventData data = new ScanEventData { IsBarcode = true, IsFlush = false, RawData = opticalRawData };
            searchForSerialNumber(data.SerialNumber);
        }

        #endregion  

        #region Commands
        public RelayCommand<string> ManualSearchCommand { get; private set; }

        private void ExecuteManualSearchCommand(string serialNumber)
        {
            searchForSerialNumber(serialNumber);
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<ModuleScanViewModel>(SearchResult);
        }

        public RelayCommand TransmitCommand { get; private set; }

        private void ExecuteTransmitCommand()
        {
            IsBusy = true;
            BusyMessage = "Building file...";
            Task.Run(() =>
            {
                List<FieldScan> scans = _dataService.Find<FieldScan, DateTime>(s => s.ID == _fieldScanID, o => o.Created).ToList();

                string file = FileHelper.GetCSVFileString(scans, _dataService);

                string body = string.Format("GROWER: {0}\r\nFARM: {1}\r\nFIELD: {2}\r\n\r\nPlease see attached load list.\r\n", Grower, Farm, Field);

                IFileService svc = Xamarin.Forms.DependencyService.Get<IFileService>();
                string filename = "Transmission-" + DateTime.Now.ToString("MMddyyyy_hh_mm_ss_tt") + ".csv";
                string fullPath = svc.SaveText(filename, file);

                List<string> files = new List<string>();
                files.Add(fullPath);

                _emailService.ShowDraft("Load List", body, false, "", files);

                IsBusy = false;
                BusyMessage = "Loading...";
            });
        }

        public RelayCommand FindWithCameraCommand { get; private set; }
        private void ExecuteFindWithCameraCommand()
        {
            _navigationService.NavigateTo(ViewLocator.OpticalFindPage, this._fieldScanID.Value);
        }

        #endregion

    }
}
