using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace FullScreenTitleBarRepo
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void OnChangeTitleBarBackgroundClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.Behavior.Foreground = new SolidColorBrush(Colors.BlanchedAlmond);
            this.Behavior.Background = new SolidColorBrush(Colors.LightPink);
        }
    }
}
