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
    public class BusyIndicatorView : ContentView
    {
        private AbsoluteLayout rootView = new AbsoluteLayout();
        private StackLayout rootStackLayout = new StackLayout();
        private StackLayout innerStack = new StackLayout();
        private Grid gridLayout = new Grid();      
        private Label message = new Label();
        private ActivityIndicator busyIndicator = new ActivityIndicator();

        private bool executeCommand = true;

        public BusyIndicatorView()
        {
            BusyMessage = "Loading...";

            rootView.BackgroundColor = Color.FromRgba(0, 0, 0, 0.5);

            rootView.Children.Add(rootStackLayout);
            AbsoluteLayout.SetLayoutBounds(rootStackLayout, new Rectangle(0.0, 0.0, 1.0, 1.0));
            AbsoluteLayout.SetLayoutFlags(rootStackLayout, AbsoluteLayoutFlags.All);
            rootStackLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
            rootStackLayout.VerticalOptions = LayoutOptions.StartAndExpand;
            innerStack.BackgroundColor = Color.FromRgb(255, 255, 255);

            rootStackLayout.Children.Add(innerStack);
            innerStack.Margin = new Thickness(30);
            innerStack.Padding = new Thickness(15);
            innerStack.HorizontalOptions = LayoutOptions.Center;
            innerStack.VerticalOptions = LayoutOptions.Center;

            innerStack.Children.Add(message);
            innerStack.Children.Add(busyIndicator);
            busyIndicator.IsRunning = true;
            busyIndicator.HorizontalOptions = LayoutOptions.Center;
            message.FontSize = 18;
            message.IsVisible = true;
            message.TextColor = Color.FromHex("#333333");
            Content = rootView;

            PropertyChanged += (sender, e) => {
                if (e.PropertyName == BusyMessageProperty.PropertyName)
                {
                    message.Text = BusyMessage;
                }
            };
        }       

        public static readonly BindableProperty BusyMessageProperty = BindableProperty.Create<BusyIndicatorView, string>(p => p.BusyMessage, null);

        public string BusyMessage
        {
            get
            {
                return (string)GetValue(BusyMessageProperty);
            }
            set
            {                
                SetValue(BusyMessageProperty, value);
            }
        }


    }
}
