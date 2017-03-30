//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;
using RFIDModuleScan.Core.ViewModels;
using RFIDModuleScan.Core;
using System.ComponentModel;
using Xamarin.Forms.Xaml;

namespace RFIDModuleScan.UserControls
{
    public class FindDialogView : ContentView
    {
        private AbsoluteLayout rootView = new AbsoluteLayout();
        private StackLayout rootStackLayout = new StackLayout();
        private StackLayout innerStack = new StackLayout();
        private Grid gridLayout = new Grid();        
        private Button btnClose = new Button();
        private Label titleLabel = new Label();
        private Entry serialNumberEntry = new Entry();

        private Label notFoundLabel = new Label();
        private Grid fieldGrid = new Grid();
        private Label loadLabel = new Label();
        private Label serialNumberLabel = new Label();
        private Label serialNumberField = new Label();
        private Label gpsLabel = new Label();
        private Label timeLabel = new Label();

        private Grid searchGrid = new Grid();
        private Button searchButton = new Button();

        private bool executeCommand = true;

        public FindDialogView()
        {
            Content = rootView;

            rootView.BackgroundColor = Color.FromRgba(0, 0, 0, 0.5);
            rootView.Children.Add(rootStackLayout);

            AbsoluteLayout.SetLayoutBounds(rootStackLayout, new Rectangle(0.0, 0.0, 1.0, 1.0));
            AbsoluteLayout.SetLayoutFlags(rootStackLayout, AbsoluteLayoutFlags.All);
            
            rootStackLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
            //rootStackLayout.VerticalOptions = LayoutOptions.StartAndExpand;
            //rootStackLayout.BackgroundColor = Color.FromRgb(255, 0, 0);            

            innerStack.BackgroundColor = Color.FromRgb(255, 255, 255);            
            innerStack.Margin = new Thickness(30);
            innerStack.Padding = new Thickness(15);
            innerStack.HorizontalOptions = LayoutOptions.Center;
            innerStack.VerticalOptions = LayoutOptions.Center;
                        

            titleLabel.Text = "Find Serial Number";
            titleLabel.HorizontalTextAlignment = TextAlignment.Center;
            titleLabel.FontAttributes = FontAttributes.Bold;
            titleLabel.FontSize = 16.00;

            serialNumberEntry.Placeholder = "Enter or scan a serial number";
            serialNumberEntry.HorizontalOptions = LayoutOptions.Fill;
            serialNumberEntry.VerticalOptions = LayoutOptions.Fill;      
            searchButton.Text = "Search";            
            searchButton.Style = App.Current.Resources["primaryTabButton"] as Style;
            searchButton.Clicked += SearchButton_Clicked;
            searchButton.HorizontalOptions = LayoutOptions.Start;

            searchGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            searchGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            searchGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            searchGrid.Children.Add(serialNumberEntry, 0, 0);
            searchGrid.Children.Add(searchButton, 1, 0);


            rootStackLayout.Children.Add(innerStack);
            innerStack.Children.Add(titleLabel);
            innerStack.Children.Add(searchGrid);

            notFoundLabel.Text = "Serial number not found.";
            notFoundLabel.IsVisible = false;
            notFoundLabel.TextColor = Color.FromHex("#FF0000");

            fieldGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            fieldGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            fieldGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            fieldGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            fieldGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            fieldGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            fieldGrid.Children.Add(new Label { Text = "Load:", FontAttributes = FontAttributes.Bold }, 0, 0);
            fieldGrid.Children.Add(new Label { Text = "Serial Number:", FontAttributes = FontAttributes.Bold }, 0, 1);
            fieldGrid.Children.Add(new Label { Text = "GPS:", FontAttributes = FontAttributes.Bold }, 0, 2);
            fieldGrid.Children.Add(new Label { Text = "Scan Time:", FontAttributes = FontAttributes.Bold }, 0, 3);

            fieldGrid.Children.Add(loadLabel, 1, 0);
            fieldGrid.Children.Add(serialNumberField, 1, 1);
            fieldGrid.Children.Add(gpsLabel, 1, 2);
            fieldGrid.Children.Add(timeLabel, 1, 3);
            fieldGrid.IsVisible = false;            

            innerStack.Children.Add(notFoundLabel);
            innerStack.Children.Add(fieldGrid);
            
            gridLayout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            gridLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            gridLayout.ColumnSpacing = 5.0;
            gridLayout.RowSpacing = 0.0;            
                        
            btnClose.Text = "Close";
                        
            btnClose.Style = App.Current.Resources["primaryTabButton"] as Style;

            btnClose.HorizontalOptions = LayoutOptions.Center;
            gridLayout.Children.Add(btnClose, 0, 0);
            Grid.SetColumnSpan(btnClose, 2);
            innerStack.Children.Add(gridLayout);
                        
            btnClose.Clicked += btnClose_Clicked;

        }

        private void SearchButton_Clicked(object sender, EventArgs e)
        {
            if (SearchCommand.CanExecute(serialNumberEntry.Text))
            {
                SearchCommand.Execute(serialNumberEntry.Text);
            }            
        }

        public void Show(ModuleScanViewModel vm)
        {
            serialNumberEntry.IsVisible = true;
            if (!this.IsVisible && vm == null) //find button was clicked
            {
                this.IsVisible = true;
                notFoundLabel.IsVisible = false;
                fieldGrid.IsVisible = false;                
            }
            else
            {
                this.IsVisible = true;
                if (vm == null) //scan event occurred but no serial number found
                {
                    notFoundLabel.IsVisible = true;
                    fieldGrid.IsVisible = false;
                }
                else  //serial number was found
                {
                    notFoundLabel.IsVisible = false;
                    fieldGrid.IsVisible = true;
                    loadLabel.Text = vm.LoadNumber.ToString();
                    serialNumberField.Text = vm.SerialNumber;
                    timeLabel.Text = vm.TimeStamp.ToString("MM/dd/yyyy hh:mm tt");

                    if (!vm.NoLocation)
                    {
                        gpsLabel.Text = string.Format("{0}, {1}", vm.Latitude, vm.Longitude);
                    }
                    else
                    {
                        gpsLabel.Text = "No GPS coordinates";
                    }
                }
            }        
        }      

        private void btnClose_Clicked(object sender, EventArgs e)
        {
            var obj = this.Parent;
            serialNumberEntry.Text = "";
            this.IsVisible = false;
        }

        public static readonly BindableProperty SearchCommandProperty = BindableProperty.Create<FindDialogView, ICommand>(p => p.SearchCommand, null);

        public ICommand SearchCommand
        {
            get
            {
                return (ICommand)GetValue(SearchCommandProperty);
            }
            set
            {
                SetValue(SearchCommandProperty, value);
            }
        }
    }
}
