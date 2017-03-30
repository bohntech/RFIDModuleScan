//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using RFIDModuleScan.Core.ViewModels;
using RFIDModuleScan.Core;
using System.Threading.Tasks;

namespace RFIDModuleScan.UserControls
{
    public class LoadsLayout : ContentView, IDisposable
    {
        private StackLayout loadLayout = new StackLayout();

        public bool IsMultiLoadList { get; set; }

        public RangeObservableCollection<LoadViewModel> LoadSource
        {
            get; set;
        }

        public LoadsLayout()
        {
            Content = null;
            //LoadSource = new ObservableCollection<LoadViewModel>();

        }

        /*public void BindToVM(RangeObservableCollection<LoadViewModel> loads)
        {
            LoadSource = loads;
            LoadSource.CollectionChanged += LoadSource_CollectionChanged;
        }*/

        public void InitialBind(RangeObservableCollection<LoadViewModel> loads)
        {
            LoadSource = loads;

            lock (loads)
            {
                foreach (var l in loads)
                {
                    var loadView = new LoadView(IsMultiLoadList);                    
                    loadView.BindToViewModel((LoadViewModel)l);
                    loadLayout.Children.Add(loadView);
                }
            }

            Device.BeginInvokeOnMainThread(() => {            
                Content = loadLayout;
            });

            LoadSource.CollectionChanged += LoadSource_CollectionChanged;
        }

        public void Dispose()
        {
            Content = null;
            loadLayout.Children.Clear();
            LoadSource.CollectionChanged -= LoadSource_CollectionChanged;
        }

        private void LoadSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                //remove any old items no longer in list
                LoadViewModel dataContext = null;
                LoadViewModel eventItem = null;

                List<LoadView> controlsToRemove = new List<LoadView>();

                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                {
                    //remove from wrap layout
                    foreach (var item in e.OldItems)
                    {

                        eventItem = item as LoadViewModel;
                        foreach (LoadView c in loadLayout.Children)
                        {
                            dataContext = c.BindingContext as LoadViewModel;
                            if (dataContext.ID == eventItem.ID)
                            {
                                controlsToRemove.Add(c);
                            }
                        }
                    }

                    foreach (var c in controlsToRemove)
                    {
                        loadLayout.Children.Remove(c);
                    }


                }
                else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    List<LoadView> childLoads = new List<LoadView>();

                    foreach (var m in e.NewItems)
                    {
                        var loadView = new LoadView(IsMultiLoadList);                        
                        loadView.BindToViewModel((LoadViewModel)m);
                        childLoads.Add(loadView);
                    }


                    foreach (var l in childLoads)
                    {
                        loadLayout.Children.Add(l);
                    }

                }
            });       
        }
    }

}
