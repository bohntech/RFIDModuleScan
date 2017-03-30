//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Globalization;

namespace RFIDModuleScan.Converters
{
    public class BoolToArrowImageConverter : IValueConverter
    {
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo ci)
        {
            bool val = (bool)value;

            return (val) ? "downarrow.png" : "uparrow.png";
        }
                
        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo ci)
        {
            throw new NotImplementedException();
        }

    }
}