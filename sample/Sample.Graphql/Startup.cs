using AutoMapper.QueryableExtensions;
using FairyBread;
using FluentValidation;
using FluentValidation.Results;
using HotChocolate;
using HotChocolate.Configuration;
using HotChocolate.Data.Sorting;
using HotChocolate.Execution;
using HotChocolate.Internal;
using HotChocolate.Language;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;
using HotChocolate.Types.Descriptors.Definitions;
using HotChocolate.Types.Pagination;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Rocket.Surgery.LaunchPad.AspNetCore.Validation;
using Rocket.Surgery.LaunchPad.EntityFramework;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;
using Sample.Graphql;
using Sample.Graphql.Extensions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using IConventionContext = Rocket.Surgery.Conventions.IConventionContext;
using RequestDelegate = Microsoft.AspNetCore.Http.RequestDelegate;

[assembly: Convention(typeof(LocalConvention))]

namespace Sample.Graphql
{
    public class QueryType : ObjectType
    {
        public QueryType() { }

        protected override void Configure(IObjectTypeDescriptor descriptor)
        {
            descriptor
               .Field("rockets")
               .UseDbContext<RocketDbContext>()
               .Resolver(
                    (ctx, ct) => ctx.Service<RocketDbContext>().Rockets
                )
               .UsePaging(
                    typeof(ObjectType<ReadyRocket>),
                    options: new PagingOptions()
                    {
                        DefaultPageSize = 10,
                        IncludeTotalCount = true,
                        MaxPageSize = 20
                    }
                )
               .UseProjection()
               .UseFiltering()
               .UseSorting<RocketSort>()
                ;
            descriptor
               .Field("launchRecords")
               .UseDbContext<RocketDbContext>()
               .Resolver(
                    (ctx, ct) => ctx.Service<RocketDbContext>().LaunchRecords
                )
               .UseOffsetPaging(
                    typeof(ObjectType<LaunchRecord>),
                    options: new PagingOptions()
                    {
                        DefaultPageSize = 10,
                        IncludeTotalCount = true,
                        MaxPageSize = 20
                    }
                )
               .UseProjection()
               .UseFiltering()
               .UseSorting<LaunchRecordSort>()
                ;
        }

        class LaunchRecordSort : SortInputType<LaunchRecord>
        {
            protected override void Configure(ISortInputTypeDescriptor<LaunchRecord> descriptor)
            {
                descriptor.BindFieldsExplicitly();
                descriptor.Field(z => z.Partner);
                descriptor.Field(z => z.Payload);
                descriptor.Field(z => z.PayloadWeightKg);
                descriptor.Field(z => z.ActualLaunchDate);
                descriptor.Field(z => z.ScheduledLaunchDate);
            }
        }

        class RocketSort : SortInputType<ReadyRocket>
        {
            protected override void Configure(ISortInputTypeDescriptor<ReadyRocket> descriptor)
            {
                descriptor.BindFieldsExplicitly();
                descriptor.Field(z => z.Type);
                descriptor.Field(z => z.SerialNumber);
            }
        }
    }

    public class Author
    {
        public string Name { get; set; }
    }

    public class Book
    {
        public string Title { get; set; }

        public Author Author { get; set; }
    }

    public class QueryTwo
    {
        public Book GetBook() => new Book
        {
            Title = "C# in depth.",
            Author = new Author
            {
                Name = "Jon Skeet"
            }
        };
    }

    public class LocalTypeInterceptor : TypeInterceptor
    {
        public override void OnBeforeInitialize(ITypeDiscoveryContext discoveryContext) { }

        public override void OnBeforeCompleteName(
            ITypeCompletionContext completionContext,
            DefinitionBase? definition,
            IDictionary<string, object?> contextData
        )
        {
            if (definition is ObjectTypeDefinition ot)
            {
                if (ot.RuntimeType is { IsNested: true, DeclaringType: { } })
                {
                    ot.Name = $"{ot.RuntimeType.DeclaringType.Name}{ot.Name}";
                }
            }

            if (definition is InputObjectTypeDefinition iotd)
            {
                if (iotd.RuntimeType is { IsNested: true, DeclaringType: { } })
                {
                    iotd.Name = $"{iotd.RuntimeType.DeclaringType.Name}{iotd.Name}";

                    if (iotd.Name.Value.EndsWith("Input") && iotd.RuntimeType.Name == "Request")
                    {
                        iotd.Name = $"{iotd.RuntimeType.DeclaringType.Name}{iotd.RuntimeType.Name}";
                    }
                }
            }
        }
    }

    public class LocalSchemaInterceptor : SchemaInterceptor
    {
        private readonly IEnumerable<Type> _mediatorRequestTypes;

        public LocalSchemaInterceptor(IEnumerable<Type> mediatorRequestTypes)
        {
            _mediatorRequestTypes = mediatorRequestTypes;
        }

