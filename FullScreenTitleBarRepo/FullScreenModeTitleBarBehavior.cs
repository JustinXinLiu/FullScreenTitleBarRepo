using Microsoft.Xaml.Interactivity;
using System;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace FullScreenTitleBarRepo
{
    public class FullScreenModeTitleBarBehavior : DependencyObject, IBehavior
    {
        static ApplicationViewTitleBar _nativeTitleBar = ApplicationView.GetForCurrentView().TitleBar;
        static FullScreenModeTitleBar _customTitleBar = new FullScreenModeTitleBar();
        Page _mainPage = null;

        public DependencyObject AssociatedObject { get; private set; }

        public void Attach(DependencyObject associatedObject)
        {
            _mainPage = associatedObject as Page;

            if (_mainPage == null)
            {
                throw new ArgumentException("The Associated Object needs to inherit from Panel!");
            }

            _mainPage.Loaded += OnMainPageLoaded;
        }

        void OnMainPageLoaded(object sender, RoutedEventArgs e)
        {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

            // Store the original main page content.
            var mainPageContent = _mainPage.Content;
            // Clear the content for now.
            _mainPage.Content = null;

            // Move the content of the main page to our title bar control.
            _customTitleBar.SetPageContent(mainPageContent);
            // Refill the content with our new title bar control.
            _mainPage.Content = _customTitleBar;

            UpdateBackground(this.Background);
            UpdateForeground(this.Foreground);
        }

        public void Detach()
        {
            _mainPage.Loaded -= OnMainPageLoaded;
        }

        #region Brushes

        public SolidColorBrush Background
        {
            get { return (SolidColorBrush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(SolidColorBrush), typeof(FullScreenModeTitleBarBehavior), new PropertyMetadata(new SolidColorBrush(Colors.White), OnBackgroundChanged));

        private static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateBackground((SolidColorBrush)e.NewValue);
        }

        public SolidColorBrush Foreground
        {
            get { return (SolidColorBrush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(SolidColorBrush), typeof(FullScreenModeTitleBarBehavior), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnForegroundChanged));

        private static void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateForeground((SolidColorBrush)e.NewValue);
        }

        #endregion

        private static void UpdateBackground(SolidColorBrush brush)
        {
            _nativeTitleBar.ButtonBackgroundColor = brush.Color;
            _nativeTitleBar.ButtonInactiveBackgroundColor = new Color() { A = 0 };
            _customTitleBar.Background = brush;
        }

        private static void UpdateForeground(SolidColorBrush brush)
        {
            _nativeTitleBar.ButtonForegroundColor = brush.Color;
            _nativeTitleBar.ButtonInactiveBackgroundColor = new Color() { A = 0 };
            _customTitleBar.Foreground = brush;
        }
    }
}
