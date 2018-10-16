using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace InternetRadioShared.Models
{
    [DataContract]
    [KnownType(typeof(Radio))]
    public class Radio
    {
        public Radio(string title, string url)
        {
            Id = Guid.NewGuid().ToString();
            Title = title;
            Uri = new Uri(url, UriKind.Absolute);
        }

        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public Uri Uri { get; set; }
    }
}
