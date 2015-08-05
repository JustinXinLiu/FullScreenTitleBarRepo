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
    public class AddFullScreenModeButtonToTitleBarBehavior : DependencyObject, IBehavior
    {
        static ApplicationViewTitleBar _coreTitleBar = ApplicationView.GetForCurrentView().TitleBar;
        static FullScreenTitleBar _customTitleBar = new FullScreenTitleBar();
        Page _mainPage = null;

        public DependencyObject AssociatedObject { get; private set; }

        public void Attach(DependencyObject associatedObject)
        {
            _mainPage = associatedObject as Page;

            if (_mainPage == null)
            {
                throw new ArgumentException("The Associated Object needs to inherit from Panel!");
            }

            _mainPage.Loaded += OnMainPageLoad;
        }

        void OnMainPageLoad(object sender, RoutedEventArgs e)
        {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

            var mainContent = _mainPage.Content;
            _mainPage.Content = null;

            _customTitleBar.SetPageContent(mainContent);
            _mainPage.Content = _customTitleBar;

            // Coloring
            _coreTitleBar.ButtonBackgroundColor = this.Background.Color;
            _customTitleBar.Background = this.Background;
            _coreTitleBar.ButtonForegroundColor = this.Foreground.Color;
            _customTitleBar.Foreground = this.Foreground;
        }

        public void Detach()
        {
            _mainPage.Loaded -= OnMainPageLoad;
        }

        #region Brushes

        public SolidColorBrush Background
        {
            get { return (SolidColorBrush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(SolidColorBrush), typeof(AddFullScreenModeButtonToTitleBarBehavior), new PropertyMetadata(new SolidColorBrush(Colors.White), OnBackgroundChanged));

        private static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var brush = (SolidColorBrush)e.NewValue;
            _customTitleBar.Background = brush;
            _coreTitleBar.ButtonBackgroundColor = brush.Color;
        }

        public SolidColorBrush Foreground
        {
            get { return (SolidColorBrush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(SolidColorBrush), typeof(AddFullScreenModeButtonToTitleBarBehavior), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnForegroundChanged));

        private static void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var brush = (SolidColorBrush)e.NewValue;
            _customTitleBar.Foreground = brush;
            _coreTitleBar.ButtonForegroundColor = brush.Color;
        }

        #endregion
    }
}
