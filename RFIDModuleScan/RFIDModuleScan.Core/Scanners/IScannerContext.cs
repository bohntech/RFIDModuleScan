//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RFIDModuleScan.Core.Models;
using RFIDModuleScan.Core.Enums;

namespace RFIDModuleScan.Core.Scanners
{
    public interface IScannerContext
    {
        ScannerModel GetConnectedScanner();

        ScannerType GetScannerType();

        Task<IEnumerable<ScannerModel>> GetAvailableScannersAsync();

        Task ConnectToScannerAsync(string displayName, string descriptor);

        void OpenConnectionUI();

        void Disconnect();

        event EventHandler<ConnectionStateEventArgs> ConnectionStateChanged;

        event EventHandler<ItemScannedEventArgs> ItemScanned;
    }
}
