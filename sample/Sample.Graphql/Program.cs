using HotChocolate;
using HotChocolate.Configuration;

using Rocket.Surgery.LaunchPad.AspNetCore;
using Sample.Graphql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpLogging(_ => { });

builder
   .Services
   .AddGraphQLServer()
   .AddSorting()
   .AddFiltering()
   .AddProjections()
   // HotChocolate's spatial types with a GeoJSON Geometry scalar that covers every geometry, GeometryCollection included
   .AddGeoJsonSpatialTypes()
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
