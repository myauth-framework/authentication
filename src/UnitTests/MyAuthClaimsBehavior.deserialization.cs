using System;
using System.Linq;
using System.Security.Claims;
using MyAuth.Authentication;
using Xunit;

namespace UnitTests
{
    public partial class MyAuthV1ClaimsBehavior
    {
        [Theory]
        [InlineData("sub=\"user-1\"", "user-1")]
        [InlineData("sub=\"user-1\", name=\"John\"", "John")]
        public void ShouldDeserializeSub(string str, string nameVal)
        {
            //Arrange
            
            //Act
            var claims = MyAuth1Claims.Deserialize(str);

            var nameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            var idClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            //Assert
            Assert.NotNull(nameClaim);
            Assert.Equal(nameVal, nameClaim.Value);
            Assert.NotNull(idClaim);
            Assert.Equal("user-1", idClaim.Value);
        }

        [Theory]
        [InlineData("sub=\"user-1\", roles=\"admin\"")]
        [InlineData("sub=\"user-1\", role=\"admin\"")]
        public void ShouldDeserializeSingleRole(string str)
        {
            //Arrange


            //Act
            var claims = MyAuth1Claims.Deserialize(str);
            var roleClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

            //Assert
            Assert.NotNull(roleClaim);
            Assert.Equal("admin", roleClaim.Value);
        }

        [Fact]
        public void ShouldDeserializeMultipleRole()
        {
            //Arrange
            var str = "sub=\"user-1\", role=\"admin,user\"";

            //Act
            var claims = MyAuth1Claims.Deserialize(str);
            var roles = claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToArray();

            //Assert
            Assert.Contains(roles, r => r == "admin");
            Assert.Contains(roles, r => r == "user");
        }

        [Fact]
        public void ShouldFailWhenDeserializeAndSubNotFound()
        {
            //Arrange
            var str = "name=\"user-1\"";

            //Act & Assert 
            Assert.Throws<FormatException>(() => MyAuth1Claims.Deserialize(str));
        }

        [Fact]
        public void ShouldDeserializeWithWarningChars()
        {
            //Arrange
            var str = "sub=\"user-1\",key://some_key-123/=\"val_ \\\" .,'nj\"";
            
            //Act
            var claims = MyAuth1Claims.Deserialize(str);
            var warningClaim = claims.FirstOrDefault(c => c.Type == "key://some_key-123/");

            //Assert
            Assert.NotNull(warningClaim);
            Assert.Equal("val_ \\\" .,'nj", warningClaim.Value);
        }
    }
}
