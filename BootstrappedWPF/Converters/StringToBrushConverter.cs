namespace BootstrappedWPF.Converters {
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    using BootstrappedWPF.Models;
    using BootstrappedWPF.Utilities;

    using NLog;

    public class StringToBrushConverter : IValueConverter {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            BrushConverter brushConverter = new BrushConverter();
            value = value.ToString().StartsWith("#")
                        ? value
                        : "#" + value;
            Brush result = (Brush) brushConverter.ConvertFrom("#FFFFFFFF");
            try {
                result = (Brush) brushConverter.ConvertFrom(value);
            }
            catch (Exception ex) {
                Logging.Log(Logger, new LogItem(ex, true));
            }

            return result;
        }

        public object Convert(object value) {
            BrushConverter brushConverter = new BrushConverter();
            value = value.ToString()?.Substring(0, 1) == "#"
                        ? value
                        : "#" + value;
            Brush result = (Brush) brushConverter.ConvertFrom("#FFFFFFFF");
            try {
                result = (Brush) brushConverter.ConvertFrom(value);
            }
            catch (Exception ex) {
                Logging.Log(Logger, new LogItem(ex, true));
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return new BrushConverter().ConvertFrom("#FFFFFFFF");
        }
    }
}