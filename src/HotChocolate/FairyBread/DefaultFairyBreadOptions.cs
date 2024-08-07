﻿using HotChocolate.Types.Descriptors.Definitions;

namespace Rocket.Surgery.LaunchPad.HotChocolate.FairyBread;

public class DefaultFairyBreadOptions : IFairyBreadOptions
{
    /// <inheritdoc />
    public virtual bool ThrowIfNoValidatorsFound { get; set; } = true;

    /// <inheritdoc />
    public Func<ObjectTypeDefinition, ObjectFieldDefinition, ArgumentDefinition, bool> ShouldValidateArgument { get; set; }
        = (o, f, a) => true;
}