using Helpers.Navigation;
using Helpers.Storage;
using InternetRadio.ViewModels;
using InternetRadio.Views;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace InternetRadio
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Current;
        public RadioViewModel RadioViewModel = RadioViewModel.Instance;

        public MainPage()
        {
            this.InitializeComponent();
            Current = this;
            App.Current.Resuming += Current_Resuming;
        }

        public NavigationLink[] NavigationLinks =
        {
            new NavigationLink("Radio", Symbol.Audio, new RadioView()),
            new NavigationLink("Settings", Symbol.Setting, new SettingsView())
        };

        /// <summary>
        /// Invoked when application is being resumed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Current_Resuming(object sender, object e)
        {
            // Update data
            await RadioViewModel.RefreshInternetRadioDataAsync();
        }

        /// <summary>
        /// Invoked when frame is navigated to this page.
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Update titlebar backbutton visibility
            NavigationService.UpdateAppViewBackButtonVisibility();
            // Update data
            await RadioViewModel.RefreshInternetRadioDataAsync();

            base.OnNavigatedTo(e);
        }
    }
}

