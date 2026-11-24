# Rocket.Surgery.LaunchPad.Primitives

- Package: `Rocket.Surgery.LaunchPad.Primitives`
- Version: `2.0.1-beta.452`
- Target framework: `net11.0`
- Source: [GitHub](https://github.com/RocketSurgeonsGuild/LaunchPad/tree/Branch.main.Sha.166abe0de80666ff26996d1f2603a1f6ba81c14c/src/Primitives)
- NuGet: [Rocket.Surgery.LaunchPad.Primitives](https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.Primitives)

## Namespaces

### Rocket.Surgery.LaunchPad.Primitives

- [DomainPrimitiveValidationException](/LaunchPad/api/rocket.surgery.launchpad.primitives/domainprimitivevalidationexception.md) -- `class` `sealed` -- Thrown when a domain primitive is created from a value its rules reject, unless `LaunchPadPrimitivesExceptionType` names another exception.
- [ICustomValidator<T>](/LaunchPad/api/rocket.surgery.launchpad.primitives/icustomvalidator-1.md) -- `interface` -- A rule of a domain primitive written as code, for `[CustomValidator<TValidator>]`. It runs after the domain primitive's other rules.
- [IDomainPrimitive<TSelf, TValue>](/LaunchPad/api/rocket.surgery.launchpad.primitives/idomainprimitive-2.md) -- `interface` -- A domain primitive that holds one value.
- [IDomainPrimitive<TSelf>](/LaunchPad/api/rocket.surgery.launchpad.primitives/idomainprimitive-1.md) -- `interface` -- A domain primitive: a type the Primitives generator completes, which only holds values its rules accept.
- [INormalizer<T>](/LaunchPad/api/rocket.surgery.launchpad.primitives/inormalizer-1.md) -- `interface` -- Normalizes a value before a domain primitive validates it, for `[Normalize<TNormalizer>]`.
- [IStrongId<TSelf, TValue>](/LaunchPad/api/rocket.surgery.launchpad.primitives/istrongid-2.md) -- `interface` -- A strong ID: a domain primitive that identifies an entity.
- [IValueObject<TSelf>](/LaunchPad/api/rocket.surgery.launchpad.primitives/ivalueobject-1.md) -- `interface` -- A value object: a domain primitive that holds several values, validated together.
- [PrimitiveCollectionExtensions](/LaunchPad/api/rocket.surgery.launchpad.primitives/primitivecollectionextensions.md) -- `class` `static` -- Creates domain primitives from collections of values.
- [PrimitiveError](/LaunchPad/api/rocket.surgery.launchpad.primitives/primitiveerror.md) -- `struct` `readonly` -- Why a domain primitive rejected a value: a code ( `LENGTH`, `FORMAT`, `RANGE`, ...) and a message.

### Rocket.Surgery.LaunchPad.Primitives.Advanced

- [PrimitiveBuilder<TPrimitive, TValue>](/LaunchPad/api/rocket.surgery.launchpad.primitives/primitivebuilder-2.md) -- `class` `sealed` -- Creates a domain primitive with rules of the caller's own on top of its rules.
- [PrimitivePatterns](/LaunchPad/api/rocket.surgery.launchpad.primitives/primitivepatterns.md) -- `class` `static` -- The patterns of the primitive kinds, which the code the generator writes for a kind matches the value against.

### Rocket.Surgery.LaunchPad.Primitives.Descriptions

- [DomainPrimitiveDescription](/LaunchPad/api/rocket.surgery.launchpad.primitives/domainprimitivedescription.md) -- `class` `abstract` -- What an integration needs to know about one domain primitive that holds one value: its value type, the format it is written in, and the constraints of its rules (lengths, patterns, bounds, allowed values). A rule written as code ( `[CustomValidator<T>]`) is not described.
- [DomainPrimitiveDescription<TPrimitive, TValue>](/LaunchPad/api/rocket.surgery.launchpad.primitives/domainprimitivedescription-2.md) -- `class` `sealed` -- The description of one domain primitive, which knows the domain primitive and its value as generic arguments.
- [IDomainPrimitiveVisitor<TResult>](/LaunchPad/api/rocket.surgery.launchpad.primitives/idomainprimitivevisitor-1.md) -- `interface` -- Does something with a domain primitive known by its generic arguments, such as building a converter for it.
- [ValueObjectDescription](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectdescription.md) -- `record` `sealed` -- What an integration needs to know about one value object: the type and the values it holds, in declaration order.
- [ValueObjectMemberDescription](/LaunchPad/api/rocket.surgery.launchpad.primitives/valueobjectmemberdescription.md) -- `record` `sealed` -- One value a value object holds.

### Rocket.Surgery.LaunchPad.Primitives.Diagnostics

- [DomainPrimitivesInstrumentation](/LaunchPad/api/rocket.surgery.launchpad.primitives/domainprimitivesinstrumentation.md) -- `class` `static` -- The metrics and trace events of domain primitives, which the generated `Create` and `TryCreate` record. Nothing is recorded until a listener subscribes: `AddLaunchPadPrimitivesInstrumentation()` on OpenTelemetry's meter and tracer provider builders, which the Primitives generator writes when the application references OpenTelemetry, or a `Metrics.MeterListener` / `Diagnostics.ActivityListener` of [DomainPrimitivesInstrumentation.Name](/LaunchPad/api/rocket.surgery.launchpad.primitives/domainprimitivesinstrumentation/fields.md#name).

### Rocket.Surgery.LaunchPad.Primitives.Registries

- [Iso3166Countries](/LaunchPad/api/rocket.surgery.launchpad.primitives/iso3166countries.md) -- `class` `static` -- The 249 officially assigned ISO 3166-1 alpha-2 codes (the list Debian iso-codes publishes from the ISO 3166 Maintenance Agency). User-assigned codes (such as XK), exceptionally reserved codes (such as UK and EU) and withdrawn codes are not in it. Update it when the agency publishes a newsletter that adds or withdraws a code.
- [Iso4217Currencies](/LaunchPad/api/rocket.surgery.launchpad.primitives/iso4217currencies.md) -- `class` `static` -- The alphabetic codes of ISO 4217 list one (current currency and funds codes), as published by SIX, the ISO 4217 maintenance agency, on 2026-09-17 (https://www.six-group.com/en/products-services/financial-information/data-standards.html). Update it from list-one.xml when the agency publishes an amendment.
