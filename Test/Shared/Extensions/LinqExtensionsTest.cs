using System.Collections.Generic;
using System.Linq;
using WelcomeTo.Shared.Extensions;
using Xunit;

namespace WelcomeTo.Test.Shared.Extensions
{
    public class LinqExtensionsTest
    {
        [Fact]
        public void Without_SourceEnumerableWithDuplicatedElement_ExcludesOnlyOne()
        {
            // Arrange
            var source = new List<int> { 4, 5, 5 };
            var exclude = new List<int> { 4, 5 };

            // Act
            var result = source.Without(exclude).ToArray();

            // Assert
            Assert.Equal(5, Assert.Single(result));
        }
    }
}