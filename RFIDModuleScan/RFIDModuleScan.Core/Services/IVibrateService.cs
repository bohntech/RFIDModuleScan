using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDModuleScan.Core.Services
{
    public interface IVibrateService
    {
        void Vibrate(int milliseconds);
    }
}
