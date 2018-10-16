using Helpers.Storage;
using InternetRadioShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetRadioShared.Helpers
{
    public static class InternetRadioStatusHelper
    {
        public static SocketResponse GetStatus(Radio currentRadio)
        {
            try
            {
                InternetRadioStatus status = new InternetRadioStatus();
                status.CurrentRadio = currentRadio;
                status.State = BackgroundAudioHelper.GetState();
                status.Volume = BackgroundAudioHelper.GetVolumeValue();

                return new SocketResponse(SocketResponse.StatusCode.OK, JsonHelper.ToJson(status));
            }
            catch (Exception ex)
            {
                return new SocketResponse(SocketResponse.StatusCode.EXCEPTION, ex.ToString());
            }
        }
    }
}
