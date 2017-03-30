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

namespace RFIDModuleScan.UserControls
{
    public class ReviewLoadsView : ContentView, IDisposable
    {
        private StackLayout loadLayout = new StackLayout();
        
        public RangeObservableCollection<LoadViewModel> LoadSource
        {
            get; set;
        }

        public ReviewLoadsView()
        {            
            //LoadSource = new ObservableCollection<LoadViewModel>();
        }

        public void BindToVM(RangeObservableCollection<LoadViewModel> loads)
        {
            LoadSource = loads;           

            foreach (var l in loads)
            {
                var loadView = new ReviewLoadView();
                loadView.BindToViewModel((LoadViewModel)l);
                loadLayout.Children.Add(loadView);
            }

            Device.BeginInvokeOnMainThread(() => {
                Content = null;
                Content = loadLayout;
            });
        }
       

        public void Dispose()
        {
            
        }
        
    }
}
