using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using Microsoft.Build.Utilities.ProjectCreation;

using Sdk.Tests.Scaffolding;

namespace Sdk.Tests;

/// <summary>
///     Domain primitives are optional: an application opts in by referencing <c>Rocket.Surgery.LaunchPad.Primitives</c> itself, which writes the
///     templates that describe them to the libraries it uses (docs/adr/0011-primitives-is-a-port-of-dotnet-domain-primitives.md). The domain
///     primitives here are declared in a contracts library, as an application usually does: a strong ID, an email address, a number with a
///     range, a rating, a smart enum, a value object of the application's own and the money and date range kinds, as structs and classes.
/// </summary>
/// <param name="targetFramework">The target framework of the throwaway projects.</param>
[TargetFrameworks]
public partial class SdkPrimitivesTests(string targetFramework)
{
    private static CancellationToken Cancellation => TestContext.Current!.Execution.CancellationToken;

    /// <summary>
    ///     The SDK never adds Primitives: an application without a reference to it gets no domain primitives.
    /// </summary>
    [Test]
    public async Task Should_Not_Add_Primitives_To_An_Application()
    {
        using var tree = new SdkTree(await PackedSdk.Pack(), targetFramework);
        tree.AddProject("app", HostModel.AspNetCore);

        await ( await tree.Evaluate("app", Cancellation) ).PackageReferences.Should().All(z => !z.StartsWith(Primitives, StringComparison.Ordinal));
    }

    /// <summary>
    ///     Primitives and its analyzer package each carry the notice of dotnet-domain-primitives, which they are ported from.
    /// </summary>
    [Test]
    [Arguments(Primitives)]
    [Arguments(Primitives + ".Analyzers")]
    public async Task Should_Ship_The_Third_Party_Notices(string packageId)
    {
        var packages = await PackedSdk.Pack();
        using var archive = ZipFile.OpenRead(Path.Combine(packages.Feed, $"{packageId}.{packages.Version}.nupkg"));
        var notices = archive.GetEntry("THIRD-PARTY-NOTICES.md");

        await notices.Should().NotBeNull();
        using var reader = new StreamReader(notices!.Open());
        await ( await reader.ReadToEndAsync(Cancellation) ).Should().Contain("dotnet-domain-primitives");
    }

    /// <summary>
    ///     The OpenAPI document gives each domain primitive the schema of its value, narrowed by its rules, as a named schema, in a body and in the
    ///     route and the query of a Minimal API and an MVC controller, and each value object the named schema of an object of its values: a
    ///     domain primitive value refers to its schema, a NodaTime value is text, and a nullable value may be null. A nullable domain primitive
    ///     or value object refers to the schema with null.
    /// </summary>
    [Test]
    public async Task Should_Describe_Domain_Primitives_In_The_OpenApi_Document()
    {
        using var tree = await Tree();

        await tree.Build("app", Cancellation);

        await Verify(await tree.Run("app", Cancellation), "json");
    }

    /// <summary>
    ///     <c>LaunchPadPrimitivesOpenApiRepresentation</c> set to <c>Inline</c> writes the value's schema in place instead of a named one, in a
    ///     value object too. A value object keeps its named schema, which a nullable property refers to with null: the schema itself is not
    ///     nullable.
    /// </summary>
    [Test]
    public async Task Should_Describe_Domain_Primitives_In_Place_When_Asked()
    {
        using var tree = await Tree(application => application.Property("LaunchPadPrimitivesOpenApiRepresentation", "Inline"));

        await tree.Build("app", Cancellation);

        await Verify(await tree.Run("app", Cancellation), "json");
    }

    /// <summary>
    ///     EF Core stores a domain primitive as its value, with the maximum length and the scale of its rules, and a value object (the
    ///     application's own, money and a date range) as a complex type: the database the context creates (SQLite's column types have no
    ///     length or precision, so the type mapping of each column is printed too), a key that is a strong ID of a Guid generated when the
    ///     entity is added, and the entity read back out.
    /// </summary>
    [Test]
    public async Task Should_Map_Domain_Primitives_In_Every_DbContext()
    {
        using var tree = await Tree();

        await tree.Build("app", Cancellation);
        var output = await tree.Run("app", Cancellation, "--ef");

        await output.Should().Contain("Id: CustomerId stored as Guid, size -, precision -, scale -, generated OnAdd by a value generator");
        await output.Should().Contain("Key generated on insert: True (version 7)");
        await output.Should().Contain("Complex types: Address (Address), Balance (Balance), Membership (Membership)");
        await output.Should().Contain("Email: CustomerEmail stored as String, size 320,");
        await output.Should().Contain("Rating: Stars stored as Decimal, size -, precision 18, scale 1,");
        await Verify(output, "txt");
    }

