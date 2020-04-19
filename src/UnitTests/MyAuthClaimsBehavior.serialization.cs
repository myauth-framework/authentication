using System;
using System.Linq;
using System.Security.Claims;
using MyAuth.Authentication;
using Xunit;

namespace UnitTests
{
    public partial class MyAuthClaimsBehavior
    {
        [Fact]
        public void ShouldSerializeSub()
        {
            //Arrange
            var claims = new MyAuthClaims(new []
            {
                new Claim(ClaimTypes.NameIdentifier, "user-1"), 
            });

            //Act
            var str = claims.Serialize();

            //Assert
            Assert.Equal("sub=\"user-1\"", str);
        }

        [Fact]
        public void ShouldSerializeSingleRole()
        {
            //Arrange
            var claims = new MyAuthClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user-1"),
                new Claim(ClaimTypes.Role, "admin"),
            });

            //Act
            var str = claims.Serialize();

            //Assert
            Assert.Equal("sub=\"user-1\",roles=\"admin\"", str);
        }

        [Fact]
        public void ShouldSerializeMultipleRole()
        {
            //Arrange
            var claims = new MyAuthClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user-1"),
                new Claim(ClaimTypes.Role, "admin"),
                new Claim(ClaimTypes.Role, "user"),
            });

            //Act
            var str = claims.Serialize();

            //Assert
            Assert.Equal("sub=\"user-1\",roles=\"admin,user\"", str);
        }

        [Fact]
        public void ShouldSerializeWithWarningChars()
        {
            //Arrange
            var claims = new MyAuthClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user-1"),
                new Claim("key://some_key-123/", "val_ \\\" .,'nj")
            });

            //Act
            var str = claims.Serialize();

            //Assert
            Assert.Equal("sub=\"user-1\",key://some_key-123/=\"val_ \\\" .,'nj\"", str);
        }
    }
}
