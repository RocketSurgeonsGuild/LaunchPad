using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.LaunchPad.Restful.Composition;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCore.Tests.Restful
{
    public class RestfulApiMethodBuilderTests : AutoFakeTest
    {
        public RestfulApiMethodBuilderTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void Should_Have_Method()
        {
            var builder = new RestfulApiMethodBuilder(RestfulApiMethod.List);
            builder.Method.Should().Be(RestfulApiMethod.List);
        }

        [Theory]
        [InlineData("MyActionName")]
        [InlineData("myactionname")]
        [InlineData("MYACTIONNAME")]
        public void Matches_Method_Name(string nameToMatch)
        {
            var builder = new RestfulApiMethodBuilder(RestfulApiMethod.List);
            builder.MatchName(nameToMatch);

            builder.IsMatch(
                CreateActionModel(nameof(MyActionName))
            ).Should().BeTrue();
        }

        [Theory]
        [InlineData("My")]
        [InlineData("my")]
        [InlineData("MY")]
        public void Matches_Method_Prefix(string prefix)
        {
            var builder = new RestfulApiMethodBuilder(RestfulApiMethod.List);
            builder.MatchPrefix(prefix);

            builder.IsMatch(
                CreateActionModel(nameof(MyActionName))
            ).Should().BeTrue();
        }

        [Theory]
        [InlineData("Name")]
        [InlineData("name")]
        [InlineData("NAME")]
        public void Matches_Method_Suffix(string suffix)
        {
            var builder = new RestfulApiMethodBuilder(RestfulApiMethod.List);
            builder.MatchSuffix(suffix);

            builder.IsMatch(
                CreateActionModel(nameof(MyActionName))
            ).Should().BeTrue();
        }


        [Theory]
        [InlineData("OneParameter", 0)]
        [InlineData("TwoParameter", 1)]
        [InlineData("ThreeParameter", 2)]
        public void Matches_Parameter_Existence(string methodName, int parameterIndex)
        {
            var builder = new RestfulApiMethodBuilder(RestfulApiMethod.List);
            builder.HasParameter(parameterIndex);

            builder.IsMatch(CreateActionModel(methodName)).Should().BeTrue();
        }


        [Theory]
        [InlineData("OneParameter", 0, typeof(Guid))]
        [InlineData("TwoParameter", 1, typeof(object))]
        [InlineData("ThreeParameter", 2, typeof(int))]
        [InlineData("MyActionName", 1, typeof(IBaseRequest))]
        public void Matches_Parameter_Type(string methodName, int parameterIndex, Type type)
        {
            var builder = new RestfulApiMethodBuilder(RestfulApiMethod.List);
            builder.MatchParameterType(parameterIndex, type);

            builder.IsMatch(CreateActionModel(methodName)).Should().BeTrue();
        }


        [Theory]
        [InlineData("OneParameter", 0, "id")]
        [InlineData("TwoParameter", 1, "two")]
        [InlineData("ThreeParameter", 2, "three")]
        public void Matches_Parameter_Name(string methodName, int parameterIndex, string parameterName)
        {
            var builder = new RestfulApiMethodBuilder(RestfulApiMethod.List);
            builder.MatchParameterName(parameterIndex, parameterName);

            builder.IsMatch(CreateActionModel(methodName)).Should().BeTrue();
        }


        [Theory]
        [InlineData("MyActionName", 1, "Request")]
        public void Matches_Parameter_Suffix(string methodName, int parameterIndex, string parameterName)
        {
            var builder = new RestfulApiMethodBuilder(RestfulApiMethod.List);
            builder.MatchParameterSuffix(parameterIndex, parameterName);

            builder.IsMatch(CreateActionModel(methodName)).Should().BeTrue();
        }


        [Theory]
        [InlineData("MyActionName", 1, "base")]
        public void Matches_Parameter_Prefix(string methodName, int parameterIndex, string parameterName)
        {
            var builder = new RestfulApiMethodBuilder(RestfulApiMethod.List);
            builder.MatchParameterPrefix(parameterIndex, parameterName);

            builder.IsMatch(CreateActionModel(methodName)).Should().BeTrue();
        }

        private ActionModel CreateActionModel(string methodName, string? name = null) => new ActionModel(
            GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic),
            new List<object>()
        )
        {
            ActionName = name ?? "MyActionName"
        };

        private Task<ActionResult<object>> MyActionName(Guid id, IBaseRequest baseRequest)
        {
            throw new NotImplementedException();
        }

        private Task<ActionResult<object>> OneParameter(Guid id)
        {
            throw new NotImplementedException();
        }

        private Task<ActionResult<object>> TwoParameter(Guid id, object two)
        {
            throw new NotImplementedException();
        }

        private Task<ActionResult<object>> ThreeParameter(Guid id, object two, int three)
        {
            throw new NotImplementedException();
        }
    }
}