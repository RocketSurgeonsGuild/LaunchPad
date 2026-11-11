using Microsoft.Build.Utilities.ProjectCreation;

using Sdk.Tests.Scaffolding;

namespace Sdk.Tests;

/// <summary>
///     Strong types are optional: an application opts in by referencing <c>Rocket.Surgery.LaunchPad.Primitives</c> itself, which writes the
///     templates that describe them to the libraries it uses (docs/adr/0008-primitives-is-an-independent-strong-types-package.md). The strong
///     types here are declared in a contracts library, as an application usually does, and come from Primitives' ready-made ones.
/// </summary>
/// <param name="targetFramework">The target framework of the throwaway projects.</param>
[TargetFrameworks]
public class SdkStrongTypesTests(string targetFramework)
{
    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     The SDK never adds Primitives: an application without a reference to it gets no strong types.
    /// </summary>
    [Test]
    public async Task Should_Not_Add_Primitives_To_An_Application()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.AspNetCore);

        ( await tree.Evaluate("app", Cancellation) ).PackageReferences.ShouldNotContain(z => z.StartsWith(Primitives, StringComparison.Ordinal));
    }

    /// <summary>
    ///     The OpenAPI document gives each strong type the schema of its value, narrowed by its rules, as a named schema, in a body and in the
    ///     route and the query of a Minimal API and an MVC controller. A nullable strong type refers to the schema with null.
    /// </summary>
    [Test]
    public async Task Should_Describe_Strong_Types_In_The_OpenApi_Document()
    {
        using var tree = await Tree();

        await tree.Build("app", Cancellation);

        await Verify(await tree.Run("app", Cancellation), "json");
    }

    /// <summary>
    ///     <c>LaunchPadStrongTypeOpenApiRepresentation</c> set to <c>Inline</c> writes the value's schema in place instead of a named one.
    /// </summary>
    [Test]
    public async Task Should_Describe_Strong_Types_In_Place_When_Asked()
    {
        using var tree = await Tree(application => application.Property("LaunchPadStrongTypeOpenApiRepresentation", "Inline"));

        await tree.Build("app", Cancellation);

        await Verify(await tree.Run("app", Cancellation), "json");
    }

    /// <summary>
    ///     EF Core stores a strong type as its value, with the maximum length of its rules, and DateRange and Money as complex types: the
    ///     database the context creates, and an entity that goes in and comes back out.
    /// </summary>
    [Test]
    public async Task Should_Map_Strong_Types_In_Every_DbContext()
    {
        using var tree = await Tree();

        await tree.Build("app", Cancellation);

        await Verify(await tree.Run("app", Cancellation, "--ef"), "txt");
    }

    /// <summary>
    ///     An application whose strong types reach it only through its contracts has not referenced Primitives itself, so it gets no
    ///     templates, and ASP.NET Core describes the strong types as empty schemas and strings.
    /// </summary>
    [Test]
    public async Task Should_Not_Describe_Strong_Types_Without_A_Reference_To_Primitives()
    {
        using var tree = await Tree(referencePrimitives: false);

        await tree.Build("app", Cancellation);

        await Verify(await tree.Run("app", Cancellation), "json");
    }

    private const string Primitives = "Rocket.Surgery.LaunchPad.Primitives";

    private async Task<SdkTree> Tree(Func<ProjectCreator, ProjectCreator>? application = null, bool referencePrimitives = true)
    {
        var packages = await PackedSdk.Pack();
        var tree = new SdkTree(packages, targetFramework);
        tree.AddProject("contracts", HostModel.None, HostModelShape.Project(HostModel.None, targetFramework).ItemPackageReference(Primitives, packages.Version));
        File.WriteAllText(Path.Combine(tree.Directory, "contracts", "Program.cs"), Contracts);

        var project = HostModelShape
                     .Project(HostModel.AspNetCore, targetFramework)
                     .ItemProjectReference("../contracts/contracts.csproj")
                     .ItemPackageReference("Microsoft.EntityFrameworkCore.Sqlite", Repository.PackageVersion("Microsoft.EntityFrameworkCore.Sqlite", targetFramework));
        if (referencePrimitives) project.ItemPackageReference(Primitives, packages.Version);
        tree.AddProject("app", HostModel.AspNetCore, application?.Invoke(project) ?? project);
        File.WriteAllText(Path.Combine(tree.Directory, "app", "Program.cs"), App);
        return tree;
    }

    // A strong type of the application's own, one with rules of Primitives, and the records.
    private const string Contracts = """
        #nullable enable
        using System;
        using Rocket.Surgery.LaunchPad.StrongTypes;
        using SuperStrong.Types;

        namespace Throwaway.Contracts;

        [StrongType<Guid>]
        public sealed partial class CustomerId;

        [StrongType<string, EmailAddressRules>]
        public sealed partial class CustomerEmail;

        public sealed class Customer
        {
            public required CustomerId Id { get; set; }
            public required CustomerEmail Email { get; set; }
            public required CountryCode Country { get; set; }
            public required Age Age { get; set; }
            public required Money Balance { get; set; }
            public DateRange? Membership { get; set; }
            public Slug? Handle { get; set; }
        }
        """;

    // The application serves the operations and prints the parameters and the schemas of its OpenAPI document, or, with --ef, the database
    // its context creates and an entity read back from it. The minimal API endpoints are only in the document once the application started.
    private const string App = """
        #nullable enable
        using System;
        using System.IO;
        using System.Linq;
        using System.Text.Encodings.Web;
        using System.Text.Json;
        using System.Text.Json.Nodes;
        using Microsoft.AspNetCore.Builder;
        using Microsoft.AspNetCore.Mvc;
        using Microsoft.AspNetCore.OpenApi;
        using Microsoft.Data.Sqlite;
        using Microsoft.EntityFrameworkCore;
        using Microsoft.Extensions.DependencyInjection;
        using Microsoft.Extensions.Logging;
        using Microsoft.OpenApi;
        using Rocket.Surgery.LaunchPad.StrongTypes;
        using Throwaway;
        using Throwaway.Contracts;

        var builder = WebApplication.CreateBuilder(args);
        builder.Logging.ClearProviders();
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        builder.Services.AddDbContext<ShopContext>(options => options.UseSqlite(connection));
        var app = await builder.ConfigureDovetail();

        if (args.Contains("--ef"))
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ShopContext>();
            Console.WriteLine(context.Database.GenerateCreateScript().Trim());
            context.Database.EnsureCreated();
            var id = CustomerId.From(Guid.Parse("6f9619ff-8b86-d011-b42d-00cf4fc964ff"));
            context.Customers.Add(
                new()
                {
                    Id = id,
                    Email = CustomerEmail.From("ada@example.com"),
                    Country = CountryCode.From("GB"),
                    Age = Age.From(36),
                    Balance = Money.From(12.5m, CurrencyCode.From("GBP")),
                    Membership = DateRange.From(new(2026, 1, 1), new(2026, 12, 31)),
                    Handle = Slug.From("ada-lovelace"),
                }
            );
            context.SaveChanges();
            context.ChangeTracker.Clear();
            var customer = context.Customers.Single(z => z.Id == id);
            Console.WriteLine();
            Console.WriteLine($"{customer.Id} {customer.Email} {customer.Country} {customer.Age} {customer.Balance} {customer.Membership?.Start:O}..{customer.Membership?.End:O} {customer.Handle}");
            return;
        }

        app.MapGet("/customers/{id}", (CustomerId id, CustomerEmail email, Age? age) => (Customer?)null);
        app.MapControllers();
        app.Urls.Add("http://127.0.0.1:0");
        await app.StartAsync();

        var document = await app.Services.GetRequiredKeyedService<IOpenApiDocumentProvider>("v1").GetOpenApiDocumentAsync();
        using var writer = new StringWriter();
        document.SerializeAsV31(new OpenApiJsonWriter(writer));
        var json = JsonNode.Parse(writer.ToString())!;
        var parameters = new JsonObject();
        foreach (var (path, item) in json["paths"]!.AsObject().OrderBy(z => z.Key, StringComparer.Ordinal))
        {
            parameters[path] = item!["get"]!["parameters"]!.DeepClone();
        }

        var schemas = new JsonObject();
        if (json["components"]?["schemas"]?.AsObject() is { } components)
        {
            foreach (var (name, schema) in components.OrderBy(z => z.Key, StringComparer.Ordinal))
            {
                schemas[name] = schema?.DeepClone();
            }
        }

        Console.WriteLine(new JsonObject { ["parameters"] = parameters, ["schemas"] = schemas }.ToJsonString(new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
        await app.StopAsync();

        public sealed class ShopContext(DbContextOptions<ShopContext> options) : DbContext(options)
        {
            public DbSet<Customer> Customers => Set<Customer>();

            protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.Entity<Customer>().HasKey(z => z.Id);
        }

        [ApiController]
        [Route("customers")]
        public sealed class CustomersController : ControllerBase
        {
            [HttpGet("{id}/age")]
            public Age? Get(CustomerId id, [FromQuery] CountryCode country) => null;
        }
        """;
}
