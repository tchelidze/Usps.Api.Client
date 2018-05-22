using System;
using Usps.Api.Client.Models.Response;

namespace Usps.Api.Client.Exceptions
{
    public class ErrorUspsApiResponseException : Exception
    {
        public UspsErrorResponse ErrorResponse { get; }

        public ErrorUspsApiResponseException(UspsErrorResponse errorResponse)
        {
            ErrorResponse = errorResponse;
        }
    }
}