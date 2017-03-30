//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace RFIDModuleScan.Core.ViewModels
{
    public class ScanItemViewModel : ObservableObject
    {
        private string _grower;
        public string Grower
        {
            get
            {
                return _grower;
            }
            set
            {
                Set<string>(() => Grower, ref _grower, value);
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
                Set<string>(() => Farm, ref _farm, value);
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
                Set<string>(() => Field, ref _field, value);
            }
        }

        private int _loads;
        public int Loads
        {
            get
            {
                return _loads;
            }
            set
            {
                Set<int>(() => Loads, ref _loads, value);
            }
        }

        private int _modules;
        public int Modules
        {
            get
            {
                return _modules;
            }
            set
            {
                Set<int>(() => Modules, ref _modules, value);
            }
        }

        private DateTime _lastScan;
        public DateTime LastScan
        {
            get
            {
                return _lastScan;
            }
            set
            {
                Set<DateTime>(() => LastScan, ref _lastScan, value);
            }
        }

        private string _transmitMsg;
        public string TransmitMsg
        {
            get
            {
                return _transmitMsg;
            }
            set
            {
                Set<string>(() => TransmitMsg, ref _transmitMsg, value);
            }
        }

        private Guid _fieldScanID;
        public Guid FieldScanID
        {
            get
            {
                return _fieldScanID;
            }
            set
            {
                Set<Guid>(() => FieldScanID, ref _fieldScanID, value);
            }
        }

    }
}
