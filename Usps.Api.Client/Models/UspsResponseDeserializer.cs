using Usps.Api.Client.Extensions;
using Usps.Api.Client.Models.Response;

namespace Usps.Api.Client.Models
{
    public class UspsResponseDeserializer
    {
        public TResponse TryGetAs<TResponse>(string uspsResponse)
        {
            var a = uspsResponse.Deserialize<UspsErrorResponse>();
            return default(TResponse);
        }
    }
}