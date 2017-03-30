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

        public SettingsViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            SaveSettings = new RelayCommand( this.ExecuteSaveSettings, this.CanExecuteSaveSettings);
            RefreshList = new RelayCommand(this.ExecuteRefreshList, this.CanExecuteRefreshList);
            ConnectToSelected = new RelayCommand(this.ExecuteConnectToSelected, this.CanExecuteConnectToSelected);
            Disconnect = new RelayCommand(this.ExecuteDisconnect, this.CanExecuteDisconnect);
            OpenConnections = new RelayCommand(this.ExecuteOpenConnections, this.CanExecuteOpenConnections);

            AvailableScanners = new ObservableCollection<ScannerModel>();

            ScanSettings = new ScanSettingsViewModel();

            ScanSettings.LastConnectedScannerID = Configuration.LastScannerID;
            ScanSettings.LastConnectedScannerName = Configuration.LastScannerDisplayName;
            ScanSettings.MaxModulesPerLoad = Configuration.MaxModulesPerLoad.ToString();
            ScanSettings.TabletID = Configuration.TabletID;
            
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
        }

        private bool CanExecuteSaveSettings()
        {
            return true;
        }


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
