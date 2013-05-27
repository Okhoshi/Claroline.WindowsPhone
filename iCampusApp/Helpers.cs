using ClarolineApp.Languages;
using ClarolineApp.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
                return AppSettings.instance.PlatformSetting.ToUpperInvariant() + " - " + (string)value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)value).Replace(AppSettings.instance.PlatformSetting.ToUpperInvariant() + " - ", String.Empty);
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

    public class ExtSelector : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string img = "/Mime/";
            switch ((string)value)
            {
                case "":
                    img = "/icons/appbar.folder.rest" + ((Visibility)App.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible ? ".dark" : ".light");
                    break;

                case "gz":
                case "bz2":
                case "zip":
                case "tar":
                case "rar":
                    img += "package-x-generic";
                    break;

                case "pgp":
                    img += "text-x-pgp";
                    break;

                case "url":
                case "htm":
                case "html":
                case "htx":
                case "swf":
                    img += "link";
                    break;

                case "sh":
                case "exe":
                    img += "applications-system";
                    break;

                case "js":
                case "css":
                case "xsl":
                case "pl":
                case "plm":
                case "ml":
                case "lsp":
                case "cls":
                    img += "text-x-script";
                    break;

                case "php":
                    img += "application-x-php";
                    break;

                case "py":
                    img += "text-x-python";
                    break;

                case "rb":
                    img += "application-x-ruby";
                    break;

                case "c":
                case "h":
                case "cpp":
                case "java":
                    img += "text-x-code";
                    break;

                case "xml":
                case "tex":
                case "txt":
                case "rtf":
                    img += "text-x-generic";
                    break;

                case "pdf":
                    img += "pdf";
                    break;

                case "ps":
                    img += "x-office-document";
                    break;

                case "ogg":
                case "wav":
                case "midi":
                case "mp2":
                case "mp3":
                case "mp4":
                case "vqf":
                    img += "audio-x-generic";
                    break;

                case "avi":
                case "mpg":
                case "mpeg":
                case "mov":
                case "wmv":
                    img += "video-x-generic";
                    break;

                case "png":
                case "jpeg":
                case "jpg":
                case "xcf":
                case "gif":
                case "bmp":
                    img += "image-x-generic";
                    break;

                case "svg":
                case "odg":
                    img += "x-office-drawing";
                    break;

                case "odt":
                case "doc":
                case "docx":
                case "dot":
                case "mcw":
                case "wps":
                    img += "x-office-document";
                    break;

                case "ods":
                case "xls":
                case "xlsx":
                case "xlt":
                    img += "x-office-spreadsheet";
                    break;

                case "odp":
                case "ppt":
                case "pptx":
                case "pps":
                    img += "x-office-presentation";
                    break;

                case "odf":
                    img += "x-office-formula";
                    break;

                case "ttf":
                    img += "font-x-generic";
                    break;
                default:
                    img += "default";
                    break;
            }

            img += ".png";
            return img;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)parameter)
                return "";
            return "ext";
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
}