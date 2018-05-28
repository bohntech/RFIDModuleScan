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
using GalaSoft.MvvmLight.Views;
using RFIDModuleScan.Core.Data;
using RFIDModuleScan.Core.Enums;
using RFIDModuleScan.Core.Messages;

namespace RFIDModuleScan.Core.ViewModels
{    

    public class SettingsViewModel : ViewModelBase, IDisposable
    {
        private INavigationService _navigationService;

        private ModuleDataService _db = new ModuleDataService();

        private ScanSettingsViewModel _scanSettings;

        public ScanSettingsViewModel ScanSettings
        {
            get
            {
                return _scanSettings;
            }
            set
            {
                Set<ScanSettingsViewModel>(() => ScanSettings, ref _scanSettings, value);
            }                
        }

        private string _scannerDisplayName = "Not connected";
        public string ScannerDisplayName
        {
            get
            {
                return _scannerDisplayName;
            }
            private set
            {
                Set<string>(() => ScannerDisplayName, ref _scannerDisplayName, value);
            }
        }

        public ObservableCollection<ScannerModel> AvailableScanners { get; private set; }

        private ScannerModel _selectedScanner;
        public ScannerModel SelectedScanner
        {
            get
            {
                return _selectedScanner;
            }
            set
            {
                Set<ScannerModel>(() => SelectedScanner, ref _selectedScanner, value);               
            }
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
            private set
            {
                Set<bool>(() => IsConnected, ref _isConnected, value);
            }
        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            private set
            {
                Set<string>(() => ErrorMessage, ref _errorMessage, value);
                ErrorMessageVisible = !string.IsNullOrEmpty(value);
            }
        }


        private bool _errorMessageVisible;
        public bool ErrorMessageVisible
        {
            get
            {
                return !string.IsNullOrEmpty(ErrorMessage);
            }
            private set
            {
                Set<bool>(() => ErrorMessageVisible, ref _errorMessageVisible, value);
            }
        }

        #region Gin Connection Settings
        private bool _isConnectedToGin = false;
        public bool IsConnectedToGin
        {
            get
            {
                return _isConnectedToGin;
            }
            private set
            {
                Set<bool>(() => IsConnectedToGin, ref _isConnectedToGin, value);
            }
        }

        private string _appMode;
        public string AppMode
        {
            get
            {
                return _appMode;
            }
            private set
            {
                Set<string>(() => AppMode, ref _appMode, value);
            }
        }

        private string _ginName;
        public string GinName
        {
            get
            {
                return _ginName;
            }
            private set
            {
                Set<string>(() => GinName, ref _ginName, value);
            }
        }

        private string _ginDBUrl;
        public string GinDBUrl
        {
            get
            {
                return _ginDBUrl;
            }
            private set
            {
                Set<string>(() => GinDBUrl, ref _ginDBUrl, value);
            }
        }

        private string _ginDBKey;
        public string GinDBKey
        {
            get
            {
                return _ginDBKey;
            }
            private set
            {
                Set<string>(() => GinDBKey, ref _ginDBKey, value);
            }
        }

        private string _ginEmail;
        public string GinEmail
        {
            get
            {
                return _ginEmail;
            }
            private set
            {
                Set<string>(() => GinEmail, ref _ginEmail, value);
            }
        }
        #endregion


        private void fetchGinSettings()
        {
            GinName = Configuration.GinName;
            GinDBUrl = Configuration.GinDBUrl;
            GinEmail = Configuration.GinEmail;
            GinDBKey = Configuration.GinDBKey;

            if (!string.IsNullOrWhiteSpace(GinDBUrl))
            {
                AppMode = "Gin mode";
            }
            else
            {
                AppMode = "Standalone mode";
            }

            IsConnectedToGin = !string.IsNullOrWhiteSpace(GinDBUrl);
        }

        public SettingsViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            SaveSettings = new RelayCommand( this.ExecuteSaveSettings, this.CanExecuteSaveSettings);
            RefreshList = new RelayCommand(this.ExecuteRefreshList, this.CanExecuteRefreshList);
            ConnectToSelected = new RelayCommand(this.ExecuteConnectToSelected, this.CanExecuteConnectToSelected);
            Disconnect = new RelayCommand(this.ExecuteDisconnect, this.CanExecuteDisconnect);
            OpenConnections = new RelayCommand(this.ExecuteOpenConnections, this.CanExecuteOpenConnections);

            ConnectToGin = new RelayCommand(this.ExecuteConnectToGin);
            DisconnectFromGin = new RelayCommand(this.ExecuteDisconnectFromGin);

            AvailableScanners = new ObservableCollection<ScannerModel>();

            ScanSettings = new ScanSettingsViewModel();

            ScanSettings.LastConnectedScannerID = Configuration.LastScannerID;
            ScanSettings.LastConnectedScannerName = Configuration.LastScannerDisplayName;
            ScanSettings.MaxModulesPerLoad = Configuration.MaxModulesPerLoad.ToString();
            ScanSettings.TabletID = Configuration.TabletID;

            fetchGinSettings();
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<GinQRCodeScannedMessage>(this, HandleGinQRScanned);            
                                                                        
            ScanSettings.SelectedExportIndex = (int)Configuration.SelectedExportFormat;

            if (ScannerConnectionManager.ScannerContext != null)
            {
                ScannerConnectionManager.ScannerContext.ConnectionStateChanged += ActiveScanner_ConnectionStateChanged;
                ExecuteRefreshList();

                var connectedReader = ScannerConnectionManager.ScannerContext.GetConnectedScanner();

                if (connectedReader != null)
                {
                    ScannerDisplayName = connectedReader.DisplayName;
                }
                else
                {
                    ScannerDisplayName = "Not connected.";
                }
            }            
        }

