using System.Collections.Generic;

namespace Usps.Api.Client.Models
{
    public class RateResponse
    {
        public RateResponse() => Postage = new List<Postage>();

        public string OriginZip { get; set; } = "";

        public string DestZip { get; set; } = "";

        public int Pounds { get; set; } = 0;

        public int Ounces { get; set; } = 0;

        public string Container { get; set; } = "";

        public string Size { get; set; } = "";

        public string Zone { get; set; } = "";

        public List<Postage> Postage { get; set; }

        public static RateResponse FromXml(string xml)
        {
            var r = new RateResponse();
            var idx1 = 0;
            var idx2 = 0;

            if (xml.Contains("<ZipOrigination>"))
            {
                idx1 = xml.IndexOf("<ZipOrigination>") + 17;
                idx2 = xml.IndexOf("</ZipOrigination>");
                r.OriginZip = xml.Substring(idx1, idx2 - idx1);
            }

            if (xml.Contains("<ZipDestination>"))
            {
                idx1 = xml.IndexOf("<ZipDestination>") + 16;
                idx2 = xml.IndexOf("</ZipDestination>");
                r.OriginZip = xml.Substring(idx1, idx2 - idx1);
            }

            if (xml.Contains("<Pounds>"))
            {
                idx1 = xml.IndexOf("<Pounds>") + 8;
                idx2 = xml.IndexOf("</Pounds>");
                r.OriginZip = xml.Substring(idx1, idx2 - idx1);
            }

            if (xml.Contains("<Ounces>"))
            {
                idx1 = xml.IndexOf("<Ounces>") + 8;
                idx2 = xml.IndexOf("</Ounces>");
                r.OriginZip = xml.Substring(idx1, idx2 - idx1);
            }

            if (xml.Contains("<Container>"))
            {
                idx1 = xml.IndexOf("<Container>") + 11;
                idx2 = xml.IndexOf("</Container>");
                r.OriginZip = xml.Substring(idx1, idx2 - idx1);
            }

            if (xml.Contains("<Size>"))
            {
                idx1 = xml.IndexOf("<Size>") + 6;
                idx2 = xml.IndexOf("</Size>");
                r.OriginZip = xml.Substring(idx1, idx2 - idx1);
            }

            if (xml.Contains("<Zone>"))
            {
                idx1 = xml.IndexOf("<Zone>") + 6;
                idx2 = xml.IndexOf("</Zone>");
                r.OriginZip = xml.Substring(idx1, idx2 - idx1);
            }

            var lastidx = 0;
            while (xml.IndexOf("<MailService>", lastidx) > -1)
            {
                var p = new Postage();
                idx1 = xml.IndexOf("<MailService>", lastidx) + 13;
                idx2 = xml.IndexOf("</MailService>", lastidx + 13);
                p.MailService = xml.Substring(idx1, idx2 - idx1);

                idx1 = xml.IndexOf("<Rate>", lastidx) + 6;
                idx2 = xml.IndexOf("</Rate>", lastidx + 13);
                p.Amount = decimal.Parse(xml.Substring(idx1, idx2 - idx1));
                r.Postage.Add(p);
                lastidx = idx2;
            }

            return r;
        }
    }

    public class Postage
    {
        public string MailService { get; set; }

        public decimal Amount { get; set; }
    }
}