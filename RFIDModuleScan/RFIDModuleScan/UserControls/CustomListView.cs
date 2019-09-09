using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RFIDModuleScan.UserControls
{
    public class CustomListView : ListView
    {
        public CustomListView() : base(GetCachingStrategy()) { }

        public CustomListView(ListViewCachingStrategy cachingStrategy) : base(cachingStrategy) { }

        static ListViewCachingStrategy GetCachingStrategy()
        {
            switch (Device.OS)
            {
                case TargetPlatform.Android:
                    return ListViewCachingStrategy.RecycleElement;
                default:
                    return ListViewCachingStrategy.RetainElement;
            }
        }
    }
}