        public void Dispose()
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Unregister<GinQRCodeScannedMessage>(this);

            if (ScannerConnectionManager.ScannerContext != null)
            {
                ScannerConnectionManager.ScannerContext.ConnectionStateChanged -= ActiveScanner_ConnectionStateChanged;
            }
        }

        private void ActiveScanner_ConnectionStateChanged(object sender, ConnectionStateEventArgs e)
        {
            if (ScannerConnectionManager.ScannerContext != null)
            {
                if (e.State == ScannerState.Connected)
                {
                    var scanner = ScannerConnectionManager.ScannerContext.GetConnectedScanner();
                    if (scanner != null)
                    {
                        ScannerDisplayName = scanner.DisplayName;
                        _db.SaveSetting(AppSettingID.LastScannerName, scanner.DisplayName);
                        _db.SaveSetting(AppSettingID.LastScannerID, scanner.Descriptor);
                    }
                    else
                    {
                        ScannerDisplayName = "Not connected.";
                    }
                }
                else if (e.State == ScannerState.Connecting)
                {
                    ScannerDisplayName = "Connecting...";
                }
                else if (e.State == ScannerState.Disconnecting)
                {
                    ScannerDisplayName = "Disconnecting...";
                }
                else
                {
                    ScannerDisplayName = "Not connected.";
                }
            }
        }

        private void HandleGinQRScanned(GinQRCodeScannedMessage message)
        {
            fetchGinSettings();
        }

        //save command
        public RelayCommand SaveSettings
        {
            get;
            private set;
        }

        private void ExecuteSaveSettings()
        {
            int temp = 0;

            if (int.TryParse(ScanSettings.MaxModulesPerLoad, out temp) && !string.IsNullOrWhiteSpace(ScanSettings.MaxModulesPerLoad) &&  !string.IsNullOrEmpty(ScanSettings.TabletID)) {
                _db.SaveSetting(AppSettingID.MaxModulesPerLoad, temp.ToString());
                _db.SaveSetting(AppSettingID.TabletID, ScanSettings.TabletID);
            }

            _db.SaveSetting(AppSettingID.ExportFormatID, (ScanSettings.SelectedExportIndex).ToString());
        }

        private bool CanExecuteSaveSettings()
        {
            return true;
        }

        #region Connect to Gin Commands
        public RelayCommand ConnectToGin
        {
            get;
            private set;
        }

        public RelayCommand DisconnectFromGin
        {
            get;
            private set;
        }

        private void ExecuteConnectToGin()
        {
            _navigationService.NavigateTo(ViewLocator.GinConnectPage);
        }

        private void ExecuteDisconnectFromGin()
        {
            _db.SaveSetting(AppSettingID.GinDBKey, string.Empty);
            _db.SaveSetting(AppSettingID.GinDBUrl, string.Empty);
            _db.SaveSetting(AppSettingID.GinEmail, string.Empty);
            _db.SaveSetting(AppSettingID.GinName, string.Empty);

            IsConnectedToGin = false;
            GinName = string.Empty;
            GinEmail = string.Empty;
            GinDBUrl = string.Empty;
            GinDBKey = string.Empty;
            AppMode = "Standalone mode";
        }
        #endregion


        //refresh command
        #region RefreshCommand
        public RelayCommand RefreshList
        {
            get;
            private set;
        }
        

     

        private async void ExecuteRefreshList()
        {
            ErrorMessage = "";
            var results = await ScannerConnectionManager.ScannerContext.GetAvailableScannersAsync();
            AvailableScanners.Clear();
            foreach(var result in results)
            {
                AvailableScanners.Add(result);
            }
        }

        private bool CanExecuteRefreshList()
        {
            return true;
        }
        #endregion RefreshCommand

        //connect command
        public RelayCommand ConnectToSelected
        {
            get;
            private set;
        }

        private async void ExecuteConnectToSelected()
        {
            ErrorMessage = "";
            if (SelectedScanner != null)
            {
                try
                {
                    
                    await ScannerConnectionManager.ScannerContext.ConnectToScannerAsync(SelectedScanner.DisplayName, SelectedScanner.Descriptor);
                }
                catch(Exception exc)
                {                    
                    ErrorMessage = "Unable to connect.  Verify the scanner is turned on.";
                }
            }
            else
            {
                ErrorMessage = "No scanner selected.";
            }
        }

        private bool CanExecuteConnectToSelected()
        {
            return true;
        }
        
        //disconnect command
        public RelayCommand Disconnect
        {
            get;
            private set;
        }

        private void ExecuteDisconnect()
        {
            ErrorMessage = "";
            try
            {
                ScannerConnectionManager.ScannerContext.Disconnect();
            }
            catch(Exception exc)
            {
                ErrorMessage = "An error occurred disconnecting from scanner.";
            }
        }

        private bool CanExecuteDisconnect()
        {
            return true;
        }

        public RelayCommand OpenConnections
        {
            get;
            private set;
        }

        private void ExecuteOpenConnections()
        {
            ErrorMessage = "";
            try
            {
                ScannerConnectionManager.ScannerContext.OpenConnectionUI();
            }
            catch (Exception exc)
            {
                ErrorMessage = "Unable to open bluetooth settings";
            }
        }

        private bool CanExecuteOpenConnections()
        {
            return true;
        }
    }
}
