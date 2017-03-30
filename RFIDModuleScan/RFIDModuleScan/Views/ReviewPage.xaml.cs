//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using RFIDModuleScan.Core.ViewModels;
using RFIDModuleScan.Core.Data;
using Xamarin.Forms;
using GalaSoft.MvvmLight.Views;
using RFIDModuleScan.Converters;
using System.Collections.ObjectModel;
using RFIDModuleScan.Layouts;
using System.Threading;
using RFIDModuleScan.Core.Messages;
using XLabs.Forms.Controls;
using Xamarin.Forms.Xaml;

namespace RFIDModuleScan.Views
{    
    public partial class ReviewPage : ContentPage, IDisposable
    {
        ReviewPageViewModel vm = null;
        INavigationService _navService = null;

        #region Private methods
        private void init(Guid? id = null)
        {
            InitializeComponent();

            _navService = SimpleIoc.Default.GetInstance<INavigationService>();

            ToolbarItems.Add(new ToolbarItem("Settings", "gear.png", () =>
            {
                _navService.NavigateTo(ViewLocator.SettingsPage);
            }));

            vm = new ReviewPageViewModel(_navService, SimpleIoc.Default.GetInstance<IModuleDataService>(), id);                       
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<ModuleScanViewModel>(this, HandleSearchScan);

            this.BindingContext = vm;

            Task.Run(() => {                
                vm.Initialize();
                vm.IsBusy = true;
                loadsList.BindToVM(vm.Loads);
                vm.IsBusy = false;
            });
        }       

        #endregion

        public void Dispose()
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<ReviewCancelMessage>(new ReviewCancelMessage());
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Unregister<ModuleScanViewModel>(this);
            vm.Cleanup();
            vm.Dispose();
            vm = null;
        }

        public ReviewPage(Guid id)
        {
            init(id);
        }      

        private void HandleSearchScan(ModuleScanViewModel message)
        {
            //show dialog with module info
            Device.BeginInvokeOnMainThread(() => { 
                findDialog.Show(message);
            });
        }       

        #region Events


        #endregion

        private void btnFind_Clicked(object sender, EventArgs e)
        {
            findDialog.Show(null);
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
           
            
        }

        private void ContentPage_Disappearing(object sender, EventArgs e)
        {
           // rootContentView.IsVisible = false;
           // busyLayout.IsVisible = false;
        }
    }
}
