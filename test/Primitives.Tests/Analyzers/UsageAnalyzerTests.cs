using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;
using Rocket.Surgery.LaunchPad.Primitives.Analyzers.CodeFixes;

using static Primitives.Tests.Analyzers.AnalyzerTestSources;

namespace Primitives.Tests.Analyzers;

/// <summary>LPAD2007: an uninitialized struct domain primitive.</summary>
public class UninitializedStructTests
{
    private const string Primitives = """
                                      using Rocket.Surgery.LaunchPad.Primitives;

                                      namespace Sample;

                                      [StringPrimitive, NotEmpty]
                                      public readonly partial record struct UserName
                                      {
                                          public UserName(int length) { }
                                      }

                                      [StringPrimitive, NotEmpty]
                                      public sealed partial record DisplayName;

                                      public readonly record struct Plain;


                                      """;

    [Test]
    public async Task Should_Report_Every_Way_To_Make_An_Uninitialized_Struct()
    {
        var result = await Analyze<UsageAnalyzer>(
            Primitives
          + """
            public static class Uses
            {
                public static void Run(UserName parameter = default)
                {
                    var a = new UserName();
                    UserName b = new();
                    var c = default(UserName);
                    UserName d = default;
                }
            }
            """
        );

        await VerifyAnalysis(result);
    }

    [Test]
    public async Task Should_Ignore_Constructors_With_Arguments_Classes_And_Other_Types()
    {
        var result = await Analyze<UsageAnalyzer>(
            Primitives
          + """
            public static class Uses
            {
                public static void Run()
                {
                    var a = new UserName(3);
                    DisplayName? b = default;
                    UserName? c = default;
                    var d = new Plain();
                    Plain e = default;
                    int f = default;
                }
            }
            """
        );

        await VerifyAnalysis(result);
    }

    // A domain primitive of another assembly has lost its attributes: it is found by the IDomainPrimitive it implements.
    [Test]
    public async Task Should_Report_A_Domain_Primitive_Of_A_Referenced_Assembly()
    {
        var contracts = await GeneratorTestContextBuilder
                             .Create()
                             .WithProjectName("Contracts")
                             .WithLanguageVersion(Microsoft.CodeAnalysis.CSharp.LanguageVersion.Preview)
                             .AddReferences(typeof(Rocket.Surgery.LaunchPad.Primitives.IDomainPrimitive<>).Assembly)
                             .AddSources(
                                  """
                                  namespace Contracts;

                                  public readonly record struct Sku : Rocket.Surgery.LaunchPad.Primitives.IDomainPrimitive<Sku>
                                  {
                                      public static string PrimitiveName => "Sku";
                                  }
                                  """
                              )
                             .Build()
                             .GenerateAsync(CancellationToken.None);
        await contracts.FinalDiagnostics.Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error).Should().BeEmpty();

        var result = await Analyze<UsageAnalyzer>(
            """
            namespace Sample;

            public static class Uses
            {
                public static Contracts.Sku Empty() => default;
            }
            """,
            builder => builder.AddCompilationReferences(contracts)
        );

        await VerifyAnalysis(result);
    }

    // A project that can't see IDomainPrimitive<TSelf> has no domain primitive: nothing is analyzed, even a type with a primitive attribute.
    [Test]
    public async Task Should_Ignore_A_Project_That_Does_Not_Reference_Primitives()
    {
        var result = await GeneratorTestContextBuilder
                          .Create()
                          .WithLanguageVersion(Microsoft.CodeAnalysis.CSharp.LanguageVersion.Preview)
                          .AddSource("PrimitiveAttributes.g.cs", Attributes)
                          .AddSource("EmbeddedAttribute.g.cs", EmbeddedAttribute)
                          .AddSource("Input.cs", Primitives + "public static class Uses { public static UserName Empty() => default; }")
                          .WithAnalyzer<UsageAnalyzer>()
                          .Build()
                          .GenerateAsync(CancellationToken.None);

        await VerifyAnalysis(result);
    }
}

/// <summary>
///     LPAD2010 and LPAD2011: a domain primitive of a string used as a string, and their fix, whose output is compiled with the generator (which
///     writes the Value the fix reads).
/// </summary>
public class StringUsageTests
{
    private const string Primitives = """
                                      using Rocket.Surgery.LaunchPad.Primitives;

                                      namespace Sample;

                                      [StringPrimitive, NotEmpty]
                                      public readonly partial record struct UserName;

                                      [StrongId<string>, MaxLength(10)]
                                      public sealed partial record Sku;

                                      [SmartEnum<string>]
                                      public sealed partial record Color
                                      {
                                          public static readonly Color Red = new("red");
                                      }


                                      """;

    [Test]
    public async Task Should_Report_A_String_Compared_With_A_Domain_Primitive()
    {
        var result = await Fix<UsageAnalyzer, UseValueCodeFix>(
            Primitives
          + """
            public static class Uses
            {
                public static bool Left(UserName name) => "admin" == name;
                public static bool Right(UserName name) => name != "admin";
                public static bool Strong(Sku sku) => sku == "A1";
                public static bool Member(Color color) => color == "red";
            }
            """
        );

        await VerifyAnalysis(result);
        await ( await ErrorsAfterFix<UseValueCodeFix>(result, "Compare with the domain primitive's Value") ).Should().BeEmpty();
    }

