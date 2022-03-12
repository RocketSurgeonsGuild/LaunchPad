// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;

namespace Microsoft.AspNetCore.Authorization.Test;

public class AuthorizationEndpointConventionBuilderExtensionsTests
{
    [Fact]
    public void RequireAuthorization_IAuthorizeData()
    {
        // Arrange
        var builder = new TestEndpointConventionBuilder();
        var metadata = new AuthorizeAttribute();

        // Act
        builder.RequireAuthorization(metadata);

        // Assert
        var convention = Assert.Single(builder.Conventions);

        var endpointModel = new RouteEndpointBuilder(context => Task.CompletedTask, RoutePatternFactory.Parse("/"), 0);
        convention(endpointModel);

        Assert.Equal(metadata, Assert.Single(endpointModel.Metadata));
    }

    [Fact]
    public void RequireAuthorization_IAuthorizeData_Empty()
    {
        // Arrange
        var builder = new TestEndpointConventionBuilder();

        // Act
        builder.RequireAuthorization(Array.Empty<IAuthorizeData>());

        // Assert
        var convention = Assert.Single(builder.Conventions);

        var endpointModel = new RouteEndpointBuilder(context => Task.CompletedTask, RoutePatternFactory.Parse("/"), 0);
        convention(endpointModel);

        var authMetadata = Assert.IsAssignableFrom<IAuthorizeData>(Assert.Single(endpointModel.Metadata));
        Assert.Null(authMetadata.Policy);
    }

    [Fact]
    public void RequireAuthorization_PolicyName()
    {
        // Arrange
        var builder = new TestEndpointConventionBuilder();

        // Act
        builder.RequireAuthorization("policy");

        // Assert
        var convention = Assert.Single(builder.Conventions);

        var endpointModel = new RouteEndpointBuilder(context => Task.CompletedTask, RoutePatternFactory.Parse("/"), 0);
        convention(endpointModel);

        var authMetadata = Assert.IsAssignableFrom<IAuthorizeData>(Assert.Single(endpointModel.Metadata));
        Assert.Equal("policy", authMetadata.Policy);
    }

    [Fact]
    public void RequireAuthorization_PolicyName_Empty()
    {
        // Arrange
        var builder = new TestEndpointConventionBuilder();

        // Act
        builder.RequireAuthorization(Array.Empty<string>());

        // Assert
        var convention = Assert.Single(builder.Conventions);

        var endpointModel = new RouteEndpointBuilder(context => Task.CompletedTask, RoutePatternFactory.Parse("/"), 0);
        convention(endpointModel);

        var authMetadata = Assert.IsAssignableFrom<IAuthorizeData>(Assert.Single(endpointModel.Metadata));
        Assert.Null(authMetadata.Policy);
    }

    [Fact]
    public void RequireAuthorization_Default()
    {
        // Arrange
        var builder = new TestEndpointConventionBuilder();

        // Act
        builder.RequireAuthorization();

        // Assert
        var convention = Assert.Single(builder.Conventions);

        var endpointModel = new RouteEndpointBuilder(context => Task.CompletedTask, RoutePatternFactory.Parse("/"), 0);
        convention(endpointModel);

        var authMetadata = Assert.IsAssignableFrom<IAuthorizeData>(Assert.Single(endpointModel.Metadata));
        Assert.Null(authMetadata.Policy);
    }

    [Fact]
    public void RequireAuthorization_ChainedCall()
    {
        // Arrange
        var builder = new TestEndpointConventionBuilder();

        // Act
        var chainedBuilder = builder.RequireAuthorization();

        // Assert
        Assert.True(chainedBuilder.TestProperty);
    }

    [Fact]
    public void AllowAnonymous_Default()
    {
        // Arrange
        var builder = new TestEndpointConventionBuilder();

        // Act
        builder.AllowAnonymous();

        // Assert
        var convention = Assert.Single(builder.Conventions);

        var endpointModel = new RouteEndpointBuilder(context => Task.CompletedTask, RoutePatternFactory.Parse("/"), 0);
        convention(endpointModel);

        Assert.IsAssignableFrom<IAllowAnonymous>(Assert.Single(endpointModel.Metadata));
    }

    [Fact]
    public void AllowAnonymous_ChainedCall()
    {
        // Arrange
        var builder = new TestEndpointConventionBuilder();

        // Act
        var chainedBuilder = builder.AllowAnonymous();

        // Assert
        Assert.True(chainedBuilder.TestProperty);
    }

    private class TestEndpointConventionBuilder : IEndpointConventionBuilder
    {
        public IList<Action<EndpointBuilder>> Conventions { get; } = new List<Action<EndpointBuilder>>();
        public bool TestProperty { get; } = true;

        public void Add(Action<EndpointBuilder> convention)
        {
            Conventions.Add(convention);
        }
    }
}
