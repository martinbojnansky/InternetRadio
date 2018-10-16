using InternetRadioShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;

namespace InternetRadioShared.Helpers
{
    public static class BackgroundAudioHelper
    {
        /// <summary>
        /// Gets current state of background media player.
        /// </summary>
        /// <returns></returns>
        public static MediaPlayerState GetState()
        {
            try { return BackgroundMediaPlayer.Current.CurrentState; }
            catch { return MediaPlayerState.Closed; }
        }

        /// <summary>
        /// Plays radio station.
        /// </summary>
        /// <param name="radio"></param>
        /// <returns></returns>
        public static SocketResponse PlayRadio(Radio radio)
        {
            try
            {
                BackgroundMediaPlayer.Current.SetUriSource(radio.Uri);
                return new SocketResponse(SocketResponse.StatusCode.OK);
            }
            catch(Exception ex)
            {
                return new SocketResponse(SocketResponse.StatusCode.EXCEPTION, ex.ToString()); 
            }
        }

        /// <summary>
        /// Stops background media player.
        /// </summary>
        /// <returns></returns>
        public static SocketResponse Stop()
        {
            try
            {
                if (BackgroundMediaPlayer.Current.CanPause)
                {
                    BackgroundMediaPlayer.Current.Pause();
                    return new SocketResponse(SocketResponse.StatusCode.OK);
                }
                else
                {
                    return new SocketResponse(SocketResponse.StatusCode.EXCEPTION, "Pause is not enabled.");
                }
            }
            catch (Exception ex)
            {
                return new SocketResponse(SocketResponse.StatusCode.EXCEPTION, ex.ToString());
            }
        }

        /// <summary>
        /// Sets volume of background media player.
        /// </summary>
        /// <param name="volume">Volume within range 0-1</param>
        /// <returns></returns>
        public static SocketResponse SetVolume(double volume)
        {
            try
            {
                BackgroundMediaPlayer.Current.Volume = volume;
                return new SocketResponse(SocketResponse.StatusCode.OK);
            }
            catch (Exception ex)
            {
                return new SocketResponse(SocketResponse.StatusCode.EXCEPTION, ex.ToString());
            }
        }

        /// <summary>
        /// Gets volume of background media player.
        /// </summary>
        /// <returns></returns>
        public static SocketResponse GetVolume()
        {
            try
            {
                return new SocketResponse(SocketResponse.StatusCode.OK, BackgroundMediaPlayer.Current.Volume.ToString());
            }
            catch (Exception ex)
            {
                return new SocketResponse(SocketResponse.StatusCode.EXCEPTION, ex.ToString());
            }
        }

        /// <summary>
        /// Gets volume as double.
        /// </summary>
        /// <returns></returns>
        public static double GetVolumeValue()
        {
            try { return BackgroundMediaPlayer.Current.Volume; }
            catch { return 0; }
        }
    }
}