    [Test]
    public async Task Should_Fix_The_Primitive_Side_Of_A_Comparison()
    {
        var result = await Fix<UsageAnalyzer, UseValueCodeFix>(
            Primitives
          + """
            public static class Uses
            {
                public static bool Right(UserName? name) => (name ?? default) != "admin";
            }
            """
        );

        await VerifyAnalysis(result);
        await ( await ErrorsAfterFix<UseValueCodeFix>(result, "Compare with the domain primitive's Value") ).Should().BeEmpty();
    }

    [Test]
    public async Task Should_Ignore_Comparisons_Without_A_String_And_A_Primitive()
    {
        var result = await Analyze<UsageAnalyzer>(
            Primitives
          + """
            public static class Uses
            {
                public static bool Same(UserName a, UserName b) => a == b;
                public static bool Strings(string a, string b) => a == b;
                public static bool Values(UserName a) => a.Value == "admin";
            }
            """
        );

        await VerifyAnalysis(result);
    }

    [Test]
    public async Task Should_Report_A_Domain_Primitive_Given_Where_A_String_Is_Expected()
    {
        var result = await Fix<UsageAnalyzer, UseValueCodeFix>(
            Primitives
          + """
            public class Uses
            {
                private readonly string _field = default(UserName);

                public void Run(UserName name, Sku sku)
                {
                    string local = name;
                    string assigned;
                    assigned = sku;
                    Take(name);
                    Take2(0, name);
                    Take2(text: name, number: 1);
                }

                private static void Take(string text) { }
                private static void Take2(int number, string text) { }
            }
            """
        );

        await VerifyAnalysis(result);
        await ( await ErrorsAfterFix<UseValueCodeFix>(result, "Use the domain primitive's Value") ).Should().BeEmpty();
    }

    [Test]
    [Arguments("string local = name;")]
    [Arguments("string assigned; assigned = name;")]
    [Arguments("Take(name);")]
    [Arguments("Take(text: name);")]
    [Arguments("Take(Get() ?? name);")]
    public async Task Should_Fix_By_Reading_The_Value(string statement)
    {
        var result = await Fix<UsageAnalyzer, UseValueCodeFix>(
            Primitives
          + $$"""
              public class Uses
              {
                  public void Run(UserName name)
                  {
                      {{statement}}
                  }

                  private static UserName? Get() => null;
                  private static void Take(string text) { }
              }
              """
        );

        await VerifyAnalysis(result);
        await ( await ErrorsAfterFix<UseValueCodeFix>(result, "Use the domain primitive's Value") ).Should().BeEmpty();
    }

    // Value is a number, a Guid, or there is none (a value object): reading it would not make the code compile, so nothing is reported.
    [Test]
    public async Task Should_Ignore_Domain_Primitives_That_Are_Not_Strings()
    {
        var result = await Fix<UsageAnalyzer, UseValueCodeFix>(
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Sample;

            [NumericPrimitive<decimal>, PrimitiveRange(0, 100)]
            public readonly partial record struct Amount;

            [StrongId<System.Guid>]
            public readonly partial record struct OrderId;

            [StrongId<int>]
            public sealed partial record Number;

            [SmartEnum<int>]
            public sealed partial record Level;

            [ValueObject]
            public sealed partial record Address
            {
                public required string Street { get; init; }
            }

            public class Uses
            {
                public void Run(Amount amount, OrderId order, Number number, Level level, Address address)
                {
                    _ = amount == "1";
                    _ = "2" != order;
                    _ = number == "3";
                    _ = level == "4";
                    _ = address == "5";
                    string a = amount;
                    string b;
                    b = order;
                    Take(number);
                    Take(text: address);
                }

                private static void Take(string text) { }
            }
            """
        );

        await VerifyAnalysis(result);
    }

    // A domain primitive of another assembly has lost its attributes: its IDomainPrimitive<TSelf, TValue> says what its value is.
    [Test]
    public async Task Should_Read_The_Value_Of_A_Domain_Primitive_Of_A_Referenced_Assembly()
    {
        var contracts = await CompileWithGenerator(
            "Contracts",
            """
            using Rocket.Surgery.LaunchPad.Primitives;

            namespace Contracts;

            [StringPrimitive, NotEmpty]
            public readonly partial record struct Sku;

            [StrongId<int>]
            public readonly partial record struct Number;

            [ValueObject]
            public sealed partial record Address
            {
                public required string Street { get; init; }
            }
            """
        );

        var result = await Analyze<UsageAnalyzer>(
            """
            namespace Sample;

            public static class Uses
            {
                public static void Run(Contracts.Sku sku, Contracts.Number number, Contracts.Address address)
                {
                    _ = sku == "A1";
                    _ = number == "1";
                    _ = address == "Main Street";
                    string a = sku;
                    string b = number;
                    string c = address;
                }
            }
            """,
            builder => builder.AddCompilationReferences(contracts)
        );

        await VerifyAnalysis(result);
    }

    [Test]
    public async Task Should_Ignore_Values_That_Are_Not_Given_As_A_String()
    {
        var result = await Analyze<UsageAnalyzer>(
            Primitives
          + """
            public class Uses
            {
                public void Run(UserName name)
                {
                    var copy = name;
                    object boxed = name;
                    UserName same = name;
                    string text = name.Value;
                    Take(name);
                    TakeMany(name.Value, name.Value);
                }

                private static void Take(UserName name) { }
                private static void TakeMany(params string[] values) { }
            }
            """
        );

        await VerifyAnalysis(result);
    }
}
