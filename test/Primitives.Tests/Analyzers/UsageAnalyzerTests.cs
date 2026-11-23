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

        await Diagnostics(result)
             .Should()
             .BeEquivalentTo(
                  ["LPAD2007: default", "LPAD2007: new UserName()", "LPAD2007: new()", "LPAD2007: default(UserName)", "LPAD2007: default"],
                  CollectionOrdering.Matching
              );
        await Messages(result, "LPAD2007")
             .Should()
             .BeEquivalentTo(["This makes an uninitialized 'UserName' that no rule has checked; use UserName.Create or UserName.TryCreate"], CollectionOrdering.Matching);
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

        await Diagnostics(result).Should().BeEmpty();
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

        await Diagnostics(result).Should().BeEquivalentTo(["LPAD2007: default"], CollectionOrdering.Matching);
    }
}

/// <summary>LPAD2010 and LPAD2011: a domain primitive used as a string, and their fix.</summary>
public class StringUsageTests
{
    private const string Primitives = """
                                      using Rocket.Surgery.LaunchPad.Primitives;

                                      namespace Sample;

                                      [StringPrimitive, NotEmpty]
                                      public readonly partial record struct UserName
                                      {
                                          public string Value => "";
                                      }

                                      [StrongId<string>, MaxLength(10)]
                                      public sealed partial record Sku
                                      {
                                          public string Value => "";
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
            }
            """
        );

        await Diagnostics(result)
             .Should()
             .BeEquivalentTo(["LPAD2010: \"admin\" == name", "LPAD2010: name != \"admin\"", "LPAD2010: sku == \"A1\""], CollectionOrdering.Matching);
        var fixedSources = await FixedAll<UseValueCodeFix>(result, "Compare with the domain primitive's Value");
        await fixedSources.Should().Contain(s => s.Contains("public static bool Left(UserName name) => \"admin\" == name.Value;"));
        await fixedSources.Should().Contain(s => s.Contains("public static bool Right(UserName name) => name.Value != \"admin\";"));
        await fixedSources.Should().Contain(s => s.Contains("public static bool Strong(Sku sku) => sku.Value == \"A1\";"));
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

        await ( await Fixed<UseValueCodeFix>(result, "Compare with the domain primitive's Value") ).Should()
                                                                                                    .Contain("=> (name ?? default).Value != \"admin\";");
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

        await Diagnostics(result).Should().BeEmpty();
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

        await Diagnostics(result)
             .Where(d => d.StartsWith("LPAD2011", StringComparison.Ordinal))
             .Should()
             .BeEquivalentTo(
                  [
                      "LPAD2011: _field = default(UserName)",
                      "LPAD2011: local = name",
                      "LPAD2011: assigned = sku",
                      "LPAD2011: name",
                      "LPAD2011: name",
                      "LPAD2011: text: name",
                  ],
                  CollectionOrdering.Matching
              );
        await Messages(result, "LPAD2011").Should().Contain("Domain primitive 'UserName' is used as a string; use its Value");
        var fixedSources = await FixedAll<UseValueCodeFix>(result, "Use the domain primitive's Value");
        await fixedSources.Should().Contain(s => s.Contains("private readonly string _field = default(UserName).Value;"));
        await fixedSources.Should().Contain(s => s.Contains("Take2(0, name.Value);"));
    }

    [Test]
    [Arguments("string local = name;", "string local = name.Value;")]
    [Arguments("string assigned; assigned = name;", "string assigned; assigned = name.Value;")]
    [Arguments("Take(name);", "Take(name.Value);")]
    [Arguments("Take(text: name);", "Take(text: name.Value);")]
    [Arguments("Take(Get() ?? name);", "Take((Get() ?? name).Value);")]
    public async Task Should_Fix_By_Reading_The_Value(string statement, string fixedStatement)
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

        await ( await Fixed<UseValueCodeFix>(result, "Use the domain primitive's Value") ).Should().Contain(fixedStatement);
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

        await Diagnostics(result).Should().BeEmpty();
    }
}
