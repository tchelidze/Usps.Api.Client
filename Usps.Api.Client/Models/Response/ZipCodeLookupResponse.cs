using System.Collections.Generic;

namespace Usps.Api.Client.Models.Response
{
    public class AddressValidateResponse
    {
        private AddressValidateResponse()
        {
        }

        public List<Address> Addresses { get; set; }
    }
}