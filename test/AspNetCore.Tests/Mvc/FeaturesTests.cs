using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Rocket.Surgery.SpaceShuttle.AspNetCore;
using Xunit;

namespace AspNetCore.Tests.Mvc
{
    public class FeaturesTests
    {
        class Controller1 { }

        [Fact]
        public void ShouldAddFeatureAsAnEmptyStringForControllersFoundWithoutAClearReference()
        {
            var fc = new FeatureFolderProvider();
            var model = new ControllerModel(typeof(Mvc.Controller1).GetTypeInfo(), new List<object>())
            {
                ControllerName = "Controller1"
            };

            fc.Apply(model);

            model.Properties.Should().ContainKey("feature");
            model.Properties["feature"].Should().IsSameOrEqualTo(string.Empty);
        }

        [Fact]
        public void ShouldAddExpectedFeatureName_TheNamespaceClosestToTheController()
        {
            var fc = new FeatureFolderProvider();
            var model = new ControllerModel(typeof(Controller1).GetTypeInfo(), new List<object>())
            {
                ControllerName = "Controller1"
            };

            fc.Apply(model);

            model.Properties.Should().ContainKey("feature");
            model.Properties["feature"].Should().IsSameOrEqualTo("Tests");
        }

        [Fact]
        public void ShouldAddExpectedFeatureName_TheNamespaceClosestToTheController2()
        {
            var fc = new FeatureFolderProvider();
            var model = new ControllerModel(typeof(Features.Controller1).GetTypeInfo(), new List<object>())
            {
                ControllerName = "Controller1"
            };

            fc.Apply(model);

            model.Properties.Should().ContainKey("feature");
            model.Properties["feature"].Should().IsSameOrEqualTo("Features");
        }
    }
}
