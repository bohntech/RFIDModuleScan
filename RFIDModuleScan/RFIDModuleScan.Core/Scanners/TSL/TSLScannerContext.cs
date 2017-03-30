//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RFIDModuleScan.Core.Models;
using RFIDModuleScan.Core.Enums;
using TechnologySolutions.Rfid.AsciiProtocol;
using TechnologySolutions.Rfid.AsciiProtocol.Extensions;


namespace RFIDModuleScan.Core.Scanners
{
    public class TSLScannerContext : IScannerContext
    {
        private IReaderConnectionManager _connManager = null;
        private INamedReader _connectedReader = null;

        private AsciiCommander _commander = null;


        public ScannerType GetScannerType() { return ScannerType.TSL; }

        //private ScannerModel _connectedScanner;

        public ScannerModel GetConnectedScanner()
        {
            if (_connManager != null && _connManager.ConnectedReader != null)
            {
                return new Models.ScannerModel { Descriptor = _connectedReader.DisplayInfoLine, DisplayName = _connectedReader.DisplayName, Type = ScannerType.TSL };
            }
            return null;
        }

        public void OpenConnectionUI()
        {
            if (_connManager != null)
            {
                _connManager.AddNewReader();
            }
        }

        public async Task<IEnumerable<ScannerModel>> GetAvailableScannersAsync()
        {
            if (_connManager != null)
            {
                var results = await _connManager.ListAvailableReadersAsync();

                List<ScannerModel> newList = new List<ScannerModel>();
                foreach (var r in results)
                {
                    newList.Add(new ScannerModel { Descriptor = r.DisplayInfoLine, DisplayName = r.DisplayName });
                }

                return newList;
            }
            else
            {
                return new List<ScannerModel>();
            }
        }

        public async Task ConnectToScannerAsync(string name, string descriptor)
        {
            if (_connManager != null && _connManager.ConnectedReader == null)
            {
                var scanners = await _connManager.ListAvailableReadersAsync();

                var desiredScanner = scanners.SingleOrDefault(s => s.DisplayName == name && s.DisplayInfoLine == descriptor);

                await _connManager.ConnectAsync(desiredScanner);
                _connectedReader = desiredScanner;        
                
                if (_commander != null)
                {
                    _commander.Dispose();
                }

                _commander = new AsciiCommander();
                _commander.Transport = _connManager.ConnectionTransport;                  
                //_commander.AddSynchronousResponder();
                _commander.AddResponder(new ScanResponder(ProcessScanLine));
            }
        }

        private void ProcessScanLine(ScanEventData data)
        {
            OnItemScanned(new ItemScannedEventArgs { EventData=data });
        }

        public void Disconnect()
        {
            if (_connManager != null && _connManager.ConnectedReader != null)
            {
                _connManager.Disconnect();
            }
        }

        public TSLScannerContext()
        {
            _connManager = new ReaderConnectionManager();
            _connManager.ConnectionStateChanged += _connManager_ConnectionStateChanged;
        }

        private void _connManager_ConnectionStateChanged(object sender, EventArgs e)
        {
            if (_connManager != null)
            {
                if (_connManager.ConnectionState == ReaderConnectionState.Connected)
                {
                    _connectedReader = _connManager.ConnectedReader;
                    OnConnectionStateChanged(new ConnectionStateEventArgs { State = ScannerState.Connected });
                }
                else if (_connManager.ConnectionState == ReaderConnectionState.Connecting)
                {
                    OnConnectionStateChanged(new ConnectionStateEventArgs { State = ScannerState.Connecting });
                }
                else if (_connManager.ConnectionState == ReaderConnectionState.Disconnected)
                {
                    OnConnectionStateChanged(new ConnectionStateEventArgs { State = ScannerState.Disconnnected });
                }
                else if (_connManager.ConnectionState == ReaderConnectionState.Disconnecting)
                {
                    OnConnectionStateChanged(new ConnectionStateEventArgs { State = ScannerState.Disconnecting });
                }
            }
        }

        public event EventHandler<ConnectionStateEventArgs> ConnectionStateChanged;
        protected virtual void OnConnectionStateChanged(ConnectionStateEventArgs e)
        {
            if (ConnectionStateChanged != null)
            {
                ConnectionStateChanged(this, e);
            }
        }

        public event EventHandler<ItemScannedEventArgs> ItemScanned;
        protected virtual void OnItemScanned(ItemScannedEventArgs e)
        {
            if (ItemScanned != null)
            {
                ItemScanned(this, e);
            }
        }
    }
}
