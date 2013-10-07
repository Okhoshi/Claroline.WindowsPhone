using System.Windows.Input;
using Windows.UI;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace ClarolineApp.RT.Controls
{
    [ContentProperty(Name = "Content")]
    public sealed class CharmFlyout : ContentControl
    {
        public CharmFlyout()
        {
            DefaultStyleKey = typeof(CharmFlyout);
            FlyoutWidth = 346; // or 646
            BackCommand = new RelayCommand(OnBack);
            this.SizeChanged += OnSizeChanged;
            HeadingBackgroundBrush = new SolidColorBrush(Colors.Black);
            HeadingForegroundBrush = new SolidColorBrush(Colors.White);
            ContentBackgroundBrush = new SolidColorBrush(Colors.White);
            ContentForegroundBrush = new SolidColorBrush(Colors.Black);
        }

        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            FlyoutHeight = e.NewSize.Height;
        }

        private void OnIsOpenChanged()
        {
            if (ParentFlyout != null && IsOpen)
            {
                ParentFlyout.IsOpen = false;
            }
        }

        private void OnBack(object obj)
        {
            IsOpen = false;

            if (ParentFlyout != null)
            {
                ParentFlyout.IsOpen = true;
            }
            else
            {
                SettingsPane.Show();
            }
        }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, false); SetValue(IsOpenProperty, value); } // this set to false first fixes a wierd dissapearance bug that happens when you click to the way to the right of "My Flyout"
        }

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(CharmFlyout), new PropertyMetadata(null, OnIsOpenChanged));

        public CharmFlyout ParentFlyout
        {
            get { return (CharmFlyout)GetValue(ParentFlyoutProperty); }
            set { SetValue(ParentFlyoutProperty, value); }
        }

        public static readonly DependencyProperty ParentFlyoutProperty =
            DependencyProperty.Register("ParentFlyout", typeof(CharmFlyout), typeof(CharmFlyout), new PropertyMetadata(null));

        public double FlyoutHeight
        {
            get { return (double)GetValue(FlyoutHeightProperty); }
            private set { SetValue(FlyoutHeightProperty, value); }
        }

        public static readonly DependencyProperty FlyoutHeightProperty =
            DependencyProperty.Register("FlyoutHeight", typeof(double), typeof(CharmFlyout), new PropertyMetadata(null));

        public double FlyoutWidth
        {
            get { return (double)GetValue(FlyoutWidthProperty); }
            set { SetValue(FlyoutWidthProperty, value); }
        }

        public static readonly DependencyProperty FlyoutWidthProperty =
            DependencyProperty.Register("FlyoutWidth", typeof(double), typeof(CharmFlyout), new PropertyMetadata(null));

        public string Heading
        {
            get { return (string)GetValue(HeadingProperty); }
            set { SetValue(HeadingProperty, value); }
        }

        public static readonly DependencyProperty HeadingProperty =
            DependencyProperty.Register("Heading", typeof(string), typeof(CharmFlyout), new PropertyMetadata(null));

        public ICommand BackCommand
        {
            get { return (ICommand)GetValue(BackCommandProperty); }
            private set { SetValue(BackCommandProperty, value); }
        }

        public static readonly DependencyProperty BackCommandProperty =
            DependencyProperty.Register("BackCommand", typeof(ICommand), typeof(CharmFlyout), new PropertyMetadata(null));

        public Brush HeadingForegroundBrush
        {
            get { return (Brush)GetValue(HeadingForegroundBrushProperty); }
            set { SetValue(HeadingForegroundBrushProperty, value); }
        }

        public static readonly DependencyProperty HeadingForegroundBrushProperty =
            DependencyProperty.Register("HeadingForegroundBrush", typeof(Brush), typeof(CharmFlyout), new PropertyMetadata(null));

        public Brush HeadingBackgroundBrush
        {
            get { return (Brush)GetValue(HeadingBackgroundBrushProperty); }
            set { SetValue(HeadingBackgroundBrushProperty, value); }
        }

        public static readonly DependencyProperty HeadingBackgroundBrushProperty =
            DependencyProperty.Register("HeadingBackgroundBrush", typeof(Brush), typeof(CharmFlyout), new PropertyMetadata(null));

        public Brush ContentForegroundBrush
        {
            get { return (Brush)GetValue(ContentForegroundBrushProperty); }
            set { SetValue(ContentForegroundBrushProperty, value); }
        }

        public static readonly DependencyProperty ContentForegroundBrushProperty =
            DependencyProperty.Register("ContentForegroundBrush", typeof(Brush), typeof(CharmFlyout), new PropertyMetadata(null));

        public Brush ContentBackgroundBrush
        {
            get { return (Brush)GetValue(ContentBackgroundBrushProperty); }
            set { SetValue(ContentBackgroundBrushProperty, value); }
        }

        public static readonly DependencyProperty ContentBackgroundBrushProperty =
            DependencyProperty.Register("ContentBackgroundBrush", typeof(Brush), typeof(CharmFlyout), new PropertyMetadata(null));

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var charmFlyout = d as CharmFlyout;
            if (charmFlyout != null)
            {
                charmFlyout.OnIsOpenChanged();
            }
        }
    }
}
