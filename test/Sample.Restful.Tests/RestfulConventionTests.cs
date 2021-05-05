using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Xunit;

namespace Sample.Restful.Tests
{
    public class RestfulConventionTests
    {
        [Theory]
        [ClassData(typeof(ApiDescriptionData<TestWebHost>))]
        public void Should_Have_Success_Response_Types(string name, ApiDescription description)
        {
            description.SupportedResponseTypes.Should().Contain(z => z.StatusCode >= 200 && z.StatusCode < 300);
        }

        [Theory]
        [ClassData(typeof(ApiDescriptionData<TestWebHost>))]
        public void Should_Have_Not_Found_Responses(string name, ApiDescription description)
        {
            description.SupportedResponseTypes.Should().Contain(z => z.StatusCode == StatusCodes.Status404NotFound);
        }

        [Theory]
        [ClassData(typeof(ApiDescriptionData<TestWebHost>))]
        public void Should_Have_Validation_Responses(string name, ApiDescription description)
        {
            description.SupportedResponseTypes.Should().Contain(z => z.StatusCode == StatusCodes.Status422UnprocessableEntity);
        }

        [Theory]
        [ClassData(typeof(ApiDescriptionData<TestWebHost>))]
        public void Should_Have_Bad_Request_Responses(string name, ApiDescription description)
        {
            description.SupportedResponseTypes.Should().Contain(z => z.IsDefaultResponse);
        }
    }
}