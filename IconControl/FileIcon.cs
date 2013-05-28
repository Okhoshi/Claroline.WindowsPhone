using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IconControl
{
    public class FileIcon : ContentControl
    {
        public static readonly DependencyProperty ExtensionLabelProperty =
                    DependencyProperty.Register("ExtensionLabel", typeof(String), typeof(FileIcon), null);

        public static readonly DependencyProperty LabelForegroundProperty =
                    DependencyProperty.Register("LabelForeground", typeof(SolidColorBrush), typeof(FileIcon), null);

        public static readonly DependencyProperty PathColorProperty =
                    DependencyProperty.Register("PathColor", typeof(SolidColorBrush), typeof(FileIcon), null);

        public static readonly DependencyProperty CanvasTopProperty =
                    DependencyProperty.Register("CanvasTop", typeof(double), typeof(FileIcon), null);

        public static readonly DependencyProperty PathVisibilityProperty =
                    DependencyProperty.Register("PathVisibility", typeof(Visibility), typeof(FileIcon), null);

        public static readonly DependencyProperty ImageVisibilityProperty =
                    DependencyProperty.Register("ImageVisibility", typeof(Visibility), typeof(FileIcon), null);

        public static readonly DependencyProperty IconSourceProperty =
                    DependencyProperty.Register("IconSource", typeof(ImageSource), typeof(FileIcon), null);

        public FileIcon()
        {
            DefaultStyleKey = typeof(FileIcon);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            string source = null;
            string folder = @"/IconControl;component/Images" + ((Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible ? ".dark" : ".light");

            switch (ExtensionLabel.ToLower())
            {
                case "":
                    source = folder + @"/appbar.folder.png";
                    break;
                case "gz":
                case "bz2":
                case "zip":
                case "tar":
                case "rar":
                    source = folder + @"/appbar.archive.png";
                    break;
                case "url":
                case "htm":
                case "html":
                case "htx":
                case "swf":
                    source = folder + @"/appbar.link.png";
                    break;
                case "js":
                case "css":
                case "xsl":
                case "pl":
                case "plm":
                case "ml":
                case "lsp":
                case "cls":
                case "xml":
                    source = folder + @"/appbar.xml.png";
                    break;
                case "php":
                case "py":
                case "rb":
                case "c":
                case "h":
                case "cpp":
                case "java":
                    source = folder + @"/appbar.code.png";
                    break;
                case "tex":
                case "txt":
                case "rtf":
                    source = folder + @"/appbar.page.text.png";
                    break;
                case "pdf":
                    source = folder + @"/appbar.page.pdf.png";
                    break;
                case "ogg":
                case "wav":
                case "midi":
                case "mp2":
                case "mp3":
                case "mp4":
                case "vqf":
                    source = folder + @"/appbar.page.music.png";
                    break;

                case "avi":
                case "mpg":
                case "mpeg":
                case "mov":
                case "wmv":
                    source = folder + @"/appbar.film.png";
                    break;

                case "png":
                case "jpeg":
                case "jpg":
                case "xcf":
                case "gif":
                case "bmp":
                    source = folder + @"/appbar.page.image.png";
                    break;

                case "odt":
                case "doc":
                case "docx":
                case "dot":
                case "mcw":
                case "wps":
                    source = folder + @"/appbar.page.word.png";
                    break;

                case "ods":
                case "xls":
                case "xlsx":
                case "xlt":
                    source = folder + @"/appbar.page.excel.png";
                    break;

                case "odp":
                case "ppt":
                case "pptx":
                case "pps":
                    source = folder + @"/appbar.page.powerpoint.png";
                    break;
                default:
                    switch (ExtensionLabel.Length)
                    {
                        case 1:
                        case 2:
                        case 3:
                            FontSize = 11;
                            CanvasTop = 31;
                            break;
                        case 4:
                            FontSize = 9;
                            CanvasTop = 30;
                            break;
                        case 5:
                            FontSize = 7;
                            CanvasTop = 30;
                            break;
                        default:
                            FontSize = 5;
                            CanvasTop = 30;
                            break;
                    }
                    break;
            }

            if (String.IsNullOrEmpty(source))
            {
                ImageVisibility = Visibility.Collapsed;
                PathVisibility = Visibility.Visible;
            }
            else
            {
                ImageVisibility = Visibility.Visible;
                PathVisibility = Visibility.Collapsed;
                IconSource = new BitmapImage(new Uri(source, UriKind.Relative));
            }
        }

        public String ExtensionLabel
        {
            get { return base.GetValue(ExtensionLabelProperty) as String; }
            set { base.SetValue(ExtensionLabelProperty, value); }
        }

        public SolidColorBrush LabelForeground
        {
            get { return base.GetValue(LabelForegroundProperty) as SolidColorBrush; }
            set { base.SetValue(LabelForegroundProperty, value); }
        }

        public SolidColorBrush PathColor
        {
            get { return base.GetValue(PathColorProperty) as SolidColorBrush; }
            set { base.SetValue(PathColorProperty, value); }
        }

        internal double CanvasTop
        {
            get { return (double)base.GetValue(CanvasTopProperty); }
            set { base.SetValue(CanvasTopProperty, value); }
        }

        internal Visibility ImageVisibility
        {
            get { return (Visibility)base.GetValue(ImageVisibilityProperty); }
            set { base.SetValue(ImageVisibilityProperty, value); }
        }

        internal Visibility PathVisibility
        {
            get { return (Visibility)base.GetValue(PathVisibilityProperty); }
            set { base.SetValue(PathVisibilityProperty, value); }
        }

        public ImageSource IconSource
        {
            get { return base.GetValue(IconSourceProperty) as ImageSource; }
            set { base.SetValue(IconSourceProperty, value); }
        }

    }
}
