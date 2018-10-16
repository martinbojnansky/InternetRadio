using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InternetRadioShared.Models
{
    [DataContract]
    public class SocketCommand
    {
        public enum CommandType
        {
            PLAY, STOP, GET_STATUS,
            SET_VOLUME, GET_VOLUME,
            GET_RADIOS, ADD_RADIO, DELETE_RADIO
        }

        public SocketCommand(CommandType type, object[] parameters = null)
        {
            Type = type;
            Parameters = parameters;
        }

        [DataMember]
        public CommandType Type { get; set; }
        [DataMember]
        public object[] Parameters { get; set; }
    }
}
