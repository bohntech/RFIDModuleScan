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
using ZXing.Net.Mobile.Forms;
using RFIDModuleScan.Core.Services;

namespace RFIDModuleScan.Views
{
    public partial class OpticalFindPage : ContentPage
    {
        ZXingScannerView zxing;
        Button torchButton = new Button();
        ReviewPageViewModel vm = null;
        bool allowGPSUpdate = true;
        INavigationService _navService = null;
        double width = 0;
        double height = 0;

        public OpticalFindPage(Guid id)
        {
            InitializeComponent();

            _navService = SimpleIoc.Default.GetInstance<INavigationService>();

            vm = new ReviewPageViewModel(_navService, SimpleIoc.Default.GetInstance<IModuleDataService>(), id);

            //this.BindingContext = vm;

            Task.Run(() => {
                vm.IsBusy = true;
                vm.Initialize();                                
                vm.IsBusy = false;
            });

            
            initPage();
        }

        private void initPage()
        {
            absLayout.Children.Clear();

            zxing = new ZXingScannerView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            zxing.Options.AutoRotate = true;
            zxing.Options.TryInverted = true;

            zxing.OnScanResult += Zxing_OnScanResult;

            torchButton.IsVisible = true;
            torchButton.Text = (zxing.IsTorchOn) ? "Flash On" : "Flash Off";

            var grid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            AbsoluteLayout.SetLayoutBounds(zxing, new Rectangle(0, 0, 1.0, 1.0));
            AbsoluteLayout.SetLayoutFlags(zxing, AbsoluteLayoutFlags.SizeProportional);
            absLayout.Children.Add(zxing);

            BoxView centerLine = new BoxView();
            centerLine.BackgroundColor = Color.Red;
            centerLine.HeightRequest = 3;
            centerLine.WidthRequest = 1000;
            AbsoluteLayout.SetLayoutFlags(centerLine, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(centerLine, new Rectangle(0, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
            absLayout.Children.Add(centerLine);

            torchButton.Clicked += TorchButton_Clicked;
            torchButton.Opacity = 1.0;
            AbsoluteLayout.SetLayoutFlags(torchButton, AbsoluteLayoutFlags.XProportional);
            AbsoluteLayout.SetLayoutBounds(torchButton, new Rectangle(1.0, 10.0, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
            absLayout.Children.Add(torchButton);

            Label textOverlay = new Label();
            textOverlay.Text = "Hold device up to bar code to scan and lookup.";
            textOverlay.Margin = new Thickness(10);
            textOverlay.TextColor = Color.White;
            AbsoluteLayout.SetLayoutFlags(textOverlay, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(textOverlay, new Rectangle(0.5, 1.0, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
            absLayout.Children.Add(textOverlay);

        }

        private void Zxing_OnScanResult(ZXing.Result result)
        {
            Device.BeginInvokeOnMainThread(() => {
                zxing.IsAnalyzing = false;

                var vibrateService = Xamarin.Forms.DependencyService.Get<IVibrateService>();                
                vibrateService.Vibrate(500);
                //search for serial number
                vm.LocateOpticalSearchResult(result.Text);
                fieldGrid.IsVisible = false;
                notFoundGrid.IsVisible = false;
                if (vm.SearchResult != null)
                {
                    lblSerialNumber.Text = vm.SearchResult.SerialNumber;
                    lblScanTime.Text = vm.SearchResult.TimeStamp.ToString("MM/dd/yyyy hh:mm tt");
                    lblLoad.Text = vm.SearchResult.LoadNumber.ToString();
                    if (!vm.SearchResult.NoLocation)
                    {
                        lblGPS.Text = string.Format("{0}, {1}", vm.SearchResult.Latitude, vm.SearchResult.Longitude);
                    }
                    else
                    {
                        lblGPS.Text = "No GPS coordinates";
                    }
                    fieldGrid.IsVisible = true;
                }
                else
                {
                    notFoundGrid.IsVisible = true;
                }

                zxing.IsAnalyzing = true;
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            zxing.IsScanning = true;
            allowGPSUpdate = true;

            width = Width;
            height = Height;
        }

        protected override void OnDisappearing()
        {
            zxing.IsScanning = false;
            base.OnDisappearing();
            allowGPSUpdate = false;
        }

        private void TorchButton_Clicked(object sender, EventArgs e)
        {
            zxing.IsTorchOn = !zxing.IsTorchOn;
            torchButton.Text = (zxing.IsTorchOn) ? "Flash On" : "Flash Off";
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height); //must be called            
            if (this.width != width || this.height != height)
            {
                this.width = width;
                this.height = height;
                //reconfigure layout

                zxing.IsScanning = false;
                zxing.OnScanResult -= Zxing_OnScanResult;
                torchButton.Clicked -= TorchButton_Clicked;
                initPage();
                zxing.IsScanning = true;
            }
        }
    }
}
