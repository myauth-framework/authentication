using System.Linq;
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

        [Fact]
        public async Task ShouldPassIdentifier()
        {
            //Arrange
            var client = _factory.CreateClient()
                .MyAuth1Authentication("123");

            //Act
            var claims = (await SenRequest(client))
                .ToDictionary(c => c.Type, c => c.Value);

            //Assert
            Assert.Equal("123", claims[ClaimTypes.NameIdentifier]);
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

        [Fact]
        public async Task ShouldPassClaims()
        {
            //Arrange
            var client = _factory.CreateClient()
                .MyAuth1Authentication(
                    "123",
                    new []{ new Claim("foo-claim", "foo-val") });

            //Act
            var claims = (await SenRequest(client))
                .ToDictionary(c => c.Type, c => c.Value);

            //Assert
            Assert.Equal("foo-val", claims["foo-claim"]);
        }

        [Fact]
        public async Task ShouldPassRoles()
        {
            //Arrange
            var client = _factory.CreateClient()
                .MyAuth1Authentication(
                    "123",
                    new[]
                    {
                        new Claim(ClaimTypes.Role, "admin"),
                        new Claim(ClaimTypes.Role, "user")
                    });

            //Act
            var roles = (await SenRequest(client))
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToArray();

            //Assert
            Assert.Contains("admin", roles);
            Assert.Contains("user", roles);
        }



        [Fact]
        public async Task ShouldResolveUrlEncodedHeaders()
        {
            //Arrange
            var client = _factory.CreateClient()
                .MyAuth1Authentication(
                    "123",
                    new[] { new Claim("name", HttpUtility.UrlEncode("Растислав"))});

            //Act
            var claims = (await SenRequest(client))
                .ToDictionary(c => c.Type, c => c.Value);

            //Assert
            Assert.Equal("Растислав", claims[ClaimTypes.Name]);
        }

    }
}
