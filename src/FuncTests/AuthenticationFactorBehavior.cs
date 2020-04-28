using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using MyAuth.Authentication;
using MyLab.ApiClient.Test;
using Newtonsoft.Json;
using TestServer;
using TestServer.Models;
using Xunit;
using Xunit.Abstractions;

namespace FuncTests
{
    public class AuthenticationFactorBehavior : ApiClientTest<Startup, ITestService>
    {
        public AuthenticationFactorBehavior(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldNotAuthorizeWithoutAuthCredentials()
        {
            //Arrange
            
            //Act
            var resp = await TestCall(s => s.GetAuthorized());
            
            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Theory]
        [InlineData(MyAuthAuthenticationDefinitions.SchemeV1)]
        [InlineData(MyAuthAuthenticationDefinitions.SchemeV2)]
        public async Task ShouldAuthenticate(string schemeVersion)
        {
            //Act
            var resp = await TestCall(
                s => s.GetAuthorized(),
                httpClientPostInit: c =>
                {
                    c.AddMyAuthAuthentication(
                        "123",
                        null,
                        schemeVersion);
                });

            //Assert
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }

        [Fact]
        public async Task ShouldAuthenticateIfV2AuthorizationHeaderNotSpecified()
        {
            //Act
            var resp = await TestCall(
                s => s.GetAuthorized(),
                httpClientPostInit: c =>
                {
                    c.DefaultRequestHeaders.Add(MyAuth2HeaderNames.UserId, "123");
                });

            //Assert
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }
    }
}
