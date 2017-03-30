//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RFIDModuleScan.Core.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using Xamarin.Forms;
using GalaSoft.MvvmLight.Views;
using Xamarin.Forms.Xaml;
using RFIDModuleScan.Core.Data;
using System.IO;

namespace RFIDModuleScan.Views
{  
    public partial class HomePage : ContentPage, IDisposable
    {
        private HomeViewModel vm = null;

        public HomePage()
        {
            InitializeComponent();
            vm = new HomeViewModel(SimpleIoc.Default.GetInstance<INavigationService>(), SimpleIoc.Default.GetInstance<IModuleDataService>());
            this.BindingContext = vm;
            Device.StartTimer(new TimeSpan(0, 0, 2), initiateConnection);
                        
        }
       
        public void Dispose()
        {
            vm.Dispose();
        }

        private bool initiateConnection()
        {           
            vm.Initialize();           
            return false;
        }

        private bool initiateRefresh()
        {
            vm.Refresh();
            return false;
        }

        private void btnClearData_Clicked(object sender, EventArgs e)
        {
            msgPromptView.Show("Are you sure you want to clear all scan data from this device?", true, true, true);
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 500), initiateRefresh);
        }
    }
}
