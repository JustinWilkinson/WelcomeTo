using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using WelcomeTo.Shared.Extensions;

namespace WelcomeTo.Test.Shared.Extensions
{
    [TestFixture]
    public class LinqExtensionsTest
    {
        [Test]
        public void Without_SourceEnumerableWithDuplicatedElement_ExcludesOnlyOne()
        {
            // Arrange
            var source = new List<int> { 4, 5, 5 };
            var exclude = new List<int> { 4, 5 };

            // Act
            var result = source.Without(exclude).ToArray();

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(5, result[0]);
        }
    }
}