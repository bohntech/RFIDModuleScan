//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using RFIDModuleScan.Core.Enums;

namespace RFIDModuleScan.Core.ViewModels
{
    public class ModuleScanViewModel : ObservableObject
    {       

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
            }
        }


        private Guid _loadID;
        public Guid LoadID
        {
            get
            {
                return _loadID;
            }
            set
            {
                Set<Guid>(() => LoadID, ref _loadID, value);
            }
        }

        private int _loadNumber;
        public int LoadNumber
        {
            get
            {
                return _loadNumber;
            }
            set
            {
                Set<int>(() => LoadNumber, ref _loadNumber, value);
            }
        }


        private string _serialNumber = "";
        public string SerialNumber
        {
            get
            {
                return _serialNumber;
            }
            set
            {
                Set<string>(() => SerialNumber, ref _serialNumber, value);

                SerialNumberWithMessage = (NoLocation) ? SerialNumber + " " + "\nNO GPS" : SerialNumber;
            }
        }

        private string _moduleID = "";
        public string ModuleID
        {
            get
            {
                return _moduleID;
            }
            set
            {
                Set<string>(() => ModuleID, ref _moduleID, value);                
            }
        }


        private string _serialNumberWithMessage = "";
        public string SerialNumberWithMessage
        {
            get
            {
                return _serialNumberWithMessage;
            }
            set
            {
                Set<string>(() => SerialNumberWithMessage, ref _serialNumberWithMessage, value);
            }
        }

        private DateTime _timeStamp = DateTime.Now;
        public DateTime TimeStamp
        {
            get
            {
                return _timeStamp;
            }
            set
            {
                Set<DateTime>(() => TimeStamp, ref _timeStamp, value);
            }
        }

        private BarCodeTypeEnum _moduleType;
        public BarCodeTypeEnum ModuleType
        {
            get
            {
                return _moduleType;
            }
            set
            {
                Set<BarCodeTypeEnum>(() => ModuleType, ref _moduleType, value);
            }
        }

        private decimal? _lat = null;
        public decimal? Latitude
        {
            get
            {
                return _lat;
            }
            set
            {
                Set<decimal?>(() => Latitude, ref _lat, value);
                NoLocation = (!_long.HasValue || !_lat.HasValue || _long.Value == 0.000M || _lat.Value == 0.00M);
            }
        }

        private decimal? _long = null;
        public decimal? Longitude
        {
            get
            {
                return _long;
            }
            set
            {
                Set<decimal?>(() => Longitude, ref _long, value);
                NoLocation = (!_long.HasValue || !_lat.HasValue || _long.Value == 0.000M || _lat.Value == 0.00M);
            }
        }

        private bool _selected = false;
        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                Set<bool>(() => Selected, ref _selected, value);                
            }
        }

        private bool _noLocation;
        public bool NoLocation
        {
            get
            {
                return _noLocation;
            }
            set
            {
                Set<bool>(() => NoLocation, ref _noLocation, value);
                SerialNumberWithMessage = (value) ? SerialNumber + " " + " (NO GPS)" : SerialNumber + "         ";
            }
        }
    }
}
