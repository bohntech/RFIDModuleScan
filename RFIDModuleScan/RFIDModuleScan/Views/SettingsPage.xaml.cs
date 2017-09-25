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
using RFIDModuleScan.Core.Data;


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
            exportFormatPicker.Items.Add("Plain CSV");
            exportFormatPicker.Items.Add("Harvest ID CSV");
            exportFormatPicker.SelectedIndex = (int)RFIDModuleScan.Core.Configuration.SelectedExportFormat;
        }

        public void Dispose()
        {
            vm.Cleanup();
        }

        private void exportFormatPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            vm.ScanSettings.SelectedExportIndex = exportFormatPicker.SelectedIndex;
        }
    }
}
