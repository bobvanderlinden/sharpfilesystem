using System;
using SharpFileSystem.Collections;
using Xunit;

namespace SharpFileSystem.Tests.Collections
{
    public class EnumerableCollectionTests
    {
        [Fact]
        public void When_CopyToArray_Expect_OutputEqualToInput()
        {
            var input = new[] {"a", "b", "c"};
            var enumerableCollection = new EnumerableCollection<string>(input, input.Length);
            var output = new string[3];
            enumerableCollection.CopyTo(output, 0);
            Assert.Equal(input, output);
        }

        [Fact]
        public void When_CopyToTooSmallArray_Expect_ArgumentOutOfRangeException()
        {
            var input = new[] {"a", "b", "c"};
            var enumerableCollection = new EnumerableCollection<string>(input, input.Length);
            var output = new string[2];
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                enumerableCollection.CopyTo(output, 0);
            });
        }

        [Fact]
        public void When_CopyToInvalidIndex_Expect_ArgumentOutOfRangeException()
        {
            var input = new[] {"a", "b", "c"};
            var enumerableCollection = new EnumerableCollection<string>(input, input.Length);
            var output = new string[3];
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                enumerableCollection.CopyTo(output, 1);
            });
        }
    }
}
