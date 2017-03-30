using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using RFIDModuleScan.ViewModels;
using TechnologySolutions.Rfid.AsciiProtocol;
using TechnologySolutions.Rfid.AsciiProtocol.Extensions;
using RFIDModuleScan.Helpers;
using System.ComponentModel;

namespace RFIDModuleScan.ViewModels
{
    public class SettingsViewModel
    {                           
        public string ConnectedScanner {
            get {
                if (ConnectionManager.ConnectedReader != null)
                {
                    return ConnectionManager.ConnectedReader.DisplayName;
                }
                else
                {
                    return "Not connected";
                }
            }
        }

        public string SelectedScanner { get; set; }

        public string MaxModulesPerLoad { get; set; }

        public string TabletID { get; set; }

        public NotifyTaskCompletion<IEnumerable<INamedReader>> AvailableScanners { get; private set; }        

        public SettingsViewModel()
        {            
            MaxModulesPerLoad = "4";
            TabletID = "tablet-1";
                    
            //AvailableScanners = new NotifyTaskCompletion<IEnumerable<INamedReader>>(LoadScannerData());
                        
        }           

        private async Task<IEnumerable<INamedReader>> LoadScannerData()
        {
            return await ConnectionManager.ReaderConnectionManager.ListAvailableReadersAsync();
        }
        
    }
}