        public override void OnBeforeCreate(IDescriptorContext context, ISchemaBuilder schemaBuilder)
        {
            var method = typeof(LocalSchemaInterceptor).GetMethod(nameof(Configure), BindingFlags.Static | BindingFlags.NonPublic)!;

            schemaBuilder.AddMutationType(
                descriptor =>
                {
                    descriptor.Name("mutations");
                    foreach (var type in _mediatorRequestTypes)
                    {
                        var response = type.GetInterfaces().Single(z => z.IsGenericType && z.GetGenericTypeDefinition() == typeof(IRequest<>))
                           .GetGenericArguments()[0];
                        method.MakeGenericMethod(type, response).Invoke(null, new object?[] { descriptor.Field(type.DeclaringType!.Name) });
                    }
                }
            );
        }

        private static void Configure<TRequest, TResponse>(IObjectFieldDescriptor descriptor)
            where TRequest : IRequest<TResponse>
        {
            var d = descriptor
               .Resolver(
                    (context, ct) => context.Services.GetRequiredService<IMediator>().Send(
                        context.ArgumentValue<TRequest?>("request") ?? Activator.CreateInstance<TRequest>(),
                        ct
                    )
                );
            if (typeof(TRequest).GetProperties() is { Length: > 0 })
            {
                d.Argument("request", z => z.Type(typeof(TRequest)));
            }

            ;
            if (typeof(TResponse) == typeof(Unit))
            {
                d.Type<VoidType>();
            }
        }
    }

    public class VoidType : ScalarType
    {
        public VoidType() : base("Void", BindingBehavior.Implicit) { }
        public override bool IsInstanceOfType(IValueNode valueSyntax) => false;

        public override object? ParseLiteral(IValueNode valueSyntax, bool withDefaults = true) => null;

        public override IValueNode ParseValue(object? runtimeValue) => new NullValueNode(null);

        public override IValueNode ParseResult(object? resultValue) => new NullValueNode(null);

        public override bool TrySerialize(object? runtimeValue, out object? resultValue)
        {
            resultValue = null;
            return true;
        }

        public override bool TryDeserialize(object? resultValue, out object? runtimeValue)
        {
            runtimeValue = null;
            return true;
        }

        public override Type RuntimeType { get; } = typeof(void);
    }