    /// <summary>
    ///     In a GraphQL schema a strong ID is an ID and any other domain primitive the scalar of its value (an email address is a String): it is
    ///     read and written as its value, and a value that breaks its rules is an input error. A value object is an object of its values.
    /// </summary>
    [Test]
    public async Task Should_Make_Domain_Primitives_Scalars_Of_Their_Value()
    {
        using var tree = await Tree(graphQL: true);

        await tree.Build("app", Cancellation);
        var output = await tree.Run("app", Cancellation);

        await output.Should().Contain("customer(id: ID!, country: String!): Customer!");
        await output.Should().Contain("  id: ID!");
        await Verify(output, "txt");
    }

    /// <summary>
    ///     <c>LaunchPadPrimitivesGraphQLRepresentation</c> set to <c>Named</c> makes each domain primitive a scalar named after it, written as its
    ///     value.
    /// </summary>
    [Test]
    public async Task Should_Make_Domain_Primitives_Named_Scalars_When_Asked()
    {
        using var tree = await Tree(application => application.Property("LaunchPadPrimitivesGraphQLRepresentation", "Named"), graphQL: true);

        await tree.Build("app", Cancellation);

        await Verify(await tree.Run("app", Cancellation), "txt");
    }

    /// <summary>
    ///     An application that references HotChocolate.Data gets the filtering joint: a strong ID field is filtered by its ID (equal, not equal,
    ///     in and not in), written as the ID scalar, instead of by an object filter of the strong ID's properties.
    /// </summary>
    [Test]
    public async Task Should_Filter_Strong_IDs_By_Their_ID()
    {
        using var tree = await Tree(graphQL: true, program: FilteringApp, packages: ["HotChocolate.Data"]);

        await tree.Build("app", Cancellation);
        var output = await tree.Run("app", Cancellation);

        await output.Should().Contain("id: IdOperationFilterInput");
        await Verify(output, "txt");
    }

    /// <summary>
    ///     With <c>LaunchPadPrimitivesGraphQLRepresentation</c> set to <c>Named</c>, a strong ID field is filtered by the scalar named after
    ///     the strong ID.
    /// </summary>
    [Test]
    public async Task Should_Filter_Strong_IDs_By_Their_Named_Scalar_When_Asked()
    {
        using var tree = await Tree(
            application => application.Property("LaunchPadPrimitivesGraphQLRepresentation", "Named"),
            graphQL: true,
            program: FilteringApp,
            packages: ["HotChocolate.Data"]
        );

        await tree.Build("app", Cancellation);
        var output = await tree.Run("app", Cancellation);

        await output.Should().Contain("id: CustomerIdOperationFilterInput");
        await Verify(output, "txt");
    }

    /// <summary>
    ///     An application whose domain primitives reach it only through its contracts has not referenced Primitives itself, so it gets no
    ///     templates, and ASP.NET Core describes the domain primitives as it describes any other type.
    /// </summary>
    [Test]
    public async Task Should_Not_Describe_Domain_Primitives_Without_A_Reference_To_Primitives()
    {
        using var tree = await Tree(referencePrimitives: false);

        await tree.Build("app", Cancellation);

        await Verify(await tree.Run("app", Cancellation), "json");
    }

    /// <summary>
    ///     An application that references Dapper gets a type handler for every domain primitive it declares or references, registered when it
    ///     loads: a domain primitive goes into a command as its value and comes back out of a query through its rules, as a column of a row
    ///     and as a value of its own.
    /// </summary>
    [Test]
    public async Task Should_Read_And_Write_Domain_Primitives_With_Dapper()
    {
        using var tree = await Tree(program: DapperApp, packages: ["Dapper"]);

        await tree.Build("app", Cancellation);

        await Verify(await tree.Run("app", Cancellation), "txt");
    }

    /// <summary>
    ///     An application that turns OpenTelemetry on (the SDK gives an ASP.NET Core application <c>Joinery.Dovetail.OpenTelemetry</c>) gets the
    ///     instrumentation joint, which collects the counters of domain primitives and listens to their traces.
    /// </summary>
    [Test]
    public async Task Should_Collect_The_Metrics_Of_Domain_Primitives_With_OpenTelemetry()
    {
        using var tree = await Tree(program: OpenTelemetryApp);

        await tree.Build("app", Cancellation);
        var output = await tree.Run("app", Cancellation);

        await output.Should().Contain("Joint:True");
        await Verify(output, "txt");
    }

    /// <summary>
    ///     A domain primitive of a NodaTime value reads and writes its value through the JSON options it is given, so with
    ///     <c>Rocket.Surgery.LaunchPad.NodaTime</c> (which the SDK adds to an application that references NodaTime) it is written in LaunchPad's
    ///     patterns and read in every pattern LaunchPad's converters accept.
    /// </summary>
    [Test]
    public async Task Should_Serialize_NodaTime_Domain_Primitives_Through_LaunchPad_NodaTime()
    {
        using var tree = await Tree(program: NodaTimeApp, packages: ["NodaTime"], contracts: NodaTimeContracts, contractsPackages: ["NodaTime"]);

        await tree.Build("app", Cancellation);

        await Verify(await tree.Run("app", Cancellation), "txt");
    }

