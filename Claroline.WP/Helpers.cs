using ClarolineApp.Languages;
using ClarolineApp.Model;
using ClarolineApp.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml.Linq;

namespace ClarolineApp
{
    public class ToLower : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as String).ToLower(culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class ToUpper : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as String).ToUpper(culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush scb;
            switch (parameter as string)
            {
				case "White":
					scb = new SolidColorBrush(Colors.White);
					break;
                case "Black":
                    scb = new SolidColorBrush(Colors.Black);
                    break;
                default:
                    scb = App.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush;
                    break;
            }
            var ret = ((bool)value) ? App.Current.Resources["PhoneAccentBrush"] : scb;
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as SolidColorBrush == App.Current.Resources["PhoneAccentBrush"]);
        }
    }

    public class PostSubtitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return String.Format(AppLanguage.Forum_PostInTopicCount, (value as Topic).Posts.Count, (value as Topic).Views);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class CoursPageTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DesignerProperties.IsInDesignTool)
            {
                return ("Claromobile").ToUpperInvariant() + " - " + (string)value;
            }
            else
            {
                return AppSettings.Instance.PlatformSetting.ToUpperInvariant() + " - " + (string)value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)value).Replace(AppSettings.Instance.PlatformSetting.ToUpperInvariant() + " - ", String.Empty);
        }
    }

    public class SizeFileConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Double div = (double)value;
            if (div < 1) return String.Empty;
            div /= Double.Parse("1E+9");
            if (div > 1)
                return Math.Round(div, 2).ToString() + " Go";
            else
            {
                div *= 1000;
                if (div > 1)
                    return Math.Round(div,2).ToString() + " Mo";
                else
                {
                    div *= 1000;
                    if (div > 1)
                        return Math.Round(div, 2).ToString() + " Ko";
                }
            }
            return Math.Round((double)value, 2).ToString() + " o";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (String.IsNullOrEmpty((string)value)) return Double.Parse("0.0");
            string[] partialSize = ((string)value).Split(' ');
            switch (partialSize[1])
            {
                case "Ko":
                    partialSize[1] = "E+3";
                    break;
                case "Mo":
                    partialSize[1] = "E+6";
                    break;
                case "Go":
                    partialSize[1] = "E+9";
                    break;
                default:
                    partialSize[1] = "";
                    break;
            }
            return Double.Parse(partialSize[0] + partialSize[1]);
        }
    }

    public class DescVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return String.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }

    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool) value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible;
        }
    }

    public class TypeVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Type.GetType(parameter as string).IsInstanceOfType(value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new object();
        }
    }

    public class Helper
    {

		public static String GetVersionNumber(bool full = false)
		{
			try
			{
				var app = XElement.Load("WMAppManifest.xml");
				var version = app.Element("App").Attribute("Version").Value;
				return full?version:version.Substring(0, version.IndexOf('.', version.IndexOf('.')+1));
			}
			catch {
				return "";
			}
		}

        public static T FindFirstElementInVisualTree<T>(DependencyObject parentElement) where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(parentElement);
            if (count == 0)
                return null;

            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parentElement, i);

                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    var result = FindFirstElementInVisualTree<T>(child);
                    if (result != null)
                        return result;

                }
            }
            return null;
        }
    }

    public class Group<T> : IEnumerable<T>
    {
        public Group(string name, IEnumerable<T> items)
        {
            this.Title = name;
            this.Items = new List<T>(items);
        }

        public override bool Equals(object obj)
        {
            Group<T> that = obj as Group<T>;

            return (that != null) && (this.Title.Equals(that.Title));
        }

        public override int GetHashCode()
        {
            return Title.GetHashCode();
        }

        public string Title
        {
            get;
            set;
        }

        public IList<T> Items
        {
            get;
            set;
        }

        public bool HasItems
        {
            get
            {
                return Items.Count > 0;
            }
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        #endregion
    }

    public class DebugStreamWriter : TextWriter
    {
        private const int DefaultBufferSize = 256;
        private StringBuilder _buffer;

        public DebugStreamWriter()
        {
            BufferSize = 256;
            _buffer = new StringBuilder(BufferSize);
        }

        public int BufferSize
        {
            get;
            private set;
        }

        public override System.Text.Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }

        #region StreamWriter Overrides
        public override void Write(char value)
        {
            _buffer.Append(value);
            if (_buffer.Length >= BufferSize)
                Flush();
        }

        public override void WriteLine(string value)
        {
            Flush();

            using (var reader = new StringReader(value))
            {
                string line;
                while (null != (line = reader.ReadLine()))
                    Debug.WriteLine(line);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Flush();
        }

        public override void Flush()
        {
            if (_buffer.Length > 0)
            {
                Debug.WriteLine(_buffer);
                _buffer.Clear();
            }
        }
        #endregion
    }
}