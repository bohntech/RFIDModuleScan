//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using RFIDModuleScan.Core.Enums;

namespace RFIDModuleScan.Core.Models
{
    public class ScannerModel : ObservableObject
    {
        private string _displayName = "";
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                Set<string>(() => DisplayName, ref _displayName, value);
            }
        }

        private string _descriptor = "";
        public string Descriptor
        {
            get
            {
                return _descriptor;
            }
            set
            {
                Set<string>(() => Descriptor, ref _descriptor, value);
            }
        }

        public ScannerType Type { get; set; }
    }
}
