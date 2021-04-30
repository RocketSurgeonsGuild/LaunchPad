using FluentAssertions;
using Rocket.Surgery.LaunchPad.AspNetCore.OpenApi.Validation.Core;
using Xunit;

namespace AspNetCore.Tests.Validation
{
    public class StringTests
    {
        [Theory]
        [InlineData("BlogId", "blogId", true)]
        [InlineData("blogId", "blogId", true)]
        [InlineData("blogId", "blogId1", false)]
        [InlineData("blogIdentifier", "blog_id", false)]
        [InlineData("blog_id", "BlogId", true)]
        [InlineData("BlogId", "blog-id", true)]
        public void EqualsIgnoreAll(string left, string right, bool shouldBeEqual)
        {
            left.EqualsIgnoreAll(right).Should().Be(shouldBeEqual);
        }
    }
}