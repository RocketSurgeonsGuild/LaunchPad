﻿using System;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Rocket.Surgery.LaunchPad.Restful.OpenApi.Validation
{
    /// <summary>
    /// Allows for the creation of validators that have dependencies on scoped services.
    /// </summary>
    public class HttpContextServiceProviderValidatorFactory : ValidatorFactoryBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpContextServiceProviderValidatorFactory"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">Access to the current HttpContext</param>
        public HttpContextServiceProviderValidatorFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <inheritdoc/>
        public override IValidator CreateInstance(Type validatorType)
        {
            var serviceProvider = _httpContextAccessor.HttpContext.RequestServices;
            return serviceProvider.GetService(validatorType) as IValidator;
        }
    }
}