    /// <summary>
    ///     The MSBuild settings of the generator change the domain primitives the project declares: <c>LaunchPadPrimitivesTrim</c>,
    ///     <c>LaunchPadPrimitivesNotEmpty</c> and <c>LaunchPadPrimitivesMaxLength</c> the rules of a string without its own, and
    ///     <c>LaunchPadPrimitivesExceptionType</c> the exception <c>Create</c> throws.
    /// </summary>
    [Test]
    public async Task Should_Apply_The_MSBuild_Settings_Of_The_Generator()
    {
        using var tree = await Tree(
            application => application
                          .Property("LaunchPadPrimitivesTrim", "true")
                          .Property("LaunchPadPrimitivesNotEmpty", "true")
                          .Property("LaunchPadPrimitivesMaxLength", "5")
                          .Property("LaunchPadPrimitivesExceptionType", "InvalidNicknameException"),
            program: SettingsApp
        );

        await tree.Build("app", Cancellation);

        await Verify(await tree.Run("app", Cancellation), "txt");
    }

    /// <summary>
    ///     A console application that references Primitives and a library with one domain primitive of each kind publishes with native AOT
    ///     without a trimming or AOT warning, and the domain primitives work in the native executable: rules, parsing, the exception of a
    ///     rejected value, JSON through a source generated context, arithmetic.
    /// </summary>
    [Test]
    public async Task Should_Publish_Domain_Primitives_With_Native_Aot()
    {
        if (await NativeToolchainMissing() is { } reason) Skip.Test(reason);

        var packages = await PackedSdk.Pack();
        using var tree = new SdkTree(packages, targetFramework);
        tree.AddProject("contracts", HostModel.None, HostModelShape.Project(HostModel.None, targetFramework).ItemPackageReference(Primitives, packages.Version));
        await File.WriteAllTextAsync(Path.Combine(tree.Directory, "contracts", "Program.cs"), AotContracts, Cancellation);
        tree.AddProject(
            "aot",
            HostModel.None,
            HostModelShape
               .Project(HostModel.None, targetFramework)
               .Property("OutputType", "Exe")
               .Property("PublishAot", "true")
               .ItemProjectReference("../contracts/contracts.csproj")
               .ItemPackageReference(Primitives, packages.Version)
        );
        await File.WriteAllTextAsync(Path.Combine(tree.Directory, "aot", "Program.cs"), AotApp, Cancellation);

        var (publish, folder) = await tree.Publish("aot", RuntimeInformation.RuntimeIdentifier, Cancellation);
        var warnings = TrimmingWarning().Matches(publish).Select(z => z.Value).Distinct(StringComparer.Ordinal).ToArray();
        var output = await RunExecutable(Path.Combine(folder, OperatingSystem.IsWindows() ? "aot.exe" : "aot"));

        await warnings.Should().BeEmpty();
        await Verify(output, "txt");
    }

    private const string Primitives = "Rocket.Surgery.LaunchPad.Primitives";

    [GeneratedRegex(@"warning IL[23]\d{3}:[^\r\n]*")]
    private static partial Regex TrimmingWarning();

    /// <summary>
    ///     Why native AOT can't publish on this machine, if it can't: the ILCompiler links with the platform's native toolchain. On macOS
    ///     <c>/usr/bin/xcrun</c> is there without the Command Line Tools too, so it has to find clang.
    /// </summary>
    private static async Task<string?> NativeToolchainMissing()
    {
        if (OperatingSystem.IsWindows()) return "native AOT needs the Visual Studio C++ tools, which these tests don't look for on Windows";
        if (OperatingSystem.IsMacOS())
        {
            return await XcrunFindsClang() ? null : "native AOT needs clang, and 'xcrun --find clang' finds none: install the Xcode Command Line Tools";
        }

        string[] linkers = ["/usr/bin/clang", "/usr/bin/gcc", "/usr/local/bin/clang"];
        return linkers.Any(File.Exists) ? null : $"native AOT needs a native linker ({string.Join(", ", linkers)}), and this machine has none";
    }

    private static async Task<bool> XcrunFindsClang()
    {
        if (!File.Exists("/usr/bin/xcrun")) return false;
        var psi = new ProcessStartInfo("/usr/bin/xcrun", ["--find", "clang"]) { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false, };
        using var process = Process.Start(psi);
        if (process is null) return false;
        var output = process.StandardOutput.ReadToEndAsync(Cancellation);
        var error = process.StandardError.ReadToEndAsync(Cancellation);
        await process.WaitForExitAsync(Cancellation);
        await output;
        await error;
        return process.ExitCode == 0;
    }

    private static async Task<string> RunExecutable(string path)
    {
        var psi = new ProcessStartInfo(path) { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false, };
        using var process = Process.Start(psi)!;
        var output = process.StandardOutput.ReadToEndAsync(Cancellation);
        var error = process.StandardError.ReadToEndAsync(Cancellation);
        await process.WaitForExitAsync(Cancellation);
        var text = await output + await error;
        return process.ExitCode == 0 ? text : throw new InvalidOperationException($"{path} exited with {process.ExitCode}:{Environment.NewLine}{text}");
    }

