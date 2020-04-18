using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Xunit;

namespace UnitTests
{
    public class JwtPayloadBehavior
    {
        [Theory]
        [InlineData("{\"roles\":[\"Admin\",\"SimpleUser\"]}", false)]
        [InlineData("{\"role\":[\"Admin\",\"SimpleUser\"]}", false)]
        [InlineData("{\"http://schemas.microsoft.com/ws/2008/06/identity/claims/role\":[\"Admin\",\"SimpleUser\"]}", true)]
        public void ShouldNormalizeRoleClaims(string str, bool support)
        {
            //Arrange

            //Act
            var claims = JwtPayload.Deserialize(str);
            var roles = claims.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToArray();

            //Assert
            if (support)
                Assert.Contains(roles, r => r == "Admin");
            else
                Assert.DoesNotContain(roles, r => r == "Admin");
        }
    }
}
