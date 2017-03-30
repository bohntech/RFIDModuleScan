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

[assembly: Dependency(typeof(RFIDModuleScan.Droid.VibrateService))]
namespace RFIDModuleScan.Droid
{
    public class VibrateService : RFIDModuleScan.Core.Services.IVibrateService
    {
        public VibrateService()
        {

        }

        public void Vibrate(int ms)
        {
            ToneGenerator toneGen1 = new ToneGenerator(Android.Media.Stream.Dtmf, 100);
            toneGen1.StartTone(Tone.PropBeep, ms);

            var activity = CrossCurrentActivity.Current;
            Vibrator vibrator = (Vibrator) activity.Activity.GetSystemService(Context.VibratorService);            
            vibrator.Vibrate(250);
        }
    }
}