    /// <summary>
    ///     A contracts library with the domain primitives (and <paramref name="contracts" />), and an ASP.NET Core application that references
    ///     it and, unless told otherwise, Primitives, with <paramref name="program" /> (the OpenAPI and EF Core application by default, for
    ///     which the library also declares the value objects of a booking and references NodaTime). The application references
    ///     <paramref name="packages" /> and the library <paramref name="contractsPackages" />. The domain primitives
    ///     are declared in a library of their own, as an application usually does, and can't be in the project of a JSON context that
    ///     serializes them: the JSON source generator doesn't see the converter the domain primitive generator declares in the same
    ///     compilation, as no source generator sees what another one writes.
    /// </summary>
    private async Task<SdkTree> Tree(
        Func<ProjectCreator, ProjectCreator>? application = null,
        bool referencePrimitives = true,
        bool graphQL = false,
        string? program = null,
        string[]? packages = null,
        string? contracts = null,
        string[]? contractsPackages = null
    )
    {
        if (program is null && !graphQL)
        {
            contracts = BookingContracts + contracts;
            contractsPackages = [.. contractsPackages ?? [], "NodaTime"];
        }

        var packed = await PackedSdk.Pack();
        var tree = new SdkTree(packed, targetFramework);
        var library = HostModelShape.Project(HostModel.None, targetFramework).ItemPackageReference(Primitives, packed.Version);
        foreach (var package in contractsPackages ?? [])
        {
            library.ItemPackageReference(package, Repository.PackageVersion(package, targetFramework));
        }

        tree.AddProject("contracts", HostModel.None, library);
        await File.WriteAllTextAsync(Path.Combine(tree.Directory, "contracts", "Program.cs"), Contracts + ( contracts ?? "" ), Cancellation);

        var project = HostModelShape
                     .Project(HostModel.AspNetCore, targetFramework)
                     .ItemProjectReference("../contracts/contracts.csproj")
                     .ItemPackageReference("Microsoft.EntityFrameworkCore.Sqlite", Repository.PackageVersion("Microsoft.EntityFrameworkCore.Sqlite", targetFramework));
        if (referencePrimitives) project.ItemPackageReference(Primitives, packed.Version);
        if (graphQL) project.ItemPackageReference("HotChocolate.AspNetCore", Repository.PackageVersion("HotChocolate.AspNetCore", targetFramework));
        foreach (var package in packages ?? [])
        {
            project.ItemPackageReference(package, Repository.PackageVersion(package, targetFramework));
        }

        tree.AddProject("app", HostModel.AspNetCore, application?.Invoke(project) ?? project);
        await File.WriteAllTextAsync(Path.Combine(tree.Directory, "app", "Program.cs"), program ?? ( graphQL ? GraphQLApp : App ), Cancellation);
        return tree;
    }

    // One domain primitive of each kind, as structs and classes.
    private const string Contracts = """
        #nullable enable
        using System;
        using Rocket.Surgery.LaunchPad.Primitives;

        namespace Throwaway.Contracts;

        [StrongId<Guid>]
        public readonly partial record struct CustomerId;

        [Email]
        public sealed partial record CustomerEmail;

        [CountryCode]
        public readonly partial record struct Country;

        [NumericPrimitive<int>, PrimitiveRange(0, 1000)]
        public readonly partial record struct LoyaltyPoints;

        [SmartEnum<int>]
        public sealed partial record CustomerTier
        {
            public static readonly CustomerTier Standard = new(1);
            public static readonly CustomerTier Gold = new(2);
        }

        [ValueObject]
        public readonly partial record struct Address
        {
            public string Street { get; init; }
            public string City { get; init; }
        }

        [Money]
        public readonly partial record struct Balance;

        [DateRange]
        public sealed partial record Membership;

        [Slug]
        public sealed partial record Handle;

        [Rating]
        public readonly partial record struct Stars;

        public sealed class Customer
        {
            public CustomerId Id { get; set; }
            public required CustomerEmail Email { get; set; }
            public Country Country { get; set; }
            public LoyaltyPoints Points { get; set; }
            public required CustomerTier Tier { get; set; }
            public Address Address { get; set; }
            public Balance Balance { get; set; }
            public Membership? Membership { get; set; }
            public Handle? Handle { get; set; }
            public Stars Rating { get; set; }
        }

        public static class Customers
        {
            public static Customer Ada(CustomerId id, Country country) =>
                new()
                {
                    Id = id,
                    Email = CustomerEmail.Create("ada@example.com"),
                    Country = country,
                    Points = LoyaltyPoints.Create(40),
                    Tier = CustomerTier.Gold,
                    Address = Address.Create("12 St James's Square", "London"),
                    Balance = Balance.Create(12.5m, "GBP"),
                    Membership = Membership.Create(new(2026, 1, 1), new(2026, 12, 31)),
                    Handle = Handle.Create("ada-lovelace"),
                    Rating = Stars.Create(4.5m),
                };
        }
        """;