    public class LocalConvention : IServiceConvention
    {
        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            var types = context.AssemblyCandidateFinder.GetCandidateAssemblies("MediatR")
               .SelectMany(z => z.GetTypes())
               .Where(typeof(IBaseRequest).IsAssignableFrom)
               .Where(z => z is { IsNested: true, DeclaringType: { } }) // TODO: Configurable?
               .ToArray();
            var sb = services.AddGraphQL()
               .ConfigureSchema(sb => sb.AddNodaTime())
               .TryAddSchemaInterceptor(new LocalSchemaInterceptor(types));
        }
    }

    public class LocalErrorFilter : IErrorFilter
    {
        public IError OnError(IError error)
        {
            if (error.Exception is IProblemDetailsData ex)
            {
                return ErrorBuilder.FromError(error)
                   .SetMessage(error.Exception.Message)
                   .RemoveException()
                   .WithProblemDetails(ex)
                   .Build();
            }

            if (error.Exception is ValidationException vx)
            {
                return ErrorBuilder.FromError(error)
                   .SetMessage(vx.Message)
                   .RemoveException()
                   .SetExtension("type", "ValidationProblemDetails")
                   .SetExtension("title", "Unprocessable Entity")
                   .SetExtension("link", "https://tools.ietf.org/html/rfc4918#section-11.2")
                   .SetExtension("errors", vx.Errors.Select<ValidationFailure, object>(FormatFailure).ToArray())
                   .Build();
                // return ErrorBuilder.FromError(error)
                //    .SetMessage(vx.Message)
                //    .SetException(null)
                //    .Build();
            }

            return error;
        }

        protected virtual ReadOnlyDictionary<string, object?> FormatFailure(ValidationFailure failure) => new FluentValidationProblemDetail(failure);
    }

    public static class LocalExtensions
    {
        public static IErrorBuilder WithProblemDetails(this IErrorBuilder error, IProblemDetailsData data)
        {
            error.SetExtension("type", "ProblemDetails");
            if (data.Title is { })
                error.SetExtension("title", data.Title);
            if (data.Link is { })
                error.SetExtension("link", data.Link);
            if (data.Instance is { })
                error.SetExtension("instance", data.Instance);
            foreach (var property in data.Properties)
            {
                if (property.Value is { })
                {
                    error.SetExtension(property.Key, property.Value);
                }
            }

            return error;
        }
    }

    public class FairyBreadValidatorProvider : IValidatorProvider
    {
        private readonly IValidatorFactory _factory;

        public FairyBreadValidatorProvider(IValidatorFactory factory)
        {
            _factory = factory;
        }

        public IEnumerable<ResolvedValidator> GetValidators(IMiddlewareContext context, IInputField argument)
        {
            var validator = _factory.GetValidator(argument.RuntimeType);
            if (validator is { })
            {
                yield return new ResolvedValidator(validator);
            }
        }

        protected static readonly Type HasOwnScopeInterfaceType = typeof(IRequiresOwnScopeValidator);
        public bool ShouldBeResolvedInOwnScope(Type validatorType) => HasOwnScopeInterfaceType.IsAssignableFrom(validatorType);
    }


    /// <summary>
    /// This is just to ensure the service provider is set.
    /// </summary>
    public class CustomInputValidationMiddleware
    {
        private readonly FieldDelegate _next;
        private readonly IFairyBreadOptions _options;
        private readonly IValidatorProvider _validatorProvider;
        private readonly IValidationResultHandler _validationResultHandler;

        public CustomInputValidationMiddleware(
            FieldDelegate next,
            IFairyBreadOptions options,
            IValidatorProvider validatorProvider,
            IValidationResultHandler validationResultHandler
        )
        {
            _next = next;
            _options = options;
            _validatorProvider = validatorProvider;
            _validationResultHandler = validationResultHandler;
        }

        public async Task InvokeAsync(IMiddlewareContext context)
        {
            var arguments = context.Field.Arguments;

            var validationResults = new List<ValidationResult>();

            foreach (var argument in arguments)
            {
                if (argument == null ||
                    !_options.ShouldValidate(context, argument))
                {
                    continue;
                }

                var resolvedValidators = _validatorProvider.GetValidators(context, argument).ToArray();
                try
                {
                    var value = context.ArgumentValue<object>(argument.Name);
                    foreach (var resolvedValidator in resolvedValidators)
                    {
                        var validationContext = new ValidationContext<object>(value);
                        validationContext.SetServiceProvider(context.Services);
                        var validationResult = await resolvedValidator.Validator.ValidateAsync(validationContext, context.RequestAborted);
                        if (validationResult != null)
                        {
                            validationResults.Add(validationResult);
                            _validationResultHandler.Handle(context, validationResult);
                        }
                    }
                }
                finally
                {
                    foreach (var resolvedValidator in resolvedValidators)
                    {
                        resolvedValidator.Scope?.Dispose();
                    }
                }
            }

            var invalidValidationResults = validationResults.Where(r => !r.IsValid);
            if (invalidValidationResults.Any())
            {
                OnInvalid(context, invalidValidationResults);
            }

            await _next(context);
        }

        protected virtual void OnInvalid(IMiddlewareContext context, IEnumerable<ValidationResult> invalidValidationResults)
        {
            throw new ValidationException(invalidValidationResults.SelectMany(vr => vr.Errors));
        }
    }

    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton<IValidatorProvider, FairyBreadValidatorProvider>();
            services.TryAddSingleton<IValidationResultHandler, DefaultValidationResultHandler>();
            services.AddSingleton<IFairyBreadOptions>(new DefaultFairyBreadOptions());
            services
               .AddPooledDbContextFactory<RocketDbContext>((s, o) => { })
                //.AddGraphQL()
               .AddGraphQLServer()
                // .AddFairyBread() // covered above
               .UseField<CustomInputValidationMiddleware>()
               .AddSorting()
               .AddFiltering()
                // .SetPagingOptions(new PagingOptions() { })
               .AddProjections()
               .AddQueryType<QueryType>()
                // .AddErrorFilter<ValidationErrorFilter>()
               .AddErrorFilter<LocalErrorFilter>()
                // .AddMutationType(z => z.Name("mutations"))
                // .ConfigureSchema(z => z.AddMutationType(typeof(MutationType)))
                // .AddQueryType<QueryType>(z => z.Name("Query"))
                // .AddMutationType<MutationType>()
                // .AddType<>()
                // .AddType<QueryType>()
               .ConfigureSchema(
                    sb =>
                    {
                        sb.TryAddTypeInterceptor(new LocalTypeInterceptor());
                        // sb.AddMutationType(typeof(CreateRocket.Request));
                        // sb.AddMutationType(typeof(EditRocket.Request));
                        // sb.AddType(typeof(CreateRocket.Request));
                        // sb.AddType(typeof(CreateRocket.Response));
                    }
                )
                ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Should this move into an extension method?
            app.UseSerilogRequestLogging(
                x =>
                {
                    x.GetLevel = LaunchPadLogHelpers.DefaultGetLevel;
                    x.EnrichDiagnosticContext = LaunchPadLogHelpers.DefaultEnrichDiagnosticContext;
                }
            );
            app.UseMetricsAllMiddleware();

            app.UseRouting();

            app.UseEndpoints(
                endpoints => { endpoints.MapGraphQL(); }
            );
        }
    }
}