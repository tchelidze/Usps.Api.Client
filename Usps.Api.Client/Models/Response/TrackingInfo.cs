using System.Collections.Generic;

namespace Usps.Api.Client.Models.Response
{
    public class TrackingInfo
    {
        public TrackingInfo() => Details = new List<string>();

        /// <summary>
        ///     The tracking number for the package
        /// </summary>
        public string TrackingNumber { get; set; }

        /// <summary>
        ///     Summary information for the package
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        ///     Tracking Details
        /// </summary>
        public List<string> Details { get; set; }

        public static TrackingInfo FromXml(string xml)
        {
            var idx1 = 0;
            var idx2 = 0;
            var t = new TrackingInfo();
            if (xml.Contains("<TrackSummary>"))
            {
                idx1 = xml.IndexOf("<TrackSummary>") + 14;
                idx2 = xml.IndexOf("</TrackSummary>");
                t.Summary = xml.Substring(idx1, idx2 - idx1);
            }

            var lastidx = 0;
            while (xml.IndexOf("<TrackDetail>", lastidx) > -1)
            {
                idx1 = xml.IndexOf("<TrackDetail>", lastidx) + 13;
                idx2 = xml.IndexOf("</TrackDetail>", lastidx + 13);
                t.Details.Add(xml.Substring(idx1, idx2 - idx1));
                lastidx = idx2;
            }

            return t;
        }
    }
}