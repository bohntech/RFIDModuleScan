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
    public partial class ScanDropListPage : ContentPage, IDisposable
    {
        private static object locker = new object();

        ScanPageViewModel vm = null;

        #region Private methods
        private void init()
        {
            InitializeComponent();

            INavigationService _navService = SimpleIoc.Default.GetInstance<INavigationService>();

            ToolbarItems.Add(new ToolbarItem("Settings", "gear.png", () =>
            {
                _navService.NavigateTo(ViewLocator.SettingsPage);
            }));

            vm = new ScanPageViewModel(_navService, SimpleIoc.Default.GetInstance<IModuleDataService>(), null, ListTypeEnum.Other);

            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<LoadsChangedMessage>(this, HandleLoadsChangedMessage);

            this.BindingContext = vm;
            loadList.IsMultiLoadList = false;

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

        public ScanDropListPage()
        {
            init();
        }

        #region Events
        private void btnDelete_Clicked(object sender, EventArgs e)
        {
            int count = 0;
            lock (vm.Loads)
            {
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


        #endregion

        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            if (vm.SaveCommand.CanExecute(null))
            {
                vm.SaveCommand.Execute(null);
            }
        }

        private void btnDeleteAll_Clicked(object sender, EventArgs e)
        {
            deleteAllPrompt.Show("Are you sure you want to delete this entire scan including all serial numbers?");
        }
    }
}
