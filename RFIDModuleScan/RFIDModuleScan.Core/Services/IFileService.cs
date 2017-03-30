//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDModuleScan.Core.Services
{
    public interface IFileService
    {
        string SaveText(string filename, string text);

        string LoadText(string filename);
    }
}
