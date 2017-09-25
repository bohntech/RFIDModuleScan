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
    public class ScanPageViewModel : ViewModelBase
    {
        #region Private Properties
        private INavigationService _navigationService;
        private ScannerModel _connectedReader;
        private IGeolocator _currentLocator;
        private Position _lastPosition;
        private IModuleDataService _dataService;
        private Guid? _fieldScanID = null;
        private bool _newScan = true;
        private ListTypeEnum _listType = ListTypeEnum.Staging;

        private string _origGrower = string.Empty;
        private string _origFarm = string.Empty;
        private string _orgField = string.Empty;
        private string _origScanLocation = string.Empty;
        private bool _origAutoLoadAssign = true;
        private string _origStartLoadNumber = "1";
        private string _origMaxModulesPerLoad = "1";

        private Queue<ScanEventData> itemQueue = null;

        #endregion

        #region Observable Properties
               
        
        /*private void refreshErrors()
        {
            ErrorMessages = "";

            if (!ValidationHelper.ValidInt(MaxModulesPerLoad))
            {
                ErrorMessages += "Max modules per load must be a number.\r\n";
            }
            if (!ValidationHelper.ValidInt(StartingLoadNumber))
            {
                ErrorMessages += "Starting load number must be a number.";
            }
        }*/

        private string _gpsMessage;
        public string GPSMessage
        {
            get
            {
                return _gpsMessage;
            }
            private set
            {
                Set<string>(() => GPSMessage, ref _gpsMessage, value);
            }
        }


        private string _connectionMessage;
        public string ConnectionMessage
        {
            get
            {
                return _connectionMessage;
            }
            private set
            {
                Set<string>(() => ConnectionMessage, ref _connectionMessage, value);
            }
        }

        private RangeObservableCollection<LoadViewModel> _loads;
        public RangeObservableCollection<LoadViewModel> Loads
        {
            get { return _loads; }
            private set
            {
                _loads = value;                
                RaisePropertyChanged("Loads");
            }
        }

        private string _grower;
        public string Grower
        {
            get
            {
                return _grower;
            }
            set
            {
                Set<String>(() => Grower, ref _grower, value);
            }
        }

        private string _farm;
        public string Farm
        {
            get
            {
                return _farm;
            }
            set
            {
                Set<String>(() => Farm, ref _farm, value);
            }
        }

        private string _field;
        public string Field
        {
            get
            {
                return _field;
            }
            set
            {
                Set<String>(() => Field, ref _field, value);
            }
        }

        private string _notes;
        public string Notes
        {
            get { return _notes; }
            set { Set<String>(() => Notes, ref _notes, value); }
        }

        private string _scanLocation;
        public string ScanLocation
        {
            get { return _scanLocation; }
            set {
                Set<String>(() => ScanLocation, ref _scanLocation, value);
            }
        }

        private bool _autoLoadAssign=true;
        public bool AutoLoadAssign
        {
            get
            {
                return _autoLoadAssign;
            }
            set
            {
                Set<bool>(() => AutoLoadAssign, ref _autoLoadAssign, value);
                MaxModulesPerLoadVisible = AutoLoadAssign;
                refreshCanStartNewLoad();
            }
        }

        private string _maxModulesPerLoad;
        public string MaxModulesPerLoad
        {
            get
            {
                return _maxModulesPerLoad;
            }
            set
            {
                Set<string>(() => MaxModulesPerLoad, ref _maxModulesPerLoad, value);
               // refreshErrors();
            }
        }

        private bool _canEditStartingLoadNumber;
        public bool CanEditStartingLoadNumber
        {
            get
            {
                return _canEditStartingLoadNumber;
            }
            set
            {
                Set<bool>(() => CanEditStartingLoadNumber, ref _canEditStartingLoadNumber, value);
            }
        }

        private bool _maxModulesPerLoadVisible;
        public bool MaxModulesPerLoadVisible
        {
            get
            {
                return _maxModulesPerLoadVisible;
            }
            set
            {
                Set<bool>(() => MaxModulesPerLoadVisible, ref _maxModulesPerLoadVisible, value);
            }
        }

        private string _startingLoadNumber;
        public string StartingLoadNumber
        {
            get
            {
                return _startingLoadNumber;
            }
            set
            {
                Set<string>(() => StartingLoadNumber, ref _startingLoadNumber, value);
                //refreshErrors();
            }
        }

        private int _moduleCount;
        public int ModuleCount
        {
            get
            {
                return _moduleCount;
            }
            set
            {
                Set<int>(() => ModuleCount, ref _moduleCount, value);
                refreshCanStartNewLoad();
            }
        }

        private int _loadCount;
        public int LoadCount
        {
            get
            {
                return _loadCount;
            }
            set
            {
                Set<int>(() => LoadCount, ref _loadCount, value);
            }
        }

        private bool _isEditMode;
        public bool IsEditMode
        {
            get
            {
                return _isEditMode;
            }
            private set
            {
                Set<bool>(() => IsEditMode, ref _isEditMode, value);
                refreshCanStartNewLoad();
            }
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

        private bool _isReviewMode;
        public bool IsReviewMode
        {
            get
            {
                return _isReviewMode;
            }
            private set
            {
                Set<bool>(() => IsReviewMode, ref _isReviewMode, value);                
            }
        }

        private bool _canStartNewLoad;
        public bool CanStartNewLoad
        {
            get
            {
                return _canStartNewLoad;
            }
            private set
            {
                Set<bool>(() => CanStartNewLoad, ref _canStartNewLoad, value);
            }
        }

        private bool _isStagingScan;
        public bool IsStagingScan
        {
            get
            {
                return _isStagingScan;
            }
            set
            {
                Set<bool>(() => IsStagingScan, ref _isStagingScan, value);
            }
        }

        public Guid? ScanID { get { return _fieldScanID; } }

        #endregion

        #region Private Methods       

        private async void refreshPosition()
        {
            if (_currentLocator != null)
            {
                _lastPosition = await _currentLocator.GetPositionAsync();
                GPSMessage = _lastPosition.Latitude.ToString() + ", " + _lastPosition.Longitude.ToString();
            }
        }

        private void refreshCanStartNewLoad()
        {
            CanStartNewLoad = CanExecuteStartNewLoadCommand();            
        }

        private bool validateForm()
        {
            if (IsStagingScan)
            {
                return (!string.IsNullOrWhiteSpace(Grower) &&
                    !string.IsNullOrWhiteSpace(Farm) &&
                    !string.IsNullOrWhiteSpace(Field) &&
                    ValidationHelper.ValidInt(StartingLoadNumber) &&
                    ValidationHelper.ValidInt(MaxModulesPerLoad));
            }
            else
            {
                return (!string.IsNullOrWhiteSpace(Grower) &&
                    !string.IsNullOrWhiteSpace(Farm) &&
                    !string.IsNullOrWhiteSpace(Field) &&
                    !string.IsNullOrWhiteSpace(ScanLocation) &&
                    ValidationHelper.ValidInt(StartingLoadNumber) &&
                    ValidationHelper.ValidInt(MaxModulesPerLoad));
            }
        }

        private bool serialNumberExists(string serialNumber)
        {
            //return false;
            return _loads.Any(l => l.Modules.Any(m => m.SerialNumber == serialNumber));
        }

        private void addModules(List<ModuleScanViewModel> scans)
        {
            lock (Loads)
            {
                List<ModuleScan> scansToInsert = new List<ModuleScan>();
                List<Load> loadsToInsert = new List<Load>();

                LoadViewModel lastLoad = Loads.OrderBy(s => s.LoadNumber).LastOrDefault();
                foreach (var scan in scans)
                { 
                    //update GPS for serial numbers with no GPS
                    foreach(var load in _loads)
                    {
                        foreach(var module in load.Modules)
                        {
                            if (module.SerialNumber == scan.SerialNumber && module.NoLocation && !scan.NoLocation)
                            {
                                module.Latitude = scan.Latitude;
                                module.Longitude = scan.Longitude;
                                module.SerialNumberWithMessage = scan.SerialNumber;

                                ModuleScan dbScan = _dataService.GetByID<ModuleScan>(module.ID);
                                dbScan.Latitude = scan.Latitude.Value;
                                dbScan.Longitude = scan.Longitude.Value;
                                _dataService.Save(dbScan);
                            }
                        }
                    }

                    if (!serialNumberExists(scan.SerialNumber))
                    {
                        //LoadViewModel loadVM = null;
                        if (lastLoad == null || (lastLoad.Modules.Count() >= int.Parse(MaxModulesPerLoad) && AutoLoadAssign))
                        {
                            //start a new load
                            var nextLoadNumber = (lastLoad != null) ? lastLoad.LoadNumber + 1 : int.Parse(StartingLoadNumber);

                            lastLoad = new LoadViewModel();
                            lastLoad.LoadNumber = nextLoadNumber;
                            lastLoad.Modules = new RangeObservableCollection<ModuleScanViewModel>();

                            //create the new load
                            var dbLoad = new Load { FieldScanID = _fieldScanID.Value, LoadNumber = lastLoad.LoadNumber, Notes = Notes, ID = Guid.NewGuid() };
                            loadsToInsert.Add(dbLoad);
                            //dbLoad.ID = _dataService.Save(dbLoad);
                            //GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<LoadViewModel>(loadVM);                    
                            lastLoad.ID = dbLoad.ID;
                            Loads.AddWithoutNotify(lastLoad);
                        }


                        scan.LoadID = lastLoad.ID;
                        lock (lastLoad.Modules)
                        {
                            lastLoad.Modules.AddWithoutNotify(scan);
                        }
                        lastLoad.IsDirty = true;
                        var moduleScan = new ModuleScan();
                        moduleScan.ID = scan.ID;
                        moduleScan.BarcodeType = (int)scan.ModuleType;
                        moduleScan.LoadID = lastLoad.ID;
                        moduleScan.FieldScanID = _fieldScanID.Value;
                        moduleScan.Latitude = scan.Latitude.HasValue ? scan.Latitude.Value : 0.000M;
                        moduleScan.Longitude = scan.Longitude.HasValue ? scan.Longitude.Value : 0.000M;
                        moduleScan.SerialNumber = scan.SerialNumber;
                        moduleScan.TimeStamp = scan.TimeStamp;
                        moduleScan.Note = Notes;
                        scansToInsert.Add(moduleScan);
                    }
                }

                //notify of changes                     
                foreach (var l in Loads)
                {
                    lock (l.Modules)
                    {
                        l.Modules.ApplyUpdates();
                        l.RefreshCount();
                    }
                }
                Loads.ApplyUpdates();
                
                _dataService.InsertAll(loadsToInsert);
                _dataService.InsertAll(scansToInsert);

                LoadCount = Loads.Count();
                ModuleCount = Loads.Sum(x => x.Modules.Count());

                for(var i=0; i < Loads.Count()-1; i++)
                {
                    Loads[i].IsOpen = false;
                }

                Loads[Loads.Count() - 1].IsOpen = true;               
            }            
        }
        #endregion

        #region Constructor and Cleanup
        public ScanPageViewModel(INavigationService navigationService, IModuleDataService dataService, Guid? FieldScanID = null, ListTypeEnum listType=ListTypeEnum.Staging)
        {
            _fieldScanID = FieldScanID;
            _dataService = dataService;
            _navigationService = navigationService;
            _listType = listType;
            Loads = new RangeObservableCollection<LoadViewModel>();
            IsBusy = true;
            AutoLoadAssign = true;
            BusyMessage = "Loading...";
            CanEditStartingLoadNumber = true;

            IsStagingScan = (listType == ListTypeEnum.Staging);

           

            itemQueue = new Queue<ScanEventData>();

            SaveCommand = new RelayCommand(this.ExecuteSave);
            DeleteCommand = new RelayCommand(this.ExecuteDeleteCommand);
            MoveCommand = new RelayCommand<int>(this.ExecuteMoveCommand);
            CancelCommand = new RelayCommand(this.ExecuteCancelCommand);
            ToggleEditCommand = new RelayCommand(this.ExecuteToggleEditCommand);
            StartNewLoadCommand = new RelayCommand(this.ExecuteStartNewLoadCommand);
            ReviewCommand = new RelayCommand(this.ExecuteReviewCommand);
            TransmitCommand = new RelayCommand(this.ExecuteTransmitCommand);
            DeleteAllCommand = new RelayCommand(this.ExecuteDeleteAllCommand);
            RenumberCommand = new RelayCommand(this.ExecuteRenumberCommand);
            StartOpticalScanCommand = new RelayCommand(this.ExecuteStartOpticalScanCommand);

            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<NotesChangedMessage>(this, this.SaveNotes);
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<ReviewCancelMessage>(this, this.HandleReviewCancel);
        }

        public void Dispose()
        {
            this.itemQueue.Clear();
            this.Loads.Clear();
            
            if (_currentLocator != null && _currentLocator.IsListening)
            {
                _currentLocator.StopListeningAsync();

                _currentLocator.PositionChanged -= _currentLocator_PositionChanged;
                _currentLocator.PositionError -= _currentLocator_PositionError;
            }

            GalaSoft.MvvmLight.Messaging.Messenger.Default.Unregister<NotesChangedMessage>(this);
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Unregister<ReviewCancelMessage>(this);
            ScannerConnectionManager.ScannerContext.ConnectionStateChanged -= ScannerContext_ConnectionStateChanged;
            ScannerConnectionManager.ScannerContext.ItemScanned -= ScannerContext_ItemScanned;
            this.Cleanup();
        }

        public void Initialize()
        {
            IsReviewMode = false;
            IsBusy = true;
            BusyMessage = "Loading...";

            if (!IsStagingScan) //there will only be one drop location scan list on device
            {
                MaxModulesPerLoad = int.MaxValue.ToString();
                AutoLoadAssign = false;
                StartingLoadNumber = "1";

                var result = _dataService.Find<FieldScan, DateTime>(x => x.ListTypeID == (int)_listType, o => o.Created).FirstOrDefault();

                if (result != null)
                {
                    _fieldScanID = result.ID;
                }               
            }
            else
            {
                ScanLocation = "Staging";
            }

            if (ScannerConnectionManager.ScannerContext != null)
            {
                ScannerConnectionManager.ScannerContext.ConnectionStateChanged += ScannerContext_ConnectionStateChanged;
                ScannerConnectionManager.ScannerContext.ItemScanned += ScannerContext_ItemScanned;
                _connectedReader = ScannerConnectionManager.ScannerContext.GetConnectedScanner();
                if (_connectedReader != null)
                {
                    ConnectionMessage = _connectedReader.DisplayName;
                }
                else
                {
                    ConnectionMessage = "Not connected.";
                }
            }
           

            Task.Run(() =>
            {
                _currentLocator = CrossGeolocator.Current;
                _currentLocator.AllowsBackgroundUpdates = false;
                _currentLocator.DesiredAccuracy = 5;
                _currentLocator.PositionChanged += _currentLocator_PositionChanged;
                _currentLocator.PositionError += _currentLocator_PositionError;
                _currentLocator.StopListeningAsync();

                //start a timer to check every 5 seconds for GPS being switched on/off
                Xamarin.Forms.Device.StartTimer(new TimeSpan(0, 0, 5), () =>
                {

                    if (_currentLocator == null)
                    {
                        _currentLocator = CrossGeolocator.Current;
                    }

                    if (!_currentLocator.IsGeolocationAvailable || !_currentLocator.IsGeolocationEnabled)
                    {
                        GPSMessage = "GPS not available";
                    }
                    else if (!_currentLocator.IsListening && _currentLocator.IsGeolocationEnabled && _currentLocator.IsGeolocationAvailable)
                    {
                        _currentLocator.StartListeningAsync(5, 5.0, false);
                        refreshPosition();
                    }

                    return true;
                });
            });

            if (_fieldScanID.HasValue)  //existing scan so load scan info
            {
                //_fieldScanID = FieldScanID;
                FieldScan scan = _dataService.GetByID<FieldScan>(_fieldScanID.Value);

                LoadCount = 0;
                ModuleCount = 0;
                CanEditStartingLoadNumber = false;
                StartingLoadNumber = scan.StartingLoadNumber.ToString();
                MaxModulesPerLoad = scan.MaxModulesPerLoad.ToString();
                AutoLoadAssign = scan.AutoLoadAssign;
                Grower = scan.Grower;
                Field = scan.Field;
                Farm = scan.Farm;
                ScanLocation = scan.ScanLocation;
                Notes = scan.Note;
                CanEditStartingLoadNumber = true;
                _newScan = false;
                IsEditMode = false;

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
                        loadVM.IsOpen = false;

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


                    foreach(var l in Loads)
                    {                       
                        l.Modules.ApplyUpdates();
                        l.RefreshCount();
                    }
                    Loads.ApplyUpdates();

                    if (lastLoad != null)
                    {
                        lastLoad.IsOpen = true;
                    }

                    persistScanCounts();

                    GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<LoadsChangedMessage>(new LoadsChangedMessage());
                }
            }
            else
            {
                _newScan = true;
                IsEditMode = true;
                _fieldScanID = null;
                LoadCount = 0;
                ModuleCount = 0;
                CanEditStartingLoadNumber = true;
                _origStartLoadNumber = StartingLoadNumber = "1";
                AutoLoadAssign = true;

                if (IsStagingScan)
                {
                    _origMaxModulesPerLoad = MaxModulesPerLoad = Configuration.MaxModulesPerLoad.ToString();
                }
                else
                {
                    _origMaxModulesPerLoad = MaxModulesPerLoad = int.MaxValue.ToString();
                }
            }

            refreshCanStartNewLoad();
            IsBusy = false;
        }

        #endregion
                
        #region View Commands
        private void persistScanCounts()
        {
            if (_fieldScanID.HasValue)
            {
                FieldScan scan = _dataService.GetByID<FieldScan>(_fieldScanID.Value);
                scan.LoadCount = LoadCount;
                scan.ModuleCount = ModuleCount;
                scan.Transmitted = null;                         
                var lastLoad = Loads.LastOrDefault();
                if (lastLoad != null)
                {
                    var lastScan = lastLoad.Modules.LastOrDefault();
                    if (lastScan != null)
                        scan.LastScan = lastScan.TimeStamp;
                }

                _dataService.Save<FieldScan>(scan);
            }
        }

        public RelayCommand SaveCommand { get; private set; }

        private void ExecuteSave()
        {
            if (validateForm())
            {
                //save                
                IsEditMode = false;
                FieldScan scan = new FieldScan();
                scan.AutoLoadAssign = AutoLoadAssign;
                scan.Farm = Farm;
                scan.Field = Field;
                scan.Grower = Grower;
                scan.ListTypeID = (int)_listType;
                scan.MaxModulesPerLoad = int.Parse(MaxModulesPerLoad);
                scan.Note = Notes;
                scan.ScanLocation = ScanLocation;
                scan.StartingLoadNumber = int.Parse(StartingLoadNumber);                                                      

                if (_fieldScanID.HasValue)
                {
                    scan.ID = _fieldScanID.Value;                  
                }               

                _fieldScanID = _dataService.Save(scan);
                                
                persistScanCounts();
            }
        }

        public RelayCommand DeleteCommand { get; private set; }

        private async void ExecuteDeleteCommand()
        {

            IsBusy = true;
            await Task.Run(() =>
            {
                List<ModuleScanViewModel> modulesToRemove = new List<ModuleScanViewModel>();

                List<ModuleScan> dbModulesToDelete = new List<ModuleScan>();

                foreach (var l in Loads)
                {
                    foreach (var m in l.Modules.Where(m => m.Selected))
                    {
                        modulesToRemove.Add(m);
                    }

                    foreach (var m in modulesToRemove)
                    {
                        l.Modules.RemoveWithoutNotify(m);
                        _dataService.DeleteByID<ModuleScan>(m.ID);
                    }

                    l.Modules.ApplyUpdates();
                    l.RefreshCount();
                }

                ModuleCount = Loads.Sum(x => x.ModuleCount);
                LoadCount = Loads.Count();
                persistScanCounts();
                IsBusy = false;
            });
        }

        public RelayCommand<int> MoveCommand { get; private set; }

        private async void ExecuteMoveCommand(int LoadNumber)
        {
            IsBusy = true;
            await Task.Run(() =>
            {
                List<ModuleScanViewModel> modulesSelected = new List<ModuleScanViewModel>();
                List<ModuleScanViewModel> modulesToAdd = new List<ModuleScanViewModel>();
                List<ModuleScanViewModel> modulesToRemove = new List<ModuleScanViewModel>();

                LoadViewModel targetLoad = null;

                if (LoadNumber == 0)
                {
                    targetLoad = new LoadViewModel();
                    var lastLoad = Loads.OrderBy(l => l.LoadNumber).Last();
                    targetLoad.LoadNumber = lastLoad.LoadNumber + 1;
                    targetLoad.Modules = new RangeObservableCollection<ModuleScanViewModel>();
                    targetLoad.IsOpen = true;
                    targetLoad.Notes = string.Empty;
                    targetLoad.IsDirty = true;
                    Loads.AddWithoutNotify(targetLoad);

                    Load dbLoad = new Load();
                    dbLoad.Created = DateTime.Now;
                    dbLoad.FieldScanID = _fieldScanID.Value;
                    dbLoad.LoadNumber = targetLoad.LoadNumber;
                    dbLoad.Notes = string.Empty;
                    targetLoad.ID = _dataService.Save(dbLoad);
                }
                else
                {
                    targetLoad = Loads.Single(l => l.LoadNumber == LoadNumber);
                }

                //remove modules from source loads
                foreach (var l in Loads)
                {
                    modulesToRemove.Clear();
                    modulesSelected.AddRange(l.Modules.Where(m => m.Selected));
                    modulesToRemove.AddRange(l.Modules.Where(m => m.Selected));

                    foreach (var m in modulesToRemove)
                    {
                        l.Modules.RemoveWithoutNotify(m);
                        _dataService.DeleteByID<ModuleScan>(m.ID);
                    }
                }

                foreach (var l in Loads) //update loads for removals
                {
                    l.Modules.ApplyUpdates();
                    l.RefreshCount();
                }

                //add modules to target load
                foreach (var m in modulesSelected)
                {
                    m.Selected = false;
                    targetLoad.Modules.AddWithoutNotify(m);
                    var moduleScan = new ModuleScan();
                    //moduleScan.ID = m.ID;
                    moduleScan.BarcodeType = (int)m.ModuleType;
                    moduleScan.LoadID = targetLoad.ID;
                    moduleScan.FieldScanID = _fieldScanID.Value;
                    moduleScan.Latitude = m.Latitude.HasValue ? m.Latitude.Value : 0.000M;
                    moduleScan.Longitude = m.Longitude.HasValue ? m.Longitude.Value : 0.000M;
                    moduleScan.SerialNumber = m.SerialNumber;
                    moduleScan.TimeStamp = m.TimeStamp;
                    m.ID = _dataService.Save<ModuleScan>(moduleScan);
                }

                foreach (var l in Loads)
                {
                    l.Modules.ApplyUpdates();
                    l.RefreshCount();
                }
                Loads.ApplyUpdates();

                ModuleCount = Loads.Sum(x => x.ModuleCount);
                LoadCount = Loads.Count();

                persistScanCounts();

                IsBusy = false;
            });    
        }

        public RelayCommand CancelCommand { get; private set; }

        private void ExecuteCancelCommand()
        {
            if (IsEditMode)
            {
                if (IsEditMode && !_fieldScanID.HasValue)
                {
                    _navigationService.GoBack();
                }
                else
                {
                    IsEditMode = false;

                    Field = _orgField;
                    AutoLoadAssign = _origAutoLoadAssign;
                    Farm = _origFarm;
                    Grower = _origGrower;
                    ScanLocation = _origScanLocation;
                    MaxModulesPerLoad = _origMaxModulesPerLoad;
                    StartingLoadNumber = _origStartLoadNumber;
                }
            }
        }

        public RelayCommand ToggleEditCommand { get; private set; }
        private void ExecuteToggleEditCommand()
        {
            if (!IsEditMode)
            {
                IsEditMode = true;

                //save original form values for cancellation
                _orgField = Field;
                _origAutoLoadAssign = AutoLoadAssign;
                _origFarm = Farm;
                _origScanLocation = ScanLocation;
                _origGrower = Grower;
                _origMaxModulesPerLoad = MaxModulesPerLoad;
                _origStartLoadNumber = StartingLoadNumber;
            }
        }

        public RelayCommand StartNewLoadCommand { get; private set; }
        private bool CanExecuteStartNewLoadCommand()
        {
            LoadViewModel lastLoad = Loads.OrderBy(s => s.LoadNumber).LastOrDefault();
            return (!AutoLoadAssign && lastLoad != null && lastLoad.Modules.Count() > 0 && !IsEditMode);
        }
        private async void ExecuteStartNewLoadCommand()
        {
            if (!IsEditMode)
            {
                LoadViewModel lastLoad = Loads.OrderBy(s => s.LoadNumber).LastOrDefault();

                if (!AutoLoadAssign && lastLoad.Modules.Count() > 0)
                {
                    foreach(var load in Loads)
                    {
                        load.IsOpen = false;
                    }

                    await Task.Run(() => {

                        lock (Loads)
                        {
                            LoadViewModel newLoad = new LoadViewModel();                            
                            newLoad.IsOpen = true;
                            newLoad.IsDirty = true;
                            newLoad.LoadNumber = lastLoad.LoadNumber + 1;
                            newLoad.ModuleCount = 0;
                            newLoad.Modules = new RangeObservableCollection<ModuleScanViewModel>();
                            Loads.Add(newLoad);

                            Load dbLoad = new Load();
                            dbLoad.Created = DateTime.Now;
                            dbLoad.FieldScanID = _fieldScanID.Value;
                            dbLoad.LoadNumber = newLoad.LoadNumber;
                            dbLoad.Notes = string.Empty;
                            newLoad.ID=_dataService.Save(dbLoad);
                        }
                    });
                }
                LoadCount = Loads.Count();
                persistScanCounts();
            }
        }

        public RelayCommand StartOpticalScanCommand { get; private set; }
        private void ExecuteStartOpticalScanCommand()
        {
            _navigationService.NavigateTo(ViewLocator.OpticalScanPage, this);
        }

        public RelayCommand ReviewCommand { get; private set; }
        private void ExecuteReviewCommand()
        {
            IsReviewMode = true;
            _navigationService.NavigateTo(ViewLocator.ReviewPage, _fieldScanID.Value);
        }

        public RelayCommand TransmitCommand { get; private set; }
        private void ExecuteTransmitCommand()
        {
            IsBusy = true;
            BusyMessage = "Building file...";
            Task.Run(() =>
            {
                List<FieldScan> scans = _dataService.Find<FieldScan, DateTime>(s => s.ID == _fieldScanID, o => o.Created).ToList();

                string file = "";
                string extension = ".csv";

                if (Configuration.SelectedExportFormat == ExportFormatEnum.PlainCSV)
                    file = FileHelper.GetCSVFileString(scans, _dataService);
                else
                {
                    file = FileHelper.GetHIDFileString(scans, _dataService);
                    extension = ".TXT";
                }

                string body = string.Format("GROWER: {0}\r\nFARM: {1}\r\nFIELD: {2}\r\nNOTES: \r\n{3}\r\n\r\nPlease see attached load list.\r\n", Grower, Farm, Field, Notes);

                IFileService svc = Xamarin.Forms.DependencyService.Get<IFileService>();
                IEmailService _emailService = Xamarin.Forms.DependencyService.Get<IEmailService>();
                string filename = "Transmission-" + DateTime.Now.ToString("MMddyyyy_hh_mm_ss_tt") + extension;
                string fullPath = svc.SaveText(filename, file);

                List<string> files = new List<string>();
                files.Add(fullPath);

                _emailService.ShowDraft("Module List", body, false, "", files);

                IsBusy = false;
                BusyMessage = "Loading...";
            });
        }

        public RelayCommand DeleteAllCommand { get; private set; }

        private void ExecuteDeleteAllCommand()
        {
            if (_fieldScanID.HasValue)
            {
                _dataService.DeleteScanAndRelatedData(_fieldScanID.Value);
            }

            _navigationService.GoBack();
        }

        public RelayCommand RenumberCommand { get; private set; }
        private async void ExecuteRenumberCommand()
        {
            IsBusy = true;

            await Task.Run(() => { 
                //remove empty loads
                //var scanLoads = _dataService.Find<Load, int>(l => l.FieldScanID == _fieldScanID.Value, o => o.LoadNumber);
                //var scanModules = _dataService.Find<ModuleScan, DateTime>(m => m.FieldScanID == _fieldScanID.Value, o => o.TimeStamp);
                List<LoadViewModel> loadsToDelete = new List<LoadViewModel>();            

                foreach(var load in Loads)
                {
                    if (load.Modules.Count() == 0)
                    {
                        loadsToDelete.Add(load);
                    }
                }

                foreach(var load in loadsToDelete)
                {
                    Loads.Remove(load);
                    _dataService.DeleteByID<Load>(load.ID);
                }

                //renumber from start load number
                int currentNumber = int.Parse(StartingLoadNumber);
                foreach(var load in Loads.OrderBy(o => o.LoadNumber))
                {
                    load.LoadNumber = currentNumber;
                    _dataService.UpdateLoadNumber(load.ID, currentNumber);
                    currentNumber++;
                }

                LoadCount = Loads.Count();

                persistScanCounts();

                IsBusy = false;
            });
        }

        #endregion

        #region GPS Events
        private void _currentLocator_PositionError(object sender, PositionErrorEventArgs e)
        {
            if (e.Error == GeolocationError.PositionUnavailable)
            {
                GPSMessage = "GPS not available";
            }
            else
            {
                GPSMessage = "GPS not authorized";
            }
        }

        private void _currentLocator_PositionChanged(object sender, PositionEventArgs e)
        {
            _lastPosition = e.Position;
            GPSMessage = e.Position.Latitude.ToString() + ", " + e.Position.Longitude.ToString();
        }
        #endregion

        #region Scanner Events      

        public void AddOpticalScan(string rawScanData, out string serialNumber, out string gpsMessage)
        {
            ItemScannedEventArgs args = new ItemScannedEventArgs();
            args.EventData = new ScanEventData();
            args.EventData.IsBarcode = true;
            args.EventData.IsFlush = false;
            args.EventData.MoreLines = false;
            args.EventData.RawData = rawScanData;
            ScannerContext_ItemScanned(this, args);

            ItemScannedEventArgs argsFlush = new ItemScannedEventArgs();
            argsFlush.EventData = new ScanEventData { IsBarcode = false, RawData = "", MoreLines = false, IsFlush = true };
            ScannerContext_ItemScanned(this, argsFlush);

            serialNumber = args.EventData.SerialNumber;
            gpsMessage = GPSMessage;
        }
        
        private void ScannerContext_ItemScanned(object sender, ItemScannedEventArgs e2)
        {
            lock (itemQueue)
            {
                if (!IsEditMode && !IsReviewMode && !IsBusy)  //don't add items when on review page
                {
                    if (!e2.EventData.IsFlush)
                    {
                        itemQueue.Enqueue(e2.EventData);
                    }
                    else
                    {

                        ScanEventData eventData = null;
                        List<ModuleScanViewModel> scansToAdd = new List<ModuleScanViewModel>();
                        while (itemQueue.Count() > 0)
                        {
                            eventData = itemQueue.Dequeue();
                            var scan = new ModuleScanViewModel();
                            if (eventData.IsJohnDeereTag)
                            {
                                if (eventData.IsBarcode)
                                {
                                    scan.ModuleType = Enums.BarCodeTypeEnum.JohnDeereBarCode;
                                }
                                else
                                {
                                    scan.ModuleType = Enums.BarCodeTypeEnum.JohnDeereRFTag;
                                }
                            }
                            else
                            {
                                if (eventData.IsBarcode)
                                {
                                    scan.ModuleType = Enums.BarCodeTypeEnum.GenericBarcode;
                                }
                                else
                                {
                                    scan.ModuleType = Enums.BarCodeTypeEnum.GenericRFTag;
                                }
                            }

                            if (_currentLocator != null && _currentLocator.IsGeolocationAvailable && _currentLocator.IsGeolocationEnabled && _lastPosition != null)
                            {
                                scan.Latitude = Convert.ToDecimal(_lastPosition.Latitude);
                                scan.Longitude = Convert.ToDecimal(_lastPosition.Longitude);
                            }
                            else
                            {
                                scan.Longitude = null;
                                scan.Latitude = null;
                            }
                            scan.SerialNumber = eventData.SerialNumber;
                            scan.ID = Guid.NewGuid();
                            scansToAdd.Add(scan);
                        }
                        addModules(scansToAdd);
                        persistScanCounts();
                        GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<LoadsChangedMessage>(new LoadsChangedMessage());
                    }
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
                    ConnectionMessage = _connectedReader.DisplayName;
                }
                else
                {
                    ConnectionMessage = "Not connected.";
                }
            }
            else if (e.State == ScannerState.Connecting)
            {
                ConnectionMessage = "Connecting...";
            }
            else if (e.State == ScannerState.Disconnecting)
            {
                ConnectionMessage = "Disconnecting...";
            }
            else if (e.State == ScannerState.Disconnnected)
            {
                ConnectionMessage = "Not connected.";
            }
        }
        #endregion

        #region Messaging

        private void SaveNotes(NotesChangedMessage msg)
        {
            //find load with id
            var load = _dataService.GetByID<Load>(msg.ID);
            load.Notes = msg.Notes;
            _dataService.Save<Load>(load);
        }

        private void HandleReviewCancel(ReviewCancelMessage msg)
        {
            IsReviewMode = false;
        }

        #endregion
    }
}
