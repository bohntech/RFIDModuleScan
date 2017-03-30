//Licensed under MIT License see LICENSE.TXT in project root folder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Globalization;
using RFIDModuleScan.Core.Helpers;

namespace RFIDModuleScan.Converters
{
    public class InvalidIntStringTextColorConverter : IValueConverter
    {
        // Define a method to convert an invalid integer string to a color
        public object Convert(object value, Type targetType, object parameter, CultureInfo ci)
        {            
            if (ValidationHelper.ValidInt((string)value))
            {
                return "#000000";
            }
            else
            {
                return "#FF0000";
            }

        }

        // ConvertBack is not implemented for a OneWay binding.
        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo ci)
        {
            return string.Empty;
        }
    }
}

