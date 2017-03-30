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
    public class InvalidIntStringColorConverter : IValueConverter
    {        

        // Define a method to convert an invalid integer string to a color
        public object Convert(object value, Type targetType, object parameter, CultureInfo ci)
        {          
            if (ValidationHelper.ValidInt((string)value))
            {
                return "Transparent";
            }
            else
            {
                return "#e7c3c3";
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