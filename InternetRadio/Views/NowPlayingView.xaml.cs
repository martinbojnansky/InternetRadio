using InternetRadio.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
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
    public sealed partial class NowPlayingView : UserControl
    {
        public RadioViewModel RadioViewModel = RadioViewModel.Instance;
        public static NowPlayingView Current;

        public NowPlayingView()
        {
            this.InitializeComponent();
            Current = this;
           
            // Update button state
            UpdatePlayPauseState();
        }

        /// <summary>
        /// Updates button icon and availability.
        /// </summary>
        public void UpdatePlayPauseState()
        {
            try
            {
                switch (RadioViewModel.InternetRadioStatus.State)
                {
                    case Windows.Media.Playback.MediaPlayerState.Buffering:
                    case Windows.Media.Playback.MediaPlayerState.Playing:
                    case Windows.Media.Playback.MediaPlayerState.Opening:
                        VisualStateManager.GoToState(this, Playing.Name, true);
                        PlayPauseButton.IsEnabled = true;
                        break;
                    case Windows.Media.Playback.MediaPlayerState.Closed:
                        VisualStateManager.GoToState(this, Paused.Name, true);
                        PlayPauseButton.IsEnabled = false;
                        break;
                    case Windows.Media.Playback.MediaPlayerState.Paused:
                    case Windows.Media.Playback.MediaPlayerState.Stopped:
                        VisualStateManager.GoToState(this, Paused.Name, true);
                        PlayPauseButton.IsEnabled = true;
                        break;
                }
            }
            catch { }
        }

        #region Volume slider

        /// <summary>
        /// Indicates whether volume slider is being dragged or not.
        /// </summary>
        private bool _isVolumeSliderDraging = false;

        /// <summary>
        /// Occurs when slider drag is starting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void VolumeSlider_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            _isVolumeSliderDraging = true;
        }

        /// <summary>
        /// Occurs when slider drag completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void VolumeSlider_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            _isVolumeSliderDraging = false;

            Slider slider = sender as Slider;
            await RadioViewModel.SetVolumeAsync(slider.Value / 30);
        }

        /// <summary>
        /// Occurs when value changed and updates volume of radio if mode is another from dragging.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void VolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (_isVolumeSliderDraging)
                return;

            Slider slider = sender as Slider;
            await RadioViewModel.SetVolumeAsync(slider.Value / 30);
        }

        #endregion
    }
}
