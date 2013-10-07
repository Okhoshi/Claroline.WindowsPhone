using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ClarolineApp.RT.Controls
{
    public sealed class CharmFrame : Frame
    {
        public CharmFrame()
        {
            this.DefaultStyleKey = typeof(CharmFrame);
        }

        public object CharmContent
        {
            get { return (object)GetValue(CharmContentProperty); }
            set { SetValue(CharmContentProperty, value); }
        }

        public static readonly DependencyProperty CharmContentProperty =
            DependencyProperty.Register("CharmContent", typeof(object), typeof(CharmFrame), new PropertyMetadata(null));
    }
}
