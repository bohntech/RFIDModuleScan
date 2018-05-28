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

            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<OpenScanEditFormMessage>(this, HandleOpenScanEditMessage);
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<LoadsChangedMessage>(this, HandleLoadsChangedMessage);

            this.BindingContext = vm;
            loadList.IsMultiLoadList = false;

            Task.Run(() => {
                vm.Initialize();
                vm.IsBusy = true;

                var initialFarm = vm.Farm;
                var initialField = vm.Field;

                Device.BeginInvokeOnMainThread(() =>
                {
                    //picker doesn't support binding so have to create the lists manually
                    foreach (var c in vm.Clients) clientPicker.Items.Add(c.Name);
                    foreach (var f in vm.Farms) farmPicker.Items.Add(f.Name);
                    foreach (var f in vm.Fields) fieldPicker.Items.Add(f.Name);

                    clientPicker.SelectedIndex = vm.SelectedClientIndex;

                    if (!string.IsNullOrWhiteSpace(initialFarm))
                    {
                        farmPicker.SelectedIndex = farmPicker.Items.IndexOf(initialFarm);
                    }

                    if (!string.IsNullOrWhiteSpace(initialField))
                    {
                        fieldPicker.SelectedIndex = fieldPicker.Items.IndexOf(initialField);
                    }
                });

                loadList.InitialBind(vm.Loads);
                vm.IsBusy = false;
            });
        }

        private void HandleLoadsChangedMessage(LoadsChangedMessage param)
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 200), scrollCallback);
        }

        //since binding not supported on picker we use a message broadcase from the view model
        //to let us know we need to re initialize the picker's when the edit form opens
        private void HandleOpenScanEditMessage(OpenScanEditFormMessage msg)
        {

            var initialFarm = msg.Farm;
            var initialField = msg.Field;

            Device.BeginInvokeOnMainThread(() =>
            {
                //picker doesn't support binding so have to create the lists manually
                clientPicker.Items.Clear();
                farmPicker.Items.Clear();
                fieldPicker.Items.Clear();
                foreach (var c in vm.Clients) clientPicker.Items.Add(c.Name);
                foreach (var f in vm.Farms) farmPicker.Items.Add(f.Name);
                foreach (var f in vm.Fields) fieldPicker.Items.Add(f.Name);

                int selectedClientIndex = vm.SelectedClientIndex;
                clientPicker.SelectedIndex = selectedClientIndex;

                if (!string.IsNullOrWhiteSpace(initialFarm))
                {
                    farmPicker.SelectedIndex = farmPicker.Items.IndexOf(initialFarm);
                }

                if (!string.IsNullOrWhiteSpace(initialField))
                {
                    fieldPicker.SelectedIndex = fieldPicker.Items.IndexOf(initialField);
                }
            });
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
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Unregister<OpenScanEditFormMessage>(this);
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

        private void clientPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (clientPicker.SelectedIndex > -1)
            {
                vm.SelectNewClient(clientPicker.Items[clientPicker.SelectedIndex]);

                farmPicker.Items.Clear();
                foreach (var f in vm.Farms) farmPicker.Items.Add(f.Name);
                farmPicker.SelectedIndex = vm.SelectedFarmIndex;
            }
        }

        private void farmPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (farmPicker.SelectedIndex > -1)
            {
                vm.SelectNewFarm(farmPicker.Items[farmPicker.SelectedIndex]);
                fieldPicker.Items.Clear();
                foreach (var f in vm.Fields) fieldPicker.Items.Add(f.Name);
                fieldPicker.SelectedIndex = vm.SelectedFieldIndex;
            }
        }

        private void fieldPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fieldPicker.SelectedIndex > -1)
            {
                vm.SelectNewField(fieldPicker.Items[fieldPicker.SelectedIndex]);
            }
        }
    }
}
