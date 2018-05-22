using System;

namespace Usps.Api.Client
{
    public class UspsRequestUrlBuilder
    {
        private string _baseUrl;

        public UspsRequestUrlBuilder BaseUrl(string baseUrl)
        {
            _baseUrl = baseUrl;
            return this;
        }

        private string _api;

        public UspsRequestUrlBuilder ToApi(string api)
        {
            _api = api;
            return this;
        }

        private string _xmlBody;

        public UspsRequestUrlBuilder WithBody(string xmlBody)
        {
            _xmlBody = xmlBody;
            return this;
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(_api))
            {
                throw new InvalidOperationException("Target Api is mandatory for building usps request url");
            }

            if (string.IsNullOrEmpty(_xmlBody))
            {
                throw new InvalidOperationException("Request Body is mandatory for building usps request url");
            }

            if (string.IsNullOrEmpty(_baseUrl))
            {
                throw new InvalidOperationException("Base url is mandatory for building usps request url");
            }
        }

        public string Build()
        {
            Validate();
            return $"{_baseUrl}?API={_api}&XML={_xmlBody}";
        }
    }
}