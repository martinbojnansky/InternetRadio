using InternetRadio.ViewModels;
using InternetRadioShared;
using System;
using System.Collections.Generic;
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

namespace InternetRadio.Views
{
    public sealed partial class SettingsView : UserControl
    {
        public RadioViewModel RadioViewModel = RadioViewModel.Instance;
        public string Port = Constants.Port.ToString();

        public SettingsView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Shows notification for required restart.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FindName("RestartRequiredTextBlock");
        }
    }
}
