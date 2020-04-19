using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace UnitTests
{
    public class AuthenticationHeaderValueBehavior
    {
        [Theory]
        [InlineData("sub=\"user-1\", role=\"admin,user\"", true)]
        [InlineData("sub=\"foo\",i:acc-id=\"foo\"", false)]
        [InlineData("sub=\"foo\",i_acc-id=\"foo\"", true)]
        [InlineData("sub=\"foo\",i-acc-id=\"foo\"", true)]
        [InlineData("sub=\"foo\",i/acc/id=\"foo\"", false)]
        [InlineData("sub=\"foo\",i\\acc\\id=\"foo\"", false)]
        public void ShouldParse(string claims, bool expected)
        {
            //Arrange
            var header = $"MyAuth1 {claims}";

            //Act
            var succ = AuthenticationHeaderValue.TryParse(header, out var authVal);

            //Assert

            if (expected)
            {
                Assert.True(succ);
                Assert.Equal("MyAuth1", authVal.Scheme);
                Assert.Equal(claims, authVal.Parameter);
            }
            else
            {
                Assert.False(succ);
            }
        }
    }
}
