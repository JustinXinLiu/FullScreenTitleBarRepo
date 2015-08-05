using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FullScreenTitleBarRepo
{
    /// <summary>
    /// This class was heavily inspired by this GitHub sample -
    /// https://github.com/Microsoft/Windows-universal-samples/tree/master/Samples/TitleBar
    /// </summary>
    public sealed partial class FullScreenModeTitleBar : UserControl
    {
        CoreApplicationViewTitleBar _coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
        UIElement _mainPageContent = null;

        public FullScreenModeTitleBar()
        {
            this.InitializeComponent();

            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;

            // Clicks on the BackgroundElement will be treated as clicks on the title bar.
            Window.Current.SetTitleBar(BackgroundElement);
        }

        #region CoreTitleBarHeight dp

        public double CoreTitleBarHeight
        {
            get { return (double)GetValue(CoreTitleBarHeightProperty); }
            set { SetValue(CoreTitleBarHeightProperty, value); }
        }

        public static readonly DependencyProperty CoreTitleBarHeightProperty =
            DependencyProperty.Register("CoreTitleBarHeight", typeof(double), typeof(FullScreenModeTitleBar), new PropertyMetadata(0d));

        #endregion

        #region CoreTitleBarPadding db

        public Thickness CoreTitleBarPadding
        {
            get { return (Thickness)GetValue(CoreTitleBarPaddingProperty); }
            set { SetValue(CoreTitleBarPaddingProperty, value); }
        }

        public static readonly DependencyProperty CoreTitleBarPaddingProperty =
            DependencyProperty.Register("CoreTitleBarPadding", typeof(Thickness), typeof(FullScreenModeTitleBar), new PropertyMetadata(default(Thickness)));

        #endregion

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            // When the app window moves to a different screen
            _coreTitleBar.LayoutMetricsChanged += OnTitleBarLayoutMetricsChanged;

            // When in full screen mode, the title bar is collapsed by default.
            _coreTitleBar.IsVisibleChanged += OnTitleBarIsVisibleChanged;

            // The SizeChanged event is raised when the view enters or exits full screen mode.
            Window.Current.SizeChanged += OnWindowSizeChanged;

            this.UpdateLayoutMetrics();
            this.UpdatePositionAndVisibility();

            this.AppName.Text = Package.Current.DisplayName;
        }

        void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _coreTitleBar.LayoutMetricsChanged -= OnTitleBarLayoutMetricsChanged;
            _coreTitleBar.IsVisibleChanged -= OnTitleBarIsVisibleChanged;
            Window.Current.SizeChanged -= OnWindowSizeChanged;
        }

        void OnTitleBarLayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            this.UpdateLayoutMetrics();
        }

        void OnTitleBarIsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            this.UpdatePositionAndVisibility();
        }

        void OnWindowSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            this.UpdatePositionAndVisibility();
        }

        private void UpdateLayoutMetrics()
        {
            this.CoreTitleBarHeight = _coreTitleBar.Height;

            // The SystemOverlayLeftInset and SystemOverlayRightInset values are
            // in terms of physical left and right. Therefore, we need to flip
            // then when our flow direction is RTL.
            if (FlowDirection == FlowDirection.LeftToRight)
            {
                this.CoreTitleBarPadding = new Thickness()
                {
                    Left = _coreTitleBar.SystemOverlayLeftInset,
                    Right = _coreTitleBar.SystemOverlayRightInset
                };
            }
            else
            {
                this.CoreTitleBarPadding = new Thickness()
                {
                    Left = _coreTitleBar.SystemOverlayRightInset,
                    Right = _coreTitleBar.SystemOverlayLeftInset
                };
            }
        }

        // We wrap the normal contents of the MainPage inside a grid
        // so that we can place a title bar on top of it.
        //
        // When not in full screen mode, the grid looks like this:
        //
        //      Row 0: Custom-rendered title bar
        //      Row 1: Rest of content
        //
        // In full screen mode, the the grid looks like this:
        //
        //      Row 0: (empty)
        //      Row 1: Custom-rendered title bar
        //      Row 1: Rest of content
        //
        // The title bar is either Visible or Collapsed, depending on the value of
        // the IsVisible property.
        void UpdatePositionAndVisibility()
        {
            if (ApplicationView.GetForCurrentView().IsFullScreenMode)
            {
                // In full screen mode, the title bar overlays the content.
                // and might or might not be visible.
                TitleBar.Visibility = _coreTitleBar.IsVisible ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
                Grid.SetRow(TitleBar, 1);

                // As there's already a button for exiting full screen mode,
                // we simply hide our custom full screen button here.
                // Also, if you use this button to exit the full screen mode,
                // the three default buttons will stop working, this might be a bug...
                this.FullScreenModeToggle.Visibility = Visibility.Collapsed;
            }
            else
            {
                // When not in full screen mode, the title bar is visible and does not overlay content.
                TitleBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                Grid.SetRow(TitleBar, 0);

                this.FullScreenModeToggle.Visibility = Visibility.Visible;
            }
        }

        public UIElement SetPageContent(UIElement newContent)
        {
            UIElement oldContent = _mainPageContent;

            if (oldContent != null)
            {
                _mainPageContent = null;
                RootGrid.Children.Remove(oldContent);
            }

            _mainPageContent = newContent;

            if (newContent != null)
            {
                RootGrid.Children.Add(newContent);
                // The page content is row 1 in our grid. (See diagram above.)
                Grid.SetRow((FrameworkElement)_mainPageContent, 1);
            }

            return oldContent;
        }

        private void OnFullScreenModeToggleClick(object sender, RoutedEventArgs e)
        {
            var view = ApplicationView.GetForCurrentView();

            if (view.IsFullScreenMode)
            {
                view.ExitFullScreenMode();
            }
            else
            {
                view.TryEnterFullScreenMode();
            }
        }
    }
}
