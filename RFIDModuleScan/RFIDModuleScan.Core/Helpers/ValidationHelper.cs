//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDModuleScan.Core.Helpers
{
    public class ValidationHelper
    {
        public static bool ValidInt(string val)
        {
            int temp;
            return (int.TryParse(val, out temp) && temp > 0);
        }
    }
}
