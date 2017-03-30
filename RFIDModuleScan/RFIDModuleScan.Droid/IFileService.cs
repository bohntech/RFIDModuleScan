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

[assembly: Dependency(typeof(RFIDModuleScan.Droid.FileService))]
namespace RFIDModuleScan.Droid
{    
    public class FileService : IFileService
    {
        public FileService()
        {

        }

        public string SaveText(string filename, string text)
        {
            var externalPath = global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

            externalPath = externalPath + "/RFIDModuleScan";

            if (!System.IO.Directory.Exists(externalPath))
            {
                System.IO.Directory.CreateDirectory(externalPath);
            }

            externalPath = Path.Combine(externalPath, filename);

            //var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            //var filePath = Path.Combine(documentsPath, filename);
            System.IO.File.WriteAllText(externalPath, text);

            return externalPath;
        }
        public string LoadText(string filename)
        {
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, filename);
            return System.IO.File.ReadAllText(filePath);
        }
    }
}
