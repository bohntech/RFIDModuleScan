using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using RFIDModuleScan.Core.ViewModels;
using RFIDModuleScan.Core.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RFIDModuleScan.Views
{    
    public partial class OwnershipLookupPage : ContentPage, IDisposable
    {
        private OwnershipLookupViewModel vm = null;

        public OwnershipLookupPage()
        {
            InitializeComponent();
            vm = new OwnershipLookupViewModel(SimpleIoc.Default.GetInstance<INavigationService>(), SimpleIoc.Default.GetInstance<IModuleDataService>());
            this.BindingContext = vm;
        }
       
        public void Dispose()
        {
            vm.Dispose();
        }                     

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                vm.Initialize();
            }); 
        }

        private void ContentPage_Appearing_1(object sender, EventArgs e)
        {

        }

        private void ContentPage_Disappearing(object sender, EventArgs e)
        {

        }
    }
}