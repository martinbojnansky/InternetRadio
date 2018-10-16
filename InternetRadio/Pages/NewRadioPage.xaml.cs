using Helpers.Navigation;
using InternetRadio.ViewModels;
using InternetRadioShared.Helpers;
using InternetRadioShared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace InternetRadio.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewRadioPage : Page
    {
        public RadioViewModel RadioViewModel = RadioViewModel.Instance;
        public string Title = "";
        public string StreamUrl = "http://";

        public NewRadioPage()
        {
            this.InitializeComponent();
            this.Loaded += NewRadioPage_Loaded;
        }

        private void NewRadioPage_Loaded(object sender, RoutedEventArgs e)
        {
            TitleTextBox.Focus(FocusState.Programmatic);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Update titlebar backbutton visibility
            NavigationService.UpdateAppViewBackButtonVisibility();
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Saves radio.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await RadioViewModel.AddRadioAsync(Title, StreamUrl);
                NavigationService.NavigateBack();
            }
            catch
            {
                MessageDialog dlg = new MessageDialog("Radio could not be created. Check network connection, stream url and try again please.");
                await dlg.ShowAsync();
            }
        }
    }
}
