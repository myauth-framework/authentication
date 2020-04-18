using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using MyAuth.Authentication;
using Newtonsoft.Json;
using TestServer;
using TestServer.Models;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FuncTests
{
    public class HeaderAuthenticationBehavior : IClassFixture<WebApplicationFactory<Startup>>
    {
        private const string UserId = "2bbddfc6a668492ebac555a28e7381e1";
        //private static readonly Claim[] UserClaims = new Claim[]
        //{
        //    new Claim("Claim", "ClaimVal"),
        //    new Claim(ClaimTypes.Role, "Admin"),
        //    new Claim(ClaimTypes.Role, "SimpleUser"),
        //    new Claim("Roles", "Admin2"),
        //    new Claim("Roles", "Admin2"),
        //    new Claim("name", HttpUtility.UrlEncode("Растислав")),
        //};

        private readonly WebApplicationFactory<Startup> _factory;
        private readonly ITestOutputHelper _output;

        public HeaderAuthenticationBehavior(WebApplicationFactory<Startup> factory,
            ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
        }

        [Fact]
        public async Task ShouldPassClaimsFromJwt()
        {
            //Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                HeaderBasedDefinitions.AuthenticationSchemeV1, UserId);

            Claim[] userClaims = 
            {
                new Claim("Claim", "ClaimVal")
            };

        client.DefaultRequestHeaders.Add(
            HeaderBasedDefinitions.UserClaimsHeaderName, 
            new JwtPayload(userClaims).SerializeToJson());

            //Act
            var resp = await client.GetAsync("test");

            if (!resp.IsSuccessStatusCode)
            {
                _output.WriteLine(await resp.Content.ReadAsStringAsync());
                throw new Exception();
            }

            var respStr = await resp.Content.ReadAsStringAsync();

            _output.WriteLine($"HTTP code: {(int)resp.StatusCode}({resp.StatusCode})");
            _output.WriteLine(respStr);

            var claims = JsonConvert.DeserializeObject<ClaimModel[]>(respStr).ToDictionary(c => c.Type, c => c.Value);

            //Assert
            Assert.True(resp.IsSuccessStatusCode);
            Assert.Equal("ClaimVal", claims["Claim"]);
            Assert.Equal("2bbddfc6a668492ebac555a28e7381e1", claims[ClaimTypes.NameIdentifier]);
            Assert.DoesNotContain("nbf", claims.Keys);
            Assert.DoesNotContain("exp", claims.Keys);
            Assert.DoesNotContain("iss", claims.Keys);
            Assert.DoesNotContain("aud", claims.Keys);
        }

        [Theory]
        [InlineData("Roles-user", true)]
        [InlineData("Role-user", true)]
        [InlineData("ClaimTypes-user", true)]
        [InlineData("Wrong-user", false)]
        public async Task ShouldPassRoles(string role, bool isInRoleExpected)
        {
            //Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                HeaderBasedDefinitions.AuthenticationSchemeV1, UserId);

            Claim[] userClaims =
            {
                new Claim("Roles", "Roles-user"),
                new Claim("Role", "Role-user"),
                new Claim(ClaimTypes.Role, "ClaimTypes-user")
            };

            client.DefaultRequestHeaders.Add(
                HeaderBasedDefinitions.UserClaimsHeaderName, 
                new JwtPayload(userClaims).SerializeToJson());

            //Act
            var resp = await client.PostAsync("test/is-in-role", new StringContent("\"" + role + "\"", Encoding.UTF8, "application/json"));
            var respStr = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                _output.WriteLine(respStr);
                throw new Exception();
            }
            
            _output.WriteLine($"HTTP code: {(int)resp.StatusCode}({resp.StatusCode})");
            _output.WriteLine(respStr);

            bool isInRole = bool.Parse(respStr);

            //Assert
            Assert.True(resp.IsSuccessStatusCode);
            Assert.Equal(isInRoleExpected, isInRole);
        }

        [Fact]
        public async Task ShouldResolveUrlEncodedHeaders()
        {
            //Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                HeaderBasedDefinitions.AuthenticationSchemeV1, UserId);

            Claim[] userClaims =
            {
                new Claim("name", HttpUtility.UrlEncode("Растислав"))
            };

            client.DefaultRequestHeaders.Add(
                HeaderBasedDefinitions.UserClaimsHeaderName, 
                new JwtPayload(userClaims).SerializeToJson());

            //Act
            var resp = await client.GetAsync("test");
            var respStr = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                _output.WriteLine(respStr);
                throw new Exception();
            }

            _output.WriteLine($"HTTP code: {(int)resp.StatusCode}({resp.StatusCode})");
            _output.WriteLine(respStr);

            var claims = JsonConvert.DeserializeObject<ClaimModel[]>(respStr).ToDictionary(c => c.Type, c => c.Value);

            //Assert
            Assert.True(resp.IsSuccessStatusCode);
            Assert.Equal("Растислав", claims["name"]);
        }

        [Fact]
        public async Task ShouldReturn401AndDescWhenClaimsHasWrongFormatAndRequired()
        {
            //Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                HeaderBasedDefinitions.AuthenticationSchemeV1, UserId);
            client.DefaultRequestHeaders.Add(HeaderBasedDefinitions.UserClaimsHeaderName, "Wrong claims");

            //Act
            var resp = await client.GetAsync("test/authorized");
            var respStr = await resp.Content.ReadAsStringAsync();
            
            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
            Assert.Equal("", respStr);
        }
    }
}
