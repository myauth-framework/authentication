using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using Xunit;

namespace UnitTests
{
    public class ClaimsIdentityBehavior
    {
        [Theory]
        [InlineData("{\"roles\":[\"Admin\",\"SimpleUser\"]}", false)]
        [InlineData("{\"role\":[\"Admin\",\"SimpleUser\"]}", false)]
        [InlineData("{\"http://schemas.microsoft.com/ws/2008/06/identity/claims/role\":[\"Admin\",\"SimpleUser\"]}", true)]
        public void ShouldSupportRolesClaims(string strClaims, bool suppor)
        {
            //Arrange
            var jwtPayload = JwtPayload.Deserialize(strClaims);

            //Act
            IIdentity ci = new ClaimsIdentity(jwtPayload.Claims);
            IPrincipal p = new ClaimsPrincipal(ci);

            //Assert
            if(suppor)
                Assert.True(p.IsInRole("Admin"));
            else
                Assert.False(p.IsInRole("Admin"));
        }
    }
}