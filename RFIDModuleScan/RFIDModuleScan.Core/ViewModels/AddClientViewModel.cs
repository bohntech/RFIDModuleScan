﻿//Licensed under MIT License see LICENSE.TXT in project root folder
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

namespace RFIDModuleScan.Core.ViewModels
{
    public class AddClientViewModel : ViewModelBase
    {
        private IModuleDataService _dataService;

        private string _client;
        public string Client
        {
            get
            {
                return _client;
            }
            set
            {
                Set<string>(() => Client, ref _client, value);
            }
        }       

        private string _errorMessage;
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                Set<string>(() => ErrorMessage, ref _errorMessage, value);
            }
        }

        private bool _isValid;
        public bool IsValid
        {
            get
            {
                return _isValid;
            }
            set
            {
                Set<bool>(() => IsValid, ref _isValid, value);
            }
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(_client))
            {
                ErrorMessage = "required";
                IsValid = false;
            }
            else if (_dataService.ClientNameExists(Client))
            {
                ErrorMessage = "Client already exists.";
                IsValid = false;
            }
            else
            {
                IsValid = true;
            }
        }

        public RelayCommand<string> AddCommand { get; private set; }

        private void ExecuteAddCommand(string clientName)
        {
            _dataService.AddClient(_client);
        }

        public AddClientViewModel(IModuleDataService dataService)
        {
            _dataService = dataService;
            AddCommand = new RelayCommand<string>(this.ExecuteAddCommand);
        }
    }
}
