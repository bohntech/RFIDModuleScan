using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFIDModuleScan.Views;
using Xamarin.Forms;
using RFIDModuleScan.Core;
using RFIDModuleScan.Core.Models;
using RFIDModuleScan.Core.Scanners;
using RFIDModuleScan.Navigation;
using RFIDModuleScan.Core.ViewModels;
using GalaSoft.MvvmLight.Views;
using GalaSoft.MvvmLight.Ioc;
using RFIDModuleScan.Core.Data;
using RFIDModuleScan.Core.Enums;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml.Internals;
using RFIDModuleScan.Core.Services;

namespace RFIDModuleScan
{
    public partial class App : Application
    {
        public App()
        {
            // The root page of your application
            InitializeComponent();            
            NavigationService nav = new NavigationService();

            nav.Configure(ViewLocator.AllScansPage, typeof(AllScansPage));
            nav.Configure(ViewLocator.HomePage, typeof(HomePage));
            nav.Configure(ViewLocator.ReviewPage, typeof(ReviewPage));
            nav.Configure(ViewLocator.ScanPage, typeof(ScanPage));
            nav.Configure(ViewLocator.SettingsPage, typeof(SettingsPage));
            nav.Configure(ViewLocator.ScanDropListPage, typeof(ScanDropListPage));
            nav.Configure(ViewLocator.AboutPage, typeof(AboutPage));
            nav.Configure(ViewLocator.OpticalScanPage, typeof(OpticalScanPage));
            nav.Configure(ViewLocator.OpticalFindPage, typeof(OpticalFindPage));
            SimpleIoc.Default.Register<INavigationService>(() => nav);
            SimpleIoc.Default.Register<IModuleDataService>(() => new ModuleDataService());

         

            var startPage = new NavigationPage(new HomePage());            
            startPage.Popped += StartPage_Popped;
                        
            startPage.BarBackgroundColor = Color.FromHex("#404040");
            startPage.BarTextColor = Color.FromHex("#FFFFFF");
            nav.Initialize(startPage);
            MainPage = startPage;
        }

        private void StartPage_Popped(object sender, NavigationEventArgs e)
        {
            var navpage = e.Page as IDisposable;
            if (navpage != null)
            {
                navpage.Dispose();
            }
            e.Page.BindingContext = null;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            Task.Run(() =>
            {
                ScannerConnectionManager.InitializeContext(ScannerType.TSL);
            });
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps           
        }

        protected override void OnResume()
        {
          
        }
    }
}
