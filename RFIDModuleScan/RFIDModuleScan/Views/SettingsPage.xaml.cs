//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RFIDModuleScan.Core.ViewModels;
using Xamarin.Forms;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Xamarin.Forms.Xaml;

namespace RFIDModuleScan.Views
{    
    public partial class SettingsPage : ContentPage, IDisposable
    {
        private SettingsViewModel vm = null;

        public SettingsPage()
        {
            InitializeComponent();

            vm = new SettingsViewModel(SimpleIoc.Default.GetInstance<INavigationService>());
            this.BindingContext = vm;
        }

        public void Dispose()
        {
            vm.Cleanup();
        }
    }
}
