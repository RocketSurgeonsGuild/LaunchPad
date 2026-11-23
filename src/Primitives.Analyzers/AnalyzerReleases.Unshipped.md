; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
LPAD2001 | LaunchPad.Primitives.Declaration | Error | DeclarationAnalyzer, domain primitive must be partial (DP0001)
LPAD2002 | LaunchPad.Primitives.Declaration | Error | DeclarationAnalyzer, struct domain primitive must be readonly (DP0002)
LPAD2003 | LaunchPad.Primitives.Declaration | Error | DeclarationAnalyzer, domain primitive must be a record (DP0003)
LPAD2004 | LaunchPad.Primitives.Declaration | Error | RulesAnalyzer, invalid regular expression (DP0004)
LPAD2005 | LaunchPad.Primitives.Declaration | Error | RulesAnalyzer, conflicting normalizations (DP0005)
LPAD2006 | LaunchPad.Primitives.Declaration | Error | RulesAnalyzer, invalid bounds (DP0006)
LPAD2007 | LaunchPad.Primitives.Usage | Warning | UsageAnalyzer, uninitialized struct domain primitive (DP0007)
LPAD2008 | LaunchPad.Primitives.Declaration | Error | ValueObjectAnalyzer, value object property must use init (DP0008)
LPAD2009 | LaunchPad.Primitives.Design | Warning | RulesAnalyzer, domain primitive has no rules (DP0009)
LPAD2010 | LaunchPad.Primitives.Usage | Warning | UsageAnalyzer, string compared with a domain primitive (DP0010)
LPAD2011 | LaunchPad.Primitives.Usage | Warning | UsageAnalyzer, string assigned from a domain primitive (DP0011)
LPAD2012 | LaunchPad.Primitives.Declaration | Warning | DeclarationAnalyzer, public constructor bypasses the rules (DP0012)
LPAD2013 | LaunchPad.Primitives.Design | Info | DuplicatePrimitiveAnalyzer, possible duplicate domain primitive (DP0013)
LPAD2014 | LaunchPad.Primitives.Design | Info | ApiReviewAnalyzer, API surface budget exceeded (DP0014)
LPAD2015 | LaunchPad.Primitives.Design | Info | ApiReviewAnalyzer, missing XML documentation (DP0015)
LPAD2016 | LaunchPad.Primitives.Design | Info | ApiReviewAnalyzer, factory method name (DP0016)
LPAD2017 | LaunchPad.Primitives.Declaration | Error | ExceptionTypeAnalyzer, invalid LaunchPadPrimitivesExceptionType (DP0017)
LPAD2018 | LaunchPad.Primitives.Declaration | Warning | ValueObjectAnalyzer, value object property is a mutable collection (DP0018)
LPAD2019 | LaunchPad.Primitives.Declaration | Error | DeclarationAnalyzer, class domain primitive must be sealed
LPAD2020 | LaunchPad.Primitives.Declaration | Error | RulesAnalyzer, PastOnly or FutureOnly misused
LPAD2021 | LaunchPad.Primitives.Declaration | Warning | RulesAnalyzer, NodaTime domain primitive without LaunchPad.NodaTime
LPAD2022 | LaunchPad.Primitives.Declaration | Warning | RulesAnalyzer, rule does not apply to the primitive kind
LPAD2101 | LaunchPad.Primitives | Error | GeneratorDiagnostics, the value type a domain primitive can't hold
LPAD2102 | LaunchPad.Primitives | Error | GeneratorDiagnostics, more than one primitive attribute on a type
LPAD2103 | LaunchPad.Primitives | Error | GeneratorDiagnostics, an invalid attribute argument
