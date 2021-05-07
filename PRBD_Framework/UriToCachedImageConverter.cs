using System;
using System.IO;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace PRBD_Framework {
    // see: http://www.thejoyofcode.com/WPF_Image_element_locks_my_local_file.aspx
    public class UriToCachedImageConverter : MarkupExtension, IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == null || !File.Exists(value.ToString()))
                return null;

            if (!string.IsNullOrEmpty(value.ToString())) {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(value.ToString());
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bi.EndInit();
                bi.Freeze();
                return bi;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException("Two way conversion is not supported.");
        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }
}
