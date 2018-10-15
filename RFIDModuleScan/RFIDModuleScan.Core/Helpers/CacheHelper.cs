using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDModuleScan.Core.Helpers
{
    public static class CacheTimingHelper
    {
        private static DateTime? LastSync = null;

        private static double CACHE_MINUTES = 5.0;

        public static bool AllowSync
        {
            get
            {
                if (LastSync.HasValue) return LastSync.Value.AddMinutes(CACHE_MINUTES) <= DateTime.Now;
                else return true;
            }
        }

        public static void ResetSyncTime()
        {
            LastSync = null;
        }

        public static void RecordSyncTime()
        {
            LastSync = DateTime.Now;
        }
    }
}
