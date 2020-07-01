using System;
using FluentValidation;

namespace Rocket.Surgery.Extensions.FluentValidation
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
            var service = _context.GetService(validatorType);
            if (service is IValidator validator)
            {
                return validator;
            }

            return null!;
        }
    }
}