    // The application prints its GraphQL schema, then the result of a query and of one whose country breaks Country's rules.
    private const string GraphQLApp = """"
        #nullable enable
        using System;
        using HotChocolate;
        using HotChocolate.Execution;
        using Microsoft.AspNetCore.Builder;
        using Microsoft.Extensions.DependencyInjection;
        using Microsoft.Extensions.Logging;
        using Throwaway;
        using Throwaway.Contracts;

        var builder = WebApplication.CreateBuilder(args);
        builder.Logging.ClearProviders();
        builder.Services.AddGraphQL().AddQueryType<Query>();
        var app = await builder.ConfigureDovetail();

        var executor = await app.Services.GetRequiredService<IRequestExecutorProvider>().GetExecutorAsync();
        Console.WriteLine(executor.Schema.ToString().Trim());
        foreach (var country in new[] { "GB", "ZZ" })
        {
            var result = await executor.ExecuteAsync(
                $$"""{ customer(id: "6f9619ff-8b86-d011-b42d-00cf4fc964ff", country: "{{country}}") { id email country points tier address { street city } balance { amount currency } membership { start end } handle rating } }"""
            );
            Console.WriteLine();
            Console.WriteLine(result.ToJson());
        }

        public sealed class Query
        {
            public Customer GetCustomer(CustomerId id, Country country) => Customers.Ada(id, country);
        }
        """";

    // The application prints its GraphQL schema, then the rows a filter on their strong ID finds: with eq, in and nin.
    private const string FilteringApp = """"
        #nullable enable
        using System;
        using System.Linq;
        using HotChocolate;
        using HotChocolate.Data;
        using HotChocolate.Execution;
        using Microsoft.AspNetCore.Builder;
        using Microsoft.Extensions.DependencyInjection;
        using Microsoft.Extensions.Logging;
        using Throwaway;
        using Throwaway.Contracts;

        var builder = WebApplication.CreateBuilder(args);
        builder.Logging.ClearProviders();
        builder.Services.AddGraphQL().AddFiltering().AddQueryType<Query>();
        var app = await builder.ConfigureDovetail();

        var executor = await app.Services.GetRequiredService<IRequestExecutorProvider>().GetExecutorAsync();
        Console.WriteLine(executor.Schema.ToString().Trim());
        foreach (var filter in new[]
        {
            $$"""{ id: { eq: "{{Query.Ada}}" } }""",
            $$"""{ id: { in: ["{{Query.Ada}}", "{{Query.Grace}}"] } }""",
            $$"""{ id: { nin: ["{{Query.Ada}}"] } }""",
        })
        {
            var result = await executor.ExecuteAsync($$"""{ rows(where: {{filter}}) { id name } }""");
            Console.WriteLine();
            Console.WriteLine(filter);
            Console.WriteLine(result.ToJson());
        }

        public sealed record Row(CustomerId Id, string Name);

        public sealed class Query
        {
            public const string Ada = "6f9619ff-8b86-d011-b42d-00cf4fc964ff";
            public const string Grace = "0198d6a4-1c2b-7000-8000-000000000001";
            public const string Linus = "0198d6a4-1c2b-7000-8000-000000000002";

            private static readonly Row[] Rows =
            [
                new(CustomerId.Create(Guid.Parse(Ada)), "Ada"),
                new(CustomerId.Create(Guid.Parse(Grace)), "Grace"),
                new(CustomerId.Create(Guid.Parse(Linus)), "Linus"),
            ];

            [UseFiltering]
            public IQueryable<Row> GetRows() => Rows.AsQueryable();
        }
        """";

    // The application serves the operations and prints the parameters and the schemas of its OpenAPI document, or, with --ef, the database
    // its context creates, the key and complex types of the entity, and an entity read back from it. The minimal API endpoints are only in
    // the document once the application started.
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
        using Throwaway;
        using Throwaway.Contracts;

        var builder = WebApplication.CreateBuilder(args);
        builder.Logging.ClearProviders();
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        builder.Services.AddDbContext<ShopContext>(options => options.UseSqlite(connection));
        // Customer is no mediator request, so no manifest lists it: the application declares its JSON metadata itself.
        Rocket.Surgery.LaunchPad.Foundation.LaunchPadJsonServiceCollectionExtensions.AddJsonTypeInfoResolver(builder.Services, AppJsonContext.Default);
        var app = await builder.ConfigureDovetail();

