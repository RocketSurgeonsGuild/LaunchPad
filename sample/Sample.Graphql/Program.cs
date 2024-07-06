using HotChocolate.Configuration;
using HotChocolate.Types.Spatial;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Rocket.Surgery.LaunchPad.HotChocolate;
using Sample.Core.Models;
using Sample.Graphql;

var builder = WebApplication.CreateBuilder(args);

builder
   .Services
   .AddGraphQLServer()
   .AddSorting()
   .AddFiltering()
   .AddProjections()
   .ConfigureStronglyTypedId<RocketId, UuidType>()
   .ConfigureStronglyTypedId<LaunchRecordId, UuidType>()
//           .AddDefaultTransactionScopeHandler()

//           .AddSpatialProjections()
   .AddType(new GeometryType("Geometry", BindingBehavior.Implicit))
   .AddSpatialTypes()
   .AddSpatialFiltering()
   .AddSpatialProjections()
   .AddExecutableTypes()
   .AddQueryType()
   .AddMutationType()
   .ModifyOptions(
        options =>
        {
            options.RemoveUnreachableTypes = false;
            options.EnableDirectiveIntrospection = true;
            options.EnableTrueNullability = true;
            options.DefaultDirectiveVisibility = DirectiveVisibility.Public;
            options.RemoveUnusedTypeSystemDirectives = false;
        }
    )
   .ModifyRequestOptions(
        options => { options.IncludeExceptionDetails = true; }
    );

var app = await builder
   .LaunchWith(RocketBooster.For(Imports.Instance));

app.UseHttpLogging();
app.UseLaunchPadRequestLogging();

app.UseRouting();

app.MapGraphQL();

app.Run();

public partial class Program;