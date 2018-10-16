using Helpers.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;

namespace InternetRadioShared.Models
{
    [DataContract]
    public class InternetRadioStatus
    {
        [DataMember]
        public MediaPlayerState State { get; set; }
        [DataMember]
        public Radio CurrentRadio { get; set; }
        [DataMember]
        public double Volume { get; set; }
    }
}
