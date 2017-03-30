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

[assembly: Dependency(typeof(RFIDModuleScan.Droid.EmailService))]
namespace RFIDModuleScan.Droid
{
    public class EmailService : RFIDModuleScan.Core.Services.IEmailService
    {
        public EmailService()
        {

        }

        public void ShowDraft(string subject, string body, bool html, string to, List<string> files)
        {
            var svc = new XLabs.Platform.Services.Email.EmailService();
            svc.ShowDraft(subject, body, html, to, files);
        }
    }
}
