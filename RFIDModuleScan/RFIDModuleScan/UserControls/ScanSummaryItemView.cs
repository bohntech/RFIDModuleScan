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

namespace RFIDModuleScan.UserControls
{
    public class ScanSummaryItemView : ContentView
    {
        private StackLayout rootStack = new StackLayout();
        private Grid rootGrid = new Grid();

        private Label growerLabel = new Label { FontAttributes=FontAttributes.Bold };
        private Label farmLabel = new Label { FontAttributes = FontAttributes.Bold };
        private Label fieldLabel = new Label { FontAttributes = FontAttributes.Bold };
        private Label loadsLabel = new Label { FontAttributes = FontAttributes.Bold };
        private Label modulesLabel = new Label { FontAttributes = FontAttributes.Bold };
        private Label lastScanLabel = new Label { FontAttributes = FontAttributes.Bold };
        private Label sentLabel = new Label {Text="SENT:", FontAttributes = FontAttributes.Bold, TextColor = Color.FromHex("#409740") };

        private Label growerValueLabel = new Label();
        private Label farmValueLabel = new Label();
        private Label fieldValueLabel = new Label();
        private Label loadsValueLabel = new Label();
        private Label modulesValueLabel = new Label();
        private Label lastScanValueLabel = new Label();
        private Label sentValueLabel = new Label { TextColor = Color.FromHex("#409740") };

        public ScanSummaryItemView()
        {
            
            
            rootGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            rootGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            rootGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            rootGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            //rootGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });

            rootGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            rootGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            rootGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            rootGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            //rootGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2) });

            rootGrid.Margin = new Thickness(0);            
                        
            rootGrid.ColumnSpacing = 5.0;
            rootGrid.RowSpacing = 5.0;
            rootGrid.Padding = new Thickness(5.0);
            rootGrid.Children.Add(growerLabel, 0, 0);
            rootGrid.Children.Add(growerValueLabel, 1, 0);

            rootGrid.Children.Add(farmLabel, 0, 1);
            rootGrid.Children.Add(farmValueLabel, 1, 1);

            rootGrid.Children.Add(fieldLabel, 0, 2);
            rootGrid.Children.Add(fieldValueLabel, 1, 2);

            rootGrid.Children.Add(sentLabel, 0, 3);
            rootGrid.Children.Add(sentValueLabel, 1, 3);


            rootGrid.Children.Add(loadsLabel, 2, 0);
            rootGrid.Children.Add(loadsValueLabel, 3, 0);

            rootGrid.Children.Add(modulesLabel, 2, 1);
            rootGrid.Children.Add(modulesValueLabel, 3, 1);

            rootGrid.Children.Add(lastScanLabel, 2, 2);
            rootGrid.Children.Add(lastScanValueLabel, 3, 2);
            rootGrid.BackgroundColor = Color.FromHex("#FFFFFF");

           /* BoxView bv = new BoxView();
            bv.BackgroundColor = Color.FromHex("#666666");


            rootGrid.Children.Add(bv, 0, 4);
            Grid.SetColumnSpan(bv, 4);*/
            
            this.Content = rootGrid;
        }        

        public void BindToVM(ScanItemViewModel vm, INavigationService _navService)
        {
            this.BindingContext = vm;

            growerLabel.Text = "Grower:";            
            farmLabel.Text = "Farm:";
            fieldLabel.Text = "Field:";
            modulesLabel.Text = "Modules:";
            loadsLabel.Text = "Loads:";
            lastScanLabel.Text = "Last scan:";
            
            growerValueLabel.Text = vm.Grower;
            farmValueLabel.Text = vm.Farm;
            fieldValueLabel.Text = vm.Field;

            modulesValueLabel.Text = vm.Modules.ToString();
            loadsValueLabel.Text = vm.Loads.ToString();

            lastScanValueLabel.Text = vm.LastScan.ToString("MM/dd/yyyy hh:mm tt");

            sentValueLabel.Text = vm.TransmitMsg;

            sentLabel.IsVisible = sentValueLabel.IsVisible = !(string.IsNullOrEmpty(vm.TransmitMsg));

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) => {
                // handle the tap
                var viewModel = (ScanItemViewModel)this.BindingContext;

                rootGrid.BackgroundColor = Color.FromHex("#BEBEBE");

                //start a timer to check every 5 seconds for GPS being switched on/off
                Xamarin.Forms.Device.StartTimer(new TimeSpan(0, 0, 0, 0, 200), () =>
                {
                    rootGrid.BackgroundColor = Color.FromHex("#FFFFFF");
                    _navService.NavigateTo(ViewLocator.ScanPage, viewModel.FieldScanID);
                    return false;
                });
            };

            rootGrid.GestureRecognizers.Add(tapGestureRecognizer);
            //sentLabel.TextColor = Color.FromHex("#00FF00");
            //sentValueLabel.TextColor = Color.FromHex("#00FF00");
        }
    }
}
