using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace MyAuth.Authentication
{
    interface ISchemeAdder
    {
        void Add(HttpClient httpClient);
    }

    class MyAuth1SchemeAdder : ISchemeAdder
    {
        private readonly IEnumerable<Claim> _claims;

        public MyAuth1SchemeAdder(IEnumerable<Claim> claims)
        {
            _claims = claims;
        }
        public void Add(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                MyAuthAuthenticationDefinitions.SchemeV1,
                new MyAuth1Claims(_claims).Serialize()
            );
        }
    }

    class MyAuth2SchemeAdder : ISchemeAdder
    {
        private readonly IEnumerable<Claim> _claims;

        public MyAuth2SchemeAdder(IEnumerable<Claim> claims)
        {
            _claims = claims;
        }

        public void Add(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                MyAuthAuthenticationDefinitions.SchemeV2);

            foreach (var newHeader in new MyAuth2Claims(_claims).ToHeaders())
            {
                httpClient.DefaultRequestHeaders.Add(newHeader.Key, newHeader.Value);
            }
        }
    }
}