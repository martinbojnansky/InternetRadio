using Helpers.Storage;
using Helpers.ViewModel;
using InternetRadioShared.Models;
using InternetRadioShared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Helpers.Navigation;
using InternetRadio.Pages;
using Helpers.Controls;
using InternetRadioShared.Helpers;
using InternetRadio.Views;
using System.Collections.ObjectModel;
using Windows.UI.Popups;

namespace InternetRadio.ViewModels
{
    public partial class RadioViewModel : ViewModelBase
    {
        // This special pattern to prevent only one instance of this class
        #region Singleton

        private static readonly RadioViewModel instance = new RadioViewModel();
        private RadioViewModel()
        {
            _socketClient = new SocketClient(HostName, Constants.Port);
        }
        public static RadioViewModel Instance
        {
            get
            {
                return instance;
            }
        }

        #endregion

        // Client for communication with server
        #region SocketClient

        private SocketClient _socketClient;
        public string HostName
        {
            get
            {
                try { return LocalSettingsHelper.GetValue("HostName") as string; }
                catch { return Constants.HostName; }
            }
            set { LocalSettingsHelper.SetValue("HostName", value); }
        }

        /// <summary>
        /// Disconnects client.
        /// </summary>
        public void Disconnect()
        {
            _socketClient.Disconnect();
        }

        #endregion

        // Progress overlay for long tasks
        #region ProgressOverlay

        public ProgressObject ProgressObject = new ProgressObject();

        #endregion

        // Provided commands for audio manipulation
        #region BackgroundAudioManipulation

        #region Play/Pause

        /// <summary>
        /// Click event of play/pause button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void PlayPauseButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            switch (InternetRadioStatus.State)
            {
                case Windows.Media.Playback.MediaPlayerState.Buffering:
                case Windows.Media.Playback.MediaPlayerState.Playing:
                case Windows.Media.Playback.MediaPlayerState.Opening:
                    await StopRadioAsync();
                    break;
                case Windows.Media.Playback.MediaPlayerState.Closed:
                case Windows.Media.Playback.MediaPlayerState.Paused:
                case Windows.Media.Playback.MediaPlayerState.Stopped:
                    if (InternetRadioStatus.CurrentRadio != null)
                        await PlayRadioAsync(InternetRadioStatus.CurrentRadio);
                    break;
            }
        }

        /// <summary>
        /// Click event of listview item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void RadiosListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Get clicked radio from parameters
            Radio r = e.ClickedItem as Radio;
            if (r == null)
                return;

