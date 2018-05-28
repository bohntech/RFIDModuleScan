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
using RFIDModuleScan.Core.Enums;
using RFIDModuleScan.Core.Messages;
using RFIDModuleScan.Core.Services;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace RFIDModuleScan.Core.ViewModels
{
    public class GinConnectPageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        private IModuleDataService _dataService;

        #region Constructor and Cleanup
        public GinConnectPageViewModel(INavigationService navigationService, IModuleDataService dataService)
        {
            _navigationService = navigationService;
            _dataService = dataService;
        }

        public void Dispose()
        {           
            this.Cleanup();
        }

        public void Initialize()
        {
           
        }

        #endregion

        private GinConnectionModel parseFromText(string codeText)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    var writer = new StreamWriter(stream);
                    writer.Write(codeText);
                    writer.Flush();
                    stream.Position = 0;
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(GinConnectionModel));
                    GinConnectionModel m = (GinConnectionModel)ser.ReadObject(stream);
                    return m;
                }
            }
            catch(Exception exc)
            {
                return null;
            }
        }

        public bool IsValidConnectionCode(string codeText)
        {
            var m = parseFromText(codeText);
                        
            if (m != null && !string.IsNullOrEmpty(m.Url) && !string.IsNullOrEmpty(m.Key) && !string.IsNullOrEmpty(m.Gin) && !string.IsNullOrEmpty(m.Email))
            {
                return true;
            }
            else return false;
        }

        public void SaveConnectionInfo(string codeText)
        {
            var m = parseFromText(codeText);

            if (m != null)
            {
                _dataService.SaveSetting(AppSettingID.GinDBKey, m.Key);
                _dataService.SaveSetting(AppSettingID.GinDBUrl, m.Url);
                _dataService.SaveSetting(AppSettingID.GinEmail, m.Email);
                _dataService.SaveSetting(AppSettingID.GinName, m.Gin);
            }

            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<GinQRCodeScannedMessage>(new GinQRCodeScannedMessage());

            _navigationService.GoBack();
        }
    }
}
