using System;
using System.Linq;
using System.Security.Claims;
using MyAuth.Authentication;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public partial class MyAuthClaimsBehavior
    {
        private readonly ITestOutputHelper _output;

        /// <summary>
        /// Initializes a new instance of <see cref="MyAuthClaimsBehavior"/>
        /// </summary>
        public MyAuthClaimsBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

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
        public void ShouldSerializeValueWithWarningChars()
        {
            //Arrange
            var claims = new MyAuthClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user-1"),
                new Claim("foo", "val_ \\\" .,'nj")
            });

            //Act
            var str = claims.Serialize();

            //Assert
            Assert.Equal("sub=\"user-1\",foo=\"val_ \\\" .,'nj\"", str);
        }

        [Theory]
        [InlineData("aaa:aaa")]
        [InlineData("aaa\\aaa")]
        [InlineData("aaa/aaa")]
        public void ShouldFailSerializationWhenClaimKeyHasWrongFormat(string key)
        {
            //Arrange
            var claims = new MyAuthClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user-1"),
                new Claim(key, "bbb")
            });

            //Act & Assert
            var e = Assert.Throws<FormatException>(() => claims.Serialize());
            _output.WriteLine(e.ToString());
        }
    }
}
