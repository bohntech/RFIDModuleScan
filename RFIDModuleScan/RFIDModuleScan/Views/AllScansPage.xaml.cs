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
using RFIDModuleScan.Core.Messages;
using RFIDModuleScan.UserControls;
using Xamarin.Forms.Xaml;

namespace RFIDModuleScan.Views
{   
    public partial class AllScansPage : ContentPage, IDisposable
    {
        private INavigationService _navService;

        private AllScansPageViewModel vm;

        private void scansLoaded(AllScansLoadComplete msg)
        {
            StackLayout scanItemLayout = new StackLayout();
            scanItemLayout.Spacing = 2.0;
            scanItemLayout.BackgroundColor = Color.FromHex("#A0A0A0");

            if (vm.ScanItems.Count() > 0)
            {
                foreach (var scan in vm.ScanItems)
                {
                    ScanSummaryItemView summary = new ScanSummaryItemView();
                    summary.BindToVM(scan, _navService);
                    scanItemLayout.Children.Add(summary);
                }
            }
            else
            {
                scanItemLayout.BackgroundColor = Color.FromHex("#FFFFFF");
                scanItemLayout.Children.Add(new Label { Text = "No scans found on this device.", FontSize = 18, Margin = 30, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center});
            }
                    
            scanItemLayout.IsVisible = true;
                        
            Device.BeginInvokeOnMainThread(() =>
            {
                scrollLayout.Content = scanItemLayout;
                scrollLayout.IsVisible = true;
                busyLayout.IsVisible = false;
            });
        }
               

        public AllScansPage()
        {
            InitializeComponent();

            _navService = SimpleIoc.Default.GetInstance<INavigationService>();
            var _dataService = SimpleIoc.Default.GetInstance<IModuleDataService>();

                        
            BindingContext = vm = new AllScansPageViewModel(_navService, _dataService);

            ToolbarItems.Add(new ToolbarItem("New Scan", "plusicon.png", () =>
            {
                _navService.NavigateTo(ViewLocator.ScanPage);                
            }));

            ToolbarItems.Add(new ToolbarItem("Settings", "gear.png", () =>
            {
                _navService.NavigateTo(ViewLocator.SettingsPage);
            }));
                       

            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<AllScansLoadComplete>(this, scansLoaded);           
        }

        public void Dispose()
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Unregister<AllScansLoadComplete>(this);
            vm.Dispose();
        }
/*
        private void ViewCell_Tapped(object sender, EventArgs e)
        {
            ScanItemViewModel vm = (ScanItemViewModel)((ViewCell)sender).BindingContext;
            _navService.NavigateTo(ViewLocator.ScanPage, vm.FieldScanID);
        }*/

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            if (vm != null)
            {
                busyLayout.IsVisible = true;
                scrollLayout.IsVisible = false;
                Device.StartTimer(new TimeSpan(0, 0, 0, 0, 250), onRefresh);               
            }
        }

        private bool onRefresh()
        {
            Task.Run(() =>
            {
                vm.Initialize();
            });           
            return false;
        }
    }
}
