using RFIDModuleScan.Core.Services;
using RFIDModuleScan.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace RFIDModuleScan.Views
{    
    public partial class OpticalOwnershipLookupPage : ContentPage
    {
        ZXingScannerView zxing;
        Button torchButton = null;
        OwnershipLookupViewModel vm = null;
        
        double width = 0;
        double height = 0;

        public OpticalOwnershipLookupPage(OwnershipLookupViewModel viewModel)
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
           
           
        }

        private void Zxing_OnScanResult(ZXing.Result result)
        {
            Device.BeginInvokeOnMainThread(() => {
                zxing.IsAnalyzing = false;
                var vibrateService = Xamarin.Forms.DependencyService.Get<IVibrateService>();
                string sn = "";                
                vibrateService.Vibrate(500);
                vm.AddOpticalScan(result.Text, out sn);
                lblLastModuleScanned.Text = sn;              
                zxing.IsAnalyzing = true;
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            zxing.IsScanning = true;
            width = this.Width;
            height = this.Height;
        }

        protected override void OnDisappearing()
        {
            zxing.IsScanning = false;
            base.OnDisappearing();            
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