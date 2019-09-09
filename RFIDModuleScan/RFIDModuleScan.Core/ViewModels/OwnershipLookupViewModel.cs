//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RFIDModuleScan.Core.Scanners;
using RFIDModuleScan.Core.Data;
using RFIDModuleScan.Core.Services;
using RFIDModuleScan.Core.Enums;
using RFIDModuleScan.Core.Helpers;

namespace RFIDModuleScan.Core.ViewModels
{
    public class OwnershipLookupViewModel : ViewModelBase, IDisposable
    {
        protected INavigationService _navigationService;
        protected IModuleDataService _dataService;

        private bool _isBusy;
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            private set
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

        private string _serialNumber;
        public string SerialNumber
        {
            get
            {
                return _serialNumber;
            }
            set
            {
                Set<string>(() => SerialNumber, ref _serialNumber, value);
            }
        }

        private string _Client;
        public string Client
        {
            get
            {
                return _Client;
            }
            private set
            {
                Set<string>(() => Client, ref _Client, value);
            }
        }

        private string _Farm;
        public string Farm
        {
            get
            {
                return _Farm;
            }
            private set
            {
                Set<string>(() => Farm, ref _Farm, value);
            }
        }

        private string _Field;
        public string Field
        {
            get
            {
                return _Field;
            }
            private set
            {
                Set<string>(() => Field, ref _Field, value);
            }
        }

        private string _TruckLoadNumber;
        public string TruckLoadNumber
        {
            get
            {
                return _TruckLoadNumber;
            }
            private set
            {
                Set<string>(() => TruckLoadNumber, ref _TruckLoadNumber, value);
            }
        }

        private string _BridgeLoadNumber;
        public string BridgeLoadNumber
        {
            get
            {
                return _BridgeLoadNumber;
            }
            private set
            {
                Set<string>(() => BridgeLoadNumber, ref _BridgeLoadNumber, value);
            }
        }

        private string _GinTicketLoadNumber;
        public string GinTicketLoadNumber
        {
            get
            {
                return _GinTicketLoadNumber;
            }
            private set
            {
                Set<string>(() => GinTicketLoadNumber, ref _GinTicketLoadNumber, value);
            }
        }

        private string _Status;
        public string Status
        {
            get
            {
                return _Status;
            }
            private set
            {
                Set<string>(() => Status, ref _Status, value);
            }
        }

        private string _Variety;
        public string Variety
        {
            get
            {
                return _Variety;
            }
            private set
            {
                Set<string>(() => Variety, ref _Variety, value);
            }
        }

        private bool _ShowResults;
        public bool ShowResults
        {
            get
            {
                return _ShowResults;
            }
            private set
            {
                Set<bool>(() => ShowResults, ref _ShowResults, value);
            }
        }

        private bool _NoMatchFound;
        public bool NoMatchFound
        {
            get
            {
                return _NoMatchFound;
            }
            private set
            {
                Set<bool>(() => NoMatchFound, ref _NoMatchFound, value);
            }
        }

        public OwnershipLookupViewModel(INavigationService navigationService, IModuleDataService dataService)
        {
            _navigationService = navigationService;
            _dataService = dataService;
            IsBusy = false;
            ShowResults = false;
            SearchCommand = new RelayCommand(this.ExecuteSearchCommand);
            OpenCameraCommand = new RelayCommand(this.ExecuteOpenCameraCommand);
            BusyMessage = "Loading...";
        }

        
        public void Initialize()
        {
            if (ScannerConnectionManager.ScannerContext != null)
            {
                ScannerConnectionManager.ScannerContext.ItemScanned += ScannerContext_ItemScanned; ;
            }

            Task.Run(() =>
            {                
                ShowResults = false;
                IsBusy = false;
            });
        }

        private void ScannerContext_ItemScanned(object sender, ItemScannedEventArgs e)
        {
            if (!e.EventData.IsFlush)
            {
                SerialNumber = e.EventData.SerialNumber;
                ExecuteSearchCommand();
            }
        }

        public void AddOpticalScan(string rawScanData, out string serialNumber)
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
            _navigationService.GoBack();
        }

        public void Dispose()
        {
            ScannerConnectionManager.ScannerContext.ItemScanned -= ScannerContext_ItemScanned;
        }

        public RelayCommand SearchCommand { get; private set; }

        private void ExecuteSearchCommand()
        {
            IsBusy = true;
            BusyMessage = "Searching";
            Task.Run(() =>
            {
                _dataService.SyncOwnership();


                var results = _dataService.FindObjects<ModuleOwnership, string>(m => m.Name == SerialNumber, m => m.Name).OrderByDescending(t => t.LastCreatedOrUpdated);
                var result = results.FirstOrDefault();
                if (result == null)
                {
                    NoMatchFound = true;
                    ShowResults = false;
                }
                else
                {
                    NoMatchFound = false;
                    ShowResults = true;

                    Client = result.Client;
                    Farm = result.Farm;
                    Field = result.Field;
                    TruckLoadNumber = result.TruckLoadNumber;
                    BridgeLoadNumber =  (result.BridgeLoadNumber > 0) ? result.BridgeLoadNumber.ToString() : "";
                    GinTicketLoadNumber = result.GinTagLoadNumber;
                    Variety = result.Variety;
                    Status = result.Status.Replace("_", " ");
                }

                IsBusy = false;
            });
        }

        public RelayCommand OpenCameraCommand { get; private set; }

        private void ExecuteOpenCameraCommand()
        {
            _navigationService.NavigateTo(ViewLocator.OpticalOwnershipLookupPage, this);
        }
    }
}
