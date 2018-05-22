using System;
using System.Xml.Serialization;

namespace Usps.Api.Client.Models.Request
{
    [Serializable]
    public class UspsTrackInfoRequest
    {
        [XmlElement(ElementName = "USERID")]
        public string UserId { get; set; }

        [XmlElement(ElementName = "TrackID")]
        public string TrackId { get; set; }
    }
}