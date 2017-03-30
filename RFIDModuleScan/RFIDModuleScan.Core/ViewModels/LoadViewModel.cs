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


namespace RFIDModuleScan.Core.ViewModels
{
    public class LoadViewModel : ViewModelBase
    {
        public bool IsDirty { get; set; }

        private Guid _id;
        public Guid ID
        {
            get
            {
                return _id;
            }
            set
            {
                Set<Guid>(() => ID, ref _id, value);
                IsDirty = true;
            }
        }

        private int _loadNumber = 1;
        public int LoadNumber
        {
            get
            {
                return _loadNumber;
            }
            set
            {
                Set<int>(() => LoadNumber, ref _loadNumber, value);
                IsDirty = true;
            }
        }

        private int _moduleCount = 1;
        public int ModuleCount
        {
            get
            {
                return _moduleCount;
            }
            set
            {
                Set<int>(() => ModuleCount, ref _moduleCount, value);                
            }
        }

        private bool _isOpen = true;
        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                Set<bool>(() => IsOpen, ref _isOpen, value);
                IsDirty = true;
            }
        }

        private RangeObservableCollection<ModuleScanViewModel> _modules;
        public RangeObservableCollection<ModuleScanViewModel> Modules
        {
            get
            {
                return _modules;
            }
            set
            {
                _modules = value;
                ModuleCount = _modules.Count();
                RaisePropertyChanged("Modules");
            }
        }

        private string _notes = "";
        public string Notes
        {
            get
            {
                return _notes;
            }
            set
            {
                Set<string>(() => Notes, ref _notes, value);
            }
        }

        public LoadViewModel()
        {
            Modules = new RangeObservableCollection<ModuleScanViewModel>();
        }

     

        public void RefreshCount()
        {
            ModuleCount = _modules.Count();
        }

    }
}
