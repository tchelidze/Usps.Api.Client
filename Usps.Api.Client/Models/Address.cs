using System;
using System.Text;
using System.Xml;

namespace Usps.Api.Client.Models
{
    [Serializable]
    public class Address
    {
        private string _address1 = "";

        private string _address2 = "";

        private string _city = "";

        private string _contact = "";

        private string _contactEmail = "";

        private string _firmName = "";

        private int _id;

        private string _state = "";

        private string _zip = "";

        private string _zipPlus4 = "";

        public Address() => _id = 1;

        /// <summary>
        ///     ID of this address
        /// </summary>
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        /// <summary>
        ///     Name of the Firm or Company
        /// </summary>
        public string FirmName
        {
            get => _firmName;
            set => _firmName = value;
        }

        /// <summary>
        ///     The contact is used to send confirmation when a package is shipped
        /// </summary>
        public string Contact
        {
            get => _contact;
            set => _contact = value;
        }

        /// <summary>
        ///     The contacts email address is used to send delivery confirmation
        /// </summary>
        public string ContactEmail
        {
            get => _contactEmail;
            set => _contactEmail = value;
        }

        /// <summary>
        ///     Address Line 1 is used to provide an apartment or suite
        ///     number, if applicable. Maximum characters allowed: 38
        /// </summary>
        public string Address1
        {
            get => _address1;
            set
            {
                if (value.Length > 38)
                {
                    throw new ArgumentException("Address1 is is limited to a maximum of 38 characters.");
                }

                _address1 = value;
            }
        }

        /// <summary>
        ///     Street address
        ///     Maximum characters allowed: 38
        /// </summary>
        public string Address2
        {
            get => _address2;
            set
            {
                if (value.Length > 38)
                {
                    throw new ArgumentException("Address2 is is limited to a maximum of 38 characters.");
                }

                _address2 = value;
            }
        }

        /// <summary>
        ///     City
        ///     Either the City and State or Zip are required.
        ///     Maximum characters allowed: 15
        /// </summary>
        public string City
        {
            get => _city;
            set
            {
                if (value.Length > 15)
                {
                    throw new ArgumentException("City is is limited to a maximum of 15 characters.");
                }

                _city = value;
            }
        }

        /// <summary>
        ///     State
        ///     Either the City and State or Zip are required.
        ///     Maximum characters allowed = 2
        /// </summary>
        public string State
        {
            get => _state;
            set
            {
                if (value.Length > 2)
                {
                    throw new ArgumentException("State is is limited to a maximum of 2 characters.");
                }

                _state = value;
            }
        }

        /// <summary>
        ///     Zipcode
        ///     Maximum characters allowed = 5
        /// </summary>
        public string Zip
        {
            get => _zip;
            set
            {
                if (value.Length > 5)
                {
                    throw new ArgumentException("Zip is is limited to a maximum of 5 characters.");
                }

                _zip = value;
            }
        }

        /// <summary>
        ///     Zip code extension
        ///     Maximum characters allowed = 4
        /// </summary>
        public string ZipPlus4
        {
            get => _zipPlus4;
            set
            {
                if (value.Length > 5)
                {
                    throw new ArgumentException("Zip is is limited to a maximum of 5 characters.");
                }

                _zipPlus4 = value;
            }
        }

        //////////////////////////////////////////////////////////////////////////
        // FromXML medthod provided by viperguynaz via codeproject
        //////////////////////////////////////////////////////////////////////////


        /// <summary>
        ///     Get an Address object from an xml string.
        /// </summary>
        /// <param name="xml">XML representation of an Address Object</param>
        /// <returns>Address object</returns>
        public static Address FromXml(string xml)
        {
            var address = new Address();

            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var element = doc.SelectSingleNode("/AddressValidateResponse/Address/FirmName");
            if (element != null)
            {
                address._firmName = element.InnerText;
            }

            element = doc.SelectSingleNode("/AddressValidateResponse/Address/Address1");
            if (element != null)
            {
                address._address1 = element.InnerText;
            }

            element = doc.SelectSingleNode("/AddressValidateResponse/Address/Address2");
            if (element != null)
            {
                address._address2 = element.InnerText;
            }

            element = doc.SelectSingleNode("/AddressValidateResponse/Address/City");
            if (element != null)
            {
                address._city = element.InnerText;
            }

            element = doc.SelectSingleNode("/AddressValidateResponse/Address/State");
            if (element != null)
            {
                address._state = element.InnerText;
            }

            element = doc.SelectSingleNode("/AddressValidateResponse/Address/Zip5");
            if (element != null)
            {
                address._zip = element.InnerText;
            }

            element = doc.SelectSingleNode("/AddressValidateResponse/Address/Zip4");
            if (element != null)
            {
                address._zipPlus4 = element.InnerText;
            }

            return address;
        }

        /// <summary>
        ///     Get the Xml representation of this address object
        /// </summary>
        /// <returns>String</returns>
        public string ToXml()
        {
            var sb = new StringBuilder();
            sb.Append("<Address ID=\"" + Id + "\">");
            sb.Append("<Address1>" + _address1 + "</Address1>");
            sb.Append("<Address2>" + _address2 + "</Address2>");
            sb.Append("<City>" + City + "</City>");
            sb.Append("<State>" + State + "</State>");
            sb.Append("<Zip5>" + Zip + "</Zip5>");
            sb.Append("<Zip4>" + ZipPlus4 + "</Zip4>");
            sb.Append("</Address>");
            return sb.ToString();
        }
    }
}