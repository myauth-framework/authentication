using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using MyAuth.Authentication;
using Xunit;

namespace UnitTests
{
    public class V1ClaimsReaderBehavior
    {
        private const string TestStr = "sub=\"user-1\",roles=\"admin,user\"";

        [Fact]
        public void ShouldReadFirstKey()
        {
            //Arrange
            var reader = new V1ClaimsReader(TestStr);

            //Act
            var key1 = reader.ReadKey();

            //Assert
            Assert.Equal("sub", key1);
            Assert.Equal(5, reader.CurrentPosition);
        }

        [Fact]
        public void ShouldReadFirstValue()
        {
            //Arrange
            var reader = new V1ClaimsReader(TestStr)
            {
                CurrentPosition = 5
            };

            //Act
            var val1 = reader.ReadValue();

            //Assert
            Assert.Equal("user-1", val1);
            Assert.Equal(13, reader.CurrentPosition);
        }

        [Fact]
        public void ShouldReadNextKey()
        {
            //Arrange
            var reader = new V1ClaimsReader(TestStr)
            {
                CurrentPosition = 13
            };

            //Act
            var nextKey = reader.ReadKey();

            //Assert
            Assert.Equal("roles", nextKey);
            Assert.Equal(20, reader.CurrentPosition);
        }

        [Fact]
        public void ShouldReadLastValue()
        {
            //Arrange
            var reader = new V1ClaimsReader(TestStr)
            {
                CurrentPosition = 20
            };

            //Act
            var lastVal = reader.ReadValue();

            //Assert
            Assert.Equal("admin,user", lastVal);
            Assert.Equal(TestStr.Length, reader.CurrentPosition);
            Assert.True(reader.Eof());
        }
    }
}
