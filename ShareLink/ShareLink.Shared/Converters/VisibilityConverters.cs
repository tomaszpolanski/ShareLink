using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ShareLink.Converters
{
    /// <summary>
    /// Value converter that translates true to <see cref="Visibility.Visible"/> and false to
    /// <see cref="Visibility.Collapsed"/>.
    /// </summary>
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Debug.Assert(value is bool);
            return (value is bool && (bool) value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Debug.Assert(value is Visibility);
            return value is Visibility && (Visibility) value == Visibility.Visible;
        }
    }


    /// <summary>
    /// Provides inverse value of the BooleanToVisibilityConverter
    /// </summary>
    public sealed class InvertedBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Debug.Assert(value is bool);
            return (value is bool && (bool) value) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Debug.Assert(value is Visibility);
            return value is Visibility && (Visibility) value != Visibility.Visible;
        }
    }

    public sealed class InvertedBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Debug.Assert(value is bool);
            return (!(value is bool) || !(bool) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Debug.Assert(value is bool);
            return value is bool && (bool) value;
        }
    }

    /// <summary>
    /// Value converter that translates an integer count to visibility (0 for hiding, non 0 for showing).
    /// </summary>
    public sealed class CountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Debug.Assert(value is int);
            return value is int && (int) value != 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Provides the inverse value of CountToVisibilityConverter
    /// </summary>
    public sealed class InvertedCountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Debug.Assert(value is int);
            return value is int && (int) value == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Provides the inverse value of Visibility
    /// </summary>
    public sealed class InvertedVisibilityToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Debug.Assert(value is Visibility);
            return value is Visibility && (Visibility) value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Value converter that translates an empty string to hidden and non empty to a shown.
    /// </summary>
    public sealed class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Value converter that translates an empty string to shown and non empty to hidden.
    /// </summary>
    public sealed class InvertedStringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return string.IsNullOrEmpty(value as string) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }

    public sealed class ObjectToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return IsVisible(value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }

        internal static bool IsVisible(object value)
        {
            var str = value as string;
            if (str != null)
            {
                return !string.IsNullOrEmpty(str);
            }
            return value != null;
        }
    }

    public sealed class InvertedObjectToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ObjectToVisibilityConverter.IsVisible(value) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }

    public sealed class GreaterThanVisibilityConverter : IValueConverter
    {
        public int GreaterThan { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Debug.Assert(value is int);
            return value is int && (int) value > GreaterThan ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}