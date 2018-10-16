using InternetRadio.ViewModels;
using InternetRadioShared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

namespace InternetRadio.Views
{
    public sealed partial class BrowseView : UserControl
    {
        public RadioViewModel RadioViewModel = RadioViewModel.Instance;

        public BrowseView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Event for opening flyout menu of sender.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioItem_OpenFlyoutMenu(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }
    }
}
