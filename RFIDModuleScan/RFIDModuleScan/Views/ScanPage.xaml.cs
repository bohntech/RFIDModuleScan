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
    public partial class ScanPage : ContentPage, IDisposable
    {
        private static object locker = new object();

        ScanPageViewModel vm = null;

        #region Private methods
        private void init(Guid? id = null)
        {
            InitializeComponent();
            
            INavigationService _navService = SimpleIoc.Default.GetInstance<INavigationService>();

            if (id.HasValue)
            {
                Title = "Continue Load Scan";
            }
            else
            {
                Title = "New Load Scan";
            }

            ToolbarItems.Add(new ToolbarItem("Settings", "gear.png", () =>
            {
                _navService.NavigateTo(ViewLocator.SettingsPage);
            }));
            
            vm = new ScanPageViewModel(_navService, SimpleIoc.Default.GetInstance<IModuleDataService>(), id);
           
            //GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<LoadsChangedMessage>(this, HandleLoadsChangedMessage);

            this.BindingContext = vm;
            loadList.IsMultiLoadList = true;

            Task.Run(() => {                
                vm.Initialize();
                vm.IsBusy = true;
                
                loadList.InitialBind(vm.Loads);
                vm.IsBusy = false;
            });
        }

        private void HandleLoadsChangedMessage(LoadsChangedMessage param)
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 200), scrollCallback);
        }

        private bool scrollCallback()
        {
            Device.BeginInvokeOnMainThread(() => { 
                moduleScrollView.ScrollToAsync(0.0, loadList.Height, false);
            });

            return false;
        }

        #endregion



        public void Dispose()
        {         
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Unregister<LoadsChangedMessage>(this);
                vm.Cleanup();
                vm.Dispose();           
        }
             
        public ScanPage(Guid id)
        {
            init(id);
        }

        public ScanPage()
        {
            init();
        }

        #region Events
        private void btnDelete_Clicked(object sender, EventArgs e)
        {
            int count = 0;
            lock (vm.Loads) {
                foreach (var l in vm.Loads)
                {
                    count += l.Modules.Count(x => x.Selected);
                }
            }

            if (count > 0)
            {
                msgPromptView.Show(string.Format("Are you sure you want to delete {0} modules?", count));
            }
            else
            {
                msgPromptView.Show("No modules selected.", true, false, false);
            }
        }
               
        private void btnMove_Clicked(object sender, EventArgs e)
        {
            int count = 0;
            lock (vm.Loads)
            {
                foreach (var l in vm.Loads)
                {
                    count += l.Modules.Count(x => x.Selected);
                }
            }

            if (vm.Loads.Count() == 0)
            {
                msgPromptView.Show("No loads created yet. Create a load by scanning a module.", true, false, false);
            }
            else if (count > 0)
            {
                moveModulesView.Show();
            }
            else
            {
                msgPromptView.Show("No modules selected.", true, false, false);
            }
        }

        private void btnDeleteScan_Clicked(object sender, EventArgs e)
        {
            deleteAllPrompt.Show("Are you sure you want to delete this entire scan including all loads and serial numbers?");
        }

        private void btnRenumber_Clicked(object sender, EventArgs e)
        {
            renumberPrompt.Show("Are you sure you want to renumber all loads?  This will delete empty loads and renumber all loads.");
        }

        #endregion

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
           
        }

      

        private void ContentPage_Disappearing(object sender, EventArgs e)
        {
                
        }

        private void ContentPage_Focused(object sender, FocusEventArgs e)
        {

        }

        
    }
}
