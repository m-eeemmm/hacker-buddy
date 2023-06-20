using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace hacker_buddy_app
{
    public class StringToImageConverter : IValueConverter
    {
        public StringToImageConverter()
        {

        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string)
            {
                // Create a BitmapSource  
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                string p = Path.Combine(".", "Data", "Img", value + ".png");
                bool result=    File.Exists(p);
                bitmap.UriSource = new Uri(Path.GetFullPath(p));
                bitmap.EndInit();

                return bitmap;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
