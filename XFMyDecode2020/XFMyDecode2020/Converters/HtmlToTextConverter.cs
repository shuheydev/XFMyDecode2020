using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XFMyDecode2020.Converters
{
    public class HtmlToTextConverter : IValueConverter, IMarkupExtension
    {
        //Source→View
        //ViewModelやコードビハインドからXaml側へ
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                throw new ArgumentException();
            }

            var targetValue = (string)value;

            return Regex.Replace(targetValue, "<br/>", Environment.NewLine);
        }

        //View→Source
        //Xaml側からViewModelやコードビハインドへ
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
