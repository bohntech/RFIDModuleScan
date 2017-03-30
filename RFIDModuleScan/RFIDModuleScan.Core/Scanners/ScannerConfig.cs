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
    public static class ScannerConnectionManager
    {
        private static IScannerContext _context = null;
        public static IScannerContext ScannerContext
        {
            get
            {
                return _context;
            }
            private set
            {
                _context = value;
            }
        }

        public static bool Initialized { get; set; }

        /// <summary>
        /// This method could be extended to support new scanners
        /// </summary>
        /// <param name="scannerType"></param>
        /// <returns></returns>
        public static void InitializeContext(ScannerType scannerType)
        {
            //if connected to another scanner disconnect/clean up should go here
            //at this time only one scanner is supported

            Initialized = true;

            if (scannerType == ScannerType.TSL)
            {
                if (_context == null)
                {
                    _context = new TSLScannerContext();
                }               
                else
                {
                    _context = null;
                }
            }
            else
            {
                _context = null;
            }
        }
                

        public static void SaveLastScanner()
        {

        }

        public static void RestoreLastScanner()
        {

        }
    }
    
}
