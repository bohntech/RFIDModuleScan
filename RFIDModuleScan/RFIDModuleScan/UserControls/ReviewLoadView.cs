//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using RFIDModuleScan.Converters;
using RFIDModuleScan.Core.ViewModels;
using RFIDModuleScan.Core.Messages;

namespace RFIDModuleScan.UserControls
{
    public class ReviewLoadView : ContentView
    {
        Grid buttonLayout = new Grid();
        Button expandButton = new Button();
        Label loadLabel = new Label();
        Label moduleCountLabel = new Label();
        StackLayout notesLayout = new StackLayout();
        Label notesLabel = new Label();
        Label notesTitleLabel = new Label();
        Label ginTicketLoadNumberLabel = new Label();
        Label ginTicketLoadNumberTitleLabel = new Label();
        StackLayout container = new StackLayout();
        WrapLayout moduleWrapper = new WrapLayout();
        LoadViewModel _vm = null;
        
        public ReviewLoadView()
        {
            buttonLayout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(45, GridUnitType.Absolute) });
            buttonLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50.0) });
            buttonLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            buttonLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            buttonLayout.ColumnSpacing = 5.0;
            buttonLayout.RowSpacing = 0.0;
            notesTitleLabel.LineBreakMode = LineBreakMode.NoWrap;
            notesLayout.Orientation = StackOrientation.Vertical;
            notesTitleLabel.Text = "Notes: ";
            notesTitleLabel.FontAttributes = FontAttributes.Bold;
            notesLabel.LineBreakMode = LineBreakMode.WordWrap;
            notesLayout.Children.Add(notesTitleLabel);
            notesLayout.Children.Add(notesLabel);

            ginTicketLoadNumberTitleLabel.LineBreakMode = LineBreakMode.NoWrap;            
            ginTicketLoadNumberTitleLabel.Text = "Gin Ticket Load #: ";
            ginTicketLoadNumberTitleLabel.FontAttributes = FontAttributes.Bold;
            notesLayout.Children.Add(ginTicketLoadNumberTitleLabel);
            notesLayout.Children.Add(ginTicketLoadNumberLabel);

            buttonLayout.BackgroundColor = Color.FromHex("#CECECE");
            expandButton.Image = "downarrow.png";
            expandButton.Margin = new Thickness(5, 0, 5, 0);
            expandButton.Text = "";
            expandButton.Clicked += ExpandButton_Clicked;
            expandButton.BackgroundColor = Color.FromHex("#CECECE");
            expandButton.HorizontalOptions = LayoutOptions.FillAndExpand;
            loadLabel.FontSize = 18.0;
            loadLabel.VerticalTextAlignment = TextAlignment.Center;
            moduleCountLabel.VerticalTextAlignment = TextAlignment.Center;
            moduleCountLabel.FontSize = 18.0;            
            moduleCountLabel.Margin = new Thickness(0, 0, 5, 0);
            moduleCountLabel.HorizontalTextAlignment = TextAlignment.End;
            moduleWrapper.Orientation = StackOrientation.Horizontal;
            moduleWrapper.Spacing = 15.0;

            buttonLayout.Children.Add(expandButton, 0, 0);
            buttonLayout.Children.Add(loadLabel, 1, 0);
            buttonLayout.Children.Add(moduleCountLabel, 2, 0);
            container.Children.Add(buttonLayout);
            container.Children.Add(notesLayout);
            container.Children.Add(moduleWrapper);
            Content = container;
        }

        private void ExpandButton_Clicked(object sender, EventArgs e)
        {
            _vm.IsOpen = !_vm.IsOpen;
        }

        public void BindToViewModel(LoadViewModel vm)
        {
            this.BindingContext = _vm = vm;

            notesLayout.SetBinding(StackLayout.IsVisibleProperty, "IsOpen");
            notesLabel.SetBinding(Label.IsVisibleProperty, new Binding { Path = "Notes", Converter = new EmptyStringInvisibleConverter() });
            notesTitleLabel.SetBinding(Label.IsVisibleProperty, new Binding { Path = "Notes", Converter = new EmptyStringInvisibleConverter() });

            ginTicketLoadNumberLabel.SetBinding(Label.IsVisibleProperty, new Binding { Path = "GinTicketLoadNumber", Converter = new EmptyStringInvisibleConverter() });
            ginTicketLoadNumberTitleLabel.SetBinding(Label.IsVisibleProperty, new Binding { Path = "GinTicketLoadNumber", Converter = new EmptyStringInvisibleConverter() });
            
            notesLabel.SetBinding(Label.TextProperty, "Notes");
            ginTicketLoadNumberLabel.SetBinding(Label.TextProperty, "GinTicketLoadNumber");

            expandButton.SetBinding(Button.ImageProperty, new Binding { Path = "IsOpen", Converter = new BoolToArrowImageConverter() });
            loadLabel.SetBinding(Label.TextProperty, new Binding { Path = "LoadNumber", Converter = new LoadNumberConverter() });            
            moduleCountLabel.SetBinding(Label.TextProperty, new Binding { Path = "ModuleCount", Converter = new ModuleCountToTextConverter() });
            moduleWrapper.SetBinding(WrapLayout.IsVisibleProperty, "IsOpen");
            
            if (vm.Modules != null)
            {
                moduleWrapper.Children.Clear();

                foreach (var m in vm.Modules)
                {
                    var serialNumber = new Label { BindingContext = m };                    
                    serialNumber.SetBinding(Label.TextProperty, new Binding { Path = "SerialNumberWithMessage" });
                    serialNumber.SetBinding(Label.TextColorProperty, new Binding { Path = "NoLocation", Converter = new TrueToErrorColorConverter() });
                    moduleWrapper.Children.Add(serialNumber);
                }
            }            
        }

        public void Dispose()
        {
            
        }

        
    }
}
