using Helpers.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Helpers.Controls
{
    public sealed partial class HamburgerMenu : UserControl
    {
        public HamburgerMenu()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        #region NavigationLinksProperty

        public NavigationLink[] NavigationLinks
        {
            get { return GetValue(NavigationLinksProperty) as NavigationLink[]; }
            set { SetValue(NavigationLinksProperty, value); }
        }

        public static readonly DependencyProperty NavigationLinksProperty =
              DependencyProperty.Register(
                  "NavigationLinks", typeof(NavigationLink[]), typeof(HamburgerMenu), new PropertyMetadata(new NavigationLink[0])
                  );

        #endregion

        #region HeaderBackgroundProperty

        public SolidColorBrush HeaderBackground
        {
            get { return GetValue(HeaderBackgroundProperty) as SolidColorBrush; }
            set { SetValue(HeaderBackgroundProperty, value); }
        }

        public static readonly DependencyProperty HeaderBackgroundProperty =
              DependencyProperty.Register(
                  "HeaderBackground", typeof(SolidColorBrush), typeof(HamburgerMenu), new PropertyMetadata(null)
                  );

        #endregion

        #region HeaderForegroundProperty

        public SolidColorBrush HeaderForeground
        {
            get { return GetValue(HeaderForegroundProperty) as SolidColorBrush; }
            set { SetValue(HeaderForegroundProperty, value); }
        }

        public static readonly DependencyProperty HeaderForegroundProperty =
              DependencyProperty.Register(
                  "HeaderForeground", typeof(SolidColorBrush), typeof(HamburgerMenu), new PropertyMetadata(null)
                  );

        #endregion

        #region PaneBackgroundProperty

        public SolidColorBrush PaneBackground
        {
            get { return GetValue(PaneBackgroundProperty) as SolidColorBrush; }
            set { SetValue(PaneBackgroundProperty, value); }
        }

        public static readonly DependencyProperty PaneBackgroundProperty =
              DependencyProperty.Register(
                  "PaneBackground", typeof(SolidColorBrush), typeof(HamburgerMenu), new PropertyMetadata(null)
                  );

        #endregion

        #region PaneForegroundProperty

        public SolidColorBrush PaneForeground
        {
            get { return GetValue(PaneForegroundProperty) as SolidColorBrush; }
            set { SetValue(PaneForegroundProperty, value); }
        }

        public static readonly DependencyProperty PaneForegroundProperty =
              DependencyProperty.Register(
                  "PaneForeground", typeof(SolidColorBrush), typeof(HamburgerMenu), new PropertyMetadata(null)
                  );

        #endregion

        #region PaneBottomContentControlProperty

        public FrameworkElement PaneBottomContent
        {
            get { return GetValue(PaneBottomContentProperty) as FrameworkElement; }
            set { SetValue(PaneBottomContentProperty, value); }
        }

        public static readonly DependencyProperty PaneBottomContentProperty =
              DependencyProperty.Register(
                  "PaneBottomContent", typeof(FrameworkElement), typeof(HamburgerMenu), new PropertyMetadata(null)
                  );

        #endregion

        #region HeaderRightContentControlProperty

        public FrameworkElement HeaderRightContent
        {
            get { return GetValue(HeaderRightContentProperty) as FrameworkElement; }
            set { SetValue(HeaderRightContentProperty, value); }
        }

        public static readonly DependencyProperty HeaderRightContentProperty =
              DependencyProperty.Register(
                  "HeaderRightContent", typeof(FrameworkElement), typeof(HamburgerMenu), new PropertyMetadata(null)
                  );

        #endregion

        #region Control selection

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HamburgerMenuNavigationListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NavigationLink navLink = HamburgerMenuNavigationListView.SelectedItem as NavigationLink;
            HamburgerMenuSplitView.Content = navLink.Control;

            if (HamburgerMenuSplitView.DisplayMode == SplitViewDisplayMode.Overlay || HamburgerMenuSplitView.DisplayMode == SplitViewDisplayMode.CompactOverlay)
                HamburgerMenuToggleButton.IsChecked = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HamburgerMenuNavigationListView_Loaded(object sender, RoutedEventArgs e)
        {
            HamburgerMenuNavigationListView.SelectedIndex = 0;
        }

        #endregion
    }
}
