using System.Net;
using System.Threading.Tasks;
using MyLab.ApiClient.Test;
using TestServer;
using Xunit;
using Xunit.Abstractions;

namespace FuncTests
{
    public class RequiredClaimHeadersControlBehavior : ApiClientTest<Startup, ITestService>
    {
        public RequiredClaimHeadersControlBehavior(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData("foo", "bar", HttpStatusCode.OK)]
        [InlineData(null, "bar", HttpStatusCode.Unauthorized)]
        [InlineData("foo", null, HttpStatusCode.Unauthorized)]
        [InlineData(null, null, HttpStatusCode.Unauthorized)]
        public async Task ShouldControlRequiredHeaders(string userId, string accountId, HttpStatusCode expectedStatus)
        {
            //Act
            var res = await TestCall(s => s.GetWithRequiredHeaders(userId, accountId));

            //Assert
            Assert.Equal(expectedStatus, res.StatusCode);
        }
    }
}
