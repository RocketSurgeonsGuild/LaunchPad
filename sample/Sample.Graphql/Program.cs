using System.Runtime.Loader;
using HotChocolate.Types.Spatial;
using Microsoft.Extensions.DependencyModel;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Rocket.Surgery.LaunchPad.HotChocolate;
using Sample.Core.Models;
using Sample.Graphql;

var builder = await WebApplication
             .CreateBuilder(args)
                   .LaunchWith(RocketBooster.For(Imports.GetConventions), b => b.Set(AssemblyLoadContext.Default));

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
   .ModifyRequestOptions(options => options.IncludeExceptionDetails = true);

var app = builder.Build();

app.UseHttpLogging();
app.UseLaunchPadRequestLogging();

app.UseRouting();

app.MapGraphQL();

app.Run();

public partial class Program;
