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
using System.Threading;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Helpers;
using RFIDModuleScan.Core.Messages;
using RFIDModuleScan.Core.Helpers;
using Xamarin.Forms;

namespace RFIDModuleScan.Core.ViewModels
{
    public class AllScansPageViewModel : ViewModelBase, IDisposable
    {
        private INavigationService _navService;
        private IModuleDataService _dataService;

        private RangeObservableCollection<ScanItemViewModel> _scanItems = null;
        public RangeObservableCollection<ScanItemViewModel> ScanItems
        {
            get
            {
                return _scanItems;
            }
            private set
            {
                Set<RangeObservableCollection<ScanItemViewModel>>(() => ScanItems, ref _scanItems, value);
            }
        }

        private bool _isBusy = false;
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

        public AllScansPageViewModel(INavigationService navService, IModuleDataService dataService)
        {
            IsBusy = true;
            _navService = navService;
            _dataService = dataService;            
            OpenScanPage = new RelayCommand<Guid>(this.ExecutOpenScanPage);

            
            BusyMessage = "Loading...";
        }

        public void Initialize()
        {

            if (Configuration.ConnectedToGin)
            {
                //_dataService.SyncRemoteLists(); //sync client/farm/field lists
            }
            else
            {
                _dataService.CleanUpLists(); //clean up client/farms/fields no longer used or referenced on a scan
            }

            var items = _dataService.Find<FieldScan, DateTime>(x => x.ListTypeID == (int)ListTypeEnum.Staging, s => s.Created);

            //ScanItems.Clear();
            //ScanItems = new ObservableCollection<ScanItemViewModel>();
            List<ScanItemViewModel> scans = new List<ScanItemViewModel>();
            DateTime lastScanTime = DateTime.Now;
            string TransmitMsg = "";
            DateTime transmitTime = DateTime.Now;
            foreach (var item in items)
            {                
                if (item.Transmitted != null)
                {                    
                    TransmitMsg = item.Transmitted.Value.ToString("MM/dd/yyyy hh:mm tt");
                }
                else
                {
                    TransmitMsg = "";
                }

                ScanItemViewModel vm = new ScanItemViewModel
                {
                    Farm = item.Farm,
                    Grower = item.Grower,
                    Field = item.Field,
                    Loads = item.LoadCount,
                    Modules = item.ModuleCount,
                    TransmitMsg = TransmitMsg,
                    LastScan = (item.LastScan.HasValue) ? item.LastScan.Value : DateTime.Now,
                    FieldScanID = item.ID
                };

                scans.Add(vm);
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                ScanItems = new RangeObservableCollection<ScanItemViewModel>();
                foreach (var s in scans) ScanItems.AddWithoutNotify(s);
                ScanItems.ApplyUpdates();
            });

            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<AllScansLoadComplete>(new AllScansLoadComplete());

            IsBusy = false;
        }

        public void Dispose()
        {
            this.Cleanup();
        }      

        public RelayCommand<Guid> OpenScanPage { get; private set; }

        private void ExecutOpenScanPage(Guid id)
        {            
            _navService.NavigateTo(ViewLocator.ScanPage, id);
        }
    }
}
