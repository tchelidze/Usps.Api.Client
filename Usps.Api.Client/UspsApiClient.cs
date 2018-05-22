using System;
using System.Net;
using System.Text;
using Usps.Api.Client.Exceptions;
using Usps.Api.Client.Extensions;
using Usps.Api.Client.Models;
using Usps.Api.Client.Models.Request;
using Usps.Api.Client.Models.Response;

namespace Usps.Api.Client
{
    public class UspsApiClient
    {
        private const string ProductionUrl = "http://production.shippingapis.com/ShippingAPI.dll";
        private const string TestingUrl = "http://testing.shippingapis.com/ShippingAPITest.dll";
        private readonly string _userid;
        private readonly WebClient _uspsApiClient;

        public UspsApiClient(string uspsWebtoolUserId)
        {
            _uspsApiClient = new WebClient();
            _userid = uspsWebtoolUserId;
            TestMode = false;
        }

        public UspsApiClient(string uspsWebtoolUserId, bool testmode)
        {
            TestMode = testmode;
            _uspsApiClient = new WebClient();
            _userid = uspsWebtoolUserId;
        }

        public bool TestMode { get; set; }

        public TrackingInfo GetTrackingInfo(string trackingNumber)
        {
            var trackRequestXml = new UspsTrackInfoRequest
            {
                TrackId = trackingNumber,
                UserId = _userid
            }.Serialize(true);

            var url =
                new UspsRequestUrlBuilder()
                    .BaseUrl(GetURL())
                    .ToApi("TrackV2")
                    .WithBody(trackRequestXml)
                    .Build();

            var xml = _uspsApiClient.DownloadString(url);
            if (xml.Contains("<Error>"))
            {
                var idx1 = xml.IndexOf("<Description>") + 13;
                var idx2 = xml.IndexOf("</Description>");
                var errDesc = xml.Substring(idx1, idx2 - idx1);
                throw new ErrorUspsApiResponseException(null);
            }

            return TrackingInfo.FromXml(xml);
        }

        public RateResponse GetRate(Package package)
        {
            var url =
                "?API=RateV2&XML=<RateV2Request USERID=\"{0}\"><Package ID=\"0\"><Service>{1}</Service><ZipOrigination>{2}</ZipOrigination><ZipDestination>{3}</ZipDestination><Pounds>{4}</Pounds><Ounces>{5}</Ounces><Container>{6}</Container><Size>{7}</Size></Package></RateV2Request>";

            var lb = package.WeightInOunces / 16;
            var oz = package.WeightInOunces % 16;
            var container = package.PackageType.ToString().Replace("_", " ");
            if (container == "None")
            {
                url = url.Replace("<Container>{6}</Container>", "");
            }

            var fromZip = package.FromAddress.Zip;
            if (!string.IsNullOrEmpty(package.OriginZipcode))
            {
                fromZip = package.OriginZipcode;
            }


            url = GetURL() + url;
            url = string.Format(url, _userid, package.ServiceType.ToString(), fromZip, package.ToAddress.Zip, lb,
                oz, container, package.PackageSize.ToString().Replace("_", " "));
            var xml = _uspsApiClient.DownloadString(url);
            if (xml.Contains("<Error>"))
            {
                var idx1 = xml.IndexOf("<Description>") + 13;
                var idx2 = xml.IndexOf("</Description>");
                var l = xml.Length;
                var errDesc = xml.Substring(idx1, idx2 - idx1);
                throw new ErrorUspsApiResponseException(null);
            }

            return RateResponse.FromXml(xml);
        }

        private string GetURL()
        {
            var url = ProductionUrl;
            if (TestMode)
            {
                url = TestingUrl;
            }

            return url;
        }

        public Address ValidateAddress(Address address)
        {
            const string validateUrl =
                "?API=Verify&XML=<AddressValidateRequest USERID=\"{0}\"><Address ID=\"{1}\"><Address1>{2}</Address1><Address2>{3}</Address2><City>{4}</City><State>{5}</State><Zip5>{6}</Zip5><Zip4>{7}</Zip4></Address></AddressValidateRequest>";
            var url = GetURL() + validateUrl;
            url = string.Format(url, _userid, address.Id, address.Address1, address.Address2, address.City,
                address.State, address.Zip, address.ZipPlus4);
            var addressxml = _uspsApiClient.DownloadString(url);
            if (addressxml.Contains("<Error>"))
            {
                var idx1 = addressxml.IndexOf("<Description>") + 13;
                var idx2 = addressxml.IndexOf("</Description>");
                var l = addressxml.Length;
                var errDesc = addressxml.Substring(idx1, idx2 - idx1);
                throw new ErrorUspsApiResponseException(null);
            }

            return Address.FromXml(addressxml);
        }

        public Address GetZipcode(string city, string state)
        {
            var address = new Address
            {
                City = city,
                State = state
            };
            return GetZipcode(address);
        }