            // In the mobile state navigate to NowPlayingView
            RadioView.Current.SetRadiosPivotSelectedItem(0);
            // Start playing radio
            await PlayRadioAsync(r);
        }

        /// <summary>
        /// Plays radio.
        /// </summary>
        /// <param name="radio"></param>
        /// <returns></returns>
        public async Task PlayRadioAsync(Radio radio)
        {
            try
            {
                object[] parameters = { radio.Id }; 
                var response = await _socketClient.SendCommand(new SocketCommand(SocketCommand.CommandType.PLAY, parameters));
                if (response.Status == SocketResponse.StatusCode.EXCEPTION)
                    throw new Exception(response.Response);
            }
            catch { }
        }

        /// <summary>
        /// Stops radio.
        /// </summary>
        /// <returns></returns>
        public async Task StopRadioAsync()
        {
            try
            {
                var response = await _socketClient.SendCommand(new SocketCommand(SocketCommand.CommandType.STOP));
                if (response.Status == SocketResponse.StatusCode.EXCEPTION)
                    throw new Exception(response.Response);
            }
            catch { }
        }

        #endregion

        #region Volume

        /// <summary>
        /// Sets volume.
        /// </summary>
        /// <param name="volume"></param>
        /// <returns></returns>
        public async Task SetVolumeAsync(double volume)
        {
            try
            {
                object[] parameters = { volume };
                var response = await _socketClient.SendCommand(new SocketCommand(SocketCommand.CommandType.SET_VOLUME, parameters));
                if (response.Status == SocketResponse.StatusCode.EXCEPTION)
                    throw new Exception(response.Response);
            }
            catch { }
        }

        #endregion

        #endregion

        // Provided commands for radios management
        #region InternetRadioManipulation

        private InternetRadioStatus _internetRadioStatus = new InternetRadioStatus();
        public InternetRadioStatus InternetRadioStatus
        {
            get
            {
                return _internetRadioStatus;
            }
            set
            {
                _internetRadioStatus = value;
                RaisePropertyChanged(nameof(InternetRadioStatus));
                NowPlayingView.Current.UpdatePlayPauseState();
            }
        }

        private ObservableCollection<Radio> _radios;
        public ObservableCollection<Radio> Radios
        {
            get
            {
                return _radios;
            }
            set
            {
                _radios = value;
                RaisePropertyChanged(nameof(Radios));
            }
        }

        private bool _isOffline = true;
        public bool IsOffline
        {
            get
            {
                return _isOffline;
            }
            set
            {
                _isOffline = value;
                RaisePropertyChanged(nameof(IsOffline));
            }
        }

        #region Update

        /// <summary>
        /// Refreshes all related data for internet radio.
        /// </summary>
        public async void RefreshInternetRadioData()
        {
            await RefreshInternetRadioDataAsync();
        }
        public async Task RefreshInternetRadioDataAsync()
        {
            await UpdateRadiosAsync();
            StartUpdateTimer();
        }

        /// <summary>
        /// Updates internet radio status.
        /// </summary>
        /// <returns></returns>
        private async Task UpdateStatusAsync(int connectionTimeout = 2000)
        {
            try
            {
                do
                {
                    var response = await _socketClient.SendCommand(new SocketCommand(SocketCommand.CommandType.GET_STATUS), connectionTimeout);
                    if (response.Status == SocketResponse.StatusCode.EXCEPTION)
                        throw new Exception(response.Response);
                    InternetRadioStatus = JsonHelper.FromJson<InternetRadioStatus>(response.Response);
                } while (InternetRadioStatus.State == Windows.Media.Playback.MediaPlayerState.Buffering
                    || InternetRadioStatus.State == Windows.Media.Playback.MediaPlayerState.Opening);
                IsOffline = false;
            }
            catch { IsOffline = true; }
        }

        /// <summary>
        /// Updates internet radios dictionary.
        /// </summary>
        /// <returns></returns>
        private async Task UpdateRadiosAsync()
        {
            try
            {
                var response = await _socketClient.SendCommand(new SocketCommand(SocketCommand.CommandType.GET_RADIOS));
                if (response.Status == SocketResponse.StatusCode.EXCEPTION)
                    throw new Exception(response.Response);

                Radios = new ObservableCollection<Radio>(JsonHelper.FromJson<List<Radio>>(response.Response));
            }
            catch { }
        }

        #region UpdateTimer

        private DispatcherTimer _UpdateTimer;

        /// <summary>
        /// Starts periodic status updates
        /// </summary>
        public void StartUpdateTimer()
        {
            if (_UpdateTimer == null)
            {
                _UpdateTimer = new DispatcherTimer();
                _UpdateTimer.Tick += _UpdateTimer_Tick;
                _UpdateTimer.Interval = TimeSpan.FromSeconds(0.5);
            }
            if (!_UpdateTimer.IsEnabled)
                _UpdateTimer.Start();
        }

        /// <summary>
        /// Pauses periodic status updates
        /// </summary>
        private void PauseUpdateTimer()
        {
            if (_UpdateTimer == null)
                return;
            if (_UpdateTimer.IsEnabled)
                _UpdateTimer.Stop();
        }

        /// <summary>
        /// Handles periodic status update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void _UpdateTimer_Tick(object sender, object e)
        {
            PauseUpdateTimer();
            await UpdateStatusAsync(500);
            if (Radios == null)
                await UpdateRadiosAsync();
            StartUpdateTimer();
        }

        #endregion

        #endregion

        #region Create radio

        /// <summary>
        /// Navigates to Radio page.
        /// </summary>
        public void GoToNewRadioPage()
        {
            try { NavigationService.Navigate(typeof(NewRadioPage)); } catch { }
        }

        /// <summary>
        /// Adds radio to internet radios dictionary.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public async Task AddRadioAsync(string title, string stream)
        {
            ProgressObject.Show("Saving radio");
            try
            {
                Radio radio = new Radio(title, stream);
                object[] parameters = { JsonHelper.ToJson<Radio>(radio) };
                var response = await _socketClient.SendCommand(new SocketCommand(SocketCommand.CommandType.ADD_RADIO, parameters));
                if (response.Status == SocketResponse.StatusCode.EXCEPTION)
                    throw new Exception(response.Response);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally { ProgressObject.Hide(); }
        }

        #endregion

        #region Delete

        /// <summary>
        /// Flyout handler for deleting radio.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public async void DeleteRadioMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProgressObject.Show("Deleting radio");
                FrameworkElement element = (FrameworkElement)sender;
                Radio radio = (Radio)element.DataContext;
                await DeleteRadioAsync(radio);
                await UpdateRadiosAsync();
            }
            catch
            {
                MessageDialog dlg = new MessageDialog("Radio could not be deleted. Check network connection and try again please.");
                await dlg.ShowAsync();
            }
            finally { ProgressObject.Hide(); }
        }

        /// <summary>
        /// Deletes radio from internet radios dictionary.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public async Task<SocketResponse> DeleteRadioAsync(Radio r)
        {
            object[] parameters = { r.Id };
            return await _socketClient.SendCommand(new SocketCommand(SocketCommand.CommandType.DELETE_RADIO, parameters));
        }

        #endregion

        #endregion
    }
}