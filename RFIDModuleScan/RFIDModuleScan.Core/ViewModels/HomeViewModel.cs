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
    public class HomeViewModel : ViewModelBase, IDisposable
    {
        protected INavigationService _navigationService;
        protected IModuleDataService _dataService;
        private IEmailService _emailService;

        private bool _hasData;
        public bool HasData
        {
            get
            {
                return _hasData;
            }
            private set
            {
                Set<bool>(() => HasData, ref _hasData, value);
            }
        }

        private bool _connectedToGin;
        public bool ConnectedToGin
        {
            get
            {
                return _connectedToGin;
            }
            private set
            {
                Set<bool>(() => ConnectedToGin, ref _connectedToGin, value);
            }
        }

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

        public HomeViewModel(INavigationService navigationService, IModuleDataService dataService)
        {
            _navigationService = navigationService;
            _dataService = dataService;

            _emailService = Xamarin.Forms.DependencyService.Get<IEmailService>();

            NavigateToAllScansPage = new RelayCommand(this.ExecuteNavigateToAllScansPage, this.CanExecuteNavigateToAllScansPage);
            NavigateToModuleOwnershipLookupPage = new RelayCommand(this.ExecuteNavigateToModuleOwnershipLookupPage);
            NavigateToSettingsPage = new RelayCommand(this.ExecuteNavigateToSettingsPage, this.CanExecuteNavigateToSettingsPage);            
            NavigateToScanPage = new RelayCommand(this.ExecuteNavigateToScanPage, this.CanExecuteNavigateToScanPage);
            NavigateToDropScanPage = new RelayCommand(this.ExecuteNavigateToDropScanPage);
            TransmitAllCommand = new RelayCommand(this.ExecuteTransmitAllCommand);
            DeleteAllCommand = new RelayCommand(this.ExecuteDeleteAllCommand);
            SyncCommand = new RelayCommand(this.ExecuteSyncCommand);
            DumpDatabaseCommand = new RelayCommand(this.ExecuteDumpDatabaseCommand);
            AboutCommand = new RelayCommand(this.ExecuteAboutCommand);

            IsBusy = false;
            HasData = false;
            ConnectedToGin = Configuration.ConnectedToGin;
            BusyMessage = "Loading...";
        }

        public void Refresh()
        {
            HasData = (_dataService.GetAll<FieldScan>().Count() > 0);
            ConnectedToGin = Configuration.ConnectedToGin;
        }

        public void Initialize()
        {
           
            Task.Run(() =>
            {
                BusyMessage = "Connecting...";
                HasData = (_dataService.GetAll<FieldScan>().Count() > 0) || (_dataService.GetAllObjects<ModuleOwnership>().Count() > 0);
                IsBusy = true;              
                if (ScannerConnectionManager.ScannerContext != null)
                {
                    var _connectedReader = ScannerConnectionManager.ScannerContext.GetConnectedScanner();
                    if (_connectedReader == null)
                    {
                        var lastName = Configuration.LastScannerDisplayName;
                        var lastID = Configuration.LastScannerID;

                        if (!string.IsNullOrEmpty(lastName) && !string.IsNullOrEmpty(lastID))
                        {
                            try
                            {
                                ScannerConnectionManager.ScannerContext.ConnectToScannerAsync(lastName, lastID).Wait();
                            }
                            catch (Exception exc) // if there is a error we silently 
                            {
                                string msg = exc.Message;
                            }
                        }
                    }
                }

                if (Configuration.ConnectedToGin)
                {
                    BusyMessage = "Syncing lists";
                    _dataService.SyncRemoteLists(false); //sync client/farm/field lists    

                    BusyMessage = "Updating module ownership.";
                    _dataService.SyncOwnership(); //sync module ownership table 
                }
                else
                {
                    _dataService.CleanUpLists(); //clean up client/farms/fields no longer used or referenced on a scan
                }               

                IsBusy = false;
            });
        }

        public void Dispose()
        {

        }

        #region Navigate To All Scans Command
        public RelayCommand NavigateToAllScansPage {get; private set;}

        private void ExecuteNavigateToAllScansPage()
        {
            _navigationService.NavigateTo(ViewLocator.AllScansPage);
        }

        private bool CanExecuteNavigateToAllScansPage() { return true; }

        public RelayCommand TransmitAllCommand { get; set; }
        private void ExecuteTransmitAllCommand()
        {
            IsBusy = true;
            BusyMessage = "Preparing file...";
            Task.Run(() =>
            {
                List<FieldScan> scans = _dataService.GetAll<FieldScan>().OrderBy(o => o.Created).ToList();

                string file = "";
                string extension = ".csv";

                if (Configuration.SelectedExportFormat == ExportFormatEnum.PlainCSV)
                    file = FileHelper.GetCSVFileString(scans, _dataService);
                else
                {
                    file = FileHelper.GetHIDFileString(scans, _dataService);
                    extension = ".TXT";
                }

                string body = "Please see attached list.";

                

                body += "<RFID_DATA>\r\n" + file + "</RFID_DATA>";

                //IFileService svc = Xamarin.Forms.DependencyService.Get<IFileService>();
                string filename = "AllScans-" + DateTime.Now.ToString("MMddyyyy_hh_mm_ss_tt") + extension;
                //string fullPath = svc.SaveText(filename, file);
                body += "<FILENAME>" + filename + "</FILENAME>";
                List<string> files = new List<string>();
                //files.Add(fullPath);

                string toEmail = Configuration.GinEmail ?? "";
                _emailService.ShowDraft("All Scans", body, false, toEmail, files);

                IsBusy = false;
                BusyMessage = "Loading...";
            });
        }

        public RelayCommand DeleteAllCommand { get; set; }
        private void ExecuteDeleteAllCommand()
        {
            IsBusy = true;
            BusyMessage = "Clearing data...";
            Task.Run(() =>
            {
                _dataService.ClearData();
                IsBusy = false;
                HasData = false;
                BusyMessage = "Loading...";
            });
        }

        public RelayCommand SyncCommand { get; set; }
        private void ExecuteSyncCommand()
        {
            IsBusy = true;
            BusyMessage = "Syncing ownership.";
            Task.Run(() =>
            {
                _dataService.SyncOwnership();

                BusyMessage = "Syncing lists.";
                _dataService.SyncRemoteLists(true);
                IsBusy = false;                
                BusyMessage = "Loading...";
            });
        }

        public RelayCommand DumpDatabaseCommand { get; set; }
        private void ExecuteDumpDatabaseCommand()
        {
            Dictionary<string, string> csvTables = FileHelper.DumpDBTables(_dataService);
            
            IFileService svc = Xamarin.Forms.DependencyService.Get<IFileService>();

            string dateStamp = DateTime.Now.ToString("MMddyyyy_hh_mm_ss_tt");
            foreach (var key in csvTables.Keys)
            {
                string filename = key + "-" + dateStamp + ".csv";
                string fullPath = svc.SaveText(filename, csvTables[key]);
            }
        }
        #endregion     

        #region NavigateToScanPage
        public RelayCommand NavigateToScanPage { get; private set; }

        private void ExecuteNavigateToScanPage()
        {
            _navigationService.NavigateTo(ViewLocator.ScanPage);
        }

        private bool CanExecuteNavigateToScanPage() { return true; }

        public RelayCommand NavigateToDropScanPage { get; private set; }
        private void ExecuteNavigateToDropScanPage()
        {
            _navigationService.NavigateTo(ViewLocator.ScanDropListPage);
        }
        #endregion

        #region NavigateToSettingsPage
        public RelayCommand NavigateToSettingsPage { get; private set; }

        private void ExecuteNavigateToSettingsPage()
        {
            _navigationService.NavigateTo(ViewLocator.SettingsPage);
        }

        private bool CanExecuteNavigateToSettingsPage() { return true; }

        #endregion

        #region About Command
        public RelayCommand AboutCommand { get; private set; }
        private void ExecuteAboutCommand()
        {
            _navigationService.NavigateTo(ViewLocator.AboutPage);
        }
        #endregion

        /*
            #region NavigateToReviewPage        
            public RelayCommand NavigateToReviewPage { get; private set; }

            private void ExecuteNavigateToReviewPage()
            {
                _navigationService.NavigateTo(ViewModelLocator.ReviewPage);
            }

            private bool CanExecuteNavigateToReviewPage() { return true; }
        #endregion
        */

        public RelayCommand NavigateToModuleOwnershipLookupPage { get; private set; }

        private void ExecuteNavigateToModuleOwnershipLookupPage()
        {
            _navigationService.NavigateTo(ViewLocator.OwnershipLookupPage);
        }
    }
}
