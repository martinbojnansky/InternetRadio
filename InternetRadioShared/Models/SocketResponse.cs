using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InternetRadioShared.Models
{
    [DataContract]
    public class SocketResponse
    {
        public enum StatusCode { OK, EXCEPTION, BAD_REQUEST }

        public SocketResponse(StatusCode status, string response = "")
        {
            Status = status;
            Response = response;
        }

        [DataMember]
        public StatusCode Status { get; set; }
        [DataMember]
        public string Response { get; set; }
    }
}