        if (args.Contains("--ef"))
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ShopContext>();
            Console.WriteLine(context.Database.GenerateCreateScript().Trim());
            context.Database.EnsureCreated();
            var entity = context.Model.FindEntityType(typeof(Customer))!;
            Console.WriteLine();
            Console.WriteLine($"Complex types: {string.Join(", ", entity.GetComplexProperties().OrderBy(z => z.Name, StringComparer.Ordinal).Select(z => $"{z.Name} ({z.ComplexType.ClrType.Name})"))}");
            // SQLite's column types have no length or precision, so the type mapping of each column says what the joint gave it
            var table = Microsoft.EntityFrameworkCore.Metadata.StoreObjectIdentifier.Table("Customers");
            var properties = entity.GetProperties().Concat(entity.GetComplexProperties().SelectMany(z => z.ComplexType.GetProperties()));
            foreach (var property in properties.OrderBy(z => z.GetColumnName(table), StringComparer.Ordinal))
            {
                var mapping = property.GetRelationalTypeMapping();
                Console.WriteLine($"{property.GetColumnName(table)}: {property.ClrType.Name} stored as {( mapping.Converter?.ProviderClrType ?? mapping.ClrType ).Name}, size {mapping.Size?.ToString() ?? "-"}, precision {mapping.Precision?.ToString() ?? "-"}, scale {mapping.Scale?.ToString() ?? "-"}, generated {property.ValueGenerated}{( property.GetValueGeneratorFactory() is null ? "" : " by a value generator" )}");
            }

