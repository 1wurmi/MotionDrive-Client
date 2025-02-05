using Avalonia.Data.Converters;
using Avalonia.Svg.Skia;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionDrive.Desktop.Converter;
public class SvgStreamConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is string svgString)
        {
            // Convert the SVG string to a MemoryStream
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(svgString));
            return memoryStream;
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
