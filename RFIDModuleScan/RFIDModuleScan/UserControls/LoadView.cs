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
    public class LoadView : ContentView
    {
        Grid buttonLayout = new Grid();
        Button expandButton = new Button();
        Label loadLabel = new Label();
        Label moduleCountLabel = new Label();
        Entry notesEntry = new Entry();
        Entry ginLoadEntry = new Entry();
        StackLayout container = new StackLayout();
        WrapLayout moduleWrapper = new WrapLayout();
        LoadViewModel _vm = null;

        private bool _showLoadHeaders { get; set; }
      

        public LoadView(bool showHeaders)
        {   
            buttonLayout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(45, GridUnitType.Absolute) });
            buttonLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50.0)});
            buttonLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            buttonLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            buttonLayout.ColumnSpacing = 5.0;
            buttonLayout.RowSpacing = 0.0;
            notesEntry.HorizontalOptions = LayoutOptions.FillAndExpand;
            notesEntry.Placeholder = "Tap here to add notes...";

            ginLoadEntry.HorizontalOptions = LayoutOptions.FillAndExpand;
            ginLoadEntry.Placeholder = "Gin ticket load #...";
            
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
            moduleCountLabel.HorizontalTextAlignment = TextAlignment.End;
            moduleCountLabel.FontSize = 18.0;
            moduleCountLabel.Margin = new Thickness(0, 0, 5, 0);

            moduleWrapper.Orientation = StackOrientation.Horizontal;
            //moduleWrapper.HorizontalOptions = LayoutOptions.FillAndExpand;
            //moduleWrapper.BackgroundColor = Color.Blue;
               
            buttonLayout.Children.Add(expandButton, 0, 0);
            buttonLayout.Children.Add(loadLabel, 1, 0);
            buttonLayout.Children.Add(moduleCountLabel, 2, 0);

            _showLoadHeaders = showHeaders;

            if (_showLoadHeaders)
            {
                container.Children.Add(buttonLayout);
                container.Children.Add(notesEntry);
                container.Children.Add(ginLoadEntry);
            }
            
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

            notesEntry.SetBinding(Entry.IsVisibleProperty, "IsOpen");
            notesEntry.SetBinding(Entry.TextProperty, "Notes");
            ginLoadEntry.SetBinding(Entry.TextProperty, "GinTicketLoadNumber");
            ginLoadEntry.SetBinding(Entry.IsVisibleProperty, "IsOpen");
            expandButton.SetBinding(Button.ImageProperty, new Binding { Path = "IsOpen", Converter = new BoolToArrowImageConverter() });
            loadLabel.SetBinding(Label.TextProperty, new Binding { Path = "LoadNumber", Converter = new LoadNumberConverter() });
            moduleCountLabel.SetBinding(Label.TextProperty, new Binding { Path = "ModuleCount", Converter = new ModuleCountToTextConverter() });
            moduleWrapper.SetBinding(WrapLayout.IsVisibleProperty, "IsOpen");
            
            notesEntry.Unfocused += NotesEntry_Unfocused;
            ginLoadEntry.Unfocused += GinLoadEntry_Unfocused;

            ginLoadEntry.Focused += GinLoadEntry_Focused;

            if (vm.Modules != null)
            {
                moduleWrapper.Children.Clear();

                lock (vm.Modules)
                {
                    foreach (var m in vm.Modules)
                    {
                        var cbx = new CheckBox { BindingContext = m };
                        cbx.SetBinding(CheckBox.CheckedProperty, new Binding { Path = "Selected" });
                        cbx.SetBinding(CheckBox.DefaultTextProperty, new Binding { Path = "SerialNumberWithMessage", Mode= BindingMode.OneWay });
                        cbx.SetBinding(CheckBox.CheckedTextProperty, new Binding { Path = "SerialNumberWithMessage", Mode = BindingMode.OneWay });
                        cbx.SetBinding(CheckBox.UncheckedTextProperty, new Binding { Path = "SerialNumberWithMessage", Mode = BindingMode.OneWay });
                        cbx.SetBinding(CheckBox.TextColorProperty, new Binding { Path = "NoLocation", Converter = new TrueToErrorColorConverter() });
                        moduleWrapper.Children.Add(cbx);
                    }
                }
            }


            vm.Modules.CollectionChanged += Modules_CollectionChanged;
        }

        private void GinLoadEntry_Focused(object sender, FocusEventArgs e)
        {
            _vm.IsGinLoadFocused = true;
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<LoadFocusedMessage>(new LoadFocusedMessage { ID = _vm.ID, VM = _vm });
        }

        private void GinLoadEntry_Unfocused(object sender, FocusEventArgs e)
        {
            Entry entry = sender as Entry;
            _vm.IsGinLoadFocused = false;

            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<LoadUnFocusedMessage>(new LoadUnFocusedMessage { ID = _vm.ID, VM = _vm });
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<GinTicketLoadNumberChangedMessage>(new GinTicketLoadNumberChangedMessage { ID = _vm.ID, GinTicketLoadNumber = ginLoadEntry.Text });
        }

        private void NotesEntry_Unfocused(object sender, FocusEventArgs e)
        {
            Entry entry = sender as Entry;
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<NotesChangedMessage>(new NotesChangedMessage { ID = _vm.ID, Notes = entry.Text });
        }

        private void NotesEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            Entry entry = sender as Entry;
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<NotesChangedMessage>(new NotesChangedMessage { ID = _vm.ID, Notes = e.NewTextValue });
        }

        private void NotesEntry_Completed(object sender, EventArgs e)
        {
           
        }

        public void Dispose()
        {
            _vm.Modules.CollectionChanged -= Modules_CollectionChanged;
        }

        private void Modules_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                //remove any old items no longer in list
                ModuleScanViewModel dataContext = null;
                ModuleScanViewModel eventItem = null;

                List<CheckBox> controlsToRemove = new List<CheckBox>();

                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                {
                    //remove from wrap layout
                    foreach (var item in e.OldItems)
                    {
                        eventItem = item as ModuleScanViewModel;
                        foreach (CheckBox c in moduleWrapper.Children)
                        {
                            dataContext = c.BindingContext as ModuleScanViewModel;
                            if (dataContext.ID == eventItem.ID)
                            {
                                controlsToRemove.Add(c);
                            }
                        }
                    }

                    foreach (var c in controlsToRemove)
                    {
                        moduleWrapper.Children.Remove(c);
                    }
                }
                else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {

                    foreach (var m in e.NewItems)
                    {
                        var cbx = new CheckBox { BindingContext = m };
                        cbx.SetBinding(CheckBox.CheckedProperty, new Binding { Path = "Selected" });
                        cbx.SetBinding(CheckBox.DefaultTextProperty, new Binding { Path = "SerialNumberWithMessage", Mode = BindingMode.OneWay });
                        cbx.SetBinding(CheckBox.CheckedTextProperty, new Binding { Path = "SerialNumberWithMessage", Mode = BindingMode.OneWay });
                        cbx.SetBinding(CheckBox.UncheckedTextProperty, new Binding { Path = "SerialNumberWithMessage", Mode = BindingMode.OneWay });
                        cbx.SetBinding(CheckBox.TextColorProperty, new Binding { Path = "NoLocation", Converter = new TrueToErrorColorConverter() });
                        moduleWrapper.Children.Add(cbx);
                    }

                }
            });
        }
    }
}