            Console.WriteLine();
            // no ID: the key is generated when the entity is added
            var added = Customers.Ada(default, Country.Create("GB"));
            context.Customers.Add(added);
            context.SaveChanges();
            Console.WriteLine($"Key generated on insert: {added.Id.Value != Guid.Empty} (version {added.Id.Value.Version})");
            context.ChangeTracker.Clear();
            var customer = context.Customers.Single(z => z.Email == CustomerEmail.Create("ada@example.com"));
            Console.WriteLine($"Read back: {customer.Id == added.Id} {customer.Email} {customer.Country} {customer.Points} {customer.Tier.Name} {customer.Address.Street}, {customer.Address.City} {customer.Balance} {customer.Membership?.Start:O}..{customer.Membership?.End:O} {customer.Handle} {customer.Rating}");
            return;
        }

        app.MapGet("/customers/{id}", (CustomerId id, CustomerEmail email, LoyaltyPoints? points) => (Customer?)null);
        app.MapGet("/bookings/{id}", (CustomerId id) => (Booking?)null);
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

        [System.Text.Json.Serialization.JsonSerializable(typeof(Customer))]
        [System.Text.Json.Serialization.JsonSerializable(typeof(Booking))]
        internal sealed partial class AppJsonContext : System.Text.Json.Serialization.JsonSerializerContext;

        [ApiController]
        [Route("customers")]
        public sealed class CustomersController : ControllerBase
        {
            [HttpGet("{id}/tier")]
            public CustomerTier? Get(CustomerId id, [FromQuery] Country country) => null;
        }
        """;

    // The application writes domain primitives into a table through Dapper and queries them back: as the columns of a row, as a value of
    // its own and as a parameter.
    private const string DapperApp = """
        #nullable enable
        using System;
        using System.Linq;
        using Dapper;
        using Microsoft.Data.Sqlite;
        using Throwaway.Contracts;

        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        connection.Execute("create table Customers (Id text not null primary key, Email text not null, Country text not null, Points integer not null, Tier integer not null, Handle text null)");

        var id = CustomerId.Create(Guid.Parse("6f9619ff-8b86-d011-b42d-00cf4fc964ff"));
        var ada = Customers.Ada(id, Country.Create("GB"));
        connection.Execute(
            "insert into Customers (Id, Email, Country, Points, Tier, Handle) values (@Id, @Email, @Country, @Points, @Tier, @Handle)",
            new { ada.Id, ada.Email, ada.Country, ada.Points, ada.Tier, Handle = (Handle?)null }
        );
        Console.WriteLine($"Stored: {string.Join(" ", connection.QuerySingle<(string, string, string, long, long)>("select Id, Email, Country, Points, Tier from Customers"))}");

        var row = connection.QuerySingle<CustomerRow>("select * from Customers where Email = @Email", new { ada.Email });
        Console.WriteLine($"Row: {row.Id == id} {row.Email} {row.Country} {row.Points} {row.Tier.Name} {row.Handle?.ToString() ?? "(null)"}");
        Console.WriteLine($"Value: {connection.QuerySingle<CustomerTier>("select Tier from Customers").Name}");
        Console.WriteLine($"Ids: {connection.Query<CustomerId>("select Id from Customers where Points > @Points", new { Points = LoyaltyPoints.Create(10) }).Single() == id}");

        // a value that breaks the rules of its domain primitive is not read back
        connection.Execute("update Customers set Points = 5000");
        try
        {
            connection.QuerySingle<LoyaltyPoints>("select Points from Customers");
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Broken: {exception.GetType().Name}: {exception.Message}");
        }

        public sealed class CustomerRow
        {
            public CustomerId Id { get; set; }
            public CustomerEmail Email { get; set; } = null!;
            public Country Country { get; set; }
            public LoyaltyPoints Points { get; set; }
            public CustomerTier Tier { get; set; } = null!;
            public Handle? Handle { get; set; }
        }
        """;

    // The application collects the metrics of domain primitives with a reader of its own, creates and rejects a few values, and prints
    // whether the instrumentation joint was written, whether their traces are listened to and what was collected.
    private const string OpenTelemetryApp = """
        #nullable enable
        using System;
        using System.Collections.Generic;
        using System.Diagnostics;
        using System.Linq;
        using Microsoft.AspNetCore.Builder;
        using Microsoft.Extensions.DependencyInjection;
        using Microsoft.Extensions.Logging;
        using OpenTelemetry;
        using OpenTelemetry.Metrics;
        using OpenTelemetry.Trace;
        using Throwaway;
        using Throwaway.Contracts;

        var builder = WebApplication.CreateBuilder(args);
        builder.Logging.ClearProviders();
        var exporter = new CollectingExporter();
        builder.Services.ConfigureOpenTelemetryMeterProvider(metrics => metrics.AddReader(new BaseExportingMetricReader(exporter)));
        var app = await builder.ConfigureDovetail();
        var meters = app.Services.GetRequiredService<MeterProvider>();
        _ = app.Services.GetService<TracerProvider>();

        Console.WriteLine($"Joint:{typeof(CollectingExporter).Assembly.GetType("Rocket.Surgery.LaunchPad.Primitives.Generated.LaunchPadPrimitivesInstrumentationJoint") is not null}");
        using (var source = new ActivitySource("Rocket.Surgery.LaunchPad.Primitives"))
        {
            Console.WriteLine($"Traces:{source.HasListeners()}");
        }

        CustomerEmail.Create("ada@example.com");
        LoyaltyPoints.Create(40);
        LoyaltyPoints.TryCreate(5000, out _, out _);
        CustomerEmail.TryCreate("not an email", out _, out _);
        meters.ForceFlush();

        foreach (var line in exporter.Points.Order(StringComparer.Ordinal))
        {
            Console.WriteLine(line);
        }

        internal sealed class CollectingExporter : BaseExporter<Metric>
        {
            private readonly Dictionary<string, long> _points = new(StringComparer.Ordinal);

            public IEnumerable<string> Points => _points.Select(z => $"{z.Key}: {z.Value}");

            public override ExportResult Export(in Batch<Metric> batch)
            {
                foreach (var metric in batch)
                {
                    if (metric.MeterName != "Rocket.Surgery.LaunchPad.Primitives") continue;
                    foreach (ref readonly var point in metric.GetMetricPoints())
                    {
                        var tags = new List<string>();
                        foreach (var tag in point.Tags)
                        {
                            tags.Add($"{tag.Key}={tag.Value}");
                        }

                        _points[$"{metric.Name} {string.Join(" ", tags.Order(StringComparer.Ordinal))}"] = point.GetSumLong();
                    }
                }

                return ExportResult.Success;
            }
        }
        """;

    // The value objects of a booking, which the OpenAPI and EF Core application declares a Minimal API endpoint for: a range of NodaTime
    // dates, and a value object of the application's own with a domain primitive, nullable values and a nullable NodaTime value.
    private const string BookingContracts = """

        [DateRange(Kind = DatePrimitiveKind.LocalDate)]
        public sealed partial record Stay;

        [ValueObject]
        public readonly partial record struct Contact
        {
            public CustomerEmail Email { get; init; }
            public string? Phone { get; init; }
            public NodaTime.LocalDate? Since { get; init; }
            public Handle? Handle { get; init; }
        }

        public sealed record Booking(CustomerId Id, Contact Contact, Stay Stay);
        """;

    // Domain primitives of NodaTime values.
    private const string NodaTimeContracts = """

        [DatePrimitive(Kind = DatePrimitiveKind.LocalDate)]
        public sealed partial record Birthday;

        [DatePrimitive(Kind = DatePrimitiveKind.Instant)]
        public readonly partial record struct ArrivedAt;

        [DatePrimitive(Kind = DatePrimitiveKind.Duration)]
        public readonly partial record struct StayLength;

        public sealed record Visit(Birthday Birthday, ArrivedAt ArrivedAt, StayLength StayLength);
        """;

    // The application writes and reads domain primitives of NodaTime values with the JSON options LaunchPad.NodaTime configures.
    private const string NodaTimeApp = """"
        #nullable enable
        using System;
        using System.Text.Json;
        using System.Text.Json.Serialization;
        using Microsoft.AspNetCore.Builder;
        using Microsoft.Extensions.DependencyInjection;
        using Microsoft.Extensions.Logging;
        using Microsoft.Extensions.Options;
        using NodaTime;
        using Throwaway;
        using Throwaway.Contracts;

        var builder = WebApplication.CreateBuilder(args);
        builder.Logging.ClearProviders();
        builder.Services.Configure<JsonSerializerOptions>(options => options.TypeInfoResolverChain.Add(AppJsonContext.Default));
        var app = await builder.ConfigureDovetail();
        var options = app.Services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        var visit = new Visit(Birthday.Create(new LocalDate(1815, 12, 10)), ArrivedAt.Create(Instant.FromUtc(2026, 1, 2, 3, 4, 5)), StayLength.Create(Duration.FromHours(36)));
        Console.WriteLine(JsonSerializer.Serialize(visit, options));
        // LaunchPad.NodaTime's converters read more than the pattern they write: a date with its calendar, an instant with an offset, a
        // duration in days
        var read = JsonSerializer.Deserialize<Visit>("""{"birthday":"1815-12-10 (ISO)","arrivedAt":"2026-01-02T04:04:05+01:00","stayLength":"1:12:00:00"}""", options)!;
        Console.WriteLine($"Read back: {read == visit} {JsonSerializer.Serialize(read, options)}");

        [JsonSerializable(typeof(Visit))]
        internal sealed partial class AppJsonContext : JsonSerializerContext;
        """";

    // The application declares a string domain primitive without rules of its own, so the settings of the project apply to it.
    private const string SettingsApp = """
        #nullable enable
        using System;
        using Rocket.Surgery.LaunchPad.Primitives;

        Console.WriteLine($"Trimmed: [{Nickname.Create("  ada  ")}]");
        foreach (var value in new[] { "   ", "lovelace" })
        {
            try
            {
                Nickname.Create(value);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"[{value}]: {exception.GetType().Name}: {exception.Message}");
            }
        }

        Console.WriteLine($"TryCreate: {Nickname.TryCreate("lovelace", out _, out var error)} {error.Code}");

        [StringPrimitive]
        public readonly partial record struct Nickname;

        public sealed class InvalidNicknameException(string message) : Exception(message);
        """;

    // One domain primitive of each kind, as structs and classes.
    private const string AotContracts = """
        #nullable enable
        using System;
        using Rocket.Surgery.LaunchPad.Primitives;

        namespace Throwaway.Contracts;

        [StrongId<Guid>]
        public readonly partial record struct OrderId;

        [Email]
        public sealed partial record CustomerEmail;

        [NumericPrimitive<int>(Operations = NumericOperations.All), PrimitiveRange(0, 100)]
        public readonly partial record struct Points;

        [SmartEnum<int>]
        public sealed partial record OrderStatus
        {
            public static readonly OrderStatus Pending = new(1);
            public static readonly OrderStatus Shipped = new(2);
        }

        [ValueObject]
        public readonly partial record struct ShippingAddress
        {
            public string Street { get; init; }
            public string City { get; init; }
        }

        [Money]
        public readonly partial record struct Total;

        [DateRange]
        public sealed partial record Delivery;

        public sealed record Order(OrderId Id, CustomerEmail Email, Points Points, OrderStatus Status, ShippingAddress Address, Total Total, Delivery Delivery);
        """;

    // A console application that uses the domain primitives, published with native AOT.
    private const string AotApp = """
        #nullable enable
        using System;
        using System.Text.Json;
        using System.Text.Json.Serialization;
        using Rocket.Surgery.LaunchPad.Primitives;
        using Throwaway.Contracts;

        var id = OrderId.Parse("0198d6a4-1c2b-7000-8000-000000000001", null);
        Console.WriteLine($"Strong ID: {id} {OrderId.Create().Value.Version} {OrderId.TryCreate(Guid.Empty, out _, out var empty)} {empty.Code}");
        Console.WriteLine($"Email: {CustomerEmail.Create("  Ada@Example.COM ")} {CustomerEmail.TryCreate("nope", out _, out var email)} {email.Code}");
        Console.WriteLine($"Number: {(Points.Create(40) + Points.Create(2)).Value} {Points.TryCreate(101, out _, out var range)} {range.Code}");
        try
        {
            Points.Create(-1);
        }
        catch (DomainPrimitiveValidationException exception)
        {
            Console.WriteLine($"Thrown: {exception.GetType().Name}");
        }

        Console.WriteLine($"Smart enum: {OrderStatus.FromName("shipped", true).Name} {OrderStatus.Shipped.Value}");
        Console.WriteLine($"Money: {Total.Create(10m, "usd") + Total.Create(5.25m, "USD")}");
        var delivery = Delivery.Create(new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 3));
        Console.WriteLine($"Date range: {delivery} {delivery.Contains(new DateOnly(2026, 1, 2))} {Delivery.TryCreate(delivery.End, delivery.Start, out _, out _)}");

        var order = new Order(
            id,
            CustomerEmail.Create("ada@example.com"),
            Points.Create(40),
            OrderStatus.Shipped,
            ShippingAddress.Create("12 St James's Square", "London"),
            Total.Create(12.5m, "GBP"),
            delivery
        );
        var json = JsonSerializer.Serialize(order, AppJsonContext.Default.Order);
        Console.WriteLine($"JSON: {json}");
        Console.WriteLine($"Read back: {JsonSerializer.Deserialize(json, AppJsonContext.Default.Order) == order}");

        [JsonSerializable(typeof(Order))]
        internal sealed partial class AppJsonContext : JsonSerializerContext;
        """;
}
