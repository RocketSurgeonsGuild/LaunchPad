using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rocket.Surgery.LaunchPad.Foundation.Validation
{
    /// <summary>
    /// ValidatorFactory.
    /// Implements the <see cref="ValidatorFactoryBase" />
    /// </summary>
    /// <seealso cref="ValidatorFactoryBase" />
    internal class ValidatorFactory : ValidatorFactoryBase
    {
        private readonly IServiceProvider _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatorFactory" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ValidatorFactory(IServiceProvider context) => _context = context;

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="validatorType">Type of the validator.</param>
        /// <returns>IValidator.</returns>
        public override IValidator CreateInstance(Type validatorType)
        {
            var services = _context.GetServices(validatorType).OfType<IValidator>().ToArray();
            if (services.Length > 0 && validatorType.IsGenericType && validatorType.GetGenericArguments().Length == 1)
            {
                return (IValidator)CreateValidatorMethod.MakeGenericMethod(validatorType.GetGenericArguments()[0]).Invoke(null, new object[]
                {
                    services.AsEnumerable()
                })!;
            }

            return null!;
        }

        private static CompositeValidator<T> CreateValidator<T>(IEnumerable<object> validators) => new CompositeValidator<T>(validators.OfType<IValidator>());

        private static readonly MethodInfo CreateValidatorMethod = typeof(ValidatorFactory)
           .GetMethod(nameof(CreateValidator), BindingFlags.Static | BindingFlags.NonPublic)!;
    }
}