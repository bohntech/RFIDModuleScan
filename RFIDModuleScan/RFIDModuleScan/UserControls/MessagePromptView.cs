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
    public class MessagePromptView : ContentView
    {
        private AbsoluteLayout rootView = new AbsoluteLayout();
        private StackLayout rootStackLayout = new StackLayout();
        private StackLayout innerStack = new StackLayout();
        private Grid gridLayout = new Grid();
        private Button btnOk = new Button();
        private Button btnClose = new Button();
        private Label message = new Label();

        private bool executeCommand = true;

        public MessagePromptView()
        {
            

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
            message.FontSize = 18.0;
            message.Margin = new Thickness(0.0, 25.0);

            gridLayout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            gridLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            gridLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            gridLayout.ColumnSpacing = 5.0;
            gridLayout.RowSpacing = 0.0;

            btnOk.Text = "Ok";
            btnClose.Text = "Cancel";

            btnOk.Style = App.Current.Resources["primaryButton"] as Style;
            btnClose.Style = App.Current.Resources["primaryButton"] as Style;

            gridLayout.Children.Add(btnOk, 0, 0);
            gridLayout.Children.Add(btnClose, 1, 0);

            innerStack.Children.Add(gridLayout);

            btnOk.Clicked += btnOk_Clicked;
            btnClose.Clicked += btnClose_Clicked;

            Content = rootView;

        }

        public void Show(string msg, bool showOk=true, bool showCancel=true, bool execCommand=true)
        {
            executeCommand = execCommand;
            btnOk.IsVisible = showOk;
            btnClose.IsVisible = showCancel;

            message.Text = msg;
            this.IsVisible = true;
        }

        private void btnOk_Clicked(object sender, EventArgs e)
        {
            this.IsVisible = false;

            if (executeCommand && OkCommand.CanExecute(null))
            {
                OkCommand.Execute(null);
            }
        }

        private void btnClose_Clicked(object sender, EventArgs e)
        {
            var obj = this.Parent;

            this.IsVisible = false;
        }

        public static readonly BindableProperty OkCommandProperty = BindableProperty.Create<MessagePromptView, ICommand>(p => p.OkCommand, null);

        public ICommand OkCommand
        {
            get
            {
                return (ICommand)GetValue(OkCommandProperty);
            }
            set
            {
                SetValue(OkCommandProperty, value);
            }
        }


    }
}
