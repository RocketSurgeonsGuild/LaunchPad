using HotChocolate;
using HotChocolate.Configuration;
using HotChocolate.Types.Spatial;

using NodaTime;

using Rocket.Surgery.LaunchPad.AspNetCore;
using Rocket.Surgery.LaunchPad.HotChocolate;

using Sample.Core.Models;
using Sample.Graphql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpLogging(_ => { });

builder
   .Services
   .AddGraphQLServer()
   .AddSorting()
   .AddFiltering()
   .AddProjections()
   .ConfigureStronglyTypedId<RocketId, UuidType>()
   .ConfigureStronglyTypedId<LaunchRecordId, UuidType>()
   // HotChocolate 16 has no Instant scalar: the launch dates of Sample.Core travel as the DateTime scalar, in UTC.
   .BindRuntimeType<Instant, HotChocolate.Types.NodaTime.DateTimeType>()
   .AddTypeConverter<Instant, OffsetDateTime>(instant => instant.WithOffset(Offset.Zero))
   .AddTypeConverter<OffsetDateTime, Instant>(dateTime => dateTime.ToInstant())
//           .AddDefaultTransactionScopeHandler()

//           .AddSpatialProjections()
   .AddType(new GeometryType("Geometry", BindingBehavior.Implicit))
   .AddSpatialTypes()
   .AddSpatialFiltering()
   .AddSpatialProjections()
   .AddExecutableTypes()
   .AddQueryType()
   .AddMutationType()
   // The generated mutations, with their input and response types named.
   .AddRocketMutation()
   .AddLaunchRecordMutation()
   .ModifyOptions(
        options =>
        {
            options.RemoveUnreachableTypes = false;
            options.EnableDirectiveIntrospection = true;
            //            options.EnableTrueNullability = true;
            options.DefaultDirectiveVisibility = DirectiveVisibility.Public;
            options.RemoveUnusedTypeSystemDirectives = false;
        }
    )
   .ModifyRequestOptions(
        options => options.IncludeExceptionDetails = true
    );

var app = await builder.ConfigureDovetail();

app.UseHttpLogging();
app.UseLaunchPadRequestLogging();

app.UseRouting();

app.MapGraphQL();

app.Run();

namespace Sample.Graphql
{
    public partial class Program;
}
