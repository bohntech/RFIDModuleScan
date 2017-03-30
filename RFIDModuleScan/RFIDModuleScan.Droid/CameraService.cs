//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFIDModuleScan.Core.Services;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using XLabs.Platform.Services;
using Xamarin.Forms;
using System.IO;
using Plugin.CurrentActivity;
using Android.Media;

[assembly: Dependency(typeof(RFIDModuleScan.Droid.CameraService))]
namespace RFIDModuleScan.Droid
{
    public class CameraService : RFIDModuleScan.Core.Services.ICameraService
    {
        public CameraService()
        {

        }

        public void SetOrientation(int degrees)
        {
           
        }
    }
}