        public Address GetZipcode(Address address)
        {
            //The address must contain a city and state
            if (string.IsNullOrEmpty(address.City) || address.State == null ||
                address.State.Length < 1)
            {
                throw new ArgumentException("You must supply a city and state for a zipcode lookup request.");
            }

            var zipcodeurl =
                "?API=ZipCodeLookup&XML=<ZipCodeLookupRequest USERID=\"{0}\"><Address ID=\"{1}\"><Address1>{2}</Address1><Address2>{3}</Address2><City>{4}</City><State>{5}</State></Address></ZipCodeLookupRequest>";
            var url = GetURL() + zipcodeurl;
            url = string.Format(url, _userid, address.Id, address.Address1, address.Address2, address.City,
                address.State, address.Zip, address.ZipPlus4);
            var addressxml = _uspsApiClient.DownloadString(url);
            if (addressxml.Contains("<Error>"))
            {
                var idx1 = addressxml.IndexOf("<Description>") + 13;
                var idx2 = addressxml.IndexOf("</Description>");
                var l = addressxml.Length;
                var errDesc = addressxml.Substring(idx1, idx2 - idx1);
                throw new ErrorUspsApiResponseException(null);
            }

            return Address.FromXml(addressxml);
        }

        public Address GetCityState(string zipcode)
        {
            var address = new Address { Zip = zipcode };
            return GetCityState(address);
        }

        public Address GetCityState(Address address)
        {
            //The address must contain a city and state
            if (string.IsNullOrEmpty(address.Zip))
            {
                throw new ArgumentException("You must supply a zipcode for a city/state lookup request.");
            }

            var citystateurl =
                "?API=CityStateLookup&XML=<CityStateLookupRequest USERID=\"{0}\"><ZipCode ID= \"{1}\"><Zip5>{2}</Zip5></ZipCode></CityStateLookupRequest>";
            var url = GetURL() + citystateurl;
            url = string.Format(url, _userid, address.Id, address.Zip);
            var addressxml = _uspsApiClient.DownloadString(url);
            if (addressxml.Contains("<Error>"))
            {
                var idx1 = addressxml.IndexOf("<Description>") + 13;
                var idx2 = addressxml.IndexOf("</Description>");
                var l = addressxml.Length;
                var errDesc = addressxml.Substring(idx1, idx2 - idx1);
                throw new ErrorUspsApiResponseException(null);
            }

            return Address.FromXml(addressxml);
        }


        public Package GetDeliveryConfirmationLabel(Package package)
        {
            var labeldate = package.ShipDate.ToShortDateString();
            if (package.ShipDate.ToShortDateString() == DateTime.Now.ToShortDateString())
            {
                labeldate = "";
            }

            var url =
                "?API=DeliveryConfirmationV3&XML=<DeliveryConfirmationV3.0Request USERID=\"{0}\"><Option>{1}</Option><ImageParameters></ImageParameters><FromName>{2}</FromName><FromFirm>{3}</FromFirm><FromAddress1>{4}</FromAddress1><FromAddress2>{5}</FromAddress2><FromCity>{6}</FromCity><FromState>{7}</FromState><FromZip5>{8}</FromZip5><FromZip4>{9}</FromZip4><ToName>{10}</ToName><ToFirm>{11}</ToFirm><ToAddress1>{12}</ToAddress1><ToAddress2>{13}</ToAddress2><ToCity>{14}</ToCity><ToState>{15}</ToState><ToZip5>{16}</ToZip5><ToZip4>{17}</ToZip4><WeightInOunces>{18}</WeightInOunces><ServiceType>{19}</ServiceType><POZipCode>{20}</POZipCode><ImageType>{21}</ImageType><LabelDate>{22}</LabelDate><CustomerRefNo>{23}</CustomerRefNo><AddressServiceRequested>{24}</AddressServiceRequested><SenderName>{25}</SenderName><SenderEMail>{26}</SenderEMail><RecipientName>{27}</RecipientName><RecipientEMail>{28}</RecipientEMail></DeliveryConfirmationV3.0Request>";
            url = GetURL() + url;
            //url = String.Format(url,this._userid, (int)package.LabelType, package.FromAddress.Contact, package.FromAddress.FirmName, package.FromAddress.Address1, package.FromAddress.Address2, package.FromAddress.City, package.FromAddress.State, package.FromAddress.Zip, package.FromAddress.ZipPlus4, package.ToAddress.Contact, package.ToAddress.FirmName, package.ToAddress.Address1, package.ToAddress.Address2, package.ToAddress.City, package.ToAddress.State, package.ToAddress.Zip, package.ToAddress.ZipPlus4, package.WeightInOunces.ToString(), package.ServiceType.ToString().Replace("_", " "), package.OriginZipcode, package.LabelImageType.ToString(), labeldate, package.ReferenceNumber, package.AddressServiceRequested.ToString(),  package.FromAddress.Contact, package.FromAddress.ContactEmail, package.ToAddress.Contact, package.ToAddress.ContactEmail);
            url = string.Format(url, _userid, (int)package.LabelType, package.FromAddress.Contact,
                package.FromAddress.FirmName, package.FromAddress.Address1, package.FromAddress.Address2,
                package.FromAddress.City, package.FromAddress.State, package.FromAddress.Zip,
                package.FromAddress.ZipPlus4, package.ToAddress.Contact, package.ToAddress.FirmName,
                package.ToAddress.Address1, package.ToAddress.Address2, package.ToAddress.City, package.ToAddress.State,
                package.ToAddress.Zip, package.ToAddress.ZipPlus4, package.WeightInOunces,
                package.ServiceType.ToString().Replace("_", " "), package.OriginZipcode,
                package.LabelImageType.ToString(), labeldate, package.ReferenceNumber, package.AddressServiceRequested,
                "", "", "", "");
            var xml = _uspsApiClient.DownloadString(url);
            if (xml.Contains("<Error>"))
            {
                var idx1 = xml.IndexOf("<Description>") + 13;
                var idx2 = xml.IndexOf("</Description>");
                var l = xml.Length;
                var errDesc = xml.Substring(idx1, idx2 - idx1);
                throw new ErrorUspsApiResponseException(null);
            }

            var i1 = xml.IndexOf("<DeliveryConfirmationLabel>") + 27;
            var i2 = xml.IndexOf("</DeliveryConfirmationLabel>");
            package.ShippingLabel = StringToUtf8ByteArray(xml.Substring(i1, i2 - i1));
            return package;
        }

