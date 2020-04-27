using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using MyAuth.Authentication;
using Newtonsoft.Json;
using TestServer;
using TestServer.Models;
using Xunit;
using Xunit.Abstractions;

namespace FuncTests
{
    public class MyAuthAuthenticationBehavior : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly ITestOutputHelper _output;

        public MyAuthAuthenticationBehavior(WebApplicationFactory<Startup> factory,
            ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
        }

        [Theory]
        [InlineData(MyAuthAuthenticationDefinitions.SchemeV1)]
        [InlineData(MyAuthAuthenticationDefinitions.SchemeV2)]
        public async Task ShouldPassIdentifier(string schemeVersion)
        {
            //Arrange
            var client = _factory.CreateClient()
                .AddMyAuthAuthentication("123",
                    scheme: schemeVersion);

            //Act
            var claims = (await SenRequest(client))
                .ToDictionary(c => c.Type, c => c.Value);

            //Assert
            Assert.Equal("123", claims[ClaimTypes.NameIdentifier]);
        }

        [Theory]
        [InlineData(MyAuthAuthenticationDefinitions.SchemeV1)]
        [InlineData(MyAuthAuthenticationDefinitions.SchemeV2)]
        public async Task ShouldPassClaims(string schemeVersion)
        {
            //Arrange
            var client = _factory.CreateClient()
                .AddMyAuthAuthentication(
                    "123",
                    new []{ new Claim("foo-claim", "foo-val") },
                    schemeVersion);

            //Act
            var claims = (await SenRequest(client))
                .ToDictionary(c => c.Type, c => c.Value);

            //Assert
            Assert.Equal("foo-val", claims["foo-claim"]);
        }

        [Theory]
        [InlineData(MyAuthAuthenticationDefinitions.SchemeV1)]
        [InlineData(MyAuthAuthenticationDefinitions.SchemeV2)]
        public async Task ShouldPassRoles(string schemeVersion)
        {
            //Arrange
            var client = _factory.CreateClient()
                .AddMyAuthAuthentication(
                    "123",
                    new[]
                    {
                        new Claim(ClaimTypes.Role, "admin"),
                        new Claim(ClaimTypes.Role, "user")
                    },
                    schemeVersion);

            //Act
            var roles = (await SenRequest(client))
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToArray();

            //Assert
            Assert.Contains("admin", roles);
            Assert.Contains("user", roles);
        }



        [Theory]
        [InlineData(MyAuthAuthenticationDefinitions.SchemeV1)]
        [InlineData(MyAuthAuthenticationDefinitions.SchemeV2)]
        public async Task ShouldResolveUrlEncodedHeaders(string schemeVersion)
        {
            //Arrange
            var client = _factory.CreateClient()
                .AddMyAuthAuthentication(
                    "123",
                    new[] { new Claim("name", HttpUtility.UrlEncode("Растислав"))},
                    schemeVersion);

            //Act
            var claims = (await SenRequest(client))
                .ToDictionary(c => c.Type, c => c.Value);

            //Assert
            Assert.Equal("Растислав", claims[ClaimTypes.Name]);
        }

        [Fact]
        public async Task ShouldNotAuthorizeWithoutAuthCredentials()
        {
            //Arrange
            var client = _factory.CreateClient();


            //Act
            var resp = await client.GetAsync("test/authorized");
            
            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Theory]
        [InlineData(MyAuthAuthenticationDefinitions.SchemeV1)]
        [InlineData(MyAuthAuthenticationDefinitions.SchemeV2)]
        public async Task ShouldAuthenticate(string schemeVersion)
        {
            //Arrange
            var client = _factory.CreateClient()
                .AddMyAuthAuthentication(
                    "123",
                    null,
                    schemeVersion);

            //Act
            var resp = await client.GetAsync("test/authorized");

            //Assert
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }

        [Fact]
        public async Task ShouldAuthenticateIfV2AuthorizationHeaderNotSpecified()
        {
            //Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-Claim-User-Id", "123");

            //Act
            var resp = await client.GetAsync("test/authorized");

            //Assert
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }

        async Task<ClaimModel[]> SenRequest(HttpClient client)
        {
            var resp = await client.GetAsync("test");
            var respStr = await resp.Content.ReadAsStringAsync();

            _output.WriteLine($"HTTP code: {(int)resp.StatusCode}({resp.StatusCode})");
            _output.WriteLine(respStr);
            resp.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<ClaimModel[]>(respStr);
        }

    }
}
