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
    public class MoveModulesView : ContentView
    {
        private AbsoluteLayout rootView = new AbsoluteLayout();
        private StackLayout rootStackLayout = new StackLayout();
        private StackLayout innerStack = new StackLayout();
        private Grid buttonGridLayout = new Grid();
        private Button btnOk = new Button();
        private Button btnClose = new Button();
        private Label title = new Label();
        private Label fieldLabel = new Label();
        private Label selectedLabel = new Label();
        private Picker picker = new Picker();
        
        private bool executeCommand = true;

        public MoveModulesView()
        {
            Content = rootView;

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

            title.Text = "Move modules to different load";
            title.FontAttributes = FontAttributes.Bold;
            title.FontSize = 18.0;

            innerStack.Children.Add(title);                        
            innerStack.Children.Add(selectedLabel);
            innerStack.Children.Add(fieldLabel);
            innerStack.Children.Add(picker);
            innerStack.Children.Add(buttonGridLayout);

            buttonGridLayout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            buttonGridLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            buttonGridLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            buttonGridLayout.ColumnSpacing = 5.0;
            buttonGridLayout.RowSpacing = 0.0;

            fieldLabel.Text = "Move to: ";

            btnOk.Text = "Ok";
            btnClose.Text = "Cancel";

            btnOk.Style = App.Current.Resources["primaryButton"] as Style;
            btnClose.Style = App.Current.Resources["primaryButton"] as Style;
            
            buttonGridLayout.Children.Add(btnOk, 0, 2);
            buttonGridLayout.Children.Add(btnClose, 1, 2);
            
            btnOk.Clicked += btnOk_Clicked;
            btnClose.Clicked += btnClose_Clicked;

        }

        public void Show()
        {
            ScanPageViewModel vm = (ScanPageViewModel)this.BindingContext;

            int selectedModules = 0;

            picker.Items.Clear();
            int count = 0;
            picker.Title = "Select a load";
            picker.Items.Add("New Load");
            foreach (var l in vm.Loads)
            {
                count = l.Modules.Count();
                selectedModules += l.Modules.Count(m => m.Selected);

                picker.Items.Add(string.Format("Load {0} - {1} modules", l.LoadNumber, count));
            }

            selectedLabel.Text = string.Format("Modules selected: {0}", selectedModules);

            this.IsVisible = true;

        }

        private void btnOk_Clicked(object sender, EventArgs e)
        {
            this.IsVisible = false;

            int LoadNumber = 0;

            if (picker.SelectedIndex == 0)
            {
                LoadNumber = 0;
            }
            else
            {
                string loadNum = picker.Items[picker.SelectedIndex];
                loadNum = loadNum.Substring(0, loadNum.IndexOf("-")).Trim();
                loadNum = loadNum.Replace("Load ", "").Trim();
                LoadNumber = int.Parse(loadNum);
            }

            if (executeCommand && OkCommand.CanExecute(LoadNumber))
            {
                OkCommand.Execute(LoadNumber);
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
