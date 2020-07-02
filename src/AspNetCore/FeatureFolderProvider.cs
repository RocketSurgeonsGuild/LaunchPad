using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Rocket.Surgery.Build.Information;

namespace Rocket.Surgery.LaunchPad.AspNetCore
{
    /// <summary>
    /// Class FeatureConvention.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ApplicationModels.IControllerModelConvention" />
    /// TODO Edit XML Comment Template for FeatureConvention
    public class FeatureFolderProvider : IControllerModelConvention
    {
        /// <summary>
        /// Called to apply the convention to the <see cref="ControllerModel" />.
        /// </summary>
        /// <param name="controller">The <see cref="ControllerModel" />.</param>
        /// TODO Edit XML Comment Template for Apply
        public void Apply([NotNull] ControllerModel controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            controller.Properties.Add("feature", GetFeatureName(controller));
        }

        /// <summary>
        /// Gets the name of the feature.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <returns>System.String.</returns>
        /// TODO Edit XML Comment Template for GetFeatureName
        internal string GetFeatureName(ControllerModel controller)
        {
            var assemblyInformationProvider = new InformationProvider(controller.ControllerType.Assembly);
            var possibleNamespaces = assemblyInformationProvider.GetValue("FeatureFolderNamespace")
                .Concat(new[] { controller.ControllerType.Assembly.GetName().Name! });

            foreach (var @namespace in possibleNamespaces)
            {
                var controllerFullName = controller.ControllerType.FullName!;
                if (controllerFullName.StartsWith(@namespace, StringComparison.OrdinalIgnoreCase))
                {
                    var featureName = controllerFullName.Substring(@namespace.Length + 1);
                    if (featureName.Contains(".", StringComparison.Ordinal))
                    {
                        var items = featureName.Split('.');
                        return items[^2];
                    }
                    return controller.ControllerName;
                }
            }

            return "Default";
        }
    }
}
