using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace MyAuth.Authentication
{
    static class SchemeDetector
    {
        public static bool IsSchema2(IHeaderDictionary headerDictionary)
        {
            var authHeader = headerDictionary["Authorization"];

            if (!string.IsNullOrEmpty(authHeader))
            {
                if (!AuthenticationHeaderValue.TryParse(authHeader, out var authVal))
                {
                    return false;
                }

                if (authVal.Scheme != MyAuthAuthenticationDefinitions.SchemeV2)
                {
                    return false;
                }
            }
            else
            {
                var userIdHeader = headerDictionary["X-Claim-User-Id"];

                if (string.IsNullOrEmpty(userIdHeader))
                    return false;
            }

            return true;
        }

        public static bool IsSchema1(IHeaderDictionary headerDictionary, out AuthenticationHeaderValue authenticationHeader)
        {
            var authHeader = headerDictionary["Authorization"];

            if (!AuthenticationHeaderValue.TryParse(authHeader, out authenticationHeader))
            {
                return false;
            }

            if (authenticationHeader.Scheme != MyAuthAuthenticationDefinitions.SchemeV1)
            {
                return false;
            }

            return true;
        }
    }
}