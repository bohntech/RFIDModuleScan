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
    public partial class OpticalScanPage : ContentPage
    {
        ZXingScannerView zxing;        
        Button torchButton = null;
        ScanPageViewModel vm = null;
        bool allowGPSUpdate = true;

        double width = 0;
        double height = 0;

        public OpticalScanPage(ScanPageViewModel viewModel)
        {
            InitializeComponent();

            vm = viewModel;

            initPage();
        }

        private void initPage()
        {
            zxing = new ZXingScannerView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            zxing.Options.AutoRotate = true;
            zxing.Options.TryInverted = true;
            zxing.OnScanResult += Zxing_OnScanResult;

            torchButton = new Button();
            torchButton.IsVisible = true;
            torchButton.Text = (zxing.IsTorchOn) ? "Flash On" : "Flash Off";

            absLayout.Children.Clear();            

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
            textOverlay.TextColor = Color.White;
            textOverlay.Text = "Hold device up to bar code to scan.";
            textOverlay.Margin = new Thickness(10);
            AbsoluteLayout.SetLayoutFlags(textOverlay, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(textOverlay, new Rectangle(0.5, 1.0, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
            absLayout.Children.Add(textOverlay);

            lblLoadCount.Text = vm.LoadCount.ToString();
            lblModuleCount.Text = vm.ModuleCount.ToString();

            if (vm.Loads.Count() > 0)
            {
                var lastModule = vm.Loads.ToArray()[vm.Loads.Count() - 1].Modules.LastOrDefault();
                if (lastModule != null)
                {
                    lblLastModuleScanned.Text = lastModule.SerialNumber;
                }
            }

            Xamarin.Forms.Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                lblCurrentGPS.Text = vm.GPSMessage;
                return allowGPSUpdate;
            });
        }

        private void Zxing_OnScanResult(ZXing.Result result)
        {
            Device.BeginInvokeOnMainThread(() => {                                
                zxing.IsAnalyzing = false;
                var vibrateService = Xamarin.Forms.DependencyService.Get<IVibrateService>();
                string sn = "";
                string gps = "";
                vibrateService.Vibrate(500);
                vm.AddOpticalScan(result.Text, out sn, out gps);                
                lblLastModuleScanned.Text = sn;
                lblCurrentGPS.Text = gps;
                lblLoadCount.Text = vm.LoadCount.ToString();
                lblModuleCount.Text = vm.ModuleCount.ToString();
                zxing.IsAnalyzing = true;
            });
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            zxing.IsScanning = true;
            allowGPSUpdate = true;

            width = this.Width;
            height = this.Height;
        }

        protected override void OnDisappearing()
        {
            zxing.IsScanning = false;
            base.OnDisappearing();
            allowGPSUpdate = false;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {

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
