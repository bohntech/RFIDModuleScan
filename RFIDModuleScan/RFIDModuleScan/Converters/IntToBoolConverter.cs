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
    public class IntToBoolConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo ci)
        {
            int val = (int)value;

            return (val > 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo ci)
        {
            throw new NotImplementedException();
        }

    }
}