        public Package GetSignatureConfirmationLabel(Package package)
        {
            var url =
                "?API=SignatureConfirmationV3&XML=<SignatureConfirmationV3.0Request USERID=\"{0}\"><Option>{1}</Option><ImageParameters></ImageParameters><FromName>{2}</FromName><FromFirm>{3}</FromFirm><FromAddress1>{4}</FromAddress1><FromAddress2>{5}</FromAddress2><FromCity>{6}</FromCity><FromState>{7}</FromState><FromZip5>{8}</FromZip5><FromZip4>{9}</FromZip4><ToName>{10}</ToName><ToFirm>{11}</ToFirm><ToAddress1>{12}</ToAddress1><ToAddress2>{13}</ToAddress2><ToCity>{14}</ToCity><ToState>{15}</ToState><ToZip5>{16}</ToZip5><ToZip4>{17}</ToZip4><WeightInOunces>{18}</WeightInOunces><ServiceType>{19}</ServiceType><POZipCode>{20}</POZipCode><ImageType>{21}</ImageType><LabelDate>{22}</LabelDate><CustomerRefNo>{23}</CustomerRefNo><AddressServiceRequested>{24}</AddressServiceRequested></SignatureConfirmationV3.0Request>";
            url = GetURL() + url;
            url = string.Format(url, _userid, (int)package.LabelType, package.FromAddress.Contact,
                package.FromAddress.FirmName, package.FromAddress.Address1, package.FromAddress.Address2,
                package.FromAddress.City, package.FromAddress.State, package.FromAddress.Zip,
                package.FromAddress.ZipPlus4, package.ToAddress.Contact, package.ToAddress.FirmName,
                package.ToAddress.Address1, package.ToAddress.Address2, package.ToAddress.City, package.ToAddress.State,
                package.ToAddress.Zip, package.ToAddress.ZipPlus4, package.WeightInOunces,
                package.ServiceType.ToString().Replace("_", " "), package.OriginZipcode,
                package.LabelImageType.ToString(), package.ShipDate.ToShortDateString(), package.ReferenceNumber,
                package.AddressServiceRequested, package.FromAddress.Contact, package.FromAddress.ContactEmail,
                package.ToAddress.Contact, package.ToAddress.ContactEmail);
            var xml = _uspsApiClient.DownloadString(url);
            if (xml.Contains("<Error>"))
            {
                var idx1 = xml.IndexOf("<Description>") + 13;
                var idx2 = xml.IndexOf("</Description>");
                var l = xml.Length;
                var errDesc = xml.Substring(idx1, idx2 - idx1);
                throw new ErrorUspsApiResponseException(null);
            }

            var i1 = xml.IndexOf("<SignatureConfirmationLabel>") + 28;
            var i2 = xml.IndexOf("</DeliveryConfirmationLabel>");
            package.ShippingLabel = StringToUtf8ByteArray(xml.Substring(i1, i2 - i1));
            return package;
        }

        private static byte[] StringToUtf8ByteArray(string pXmlString)
        {
            var encoding = new UTF8Encoding();
            var byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }
    }
}